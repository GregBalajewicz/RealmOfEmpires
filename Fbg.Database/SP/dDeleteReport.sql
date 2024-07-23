IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dDeleteReport')
	BEGIN
		DROP  Procedure dDeleteReport
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure dDeleteReport
       @PlayerID int
       ,@RecordIDs varchar(2000)
     
AS

BEGIN Try

	IF @RecordIDs is null BEGIN
		delete ReportInfoFlag where recordid in (select RecordID from ReportAddressees where folderid is null and playerid = @PlayerID )
		delete ReportAddressees where folderid is null and playerid = @PlayerID
	END ELSE BEGIN 
		-- NOTE: we are not ensuring that report flag deleted is for a report that belongs to a players but we do so when 
		--  we delete the report so its enough.      
		delete ReportInfoFlag where RecordID In (Select ID from fnGetIds(@recordIDs))
    
    
		 delete From ReportAddressees 
			where RecordID In (Select ID from fnGetIds(@recordIDs))
			AND PlayerID = @PlayerID
    END



End Try

BEGIN Catch

	DECLARE @ERROR_MSG AS VARCHAR(8000) 

  	IF @@TRANCOUNT > 0 ROLLBACK
	
	SET @ERROR_MSG = 'dDeleteReport FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

End Catch	
	

