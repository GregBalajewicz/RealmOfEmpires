   IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uClanSettings')
	BEGIN
		DROP  Procedure  uClanSettings
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE proc [dbo].uClanSettings
		@ClanID int
		,@InviterRoleFlag bit
as
	IF @InviterRoleFlag = 1 BEGIN
		IF not exists (select * from DefaultRoles where ClanID = @ClanID and RoleID = 2) BEGIN
			insert into DefaultRoles (ClanID, RoleID) values (@ClanID, 2) 
		END	
	END ELSE BEGIN
		delete DefaultRoles 
			where ClanID = @ClanID and RoleID = 2
			
	END


