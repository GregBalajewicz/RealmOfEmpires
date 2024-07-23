IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_OneOnOne2')
	BEGIN
		DROP Procedure qChat_OneOnOne2
	END

GO

CREATE PROCEDURE qChat_OneOnOne2
	@UserId uniqueidentifier = null,
	@PlayerId int = null,
	@OtherUserId uniqueidentifier = null,
	@OtherPlayerId int = null,
	@RealmId int = null
AS

BEGIN
	declare @GroupId uniqueidentifier
	declare @Name nvarchar(256)

	if @PlayerId is not null begin
		select @GroupId = grp.GroupId, @OtherUserId = u2c.UserId from GroupChat2 grp 
			inner join UsersToChats2 u2c on u2c.GroupId = grp.GroupId and u2c.PlayerId = @OtherPlayerId
			inner join UsersToChats2 u2c2 on u2c2.GroupId = grp.GroupId and u2c2.PlayerId = @PlayerId
		where grp.GroupType =  1
		set @Name = (select Name from vPlayers where PlayerID = @PlayerId) + ';' + (select Name from vPlayers where PlayerID = @OtherPlayerId)

		if @GroupId is null begin
			set @GroupId = NEWID()
			set @OtherUserId = (select UserId from vPlayers where PlayerID = @OtherPlayerId)
			insert into GroupChat2 values (@GroupId, @Name, @RealmId, 1, null)
			insert into UsersToChats2 select UserId, PlayerId, @GroupId, 0 from vPlayers where PlayerID = @PlayerId or PlayerId = @OtherPlayerId
		end
	end
	else begin
		select @GroupId = grp.GroupId from GroupChat2 grp
			inner join UsersToChats2 u2c on u2c.GroupId = grp.GroupId and u2c.UserId = @OtherUserId and u2c.PlayerId is null
			inner join UsersToChats2 u2c2 on u2c2.GroupId = grp.GroupId and u2c2.UserId = @UserId and u2c2.PlayerId is null
		where grp.GroupType = 1
		set @Name = (select GlobalPlayerName from users where UserId = @UserId) + ';' + (select GlobalPlayerName from users where UserId = @OtherUserId)

		if @GroupId is null begin
			set @GroupId = NEWID()
			insert into GroupChat2 values (@GroupId, @Name, @RealmId, 1, null)
			insert into UsersToChats2 select UserId, null, @GroupId, 0 from users where UserId = @UserId or UserId = @OtherUserId
		end
	end

	select @GroupId as GroupId, @Name as Name, @OtherUserId as OtherUserId
END