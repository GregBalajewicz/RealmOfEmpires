IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iBlockUser2')
	BEGIN
		DROP Procedure iBlockUser2
	END

GO

CREATE PROCEDURE iBlockUser2
	@userId as uniqueidentifier,
	@playerId as int = null,
	@blockUserId as uniqueidentifier = null,
	@blockPlayerId as int = null
AS

if @blockUserId is null begin
	if not exists (select UserId from ChatMsgsBlockedUsers2 where PlayerId = @playerId and BlockedPlayerId = @blockPlayerId) begin
		insert into ChatMsgsBlockedUsers2 values (@userId, @playerId, (select UserId from vPlayers where PlayerID = @blockPlayerId), @blockPlayerId)
	end
end
else begin
	if not exists (select UserId from ChatMsgsBlockedUsers2 where UserId = @userId and PlayerId is null and BlockedUserId = @blockUserId and BlockedPlayerId is null) begin
		insert into ChatMsgsBlockedUsers2 values (@userId, @playerId, @blockUserId, @blockPlayerId)
	end
end

