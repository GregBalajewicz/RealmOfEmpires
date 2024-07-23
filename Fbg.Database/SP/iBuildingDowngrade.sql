 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iBuildingDowngrade')
	BEGIN
		DROP  Procedure  iBuildingDowngrade
	END

GO
CREATE Procedure iBuildingDowngrade
	@VillageID int
	,@BuildingID int
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
	IF @DEBUG = 1 SELECT 'BEGIN iBuildingDowngrade' + cast(@VillageID as varchar(10)) + cast(@BuildingID as varchar(10))

	declare @Now DateTime
	declare @CurrentBuildingLevel int
	declare @NumOfCurrentlyUpgrading int 
	declare @NumOfEntriesInUpgradeQ int 
	declare @NumOfUpgradesInQ int
	
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
		-- determin if there is a building in this village. if no, we certainly cannot downgrade
		--
		set @CurrentBuildingLevel = (select level from Buildings where VillageID = @VillageID and BuildingTypeID = @BuildingID)
		IF @CurrentBuildingLevel is null BEGIN 
			IF @@TRANCOUNT > 0 ROLLBACK
			IF @DEBUG = 1 SELECT ' no such building in village. Exit quietly...'
			return 
		END
		IF @DEBUG = 1 SELECT @CurrentBuildingLevel as '@CurrentBuildingLevel'
		
		--
		-- see if there are any upgrades currently in progress. if so, do not allow the downgrade
		--
		set @NumOfCurrentlyUpgrading = (select count(*)  
												from BuildingUpgrades BU
												join  Events E on BU.EventID = E.EventID 		
												where VillageID = @VillageID
												and status <> 1
												)
		set @NumOfEntriesInUpgradeQ = (SELECT count(*) 
												FROM BuildingUpgradeQEntries BUQ
												WHERE VillageID = @VillageID
												)
		IF @NumOfCurrentlyUpgrading <> 0 
		    OR @NumOfEntriesInUpgradeQ <> 0 BEGIN 
			IF @@TRANCOUNT > 0 ROLLBACK
			IF @DEBUG = 1 SELECT ' buildings are currently being upgraded. Exit quietly...'
			return 
		END
		
		
		
					
		--
		-- submit to Q
		--
		insert into BuildingDowngradeQEntries (VillageID, BuildingTypeID, DateAdded) values (@VillageID, @BuildingID, getdate())
		--
		-- see if we need to trigger processing this from Q to upgrade
		--
		set @Now = getdate()
		exec iBuildingDowngrade_GetOneFromQ @VillageID, @Now , @PrintDebugInfo
		
	commit tran 
	
	IF @DEBUG = 1 SELECT 'END iBuildingDowngrade' + cast(@VillageID as varchar(10)) + cast(@BuildingID as varchar(10))	
	end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iBuildingDowngrade FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @VillageID'			+ ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @BuildingID'			+ ISNULL(CAST(@BuildingID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @CurrentBuildingLevel'+ ISNULL(CAST(@CurrentBuildingLevel AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @Now'					+ ISNULL(CAST(@Now AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @NumOfCurrentlyUpgrading' + ISNULL(CAST(@NumOfCurrentlyUpgrading AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @NumOfEntriesInUpgradeQ' + ISNULL(CAST(@NumOfEntriesInUpgradeQ AS VARCHAR(100)), 'Null') + CHAR(10)
		
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
