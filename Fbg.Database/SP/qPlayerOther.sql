IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerOther')
	BEGIN
		DROP  Procedure  qPlayerOther
	END

GO


CREATE Procedure  [dbo].[qPlayerOther]
	@PlayerID as int
	, @PlayerName as varchar(100)
	, @BasePlayerID as int
as
	begin try
		--Getting Original PlayerID if only PlayerName supplied
		if(@PlayerID is null or @PlayerID=0)
		begin
			set @PlayerID = null
			select @PlayerID=PlayerID
				from Players
				where Name=@PlayerName
		end
		--Getting Player's Name, his total point, Clan Name and Notes (given by BasePlayer)
		select P.PlayerID
			,P.Name
			, P.points as 'Total Points'
			, isnull(C.Name,'') as 'Clan.Name'
			, isnull(C.ClanID,0)
			, isnull(PN.Note,'')			
			, P.Anonymous
			, P.TitleID 
			, P.Sex
			, P.SleepModeActiveFrom
			, P.AvatarID
			, P.XP_Cached
			, (select data from playerflags where playerid = @PlayerID and flagid = 83)
			, P.UserID
			from players P
			left join ClanMembers CM
				on cm.PlayerID=p.PlayerID
			left join Clans C
				on CM.ClanID=C.ClanID
			left join PlayerNotes PN
				on PN.PlayerID=@PlayerID and PN.NoteOwnerPlayerID=@BasePlayerID
			where P.playerid=@PlayerID
				and P.playerid not in (select PlayerID from SpecialPlayers ) -- exclude special players 
				and PlayerStatus = 1			

		--Getting The link text for Invitation, on the basis of the Clan's Status, whether he has been invited or not
		SELECT Count(ClanID) as 'Invited'
			FROM ClanInvites
			WHERE PlayerID = @PlayerID
				AND ClanID = (select clanID FROM ClanMembers where PlayerID = @BasePlayerID) 
				and datediff(day,getdate(),dateadd(day,10,ClanInvites.InvitedOn))>0
		--Getting Player's villages information
		select v.VillageID
			, v.Name as Name
			,XCord
			,YCord
			,V.Points
			from villages v
			where ownerPlayerID = @PlayerID
			order by v.Name
		
		select [Text] from PlayerProfile where playerid = @playerid and  not exists (select * from playersuspensions where playerid = @playerid and SupensionID = 1)

		
		
	end try
	begin catch
		DECLARE @ERROR_MSG AS VARCHAR(8000)
		IF @@TRANCOUNT > 0 ROLLBACK
		
		SET @ERROR_MSG = 'qPlayerOther FAILED! ' +  + CHAR(10)
			+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)

			+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
			+ '   @PlayerName' + ISNULL(CAST(@PlayerName AS VARCHAR(10)), 'Null') + CHAR(10)
			+ '   @BasePlayerID' + ISNULL(CAST(@BasePlayerID AS VARCHAR(10)), 'Null') + CHAR(10)

			+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
			+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
			+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
			+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
			+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
			+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
		RAISERROR(@ERROR_MSG,11,1)	
	end catch