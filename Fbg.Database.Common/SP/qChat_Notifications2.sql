
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_Notifications2')
	BEGIN
		DROP Procedure qChat_Notifications2
	END

GO

CREATE PROCEDURE qChat_Notifications2
	@GroupId uniqueidentifier,
	@UserId uniqueidentifier,
	@PlayerId int = null
AS

BEGIN
	declare @lastSeenDate datetime = (select LastSeenMsg from UsersToChats2 where UserId = @UserId and GroupId = @GroupId and ((@PlayerId is null and PlayerId is null) or PlayerId = @PlayerId))
	if @lastSeenDate is null begin --if the row does not exist yet, insert a new row
		insert into UsersToChats2 values (@UserId, @PlayerId, @GroupId, GETUTCDATE())
		set @lastSeenDate = 0
	end
	select @lastSeenDate as LastSeenDate, COUNT(*) as Notifications from ChatMsgs2 where [DateTime] > @lastSeenDate and GroupId = @GroupId
END