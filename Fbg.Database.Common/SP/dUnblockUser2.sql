IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dUnblockUser2')
	BEGIN
		DROP Procedure dUnblockUser2
	END

GO

CREATE PROCEDURE dUnblockUser2
	@userId as uniqueidentifier,
	@playerId as int = null,
	@blockUserId as uniqueidentifier = null,
	@blockPlayerId as int = null
AS

if @blockUserId is null begin
	delete ChatMsgsBlockedUsers2 where PlayerId = @playerId and BlockedPlayerId = @blockPlayerId
end
else begin
	delete ChatMsgsBlockedUsers2 where UserId = @userId and PlayerId is null and BlockedUserId = @blockUserId and BlockedPlayerId is null
end