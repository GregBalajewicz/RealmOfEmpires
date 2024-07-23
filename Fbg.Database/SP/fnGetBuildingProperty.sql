
IF OBJECT_ID (N'dbo.fnGetBuildingProperty') IS NOT NULL
   DROP FUNCTION dbo.fnGetBuildingProperty
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		dbo
-- Create date: dec 16 07
-- Description:	
-- =============================================
CREATE FUNCTION fnGetBuildingProperty 
(
	@VillageID int
	,@BuildingLevelProperyID int
)
RETURNS real
AS
BEGIN
	DECLARE @Value real

	--
	-- Get max population
	--
	set @Value = (select sum(CAST(PropertyValue AS real)) 
		from Buildings B 
		join LevelProperties LP 
			on B.BuildingTypeID = LP.BuildingTypeID 
			and B.Level = LP.Level
			and LP.PropertyID = @BuildingLevelProperyID
		where B.VillageID = @VillageID )

	RETURN @Value
END
GO

 