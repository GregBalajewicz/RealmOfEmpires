IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qThroneRoomLike')
	BEGIN
		DROP  Procedure  qThroneRoomLike
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].qThroneRoomLike
	@userid uniqueidentifier ,
	@pid int =null
AS

	if @userid is null begin
		select @userid = userid from vplayers where playerid = @pid
	end
	if @userid is null begin
		-- invalid data
		select 0
		return 
	END
	--
	-- if player id is specified, we only keep it if this realm has been hidden by the owner of this player
	-- 
	IF @PID is not null BEGIN 
		if (not exists (select * from tr_PlayerListSettings where userid = @userid and playerid = @pid and displayStatus = 0 )) BEGIN
			set @PID = null
		END
	END 

	--
	-- we get the # of likes. 
	--	if we still have the @PID, this means that this is a hidden realm, so only grab likes for that realm
	--	if we do not have @PID, this means we are getting likes for the user, so remove likes from the hidden realms
	--
	select count(*) from tr_likes L
		where userid = @userid 
		and (@pid is null OR PlayerID = @pid) -- if we still have the @PID, this means that this is a hidden realm, so only grab likes for that realm
		and (@pid is not null OR PlayerID is null ) -- if we do not have @PID, this means we are getting likes for the user, so remove likes from the hidden realms