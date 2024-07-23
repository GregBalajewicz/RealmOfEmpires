IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_GetPostByID')
	BEGIN
		DROP  Procedure  tbh_Forums_GetPostByID
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[tbh_Forums_GetPostByID]
(
   @PostID  int,
   @PlayerID  int
)
AS
SET NOCOUNT ON
	
	if (exists(SELECT   tbh_Forums.ForumID       
FROM         tbh_Forums INNER JOIN
                      tbh_Posts ON tbh_Forums.ForumID = tbh_Posts.ForumID 
                      INNER JOIN Clans 
						ON tbh_Forums.ClanID = Clans.ClanID 
					  INNER JOIN ClanMembers 
						ON Clans.ClanID = ClanMembers.ClanID
					  
WHERE     (ClanMembers.PlayerID = @PlayerID 
			and tbh_Posts.PostID=@PostID
			and (tbh_forums.SecurityLevel = 0
				OR exists (select * from PlayerInRoles PIR WHERE PIR.PlayerID = @PlayerID AND RoleID in (select ID from dbo.fnGetPlayerRolesFromSecurityLevel(tbh_forums.SecurityLevel)))
			) )
			union all
			SELECT  tbh_Forums.ForumID     
FROM         tbh_Forums INNER JOIN
                      tbh_Posts ON tbh_Forums.ForumID = tbh_Posts.ForumID 
			JOIN ForumSharing  
					ON tbh_Forums.ForumID = ForumSharing.ForumID
			JOIN Clans 
					ON ForumSharing.ClanID=Clans.ClanID
			JOIN ClanMembers 
					ON Clans.ClanID = ClanMembers.ClanID
			  
					  
WHERE     (ClanMembers.PlayerID = @PlayerID 
			and tbh_Posts.PostID=@PostID
			and (tbh_forums.SecurityLevel = 0
				OR exists (select * from PlayerInRoles PIR WHERE PIR.PlayerID = @PlayerID AND RoleID in (select ID from dbo.fnGetPlayerRolesFromSecurityLevel(tbh_forums.SecurityLevel)))
			) )
			
			
			))--to verify that the player asks for his forum and did not play with Query String
begin
	


	SELECT tbh_Posts.PostID, tbh_Posts.AddedDate, tbh_Posts.AddedBy, tbh_Posts.AddedByIP, tbh_Posts.ForumID, tbh_Posts.ParentPostID, tbh_Posts.Title, tbh_Posts.Body, tbh_Posts.Approved, tbh_Posts.Closed, tbh_Posts.ViewCount, tbh_Posts.ReplyCount, tbh_Posts.LastPostDate, tbh_Posts.LastPostBy,Sticky,
   tbh_Forums.Title AS ForumTitle
   FROM tbh_Posts INNER JOIN
      tbh_Forums ON tbh_Posts.ForumID = tbh_Forums.ForumID
   WHERE PostID = @PostID




end 