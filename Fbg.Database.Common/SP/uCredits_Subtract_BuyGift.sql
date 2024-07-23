 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uCredits_Subtract_BuyGift')
	BEGIN
		DROP  Procedure  uCredits_Subtract_BuyGift
	END

GO

CREATE Procedure dbo.uCredits_Subtract_BuyGift
	@PlayerID as int,
	@CreditAmount as int,
	@GiftID as int
	,@rid int
	,@vid int
	,@payout varchar(max)
	,@Result as int output
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
			    (UserID,Time ,EventType,Credits ,Cost, realmid, villageid, data)
			    values
			    (@UserID,getdate(),24,@creditAmount,@GiftID, @rid, @vid, @payout)
		end
		
		 
		select @result
	commit tran
 end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uCredits_Subtract_BuyGift FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID'				  + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
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