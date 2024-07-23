 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_IncrementViewCount')
	BEGIN
		DROP  Procedure  tbh_Forums_IncrementViewCount
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
 CREATE PROCEDURE [dbo].[tbh_Forums_IncrementViewCount]
(
   @PostID  int
)
AS
   
UPDATE tbh_Posts
   SET ViewCount = ViewCount + 1
   WHERE PostID = @PostID