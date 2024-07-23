 
  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uSpeedUpDowngrade')
	BEGIN
		DROP  Procedure  uSpeedUpDowngrade
	END

GO

--
-- If successful, this returns the amount of credits used up for this action in the OUTPUT param @Cost
--  if not succesful, @Cost is zero or null
--
CREATE Procedure uSpeedUpDowngrade
		@EventID bigint
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
	IF @DEBUG = 1 SELECT 'BEGIN uSpeedUpDowngrade ' + cast(@EventID as varchar(10))
	
	declare @VillageID int
	declare @EventTime Datetime
	declare @NewEventTime Datetime
	declare @now datetime
	declare @PackageID int
	declare @Result int
	declare @buildingID int
	declare @success_subtractCredits bit
	
	__RETRY__:
	begin try 
		set @success_subtractCredits = 0 --default val
		--
		-- figure out what PF we are using. we note the PackageID, not PF id 
		--
        SET @PackageID = 29
		
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
			FROM BuildingDowngrades
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
            
			UPDATE Events SET EventTime = @now WHERE EventTIme = @EventTime
                           
			IF @@rowcount = 1 BEGIN	
				set @success_subtractCredits = 1							
       		    -- log the process of extending new package
			    insert into PlayerPFLog
    			    (PlayerID,Time ,EventType,Credits ,Cost,notes)
			        values
	    		    (@PlayerID,getdate(),3,@Cost,-1
	    		        , Cast(@PackageID as varchar(max)) + ' - EID=' + Cast(@EventID as varchar(max)) + ', BID=' + Cast(@buildingID as varchar(max)) + ', VID=' + Cast(@VillageID as varchar(max)) + ', Completion-time-before-speedup=' + Cast(@EventTime as varchar(max)) 
	    		    )
	    		    
              
            END			
		END 
		
		commit tran
		
		--
		-- subtract the credits, make sure this worked
		--
		IF @success_subtractCredits = 1 BEGIN	    	    	
			EXEC FBGC.FBGCommon.dbo.uCredits_Subtract2 @playerID, @Cost, @Result output
			IF @Result <> 0 BEGIN 
				-- credits not deducted. this means player got this for free. we ignore this.
        		IF @DEBUG = 1 SELECT 'credits not deducted. this means player got the speed up for free. we ignore this.'   
				INSERT INTO ErrorLog VALUES (getdate(), 99, 'no-credits', 'downgrade')		 
			END
		END

        --
        --  since we cut the downgrade time to NOW, lets complete it. 
        --      checking if the event is not processed just in case. 
        --
        IF EXISTS (select * from Events where Status = 0 AND EventID = @EventID) BEGIN
            EXEC iCompleteBuildingDowngrade @EventID
        END 
		
		IF @DEBUG = 1 SELECT 'END uSpeedUpDowngrade ' + cast(@EventID as varchar(10))
		
	end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uSpeedUpDowngrade FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @VillageID'		+ ISNULL(CAST(@VillageID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @now'				+ ISNULL(CAST(@now AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PackageID'		+ ISNULL(CAST(@PackageID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @buildingID'		+ ISNULL(CAST(@buildingID AS VARCHAR(20)), 'Null') + CHAR(10)
		
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