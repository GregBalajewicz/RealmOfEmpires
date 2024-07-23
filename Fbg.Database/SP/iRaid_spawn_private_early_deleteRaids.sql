 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iRaid_spawn_private_early_deleteRaids')
	BEGIN
		DROP  Procedure  iRaid_spawn_private_early_deleteRaids
	END

GO

CREATE Procedure [dbo].iRaid_spawn_private_early_deleteRaids
	@PrintDebugInfo BIT 

AS
--
--
--  this procedure, depends on having a temp table #RelevantRaidTemplates already created and populated
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


IF @DEBUG = 1 print 'BEGIN iRaid_spawn_deleteRaids ' 
--
--  this procedure, depends on having a temp table #RelevantRaidTemplates already created and populated
--
IF OBJECT_ID('tempdb..#RelevantRaidTemplates') IS  NULL BEGIN
	RETURN 
END

--
-- delete any raids, that require a respawn, and reward is accepted, that are part of the relevant templates as specified in 
-- 

IF @DEBUG = 1 print 'deleting completed raids:' 
-- delete any raids that are past their spawn rate, and have a reward accepted 
select raidid into #RaidsToDelete from Raids R 
	join RaidTemplate RT ON
		R.RaidTemplateID = RT.RaidTemplateID 
	where exists (select * from RaidRewardAcceptanceRecord A where A.RaidID = R.RaidID) -- reward accepted
		and DATEADD(minute, RT.SpawnRate, R.CreatedOn) < getdate()					-- raid is older than its respawn rate
		and RT.RaidTemplateID in (select RaidTemplateID from #RelevantRaidTemplates)		-- early raids

IF @DEBUG = 1 print 'deleting not completed but expired raids:' 
-- delete any raids that are past their respawn rate, and are expired, but the monster has not been defeated, and there are no troops still outgoing to it
insert into #RaidsToDelete
select R.raidid from Raids R 
	join RaidTemplate RT ON
		R.RaidTemplateID = RT.RaidTemplateID 
	join RaidMonster RM 
		on RM.RaidID = R.RaidID
	where 
		 DATEADD(minute, RT.SpawnRate, R.CreatedOn) < getdate()					-- raid is older than its respawn rate
		 and RM.CurrentHealth > 0												-- monster not defeated
		 and not exists (select * from RaidUnitMovements RUM where RUM.RaidID = R.RaidID)	-- no more outgoing troops to it
		 and DATEADD(minute, RT.ActByDuration, R.CreatedOn) < getdate()			-- raid is expired
		and RT.RaidTemplateID in (select RaidTemplateID from #RelevantRaidTemplates)		-- early raids

delete RaidResults from RaidResults rr where rr.RaidID in (select raidid from #RaidsToDelete)
delete PrivateRaids from privateraids pr where pr.RaidID in (select raidid from #RaidsToDelete)
delete RaidMonster from RaidMonster Rd where Rd.raidid in (select raidid from #RaidsToDelete)
delete raids from raids Rd where Rd.raidid in (select raidid from #RaidsToDelete)


IF @DEBUG = 1 print 'END iRaid_spawn_deleteRaids ' 