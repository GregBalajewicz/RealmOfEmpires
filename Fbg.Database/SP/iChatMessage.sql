IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iChatMessage')
	BEGIN
		DROP  Procedure  iChatMessage
	END

GO

CREATE PROCEDURE iChatMessage
	@PlayerID as int = null,
	@Msg  nvarchar(max),
	@IsClan as bit
AS
BEGIN    
	
	declare @ClanID int
	
	if @IsClan = 1 and @PlayerID is not null
		select @ClanID = cm.ClanID
		from ClanMembers cm
		where cm.PlayerID = @PlayerID
	else
		set @ClanID = null
	
	if @IsClan = 0 and (select COUNT(*) from Chat where PlayerID = @PlayerID and ClanID is null and Time > Dateadd(ss, -30, GETDATE()) and rtrim(ltrim(Msg)) = rtrim(ltrim(@Msg))) >= 3 begin
		select 1; -- Too many repeated posts
		return
	end	
	
	if @IsClan = 0 and (select COUNT(*) from Chat where PlayerID = @PlayerID and ClanID is null and Time > Dateadd(ss, -10, GETDATE())) >= 5 begin
		select 2; -- Too many posts
		return
	end
	

	
	insert into Chat(PlayerID, ClanID, Msg, [Time]) select @PlayerID, @ClanID, @Msg, GetDate()
	    where not (@ClanID is null) OR ( not exists (select * From playersuspensions where playerid = @playerid and SupensionID = 2))
	
	select 0
	
END
GO 
