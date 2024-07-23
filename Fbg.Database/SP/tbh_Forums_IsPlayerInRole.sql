  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_IsPlayerInRole')
	BEGIN
		DROP  Procedure  tbh_Forums_IsPlayerInRole
	END

GO

CREATE PROCEDURE [dbo].[tbh_Forums_IsPlayerInRole]
(
  @PlayerID  int,
  @ClanID  int,
 @RoleID  int
)
AS
SET NOCOUNT ON


SELECT    count(PlayerInRoles.RoleID) 
FROM         PlayerInRoles
where PlayerInRoles.playerid=@PlayerID and PlayerInRoles.RoleID=@RoleID and PlayerInRoles.ClanID=@ClanID


