 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iLoginLog')
	BEGIN
		DROP  Procedure  iLoginLog
	END

--
-- without this, was getting :
-- Heterogeneous queries require the ANSI_NULLS and ANSI_WARNINGS options to be set for the connection. This ensures consistent query semantics. Enable these options and then reissue your query.
--
SET ANSI_WARNINGS ON
SET ANSI_NULLS ON 
go

/*
*/
CREATE Procedure dbo.iLoginLog
	@PlayerID  int
	, @RemoteAddress varchar(15)
	, @RemotePort bigint
	, @Agent varchar(500)
	, @RealmID int
	, @LastLoginDate Datetime output

AS
set nocount on 
begin try 
	declare @AgentID int 	
	declare @Now datetime

	set @now = getdate()
	
	set @AgentID  = null
	select @AgentID = HTTP_USER_AGENT_ID from HTTP_USER_AGENTs where HTTP_USER_AGENT = @Agent

	if @AgentID is null begin 
		insert into HTTP_USER_AGENTs (HTTP_USER_AGENT) values (@Agent)
		
		SELECT top 1 @AgentID = HTTP_User_agent_ID  FROM HTTP_USER_AGENTs order by HTTP_User_agent_ID desc
	END 

	insert into LoginLog (RealmID, PlayerID, Time, REMOTE_ADDR, HTTP_USER_AGENT_ID, REMOTE_PORT)
		values (@RealmID, @PlayerID, @now, @RemoteAddress,  @AgentID, @RemotePort )

	--
	-- since this player logged in, he certainly is not to be deleted or warned of the deletion
	--
	delete from InactivePlayersToBeWarned where PlayerID = @PlayerID
	

	select top 1 @LastLoginDate = Time from loginlog where playerid = @PlayerID order by time desc

	UPDATE   dbo.aspnet_Users
    SET      LastActivityDate = dateadd(hour, datediff( hour, getdate(), getutcdate()), @LastLoginDate)
    WHERE    UserId = (select top 1 userid from Players where playerid = @PlayerID)


end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iLoginLog FAILED! '	+  CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:'	+ CHAR(10)
		+ '   @PlayerID'		+ ISNULL(CAST(@PlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @RemoteAddress'	+ ISNULL(CAST(@RemoteAddress AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @RemotePort'		+ ISNULL(CAST(@RemotePort AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @Agent'			+ ISNULL(CAST(@Agent AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AgentID'			+ ISNULL(CAST(@AgentID AS VARCHAR(max)), 'Null') + CHAR(10)

		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



