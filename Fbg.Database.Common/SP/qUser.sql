IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qUser')
	BEGIN
		DROP  Procedure  qUser
	END

GO

CREATE Procedure dbo.qUser
	@userID uniqueidentifier
AS

select U.TimeZone, AU.UserName , ULT.UserLoginTypeID, U.GlobalPlayerName, U.AvatarID, U.Sex , PS.PublicReason, PS.SuspendedUntil
	from Users U
	join aspnet_users AU
		on AU.userid = U.userid
	left join UserLoginTypes ULT 
		on ULT.UserID = U.UserID
	left join PlayerSuspensions PS 
		on u.UserID = PS.UserID 
		and IsSuspensionActive = 1
    where U.UserID = @userID 


select P.PLayerID, P.RealmID, P.UserID, P.Name, null, P.AvatarID
	from players P 
	join Users U
		on P.UserID = U.UserID
	join aspnet_users AU
		on AU.userid = U.userid
  --  left join PlayerSuspensions PS 
		--on P.UserID = PS.UserID 
		--and IsSuspensionActive = 1
		--AND ( SuspendedUntil >= getdate() OR SuspendedUntil is null)
    where P.UserID = @userID 

select FlagID, UpdatedOn,Data from UserFlags where UserID= @userID
