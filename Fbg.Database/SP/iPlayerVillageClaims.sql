if exists (select * from sysobjects where type = 'P' and name = 'iPlayerVillageClaims')
begin
	drop procedure iPlayerVillageClaims
end
go


create Procedure iPlayerVillageClaims
	@PlayerID int
	,@ClanID int
	,@ClaimedVID int 
	
as

-- max number of claims per clan member (int)
 ----- SettingID : 2. 
 -- max claim duration (int, hours) 
 ----- SettingID : 3.
declare @ExpiresInH int
declare @maxclaims int
select @ExpiresInH = SettingInt from PlayerVillageClaims_ClanSetting where clanid = @ClanID and settingid = 3
select @maxclaims = SettingInt from PlayerVillageClaims_ClanSetting where clanid = @ClanID and settingid = 2
set @ExpiresInH = isnull(@ExpiresInH, 99999) 
set @maxclaims = isnull(@maxclaims, 99999)

-- check if # of active claims has been made, and if so, do not allow more
-- not thread safe; oh well 
if exists(select * from PlayerVillageClaims where playerid = @PlayerID and clanid = @ClanID and dateadd(hour, @ExpiresInH, TimeOfClaim) > getdate() group by playerid having count(*) >= @maxclaims) BEGIN 
	select 3 -- means max # of claims has been made. 
	RETURN;
END 

--
-- make the claim. 
--	 we first delete any expired claims
--	then we make the claim unless someone else has claimed it. 
--
-- not thread safe but at worse this will fail due to key violations
--
delete PlayerVillageClaims where clanid = @ClanID and claimedVID = @ClaimedVID and dateadd(hour, @ExpiresInH, TimeOfClaim) < getdate()

if not exists(select * from PlayerVillageClaims where playerid <> @PlayerID and clanid = @ClanID and claimedVID = @ClaimedVID) BEGIN 
	insert into PlayerVillageClaims (PlayerID, ClanID , ClaimedVID, TimeOfClaim )
		select @PlayerID, @ClanID, @ClaimedVID , getdate()
			FROM Villages V, ClanMembers CM 
			where villageid = @ClaimedVID 
				and CM.ClanID = @ClanID and CM.PlayerID = @PlayerID
END 
 
-- not claimed. so return a status; is this village claimed or not? 
if exists (select * from PlayerVillageClaims where clanid = @ClanID and ClaimedVID = @ClaimedVID and PlayerID = @PlayerID) BEGIN
	select 1 -- means the player claimed it
END ELSE IF exists (select * from PlayerVillageClaims where clanid = @ClanID and ClaimedVID = @ClaimedVID and PlayerID != @PlayerID) BEGIN
	select 2 -- means so other player claimed it
END ELSE BEGIN 
	select 0 -- means village was not claimed, and we dont know why; could be inconsistent params
END 