IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iThroneRoomLike')
	BEGIN
		DROP  Procedure  iThroneRoomLike
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].iThroneRoomLike
	@LikerIP varchar(25),
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
	-- if this IP did not yet like this TR, then do it. This is not thread safe but we ignore that. 
	--
	IF (not exists (
		SELECT * from tr_likes where userid = @userid and LikerIP = @LikerIP AND (@pid is null OR PlayerID = @pid)	
	)) BEGIN
		insert into tr_likes (userid, playerid, LikerIP) values (@userid, @pid, @LikerIP)
	END
