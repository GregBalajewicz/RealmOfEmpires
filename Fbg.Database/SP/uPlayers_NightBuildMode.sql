
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayers_NightBuildMode')
	BEGIN
		DROP  Procedure  uPlayers_NightBuildMode
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--
-- return value is a single row, single column - either NULL if build mode was not activated
--	or the date that the build mode was activated on.
--
CREATE Procedure [dbo].uPlayers_NightBuildMode
		@PlayerID int
AS
	
	
begin try 

	begin tran
	
	declare  @now datetime
	set @now = getdate()

	update players set NightBuildActivatedOn=getdate() 
		where playerid = @PlayerID
		and datediff(hour, NightBuildActivatedOn, @now) >= 24
	IF @@rowcount = 1 BEGIN			
		SELECT @now
	END ELSE BEGIN
		SELECT NULL
	END


	commit tran
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uPlayers_NightBuildMode FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



  