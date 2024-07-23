IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dChat_KickUser')
	BEGIN
		DROP Procedure dChat_KickUser
	END

GO

CREATE PROCEDURE dChat_KickUser
	@GroupId uniqueidentifier,
	@UserId uniqueidentifier = null,
	@PlayerId int = null
AS

BEGIN
	if @UserId is null begin
		delete UsersToChats2 where GroupId = @GroupId and PlayerId = @PlayerId
		select UserId as UserId from vPlayers where PlayerId = @PlayerId
	end
	else begin
		delete UsersToChats2 where GroupId = @GroupId and UserId = @UserId
		select @UserId as UserId
	end
END