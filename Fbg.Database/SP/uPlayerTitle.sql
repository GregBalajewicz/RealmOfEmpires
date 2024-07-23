   IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayerTitle')
	BEGIN
		DROP  Procedure  uPlayerTitle
	END

GO

CREATE Procedure dbo.uPlayerTitle
	@PlayerID as int
AS
	declare @PlayersPoints int
	declare @TitleID int 
	declare @XPForTitle int
	declare @UserID uniqueidentifier
	declare @ThisRealmID int 
	declare @titleUpdated bit 

__RETRY__:
begin try 


	--
	-- get player's points
	--
	select @PlayersPoints = Points from Players where PlayerID = @PlayerID


	--
	-- get highest possible title
	--
	select top 1 @TitleID = TitleID
		, @XPForTitle = XP
		from Titles 
		where @PlayersPoints <= MaxPoints 
		order by MaxPoints asc


	IF @TitleID is not null BEGIN
		BEGIN TRAN 
			set @titleUpdated = 0 
			Update Players set TitleID = @TitleID where PlayerID = @PlayerID and TitleID <> @TitleID 
			
			IF @@rowcount = 1 BEGIN
				insert into PlayerTitleHistory(PlayerID, TitleID, AcceptedOn)
					values (@PlayerID, @TitleID, getdate())
				set @titleUpdated = 1
			END
		COMMIT TRAN

		--
		-- doing the following outside of the transaction to avoid distributed transactions
		--
		IF @titleUpdated = 1 BEGIN 
			-- give player XP
			select distinct @userid = USERID from FBGC.FBGCommon.dbo.players where playerid = @PlayerID
			select @ThisRealmID = AttribValue from realmAttributes where attribID = 33

			update FBGC.FBGCommon.dbo.users set XP = XP + @XPForTitle where userid = @userid
			insert into FBGC.FBGCommon.dbo.UserXPHistory (userid, time, XPReceived, XPFromTypeID, XPFromRealmID) 
				values (@UserID, GETDATE(), @XPForTitle, 1, @ThisRealmID)

			Update Players set xp_cached = (select xp from FBGC.FBGCommon.dbo.users where userid = @userid) where playerid = @PlayerID
		END 

	END 

end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uPlayerTitle FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @PlayerID'			+ ISNULL(CAST(@PlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @PlayersPoints'		+ ISNULL(CAST(@PlayersPoints AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @TitleID'				+ ISNULL(CAST(@TitleID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'	+ ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'	+ ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'		+ ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'		+  ERROR_MESSAGE() + CHAR(10)
		
	
	--
	-- IF deadlock, rerun
	--
	IF ERROR_NUMBER() = 1205 -- deadlock occured in this SP 
		OR (ERROR_NUMBER() = 50000 AND left(ERROR_MESSAGE(), 8) = 'DEADLOCK' ) -- deadlock occured in some called SP 
	BEGIN
		INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock', @ERROR_MSG)		
		WAITFOR DELAY '00:00:05'
		GOTO __RETRY__
	END 		
		
	RAISERROR(@ERROR_MSG,11,1)	
end catch	


