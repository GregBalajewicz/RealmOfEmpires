IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iChat_CreateGroup')
	BEGIN
		DROP Procedure iChat_CreateGroup
	END

GO

CREATE PROCEDURE iChat_CreateGroup
	@Name nvarchar(256),
	@RealmId int = null,
	@UserId uniqueidentifier,
	@PlayerId int = null
AS

BEGIN
	declare @GroupId uniqueidentifier = NEWID()
	insert into GroupChat2 values (@GroupId, @Name, @RealmId, 2, null)
	insert into UsersToChats2 values (@UserId, @PlayerId, @GroupId, 0)
	select @GroupId
END