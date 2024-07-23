IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qMapTroopsMove_Outgoing')
	BEGIN
		DROP  Procedure  qMapTroopsMove_Outgoing
	END

GO

CREATE PROCEDURE [dbo].[qMapTroopsMove_Outgoing] 
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
				and UM.OriginVillageID in (select villageid from villages where ownerplayerid = @PlayerID)    		
				and UM.CommandType in (0) --support
	     ) as 'OutgoingSupportCount', 
	     
	    (select count (*) 
			from unitmovements UM with(nolock)
			join Events E with(nolock) on E.EventID = UM.EventID 
			where E.Status = 0
				and UM.DestinationVillageID=v.VillageID
				and UM.OriginVillageID in (select villageid from villages where ownerplayerid = @PlayerID)    		
				and UM.CommandType  in (1) --attack
         ) as 'OutgoingAttackCount'         
         
	from Villages as v 
	where 
		v.XCord>=@RegularMapBottomLeftX AND v.XCord < @RegularMapBottomLeftX + @RegularMapSize
		and v.YCord>=@RegularMapBottomLeftY AND v.YCord < @RegularMapBottomLeftY + @RegularMapSize
END
go
