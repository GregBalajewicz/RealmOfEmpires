IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dChat_ClanChat')
	BEGIN
		DROP Procedure dChat_ClanChat
	END

GO

CREATE PROCEDURE dChat_ClanChat
	@RealmID int,
	@ClanID int
AS

BEGIN
	declare @GroupId uniqueidentifier = (select GroupId from GroupChat2 where RealmID = @RealmID and ClanID = @ClanID)
	delete UsersToChats2 where GroupId = @GroupId
	delete ChatMsgs2 where GroupId = @GroupId
	delete GroupChat2 where GroupId = @GroupId
END