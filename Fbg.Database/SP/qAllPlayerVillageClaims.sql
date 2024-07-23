
if exists (select * from sysobjects where type = 'P' and name = 'qAllPlayerVillageClaims')
begin
	drop procedure qAllPlayerVillageClaims
end
go

create Procedure qAllPlayerVillageClaims
	@LoggedInPlayerID int
	,@ClanID int
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
set @ExpiresInH = isnull(@ExpiresInH, 999999) 



select ClaimingP.PlayerID,
	ClaimingP.Name,
	ClaimedVID ,
	TimeOfClaim,
	ClaimedV.Name as ClaimedVillageName, 
	ClaimedV.XCord as ClaimedVillageX, 
	ClaimedV.YCord as ClaimedVillageY, 
	ClaimedP.PlayerID as ClaimedVillageOwnerID,
	ClaimedP.Name as ClaimedVillageOwnerName,
	ClaimedP.PlayerID as ClaimedVillageOwnerID,
	C.ClanID as ClaimedVillageOwnerClanID,
	C.Name as ClaimedVillageOwnerClanName
	
	from PlayerVillageClaims PVC 
	join Players ClaimingP
		on ClaimingP.PlayerID = PVC.PlayerID
	join villages ClaimedV 
		on ClaimedV.VillageID = pvc.ClaimedVID
	join players ClaimedP
		on ClaimedP.PlayerID = ClaimedV.OwnerPlayerID
	left join ClanMembers CM 
		on CM.PlayerID = ClaimedP.PlayerID
	left join Clans C
		on C.ClanID = cm.ClanID
	
	where pvc.ClanID=@ClanID
		and ClaimedV.OwnerPlayerID != PVC.PlayerID
		and ( PVC.playerID = @limitClaimsToOnlyThisPlayerID OR @limitClaimsToOnlyThisPlayerID is null)
		and  dateadd(hour, @ExpiresInH, TimeOfClaim) > getdate()
	order by C.ClanID desc, ClaimedP.PlayerID, ClaimedV.XCord, ClaimedV.YCord


go
