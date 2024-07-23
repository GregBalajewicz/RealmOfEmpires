IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qTransferableCredits')
BEGIN
	DROP  Procedure  qTransferableCredits
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

--
-- this returns you the max number of credits the user can trasnfer
--
CREATE Procedure dbo.qTransferableCredits
	@UserID Uniqueidentifier 
	, @CurrentTransferableCredits int output 

AS
set nocount on 
begin try 
	declare @TransferableCredits int
	declare @TransferedCredits int
	declare @TransferedCreditsInLast24h int
	declare @CurrentCredits int

	set @TransferableCredits = 0
	set @TransferedCredits = 0

	--
	-- get sum of all bought/earned credits
	--
	select @TransferableCredits = sum(credits) from userpflog 
		where userid = @UserID
			and EventType in (1, 11,12,19,22,26,29,30,31)
		group by UserID
	--
	-- get the sum of all transfers thus far
	--
	select @TransferedCredits = sum(credits) from userpflog 
		where userid = @UserID
			and EventType in (16)
		group by UserID
	--
	-- get current credits
	--
	select @CurrentCredits = credits from users 
		where userid = @UserID

--
	-- get the sum of all transfers in the last 24 hours
	--
	select @TransferedCreditsInLast24h = sum(credits) from userpflog 
		where userid = @UserID
			and EventType in (16)
			and time > dateadd(day, -1, getdate())
		group by UserID
	set @TransferedCreditsInLast24h = ISNULL(@TransferedCreditsInLast24h, 0)
	--
	-- get the max number of credits the user is able to transfer at this time. 
	--
	set @CurrentTransferableCredits = @TransferableCredits - @TransferedCredits
	IF @CurrentTransferableCredits > @CurrentCredits BEGIN
		SET @CurrentTransferableCredits = @CurrentCredits
	END 

	--
	-- max 5000 per today
	--
	if @CurrentTransferableCredits > 5000 BEGIN 
		SET @CurrentTransferableCredits = 5000
	END

	--
	-- but subtract any sent already today
	--
	set @CurrentTransferableCredits = @CurrentTransferableCredits - @TransferedCreditsInLast24h
	--
	-- but subtract any sent already today
	--
	if @CurrentTransferableCredits < 0 BEGIN 
		SET @CurrentTransferableCredits = 0
	END

	
	
	select @CurrentTransferableCredits
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qTransferableCredits FAILED! '	+  CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserID'					 + ISNULL(CAST(@UserID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TransferableCredits'	 + ISNULL(CAST(@TransferableCredits AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TransferedCredits'		 + ISNULL(CAST(@TransferedCredits AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @CurrentCredits'		 + ISNULL(CAST(@CurrentCredits AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(max)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(max)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



 