IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uChat_ChangeGroupName')
	BEGIN
		DROP Procedure uChat_ChangeGroupName
	END

GO

CREATE PROCEDURE uChat_ChangeGroupName
	@GroupId uniqueidentifier,
	@Name nvarchar(25)
AS

BEGIN
	update GroupChat2 set Name = @Name where GroupId = @GroupId
END