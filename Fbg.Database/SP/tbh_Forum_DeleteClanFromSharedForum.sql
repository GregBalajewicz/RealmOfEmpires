   IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_DeleteClanFromSharedForum')
	BEGIN
		DROP  Procedure  tbh_Forums_DeleteClanFromSharedForum
	END

GO
--

create PROCEDURE [dbo].[tbh_Forums_DeleteClanFromSharedForum]
(
   @SharedForumID int,
   @SharedWithClanID int,
   @ClanID  int,
   @PlayerID int 
)
AS
SET NOCOUNT ON

	delete from ForumSharing  
	where ForumID =@SharedForumID and ClanID=@SharedWithClanID
	
	if (@@ROWCOUNT>0)
	  select 1
	else 
	  select 0		





GO


