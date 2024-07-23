IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qUnblockUser')
	BEGIN
		DROP Procedure qUnblockUser
	END
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dUnblockUser')
	BEGIN
		DROP Procedure dUnblockUser
	END

GO

CREATE PROCEDURE dUnblockUser
	@userId as uniqueidentifier,
	@name as nvarchar(25),
	@realmID as int = null
AS

if @realmID is null begin
	delete ChatMsgsBlockedUsers where UserId = @userId and BlockedUserId = (select UserId from Users where GlobalPlayerName = @name) and realmID is null
end
else begin
	delete ChatMsgsBlockedUsers where UserId = @userId and BlockedUserId = (select UserId from Players where Name = @name) and realmID = @realmID
end