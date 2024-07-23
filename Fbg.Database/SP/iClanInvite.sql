IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iClanInvite')
	BEGIN
		DROP  Procedure  iClanInvite
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[iClanInvite]
		@ClanID int,
		@InviterID int,
		@PlayerName nvarchar(25),
		@Error int output, -- 0 means all good; 
							-- 1 means player not found; 
							-- 2 means player already in clan; 
							-- 3 player already invited
							-- 4 inviter_Donnot_have_Permssion
							-- 5 ran out of invites
		@InviteType smallint = 1,--1 means normal, user initiated invite
								--2 means auto generated clan invite from an accepted FB invite 
									-- for that, generate report but do not do turn on a new report indicator
									-- this does not count to invite limit either. 
		@MoreInvitesAvailableOn DateTime output -- only valid if @Error = 5

AS

	set @Error=0;
	
begin try 
	begin tran
		declare @ClanName as nvarchar(30);
		declare @ReportID as int;
		declare @EventMessage as nvarchar(max);
		declare @InviterName as nvarchar(25);
		declare @EventTime as datetime;
		declare @MaxInvitesPerPlayer as int
		declare @ClanMemberCount as int
		declare @PlayersInvitesLastDay as int
		declare @InvitedLeft int
		
		--get clan name to put it in the report 
		select @ClanName=Name from clans where ClanID=@ClanID;	
		
		declare @PlayerID as int
	
		SELECT    @PlayerID = Players.PlayerID 
		FROM         Players
		where Players.Name=@PlayerName;
		IF @PlayerID is null BEGIN
			set @Error  = 1--Player_Not_Found = 1
			GOTO done
		END	 
		IF exists  (select * from ClanMembers where   PlayerID = @PlayerID and ClanID=@ClanID) BEGIN	
			set @Error  = 2 --Player_Already_In_Clan = 2
			GOTO done
		END
		
		IF not exists  (select * from PlayerInRoles where   PlayerID = @InviterID and ClanID=@ClanID and (RoleID=0 or roleID=2 or roleid=3)) BEGIN	
			set @Error  = 4 --inviter_Donnot_have_Permssoin=4
			GOTO done
		END

		--
		-- does the player have invites left ?
		--
		IF @InviteType <> 2  BEGIN
			EXEC qInvitesLeft @ClanID, @InviterID, @InvitedLeft output, @MoreInvitesAvailableOn output
			
			IF @InvitedLeft <= 0 BEGIN 
				set @Error  = 5 --ran out of invites
				GOTO done				
			END
		END


		--
		-- try to do the invite
		--
		IF not exists  (select * from ClanInvites where   PlayerID = @PlayerID and ClanID=@ClanID) BEGIN	
			Insert into ClanInvites(ClanID,PlayerID,invitedOn)           
				values(@ClanID,@PlayerID,getdate())  
			goto report
			
		END else if exists  (select * from ClanInvites where   PlayerID = @PlayerID and ClanID=@ClanID and datediff(day,getdate(),dateadd(day,10,ClanInvites.InvitedOn))<=0) BEGIN	
			-- we can replace this by update statment
			--update ClanInvites set InvitedOn=getdate() where   PlayerID = @PlayerID and ClanID=@ClanID
			delete from ClanInvites where   PlayerID = @PlayerID and ClanID=@ClanID
			
			Insert into ClanInvites(ClanID,PlayerID,invitedOn)           
				values(@ClanID,@PlayerID,getdate())  
			goto report
		END ELSE begin
			set @Error=3; -- player already invited  
			GOTO done
		end
		
		report:
			IF @InviteType <> 2  BEGIN
				insert into ClanInviteLog(ClanID, PlayerID, InvitedPlayerID, InvitedOn) values (@ClanID, @InviterID, @PlayerID, getdate())
			END
		
			insert into Reports values (getdate(), dbo.Translate('iClanInvite_beenInvited')+@ClanName, 4
				,dbo.Translate('iClanInvite_toJoin')+@ClanName+'.<BR>' + replace(dbo.Translate('iClanInvite_click'), '[%clanid%]', @ClanID) + '<a href="ClanOverview.aspx">' + dbo.Translate('iClanInvite_invites') + '</a>' + dbo.Translate('iClanInvite_toSee') 
				+ '<BR>' + replace(dbo.Translate('iClanInvite_click'), '[%clanid%]', @ClanID)  + '<a href="ClanPublicProfile.aspx?ClanID='+CAST(@ClanID AS VARCHAR(20))+'">' + dbo.Translate('iClanInvite_clanOV') +'</a>' + dbo.Translate('iClanInvite_toLearn'))					
					
			set @ReportID = SCOPE_IDENTITY()
					
			insert into ReportAddressees (PlayerID, ReportID, ForwardedByPlayerID, ForwardedOn, IsViewed) 
				values (@PlayerID, @ReportID, null, null, 0)	
				
			IF @InviteType <> 2 BEGIN
				update Players set NewReportIndicator = 1 where PlayerID = @PlayerID
			END 
				
			set @Error=0;  --sucess   
			-- handle clan events
			select @InviterName=Name from Players where PlayerID=@InviterID;
			set @EventMessage=@InviterName+ dbo.Translate('iClanInvite_invited') +@PlayerName;
			set @EventTime=getdate()
			
			exec iClanEvents @ClanID,@EventTime,@EventMessage
			
		done:
			commit tran
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iClanInvite FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @ClanID' + ISNULL(CAST(@ClanID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PlayerName' + ISNULL(CAST(@PlayerName AS VARCHAR(25)), 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @Error' + ISNULL(CAST(@Error AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @ClanName' + ISNULL(CAST(@ClanName AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @ReportID' + ISNULL(CAST(@ReportID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @EventMessage' + ISNULL(CAST(@EventMessage AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @EventTime' + ISNULL(CAST(@EventTime AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @MaxInvitesPerPlayer' + ISNULL(CAST(@MaxInvitesPerPlayer AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @ClanMemberCount' + ISNULL(CAST(@ClanMemberCount AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PlayersInvitesLastDay' + ISNULL(CAST(@PlayersInvitesLastDay AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @InvitedLeft' + ISNULL(CAST(@InvitedLeft AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @MoreInvitesAvailableOn' + ISNULL(CAST(@MoreInvitesAvailableOn AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 