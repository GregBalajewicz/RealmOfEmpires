 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qIncomingTroops')
	BEGIN
		DROP  Procedure  qIncomingTroops
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
CREATE Procedure qIncomingTroops
	@ToPlayerID as int
	, @ToVillageID as int = null
	, @FromVillageID as int = null
	, @FromPlayerID as int = null
    , @showAttacks as bit
    , @showSupports as bit
    , @showReturns as bit
    , @realRetrieve bit = 0     
    , @showHidden bit = 0   

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
	   @realRetrieve = 1
		and  ( 
		    E.Status = 0

		    -- since this returns incoming troops, the TO PLAYER must always be specified. 
		    and DESTP.PlayerID = @ToPlayerID

		    -- IF destination village is specified, show movements for this destination village only
		    and (UM.DestinationVillageID  = @ToVillageID OR  @ToVillageID is null)
    		
		    -- IF origin village (@FromVillageID) is specified, only show movements from this village 
		    and (UM.OriginVillageID = @FromVillageID OR @FromVillageID is null)

		    -- if @FromPlayerID specified, then make sure we only show incoming from the villages belonging to this player
		    and (ORGINP.PlayerID = @FromPlayerID or @FromPlayerID is null)
    		
		    -- only show visible incoming troops UNLESS these troops are your returns or recalls
		    and ( UM.VisibleToTarget >= 1 OR UM.CommandType in (2,3) /*2==return, 3=recall*/) 
		    --	only show fully visible incoming troops UNLESS from playerID and from village id is NOT specified (see note1 below)
		    and ( UM.VisibleToTarget >= 2 OR (@FromVillageID is null AND @FromPlayerID is null)) 
    		
		    -- do we show attacks?
		    and ( UM.CommandType <> 1 or @showAttacks = 1)
    		
		    -- do we show supports?
		    and ( UM.CommandType <> 0 or @showSupports = 1)		
    	
		    -- do we show supports?
		    and ( UM.CommandType  not in (2,3) /*2==return, 3=recall*/ or @showReturns = 1)		

		    -- do we show hidden movements?
		    and ( @showHidden = 1 OR PA.EventID is null)
	    )
	order by EventTime asc
	
	
	
/* NOTE 1

the reason we show only fully visible incoming (ie, we do not show attacks from unknown village)
if from village or from player is speficied, is because otherwise 'unknown' makes no sense as it is know 
by the from village or from player id specified
*/
	