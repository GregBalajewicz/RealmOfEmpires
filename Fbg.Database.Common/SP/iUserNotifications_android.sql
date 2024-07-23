IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iUserNotifications_android')
	BEGIN
		DROP PROCEDURE dbo.iUserNotifications_android
	END
GO

CREATE PROCEDURE dbo.iUserNotifications_android
	@UserName AS VARCHAR(256) 
	,@Notifications AS BIT
	,@RegistrationID AS NVARCHAR(MAX)
	AS
	
BEGIN TRY
	BEGIN TRAN

	DECLARE @UserId AS UniqueIdentifier

	SELECT @UserId = UserId
	FROM         aspnet_Users
	WHERE     (aspnet_Users.UserName = @UserName)

	IF EXISTS (SELECT * FROM UserNotifications_android WHERE UserId = @UserId)	
		UPDATE UserNotifications_android
		   SET Notifications = @Notifications
			  ,RegistrationID = @RegistrationID
		 WHERE UserId = @UserId
		
	ELSE	  
		INSERT INTO UserNotifications_android
				   (UserId
				   ,Notifications
				   ,RegistrationID)
			 VALUES
				   (@UserId
				   ,@Notifications
				   ,@RegistrationID)
	  
	COMMIT TRAN
END TRY

BEGIN CATCH
	DECLARE @ERROR_MSG AS VARCHAR(MAX)
	IF @@TRANCOUNT > 0 ROLLBACK
	SET @ERROR_MSG = 'iUserNotifications_android FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserName'			+ ISNULL(CAST(@UserName AS VARCHAR(256)), 'Null') + CHAR(10)		
		+ '   @UserId'		+ ISNULL(CAST(@UserId AS VARCHAR(256)), 'Null') + CHAR(10)
		+ '   @Notifications'	+ ISNULL(CAST(@Notifications AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @RegistrationID'	+ ISNULL(CAST(@RegistrationID AS VARCHAR(MAX)), 'Null') + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
END CATCH

GO


