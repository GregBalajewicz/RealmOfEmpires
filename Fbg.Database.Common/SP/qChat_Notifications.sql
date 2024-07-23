IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qNotifications')
	BEGIN
		DROP Procedure qNotifications
	END

GO

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_Notifications')
	BEGIN
		DROP Procedure qChat_Notifications
	END

GO

CREATE PROCEDURE qChat_Notifications
	@UserId uniqueidentifier,
	@RealmID int = null,
	@GroupId uniqueidentifier = null
AS

BEGIN
	declare @lastSeenDate datetime
	set @lastSeenDate = (select LastSeenMsg from UsersToChats where UserId = @UserId 
							and ((@RealmID is null and RealmID is null) or RealmID = @RealmID)
							and ((@GroupId is null and GroupId is null) or GroupId = @GroupId))
	if @lastSeenDate is null begin --if the row does not exist yet, insert a new row
		insert into UsersToChats values (@UserId, @RealmID, @GroupId, 0)
		set @lastSeenDate = 0
	end
	select @lastSeenDate as LastSeenDate, COUNT(*) as Notifications from ChatMsgs where [DateTime] > @lastSeenDate
		and ((@RealmID is null and RealmID is null) or RealmID = @RealmID)
		and ((@GroupId is null and GroupId is null) or GroupId = @GroupId)
END