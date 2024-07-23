 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qGetUserAvatarData')
	BEGIN
		DROP  Procedure  qGetUserAvatarData
	END
GO

CREATE Procedure [dbo].qGetUserAvatarData	
	@UserID uniqueidentifier
AS

select * from Avatars2;
select * from UsersUnlockedAvatars where @UserID = UsersUnlockedAvatars.UserID;
