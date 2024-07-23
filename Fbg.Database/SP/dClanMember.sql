IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dClanMember')
	BEGIN
		DROP  Procedure  dClanMember
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--
-- this procedure will delete the clan if last member of a clan is deleted; which will only happen if @ForceDelete=1 since last member must be an ownet
--
-- RETURN value of 1 means player was not deleted because he is the only owner. Value > 1 means delete was successful
CREATE Procedure [dbo].[dClanMember]
		@ClanID int,
		@loggedInPlayerID int, -- player ID of the person deleting the member. This is used to check the person 
		    -- has necessary permissions to delete this member. This is only used then @IsLeave=0 
		@PlayerID int,
		@IsLeave bit, -- 0 means thie player is being deleted or dimissed from clan. 1 means the player if leaving on his own, ie, chose to leave the clan
		@ReturnValue int output,-- return value of -1 means @IsLeave=0 and loggedin player is not an owner and player to be discussed is an owner
								-- return value of 0 means  @IsLeave=0 and loggedin player not owner or admin
								-- retun value of 1 means player was not deleted because he is the only owner. 
								-- return value of 2 means delete was successful
		@ForceDelete bit =0 -- BE CAREFUL WITH THIS. ALWAYS LEAVE SET TO 0 unless you know what you are doing and understood the code below
AS

begin try 
	begin tran
		
		declare @Count as int;
		declare @ReportID as int;
		declare @ClanName as nvarchar(max);
		declare @EventMessage as nvarchar(max);
		declare @PlayerName as nvarchar(max);
		declare @LoggedInPlayerName as nvarchar(max);
		declare @EventTime as datetime;
		
		declare @IsLoggedinPlayerAnOwner as bit 
		declare @IsLoggedinPlayerAnAdmin as bit 
		declare @IsPlayerToBeDismissedAnAdmin as bit
		
		SET @ReturnValue = 2 -- default value meaning all is well.
		 
		--
		-- get roles of the player in question
		--
		SET @IsLoggedinPlayerAnOwner = 0;
		if exists(select * from PlayerInRoles where PlayerInRoles.RoleID=0 and PlayerInRoles.ClanID=@ClanID and PlayerID=@loggedInPlayerID) begin
			SET @IsLoggedinPlayerAnOwner = 1;
		END 
		
		SET @IsLoggedinPlayerAnAdmin = 0;
		if exists(select * from PlayerInRoles where PlayerInRoles.RoleID=3 and PlayerInRoles.ClanID=@ClanID and PlayerID=@loggedInPlayerID) begin
			SET @IsLoggedinPlayerAnAdmin = 1;
		END 
		
		SET @IsPlayerToBeDismissedAnAdmin = 0;
		if exists(select * from PlayerInRoles where PlayerInRoles.RoleID=0 and PlayerInRoles.ClanID=@ClanID and PlayerID=@PlayerID) begin
			SET @IsPlayerToBeDismissedAnAdmin = 1;
		END 
		
		--
		-- if this is the loggedin player dismissing someone else
		--
		IF @IsLeave = 0 BEGIN
		
			IF @IsPlayerToBeDismissedAnAdmin = 1 BEGIN 
				--
				-- only onwers can dismiss other owners
				--
				IF @IsLoggedinPlayerAnOwner <> 1  BEGIN
					SET @ReturnValue = -1
				END					
			END ELSE BEGIN
				--
				-- only owners and admins can dismiss players
				--
				IF NOT(@IsLoggedinPlayerAnOwner = 1 OR @IsLoggedinPlayerAnAdmin = 1) BEGIN
					SET @ReturnValue = 0
				END
			END
		END 
		
		
		IF @ReturnValue = 2 BEGIN 
		
			if @IsPlayerToBeDismissedAnAdmin = 1 begin 			
				--
				-- ** This player is an owner ** so we must do some extra stuff
				--
				-- see how many owners there are in the clan
				--
				select @Count =count(PlayerInRoles.RoleID) 
					FROM  PlayerInRoles
					where PlayerInRoles.RoleID=0 -- owner
					and PlayerInRoles.ClanID=@ClanID	
				
				--
				-- ok, we found that there is only one owner, and we want to delet him. 
				--	if we are asking for a ForceDelte, this means we try to make other members owners and then delete the member.
				--	if this member is the last member of the clan, we will delete the clan. 
				--	
				IF @Count = 1 AND @ForceDelete = 1 BEGIN
					--
					--	lets try to make all Admins Owners. 
					--
					update PlayerInRoles  set RoleID=0 where RoleID=3 and ClanID=@ClanID and PlayerID<>@PlayerID--update administrators to owners				
					if @@rowcount = 0 begin--update fails mean no adminstrators
						--
						-- no Admins, so, we will make EVERYONE owners
						--	First lets add every member to our role table the does not have a role
						--	then lets make everyone owners
						--
						insert into PlayerInRoles (PlayerID, RoleID, ClanID)
							select playerid,0,@ClanID
							 from clanmembers
								where PlayerID not in(select playerid from PlayerInRoles where clanid=@ClanID)
								and ClanID = @ClanID
								
						--update all members to owners
						update PlayerInRoles 
							set RoleID=0 
							where ClanID=@ClanID 
							and PlayerID<> @PlayerID 
							and not exists (select * from PlayerInRoles PR where ClanID=@ClanID and PR.PlayerID = PlayerID and RoleID = 0)
					end
					SET @Count = 2 -- say that there are at least 2 owners in this clan now and the player will be deleted
				END 
				
				--
				-- OK, if at this point we still got one onwer, then we do not proceeed with the deletion
				--
				IF @Count <= 1 BEGIN
					SET @ReturnValue = 1
				END 
			end 
			
			--
			-- if return value still says all is well, that means go ahead and delete the member
			--		
			IF @ReturnValue = 2 BEGIN
			
				delete from PlayerInRoles where ClanID = @ClanID and PlayerID=@PlayerID
				delete from ClanMembers where ClanID = @ClanID and PlayerID=@PlayerID
				delete from PlayerVillageClaims where ClanID = @ClanID and PlayerID=@PlayerID

				--
				-- get some info about the player and clan 
				select @PlayerName=Name from Players where PlayerID=@PlayerID;
				select @LoggedInPlayerName=Name from Players where PlayerID=@loggedInPlayerID;
				set @EventTime=getdate()
				select @ClanName=Name from clans where ClanID=@ClanID;
				
				if @IsLeave=0 begin		
					--
					-- someone is deleting this player 
					--
					-- report
					insert into Reports values (getdate(),dbo.Translate('dClanMembers_beenDismissed')+@ClanName, 4,dbo.Translate('dClanMembers_beenDismissed')+@ClanName)
					set @ReportID = SCOPE_IDENTITY()
					insert into ReportAddressees (PlayerID, ReportID, ForwardedByPlayerID, ForwardedOn, IsViewed) 
						values (@PlayerID, @ReportID, null, null, 0)				
					update Players set NewReportIndicator = 1 where PlayerID = @PlayerID
					
					-- handle clan events
					set @EventMessage=@LoggedInPlayerName + dbo.Translate('dClanMembers_dismissed') + @PlayerName +dbo.Translate('dClanMembers_fromClan');
					exec iClanEvents @ClanID,@EventTime,@EventMessage						
				end  else begin
					--
					-- player is leaving on his won 
					--
					
					-- handle clan events
					set @EventMessage=@PlayerName+ dbo.Translate('dClanMembers_hasLeft');
					exec iClanEvents @ClanID,@EventTime,@EventMessage
				end

				-- remove player from any possible clan raids
				delete ClanRaids where playerid = @PlayerID
			END 
			--
			-- if no members left, delete the clan 
			--
			IF not exists (SELECT * FROM clanmembers WHERE ClanID = @ClanID) BEGIN
				exec dClan @ClanID 
			END
		END 
		--
		-- return value. 
		--		
		select @ReturnValue
		
	commit tran
end try


begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK


	SET @ERROR_MSG = 'dClanMember FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @ClanID' + ISNULL(CAST(@ClanID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @IsLeave' + ISNULL(CAST(@IsLeave AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @Count' + ISNULL(CAST(@Count AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ReportID' + ISNULL(CAST(@ReportID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @ClanName' + ISNULL(CAST(@ClanName AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @EventMessage' + ISNULL(CAST(@EventMessage AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @PlayerName' + ISNULL(CAST(@PlayerName AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @EventTime' + ISNULL(CAST(@EventTime AS VARCHAR(max)), 'Null') + CHAR(10)		
		+ '   @LoggedInPlayerName' + ISNULL(CAST(@LoggedInPlayerName AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @IsLoggedinPlayerAnOwner' + ISNULL(CAST(@IsLoggedinPlayerAnOwner AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @IsLoggedinPlayerAnAdmin' + ISNULL(CAST(@IsLoggedinPlayerAnAdmin AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @IsPlayerToBeDismissedAnAdmin' + ISNULL(CAST(@IsPlayerToBeDismissedAnAdmin AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ReturnValue' + ISNULL(CAST(@ReturnValue AS VARCHAR(max)), 'Null') + CHAR(10)


		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	
 

 
begin try 
	if @ReturnValue > 1 begin
		declare @RealmID int = (select AttribValue from RealmAttributes where AttribID = 33)
		exec FBGC.fbgcommon.dbo.dChat_UsersToChats2 @PlayerId = @PlayerID, @RealmID = @RealmID, @ClanID = @ClanID
	end
end try


begin catch
	DECLARE @ERROR_MSG2 AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK


	SET @ERROR_MSG2 = 'dClanMember -- DELETE FROM CHAT - FAILED ! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @ClanID' + ISNULL(CAST(@ClanID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @IsLeave' + ISNULL(CAST(@IsLeave AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @Count' + ISNULL(CAST(@Count AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ReportID' + ISNULL(CAST(@ReportID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @ClanName' + ISNULL(CAST(@ClanName AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @EventMessage' + ISNULL(CAST(@EventMessage AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @PlayerName' + ISNULL(CAST(@PlayerName AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @EventTime' + ISNULL(CAST(@EventTime AS VARCHAR(max)), 'Null') + CHAR(10)		
		+ '   @LoggedInPlayerName' + ISNULL(CAST(@LoggedInPlayerName AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @IsLoggedinPlayerAnOwner' + ISNULL(CAST(@IsLoggedinPlayerAnOwner AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @IsLoggedinPlayerAnAdmin' + ISNULL(CAST(@IsLoggedinPlayerAnAdmin AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @IsPlayerToBeDismissedAnAdmin' + ISNULL(CAST(@IsPlayerToBeDismissedAnAdmin AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ReturnValue' + ISNULL(CAST(@ReturnValue AS VARCHAR(max)), 'Null') + CHAR(10)


		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG2,11,1)	

end catch	