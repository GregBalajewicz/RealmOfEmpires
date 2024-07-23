
if exists (select * from sysobjects where type = 'P' and name = 'qClanClaimSysSettings')
begin
	drop procedure qClanClaimSysSettings
end
go

create Procedure qClanClaimSysSettings
	@ClanID int
	
as

select SettingID, SettingInt from PlayerVillageClaims_ClanSetting where clanid = @ClanID

