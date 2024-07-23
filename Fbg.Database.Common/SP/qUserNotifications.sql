IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[qUserNotifications]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[qUserNotifications]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO


CREATE PROCEDURE [dbo].[qUserNotifications]
	@UserName AS VARCHAR(256)
	AS
	
BEGIN TRY
	
	SELECT     aspnet_Users.UserId, aspnet_Users.UserName, UserNotifications.UserID, UserNotifications.Notifications, UserNotifications.RegistrationID, UserNotifications.DeviceType
	FROM         aspnet_Users INNER JOIN
				UserNotifications ON UserNotifications.UserId = aspnet_Users.UserId
	WHERE     (aspnet_Users.UserName = @UserName)

END TRY

BEGIN CATCH
	DECLARE @ERROR_MSG AS VARCHAR(MAX)
	IF @@TRANCOUNT > 0 ROLLBACK
	SET @ERROR_MSG = 'qUserNotifications FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserName'			+ ISNULL(CAST(@UserName AS VARCHAR(256)), 'Null') + CHAR(10)		
	RAISERROR(@ERROR_MSG,11,1)	
END CATCH


GO


