   IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dAccountStewardsForInactivity')
	BEGIN
		DROP  Procedure  dAccountStewardsForInactivity
	END

GO
--
-- this is an admin function called to delete stewardship for ppl inactive 30 days or more 
--
CREATE Procedure dbo.dAccountStewardsForInactivity
	AS

begin try 
	BEGIN TRAN
	

    set nocount on

    select S.* into #PlayerToCancelS from players P
	    join AccountStewards S
		    on P.PlayerId = S.PlayerID 
    where dateadd(day, 30, LastActivity) < getdate()
    select * from #PlayerToCancelS -- FOR DEBUGGING

    declare @pid int
    declare @stewardPid int
    declare @now datetime
    set @now = getdate()


    select top 1 @pid = playerid,@stewardPid = stewardPlayerID  from #PlayerToCancelS 
    while @pid is not null BEGIN
	    delete from #PlayerToCancelS where playerid = @pid


	    Delete AccountStewards
		    where StewardPlayerID = @stewardPid
		    and PlayerID  = @pid
	    IF @@rowcount >= 1 begin 
		    insert into AccountStewardLog(ActingPlayerId, EventTypeID, Time, Notes)
			    values (@pid , 7 ,@now , @stewardPid)
			print @pid -- FOR DEBUGGING
	    END 
	    set @pid = null 
	    select top 1 @pid = playerid,@stewardPid = stewardPlayerID  from #PlayerToCancelS 
    END

    --select * from dbo.AccountStewardLog where eventtypeid = 7
    	
	COMMIT TRAN
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'dAccountStewardsForInactivity FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   ERROR_NUMBER():'			+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		+ ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		+ ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			+ ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'         + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



   