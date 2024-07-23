IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qAllBlockedFromUsers')
	BEGIN
		DROP Procedure qAllBlockedFromUsers
	END

GO

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qAllBlockedUsers')
	BEGIN
		DROP Procedure qAllBlockedUsers
	END

GO

CREATE PROCEDURE qAllBlockedUsers
@realmID as nvarchar(256) = null
AS

BEGIN
	--when we want to get all the blocked users in every realm
	if @realmID is null begin
		select bUsers.UserId, (case when u.GlobalPlayerName is null then p.Name else u.GlobalPlayerName end) as BlockedPlayerName, bUsers.RealmID from ChatMsgsBlockedUsers bUsers
			left join Players p on p.UserId = bUsers.BlockedUserID and p.RealmID = bUsers.RealmID
			left join Users u on u.UserId = bUsers.BlockedUserID and bUsers.RealmID is null
		order by bUsers.RealmID, UserId
	end

	--we want to get all blocked users from a specific realm or global
	else begin
		declare @realRealmID as int = (select case when ISNUMERIC(@realmID) = 1 then CAST(@realmID as int) else null end)
		if @realRealmID is null begin
			select bUsers.UserId, u.GlobalPlayerName as BlockedPlayerName, RealmID from ChatMsgsBlockedUsers bUsers
				inner join Users u on u.UserId = bUsers.BlockedUserID
			where bUsers.RealmID is null order by bUsers.UserId
		end
		else begin
			select bUsers.UserId, p.Name as BlockedPlayerName, bUsers.RealmID from ChatMsgsBlockedUsers bUsers
				inner join Players p on p.UserId = bUsers.BlockedUserID
			where bUsers.RealmID = @realRealmID order by bUsers.UserId
		end	
	end
END