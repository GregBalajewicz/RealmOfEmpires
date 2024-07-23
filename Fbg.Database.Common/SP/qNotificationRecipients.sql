  
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qNotificationRecipients')
	BEGIN
		DROP  Procedure  qNotificationRecipients
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].qNotificationRecipients
		@NotificationID as int
		, @IsTest bit
AS
--
-- THERE IS A PROBLEM with notifications!!
--	this SP marks the entire result set as a send attempt yet the UI may only do a part of this. 
--	SO, this sp shoudl somehow know how many are actually going to be sent. 
--
	
begin try 
	create table #tmp (email varchar(500), PlayerName varchar(25) null, RealmID int null)

	--
	-- All users
	--
	if @NotificationID = 0 BEGIN 
	
		insert into #tmp
		
		select email from aspnet_membership 
			select top 10000 email from aspnet_membership where email <> 'nobody@facebook.user'
			select count(*) from aspnet_membership where email <> 'nobody@facebook.user'

		select  AU.username as facebookid, '', 0
			from aspnet_users AU
			where 
				AU.LastActivityDate < dateadd(day,  35, getdate()) 
				and username not in (select facebookid from notificationssent where notificationid = @NotificationID)
			order by lastactivitydate desc
	END
			
	
	
	--
	-- remove any users we don't want 
	--
	delete #tmp	where FacebookID in ('-1', '-2', '0', '-999') -- special users
				or FacebookID in (select UserName from aspnet_users U join PlayerSuspensions PS 
									on U.UserID = PS.UserID where U.UserName = Facebookid ) -- suspended users
									
	--
	-- remove from notification any users who have be attempted to get this notification at least 10 times
	--  and we have no record of them getting this notification
	--
	-- IE, this is a crude way to eliminiate ppl who probably disallowed notifications or removed the game altogether. 
	--
    delete from #tmp where FacebookID in (
      select FacebookID from notificationsendattempts NA 
	    where notificationid = @NotificationID 
	    group by facebookid having count(*) > 10
    )
									
	--
	-- if test do not log this 
	--		
	if @IsTest = 0 begin
		insert into notificationsendattempts 
			select distinct @NotificationID,  facebookid, getdate() from #tmp
	end


	IF @NotificationID = 3 BEGIN 
		select FacebookID, PlayerName, RealmID from #tmp	
	END ELSE BEGIN
		select distinct FacebookID from #tmp	
	END 
	
	
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qNotificationRecipients FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @NotificationID' + ISNULL(CAST(@NotificationID AS VARCHAR(30)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 
