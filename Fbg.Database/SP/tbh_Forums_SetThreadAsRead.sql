IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_SetThreadAsRead')
	BEGIN
		DROP  Procedure  tbh_Forums_SetThreadAsRead
	END

GO

CREATE PROCEDURE [dbo].[tbh_Forums_SetThreadAsRead]
(
   @ThreadPostID  int,
  @PlayerID  int
)
AS
SET NOCOUNT ON
   
 if (exists(SELECT   *  
FROM         tbh_Forums INNER JOIN
                      tbh_Posts ON tbh_Forums.ForumID = tbh_Posts.ForumID INNER JOIN
                      Clans ON tbh_Forums.ClanID = Clans.ClanID INNER JOIN
                      ClanMembers ON Clans.ClanID = ClanMembers.ClanID
WHERE     (ClanMembers.PlayerID = @PlayerID and tbh_Posts.PostID=@ThreadPostID)))
begin
	declare @IsViewed as bit;
	select @IsViewed=IsViewed from PlayerPostViews where PlayerID=@PlayerID and ThreadPostID=@ThreadPostID
	if (ISNULL(CAST(@IsViewed AS VARCHAR(20)), 'Null')<> 'Null')
		begin
			if (@IsViewed<>'True')--doing this to avoid muliable updates on refresh
				begin 
					update PlayerPostViews set IsViewed='True' where PlayerID=@PlayerID and ThreadPostID=@ThreadPostID ;
				end 
		end
	else
		begin
			insert into PlayerPostViews (PlayerID,ThreadPostID,IsViewed)values(@PlayerID,@ThreadPostID,'True')
		end
end

