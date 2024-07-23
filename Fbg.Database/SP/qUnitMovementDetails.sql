IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qUnitMovementDetails')
	BEGIN
		DROP  Procedure  qUnitMovementDetails
	END

GO

CREATE Procedure qUnitMovementDetails
	@EventID int 
	,@PlayerID int

AS

select UM.EventID
	, UMing.UnitTypeID
	, UMing.UnitCount
	, DestV.Name as DestinationVillageName
	, DestV.VillageID as DestinationVillageID
	, DestP.Name as DestinationPlayerName
	, DestP.PlayerID as DestinationPlayerID
	, UM.CommandType
	, UM.TripDuration
	, E.EventTime
	, OrigV.Name as OriginVillageName
	, OrigV.VillageID as OriginVillageID
	, DestV.XCord
	, DestV.YCord
	, OrigV.XCord
	, OrigV.YCord	
	from unitmovements UM
	join Events E
		on E.EventID = UM.EventID 
	join UnitsMoving UMing
		on UM.EventID = UMing.EventID 
	join Villages DestV
		on DestV.VillageID = UM.DestinationVillageID
	join Players DestP
		on DestV.OwnerPlayerID = DestP.PlayerID 
	join Villages OrigV
		on OrigV.VillageID = UM.OriginVillageID
	where Status = 0
		and E.EventID  = @EventID 
		
		-- SECURITY CHECK.  If command type is 0 or 1 (support or attack), then the origin village must belong to the player
		--                  If command type is 2 or 3 (return attack or support recall), then the destination player must be the player		
		and ( 
		            ( (CommandType = 0 OR CommandType = 1) AND OriginVillageID in (select villageID from villages where ownerplayerid = @PlayerID) )
		        OR  ( (CommandType = 2 OR CommandType = 3) AND DestP.PlayerID = @PlayerID )
		    )

GO 