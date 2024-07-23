 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qIncomingTroops3')
	BEGIN
		DROP  Procedure  qIncomingTroops3
	END

GO

/*
This SP assumes that the logged in player is the owner of the ToVillage, or, 
	if not specified, ToPlayerID is the ID of the logged in player.
	
Then, it shows all incoming troops, for any player to this village/all your villages. 

This will also show your Returns & Recalls

Because this SP show incoming from ALL players, its critical that 
	toPlayer and toVillage represent the logged in player. 
	
Added the "WITH RECOMPILE" because I found that when @ToVillageID=null, the querry plan would be terrible
    when executed by the website. on average this call ran in 1 second, immediatelly after adding this, the 
    average went down to 100MS

*/
CREATE Procedure qIncomingTroops3
	@ToPlayerID as int
	, @ToVillageID as int = null	-- IGNORED!! DO NOT USE
	, @FromVillageID as int = null	-- IGNORED!! DO NOT USE
	, @FromPlayerID as int = null	-- IGNORED!! DO NOT USE
    , @showAttacks as bit			-- IGNORED!! DO NOT USE
    , @showSupports as bit			-- IGNORED!! DO NOT USE
    , @showReturns as bit			-- IGNORED!! DO NOT USE
    , @realRetrieve bit = 0			-- IGNORED!! DO NOT USE
    , @showHidden bit = 0			-- IGNORED!! DO NOT USE

WITH RECOMPILE
AS

select UM.EventID			as EventID
	, (CASE UM.VisibleToTarget WHEN 1 THEN '-999' -- we hide the origin info if VisibleToTarget==1 meaning that origin is not supposed to be known
		ELSE UM.OriginVillageID			
		END)
		as OriginVillageID
	, UM.DestinationVillageID as DestinationVillageID
	, CommandType			as CommandType			
	, E.EventTime			as EventTime
	, DESTV.Name			as DestinationVillageName
	, (CASE UM.VisibleToTarget WHEN 1 THEN '' -- we hide the origin info if VisibleToTarget==1 meaning that origin is not supposed to be known
		ELSE ORIGNV.Name			
		END)
		as OriginVillageName
	, (CASE UM.VisibleToTarget WHEN 1 THEN '' -- we hide the origin info if VisibleToTarget==1 meaning that origin is not supposed to be known
		ELSE ORGINP.Name			
		END)
		as OriginPlayerName

	, (CASE UM.VisibleToTarget WHEN 1 THEN '-999' -- we hide the origin info if VisibleToTarget==1 meaning that origin is not supposed to be known
		ELSE ORGINP.PlayerID			
		END)
		as OriginPlayerID
	, UM.VisibleToTarget	as VisibleToTarget
	, DESTV.XCord			as DestVillageX
	, DESTV.YCord			as DestVillageY
	, (CASE UM.VisibleToTarget WHEN 1 THEN '-999' -- we hide the origin info if VisibleToTarget==1 meaning that origin is not supposed to be known
		ELSE ORIGNV.XCord			
		END)
		as OriginVillageX
	, (CASE UM.VisibleToTarget WHEN 1 THEN '-999' -- we hide the origin info if VisibleToTarget==1 meaning that origin is not supposed to be known
		ELSE ORIGNV.YCord			
		END)
		as OriginVillageY
	, isnull(PA.AttribID, 0)as Hidden
	, TripDuration
	into #mov
	from unitmovements UM with(nolock)
	join Events E with(nolock)
		on E.EventID = UM.EventID 
	join Villages DESTV with(nolock)
		on DESTV.VillageID = UM.DestinationVillageID
	join Villages ORIGNV with(nolock)
		on ORIGNV.VillageID = UM.OriginVillageID
	join Players DESTP with(nolock)
		on DESTP.PlayerID = DESTV.OwnerPlayerID 
	join Players ORGINP with(nolock)
		on ORGINP.PlayerID = ORIGNV.OwnerPlayerID 
    LEFT JOIN UnitMovements_PlayerAttributes PA with(nolock) 
        on PA.EventID = E.EventID
        and PA.playerid = @ToPlayerID 
        and AttribID=1
	where 
		E.Status = 0

		-- since this returns incoming troops, the TO PLAYER must always be specified. 
		and DESTP.PlayerID = @ToPlayerID

		-- only show visible incoming troops UNLESS these troops are your returns or recalls
		and ( UM.VisibleToTarget >= 1 OR UM.CommandType in (2,3) /*2==return, 3=recall*/)     		
	order by EventTime asc
	
select distinct OriginPlayerID as PlayerID, OriginPlayerName as PlayerName from #mov 
	where OriginPlayerID <> -999 -- remove "unknown origin" incoming

select distinct DestinationVillageID as VID, DestinationVillageName as VName, DestVillageX as X, DestVillageY as Y from #mov 
union
select distinct OriginVillageID, OriginVillageName, OriginVillageX, OriginVillageY from #mov   
	where VisibleToTarget  <> 1 -- remove "unknown origin" incoming


select 
	EventID
	, OriginVillageID
	, DestinationVillageID
	, CommandType			
	, EventTime
	, OriginPlayerID
	, VisibleToTarget
	, Hidden
	, TripDuration
 from #mov
	order by EventTime asc

select [TimeStamp] FROM PlayerCacheTimeStamps where PlayerID = @ToPlayerID and CachedItemID = 1 /*incoming */
 
	