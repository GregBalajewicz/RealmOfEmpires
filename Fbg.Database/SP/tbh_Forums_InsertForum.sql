 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_InsertForum')
	BEGIN
		DROP  Procedure  tbh_Forums_InsertForum
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
 CREATE PROCEDURE [dbo].[tbh_Forums_InsertForum]
(
   @AddedDate     datetime,
   @AddedBy       nvarchar(256),
   @Title         nvarchar(256),
   @Moderated   bit,
   @Importance    int,
   @Description   nvarchar(4000),
   @ImageUrl      nvarchar(256),   
	@ClanID       int ,
	@AlertClanMembers bit,
	@SecurityLevel tinyint,
   @ForumID       int OUTPUT
)
AS
SET NOCOUNT ON

-- check whether a forum with the same name already exists
DECLARE @CurrID int
SELECT @CurrID = ForumID FROM tbh_Forums WHERE
   LOWER(@Title) = LOWER(Title) and ClanID =@ClanID 

IF @CurrID IS NOT NULL
   BEGIN
   SET @ForumID = -1
   RETURN
   END
   
INSERT INTO tbh_Forums 
   (AddedDate, AddedBy, Title, Moderated, Importance, Description, ImageUrl,ClanID,AlertClanMembers,SecurityLevel)
   VALUES (@AddedDate, @AddedBy, @Title, @Moderated, @Importance, @Description, @ImageUrl,@ClanID,@AlertClanMembers,@SecurityLevel)

SET @ForumID = scope_identity()