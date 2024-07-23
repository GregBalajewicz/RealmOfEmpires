 
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dTag')
	BEGIN
		DROP  Procedure  dTag
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[dTag]
		@PlayerID int,
		@TagID int
AS
	
	
begin try 

	begin tran
	
	DELETE FilterTags 
		FROM FilterTags
		join Tags T on T.TagID=FilterTags.TagID
		WHERE 
			FilterTags.TagID=@TagID 
			and T.playerid=@PlayerID
	
	delete VillageTags from VillageTags 
	join tags on tags.TagID=VillageTags.TagID 
	where VillageTags.TagID=@TagID and tags.playerid=@PlayerID
	
	delete from Tags 
	where tagid=@TagID and PlayerID=@PlayerID
	
	
	commit tran
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'dTag FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @TagID' + ISNULL(CAST(@TagID AS VARCHAR(30)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 