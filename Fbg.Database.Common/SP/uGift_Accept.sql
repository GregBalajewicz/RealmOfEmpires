  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uGift_Accept')
	BEGIN
		DROP  Procedure  uGift_Accept
	END

GO


--
-- no result set if something failed. Otherwise, it returns userID of the invited player - one row, one column
--
CREATE Procedure dbo.uGift_Accept
	@UserID uniqueidentifier 
	,@GiftID int 
AS
begin try 
	begin tran
	
		--
		-- try to update the invites status to 2 - Meaning gift of this type accepted. NOTE!! it does not mean that it is 
		--  exactly this gift, sent by this person on this date that got accepted. just a gift of this type
		--
		update GiftsSent set statusid = 2
		    where RecordID = 
		        (Select top 1 RecordID from GiftsSent 
		            WHERE GiftID = @GiftID 
		            AND StatusID = 1
		            AND SentTo = (select UserName from aspnet_users where userid = @UserID)
					order by senton desc
		        )
		    
		IF @@rowcount <> 1 BEGIN
			--
			-- if update failed, then we got inconsistent data. 
			--
			IF @@TRANCOUNT > 0 ROLLBACK
			INSERT INTO ErrorLog VALUES (getdate(), 0, 'uGift_Accept :  @@rowcount <> 1',  ISNULL(CAST(@UserID AS VARCHAR(100)), 'Null'))
			return 		
		END 
	commit tran	
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uGift_Accept FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @userID'				  + ISNULL(CAST(@userID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @GiftID'			  + ISNULL(CAST(@GiftID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO







--select * from invites
--select * from aspnet_Users
-- select * from players