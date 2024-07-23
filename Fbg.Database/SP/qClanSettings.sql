  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qClanSettings')
	BEGIN
		DROP  Procedure  qClanSettings
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE proc [dbo].qClanSettings
		@ClanID int
as
	select 
		* from DefaultRoles 
		where ClanID = @ClanID



