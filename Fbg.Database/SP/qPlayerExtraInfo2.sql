IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerExtraInfo2')
	BEGIN
		DROP  Procedure  qPlayerExtraInfo2
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



/*
WHY the "with(nolock)" hint on all tables ?? because we really don't care about dirty reads here and this is executed so 
    often, that we want this to run as fast as possible. 
*/
CREATE Procedure [dbo].qPlayerExtraInfo2
	@PlayerID int
	, @UpdateLastActive bit
	, @ForceGetResearch bit = 0
	, @ActiveStwardsRecordID int
	, @LastHandled_VillageCacheTimeStamps Datetime
AS

begin try 
	DECLARE @ResearchUpdated bit
	declare @Points int
	declare @NewMessageIndicator bit
	declare @NewReportIndicator bit
	declare @SleepModeActiveFrom datetime
    declare @UpdateCompletedQuests bit
	declare @XP int
	declare @StewardActiveORstewardshipActive bit
	declare @VacationModeRequestOn datetime
	declare @VacationModeDaysUsed int
	declare @VacationModeLastEndOn datetime
	declare @Morale int
	declare @MoraleLastUpdated datetime

	declare @WeekendModeTakesEffectOn datetime;
	declare @WeekendModeLastEndOn datetime;

	declare @Raid_NextRaidExpireTime datetime
	declare @Raid_numberOfCompletedRaidsWaitingForRewardAcceptance int

	set @StewardActiveORstewardshipActive = 0 

	-- if not specified, default this to future, so that no cached time stamps will be returned
	IF @LastHandled_VillageCacheTimeStamps is null BEGIN 
		SET @LastHandled_VillageCacheTimeStamps = dateadd(day, 1, @LastHandled_VillageCacheTimeStamps)
	END 

	select @Points= points
	    , @NewMessageIndicator = NewMessageIndicator
	    , @NewReportIndicator = NewReportIndicator
	    , @SleepModeActiveFrom = SleepModeActiveFrom
	    , @ResearchUpdated = ResearchUpdated 
	    , @UpdateCompletedQuests  = UpdateCompletedQuests
		, @XP = XP_cached
		, @VacationModeRequestOn = VacationModeRequestOn
		, @VacationModeDaysUsed = VacationModeDaysUsed
		, @VacationModeLastEndOn = VacationModeLastEndOn
		, @Morale = Morale
		, @MoraleLastUpdated = MoraleLastUpdated
		, @WeekendModeTakesEffectOn = WeekendModeTakesEffectOn
		, @WeekendModeLastEndOn = WeekendModeLastEndOn
	    from Players with(nolock) where playerid = @PlayerID
	
	--
	-- do a stewardship check
	--	IF @ActiveStwardsRecordID is not null  THEN 
	--		we must validate that the stewardship record still exists. This is done when 
	--		steward logs in to the account he is stewarding, and we need to verify that the account owner has not cancelled stwardship
	--
	--  ELSE
	--		otherwise, we check to see if this player has given stewardship to someone and this person accepted.
	--
	if @ActiveStwardsRecordID is not null  BEGIN 
		IF exists(select * from accountstewards where recordid = @ActiveStwardsRecordID) BEGIN
			set @StewardActiveORstewardshipActive = 1
		END
	END ELSE BEGIN
		IF exists(select * from accountstewards where playerid = @PlayerID and state=2) BEGIN
			set @StewardActiveORstewardshipActive = 1
		END
	END 
		    
	--
	-- RESULT SET #0 
	--
	SELECT 	@Points, @NewMessageIndicator , @NewReportIndicator , @SleepModeActiveFrom, @ResearchUpdated, @UpdateCompletedQuests
	, @XP, @StewardActiveORstewardshipActive, @VacationModeRequestOn, @VacationModeDaysUsed, @VacationModeLastEndOn
	, @Morale , @MoraleLastUpdated , @WeekendModeTakesEffectOn, @WeekendModeLastEndOn
    update players set UpdateCompletedQuests = 0 where PlayerID = @PlayerID

	--
	-- RESULT SET #1
	--
	select count(*) from villages with(nolock) where ownerplayerid = @PlayerID	
	
	--
	-- update the last activity field. we normally do not update this if a steward is logged in
	--
	IF @UpdateLastActive = 1 BEGIN
	    update Players set LastActivity = getdate() where playerid = @PlayerID
	END
	
	--
	-- RESULT SET #2, #3
	--
	--
	-- get info about when were some cached items updated last
	--
	select CachedItemID, [TimeStamp] FROM PlayerCacheTimeStamps where PlayerID = @PlayerID

	select VillageID, CachedItemID, [TimeStamp] from VillageCacheTimeStamps where PlayerID = @PlayerID and [timestamp] > @LastHandled_VillageCacheTimeStamps

	--
	-- RESULT set #4 - raids
	--
	select top 1 @Raid_NextRaidExpireTime = Dateadd(minute, ActByDuration, CreatedOn) 
		from Raids R		
		JOIN RaidTemplate RT
			on RT.RaidTemplateID = R.RaidTemplateID
		where
			(  
				R.RaidID in (select raidID from privateraids where playerid = @PlayerID)
				OR R.RaidID in (select raidID from clanraids where playerid = @PlayerID)
			) 
			and not exists (select * from RaidRewardAcceptanceRecord where RaidID = r.RaidID and PlayerID = @PlayerID)
			and Dateadd(minute, ActByDuration, CreatedOn) > getdate()
		order by Dateadd(minute, ActByDuration, CreatedOn) asc

	select @Raid_numberOfCompletedRaidsWaitingForRewardAcceptance = count(*) 
		from Raids R
		JOIN RaidMonster RM
			on RM.RaidID = R.RaidID
		where 
			(  
				R.RaidID in (select raidID from privateraids where playerid = @PlayerID)
				OR R.RaidID in (select raidID from clanraids where playerid = @PlayerID)
			) 
			and not exists (select * from RaidRewardAcceptanceRecord where RaidID = r.RaidID and PlayerID = @PlayerID)
			and RM.CurrentHealth <= 0
			and exists (select * from RaidResults where damagehp > 0 and RaidID = r.RaidID and PlayerID = @PlayerID)


	select @Raid_NextRaidExpireTime, @Raid_numberOfCompletedRaidsWaitingForRewardAcceptance
	--
	-- RESULT SET #5 & #6 (OPTIONAL)
	--

    --
    -- since this result may or may not be returned, always leave it at the very end of the SP. 
    --
	IF @ResearchUpdated = 1 OR @ForceGetResearch = 1 BEGIN
	    SELECT ResearchItemTypeID, ResearchItemID FROM PlayerResearchItems where PlayerID = @PlayerID
	    SELECT R.EventID, E.EventTime, R.ResearchItemTypeID, R.ResearchItemID FROM ResearchInProgress R JOIN Events E on E.EventID = R.EventID WHERE E.Status = 0 and R.PlayerID = @PlayerID order by EventTime desc
	    update players set ResearchUpdated = 0 where PlayerID = @PlayerID
	END

end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qPlayerExtraInfo FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	




 