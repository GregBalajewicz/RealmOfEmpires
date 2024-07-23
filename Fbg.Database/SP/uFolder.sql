    IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uFolder')
	BEGIN
		DROP  Procedure  uFolder
	END

GO
-- 0 means sucess ,1 means failed
CREATE Procedure uFolder
	@PlayerID int
	,@FolderID int
	,@FolderName nvarchar(30)
	,@FolderFor smallint
	
AS

BEGIN TRY

if not exists (select * from Folders where [Name] = @FolderName and FolderFor=@FolderFor)
	begin
		update Folders set [Name] = @FolderName
			where FolderID = @FolderID and PlayerID=@PlayerID
			
		select 0
	end
else
	begin
		select 1
	end
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'uFolder FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @FolderID' + ISNULL(CAST(@FolderID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @FolderName' + ISNULL(@FolderName, 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)

		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



