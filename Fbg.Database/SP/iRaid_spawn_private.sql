 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iRaid_spawn_private')
	BEGIN
		DROP  Procedure  iRaid_spawn_private
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].iRaid_spawn_private
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
IF @DEBUG = 1 print  'BEGIN spawn raids ' 


--
--
--  ********************* P R I V A T E     R A I D S **************************
--
--

--
-- get private raid templates
--
IF OBJECT_ID('tempdb..#RelevantRaidTemplates') IS NOT NULL BEGIN
	drop table  #RelevantRaidTemplates
END
select RaidTemplateID into #RelevantRaidTemplates from RaidTemplate where TypeID = 2


--
-- get realm age 
--
declare @DayOfThisRealm int 
SELECT @DayOfThisRealm = datediff(day, openon, getdate()) from realm 

--
-- how long do the early raids last? this param represents the time-on-realm for new player until which he has early raids, thus no permanent private raids 
--
declare @EarlyRaidsUntill real 
select top 1 @EarlyRaidsUntill = AvailableToDay from RaidTemplate where TypeID = 1 order by AvailableToDay desc


--
-- get players that need a new private raid (that is, have no private raid that does not need a respawn now)
--

IF OBJECT_ID('tempdb..#ps') IS NOT NULL BEGIN
	drop table  #ps
END 
select p.playerid into  #ps from players P 
	where  p.points > 0 
		and dateadd(minute, @EarlyRaidsUntill * 1440, RegisteredOn) < getdate() -- do not get players that still are getting the early raids
		and not exists 
		(		
			select * from Raids R 
				join PrivateRaids PR 
					on R.RaidID = PR.RaidID				
				join RaidTemplate RT 
					ON R.RaidTemplateID = RT.RaidTemplateID 
				where PR.PlayerID = P.PlayerID
					and DATEADD(minute, RT.SpawnRate, R.CreatedOn) > getdate() -- created date + spawn rate has still not passed
		)
--
-- get raid templates for possile private raids to spawn
--
IF OBJECT_ID('tempdb..#raidTemplatesToConsider') IS NOT NULL BEGIN
	drop table  #raidTemplatesToConsider
END 
select null as PlayerID, RT.RaidTemplateID into #raidTemplatesToConsider from raidtemplate RT 
		where ( AvailableFromDay <= @DayOfThisRealm and @DayOfThisRealm <= AvailableToDay ) and  TypeID = 2

EXEC iRaid_spawn_givePrivateRaids @DEBUG


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



IF @DEBUG = 1 print 'END spawn raids ' 
