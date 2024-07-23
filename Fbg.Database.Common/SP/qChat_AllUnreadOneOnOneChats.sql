IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_AllUnreadOneOnOneChats')
	BEGIN
		DROP Procedure qChat_AllUnreadOneOnOneChats
	END

GO

CREATE PROCEDURE qChat_AllUnreadOneOnOneChats
	@UserId as uniqueidentifier
AS

--returns all one on one chats of this user where he or she has unread msgs.
--return includes the groupid, relamid, the other user's id, the other user's name for that realm, the last seen msg date, and the number of notifications that are unread
BEGIN
	select grp.GroupId
		, grp.RealmID
		, usr.UserId 
		, case when grp.RealmID is null then usr.GlobalPlayerName else 
			(select top 1 Name from vPlayers where UserId = usr.UserId and RealmID = grp.RealmID order by playerid ) end as Name
		, u2c.LastSeenMsg, 
	(select COUNT(*) from ChatMsgs where [DateTime] > u2c.LastSeenMsg
		and ((grp.RealmID is null and RealmID is null) or grp.RealmID = RealmID) 
		and GroupId = grp.GroupId) as Notifications

	from UsersToChats u2c 
	inner join GroupChat grp on u2c.GroupId = grp.GroupId 
	inner join UsersToChats otheru2c on otheru2c.GroupId = grp.GroupId and otheru2c.UserId != @UserId --get the other user's id
	inner join Users usr on usr.UserId = otheru2c.UserId
	where u2c.GroupId is not null and grp.IsOneOnOne = 1 and u2c.UserId = @userId
	--warning: duplicated logic here which is probably a lot less efficient
	and (select COUNT(*) from ChatMsgs where [DateTime] > u2c.LastSeenMsg
		and ((grp.RealmID is null and RealmID is null) or grp.RealmID = RealmID) 
		and GroupId = grp.GroupId) > 0
END