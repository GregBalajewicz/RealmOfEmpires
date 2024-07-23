IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_DoesClanChatExist')
	BEGIN
		DROP Procedure qChat_DoesClanChatExist
	END

GO

CREATE PROCEDURE qChat_DoesClanChatExist
	@RealmID int,
	@ClanID int
AS

BEGIN
	declare @GroupId uniqueidentifier
	declare @DoesExist bit
	if exists (select GroupId from GroupChat2 where RealmID = @RealmID and ClanID = @ClanID) begin
		set @DoesExist = 1
	end
	else begin
		set @DoesExist = 0
	end
	select @DoesExist
END