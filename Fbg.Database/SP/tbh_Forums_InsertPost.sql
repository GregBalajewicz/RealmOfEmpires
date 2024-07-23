IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_InsertPost')
	BEGIN
		DROP  Procedure  tbh_Forums_InsertPost
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[tbh_Forums_InsertPost]
(
   @AddedDate        datetime,
   @AddedBy          nvarchar(256),
   @AddedByIP        nchar(15),
   @ForumID          int,
   @ParentPostID     int,
   @Title            nvarchar(256),
   @Body             ntext,
   @BodyForChat      nvarchar(max),
   @Approved         bit,
   @Closed           bit,
   @Sticky			bit,
	@PlayerID		int,
   @PostID           int OUTPUT
   
)
AS
SET NOCOUNT ON
if (exists(SELECT   tbh_Forums.ForumID    
FROM         tbh_Forums 
			INNER JOIN Clans 
				ON tbh_Forums.ClanID = Clans.ClanID 
			INNER JOIN ClanMembers 
				ON Clans.ClanID = ClanMembers.ClanID
			  
WHERE     (ClanMembers.PlayerID = @PlayerID and (tbh_forums.SecurityLevel = 0
				OR exists (select * from PlayerInRoles PIR WHERE PIR.PlayerID = @PlayerID AND RoleID in (select ID from dbo.fnGetPlayerRolesFromSecurityLevel(tbh_forums.SecurityLevel)))
			) 
			 and tbh_Forums.ForumID=@ForumID
		   )
		   union all
		   
		   SELECT   tbh_Forums.ForumID     
FROM         tbh_Forums 
			JOIN ForumSharing  
					ON tbh_Forums.ForumID = ForumSharing.ForumID
			JOIN Clans 
					ON ForumSharing.ClanID=Clans.ClanID
			JOIN ClanMembers 
					ON Clans.ClanID = ClanMembers.ClanID
			  
WHERE     (ClanMembers.PlayerID = @PlayerID and (tbh_forums.SecurityLevel = 0
				OR exists (select * from PlayerInRoles PIR WHERE PIR.PlayerID = @PlayerID AND RoleID in (select ID from dbo.fnGetPlayerRolesFromSecurityLevel(tbh_forums.SecurityLevel)))
			) 
			 and tbh_Forums.ForumID=@ForumID
		   )
		   
		   ))
begin
	BEGIN TRANSACTION InsertPost
	 
		INSERT INTO tbh_Posts 
			(AddedDate, AddedBy, AddedByIP, ForumID, ParentPostID, Title, Body, Approved, Closed, LastPostDate, LastPostBy,Sticky)
			VALUES (@AddedDate, @AddedBy, @AddedByIP, @ForumID, @ParentPostID, @Title, @Body, @Approved, @Closed, @AddedDate, @AddedBy,@Sticky)

		SET @PostID = scope_identity()

		-- if the post is approved, update the parent post''s ReplyCount and LastReplyDate fields
		IF @Approved = 1 AND @ParentPostID > 0 BEGIN
			UPDATE tbh_Posts SET ReplyCount = ReplyCount + 1, LastPostDate = @AddedDate, LastPostBy = @AddedBy
			  WHERE PostID = @ParentPostID
		END
	
	COMMIT TRANSACTION InsertPost


	if  @ParentPostID > 0 begin
		--
		-- this is not a new thread but a report to an existing thread. 
		--
		update PlayerPostViews set IsViewed='False' where   ThreadPostID=@ParentPostID ;--new post is inserted update all players with it
		
		declare @ParentIsViewed as bit;
		select @ParentIsViewed=IsViewed from PlayerPostViews where PlayerID=@PlayerID and ThreadPostID=@ParentPostID
		if (ISNULL(CAST(@ParentIsViewed AS VARCHAR(20)), 'Null')<> 'Null') begin
			if (@ParentIsViewed<>'True')--doing this to avoid muliable updates on refresh 
			begin 
				update PlayerPostViews set IsViewed='True' where PlayerID=@PlayerID and ThreadPostID=@ParentPostID ;
			end 
		end else begin
			insert into PlayerPostViews (PlayerID,ThreadPostID,IsViewed)values(@PlayerID,@ParentPostID,'True')
		end
	end	else begin 
		--
		-- this is a new thread. mark it as read for poster
		--
		insert into PlayerPostViews (PlayerID,ThreadPostID,IsViewed)values(@PlayerID,@PostID,'True')
	end 


	--
	-- for forums open to all players, and marked with "alert clan members", post this post to the chat
	--
	if ( exists 
		(SELECT   tbh_Forums.ForumID    
		 FROM         tbh_Forums 			  
		 WHERE     tbh_forums.SecurityLevel = 0
				 and tbh_Forums.ForumID=@ForumID
				 and tbh_Forums.AlertClanMembers = 1 
		)
	) 
	BEGIN
		declare @actualChatbody nvarchar(max) 
		declare @datafpid int
		if  @ParentPostID > 0 begin
			set @datafpid = @ParentPostID -- this is a reply, so use parent ID
		END ELSE BEGIN 
			set @datafpid = @PostID -- this is a new post, so use this ID 
		END
		set @actualChatbody = '<a class=fp data-fpid=' 
			+ cast(@datafpid as nvarchar(max))
			+ '>'
			+ SUBSTRING(@Title, 0, 15)
			+ ':</a> ' + @BodyForChat

		

		exec iChatMessage @PlayerID, @actualChatbody , 1
	END 
end





 