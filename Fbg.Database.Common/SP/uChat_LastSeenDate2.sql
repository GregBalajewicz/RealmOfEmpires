
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uChat_LastSeenDate2')
	BEGIN
		DROP Procedure uChat_LastSeenDate2
	END

GO

CREATE PROCEDURE uChat_LastSeenDate2
	@UserId uniqueidentifier,
	@PlayerId int = null,
	@GroupId uniqueidentifier,
	@LastSeenDate datetime
AS

BEGIN
	update UsersToChats2 set LastSeenMsg = @LastSeenDate where UserId = @UserId 
	and ((@PlayerId is null and PlayerId is null) or PlayerId = @PlayerId)
	and GroupId = @GroupId
END