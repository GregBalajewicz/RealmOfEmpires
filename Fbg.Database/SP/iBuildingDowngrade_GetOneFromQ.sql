
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iBuildingDowngrade_GetOneFromQ')
	BEGIN
		DROP  Procedure  iBuildingDowngrade_GetOneFromQ
	END
GO

CREATE Procedure iBuildingDowngrade_GetOneFromQ
	@VillageID int
	, @LastDowngradeCompletedOn DateTime
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
	IF @DEBUG = 1 SELECT 'BEGIN iBuildingDowngrade_GetOneFromQ ' + cast(@VillageID as varchar(10))

	declare @BuildingTypeID int
	declare @QEntryID int
	declare @EventID int
	declare @Cost int
	declare @BaseDowngradeTime bigint
	declare @ActualDowngradeTime bigint
	declare @DowngradeLevelTimeFactor real 
	declare @DowngradeTimeFactor real 
	declare @researchPercentBonus real 
	declare @CompletedOn as DateTime
	declare @CurrentBuildingLevel int	
	declare @MinLevel as int

	-- CONSTS
	declare @LEVELPROP_HQTimeFactor int
	set @LEVELPROP_HQTimeFactor = 1


	begin try 
	BEGIN TRAN GetOneFromQ
		--
		-- Obtain a village LOCK - WE DO *NOT* obtain a village lock on purpose to avoid deadlocks
		--		update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @VillageID

		--
		-- Check currently downgrading building and make sure there is none at this time. 
		--
		IF not exists (
			select * 
			from BuildingDowngrades BU
			join  Events E on BU.EventID = E.EventID 		
			where VillageID = @VillageID
			and status <> 1
		) BEGIN
			--
			-- get the next downgrade in Q 
			--
			select top 1 
				@BuildingTypeID = BuildingTypeID 
				, @QEntryID = QEntryID
			from BuildingDowngradeQEntries BUQ
			where
				VillageID = @VillageID
			order by DateAdded asc
			IF @DEBUG = 1 SELECT @BuildingTypeID as '@BuildingTypeID', @QEntryID '@QEntryID'
			--
			-- remove this Q entry
			--
			delete BuildingdowngradeQEntries where QEntryID  = @QEntryID 


			--
			-- If there is some downgrade then start doing it. 
			--
			IF @BuildingTypeID is not null BEGIN
				-- 
				-- determin the current level of this building in the village, and get the minimum level
				--			
				SELECT @CurrentBuildingLevel = level from Buildings where VillageID = @VillageID and BuildingTypeID = @BuildingTypeID
				SELECT @MinLevel = MinimumLevelAllowed  FROM buildingtypes where BuildingTypeID = BuildingTypeID
		        --
		        -- only continue if building at the village is at a greater them min level
		        --
		        IF @CurrentBuildingLevel is not null AND @CurrentBuildingLevel > @MinLevel BEGIN 
				    --
				    -- Get the time of this downgrade
				    --
				    SELECT @BasedowngradeTime = BuildTime
					    FROM buildingLevels where buildingTypeID = @BuildingTypeID and [level] = @CurrentBuildingLevel
    				
    				-- get level factor. will be like 95 meaning 95%
				    set @DowngradeLevelTimeFactor = dbo.fnGetBuildingProperty(@VillageID, @LEVELPROP_HQTimeFactor)				                       	
                    	    
                    -- get research bonus. will be like 0.1 meaing 10%
                    select @researchPercentBonus = sum(cast(PropertyValue as float))
                        from ResearchItemPropertyTypes PT 
                        join ResearchItemProperties P on P.ResearchItemPropertyID = PT.ResearchItemPropertyID
                        join ResearchItems RI on RI.ResearchItemTypeID = P.ResearchItemTypeID AND RI.ResearchItemID = P.ResearchItemID
                        join PlayerResearchItems PRI ON RI.ResearchItemTypeID = PRI.ResearchItemTypeID AND RI.ResearchItemID = PRI.ResearchItemID
                        where PT.PropertyID = @LEVELPROP_HQTimeFactor
                            AND PlayerID = (select OwnerPlayerID from Villages where VillageID = @VillageID)
                            AND PT.Type = 3 			            
                    SET @researchPercentBonus = isnull(@researchPercentBonus,0)
                    IF @DEBUG = 1 select @researchPercentBonus as '@researchPercentBonus'		            
                    
                    -- get the actual downgrade factor with reserch bonus applied	    
				    SET @DowngradeTimeFactor = @DowngradeLevelTimeFactor /  (1+ @researchPercentBonus) 
				    
				    set @ActualdowngradeTime = @BasedowngradeTime * (@downgradeTimeFactor/100)				   				    
				    
				    set @CompletedOn = dateadd(millisecond, @ActualdowngradeTime/10000, @LastdowngradeCompletedOn)
				    IF @DEBUG = 1 SELECT @downgradeTimeFactor as '@downgradeTimeFactor', @BasedowngradeTime as '@BasedowngradeTime'
					    , @ActualdowngradeTime as '@ActualdowngradeTime', @CompletedOn as '@CompletedOn'    				
				    --
				    -- Create the event to do this downgrade
				    --			
				    insert into Events (EventTime, Status) values(@CompletedOn, 0)
				    set @EventID = SCOPE_IDENTITY() 
				    insert into Buildingdowngrades (VillageID, EventID, BuildingTypeID, OriginalDuration, InitiatedOn) 
					     values (@VillageID, @EventID, @BuildingTypeID, @ActualdowngradeTime, @LastdowngradeCompletedOn)
                END ELSE BEGIN 
                    -- this village does not have this building at all or building alreayd at min level 
			        IF @DEBUG = 1 SELECT ' @CurrentBuildingLevel is 0 or less then min level. Nothing to do except remove Q entry'
		        END 
			END
			
			
		END ELSE BEGIN		
		
			IF @DEBUG = 1 SELECT 'BuildingDowngrades not empty. Do nothing'
		END

		
			
	COMMIT TRAN GetOneFromQ	
	IF @DEBUG = 1 SELECT 'END iBuildingdowngrade_GetOneFromQ ' + cast(@VillageID as varchar(10))
	end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION 

	
	SET @ERROR_MSG = 'iBuildingDowngrade_GetOneFromQ FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @VillageID'			+ ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @BuildingTypeID'		+ ISNULL(CAST(@BuildingTypeID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @QEntryID'			+ ISNULL(CAST(@QEntryID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @BasedowngradeTime'		+ ISNULL(CAST(@BasedowngradeTime AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @EventID'				+ ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @ActualdowngradeTime'	+ ISNULL(CAST(@ActualdowngradeTime AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @downgradeTimeFactor'	+ ISNULL(CAST(@downgradeTimeFactor AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CompletedOn'			+ ISNULL(CAST(@CompletedOn AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @LastdowngradeCompletedOn'+ ISNULL(CAST(@LastdowngradeCompletedOn AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @CurrentBuildingLevel' + ISNULL(CAST(@CurrentBuildingLevel AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @Cost'				+ ISNULL(CAST(@Cost AS VARCHAR(10)), 'Null') + CHAR(10)
		
		+ '   ERROR_NUMBER():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'	+ ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'	+ ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'		+ ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'		+  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
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