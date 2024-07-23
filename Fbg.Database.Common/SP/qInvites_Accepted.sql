    
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qInvites_Accepted')
	BEGIN
		DROP  Procedure  qInvites_Accepted
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].qInvites_Accepted
		@UserID as uniqueidentifier
AS
	
	
begin try 

select  InviteID
	, InvitedID
	, P.PlayerID 
	, P.RealmID
	, I.StatusID
	, P.UserID
	from invites I
	join aspnet_users U 
		on I.InvitedID = U.UserName
	join Players P -- player who accepted the invite
		on P.UserID = U.UserID
	where StatusID in( 2,3) -- 2==accepted invitation, 3==accepted invite & reward accepted
		and Type = 1 -- means facebook invitation
		and I.PlayerID in (select playerid from players where userid = @UserID)
	order by P.UserID, StatusID -- must be orderd this way! code relies on this!
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qInvites_Accepted FAILED! ' +  + CHAR(10)
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



 