 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iBadEmail')
	BEGIN
		DROP  Procedure  iBadEmail
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE PROC iBadEmail
	@email                         varchar(256)        
	,@bounceType                   varchar(20)        = null 
	,@bounceSubType                varchar(20)        = null
as
begin try
	IF NOT EXISTS(select * from BadEmails where email = @email ) BEGIN
		insert into BadEmails(email, bounceType, bounceTypeSub ) values (@email, @bounceType, @bounceSubType) 
	END 
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iBadEmail FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @@email' + ISNULL(CAST(@email AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)
end catch