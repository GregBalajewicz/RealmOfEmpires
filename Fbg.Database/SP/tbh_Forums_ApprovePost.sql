IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_ApprovePost')
	BEGIN
		DROP  Procedure  tbh_Forums_ApprovePost
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[tbh_Forums_ApprovePost]
(
	@PostID  int
)
AS

BEGIN TRANSACTION ApprovePost

UPDATE tbh_Posts SET Approved = 1 WHERE PostID = @PostID

-- get the approved post''s parent post and added date
DECLARE @ParentPostID   int
DECLARE @AddedDate      datetime
DECLARE @AddedBy        nvarchar(256)

SELECT @ParentPostID = ParentPostID, @AddedDate = AddedDate, @AddedBy = AddedBy
   FROM tbh_Posts
   WHERE PostID = @PostID

-- update the LastPostDate, LastPostBy and ReplyCount fields of the approved post''s parent post
IF @ParentPostID > 0
   BEGIN
   UPDATE tbh_Posts 
      SET ReplyCount = ReplyCount + 1, LastPostDate = @AddedDate, LastPostBy = @AddedBy
      WHERE PostID = @ParentPostID
   END
   
IF @@ERROR > 0
   BEGIN
   RAISERROR('Approval of post failed', 16, 1)
   ROLLBACK TRANSACTION ApprovePost
   RETURN 99
   END

COMMIT TRANSACTION ApprovePost
 