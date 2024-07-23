    
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dMoveFolder')
	BEGIN
		DROP  Procedure  dMoveFolder
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[dMoveFolder]
		@PlayerID int,
		@FolderID int,
		@MoveToFolderID int,
		@FolderFor smallint
AS
	
	
begin try 
	if @MoveToFolderID=0 set @MoveToFolderID=null;

	begin tran
	if @FolderFor=0--delete mail
		begin
			update MessageAddressees set FolderID=@MoveToFolderID where PlayerID=@PlayerID and FolderID=@FolderID
			
			delete Folders where PlayerID=@PlayerID and FolderID=@FolderID
			
			IF  @@rowcount = 0  BEGIN
				select 1
				return
			end
			select 0
		end
	else if @FolderFor=1--delete report
		begin
			update ReportAddressees set  FolderID=@MoveToFolderID  where 	PlayerID=@PlayerID and 	FolderID=@FolderID
			delete Folders where PlayerID=@PlayerID and FolderID=@FolderID
			
			IF  @@rowcount = 0  BEGIN
				select 1
				return
			end
			select 0
		end
	commit tran
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'dMoveFolder FAILED! ' +  + CHAR(10)
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



 