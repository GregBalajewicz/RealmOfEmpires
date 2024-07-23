 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerRaids')
	BEGIN
		DROP  Procedure  qPlayerRaids
	END

GO

--GET ALL RAIDS THAT APPLY TO A PLAYER (Player's Private Raids, Player's Clan Raids, and Global Raids)
CREATE Procedure [dbo].qPlayerRaids
	@playerid int
AS

--get clan ID if player is in a clan,
declare @ClanID int;
select @ClanID = C.ClanID from ClanMembers C where C.PlayerID = @playerid;


select R.RaidID, R.CreatedOn, RT.Distance, RT.RarityID, R.RaidTemplateID, RT.ActByDuration, ExpirationDate = DATEADD(minute,RT.ActByDuration, R.CreatedOn), 
PR.PlayerID, CR.PlayerID as clanRaidPlayerID, R.Size, R.PlayerCount, CR.PlayerVillageCount,
RMT.Name, RMT.[Desc], RMT.ImageUrlBG, RMT.ImageUrlIcon, RMT.CasultyPerc, RM.CurrentHealth, RM.MaxHealth, RRA.AcceptedOn
from Raids as R
join RaidTemplate as RT on RT.RaidTemplateID = R.RaidTemplateID
join RaidMonster as RM on RM.RaidID = R.RaidID
join RaidMonsterTemplate as RMT on RMT.MonsterTemplateID = RM.MonsterTemplateID
left join PrivateRaids as PR on PR.RaidID = R.RaidID
left join ClanRaids as CR on (CR.RaidID = R.RaidID and CR.PlayerID = @playerid)
left join RaidRewardAcceptanceRecord as RRA on RRA.PlayerID = @playerid and RRA.RaidID = R.RaidID
where (PR.PlayerID = @playerid or CR.PlayerID = @playerid) 
	
	-- rules for private raids
	and (PR.PlayerID is null 
		
		OR ( 
			(RM.CurrentHealth <= 0 and not exists (select * from RaidRewardAcceptanceRecord A where A.RaidID = R.RaidID)) -- beaten raids, but reward not accepted
			OR (RM.CurrentHealth > 0 and dateadd(minute, RT.SpawnRate/2, DATEADD(minute, RT.ActByDuration, R.CreatedOn)) >  getdate()) -- not beaten raids, show for a while after it expired
			)
		
		)
	
	-- rules for clan raids
	and (CR.PlayerID is null 
		
		OR (
			RM.CurrentHealth <= 0 -- beaten raids

			and ( 
					-- if not accepted the reward, keep showing. 
					not exists (select * from RaidRewardAcceptanceRecord A where A.RaidID = R.RaidID and A.playerID = @playerid) 

					-- if accepted the reward, keep showing a while after it expires
					OR ( 
						exists (select * from RaidRewardAcceptanceRecord A where A.RaidID = R.RaidID) 
						and dateadd(minute, RT.SpawnRate/2, DATEADD(minute, RT.ActByDuration, R.CreatedOn)) >  getdate()
						) 
				)
		
		) 

			-- not beaten raid 
		OR (RM.CurrentHealth > 0 and dateadd(minute, RT.SpawnRate/2, DATEADD(minute, RT.ActByDuration, R.CreatedOn)) >  getdate()) -- not beaten raids, show for a while after it expired
		
		)

order by case when DATEADD(minute, RT.ActByDuration, R.CreatedOn) <  getdate() then 1 else 0 end, DATEADD(minute, RT.ActByDuration, R.CreatedOn) asc
