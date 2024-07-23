IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iCommandUnits')
	BEGIN
		DROP  Procedure  iCommandUnits
	END

GO

--
-- return -1 if the units to send are not available in the village
--
CREATE Procedure iCommandUnits
	@UnitTypes varchar(100)
	,@UnitAmounts varchar(100)
	,@UnitBuildingTargets varchar(100)
	,@OriginVillageID int
	,@DestVillageID int
	,@CommandType int -- 1:attack 0:support
	,@CompletedOn DateTime
	,@TripDurationInTicks Bigint
	,@OriginPlayerID int
	,@hasCommandTroopsPF bit
	,@DesertionFactor real
	,@playerMoraleAfterCmd int output 
	,@playerMoraleLastUpdatedAfterCmd DateTime output 
	,@PrintDebugInfo BIT =null 
	
AS
DECLARE @DEBUG INT, @DBG varchar(10)
IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
	SET @DEBUG = 1
	set @DBG = ':::::::'
	SET NOCOUNT OFF
END ELSE BEGIN
	SET @DEBUG = 0
	SET NOCOUNT ON
END 
IF @DEBUG = 1 print @DBG + 'BEGIN iCommandUnits ' 


declare @UnitTypeID int
declare @UnitAmountOriginal int
declare @UnitAmountActual int
declare @UnitAmountDeserting int
declare @UnitTargetBuilding int
declare @UnitTypes_CommaLoc int
declare @UnitTypes_CurLoc int
declare @UnitAmounts_CommaLoc int
declare @UnitAmounts_CurLoc int
declare @UnitBuildingTargets_CommaLoc int
declare @UnitBuildingTargets_CurLoc int
declare @EventID int
declare @PlayerMorale int 

declare @VisibleToTarget smallint
declare @AttackSpyStrength real
declare @TargetCounterSpyStrength real
declare @TargetCounterSpyStrength_TargetsOwnTroops real
declare @TargetCounterSpyStrength_SupportingTroops real
declare @SpySuccessChance as real -- the % chance that the spies are succesful

declare @DestPlayerID int

declare @SpySuccessChance_AlgA as real 
declare @SpySuccessChance_AlgB as real 
declare @spyIdentityKnownChance as real 
declare @spyAttackVisibleChance as real 

declare @morale_isactive as bit
declare @morale_deductAmount as int
declare @morale_deductAmountNPC as int
declare @morale_costOfThisAttack as int
declare @morale_costOfThisAttack_actual as int -- this can be different from @morale_costOfThisAttack if, for example, player is already at min morale
declare @morale_maxNormal as int

declare @isOnlySpyAttack bit 
declare @hasGovPresent bit
set @isOnlySpyAttack = 0
set @hasGovPresent = 0

declare @ReturnValue int
set @ReturnValue = 0 -- default value meaning all is well 

declare @AttackingUnits_ForAdmin as varchar(100)
SET @AttackingUnits_ForAdmin = ''

--
-- Constants
--
declare @UNIT_SPY_ID int
set @UNIT_SPY_ID = 12
declare @UNIT_LORD_ID int
set @UNIT_LORD_ID  = 10

--
-- Init vars 
--
set @VisibleToTarget = 2 -- all visible, default. 

BEGIN TRY
	--
	-- get the latest morale. This is done here, so that in case something fails in the SP and it returns without completing, we still return the valid latest morale info. 
	--
	select @playerMoraleAfterCmd = Morale, @playerMoraleLastUpdatedAfterCmd = MoraleLastUpdated from Players with(nolock) where playerid = @OriginPlayerID

    -- sanity check. desertion for attack only 
    IF @CommandType = 0 BEGIN
        SET @DesertionFactor = 0
    END

	--
	-- Validate preconditions - last char must be ','
	--
	IF substring(@UnitTypes, len(@UnitTypes),1) <> ',' BEGIN
		RAISERROR('@UnitTypes must end with a comma',11,1)	
	END 
	IF substring(@UnitAmounts, len(@UnitAmounts),1) <> ',' BEGIN
		RAISERROR('@UnitAmounts must end with a comma',11,1)	
	END 
	IF substring(@UnitBuildingTargets, len(@UnitBuildingTargets),1) <> ',' BEGIN
		RAISERROR('@UnitBuildingTargets must end with a comma',11,1)	
	END 
	--	
	-- Temp table to hold the units to be sent. 
	create table #temp 
	(
		UnitTypeID int,
		UnitAmountOriginal int, -- original amount to send, before desertion
		UnitAmountActual int, -- actual amount to send, after desertion
		UnitAmountDeserting int, -- if not 0, must take away this number of troops from the village; those troops deserted 
		UnitTargetBuilding int
	)


	IF @DEBUG = 1 print @DBG + '@UnitTypes:' + @UnitTypes 
	IF @DEBUG = 1 print @DBG + '@UnitAmounts:' + @UnitAmounts
	IF @DEBUG = 1 print @DBG + '@UnitBuildingTargets:' + @UnitBuildingTargets

	set @UnitTypes_CurLoc			= 1
	set @UnitAmounts_CurLoc			= 1
	set @UnitBuildingTargets_CurLoc	= 1

	set @UnitTypes_CommaLoc = charindex( ',', @UnitTypes)
	IF @DEBUG = 1 print @DBG + '@UnitTypes_CommaLoc:' + cast(@UnitTypes_CommaLoc as varchar(10))

	while (@UnitTypes_CommaLoc > 0) BEGIN
		IF @DEBUG = 1 print @DBG + 'WHILE'
		-- 
		-- read the unit type ID 
		--
		set @UnitTypeID	= substring(@UnitTypes, @UnitTypes_CurLoc ,@UnitTypes_CommaLoc -  @UnitTypes_CurLoc)
		IF @DEBUG = 1 print @DBG + '  *@UnitTypeID:' + cast(@UnitTypeID as varchar(10))
		--
		-- read the unit amount
		--
		set @UnitAmounts_CommaLoc = charindex( ',', @UnitAmounts, @UnitAmounts_CurLoc)
		IF @DEBUG = 1 print @DBG + '  @UnitAmounts_CommaLoc:' + cast(@UnitAmounts_CommaLoc as varchar(10))

		IF @UnitAmounts_CommaLoc is null OR @UnitAmounts_CommaLoc <= 0 BEGIN
			RAISERROR('Got different number of unitTypeIDs in @UnitTypes and unit amounts in @UnitAmounts. Check the commas',11,1)	
		END

		set @UnitAmountOriginal	= substring(@UnitAmounts, @UnitAmounts_CurLoc ,@UnitAmounts_CommaLoc -  @UnitAmounts_CurLoc)
		IF @DEBUG = 1 print @DBG + '  *@UnitAmountOriginal' + cast(@UnitAmountOriginal as varchar(10))
		--
		-- read the Target building
		--
		set @UnitBuildingTargets_CommaLoc = charindex( ',', @UnitBuildingTargets, @UnitBuildingTargets_CurLoc)
		IF @DEBUG = 1 print @DBG + '  @UnitBuildingTargets_CommaLoc:' + cast(@UnitBuildingTargets_CommaLoc as varchar(10))

		IF @UnitBuildingTargets_CommaLoc is null OR @UnitBuildingTargets_CommaLoc <= 0 BEGIN
			RAISERROR('Got different number of unitTypeIDs in @UnitTypes and unit target buildings in @UnitBuildingTargets. Check the commas',11,1)	
		END

		set @UnitTargetBuilding	= substring(@UnitBuildingTargets, @UnitBuildingTargets_CurLoc ,@UnitBuildingTargets_CommaLoc -  @UnitBuildingTargets_CurLoc)
		IF @DEBUG = 1 print @DBG + '  *@UnitTargetBuilding:' + cast(@UnitTargetBuilding as varchar(10))

		IF @UnitTargetBuilding = -1 BEGIN -- -1 means no target building		
			SET @UnitTargetBuilding = null 
		END  

		--
		-- Update CurLoc
		set @UnitTypes_CurLoc = @UnitTypes_CommaLoc + 1
		set @UnitAmounts_CurLoc = @UnitAmounts_CommaLoc + 1
		set @UnitBuildingTargets_CurLoc = @UnitBuildingTargets_CommaLoc + 1
		IF @DEBUG = 1 print @DBG + '  @UnitTypes_CurLoc :' + cast(@UnitTypes_CurLoc as varchar(10))
		IF @DEBUG = 1 print @DBG + '  @UnitAmounts_CurLoc :' + cast(@UnitAmounts_CurLoc as varchar(10))
		IF @DEBUG = 1 print @DBG + '  @UnitBuildingTargets_CurLoc :' + cast(@UnitBuildingTargets_CurLoc as varchar(10))

		-- get next comma location
		set @UnitTypes_CommaLoc = charindex(',', @UnitTypes,@UnitTypes_CurLoc)
		IF @DEBUG = 1 print @DBG + '  @UnitTypes_CommaLoc:' + cast(@UnitTypes_CommaLoc as varchar(10))

        -- calculate desertion, if any 
        SET @UnitAmountActual = @UnitAmountOriginal
        SET @UnitAmountDeserting = 0
        if @DesertionFactor > 0 BEGIN
            SET @UnitAmountDeserting = round(@UnitAmountOriginal * @DesertionFactor,0)
            SET @UnitAmountActual = @UnitAmountActual - @UnitAmountDeserting
        END

		SET @AttackingUnits_ForAdmin = @AttackingUnits_ForAdmin + '(' + cast(@UnitTypeID as varchar(10)) + ')' + cast(@UnitAmountActual as varchar(10)) + ','

        --
        -- remember this unit
		insert into #temp values (@UnitTypeID, @UnitAmountOriginal, @UnitAmountActual,@UnitAmountDeserting, @UnitTargetBuilding)
		IF @DEBUG = 1 print @DBG + 'END (while)'
	END

	IF @DEBUG = 1 select * from #temp T 

	SELECT @DestPlayerID = ownerplayerid from Villages where villageID = @DestVillageID

	
	--
	-- has gov present? has only spies?
	--
	IF NOT EXISTS (SELECT * FROM #temp WHERE UnitTypeID <> @UNIT_SPY_ID) -- if no non-spy units exist
		AND EXISTS (SELECT * FROM #temp WHERE UnitTypeID = @UNIT_SPY_ID) -- if spy units exist
	BEGIN
		set @isOnlySpyAttack = 1
	END
	IF EXISTS (SELECT * FROM #temp WHERE UnitTypeID = @UNIT_LORD_ID ) -- if gov units exist
	BEGIN
		set @hasGovPresent = 1
	END

	--
	-- Morale system params
	--
	select @morale_isactive = AttribValue from RealmAttributes where attribid = 70
	if @morale_isactive=1 BEGIN
		select @morale_deductAmount = AttribValue from RealmAttributes where attribid = 71 -- 'Morale System - amount to deduce for attacks on real players') 
		select @morale_deductAmountNPC = AttribValue from RealmAttributes where attribid = 72 -- 'Morale System - amount to deduce for attacks on NPC') 
		select @morale_maxNormal = AttribValue from RealmAttributes where attribid = 75 -- 'Morale System - max normal morale') 
	END
	--
	-- From now on, everything must be in a transaction. 
	--
	BEGIN TRAN 
		--
		-- Obtain a village LOCK
		--		this will ensure that no one else will change anything in the village, or effect the upgrade Q 
		--
		update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID in (@OriginVillageID, @DestVillageID)
		--
		-- ensure VillageUnits table is uptodate
		--
		exec iCompleteUnitRecruitment @OriginVillageID
		exec iCompleteUnitRecruitment @DestVillageID
		
		--
		-- If Attack AND only spies attacking, then figure out if the attack is perhaps invisible. 
		--
		IF NOT EXISTS (SELECT * FROM #temp WHERE UnitTypeID <> @UNIT_SPY_ID) -- if no non-spy units exist
			AND EXISTS (SELECT * FROM #temp WHERE UnitTypeID = @UNIT_SPY_ID) -- if spy units exist
			AND @CommandType = 1											 -- if this is an Attack		
			BEGIN
			-- Since only spies are attacking, then at the very least, we do not show the origin village
			SET @VisibleToTarget = 1
		
			-- get attack spyability strength 
			select @AttackSpyStrength = sum(T.UnitAmountActual * UT.SpyAbility)
				from #temp T
				join UnitTypes UT
					on UT.UnitTypeID = T.UnitTypeID
				group by T.UnitTypeID					
		
			-- get the target's counter spyability strength - target's own troops	
			select @TargetCounterSpyStrength_TargetsOwnTroops = sum( VU.CurrentCount * UT.CounterSpyAbility) 
				from VillageUnits VU
				join UnitTypes UT
					on VU.UnitTypeID = UT.UnitTypeID
				where VillageID = @DestVillageID	
					
			-- get the target's counter spyability strength - target's supporting troops	
			select @TargetCounterSpyStrength_SupportingTroops = sum( VSU.UnitCount * UT.CounterSpyAbility)  
				from VillageSupportUnits VSU
				join UnitTypes UT
					on VSU.UnitTypeID = UT.UnitTypeID
				where SupportedVillageID = @DestVillageID
					AND VSU.UnitCount >0

			SET @TargetCounterSpyStrength = 20 /*a village has a base counter spyability of 20*/
				+ isnull(@TargetCounterSpyStrength_TargetsOwnTroops, 0)
				+ isnull(@TargetCounterSpyStrength_SupportingTroops, 0)				
				
			EXEC qCalculateSpyStuff @AttackSpyStrength 
				,@TargetCounterSpyStrength
				,@SpySuccessChance output  
				,@SpySuccessChance_AlgA output 
				,@SpySuccessChance_AlgB output 
				,@spyIdentityKnownChance output 
				,@spyAttackVisibleChance output 
							
			-- Now, if chance shows the spy attack being 'sucessful', then we hide the attack completely from the target village/player
			IF RAND() <= @SpySuccessChance BEGIN
				SET @VisibleToTarget = 0
			END

			IF @DEBUG = 1 SELECT @AttackSpyStrength as '@AttackSpyStrength'
							, @TargetCounterSpyStrength as '@TargetCounterSpyStrength'
							, @TargetCounterSpyStrength_TargetsOwnTroops as '@TargetCounterSpyStrength_TargetsOwnTroops'
							, @TargetCounterSpyStrength_SupportingTroops as '@TargetCounterSpyStrength_SupportingTroops'
							, @SpySuccessChance as '@SpySuccessChance'
							, @VisibleToTarget as '@VisibleToTarget'			
		END 				
		--
		-- CHECK IF this amount of units is available in village
		--		
		IF EXISTS(select * from #temp T 
			left join VillageUnits UV
				on T.UnitTypeID = UV.UnitTypeID
				and UV.VillageID = @OriginVillageID
			where VillageID is null 
				or CurrentCount - UnitAmountOriginal <0
			) BEGIN
			IF @DEBUG = 1 print @DBG + '  ERROR:Specified units not avail in village'

			--
			-- EXIT 
			-- We exit without an error because this could happen in case two different commands were executed simulatniously
			--	This should not happen but it can so we just abondon quietly. 
			SET @REturnValue = -1
			GOTO DONE
		END
		

		--
		-- Finally do the thing - insert the necessary stuff to DB to execute the command
		--
		insert into Events (EventTime) values ( @CompletedOn )	
		set @EventID = SCOPE_IDENTITY() 
		IF @DEBUG = 1 print @DBG + '  @EventID:' + cast(@EventID as varchar(10))

		insert into UnitMovements (EventID, OriginVillageID, DestinationVillageID, CommandType, TripDuration, Loot, VisibleToTarget)
			values (@EventID, @OriginVillageID, @DestVillageID, @CommandType, @TripDurationInTicks,0, @VisibleToTarget)

		insert into UnitsMoving 
			select @EventID, T.UnitTypeID, T.UnitAmountActual, T.UnitTargetBuilding
			from #temp T
			
		--
		-- Grab any additional attributes necessary. 
		--  This grab the attack bonus pf of attacker if he has it active
		--				
        insert UnitMovements_Attributes (EventID, AttribID)   
	        select @EventID, PFs.FeatureID 
	        from PFs
	        join PFsInPackage on
		        PFsInPackage.FeatureID=PFs.FeatureID
	        join PFPackages on
		        PFPackages.PFPackageID=PFsInPackage.PFPackageID
	        join PlayersPFPackages on
		        PlayersPFPackages.PFPackageID=PFsInPackage.PFPackageID
	        where 
			        PlayerID=@OriginPlayerID 
		        AND PFs.FeatureID = 26 -- ATTACK BONUS
		        AND ExpiresOn>getdate()
		--
		--  Grab the morale for attacks and save in attack attributes. 
		--		spy only attacks, or attacks with gov alweays travel at 100 (@morale_maxNormal)
		--			
		IF  @morale_isactive=1 AND @CommandType = 1 -- if this is an Attac 
		BEGIN		
			exec @PlayerMorale = uPlayerMorale @OriginPlayerID
			if @isOnlySpyAttack = 1 BEGIN
				SET @PlayerMorale = @morale_maxNormal
			END
			insert UnitMovements_Attributes (EventID, AttribID, AttribValue) select @EventID, 1,@PlayerMorale
		END     
		
				
		update VillageUnits set CurrentCount = CurrentCount - T.UnitAmountActual
			from VillageUnits UV 
			join #temp T 
				on T.UnitTypeID = UV.UnitTypeID
				and UV.VillageID = @OriginVillageID

		update VillageUnits set CurrentCount = CurrentCount - T.UnitAmountDeserting
		    , TotalCount = TotalCount - T.UnitAmountDeserting
			from VillageUnits UV 
			join #temp T 
				on T.UnitTypeID = UV.UnitTypeID
				and UV.VillageID = @OriginVillageID

		--
		-- update cache time stamps
		--
		-- outgoing cache item (2) 
		UPDATE PlayerCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @OriginPlayerID and CachedItemID = 2  
		IF (@@rowcount < 1 ) BEGIN
			INSERT INTO PlayerCacheTimeStamps values (@OriginPlayerID, 2, getdate())
		END
		-- incoming cache item (1) 
		UPDATE PlayerCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @DestPlayerID and CachedItemID = 1  
		IF (@@rowcount < 1 ) BEGIN
			INSERT INTO PlayerCacheTimeStamps values (@DestPlayerID, 1, getdate())
		END
		-- say that troops in the village of origin have changed
		UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @OriginPlayerID and VillageID = @OriginVillageID and CachedItemID = 0
		IF (@@rowcount < 1 ) BEGIN
			INSERT INTO VillageCacheTimeStamps values (@OriginPlayerID, @OriginVillageID, 0, getdate())
		END

		--
		-- Update player morale, if required
		--
		IF  @morale_isactive=1 
			AND @CommandType = 1 -- if this is an Attac 
			AND  NOT ( @isOnlySpyAttack = 1) 
		BEGIN
			IF exists(select * from specialplayers where playerid = @DestPlayerID) BEGIN
				set @morale_costOfThisAttack = 0 - @morale_deductAmountNPC
			END ELSE BEGIN 
				set @morale_costOfThisAttack = 0 - @morale_deductAmount
			END
			
			EXEC uPlayerMorale_SubtractAdd_Unsafe @OriginPlayerID, @morale_costOfThisAttack, @morale_costOfThisAttack_actual out	
			insert UnitMovements_Attributes (EventID, AttribID, AttribValue) select @EventID, 2, 0- @morale_costOfThisAttack_actual	
		END

		--
		-- get the latest morale
		--
		select @playerMoraleAfterCmd = Morale, @playerMoraleLastUpdatedAfterCmd = MoraleLastUpdated from Players with(nolock) where playerid = @OriginPlayerID

		IF @DEBUG = 1 select * from VillageUnits where VillageID = @OriginVillageID
		IF @DEBUG = 1 select * from UnitMovements
		IF @DEBUG = 1 select * from UnitsMoving 
		IF @DEBUG = 1 select * from Events;

DONE:
	COMMIT TRAN 

	--
	-- LOG TO ADMIN REPORT TRACKING
	--
	IF @REturnValue = 0 BEGIN
		IF @CommandType = 1 BEGIN
			INSERT INTO admin_attackLog
			( eventID ,
				attackerPID ,
				attackerVID , 
				defenderPID ,
				defenderVID , 
				launchTime ,
				attackTroops,
				morale)		
			 values (@EventID, @OriginPlayerID, @OriginVillageID, @DestPlayerID, @DestVillageID, getdate(), @AttackingUnits_ForAdmin, @PlayerMorale )
		END
	END
	
	--
	-- Note the last target in recent target stack 
	--
	IF @hasCommandTroopsPF = 1 BEGIN
	    EXEC uPlayerRecentTargetStack @OriginPlayerID, @DestVillageID
	END
	
	IF @REturnValue <> 0 BEGIN
		RETURN @REturnValue
	END
END TRY 
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iCommandUnits FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @UnitTypes'			+ ISNULL(@UnitTypes, 'Null') + CHAR(10)
		+ '   @UnitAmounts'			+ ISNULL(@UnitAmounts, 'Null') + CHAR(10)
		+ '   @UnitBuildingTargets' + ISNULL(@UnitAmounts, 'Null') + CHAR(10)		
		+ '   @OriginVillageID'		+ ISNULL(CAST(@OriginVillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @DestVillageID'		+ ISNULL(CAST(@DestVillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CommandType'			+ ISNULL(CAST(@CommandType AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CompletedOn'			+ ISNULL(CAST(@CompletedOn AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @TripDurationInTicks' + ISNULL(CAST(@TripDurationInTicks AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @PrintDebugInfo'		+ ISNULL(CAST(@PrintDebugInfo AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @UnitAmounts_CurLoc'	+ ISNULL(CAST(@UnitAmounts_CurLoc AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @UnitAmounts_CommaLoc'+ ISNULL(CAST(@UnitAmounts_CommaLoc AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @UnitTypes_CurLoc'	+ ISNULL(CAST(@UnitTypes_CurLoc AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @UnitTypes_CommaLoc'	+ ISNULL(CAST(@UnitTypes_CommaLoc AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @UnitBuildingTargets_CurLoc'	+ ISNULL(CAST(@UnitBuildingTargets_CurLoc AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @UnitBuildingTargets_CommaLoc'+ ISNULL(CAST(@UnitBuildingTargets_CommaLoc AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @UnitAmountActual'			+ ISNULL(CAST(@UnitAmountActual AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @UnitAmountOriginal'			+ ISNULL(CAST(@UnitAmountOriginal AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @UnitAmountDeserting'			+ ISNULL(CAST(@UnitAmountDeserting AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @UnitTargetBuilding'	+ ISNULL(CAST(@UnitTargetBuilding AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @EventID'				+ ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @UnitTypeID'			+ ISNULL(CAST(@UnitTypeID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @VisibleToTarget'		+ ISNULL(CAST(@VisibleToTarget AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @OriginPlayerID'		+ ISNULL(CAST(@OriginPlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @hasCommandTroopsPF'		+ ISNULL(CAST(@hasCommandTroopsPF AS VARCHAR(10)), 'Null') + CHAR(10)
		
		
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
UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @OriginPlayerID and VillageID = @OriginVillageID and CachedItemID = 0
IF (@@rowcount < 1 ) BEGIN
	INSERT INTO VillageCacheTimeStamps values (@OriginPlayerID, @OriginVillageID, 0, getdate())
END

GO