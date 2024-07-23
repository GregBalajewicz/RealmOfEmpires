 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uRefundCredits')
	BEGIN
		DROP  Procedure  uRefundCredits
	END

GO

CREATE Procedure dbo.uRefundCredits
	@PlayerID as int,
	@CreditAmount as int
AS
begin try 
	begin tran
		declare @result as int
		declare @UserID as uniqueidentifier
		
		select @UserID=UserId from Players where PlayerID=@PlayerID
		
		update Users 
			set Credits=Credits+@creditAmount 
			where UserID=@UserID
		
		
		IF @@rowcount = 0 
			BEGIN
				set @result=1 --error accured in update according to this condtion Credits+@creditAmount>=0
			END
		else
			begin 
				set @result=0 -- update sucess
			end
		-- log the process of buy credits
		insert into UserPFLog
		(UserID,Time ,EventType,Credits ,Cost)
		values
		(@UserID,getdate(),7,@creditAmount,-1)
		
		select @result
	commit tran
 end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uRefundCredits FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID'				  + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CreditAmount'			  + ISNULL(CAST(@CreditAmount AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @UserID'			  + ISNULL(CAST(@UserID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @result'			  + ISNULL(CAST(@result AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO