IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'qUserNotificationRegID_android') AND type in (N'P', N'PC'))
	DROP PROCEDURE dbo.qUserNotificationRegID_android
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE dbo.qUserNotificationRegID_android
	@UserName AS VARCHAR(256)
	AS
	
BEGIN TRY
	
	SELECT     UserNotifications_android.RegistrationID
	FROM         aspnet_Users INNER JOIN
				UserNotifications_android ON UserNotifications_android.UserId = aspnet_Users.UserId
	WHERE     (aspnet_Users.UserName = @UserName)

END TRY

BEGIN CATCH
	DECLARE @ERROR_MSG AS VARCHAR(MAX)
	IF @@TRANCOUNT > 0 ROLLBACK
	SET @ERROR_MSG = 'dbo.qUserNotificationRegID_android FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserName'			+ ISNULL(CAST(@UserName AS VARCHAR(256)), 'Null') + CHAR(10)		
	RAISERROR(@ERROR_MSG,11,1)	
END CATCH


GO


