IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerRanking')
	BEGIN
		DROP  Procedure  qPlayerRanking
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE proc [dbo].qPlayerRanking
    @bottomLeftX int
    ,@bottomLeftY int
    ,@topRightX int
    ,@topRightY int
as
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	select P.PlayerID
		,P.Name
		, count(distinct V.VillageID) as NumberOfVillages 
		, sum(v.points) as 'Total Points'
		, (sum(v.points) / count(distinct V.VillageID)) as AveragePoints
		,isnull(C.ClanID,0)
		,C.Name
		,row_number() over (order by sum(v.points) desc) as Rank
		,P.TitleID
		, P.Sex
		, isnull((select StatValue from PlayerStats where statid = 1 and playerid = P.PlayerID ),0) as PointsAsAttacker
		, isnull((select StatValue from PlayerStats where statid = 2 and playerid = P.PlayerID ),0) as PointsAsDefender
		, isnull((select StatValue from PlayerStats where statid = 3 and playerid = P.PlayerID ),0) as GovKilledAsDefender		
		,  count(case villagetypeid when 0 then null else 1 end)  as NumBonusVillages
	from players P 
	join Villages V 
		on P.PlayerID = V.OwnerPlayerID 
	left Join ClanMembers CM 
		on CM.PlayerID=P.PlayerID
	left Join Clans C 
		on C.ClanID=CM.ClanID
	where P.PlayerID not in (select PlayerID from SpecialPlayers ) -- exclude special players
	    and (V.XCord >= @bottomLeftX or @bottomLeftX is null)
	    and (V.YCord >= @bottomLeftY or @bottomLeftY is null)
	    and (V.XCord <= @topRightX or @topRightX is null)
	    and (V.YCord <= @topRightY or @topRightY is null)
	group by P.Name,C.ClanID,C.Name,P.PlayerID, P.TitleID, P.sex, P.points
	order by 'Total Points' Desc 	