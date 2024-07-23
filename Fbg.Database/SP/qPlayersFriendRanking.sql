 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayersFriendRanking')
	BEGIN
		DROP  Procedure  qPlayersFriendRanking
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE proc [dbo].qPlayersFriendRanking
as

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

 select  P2.PlayerID
		,P2.Name
		, isnull(C.ClanID,0)
		,C.Name
		, count(distinct PF.FriendPlayerID) as 'NumOfFriends'
		, count(distinct V.VillageID) as NumberOfVillages 
		, sum(cast(V.points as bigint)) as 'Total Points'
		, sum(cast(V.points as bigint)) / count(distinct PF.FriendPlayerID) 'AveragePointsPerFriend'
		from Players P2
		left join playersfriends PF
			on PF.PlayerID = P2.Playerid
 		left join players P 
			on PF.FriendPlayerID = P.PlayerID
		left join Villages V 
			on P.PlayerID = V.OwnerPlayerID 
		left Join ClanMembers CM on CM.PlayerID=P2.PlayerID
		left Join Clans C on C.ClanID=CM.ClanID
		where P.points > 0
		group by  P2.PlayerID, P2.Name, C.ClanID,C.Name
		order by 'NumOfFriends' Desc