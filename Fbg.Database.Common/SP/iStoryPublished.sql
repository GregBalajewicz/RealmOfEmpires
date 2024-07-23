  
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iStoryPublished')
	BEGIN
		DROP  Procedure  iStoryPublished
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].iStoryPublished
		@PlayerID int,
		@StoryID smallint,
		@body as nvarchar(max),
		@Status as smallint -- 0 means publish failed, 1 means publish successful
AS
	
	
begin try 


	insert into StoriesPublished(PlayerID, StoryID, Body, PublishStatus)
		values( @PlayerID, @StoryID, @Body, @Status)

				
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iStoryPublished FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @PlayerId' + ISNULL(CAST(@PlayerId AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @StoryID' + ISNULL(CAST(@StoryID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @Body' + ISNULL(CAST(@Body AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @Status' + ISNULL(CAST(@Status AS VARCHAR(max)), 'Null') + CHAR(10)
		
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



  