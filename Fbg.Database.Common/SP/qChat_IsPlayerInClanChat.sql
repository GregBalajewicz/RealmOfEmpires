IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_IsPlayerInClanChat')
	BEGIN
		DROP Procedure qChat_IsPlayerInClanChat
	END

GO

CREATE PROCEDURE qChat_IsPlayerInClanChat
	@PlayerID int,
	@RealmID int,
	@ClanID int
AS

BEGIN
	declare @GroupId uniqueidentifier = (select GroupId from GroupChat2 where RealmID = @RealmID and ClanID = @ClanID)
	declare @IsInChat bit
	if exists (select UserId from UsersToChats2 where GroupId = @GroupId and PlayerID = @PlayerID) begin
		set @IsInChat = 1
	end
	else begin
		set @IsInChat = 0
	end
	select @IsInChat
END