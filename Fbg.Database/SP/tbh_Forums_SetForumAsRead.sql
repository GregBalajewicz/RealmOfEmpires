 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_SetForumAsRead')
	BEGIN
		DROP  Procedure  tbh_Forums_SetForumAsRead
	END

GO

CREATE PROCEDURE [dbo].[tbh_Forums_SetForumAsRead]
(
   @ForumID  int,
  @PlayerID  int
)
AS
SET NOCOUNT ON
   
 if (exists(SELECT   *  
FROM         tbh_Forums INNER JOIN
                      tbh_Posts ON tbh_Forums.ForumID = tbh_Posts.ForumID INNER JOIN
                      Clans ON tbh_Forums.ClanID = Clans.ClanID INNER JOIN
                      ClanMembers ON Clans.ClanID = ClanMembers.ClanID
WHERE     (ClanMembers.PlayerID = @PlayerID and tbh_Posts.ForumID=@ForumID)))
begin
			--update all the isViewed to true for all posts
			update PlayerPostViews set IsViewed='True' 
			from PlayerPostViews join tbh_Posts on tbh_Posts.ParentPostID=PlayerPostViews.ThreadPostID 
			where PlayerID=@PlayerID and ForumID=@ForumID ;
					
			-- insert record for all forum posts
			insert into PlayerPostViews 
			select @PlayerID,tbh_Posts.PostID,'True'
			from tbh_Posts where ForumID=@ForumID and ParentPostID=0 and not exists(select * from PlayerPostViews where PlayerID=@PlayerID and ThreadPostID=tbh_Posts.PostID )
			
	
end

