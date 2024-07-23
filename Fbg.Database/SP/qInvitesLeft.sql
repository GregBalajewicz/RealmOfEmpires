IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qInvitesLeft')
	BEGIN
		DROP  Procedure  qInvitesLeft
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].qInvitesLeft
		@ClanID int,
		@InviterID int,
		@InvitesLeft int output, -- returns # of invites this player can still issue at this time; or 999,999 if there is no limit at all
		@InvitedAvailableOn DateTime output
AS
BEGIN TRY 
	declare @MaxInvitesPerPlayer as int
	declare @ClanMemberCount as int
	declare @PlayersInvitesLastDay as int
	
	--
	-- does the player have invites left ?
	--
	IF exists (select * FROM RealmAttributes WHERE AttribID between 1000 and 1010) BEGIN
		select @ClanMemberCount = count(*) from clanmembers where ClanID = @ClanID group by clanid 

		select top 1 @MaxInvitesPerPlayer = cast (AttribDesc as int) 
			FROM RealmAttributes 
			WHERE AttribID between 1000 and 1010
				AND @ClanMemberCount <= cast (AttribValue as int)
			order by cast (AttribValue as int) asc		
		
		SELECT @PlayersInvitesLastDay = count(*) 
			FROM ClanInviteLog
			where PlayerID = @InviterID
				and ClanID = @ClanID
				and InvitedOn > dateadd(day, -1, getdate()) 
		
		set @InvitesLeft = @MaxInvitesPerPlayer - @PlayersInvitesLastDay
		
		--
		-- if no invites, find out when next invite would be available - ie, grab the oldes invite in the last 24 hour period, and add 24 hours; this is when it will expire.
		--
		--IF @InvitesLeft <= 0 BEGIN
			SELECT top 1 @InvitedAvailableOn = dateadd(day, 1, InvitedOn)
				FROM ClanInviteLog
				WHERE PlayerID = @InviterID
					and ClanID = @ClanID
					and InvitedOn > dateadd(day, -1, getdate()) 
				ORDER BY InvitedOn Asc					
		--END
	END ELSE BEGIN
		SET @InvitesLeft = 999999
	END

end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qInvitesLeft FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @ClanID' + ISNULL(CAST(@ClanID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @InviterID' + ISNULL(CAST(@InviterID AS VARCHAR(25)), 'Null') + CHAR(10)
		+ '   @InvitesLeft' + ISNULL(CAST(@InvitesLeft AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @InvitedAvailableOn' + ISNULL(CAST(@InvitedAvailableOn AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



  