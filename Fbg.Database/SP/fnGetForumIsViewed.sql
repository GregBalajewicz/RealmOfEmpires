	 
IF OBJECT_ID (N'dbo.fnGetForumIsViewed') IS NOT NULL
   DROP FUNCTION dbo.fnGetForumIsViewed
GO


CREATE FUNCTION [dbo].[fnGetForumIsViewed]
(
	@PlayerID int,
	@ForumID int
)
RETURNS int
AS
BEGIN

declare @Ret as int
declare @playerName as nvarchar(25);
declare @Count as int
declare @Min as int

	select @playerName=Name from Players where PlayerID=@PlayerID
	select @Count=count(*),@Min=min(dbo.fnConvertToInt(IsViewed)) from tbh_posts 
	left outer join  PlayerPostViews on PlayerPostViews.ThreadPostID = tbh_Posts.PostID and playerid=@PlayerID
	join tbh_Forums on tbh_Forums.ForumID=tbh_Posts.ForumID
	where tbh_Posts.forumid=@ForumID and lastpostby <>@playerName and parentpostid=0 and approved=1 
if (@Count>0)
	begin 
		set @Ret=@Min;--this means that there is some threads or posts that the player didn't see it
	end
else
	begin
		set @Ret=1;-- this mean that this forum don't have posts yet
	end
	
	RETURN @Ret

END
