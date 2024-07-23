   IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dAccountStewards')
	BEGIN
		DROP  Procedure  dAccountStewards
	END

GO
--
-- this is when the owner of an account cancells the appoitment for a steward
--
CREATE Procedure dbo.dAccountStewards
	@AccountOwnerPlayerID as int
	,@RecordID as int
	AS

begin try 
	BEGIN TRAN
		declare @StewardPlayerID int
		
		SELECT @StewardPlayerID = StewardPlayerID
			FROM AccountStewards
			where PlayerID = @AccountOwnerPlayerID
			and RecordID = @RecordID
		
		IF @StewardPlayerID is not null BEGIN
		
			Delete AccountStewards
				where PlayerID = @AccountOwnerPlayerID
				and RecordID = @RecordID
			IF @@rowcount >= 1 begin 
				insert into AccountStewardLog(ActingPlayerId, EventTypeID, Time, Notes)
					values (@AccountOwnerPlayerID, 2 , getdate(), @StewardPlayerID)
			END 
		END

	COMMIT TRAN
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'dAccountStewards FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @StewardPlayerID'			+ ISNULL(CAST(@StewardPlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @AccountOwnerPlayerID'	+ ISNULL(CAST(@AccountOwnerPlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @RecordID'				+ ISNULL(CAST(@RecordID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		+ ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		+ ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			+ ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'         + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



  