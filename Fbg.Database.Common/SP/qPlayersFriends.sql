IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayersFriends')
	BEGIN
		DROP  Procedure  qPlayersFriends
	END
GO

CREATE Procedure qPlayersFriends
	(
		@PlayerID int
	)

AS
	declare @userid uniqueidentifier 
	select top 1 @userid = userid from players where playerid = @playerid

	select P.PlayerID from Usersfriends UF
	join players P
		on P.userid = UF.FriendUserID
		and P.RealmID = (select RealmID from players where playerid = @playerid)
	where UF.userid = @userid
	