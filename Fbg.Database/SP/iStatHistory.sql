  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iStatHistory')
	BEGIN
		DROP  Procedure  iStatHistory
	END

GO
 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].iStatHistory		
AS

	declare @RealmID int
	select @RealmID = AttribValue from RealmAttributes where AttribID = 33
	--
	-- if realm is not opened, then we do nothing
	--
	if exists ( select * from FBGC.fbgcommon.dbo.realms where realmid = @RealmID and endson < getdate()  ) BEGIN 
		select 'realm ended. abandon'
		return
	END 
	if not exists ( select * from realm  where openon <= getdate()) BEGIN 
		select 'realm not yet opened. abandon'
		return
	END 


	select P.PlayerID
		, count(distinct V.VillageID) as NumberOfVillages 
		, sum(v.points) as 'TotalPoints'
		, isnull((select StatValue from PlayerStats where statid = 1 and playerid = P.PlayerID ),0) as PointsAsAttacker
		, isnull((select StatValue from PlayerStats where statid = 2 and playerid = P.PlayerID ),0) as PointsAsDefender
		, isnull((select StatValue from PlayerStats where statid = 3 and playerid = P.PlayerID ),0) as GovKilledAsDefender		
	into #ptemp
	from players P 
	join Villages V 
		on P.PlayerID = V.OwnerPlayerID 
	left Join ClanMembers CM 
		on CM.PlayerID=P.PlayerID
	where P.PlayerID not in (select PlayerID from SpecialPlayers ) -- exclude special players
	group by P.Name,P.PlayerID, P.TitleID, P.sex, P.points

	declare @Date datetime

	set @Date = getdate()

	insert into playerstathistory(PlayerID, Date, StatID, StatValue) select playerid, @date, 1, numberofvillages from #ptemp
	insert into playerstathistory(PlayerID, Date, StatID, StatValue) select playerid, @date, 2, TotalPoints from #ptemp
	insert into playerstathistory(PlayerID, Date, StatID, StatValue) select playerid, @date, 3, PointsAsAttacker from #ptemp
	insert into playerstathistory(PlayerID, Date, StatID, StatValue) select playerid, @date, 4, PointsAsDefender from #ptemp
	insert into playerstathistory(PlayerID, Date, StatID, StatValue) select playerid, @date, 5, GovKilledAsDefender from #ptemp




	select 
		c.clanid
		, (select count(distinct ownerplayerid) from villages V where ownerplayerid in (select playerid from ClanMembers where ClanId = C.ClanId))as NumOfPlayers
		, (select count(*) from villages V where ownerplayerid in (select playerid from ClanMembers where ClanId = C.ClanId))as NumOfVillages
		, (
			select  
				isnull(sum(v.points),0) as point
			from villages v 
			where ownerplayerid in (select playerid from ClanMembers where ClanId = C.ClanId)
		) as points
		, (
			select  
				isnull(sum(ps.StatValue),0) as point
			from PlayerStats ps 
			where ps.PlayerID in (select distinct ownerplayerid from villages V where ownerplayerid in (select playerid from ClanMembers where ClanId = C.ClanId)
				)
				and ps.StatID = 1

		) as AttPoints
		, (
			select  
				isnull(sum(ps.StatValue),0) as point
			from PlayerStats ps 
			where ps.PlayerID in (select distinct ownerplayerid from villages V where ownerplayerid in (select playerid from ClanMembers where ClanId = C.ClanId)	)
				and ps.StatID = 2

		) as DefPoints
		into #ctemp
	from clans c  
	join ClanMembers  cm 
		on c.Clanid = Cm.clanid
	join Players P 
		on P.PlayerID = CM.PlayerID 
	where P.Points > 0 
	group by c.clanid, c.name

	insert into clanstathistory(ClanID, Date, StatID, StatValue) select clanid, @date, 1, NumOfPlayers from #ctemp
	insert into clanstathistory(ClanID, Date, StatID, StatValue) select clanid, @date, 2, NumOfVillages from #ctemp
	insert into clanstathistory(ClanID, Date, StatID, StatValue) select clanid, @date, 3, points from #ctemp
	insert into clanstathistory(ClanID, Date, StatID, StatValue) select clanid, @date, 4, AttPoints from  #ctemp
	insert into clanstathistory(ClanID, Date, StatID, StatValue) select clanid, @date, 5, DefPoints from #ctemp
