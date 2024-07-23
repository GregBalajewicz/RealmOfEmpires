IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uUserUnlockAvatarID')
	BEGIN
		DROP  Procedure  uUserUnlockAvatarID
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

CREATE Procedure dbo.uUserUnlockAvatarID
	@userID uniqueidentifier,
	@avatarID int
AS


begin try 

	--check unlock table, if not already unlocked, then add the unlock for user
	if not exists (select * from UsersUnlockedAvatars where UserID = @userID and @avatarID = AvatarID) begin
		insert into UsersUnlockedAvatars values (@userID,@avatarID);
	end

end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'uUserUnlockAvatarID FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserID:'					  + ISNULL(CAST(@UserID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @avatarID:'				  + ISNULL(CAST(@avatarID AS VARCHAR(max)), 'Null')
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



