    
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'Items2_iItem_ResearchSpeedup')
	BEGIN
		DROP  Procedure  Items2_iItem_ResearchSpeedup
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].Items2_iItem_ResearchSpeedup
		@expiresInHours int, -- maybe null
		@userid as uniqueidentifier,
		@playerID as int, -- maybe null 
		@amountInMinutes as int 


AS

begin try 
	declare @expiresOn DateTime
	if @expiresInHours is not null BEGIN 
		set @expiresOn = DATEADD(hour, @expiresInHours, getdate())
	END

	insert into Items (userid, playerid, ReceivedOn, ExpiresOn) values (@userid, @playerID, getdate(), @expiresOn)
	insert into Items_ResearchSpeedup(itemID, MinutesAmount) values (@@IDENTITY, @amountInMinutes)


end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'Items2_iItem_ResearchSpeedup FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @userid' + ISNULL(CAST(@userid AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 