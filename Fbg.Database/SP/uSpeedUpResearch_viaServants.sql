 
  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uSpeedUpResearch_viaServants')
	BEGIN
		DROP  Procedure  uSpeedUpResearch_viaServants
	END

GO

--
-- If successful, this returns @ResearchItemID that was speed up in @success
--  if not succesful, @Cost is zero or null
--
CREATE Procedure uSpeedUpResearch_viaServants
		@PlayerID int 
        , @success int OUTPUT
        , @EventTimeBeforeSpeedUp DateTime OUTPUT -- only valid if @success != 0 
		, @Cost int OUTPUT -- only valid if @success != 0 
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
	IF @DEBUG = 1 SELECT 'BEGIN uSpeedUpResearch_viaServants ' + cast(@PlayerID as varchar(10))
	
	declare @ResearchItemID int
	declare @EventTime Datetime
	declare @NewEventTime Datetime
	declare @now datetime
	declare @Result int
	declare @EventID bigint
	declare @MinToCut int
	declare @HoursBehind real
	declare @MinLeftToResearch int

	
	SET @success = 0 --default value; will be set to 1 if successful

	__RETRY__:
	begin try 	
		begin tran	
			select top 1 
				@EventID = e.eventid,
				@ResearchItemID=  ResearchItemID,
				@EventTime = EventTime
				from ResearchInProgress RIP join Events E 
				on RIP.EventID = E.EventID 
				where Status = 0 and playerid = @PlayerID
	
			
    		IF @EventID IS NULL BEGIN 
				IF @DEBUG = 1 SELECT '@VillageID IS NULL OR @EventTime is null. Exit quietly'
        		IF @@TRANCOUNT > 0 ROLLBACK
        		RETURN 
			END
								

    		set @now = getdate()
			IF @EventTime > @now BEGIN  
				--
				-- figure out how much we can cut
				--
				EXEC qPlayersResearch_HourseBehind @PlayerID,  @HoursBehind output, @PrintDebugInfo
				SET @MinLeftToResearch = ceiling(DATEDIFF(second, @now,  @EventTime)/60.0)

				SET @MinToCut = dbo.fbGetMinReal(@HoursBehind*60, @MinLeftToResearch)
				IF @DEBUG = 1 SELECT @MinToCut as '@MinToCut', @MinLeftToResearch as '@MinLeftToResearch', @HoursBehind as '@HoursBehind'

				-- sanity check					
    			IF @MinToCut < 1 OR @MinToCut is null BEGIN 
					IF @DEBUG = 1 SELECT '@MinToCut < 1 OR @MinToCut is null. Exit quietly'
        			IF @@TRANCOUNT > 0 ROLLBACK
        			RETURN 
				END
				--
				-- COST & PAY
				--
				SET @Cost =  CEILING( @MinToCut / 30.0)

				declare @HasEnoughCredits int 
				exec @HasEnoughCredits = FBGC.FBGCommon.dbo.qDoesPlayerHaveEnoughCredits @PlayerID, @Cost
				IF @HasEnoughCredits != 1 BEGIN 
					--
					-- not enough credits. abort. 
					--
        			IF @DEBUG = 1 SELECT 'insufficient funds. Abort'   
					IF @@TRANCOUNT > 0 ROLLBACK     
					SET @Cost = 0
					RETURN 
				END

				--
				-- figure out the completion time after speed up. 
				--  make sure it is not earlier than NOW
				--      
				SET @NewEventTime = dateadd(minute, -(@MinToCut), @EventTime)
				IF @NewEventTime < @now BEGIN
					SET @NewEventTime = @now 
				END
            
				UPDATE Events SET EventTime = @NewEventTime WHERE EventTIme = @EventTime
                           
				IF @@rowcount = 1 BEGIN								
       				SET @success = @ResearchItemID
					set @EventTimeBeforeSpeedUp = @EventTime

       				-- log the process of spending servants
					insert into PlayerPFLog
    					(PlayerID,Time ,EventType,Credits ,Cost,notes)
						values
	    				(@PlayerID,getdate(),3,@Cost,-1
	    					, 'ResearchItemID' + Cast(@ResearchItemID as varchar(max)) + ', @MinToCut=' + Cast(@MinToCut as varchar(max))
	    				)	    		    
				END			

			END 
		
		commit tran	

		IF @success > 0  BEGIN
			--
			-- subtract the credits
			--	doing the following outside of the transaction to avoid distributed transactions
			--	    	    				
			declare @RealmID int
			select @RealmID = AttribValue from RealmAttributes where AttribID = 33
			EXEC FBGC.FBGCommon.dbo.uCredits_Subtract3_ByPID @playerID, @Cost, 36, @RealmID, @Result output
			IF @Result <> 0 BEGIN 
				-- credits not deducted. this means player got the speed up for free. we ignore this.
        		IF @DEBUG = 1 SELECT 'credits not deducted. this means player got the speed up for free. we ignore this.'   
				INSERT INTO ErrorLog VALUES (getdate(), 99, 'no-credits', 'speedup research - catchup')		
			END
		END 


		--
        -- if reseaerch is consdered complete but the event has not been processed yet, then do so now
        --  so that the player does not get 'overdue'
        --  
        -- We do this outside of the transaction on purpose since the 2 are really 2 different transactions. 
        --  This call is helpful, that's all
        --
        IF EXISTS (select * from Events where EventTime <= @now AND Status = 0 AND EventID = @EventID) BEGIN
		    IF @DEBUG = 1 SELECT 'upgrade is complete. Complete it now.'
            EXEC iCompleteResearch @EventID
        END 
				
		
		IF @DEBUG = 1 SELECT 'END uSpeedUpResearch_viaServants ' + cast(@EventID as varchar(10))
		
	end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uSpeedUpResearch_viaServants FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @ResearchItemID'		+ ISNULL(CAST(@ResearchItemID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @now'				+ ISNULL(CAST(@now AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @MinToCut'	    + ISNULL(CAST(@MinToCut AS VARCHAR(20)), 'Null') + CHAR(10)
		
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