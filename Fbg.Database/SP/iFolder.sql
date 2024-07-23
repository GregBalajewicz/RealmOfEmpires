  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iFolder')
	BEGIN
		DROP  Procedure  iFolder
	END

GO
 
 SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--- 1 if the name is exists ,
--- 0 if the filter name not exists 

CREATE Procedure [dbo].[iFolder]
		@PlayerID int,
		@FolderName nvarchar(30) ,
		@FolderFor smallint
		
		
AS
	
begin try 
		begin tran
			if not exists (select * from Folders where Name=@FolderName and Playerid=@PlayerID and FolderFor=@FolderFor)
			begin
				insert into Folders (Name,FolderFor,PlayerID) 
				values(@FolderName,@FolderFor,@PlayerID)
				select 0;--sucess
			end
			else
			begin
				select 1;-- failed this filter already exists 
			end
			
		commit tran
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iFolder FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @FolderName:' + ISNULL(@FolderName , 'Null') + CHAR(10)
		+ '   @PlayerID:' + ISNULL(CAST(@PlayerID AS VARCHAR(100)) , 'Null') + CHAR(10)
		+ '   @FolderFor:' + ISNULL(CAST(@FolderFor AS VARCHAR(100)) , 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

 
  