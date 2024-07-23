IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uBuyCredits')
	BEGIN
		DROP  Procedure  uBuyCredits
	END

GO

CREATE Procedure dbo.uBuyCredits
	@userID uniqueidentifier,
	@creditAmount as int
AS
begin try 
	begin tran

		update Users set Credits=Credits+@creditAmount where UserID=@userID
		-- log the process of buy credits
			insert into UserPFLog
		(UserID,Time ,EventType,Credits ,Cost)
		values
		(@userID,getdate(),1,@creditAmount,-1)
	commit tran	
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uBuyCredits FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @userID'				  + ISNULL(CAST(@userID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @creditAmount'			  + ISNULL(CAST(@creditAmount AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO