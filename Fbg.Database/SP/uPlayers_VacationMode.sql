IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayers_VacationMode')
	BEGIN
		DROP  Procedure  uPlayers_VacationMode
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--
-- no logic is checked, stores requested date as now, returns it.
--
CREATE Procedure [dbo].uPlayers_VacationMode
		@PlayerID int
AS
	
begin try 
    
	begin tran
	
		--VacationModeRequestOn is the exact date a player requested the activation. Vacation takes effect on this date + realm delay
        update Players set VacationModeRequestOn = getdate() WHERE PlayerID = @PlayerID

        IF @@ROWCOUNT = 1 BEGIN 
            select  VacationModeRequestOn from players WHERE PlayerID = @PlayerID
        END
        
	commit tran
	
end try

begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'uPlayers_VacationMode FAILED! ' +  + CHAR(10)
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


   