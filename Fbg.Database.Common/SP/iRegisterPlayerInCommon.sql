IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iRegisterPlayerInCommon')
	BEGIN
		DROP  Procedure  iRegisterPlayerInCommon
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO
CREATE Procedure dbo.iRegisterPlayerInCommon
	@UserID uniqueidentifier ,
	@PlayerName varchar(50),
	@RealmID integer,
	@InvitationID int, -- maybe null if this registration is not a fulfillment of an invite
	@PlayerLoginType smallint

AS
DECLARE @PlayerID AS integer
DECLARE @InvitingPlayerID as integer
set @PlayerID=0;
set @InvitingPlayerID=0;
-- 
--
--
begin try 
	begin tran
		--
		-- check if this player name already exists. 
		--	do 2 checks. First to see if this playername already exists on this realm, either active or deleted (went abandoned)
		--		then to see if it exists on any other realm (active or abandoned), unless it is owned by this user
		--	3rd check makes sure there is no one else with the same user-name as 
		-- TODO: THIS IS NOT THREAD SAFE!!! 
		--
		IF     NOT EXISTS (select PlayerID from Players where RealmID=@RealmID and [Name]=@PlayerName
					 UNION select PlayerID from DeletedPlayers where RealmID=@RealmID and [Name]=@PlayerName)
		   AND NOT EXISTS (select PlayerID from Players where RealmID<>@RealmID and [Name]=@PlayerName and UserID <> @UserID
					 UNION select PlayerID from DeletedPlayers where RealmID<>@RealmID and [Name]=@PlayerName and UserID <> @UserID)
		   AND NOT EXISTS (select userID from Users where GlobalPlayerName=@PlayerName and UserID <> @UserID)
		BEGIN 
		
			-- Inserts the data in Players table.
			insert into players (RealmID, UserID, Name, AvatarID) 
			values ( @RealmID, @UserID, @PlayerName, 1)
			set @PlayerID = SCOPE_IDENTITY() 
	
			if  not exists (select * from Users where UserID = @UserID) begin 
				insert into Users(UserID, Credits) values (@UserID, 25)
			end 

			--
			-- note the login type. We do delete, then insert, to always update it with the current long type; 
			--	this is especially important, if perhaps the user did not register on a realm, then the login type was inferred based on 
			--	usernane; hence here we 'd update it to the proper login type.  
			delete UserLoginTypes where UserID = @UserID
			insert into UserLoginTypes(UserID , UserLoginTypeID) values (@UserID, @PlayerLoginType)
			
			
			--
			-- If registration is an acceptance of an invite, then register this fact
			--
			IF @InvitationID is not null BEGIN 
				IF EXISTS -- making sure this player has this invitation
					(
					select * 
						from Invites I
						join aspnet_Users U
							on I.InvitedID = U.UserName -- make sure this invite is for registering user
						where I.Type = 1 -- facebookinvite
							and I.StatusID = 1 -- pending invite
							and I.InviteID = @InvitationID -- make sure right inviteID passed
							and U.UserID = @UserID 
					)
				BEGIN
					UPDATE Invites 
						SET StatusID = 2 --this mean invitation has been accepted 
						WHERE InviteID = @InvitationID
						
					-- get the player ID of the player who invited this new player
					select  @InvitingPlayerID = PlayerID
						from Invites I
						where I.InviteID = @InvitationID 
							
				END
			END 
			
			select @PlayerID, @InvitingPlayerID
		end 
	ELSE BEGIN 
		--
		-- player name already taken!
		--
		select 0, 0
	END 
		
	commit tran
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iRegisterPlayerInCommon FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @RealmID:'				  + ISNULL(CAST(@RealmID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @UserID:'					  + ISNULL(CAST(@UserID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @PlayerName:'				  + ISNULL(@PlayerName, 'Null') + CHAR(10)
		+ '   @PlayerID:'				  + ISNULL(CAST(@PlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @InvitationID'			  + ISNULL(CAST(@InvitationID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



