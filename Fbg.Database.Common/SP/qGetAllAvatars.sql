 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qGetAllAvatars')
	BEGIN
		DROP  Procedure  qGetAllAvatars
	END
GO

CREATE Procedure [dbo].qGetAllAvatars	
AS

select * from Avatars;