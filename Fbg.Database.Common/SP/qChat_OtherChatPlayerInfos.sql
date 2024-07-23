IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_OtherChatPlayerInfos')
	BEGIN
		DROP Procedure qChat_OtherChatPlayerInfos
	END

GO

CREATE PROCEDURE qChat_OtherChatPlayerInfos
	@GroupId uniqueidentifier,
	@UserId uniqueidentifier,
	@PlayerId int = null
AS

BEGIN
	if (@PlayerId is null) begin
		select u.UserId as UserId, u.UserId as Id, u2c.LastSeenMsg, (select COUNT(*) from ChatMsgs2 where [DateTime] > u2c.LastSeenMsg and GroupId = @GroupId) as Notifications from UsersToChats2 u2c
			inner join Users u on u.UserId = u2c.UserId
		where u2c.GroupId = @GroupId and u2c.UserId != @UserId
	end
	else begin
		select p.UserId as UserId, p.PlayerId as Id, u2c.LastSeenMsg, (select COUNT(*) from ChatMsgs2 where [DateTime] > u2c.LastSeenMsg and GroupId = @GroupId) as Notifications from UsersToChats2 u2c
			inner join vPlayers p on p.PlayerId = u2c.PlayerId
		where u2c.GroupId = @GroupId and u2c.PlayerId != @PlayerId
	end
END