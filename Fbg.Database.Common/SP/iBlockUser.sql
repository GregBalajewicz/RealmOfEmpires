IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qBlockUser')
	BEGIN
		DROP Procedure qBlockUser
	END

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iBlockUser')
	BEGIN
		DROP Procedure iBlockUser
	END

GO

CREATE PROCEDURE iBlockUser
	@userId as uniqueidentifier,
	@name as nvarchar(25),
	@realmID as int = null
AS

if @realmID is null begin
	insert into ChatMsgsBlockedUsers values (@userId, (select UserId from Users where GlobalPlayerName = @name), @realmID)
end
else begin
	insert into ChatMsgsBlockedUsers values (@userId, (select UserId from Players where Name = @name), @realmID)
end

