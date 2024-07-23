IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qCompleteRealmInfo2')
	BEGIN
		DROP  Procedure  qCompleteRealmInfo2
	END

GO

CREATE Procedure dbo.qCompleteRealmInfo2
AS
select BT.BuildingTypeID, BT.Name ,BT.MinimumLevelAllowed
	from BuildingTypes BT 
	order by BT.Sort

select BT.BuildingTypeID
	, BL.Level
	, Cost
	, BuildTime
	, levelName
	, LP.PropertyID
	, LP.PropertyValue
	, BL.Population
	, BL.CumulativePopulation
	, BL.Points
	, BL.LevelStrength
	, BL.CumulativeLevelStrength
	, LPT.Name
	, LPT.Type
	from  BuildingTypes BT 
	join BuildingLevels BL on BT.BuildingTypeID = BL.BuildingTypeID
	left join LevelProperties LP on BL.BuildingTypeID =  LP.BuildingTypeID and BL.Level= LP.Level
	join LevelPropertyTypes LPT on LPT.PropertyID = LP.PropertyID
	order by BT.Sort, BL.Level


select BL.BuildingTypeID, BL.Level, RequiredBuildingTypeID, RequiredLevel from  BuildingLevels BL 
	join BuildingTypeRequirements R 
		on BL.BuildingTypeID = R.BuildingTypeID
		and BL.Level = R.Level

select UT.UnitTypeID
	, UT.Name
	, UT.[Description]
	, UT.Cost
	, [Population]
	, RecruitmentTime
	, BuildingTypeID 
	, Speed
	, CarryCapacity
	, AttackStrength
	, ImageIcon
	, [Image]
	,SpyAbility
	,CounterSpyAbility
	from UnitTypes UT
order by UT.Sort 

select UT.UnitTypeID, UTRR.BuildingTypeID ,Level
	from UnitTypes UT
	join UnitTypeRecruitmentRequirements UTRR
		on UT.UnitTypeID = UTRR.UnitTypeID
order by UT.Sort 

select max(UnitTypeID)
	from UnitTypes UT

select
	IconUrl
	, VillageTypeName
	, MaxVillagePoints
	, VillageTypeID
from Map
order by VillageTypeID, MaxVillagePoints asc

select D.UnitTypeID
	, D.DefendAgainstUnitTypeID	
	, D.DefenseStrength
	FROM UnitTypeDefense D
	JOIN UnitTypes UT
		ON D.UnitTypeID = UT.UnitTypeID
	ORDER BY UT.Sort

select A.BuildingTypeID
	, A.UnitTypeID
	, A.AttackStrength
	FROM UnitOnBuildingAttack A
	JOIN BuildingTypes BT 
		ON A.BuildingTypeID = BT.BuildingTypeID
	ORDER BY BT.Sort

select CancelAttackInMin, CoinTransportSpeed, BeginnerProtectionDays from Realm

SELECT [BuildingTypeID]
      ,[Level]
      ,[PropertyValue]
      ,[PropertyID]
  FROM [LevelProperties]
  
  
--
-- Get PF info 
--
-- result set #11
select FeatureID, Description from PFs
-- result set #12
select * from PFPackages
-- result set #13
select * from PFsInPackage
-- result set #14
select * from PFTrails where IsActive = 1

--
-- Titles
--
-- result set #15
select TitleID
	, Title_Male
	, Title_Female
	, Description
	, MaxPoints
	, ROW_NUMBER() OVER (order by Titles.MaxPoints) as Level
	, XP
	, (select sum(xp) from Titles T2 where T2.TitleID <= Titles.TitleID ) as CumulativeXP
from Titles order by MaxPoints Asc
--
-- realm attributes
--	result set #16
--
select * from RealmAttributes




--
-- RESEARCH
--	result set #17
--
select ResearchItemTypeID from ResearchItemTypes
--
--	result set #18
--
select RI.ResearchItemTypeID
	, RI.ResearchItemID
	, PriceInCoins
	, RI.Name
	, RI.Description
	, Image1Url
	, Image2Url
	, RILP.PropertyValue
	, RILPT.Type
	, RILPT.PropertyID
	, RI.ResearchTime
		-- HACK!! - we assume each RI has only one RI property. That RI property is related to one building level property and we assume each building has only
		--- one property, and that one level property is related with only one building so we get the building that is related to the level property that our RI
		--- property is related to. WHY DO WE DO THIS?!?!? because the BLL already assumes some of this and there is no way to us to get the level property in
		--- in the bll, just the building and levels
	, (select top 1 BL.BuildingTypeID from BuildingLevels BL join LevelProperties LP on BL.BuildingTypeID = LP.BuildingTypeID and BL.Level = LP.level
			WHERE LP.PropertyID =  RILPT.PropertyID) 
    , RILPT.name
	, RI.AgeNumber
	, RISL.X
	, RISL.Y
	, RILPT.ResearchItemPropertyID as ResearchPropertyTypeID
	from  ResearchItemTypes RIT 
	join ResearchItems RI on RIT .ResearchItemTypeID = RI.ResearchItemTypeID
	left join ResearchItemProperties RILP on RI.ResearchItemTypeID =  RILP.ResearchItemTypeID and RI.ResearchItemID= RILP.ResearchItemID
	left join ResearchItemPropertyTypes RILPT on RILPT.ResearchItemPropertyID = RILP.ResearchItemPropertyID
	left join ResearchItemSpriteLocation RISL on RI.ResearchItemTypeID =  RISL.ResearchItemTypeID and RI.ResearchItemID= RISL.ResearchItemID
	order by RI.ResearchItemTypeID, RI.ResearchItemID
--
--	result set #19
--
select BL.ResearchItemTypeID, BL.ResearchItemID, RequiredResearchItemTypeID, RequiredResearchItemID from  ResearchItems BL 
	join ResearchItemRequirements R 
		on BL.ResearchItemTypeID = R.ResearchItemTypeID
		and BL.ResearchItemID = R.ResearchItemID
--
--	result set #20
--
select 1,'',1,''

--
-- result sets #21,22,23
--
select * from VillageTypes
select * 
		-- HACK!! same as with research
	, (select top 1 BL.BuildingTypeID from BuildingLevels BL join LevelProperties LP on BL.BuildingTypeID = LP.BuildingTypeID and BL.Level = LP.level
			WHERE LP.PropertyID =  V.PropertyID) 
from VillageTypePropertyTypes V
select * from VillageTypeProperties

--
-- result set # 24 - research unit req
--
select * from UnitTypeRecruitmentResearchRequirements

--
-- result set # 25,26,27,28,29 - quests
--
select RowID, QT.TagName, DependantQuestTagName, reward_silver ,reward_credits
	, CompleteCondition_Building_ID, CompleteCondition_Building_Level
	, CompleteCondition_TitleLevel, CompleteCondition_NumVillages, CompleteCondition_ResearchItemID, Title, Goal, Description
 from QuestTemplates QT 
 order by rowID asc

select TagName, RT.UnitTypeID, RT.Amount from QuestTemplates_Reward_Items_Troops RT
 
select TagName, RPFD.PFPackageID, RPFD.DurationInMinutes from QuestTemplates_Reward_Items_PFWithDuration RPFD

select TagName, UITypeID, Description
 from QuestTemplates_Descriptions

select * from questProgression order by Step asc

--
-- result set # 30 - ages
--
select * from RealmAges order by AgeNumber asc

--
-- result set # 31 & 32 - morale
--

select * from  PlayerMoraleEffectsTemplate order by MoraleMax desc
select * from  PlayerMoraleEffects order by Morale desc
