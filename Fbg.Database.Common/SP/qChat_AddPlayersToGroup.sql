IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_AddPlayersToGroup')
	BEGIN
		DROP Procedure qChat_AddPlayersToGroup
	END

GO

CREATE PROCEDURE qChat_AddPlayersToGroup
	@GroupId uniqueidentifier,
	@Names varchar(max),
	@UserId uniqueidentifier = null,
	@PlayerId int = null
AS

BEGIN
	--make sure not adding player to anything other than a group chat
	if (select GroupType from GroupChat2 where GroupId = @GroupId) = 2 begin
		Create table #TempUsersForGroup
		(
			Name varchar(25)
		)
		Create table #InvalidUsers
		(
			Name varchar(25),
			Error varchar(max)
		)

		declare @RealmID int = (select RealmID from GroupChat2 where GroupId = @GroupId)

		set @Names = LTRIM(RTRIM(@Names)) + ','
		declare @Pos int = CHARINDEX(',', @Names, 1)
		declare @CurrName varchar(25)

		while (@Pos > 0) begin
			set @CurrName = LTRIM(RTRIM(LEFT(@Names, @Pos - 1)))
			if @CurrName <> '' begin
				insert into #TempUsersForGroup values (@CurrName)
			end
			set @Names = RIGHT(@Names, LEN(@Names) - @Pos)
			set @Pos = CHARINDEX(',', @Names, 1)
		end

		if @RealmID is null begin
			insert into #InvalidUsers --get invalid players
				select distinct Name, Name + ' is invalid' from #TempUsersForGroup n
			where not exists (select GlobalPlayerName from Users where GlobalPlayerName = n.Name)

			insert into #InvalidUsers --get duplicate players
				select distinct u.GlobalPlayerName, u.GlobalPlayerName + ' is already in the group' from Users u
					inner join #TempUsersForGroup tempUser on tempUser.Name = u.GlobalPlayerName
				where exists (select u2c.UserId from UsersToChats2 u2c where u2c.UserId = u.UserId and u2c.GroupId = @GroupId)

			insert into #InvalidUsers --get players that blocked this player
				select distinct u.GlobalPlayerName, u.GlobalPlayerName + ' blocked you' from Users u
					inner join #TempUsersForGroup tempUser on tempUser.Name = u.GlobalPlayerName
					inner join ChatMsgsBlockedUsers2 bUser on bUser.UserId = u.UserId and bUser.PlayerId is null and bUser.BlockedUserId = @UserId

			insert into UsersToChats2 
				select distinct UserId, null, @GroupId, GETUTCDATE() from Users u
					inner join #TempUsersForGroup tempUser on tempUser.Name = u.GlobalPlayerName and 
						not exists (select invalidUser.Name from #InvalidUsers invalidUser where invalidUser.Name = tempUser.Name)

			select distinct tempUser.Name, u.UserId, u.UserId as Id from #TempUsersForGroup tempUser
				inner join Users u on u.GlobalPlayerName = tempUser.Name
			where not exists (select invalidUser.Name from #InvalidUsers invalidUser where invalidUser.Name = tempUser.Name)
		end
		else begin
			insert into #InvalidUsers --get invalid players
				select distinct Name, Name + ' is invalid' from #TempUsersForGroup n
			where not exists (select p.Name from vPlayers p where p.RealmID = @RealmID and p.Name = n.Name)

			insert into #InvalidUsers --get duplicate players
				select distinct p.Name, p.Name + ' is already in the group' from vPlayers p
					inner join #TempUsersForGroup tempUser on tempUser.Name = p.Name
				where exists (select u2c.UserId from UsersToChats2 u2c where u2c.PlayerId = p.PlayerId and u2c.GroupId = @GroupId)

			insert into #InvalidUsers --get players that blocked this player
				select distinct p.Name, p.Name + ' blocked you' from vPlayers p
					inner join #TempUsersForGroup tempUser on tempUser.Name = p.Name
					inner join ChatMsgsBlockedUsers2 bUser on bUser.PlayerId = p.PlayerID and bUser.BlockedPlayerId = @PlayerId

			insert into UsersToChats2
				select distinct UserId, PlayerId, @GroupId, GETUTCDATE() from vPlayers p
					inner join #TempUsersForGroup tempUser on tempUser.Name = p.Name and
						not exists (select invalidUser.Name from #InvalidUsers invalidUser where invalidUser.Name = tempUser.Name)
				where p.RealmId = @RealmID
			
			select distinct tempUser.Name, p.UserId, p.PlayerId as Id from #TempUsersForGroup tempUser 
				inner join vPlayers p on p.Name = tempUser.Name and p.RealmID = @RealmID
			where not exists (select invalidUser.Name from #InvalidUsers invalidUser where invalidUser.Name = tempUser.Name)
		end
		
		select distinct Error from #InvalidUsers

		drop table #TempUsersForGroup
		drop table #InvalidUsers
	end
END