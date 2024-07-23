IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayers_Avatar')
	BEGIN
		DROP  Procedure  uPlayers_Avatar 
	END

GO

--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO

----
---- return value is a single row, single column - either NULL if sleep mode was not activated
----	or the date that the sleep mode will go active on.
----
--CREATE Procedure [dbo].uPlayers_Avatar 
--		@PlayerID int,
--		@AvatarID int
--AS
	
--begin try 
--    update players set AvatarID = @AvatarID where playerid = @playerid
--end try

--begin catch
--	DECLARE @ERROR_MSG AS VARCHAR(8000) 

--	IF @@TRANCOUNT > 0 ROLLBACK

	
--	SET @ERROR_MSG = 'uPlayers_Avatar FAILED! ' +  + CHAR(10)
--		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
--		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
--		+ '   @AvatarID' + ISNULL(CAST(@AvatarID AS VARCHAR(20)), 'Null') + CHAR(10)
--		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
--		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
--		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
--		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
--		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
--		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
--	RAISERROR(@ERROR_MSG,11,1)	

--end catch	



   