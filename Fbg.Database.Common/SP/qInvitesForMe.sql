    
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qInvitesForMe')
	BEGIN
		DROP  Procedure  qInvitesForMe
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].qInvitesForMe
		@InvitedID as varchar(320)
AS
	
	
begin try 


--
-- get the list of invitations for me 
--
select  InviteID
	, I.PlayerID
	, InvitedOn
	, U.UserName as FacebookID
	, P.RealmID
	, P.Name
	from invites I
	join Players P
		on P.PlayerID = I.PlayerID
	join aspnet_users U
		on U.UserID = P.UserID
	where StatusID = 1 -- status 1==pending invitation
		and Type = 1 -- means facebook invitation
		and I.invitedid = @InvitedID
	order by InvitedOn Asc


end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qInvitesForMe FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @InvitedID' + ISNULL(CAST(@InvitedID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 