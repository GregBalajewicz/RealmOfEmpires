IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_UnDeleteForum')
	BEGIN
		DROP  Procedure  tbh_Forums_UnDeleteForum
	END

GO

CREATE PROCEDURE [dbo].[tbh_Forums_UnDeleteForum]
(
	@ForumID int
)
AS
update tbh_Forums set Deleted=0 where ForumID = @ForumID;

-- TODO SECURITY CHECK