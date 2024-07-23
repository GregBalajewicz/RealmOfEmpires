 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_UpdatePost')
	BEGIN
		DROP  Procedure  [tbh_Forums_UpdatePost]
	END

go
CREATE PROCEDURE [dbo].[tbh_Forums_UpdatePost]
(
	@PostID        int,
	@Title         nvarchar(256),
	@Body          ntext,
	@Sticky			bit,
	@PlayerID          int
)
AS

-- ROLES for reference
--Owner = 0,
--Inviter = 2,
--Administrator = 3,
--ForumAdministrator = 4,
--Diplomat =5

if (exists(SELECT  tbh_Forums.ForumID 
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
		left outer join PlayerInRoles
			on PlayerInRoles.ClanID=CM.ClanID and PlayerInRoles.PlayerID=CM.PlayerID
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
		left outer join PlayerInRoles
			on PlayerInRoles.ClanID=CM.ClanID and PlayerInRoles.PlayerID=CM.PlayerID
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
	UPDATE tbh_Posts
	   SET Title = @Title,
		  Body = @Body,
		  Sticky=@Sticky
	   WHERE PostID = @PostID
end


