IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayerNotificationSettings')
	BEGIN
		DROP  Procedure  uPlayerNotificationSettings
	END

GO
CREATE Procedure [dbo].uPlayerNotificationSettings
		@PlayerID int,
		@noficationID int,
		@vibrateOptionID smallint,
		@soundSettingID smallint,
		@activeStateID smallint
AS


begin try 
	
	update PlayerNotificationSettings 
	set	 
	    Vibrate= @vibrateOptionID,
	     SoundID= @soundSettingID,
		 isActive= @activeStateID
	where PlayerID = @PlayerID
		and NotificationID = @noficationID

	IF (@@rowcount < 1 ) BEGIN
		insert into PlayerNotificationSettings (Playerid, NotificationID ,Vibrate ,SoundID,isActive) values (@PlayerID , @noficationID ,@vibrateOptionID ,@soundSettingID ,@activeStateID)
	END



end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uPlayerNotificationSettings FAILED! ' +  + CHAR(10)
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