  
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dFilter')
	BEGIN
		DROP  Procedure  dFilter
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[dFilter]
		@PlayerID int,
		@FilterID int
AS
	
	
begin try 

	begin tran
	
	delete FilterTags from FilterTags 
	join Filters on Filters.FilterID=FilterTags.FilterID 
	where FilterTags.FilterID=@FilterID and Filters.playerid=@PlayerID
	
	delete from Filters 
	where Filterid=@FilterID and PlayerID=@PlayerID
	
	
	commit tran
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'dFilter FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @FilterID' + ISNULL(CAST(@FilterID AS VARCHAR(30)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 