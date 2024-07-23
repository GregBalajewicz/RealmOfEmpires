
if exists (select * from sysobjects where type = 'P' and name = 'qDefinedTargetsLight')
begin
	drop procedure qDefinedTargetsLight
end
go

create Procedure qDefinedTargetsLight
	@PlayerID int
as

declare 	@ClanID int
select @ClanID = clanID from ClanMembers CM2 where CM2.PlayerID = @PlayerID

	select 
		DefinedTargetID		,    
		DT.PlayerID callerPlayerID			,
		V.VillageID			,
		V.Xcord				,
		V.YCord				,
		DefinedTargetTypeID	
		INTO #D
		FROM DefinedTargets DT
		JOIN Villages V on 
			V.VillageID = DT.VillageID 
		where 
		ExpiresOn >  getdate()

			AND ( 
		
				DT.playerid in (select cm1.playerid from clanmembers CM1 where clanid = @ClanID) 
				OR DT.playerID = @playerID
				or DT.PlayerID in 
					( select cm2.playerid from clanmembers CM2 where clanid in 
						( 
							-- get all clans, that and I set as allies, that also set me as an ally
							select ClanID as ClanIDOfOtherClanSharingClaimsWithMe from ClanDiplomacy CD
								where 
									clanid in (
										-- select all clans, that i am allied with
										select OtherClanID from ClanDiplomacy  where clanid = @ClanID and StatusID = 0 				
										) 
								and OtherClanID=@ClanID  
								and StatusID = 0
								--and exists(select * from PlayerVillageClaims_ClanSetting S where S.ClanID = CD.ClanID and SettingID =4 and SettingInt = 1)
						) 
					)		
				)

select 
		DefinedTargetID		,    
		callerPlayerID		,
		VillageID			,
		Xcord				,
		YCord			
 from #D

select  
	D.DefinedTargetID	,   
	ResponseTypeID	, 
	count(*) as count
	
	from #D D 
		join DefinedTargetResponses R
			on D.DefinedTargetID = R.DefinedTargetID			
		
		group by D.DefinedTargetID, ResponseTypeID
		order by D.DefinedTargetID
		
		
