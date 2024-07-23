 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uTransferCredits')
BEGIN
	DROP  Procedure  uTransferCredits
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE Procedure dbo.uTransferCredits
	@TransferingUserID Uniqueidentifier 
	,@AmountOfCredits int 
	,@PlayerToTransferToID int
AS
set nocount on 
begin try 
	declare @MaxCanTransfer int
	declare @UserToTransferToID Uniqueidentifier	
	declare @TimeOfTransaction datetime
	declare @RealmID int
	--
	-- get user we transfer to 
	--
	select @UserToTransferToID = userid, @RealmID = realmid from players where PlayerID = @PlayerToTransferToID
	IF @UserToTransferToID is null BEGIN
		RAISERROR(' @UserToTransferToID is null ', 11, 1)
	END
	

	--
	-- find out how many credits are available for transfer. Only allow transfering this much. 
	--	yes, yes, this is not thread safe but should not be an issue. 
	--
	exec qTransferableCredits @TransferingUserID, @MaxCanTransfer output
	IF @MaxCanTransfer < @AmountOfCredits BEGIN
		RAISERROR(' @MaxCanTransfer < @AmountOfCredits', 11, 1)
	END
	--
	-- do the transfer
	--
	BEGIN TRAN
		update Users 
			set Credits=Credits-@AmountOfCredits 
			where UserID=@TransferingUserID
				and Credits-@AmountOfCredits>=0
		IF @@rowcount <> 1 BEGIN
			RAISERROR('subtract credit from user who is transfering the credits:@@rowcount <> 1', 11, 1)
		end				
				
		update Users 
			set Credits=Credits+@AmountOfCredits 
			where UserID=@UserToTransferToID
		IF @@rowcount <> 1 BEGIN
			RAISERROR('add credit to transfer-to user :@@rowcount <> 1', 11, 1)
		end				
		--
		-- log this transaction
		--
		set @TimeOfTransaction= getdate()
		insert into UserPFLog
			(UserID,Time ,EventType,Credits ,Cost, realmid, data)
			values
			(@TransferingUserID,@TimeOfTransaction,16,@AmountOfCredits,0,  @RealmID, @UserToTransferToID)

		insert into UserPFLog
			(UserID,Time ,EventType,Credits ,Cost, realmid, data)
			values
			(@UserToTransferToID,@TimeOfTransaction,17,@AmountOfCredits,0, @RealmID, @TransferingUserID)
		
	COMMIT TRAN 
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uTransferCredits FAILED! '	+  CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @TransferingUserID'					 + ISNULL(CAST(@TransferingUserID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AmountOfCredits'	 + ISNULL(CAST(@AmountOfCredits AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @PlayerToTransferToID'		 + ISNULL(CAST(@PlayerToTransferToID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @MaxCanTransfer'		 + ISNULL(CAST(@MaxCanTransfer AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @UserToTransferToID'		 + ISNULL(CAST(@UserToTransferToID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TimeOfTransaction'		 + ISNULL(CAST(@TimeOfTransaction AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(max)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(max)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



 