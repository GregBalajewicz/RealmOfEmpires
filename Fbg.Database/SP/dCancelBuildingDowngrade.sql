﻿ IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dCancelBuildingDowngrade')
	BEGIN
		DROP  Procedure  dCancelBuildingDowngrade
	END

GO

CREATE Procedure dCancelBuildingDowngrade
		@EventOrQEntryID bigint
		, @IsQ bit -- true, means @EventOrQEntryID  is QEntryID, false means @EventOrQEntryID  is EventID
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
	IF @DEBUG = 1 SELECT 'BEGIN dCancelBuildingDowngrade ' + cast(@EventOrQEntryID as varchar(10))
	
	declare @BuildingTypeID INT
	declare @BuildingLevel INT
	declare @BuildingLevelInQ INT
	declare @cost int
	declare @VillageID int
	declare @CoinsOverflow int
	declare @now datetime
	
	
	__RETRY__:
	begin try 
		begin tran
		--
		-- get the villageid involved so that we can lock it 
		--
		IF @IsQ = 0 BEGIN
			select @VillageID = VillageID
				FROM BuildingDowngrades
				WHERE EventID = @EventOrQEntryID
		END ELSE BEGIN			
			select @VillageID = VillageID
				FROM BuildingDowngradeQEntries
				WHERE QEntryID = @EventOrQEntryID
		END	
		IF 	@VillageID is null BEGIN 
		    -- No VID found, the event id must be invalid, return quietly
		    ROLLBACK 
		    RETURN;
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

		
		IF @IsQ = 0 BEGIN
			--
			-- ----------------------------------------------
			-- cancel event downgrade -------------------------
			-- ----------------------------------------------
			--
			
			--
			-- First we set the event as completed so that no one, in the mean time, can cancel it; or process it 
			--
			UPDATE Events SET [Status] =1 WHERE EventID = @EventOrQEntryID AND [Status] = 0

			-- IF no rows where updated, then the event must have been cancelled (or something like this) therefore we do nothing
			IF @@rowcount = 1 BEGIN								
				--
				-- Since we just removed an downgrade Event, try to grab another downgrade from the Q (if any)
				--	
				set @now = getdate()
				exec iBuildingDowngrade_GetOneFromQ @VillageID, @now, @PrintDebugInfo
							
			END 
		END ELSE BEGIN
			--
			-- ------------------------------------------
			-- cancel Q downgrade -------------------------
			-- ------------------------------------------
			--

			--
			-- Delete the downgrade from the Q
			--
			DELETE  BuildingDowngradeQEntries WHERE QEntryID = @EventOrQEntryID
			IF @@ROWCOUNT <> 1 BEGIN 
				IF @DEBUG = 1 SELECT 'DELETE  BuildingdowngradeQEntries failed...' 

				-- 
				-- IF we are here, then the delete did not occur meaning that the QEntryID @EventOrQEntryID is no longer valid.
				--	This should never happen and is probably a bug somewhere as all SPs should obtain a village lock like we do here... 
				--	however, we exit gracefully to avoid exceptions 
			END 
		END
		
		
		commit tran
		
		IF @DEBUG = 1 SELECT 'END dCancelBuildingDowngrade ' + cast(@EventOrQEntryID as varchar(10))
		
	end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'dCancelBuildingDowngrade FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @BuildingTypeID'	+ ISNULL(CAST(@BuildingTypeID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @BuildingLevel'	+ ISNULL(CAST(@BuildingLevel AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @EventOrQEntryID' + ISNULL(CAST(@EventOrQEntryID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @IsQ'				+ ISNULL(CAST(@IsQ AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @BuildingLevelInQ'+ ISNULL(CAST(@BuildingLevelInQ AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @cost'			+ ISNULL(CAST(@cost AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @VillageID'		+ ISNULL(CAST(@VillageID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @CoinsOverflow'	+ ISNULL(CAST(@CoinsOverflow AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @now'				+ ISNULL(CAST(@now AS VARCHAR(20)), 'Null') + CHAR(10)
		
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

--
-- say that the village has changed. this is done deliberately outside of the main tran and try 
--
declare @PlayerID int 
select @PlayerID = OwnerPlayerID from villages where VillageID = @VillageID
UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @PlayerID and VillageID = @VillageID and CachedItemID = 0
IF (@@rowcount < 1 ) BEGIN
	INSERT INTO VillageCacheTimeStamps values (@PlayerID, @VillageID, 0, getdate())
END

GO