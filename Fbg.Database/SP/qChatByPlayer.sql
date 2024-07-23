IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qChatByPlayer')
	BEGIN
		DROP  Procedure  qChatByPlayer
	END

GO

CREATE PROCEDURE [dbo].[qChatByPlayer]
	@PlayerID as int = null,
	@IsClan as bit,
	@Time as datetime,
	@Count as int = 10,
	@Older bit = 0
AS
BEGIN
	--
	-- get the clan of the player
	--
	declare @ClanID int
	set @ClanID = -1
	select @ClanID = cm.ClanID
	from ClanMembers cm
	where cm.PlayerID = @PlayerID
	
	
	if @IsClan = 1 and @PlayerID is not null begin
	    --
	    -- if this is retrieve for the clan chat, return a message if 
	    --  player is not part of the clan
	    --
		if (@ClanID = -1) begin
			select 'Not in clan' as Mes
			return
		end
	end 
	
	declare @top int
	
	if (Year(@Time) <> YEAR(GetDate()) or @Older = 1) set rowcount @Count
	
	select p.Name as PlayerName, c.*, Image1Url = 'https://static.realmofempires.com/images/icons/M_chooseavatar.png', p.AvatarID,
		Title = CASE p.Sex
			 WHEN 0 THEN t.Title_Female
			 ELSE  T.Title_Male	
		  END,
		  P.XP_cached
	from Chat c
		inner join Players p on p.PlayerID = c.PlayerID
		--join Avatars A on a.avatarid = p.avatarid
		join Titles T on t.TitleID = P.titleID
	where 
		c.PlayerID not in (select BlockedPlayerID from dbo.MessagesBlockedPlayers mbp where mbp.PlayerID = @PlayerID) and
		((c.[Time] > @Time and @Older = 0) or (c.[Time] < @Time and @Older = 1)) and
		((c.ClanID = @ClanID) or (c.ClanID is null and @IsClan=0) )  -- (c.ClanID is null and @IsClan = 1) and (c.ClanID = @ClanID or @IsClan = 0)
	order by [Time] desc	
	set rowcount 0
	
END
GO