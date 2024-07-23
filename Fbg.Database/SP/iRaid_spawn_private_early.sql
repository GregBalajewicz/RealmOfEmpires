 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iRaid_spawn_private_early')
	BEGIN
		DROP  Procedure  iRaid_spawn_private_early
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].iRaid_spawn_private_early
			@PrintDebugInfo BIT = 0

AS

--
--
--  ********************* E A R L Y    P R I V A T E     R A I D S **************************
--	raids that a new to a realm player gets for the first few hours on the realm
--
--
DECLARE @DEBUG INT, @DBG varchar(10)
IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
	SET @DEBUG = 1
	set @DBG = ':::::::'
	SET NOCOUNT OFF
END ELSE BEGIN
	SET @DEBUG = 0
	SET NOCOUNT ON
END 
IF @DEBUG = 1 print  'BEGIN spawn raids - early ' 

--
-- get the early raids - raids for player for the first few hours of his time in realm. 
--
IF OBJECT_ID('tempdb..#RelevantRaidTemplates') IS NOT NULL BEGIN
	drop table  #RelevantRaidTemplates
END
select RaidTemplateID into #RelevantRaidTemplates from RaidTemplate where TypeID = 1

--
-- if no early raids, exit
--
IF not exists (select * from #RelevantRaidTemplates) BEGIN 
	IF @DEBUG = 1 print  'No raids - early ' 
	RETURN 
END

--
-- how long do the early raids last? this param represents the time-on-realm for new player until which he has early raids 
--
declare @EarlyRaidsUntill real 
select top 1 @EarlyRaidsUntill = AvailableToDay from RaidTemplate  where TypeID = 1 order by AvailableToDay desc

--
-- delete any early raids, that require a respawn, and reward is accepted
--	this called SP, uses #RelevantRaidTemplates to know which raids to conside in deletion
-- 
EXEC iRaid_spawn_private_early_deleteRaids @DEBUG
--
-- get realm age 
--
declare @DayOfThisRealm int 
SELECT @DayOfThisRealm = datediff(day, openon, getdate()) from realm 

--
-- get players that do not have a private raid
--

IF OBJECT_ID('tempdb..#ps') IS NOT NULL BEGIN
	drop table  #ps
END 
select p.playerid into #Ps from players P 
	left join PrivateRaids PR 
		on P.PlayerID = PR.PlayerID 	
	where PR.RaidID is null 
		and p.points > 0 
		and dateadd(minute, @EarlyRaidsUntill * 1440, RegisteredOn) > getdate() -- get players that still are getting the early raids

--
-- get raid templates for possible private raids to spaw
--

IF OBJECT_ID('tempdb..#raidTemplatesToConsider') IS NOT NULL BEGIN
	drop table  #raidTemplatesToConsider
END 
select playerid, raidtemplateid into #raidTemplatesToConsider
	from raidtemplate RT 
	, players P 
		where ( AvailableFromDay <= datediff(minute, p.registeredon, getdate()) / 1440.0  and datediff(minute, p.registeredon, getdate()) / 1440.0 <= AvailableToDay )
		and P.Playerid in (select playerid from #ps)
		and TypeID = 1

IF exists (select * from #raidTemplatesToConsider) BEGIN 

	EXEC iRaid_spawn_givePrivateRaids @DEBUG

END 

--IF @DEBUG = 1 BEGIN

--	--
--	-- temp sanity checks 
--	--
--	select * from Raids R join RaidMonster RM on R.RaidID = RM.RaidID join PrivateRaids PR on PR.RaidID = R.RaidID
--END 
----
---- simulate one player accepting reward
----
--insert into RaidRewardAcceptanceRecord(AcceptedOn, RaidID, PlayerID) select top 1 getdate(), RaidID, PlayerID from PrivateRaids order by NEWID()



IF @DEBUG = 1 print 'END spawn raids - early' 
