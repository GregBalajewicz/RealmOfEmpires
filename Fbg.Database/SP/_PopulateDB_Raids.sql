IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = '_PopulateDB_Raids')
BEGIN
	DROP  Procedure  _PopulateDB_Raids
END

GO


CREATE Procedure dbo._PopulateDB_Raids

	AS
set nocount on

declare @RealmType varchar(100) -- NOOB, MC, HC, X etc
declare @RealmXType varchar(100) -- Holiday14d etc
select @RealmType =  attribvalue from RealmAttributes where attribid =2000
select @RealmXType =  attribvalue from RealmAttributes where attribid =2001
declare @RealmID int
select @RealmID = AttribValue from RealmAttributes where AttribID = 33



------------------------------------------------------------
------------------------------------------------------------
------ SOLO EARLY RAID TEMPLATE ------
------------------------------------------------------------
------------------------------------------------------------


-----------------------------------------------
------ 15 - 30 minutes in the realm: ------
-----------------------------------------------

insert into RaidTemplate (RaidTemplateID, SpawnRate, Distance, RarityID, ActByDuration, AvailableFromDay, AvailableToDay, TypeID) 
values(1, 3, 0.1, 0, 20, 0.010416667, 0.020833333, 1);
insert into RaidReward (RaidTemplateID, TypeID, Count) values (1, 102, 500); --silverBag
insert into RaidReward (RaidTemplateID, TypeID, Count) values (1, 101, 2); --speedBuilding
insert into RaidReward (RaidTemplateID, TypeID, Count) values (1, 100, 2); --speedResearch




------------------------------------------------------------------
------------------------------------------------------------------
------ SOLO EARLY MONSTER TEMPLATES ------
------------------------------------------------------------------
------------------------------------------------------------------

-------------------------------------------------------
------ Monster 1 - Bandits ------
-------------------------------------------------------
insert into RaidMonsterTemplate (MonsterTemplateID, Name, [Desc], ImageUrlBG, ImageUrlIcon, CasultyPerc, BaseMaxHealth)
values(1, 'Bandits', 'These bandits have been stealing your sheep.', 'https://static.realmofempires.com/images/raids/Raid_MerryMen.jpg', 'https://static.realmofempires.com/images/raids/Raid_MerryMen.png', 0, 800);
insert into RaidTemplatePossibleMonsters (RaidTemplateID, MonsterTemplateID) values (1, 1);


------------------------------------------------------------
------------------------------------------------------------
------ SOLO REGULAR RAID TEMPLATES ------
------------------------------------------------------------
------------------------------------------------------------


-----------------------------------------------
------ Template Group 1 (1-3day) ------
-----------------------------------------------

insert into RaidTemplate (RaidTemplateID, SpawnRate, Distance, RarityID, ActByDuration, AvailableFromDay, AvailableToDay, TypeID) 
values(1000, 1380, 2, 0, 1440, 0.0831, 3, 2);
insert into RaidReward (RaidTemplateID, TypeID, Count) values (1000, 102, 5000); --silverBag
insert into RaidReward (RaidTemplateID, TypeID, Count) values (1000, 101, 15); --speedBuilding
insert into RaidReward (RaidTemplateID, TypeID, Count) values (1000, 100, 30); --speedResearch
insert into RaidReward (RaidTemplateID, TypeID, Count) values (1000, 2, 10); --unitINF


------------------------------------------------------------------
------------------------------------------------------------------
------ SOLO REGULAR MONSTER TEMPLATES ------
------------------------------------------------------------------
------------------------------------------------------------------

-------------------------------------------------------
------ Monster 1000 - Bandits ------
-------------------------------------------------------
insert into RaidMonsterTemplate (MonsterTemplateID, Name, [Desc], ImageUrlBG, ImageUrlIcon, CasultyPerc, BaseMaxHealth)
values(1000, 'Bandits', 'They keep stealing your sheep.', 'https://static.realmofempires.com/images/raids/Raid_MerryMen.jpg', 'https://static.realmofempires.com/images/raids/Raid_MerryMen.png', 0, 10000);
insert into RaidTemplatePossibleMonsters (RaidTemplateID, MonsterTemplateID) values (1000, 1000);



------------------------------------------------------------
------------------------------------------------------------
------ CLAN RAID TEMPLATES ------
------------------------------------------------------------
------------------------------------------------------------


-----------------------------------------------
------ Template Group 1 (7-14day) ------
-----------------------------------------------

insert into RaidTemplate (RaidTemplateID, SpawnRate, Distance, RarityID, ActByDuration, AvailableFromDay, AvailableToDay, TypeID) 
values(10000, 10080, 14, 0, 7200, 7, 13, 3);
insert into RaidReward (RaidTemplateID, TypeID, Count) values (10000, 102, 200000); --silverBag
insert into RaidReward (RaidTemplateID, TypeID, Count) values (10000, 101, 30); --speedBuilding
insert into RaidReward (RaidTemplateID, TypeID, Count) values (10000, 2, 50); --unitINF
insert into RaidReward (RaidTemplateID, TypeID, Count) values (10000, 5, 150); --unitLC
insert into RaidReward (RaidTemplateID, TypeID, Count) values (10000, 22, 120); --pfElven
insert into RaidReward (RaidTemplateID, TypeID, Count) values (10000, 32, 60); --pfRebelRush


------------------------------------------------------------------
------------------------------------------------------------------
------ CLAN MONSTER TEMPLATES ------
------------------------------------------------------------------
------------------------------------------------------------------

-------------------------------------------------------
------ Monster 10000 - Red Dragon ------
-------------------------------------------------------
insert into RaidMonsterTemplate (MonsterTemplateID, Name, [Desc], ImageUrlBG, ImageUrlIcon, CasultyPerc, BaseMaxHealth)
values(10000, 'Red Dragon', 'The fires of a red dragon can melt flesh and steel alike.', 'https://static.realmofempires.com/images/raids/Raid_MonsterDragon01.jpg', 'https://static.realmofempires.com/images/raids/Raid_MonsterDragon01.png', 5, 375000);
insert into RaidTemplatePossibleMonsters (RaidTemplateID, MonsterTemplateID) values (10000, 10000);

