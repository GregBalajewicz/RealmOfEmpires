  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iResearchInProgress')
	BEGIN
		DROP  Procedure  iResearchInProgress
	END

GO
CREATE Procedure iResearchInProgress
	@PlayerID int
	,@VillageID int
	,@ResearchItemTypeID int
	,@ResearchItemID int
	,@NumConcurentItemsAllowed smallint
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
	IF @DEBUG = 1 SELECT 'BEGIN iResearchInProgress' 

	
	declare @Cost int
	declare @Now DateTime
	declare @CoinSubtractSuccess bit
	declare @NumOfUpgradesInQ int
	declare @PopForThisUpgrade int
	declare @BaseUpgradeTime bigint
	declare @EventID int
	declare @CompletedOn as DateTime
	declare @NumOfResearchItemsCurrentlyGoing int

__RETRY__:
	begin try  
	
	
	
	begin tran	
		--
		-- Obtain a village LOCK
		--		this will ensure that no one else will change the buildings in the village, or effect the building Q 
		--
		IF @DEBUG = 1 SELECT 'Obtain lock ' 
		update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @VillageID
		IF @DEBUG = 1 SELECT 'Obtain lock - success' 

        --
        -- check if player is already researching this
        --
        IF exists(SELECT * from Events E join ResearchInProgress RIP on RIP.EventId = E.EventID WHERE RIP.ResearchItemID = @ResearchItemID and RIP.PlayerID = @PlayerID and Status = 0 ) BEGIN
			IF @@TRANCOUNT > 0 ROLLBACK
			IF @DEBUG = 1 SELECT ' Already researching this'
			return 	        
        END 
        --
        -- check if player already has this researched
        --
        IF exists(SELECT * from playerresearchitems WHERE ResearchItemID = @ResearchItemID and PlayerID = @PlayerID) BEGIN
			IF @@TRANCOUNT > 0 ROLLBACK
			IF @DEBUG = 1 SELECT ' Already researched'
			return 	        
        END 

        --
        -- check the number of items in the Q and only proceed if # of items is less then the max
        --
        select @NumOfResearchItemsCurrentlyGoing = count(*) from Events E 
            join ResearchInProgress RP on E.EventID = RP.EventID  
            WHERE PlayerID = @PlayerID AND Status =0
        SET @NumOfResearchItemsCurrentlyGoing = isnull(@NumOfResearchItemsCurrentlyGoing, 0)
        IF @NumOfResearchItemsCurrentlyGoing >= @NumConcurentItemsAllowed BEGIN 
			IF @@TRANCOUNT > 0 ROLLBACK
			IF @DEBUG = 1 SELECT ' @NumOfResearchItemsCurrentlyGoing >= @NumConcurentItemsAllowed. Exit quietly...'
			return 	
        END 
		--
		-- Get the cost of this upgrade
		--
		select @cost = PriceInCoins, @BaseUpgradeTime = ResearchTime from ResearchItems where ResearchItemTypeID= @ResearchItemTypeID 
		    and ResearchItemID = @ResearchItemID

		exec uVillageCoins_Subtract @VillageID ,@cost, @CoinSubtractSuccess OUT, 0
		
		--
		-- verify the cost does not exceeed assets.
		--
		IF @CoinSubtractSuccess = 0 BEGIN
			IF @@TRANCOUNT > 0 ROLLBACK
			---INSERT INTO ErrorLog VALUES (getdate(), 0, '@CoinSubtractSuccess = 0. Exit quietly...',  ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null'))		
			IF @DEBUG = 1 SELECT ' @CoinSubtractSuccess = 0. Exit quietly...'
			return 	
		END
		
		declare @BaseUpgradeTimeMS bigint 
		set @BaseUpgradeTimeMS = @BaseUpgradeTime/ 10000
		if @BaseUpgradeTimeMS  > 2147483647 BEGIN 
			-- this kicks in when research items get ridiculously long
			set @CompletedOn = dateadd(second, @BaseUpgradeTimeMS / 1000, GetDate())
		END ELSE BEGIN
			set @CompletedOn = dateadd(millisecond, @BaseUpgradeTimeMS , GetDate())
		END 

    	-- Create the event to do this upgrade
		--			
		insert into Events (EventTime, Status) values(@CompletedOn, 0)
		set @EventID = SCOPE_IDENTITY() 
		insert into ResearchInProgress (PlayerID, EventID, ResearchItemTypeID, ResearchItemID) 
			 values (@PlayerID, @EventID, @ResearchItemTypeID, @ResearchItemID)
		
	    update players set ResearchUpdated = 0 where PlayerID = @PlayerID
		
	commit  
	
	IF @DEBUG = 1 SELECT 'END iResearchInProgress'
	end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iResearchInProgress FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @VillageID'			+ ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @ResearchItemTypeID'			+ ISNULL(CAST(@ResearchItemTypeID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @ResearchItemID'				+ ISNULL(CAST(@ResearchItemID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @ResearchItemTypeID'+ ISNULL(CAST(@ResearchItemTypeID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CoinSubtractSuccess'	+ ISNULL(CAST(@CoinSubtractSuccess AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @Cost'				+ ISNULL(CAST(@Cost AS VARCHAR(10)), 'Null') + CHAR(10)
		
		+ '   ERROR_NUMBER():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'	+ ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'	+ ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'		+ ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'		+  ERROR_MESSAGE() + CHAR(10)
		
	
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
