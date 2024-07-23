    
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qInvites')
	BEGIN
		DROP  Procedure  qInvites
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].qInvites
		@UserID as uniqueidentifier
AS
	
	
begin try 


--
-- get the list of PENDING invites. ie, invites where the invited player has not yet joined
--
select  InviteID
	, InvitedID
	, InvitedOn
	from invites I
	left join aspnet_users U on I.InvitedID = U.UserName
	where StatusID = 1 -- status 1==pending invitation
		and Type = 1 -- means facebook invitation
		and PlayerID in (select playerid from players where userid = @UserID)
		and U.UserName is null
	order by InvitedID


end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qInvites FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserID' + ISNULL(CAST(@UserID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 