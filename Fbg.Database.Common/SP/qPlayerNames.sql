IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qIsPlayerInRealm')
	BEGIN
		DROP Procedure qIsPlayerInRealm
	END

GO

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerNameByUserAndRealm')
	BEGIN
		DROP Procedure qPlayerNameByUserAndRealm
	END

GO

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerNames')
	BEGIN
		DROP Procedure qPlayerNames
	END

GO

CREATE PROCEDURE qPlayerNames
    @userID uniqueidentifier
AS

select Name, RealmID from vplayers where userid = @userid order by realmid, playerid