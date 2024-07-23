    
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iCalculateTournamentSeasonPoints')
	BEGIN
		DROP  Procedure  iCalculateTournamentSeasonPoints
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].iCalculateTournamentSeasonPoints
		@RealmType as varchar(100) -- X or X2 or ALLX
		, @topXToAcount int -- -- Top X realms to take. example - take top 6 scores (out of a season with 8 realms) 
		, @season_id varchar(100)
		, @season_start DateTime
		, @season_end DateTime
AS

set nocount on

/*

 it is NOT ok to call this SP multiple times for the same realms, as the points will be added to the already existing points. 
	this cannot be fixed, because this SP is meant to be called twice - once for Rx and another time for RX2. 
	for example: 

	EXEC fbgc.fbgcommon.dbo.iCalculateTournamentSeasonPoints 'X2', 3, 'Season1', 'july 25 2014', 'October 2 2014'
	EXEC fbgc.fbgcommon.dbo.iCalculateTournamentSeasonPoints 'X', 7, 'Season1', 'july 25 2014', 'October 2 2014'


 so if you do need to clear the points and start over, do something like : 
	delete from fbgc.fbgcommon.dbo.GlobalPlayerRanking_TotalSeasonPoints where season_id = 'season1'


 in fact, the safest way to calculate the points, is to do this : 

 	delete from fbgc.fbgcommon.dbo.GlobalPlayerRanking_TotalSeasonPoints where season_id = 'season1'
	EXEC fbgc.fbgcommon.dbo.iCalculateTournamentSeasonPoints 'X2', 3, 'Season1', 'july 25 2014', 'October 2 2014'
	EXEC fbgc.fbgcommon.dbo.iCalculateTournamentSeasonPoints 'X', 7, 'Season1', 'july 25 2014', 'October 2 2014'
*/


create table #rankingPerRealm (ranking int, RealmOpenOn Datetime, userid uniqueidentifier, playername varchar(max))
create table #userPoints (userid uniqueidentifier, points int)
--
--
--
--  PARAMS!!
-- 
--
--

create table #RealmIDs (RID  int)
if @RealmType = 'X' begin
	insert  #RealmIDs values(0 )
END else IF @RealmType = 'X2' begin
	insert  #RealmIDs values(13)
	insert  #RealmIDs values(21)
END else IF @RealmType = 'ALLX' begin
	insert  #RealmIDs values(-1 )
	insert  #RealmIDs values(-2)
	insert  #RealmIDs values(-3)
	insert  #RealmIDs values(-4)
	insert  #RealmIDs values(-5)
	insert  #RealmIDs values(-6)
	insert  #RealmIDs values(-7)
	insert  #RealmIDs values(-8)
	insert  #RealmIDs values(-9)
	insert  #RealmIDs values(-10)
	insert  #RealmIDs values(13)
	insert  #RealmIDs values(21)
END else begin
	RAISERROR('unrecognized @RealmType',11,1)	
	RETURN 
end

--
--
--
--
-- step through each realm, and get ranking on those realm
--
--
--
--
--

select distinct RealmOpenOn into #realms from GlobalPlayerRanking where realmid in (select RID from #RealmIDs)  and RealmOpenOn between @season_start and @season_end and DateStatsCaptured >= RealmCloseOn
declare @oneRealmOpenOn datetime
select top 1 @oneRealmOpenOn = RealmOpenOn from #realms
while @oneRealmOpenOn is not null BEGIN

	-- the DISTINCT here ensure that if captured the stats twice for the same realm, only one is used
	insert into #rankingPerRealm
	select  distinct rank() OVER (order by totalpoint desc) AS 'RANK' 
	 ,RealmOpenOn, userid, PlayerName from GlobalPlayerRanking where realmid in (select RID from #RealmIDs) and RealmOpenOn = @oneRealmOpenOn 



	delete #realms where RealmOpenOn = @oneRealmOpenOn 
	set @oneRealmOpenOn = null
	select top 1 @oneRealmOpenOn = RealmOpenOn from #realms
END

--
--
--
--
-- step through each USER and calculate their points 
--
--
--
--
--
select distinct userid into #userids from #rankingPerRealm 
--select * from #rankingPerRealm

declare @oneUserid uniqueidentifier

select top 1 @oneUserid = userid from #userids
while @oneUserid is not null BEGIN


	-- FOR DEBUGGING - get all rankings on all realms played by this user
	--select  * from #rankingPerRealm where userid = @oneUserid order by ranking

	-- FOR DEBUGGING - get top X realms that this player played on
	--select top (@topXToAcount) ranking, RealmOpenOn, userid, PlayerName from #rankingPerRealm where userid = @oneUserid order by ranking asc

	insert #userPoints select top (@topXToAcount) userid, dbo.fnRankToPoints( (select top 1 RID from #RealmIDs), ranking) from #rankingPerRealm where userid = @oneUserid order by ranking asc	

	delete #userids where userid = @oneUserid 
	set @oneUserid = null
	select top 1 @oneUserid = userid from #userids
END

--
--
-- RESULTS
--
--

-- NOTES: for playername, we try to grab the name from the latest persisten realm that this player plays on, but if not found, we grab the latest name he used 
--		on the tournament realm, as recorded in the stats
select @season_id as season_id
	, @season_start as season_start
	, @season_end as season_end
	, getdate() as DateStatsCaptured
	, userid
, isnull((select top 1 name from players P where P.userid = UP.userid order by realmid desc)
	, (select top 1 playername from #rankingPerRealm RPR where RPR.userid = UP.userid order by RealmOpenOn desc  )) as PlayerName
, sum(points) as TotalPoints 
into #temp_GlobalPlayerRanking_TotalSeasonPoints
from #userPoints UP group by userid order by sum(points) desc

--
-- for any existing records (for users with scores in this season), we update (add to ) their points
-- 
update R
	SET R.TotalPoints = R.TotalPoints + T.TotalPoints
	FROM GlobalPlayerRanking_TotalSeasonPoints R
	JOIN #temp_GlobalPlayerRanking_TotalSeasonPoints T
		on R.season_id = T.season_id
		and R.userid = T.userid

--
-- for NEW records (for users who do not yet have scores in this season), we add them
-- 
insert into GlobalPlayerRanking_TotalSeasonPoints (season_id , season_start , season_end , DateStatsCaptured , userid , PlayerName , TotalPoints )
	select season_id , season_start , season_end , DateStatsCaptured , userid , PlayerName , TotalPoints from #temp_GlobalPlayerRanking_TotalSeasonPoints T where not exists (select * from GlobalPlayerRanking_TotalSeasonPoints R where R.season_id = T.season_id and R.userid = T.userid)

-- FOR DEBUGGING - Spot check 
--select * from #rankingPerRealm where userid = 'AD319E27-7236-47F8-8BC8-0EEA37FFE131'
