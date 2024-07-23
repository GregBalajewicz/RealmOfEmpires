  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qAccountStewards_MyStewards')
	BEGIN
		DROP  Procedure  qAccountStewards_MyStewards
	END

GO

CREATE Procedure dbo.qAccountStewards_MyStewards
	@AccountOwnerPlayerID as int
	
	AS

begin try 
		select StewardPlayerID, P.Name, S.RecordID, S.State
			from AccountStewards S
			join Players P
				on  S.StewardPlayerID = P.PlayerID
			where S.PlayerID = @AccountOwnerPlayerID

end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qAccountStewards_MyStewards FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @AccountOwnerPlayerID'		+ ISNULL(CAST(@AccountOwnerPlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



  