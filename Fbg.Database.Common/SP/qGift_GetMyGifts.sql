  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qGift_GetMyGifts')
	BEGIN
		DROP  Procedure  qGift_GetMyGifts
	END

GO


CREATE Procedure dbo.qGift_GetMyGifts
	@UserID uniqueidentifier 
	, @RID int 
AS
begin try 	

declare @UserName varchar(max) 
declare @OpenOn DateTime
select @UserName = UserName from aspnet_users where userid = @UserID
select @OpenOn = openon from realms where realmid = @rid

select GiftID, count(*) as cnt from GiftsSent 
	    where StatusID = 2
	        AND SentTo = @username
			and  senton >= @openOn
	        group by GiftID
	        order by GiftID

end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qGift_GetMyGifts FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @userID'				  + ISNULL(CAST(@userID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO


