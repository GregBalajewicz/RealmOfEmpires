 
  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uSpeedUpUpgradeFree')
	BEGIN
		DROP  Procedure  uSpeedUpUpgradeFree
	END

GO

--
-- If successful, this returns the amount of credits used up for this action in the OUTPUT param @Cost
--  if not succesful, @Cost is zero or null
--
CREATE Procedure uSpeedUpUpgradeFree
		@EventID bigint
		, @PlayerID int 
        , @Success BIT OUTPUT
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
	IF @DEBUG = 1 SELECT 'BEGIN uSpeedUpUpgradeFree ' + cast(@EventID as varchar(10))
	
	declare @VillageID int
	declare @EventTime Datetime
	declare @now datetime
	declare @buildingID int
	declare @Level int
	
	__RETRY__:
	begin try 
		
		SET @Success = 0

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

    		SET @now = getdate()
			IF @EventTime > @now BEGIN  
            
				--set event to now, aka finish the upgrade
				UPDATE Events SET EventTime = @now where eventid = @EventID
                SET @Success = 1          		
			END 
		
		commit tran

		
        --
        -- if building is considered complete but the event has not been processed yet, then do so now
        --  so that the player does not get 'overdue'
        --  
        -- We do this outside of the transaction on purpose since the 2 are really 2 different transactions. 
        --  This call is helpful, that's all
        --
        IF EXISTS (select * from Events where EventTime <= @now AND Status = 0 AND EventID = @EventID) BEGIN
		    IF @DEBUG = 1 SELECT 'upgrade is complete. Complete it now.'
            EXEC iCompleteBuildingUpgrade @EventID, @VillageID, @buildingID, 0
			SET @Success = 1   
        END 
				
		IF @DEBUG = 1 SELECT 'END uSpeedUpUpgradeFree ' + cast(@EventID as varchar(10))
		
	end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uSpeedUpUpgradeFree FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @VillageID'		+ ISNULL(CAST(@VillageID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @now'				+ ISNULL(CAST(@now AS VARCHAR(20)), 'Null') + CHAR(10)
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