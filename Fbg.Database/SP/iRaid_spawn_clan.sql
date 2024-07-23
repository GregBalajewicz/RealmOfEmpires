 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iRaid_spawn_clan')
	BEGIN
		DROP  Procedure  iRaid_spawn_clan
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].iRaid_spawn_clan
			@PrintDebugInfo BIT = 0

AS



DECLARE @DEBUG INT, @DBG varchar(10)
IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
	SET @DEBUG = 1
	set @DBG = ':::::::'
	SET NOCOUNT OFF
END ELSE BEGIN
	SET @DEBUG = 0
	SET NOCOUNT ON
END 
IF @DEBUG = 1 print  'BEGIN spawn clan raids ' 


--
--
--  ********************* C L A N     R A I D S **************************
--
--

--
-- get clan raid templates
--
IF OBJECT_ID('tempdb..#RelevantRaidTemplates') IS NOT NULL BEGIN
	drop table  #RelevantRaidTemplates
END
select RaidTemplateID into #RelevantRaidTemplates from RaidTemplate where TypeID = 3


--
-- how long do the early raids last? this param represents the time-on-realm for new player until which he has early raids 
--
declare @EarlyRaidsUntill real 
select top 1 @EarlyRaidsUntill = AvailableToDay from RaidTemplate  where TypeID = 1 order by AvailableToDay desc
--
-- get realm age 
--
declare @DayOfThisRealm int 
SELECT @DayOfThisRealm = datediff(day, openon, getdate()) from realm 

--
-- get clans that should get the next clan raid
--
IF OBJECT_ID('tempdb..#clans') IS NOT NULL BEGIN
	drop table  #clans
END 
select c.ClanID into #clans from Clans C 	
	where 
		-- where we do not have a clan raid for this clan, that still has not past its respawn rate
		not exists (
			select * from raids R
			join RaidTemplate RT ON R.RaidTemplateID = RT.RaidTemplateID 
				where DATEADD(minute, RT.SpawnRate, R.CreatedOn) > getdate() -- created date + spawn rate has still not passed
				and R.RaidID in (
								select raidID from ClanRaids CR
									join ClanMembers CM on CM.ClanID = C.ClanID and CR.PlayerID = CM.PlayerID
								)		
		 )
		 and exists (select * from ClanMembers CM where  CM.Clanid = C.clanid group by clanid having count(*) >= 4 )


--
-- get raid templates for possible clan raids to spawn
--
IF OBJECT_ID('tempdb..#raidTemplatesToConsider') IS NOT NULL BEGIN
	drop table  #raidTemplatesToConsider
END 
select RT.RaidTemplateID into #raidTemplatesToConsider from raidtemplate RT 
		where ( AvailableFromDay <= @DayOfThisRealm and @DayOfThisRealm <= AvailableToDay )
		and TypeID = 3  

IF NOT EXISTS (SELECT * from #raidTemplatesToConsider) BEGIN 
	IF @DEBUG = 1 print 'no clan raid templates to consider'
	RETURN 
END

--
--
--
-- select one raid template randomly
--		All clans will get the same rolled raid template
--
--
--

declare @RarityRolled int
declare @RarityOfActuallyGivenRaid int
declare @randomRoll int
declare @raidTemplateChosen int
declare @monsterTemplateChosen int



--
-- Roll for rarity - select the rarity based on chance
--
set @randomRoll = rand() * 100
if @randomRoll <= 30 BEGIN			-- 30%
	set @RarityRolled = 0
END ELSE if @randomRoll <= 55 BEGIN -- 25%
	set @RarityRolled = 1
END ELSE if @randomRoll <= 75 BEGIN -- 20% 
	set @RarityRolled = 2
END ELSE if @randomRoll <= 90 BEGIN -- 15%
	set @RarityRolled = 3
END ELSE BEGIN						-- 10 
	set @RarityRolled = 4
END

IF @DEBUG = 1 print '@RarityRolled:' + cast(@RarityRolled as varchar(max)) + ', @randomRoll:' + cast(@randomRoll as varchar(max)) 


-- get random raid of this rarity;	
select TOP 1 @raidTemplateChosen = RT.RaidTemplateID 
	from raidtemplate RT 
	where 
		RT.RaidTemplateID in (select RaidTemplateID from #raidTemplatesToConsider)
		and RarityID = @RarityRolled 
	order by NEWID()


-- could not find a raid of this rarity, so just find any raid
if (@raidTemplateChosen is null) BEGIN 
	IF @DEBUG = 1 print  'could not find raid for this rarity, taking any raid of that rarity or less'
	select TOP 1 @raidTemplateChosen = RT.RaidTemplateID 
		from raidtemplate RT 
		where 
			RT.RaidTemplateID in (select RaidTemplateID from #raidTemplatesToConsider)
			and RarityID <= @RarityRolled 
		order by NEWID()
END


-- could not find a raid of this rarity, so just find any raid
if (@raidTemplateChosen is null) BEGIN 
	IF @DEBUG = 1 print  'No Clan raids'
	RETURN 
END 

-- given the code above, it is possible we did actually give the player of rarity he rolled. This could happen if not rarities are available to the player
--	 at this time (like for example for very early private raids) 
select @RarityOfActuallyGivenRaid = RarityID from raidtemplate where RaidTemplateID = @raidTemplateChosen
 
-- random moster for that raid 
select TOP 1 @monsterTemplateChosen = RMT.MonsterTemplateID
	from RaidTemplatePossibleMonsters RTPM 
	JOIN RaidMonsterTemplate RMT 
		on RMT.MonsterTemplateID = RTPM.MonsterTemplateID
	where RTPM.RaidTemplateID = @raidTemplateChosen
	order by NEWID()	 

--
-- get players that are in the clans we are dealing with
--
IF OBJECT_ID('tempdb..#ps') IS NOT NULL BEGIN
	drop table  #ps
END 
select p.playerid into #Ps from players P 
	join ClanMembers CM on CM.PlayerID = p.PlayerID
	join #clans C on c.ClanID = CM.ClanID
	where p.points > 0 
		and dateadd(minute, @EarlyRaidsUntill * 1440, RegisteredOn) < getdate() -- DO NOT get players that still are getting the early raids 


--
-- for each player, give them thier clan raid
-- 
declare @clanid int 
declare @raidID int
declare @playerCount int
declare @VillageCount int
select top 1 @clanid = clanid from #clans
WHILE (@clanid is not null) BEGIN 
	
	-- clan's player count
	SELECT @playerCount = count(*) from players P 
			join ClanMembers CM on CM.PlayerID = p.PlayerID			
			where p.points > 0 
				and dateadd(minute, @EarlyRaidsUntill * 1440, RegisteredOn) < getdate() -- DO NOT get players that still are getting the early raids 
				and CM.clanid = @clanid

	-- clan's village count
	SELECT @VillageCount = count(*) from villages V 
			join players P on v.ownerplayerid = P.playerid 
			join ClanMembers CM on CM.PlayerID = p.PlayerID			
			where p.points > 0 
				and dateadd(minute, @EarlyRaidsUntill * 1440, RegisteredOn) < getdate() -- DO NOT get players that still are getting the early raids 
				and CM.clanid = @clanid
		
	-- create the raid 
	insert into Raids (CreatedOn, RaidTemplateID, PlayerCount) values (getdate(), @raidTemplateChosen, @playerCount)
	set @raidID = @@IDENTITY

	-- monster for that raid 
	insert into RaidMonster(MonsterTemplateID, RaidID, CurrentHealth, MaxHealth) 
		select TOP 1 
			@monsterTemplateChosen
			, @raidID
			, RMT.BaseMaxHealth + ((@playerCount-1) * RMT.BaseMaxHealth * 1.00) + RMT.BaseMaxHealth * (@RarityOfActuallyGivenRaid * 0.1)
			, RMT.BaseMaxHealth + ((@playerCount-1) * RMT.BaseMaxHealth * 1.00) + RMT.BaseMaxHealth * (@RarityOfActuallyGivenRaid * 0.1)
		from RaidTemplatePossibleMonsters RTPM 
		JOIN RaidMonsterTemplate RMT 
			on RMT.MonsterTemplateID = RTPM.MonsterTemplateID
		where RTPM.RaidTemplateID = @raidTemplateChosen
					order by NEWID()	

	-- make it a clan raid
	insert into ClanRaids(RaidID, PlayerID, PlayerVillageCount) 
		SELECT @RaidID, CM.PlayerID, (select count(*) from villages with (nolock) where ownerplayerid = CM.PlayeriD )
			from ClanMembers CM 
			JOIN Players P on CM.PlayerID = P.PlayerID
			where CM.ClanID = @ClanID
				and p.points > 0 
				and dateadd(minute, @EarlyRaidsUntill * 1440, RegisteredOn) < getdate() -- DO NOT get players that still are getting the early raids 


	IF @DEBUG = 1 print '@clanid :' + cast(@clanid as varchar(max)) +  ' @RaidID:' + cast(@RaidID as varchar(max)) 


	delete #clans where clanid = @clanid
	SET @clanid  = null
	select top 1 @clanid = clanid from #clans
END

select * from raids R join clanraids CR on R.RaidID = CR.RaidID


IF @DEBUG = 1 print 'END spawn clan raids ' 
