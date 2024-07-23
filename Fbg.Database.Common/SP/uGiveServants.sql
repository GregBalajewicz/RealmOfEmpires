 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uGiveServants')
	BEGIN
		DROP  Procedure  uGiveServants
	END

GO
-- this SP return 0 for sucess and 1 for fail
CREATE Procedure dbo.uGiveServants
	@userID uniqueidentifier,
	@ServantsAmount as int,
	@EventType as int -- what to put UserPFLog.EventType
AS
begin try 
	begin tran

		update Users set Credits=Credits+@ServantsAmount where UserID=@userID
		IF @@rowcount <> 1 BEGIN
			IF @@TRANCOUNT > 0 ROLLBACK tran ugive
			INSERT INTO ErrorLog VALUES (getdate(), 0,' uGiveServants :  @@rowcount <> 1',  ISNULL(CAST(@userID AS VARCHAR(10)), 'Null'))
			
			select 1;--fail
			return 1;
		end
		-- log the process of buy credits
		insert into UserPFLog
		(UserID,Time ,EventType,Credits ,Cost)
		values
		(@userID,getdate(),@EventType,@ServantsAmount,-1)
		select 0;--sucess
		commit tran
		return 0;
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uGiveServants FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @userID'				  + ISNULL(CAST(@userID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @ServantsAmount'			  + ISNULL(CAST(@ServantsAmount AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO