   IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qIsFolderEmpty')
	BEGIN
		DROP  Procedure  qIsFolderEmpty
	END

GO
-- returns 1 mean true ,0 mean false
CREATE Procedure qIsFolderEmpty
	@PlayerID as int,
	@FolderID as int,
	@FolderFor as smallint
AS
begin try 

if @FolderFor=0--delete mail
	begin
		if exists(	select * from MessageAddressees 
			where PlayerID=@PlayerID
			and FolderID=@FolderID)
			begin 			
				select 1;			
			end
		else
			begin
				select 0;
			end
			
	end
else if @FolderFor=1--delete report
	begin
	if exists(	select * from ReportAddressees 
			where PlayerID=@PlayerID
			and FolderID=@FolderID)
			begin 			
				select 1;			
			end
		else
			begin
				select 0;
			end
	end
		
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qIsFolderEmpty FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @FolderID' + ISNULL(CAST(@FolderID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @FolderFor' + ISNULL(CAST(@FolderFor AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	




 go

 