IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iUnitRecruit_GetOneFromQ')
	BEGIN
		DROP  Procedure  iUnitRecruit_GetOneFromQ
	END
GO

--
-- THIS PROCEDURE returns error message start with words 'DEADLOCK' if deadlock has occured, ie if it failed 
--	because of deadlock.
CREATE Procedure iUnitRecruit_GetOneFromQ
	@VillageID int
	,@RecruitmentBuildingID int
	, @PrintDebugInfo BIT = null
AS


DECLARE @DEBUG INT
IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
	SET @DEBUG = 1
	SET NOCOUNT OFF
END ELSE BEGIN
	SET @DEBUG = 0
	SET NOCOUNT ON
END 
IF @DEBUG = 1 SELECT 'BEGIN iUnitRecruit_GetOneFromQ ' + cast(@VillageID as varchar(10)) + cast(@RecruitmentBuildingID as varchar(10))

declare @count int
declare @UnitTypeID int
declare @QEntryID int
declare @OneUnitPopulation int
declare @EventID int
declare @BaseRecruitTime bigint
declare @ActualRecruitTime bigint
declare @RecruitTimeFactor real 
declare @UnitRecruitmentBuildingLevelPropertyID int
declare @CompletedOn as DateTime
declare @Now as DateTime
declare @UnitCost int

begin try 
	--
	-- get the time now. this will be the time from which we calculate when the next unit will recruit. 
	--	to be exact, this should be pass to us to ensure there is no difference between the last recruited troop and the start
	--	of the next one. otherwise we could have a situation where 1000 units should produce in 20hours but will produce in 20hours 1 minute due to dealays
	--	of executing this code. for now we consider this a negligible inacuracy. 
	--
	set @Now = getdate() 
	begin tran 	
		--
		-- Check currently recruiting unit and make sure there is none in this recruitment building.
		--
		IF NOT exists (
			select * 
			from  UnitRecruitment UR 
			join UnitTypes UT
				on UR.UnitTypeID = UT.UnitTypeID
			join Events E
				on E.EventID = UR.EventID
			where UR.VillageID = @VillageID 
				and UT.BuildingTypeID = @RecruitmentBuildingID 
				and E.Status = 0
		) BEGIN
			--
			-- get the next unit to recruit in Q in this recruitment building
			--
			select top 1 
				@count = [count] 
				, @UnitTypeID = UnitTypeID 
				, @QEntryID = QEntryID
				, @UnitCost = UnitCost
			from UnitRecruitmentQEntries URQ
			where
				VillageID = @VillageID
				and BuildingTypeID = @RecruitmentBuildingID
			order by DateAdded asc
			IF @DEBUG = 1 SELECT @count as '@count', @UnitTypeID as '@UnitTypeID', @QEntryID as '@QEntryID', @UnitCost as '@UnitCost'
				
			--
			-- If there is some unit, then start recruting it. 
			--	PLEASE NOTE - we do not verify requirements
			--
			IF @count is not null BEGIN
				--
				-- get some needed unit type info
				--
				select  @UnitRecruitmentBuildingLevelPropertyID = PropertyID
					, @BaseRecruitTime = RecruitmentTime
					, @OneUnitPopulation = [Population]
					from UnitTypes where UnitTypeID = @UnitTypeID
				--
				-- get recruit time factor, calculate recruit time & completion date
				--
				set @RecruitTimeFactor = dbo.fnGetBuildingProperty(@VillageID, @UnitRecruitmentBuildingLevelPropertyID)
				set @ActualRecruitTime = @BaseRecruitTime * (@RecruitTimeFactor/100)
				set @CompletedOn = dateadd(millisecond, @ActualRecruitTime/10000, @Now)
				IF @DEBUG = 1 SELECT @RecruitTimeFactor as '@RecruitTimeFactor', @ActualRecruitTime as '@ActualRecruitTime', @CompletedOn as '@CompletedOn'
				
				--
				-- Recruit the unit, finally!
				--			
				insert into Events values(@CompletedOn, 0)
				set @EventID = SCOPE_IDENTITY() 

				IF @DEBUG = 1 SELECT @EventID as '@EventID', @UnitTypeID as '@UnitTypeID', @VillageID as '@VillageID', @OneUnitPopulation as '@OneUnitPopulation', @UnitCost as '@UnitCost'
				
				insert into UnitRecruitment (EventID,  UnitTypeID, [Count], VillageID, TotalPopulation, UnitCost)
					values (@EventID, @UnitTypeID, 1, @VillageID, @OneUnitPopulation, @UnitCost)
				--
				-- update the Q recruit count
				--
				IF @count = 1 BEGIN
					delete UnitRecruitmentQEntries where QEntryID  = @QEntryID 
				END ELSE BEGIN
					Update UnitRecruitmentQEntries 
						set [count] = [count] -1
						, TotalPopulation = TotalPopulation - @OneUnitPopulation
						where QEntryID = @QEntryID
				END	
			END
		END 
		
	commit tran

	IF @DEBUG = 1 SELECT 'END iUnitRecruit_GetOneFromQ '  + cast(@VillageID as varchar(10)) + cast(@RecruitmentBuildingID as varchar(10) )
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max)
	 IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iUnitRecruit_GetOneFromQ FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @VillageID'				+ ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @RecruitmentBuildingID'	+ ISNULL(CAST(@RecruitmentBuildingID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @UnitTypeID'				+ ISNULL(CAST(@UnitTypeID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @count'					+ ISNULL(CAST(@count AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @QEntryID'				+ ISNULL(CAST(@QEntryID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @OneUnitPopulation'		+ ISNULL(CAST(@OneUnitPopulation AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @EventID'					+ ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @BaseRecruitTime'			+ ISNULL(CAST(@BaseRecruitTime AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @ActualRecruitTime'		+ ISNULL(CAST(@ActualRecruitTime AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @UnitRecruitmentBuildingLevelPropertyID' + ISNULL(CAST(@UnitRecruitmentBuildingLevelPropertyID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @RecruitTimeFactor'		+ ISNULL(CAST(@RecruitTimeFactor AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @CompletedOn'				+ ISNULL(CAST(@CompletedOn AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @Now'						+ ISNULL(CAST(@Now AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @UnitCost'				+ ISNULL(CAST(@UnitCost AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @RecruitTimeFactor'		+ ISNULL(CAST(@RecruitTimeFactor AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'	+ ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'	+ ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'		+ ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'		+  ERROR_MESSAGE() + CHAR(10)
		
	--
	-- IF deadlock, rerun
	--
	IF ERROR_NUMBER() = 1205 BEGIN
		INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock, no rerun', @ERROR_MSG)		
		SET @ERROR_MSG = 'DEADLOCK  -- ' + @ERROR_MSG 
		RAISERROR(@ERROR_MSG,11,1)	
	END ELSE BEGIN
		RAISERROR(@ERROR_MSG,11,1)	
	END
end catch	

go