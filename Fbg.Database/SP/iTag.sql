 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iTag')
	BEGIN
		DROP  Procedure  iTag
	END

GO
 
 SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--- 1 if the name is exists ,
--- 0 if the tag name not exists even the player have a filter name with the same name or not
CREATE Procedure [dbo].[iTag]
		@PlayerID int,
		@TagName nvarchar(30) 
		
		
AS
	
begin try 
	declare @TagID int
	declare @FilterID int
	
	begin tran
		if not exists (select * from tags where Name=@TagName and Playerid=@PlayerID ) begin
			insert into tags (Name,Description,Sort,PlayerID) 
			values(@TagName,'',0,@PlayerID)
			
			set @TagID=@@identity;
			if  not exists (select * from filters where Name=@TagName and Playerid=@PlayerID ) begin
				insert into Filters (Name,Description,Sort,PlayerID) 
				values(@TagName,'',0,@PlayerID)
				
				set @FilterID=@@identity;
				
				insert into FilterTags (FilterID,TagID)
				values (@FilterID,@TagID)
			end
			select 0;--sucess
		end	else begin
			select 1;--failed this tag already exsits 
		end
	commit tran
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iTag FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @TagName:' + ISNULL(@TagName , 'Null') + CHAR(10)
		+ '   @PlayerID:' + ISNULL(CAST(@PlayerID AS VARCHAR(100)) , 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

 
 