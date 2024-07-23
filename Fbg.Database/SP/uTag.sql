  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uTag')
	BEGIN
		DROP  Procedure  uTag
	END

GO

CREATE Procedure uTag
	@TagID int
	,@TagName nvarchar(30)
	,@TagDesc nvarchar(75)
	,@TagSort smallint
	,@PlayerID int
AS

BEGIN TRY
	update Tags set [Name] = @TagName,Description=@TagDesc,Sort=@TagSort
	where TagID = @TagID 
		and PlayerID=@PlayerID
		and not exists (select * from Tags where [Name] = @TagName and TagID <> @TagID and PlayerID=@PlayerID )

	IF @@rowcount <> 1 BEGIN
		select 1
	END ELSE BEGIN
		select 0
	END 
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'uTag FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @TagID' + ISNULL(CAST(@TagID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @TagName' + ISNULL(@TagName, 'Null') + CHAR(10)
		+ '   @TagDesc' + ISNULL(@TagDesc, 'Null') + CHAR(10)
		+ '   @TagSort' + ISNULL(CAST(@TagSort AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)

		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



