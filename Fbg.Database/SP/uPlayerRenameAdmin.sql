if exists (select * from sysobjects where type = 'P' and name = 'uPlayerRenameAdmin')
begin
	drop procedure uPlayerRenameAdmin
end
go

/*

admin function only!    
*/
create Procedure uPlayerRenameAdmin
	 @PlayerName varchar(25)
	, @NewPlayerName varchar(25)
	, @RealmID int
as
	


declare @userID uniqueidentifier

begin try 

	--
	-- NO TRAN cause would need to do a distributed transaction!
	--


	select @userid = userid from FBGC.fbgcommon.dbo.players where name =@PlayerName and realmid = @realmID
	if @userid is null BEGIN 
		RAISERROR ('cannot find player by this name on this realm ', 16,1); 
	END


	update FBGC.fbgcommon.dbo.players set name = @NewPlayerName 
		where name = @PlayerName 
		and realmID = @RealmID
		and NOT EXISTS (
			select PlayerID from FBGC.fbgcommon.dbo.Players where RealmID=@RealmID and [Name]=@NewPlayerName
			UNION select PlayerID from FBGC.fbgcommon.dbo.DeletedPlayers where RealmID=@RealmID and [Name]=@NewPlayerName
			)
		AND NOT EXISTS (
			select PlayerID from FBGC.fbgcommon.dbo.Players where RealmID<>@RealmID and [Name]=@NewPlayerName and UserID <> @UserID
				UNION select PlayerID from FBGC.fbgcommon.dbo.DeletedPlayers where RealmID<>@RealmID and [Name]=@NewPlayerName and UserID <> @UserID)
	IF @@rowcount <> 1 BEGIN 
		RAISERROR ('when updating FBGC.fbgcommon.dbo.players, expected to change 1 row but either changed none or more', 11,1); 
	END

	update players set name = @NewPlayerName where 
		name = @PlayerName 
	IF @@rowcount <> 1 BEGIN 
		RAISERROR ('when updating realm specific PLAYERS table, expected to change 1 row but either changed none or more', 16,1); 
	END
	
	--
	-- log this 
	--
	insert into ErrorLog values (getdate(), 10000, 'admin - player renamed', 'from ' + @PlayerName + ' to ' + @NewPlayerName + ' userid: ' + cast(@userid as varchar(40)))

	insert into FBGC.fbgcommon.dbo.userlog  
		select getdate(), @userid, playerid, 14, 'admin - player renamed', 'from >' + @PlayerName + '< to >' + @NewPlayerName + '<'
		from players where userid = @userID
	
	


end try 
begin catch 
	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = '*** FAILED ***' +  + CHAR(10)
		+ '   REASON:'           + ERROR_MESSAGE() + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10);

	RAISERROR(@ERROR_MSG,11,1)
end catch