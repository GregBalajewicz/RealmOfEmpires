  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iFilterTags')
	BEGIN
		DROP  Procedure  iFilterTags
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Returns one result set with vote result fot specific poll
this SP used for 2 perposes 
1 - to insert vote for player and return voting result(if the OptionsIDs have values)
2- just get voting result for specific poll id (if the optionIDs is null)

*/
CREATE Procedure dbo.iFilterTags
	@PlayerID int,
	@FilterID int,
	@TagIDs varchar(max)

AS
set nocount on 
begin try 
begin tran 
	if @TagIDs is not null
		begin
		
			--clear the db form other choices done by this user .
			delete FilterTags from FilterTags
				join Filters 
					on Filters.FilterID=FilterTags.FilterID
				where Filters.PlayerID=@PlayerID
					and FilterTags.FilterID=@FilterID
		
			
			Declare @RecID int
			Declare @Pos int

			SET @TagIDs = LTRIM(RTRIM(@TagIDs))+ ','
			SET @Pos = CHARINDEX(',', @TagIDs, 1)
			
			
			while(@Pos>0)
			begin
				SET @RecID = LTRIM(RTRIM(LEFT(@TagIDs, @Pos - 1)))
				IF @RecID <> ''
				BEGIN
				-- insert the value of the new user choice
					if not exists (select * from FilterTags where FilterID=@FilterID and TagID =@RecID)
					begin
						insert into FilterTags (FilterID,TagID) 
						values (@FilterID,@RecID)
						
					end		
					
				END
				SET @TagIDs = RIGHT(@TagIDs, LEN(@TagIDs) - @Pos)
				SET @Pos = CHARINDEX(',', @TagIDs, 1)
			end
		
		end
	
commit tran
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iFilterTags FAILED! '	+  CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:'	+ CHAR(10)
		+ '   @PlayerID'				+ ISNULL(CAST(@PlayerID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @FilterID'		+ ISNULL(CAST(@FilterID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @TagIDs'		+ ISNULL(CAST(@TagIDs AS VARCHAR(100)), 'Null') + CHAR(10)

		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



