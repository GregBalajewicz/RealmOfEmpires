IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[qRealms]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[qRealms]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO


CREATE Procedure [dbo].[qRealms]
AS

	select R.RealmID
		, R.Name
		, R.Description
		, count(P.playerID) as PlayerCount
		, Version
		, ConnectionStr
		, MaxPlayers
		, OpenOn
		, AllowPrereg
		, ActiveStatus
		, ExtendedDesc
		, EndsOn
		, PG.UserID
		, PG.entrypasscode
	from Realms R 
		left join Players P on R.RealmID = P.RealmID
		left join playersuspensions PS on PS.UserId = P.UserID and IsSuspensionActive = 1
		left join Realms_PlayerGenerated PG on R.realmid = PG.RealmID 
	where PS.UserID is null and ActiveStatus > 0 
	group by R.RealmID, R.Name, R.Description, Version, SizeX, SizeY, ConnectionStr,MaxPlayers, OpenOn, AllowPrereg, ActiveStatus, ExtendedDesc, EndsOn, PG.UserID, entrypasscode
	order by RealmId

	select * from CreditPackages

	exec qPolls

	select * from CreditPackages_Device order by credits asc

	select AvatarID, AvatarType, ImageUrlS, ImageUrlL, Info, Cost from Avatars2
	--select AvatarID, AttribID, AttribValue from AvatarAttributes

GO


