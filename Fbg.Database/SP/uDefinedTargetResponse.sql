
if exists (select * from sysobjects where type = 'P' and name = 'uDefinedTargetResponse')
begin
	drop procedure uDefinedTargetResponse
end
go

create Procedure uDefinedTargetResponse
	@DefinedTargetID int
	, @respondingPlayerID int
	, @respondTypeID smallint
	, @response varchar(max)
as
set nocount on


declare @RespondingPlayerClanID int
select @RespondingPlayerClanID = clanID from ClanMembers CM2 where CM2.PlayerID = @respondingPlayerID

--
-- we delete the , then we insert. 
--	this works as an update too. 
--
-- Note that security is maintained as the delete will only delete an existing response which, if the player had, then he has the right to remove it. 
--
delete DefinedTargetResponses where DefinedTargetID = @DefinedTargetID and PlayerID = @respondingPlayerID

IF @response is not null AND @response <> '' BEGIN

	INSERT DefinedTargetResponses (DefinedTargetID, PlayerID,ResponseTypeID,Response,TimeLastUpdated) 
		SELECT definedTargetID, @respondingPlayerID, @respondTypeID, @response, getdate() 
		FROM  DefinedTargets DT
		WHERE definedTargetID = @DefinedTargetID  				
			AND 
			(
				-- person is commenting on their own target
				( DT.PlayerID = @respondingPlayerID ) 
				
				-- person is commenting on target owned by clan member
				or ( DT.PlayerID in (select cm1.playerid from clanmembers CM1 where clanid = @RespondingPlayerClanID) ) 



				-- person is commenting on a target owned by a clan member of a mutually allied clan
				or DT.PlayerID in 
				( select cm2.playerid from clanmembers CM2 where clanid in 
					( 
						-- get all clans, that and I set as allies, that also set me as an ally
						select ClanID as ClanIDOfOtherClanSharingClaimsWithMe from ClanDiplomacy CD
							where 
								clanid in (
									-- select all clans, that i am allied with
									select OtherClanID from ClanDiplomacy  where clanid = @RespondingPlayerClanID and StatusID = 0 				
									) 
							and OtherClanID=@RespondingPlayerClanID  
							and StatusID = 0
							--and exists(select * from PlayerVillageClaims_ClanSetting S where S.ClanID = CD.ClanID and SettingID =4 and SettingInt = 1)
					) 
				)	
			
			)	
END

 EXEC qDefinedTargets2 @respondingPlayerID