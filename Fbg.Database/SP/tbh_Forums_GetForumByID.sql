 
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_GetForumByID')
	BEGIN
		DROP  Procedure  tbh_Forums_GetForumByID
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[tbh_Forums_GetForumByID]
(
   @ForumID  int,
  @PlayerID  int
)
AS
SET NOCOUNT ON

if (exists(SELECT   tbh_Forums.ForumID   
FROM         tbh_Forums 
				INNER JOIN Clans 
					ON tbh_Forums.ClanID = Clans.ClanID 
				JOIN ClanMembers 
					ON Clans.ClanID = ClanMembers.ClanID
				

WHERE     (ClanMembers.PlayerID = @PlayerID and tbh_Forums.ForumID=@ForumID
and (tbh_forums.SecurityLevel = 0
				OR exists (select * from PlayerInRoles PIR WHERE PIR.PlayerID = @PlayerID AND RoleID in (select ID from dbo.fnGetPlayerRolesFromSecurityLevel(tbh_forums.SecurityLevel)))
			) )
			
			union all
			
			SELECT    tbh_Forums.ForumID   
FROM         tbh_Forums 
				JOIN ForumSharing  
					ON tbh_Forums.ForumID = ForumSharing.ForumID
				JOIN Clans 
					ON ForumSharing.ClanID=Clans.ClanID
				JOIN ClanMembers 
					ON Clans.ClanID = ClanMembers.ClanID
						

WHERE     (ClanMembers.PlayerID = @PlayerID and tbh_Forums.ForumID=@ForumID
and (tbh_forums.SecurityLevel = 0
				OR exists (select * from PlayerInRoles PIR WHERE PIR.PlayerID = @PlayerID AND RoleID in (select ID from dbo.fnGetPlayerRolesFromSecurityLevel(tbh_forums.SecurityLevel)))
			) )
			))
begin
	SELECT ForumID, AddedDate, AddedBy, Title, Moderated, Importance, Description, ImageUrl,ClanID,AlertClanMembers as AlertClanMembers,SecurityLevel
	FROM tbh_Forums
	WHERE ForumID = @ForumID 
end