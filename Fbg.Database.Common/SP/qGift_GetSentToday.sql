  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qGift_GetSentToday')
	BEGIN
		DROP  Procedure  qGift_GetSentToday
	END

GO

--
-- returns all recipients of gifts the user sent today. 
-- NOTE!! CRITICAL!!! Note that this SP is called from  iRegisterInvited_Gift via INSERT-EXEC. 
--  BE AWARE OF THIS WHEN making changes to this SP!! See : http://www.sommarskog.se/share_data.html#INSERTEXEC 
--
CREATE Procedure dbo.qGift_GetSentToday
	@UserID uniqueidentifier 
AS
begin try 	
    declare @now Datetime
    set @now = getdate()

	select SentTo from GiftsSent 
	    where SentOn > (cast(datename(year, @now) +'-'+ cast(month(@now)as varchar(2))+'-'+ datename( dd, @now) as datetime) )
	        AND PlayerID in (select playerid from players where userid = @UserID)
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qGift_GetSentToday FAILED! ' +  + CHAR(10)
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


 