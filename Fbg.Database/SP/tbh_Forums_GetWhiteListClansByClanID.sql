   IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_GetWhiteListClansByClanID')
	BEGIN
		DROP  Procedure  tbh_Forums_GetWhiteListClansByClanID
	END

GO
--

CREATE PROCEDURE [dbo].[tbh_Forums_GetWhiteListClansByClanID]
(
   @ClanID  int,
   @PlayerID int 
)
AS
SET NOCOUNT ON

	
	SELECT c.Name ,fswlc.WhiteListClanID  FROM ForumSharingWhiteListedClans fswlc 
	JOIN Clans c ON c.ClanID =fswlc.WhiteListClanID
	
	 JOIN ClanMembers 
		ON fswlc.ClanID = ClanMembers.ClanID
	
	WHERE fswlc.ClanID =@ClanID  
	and ClanMembers.PlayerID = @PlayerID 





