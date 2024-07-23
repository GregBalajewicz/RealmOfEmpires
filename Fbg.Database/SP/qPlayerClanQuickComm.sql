 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerClanQuickComm')
	BEGIN
		DROP  Procedure  qPlayerClanQuickComm
	END

GO


/*
WHY the "with(nolock)" hint on all tables ?? because we really don't care about dirty reads here and this is executed so 
    often, that we want this to run as fast as possible. 
*/
CREATE Procedure [dbo].qPlayerClanQuickComm
		@PlayerID int
AS
	
	
begin try 
	if  exists (select * from clanmembers where playerid=@PlayerID)
		begin
			select 1

			SELECT top 3 P.AddedDate 
				, P.AddedBy, P.PostID, P.Title, substring(P.Body, 1, 100) body, F.Title, P.ParentPostID
				FROM
					tbh_Posts P with(nolock)
					INNER JOIN tbh_Forums F with(nolock)
						ON P.ForumID =		F.ForumID 
					INNER JOIN Clans with(nolock)
						ON F.ClanID = Clans.ClanID                     
					INNER JOIN ClanMembers with(nolock)
						ON Clans.ClanID = ClanMembers.ClanID
				where ClanMembers.PlayerID=@PlayerID 
				and deleted = 0 -- doing this JSUT to force it to use a better index
				and (F.SecurityLevel = 0
						OR exists (select * from PlayerInRoles PIR WHERE PIR.PlayerID = @PlayerID AND RoleID in (select ID from dbo.fnGetPlayerRolesFromSecurityLevel(F.SecurityLevel)))
					) 
				order by P.addeddate desc, P.title
		end
	
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qPlayerClanQuickComm FAILED! ' +  + CHAR(10)
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