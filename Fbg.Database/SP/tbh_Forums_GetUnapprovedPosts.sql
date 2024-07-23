IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_GetUnapprovedPosts')
	BEGIN
		DROP  Procedure  tbh_Forums_GetUnapprovedPosts
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[tbh_Forums_GetUnapprovedPosts]
(
   @ClanID     int
)
AS
SET NOCOUNT ON

SELECT tbh_Posts.PostID, tbh_Posts.AddedDate, tbh_Posts.AddedBy, tbh_Posts.AddedByIP, tbh_Posts.ForumID, tbh_Posts.ParentPostID, tbh_Posts.Title,tbh_Posts.Body, tbh_Posts.Approved, tbh_Posts.Closed, tbh_Posts.ViewCount, tbh_Posts.ReplyCount, tbh_Posts.LastPostDate, tbh_Posts.LastPostBy,Sticky,
   tbh_Forums.Title AS ForumTitle, CASE
                                    WHEN ParentPostID = 0 THEN 1
                                    ELSE 0
                                 END AS IsThreadPost
   FROM tbh_Posts INNER JOIN
      tbh_Forums ON tbh_Posts.ForumID = tbh_Forums.ForumID
   WHERE Approved = 0 and tbh_Forums.ClanID = @ClanID
   ORDER BY IsThreadPost DESC, AddedDate ASC 