IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iCompleteUnitRecruitment')
	BEGIN
		DROP  Procedure  iCompleteUnitRecruitment
	END

GO

CREATE Procedure iCompleteUnitRecruitment
	@VillageID int
	, @PrintDebugInfo BIT = null
AS
declare @UnitTypeID int
declare @RecruitmentEntryUnitCount int
declare @RecruitmentBuildingID_Previous int
declare @RecruitmentBuildingID int
declare @EntryID bigint
declare @RecruitStartTime datetime
declare @TimeDifference bigint
declare @TimeDifferenceFrom DateTime
declare @now DateTime
declare @NumRecruited int

declare @BaseRecruitTime bigint
declare @ActualPerUnitRecruitTime bigint
declare @ActualPerUnitRecruitTimeInMs bigint
declare @RecruitTimeFactor real 
declare @UnitRecruitmentBuildingLevelPropertyID int

declare @ContinueWithThisBuilding bit
declare @LastRecruitmentCompletedOn DateTime

declare @villageTypePercentBonus float
declare @researchPercentBonus float

declare @OwnerPlayerID int
declare @VillageTypeID smallint

DECLARE @DEBUG INT, @DBG varchar(10)
IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
	SET @DEBUG = 1
	set @DBG = ':::::::'
	SET NOCOUNT OFF
END ELSE BEGIN
	SET @DEBUG = 0
	SET NOCOUNT ON
END 
IF @DEBUG = 1 print @DBG + 'BEGIN iCompleteUnitRecruitment ' + cast(@VillageID AS VARCHAR(100))

set @now = getdate();

begin try 
	begin transaction
		
	select EntryID
	, UR.UnitTypeID
	, UR.BuildingTypeID
	, [Count]
	, isnull(UR.DateLastUpdated, UR.DateAdded) as RecruitStartTime
	, DateAdded
	, UT.RecruitmentTime
	, PropertyID
	into #UnitRecruitments
	from UnitRecruitments UR
	join UnitTypes UT 
		on UT.UnitTypeId = UR.UnitTypeID
	where
		VillageID = @villageId   
		AND status = 0 
	order by UR.BuildingTypeID, UR.DateAdded ASC
	IF @DEBUG = 1 select '#UnitRecruitments',* from #UnitRecruitments

    -- get some village info
    select @OwnerPlayerID = ownerplayerid, @VillageTypeID = VillageTypeID from Villages where VillageID = @VillageID
	IF @DEBUG = 1 select @OwnerPlayerID as '@OwnerPlayerID'
		, @VillageTypeID as '@VillageTypeID'
	--
	-- main loop that processes all rows in #UnitRecruitments
	--
	SET @ContinueWithThisBuilding = 1
	WHILE (1 = 1) BEGIN
	
		select top 1
			@EntryID = EntryID
			,@UnitTypeID = UR.UnitTypeID
			,@RecruitmentBuildingID = UR.BuildingTypeID
			,@RecruitmentEntryUnitCount = [Count]
			,@RecruitStartTime = RecruitStartTime
			,@BaseRecruitTime = RecruitmentTime
			, @UnitRecruitmentBuildingLevelPropertyID = PropertyID
		from #UnitRecruitments UR
		order by UR.BuildingTypeID, UR.DateAdded ASC -- THIS ORDER is CRITICAL to the fuctionality of the below code. 
		IF @@rowcount = 0 BEGIN
			--
			-- WE are done, lets get out of here. 
			--
			BREAK;
		END 
		IF @DEBUG = 1 select @EntryID as '@EntryID', @UnitTypeID as '@UnitTypeID', @RecruitmentBuildingID as'@RecruitmentBuildingID'
			, @RecruitmentEntryUnitCount as '@RecruitmentEntryUnitCount', @RecruitStartTime as '@RecruitStartTime'
			, @BaseRecruitTime as '@BaseRecruitTime', @UnitRecruitmentBuildingLevelPropertyID as '@UnitRecruitmentBuildingLevelPropertyID'
			, @RecruitmentBuildingID_Previous as '@RecruitmentBuildingID_Previous'
			, @ContinueWithThisBuilding as '@ContinueWithThisBuilding'
			, @TimeDifferenceFrom as '@TimeDifferenceFrom'
			, @RecruitTimeFactor as '@RecruitTimeFactor'
		--
		-- delete this record from the temp table as we got all we wanted from it
		--
		DELETE #UnitRecruitments where EntryID = @EntryID

		--
		-- Figure out if we are to continue processing this record
		--
		IF @RecruitmentBuildingID_Previous = @RecruitmentBuildingID BEGIN		
			IF @ContinueWithThisBuilding = 0 BEGIN
				-- IF this record is for the same recruitment building as the previous one
				--	and we are not to continue with this building then lest go to the next record				
				CONTINUE;
			END 
		END ELSE BEGIN
			-- 
			-- this is a new recuitment building, we always process the first record for it. 
			-- 
			SET @RecruitmentBuildingID_Previous = @RecruitmentBuildingID
			SET @TimeDifferenceFrom = @now
			SET @RecruitTimeFactor = null
			
			--START REBEL/ABANDONED BURST RECRUITMENT
			IF  EXISTS (SELECT * FROM RealmAttributes WHERE attribID = 55 AND AttribValue = '1') AND
			EXISTS (SELECT * FROM Players WHERE playerid = @OwnerPlayerID and userid in ( '00000000-0000-0000-0000-000000000002', '00000000-0000-0000-0000-000000000000')) 
			BEGIN
				SET @RecruitTimeFactor = 10 --X% of base time, lower the faster.
			END
			--END REBEL/ABANDONED BURST RECRUITMENT

		END 
		IF @DEBUG = 1 select @RecruitmentBuildingID_Previous as '@RecruitmentBuildingID_Previous'
			, @ContinueWithThisBuilding as '@ContinueWithThisBuilding'
			, @TimeDifferenceFrom as '@TimeDifferenceFrom'
			, @RecruitTimeFactor as '@RecruitTimeFactor'
		IF @DEBUG = 1 select 'UnitRecruitments', * FROM  UnitRecruitments where EntryID = @EntryID


		--
		-- get recruit time factor, if we do not have it yet for the building, calculate recruit time etc
		--
		IF @RecruitTimeFactor is null BEGIN
			set @RecruitTimeFactor = dbo.fnGetBuildingProperty(@VillageID, @UnitRecruitmentBuildingLevelPropertyID)	
			IF @RecruitTimeFactor is null BEGIN
				--
				-- this may happen if there is no recruitment building - can happen in case if was TREBed down
				--	after the units were recruited
				--
				select @RecruitTimeFactor  = CAST(PropertyValue AS real) 
					from LevelProperties LP 
					where LP.Level = 1
					and LP.PropertyID = @UnitRecruitmentBuildingLevelPropertyID
			END			
		    IF @DEBUG = 1 select @RecruitTimeFactor as '@RecruitTimeFactor'		            
			--
			-- obtain research bonus if any. will be in a format like 0.1 meaning 10%
			-- 
            select @researchPercentBonus = sum(cast(PropertyValue as float))
	            from ResearchItemPropertyTypes PT 
	            join ResearchItemProperties P on P.ResearchItemPropertyID = PT.ResearchItemPropertyID
	            join ResearchItems RI on RI.ResearchItemTypeID = P.ResearchItemTypeID AND RI.ResearchItemID = P.ResearchItemID
	            join PlayerResearchItems PRI ON RI.ResearchItemTypeID = PRI.ResearchItemTypeID AND RI.ResearchItemID = PRI.ResearchItemID
	            where PT.PropertyID = @UnitRecruitmentBuildingLevelPropertyID
		            AND PlayerID = @OwnerPlayerID
		            AND PT.Type = 3 			            
            SET @researchPercentBonus = isnull(@researchPercentBonus,0)
		    IF @DEBUG = 1 select @researchPercentBonus as '@researchPercentBonus'		            
		    --
		    -- obtain village type bonus if any  will be in a format like 0.1 meaning 10%			
		    --
		    select @villageTypePercentBonus = sum(cast(PropertyValue as float)) 	       
	            FROM VillageTypeProperties VTP 
	            join VillageTypePropertyTypes VTPT on VTP.VillageTypePropertyTypeID = VTPT.VillageTypePropertyTypeID
	            where 
		            VTP.VillageTypeID = @VillageTypeID
		            and VTPT.PropertyID = @UnitRecruitmentBuildingLevelPropertyID		          
		            and type = 3		    
            SET @villageTypePercentBonus = isnull(@villageTypePercentBonus,0)
		    IF @DEBUG = 1 select @villageTypePercentBonus as '@villageTypePercentBonus'		            
            
            --
            -- calculate the actual number now
            --
            set @RecruitTimeFactor = @RecruitTimeFactor / ((1 + @researchPercentBonus) * (1 + @villageTypePercentBonus)  * 100)
            set @RecruitTimeFactor = @RecruitTimeFactor * 100


		    IF @DEBUG = 1 select @RecruitTimeFactor as '@RecruitTimeFactor'		                        
		END 
		
		
		set @ActualPerUnitRecruitTime = @BaseRecruitTime * (@RecruitTimeFactor/100)
		set @ActualPerUnitRecruitTimeInMs = @ActualPerUnitRecruitTime/10000

		-- find time diff in seconds first, then 'convert' to ms since doing diff in ms can result 
		--	in int overflow (and it did)
		set @TimeDifference = datediff(second, @RecruitStartTime, @now);
		set @TimeDifference = @TimeDifference * 1000
		
		set @NumRecruited = floor(cast(@TimeDifference as real) / @ActualPerUnitRecruitTimeInMs)
		
		IF @DEBUG = 1 select @RecruitTimeFactor as '@RecruitTimeFactor'
			, @ActualPerUnitRecruitTime as '@ActualPerUnitRecruitTime'
			, @ActualPerUnitRecruitTimeInMs as '@ActualPerUnitRecruitTimeInMs'
			, @TimeDifference as '@TimeDifference'
			, @NumRecruited as '@NumRecruited'
		
		--
		-- Did we actually recruit any? if not, then move on to the next record. 
		--
		IF @NumRecruited = 0 BEGIN
			SET @ContinueWithThisBuilding = 0
			DELETE #UnitRecruitments where BuildingTypeID = @RecruitmentBuildingID -- since we are not continuing with this building, lets remove all entries for it 
			CONTINUE;
		END

		IF @NumRecruited >= @RecruitmentEntryUnitCount BEGIN
			--
			-- we recruited all units so this entry is done. 
			--	and we should go to the next record in this recruitment building.
			--
			UPDATE UnitRecruitments SET Status = 1 where EntryID = @EntryID

			SET @NumRecruited = @RecruitmentEntryUnitCount  
			SET @LastRecruitmentCompletedOn = Dateadd(second, @NumRecruited * (cast(@ActualPerUnitRecruitTimeInMs as float)/1000), @RecruitStartTime)
			SET @TimeDifferenceFrom = @LastRecruitmentCompletedOn
			SET @ContinueWithThisBuilding = 1
			
			UPDATE #UnitRecruitments 
				SET  RecruitStartTime = @LastRecruitmentCompletedOn
				where BuildingTypeID = @RecruitmentBuildingID
			IF @@rowcount <> 0 BEGIN
				UPDATE UnitRecruitments 
					SET  DateLastUpdated = @LastRecruitmentCompletedOn
					where VillageID = @VillageID 
						and BuildingTypeID = @RecruitmentBuildingID  
						and EntryID <> @EntryID 
						and Status = 0				
			END

			IF @DEBUG = 1 select '@NumRecruited >= @RecruitmentEntryUnitCount'
				, @LastRecruitmentCompletedOn as '@LastRecruitmentCompletedOn'
				, @TimeDifferenceFrom as '@TimeDifferenceFrom'
			IF @DEBUG = 1 SELECT * from #UnitRecruitments where BuildingTypeID = @RecruitmentBuildingID
		END ELSE BEGIN
			--
			-- We did not 'finish' this entry so just update it and move on to the next
			--  recruitment building
			--
			SET @LastRecruitmentCompletedOn = Dateadd(second, @NumRecruited * (cast(@ActualPerUnitRecruitTimeInMs as float)/1000), @RecruitStartTime) 
			IF @DEBUG = 1 select @LastRecruitmentCompletedOn as '@LastRecruitmentCompletedOn'
			UPDATE UnitRecruitments 
				SET [count] = [count] - @NumRecruited
				, DateLastUpdated = @LastRecruitmentCompletedOn
				where EntryID = @EntryID
			IF Exists(select * from #UnitRecruitments WHERE BuildingTypeID = @RecruitmentBuildingID and EntryID <> @EntryID) BEGIN
				UPDATE UnitRecruitments 
					SET  DateLastUpdated = @LastRecruitmentCompletedOn
					where VillageID = @VillageID 
						and BuildingTypeID = @RecruitmentBuildingID 
						and EntryID <> @EntryID 
						and Status = 0
			END

			SET @ContinueWithThisBuilding = 0
			DELETE #UnitRecruitments where BuildingTypeID = @RecruitmentBuildingID -- since we are not continuing with this building, lets remove all entries for it 

			
			IF @DEBUG = 1 select '@NumRecruited NOT >= @RecruitmentEntryUnitCount'
				, @LastRecruitmentCompletedOn as '@LastRecruitmentCompletedOn'
		END

		--
		-- Update OR insert village unit count
		--
		if exists(select * from VillageUnits where VillageId =@VillageID and UnitTypeID = @UnitTypeID) BEGIN
			IF @DEBUG = 1 print @DBG + 'Doing update VillageUnits '
			update VillageUnits 
				set TotalCount = TotalCount + @NumRecruited 
				, CurrentCount = CurrentCount + @NumRecruited
				where VillageId =@VillageID and UnitTypeID = @UnitTypeID
		END ELSE BEGIN
			IF @DEBUG = 1 print @DBG + 'Doing insert into VillageUnits '
			insert into VillageUnits (VillageID, UnitTypeID, TotalCount, CurrentCount )
				values(@villageID, @UnitTypeID, @NumRecruited, @NumRecruited)
		END 

	END --WHILE (@AreThereMoreEntries = 1) 

	IF @DEBUG = 1 print @DBG + 'About to commit. @@TRANCOUNT='+ cast(@@TRANCOUNT AS VARCHAR(100))
	commit transaction
	IF @DEBUG = 1 print @DBG + 'Commited. @@TRANCOUNT='+ cast(@@TRANCOUNT AS VARCHAR(100))
end try
begin catch
    IF @@TRANCOUNT > 0 ROLLBACK
	DECLARE @ERROR_MSG AS VARCHAR(max)

	
	SET @ERROR_MSG = 'iCompleteUnitRecruitment FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @UnitTypeID' + ISNULL(CAST(@UnitTypeID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @RecruitmentEntryUnitCount' + ISNULL(CAST(@RecruitmentEntryUnitCount AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @RecruitmentBuildingID' + ISNULL(CAST(@RecruitmentBuildingID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @RecruitmentBuildingID_Previous' + ISNULL(CAST(@RecruitmentBuildingID_Previous AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @EntryID' + ISNULL(CAST(@EntryID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @RecruitStartTime' + ISNULL(CAST(@RecruitStartTime AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @TimeDifference' + ISNULL(CAST(@TimeDifference AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @TimeDifferenceFrom' + ISNULL(CAST(@TimeDifferenceFrom AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @now' + ISNULL(CAST(@now AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @NumRecruited' + ISNULL(CAST(@NumRecruited AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @BaseRecruitTime' + ISNULL(CAST(@BaseRecruitTime AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @ActualPerUnitRecruitTime' + ISNULL(CAST(@ActualPerUnitRecruitTime AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @ActualPerUnitRecruitTimeInMs' + ISNULL(CAST(@ActualPerUnitRecruitTimeInMs AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @RecruitTimeFactor' + ISNULL(CAST(@RecruitTimeFactor AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @UnitRecruitmentBuildingLevelPropertyID' + ISNULL(CAST(@UnitRecruitmentBuildingLevelPropertyID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @ContinueWithThisBuilding' + ISNULL(CAST(@ContinueWithThisBuilding AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @villageTypePercentBonus' + ISNULL(CAST(@villageTypePercentBonus AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @researchPercentBonus' + ISNULL(CAST(@researchPercentBonus AS VARCHAR(100)), 'Null') + CHAR(10)
		
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)

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



GO
