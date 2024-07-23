IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uUserSetSex')
	BEGIN
		DROP  Procedure  uUserSetSex
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO
CREATE Procedure dbo.uUserSetSex
	@UserID uniqueidentifier ,
	@Sex int
	
	
AS
-- 
--
--
begin try 
	begin tran

		Update Users set Sex = @Sex where UserID = @UserID	
		if @@rowcount < 1 begin 
			--
			-- the above update is know to fail for one one reason - this is a user who never registerd on a realm, therefore has 
			--	no record in users table; see iRegisterPlayerInCommon
			--
			insert into Users(UserID, Credits ,GlobalPlayerName,Sex) values (@UserID, 25, null, @Sex)
		END

	commit tran
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'uUserSetSex FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserID:'					  + ISNULL(CAST(@UserID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @Sex:'					  + ISNULL(@Sex, 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



