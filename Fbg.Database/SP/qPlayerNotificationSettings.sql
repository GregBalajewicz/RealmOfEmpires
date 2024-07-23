IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerNotificationSettings')
	BEGIN
		DROP  Procedure  qPlayerNotificationSettings
	END

GO
CREATE Procedure [dbo].qPlayerNotificationSettings
		@PlayerID int
AS


begin try 
	
	SELECT
	    t.NotificationID,
	    isnull(s.Vibrate, t.vibrate) ,
	     isnull(s.SoundID, t.SoundID) ,
		 isnull(s.isActive, t.isActive) ,
		 name,
		 description 
		 
	from PlayerNotificationSettings S
	right join NotificationSettings_template t on S.NotificationID = t.NotificationID
	and PlayerID = @PlayerID

end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qPlayerNotificationSettings FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @@PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	 