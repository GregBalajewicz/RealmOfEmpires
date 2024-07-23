IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPreviousChatMessages')
	BEGIN
		DROP Procedure qPreviousChatMessages
	END

GO

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_PreviousChatMessages')
	BEGIN
		DROP Procedure qChat_PreviousChatMessages
	END

GO

CREATE PROCEDURE qChat_PreviousChatMessages
	@LastMessageDate as datetime,
	@MessagesToGet as int,
	@RealmID as int = null,
	@GroupId as uniqueidentifier = null
AS

BEGIN
	if @RealmID is null begin
		select top (@MessagesToGet) u.UserId, u.GlobalPlayerName as Name, msg.[Text], msg.[DateTime] from ChatMsgs msg
		inner join Users u on u.UserId = msg.UserId
		where ((@GroupId is null and msg.GroupId is null) or msg.GroupId = @GroupId) and
		msg.RealmID is NULL and msg.[DateTime] < @LastMessageDate order by msg.[DateTime] desc
	end
	else begin
		select top (@MessagesToGet) 
			null as UserId, ( select top 1 name from vPlayers p where p.UserId = msg.UserId and p.realmid = @RealmID order by p.playerid)  as Name
			, msg.[Text]
			, msg.[DateTime] 
		from ChatMsgs msg
		where 
			((@GroupId is null and msg.GroupId is null) or msg.GroupId = @GroupId) 
			and msg.RealmID = @RealmID 
			and msg.[DateTime] < @LastMessageDate 
		order by msg.[DateTime] desc
	end
END