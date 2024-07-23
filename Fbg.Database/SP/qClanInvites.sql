IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qClanInvites')
	BEGIN
		DROP  Procedure  qClanInvites
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[qClanInvites]
		@ClanID int,
		@SearchName varchar(25)
AS
	
	
begin try 
	
	
			SELECT     ClanInvites.ClanID, ClanInvites.PlayerID, Players.Name,ClanInvites.InvitedOn
			FROM         ClanInvites INNER JOIN
							  Players ON ClanInvites.PlayerID = Players.PlayerID
			WHERE     (ClanInvites.ClanID = @ClanID) and Name like '%'+@SearchName+'%' 
				and datediff(day,getdate(),dateadd(day,10,ClanInvites.InvitedOn))>0
			order by invitedOn desc

		
	
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qClanInvites FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @ClanID' + ISNULL(CAST(@ClanID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	




 