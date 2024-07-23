IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uGift_AcceptByRequest')
	BEGIN
		DROP  Procedure  [uGift_AcceptByRequest]
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create Procedure [dbo].[uGift_AcceptByRequest]
	@UserID uniqueidentifier 
	,@RequestID varchar(550)
AS
begin try 
	begin tran
	
		--
		-- try to update the invites status to 2 - Meaning gift of this type accepted. NOTE!! it does not mean that it is 
		--  exactly this gift, sent by this person on this date that got accepted. just a gift of this type
		--
		--declare @RequestIDs table (id varchar(250))
		
		select ID into #RequestIDs from fnGetIds(@RequestID) ID
		
		--declare @RecordIDs table (id varchar(250))
		--
		
		select RecordID into #RecordIDs
		from
			(Select RecordID from GiftsSent 
		            WHERE RequestID in (select ID from #RequestIDs)
		            AND StatusID = 1
		            AND SentTo = (select UserName from aspnet_users where userid = @UserID)) r
					
		--declare @giftid int
		--set @giftid = (Select top 1 GiftID from GiftsSent 
		--            WHERE RequestID in 
		--            AND StatusID = 1
		--            AND SentTo = (select UserName from aspnet_users where userid = @UserID)
		--			order by senton desc)
		
		update GiftsSent set statusid = 2
		where RecordID in (select RecordID from #RecordIDs)
		        
		select distinct GiftID
		from GiftsSent where RecordID in (select RecordID from #RecordIDs)
		
		--rollback
		
		IF @@rowcount = 0 BEGIN
			--
			-- if update failed, then we got inconsistent data. 
			--
			IF @@TRANCOUNT > 0 ROLLBACK
			INSERT INTO ErrorLog VALUES (getdate(), 0, 'uGift_Accept :  @@rowcount = 0',  ISNULL(CAST(@UserID AS VARCHAR(100)), 'Null'))
			return 		
		END 
	commit tran	
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uGift_AcceptByRequest FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @userID'				  + ISNULL(CAST(@userID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @RequestID'			  + ISNULL(CAST(@RequestID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	

