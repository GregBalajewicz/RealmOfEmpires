IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = '_PopulateDB_OpenRealm')
BEGIN
	DROP  Procedure  _PopulateDB_OpenRealm
END

GO


CREATE Procedure dbo._PopulateDB_OpenRealm
	@REALM_ID int
AS	
set nocount on

delete StoriesPublished where playerid in (select playerid from players  where RealmID = @REALM_ID)
delete GiftsSent where playerid in (select playerid from players  where RealmID = @REALM_ID)
delete InactivePlayersToBeWarned where playerid in (select playerid from players  where RealmID = @REALM_ID)


delete ChatMsgsBlockedUsers2 where Playerid in (select playerid from players where realmid = @REALM_ID ) or BlockedPlayerid in (select playerid from players where realmid = @REALM_ID )
delete ChatMsgs2 where GroupId in (select GroupId from GroupChat2 where realmid = @REALM_ID and grouptype = 0)
delete UsersToChats2 where GroupId in (select GroupId from GroupChat2 where realmid = @REALM_ID and grouptype = 0)
delete GroupChat2 where realmid = @REALM_ID and grouptype = 0 
insert into GroupChat2 values (NEWID(), 'Realm Chat', @REALM_ID, 0, null)


delete  players where RealmID = @REALM_ID
delete  deletedplayers where RealmID = @REALM_ID

