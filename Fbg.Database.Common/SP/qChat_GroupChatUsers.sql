IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_GroupChatUsers')
	BEGIN
		DROP Procedure qChat_GroupChatUsers
	END

GO

CREATE PROCEDURE qChat_GroupChatUsers
	@GroupId uniqueidentifier,
	@UserId uniqueidentifier
AS

BEGIN
--need to make sure userid is in this group
	if exists (select UserId from UsersToChats2 where UserId = @UserId and GroupId = @GroupId) begin
		declare @RealmId int = (select RealmId from GroupChat2 where GroupId = @GroupId)
		if (@RealmId is null) begin
			select u.UserId, u.GlobalPlayerName as Name, u.UserId as Id from UsersToChats2 u2c
				inner join Users u on u.UserId = u2c.UserId
			where u2c.GroupId = @GroupId
		end
		else begin
			select u2c.UserId, p.Name as Name, p.PlayerId as Id from UsersToChats2 u2c
				inner join vPlayers p on p.PlayerId = u2c.PlayerId
			where u2c.GroupId = @GroupId
		end
	end
	else begin
		select top 0 * from UsersToChats2
	end
END