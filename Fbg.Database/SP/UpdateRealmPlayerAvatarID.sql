 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'UpdateRealmPlayerAvatarID')
	BEGIN
		DROP  Procedure  UpdateRealmPlayerAvatarID
	END

GO


CREATE Procedure [dbo].UpdateRealmPlayerAvatarID
		@PlayerID int,
		@AvatarID int
AS

--Because we are only technically duplicating common player information, 
--we dont check for unlocked avatars here

--note: this SP should NEVER be used to direclty set a player's avatarID
--this gets call only after the avatarID change has been verified by uPlayerSetAvatarID in common
update players set AvatarID = @AvatarID where PlayerID = @PlayerID;

GO
