IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qOutgoingTroops2')
	BEGIN
		DROP  Procedure  qOutgoingTroops2
	END

GO

CREATE Procedure qOutgoingTroops2
	@LoggedInPlayerID as int
	, @ToPlayerID as int	-- IGNORED!! DO NOT USE
	, @ToVillageID as int	-- IGNORED!! DO NOT USE
	, @FromVillageID as int	-- IGNORED!! DO NOT USE
    , @showAttacks as bit	-- IGNORED!! DO NOT USE
    , @showSupports as bit	-- IGNORED!! DO NOT USE
    , @realRetrieve bit = 0 -- IGNORED!! DO NOT USE
    , @showHidden bit = 0   -- IGNORED!! DO NOT USE
	
AS

select UM.EventID			as EventID
	, UM.OriginVillageID	as OriginVillageID
	, UM.DestinationVillageID as DestinationVillageID
	, CommandType			as CommandType			
	, E.EventTime			as EventTime
	, DESTV.Name			as DestinationVillageName
	, DESTP.Name			as DestinationPlayerName
	, DESTP.PlayerID		as DestinationPlayerID
	, ORIGNV.Name			as OriginVillageName
	, ORGINP.Name			as OriginPlayerName --  redundand since we change this to show troops moving only from logged in player 
	, ORGINP.PlayerID		as OriginPlayerID   --  redundand since we change this to show troops moving only from logged in player 
	, DESTV.XCord			as DestVillageX
	, DESTV.YCord			as DestVillageY
	, ORIGNV.XCord			as OriginVillageX
	, ORIGNV.YCord			as OriginVillageY
	, UM.VisibleToTarget	as VisibleToTarget
	, isnull(PA.AttribID, 0)as Hidden
	, TripDuration			as TripDuration
	, (SELECT 1 FROM UnitMovements_Attributes WHERE EventID = E.EventID and AttribID = 26) as isUnderBloodLust
	, (SELECT AttribValue FROM UnitMovements_Attributes WHERE EventID = E.EventID and AttribID = 1) Morale
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
        and PA.playerid = @LoggedInPlayerID 
        and AttribID=1
	where 
	  
		E.Status = 0
    		
		-- we only show troops moving from logged in player 
		and ORGINP.PlayerID = @LoggedInPlayerID 		
    		
		-- we never show returns in outgoing since these are not our returns
		and UM.CommandType not in (2,3) /*2==return, 3=recall*/
    					
	order by EventTime asc
	
	
select distinct DestinationPlayerID as PlayerID, DestinationPlayerName as PlayerName from #mov 

select distinct DestinationVillageID as VID, DestinationVillageName as VName, DestVillageX as X, DestVillageY as Y from #mov 
union
select distinct OriginVillageID, OriginVillageName, OriginVillageX, OriginVillageY from #mov 

select 
EventID
	, OriginVillageID
	, DestinationVillageID
	, CommandType			
	, EventTime
	, DestinationPlayerID
	, VisibleToTarget
	, Hidden
	, TripDuration
	, isnull(isUnderBloodLust, 0)
	, isnull(Morale, 100)
 from #mov
 order by EventTime asc
	
select [TimeStamp] FROM PlayerCacheTimeStamps where PlayerID = @LoggedInPlayerID and CachedItemID = 2 /*outgoing*/
