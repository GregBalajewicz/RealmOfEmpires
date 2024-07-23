
IF OBJECT_ID (N'dbo.fnGetMaxPopulation') IS NOT NULL
   DROP FUNCTION dbo.fnGetMaxPopulation
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION fnGetMaxPopulation 
(
	@VillageID int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @MaxPop int
	declare @MaxPop_raw real
	declare @researchPercentBonus float
	declare @villageTypePercentBonus float
	declare @VillageTypeID smallint
	declare @PID int 

    declare @LevelProp_PopulationCapacity int
    set @LevelProp_PopulationCapacity = 4

    --
    -- get village basic info
    --
    SELECT @VillageTypeID = VillageTypeID, @PID = OwnerPlayerID FROM Villages Where VillageID = @VillageID

	--
	-- Get max population
	--
	set @MaxPop_raw = (select PropertyValue as MaxPopulation from Buildings B 
		join LevelProperties LP 
			on LP.BuildingTypeID  = 8 -- 8 is Farm land
			and LP.Level = B.Level
			and LP.PropertyID = @LevelProp_PopulationCapacity -- 4 is PopulationCapacity
		where B.VillageID = @VillageID and B.BuildingTypeID  = 8 )-- 8 == Farm land
		
    --
    -- get research bonus, we get 0.1 meaning 10% but add 1 to it to get 1.1
    --                
    select @researchPercentBonus = sum(cast(PropertyValue as float))
        from ResearchItemPropertyTypes PT 
        join ResearchItemProperties P on P.ResearchItemPropertyID = PT.ResearchItemPropertyID
        join ResearchItems RI on RI.ResearchItemTypeID = P.ResearchItemTypeID AND RI.ResearchItemID = P.ResearchItemID
        join PlayerResearchItems PRI ON RI.ResearchItemTypeID = PRI.ResearchItemTypeID AND RI.ResearchItemID = PRI.ResearchItemID
        where PT.PropertyID = @LevelProp_PopulationCapacity
	        AND PlayerID = @PID
	        AND PT.Type = 3 	
    SET @researchPercentBonus = isnull(@researchPercentBonus,0) + 1
    --
    -- Get village Type bonus, if any. we get 0.1 meaning 10% but add 1 to it to get 1.1
    --
    select @villageTypePercentBonus = sum(cast(PropertyValue as float)) 
        from VillageTypeProperties VTP 
        join VillageTypePropertyTypes VTPT on VTP.VillageTypePropertyTypeID = VTPT.VillageTypePropertyTypeID
        where 
	        VTP.VillageTypeID = @VillageTypeID
	        and PropertyID = @LevelProp_PopulationCapacity
	        and type = 3
    SET @villageTypePercentBonus = isnull(@villageTypePercentBonus,0) + 1
	
	SET @MaxPop_raw = @MaxPop_raw * @researchPercentBonus * @villageTypePercentBonus

	set @MaxPop = FLOOR(@MaxPop_raw)

	 	

	RETURN @MaxPop
END
GO

 