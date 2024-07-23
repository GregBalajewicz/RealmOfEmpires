   IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qFilterByID')
	BEGIN
		DROP  Procedure  qFilterByID
	END

GO
CREATE Procedure qFilterByID
	@PlayerID as int,
	@FilterID as int
AS
begin try 

	select FilterID,Name,Description,Sort,PlayerID from Filters 
	where PlayerID=@PlayerID and FilterID=@FilterID
	order by [Sort] asc
	
	select tags.TagID,tags.Name ,
			isnull((select filtertags.filterid 
					from filtertags join filters on filters.FilterID=FilterTags.FilterID 
					where tagid=tags.TagID and filters.PlayerID=@PlayerID  
						and filters.filterid=@FilterID),0) as checked
	from tags 
	where playerid=@PlayerID
	order by [sort]
		
		
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qFilterByID FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @FilterID' + ISNULL(CAST(@FilterID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	




 go

