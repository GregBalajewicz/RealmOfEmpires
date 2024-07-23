 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iRaid_spawn_givePrivateRaids')
	BEGIN
		DROP  Procedure  iRaid_spawn_givePrivateRaids
	END

GO

CREATE Procedure [dbo].iRaid_spawn_givePrivateRaids
	@PrintDebugInfo BIT 
AS

DECLARE @DEBUG INT, @DBG varchar(10)
IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
	SET @DEBUG = 1
	set @DBG = ':::::::'
	SET NOCOUNT OFF
END ELSE BEGIN
	SET @DEBUG = 0
	SET NOCOUNT ON
END 
IF @DEBUG = 1 print  'BEGIN iRaid_spawn_givePrivateRaids ' 

--  
--		C  R  I  T  I  C  A  L
-- 
-- This SP expects a few temporary tables:
--
IF OBJECT_ID('tempdb..#ps') IS NULL BEGIN
	RETURN 
END 
IF OBJECT_ID('tempdb..#raidTemplatesToConsider') IS NULL BEGIN
	RETURN 
END 

--
-- for each player, give them a random raid 
-- 
declare @pid int 
declare @raidID int
declare @RarityRolled int
declare @RarityOfActuallyGivenRaid int
declare @randomRoll int
declare @playerVillCount int
select top 1 @pid = playerid from #Ps
WHILE (@pid is not null) BEGIN 
	
	-- player's village count
	SELECT @playerVillCount = count(*) from villages where ownerplayerid = @pid

	--
	-- Roll for rarity - select the rarity based on chance
	--
	set @randomRoll = rand() * 100
	if @randomRoll <= 30 BEGIN			-- 30%
		set @RarityRolled = 0
	END ELSE if @randomRoll <= 55 BEGIN -- 25%
		set @RarityRolled = 1
	END ELSE if @randomRoll <= 75 BEGIN -- 20% 
		set @RarityRolled = 2
	END ELSE if @randomRoll <= 90 BEGIN -- 15%
		set @RarityRolled = 3
	END ELSE BEGIN						-- 10 
		set @RarityRolled = 4
	END

	-- ', :' + cast( as varchar(max))
	IF @DEBUG = 1 print '@pid :' + cast(@pid as varchar(max)) +  '@RarityRolled:' + cast(@RarityRolled as varchar(max)) + ', @randomRoll:' + cast(@randomRoll as varchar(max)) + ',@playerVillCount :' + cast(@playerVillCount as varchar(max))


	-- get random raid of this rarity;	
	insert into Raids (CreatedOn, RaidTemplateID, PlayerCount)	
	select TOP 1 getdate(), RT.RaidTemplateID, 1
		from raidtemplate RT 
		where 
			RT.RaidTemplateID in (select RaidTemplateID from #raidTemplatesToConsider where (PlayerID = @pid or PlayerID is null))
			and RarityID = @RarityRolled 
		order by NEWID()
	set @raidID = @@IDENTITY

	if (@raidID is null) BEGIN 
		-- could not find a raid of this rarity, so just find any raid
		IF @DEBUG = 1 print  'could not find raid for this rarity, taking any raid of that rarity or less'
		insert into Raids (CreatedOn, RaidTemplateID ,PlayerCount)	
		select TOP 1 getdate(), RT.RaidTemplateID, 1
			from raidtemplate RT 
			where 
				RT.RaidTemplateID in (select RaidTemplateID from #raidTemplatesToConsider where (PlayerID = @pid or PlayerID is null))
				and RarityID <= @RarityRolled 
			order by NEWID()
		set @raidID = @@IDENTITY
	END
	-- given the code above, it is possible we did actually give the player of rarity he rolled. This could happen if not rarities are available to the player
	--	 at this time (like for example for very early private raids) 
	select @RarityOfActuallyGivenRaid = RarityID from raidtemplate where RaidTemplateID = (select raidtemplateid from raids where raidid = @raidID)

	if (@raidID is not null) BEGIN 
		-- random moster for that raid 
		insert into RaidMonster(MonsterTemplateID, RaidID, CurrentHealth, MaxHealth)
			select TOP 1 
				RMT.MonsterTemplateID
				, @raidID
				, RMT.BaseMaxHealth + ((@playerVillCount-1) * RMT.BaseMaxHealth * 0.75) + RMT.BaseMaxHealth * (@RarityOfActuallyGivenRaid * 0.1)
				, RMT.BaseMaxHealth + ((@playerVillCount-1) * RMT.BaseMaxHealth * 0.75) + RMT.BaseMaxHealth * (@RarityOfActuallyGivenRaid * 0.1)
			from RaidTemplatePossibleMonsters RTPM 
			JOIN RaidMonsterTemplate RMT 
				on RMT.MonsterTemplateID = RTPM.MonsterTemplateID
			where RTPM.RaidTemplateID = (select R.RaidTemplateID from Raids R where R.RaidID = @raidID)
			order by NEWID()	

		-- make it a private raid
		insert into PrivateRaids(RaidID, PlayerID) values(  @raidID, @pid ) 
	END 

	IF @DEBUG = 1 print '@pid :' + cast(@pid as varchar(max)) +  ' @RaidID:' + cast(@RaidID as varchar(max)) 


	delete #ps where playerid = @pid
	SET @PID = null
	select top 1 @pid = playerid from #Ps
END

IF @DEBUG = 1 print  'END iRaid_spawn_givePrivateRaids ' 
