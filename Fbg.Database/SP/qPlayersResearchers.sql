 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayersResearchers')
	BEGIN
		DROP  Procedure  qPlayersResearchers
	END

GO
CREATE Procedure qPlayersResearchers
	@PlayerID as int
AS
begin try 

	if not exists (select COUNT(*) FROM Researchers where playerid = @PlayerID group by playerid) begin 
		select 0 
	end else begin
		select COUNT(*) FROM Researchers where playerid = @PlayerID group by playerid
	end 
			
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qPlayersResearchers FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	




 go

