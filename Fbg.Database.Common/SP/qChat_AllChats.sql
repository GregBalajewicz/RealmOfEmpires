IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_AllChats')
	BEGIN
		DROP Procedure qChat_AllChats
	END

GO

CREATE PROCEDURE qChat_AllChats
	@UserId uniqueidentifier = null,
	@PlayerId int = null
AS

--for each player of user, return all chats associated with the player

BEGIN
	if @UserId is not null begin
		select u2c.GroupId, u2c.PlayerId, grp.RealmID, p.Name as PlayerName, grp.Name as ChatName,
		(select COUNT(*) from ChatMsgs2 where [DateTime] > u2c.LastSeenMsg and GroupId = u2c.GroupId) as Notifications,
		(select MAX(DateTime) from ChatMsgs2 where GroupId = u2c.GroupId) as LastMsgDate,
		(select R.Name) as RealmName,
		grp.GroupType
		 from UsersToChats2 u2c
			left join vPlayers p on p.PlayerId = u2c.PlayerId
			inner join GroupChat2 grp on grp.GroupId = u2c.GroupId
			left join Realms R on R.RealmID = grp.RealmID
		where u2c.UserId = @UserId
		order by (case when grp.GroupType = 0 then 0 else 1 end), LastMsgDate desc
		
	end
	else begin
		select u2c.GroupId, u2c.PlayerId, grp.RealmID, p.Name as PlayerName, grp.Name as ChatName,
		(select COUNT(*) from ChatMsgs2 where [DateTime] > u2c.LastSeenMsg and GroupId = u2c.GroupId) as Notifications,
		(select MAX(DateTime) from ChatMsgs2 where GroupId = u2c.GroupId) as LastMsgDate,
		(select R.Name) as RealmName,
		grp.GroupType 
		 from UsersToChats2 u2c
			inner join vPlayers p on p.PlayerId = u2c.PlayerId
			inner join GroupChat2 grp on grp.GroupId = u2c.GroupId
			left join Realms R on R.RealmID = grp.RealmID
		where u2c.PlayerId = @PlayerId
		order by (case when grp.GroupType = 0 then 0 else 1 end), LastMsgDate desc
	end
END