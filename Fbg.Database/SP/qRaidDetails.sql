 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qRaidDetails')
	BEGIN
		DROP  Procedure  qRaidDetails
	END

GO

--Get all raid movements for a player
CREATE Procedure [dbo].qRaidDetails
	@playerID int,
	@raidID int
AS



--get clan ID if player is in a clan,
declare @ClanID int;
select @ClanID = C.ClanID from ClanMembers C where C.PlayerID = @playerid;

--TABLE 0: Raid Details
select R.RaidID, R.CreatedOn, RT.Distance, RT.RarityID, R.RaidTemplateID, RT.ActByDuration, ExpirationDate = DATEADD(minute,RT.ActByDuration,R.CreatedOn), 
PR.PlayerID, CR.PlayerID as clanRaidPlayerID, R.Size, R.PlayerCount, CR.PlayerVillageCount,
RMT.Name, RMT.[Desc], RMT.ImageUrlBG, RMT.ImageUrlIcon, RMT.CasultyPerc, RM.CurrentHealth, RM.MaxHealth, RRA.AcceptedOn 
from Raids as R
join RaidTemplate as RT on RT.RaidTemplateID = R.RaidTemplateID
join RaidMonster as RM on RM.RaidID = R.RaidID
join RaidMonsterTemplate as RMT on RMT.MonsterTemplateID = RM.MonsterTemplateID
left join PrivateRaids as PR on PR.RaidID = R.RaidID
left join ClanRaids as CR on (CR.RaidID = R.RaidID and CR.PlayerID = @playerid)
left join RaidRewardAcceptanceRecord as RRA on RRA.PlayerID = @playerid and RRA.RaidID = R.RaidID
where R.RaidID = @raidID and ((PR.PlayerID = @playerid) or (CR.PlayerID = @playerid))

--removed untill we do global raids properly
-- or (PR.PlayerID is null and CR.PlayerID is null));

--TABLE 1: player's attacks to this raid
select RM.EventID, RM.PlayerID, RM.OriginVillageID, RM.RaidID, RM.StartTime, RM.LandTime, 
V.Name as VillageName, V.XCord as VillageXCord, V.YCord  as VillageYCord from RaidUnitMovements RM
left join villages V on VillageID = RM.OriginVillageID
join Events E with(nolock) on E.EventID = RM.EventID 
 where  E.Status = 0 and RM.playerID = @playerID and RM.RaidID = @raidID;

--TABLE 2: unit details in raid attacks
select RUM.EventID, RUM.UnitTypeID, RUM.UnitCount from RaidUnitsMoving RUM
join Events E with(nolock) on E.EventID = RUM.EventID
where  E.Status = 0 and  RUM.EventID in 
(select RM.EventID from RaidUnitMovements RM where RM.playerID = @playerID and RM.RaidID = @raidID)

--TABLE 3: all raid results for this raid
select P.Name as PlayerName, RR.PlayerID, RR.LandTime, RR.DamageHP from RaidResults RR 
join players P on P.PlayerID = RR.PlayerID
left join PrivateRaids as PR on PR.RaidID = RR.RaidID
left join ClanRaids as CR on (CR.RaidID = RR.RaidID and CR.PlayerID = @playerid)
where RR.RaidID = @raidID and ((PR.PlayerID = @playerid) or (CR.PlayerID = @playerid))
order by RR.LandTime desc

--removed untill we do global raids properly
-- or (PR.PlayerID is null and CR.PlayerID is null));

--TABLE 4: all rewards for this raid
SELECT RRW.RaidTemplateID, RRW.TypeID, RRW.Count from RaidReward RRW 
left join RaidTemplate RT on RT.RaidTemplateID = RRW.RaidTemplateID
where RT.RaidTemplateID = (select R.RaidTemplateID from Raids R where R.RaidID = @raidID) 
