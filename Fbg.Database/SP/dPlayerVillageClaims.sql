
if exists (select * from sysobjects where type = 'P' and name = 'dPlayerVillageClaims')
begin
	drop procedure dPlayerVillageClaims
end
go

create Procedure dPlayerVillageClaims
	@DeleterPlayerID int
	,@ClanID int
	,@ClaimedVID int 
	
as

if not exists (select * from PlayerInRoles where PlayerInRoles.RoleID in (0,3) and PlayerInRoles.ClanID=@ClanID and PlayerID=@DeleterPlayerID) 
	and not exists (select * from PlayerVillageClaims PCV where PCV.ClaimedVID = @ClaimedVID and ClanID=@ClanID and PlayerID=@DeleterPlayerID) BEGIN 

	RETURN 

END 

delete PlayerVillageClaims where ClaimedVID = @ClaimedVID and ClanID=@ClanID



