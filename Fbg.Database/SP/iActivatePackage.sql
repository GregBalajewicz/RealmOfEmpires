 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iActivatePackage')
	BEGIN
		DROP  Procedure  iActivatePackage
	END

GO

CREATE Procedure dbo.iActivatePackage
	@PlayerID as int,
	@PackageID as int
AS

begin try 
	begin tran
	-- Inserts the data in Players table.
		
		select 10;
	commit tran
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iActivatePackage FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID'				  + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @PackageID'					  + ISNULL(CAST(@PackageID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



 