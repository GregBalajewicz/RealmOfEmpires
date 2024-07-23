  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uGift_Use')
	BEGIN
		DROP  Procedure  uGift_Use
	END

GO


--
-- no result set if something failed. Otherwise, it returns userID of the invited player - one row, one column
--
CREATE Procedure dbo.uGift_Use
	@UserID uniqueidentifier 
	,@GiftID int 
	, @RID int 
AS
begin try 
	
		declare @username  varchar(max)
		declare @openon datetime 
		select @username  = UserName from aspnet_users where userid = @UserID
		select @openon = openon from realms where realmid = @RID

		--
		-- try to update the invites status to 3 - Meaning gift of this type has been used. NOTE!! it does not mean that it is 
		--  exactly this gift, sent by this person on this date that got used. just a gift of this type
		--
		update GiftsSent set statusid = 3
		    where RecordID = 
		        (Select top 1 RecordID from GiftsSent 
		            WHERE GiftID = @GiftID 
		            AND StatusID = 2
		            AND SentTo = @username
					and senton >= @openon
					order by senton asc
		        )
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uGift_Use FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @userID:'				  + ISNULL(CAST(@userID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @GiftID:'			  + ISNULL(CAST(@GiftID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	

GO