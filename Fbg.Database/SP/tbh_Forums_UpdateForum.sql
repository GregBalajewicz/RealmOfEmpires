  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_UpdateForum')
	BEGIN
		DROP  Procedure  tbh_Forums_UpdateForum
	END

go

CREATE PROCEDURE [dbo].[tbh_Forums_UpdateForum]
(
	@ForumID       int,
	@Title         nvarchar(256),
	@Moderated   bit,
	@Importance    int,
	@Description   nvarchar(4000),
	@ImageUrl      nvarchar(256),
	@AlertClanMembers      bit,
	@SecurityLevel tinyint
)
AS

UPDATE tbh_Forums
   SET Title = @Title, Moderated = @Moderated, Importance = @Importance, Description = @Description, ImageUrl = @ImageUrl ,AlertClanMembers=@AlertClanMembers,SecurityLevel=@SecurityLevel
   WHERE ForumID = @ForumID

-- TODO security check.