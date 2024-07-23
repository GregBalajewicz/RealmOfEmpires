 
  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uSpeedUpUpgrade')
	BEGIN
		DROP  Procedure  uSpeedUpUpgrade
	END

GO

--
-- If successful, this returns the amount of credits used up for this action in the OUTPUT param @Cost
--  if not succesful, @Cost is zero or null
--
CREATE Procedure uSpeedUpUpgrade
		@EventID bigint
		, @MinToCut int
		, @PlayerID int 
        , @Cost int OUTPUT
		, @PrintDebugInfo BIT = null

AS
	DECLARE @DEBUG INT
	IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
		SET @DEBUG = 1
		SET NOCOUNT OFF
	END ELSE BEGIN
		SET @DEBUG = 0
		SET NOCOUNT ON
	END 
	IF @DEBUG = 1 SELECT 'BEGIN uSpeedUpUpgrade ' + cast(@EventID as varchar(10))
	
	declare @VillageID int
	declare @EventTime Datetime
	declare @NewEventTime Datetime
	declare @now datetime
	declare @PackageID int
	declare @Result int
	declare @buildingID int
	declare @Level int
	declare @ActualMinutesCut int
	declare @success_subtractCredits int
	
	__RETRY__:
	begin try 
		
		--		
		--	doing the following outside of the transaction to avoid distributed transactions
		--
		set @success_subtractCredits = 0 -- default
		--
		-- figure out what PF we are using. we note the PackageID, not PF id 
		--
		IF @MinToCut = 1 BEGIN 
		    SET @PackageID = 25
		END ELSE IF @MinToCut = 15 BEGIN 
		    SET @PackageID = 26
		END ELSE IF @MinToCut = 60 BEGIN 
		    SET @PackageID = 27
		END ELSE IF @MinToCut = 240 BEGIN 
		    SET @PackageID = 28		
		END ELSE BEGIN
		    RAISERROR('unrecognized value for @MinToCut',11,1)	
		END
		
		SELECT @Cost = Cost FROM PFPackages where PFPackageID = @PackageID
		IF @Cost is null BEGIN
		    RAISERROR('No cost found',11,1)			
		END

		declare @HasEnoughCredits int 
		exec @HasEnoughCredits = FBGC.FBGCommon.dbo.qDoesPlayerHaveEnoughCredits @PlayerID, @Cost
		IF @HasEnoughCredits != 1 BEGIN 
            --
            -- not enough credits. abort. 
            --
        	IF @DEBUG = 1 SELECT 'insufficient funds. Abort'        
            SET @Cost = 0
            RETURN 
        END


		begin tran
			--
			-- get the villageid involved so that we can lock it 
			--
			select @VillageID = VillageID
				, @buildingID = BuildingTypeID
				, @Level = Level 
				FROM BuildingUpgrades
				WHERE EventID = @EventID and VillageID in (select villageID from villages where ownerPlayerID  = @PlayerID)
			

			select @EventTime = EventTime
				FROM Events
				WHERE EventID = @EventID AND status = 0
			
    		IF @VillageID IS NULL OR @EventTime is null BEGIN 
				IF @DEBUG = 1 SELECT '@VillageID IS NULL OR @EventTime is null. Exit quietly'
        		IF @@TRANCOUNT > 0 ROLLBACK
        		RETURN 
			END

							
			--
			-- Obtain a village LOCK
			--		this will ensure that no one else will change the buildings in the village, or effect the building Q 
			--
			update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @VillageID
			IF @@rowcount <> 1 BEGIN	
				IF @DEBUG = 1 SELECT 'No village semaphore taken!'
				RAISERROR('No village semaphore taken!',11,1)	
			END							

    		set @now = getdate()
			IF @EventTime > @now BEGIN  
				--
				-- figure out the completion time after speed up. 
				--  make sure it is not earlier than NOW
				--      
				SET @NewEventTime = dateadd(minute, -(@MinToCut), @EventTime)
				IF @NewEventTime < @now BEGIN
					SET @NewEventTime = @now 
				END

				--
				-- get the actual amount of time cut
				--
				SET @ActualMinutesCut = DATEDIFF(minute, @NewEventTime,@EventTime)		
            
				UPDATE Events SET EventTime = @NewEventTime WHERE EventTIme = @EventTime
                           
				IF @@rowcount = 1 BEGIN								
       				-- log the process of extending new package
					insert into PlayerPFLog
    					(PlayerID,Time ,EventType,Credits ,Cost,notes)
						values
	    				(@PlayerID,getdate(),3,@Cost,-1
	    					, Cast(@PackageID as varchar(max)) + ' - EID=' + Cast(@EventID as varchar(max)) + ', BID=' + Cast(@buildingID as varchar(max)) + ', VID=' + Cast(@VillageID as varchar(max)) + ', UpgradingLevel=' + Cast(@Level as varchar(max)) + ', Completion-time-before-speedup=' + Cast(@EventTime as varchar(max)) + ', Completion-time-After-speedup=' + Cast(@NewEventTime as varchar(max))
	    				)
	    		    
					set @success_subtractCredits = 1

					--
					-- log the speed up for the purpose of limiting speed ups per certain time period
					INSERT INTO VillageSpeedUpUsage values (@PlayerID, @VillageID, @now, @ActualMinutesCut ) 
				END			
			END 
		
		commit tran

		IF @success_subtractCredits = 1 BEGIN
			--
			-- subtract the credits
			--	doing the following outside of the transaction to avoid distributed transactions
			--	    	    	
			EXEC FBGC.FBGCommon.dbo.uCredits_Subtract2 @playerID, @Cost, @Result output
			IF @Result <> 0 BEGIN 
				-- credits not deducted. this means player got the speed up for free. we ignore this.
        		IF @DEBUG = 1 SELECT 'credits not deducted. this means player got the speed up for free. we ignore this.'   
				INSERT INTO ErrorLog VALUES (getdate(), 99, 'no-credits', 'speedup')		
			END
		END 
                


		
        --
        -- if building is consdered complete but the event has not been processed yet, then do so now
        --  so that the player does not get 'overdue'
        --  
        -- We do this outside of the transaction on purpose since the 2 are really 2 different transactions. 
        --  This call is helpful, that's all
        --
        IF EXISTS (select * from Events where EventTime <= @now AND Status = 0 AND EventID = @EventID) BEGIN
		    IF @DEBUG = 1 SELECT 'upgrade is complete. Complete it now.'
            EXEC iCompleteBuildingUpgrade @EventID, @VillageID, @buildingID, 0
        END 
				
		IF @DEBUG = 1 SELECT 'END uSpeedUpUpgrade ' + cast(@EventID as varchar(10))
		
	end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uSpeedUpUpgrade FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @VillageID'		+ ISNULL(CAST(@VillageID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @now'				+ ISNULL(CAST(@now AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @MinToCut'	    + ISNULL(CAST(@MinToCut AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PackageID'		+ ISNULL(CAST(@PackageID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @buildingID'		+ ISNULL(CAST(@buildingID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @Level'		    + ISNULL(CAST(@Level AS VARCHAR(20)), 'Null') + CHAR(10)
		
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
		
	--
	-- IF deadlock, rerun
	--
	IF ERROR_NUMBER() = 1205 -- deadlock occured in this SP 
		OR (ERROR_NUMBER() = 50000 AND left(ERROR_MESSAGE(), 8) = 'DEADLOCK' ) -- deadlock occured in some called SP 
	BEGIN
		INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock', @ERROR_MSG)		
		WAITFOR DELAY '00:00:05'
		GOTO __RETRY__
	END 		
		
	RAISERROR(@ERROR_MSG,11,1)	

end catch	


GO