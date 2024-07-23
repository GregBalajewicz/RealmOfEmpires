
IF OBJECT_ID (N'dbo.fnGetMinAmountToTransport') IS NOT NULL
   DROP FUNCTION dbo.fnGetMinAmountToTransport
GO
 
 CREATE FUNCTION [dbo].[fnGetMinAmountToTransport]
(
	@SelectedVillageID int
	,@OtherVillageID int
	,@SelectecVillageOwnerPlayerID int
)
RETURNS int
AS
BEGIN
--this fn to get min of three vlaues @SelectedVillageTreasuryCabacity,@AvailableCoinsinOtherVillage,@MaxAmountCanTransport

/*
    --
    -- ERROR !
    this function suffers from an inconsitency that could use problems.
    
    The 	
    SELECT    @TotalTransportsOtherSelectedVillage= sum( CoinTransports.Amount)
	FROM           CoinTransports 
	WHERE     (CoinTransports.OriginVillageID = @OtherVillageID) and (Direction=0 or Direction=1)

    does not check if Events.Statsu = 1 meaning that if transport coins event fails, and event handler set the status = 1,
    this fucntion will still tihnk there are coin transport away since it expect the CoinTransports.Direction to not equal 0 or 1
    and it was not done so since the event handler assumes that an event is considered completed/cancelled if status = 1
    
    This should be modified to this so that this problem is removed

	SELECT    @TotalTransportsOtherSelectedVillage= sum( CT.Amount)
	FROM           CoinTransports CT join Events E on E.EventID = CT.EventID
	WHERE     (CT.OriginVillageID = @OtherVillageID) and (Direction=0 or Direction=1) and Status = 0

*/
--

	declare @TradePostSize int
	declare @TotalTransportsOtherSelectedVillage as int;
	declare @MaxAmountCanTransport as int
	declare @TransportCapacityFloat float 
	declare @researchPercentBonus as real
	declare @villageTypeBonus as real

	declare @LevelProp_TradePostCapacity int 
	set @LevelProp_TradePostCapacity = 10

	declare @TradePostID int 
	set @TradePostID = 11

    --
    -- obtain research bonus if any. will be in a format like 0.1 meaning 10%
    -- 
    select @researchPercentBonus = sum(cast(PropertyValue as float))
        from ResearchItemPropertyTypes PT 
        join ResearchItemProperties P on P.ResearchItemPropertyID = PT.ResearchItemPropertyID
        join ResearchItems RI on RI.ResearchItemTypeID = P.ResearchItemTypeID AND RI.ResearchItemID = P.ResearchItemID
        join PlayerResearchItems PRI ON RI.ResearchItemTypeID = PRI.ResearchItemTypeID AND RI.ResearchItemID = PRI.ResearchItemID
        where PT.PropertyID = @LevelProp_TradePostCapacity
            AND PlayerID = @SelectecVillageOwnerPlayerID
            AND PT.Type = 3 			            
    SET @researchPercentBonus = isnull(@researchPercentBonus,0) + 1 

	SELECT    @TotalTransportsOtherSelectedVillage= sum( CoinTransports.Amount)
	FROM           CoinTransports 
	WHERE     (CoinTransports.OriginVillageID = @OtherVillageID) and (Direction=0 or Direction=1)

	select @TransportCapacityFloat = PropertyValue from Buildings B
	join LevelProperties LP on LP.BuildingTypeID = @TradePostID and LP.Level = B.Level 
	where B.VillageID = @OtherVillageID and B.BuildingTypeID = @TradePostID 

	-- get the possible trasnport capacity bonus from bonus village type. 
	select @villageTypeBonus = PropertyValue from villages V join VillageTypeProperties VP
		on v.VillageTypeID = VP.VillageTypeID
		and VillageTypePropertyTypeID = 8 
	where villageid = @OtherVillageID
	set @villageTypeBonus = isnull(@villageTypeBonus, 0) + 1;

	set @TradePostSize = Round(@TransportCapacityFloat * @researchPercentBonus, 0)
	set @TradePostSize = Round(@TradePostSize * @villageTypeBonus, 0)
	  
			
	set @MaxAmountCanTransport=@TradePostSize-isnull(@TotalTransportsOtherSelectedVillage,0)

	RETURN @MaxAmountCanTransport
END




