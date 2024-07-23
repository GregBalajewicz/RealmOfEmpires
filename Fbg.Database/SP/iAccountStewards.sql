  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iAccountStewards')
	BEGIN
		DROP  Procedure  iAccountStewards
	END

GO

CREATE Procedure dbo.iAccountStewards
	@AccountOwnerPlayerID as int,
	@StewardPlayerID as int
AS

begin try 
	begin tran
		IF not exists (select * From playersuspensions where playerid in (@AccountOwnerPlayerID) and SupensionID = 4) BEGIN
			insert into AccountStewards (PlayerID, StewardPlayerID) 
				select top 1 @AccountOwnerPlayerID, @StewardPlayerID 
						FROM Players 
						where not exists (select * from AccountStewards where PlayerID = @AccountOwnerPlayerID )
			IF @@rowcount >= 1 begin 			
				insert into AccountStewardLog(ActingPlayerId, EventTypeID, Time, Notes)
					values (@AccountOwnerPlayerID, 1 , getdate(), @StewardPlayerID)
			END
		END
	commit tran
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iAccountStewards FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @AccountOwnerPlayerID'		+ ISNULL(CAST(@AccountOwnerPlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @StewardPlayerID'				+ ISNULL(CAST(@StewardPlayerID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



 