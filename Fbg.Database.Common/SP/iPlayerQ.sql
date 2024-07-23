 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iPlayerQ')
	BEGIN
		DROP  Procedure  iPlayerQ
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO
CREATE Procedure dbo.iPlayerQ
	@UserID uniqueidentifier ,
	@RealmID integer 

AS
declare @Error as int;
begin try 
	begin tran
	-- Inserts the data in Players table.
		if  not exists(select PlayerQ.* from PlayerQ where RealmID=@RealmID and UserID=@UserID)
		begin
			insert into PlayerQ (RealmID,UserID,DateEntered) values ( @RealmID, @UserID, getdate())
			set @Error=0;
			select @Error
		end 
	else
		set @Error=1;
		select @Error
		
	commit tran
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iPlayerQ FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @RealmID:'				  + ISNULL(CAST(@RealmID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @UserID:'					  + ISNULL(CAST(@UserID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @Error:'				  + ISNULL(CAST(@Error AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



