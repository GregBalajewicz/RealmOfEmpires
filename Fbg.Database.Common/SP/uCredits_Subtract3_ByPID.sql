 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uCredits_Subtract3_ByPID')
	BEGIN
		DROP  Procedure  uCredits_Subtract3_ByPID
	END

GO

CREATE Procedure dbo.uCredits_Subtract3_ByPID
	@PlayerID as int,
	@CreditAmount as int,
	@EventTypeID as int,
	@CostField as int,
	@Result as int output
AS
begin try 
	begin tran		
		declare @UserID as uniqueidentifier
		select @UserID=UserId from Players where PlayerID=@PlayerID

		
		update Users 
			set Credits=Credits-@creditAmount 
			where UserID=@UserID
				and Credits-@creditAmount>=0
		
		IF @@rowcount = 0  BEGIN
			set @Result = 1 --error accured in update according to this condtion Credits-@creditAmount>=0
		END else begin 
			set @Result = 0 -- update sucess
			
			-- log the process of subtracting credits.
			insert into UserPFLog
			    (UserID,Time ,EventType,Credits ,Cost)
			    values
			    (@UserID,getdate(), @EventTypeID ,@creditAmount,@CostField)
		end
		
		 
		select @result
	commit tran
 end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uCredits_Subtract3_ByPID FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserID'				  + ISNULL(CAST(@UserID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CreditAmount'			  + ISNULL(CAST(@CreditAmount AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @Result'			  + ISNULL(CAST(@Result AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO 