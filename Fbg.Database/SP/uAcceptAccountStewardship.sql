 

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uAcceptAccountStewardship')
BEGIN
	DROP  Procedure  uAcceptAccountStewardship
END

GO

CREATE Procedure dbo.uAcceptAccountStewardship
	@StewardPlayerID  as int
	,@RecordID as int
	AS

	declare @success_subtractCredits bit
	

begin try 
	set @success_subtractCredits = 0 --default val
	BEGIN TRAN
		declare @AccountOwnerPlayerID int
		
		SELECT @AccountOwnerPlayerID = PlayerID
			FROM AccountStewards
			where StewardPlayerID = @StewardPlayerID
			and RecordID = @RecordID
	
		IF @AccountOwnerPlayerID is not null BEGIN		
			UPDATE AccountStewards
				SET State = 2
				where StewardPlayerID = @StewardPlayerID
				and RecordID = @RecordID
				and State = 1 -- meaning pending acceptance
				
			IF @@rowcount >= 1 begin 
				set @success_subtractCredits = 1 
				insert into AccountStewardLog(ActingPlayerId, EventTypeID, Time, Notes)
					values (@AccountOwnerPlayerID, 3 , getdate(), @StewardPlayerID)
					
			END 
		END
	COMMIT TRAN						
	--
	-- since this player now has an accepted steward, remove him from people to be deleted list
	--
	IF @success_subtractCredits = 1  BEGIN 
		delete from FBGC.FBGCommon.dbo.InactivePlayersToBeWarned where PlayerID = @AccountOwnerPlayerID
	END 

end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uAcceptAccountStewardship FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @StewardPlayerID'			+ ISNULL(CAST(@StewardPlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @RecordID'				+ ISNULL(CAST(@RecordID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @AccountOwnerPlayerID'	+ ISNULL(CAST(@AccountOwnerPlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		+ ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		+ ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			+ ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'         + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



  