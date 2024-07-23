IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qAllBlockedUsers2')
	BEGIN
		DROP Procedure qAllBlockedUsers2
	END

GO

CREATE PROCEDURE qAllBlockedUsers2
@UserId uniqueidentifier = null,
@PlayerId int = null
AS

BEGIN

	--when we want to get all the blocked users in every realm
	if @UserId is null and @PlayerId is null begin
		select UserId, PlayerId, BlockedUserId, BlockedPlayerId from ChatMsgsBlockedUsers2
		order by UserId, PlayerId
	end

	--we want to get all blocked users from a specific realm person or global
	else begin
		if @PlayerId is not null begin
			--select UserId, PlayerId, BlockedUserId, BlockedPlayerId from ChatMsgsBlockedUsers2
			--where PlayerId = @PlayerId
			select C.UserId, C.PlayerId, C.BlockedUserId, C.BlockedPlayerId, P.Name from ChatMsgsBlockedUsers2 C LEFT JOIN Players P on P.PlayerID = C.BlockedPlayerId 
			where C.PlayerID = @PlayerId
		end
		else begin
			--select UserId, PlayerId, BlockedUserId, BlockedPlayerId from ChatMsgsBlockedUsers2
			--where UserId = @userId and PlayerId is null
			select C.UserId, C.PlayerId, C.BlockedUserId, C.BlockedPlayerId, P.Name from ChatMsgsBlockedUsers2 C LEFT JOIN Players P on P.PlayerID = C.BlockedPlayerId 
			where C.UserId = @userId and C.PlayerId is null
		end	
	end
	
END