
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iRecruitUnits')
	BEGIN
		DROP  Procedure  iRecruitUnits
	END
GO
/*
@RecruitmentBuildingID: each unit is recruited in one building. Infantry and Pikeman are recruited in Barracks
	Cavalery in Stable, Governor in Palace etc
@TotalPopulation : each unit takes certain amount of 'population'. A infantry takes 1 population
	ie, 'how much food a units needs' :)
	so cavalry takes more because you need to feed the horse as well.
	so total population = UnitTypes.Population X # of unis to recruit
	
@TotalCost: = # of units X UnitTypes.Cost
	However, unit cost is not always the same.
	for most units, the cost of a unit is simply UnitTypes.Cost.
	However, for some units, namly Governor, the cost changes. 
	For governor, the more villages you have, the more expensive the governor is
	Hence the @UnitCost - This is needed since otherwise you will not know how much to refund in case of cancel 
*/
CREATE Procedure iRecruitUnits
	@VillageID int
	,@UnitTypeID int
	,@RecruitCount int
	,@RecruitmentBuildingID int
	,@TotalPopulation int
	,@TotalCost int
	,@UnitCost int
AS
	declare @MaxPop int
	declare @CurPop int
	declare @RemPop int
	declare @Coins int
	declare @Sucess as bit;	

	--

	begin try 

		begin tran 		
			--
			-- Obtain a village LOCK
			--		this will ensure that no one else can effect the village in any way.
			--
			update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @VillageID
		
			-- 
			-- validating population
			-- 
			set @MaxPop = dbo.fnGetMaxPopulation(@VillageId)
			set @CurPop = dbo.fnGetCurrentPopulation(@VillageID)
			set @RemPop = @MaxPop - @CurPop
			
			IF @TotalPopulation > @RemPop BEGIN
				-- the recruit request we got is inconsistent. This can happen, for example, when the recruit button is quickly clicked twice 
				--	and get end up with two identical calls. the first one comepltes, adds the recruitment to the Q, 
				-- and then the other one will fail here
				--	This is an expected behavour so we just exit quietly
				IF @@TRANCOUNT > 0 ROLLBACK
				INSERT INTO ErrorLog VALUES (getdate(), 0, 'iRecruitUnits : @TotalPopulation > @RemPop',  ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null'))		
				return 
			END			
			
			--
			-- Pay for the recruitment
			--
			exec uVillageCoins_Subtract @VillageID,@TotalCost,@Sucess output
			if @Sucess<>1 begin
				-- verify the cost does not exceeed assets. ( this can happen for the same reason the population
				--	check above)
				IF @@TRANCOUNT > 0 ROLLBACK
				INSERT INTO ErrorLog VALUES (getdate(), 0, 'iRecruitUnits : @TotalCost exceedes assets', ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null'))		
				return 	
			end
			--
			-- do the recruitment
			-- 
			insert into UnitRecruitments
			    (VillageID
					,BuildingTypeID
					,UnitTypeID
					,Count
					, UnitCost
					, DateAdded
					, DateLastUpdated
					, status)
				 values 
					(@VillageID 
					, @RecruitmentBuildingID
					, @UnitTypeID
					, @RecruitCount
					, @UnitCost
					, getdate()
					, null
					, 0)

		commit tran
	end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max)
	 IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iRecruitUnits FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @RecruitmentBuildingID' + ISNULL(CAST(@RecruitmentBuildingID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @UnitTypeID' + ISNULL(CAST(@UnitTypeID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @RecruitCount' + ISNULL(CAST(@RecruitCount AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @TotalPopulation' + ISNULL(CAST(@TotalPopulation AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @TotalCost' + ISNULL(CAST(@TotalCost AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @MaxPop' + ISNULL(CAST(@MaxPop AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CurPop' + ISNULL(CAST(@CurPop AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @RemPop' + ISNULL(CAST(@RemPop AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @Sucess' + ISNULL(CAST(@Sucess AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @Coins' + ISNULL(CAST(@Coins AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

--
-- say that the village has changed. this is done deliberately outside of the main tran and try 
--
declare @PlayerID int 
select @PlayerID = OwnerPlayerID from villages where VillageID = @VillageID
UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @PlayerID and VillageID = @VillageID and CachedItemID = 0
IF (@@rowcount < 1 ) BEGIN
	INSERT INTO VillageCacheTimeStamps values (@PlayerID, @VillageID, 0, getdate())
END


GO