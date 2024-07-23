IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dChat_UsersToChats2')
	BEGIN
		DROP Procedure dChat_UsersToChats2
	END

GO

CREATE PROCEDURE dChat_UsersToChats2
	@PlayerID int,
	@RealmID int,
	@ClanID int
AS

BEGIN
	declare @GroupId uniqueidentifier = (select GroupId from GroupChat2 where RealmID = @RealmID and ClanID = @ClanID)
	delete UsersToChats2 where GroupId = @GroupId and PlayerID = @PlayerID
END