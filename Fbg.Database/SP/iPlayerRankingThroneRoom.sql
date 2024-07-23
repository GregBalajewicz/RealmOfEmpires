IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iPlayerRankingThroneRoom')
	BEGIN
		DROP  Procedure  iPlayerRankingThroneRoom
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE proc [dbo].iPlayerRankingThroneRoom
as
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	--
	-- get raw stats 
	--
	select P.PlayerID PlayerID,
			isnull(Cp.Name, isnull(OriginalName, CDP.name)) Name
			, (select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 1)  as HighestNumOfVillages
			,(select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 2) as  HighestVillagePoints
			,isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 3), 0) as  PointsAsAttacker
			,isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 4), 0) as  PointsAsDefender
			,isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 5), 0) as  GovKilledAsDefender
			,isnull(C.ClanID,0) ClanID
			,C.Name ClanName
			, (select max(titleid) from PlayerTitleHistory PTH where pth.playerid = p.playerid) as TopTitleID
			, P.Sex
			, p.PlayerStatus
			, p.LastActivity
			, (select maxPoints from Titles where TitleID = (select max(titleid) from PlayerTitleHistory PTH where pth.playerid = p.playerid) -1 ) PointEstBasedOnTopTitleByTitleHistory

		into #stats 

		from players P 
		left Join ClanMembers CM 
			on CM.PlayerID=P.PlayerID
		left Join Clans C 
			on C.ClanID=CM.ClanID
		left join FBGC.Fbgcommon.dbo.players CP
			on CP.Playerid = P.PlayerID 
		left join FBGC.Fbgcommon.dbo.DeletedPlayers CDP 
			on CDP.Playerid = P.PlayerID
		where P.PlayerID not in (select PlayerID from SpecialPlayers ) -- exclude special players
		
			-- we only include players who got to at least 4000 villge points after 1 month of play. if player played less then 1 month, then we show them no matter how many points they got
			--and (isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 2), 0) > 4000 )
			--order by isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 2), 0) desc 
	
	--
	-- massage the stats a bit
	--
	select 
		PlayerID
		, Name
		, 
		isnull(
			isnull(HighestNumOfVillages,
				Ceiling( (isnull(isnull(HighestVillagePoints, PointEstBasedOnTopTitleByTitleHistory + 1), 0)) / cast((select avg(cast(points as bigint)) from villages) as real)) 
			) 
		,0)
		as HighestNumOfVillages
		--, isnull(HighestVillagePoints,0) as HighestVillagePoints
		, isnull(isnull(HighestVillagePoints, PointEstBasedOnTopTitleByTitleHistory + 1), 0) HighestVillagePoints
		, PointsAsAttacker
		, PointsAsDefender
		, GovKilledAsDefender
		, ClanID
		, ClanName
		, isnull(TopTitleID, 0) as TopTitleID
		, Sex
		, PlayerStatus
		, LastActivity
	
		into #stats2

	 from #stats
		

	--
	--
	-- READY WITH THE STATS, SO insert them into the table. 
	--
	--
	begin tran -- why? cause we dont want the table to be empty, not even for a second or we'll get empty stats cached in BLL

		delete PlayerStats_BestOfLifeStats

		insert into PlayerStats_BestOfLifeStats
			select * 
			 from #stats2
 				where TopTitleID > 3
				order by HighestVillagePoints desc 

	commit tran 

	/*

	this querry worked but was very slow due to going over a linked server FBGC... so it was scrapped in favor of a nonlinked server solution

	select P.PlayerID,
		
		(
			(
				select name as name from FBGC.Fbgcommon.dbo.players players where playerid  = p.playerid union all
				select isnull(OriginalName, name) as name from  FBGC.Fbgcommon.dbo.DeletedPlayers DP where DP.playerid = p.playerid
			)) as name
		, isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 1) ,0)as HighestNumOfVillages
		,isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 2), 0) as  HighestVillagePoints
		,isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 3), 0) as  PointsAsAttacker
		,isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 4), 0) as  PointsAsDefender
		,isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 5), 0) as  GovKilledAsDefender
		,isnull(C.ClanID,0) ClanID
		,C.Name ClanName
		, isnull((select max(titleid) from PlayerTitleHistory PTH where pth.playerid = p.playerid),1) as TopTitleID
		, P.Sex
		, p.PlayerStatus
		, p.LastActivity	
	from players P 
	left Join ClanMembers CM 
		on CM.PlayerID=P.PlayerID
	left Join Clans C 
		on C.ClanID=CM.ClanID
	where P.PlayerID not in (select PlayerID from SpecialPlayers ) -- exclude special players
		
		-- we only include players who got to at least 4000 villge points after 1 month of play. if player played less then 1 month, then we show them no matter how many points they got
		and (isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 2), 0) > 4000 )
		order by isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 2), 0) desc 
	*/

	/* WAS THIS BEFORE , but name was sometimes dummy


	select P.PlayerID
		,P.Name
		, isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 1) ,0)as HighestNumOfVillages
		,isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 2), 0) as  HighestVillagePoints
		,isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 3), 0) as  PointsAsAttacker
		,isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 4), 0) as  PointsAsDefender
		,isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 5), 0) as  GovKilledAsDefender
		,isnull(C.ClanID,0) ClanID
		,C.Name ClanName
		, isnull((select max(titleid) from PlayerTitleHistory PTH where pth.playerid = p.playerid),1) as TopTitleID
		, P.Sex
		, p.PlayerStatus
		, p.LastActivity	
	from players P 
	left Join ClanMembers CM 
		on CM.PlayerID=P.PlayerID
	left Join Clans C 
		on C.ClanID=CM.ClanID
	where P.PlayerID not in (select PlayerID from SpecialPlayers ) -- exclude special players
		
		-- we only include players who got to at least 4000 villge points after 1 month of play. if player played less then 1 month, then we show them no matter how many points they got
		and (isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 2), 0) > 4000 )
		order by isnull((select max(statvalue) from playerstathistory where playerid = p.playerid and statid = 2), 0) desc 
		*/