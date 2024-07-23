   IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qAccountStewards_AccountsISteward')
	BEGIN
		DROP  Procedure  qAccountStewards_AccountsISteward
	END

GO

CREATE Procedure dbo.qAccountStewards_AccountsISteward
	@StewardPlayerID as int
	
	AS

begin try 

		select S.PlayerID, P.Name, S.RecordID, S.State, P.UserID
			from AccountStewards S
			join Players P
				on  S.PlayerID = P.PlayerID
			where S.StewardPlayerID = @StewardPlayerID
			and not exists (select * From playersuspensions where playerid in (@StewardPlayerID) and SupensionID = 4) 
			and not exists ( select * from  FBGC.fbgcommon.dbo.PlayerSuspensions PS where PS.userid = p.userid and IsSuspensionActive = 1) 

			order by State desc

end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qAccountStewards_MyStewards FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @StewardPlayerID'		+ ISNULL(CAST(@StewardPlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



  