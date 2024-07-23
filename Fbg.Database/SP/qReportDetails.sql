IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qReportDetails')
	BEGIN
		DROP  Procedure  qReportDetails
	END

GO

CREATE Procedure qReportDetails
                  @PlayerID int
				 ,@RecordID int
AS

BEGIN
 
begin try

        Select R.Time
                ,R.Subject
                ,R.ReportTypeID
                ,R.ReportTypeSpecificData as Description
                ,isnull(RA.ForwardedByPlayerID,0) as ForwardedByPlayerID
				,FP.Name
				,RA.ForwardedOn
				,FP.AvatarID as ForwardedByPlayerAvatarID
                from Reports R 
                join ReportAddressees RA
                     on RA.RecordID=@RecordID
                     and RA.PlayerID=@PlayerID
                left join Players FP
					 on RA.ForwardedByPlayerID=FP.PlayerID
                join Players P
                     on RA.PlayerID = P.PlayerID
                where P.PlayerID = @PlayerID and R.ReportID=RA.ReportID
           
           
         Update ReportAddressees set IsViewed = 1 where PlayerID = @PlayerID and RecordID=@RecordID

end try
	
begin catch
DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK
	
	SET @ERROR_MSG = 'qReportDetails FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
        + '   @RecordID' + ISNULL(CAST(@RecordID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch
	
END


