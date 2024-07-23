IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_AllGlobalChats')
	BEGIN
		DROP Procedure qChat_AllGlobalChats
	END

GO

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChat_AllGlobalAndClanChats')
	BEGIN
		DROP Procedure qChat_AllGlobalAndClanChats
	END

GO

CREATE PROCEDURE qChat_AllGlobalAndClanChats
AS

--for each player of user, return all chats associated with the player

BEGIN
	select GroupId, gc.Name, gc.RealmID, ClanID, GroupType from GroupChat2 GC 
	left join realms R on 
		R.realmid = GC.RealmID 
	where 
		(GroupType = 0 or GroupType = 3)
		and (ActiveStatus = 1 or gc.realmid is null)

END