
-- 
-- -----------------------------------------------------------------------------------
-- -----------------------------------------------------------------------------------
-- -----------------------------------------------------------------------------------
--
-- Drop stored procedure if it already exists
IF EXISTS (
  SELECT * 
    FROM INFORMATION_SCHEMA.ROUTINES 
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'Temp_PopBuildingLevelInfo' 
)
   DROP PROCEDURE dbo.Temp_PopBuildingLevelInfo
GO

CREATE PROCEDURE dbo.Temp_PopBuildingLevelInfo
	@BuildingSpeedFactor float
	,@BuildingTypeID int
	,@BaseCost int
	,@BaseBuildTime bigint
	,@BasePoints int	
	,@BaseLevelStrength int	
	,@BasePopulation int
	,@CostFactor float
	,@BuildTimeFactor float
	,@PointsFactor float 
	, @LevelStrengthFactor float		
	,@PopulationFactor float
	,@MaxLevel int
AS

declare @counter as int
declare @CumPopulation as int
declare @Population as int
declare @BuildTime as bigint
declare @CumBuildTime  as bigint

declare @BaseBuildTime_NoFactor as bigint -- build time that igores the @BuildingSpeedFactor. This is needed to determing level strenght
declare @BuildTime_NoFactor as bigint -- build time t	hat igores the @BuildingSpeedFactor. This is needed to determing level strenght
--	declare @CumBuildTime_NoFactor as bigint -- cummulative build time that igores the @BuildingSpeedFactor. This is needed to determing level strenght

declare @LevelStrength as int
declare @CumLevelStrength as int

	set @BaseBuildTime_NoFactor = @BaseBuildTime
	set @BaseBuildTime = @BaseBuildTime * @BuildingSpeedFactor
	
	-- base points are: 1 point for each 100 of base cost + 1 point for each 10 minute initial build time. 
	--	ps, we use @BaseBuildTime_NoFactor do this because we do not want the points to be effected by speed factor
	set @BasePoints = cast(@BaseCost /100 as integer) + cast(@BaseBuildTime_NoFactor /12000000000 as integer)
	set @PointsFactor = (@CostFactor + @BuildTimeFactor) /2


	-- First level
	insert into BuildingLevels values (
		@BuildingTypeID
		, 1
		, @BaseCost 
		, @BaseBuildTime 
		, @BaseBuildTime
		,  null
		, @BasePoints
		, @BasePopulation
		, @BasePopulation
		, @BaseLevelStrength
		, @BaseLevelStrength)


	set @CumPopulation = @BasePopulation
	set @Population = @BasePopulation
	
	set @CumBuildTime = @BaseBuildTime
	set @BuildTime = @BaseBuildTime
	set @BuildTime_NoFactor = @BaseBuildTime_NoFactor
	set @counter = 2

	while @counter <= @MaxLevel begin
		set @CumPopulation = @BasePopulation* power(@PopulationFactor, @counter-1)
		set @Population = @CumPopulation  - cast((@BasePopulation* power(@PopulationFactor, @counter-2)) as integer)
		
		set @BuildTime = @BaseBuildTime * power(@BuildTimeFactor, @counter-1)
		set @CumBuildTime = @BuildTime  + (select CumulativeBuildTime from BuildingLevels where BuildingTypeID = @BuildingTypeID and level = @counter-1)

		set @BuildTime_NoFactor = @BaseBuildTime_NoFactor * power(@BuildTimeFactor, @counter-1)
		
		set @LevelStrength = @BaseLevelStrength *  power(@LevelStrengthFactor, @counter-1)
		set @CumLevelStrength = @LevelStrength + (select CumulativeLevelStrength from BuildingLevels where BuildingTypeID = @BuildingTypeID and level = @counter-1)

		insert into BuildingLevels 
			(BuildingTypeID
			, Level
			, Cost
			, BuildTime
			, CumulativeBuildTime
			, LevelName
			, Points
			, Population
			, CumulativePopulation
			, LevelStrength
			, CumulativeLevelStrength)
			values 
			(@BuildingTypeID
			, @counter
			, @BaseCost * power(@CostFactor, @counter-1)
			, @BuildTime
			, @CumBuildTime
			, null
			, @BasePoints* power(@PointsFactor, @counter-1)
			, @Population
			, @CumPopulation 
			, @LevelStrength
			, @CumLevelStrength)
		

		set @counter = @counter + 1
	end
GO