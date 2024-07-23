
if exists (select * from sysobjects where type = 'P' and name = 'qAllPlayerVillageClaims_ByOnePlayer')
begin
	drop procedure qAllPlayerVillageClaims_ByOnePlayer
end
go

create Procedure qAllPlayerVillageClaims_ByOnePlayer
	@LoggedInPlayerID int
	,@ClanID int
	,@ClaimingPlayerID int
	
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
		and pvc.PlayerID = @ClaimingPlayerID
		and ( PVC.playerID = @limitClaimsToOnlyThisPlayerID OR @limitClaimsToOnlyThisPlayerID is null)
		and  dateadd(hour, @ExpiresInH, TimeOfClaim) > getdate()

	order by C.ClanID desc, ClaimedP.PlayerID, ClaimedV.XCord, ClaimedV.YCord
