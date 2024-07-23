IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qOutgoingTroops')
	BEGIN
		DROP  Procedure  qOutgoingTroops
	END

GO

CREATE Procedure qOutgoingTroops
	@LoggedInPlayerID as int
	, @ToPlayerID as int
	, @ToVillageID as int
	, @FromVillageID as int
    , @showAttacks as bit
    , @showSupports as bit
    , @realRetrieve bit = 0  
    , @showHidden bit = 0   
	
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
	   @realRetrieve = 1
		and  ( 
		    E.Status = 0
    		
		    -- IF destination player is specified, show movements headed to this player's villages only
		    and (DESTP.PlayerID  = @ToPlayerID OR  @ToPlayerID is null)
    		
		    -- IF destination village is specified, show movements for this destination village only
		    and (UM.DestinationVillageID  = @ToVillageID OR  @ToVillageID is null)
    		
		    -- IF origin village (@FromVillageID) is specified, only show movements from this village 
		    and (UM.OriginVillageID = @FromVillageID OR @FromVillageID is null)

		    -- we only show troops moving from logged in player 
		    and ORGINP.PlayerID = @LoggedInPlayerID 		
    		
		    -- we never show returns in outgoing since these are not our returns
		    and UM.CommandType not in (2,3) /*2==return, 3=recall*/
    		
		    -- do we show attacks?
		    and ( UM.CommandType <> 1 or @showAttacks = 1)
    		
		    -- do we show supports?
		    and ( UM.CommandType <> 0 or @showSupports = 1)
		    
		    -- do we show hidden movements?
		    and ( @showHidden = 1 OR PA.EventID is null)
        )				
	order by EventTime asc
	
	