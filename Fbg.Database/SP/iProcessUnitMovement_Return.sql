IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iProcessUnitMovement_Return')
	BEGIN
		DROP  Procedure  iProcessUnitMovement_Return
	END

GO

CREATE Procedure iProcessUnitMovement_Return
	@EventID as int
	, @PrintDebugInfo BIT = null
AS

declare @AttackingVillageID int
declare @Loot int
declare @TrasuryOverflow int

DECLARE @DEBUG INT, @DBG varchar(10)
IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
	SET @DEBUG = 1
	set @DBG = ':::::::'
	SET NOCOUNT OFF
END ELSE BEGIN
	SET @DEBUG = 0
	SET NOCOUNT ON
END 
IF @DEBUG = 1 print @DBG + 'BEGIN iProcessUnitMovement_Return ' + cast(@EventID as varchar(10))
__RETRY__:

begin try 
	begin tran
		--
		-- get some details of the event we are processing
		--
		select @AttackingVillageID = UM.DestinationVillageID
			, @Loot = UM.Loot
		from UnitMovements UM
		where UM.EventID = @EventID
			and CommandType = 2 -- 2 == RETURN 
		IF @AttackingVillageID  is null BEGIN
			IF @DEBUG = 1 print @DBG + '@AttackingVillageID  is null. Exit quietly' 
			IF @@TRANCOUNT > 0 ROLLBACK
			RETURN 
		END 
		IF @DEBUG = 1 print '@AttackingVillageID=' + cast(@AttackingVillageID  as varchar)
	
		--	
		-- Obtain locks on target & attacking villages
		--
		update VillageSemaphore set TimeStamp = getdate() 
			where VillageID = @AttackingVillageID
	
		--
		-- Set the event as completed so that no one, in the mean time, can cancel it
		--
		UPDATE Events SET [Status] =1 WHERE EventID = @EventID AND [Status] = 0
		IF @@rowcount <> 1 BEGIN
			-- IF no rows where updated, then the event must have been cancelled (or something like this) thereforecs we abort quietly
			IF @DEBUG = 1 SELECT 'Event is no longer valid. Exit quietly' 			
			INSERT INTO ErrorLog VALUES (getdate(), 0, 'iProcessUnitMovement_Return, UPDATE Events SET [Status] =1 resulted in no rows', @EventID)		
			GOTO DONE
		END 
	
		--
		-- update the village with the returning troops
		--
		IF @DEBUG = 1 select * from UnitsMoving where EventID = @EventID
		IF @DEBUG = 1 select * from VillageUnits where VillageID = @AttackingVillageID			
			
		update VillageUnits set  CurrentCount =CurrentCount + UMing.UnitCount 
		from UnitsMoving UMing 
			where  UMing.EventID = @EventID
			and UMing.UnitTypeID = VillageUnits.UnitTypeID
			and VillageID = @AttackingVillageID

		IF @DEBUG = 1 select * from VillageUnits where VillageID = @AttackingVillageID

		--
		-- Update the village with loot carried (if any) 
		--
		IF @DEBUG = 1 print '@loot=' + cast(@loot  as varchar)
		IF @loot <> 0 BEGIN 
			exec uVillageCoins_Add @AttackingVillageID, @loot, @TrasuryOverflow out
			IF @DEBUG = 1 print '@TrasuryOverflow=' + cast(@TrasuryOverflow  as varchar)
		END
		
		
		--
		-- update cache time stamps
		--
		-- incoming cache item (1) 
		UPDATE PlayerCacheTimeStamps SET TimeStamp = getdate() where PlayerID = (select ownerplayerid from villages where villageid = @AttackingVillageID) and CachedItemID = 1  
		IF (@@rowcount < 1 ) BEGIN
			INSERT INTO PlayerCacheTimeStamps select ownerplayerid, 1, getdate() from villages where villageid = @AttackingVillageID
		END


		IF @DEBUG = 1 print @DBG + 'END iProcessUnitMovement_Return ' + cast(@EventID as varchar(10))
		DONE: 
	commit tran
end try
begin catch
    IF @@TRANCOUNT > 0 ROLLBACK
	DECLARE @ERROR_MSG AS VARCHAR(8000)

	SET @ERROR_MSG = 'iProcessUnitMovement_Return FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @EventID' + ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @AttackingVillageID' + ISNULL(CAST(@AttackingVillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @Loot' + ISNULL(CAST(@Loot AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @TrasuryOverflow' + ISNULL(CAST(@TrasuryOverflow AS VARCHAR(10)), 'Null') + CHAR(10)
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
select @PlayerID = OwnerPlayerID from villages where VillageID = @AttackingVillageID
UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @PlayerID and VillageID = @AttackingVillageID and CachedItemID = 0
IF (@@rowcount < 1 ) BEGIN
	INSERT INTO VillageCacheTimeStamps values (@PlayerID, @AttackingVillageID, 0, getdate())
END


GO
