  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_IsPlayerInClan')
	BEGIN
		DROP  Procedure  [tbh_Forums_IsPlayerInClan]
	END

GO

 create PROCEDURE [dbo].[tbh_Forums_IsPlayerInClan]
(
   @ClanID  int,
	@PlayerID  int
)
AS
SET NOCOUNT ON

SELECT count(PlayerID)
   FROM ClanMembers
   WHERE ClanID = @ClanID and 
PlayerID=@PlayerID;
