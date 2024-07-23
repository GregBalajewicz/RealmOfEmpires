IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_PreviousChatMessages2')
	BEGIN
		DROP Procedure qChat_PreviousChatMessages2
	END

GO



CREATE PROCEDURE qChat_PreviousChatMessages2
	@LastMessageDate as datetime,
	@MessagesToGet as int,
	@GroupId as uniqueidentifier,
	@UserId as uniqueidentifier,
	@PlayerId as int = null
AS

BEGIN
	select top (@MessagesToGet) 
		msg.UserId,
		msg.PlayerId, 
		(select case when msg.PlayerId is not null then (select Name from vPlayers where msg.PlayerId = PlayerId)
				when msg.UserId is not null then (select GlobalPlayerName from Users where msg.UserId = UserId)
				else null end) as Name
		, msg.[Text]
		, msg.[DateTime] 
		, P.AvatarID as PlayerAvatarID
		, U.AvatarID as UserAvatarID
		, UFL.Data as VIPStatusLevel
		, UFL2.Data as VIPStatusDisplay
	from ChatMsgs2 msg
	left join Players P 
		on P.PlayerID = msg.PlayerID
	join users U 
		on U.UserID = msg.UserID
	left join userflags UFL
		on UFL.UserId = msg.UserId and UFL.FlagID = 23
	left join userflags UFL2
		on UFL2.UserId = msg.UserId and UFL2.FlagID = 24
	where 
		msg.GroupId = @GroupId
		and msg.[DateTime] < @LastMessageDate
		and not exists
			(select bUser.UserId from ChatMsgsBlockedUsers2 bUser 
			where bUser.UserId = @UserId and ((@PlayerId is null and bUser.PlayerId is null) or bUser.PlayerId = @PlayerId)
			and bUser.BlockedUserId = msg.UserId and ((msg.PlayerId is null and bUser.BlockedPlayerId is null) or bUser.BlockedPlayerId = msg.PlayerId))
	order by msg.[DateTime] desc
END