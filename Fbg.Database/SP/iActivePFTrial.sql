 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iActivePFTrial')
	BEGIN
		DROP  Procedure  iActivePFTrial
	END

GO

CREATE Procedure dbo.iActivePFTrial
	@PlayerID as int,
	@PFTrialID as int
AS

begin try 
	begin tran
	if exists(select * from PlayerPFTrials where PlayerID=@PlayerID and PFTrialID=@PFTrialID)
		begin
			update PlayerPFTrials 
			set ExpiresOn=DATEADD(day,Duration,dbo.fnGetMax(ExpiresOn,getdate()))
			from PlayerPFTrials 
			join PFTrails
			on PFTrails.PFTrialID=PlayerPFTrials.PFTrialID
			where playerid=@PlayerID
		end
	else
		begin 
			insert into PlayerPFTrials 
			select @PlayerID,@PFTrialID,DATEADD(day,Duration,getdate())
			from PFTrails
			where PFTrialID=@PFTrialID
		end
		
	select PFTrialID,ExpiresOn from PlayerPFTrials where PlayerID = @PlayerID
	commit tran
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iActivatePackage FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID'				  + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @PFTrialID'				  + ISNULL(CAST(@PFTrialID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



  