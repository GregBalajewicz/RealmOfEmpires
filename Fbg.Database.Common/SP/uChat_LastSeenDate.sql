IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uLastSeenDate')
	BEGIN
		DROP Procedure uLastSeenDate
	END

GO

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uChat_LastSeenDate')
	BEGIN
		DROP Procedure uChat_LastSeenDate
	END

GO

CREATE PROCEDURE uChat_LastSeenDate
	@UserId uniqueidentifier,
	@RealmId int = null,
	@GroupId uniqueidentifier = null,
	@LastSeenDate datetime
AS

BEGIN
	update UsersToChats set LastSeenMsg = @LastSeenDate where UserId = @UserId 
	and ((@GroupId is null and GroupId is null and ((@RealmId is null and RealmID is null) or RealmID = @RealmId)) 
	or GroupId = @GroupId)
END