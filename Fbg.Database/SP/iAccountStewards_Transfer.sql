 


IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iAccountStewards_Transfer')
BEGIN
	DROP  Procedure  iAccountStewards_Transfer
END

GO

CREATE Procedure dbo.iAccountStewards_Transfer
	@AccountOwnerPlayerID as int,
	@StewardPlayerID as int
AS

begin try 
	begin tran
		declare @CurrentStewardPlayerID int
	
		SELECT @CurrentStewardPlayerID = StewardPlayerID
			FROM AccountStewards
			where PlayerID = @AccountOwnerPlayerID
			and State = 2
		
		IF @CurrentStewardPlayerID is not null BEGIN
			Delete AccountStewards
				where PlayerID = @AccountOwnerPlayerID
				and State = 2
				
			IF @@rowcount >= 1 begin 		
				insert into AccountStewards (PlayerID, StewardPlayerID) 
					values (@AccountOwnerPlayerID, @StewardPlayerID)
					
					
				insert into AccountStewardLog(ActingPlayerId, EventTypeID, Time, Notes)
					values (@AccountOwnerPlayerID, 5 , getdate(), @CurrentStewardPlayerID)
					
				insert into AccountStewardLog(ActingPlayerId, EventTypeID, Time, Notes)
					values (@AccountOwnerPlayerID, 6 , getdate(), @StewardPlayerID)
			END 
		END 
	commit tran
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iAccountStewards_Transfer FAILED! ' +  + CHAR(10)
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



 