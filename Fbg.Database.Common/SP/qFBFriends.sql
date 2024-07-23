IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qFBFriends')
	BEGIN
		DROP Procedure qFBFriends
	END

GO

CREATE Procedure qFBFriends
	@LoggedInUserID uniqueidentifier 	
AS

	--
	-- get the new list of friends. 
	--
	Select 
		FriendUserID
		, u.username as facebookid
		, (select count(*) from Players p where p.userid = FriendUserID) as  numRealms

		from UsersFriends uf 
		join aspnet_users u 
			on uf.FriendUserID = u.userid	
	where uf.UserID=@LoggedInUserID
	
 