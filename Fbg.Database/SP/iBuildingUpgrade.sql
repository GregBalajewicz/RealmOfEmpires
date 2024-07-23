IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iBuildingUpgrade')
	BEGIN
		DROP  Procedure  iBuildingUpgrade
	END

GO

CREATE Procedure iBuildingUpgrade
	@VillageID int
	,@BuildingID int
	,@level int
	,@MaxNumOfUpgradesInQ int  -- maximum number of upgrades allowed. used to enforce paid features. set to 9999 if no limit *** NOT USED RIGHT NOW ****
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
	IF @DEBUG = 1 SELECT 'BEGIN iBuildingUpgrade' + cast(@VillageID as varchar(10)) + cast(@BuildingID as varchar(10)) + cast(@level as varchar(10))

	declare @Cost int
	declare @Now DateTime
	declare @CoinSubtractSuccess bit
	declare @CurrentBuildingLevel int
	declare @CurrentBuildingLevel_Upgrading int -- level of the building in BuildingUpgrades 
	declare @CurrentBuildingLevel_NoOfEntriesInUpgradeQ int -- level of the building in the BuildingUpgradeQEntries. THIS IS NOT ACTULLY a LEVEL but a count of the entries for this building
	declare @NumOfUpgradesInQ int
	declare @MaxPop int
	declare @CurPop int
	declare @RemPop int
	declare @PopForThisUpgrade int
	
__RETRY__:
	begin try  
	
	
	
	
	begin tran	iBuildingUpgrade
		--
		-- Obtain a village LOCK
		--		this will ensure that no one else will change the buildings in the village, or effect the building Q 
		--
		IF @DEBUG = 1 SELECT 'Obtain lock ' 
		update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @VillageID
		IF @DEBUG = 1 SELECT 'Obtain lock - success' 

		--
		-- determin if the level is correct. 
		--	Ie, the level of the upgrade must be 1 higher then the current building level or upgrade in the Q 
		--
		set @CurrentBuildingLevel = (select level from Buildings where VillageID = @VillageID and BuildingTypeID = @BuildingID)
		IF @CurrentBuildingLevel is null BEGIN 
			set @CurrentBuildingLevel = 0	
		END 
		IF @DEBUG = 1 SELECT @CurrentBuildingLevel as '@CurrentBuildingLevel'
		
		set @CurrentBuildingLevel_Upgrading = (select Level 
												from BuildingUpgrades BU
												join  Events E on BU.EventID = E.EventID 		
												where VillageID = @VillageID and BuildingTypeID = @BuildingID 
												and status <> 1
												)
		IF @CurrentBuildingLevel_Upgrading is not null BEGIN 
			IF @CurrentBuildingLevel_Upgrading < @CurrentBuildingLevel BEGIN
				-- should never have an upgrade with a lover level then the current building level. something is wrong. 
				raiserror ('@CurrentBuildingLevel_Upgrading < @CurrentBuildingLevel', 11,1)
			END
			
			set @CurrentBuildingLevel = @CurrentBuildingLevel_Upgrading	
		END
		IF @DEBUG = 1 SELECT @CurrentBuildingLevel as '@CurrentBuildingLevel', @CurrentBuildingLevel_Upgrading as '@CurrentBuildingLevel_Upgrading'

		set @CurrentBuildingLevel_NoOfEntriesInUpgradeQ = (SELECT count(*) 
												FROM BuildingUpgradeQEntries BUQ
												WHERE VillageID = @VillageID AND BuildingTypeID = @BuildingID 
												)
		IF @CurrentBuildingLevel_NoOfEntriesInUpgradeQ is not null BEGIN 
			set @CurrentBuildingLevel = @CurrentBuildingLevel + @CurrentBuildingLevel_NoOfEntriesInUpgradeQ
		END
		IF @DEBUG = 1 SELECT @CurrentBuildingLevel as '@CurrentBuildingLevel'
			, @CurrentBuildingLevel_Upgrading as '@CurrentBuildingLevel_Upgrading'
			, @CurrentBuildingLevel_NoOfEntriesInUpgradeQ as '@CurrentBuildingLevel_NoOfEntriesInUpgradeQ'

		
		IF @CurrentBuildingLevel +1 <> @level BEGIN
			-- the upgrade request we got is inconsistent. This can happen, for example, when the upgrade link is quickly clicked twice 
			--	and get end up with two identical calls. the first one comepltes, add theupgrad to the Q, and then the other one will fail here
			--	This is an expected behavour so we just exit quietly
			IF @@TRANCOUNT > 0 ROLLBACK
			IF @DEBUG = 1 SELECT ' @CurrentBuildingLevel +1 <> @level. Exit quietly...'
			return 
		END 
			
		--
		-- Get the cost of this upgrade
		--
		select @cost = cost, @PopForThisUpgrade = population from buildingLevels where buildingTypeID = @BuildingID and level = @level

		exec uVillageCoins_Subtract @VillageID ,@cost, @CoinSubtractSuccess OUT, 0
		
		--
		-- verify the cost does not exceeed assets. ( this can happen for the same reason the level may be inconsistent above
		--
		IF @CoinSubtractSuccess = 0 BEGIN
			IF @@TRANCOUNT > 0 ROLLBACK
			--INSERT INTO ErrorLog VALUES (getdate(), 0, '@CoinSubtractSuccess = 0. Exit quietly...',  ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null'))		
			IF @DEBUG = 1 SELECT ' @CoinSubtractSuccess = 0. Exit quietly...'
			return 	
		END
		-- 
		-- validating population
		-- 
		set @MaxPop = dbo.fnGetMaxPopulation(@VillageId)
		set @CurPop = dbo.fnGetCurrentPopulation(@VillageID)
		set @RemPop = @MaxPop - @CurPop
		
		IF @PopForThisUpgrade <> 0 AND @PopForThisUpgrade > @RemPop BEGIN
			-- the upgraderequest we got is inconsistent. This can happen, for example, when the upgrade is quickly clicked twice 
			--	and get end up with two identical calls. 
			--	This is an expected behavour so we just exit quietly
			IF @@TRANCOUNT > 0 ROLLBACK
			INSERT INTO ErrorLog VALUES (getdate(), 0, '@PopForThisUpgrade <> 0 AND @PopForThisUpgrade > @RemPop',  ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null'))		
			return 
		END			
		
					
		--
		-- submit to Q
		--
		insert into BuildingUpgradeQEntries (VillageID, BuildingTypeID, DateAdded) values (@VillageID, @BuildingID, getdate())
		--
		-- see if we need to trigger processing this from Q to upgrade
		--
		set @Now = getdate()
		exec iBuildingUpgrade_GetOneFromQ @VillageID, @Now , @PrintDebugInfo
		
	commit tran iBuildingUpgrade
	
	IF @DEBUG = 1 SELECT 'END iBuildingUpgrade' + cast(@VillageID as varchar(10)) + cast(@BuildingID as varchar(10)) + cast(@level as varchar(10))	
	end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iBuildingUpgrade FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @VillageID'			+ ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @BuildingID'			+ ISNULL(CAST(@BuildingID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @level'				+ ISNULL(CAST(@level AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @CurrentBuildingLevel'+ ISNULL(CAST(@CurrentBuildingLevel AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CoinSubtractSuccess'	+ ISNULL(CAST(@CoinSubtractSuccess AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @Cost'				+ ISNULL(CAST(@Cost AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @Now'					+ ISNULL(CAST(@Now AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CurrentBuildingLevel_Upgrading' + ISNULL(CAST(@CurrentBuildingLevel_Upgrading AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CurrentBuildingLevel_NoOfEntriesInUpgradeQ' + ISNULL(CAST(@CurrentBuildingLevel_NoOfEntriesInUpgradeQ AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @MaxPop'				+ ISNULL(CAST(@MaxPop AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CurPop'				+ ISNULL(CAST(@CurPop AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @RemPop'				+ ISNULL(CAST(@RemPop AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @PopForThisUpgrade'	+ ISNULL(CAST(@PopForThisUpgrade AS VARCHAR(10)), 'Null') + CHAR(10)
		
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
