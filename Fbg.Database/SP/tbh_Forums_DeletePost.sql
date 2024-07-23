IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_DeletePost')
	BEGIN
		DROP  Procedure  tbh_Forums_DeletePost
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

 CREATE PROCEDURE [dbo].[tbh_Forums_DeletePost]
(
	@PostID  int
	,@PlayerID int
)
AS

BEGIN TRANSACTION DeletePost
-- ROLES for reference
--Owner = 0,
--Inviter = 2,
--Administrator = 3,
--ForumAdministrator = 4,
--Diplomat =5

if (exists(SELECT   tbh_Forums.ForumID   
	FROM tbh_Forums 
		JOIN tbh_Posts 
			ON tbh_Forums.ForumID = tbh_Posts.ForumID 
		JOIN Clans C
			ON tbh_Forums.ClanID = C.ClanID 
		JOIN ClanMembers CM
			ON C.ClanID = CM.ClanID
		left outer JOIN PlayerInRoles PIR
			on PIR.PlayerID = @PlayerID
			and PIR.ClanID = C.ClanID
		JOIN Players P
			on P.PlayerID = @PlayerID
		
	WHERE   
		tbh_Posts.PostID=@PostID
		
		-- player must be a member of the clan
		and CM.PlayerID = @PlayerID 
		
		-- player must be the one who added the post OR player is an admin
		and (P.Name = tbh_Posts.AddedBy OR PIR.RoleID in (0,3,4))
		and (tbh_forums.SecurityLevel = 0
				OR exists (select * from PlayerInRoles PIR WHERE PIR.PlayerID = @PlayerID AND RoleID in (select ID from dbo.fnGetPlayerRolesFromSecurityLevel(tbh_forums.SecurityLevel)))
			) 
			
			union all
			
			SELECT   tbh_Forums.ForumID   
	FROM tbh_Forums 
		JOIN ForumSharing  
			ON tbh_Forums.ForumID = ForumSharing.ForumID
		JOIN tbh_Posts 
			ON tbh_Forums.ForumID = tbh_Posts.ForumID 
		JOIN Clans C
			ON ForumSharing.ClanID = C.ClanID 
		JOIN ClanMembers CM
			ON C.ClanID = CM.ClanID
		left outer JOIN PlayerInRoles PIR
			on PIR.PlayerID = @PlayerID
			and PIR.ClanID = C.ClanID
		JOIN Players P
			on P.PlayerID = @PlayerID
		
	WHERE   
		tbh_Posts.PostID=@PostID
		
		-- player must be a member of the clan
		and CM.PlayerID = @PlayerID 
		
		-- player must be the one who added the post OR player is an admin
		and (P.Name = tbh_Posts.AddedBy OR PIR.RoleID in (0,3,4))
		and (tbh_forums.SecurityLevel = 0
				OR exists (select * from PlayerInRoles PIR WHERE PIR.PlayerID = @PlayerID AND RoleID in (select ID from dbo.fnGetPlayerRolesFromSecurityLevel(tbh_forums.SecurityLevel)))
			) 
			
			
		))
begin
	DECLARE @ParentPostID   int

	SELECT @ParentPostID = ParentPostID FROM tbh_Posts WHERE PostID = @PostID
	  delete from PlayerPostViews where ThreadPostID=@ParentPostID or ThreadPostID=@PostID
	DELETE tbh_Posts
	   WHERE PostID = @PostID OR ParentPostID = @PostID

	UPDATE tbh_Posts 
		  SET ReplyCount = ReplyCount - 1
		  WHERE PostID = @ParentPostID
end
IF @@ERROR > 0
   BEGIN
   RAISERROR('Deletion of post failed', 16, 1)
   ROLLBACK TRANSACTION DeletePost
   RETURN 99
   END

COMMIT TRANSACTION DeletePost
