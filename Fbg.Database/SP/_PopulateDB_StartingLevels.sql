IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = '_PopulateDB_StartingLevels')
BEGIN
	DROP  Procedure  _PopulateDB_StartingLevels
END

GO


CREATE Procedure dbo._PopulateDB_StartingLevels

	AS
set nocount on

declare @RealmType varchar(100) -- NOOB, MC, HC, X etc
declare @RealmXType varchar(100) -- Holiday14d etc
select @RealmType =  attribvalue from RealmAttributes where attribid =2000
select @RealmXType =  attribvalue from RealmAttributes where attribid =2001
declare @RealmID int
select @RealmID = AttribValue from RealmAttributes where AttribID = 33

--
-- for populating building info
--
declare @counter bigint
declare @BaseCost int
declare @BaseBuildTime bigint
declare @BasePoints int
declare @CostFactor float
declare @BuildTimeFactor bigint
declare @PointsFactor int
declare @BuildingTypeID int

declare @Building_BarracksID integer
declare @Building_StableID integer
declare @Building_HQID integer
declare @Building_WallID integer
declare @Building_DefenseTowersID integer
declare @Building_CoinMineID integer
declare @Building_TreasuryID integer
declare @Building_FarmLandID integer
declare @Building_PalaceID integer
declare @Building_SiegeWorkshopID integer
declare @Building_TradingPostID integer
declare @Building_TavernID integer
declare @Building_HidingSpotID integer

declare @LevelProp_HQTimeFactor int
declare @LevelProp_CoinMineProduction int
declare @LevelProp_TreasuryCapacity int
declare @LevelProp_PopulationCapacity int
declare @LevelProp_StableRecruitTimeFactor int
declare @LevelProp_BarracksRecruitTimeFactor int
declare @LevelProp_WorkshopRecruitTimeFactor int
declare @LevelProp_PalaceRecruitTimeFactor int
declare @LevelProp_DefenseFactor int
declare @LevelProp_CoinTransportAmount int
declare @LevelProp_TavernRecruitTimeFactor int
declare @LevelProp_HidingSpotCapacity int


declare @Unit_infantry_ID int
declare @Unit_LC_ID int
declare @Unit_Knight_ID int
declare @Unit_Ram_ID int
declare @Unit_trab_ID int
declare @Unit_Lord_ID int
declare @Unit_CitizenMilitia_ID int
declare @Unit_Spy_ID int

declare @PA_V1_ID int
declare @PA_V2_ID int
declare @PB_V1_ID int

--set @realmID = 1

set @Building_BarracksID = 1
set @Building_StableID = 2
set @Building_HQID = 3
set @Building_WallID = 4
set @Building_CoinMineID = 5
set @Building_TreasuryID = 6
set @Building_DefenseTowersID = 7
set @Building_FarmLandID = 8
set @Building_PalaceID = 9
set @Building_SiegeWorkshopID = 10
set @Building_TradingPostID = 11
set @Building_TavernID = 12
set @Building_HidingSpotID= 13

set @Unit_infantry_ID = 2
set @Unit_LC_ID = 5
set @Unit_Knight_ID = 6
set @Unit_Ram_ID = 7
set @Unit_trab_ID = 8
set @Unit_Lord_ID = 10
set @Unit_CitizenMilitia_ID = 11
set @Unit_Spy_ID = 12
set @LevelProp_HQTimeFactor = 1
set @LevelProp_CoinMineProduction = 2
set @LevelProp_TreasuryCapacity = 3
set @LevelProp_PopulationCapacity = 4
set @LevelProp_StableRecruitTimeFactor = 5
set @LevelProp_BarracksRecruitTimeFactor = 6
set @LevelProp_WorkshopRecruitTimeFactor = 7
set @LevelProp_PalaceRecruitTimeFactor = 8
set @LevelProp_DefenseFactor = 9
set @LevelProp_CoinTransportAmount = 10
set @LevelProp_TavernRecruitTimeFactor = 12
set @LevelProp_HidingSpotCapacity = 13


--
--
--
-- Village start levels
--
--
--

delete FBGC.FBGCommon.dbo.VillageStartLevels_ResearchSpeedup where realmid  = @RealmID
delete FBGC.FBGCommon.dbo.VillageStartLevels_BuildingSpeedup where realmid  = @RealmID
delete FBGC.FBGCommon.dbo.PlayerStartLevels where realmid  = @RealmID
--truncate table FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup
delete VillageStartLevels_Buildings
delete VillageStartLevels_Units
delete VillageStartLevels_ResearchItems
delete VillageStartLevels


	insert into VillageStartLevels values (1, 999,2000)

	insert into VillageStartLevels_Buildings values (1, @Building_CoinMineID, 1)
	insert into VillageStartLevels_Buildings values (1, @Building_HQID, 1)
	insert into VillageStartLevels_Buildings values (1, @Building_TreasuryID, 1)
	insert into VillageStartLevels_Buildings values (1, @Building_FarmLandID, 1)

	insert into FBGC.fbgcommon.dbo.PlayerStartLevels values (1, 1,@RealmID)
	insert into FBGC.fbgcommon.dbo.PlayerStartLevels values (2, 2,@RealmID)
	insert into FBGC.fbgcommon.dbo.PlayerStartLevels values (3, 3,@RealmID)

	

	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (1,5,@RealmID)
	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (1,5,@RealmID)
	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (1,5,@RealmID)
	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (1,10,@RealmID)
	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (1,10,@RealmID)

	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (2,60,@RealmID)
	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (2,60,@RealmID)
	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (2,60,@RealmID)
	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (2,60,@RealmID)
	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (2,60,@RealmID)
	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (2,60,@RealmID)

	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (3,60,@RealmID)
	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (3,60,@RealmID)
	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (3,60,@RealmID)
	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (3,60,@RealmID)
	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (3,60,@RealmID)
	insert into FBGC.fbgcommon.dbo.VillageStartLevels_BuildingSpeedup values (3,60,@RealmID)
