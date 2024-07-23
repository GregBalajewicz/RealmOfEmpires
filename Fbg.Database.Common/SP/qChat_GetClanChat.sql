IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_GetClanChat')
	BEGIN
		DROP Procedure qChat_GetClanChat
	END

GO

CREATE PROCEDURE qChat_GetClanChat
	@RealmID int,
	@ClanID int
AS

BEGIN
	declare @GroupId uniqueidentifier
	select @GroupId = GroupId from GroupChat2 where RealmID = @RealmID and ClanID = @ClanID
	if @GroupId is null begin
		set @GroupId = NEWID()
		insert into GroupChat2 values (@GroupId, 'Clan Chat', @RealmID, 3, @ClanID)
	end
	select @GroupId as GroupId 
END