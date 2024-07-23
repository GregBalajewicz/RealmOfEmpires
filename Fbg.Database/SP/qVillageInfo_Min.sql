IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qVillageInfo_Min')
	BEGIN
		DROP  Procedure  qVillageInfo_Min
	END

GO

CREATE Procedure [dbo].[qVillageInfo_Min]
	@VillageID as int,
	@X as int,
	@Y as int,
	@OwnerID as int,
	@VillageName as varchar(25)
as
select v.VillageID
	,v.Name
	,v.XCord
	,v.YCord
	, V.points as villagepoints
	, P.Name
	, P.PlayerID
	, P.Points as PlayerPoints
	, isnull(C.Name,'')
	,P.RegisteredOn,isnull(VN.Note,'')
	,C.ClanID
	, P.SleepModeActiveFrom
	, V.VillageTypeID
	, isnull(CV.VillageID, 0)
	, P.VacationModeRequestOn
	, P.VacationModeDaysUsed
	, P.XP_Cached
	, P.WeekendModetakesEffectOn
from villages v  with(nolock)
	join Players P  with(nolock) on P.PlayerID = V.OwnerPlayerID 
	left join ClanMembers CM  with(nolock) on CM.PlayerID=P.PlayerID
	left join Clans C with(nolock) on C.ClanID=CM.ClanID
	left join VillageNotes VN with(nolock) on VN.VillageID=V.Villageid and VN.NoteOwnerPlayerID=@OwnerID
    left join CapitalVillages CV with(nolock) on CV.VillageID = V.VillageID
where (v.VillageID=@VillageID or @VillageID is null) and ((v.XCord=@X or @X is null) and (v.YCord=@Y or @Y is null)and (v.Name=@VillageName or @VillageName is null)) and not(@VillageID is null and @x is null and @y is null and @VillageName is null)
	and V.OwnerPlayerID not in (select PlayerID from SpecialPlayers where type = -1) -- exclude special player roe_team		

/*CHANGE LOG
- June 15 2009
qVillageInfo_Min was deadlocking. it is doing a index scan on villages which is probably the problem but i did not see exactly what. I just added no lock
 

*/