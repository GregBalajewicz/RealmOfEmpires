
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerInfo')
	BEGIN
		DROP  Procedure  qPlayerInfo
	END

GO
CREATE Procedure [dbo].qPlayerInfo
	@PID int
AS
begin try 

	select 
	
	isnull((select max(titleid) from PlayerTitleHistory PTH where pth.playerid = p.playerid),1)
	, RegisteredOn from players P where playerid = @PID
	select statid, max(statvalue) from playerstathistory where playerid = @PID 
	group by statid

	
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qPlayerInfo FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @@PID' + ISNULL(CAST(@PID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

  