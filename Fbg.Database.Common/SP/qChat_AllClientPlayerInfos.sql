IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_AllClientPlayerInfos2')
	BEGIN
		DROP Procedure qChat_AllClientPlayerInfos2
	END

GO

CREATE PROCEDURE qChat_AllClientPlayerInfos2
	@UserId uniqueidentifier
AS

--for each player of user, return all chats associated with the player

BEGIN
	select p.PlayerID, p.RealmID, p.Name as PlayerName, 
		g.GroupId, g.Name as ChatName, g.GroupType, 
		u2c.LastSeenMsg, (select COUNT(*) from ChatMsgs2 where [DateTime] > u2c.LastSeenMsg and GroupId = g.GroupId) as Notifications
	from vPlayers p 
		left join UsersToChats2 u2c on u2c.UserId = p.UserId and u2c.PlayerId = p.PlayerId
		left join GroupChat2 g on g.GroupId = u2c.GroupId
		left join realms R on 
			P.realmid = R.RealmID 
	where p.UserId = @UserId 
		and (ActiveStatus = 1 or p.realmid is null)
	
	union
	
	select null as PlayerID, null as RealmID, u.GlobalPlayerName as PlayerName, 
			g.GroupId, g.Name as ChatName, g.GroupType, 
			u2c.LastSeenMsg, (select COUNT(*) from ChatMsgs2 where [DateTime] > u2c.LastSeenMsg and GroupId = g.GroupId) as Notifications
	from users u
		left join UsersToChats2 u2c on u2c.UserId = u.UserId and u2c.PlayerId is null
		left join GroupChat2 g on g.GroupId = u2c.GroupId
	where u.UserId = @UserId 
	
	order by RealmId, PlayerId, GroupType 
END



select * from UserSuspensions where UserID = @UserId

