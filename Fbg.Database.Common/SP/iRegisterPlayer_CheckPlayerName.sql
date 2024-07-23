 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iRegisterPlayer_CheckPlayerName')
	BEGIN
		DROP  Procedure  iRegisterPlayer_CheckPlayerName
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO
CREATE Procedure dbo.iRegisterPlayer_CheckPlayerName
	@PlayerName varchar(25),
		@UserID uniqueidentifier

AS

begin try 
		--
		-- this checks if such a player name exists on any realm. 
		--   so this is only valid for brand new players since existing player may register on a different realm with the same name
		--
		IF	NOT EXISTS (select PlayerID from Players where [Name]=@PlayerName
				UNION select PlayerID from DeletedPlayers where [Name]=@PlayerName)
			AND NOT EXISTS (select userID from Users where GlobalPlayerName=@PlayerName and UserID <> @UserID)
        BEGIN		
		
		    SELECT 1
		end ELSE BEGIN 
		    --
		    -- player name already taken!
		    --
		    select 0
	    END 
		
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iRegisterPlayer_CheckPlayerName FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerName:'				  + ISNULL(@PlayerName, 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



