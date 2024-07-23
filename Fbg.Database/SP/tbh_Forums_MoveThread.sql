IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_MoveThread')
	BEGIN
		DROP  Procedure  tbh_Forums_MoveThread
	END

GO

CREATE PROCEDURE [dbo].[tbh_Forums_MoveThread]
(
	@ThreadPostID  int,
	@ForumID       int
)
AS

UPDATE tbh_Posts
   SET ForumID = @ForumID
   WHERE PostID = @ThreadPostID OR ParentPostID = @ThreadPostID 
   
   
   -- SECURITY CHECK???