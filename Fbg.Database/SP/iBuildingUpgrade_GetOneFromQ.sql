IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iBuildingUpgrade_GetOneFromQ')
	BEGIN
		DROP  Procedure  iBuildingUpgrade_GetOneFromQ
	END
GO


CREATE Procedure iBuildingUpgrade_GetOneFromQ
	@VillageID int
	, @LastUpgradeCompletedOn DateTime
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
	IF @DEBUG = 1 SELECT 'BEGIN iBuildingUpgrade_GetOneFromQ ' + cast(@VillageID as varchar(10))

	declare @BuildingTypeID int
	declare @QEntryID int
	declare @EventID int
	declare @Cost int
	declare @BaseUpgradeTime bigint
	declare @ActualUpgradeTime bigint
	declare @UpgradeLevelTimeFactor real 
	declare @UpgradeTimeFactor real 
	declare @researchPercentBonus real 
	declare @CompletedOn as DateTime
	declare @CurrentBuildingLevel int
	declare @UpgradeBuildingLevel int
	
	
	-- CONSTS
	declare @LEVELPROP_HQTimeFactor int
	set @LEVELPROP_HQTimeFactor = 1


	begin try 
	BEGIN TRAN GetOneFromQ
		--
		-- Obtain a village LOCK - WE DO *NOT* obtain a village lock on purpose to avoid deadlocks
		--		update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @VillageID


		--
		-- Check currently upgrading building and make sure there is none at this time. 
		--
		IF not exists (
			select * 
			from BuildingUpgrades BU
			join  Events E on BU.EventID = E.EventID 		
			where VillageID = @VillageID
			and status <> 1
		) BEGIN
			--
			-- get the next upgrade in Q 
			--
			select top 1 
				@BuildingTypeID = BuildingTypeID 
				, @QEntryID = QEntryID
			from BuildingUpgradeQEntries BUQ
			where
				VillageID = @VillageID
			order by DateAdded asc
			IF @DEBUG = 1 SELECT @BuildingTypeID as '@BuildingTypeID', @QEntryID '@QEntryID'

			--
			-- If there is some upgrade then start doing it. 
			--	PLEASE NOTE - we do not verify any building requirements
			--
			IF @BuildingTypeID is not null BEGIN
				-- 
				-- determin the current level of this building in the village
				--			
				set @CurrentBuildingLevel = (select level from Buildings where VillageID = @VillageID and BuildingTypeID = @BuildingTypeID)
				IF @CurrentBuildingLevel is null BEGIN 
					set @UpgradeBuildingLevel = 1
				END ELSE BEGIN
					set @UpgradeBuildingLevel = @CurrentBuildingLevel + 1
				END 
				--
				-- Get the cost and time of this upgrade
				--
				SELECT @cost = cost 
					, @BaseUpgradeTime = BuildTime
					FROM buildingLevels where buildingTypeID = @BuildingTypeID and [level] = @UpgradeBuildingLevel
				
  				-- get level factor. will be like 95 meaning 95%
				set @UpgradeTimeFactor = dbo.fnGetBuildingProperty(@VillageID, @LEVELPROP_HQTimeFactor)
                	    
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
			    SET @UpgradeTimeFactor = @UpgradeTimeFactor /  (1+ @researchPercentBonus) 
				    
				set @ActualUpgradeTime = @BaseUpgradeTime * (@UpgradeTimeFactor/100)
				set @CompletedOn = dateadd(millisecond, @ActualUpgradeTime/10000, @LastUpgradeCompletedOn)
				IF @DEBUG = 1 SELECT @UpgradeTimeFactor as '@UpgradeTimeFactor', @BaseUpgradeTime as '@BaseUpgradeTime'
					, @ActualUpgradeTime as '@ActualUpgradeTime', @CompletedOn as '@CompletedOn'
				
				--
				-- Create the event to do this upgrade
				--			
				insert into Events (EventTime, Status) values(@CompletedOn, 0)
				set @EventID = SCOPE_IDENTITY() 
				insert into BuildingUpgrades (VillageID, EventID, BuildingTypeID, Level) 
					 values (@VillageID, @EventID, @BuildingTypeID, @UpgradeBuildingLevel)
				--
				-- update the Q recruit count
				--
				delete BuildingUpgradeQEntries where QEntryID  = @QEntryID 
								
			END
			
			
		END ELSE BEGIN		
		
			IF @DEBUG = 1 SELECT 'BuildingUpgrades not empty. Do nothing'
		END

		
			
	COMMIT TRAN GetOneFromQ	
	IF @DEBUG = 1 SELECT 'END iBuildingUpgrade_GetOneFromQ ' + cast(@VillageID as varchar(10))
	end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION 

	
	SET @ERROR_MSG = 'iBuildingUpgrade_GetOneFromQ FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @VillageID'			+ ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @BuildingTypeID'		+ ISNULL(CAST(@BuildingTypeID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @QEntryID'			+ ISNULL(CAST(@QEntryID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @BaseUpgradeTime'		+ ISNULL(CAST(@BaseUpgradeTime AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @EventID'				+ ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @ActualUpgradeTime'	+ ISNULL(CAST(@ActualUpgradeTime AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @UpgradeTimeFactor'	+ ISNULL(CAST(@UpgradeTimeFactor AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CompletedOn'			+ ISNULL(CAST(@CompletedOn AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @LastUpgradeCompletedOn'+ ISNULL(CAST(@LastUpgradeCompletedOn AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @CurrentBuildingLevel' + ISNULL(CAST(@CurrentBuildingLevel AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @UpgradeBuildingLevel' + ISNULL(CAST(@UpgradeBuildingLevel AS VARCHAR(10)), 'Null') + CHAR(10)
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