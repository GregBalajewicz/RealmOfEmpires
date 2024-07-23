
IF OBJECT_ID (N'dbo.fnGetCurrentPopulation') IS NOT NULL
   DROP FUNCTION dbo.fnGetCurrentPopulation
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		dbo
-- Create date: Dec 16 07
-- Description:	
-- =============================================
CREATE FUNCTION fnGetCurrentPopulation 
(
	-- Add the parameters for the function here
	@villageID int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result int

	-- Add the T-SQL statements to compute the return value here
	SELECT @Result = @villageID
	
	declare @UpgradeQ table ( BuildingTypeID int, UpgradeLevel int)
	
	insert into @UpgradeQ 	
		SELECT 
			BUQ.BuildingTypeID
			, isnull( ROW_NUMBER() OVER (PARTITION BY BUQ.BuildingTypeID order by BUQ.DateAdded ) + isnull(BU.Level, isnull(B.Level, 0)), 1) as UpgradeLevel
			FROM BuildingUpgradeQEntries BUQ
			left JOIN Buildings B
				on BUQ.VillageID = B.VillageID
				and BUQ.BuildingTypeID = B.BuildingTypeID
			left join buildingupgrades BU
				on BU.VillageID = BUQ.VillageID
				and BUQ.BuildingTypeID = BU.BuildingTypeID
				and BU.EventID = (SELECT EventID FROM Events WHERE EventID = BU.EventID AND Status <> 1)
			WHERE BUQ.VillageID = @VillageId



	declare @Table table ( [Count] int)

	
	insert into @Table 
	
		--
		-- population taken up by existing buildings 
		select sum(CumulativePopulation) as CurrentPopulation  from Buildings B 
			join BuildingLevels BL 
				on B.BuildingTypeID = BL.BuildingTypeID
				and B.Level = BL.Level
			where 
				B.VillageID = @VillageId

	union all
		--
		-- population taken up by buildings in currently upgrading list
		select sum(population)
			from Events E 
			join BuildingUpgrades BU
				on E.EventID = BU.EventID 
			join BuildingLevels BL
				on BU.BuildingTypeID = BL.BuildingTypeID
				and BU.level = BL.level
			where E.Status <> 1
				and BU.VillageID = @VillageId

	union all 
		--
		-- population taken up by buildings in upgrade Q
		select sum(population)
			FROM @UpgradeQ Q
			join BuildingLevels BL
				on Q.BuildingTypeID = BL.BuildingTypeID
				and Q.UpgradeLevel = BL.level

	union all 
		--
		-- population taken up by troops currently recruiting
		select sum(UR.count * UT.population) 
			from UnitRecruitments UR
			join UnitTypes UT 
				on UT.UnitTypeID = UR.UnitTypeID
			where Status = 0 and VillageID = @VillageID
	
	union all 
		--
		-- population taken up by troops in village
		select sum(totalCount * population)
		from villageUnits VU
		join UnitTypes UT
			on VU.UnitTYpeID = UT.UnitTypeID
		where VillageID = @VillageID
		group by VU.UnitTypeID



	set @Result = (select sum([count]) from @table)
	-- Return the result of the function
	RETURN @Result

END
GO