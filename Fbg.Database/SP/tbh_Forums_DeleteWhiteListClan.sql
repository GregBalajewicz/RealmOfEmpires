   IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_DeleteWhiteListClan')
	BEGIN
		DROP  Procedure  tbh_Forums_DeleteWhiteListClan
	END

GO
--


CREATE PROCEDURE [dbo].[tbh_Forums_DeleteWhiteListClan]
(
   @WhiteListClanID int,
   @ClanID  int,
   @PlayerID int 
)
AS
SET NOCOUNT ON

BEGIN TRANSACTION DeleteWhiteListClan

	delete  from ForumSharing 
	from ForumSharing 
	join tbh_Forums  on ForumSharing.ForumID =tbh_Forums.ForumID 
	
	where tbh_Forums.ClanID=@ClanID and ForumSharing.ClanID =@WhiteListClanID 
	delete from ForumSharingWhiteListedClans 
	from ForumSharingWhiteListedClans fswlc
	join Clans c on fswlc.ClanID=c.ClanID
	 JOIN ClanMembers 
		ON c.ClanID = ClanMembers.ClanID 
	where c.ClanID=@ClanID and WhiteListClanID=@WhiteListClanID
	and ClanMembers.PlayerID =@PlayerID
	
	
	
	if @@ROWCOUNT >0
		begin 
			select 1
			
		end
	else
		begin 
			select 0	
			
		end

IF @@ERROR > 0
   BEGIN
   RAISERROR('Deletion of WhiteList Clan failed', 16, 1)
   ROLLBACK TRANSACTION DeleteWhiteListClan
   RETURN 99
   END

COMMIT TRANSACTION DeleteWhiteListClan



GO


