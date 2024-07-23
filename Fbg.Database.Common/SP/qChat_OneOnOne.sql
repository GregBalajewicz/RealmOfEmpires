IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_OneOnOne')
	BEGIN
		DROP Procedure qChat_OneOnOne
	END

GO

CREATE PROCEDURE qChat_OneOnOne
	@UserId uniqueidentifier,
	@Name nvarchar(25),
	@RealmID int = null
AS

BEGIN
	declare @OtherUserId uniqueidentifier 
	if @RealmID is null begin
		set @OtherUserId = (select UserId from Users where GlobalPlayerName = @Name)
	end
	else begin
		set @OtherUserId = (select UserId from vPlayers where Name = @Name and RealmID = @RealmID)
	end

	declare @GroupId uniqueidentifier

	if @OtherUserId is not null begin
		set @GroupId = (select GroupId from GroupChat grp where grp.IsOneOnOne = 1 and 
			((@RealmID is null and grp.RealmID is null) or grp.RealmID = @RealmID) and
			exists (select UserId from UsersToChats where UserId = @UserId and GroupId = grp.GroupId) and 
			exists (select UserId from UsersToChats where UserId = @OtherUserId and GroupId = grp.GroupId))
		if @GroupId is null begin
			set @GroupId = NEWID()
			insert into GroupChat(GroupId, RealmID) values (@GroupId, @RealmID)
			insert into UsersToChats values (@UserId, null, @GroupId, 0), (@OtherUserId, null, @GroupId, 0)
		end
	 
		select @GroupId as GroupId, @OtherUserId as OtherUserId
	end
	else begin
		select null as OtherUserId
	end
END