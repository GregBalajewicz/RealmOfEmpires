
if exists (select * from sysobjects where type = 'P' and name = 'qAllPlayerVillageClaims_OfOnePlayer')
begin
	drop procedure qAllPlayerVillageClaims_OfOnePlayer
end
go

create Procedure qAllPlayerVillageClaims_OfOnePlayer
	@LoggedInPlayerID int
	,@ClanID int
	,@ClaimedPlayerID int
	
as


declare @SecurityLevel int 
select @SecurityLevel = SettingInt from PlayerVillageClaims_ClanSetting where clanid = @ClanID AND SettingID = 1 
set @SecurityLevel = isnull(@SecurityLevel, 0)

declare @limitClaimsToOnlyThisPlayerID int
SET @limitClaimsToOnlyThisPlayerID = null

if @SecurityLevel <> 0 BEGIN
	-- Make sure the player has the role requested 
	if not exists (select * from PlayerInRoles where RoleID in (select roleID from SecurityLevelToRoles where SecurityLevel = @SecurityLevel) and PlayerInRoles.ClanID=@ClanID and PlayerID=@LoggedInPlayerID)  BEGIN 
		SET @limitClaimsToOnlyThisPlayerID = @LoggedInPlayerID
	END 
END


declare @ExpiresInH int
select @ExpiresInH = SettingInt from PlayerVillageClaims_ClanSetting where clanid = @ClanID and settingid = 3
set @ExpiresInH = isnull(@ExpiresInH, 99999) 


select ClaimingP.PlayerID,
	ClaimingP.Name,
	ClaimedVID ,
	TimeOfClaim,
	ClaimedV.Name as ClaimedVillageName, 
	ClaimedV.XCord as ClaimedVillageX, 
	ClaimedV.YCord as ClaimedVillageY, 
	ClaimedP.Name as ClaimedVillageOwnerName,
	ClaimedP.PlayerID as ClaimedVillageOwnerID,
	C.Name as ClaimedVillageOwnerClanName
	from players ClaimedP
	join villages ClaimedV 
		on ClaimedV.OwnerPlayerID = ClaimedP.PlayerID
	left join ClanMembers CM 
		on CM.PlayerID = ClaimedP.PlayerID
	left join Clans C
		on C.ClanID = cm.ClanID
	left join PlayerVillageClaims PVC 
		on PVC.ClaimedVID = ClaimedV.VillageID
	left join Players ClaimingP
		on ClaimingP.PlayerID = PVC.PlayerID
	
	where 
		(pvc.ClanID=@ClanID or pvc.ClanID is null)
		and (ClaimedP.PlayerID = @ClaimedPlayerID or ClaimedP.PlayerID is null)
		and dateadd(hour, @ExpiresInH, TimeOfClaim) > getdate()
		and ( PVC.playerID = @limitClaimsToOnlyThisPlayerID OR @limitClaimsToOnlyThisPlayerID is null)

	order by ClaimedV.XCord, ClaimedV.YCord
