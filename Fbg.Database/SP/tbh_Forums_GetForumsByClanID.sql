IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_GetForumsByClanID')
	BEGIN
		DROP  Procedure  tbh_Forums_GetForumsByClanID
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[tbh_Forums_GetForumsByClanID]
(
   @ClanID  int,
   @PlayerID int ,
   @ShowDeletedForums bit
)
AS
SET NOCOUNT ON

	--update that the player hae seen the foum page
	update players set ClanForumCheckedOn=getdate() where PlayerID=@PlayerID;

	SELECT  distinct 
		tbh_Forums.ForumID AS ID
		, tbh_Forums.AddedDate
		, tbh_Forums.AddedBy
		, tbh_Forums.Title
		, tbh_Forums.Moderated
		, tbh_Forums.Importance
		, tbh_Forums.Description
		, tbh_Forums.ImageUrl
		, Deleted
		, tbh_Forums.ClanID
		, tbh_Forums.AlertClanMembers as AlertClanMembers
		, tbh_Forums.SecurityLevel
		, dbo.fnGetForumIsViewed(@PlayerID,ForumID) as IsViewed
		
	from tbh_forums
	JOIN Clans 
		ON tbh_Forums.ClanID = Clans.ClanID 
	 JOIN ClanMembers 
		ON Clans.ClanID = ClanMembers.ClanID
	
	WHERE  
		tbh_Forums.ClanID = @ClanID 
		and Deleted=@ShowDeletedForums
		and ClanMembers.PlayerID=@PlayerID
		and (tbh_forums.SecurityLevel = 0
				OR exists (select * from PlayerInRoles PIR WHERE PIR.PlayerID = @PlayerID AND RoleID in (select ID from dbo.fnGetPlayerRolesFromSecurityLevel(tbh_forums.SecurityLevel)))
			) 
	order by tbh_Forums.Importance asc





