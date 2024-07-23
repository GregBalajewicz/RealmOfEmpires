
if exists (select * from sysobjects where type = 'P' and name = 'uClanClaimSysSettings')
begin
	drop procedure uClanClaimSysSettings
end
go

create Procedure uClanClaimSysSettings
	@ClanID int
	,@SettingID int
	,@SettingInt int
as

update PlayerVillageClaims_ClanSetting set settingInt = @settingInt  where clanid = @ClanID and SettingID = @SettingID
if @@ROWCOUNT = 0 BEGIN
	insert into PlayerVillageClaims_ClanSetting(clanID, settingid, settingInt) values ( @ClanID , @SettingID, @settingInt)
END

