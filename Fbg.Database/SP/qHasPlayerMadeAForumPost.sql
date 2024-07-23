 
 
 
 
  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qHasPlayerMadeAForumPost')
	BEGIN
		DROP  Procedure  qHasPlayerMadeAForumPost
	END

GO


CREATE Procedure qHasPlayerMadeAForumPost
       @PlayerName varchar(max)
AS


select count(*) from tbh_posts where addedby = @PlayerName