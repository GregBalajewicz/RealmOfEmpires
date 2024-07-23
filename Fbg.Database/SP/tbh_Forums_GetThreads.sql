 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_GetThreads')
	BEGIN
		DROP  Procedure  tbh_Forums_GetThreads
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

 CREATE PROCEDURE [dbo].[tbh_Forums_GetThreads]
(
   @ForumID  int,
  @SortExpression  varchar(max),
  @LowerBound int,
  @UpperBound int,
  @PlayerID int
)
AS
SET NOCOUNT ON
   
 if (exists(SELECT   tbh_Forums.ForumID  
FROM         tbh_Forums INNER JOIN
                      tbh_Posts ON tbh_Forums.ForumID = tbh_Posts.ForumID 
                      INNER JOIN Clans 
						ON tbh_Forums.ClanID = Clans.ClanID 
					JOIN ClanMembers 
						ON Clans.ClanID = ClanMembers.ClanID
			
WHERE     (ClanMembers.PlayerID =@PlayerID and 
		   tbh_Posts.ForumID=@ForumID and 
		   tbh_Forums.Deleted=0  
		   and (tbh_forums.SecurityLevel = 0
				OR exists (select * from PlayerInRoles PIR WHERE PIR.PlayerID = @PlayerID AND RoleID in (select ID from dbo.fnGetPlayerRolesFromSecurityLevel(tbh_forums.SecurityLevel)))
			) )
			union all
			
			SELECT   tbh_Forums.ForumID  
FROM         tbh_Forums INNER JOIN
                      tbh_Posts ON tbh_Forums.ForumID = tbh_Posts.ForumID 
            JOIN ForumSharing  
					ON tbh_Forums.ForumID = ForumSharing.ForumID
			JOIN Clans 
					ON ForumSharing.ClanID=Clans.ClanID
			JOIN ClanMembers 
					ON Clans.ClanID = ClanMembers.ClanID
			
WHERE     (ClanMembers.PlayerID =@PlayerID and 
		   tbh_Posts.ForumID=@ForumID and 
		   tbh_Forums.Deleted=0  
		   and (tbh_forums.SecurityLevel = 0
				OR exists (select * from PlayerInRoles PIR WHERE PIR.PlayerID = @PlayerID AND RoleID in (select ID from dbo.fnGetPlayerRolesFromSecurityLevel(tbh_forums.SecurityLevel)))
			) )
			))
begin
SELECT * FROM
(
   SELECT tbh_Posts.PostID as ID, tbh_Posts.AddedDate, tbh_Posts.AddedBy, tbh_Posts.AddedByIP, 
   tbh_Posts.ForumID, tbh_Posts.ParentPostID, tbh_Posts.Title, tbh_Posts.Approved, 
   tbh_Posts.Closed, tbh_Posts.ViewCount, tbh_Posts.ReplyCount, tbh_Posts.LastPostDate, 
   tbh_Posts.LastPostBy, tbh_Forums.Title AS ForumTitle,PlayerPostViews.IsViewed as IsViewed,Sticky,
      ROW_NUMBER() OVER (ORDER BY @SortExpression) AS RowNum
      FROM         PlayerPostViews FULL OUTER JOIN
                      tbh_Posts ON PlayerPostViews.ThreadPostID = tbh_Posts.PostID  and PlayerPostViews.PlayerID =@PlayerID FULL OUTER JOIN
                      tbh_Forums ON tbh_Posts.ForumID = tbh_Forums.ForumID 
      WHERE tbh_Posts.ForumID = @ForumID AND ParentPostID = 0 AND Approved = 1
) ForumThreads
   WHERE ForumThreads.RowNum BETWEEN @LowerBound AND @UpperBound
   ORDER BY Sticky DESC ,LastPostDate  DESC
end