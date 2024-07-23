  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qLordUnitTypeCost')
	BEGIN
		DROP  Procedure  qLordUnitTypeCost
	END

GO

--
-- RETURNS the cost of the next governor in @GovCost if @NextOrLast = 1 (default) 
--	normally you want to for recruitment, ie what is the cost of the next gov to recruit
-- RETURN the cost of the previous governor in @GovCost if @NextOrLast = 0 
--	normally you want to for cancelling recruitment & refund. Ie, this it the price of the currently recruiting
--	or last recruited governor
--
--	NOTE!! if person has just 1 village, then never send it @NextOrLast = 0; result will not be valid 
--
CREATE Procedure [dbo].[qLordUnitTypeCost]
		@PlayerID int
		,@GovCost int output
		,@NextOrLast bit = 1
AS
	begin try 
		declare @count int 
		declare @StartCost int
		declare @Multiplier int
		declare @StartCount int
	
		create table #temp (ctn int)
		
		--
		-- what is current lord count?
		--
		insert into #temp 
		select count(*) from villages where ownerplayerid = @PlayerID		
			union all
		select sum(TotalCount) from VillageUnits 
			where VillageID in (select VillageID from villages where ownerplayerid = @PlayerID)
			and UnitTypeID = 10 /*10 == lord */
			union all
		select sum(Count) from UnitRecruitments UR			
			where VillageID in (select VillageID from villages where ownerplayerid = @PlayerID)
			and UnitTypeID = 10 /*10 == lord */
			and Status = 0
		
		select @count = isnull(sum(ctn ),0) from #temp
		IF @NextOrLast = 0 BEGIN
			-- if we want the cost of hte last gov, then minus 1 from current count. 
			set @count = @count - 1  
		END
		--
		-- what is the starting cost & multiplier
		--
		select @StartCount = LordCountStart
			, @StartCost = LordCountStartCost
			, @Multiplier = LordCostMultiplier 
			from LordUnitTypeCostMultiplier M
			where @count >= LordCountStart 
			and not exists 
				(select * from LordUnitTypeCostMultiplier 
					where @count >= LordCountStart 
					and LordCountStart > M.LordCountStart
				)
		--
		-- the the cost of the next gov.
		--
		select @GovCost= @StartCost + (@count - @StartCount) * @Multiplier
	
	end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qNextLordUnitTypeCost FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)

		+ '   @PlayerID'	+ ISNULL(CAST(@PlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @count'		+ ISNULL(CAST(@count AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @StartCost'	+ ISNULL(CAST(@StartCost AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @GovCost'		+ ISNULL(CAST(@GovCost AS VARCHAR(max)), 'Null') + CHAR(10)

		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	




 