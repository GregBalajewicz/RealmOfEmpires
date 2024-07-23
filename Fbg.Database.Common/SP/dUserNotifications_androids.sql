IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dUserNotifications_android')
	BEGIN
		DROP PROCEDURE dbo.dUserNotifications_android
	END
GO

CREATE PROCEDURE dbo.dUserNotifications_android
	@UserName AS VARCHAR(256)
	AS
	
BEGIN TRY
	BEGIN TRAN

	DECLARE @UserId AS UniqueIdentifier

	SELECT @UserId = UserId
	FROM         aspnet_Users
	WHERE     (aspnet_Users.UserName = @UserName)

	DELETE FROM UserNotifications_android
		  WHERE UserId = @UserId
		  
	COMMIT TRAN
END TRY

BEGIN CATCH
	DECLARE @ERROR_MSG AS VARCHAR(MAX)
	IF @@TRANCOUNT > 0 ROLLBACK
	SET @ERROR_MSG = 'dUserNotifications_android FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserName'			+ ISNULL(CAST(@UserName AS VARCHAR(256)), 'Null') + CHAR(10)		
		+ '   @UserId'		+ ISNULL(CAST(@UserId AS VARCHAR(256)), 'Null') + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
END CATCH

GO


