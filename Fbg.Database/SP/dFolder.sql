   
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dFolder')
	BEGIN
		DROP  Procedure  dFolder
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[dFolder]
		@PlayerID int,
		@FolderID int,
		@FolderFor smallint
AS
	
	
begin try 

	begin tran
	if @FolderFor=0--delete mail
	begin
		delete MessageAddressees  where PlayerID=@PlayerID and FolderID=@FolderID
	end
	else if @FolderFor=1--delete report 
	begin
		delete ReportInfoFlag where RecordId in (select recordid from ReportAddressees where PlayerID=@PlayerID and 	FolderID=@FolderID)
		delete ReportAddressees  where 	PlayerID=@PlayerID and 	FolderID=@FolderID
	end
		
	delete Folders where PlayerID=@PlayerID and FolderID=@FolderID
	IF  @@rowcount = 0  BEGIN
		select 1
		return
	end
	select 0
	commit tran
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'dFolder FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @FolderID' + ISNULL(CAST(@FolderID AS VARCHAR(30)), 'Null') + CHAR(10)
		+ '   @FolderFor' + ISNULL(CAST(@FolderFor AS VARCHAR(30)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 