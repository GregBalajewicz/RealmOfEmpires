  
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iNotificationSent')
	BEGIN
		DROP  Procedure  iNotificationSent
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[iNotificationSent]
		@FacebookIDs as varchar(max),--this holds the ID's of the Facebook Members separated by Comma ','  ex:(1,2,3)
		@NotificationID as int
AS
	
	
begin try 
	declare @time as datetime
	set @time =getdate();

	insert into NotificationsSent
		Select @NotificationID,ID,@time from fnGetIds(@FacebookIDs)

	--
	-- do a special action if neccessary
	--
	declare @UserID uniqueidentifier
	IF @NotificationID = 2  BEGIN
		--
		-- give 100 credits
		--
		select UserID into #tmp
			from NotificationsSent NS 
			join aspnet_users U 
				on NS.FacebookID = U.userName 
				and NS.Time = @time

		select top 1 @UserID = userID from #tmp
		while (@UserID is not null) BEGIN
			delete from #tmp where UserID = @UserID

			if  not exists (select * from Users where UserID = @UserID) begin 
				insert into Users(UserID, Credits) values (@UserID, 0)
			end 

			exec uGiveServants @UserID, 100, 5

			set @UserID = null
			select top 1 @UserID = userID from #tmp
		END
	END
				
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iNotificationSent FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @FacebookIDs' + ISNULL(CAST(@FacebookIDs AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @NotificationID' + ISNULL(CAST(@NotificationID AS VARCHAR(30)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 