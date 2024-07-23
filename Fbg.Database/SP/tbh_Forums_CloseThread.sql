IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_CloseThread')
	BEGIN
		DROP  Procedure  tbh_Forums_CloseThread
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[tbh_Forums_CloseThread]
(
	@ThreadPostID  int
)
AS

UPDATE tbh_Posts
   SET Closed = 1
   WHERE PostID = @ThreadPostID 