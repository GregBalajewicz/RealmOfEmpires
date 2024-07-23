
if exists (select * from sysobjects where type = 'P' and name = 'qDefinedTargets2')
begin
	drop procedure qDefinedTargets2
end
go

create Procedure qDefinedTargets2
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
		V.Name as Vname		, 
		P.Name as Pname		,
		TimeCreated			,
		isnull(SetTime,0)	 as SetTime,
		DefinedTargetTypeID	,
		Note,
		P_Caller.Name as callerName		, --PCaller is creator of target
		P_Caller.AvatarID as callerAvatarID,
		ExpiresOn,
		P.PlayerID as PID, --P is owner of village
		isnull((select clanID from ClanMembers PCM where PCM.PlayerID = P.PlayerID),-1) as PClanID,
		AssignedTo
		INTO #D
		FROM DefinedTargets DT
		JOIN Villages V on 
			V.VillageID = DT.VillageID 
		JOIN Players P on 
			P.PlayerID = V.OwnerPlayerID 
		JOIN Players P_Caller on
			Dt.PlayerID = P_Caller.PlayerID
		where 
			ExpiresOn >  getdate()

			AND ( 
				DT.playerid in (select cm1.playerid from clanmembers CM1 where clanid = @ClanID) 
				OR DT.playerID = @playerID

			
				--
				-- get calls for support from allies
				--
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
			-- if target assigned to a player, only show this target to this player, AND to the creator of the target
			AND ( DT.AssignedTo is null OR ( DT.AssignedTo = @PlayerID OR DT.playerID = @playerID)) 
			 
select 
		DefinedTargetID		,    
		callerPlayerID		,
		VillageID			,
		Xcord				,
		YCord				,
		Vname				, 
		Pname				, --village owner player name
		TimeCreated			,
		SetTime				,
		DefinedTargetTypeID	,
		Note				,
		callerName			,
		callerAvatarID		,
		ExpiresOn,
		PID,	--village owner player ID
		PClanID,	--village owner player clan ID
		D.AssignedTo AssignedToPlayerID,
		ATP.Name AssignedToPlayerName
 from #D  D 
	LEFT JOIN Players ATP -- assigned to player
		on ATP.PlayerID = D.AssignedTo
	

select  
	R.DefinedTargetID	,    
	R.PlayerID			,
	Name				,
	ResponseTypeID		,
	Response			,
	TimeLastUpdated	,
	P.AvatarID 
	
	from #D D 
		join DefinedTargetResponses R
			on D.DefinedTargetID = R.DefinedTargetID			
		join Players P
			on P.PlayerID = R.PlayerID
		order by D.DefinedTargetID, R.TimeLastUpdated desc
		
		
