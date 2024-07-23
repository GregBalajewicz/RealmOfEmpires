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
	,@RemoteAddress varchar(15)
	,@RemotePort bigint
	,@Agent varchar(500)

AS
set nocount on 
begin try 
	declare @AgentID int 	
	declare @LastLoginDate Datetime

	declare @RealmID int
	select @RealmID = AttribValue from RealmAttributes where AttribID = 33

	--
	-- note this login, do some other thigns on login, and get the last logged in time
	--
	-- WAS : 
	-- EXEC FBGC.FBGCommon.dbo.iLoginLog @PlayerID, @RemoteAddress , @RemotePort , @Agent , @RealmID, @LastLoginDate out
	-- IS : 
	EXEC FBGCommon..iLoginLog @PlayerID, @RemoteAddress , @RemotePort , @Agent , @RealmID, @LastLoginDate out
	-- This change was made when we had performance trouble with r117, it was found that this ran a very long time from time to time. so made this performance improvement
	
	
	
	--
	-- figure out if player has taken any villages since his last login to this realm. 
	--	Used for story publishing. 
	--	
	IF EXISTS( select * from 
		villageownershiphistory VH
		where VH.CurrentOwnerPlayerID = @PlayerID
		and Date > @LastLoginDate
		and PreviousOwnerPlayerID <> -1
		) BEGIN 
		select 1 
	END ELSE BEGIN 
		select 0
	END 

	
	
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



