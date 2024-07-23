 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qMapTroopsMove_Incoming')
	BEGIN
		DROP  Procedure  [qMapTroopsMove_Incoming]
	END

GO

CREATE PROCEDURE [dbo].[qMapTroopsMove_Incoming] 
	@RegularMapBottomLeftX int,        
	@RegularMapBottomLeftY int,
	@RegularMapSize int,
	@PlayerID int
AS
BEGIN
	
	select
		v.VillageID,
	         
        (select count (*) 
			from unitmovements UM with(nolock)
			join Events E with(nolock)	on E.EventID = UM.EventID 
			where E.Status = 0
				and UM.DestinationVillageID=v.VillageID
				and UM.CommandType in (0) --support
	     ) as 'IncomingSupportCount', 
	     
	    (select count (*) 
			from unitmovements UM with(nolock)
			join Events E with(nolock) on E.EventID = UM.EventID 
			where E.Status = 0
				and UM.DestinationVillageID=v.VillageID
				and UM.CommandType  in (1) --attack
				and UM.VisibleToTarget >= 1
         ) as 'IncomingAttackCount'
         
	from Villages as v 
	where 
		v.XCord>=@RegularMapBottomLeftX AND v.XCord < @RegularMapBottomLeftX + @RegularMapSize
		and v.YCord>=@RegularMapBottomLeftY AND v.YCord < @RegularMapBottomLeftY + @RegularMapSize
		and v.ownerplayerid = @PlayerID -- since this is incoming, we only care about the players villages
END
