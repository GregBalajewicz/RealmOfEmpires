 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_GetThreadByID')
	BEGIN
		DROP  Procedure  tbh_Forums_GetThreadByID
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
 CREATE PROCEDURE [dbo].[tbh_Forums_GetThreadByID]
(
   @ThreadPostID  int,
  @PlayerID  int
)
AS
SET NOCOUNT ON
   
	SELECT distinct tbh_Posts.PostID
		, tbh_Posts.AddedDate
		, tbh_Posts.AddedBy
		, tbh_Posts.AddedByIP
		, tbh_Posts.ForumID
		, tbh_Posts.ParentPostID
		, tbh_Posts.Title
		, cast(tbh_Posts.Body as NVarchar(Max)) as Body
		, tbh_Posts.Approved
		, tbh_Posts.Closed
		, tbh_Posts.ViewCount
		, tbh_Posts.ReplyCount
		, tbh_Posts.LastPostDate
		, tbh_Posts.LastPostBy
		, Sticky
		, tbh_Forums.Title AS ForumTitle
	FROM tbh_Posts 
	JOIN tbh_Forums
		ON tbh_Posts.ForumID = tbh_Forums.ForumID
	JOIN Clans 
		ON tbh_Forums.ClanID = Clans.ClanID
	JOIN ClanMembers 
		ON Clans.ClanID = ClanMembers.ClanID
	
	WHERE 
		( PostID = @ThreadPostID OR ParentPostID = @ThreadPostID AND Approved = 1 )
		and ClanMembers.PlayerID = @PlayerID 
		and tbh_Forums.deleted = 0
		and (tbh_forums.SecurityLevel = 0
				OR exists (select * from PlayerInRoles PIR WHERE PIR.PlayerID = @PlayerID AND RoleID in (select ID from dbo.fnGetPlayerRolesFromSecurityLevel(tbh_forums.SecurityLevel)))
			) 
		union all
		SELECT distinct tbh_Posts.PostID
		, tbh_Posts.AddedDate
		, tbh_Posts.AddedBy
		, tbh_Posts.AddedByIP
		, tbh_Posts.ForumID
		, tbh_Posts.ParentPostID
		, tbh_Posts.Title
		, cast(tbh_Posts.Body as NVarchar(Max)) as Body
		, tbh_Posts.Approved
		, tbh_Posts.Closed
		, tbh_Posts.ViewCount
		, tbh_Posts.ReplyCount
		, tbh_Posts.LastPostDate
		, tbh_Posts.LastPostBy
		, Sticky
		, tbh_Forums.Title AS ForumTitle
	FROM tbh_Posts 
	JOIN tbh_Forums
		ON tbh_Posts.ForumID = tbh_Forums.ForumID
	 JOIN ForumSharing  
					ON tbh_Forums.ForumID = ForumSharing.ForumID
			JOIN Clans 
					ON ForumSharing.ClanID=Clans.ClanID
			JOIN ClanMembers 
					ON Clans.ClanID = ClanMembers.ClanID
	
	WHERE 
		( PostID = @ThreadPostID OR ParentPostID = @ThreadPostID AND Approved = 1 )
		and ClanMembers.PlayerID = @PlayerID 
		and tbh_Forums.deleted = 0
		and (tbh_forums.SecurityLevel = 0
				OR exists (select * from PlayerInRoles PIR WHERE PIR.PlayerID = @PlayerID AND RoleID in (select ID from dbo.fnGetPlayerRolesFromSecurityLevel(tbh_forums.SecurityLevel)))
			) 	
	ORDER BY AddedDate ASC
	
	--
	-- update post view flag if we successfully retrieved a post
	--
	IF @@rowcount <> 0 BEGIN
		declare @IsViewed as bit;
		select @IsViewed=IsViewed from PlayerPostViews where PlayerID=@PlayerID and ThreadPostID=@ThreadPostID
		if (ISNULL(CAST(@IsViewed AS VARCHAR(20)), 'Null')<> 'Null') begin
			if (@IsViewed<>'True') begin --doing this to avoid muliable updates on refresh		
				update PlayerPostViews set IsViewed='True' where PlayerID=@PlayerID and ThreadPostID=@ThreadPostID ;
			end 
		end else begin
			insert into PlayerPostViews (PlayerID,ThreadPostID,IsViewed)values(@PlayerID,@ThreadPostID,'True')
		end
	END
