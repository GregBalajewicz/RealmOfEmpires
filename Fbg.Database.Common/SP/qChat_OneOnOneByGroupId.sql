IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_OneOnOneByGroupId')
	BEGIN
		DROP Procedure qChat_OneOnOneByGroupId
	END

GO

CREATE PROCEDURE qChat_OneOnOneByGroupId
	@GroupId uniqueidentifier,
	@UserId uniqueidentifier = null,
	@PlayerId int = null
AS

BEGIN

	if @PlayerId is not null begin
		select u2c.UserId as OtherUserId, 
			((select Name from vPlayers where PlayerID = @PlayerId) + ';' + (select Name from vPlayers where PlayerID = u2c.PlayerId)) as Name,
			u2c.PlayerId as OtherId
		from GroupChat2 grp 
			inner join UsersToChats2 u2c on u2c.GroupId = grp.GroupId and u2c.PlayerId != @PlayerId
		where grp.GroupId = @GroupId
	end
	else begin
		select u2c.UserId as OtherUserId,
		((select GlobalPlayerName from users where UserId = @UserId) + ';' + (select GlobalPlayerName from users where UserId = u2c.UserId)) as Name,
		u2c.UserId as OtherId
		 from GroupChat2 grp
			inner join UsersToChats2 u2c on u2c.GroupId = grp.GroupId and u2c.UserId != @UserId
		where grp.GroupId = @GroupId
	end
END