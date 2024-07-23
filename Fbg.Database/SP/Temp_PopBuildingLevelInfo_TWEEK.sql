

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
     AND SPECIFIC_NAME = N'Temp_PopBuildingLevelInfo_TWEEK' 
)
   DROP PROCEDURE dbo.Temp_PopBuildingLevelInfo_TWEEK
GO

CREATE PROCEDURE dbo.Temp_PopBuildingLevelInfo_TWEEK
	@BuildingSpeedFactor float
	,@BuildingTypeID int
	,@BaseCost int
	,@BaseBuildTime bigint
	,@CostFactor float
	,@BuildTimeFactor float
	,@MaxLevel int
	,@PrevLevel int -- if set to 0, use the base params. otherwise, grab base from that level
AS

    declare @counter as int
    declare @BuildTime as bigint
	declare @NextLevel as int
	declare @Cost int


	SET @NextLevel =@PrevLevel + 1

    IF @PrevLevel = 0 BEGIN 
	    set @BuildTime = @BaseBuildTime	* @BuildingSpeedFactor
		set @Cost = @BaseCost
	END ELSE BEGIN
	    SELECT @BuildTime = BuildTime * @BuildTimeFactor
	        , @Cost = Cost * @CostFactor
	        FROM BuildingLevels WHERE BuildingTypeID = @BuildingTypeID AND LEVEL = @PrevLevel
	END


	UPDATE BuildingLevels SET Cost = @Cost,  BuildTime = @BuildTime
	    WHERE BuildingTypeID = @BuildingTypeID AND LEVEL = @NextLevel

    IF @NextLevel < @MaxLevel BEGIN
        EXEC Temp_PopBuildingLevelInfo_TWEEK @BuildingSpeedFactor
            ,@BuildingTypeID
        	,0 -- its ignored this time
	        ,0 -- its ignored this time
	        ,@CostFactor 
	        ,@BuildTimeFactor 
	        ,@MaxLevel 
	        ,@NextLevel
    END



GO
