   IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uGift_Delete')
	BEGIN
		DROP  Procedure  uGift_Delete
	END

GO


--
-- no result set if something failed. Otherwise, it returns userID of the invited player - one row, one column
--
CREATE Procedure dbo.uGift_Delete
	@UserID uniqueidentifier 
	,@RecordID int 
AS
begin try 
	begin tran
		--
		-- try to update the invites status to 4 - Meaning this gift has been deleted. 
		--
		update GiftsSent set statusid = 4
		    where RecordID = @RecordID
	            AND SentTo = (select UserName from aspnet_users where userid = @UserID)
	            AND StatusID = 2 /*means gifts was accepted. do not allow deleting gifts that were not accepted*/
		    
	commit tran	
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uGift_Delete FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @userID:'				  + ISNULL(CAST(@userID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @RecordID:'			  + ISNULL(CAST(@RecordID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	


