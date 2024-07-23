
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
     AND SPECIFIC_NAME = N'Temp_PopLevelProperties' 
)
   DROP PROCEDURE dbo.Temp_PopLevelProperties
GO

CREATE PROCEDURE dbo.Temp_PopLevelProperties
	@BuildingTypeID int
	,@PropertyID int
	,@BaseValue float
	,@Factor float
	,@StartLevel int
	,@MaxLevel int
AS
	declare @counter as int
	set @counter = @StartLevel
	while @counter <= @MaxLevel begin
		insert into LevelProperties (BuildingTypeID, Level, PropertyID, PropertyValue) values (@BuildingTypeID,@counter,@PropertyID, @BaseValue )

		set @BaseValue = @Factor * @BaseValue
		set @counter = @counter + 1
	end
GO

