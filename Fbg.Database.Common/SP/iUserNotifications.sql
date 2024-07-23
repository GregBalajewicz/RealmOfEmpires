IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[iUserNotifications]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[iUserNotifications]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO


CREATE PROCEDURE [dbo].[iUserNotifications]
	@UserName AS VARCHAR(256) 
	,@Notifications AS BIT
	,@RegistrationID AS NVARCHAR(MAX)
	,@DeviceType AS INT
	AS
	
BEGIN TRY
	BEGIN TRAN

	DECLARE @UserId AS UniqueIdentifier

	SELECT @UserId = UserId
	FROM         aspnet_Users
	WHERE     (aspnet_Users.UserName = @UserName)

	IF EXISTS (SELECT * FROM UserNotifications WHERE UserId = @UserId)	
		UPDATE UserNotifications
		   SET Notifications = @Notifications
			  ,RegistrationID = @RegistrationID
			  ,DeviceType = @DeviceType
		 WHERE UserId = @UserId
		
	ELSE	  
		INSERT INTO UserNotifications
				   (UserId
				   ,Notifications
				   ,RegistrationID
				   ,DeviceType)		   
			 VALUES
				   (@UserId
				   ,@Notifications
				   ,@RegistrationID
				   ,@DeviceType)
	  
	COMMIT TRAN
END TRY

BEGIN CATCH
	DECLARE @ERROR_MSG AS VARCHAR(MAX)
	IF @@TRANCOUNT > 0 ROLLBACK
	SET @ERROR_MSG = 'iUserNotification FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserName'			+ ISNULL(CAST(@UserName AS VARCHAR(256)), 'Null') + CHAR(10)		
		+ '   @UserId'		+ ISNULL(CAST(@UserId AS VARCHAR(256)), 'Null') + CHAR(10)
		+ '   @Notifications'	+ ISNULL(CAST(@Notifications AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @RegistrationID'	+ ISNULL(CAST(@RegistrationID AS VARCHAR(MAX)), 'Null') + CHAR(10)
		+ '   @DeviceType'	+ ISNULL(CAST(@DeviceType AS VARCHAR(10)), 'Null') + CHAR(10)		
	RAISERROR(@ERROR_MSG,11,1)	
END CATCH


GO


