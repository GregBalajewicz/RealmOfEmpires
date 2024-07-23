IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qThroneRoomTournamentRealmStats')
	BEGIN
		DROP  Procedure  qThroneRoomTournamentRealmStats
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].qThroneRoomTournamentRealmStats
	@userid uniqueidentifier
AS

/*

Greg april 2020
- this shoudl really be done better. 
- for now, realmids are still hardcoded. 

*/



--
-- RX stats. RX is a realm that is 7 days or less
--
select  rankbynumberofvillages, count(*) as NumTimesRankAchieved from  GlobalPlayerRanking 
	where ( ( realmid < 0 and realmid>=-10) or  realmid in (0,13,21))  and DATEDIFF(hour, realmopenon, realmcloseon) <= 168 and userid = @userid
 group by  rankbynumberofvillages

select  DATEDIFF(hour, realmopenon, realmcloseon) as HoursLong, rankbynumberofvillages, count(*) as NumTimesRankAchieved 
	from  GlobalPlayerRanking where ( ( realmid < 0 and realmid>=-10) or  realmid in (0,13,21)) and DATEDIFF(hour, realmopenon, realmcloseon) <= 168 and userid = @userid
 group by  rankbynumberofvillages, DATEDIFF(hour, realmopenon, realmcloseon)

--
-- RS Stats. RS is a relam that is more than 5 days; we run those only on 0,13,21 - IF this changes, more IDs will have to be added
--
select  rankbynumberofvillages, count(*) as NumTimesRankAchieved 
	from  GlobalPlayerRanking where realmid in (0,13,21) and DATEDIFF(hour, realmopenon, realmcloseon) > 168 and userid = @userid
 group by  rankbynumberofvillages

select  DATEDIFF(hour, realmopenon, realmcloseon) as HoursLong, rankbynumberofvillages, count(*) as NumTimesRankAchieved 
	from  GlobalPlayerRanking where realmid in (0,13,21) and DATEDIFF(hour, realmopenon, realmcloseon) > 168 and userid = @userid
 group by  rankbynumberofvillages, DATEDIFF(hour, realmopenon, realmcloseon)

 
select  DATEDIFF(hour, realmopenon, realmcloseon) as HoursLong, rankbynumberofvillages, RealmOpenOn 
	from  GlobalPlayerRanking where ( ( realmid < 0 and realmid>=-10) or  realmid in (0,13,21))  and userid = @userid
	order by RealmOpenOn asc
 
--
-- list of all my tournament realms
--
select  DATEDIFF(hour, realmopenon, realmcloseon) as HoursLong, count( distinct realmopenon) as NumberOfSuchRealmsLaunched from  GlobalPlayerRanking where ( realmid <0 or  realmid in (0,13,21))
 group by  DATEDIFF(hour, realmopenon, realmcloseon)
