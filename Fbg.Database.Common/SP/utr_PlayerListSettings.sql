    
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'utr_PlayerListSettings')
	BEGIN
		DROP  Procedure  utr_PlayerListSettings
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].utr_PlayerListSettings
		@UserID as uniqueidentifier
		, @PlayerID as int 
		, @displayStatusID int
AS
	
	
begin try 

	update tr_PlayerListSettings set displayStatus = @displayStatusID where userid = @UserID and PlayerID = @PlayerID

	IF @@ROWCOUNT < 1 BEGIN

		insert tr_PlayerListSettings (userid, playerid, displayStatus)
		select distinct @UserID, PLayers.Playerid, @displayStatusID from 
		(
			select Playerid from players where userid = @userid and playerid = @PlayerID
			union all
			select Playerid from DeletedPlayers where userid = @userid and playerid = @PlayerID
		) as PLayers
	

	END 
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'utr_PlayerListSettings FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserID' + ISNULL(CAST(@UserID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 