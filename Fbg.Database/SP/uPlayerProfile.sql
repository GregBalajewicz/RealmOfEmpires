IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayerProfile')
	BEGIN
		DROP  Procedure  uPlayerProfile
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create Proc uPlayerProfile
    @PlayerID as int, @text as ntext
as

IF exists(select * from playerprofile where playerid= @playerid) BEGIN

	update PlayerProfile
	set [text] = @text where playerid= @playerid
END ELSE BEGIN
    INSERT into PlayerProfile (playerid, [text]) values (@playerid, @text)
    
    
END