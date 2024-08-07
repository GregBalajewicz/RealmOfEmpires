IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = '_PopulateDB_BonusVillages')
BEGIN
	DROP  Procedure  _PopulateDB_BonusVillages
END

GO


CREATE Procedure dbo._PopulateDB_BonusVillages
	@RealmType varchar(100) -- NOOB, MC, HC, X, CLASSIC, 6M
	,@RealmSubType varchar(100) -- Holiday14d, A, B, C, D, E, F, 31d
	,@BonusVillageTypes int = null

	AS
set nocount on


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
-- *************************************************************
-- Village Types
--	first we enter default types, then perhaps we overide it for some realm types
-- *************************************************************
--

IF @BonusVillageTypes is null  BEGIN 
IF @RealmType = 'CLASSIC' BEGIN
		SET @BonusVillageTypes = 0
	END ELSE IF @RealmType = 'X' AND (@RealmSubType = 'A' OR @RealmSubType = 'B') BEGIN	
		SET @BonusVillageTypes = 1
	END ELSE IF  @RealmType = '6M' OR @RealmType = 'MC' BEGIN
		SET @BonusVillageTypes = 2
	END ELSE BEGIN 
		SET @BonusVillageTypes = 3
	END
END





IF @BonusVillageTypes = 0 BEGIN
	-- no bonus types	
	insert into VillageTypes values (0, '', '') 

END ELSE IF @BonusVillageTypes = 1 BEGIN	
	declare @VT_balanced int
	declare @VT_prosperous int
	declare @VT_warrior int
	declare @VT_specialist int
	declare @VT_defensive int
	declare @VT_bountiful int
	declare @VT_spymaster int

	set @VT_balanced = 1
	set @VT_prosperous = 2
	set @VT_warrior = 3
	set @VT_specialist = 4
	set @VT_defensive = 5
	set @VT_bountiful = 6
	set @VT_spymaster = 7

	insert into VillageTypes values (0, '', '') 

	insert into VillageTypes values (@VT_balanced, 'Balanced Village: +100% Silver Production, +25% Recruitment At All Buildings.', '') 
	insert into VillageTypes values (@VT_prosperous, 'Prosperous Village: +100% Silver Production, +40% Farm Yield.', '') 
	insert into VillageTypes values (@VT_warrior, 'Warrior Encampment: +50% Stables Recruitment, +50% Siege Workshop Recruitment.', '') 
	insert into VillageTypes values (@VT_specialist, 'Specialist Village: +75% Siege Workshop Recruitment, +75% Tavern Recruitment.', '') 
	insert into VillageTypes values (@VT_defensive, 'Mighty Bulwark: +50% Barracks Recruitment, +40% Defense Factor.', '') 
	insert into VillageTypes values (@VT_bountiful, 'Bountiful Village: +40% Farm Yield, +25% Recruitment At All Buildings.', '') 
	insert into VillageTypes values (@VT_spymaster, 'Spymasters Citadel: +150% Silver Production, +100% Tavern Production', '') 

	insert into VillageTypePropertyTypes values (1, '', 3, @LevelProp_CoinMineProduction)
	insert into VillageTypePropertyTypes values (2, '', 3, @LevelProp_BarracksRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (3, '', 3, @LevelProp_StableRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (4, '', 3, @LevelProp_WorkshopRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (5, '', 3, @LevelProp_PopulationCapacity)
	insert into VillageTypePropertyTypes values (6, '', 3, @LevelProp_DefenseFactor)
	insert into VillageTypePropertyTypes values (7, '', 3, @LevelProp_TavernRecruitTimeFactor)

	insert into VillageTypeProperties values (@VT_balanced, 1, '1.00')
	insert into VillageTypeProperties values (@VT_balanced, 2, '0.25')
	insert into VillageTypeProperties values (@VT_balanced, 3, '0.25')
	insert into VillageTypeProperties values (@VT_balanced, 4, '0.25')
	insert into VillageTypeProperties values (@VT_balanced, 7, '0.25')
	insert into VillageTypeProperties values (@VT_prosperous, 1, '1.00')
	insert into VillageTypeProperties values (@VT_prosperous, 5, '0.40')
	insert into VillageTypeProperties values (@VT_warrior, 3, '0.50')
	insert into VillageTypeProperties values (@VT_warrior, 4, '0.50')
	insert into VillageTypeProperties values (@VT_specialist, 4, '0.75')
	insert into VillageTypeProperties values (@VT_specialist, 7, '0.75')
	insert into VillageTypeProperties values (@VT_defensive, 2, '0.50')
	insert into VillageTypeProperties values (@VT_defensive, 6, '0.40')
	insert into VillageTypeProperties values (@VT_bountiful, 5, '0.40')
	insert into VillageTypeProperties values (@VT_bountiful, 2, '0.25')
	insert into VillageTypeProperties values (@VT_bountiful, 3, '0.25')
	insert into VillageTypeProperties values (@VT_bountiful, 4, '0.25')
	insert into VillageTypeProperties values (@VT_bountiful, 7, '0.25')
	insert into VillageTypeProperties values (@VT_spymaster, 1, '1.50')
	insert into VillageTypeProperties values (@VT_spymaster, 7, '1.00')

END ELSE IF  @BonusVillageTypes = 2  BEGIN
	declare @VT_balanced2 int
	declare @VT_prosperous2 int
	declare @VT_warrior2 int
	declare @VT_specialist2 int
	declare @VT_defensive2 int
	declare @VT_bountiful2 int
	declare @VT_spymaster2 int

	set @VT_balanced2 = 1
	set @VT_prosperous2 = 2
	set @VT_warrior2 = 3
	set @VT_specialist2 = 4
	set @VT_defensive2 = 5
	set @VT_bountiful2 = 6
	set @VT_spymaster2 = 7

	insert into VillageTypes values (0, '', '') 

	insert into VillageTypes values (@VT_balanced2, 'Balanced Village: +50% Silver Production, +25% Recruitment At All Buildings.', 'https://static.realmofempires.com/images/BonusIcons/barracks') 
	insert into VillageTypes values (@VT_prosperous2, 'Prosperous Village: +100% Silver Production, +20% Farm Yield.', 'https://static.realmofempires.com/images/BonusIcons/silvermine') 
	insert into VillageTypes values (@VT_warrior2, 'Warrior Encampment: +50% Stables Recruitment, +50% Siege Workshop Recruitment.', 'https://static.realmofempires.com/images/BonusIcons/stable') 
	insert into VillageTypes values (@VT_specialist2, 'Specialist Village: +100% Siege Workshop Recruitment, +100% Tavern Recruitment.', 'https://static.realmofempires.com/images/BonusIcons/recruit') 
	insert into VillageTypes values (@VT_defensive2, 'Mighty Bulwark: +50% Barracks Recruitment, +40% Defense Factor.', 'https://static.realmofempires.com/images/BonusIcons/defense') 
	insert into VillageTypes values (@VT_bountiful2, 'Bountiful Village: +25% Farm Yield, +10% Recruitment At All Buildings.', 'https://static.realmofempires.com/images/BonusIcons/farm') 
	insert into VillageTypes values (@VT_spymaster2, 'Spymasters Citadel: +150% Silver Production, +200% Tavern Production', 'https://static.realmofempires.com/images/BonusIcons/tavern') 

	insert into VillageTypePropertyTypes values (1, '', 3, @LevelProp_CoinMineProduction)
	insert into VillageTypePropertyTypes values (2, '', 3, @LevelProp_BarracksRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (3, '', 3, @LevelProp_StableRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (4, '', 3, @LevelProp_WorkshopRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (5, '', 3, @LevelProp_PopulationCapacity)
	insert into VillageTypePropertyTypes values (6, '', 3, @LevelProp_DefenseFactor)
	insert into VillageTypePropertyTypes values (7, '', 3, @LevelProp_TavernRecruitTimeFactor)

	insert into VillageTypeProperties values (@VT_balanced2, 1, '0.50')
	insert into VillageTypeProperties values (@VT_balanced2, 2, '0.25')
	insert into VillageTypeProperties values (@VT_balanced2, 3, '0.25')
	insert into VillageTypeProperties values (@VT_balanced2, 4, '0.25')
	insert into VillageTypeProperties values (@VT_balanced2, 7, '0.25')
	insert into VillageTypeProperties values (@VT_prosperous2, 1, '1.00')
	insert into VillageTypeProperties values (@VT_prosperous2, 5, '0.20')
	insert into VillageTypeProperties values (@VT_warrior2, 3, '0.50')
	insert into VillageTypeProperties values (@VT_warrior2, 4, '0.50')
	insert into VillageTypeProperties values (@VT_specialist2, 4, '1.00')
	insert into VillageTypeProperties values (@VT_specialist2, 7, '1.00')
	insert into VillageTypeProperties values (@VT_defensive2, 2, '0.50')
	insert into VillageTypeProperties values (@VT_defensive2, 6, '0.40')
	insert into VillageTypeProperties values (@VT_bountiful2, 5, '0.25')
	insert into VillageTypeProperties values (@VT_bountiful2, 2, '0.10')
	insert into VillageTypeProperties values (@VT_bountiful2, 3, '0.10')
	insert into VillageTypeProperties values (@VT_bountiful2, 4, '0.10')
	insert into VillageTypeProperties values (@VT_bountiful2, 7, '0.10')
	insert into VillageTypeProperties values (@VT_spymaster2, 1, '1.50')
	insert into VillageTypeProperties values (@VT_spymaster2, 7, '2.00')

END ELSE IF  @BonusVillageTypes = 3  BEGIN
	--
	-- default bunus types
	-- 
	declare @VT_MoreSilver int
	declare @VT_MoreBarrRecruit int
	declare @VT_MoreStblRecruit int
	declare @VT_MoreTavernRecruit int
	declare @VT_MoreAllRec int
	declare @VT_Morefarm int
	declare @VT_MoreDef int

	set @VT_MoreSilver = 1
	set @VT_MoreBarrRecruit = 2
	set @VT_MoreAllRec = 3
	set @VT_Morefarm = 4
	set @VT_MoreDef = 5
	set @VT_MoreStblRecruit = 6
	set @VT_MoreTavernRecruit = 7


	insert into VillageTypes values (0, '', '') 


	insert into VillageTypes values (@VT_MoreSilver, '50% more Silver Production', 'https://static.realmofempires.com/images/BonusIcons/silvermine') 
	insert into VillageTypes values (@VT_MoreBarrRecruit, '50% faster recruitment at the Barracks', 'https://static.realmofempires.com/images/BonusIcons/barracks') 
	insert into VillageTypes values (@VT_MoreAllRec, '20% faster recruitment at all buildings', 'https://static.realmofempires.com/images/BonusIcons/recruit') 
	insert into VillageTypes values (@VT_Morefarm, '40% higher farm yield', 'https://static.realmofempires.com/images/BonusIcons/farm') 
	insert into VillageTypes values (@VT_MoreDef, '40% Wall and Tower defence bonus', 'https://static.realmofempires.com/images/BonusIcons/defense') 
	insert into VillageTypes values (@VT_MoreStblRecruit, '50% faster recruitment at the Stables', 'https://static.realmofempires.com/images/BonusIcons/stable') 
	insert into VillageTypes values (@VT_MoreTavernRecruit, '50% faster recruitment at the Tavern', 'https://static.realmofempires.com/images/BonusIcons/tavern') 

	insert into VillageTypePropertyTypes values (1, '', 3, @LevelProp_CoinMineProduction)
	insert into VillageTypePropertyTypes values (2, '', 3, @LevelProp_BarracksRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (3, '', 3, @LevelProp_StableRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (4, '', 3, @LevelProp_WorkshopRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (5, '', 3, @LevelProp_PopulationCapacity)
	insert into VillageTypePropertyTypes values (6, '', 3, @LevelProp_DefenseFactor)
	insert into VillageTypePropertyTypes values (7, '', 3, @LevelProp_TavernRecruitTimeFactor)

	insert into VillageTypeProperties values (@VT_MoreSilver, 1, '0.5')
	insert into VillageTypeProperties values (@VT_MoreBarrRecruit, 2, '0.5')
	insert into VillageTypeProperties values (@VT_MoreAllRec, 2, '0.20')
	insert into VillageTypeProperties values (@VT_MoreAllRec, 3, '0.20')
	insert into VillageTypeProperties values (@VT_MoreAllRec, 4, '0.20')
	insert into VillageTypeProperties values (@VT_MoreAllRec, 7, '0.20')
	insert into VillageTypeProperties values (@VT_Morefarm, 5, '0.40')
	insert into VillageTypeProperties values (@VT_MoreDef, 6, '0.40')
	insert into VillageTypeProperties values (@VT_MoreStblRecruit, 3, '0.5')
	insert into VillageTypeProperties values (@VT_MoreTavernRecruit, 7, '0.5')
END ELSE IF  @BonusVillageTypes = 4  BEGIN

	/*
		
		NOTE ! because this is lacking a village with towers bonus, the battle sim does not look great. see case 33826
	
	*/

	declare @VT_SiH int
	declare @VT_HoW int
	declare @VT_BlM int
	declare @VT_IhM int
	declare @VT_EnF int

	set @VT_balanced2 = 1
	set @VT_prosperous2 = 2
	set @VT_warrior2 = 3
	set @VT_bountiful2 = 6
	set @VT_spymaster2 = 7

	--
	-- HACK HACK HACK HACK HACK HACK 
	--
	-- NOTE : Legnedary bonus villages, that is, village types with ID 8 or higher, have a hardcoded, smaller chance of appearing on the map!!
	--
	-- NOTE : Legnedary bonus villages, also are consolidated differently; they are absorbed like any other village. this is hardcoded, in iPromotedVillage ,by village type id, so if you add more, or change IDs
	--		, iPromotedVillage will have to change too!!
	--
	-- HACK HACK HACK HACK HACK HACK 
	--
	set @VT_SiH = 8
	set @VT_HoW = 9
	set @VT_BlM = 10
	set @VT_IhM = 11
	set @VT_EnF = 12

	insert into VillageTypes values (0, '', '') 

	insert into VillageTypes values (@VT_balanced2, 'Guardian Village: +50% Silver Production, +75% Barracks Recruitment, +10% Recruit All.', 'https://static.realmofempires.com/images/BonusIcons/barracks') 
	insert into VillageTypes values (@VT_prosperous2, 'Prosperous Village: +100% Silver Production, +20% Farm Yield.', 'https://static.realmofempires.com/images/BonusIcons/silvermine') 
	insert into VillageTypes values (@VT_warrior2, 'Warrior Encampment: +50% Stables Recruitment, +50% Siege Workshop Recruitment.', 'https://static.realmofempires.com/images/BonusIcons/stable') 
	insert into VillageTypes values (@VT_bountiful2, 'Bountiful Village: +25% Farm Yield, +10% Recruitment At All Buildings.', 'https://static.realmofempires.com/images/BonusIcons/farm') 
	insert into VillageTypes values (@VT_spymaster2, 'Spymasters Citadel: +150% Silver Production, +200% Tavern Production', 'https://static.realmofempires.com/images/BonusIcons/tavern') 
	
--	insert into VillageTypes values (@VT_SiH, 'Silver Halls: +500% Silver Production, +1000% Trading Post Capacity, -50% Defense Penalty.', 'https://static.realmofempires.com/images/BonusIcons/silvermine') 
	insert into VillageTypes values (@VT_SiH, 'Silver Halls: +300% Silver Production, +1000% Trading Post Capacity, -50% Defense Penalty.', 'https://static.realmofempires.com/images/BonusIcons/silvermine') 
	insert into VillageTypes values (@VT_HoW, 'House of Whispers: +500% Tavern Recruitment, +100% Farm Capacity, -50% Defense Penalty.', 'https://static.realmofempires.com/images/BonusIcons/tavern') 
	insert into VillageTypes values (@VT_BlM, 'Bloodlance Mercenaries: +200% Stables Recruitment, +50% Farm Capacity, -50% Defense Penalty.', 'https://static.realmofempires.com/images/BonusIcons/stable') 
	insert into VillageTypes values (@VT_IhM, 'Ironhand Mercenaries: +300% Barracks Recruitment, +50% Farm Capacity, -50% Defense Penalty.', 'https://static.realmofempires.com/images/BonusIcons/barracks') 
	insert into VillageTypes values (@VT_EnF, 'Endless Fields: +500% Farm Capacity, -50% Defense Penalty.', 'https://static.realmofempires.com/images/BonusIcons/farm') 


	insert into VillageTypePropertyTypes values (1, '', 3, @LevelProp_CoinMineProduction)
	insert into VillageTypePropertyTypes values (2, '', 3, @LevelProp_BarracksRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (3, '', 3, @LevelProp_StableRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (4, '', 3, @LevelProp_WorkshopRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (5, '', 3, @LevelProp_PopulationCapacity)
	insert into VillageTypePropertyTypes values (6, '', 3, @LevelProp_DefenseFactor)
	insert into VillageTypePropertyTypes values (7, '', 3, @LevelProp_TavernRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (8, '', 3, @LevelProp_CoinTransportAmount)
	insert into VillageTypePropertyTypes values (9, '', 3, @LevelProp_PalaceRecruitTimeFactor)

	insert into VillageTypeProperties values (@VT_balanced2, 1, '0.50')
	insert into VillageTypeProperties values (@VT_balanced2, 2, '0.75')
	insert into VillageTypeProperties values (@VT_balanced2, 3, '0.10')
	insert into VillageTypeProperties values (@VT_balanced2, 4, '0.10')
	insert into VillageTypeProperties values (@VT_balanced2, 7, '0.10')
	insert into VillageTypeProperties values (@VT_prosperous2, 1, '1.00')
	insert into VillageTypeProperties values (@VT_prosperous2, 5, '0.20')
	insert into VillageTypeProperties values (@VT_warrior2, 3, '0.50')
	insert into VillageTypeProperties values (@VT_warrior2, 4, '0.50')
	insert into VillageTypeProperties values (@VT_bountiful2, 5, '0.25')
	insert into VillageTypeProperties values (@VT_bountiful2, 2, '0.10')
	insert into VillageTypeProperties values (@VT_bountiful2, 3, '0.10')
	insert into VillageTypeProperties values (@VT_bountiful2, 4, '0.10')
	insert into VillageTypeProperties values (@VT_bountiful2, 7, '0.10')
	insert into VillageTypeProperties values (@VT_spymaster2, 1, '1.50')
	insert into VillageTypeProperties values (@VT_spymaster2, 7, '2.00')
	--insert into VillageTypeProperties values (@VT_SiH, 1, '5.00')
	insert into VillageTypeProperties values (@VT_SiH, 1, '3.00')
	insert into VillageTypeProperties values (@VT_SiH, 8, '10.00')
	insert into VillageTypeProperties values (@VT_SiH, 6, '-0.50')
	insert into VillageTypeProperties values (@VT_HoW, 9, '10.00')
	insert into VillageTypeProperties values (@VT_HoW, 7, '5.00')
	insert into VillageTypeProperties values (@VT_HoW, 5, '1.00')
	insert into VillageTypeProperties values (@VT_HoW, 6, '-0.50')
	insert into VillageTypeProperties values (@VT_BlM, 3, '2.00')
	insert into VillageTypeProperties values (@VT_BlM, 5, '0.50')
	insert into VillageTypeProperties values (@VT_BlM, 6, '-0.50')
	insert into VillageTypeProperties values (@VT_IhM, 2, '3.00')
	insert into VillageTypeProperties values (@VT_IhM, 5, '0.50')
	insert into VillageTypeProperties values (@VT_IhM, 6, '-0.50')
	insert into VillageTypeProperties values (@VT_EnF, 5, '5.00')
	insert into VillageTypeProperties values (@VT_EnF, 6, '-0.50')
END 
ELSE IF  @BonusVillageTypes = 5  BEGIN
	set @VT_MoreSilver = 1
	set @VT_MoreBarrRecruit = 2
	set @VT_MoreAllRec = 3
	set @VT_Morefarm = 4
	set @VT_MoreDef = 5
	set @VT_MoreStblRecruit = 6
	set @VT_MoreTavernRecruit = 7
	
	insert into VillageTypePropertyTypes values (1, '', 3, @LevelProp_CoinMineProduction)
	insert into VillageTypePropertyTypes values (2, '', 3, @LevelProp_BarracksRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (3, '', 3, @LevelProp_StableRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (4, '', 3, @LevelProp_WorkshopRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (5, '', 3, @LevelProp_PopulationCapacity)
	insert into VillageTypePropertyTypes values (6, '', 3, @LevelProp_DefenseFactor)
	insert into VillageTypePropertyTypes values (7, '', 3, @LevelProp_TavernRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (8, '', 3, @LevelProp_CoinTransportAmount)

	insert into VillageTypes values (0, '', '') 

	--
	-- Regular bonus villages
	--
	insert into VillageTypes values (@VT_MoreSilver, 'Prosperous: +75% Silver Production, +300% Trading Post Capacity', 'https://static.realmofempires.com/images/BonusIcons/silvermine') 
		insert into VillageTypeProperties values (@VT_MoreSilver, 1, '0.75')
		insert into VillageTypeProperties values (@VT_MoreSilver, 8, '3.0')
	insert into VillageTypes values (@VT_MoreBarrRecruit, 'Garrison: +100% Barracks Recruitment, +25% silver', 'https://static.realmofempires.com/images/BonusIcons/barracks') 
		insert into VillageTypeProperties values (@VT_MoreBarrRecruit, 2, '1.0')
		insert into VillageTypeProperties values (@VT_MoreBarrRecruit, 1, '0.25')
	insert into VillageTypes values (@VT_Morefarm, 'Bountiful: +40% Farm Yield, +20% Recruit All', 'https://static.realmofempires.com/images/BonusIcons/farm') 
		insert into VillageTypeProperties values (@VT_Morefarm, 5, '0.40')
		insert into VillageTypeProperties values (@VT_Morefarm, 2, '0.20')
		insert into VillageTypeProperties values (@VT_Morefarm, 3, '0.20')
		insert into VillageTypeProperties values (@VT_Morefarm, 4, '0.20')
		insert into VillageTypeProperties values (@VT_Morefarm, 7, '0.20')
	insert into VillageTypes values (@VT_MoreDef, 'Citadel: +40% Wall and Tower Defence Bonus, +25% Barracks Recruitment', 'https://static.realmofempires.com/images/BonusIcons/defense') 
		insert into VillageTypeProperties values (@VT_MoreDef, 6, '0.40')
		insert into VillageTypeProperties values (@VT_MoreDef, 2, '0.25')
	insert into VillageTypes values (@VT_MoreStblRecruit, 'Warrior Encampment: +75% Stable Recruitment, +25% Silver Production', 'https://static.realmofempires.com/images/BonusIcons/stable') 
		insert into VillageTypeProperties values (@VT_MoreStblRecruit, 3, '0.75')
		insert into VillageTypeProperties values (@VT_MoreStblRecruit, 1, '0.25')
	insert into VillageTypes values (@VT_MoreTavernRecruit, 'Spymasters: +200% Tavern Recruitment, +25% silver, +20% Farm Yield', 'https://static.realmofempires.com/images/BonusIcons/tavern') 
		insert into VillageTypeProperties values (@VT_MoreTavernRecruit, 7, '2.0')
		insert into VillageTypeProperties values (@VT_MoreTavernRecruit, 1, '0.25')
		insert into VillageTypeProperties values (@VT_MoreTavernRecruit, 5, '0.20')

	--
	-- legendary bonus villages
	--
	--
	-- HACK HACK HACK HACK HACK HACK 
	--
	-- NOTE : Legnedary bonus villages, that is, village types with ID 8 or higher, have a hardcoded, smaller chance of appearing on the map!!
	--
	-- NOTE : Legnedary bonus villages, also are consolidated differently; they are absorbed like any other village. this is hardcoded, in iPromotedVillage ,by village type id, so if you add more, or change IDs
	--		, iPromotedVillage will have to change too!!
	--
	-- HACK HACK HACK HACK HACK HACK 
	--
	
	set @VT_SiH = 8
	set @VT_HoW = 9
	set @VT_BlM = 10
	set @VT_IhM = 11
	set @VT_EnF = 12

	insert into VillageTypes values (@VT_HoW, 'House of Whispers: +500% Tavern Recruitment, +100% Farm Capacity, -50% Defense Penalty.', 'https://static.realmofempires.com/images/BonusIcons/tavernL') 
		insert into VillageTypeProperties values (@VT_HoW, 7, '5.00')
		insert into VillageTypeProperties values (@VT_HoW, 5, '1.00')
		insert into VillageTypeProperties values (@VT_HoW, 6, '-0.50')
	insert into VillageTypes values (@VT_BlM, 'Bloodlance Mercenaries: +300% Stables Recruitment, +50% Farm Capacity, -50% Defense Penalty.', 'https://static.realmofempires.com/images/BonusIcons/stableL') 
		insert into VillageTypeProperties values (@VT_BlM, 3, '3.00')
		insert into VillageTypeProperties values (@VT_BlM, 5, '0.50')
		insert into VillageTypeProperties values (@VT_BlM, 6, '-0.50')
	insert into VillageTypes values (@VT_IhM, 'Ironhand Mercenaries: +300% Barracks Recruitment, +50% Farm Capacity, -50% Defense Penalty.', 'https://static.realmofempires.com/images/BonusIcons/barracksL') 
		insert into VillageTypeProperties values (@VT_IhM, 2, '3.00')
		insert into VillageTypeProperties values (@VT_IhM, 5, '0.50')
		insert into VillageTypeProperties values (@VT_IhM, 6, '-0.50')
	insert into VillageTypes values (@VT_EnF, 'Endless Fields: +300% Farm Capacity, +100% Recruit All,-50% Defense Penalty.', 'https://static.realmofempires.com/images/BonusIcons/farmL') 
		insert into VillageTypeProperties values (@VT_EnF, 5, '3.00')
		insert into VillageTypeProperties values (@VT_EnF, 2, '1.00')
		insert into VillageTypeProperties values (@VT_EnF, 3, '1.00')
		insert into VillageTypeProperties values (@VT_EnF, 4, '1.00')
		insert into VillageTypeProperties values (@VT_EnF, 7, '1.00')
		insert into VillageTypeProperties values (@VT_EnF, 6, '-0.50')
end
ELSE IF  @BonusVillageTypes = 6  BEGIN
	--
	-- same as 5 but no LVs
	--
	set @VT_MoreSilver = 1
	set @VT_MoreBarrRecruit = 2
	set @VT_MoreAllRec = 3
	set @VT_Morefarm = 4
	set @VT_MoreDef = 5
	set @VT_MoreStblRecruit = 6
	set @VT_MoreTavernRecruit = 7
	
	insert into VillageTypePropertyTypes values (1, '', 3, @LevelProp_CoinMineProduction)
	insert into VillageTypePropertyTypes values (2, '', 3, @LevelProp_BarracksRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (3, '', 3, @LevelProp_StableRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (4, '', 3, @LevelProp_WorkshopRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (5, '', 3, @LevelProp_PopulationCapacity)
	insert into VillageTypePropertyTypes values (6, '', 3, @LevelProp_DefenseFactor)
	insert into VillageTypePropertyTypes values (7, '', 3, @LevelProp_TavernRecruitTimeFactor)
	insert into VillageTypePropertyTypes values (8, '', 3, @LevelProp_CoinTransportAmount)

	insert into VillageTypes values (0, '', '') 

	--
	-- Regular bonus villages
	--
	insert into VillageTypes values (@VT_MoreSilver, 'Prosperous: +75% Silver Production, +300% Trading Post Capacity', 'https://static.realmofempires.com/images/BonusIcons/silvermine') 
		insert into VillageTypeProperties values (@VT_MoreSilver, 1, '0.75')
		insert into VillageTypeProperties values (@VT_MoreSilver, 8, '3.0')
	insert into VillageTypes values (@VT_MoreBarrRecruit, 'Garrison: +100% Barracks Recruitment, +25% silver', 'https://static.realmofempires.com/images/BonusIcons/barracks') 
		insert into VillageTypeProperties values (@VT_MoreBarrRecruit, 2, '1.0')
		insert into VillageTypeProperties values (@VT_MoreBarrRecruit, 1, '0.25')
	insert into VillageTypes values (@VT_Morefarm, 'Bountiful: +40% Farm Yield, +20% Recruit All', 'https://static.realmofempires.com/images/BonusIcons/farm') 
		insert into VillageTypeProperties values (@VT_Morefarm, 5, '0.40')
		insert into VillageTypeProperties values (@VT_Morefarm, 2, '0.20')
		insert into VillageTypeProperties values (@VT_Morefarm, 3, '0.20')
		insert into VillageTypeProperties values (@VT_Morefarm, 4, '0.20')
		insert into VillageTypeProperties values (@VT_Morefarm, 7, '0.20')
	insert into VillageTypes values (@VT_MoreDef, 'Citadel: +40% Wall and Tower Defence Bonus, +25% Barracks Recruitment', 'https://static.realmofempires.com/images/BonusIcons/defense') 
		insert into VillageTypeProperties values (@VT_MoreDef, 6, '0.40')
		insert into VillageTypeProperties values (@VT_MoreDef, 2, '0.25')
	insert into VillageTypes values (@VT_MoreStblRecruit, 'Warrior Encampment: +75% Stable Recruitment, +25% Silver Production', 'https://static.realmofempires.com/images/BonusIcons/stable') 
		insert into VillageTypeProperties values (@VT_MoreStblRecruit, 3, '0.75')
		insert into VillageTypeProperties values (@VT_MoreStblRecruit, 1, '0.25')
	insert into VillageTypes values (@VT_MoreTavernRecruit, 'Spymasters: +200% Tavern Recruitment, +25% silver, +20% Farm Yield', 'https://static.realmofempires.com/images/BonusIcons/tavern') 
		insert into VillageTypeProperties values (@VT_MoreTavernRecruit, 7, '2.0')
		insert into VillageTypeProperties values (@VT_MoreTavernRecruit, 1, '0.25')
		insert into VillageTypeProperties values (@VT_MoreTavernRecruit, 5, '0.20')

end
