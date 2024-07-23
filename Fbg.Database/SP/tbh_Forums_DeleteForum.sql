IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_DeleteForum')
	BEGIN
		DROP  Procedure  tbh_Forums_DeleteForum
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[tbh_Forums_DeleteForum]
(
	@ForumID int
)
AS
update tbh_Forums set Deleted=1 where ForumID = @ForumID; 