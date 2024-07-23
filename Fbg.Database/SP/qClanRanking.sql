 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qClanRanking')
	BEGIN
		DROP  Procedure  qClanRanking
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE proc [dbo].[qClanRanking]
    @bottomLeftX int
    ,@bottomLeftY int
    ,@topRightX int
    ,@topRightY int

as

SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	select 
		c.clanid
		, c.name
		, (select count(distinct ownerplayerid) from villages V where ownerplayerid in (select playerid from ClanMembers where ClanId = C.ClanId)
				and (V.XCord >= @bottomLeftX or @bottomLeftX is null)
				and (V.YCord >= @bottomLeftY or @bottomLeftY is null)
				and (V.XCord <= @topRightX or @topRightX is null)
				and (V.YCord <= @topRightY or @topRightY is null)
		  )as NumOfPlayers
		, (select count(*) from villages V where ownerplayerid in (select playerid from ClanMembers where ClanId = C.ClanId)
				and (V.XCord >= @bottomLeftX or @bottomLeftX is null)
				and (V.YCord >= @bottomLeftY or @bottomLeftY is null)
				and (V.XCord <= @topRightX or @topRightX is null)
				and (V.YCord <= @topRightY or @topRightY is null)
		  )as NumOfVillages
		, (
			select  
				isnull(sum(v.points),0) as point
			from villages v 
			where ownerplayerid in (select playerid from ClanMembers where ClanId = C.ClanId)
				and (V.XCord >= @bottomLeftX or @bottomLeftX is null)
				and (V.YCord >= @bottomLeftY or @bottomLeftY is null)
				and (V.XCord <= @topRightX or @topRightX is null)
				and (V.YCord <= @topRightY or @topRightY is null)

		) as points
		, (
			select  
				isnull(sum(ps.StatValue),0) as point
			from PlayerStats ps 
			where ps.PlayerID in (select distinct ownerplayerid from villages V where ownerplayerid in (select playerid from ClanMembers where ClanId = C.ClanId)
				and (V.XCord >= @bottomLeftX or @bottomLeftX is null)
				and (V.YCord >= @bottomLeftY or @bottomLeftY is null)
				and (V.XCord <= @topRightX or @topRightX is null)
				and (V.YCord <= @topRightY or @topRightY is null))
				and ps.StatID = 1

		) as AttPoints
		, (
			select  
				isnull(sum(ps.StatValue),0) as point
			from PlayerStats ps 
			where ps.PlayerID in (select distinct ownerplayerid from villages V where ownerplayerid in (select playerid from ClanMembers where ClanId = C.ClanId)
				and (V.XCord >= @bottomLeftX or @bottomLeftX is null)
				and (V.YCord >= @bottomLeftY or @bottomLeftY is null)
				and (V.XCord <= @topRightX or @topRightX is null)
				and (V.YCord <= @topRightY or @topRightY is null))
				and ps.StatID = 2

		) as DefPoints
		, (select count(*) from villages V where ownerplayerid in (select playerid from ClanMembers where ClanId = C.ClanId)
				and (V.XCord >= @bottomLeftX or @bottomLeftX is null)
				and (V.YCord >= @bottomLeftY or @bottomLeftY is null)
				and (V.XCord <= @topRightX or @topRightX is null)
				and (V.YCord <= @topRightY or @topRightY is null)
				and villagetypeid >  0
		  )as NumOfBonusVillages		  
		
	from clans c  
	join ClanMembers  cm 
		on c.Clanid = Cm.clanid
	join Players P 
		on P.PlayerID = CM.PlayerID 
	where P.Points > 0 

	group by c.clanid, c.name
	order by points desc




