   IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uFilter')
	BEGIN
		DROP  Procedure  uFilter
	END

GO

CREATE Procedure uFilter
	@FilterID int
	,@FilterName nvarchar(30)
	,@FilterDesc nvarchar(75)
	,@FilterSort smallint
	,@PlayerID int
AS

BEGIN TRY
		update Filters set [Name] = @FilterName,Description=@FilterDesc,Sort=@FilterSort
			where FilterID = @FilterID 
				and PlayerID=@PlayerID
				and not exists (select * from Filters where [Name] = @FilterName and FilterID <> @FilterID and PlayerID=@PlayerID)
	IF @@rowcount <> 1 BEGIN
		select 1
	END ELSE BEGIN
		select 0
	END 
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'uFilter FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @FilterID' + ISNULL(CAST(@FilterID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @FilterName' + ISNULL(@FilterName, 'Null') + CHAR(10)
		+ '   @FilterDesc' + ISNULL(@FilterDesc, 'Null') + CHAR(10)
		+ '   @FilterSort' + ISNULL(CAST(@FilterSort AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)

		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



