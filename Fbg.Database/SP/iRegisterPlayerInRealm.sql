IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iRegisterPlayerInRealm')
	BEGIN
		DROP  Procedure  iRegisterPlayerInRealm
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
 RETUNRS (in @Result):
	0 -> normal result. 
	1 -> player invited to clan of inviter
	2 -> player not invited because inviter lacked permissions
	
 @InQuadrant only used 
    @InQuadrant = 1 -> North East
    @InQuadrant = 2 -> South East
    @InQuadrant = 3 -> South West
    @InQuadrant = 4 -> North West

*/
CREATE Procedure dbo.iRegisterPlayerInRealm
	@PlayerID int
	, @PlayerName varchar(50)
	, @UserID varchar(256)
	, @SexID smallint
	, @InvitingPlayerID int -- set to 0 if no inviting player
	, @InQuadrant smallint = null 
    , @VillageName	nvarchar(25) -- set to null or empty string if a default name is to be generated
    , @AvatarID smallint = 1
	, @Result int output

AS
declare @ClanID int
declare @ReturnVal int
declare @Error int
declare @MoreInvitesAvailableOn datetime -- used only as our param to call a SP 

set @ReturnVal = 0

begin try 
	begin tran

	-- if realm is already open, then registed on date is now but if it is not yet opened,
	--	ie, meaning player is preregistering, then registered on date = realmopening date so that
	--	beginner protection is good. 
	declare @RegisteredOnDate datetime
	SELECT @RegisteredOnDate = OpenOn from realm 
	if @RegisteredOnDate < GETDATE() BEGIN
		set @RegisteredOnDate = GETdate()
	END

	-- Inserts the data in Players table.
	insert into players(PlayerID, Name, REgisteredOn, UserID,Chests, Anonymous, TitleID, Sex,AvatarID) 
		select top 1 @PlayerID, @PlayerName,@RegisteredOnDate, @UserID,0,1,titleid ,@SexID, @AvatarID from titles order by titleid asc

	if exists (select * from RealmAttributes where attribid =  74) BEGIN
		update players set Morale = (select attribvalue from RealmAttributes where attribid =  74) where playerid = @PlayerID
	END 

	-- Calling  iCreateVillage Stored Procedure to create the new village for new registered user.
	EXEC iCreateVillage @PlayerID,@PlayerName,@InvitingPlayerID, @InQuadrant, @VillageName

	EXEC uPoints_Player @PlayerID

	--
	-- if we have an inviter, then try to invite player to his clan. 
	--
	IF @InvitingPlayerID <> 0 BEGIN 
		SELECT @ClanID = ClanID FROM ClanMembers where PlayerID = @InvitingPlayerID
		
		IF NOT (@ClanID is null) BEGIN
			--
			-- OK, so inviter is in clan. Now try to see if he has invite permissions	
			IF EXISTS (select * from PlayerInRoles where   PlayerID = @InvitingPlayerID and ClanID=@ClanID and (RoleID=0 or roleID=2 or roleid=3)) BEGIN	
				exec iClanInvite @ClanID, @InvitingPlayerID
					, @PlayerName, @Error OUT
					, 2 /*means generate report but do not do turn on a new report indicator*/
					, @MoreInvitesAvailableOn OUT					
				
				SET @Result = 1
			END ELSE BEGIN
				SET @Result = 2
			END
		END
	END ELSE BEGIN
		SET @Result = 0
	END
	--
	-- if this is a tournament realm 
	--	OR select * from RealmAttributes where attribid = 31 and attribvalue = 1 (dont recall what this means when writing this)
	--	OR this is a retro realm
	-- give player all beginner research
	--
	IF exists(select * from RealmAttributes where attribid = 14 and attribvalue = 1)
		or exists(select * from RealmAttributes where attribid = 31 and attribvalue = 1)
		or exists (select * from RealmAttributes where attribid =2001 and attribvalue = 'Retro')	
		 BEGIN
		INSERT INTO playerresearchitems (playerid, ResearchItemTypeID, ResearchItemID )
		(select @PlayerID, ResearchItemTypeID, ResearchItemID from researchItems RI 
			where not exists 
				(select * from playerresearchitems RI2 where RI2.ResearchItemTypeID = ri.ResearchItemTypeID and RI2.ResearchItemID = ri.ResearchItemID and RI2.PlayerID = @PlayerID)
				and (AgeNumber is null or AgeNumber = 1) 
				and PriceInCoins != 1000000000 -- hack to remove the gov type research 
		) 
	END ELSE IF exists (select * from RealmAttributes where attribid =2000 and (attribvalue = 'HC' or attribvalue = 'MC'))BEGIN 
		--
		-- for HC and MC realms, give player all research items less than 30 min in duration. 
		--
		INSERT INTO playerresearchitems (playerid, ResearchItemTypeID, ResearchItemID )
		select @PlayerID, ResearchItemTypeID, ResearchItemID from ResearchItems RI
		where 
			not exists (select * from playerresearchitems RI2 where RI2.ResearchItemTypeID = ri.ResearchItemTypeID and RI2.ResearchItemID = ri.ResearchItemID and RI2.PlayerID = @PlayerID)
			and ResearchTime < 18000000000 -- less then 30 min
			and PriceInCoins != 1000000000 -- hack to remove the gov type research 		
	END

	commit tran


end try
begin catch
	 IF @@TRANCOUNT > 0 ROLLBACK
	DECLARE @ERROR_MSG AS VARCHAR(8000)

	
	SET @ERROR_MSG = 'iRegisterPlayerInRealm FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerName:'				  + ISNULL(CAST(@PlayerName AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @PlayerID:'				  + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @ClanID'				  + ISNULL(CAST(@ClanID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @Error'				  + ISNULL(CAST(@Error AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @Result'				  + ISNULL(CAST(@Result AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @InvitingPlayerID'				  + ISNULL(CAST(@InvitingPlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @SexID'				  + ISNULL(CAST(@SexID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @UserID'				  + ISNULL(CAST(@UserID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



GO



