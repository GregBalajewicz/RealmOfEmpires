IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qUserNotifications_android')
	BEGIN
		DROP PROCEDURE dbo.qUserNotifications_android
	END
GO

CREATE PROCEDURE dbo.qUserNotifications_android
	@UserName AS VARCHAR(256)
	AS
	
BEGIN TRY
	
	SELECT     aspnet_Users.UserId, aspnet_Users.UserName, UserNotifications_android.UserID, UserNotifications_android.Notifications, UserNotifications_android.RegistrationID
	FROM         aspnet_Users INNER JOIN
				UserNotifications_android ON UserNotifications_android.UserId = aspnet_Users.UserId
	WHERE     (aspnet_Users.UserName = @UserName)

END TRY

BEGIN CATCH
	DECLARE @ERROR_MSG AS VARCHAR(MAX)
	IF @@TRANCOUNT > 0 ROLLBACK
	SET @ERROR_MSG = 'qUserNotifications_Android FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserName'			+ ISNULL(CAST(@UserName AS VARCHAR(256)), 'Null') + CHAR(10)		
	RAISERROR(@ERROR_MSG,11,1)	
END CATCH

GO


