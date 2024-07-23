IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iProcessUnitMovement_Attack')
	BEGIN
		DROP  Procedure  iProcessUnitMovement_Attack
	END

GO

CREATE Procedure iProcessUnitMovement_Attack
	@EventID as int
	, @PrintDebugInfo BIT = null
AS

DECLARE @ERROR_MSG AS VARCHAR(max)
declare @UnitSpeed int -- speed of the slowest unit returning from the attack
declare @TripDurationInTicks bigint -- the duration of travel of the returning troops
declare @AttackingVillageID int
declare @AttackingPlayerName varchar(25)
declare @AttackingPlayerID int
declare @AttackingPlayerPoints int
declare @AttackingVillageXCord int
declare @AttackingVillageYCord int
declare @AttackingVillageName varchar(25)
declare @AttackingVillageName_Full varchar(max)
declare @TargetVillageID int
declare @TargetVillageName varchar(25)
declare @TargetVillageName_Full varchar(max)
declare @TargetPlayerID int  
declare @TargetVillageTypeID int
declare @TargetPlayerName varchar(25)
declare @TargetVillageXCord int
declare @TargetVillageYCord int
declare @TargetPlayersPoints int
declare @TargetSpecialPlayerTypeID as int -- null if target player is a regular player 
declare @AttackStrength real
declare @AttackStrengthBonusMultipier real -- will be in the form of 1.20 if the bonus is 20%
declare @AttackStrengthMoraleMultiplier real -- will be in the form of 1.1 if the bonus is 10% or .9 for a 10% penalty
declare @AttackMorale int
declare @Morale_MaxNormal int
declare @Morale_SpeedAdj real
declare @TargetStrength real
declare @TargetStrengthBonusMultipier real -- will be in the form of 1.20 if the bonus is 20%
declare @BattleTime DateTime 
declare @VillageCoins int
declare @LootableVillageCoins int -- village coins must those hidden in the hiding spot
declare @MaxLoot int
declare @ActualLoot int
declare @ReportID int
declare @SupportAttackedReportID int
declare @LoyaltyBeforeAttack int
declare @LoyaltyChange int
declare @GovHasNoEffect bit -- because this is the defenders last village AND this realm prevents loos of last village
declare @VillageTakenOver bit
--declare @TotalNumberOfAttackingUnits int
declare @TotalNumberOfAttackingUnitsByPop int
declare @TargetStrengthWithBuildingBonus real
declare @BuildingDefenseBonus_PostAttack real
declare @BuildingDefenseBonus_PreAttack real 
declare @BuildingDefenseBonus_Delta real 
declare @BuildingDefenseBonus_EffectiveDelta real
declare @AttackToTargetStrenghRatio_NoBuildingBonus real
declare @DefendersReportSubject nvarchar(200)
declare @FullReportSubject nvarchar(200)
declare @AttackerReportRecordID bigint 
declare @DefenderReportRecordID bigint 
declare @UpdateVillagePoints bit -- indicates if buildings in village were effected and if the points need to be calculated

declare @AttackSpyStrength real
declare @TargetCounterSpyStrength real
declare @SpySuccessChance as real -- the % chance that the spies were succesful
declare @SpyFailureChance as real -- 1 - @SpySuccessChance
declare @SpySuccessChance_AlgA as real 
declare @SpySuccessChance_AlgB as real 
declare @spyIdentityKnownChance as real 
declare @spyAttackVisibleChance as real 
declare @AttackerSpiesKilledPercentage as real -- btween 0 and 1 
declare @SpySuccessful as bit
declare @DefenderKnowsAttackersIdentify as bit	-- this can only be false if only spies are attacking
declare @isSpyOnlyAttack as bit					-- this is true if only spies were attacking 
declare @DefendersTroopsVisibleToAttacker as bit -- true if (a) at least 1 none spy unit survived OR (b) spies were succesful
declare @HasAttackersNonSpyUnitsSurvived as bit -- true if at least 1 non-spy unit survived
declare @SupportAttackedReportSubject as nvarchar(200)

declare @AbandonedPlayerID int
declare @RebelPlayerID int 		
declare @ThisBattleHandicap real
declare @ThisBattleHandicap_MaxHandicap real 
declare @ThisBattleHandicap_Steepness int
declare @ThisBattleHandicap_StartRatio int
declare @BattleHandicapAttackerStrengthMultilier real -- (1- @ThisBattleHandicap)
declare @PointsRatio real
declare @HidingSpotCapacity real 
--declare @BattleAlgorithmVersion int
declare @ChestsForLastGov int

declare @villageTypePercentBonus float
declare @researchPercentBonus float
declare @researchPercentBonus_villageDef float
declare @AttackersOffensiveResearchPercentBonus float
declare @isAttackReturning bit -- set to 1 if the attack is returning after the attack - attacker shoudl see the troops returning

declare @morale_isactive as bit

declare @RealmType varchar(100)
declare @RealmSubType varchar(100) -- Holiday14d etc
SELECT @RealmType = attribvalue FROM RealmAttributes WHERE attribID = 2000 
select @RealmSubType =  attribvalue from RealmAttributes where attribid =2001


-- -1 - no spies too part in the attack
-- 0 - spies took part in the attack but were unsucessful 
-- 1 - spies took part in the attack and were sucessful
declare @SpyOutcome as smallint


--
-- multipliers - multiply by troops to get # of troop that died in the fight.
-- 1 means all troops died, 0 means no troops died, 0.2 means 20% of troops died
--
declare @AttackKillFactor as real -- multiplier - multiply by attacking troops to get # dead
declare @TargetKillFactor as real -- multiplier - multiply by defending troops to get # dead

--
-- Constants
--
declare @LORD_UNIT_TYPE_ID int -- constant
declare @LOYALTY_DEC_BASE int -- constant
declare @LOYALTY_DEC_VAR int -- constant
declare @LOYALTY_AFTER_TAKE_OVER int -- constant
declare @UNIT_SPY_ID int
declare @LEVELPROP_HidingSpotCapacity int


set @LORD_UNIT_TYPE_ID = 10
set @LOYALTY_AFTER_TAKE_OVER = 50
set @UNIT_SPY_ID = 12
set @LEVELPROP_HidingSpotCapacity = 13

--
-- get the @LOYALTY_DEC_BASE and @LOYALTY_DEC_VAR from params table with default values if missing
--
SELECT @LOYALTY_DEC_BASE = AttribValue from RealmAttributes Where AttribID = 35
SELECT @LOYALTY_DEC_VAR = AttribValue from RealmAttributes Where AttribID = 36       
SET @LOYALTY_DEC_BASE = isnull(@LOYALTY_DEC_BASE, 13)
SET @LOYALTY_DEC_VAR = isnull(@LOYALTY_DEC_VAR, 4) 

--
-- init vars
--	
set @VillageTakenOver= 0
SET @SpySuccessful = 0
SET @DefenderKnowsAttackersIdentify = 0
SET @isSpyOnlyAttack = 0
SET @DefendersTroopsVisibleToAttacker = 0
SET @HasAttackersNonSpyUnitsSurvived = 0
SET @SpyOutcome = -1 
SET @UpdateVillagePoints = 0
select @morale_isactive = AttribValue from RealmAttributes where attribid = 70
SET @morale_isactive  = ISNULL(@morale_isactive , 0)
select @Morale_MaxNormal = AttribValue from RealmAttributes where attribid = 75 --'Morale System - max normal morale'
SET @AttackMorale = @Morale_MaxNormal -- default value, in case we do not have morale on this attack


declare @BotProtectionFactor as real 


DECLARE @DEBUG INT, @DBG varchar(10)
IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
	SET @DEBUG = 1
	set @DBG = ':::::::'
	SET NOCOUNT OFF
END ELSE BEGIN
	SET @DEBUG = 0
	SET NOCOUNT ON
END 
IF @DEBUG = 1 SELECT 'BEGIN iProcessUnitMovement_Attack ' + cast(@EventID as varchar(10));


create table #TargetStrength(strength int)
create table #TargetUnits (UnitTypeID int, DeployedUnitCount int, KilledUnitCount int)
create table #TargetSupportingUnits (SupportingVillageID int, SupportingVillageOwnerPlayerID int, UnitTypeID int, DeployedUnitCount int, KilledUnitCount int)
create table #TargetSupportingUnitsReports ([index] INT IDENTITY(1,1), SupportingVillageOwnerPlayerID int)
create table #TargetUnitDefenseStrength (UnitTypeID int, DefenseStrength real)
create table #AttackerUnits (UnitTypeID int, DeployedUnitCount int, KilledUnitCount int)
create table #BuildingAttacks (BuildingTypeID int, AttackStrength real, CurrentLevelInVillage int, PointsAfterAttack int null, LevelAfterAttack int )
create table #ChangedVillages (PlayerID int, VillageID int)

set @BattleTime = getdate()
set @ActualLoot = 0 -- default value so that we know when it has been set/changed



--
-- get some details of the event we are processing
--
select @AttackingVillageID = UM.OriginVillageID
	, @TargetVillageID = UM.DestinationVillageID
from UnitMovements UM
where UM.EventID = @EventID
AND CommandType in (1) -- MAKE sure this is an attack.

IF @AttackingVillageID  is null BEGIN
	IF @DEBUG = 1 SELECT '@AttackingVillageID  is null. Exit quietly' 
	RETURN 
END 

--
-- get the realm battle handicap 
--
declare @BattleHandicapRealmAttrib int
SELECT @BattleHandicapRealmAttrib = AttribValue from RealmAttributes Where AttribID = 5
IF @BattleHandicapRealmAttrib > 0 BEGIN
    SELECT @ThisBattleHandicap_StartRatio = AttribValue from RealmAttributes Where AttribID = 28    
    SELECT @ThisBattleHandicap_MaxHandicap = AttribValue from RealmAttributes Where AttribID = 29    
    SELECT @ThisBattleHandicap_Steepness = AttribValue from RealmAttributes Where AttribID = 30 
--	SELECT @BattleAlgorithmVersion = AttribValue FROM RealmAttributes WHERE attribID = 61
--	SET @BattleAlgorithmVersion = isnull(@BattleAlgorithmVersion , 1)
END ELSE BEGIN
	
    
    SET @ThisBattleHandicap_StartRatio = isnull(@ThisBattleHandicap_StartRatio, 4)
    SET @ThisBattleHandicap_MaxHandicap = isnull(@ThisBattleHandicap_MaxHandicap, 0.5)
    SET @ThisBattleHandicap_Steepness = isnull(@ThisBattleHandicap_Steepness, 1) 
END


IF @DEBUG = 1 SELECT @AttackingVillageID as '@AttackingVillageID'
			, @TargetVillageID as '@TargetVillageID'
			, @BattleHandicapRealmAttrib as '@BattleHandicapRealmAttrib'
			, @ThisBattleHandicap_StartRatio as '@ThisBattleHandicap_StartRatio'
			, @ThisBattleHandicap_MaxHandicap as '@ThisBattleHandicap_MaxHandicap'
			, @ThisBattleHandicap_Steepness as '@ThisBattleHandicap_Steepness';


__RETRY_BATTLE__:
BEGIN TRY 
	begin transaction BATTLE 
	--
	-- Obtain locks on effected villages
	-- We lock:
	--	>target & attacking village
	--  >all villages that are supporting the target village 
	-- This way we ensure that unit counts on any of the effected villages will not change, and that no one pulls support
	--	from target village in the middle of the attack
	CREATE TABLE #VillagesToLock (VillageID INT NOT NULL primary key  WITH (IGNORE_DUP_KEY = ON));
	insert into #VillagesToLock values (@AttackingVillageID)
	insert into #VillagesToLock values (@TargetVillageID)
	insert into #VillagesToLock select VSU.SupportingVillageID 
			from VillageSupportUnits VSU
			where SupportedVillageID = @TargetVillageID		
				and VSU.UnitCount <> 0

	update VillageSemaphore set TimeStamp = getdate() 
		where VillageID in (select VillageID from #VillagesToLock)
	
	--
	-- Secondly we set the event as completed so that no one, in the mean time, can cancel it
	--
	UPDATE Events SET [Status] =1
		FROM Events E
		join UnitMovements UM
			on UM.EventID = E.EventID
		WHERE E.EventID = @EventID
			AND [Status] = 0		
			AND CommandType in (1) -- MAKE sure this is an attack.
	IF @@rowcount <> 1 BEGIN
		-- IF no rows where updated, then the event must have been cancelled (or something like this) thereforecs we abort quietly
		IF @DEBUG = 1 SELECT 'Event is no longer valid. Exit quietly' 
		ROLLBACK TRAN  BATTLE
		RETURN
	END 
	--
	-- ensure VillageUnits table is up-to-date
	--	we only care about the TargetVillage
	--
	exec iCompleteUnitRecruitment @TargetVillageID	

	--
	-- Update village coins in target village to make sure we know what we can loot. 
	--	We do this here to make sure we get the coins BEFOER the treasury if (possibly) downgraded with treb
	exec @VillageCoins = UpdateVillageCoins @TargetVillageID

	--
	-- Can attacker's and defener's extended info
	--
	select @TargetPlayerID = P.PlayerID 
		,  @TargetVillageName = V.Name
		, @TargetPlayerName = P.Name
		, @TargetVillageXCord = V.XCord
		, @TargetVillageYCord = V.YCord
		, @TargetSpecialPlayerTypeID = SP.Type
		, @TargetPlayersPoints = P.Points
		, @TargetVillageTypeID = VillageTypeID
		from Players P
		join Villages V
			on P.PlayerID = V.OwnerPlayerID
		left join SpecialPlayers SP
			on SP.PlayerID = P.PlayerID
		where V.VillageID = @TargetVillageID
		

	select @AttackingPlayerName = P.Name 
		, @AttackingPlayerID = P.PlayerID 
		, @AttackingPlayerPoints = P.Points
		, @AttackingVillageXCord = V.XCord
		, @AttackingVillageYCord = V.YCord		
		, @AttackingVillageName = V.Name
		from Players P
		join Villages V
			on P.PlayerID = V.OwnerPlayerID
		where V.VillageID = @AttackingVillageID

	IF @DEBUG = 1 SELECT @TargetPlayerID as '@TargetPlayerID'
			, @TargetVillageName  as '@TargetVillageName' 
			, @AttackingPlayerName  as '@AttackingPlayerName'
			, @AttackingPlayerID as '@AttackingPlayerID'
			, @TargetVillageID as '@TargetVillageID'
			, @TargetPlayersPoints as '@TargetPlayersPoints'
			, @AttackingPlayerPoints as '@AttackingPlayerPoints'
			, @AttackingVillageXCord as '@AttackingVillageXCord'
			, @AttackingVillageYCord as '@AttackingVillageYCord'
	
	insert into #ChangedVillages values(@AttackingPlayerID, @AttackingVillageID)
	insert into #ChangedVillages values(@TargetPlayerID, @TargetVillageID)

	SELECT @RebelPlayerID = PlayerID FROM Players where userid = '00000000-0000-0000-0000-000000000002'
	SELECT @AbandonedPlayerID = PlayerID FROM Players where userid = '00000000-0000-0000-0000-000000000000'

	--
	-- Calculate the battle handicap if any
	--
	SET @ThisBattleHandicap = 0 -- default
	IF @BattleHandicapRealmAttrib > 0 BEGIN
		
				
		IF @TargetPlayerID = @RebelPlayerID OR @TargetPlayerID = @AbandonedPlayerID BEGIN
			SET @TargetPlayersPoints = 100000000 -- hack. no handicap on rebels and abandoned so setting to 100 million points
		END

		SET @PointsRatio = @AttackingPlayerPoints / (cast (@TargetPlayersPoints as real))
        IF @PointsRatio <= @ThisBattleHandicap_StartRatio BEGIN
            SET @ThisBattleHandicap = 0
		END ELSE BEGIN
            declare @logValB float 
            SET @logValB = 2 * log10(@PointsRatio - @ThisBattleHandicap_StartRatio + 1);

            SET @ThisBattleHandicap = @ThisBattleHandicap_MaxHandicap - @ThisBattleHandicap_MaxHandicap * power(cast(@ThisBattleHandicap_StartRatio as float), -0.25 * @ThisBattleHandicap_Steepness *(@logValB * @logValB))
        END 
		
		IF @DEBUG = 1 SELECT @ThisBattleHandicap as '@ThisBattleHandicap'
			, @PointsRatio  as '@PointsRatio' 		
	END 	
	SET @BattleHandicapAttackerStrengthMultilier = 1 - @ThisBattleHandicap
	IF @DEBUG = 1 SELECT @ThisBattleHandicap as '@ThisBattleHandicap'
			, @RebelPlayerID  as '@RebelPlayerID' 
			, @AbandonedPlayerID  as '@AbandonedPlayerID'
			, @AttackingPlayerID as '@AttackingPlayerID'
			, @logValB as '@logValB'
			, @PointsRatio as '@PointsRatio'
	
	--
    -- Get the attack bonus, if any, from Premium Feature
    --
	SET @AttackStrengthBonusMultipier = 1 -- default; no bonus since attackStrenth*1=attackStrenth
	IF EXISTS(SELECT * FROM UnitMovements_Attributes WHERE EventID = @EventID and AttribID = 26) BEGIN
	    SET @AttackStrengthBonusMultipier = 1.2	--20% bonus
	END
	--
    -- Get the defence bonus, if any, from Premium Feature
    --
	SET @TargetStrengthBonusMultipier = 1 -- default; no bonus since targetStrenth * 1 = targetStrenth
	IF EXISTS
	    (select * 
            from PFs
            join PFsInPackage on
	            PFsInPackage.FeatureID=PFs.FeatureID
            join PFPackages on
	            PFPackages.PFPackageID=PFsInPackage.PFPackageID
            join PlayersPFPackages on
	            PlayersPFPackages.PFPackageID=PFsInPackage.PFPackageID
            where 
		            PlayerID=@TargetPlayerID 
	            AND PFs.FeatureID = 25 -- DEFENCE BONUS
	            AND ExpiresOn>getdate()	
	    )
	BEGIN
	    SET @TargetStrengthBonusMultipier = 1.2	--20% bonus
	END
	--
	-- apply bonus from bonus village if any
	--
	-- obtain village type bonus if any.  will be in a format like 0.1 meaning 10%
    select @villageTypePercentBonus = sum(cast(PropertyValue as float)) 	       
        FROM VillageTypeProperties VTP 
        join VillageTypePropertyTypes VTPT on VTP.VillageTypePropertyTypeID = VTPT.VillageTypePropertyTypeID
        where 
            VTP.VillageTypeID = @TargetVillageTypeID
            and VTPT.PropertyID = 9 /*DefenseFactor*/
            and type = 3		    
    SET @villageTypePercentBonus = isnull(@villageTypePercentBonus,0)
    IF @DEBUG = 1 select @villageTypePercentBonus as '@villageTypePercentBonus'
    --
    -- update the target strength multiplyer with the village bonus
    SET @TargetStrengthBonusMultipier = @TargetStrengthBonusMultipier * (@villageTypePercentBonus + 1)
    IF @DEBUG = 1 select @TargetStrengthBonusMultipier as '@TargetStrengthBonusMultipier'
	--
	-- Get attack strength
	--
	--	1a.	For Each type of attack unit, multiply the number of those units, by its attack strength.  
	--	and take the sum of all units.
	--
	select @AttackStrength= sum(UMing.UnitCount * UT.AttackStrength) * @BattleHandicapAttackerStrengthMultilier * @AttackStrengthBonusMultipier
		, @TotalNumberOfAttackingUnitsByPop = sum( UMing.UnitCount * UT.Population)
	from UnitsMoving UMing
	join UnitTypes UT
		on UT.UnitTypeID = UMing.UnitTypeID
		and UT.AttackStrength > 0 -- the attacking units with strength 0 are ignored
	where UMing.EventID = @EventID
	group by EventID
	SET @AttackStrength = isnull(@AttackStrength,0) -- this kicks in if there are no troops attacking with strength > 0

	--
	-- apply attacker's offensive research bonus if any
	select @AttackersOffensiveResearchPercentBonus = sum(cast(PropertyValue as float))
        from ResearchItemPropertyTypes PT 
        join ResearchItemProperties P on P.ResearchItemPropertyID = PT.ResearchItemPropertyID
        join ResearchItems RI on RI.ResearchItemTypeID = P.ResearchItemTypeID AND RI.ResearchItemID = P.ResearchItemID
        join PlayerResearchItems PRI ON RI.ResearchItemTypeID = PRI.ResearchItemTypeID AND RI.ResearchItemID = PRI.ResearchItemID
        where PT.ResearchItemPropertyID = 13 /*Offensive factor*/
            AND PlayerID = @AttackingPlayerID
           			            
    SET @AttackersOffensiveResearchPercentBonus = isnull(@AttackersOffensiveResearchPercentBonus,0) + 1
	SET @AttackStrength = @AttackStrength * @AttackersOffensiveResearchPercentBonus

	--
	-- get morale adjustment
	--
	IF @morale_isactive=1 BEGIN
		SELECT @AttackMorale = AttribValue FROM UnitMovements_Attributes WHERE EventID = @EventID and AttribID = 1
		select @AttackStrengthMoraleMultiplier  = AttackAdj from PlayerMoraleEffects where Morale = @AttackMorale
		SET @AttackStrengthMoraleMultiplier  = isnull(@AttackStrengthMoraleMultiplier, 1); -- default; no bonus since attackStrenth*1=attackStrenth
		SET @AttackStrength = @AttackStrength * @AttackStrengthMoraleMultiplier
	END
	--
	-- get the spy attack strength
	--
	select  @AttackSpyStrength = sum(UMing.UnitCount * UT.SpyAbility) * @BattleHandicapAttackerStrengthMultilier * @AttackStrengthBonusMultipier
	from UnitsMoving UMing
	join UnitTypes UT
		on UT.UnitTypeID = UMing.UnitTypeID
	where UMing.EventID = @EventID
	group by EventID

	--
	-- attacker's deployed units. Used for reports and for stats tracking
	--
	insert into #AttackerUnits select UnitTypeID, UnitCount, 0 from UnitsMoving UMing where UMing.EventID = @EventID and UnitCount > 0 -- if for some reason we got a record with 0 troop count, then ignore those
	IF @DEBUG = 1 select '#AttackerUnits', * from #AttackerUnits

	--
	-- Get targets's supporting units; this is used for report and for stats tracking
	--
	INSERT INTO #TargetSupportingUnits 
		select VSU.SupportingVillageID, V.OwnerPlayerID, UT.UnitTypeID, VSU.UnitCount, 0
		from VillageSupportUnits VSU
		join UnitTypes UT
			on VSU.UnitTypeID = UT.UnitTypeID
		join Villages V
			on V.VillageID = VSU.SupportingVillageID			
		where SupportedVillageID = @TargetVillageID		
			and VSU.UnitCount <> 0

	--
	-- Get targets's deployed units; this is used for report but also for battle calculation & stats tracking
	--
	insert into #TargetUnits 
		-- target's own troops
		select UT.UnitTypeID , VU.CurrentCount, 0
		from VillageUnits VU
		join UnitTypes UT
			on VU.UnitTypeID = UT.UnitTypeID
		where VillageID = @TargetVillageID
		
		union all
		
		-- target's supporting troops
		select UnitTypeID, DeployedUnitCount, 0
		from #TargetSupportingUnits
					
	
	
					
	--
	--	GET DEFENSE BONUS, this is a PRE ATTACK NUMBER!!  
	--		
	SELECT @BuildingDefenseBonus_PreAttack  = sum( Convert(Real,LP.PropertyValue) - 100)
	FROM Buildings B
		JOIN LevelProperties LP
			ON LP.BuildingTypeID = B.BuildingTypeID
			AND LP.Level = B.Level
			AND LP.PropertyID = 9 /*DefenseFactor*/
		WHERE B.VillageID = @TargetVillageID
	SET @BuildingDefenseBonus_PreAttack = isnull(@BuildingDefenseBonus_PreAttack,0)
    IF @DEBUG = 1 select  @BuildingDefenseBonus_PreAttack as '@BuildingDefenseBonus_PreAttack'	

    --
    -- get the defense bonus from research, applicable to walls & towers. this will be a number like 0.1 meaning 10% 
    --
    select @researchPercentBonus = sum(cast(PropertyValue as float))
        from ResearchItemPropertyTypes PT 
        join ResearchItemProperties P on P.ResearchItemPropertyID = PT.ResearchItemPropertyID
        join ResearchItems RI on RI.ResearchItemTypeID = P.ResearchItemTypeID AND RI.ResearchItemID = P.ResearchItemID
        join PlayerResearchItems PRI ON RI.ResearchItemTypeID = PRI.ResearchItemTypeID AND RI.ResearchItemID = PRI.ResearchItemID
        where PT.PropertyID = 9 /*DefenseFactor*/
            AND PlayerID = @TargetPlayerID
            AND PT.Type = 3 			            
    SET @researchPercentBonus = isnull(@researchPercentBonus,0) + 1
    --
    -- get the defense bonus from research, village defense bonus. this will be a number like 0.1 meaning 10% 
    --
    select @researchPercentBonus_villageDef = sum(cast(PropertyValue as float))
        from ResearchItemPropertyTypes PT 
        join ResearchItemProperties P on P.ResearchItemPropertyID = PT.ResearchItemPropertyID
        join ResearchItems RI on RI.ResearchItemTypeID = P.ResearchItemTypeID AND RI.ResearchItemID = P.ResearchItemID
        join PlayerResearchItems PRI ON RI.ResearchItemTypeID = PRI.ResearchItemTypeID AND RI.ResearchItemID = PRI.ResearchItemID
        where PT.ResearchItemPropertyID = 12 /*Village DefenseFactor*/
            AND PlayerID = @TargetPlayerID
    SET @researchPercentBonus_villageDef = isnull(@researchPercentBonus_villageDef,0) * 100

    IF @DEBUG = 1 select @researchPercentBonus as '@researchPercentBonus'
		, @AttackersOffensiveResearchPercentBonus as '@AttackersOffensiveResearchPercentBonus'
        , @BuildingDefenseBonus_PreAttack as '@BuildingDefenseBonus_PreAttack'	
		, @researchPercentBonus_villageDef as '@researchPercentBonus_villageDef'	            

    --
    -- calc the defense bonus with research and village type bonus
    --
	SET @BuildingDefenseBonus_PreAttack = ( @BuildingDefenseBonus_PreAttack * @researchPercentBonus ) + @researchPercentBonus_villageDef  

    IF @DEBUG = 1 select @BuildingDefenseBonus_PreAttack as '@BuildingDefenseBonus_PreAttack'		            
	--
	--	GET DEFENSE STRENGTH, no building defense bonus 
	--	
	--2)	Compute Defense Points (without building defense bonus %)
	--		a.	For each defense unit
	--			i.	Multiply the defence value of the unit N vs a particular attack unit by the percentage of those units, and sum up.  
	--				Multiply that sum by the number of that defensive unit. Cell F2 – F9
	--
	--		Note. 'percentage of those units' is a percentage relative to population. so 12 infantry, and 1 knight is 50% infantry and 50% knight since knight takes 12 population
	--
	INSERT INTO #TargetUnitDefenseStrength 
		SELECT T.UnitTypeID
			, sum( (convert(Real, isnull(A.DeployedUnitCount,0)) *UT.Population / Convert(Real, @TotalNumberOfAttackingUnitsByPop) /*percentage of attack unit out of total # of attack units*/ )
					* D.DefenseStrength 
				) 
				* T.DeployedUnitCount
			FROM UnitTypeDefense D
			JOIN #TargetUnits T
				on D.UnitTypeID = T.UnitTypeID
			LEFT JOIN #AttackerUnits A
				on  D.DefendAgainstUnitTypeID = A.UnitTypeID
			JOIN UnitTypes UT
				on A.UnitTypeID = UT.UnitTypeID
				where UT.AttackStrength > 0 -- the attacking units with strength 0 are ignored				
			group by T.UnitTypeID, T.DeployedUnitCount		


	--
	--		ii.	Sum up the values from (i) 
	--
	SELECT  @TargetStrength = sum(DefenseStrength) * @TargetStrengthBonusMultipier
		FROM #TargetUnitDefenseStrength 
	
	IF @TargetStrength is null BEGIN -- this kicks in if there are no troops in the village. 
		set @TargetStrength = 0
	END 


	--
	-- ANY BUILDINGS UNDER ATTACK? COMPUTE attack strength here
	-- 
	IF @TargetStrength <> 0 BEGIN 
		SET @AttackToTargetStrenghRatio_NoBuildingBonus  = (@AttackStrength / @TargetStrength)
		IF @AttackToTargetStrenghRatio_NoBuildingBonus  > 1 BEGIN SET @AttackToTargetStrenghRatio_NoBuildingBonus  = 1 END
	END ELSE IF @AttackStrength = 0 BEGIN
		set @AttackToTargetStrenghRatio_NoBuildingBonus  = 0
	END ELSE BEGIN
		set @AttackToTargetStrenghRatio_NoBuildingBonus  = 1
	END
    IF @DEBUG = 1 select @AttackToTargetStrenghRatio_NoBuildingBonus as '@AttackToTargetStrenghRatio_NoBuildingBonus'		            


	INSERT INTO #BuildingAttacks (BuildingTypeID, AttackStrength, CurrentLevelInVillage, LevelAfterAttack)
		SELECT 
				TargetBuildingTypeID
				, sum(AttackStrength * UnitCount) * @AttackToTargetStrenghRatio_NoBuildingBonus
				, Level
				, BT.MinimumLevelAllowed
			FROM UnitsMoving UM
			JOIN UnitOnBuildingAttack BA
				ON UM.UnitTypeID = BA.UnitTypeID
				AND UM.TargetBuildingTypeID = BA.BuildingTypeID
			JOIN Buildings B
				ON B.VillageID = @TargetVillageID
				AND B.BuildingTypeID = UM.TargetBuildingTypeID
			JOIN BuildingTypes BT
				on BT.BuildingTypeID = B.BuildingTypeID
			WHERE UM.EventID = @EventID
			GROUP BY TargetBuildingTypeID, B.Level, BT.MinimumLevelAllowed

	IF @DEBUG = 1 select '#BuildingAttacks', * from #BuildingAttacks

	--
	-- Figure out what level to drop the buildings to
	-- 
	IF EXISTS(SELECT * FROM #BuildingAttacks) BEGIN
		-- compute the current level points - attack strength, ie building points remaining
		UPDATE #BuildingAttacks 
		Set PointsAfterAttack = BL.CumulativeLevelStrength - A.AttackStrength
			FROM #BuildingAttacks A
			JOIN BuildingLevels BL
				ON BL.BuildingTypeID = A.BuildingTypeID
				AND BL.Level = A.CurrentLevelInVillage

		-- Find the level that this drop will take us to
		--	we cannot go lower then level 1 on any building which is accomplished but the fact that the LevelAfterAttack has a value of 1 by default. 
		--		we do this by getting the level such that the points after attack + the level strength is higher then the cummulative level strenght
		--		and the next level 
		
		UPDATE #BuildingAttacks
		Set LevelAfterAttack = BL.Level
			FROM #BuildingAttacks A
			JOIN BuildingLevels BL
				ON BL.BuildingTypeID = A.BuildingTypeID
				AND A.PointsAfterAttack + LevelStrength > CumulativeLevelStrength
				AND 
					(	-- the next level higher then BL.Level is too high (in points)
						EXISTS (
						SELECT * FROM BuildingLevels BL2 
							WHERE BL2.BuildingTypeID = BL.BuildingTypeID 
							AND BL2.Level = BL.Level + 1 
							AND A.PointsAfterAttack + BL2.LevelStrength <= CumulativeLevelStrength
						)
						
						OR
							-- unless the BL.level is the last level for this building
							BL.Level = ( SELECT max([level]) FROM BuildingLevels WHERE BuildingTypeID =  BL.BuildingTypeID ) 
						
					)  

		--
		-- Update the village building to the new levels; IF that level is not 0
		UPDATE Buildings 
			SET Level = A.LevelAfterAttack
			FROM Buildings B
			JOIN #BuildingAttacks A
				ON A.BuildingTypeID = B.BuildingTypeID 
			WHERE B.VillageID = @TargetVillageID
			and A.LevelAfterAttack <> 0 
		--
		-- Update the village building to the new level IF that level is 0 meaning this building has to be removed from the village
		delete Buildings
			WHERE VillageID = @TargetVillageID
			and BuildingTypeID in 
				(SELECT BuildingTypeID FROM #BuildingAttacks
					WHERE levelAfterAttack = 0
				)
			 
		--
		-- note that buildings were attacked so better update the village points
		--
		set @UpdateVillagePoints = 1
		
		IF @DEBUG = 1 select '#BuildingAttacks',  * from #BuildingAttacks
	END

	--
	--	GET DEFENSE STRENGTH, WITH building defense bonus;  
	--
	
	-- get the post attack bonus	
	SELECT @BuildingDefenseBonus_PostAttack  = sum( Convert(Real,LP.PropertyValue) - 100)
	FROM Buildings B
		JOIN LevelProperties LP
			ON LP.BuildingTypeID = B.BuildingTypeID
			AND LP.Level = B.Level
			AND LP.PropertyID = 9 /*DefenseFactor*/
		WHERE B.VillageID = @TargetVillageID;


	SET @BuildingDefenseBonus_PostAttack = (isnull(@BuildingDefenseBonus_PostAttack,0) * @researchPercentBonus ) + @researchPercentBonus_villageDef
	--
	-- Compute the difference between pre attack and post attack bonus, then the actual delta, then the actual bonus	
	set @BuildingDefenseBonus_PreAttack = isnull(@BuildingDefenseBonus_PreAttack,0)
	set @BuildingDefenseBonus_PostAttack = isnull(@BuildingDefenseBonus_PostAttack,0)
		
	set @BuildingDefenseBonus_Delta = @BuildingDefenseBonus_PreAttack - @BuildingDefenseBonus_PostAttack
	set @BuildingDefenseBonus_EffectiveDelta = 0.6 * sin(0.5 * pi() * ( @BuildingDefenseBonus_Delta/100))

	set @TargetStrengthWithBuildingBonus = @TargetStrength * (1 + (@BuildingDefenseBonus_PreAttack/100 - @BuildingDefenseBonus_EffectiveDelta) )

	IF @DEBUG = 1 SELECT @BuildingDefenseBonus_PreAttack as '@BuildingDefenseBonus_PreAttack'
					, @BuildingDefenseBonus_PostAttack as '@BuildingDefenseBonus_PostAttack'
					, @BuildingDefenseBonus_Delta as '@BuildingDefenseBonus_Delta'
					, @BuildingDefenseBonus_EffectiveDelta as '@BuildingDefenseBonus_EffectiveDelta'
					, @TargetStrength  as '@TargetStrength' 
					, @AttackStrength  as '@AttackStrength'
					, @AttackersOffensiveResearchPercentBonus  as '@AttackersOffensiveResearchPercentBonus'
					, @TargetStrengthWithBuildingBonus as '@TargetStrengthWithBuildingBonus'
					, @AttackToTargetStrenghRatio_NoBuildingBonus as '@AttackToTargetStrenghRatio_NoBuildingBonus'
					, @AttackSpyStrength as '@AttackSpyStrength'
					, @AttackStrengthMoraleMultiplier as '@AttackStrengthMoraleMultiplier'	            
	
	--
	-- BATTLE! - get the death factor for each side. 
	-- 
	IF @AttackStrength > @TargetStrengthWithBuildingBonus BEGIN
		set @AttackKillFactor = power( cast(@TargetStrengthWithBuildingBonus as real) / cast(@AttackStrength as real) , 1.5)
		set @TargetKillFactor = 1		
	END ELSE IF @TargetStrengthWithBuildingBonus > @AttackStrength BEGIN
		set @AttackKillFactor = 1
		set @TargetKillFactor = power( cast(@AttackStrength as real) / cast(@TargetStrengthWithBuildingBonus as real) , 1.5)
	END ELSE IF @TargetStrengthWithBuildingBonus = 0 AND  @AttackStrength = 0 BEGIN
		set @AttackKillFactor = 1
		set @TargetKillFactor = 0
	END ELSE BEGIN
		set @AttackKillFactor = 1
		set @TargetKillFactor = 1
	END
	IF @DEBUG = 1 SELECT @AttackKillFactor as '@AttackKillFactor', @TargetKillFactor as '@TargetKillFactor'
	IF @DEBUG = 1 select 'UnitsMoving', * from UnitsMoving where EventID = @EventID
	IF @DEBUG = 1 select 'VillageUnits', * from VillageUnits where VillageID = @AttackingVillageID
	IF @DEBUG = 1 select 'VillageUnits', * from VillageUnits where VillageID = @TargetVillageID
	IF @DEBUG = 1 select 'VillageSupportUnits', * from VillageSupportUnits where SupportedVillageID = @TargetVillageID
	IF @DEBUG = 1 select 'VillageUnits', * from VillageUnits where VillageID in (select SupportingVillageID from VillageSupportUnits where SupportedVillageID = @TargetVillageID)

	--
	-- Update the attacking units to eliminate the dead.
	--	- update the units moving
	--	- update the units in the village of origins
	--	These update HAVE to be done in this order. 
	--			** SPIES are handled later

	-- Village's total count = TotalCOunt - attacking units killed
	update VillageUnits set  TotalCount =TotalCount -  round( UMing.UnitCount * @AttackKillFactor,0)
	from UnitsMoving UMing 
	join VillageUnits VU
		on VU.UnitTypeID = UMing.UnitTypeID 
		and VU.VillageID = @AttackingVillageID
	where UMing.EventID = @EventID
		and VillageID = @AttackingVillageID
		and UMing.UnitTypeID <> @UNIT_SPY_ID
	
	
	update UnitsMoving set UnitCount = UnitCount - ROUND( UnitCount * @AttackKillFactor,0)
		where EventID = @EventID
		and UnitTypeID <> @UNIT_SPY_ID

	--
	-- Update the defending units to eliminate the dead. 
	--	- update the village units
	--	- update the supporting units
	--	- update the supporting village units 
	--	These update HAVE to be done in this order. 
	--
	-- "fbGetMax-HACK" 
	--		we always had a possiblity that CurrentCount was inconsistent with TotalAcount in village units.
	--			since we've released raids in fall 2017, we've had more accounts of this; abandoned villages had inconsistent unit counts
	--		This results in attacks on those abandoned to fail and attack disapears and player is missing troops. 
	--		We did not have time to investigate why this is happeniung, however, putting the fbGetMax on some of the unpdates below, 
	--			solved the problem, at least of trying to update TotalCount and getting value less than 0 which triggered a contraint, 
	--			hence error, hence attack fail, hence missing troops
	--
	update VillageUnits set CurrentCount = CurrentCount -  ROUND( CurrentCount * @TargetKillFactor,0)
		, TotalCount = dbo.fbGetMax(TotalCount - ROUND( CurrentCount * @TargetKillFactor,0),0) -- see "fbGetMax-HACK" explanation above
		where VillageID = @TargetVillageID

	update VillageUnits set TotalCount = dbo.fbGetMax(TotalCount -  ROUND( VSU.UnitCount * @TargetKillFactor,0),0) -- see "fbGetMax-HACK" explanation above
	from VillageSupportUnits VSU 
		where  VSU.SupportedVillageID = @TargetVillageID
		and VillageUnits.UnitTypeID = VSU.UnitTypeID
		and VillageID = VSU.SupportingVillageID

	update VillageSupportUnits set UnitCount = UnitCount - ROUND( UnitCount * @TargetKillFactor,0)		
		where SupportedVillageID = @TargetVillageID


	IF @DEBUG = 1 select 'UnitsMoving', * from UnitsMoving where EventID = @EventID
	IF @DEBUG = 1 select 'VillageUnits', * from VillageUnits where VillageID = @AttackingVillageID
	IF @DEBUG = 1 select 'VillageUnits', * from VillageUnits where VillageID = @TargetVillageID
	IF @DEBUG = 1 select 'VillageSupportUnits', * from VillageSupportUnits where SupportedVillageID = @TargetVillageID
	IF @DEBUG = 1 select 'VillageUnits', * from VillageUnits where VillageID in (select SupportingVillageID from VillageSupportUnits where SupportedVillageID = @TargetVillageID)	
	
	--	
	-- get units killed for the report; also used for spy calculations 
	--
	update #TargetUnits set KilledUnitCount = ROUND( DeployedUnitCount *  @TargetKillFactor,0) 
	update #AttackerUnits set KilledUnitCount = ROUND(DeployedUnitCount *  @AttackKillFactor,0) WHERE UnitTypeID <> @UNIT_SPY_ID
	update #TargetSupportingUnits set KilledUnitCount = ROUND(DeployedUnitCount *  @TargetKillFactor,0) 
	IF @DEBUG = 1 select '#TargetUnits', * from #TargetUnits 
	IF @DEBUG = 1 select '#AttackerUnits ', * from #AttackerUnits 
	IF @DEBUG = 1 select '#TargetSupportingUnits', * from #TargetSupportingUnits 

	-- remember any of the supporting villages that have troops killed so that we can trigger the change village events on those
	insert into #ChangedVillages select distinct SupportingVillageOwnerPlayerID, SupportingVillageID from #TargetSupportingUnits where KilledUnitCount > 0 

	--
	-- Did any attacking, none spy units survived?
	IF EXISTS (SELECT * FROM #AttackerUnits WHERE UnitTypeID <> @UNIT_SPY_ID AND DeployedUnitCount > KilledUnitCount) BEGIN
		-- IF so, then it means that defender's troops are automatically visible
		SET @DefendersTroopsVisibleToAttacker = 1
		SET @HasAttackersNonSpyUnitsSurvived = 1
	END
	
	--
	-- ------------------------------------------
	-- Now handle SPIES
	--
	
	-- ONLY spies ware attacking ?
	IF exists (select  * from #AttackerUnits where UnitTypeID <> @UNIT_SPY_ID AND DeployedUnitCount > 0)  BEGIN
		SET @DefenderKnowsAttackersIdentify = 1 -- since not only spies are attacking
	END ELSE BEGIN 
		SET @isSpyOnlyAttack = 1
	END
	
	IF @AttackSpyStrength > 0 BEGIN 
		-- get the target's counter spy ability strength
		select @TargetCounterSpyStrength = sum( (TU.DeployedUnitCount - TU.KilledUnitCount) * UT.CounterSpyAbility) 
			from #TargetUnits TU 
			JOIN UnitTypes UT 
			on TU.UnitTypeID = UT.UnitTypeID
			
		SET @TargetCounterSpyStrength = @TargetCounterSpyStrength + 20 -- a village has a base counter spyability of 20			
		
		EXEC qCalculateSpyStuff @AttackSpyStrength 
			,@TargetCounterSpyStrength
			,@SpySuccessChance output  
			,@SpySuccessChance_AlgA output 
			,@SpySuccessChance_AlgB output 
			,@spyIdentityKnownChance output 
			,@spyAttackVisibleChance output 
		
		SET @SpyFailureChance = 1 - @SpySuccessChance
		
		-- if defender counter spy >20, ie, if defender had at least 1 spy defending, then some spies could have died. 
		--  if no defending spies present, just base counterspyability, then no spies die. 
		IF @TargetCounterSpyStrength > 20 BEGIN 
		    SET @AttackerSpiesKilledPercentage = 1 - @SpySuccessChance_AlgA
		END ELSE BEGIN 
		    SET @AttackerSpiesKilledPercentage = 0		
		END
		
		IF RAND() <=  @SpySuccessChance BEGIN
			SET @SpySuccessful = 1
			SET @SpyOutcome = 1
			SET @DefendersTroopsVisibleToAttacker = 1
		END ELSE BEGIN
			SET @SpySuccessful = 0
			SET @SpyOutcome = 0
		END


		IF @DefenderKnowsAttackersIdentify <> 1 BEGIN
			IF RAND() <= @spyIdentityKnownChance BEGIN
				SET @DefenderKnowsAttackersIdentify = 1
			END ELSE BEGIN
				SET @DefenderKnowsAttackersIdentify = 0
			END
		END

		IF @DEBUG = 1 SELECT @TargetCounterSpyStrength as '@TargetCounterSpyStrength'
						, @AttackSpyStrength as '@AttackSpyStrength'
						, @SpySuccessChance as '@SpySuccessChance'
						, @SpyFailureChance as '@SpyFailureChance'
						, @SpySuccessful as '@SpySuccessful'
						, @DefenderKnowsAttackersIdentify as '@DefenderKnowsAttackersIdentify'
						, @SpyOutcome as '@SpyOutcome'
						, @DefendersTroopsVisibleToAttacker AS '@DefendersTroopsVisibleToAttacker'
						, @SpySuccessChance_AlgA AS '@SpySuccessChance_AlgA'
						, @SpySuccessChance_AlgB AS '@SpySuccessChance_AlgB'
						, @spyIdentityKnownChance AS '@spyIdentityKnownChance'
						, @AttackerSpiesKilledPercentage AS '@AttackerSpiesKilledPercentage'
		--				
		-- Now kill off some attacking spies if any 
		--
		-- Update the attacking SPIES to eliminate the dead.
		--	- update the units moving
		--	- update the units in the village of origins
		--	These update HAVE to be done in this order. 

		-- Village's total count = TotalCOunt - attacking units killed
		update VillageUnits set  TotalCount = TotalCount -  round( UMing.UnitCount * @AttackerSpiesKilledPercentage , 0)
		from UnitsMoving UMing 
		join VillageUnits VU
			on VU.UnitTypeID = UMing.UnitTypeID
			and VU.VillageID = @AttackingVillageID
		where UMing.EventID = @EventID
			and VillageID = @AttackingVillageID
			and UMing.UnitTypeID = @UNIT_SPY_ID
		
		
		update UnitsMoving set UnitCount = UnitCount - ROUND( UnitCount * @AttackerSpiesKilledPercentage,0)
			where EventID = @EventID
			and UnitTypeID = @UNIT_SPY_ID	
			
		update #AttackerUnits set KilledUnitCount = ROUND(DeployedUnitCount *  @AttackerSpiesKilledPercentage,0) WHERE UnitTypeID = @UNIT_SPY_ID
			
			
		IF @DEBUG = 1 select * from UnitsMoving where EventID = @EventID
		IF @DEBUG = 1 select * from VillageUnits where VillageID = @AttackingVillageID
				
		--
		-- Handle a special case where all attacking spies were killed. then spies are definatelly not successful.
		--
		IF not exists (select  * from #AttackerUnits where UnitTypeID = @UNIT_SPY_ID AND DeployedUnitCount-KilledUnitCount > 0) BEGIN
			SET @SpySuccessful = 0
			SET @SpyOutcome = 0
			
			IF @HasAttackersNonSpyUnitsSurvived = 0 BEGIN
				--
				-- when there are no non-spy units surviving, and no spy units surviving, 
				--	then we know defender's troops should not be visible. However, if some of attacker's non-spy units survived, 
				--	then defenders units are definatelly visible (although i don't think this can actually happen)
				SET @DefendersTroopsVisibleToAttacker = 0
			END 
			IF @DEBUG = 1 SELECT @SpySuccessful as '@SpySuccessful'
							, @SpyOutcome as '@SpyOutcome'
							, @DefendersTroopsVisibleToAttacker AS '@DefendersTroopsVisibleToAttacker'			
		END
	END
	
	--
	-- ------------------------------------------
	-- Update loyalty if lord present & survived the attack.  
	--
	IF EXISTS(select * from UnitsMoving where EventID = @EventID and UnitTypeID = @LORD_UNIT_TYPE_ID and UnitCount >0)
	BEGIN	
	    --
	    -- On realms with capital villages active, do not allow take over of player's capital village
	    --
        IF EXISTS (SELECT * FROM RealmAttributes WHERE attribID = 27 and AttribValue = '1') 
            AND EXISTS (SELECT * FROM CapitalVillages WHERE  VillageID= @TargetVillageID) BEGIN
            SET @GovHasNoEffect = 1
        END ELSE BEGIN
	        --
	        -- typical case - gov has effect
	        --
		    select @LoyaltyBeforeAttack = dbo.fbGetMin(loyalty + floor(cast(datediff(minute, LoyaltyLastUpdated, @BattleTime) as real) /(60.0 / (select cast(AttribValue as real) FROM RealmAttributes where attribid =8))), 100)
			    from Villages 
			    where VillageID = @TargetVillageID				

		    set @LoyaltyChange = @LOYALTY_DEC_BASE + cast(rand()* @LOYALTY_DEC_VAR + 1 as int)
    			
		    IF @DEBUG = 1 SELECT @LoyaltyChange as '@LoyaltyChange', @LoyaltyBeforeAttack as '@LoyaltyBeforeAttack'

		    --
		    -- If loyalty is lowered to below 50 then take over the village; otherwise just update the loyalty
		    --
		    IF @LoyaltyBeforeAttack - @LoyaltyChange < 50 BEGIN
		        -- invalidate quests since attacker got another village
                update Players set UpdateCompletedQuests = 1 where PlayerID = @AttackingPlayerID
				-- note that the attacker got a new village
				insert into #ChangedVillages values(@AttackingPlayerID, @TargetVillageID)

			    -- lets remember the village changed ownership
			    set @VillageTakenOver= 1
			    --get Target Village Player Info
			    select @TargetPlayerID=OwnerPlayerID, @TargetPlayerName=Players.Name from Villages inner join Players on Players.PlayerID=Villages.OwnerPlayerID where VillageID=@TargetVillageID;

			    -- note this change of ownership in the history table
			    INSERT INTO VillageOwnershipHistory( VillageID, PreviousOwnerPlayerID, CurrentOwnerPlayerID, date) 
				    VALUES (@TargetVillageID,@TargetPlayerID ,@AttackingPlayerID, @BattleTime)

			    -- change the village owneship
			    update Villages set OwnerPlayerID = @AttackingPlayerID			
				    , Loyalty = @LOYALTY_AFTER_TAKE_OVER
				    , LoyaltyLastUpdated = @BattleTime
				    where VillageID = @TargetVillageID							
			    -- delete this village from  NoTransportCoins if exists
			    delete from NoTransportVillages where VillageID=@TargetVillageID
			    -- Remove one lord from attacking force and its home village
			    Update UnitsMoving 
				    set UnitCount = UnitCount - 1
				    where EventID = @EventID and UnitTypeID = @LORD_UNIT_TYPE_ID 				
    				
			    update VillageUnits set TotalCount = TotalCount - 1	-- TotalCount can never be 0 so this is OK
				    where VillageID = @AttackingVillageID and UnitTypeID = @LORD_UNIT_TYPE_ID
    			
			    --
			    -- delete any recruiting lords/governors at the targe village. Fixes https://roe.fogbugz.com/default.asp?1467
			    --
                update UnitRecruitments set status = 1 
                    where villageid = @TargetVillageID and unittypeid = @LORD_UNIT_TYPE_ID and status = 0
    				
    				
			    -- remove any support from this village in other villages
			    delete from VillageSupportUnits
				    where SupportingVillageID = @TargetVillageID
    				
			    -- remove all troops from the target village
			    update VillageUnits set CurrentCount = 0
				    , TotalCount = 0
				    where VillageID = @TargetVillageID				
    				
			    -- delete any troops in transit from this village
			    Update Events Set Status = 1
				    from UnitMovements UM
				    where Events.EventID = UM.EventID
				    and 
				    ( 
					    -- get rid of all movements from the taken over village UNLESS this movements it an attack returning to someother village
					    --	OR unless this movement is a recall of support from the target village. 
					    ( UM.OriginVillageID = @TargetVillageID  AND CommandType not in (2 /*return*/, 3/*recall*/) ) 
					    OR 
    					
					    -- get rid of all movements to this village if this movements is a returning attack 
					    --	OR this movements is a support recall
					    ( UM.DestinationVillageID = @TargetVillageID AND CommandType in (2,3))  -- 2==return, 3==recall
				    )			
    			
			    -- leave the troops as support			
			    exec iProcessUnitMovement_Support @EventID, 2, @PrintDebugInfo		
    			
                -- remove any notes that i made for this village before
    			delete from VillageNotes where VillageID=@TargetVillageID and NoteOwnerPlayerID=@AttackingPlayerID;

    			
			    -- Handle if last defender user village lost 
    			
			    if not exists (select * from villages where OwnerPlayerID= @TargetPlayerID)
				    begin
				     --exec iCreateVillage @TargetPlayerID,@TargetPlayerName;
				     insert into Reports values (getdate(),dbo.Translate('iPUMAtt_subject'),4,dbo.Translate('iPUMAtt_body'))
    			     
				       set @ReportID = SCOPE_IDENTITY()
    			     
				       insert into ReportAddressees (PlayerID, ReportID, ForwardedByPlayerID, ForwardedOn,        IsViewed) 
				       values (@TargetPlayerID, @ReportID, null, null, 0)  
				    update Players set NewReportIndicator = 1 where PlayerID = @TargetPlayerID
    				   
				    end
		    END ELSE BEGIN		
			    update Villages 
				    set Loyalty = @LoyaltyBeforeAttack - @LoyaltyChange
				    , LoyaltyLastUpdated = @BattleTime
				    where VillageID = @TargetVillageID				
		    END  
        END
	END
	--
	-- See if any points need to be updated
	--	we do this here because we want to be sure that the village is in proper 'hands' now. 
	--	if village still belongs to the defender, then this will work. 
	--	if village is not in the hands of the attacker, then this will also work and update points correctly
	--
	IF @UpdateVillagePoints = 1 BEGIN 
		EXEC uPoints_Village @TargetVillageID
		EXEC uPoints_Player @TargetPlayerID
	END 
	IF @VillageTakenOver = 1 BEGIN 
		EXEC uPoints_Player @AttackingPlayerID	
		EXEC uPoints_Player @TargetPlayerID
	END 
	
	--
	-- ------------------------------------------
	-- if any attacking troops left & village was not taken over, then send the troops back. 
	--	otherwise, the event is left as processed, ie with status = 1, as it was set at the begining of this SP
	--
	IF EXISTS (SELECT * FROM #AttackerUnits WHERE DeployedUnitCount > KilledUnitCount)  BEGIN 
		IF @VillageTakenOver <> 1 BEGIN 
			set @isAttackReturning = 1

			-- Update the command type to return 
			update UnitMovements set CommandType =2 --Return
			, OriginVillageID = DestinationVillageID
			, DestinationVillageID = OriginVillageID 
			, VisibleToTarget = 2 -- all visible
			where EventID = @EventID

			--
			-- calculate the travel time.
			--	first get the slowest unit returning, then calc the time
			-- 
			select @UnitSpeed = min(speed)
			from #AttackerUnits AU
			join UnitTypes UT
				on AU.UnitTypeID = UT.UnitTypeID 
			where 
				DeployedUnitCount > KilledUnitCount 
			--if this @UnitSpeed is null that means we got a bug!
			IF @UnitSpeed is null  BEGIN
				RAISERROR('@UnitSpeed is null',11,1)	
			END
			set @TripDurationInTicks = dbo.fnGetTravelTime(@TargetVillageXCord, @TargetVillageYCord, @AttackingVillageXCord, @AttackingVillageYCord, @UnitSpeed)
			IF 
				( @RebelPlayerID = @TargetPlayerID OR @AbandonedPlayerID = @TargetPlayerID) 
				AND 
				( 
					EXISTS
					(select * 
						from PFs
						join PFsInPackage on
							PFsInPackage.FeatureID=PFs.FeatureID
						join PFPackages on
							PFPackages.PFPackageID=PFsInPackage.PFPackageID
						join PlayersPFPackages on
							PlayersPFPackages.PFPackageID=PFsInPackage.PFPackageID
						where 
								PlayerID=@AttackingPlayerID 
							AND PFs.FeatureID = 34 -- attack speed up 
							AND ExpiresOn>getdate()	
					)
				)
			BEGIN
				IF NOT EXISTS (SELECT * FROM #AttackerUnits WHERE DeployedUnitCount > 0 AND UnitTypeID = @LORD_UNIT_TYPE_ID) BEGIN 
					SET @TripDurationInTicks = @TripDurationInTicks  / 20
				END 
			END
			--
			-- adjust duration based on morale, for all attacks except spy-only attacks, that were launched at rebels or abandoned. 
			--
			IF @morale_isactive=1 BEGIN
				IF NOT (@isSpyOnlyAttack = 1) BEGIN
					IF ( @RebelPlayerID = @TargetPlayerID OR @AbandonedPlayerID = @TargetPlayerID)  BEGIN
						IF NOT EXISTS (SELECT * FROM #AttackerUnits WHERE DeployedUnitCount > 0 AND UnitTypeID = @LORD_UNIT_TYPE_ID) BEGIN 
							select @Morale_SpeedAdj = MoveSpeedAdj FROM PlayerMoraleEffects where Morale = @AttackMorale
							IF @Morale_SpeedAdj is not null BEGIN 
								SET @TripDurationInTicks = @TripDurationInTicks  / @Morale_SpeedAdj
							END
						END
					END 
				END
			END

			IF @DEBUG = 1 SELECT @UnitSpeed as '@UnitSpeed'
							, @TripDurationInTicks as '@TripDurationInTicks'
							, @Morale_SpeedAdj as '@Morale_SpeedAdj'

			-- Update the event time to be battle time + travel time
			update Events 
				set EventTime = dateadd(millisecond, @TripDurationInTicks/10000, @BattleTime ) 
				, [Status] = 0
				where Events.EventID = @EventID
		END 
	END



	--
	-- Calculate the loot ------------------------------------------
	--
	IF @AttackKillFactor <> 1 AND @VillageTakenOver <> 1 BEGIN 
		SET @BotProtectionFactor = 1
		select @BotProtectionFactor = factor from BotProtection where PlayerID = @AttackingPlayerID and until > getdate()

		-- Calculate the max loot we can carry 
		select @MaxLoot= sum(UMing.UnitCount * ( CarryCapacity * CarryCapAdj ))
		from UnitsMoving UMing
		join UnitTypes UT
			on UT.UnitTypeID = UMing.UnitTypeID
		left join PlayerMoraleEffects PME on Morale = @AttackMorale
		where UMing.EventID = @EventID
		group by EventID
        --
        -- figure out if any silver is protected by the hiding spot
        set @HidingSpotCapacity = isnull(dbo.fnGetBuildingProperty(@TargetVillageID, @LEVELPROP_HidingSpotCapacity),0)
        set @VillageCoins = @VillageCoins - @HidingSpotCapacity;
        IF @VillageCoins < 0 BEGIN SET @VillageCoins = 0 END
		IF @DEBUG = 1 SELECT @MaxLoot as '@MaxLoot'
						, @HidingSpotCapacity as '@HidingSpotCapacity'
						, @VillageCoins AS '@VillageCoins'			
        
     
		IF @VillageCoins > @MaxLoot BEGIN
			set @ActualLoot = @MaxLoot
		END ELSE BEGIN
			set @ActualLoot = @VillageCoins
		END 

		-- bot protection
		set @ActualLoot = @ActualLoot * @BotProtectionFactor

		-- set the loot amount for the returning troops
		update UnitMovements set Loot = @ActualLoot WHERE EventID = @EventID
		
		-- remove the loot from village
		update villages set Coins = Coins - @ActualLoot, coinslastupdates = getdate() where villageID = @TargetVillageID		
	END

	IF @RealmType <> 'X' and @RealmType <> 'CLASSIC' BEGIN 
		--
		-- if gov lost, refund gov cost
		--
		IF exists(select * FROM #AttackerUnits T WHERE UnitTypeID = @LORD_UNIT_TYPE_ID and KilledUnitCount > 0) BEGIN	
			exec qLordUnitTypeCost @AttackingPlayerID, @ChestsForLastGov output, 1

			-- sanity check 
			IF @ChestsForLastGov /2 > 0 BEGIN 
				update players set Chests=Chests+ (@ChestsForLastGov / 2) where playerid=@AttackingPlayerID 
			END
		END
	END


	IF @DEBUG = 1 select * from UnitsMoving WHERE EventID = @EventID

	--
	-- we Commit the battle tran here
	-- 
	COMMIT transaction BATTLE 


END TRY 
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'iProcessUnitMovement_Attack FAILED, BATTLE transaction! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   ERROR_NUMBER():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'	+ ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'	+ ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'		+ ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'		+  ERROR_MESSAGE() + CHAR(10)
		+ '   @EventID'							+ ISNULL(CAST(@EventID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingVillageID'				+ ISNULL(CAST(@AttackingVillageID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingPlayerName'				+ ISNULL(@AttackingPlayerName , 'Null') + CHAR(10)
		+ '   @AttackingPlayerID'				+ ISNULL(CAST(@AttackingPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageID'					+ ISNULL(CAST(@TargetVillageID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageName'				+ ISNULL(@TargetVillageName , 'Null') + CHAR(10)
		+ '   @TargetPlayerID'					+ ISNULL(CAST(@TargetPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetPlayerName'				+ ISNULL(CAST(@TargetPlayerName AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageXCord'				+ ISNULL(CAST(@TargetVillageXCord AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageYCord'				+ ISNULL(CAST(@TargetVillageYCord AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackStrength'					+ ISNULL(CAST(@AttackStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetStrength'					+ ISNULL(CAST(@TargetStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetStrengthBonusMultipier'	+ ISNULL(CAST(@TargetStrengthBonusMultipier AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackStrengthBonusMultipier'    + ISNULL(CAST(@AttackStrengthBonusMultipier AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @UnitSpeed'						+ ISNULL(CAST(@UnitSpeed AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TripDurationInTicks'				+ ISNULL(CAST(@TripDurationInTicks AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingVillageXCord'			+ ISNULL(CAST(@AttackingVillageXCord AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingVillageYCord'			+ ISNULL(CAST(@AttackingVillageYCord AS VARCHAR(max)), 'Null') + CHAR(10)		
		+ '   @BattleTime'						+ ISNULL(CAST(@BattleTime AS VARCHAR(30)), 'Null') + CHAR(10)
		+ '   @VillageCoins'					+ ISNULL(CAST(@VillageCoins AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @MaxLoot'							+ ISNULL(CAST(@MaxLoot AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ActualLoot'						+ ISNULL(CAST(@ActualLoot AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ReportID'						+ ISNULL(CAST(@ReportID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SupportAttackedReportID'			+ ISNULL(CAST(@SupportAttackedReportID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackKillFactor'				+ ISNULL(CAST(@AttackKillFactor AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetKillFactor'				+ ISNULL(CAST(@TargetKillFactor AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @LoyaltyBeforeAttack'				+ ISNULL(CAST(@LoyaltyBeforeAttack AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @LoyaltyChange'					+ ISNULL(CAST(@LoyaltyChange AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @VillageTakenOver'				+ ISNULL(CAST(@VillageTakenOver AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TotalNumberOfAttackingUnitsByPop'+ ISNULL(CAST(@TotalNumberOfAttackingUnitsByPop AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetStrengthWithBuildingBonus'	+ ISNULL(CAST(@TargetStrengthWithBuildingBonus AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BuildingDefenseBonus_PreAttack'	+ ISNULL(CAST(@BuildingDefenseBonus_PreAttack AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BuildingDefenseBonus_PostAttack'	+ ISNULL(CAST(@BuildingDefenseBonus_PostAttack AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BuildingDefenseBonus_Delta'		+ ISNULL(CAST(@BuildingDefenseBonus_Delta AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BuildingDefenseBonus_EffectiveDelta'+ ISNULL(CAST(@BuildingDefenseBonus_EffectiveDelta AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackSpyStrength'				+ ISNULL(CAST(@AttackSpyStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetCounterSpyStrength'		+ ISNULL(CAST(@TargetCounterSpyStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpySuccessChance'				+ ISNULL(CAST(@SpySuccessChance AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpyFailureChance'				+ ISNULL(CAST(@SpyFailureChance AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpySuccessful'					+ ISNULL(CAST(@SpySuccessful AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @DefenderKnowsAttackersIdentify'	+ ISNULL(CAST(@DefenderKnowsAttackersIdentify AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @DefendersTroopsVisibleToAttacker'+ ISNULL(CAST(@DefendersTroopsVisibleToAttacker AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpyOutcome'						+ ISNULL(CAST(@SpyOutcome AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @DefendersReportSubject'			+ ISNULL(CAST(@DefendersReportSubject AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @FullReportSubject'				+ ISNULL(CAST(@FullReportSubject AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @HasAttackersNonSpyUnitsSurvived'	+ ISNULL(CAST(@HasAttackersNonSpyUnitsSurvived AS VARCHAR(max)), 'Null') + CHAR(10)		
		+ '   @AttackingPlayerPoints'	+ ISNULL(CAST(@AttackingPlayerPoints AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetPlayersPoints'	+ ISNULL(CAST(@TargetPlayersPoints AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BattleHandicapRealmAttrib'	+ ISNULL(CAST(@BattleHandicapRealmAttrib AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @PointsRatio'	+ ISNULL(CAST(@PointsRatio AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BattleHandicapAttackerStrengthMultilier'	+ ISNULL(CAST(@BattleHandicapAttackerStrengthMultilier AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ThisBattleHandicap'	+ ISNULL(CAST(@ThisBattleHandicap AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @RebelPlayerID'	+ ISNULL(CAST(@RebelPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AbandonedPlayerID'	+ ISNULL(CAST(@AbandonedPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @HidingSpotCapacity'	+ ISNULL(CAST(@HidingSpotCapacity AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @LootableVillageCoins'	+ ISNULL(CAST(@LootableVillageCoins AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @villageTypePercentBonus'	+ ISNULL(CAST(@villageTypePercentBonus AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @researchPercentBonus'	+ ISNULL(CAST(@researchPercentBonus AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @researchPercentBonus_villageDef'	+ ISNULL(CAST(@researchPercentBonus_villageDef AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @Morale_MaxNormal'	+ ISNULL(CAST(@Morale_MaxNormal AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @Morale_SpeedAdj'	+ ISNULL(CAST(@Morale_SpeedAdj AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackMorale'	+ ISNULL(CAST(@AttackMorale AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackStrengthMoraleMultiplier'	+ ISNULL(CAST(@AttackStrengthMoraleMultiplier AS VARCHAR(max)), 'Null') + CHAR(10)

		+ '   @TargetVillageTypeID'	+ ISNULL(CAST(@TargetVillageTypeID AS VARCHAR(max)), 'Null') + CHAR(10)
		
		
	--
	-- IF deadlock, rerun
	--
	IF ERROR_NUMBER() = 1205 -- deadlock occured in this SP 
		OR (ERROR_NUMBER() = 50000 AND left(ERROR_MESSAGE(), 8) = 'DEADLOCK' ) -- deadlock occured in some called SP 
	BEGIN
		INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock', @ERROR_MSG)		
		WAITFOR DELAY '00:00:05'
		GOTO __RETRY_BATTLE__
	END 						
		
	RAISERROR(@ERROR_MSG,11,1)	
	return

END CATCH


/*
		~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~



		CREDIT FARMING CODE



		~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
*/
BEGIN TRY 	
	--declared and set outside of the IF, for logging reasons
	declare @creditFarmThisAttack int; --how many credits awarded in this attack
	set @creditFarmThisAttack = 0;
	
	--Servant Farm: only IF attacker won, village wasnt taken over, target was rebel or abandoned, and realm allows credit farming
	IF ((@AttackKillFactor <> 1 AND @VillageTakenOver <> 1) AND (@TargetPlayerID = @RebelPlayerID OR @TargetPlayerID = @AbandonedPlayerID)) 
	AND (EXISTS (SELECT * FROM RealmAttributes WHERE attribID = 55 AND AttribValue = '1'))  BEGIN
				
		EXEC uCreditFarm @AttackingPlayerID, @targetVillageID, @creditFarmThisAttack output
		
	END
	--END OF SERVANT FARM CODE		

END TRY 
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'iProcessUnitMovement_Attack calling uCreditFarm failed' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @EventID'							+ ISNULL(CAST(@EventID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingVillageID'				+ ISNULL(CAST(@AttackingVillageID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingPlayerName'				+ ISNULL(@AttackingPlayerName , 'Null') + CHAR(10)
		+ '   @AttackingPlayerID'				+ ISNULL(CAST(@AttackingPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageID'					+ ISNULL(CAST(@TargetVillageID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageName'				+ ISNULL(@TargetVillageName , 'Null') + CHAR(10)
		+ '   @TargetPlayerID'					+ ISNULL(CAST(@TargetPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetPlayerName'				+ ISNULL(CAST(@TargetPlayerName AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageXCord'				+ ISNULL(CAST(@TargetVillageXCord AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageYCord'				+ ISNULL(CAST(@TargetVillageYCord AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackStrength'					+ ISNULL(CAST(@AttackStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetStrength'					+ ISNULL(CAST(@TargetStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetStrengthBonusMultipier'	+ ISNULL(CAST(@TargetStrengthBonusMultipier AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackStrengthBonusMultipier'    + ISNULL(CAST(@AttackStrengthBonusMultipier AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @UnitSpeed'						+ ISNULL(CAST(@UnitSpeed AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TripDurationInTicks'				+ ISNULL(CAST(@TripDurationInTicks AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingVillageXCord'			+ ISNULL(CAST(@AttackingVillageXCord AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingVillageYCord'			+ ISNULL(CAST(@AttackingVillageYCord AS VARCHAR(max)), 'Null') + CHAR(10)		
		+ '   @BattleTime'						+ ISNULL(CAST(@BattleTime AS VARCHAR(30)), 'Null') + CHAR(10)
		+ '   @VillageCoins'					+ ISNULL(CAST(@VillageCoins AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @MaxLoot'							+ ISNULL(CAST(@MaxLoot AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ActualLoot'						+ ISNULL(CAST(@ActualLoot AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ReportID'						+ ISNULL(CAST(@ReportID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SupportAttackedReportID'			+ ISNULL(CAST(@SupportAttackedReportID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackKillFactor'				+ ISNULL(CAST(@AttackKillFactor AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetKillFactor'				+ ISNULL(CAST(@TargetKillFactor AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @LoyaltyBeforeAttack'				+ ISNULL(CAST(@LoyaltyBeforeAttack AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @LoyaltyChange'					+ ISNULL(CAST(@LoyaltyChange AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @VillageTakenOver'				+ ISNULL(CAST(@VillageTakenOver AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TotalNumberOfAttackingUnitsByPop'+ ISNULL(CAST(@TotalNumberOfAttackingUnitsByPop AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetStrengthWithBuildingBonus'	+ ISNULL(CAST(@TargetStrengthWithBuildingBonus AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BuildingDefenseBonus_PreAttack'	+ ISNULL(CAST(@BuildingDefenseBonus_PreAttack AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BuildingDefenseBonus_PostAttack'	+ ISNULL(CAST(@BuildingDefenseBonus_PostAttack AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BuildingDefenseBonus_Delta'		+ ISNULL(CAST(@BuildingDefenseBonus_Delta AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BuildingDefenseBonus_EffectiveDelta'+ ISNULL(CAST(@BuildingDefenseBonus_EffectiveDelta AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackSpyStrength'				+ ISNULL(CAST(@AttackSpyStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetCounterSpyStrength'		+ ISNULL(CAST(@TargetCounterSpyStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpySuccessChance'				+ ISNULL(CAST(@SpySuccessChance AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpyFailureChance'				+ ISNULL(CAST(@SpyFailureChance AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpySuccessful'					+ ISNULL(CAST(@SpySuccessful AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @DefenderKnowsAttackersIdentify'	+ ISNULL(CAST(@DefenderKnowsAttackersIdentify AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @DefendersTroopsVisibleToAttacker'+ ISNULL(CAST(@DefendersTroopsVisibleToAttacker AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpyOutcome'						+ ISNULL(CAST(@SpyOutcome AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @DefendersReportSubject'			+ ISNULL(CAST(@DefendersReportSubject AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @FullReportSubject'				+ ISNULL(CAST(@FullReportSubject AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @HasAttackersNonSpyUnitsSurvived'	+ ISNULL(CAST(@HasAttackersNonSpyUnitsSurvived AS VARCHAR(max)), 'Null') + CHAR(10)		
		+ '   @AttackingPlayerPoints'	+ ISNULL(CAST(@AttackingPlayerPoints AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetPlayersPoints'	+ ISNULL(CAST(@TargetPlayersPoints AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BattleHandicapRealmAttrib'	+ ISNULL(CAST(@BattleHandicapRealmAttrib AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @PointsRatio'	+ ISNULL(CAST(@PointsRatio AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BattleHandicapAttackerStrengthMultilier'	+ ISNULL(CAST(@BattleHandicapAttackerStrengthMultilier AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ThisBattleHandicap'	+ ISNULL(CAST(@ThisBattleHandicap AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @RebelPlayerID'	+ ISNULL(CAST(@RebelPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AbandonedPlayerID'	+ ISNULL(CAST(@AbandonedPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @HidingSpotCapacity'	+ ISNULL(CAST(@HidingSpotCapacity AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @LootableVillageCoins'	+ ISNULL(CAST(@LootableVillageCoins AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @villageTypePercentBonus'	+ ISNULL(CAST(@villageTypePercentBonus AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @researchPercentBonus'	+ ISNULL(CAST(@researchPercentBonus AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageTypeID'	+ ISNULL(CAST(@TargetVillageTypeID AS VARCHAR(max)), 'Null') + CHAR(10)
		
		+ '   ERROR_NUMBER():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'	+ ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'	+ ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'		+ ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'		+  ERROR_MESSAGE() + CHAR(10)
	
	
	--
	--
	--	WE DO NOT WANT AN ERROR IN THIS CODE, TO CAUSE THE WHOLE sp TO FAIL. 
	--	SO WE LOG THE ERROR, AND MOVE ON. 
	--
	--
	INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'AttackSP-CreditFarmingCodeError', @ERROR_MSG)		


END CATCH



__RETRY_CREATEREPORT__:
BEGIN TRY 
    --
    --
    -- update stats. this is outside of a transaction on purpose
    --
    --
	IF @AttackingPlayerID <> @TargetPlayerID BEGIN -- only if attacker and defender are different
    	--
    	-- attacker should get the points for troops killed
    	--
    	insert into PlayerStats 
	        select @AttackingPlayerID, 1, 0 where not exists(select * from PlayerStats where playerid = @AttackingPlayerID and statid = 1)
    	    
	    update PlayerStats SET StatValue = StatValue +
		    (
		    select  isnull(sum(KilledUnitCount * UT.Population),0)
			    FROM #TargetUnits T
			    JOIN UnitTypes UT
				    on UT.UnitTypeID = T.UnitTypeID
		    )
		    where PlayerID = @AttackingPlayerID and StatID = 1

    	--
    	-- the above statement could have given the attacker points for killing his own troops supporting the village
    	--  therefore we now subtract the points for any kills he got for this own troops. 
	    update PlayerStats SET StatValue = StatValue -
		    (
		    select  isnull(sum(KilledUnitCount * UT.Population), 0)
			    FROM #TargetSupportingUnits T
			    JOIN UnitTypes UT
				    on UT.UnitTypeID = T.UnitTypeID
				WHERE SupportingVillageOwnerPlayerID = @AttackingPlayerID
		    )
		    where PlayerID = @AttackingPlayerID and StatID = 1
    	        	    
        --
        -- all attacking troops killed, means that defender should get the points
        --
       	insert into PlayerStats 
	        select @TargetPlayerID, 2, 0 where not exists(select * from PlayerStats where playerid = @TargetPlayerID and statid = 2)
       	insert into PlayerStats 
	        select @TargetPlayerID, 3, 0 where not exists(select * from PlayerStats where playerid = @TargetPlayerID and statid = 3)

	    update PlayerStats SET StatValue = StatValue +
		    (
		    select  isnull(sum(KilledUnitCount * UT.Population), 0)
			    FROM #AttackerUnits T
			    JOIN UnitTypes UT
				    on UT.UnitTypeID = T.UnitTypeID
		    )
		    where PlayerID = @TargetPlayerID and StatID = 2 

	    update PlayerStats SET StatValue = StatValue +
		    isnull((
		        select isnull(KilledUnitCount,0) FROM #AttackerUnits T WHERE UnitTypeID = @LORD_UNIT_TYPE_ID
		    ),0)
		    where PlayerID = @TargetPlayerID and StatID = 3 
		                 



    	--IF @AttackKillFactor <> 1 BEGIN 
    	--    --
    	--    -- if not all attacking troops killed, means that attacker should get the points
    	--    --
    	--    insert into PlayerStats 
	    --        select @AttackingPlayerID, 1, 0 where not exists(select * from PlayerStats where playerid = @AttackingPlayerID and statid = 1)
    	    
	    --    update PlayerStats SET StatValue = StatValue +
		   --     (
		   --     select  isnull(sum(KilledUnitCount * UT.Population),0)
			  --      FROM #TargetUnits T
			  --      JOIN UnitTypes UT
				 --       on UT.UnitTypeID = T.UnitTypeID
		   --     )
		   --     where PlayerID = @AttackingPlayerID and StatID = 1

    	--    --
    	--    -- the above statement could have given the attacker points for killing his own troops supporting the village
    	--    --  therefore we now subtract the points for any kills he got for this own troops. 
	    --    update PlayerStats SET StatValue = StatValue -
		   --     (
		   --     select  isnull(sum(KilledUnitCount * UT.Population), 0)
			  --      FROM #TargetSupportingUnits T
			  --      JOIN UnitTypes UT
				 --       on UT.UnitTypeID = T.UnitTypeID
				 --   WHERE SupportingVillageOwnerPlayerID = @AttackingPlayerID
		   --     )
		   --     where PlayerID = @AttackingPlayerID and StatID = 1
    	        	    
     --   END ELSE BEGIN
     --       --
     --       -- all attacking troops killed, means that defender should get the points
     --       --
     --  	    insert into PlayerStats 
	    --        select @TargetPlayerID, 2, 0 where not exists(select * from PlayerStats where playerid = @TargetPlayerID and statid = 2)
     --  	    insert into PlayerStats 
	    --        select @TargetPlayerID, 3, 0 where not exists(select * from PlayerStats where playerid = @TargetPlayerID and statid = 3)

	    --    update PlayerStats SET StatValue = StatValue +
		   --     (
		   --     select  isnull(sum(KilledUnitCount * UT.Population), 0)
			  --      FROM #AttackerUnits T
			  --      JOIN UnitTypes UT
				 --       on UT.UnitTypeID = T.UnitTypeID
		   --     )
		   --     where PlayerID = @TargetPlayerID and StatID = 2 

	    --    update PlayerStats SET StatValue = StatValue +
		   --     isnull((
		   --         select isnull(KilledUnitCount,0) FROM #AttackerUnits T WHERE UnitTypeID = @LORD_UNIT_TYPE_ID
		   --     ),0)
		   --     where PlayerID = @TargetPlayerID and StatID = 3 
		                 
     --   END    	        
    END 

	--
	-- Create the Report... ------------------------------------------
	--
	--
	--
	set @TargetVillageName_Full =  @TargetVillageName + '(' + cast(@TargetVillageXCord as varchar(10)) + ',' + cast(@TargetVillageYCord as varchar(10)) + ')'
	IF @DefenderKnowsAttackersIdentify = 1 BEGIN
		-- Your 'village' was attacked by 'player
		set @AttackingVillageName_Full = @AttackingVillageName + '(' + cast(@AttackingVillageXCord as varchar(10)) + ',' + cast(@AttackingVillageYCord as varchar(10)) + ')'
		SET @DefendersReportSubject = @TargetVillageName_Full + ' was attacked by ' +  @AttackingPlayerName  + ' ' + @AttackingVillageName_Full
	END ELSE BEGIN
		-- Your 'village' was attacked
		SET @DefendersReportSubject = @TargetVillageName_Full + ' was attacked'
	END
	
	--
	-- create a subject for the attacked report for attacker
	IF @TargetSpecialPlayerTypeID is null BEGIN
		SET @FullReportSubject = @AttackingPlayerName + ' attacks ' + @TargetPlayerName + ', ' + @TargetVillageName_Full
	END ELSE BEGIN
		SET @FullReportSubject = @AttackingPlayerName + ' attacks ' + @TargetVillageName_Full
	END



	--
	-- Create the report
	BEGIN TRAN CreateReport
	
		insert into Reports (Time,Subject, ReportTypeID)  values (@BattleTime, @FullReportSubject, 1 /*1 == battle report*/)
		set @ReportID = SCOPE_IDENTITY()

		insert into BattleReports values (@ReportID , @AttackingVillageID, @TargetVillageID, @AttackingPlayerID, @TargetPlayerID, @ActualLoot
			, @LoyaltyBeforeAttack, @LoyaltyChange, @DefendersTroopsVisibleToAttacker
			, @VillageCoins - isnull(@ActualLoot,0), @DefenderKnowsAttackersIdentify, @SpyOutcome,@SpySuccessChance)

		insert into BattleReportUnits 
			select @ReportID, 0 /*attacker*/, UnitTypeID, DeployedUnitCount, KilledUnitCount, DeployedUnitCount - KilledUnitCount from #AttackerUnits 
		insert into BattleReportUnits 
			select @ReportID, 1 /*defender*/, UnitTypeID, sum(DeployedUnitCount), sum(KilledUnitCount), sum(DeployedUnitCount) - sum(KilledUnitCount) from #TargetUnits group by UnitTypeID

		IF EXISTS(SELECT * FROM #BuildingAttacks) BEGIN
			INSERT INTO BattleReportBuildings (ReportID, BuildingTypeID, BeforeAttackLevel, AfterAttackLevel)
				SELECT  
					@ReportID
					, BuildingTypeID
					, CurrentLevelInVillage
					, LevelAfterAttack
				FROM #BuildingAttacks
				WHERE CurrentLevelInVillage <> LevelAfterAttack
		END
        --
		-- attacker
		--
		insert into ReportAddressees (PlayerID, ReportID, ForwardedByPlayerID, ForwardedOn, IsViewed, PointOfView, AlternateSubject) 
			values (@AttackingPlayerID, @ReportID, null, null, 0, 0, @FullReportSubject)
		update Players set NewReportIndicator = 1 where PlayerID = @AttackingPlayerID
		SET @AttackerReportRecordID = SCOPE_IDENTITY() 
		
		-- Figure out what flags to give the attacker - this the troops attack flag
		IF EXISTS (SELECT * FROM #AttackerUnits WHERE DeployedUnitCount > 0 AND UnitTypeID <> @UNIT_SPY_ID) BEGIN -- if attacker had any (non-spy) troops in this attack		   		
            IF 	NOT EXISTS (SELECT * FROM #AttackerUnits WHERE KilledUnitCount <> 0 AND UnitTypeID <> @UNIT_SPY_ID) BEGIN 
                -- if there are no rows where # killed is other than 0, then all rows are 0, therefore NO TROOPS KILLED
                INSERT INTO ReportInfoFlag(RecordID, FlagID, FlagValue) values (@AttackerReportRecordID, 1, 0)
            END ELSE IF NOT EXISTS (SELECT * FROM #AttackerUnits WHERE DeployedUnitCount <> KilledUnitCount  AND UnitTypeID <> @UNIT_SPY_ID) BEGIN 
                -- if there are no rows where less troops were killed then deployed, AND the above case is not true (ie, at least some troops killed) therefore some but not all troops were killed
                INSERT INTO ReportInfoFlag(RecordID, FlagID, FlagValue) values (@AttackerReportRecordID, 1, 2)
            END ELSE BEGIN  --- all troops killed
                INSERT INTO ReportInfoFlag(RecordID, FlagID, FlagValue) values (@AttackerReportRecordID, 1, 1)
            END
        END 
		-- Figure out what flags to give the attacker - this is the spy attack flag
        IF @SpyOutcome <> -1 BEGIN
            IF @SpyOutcome = 1 BEGIN -- spies were successful
                INSERT INTO ReportInfoFlag(RecordID, FlagID, FlagValue) values (@AttackerReportRecordID, 2, 0)
            END
        END				

		-- Figure out if gov was involved and give the flag if it did
        IF EXISTS (SELECT * FROM #AttackerUnits WHERE DeployedUnitCount > 0 AND UnitTypeID = @LORD_UNIT_TYPE_ID) 
			BEGIN 
            INSERT INTO ReportInfoFlag(RecordID, FlagID, FlagValue) values (@AttackerReportRecordID, 3, 1)
        END
		
		-- remember the morale of the attack for the attacker's report
		IF @morale_isactive = 1 BEGIN
            INSERT INTO ReportInfoFlag(RecordID, FlagID, FlagValue) values (@AttackerReportRecordID, 4, @AttackMorale)			
		END 				

        --
		-- defender	
		--
		IF @AttackingPlayerID <> @TargetPlayerID BEGIN
			insert into ReportAddressees (PlayerID, ReportID, ForwardedByPlayerID, ForwardedOn, IsViewed, PointOfView, AlternateSubject) 
				values (@TargetPlayerID, @ReportID, null, null, 0, 1, @DefendersReportSubject)
			update Players set NewReportIndicator = 1 where PlayerID = @TargetPlayerID
            SET @DefenderReportRecordID = SCOPE_IDENTITY() 
		    -- Figure out what flags to give the defender
		    		   	
		    IF EXISTS (SELECT * FROM #AttackerUnits WHERE DeployedUnitCount > 0 AND UnitTypeID <> @UNIT_SPY_ID) BEGIN -- if attack consisted of some non-spy units
		        IF 	(@TargetStrength = 0 AND EXISTS(SELECT * FROM #AttackerUnits WHERE DeployedUnitCount > KilledUnitCount  AND UnitTypeID <> @UNIT_SPY_ID)) -- IF there were no defending troops AND at least one attacker, non-spy unit survived
		            OR (@TargetStrength > 0 AND NOT EXISTS (SELECT * FROM #TargetUnits WHERE DeployedUnitCount > KilledUnitCount))  -- if there were troops defending AND there are no rows where # deployed > # killed, then we know all troops were killed
				    BEGIN
		            --
		            -- IF there were no defending troops AND at least one attacker, non-spy unit survived
		            -- OR IF there were defending troops and all were killed
		            -- THEN we give this flag (RED)
    		        
                    INSERT INTO ReportInfoFlag(RecordID, FlagID, FlagValue) values (@DefenderReportRecordID, 1, 2)
                
                END ELSE IF 
                    ( @TargetStrength > 0 AND NOT EXISTS (SELECT * FROM #TargetUnits WHERE KilledUnitCount > 0) ) 
                    OR ( @TargetStrength = 0 AND NOT EXISTS (SELECT * FROM #AttackerUnits WHERE DeployedUnitCount <> KilledUnitCount AND UnitTypeID <> @UNIT_SPY_ID) )
				    BEGIN
                    --
                    -- no losses AND there were some troops defending
                    -- OR no troops defending AND ALL attacker troops killed - ie, NO attacker's troops remaining
                    -- THEN we give this flag (GREEN)            
                
                    INSERT INTO ReportInfoFlag(RecordID, FlagID, FlagValue) values (@DefenderReportRecordID, 1, 0)
                END ELSE BEGIN  -- otherwise, yellow
                    INSERT INTO ReportInfoFlag(RecordID, FlagID, FlagValue) values (@DefenderReportRecordID, 1, 1)
                END
		    END 
            
	        -- Figure out what flags to give the attacker - this the spy attack flag
            IF @SpyOutcome <> -1 BEGIN
                IF @SpyOutcome = 1 BEGIN -- spied were sucessful                
                    INSERT INTO ReportInfoFlag(RecordID, FlagID, FlagValue) values (@DefenderReportRecordID, 2, 1)
                END
            END	
			
			-- Figure out if gov was involved and give the flag if it did		
			IF EXISTS (SELECT * FROM #AttackerUnits WHERE DeployedUnitCount > 0 AND UnitTypeID = @LORD_UNIT_TYPE_ID) 
				BEGIN 
				INSERT INTO ReportInfoFlag(RecordID, FlagID, FlagValue) values (@DefenderReportRecordID, 3, 1)
			END							                
		END
		
		--
		-- Add building info if spies successful
		IF @SpySuccessful = 1  BEGIN
			INSERT INTO BattleReportBuildingIntel (ReportID, BuildingTypeID, [Level])
				SELECT @ReportID, BuildingTypeID, [Level] FROM Buildings WHERE VillageID = @TargetVillageID
		END

		--
		-- create support attacked reports, if necessary
		--
		IF EXISTS (select * from #TargetSupportingUnits WHERE SupportingVillageOwnerPlayerID != @TargetPlayerID) BEGIN 
			INSERT INTO BattleReportSupport (ReportID, SupportingVillageID, SupportingPlayerID, ViewAccessLevel)
				SELECT @ReportID, SupportingVillageID, SupportingVillageOwnerPlayerID, 0
					FROM #TargetSupportingUnits WHERE SupportingVillageOwnerPlayerID != @TargetPlayerID
					GROUP BY SupportingVillageID, SupportingVillageOwnerPlayerID

			INSERT INTO BattleReportSupportUnits (ReportID, SupportingVillageID, UnitTypeID, DeployedUnitCount, KilledUnitCount, ReaminingUnitCount)
				SELECT @ReportID, SupportingVillageID, UnitTypeID, DeployedUnitCount, KilledUnitCount, DeployedUnitCount - KilledUnitCount
					FROM #TargetSupportingUnits WHERE SupportingVillageOwnerPlayerID != @TargetPlayerID

			--
			-- create a subject for the support attacked report
			IF @TargetSpecialPlayerTypeID is null BEGIN
				SET @SupportAttackedReportSubject = 'Your troops supporting ' + @TargetPlayerName + ' at ' + @TargetVillageName + '(' 
					+ cast(@TargetVillageXCord as varchar(10))+ ',' 
					+ cast(@TargetVillageYCord as varchar(10)) + ') have been attacked'
			END ELSE BEGIN
				SET @SupportAttackedReportSubject = 'Your troops supporting ' + @TargetVillageName + '(' 
					+ cast(@TargetVillageXCord as varchar(10))+ ',' 
					+ cast(@TargetVillageYCord as varchar(10)) + ') have been attacked'			
			END
			--
			--  Create reports for the supporting players. 
			--		This is a little tricky. We first get the list of distinct supporting players and put them into a table with a INDETITY column so that we have
			--		then numbered.  then we loop throught those players and create a report for each one
			--	
			INSERT INTO #TargetSupportingUnitsReports (SupportingVillageOwnerPlayerID)
				SELECT SupportingVillageOwnerPlayerID FROM #TargetSupportingUnits WHERE SupportingVillageOwnerPlayerID != @TargetPlayerID GROUP BY SupportingVillageOwnerPlayerID

			IF @DEBUG = 1 select 0 as '#TargetSupportingUnitsReports', * from #TargetSupportingUnitsReports 

			declare @i int
			declare @count int
			declare @loop bit
			set @i = 1
			set @loop = 1

			SELECT @count = max([index] ) FROM #TargetSupportingUnitsReports
			While @loop = 1 BEGIN

				insert into Reports (Time,Subject, ReportTypeID)  
					values (@BattleTime
					, @SupportAttackedReportSubject
					, 2 /*2 == support attacked report*/)
				set @SupportAttackedReportID = SCOPE_IDENTITY()

				IF @DEBUG = 1 select @SupportAttackedReportID as '@SupportAttackedReportID'

				insert into SupportAttackedReports (ReportID, BattleReportID, SupportingPlayerID)
					SELECT @SupportAttackedReportID , @ReportID, SupportingVillageOwnerPlayerID
					FROM #TargetSupportingUnitsReports
					WHERE [index] = @i

				insert into ReportAddressees (PlayerID, ReportID, ForwardedByPlayerID, ForwardedOn, IsViewed) 
					SELECT SupportingVillageOwnerPlayerID, @SupportAttackedReportID, null, null, 0
						FROM #TargetSupportingUnitsReports
						WHERE [index] = @i

				update Players set NewReportIndicator = 1 where PlayerID in 
					(SELECT SupportingVillageOwnerPlayerID
						FROM #TargetSupportingUnitsReports
						WHERE [index] = @i)
				IF  @i = @count BEGIN
					set @loop= 0
				END ELSE BEGIN
					set @i = @i + 1
				END
			
			END

		END 


	Commit TRAN CreateReport

	IF @DEBUG = 1 select 'Reports', * from Reports where ReportID = @ReportID
	IF @DEBUG = 1 select 'BattleReports', * from BattleReports where ReportID = @ReportID
	IF @DEBUG = 1 select 'BattleReportUnits', * from BattleReportUnits where ReportID = @ReportID
	IF @DEBUG = 1 select 'BattleReportBuildings', * from BattleReportBuildings where ReportID = @ReportID
	IF @DEBUG = 1 select 'BattleReportBuildings', * from BattleReportBuildingIntel where ReportID = @ReportID
	
	IF @DEBUG = 1 select 'BattleReportSupport', * from BattleReportSupport where ReportID = @ReportID
	IF @DEBUG = 1 select 'BattleReportSupportUnits', * from BattleReportSupportUnits where ReportID = @ReportID
	IF @DEBUG = 1 select 'ReportAddressees', * from ReportAddressees where ReportID = @ReportID

	IF @DEBUG = 1 select 'SupportAttackedReports', * from SupportAttackedReports where BattleReportID = @ReportID
	IF @DEBUG = 1 select 'ReportAddressees', * from ReportAddressees where ReportID in (SELECT ReportID FROM SupportAttackedReports where BattleReportID = @ReportID)
	IF @DEBUG = 1 SELECT 'END iProcessUnitMovement_Attack ' + cast(@EventID as varchar(10))


	drop table #TargetStrength
	-- drop table #TargetUnits -- we need this table for later
	--drop table #TargetSupportingUnits -- we need this table for later
	drop table #TargetSupportingUnitsReports
	drop table #TargetUnitDefenseStrength
	-- drop table #AttackerUnits -- we need this table for later
	drop table #BuildingAttacks

END TRY 
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'iProcessUnitMovement_Attack FAILED, CreateReport transaction ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @EventID'							+ ISNULL(CAST(@EventID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingVillageID'				+ ISNULL(CAST(@AttackingVillageID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingPlayerName'				+ ISNULL(@AttackingPlayerName , 'Null') + CHAR(10)
		+ '   @AttackingPlayerID'				+ ISNULL(CAST(@AttackingPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageID'					+ ISNULL(CAST(@TargetVillageID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageName'				+ ISNULL(@TargetVillageName , 'Null') + CHAR(10)
		+ '   @TargetPlayerID'					+ ISNULL(CAST(@TargetPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetPlayerName'				+ ISNULL(CAST(@TargetPlayerName AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageXCord'				+ ISNULL(CAST(@TargetVillageXCord AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageYCord'				+ ISNULL(CAST(@TargetVillageYCord AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackStrength'					+ ISNULL(CAST(@AttackStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetStrength'					+ ISNULL(CAST(@TargetStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetStrengthBonusMultipier'	+ ISNULL(CAST(@TargetStrengthBonusMultipier AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackStrengthBonusMultipier'    + ISNULL(CAST(@AttackStrengthBonusMultipier AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @UnitSpeed'						+ ISNULL(CAST(@UnitSpeed AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TripDurationInTicks'				+ ISNULL(CAST(@TripDurationInTicks AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingVillageXCord'			+ ISNULL(CAST(@AttackingVillageXCord AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingVillageYCord'			+ ISNULL(CAST(@AttackingVillageYCord AS VARCHAR(max)), 'Null') + CHAR(10)		
		+ '   @BattleTime'						+ ISNULL(CAST(@BattleTime AS VARCHAR(30)), 'Null') + CHAR(10)
		+ '   @VillageCoins'					+ ISNULL(CAST(@VillageCoins AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @MaxLoot'							+ ISNULL(CAST(@MaxLoot AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ActualLoot'						+ ISNULL(CAST(@ActualLoot AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ReportID'						+ ISNULL(CAST(@ReportID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SupportAttackedReportID'			+ ISNULL(CAST(@SupportAttackedReportID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackKillFactor'				+ ISNULL(CAST(@AttackKillFactor AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetKillFactor'				+ ISNULL(CAST(@TargetKillFactor AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @LoyaltyBeforeAttack'				+ ISNULL(CAST(@LoyaltyBeforeAttack AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @LoyaltyChange'					+ ISNULL(CAST(@LoyaltyChange AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @VillageTakenOver'				+ ISNULL(CAST(@VillageTakenOver AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TotalNumberOfAttackingUnitsByPop'+ ISNULL(CAST(@TotalNumberOfAttackingUnitsByPop AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetStrengthWithBuildingBonus'	+ ISNULL(CAST(@TargetStrengthWithBuildingBonus AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BuildingDefenseBonus_PreAttack'	+ ISNULL(CAST(@BuildingDefenseBonus_PreAttack AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BuildingDefenseBonus_PostAttack'	+ ISNULL(CAST(@BuildingDefenseBonus_PostAttack AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BuildingDefenseBonus_Delta'		+ ISNULL(CAST(@BuildingDefenseBonus_Delta AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BuildingDefenseBonus_EffectiveDelta'+ ISNULL(CAST(@BuildingDefenseBonus_EffectiveDelta AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackSpyStrength'				+ ISNULL(CAST(@AttackSpyStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetCounterSpyStrength'		+ ISNULL(CAST(@TargetCounterSpyStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpySuccessChance'				+ ISNULL(CAST(@SpySuccessChance AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpyFailureChance'				+ ISNULL(CAST(@SpyFailureChance AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpySuccessful'					+ ISNULL(CAST(@SpySuccessful AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @DefenderKnowsAttackersIdentify'	+ ISNULL(CAST(@DefenderKnowsAttackersIdentify AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @DefendersTroopsVisibleToAttacker'+ ISNULL(CAST(@DefendersTroopsVisibleToAttacker AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpyOutcome'						+ ISNULL(CAST(@SpyOutcome AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @DefendersReportSubject'			+ ISNULL(CAST(@DefendersReportSubject AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @FullReportSubject'				+ ISNULL(CAST(@FullReportSubject AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SupportAttackedReportSubject'	+ ISNULL(CAST(@SupportAttackedReportSubject AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingPlayerPoints'	+ ISNULL(CAST(@AttackingPlayerPoints AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetPlayersPoints'	+ ISNULL(CAST(@TargetPlayersPoints AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BattleHandicapRealmAttrib'	+ ISNULL(CAST(@BattleHandicapRealmAttrib AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @PointsRatio'	+ ISNULL(CAST(@PointsRatio AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BattleHandicapAttackerStrengthMultilier'	+ ISNULL(CAST(@BattleHandicapAttackerStrengthMultilier AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ThisBattleHandicap'	+ ISNULL(CAST(@ThisBattleHandicap AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @RebelPlayerID'	+ ISNULL(CAST(@RebelPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AbandonedPlayerID'	+ ISNULL(CAST(@AbandonedPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @HidingSpotCapacity'	+ ISNULL(CAST(@HidingSpotCapacity AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @LootableVillageCoins'	+ ISNULL(CAST(@LootableVillageCoins AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @villageTypePercentBonus'	+ ISNULL(CAST(@villageTypePercentBonus AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @researchPercentBonus'	+ ISNULL(CAST(@researchPercentBonus AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageTypeID'	+ ISNULL(CAST(@TargetVillageTypeID AS VARCHAR(max)), 'Null') + CHAR(10)

		+ '   ERROR_NUMBER():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'	+ ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'	+ ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'		+ ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'		+  ERROR_MESSAGE() + CHAR(10)
		

	--
	-- IF deadlock, rerun
	--
	IF ERROR_NUMBER() = 1205 -- deadlock occured in this SP 
		OR (ERROR_NUMBER() = 50000 AND left(ERROR_MESSAGE(), 8) = 'DEADLOCK' ) -- deadlock occured in some called SP 
	BEGIN
		INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock', @ERROR_MSG)		
		WAITFOR DELAY '00:00:05'
		GOTO __RETRY_CREATEREPORT__
	END 						
		
	RAISERROR(@ERROR_MSG,11,1)	

END CATCH



--
--
--
-- UPDATE CACHE TIME STAMPS. 
--
--	also do the admin log
--
--
--
--
__RETRY_CACHETIMESTAMPS__:
BEGIN TRY 

		--
		-- update cache time stamps
		--
		-- outgoing cache item (2) for attacker
		UPDATE PlayerCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @AttackingPlayerID and CachedItemID = 2  
		IF (@@rowcount < 1 ) BEGIN
			INSERT INTO PlayerCacheTimeStamps values (@AttackingPlayerID, 2, getdate())
		END
		-- incoming cache item (1) for defender
		UPDATE PlayerCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @TargetPlayerID and CachedItemID = 1  
		IF (@@rowcount < 1 ) BEGIN
			INSERT INTO PlayerCacheTimeStamps values (@TargetPlayerID, 1, getdate())
		END
		-- incoming cache item (1) for attacker if his troops are returning
		IF @isAttackReturning = 1 BEGIN 
			UPDATE PlayerCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @AttackingPlayerID and CachedItemID = 1  
			IF (@@rowcount < 1 ) BEGIN
				INSERT INTO PlayerCacheTimeStamps values (@AttackingPlayerID, 1, getdate())
			END
		END

		if @VillageTakenOver = 1 BEGIN
			EXEC uTriggerUnitMovementCacheUpdateBasedOnVID @TargetVillageID
		END
		
		--
		-- admin log
		--
		DECLARE @SupportString VARCHAR(max) 
		SELECT @SupportString = COALESCE(@SupportString + ', ', '') + name + ':(' +  CAST (UnitTypeID as varchar(10)) + ')' +  CAST (DeployedUnitCount as varchar(10)) + '->' + CAST (DeployedUnitCount - KilledUnitCount as varchar(10))
			FROM #TargetSupportingUnits T join Players P on p.PlayerID = t.SupportingVillageOwnerPlayerID 
			order by SupportingVillageOwnerPlayerID

		DECLARE @TargetUnitsString VARCHAR(max) 		
		SELECT @TargetUnitsString = COALESCE(@TargetUnitsString + ', ', '') + '(' +  CAST (UnitTypeID as varchar(10)) + ')' +  CAST (DeployedUnitCount as varchar(10)) + '->' + CAST (DeployedUnitCount - KilledUnitCount as varchar(10))
			FROM #TargetUnits T 
			where DeployedUnitCount > 0 
			order by UnitTypeID

		DECLARE @AttackerUnitsString VARCHAR(max) 		
		SELECT @AttackerUnitsString = COALESCE(@AttackerUnitsString + ', ', '') + '(' +  CAST (UnitTypeID as varchar(10)) + ')' +  CAST (DeployedUnitCount as varchar(10)) + '->' + CAST (DeployedUnitCount - KilledUnitCount as varchar(10))
			FROM #AttackerUnits T 
			order by UnitTypeID

		
		UPDATE admin_attackLog 
			SET 
			LandTime = @BattleTime,
			loyalty_beforeAttack = @LoyaltyBeforeAttack,
			loyalty_change = @LoyaltyChange,
			supportDetailedInfo = @SupportString,
			attackTroops_isGov = (SELECT top 1 1 FROM #AttackerUnits WHERE DeployedUnitCount > 0 AND UnitTypeID = @LORD_UNIT_TYPE_ID),
			defenderTroops = @TargetUnitsString,
			attackTroops = @AttackerUnitsString,
			AttackStrength = @AttackStrength,
			defenderPID = @TargetPlayerID,
			CreditFarmed = @creditFarmThisAttack
		where eventID = @EventID

		

END TRY 
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'iProcessUnitMovement_Attack FAILED, UpdateCacheTimeStamps transaction ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @EventID'							+ ISNULL(CAST(@EventID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingVillageID'				+ ISNULL(CAST(@AttackingVillageID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingPlayerName'				+ ISNULL(@AttackingPlayerName , 'Null') + CHAR(10)
		+ '   @AttackingPlayerID'				+ ISNULL(CAST(@AttackingPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageID'					+ ISNULL(CAST(@TargetVillageID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageName'				+ ISNULL(@TargetVillageName , 'Null') + CHAR(10)
		+ '   @TargetPlayerID'					+ ISNULL(CAST(@TargetPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetPlayerName'				+ ISNULL(CAST(@TargetPlayerName AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageXCord'				+ ISNULL(CAST(@TargetVillageXCord AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageYCord'				+ ISNULL(CAST(@TargetVillageYCord AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackStrength'					+ ISNULL(CAST(@AttackStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetStrength'					+ ISNULL(CAST(@TargetStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetStrengthBonusMultipier'	+ ISNULL(CAST(@TargetStrengthBonusMultipier AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackStrengthBonusMultipier'    + ISNULL(CAST(@AttackStrengthBonusMultipier AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @UnitSpeed'						+ ISNULL(CAST(@UnitSpeed AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TripDurationInTicks'				+ ISNULL(CAST(@TripDurationInTicks AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingVillageXCord'			+ ISNULL(CAST(@AttackingVillageXCord AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingVillageYCord'			+ ISNULL(CAST(@AttackingVillageYCord AS VARCHAR(max)), 'Null') + CHAR(10)		
		+ '   @BattleTime'						+ ISNULL(CAST(@BattleTime AS VARCHAR(30)), 'Null') + CHAR(10)
		+ '   @VillageCoins'					+ ISNULL(CAST(@VillageCoins AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @MaxLoot'							+ ISNULL(CAST(@MaxLoot AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ActualLoot'						+ ISNULL(CAST(@ActualLoot AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ReportID'						+ ISNULL(CAST(@ReportID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SupportAttackedReportID'			+ ISNULL(CAST(@SupportAttackedReportID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackKillFactor'				+ ISNULL(CAST(@AttackKillFactor AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetKillFactor'				+ ISNULL(CAST(@TargetKillFactor AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @LoyaltyBeforeAttack'				+ ISNULL(CAST(@LoyaltyBeforeAttack AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @LoyaltyChange'					+ ISNULL(CAST(@LoyaltyChange AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @VillageTakenOver'				+ ISNULL(CAST(@VillageTakenOver AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TotalNumberOfAttackingUnitsByPop'+ ISNULL(CAST(@TotalNumberOfAttackingUnitsByPop AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetStrengthWithBuildingBonus'	+ ISNULL(CAST(@TargetStrengthWithBuildingBonus AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BuildingDefenseBonus_PreAttack'	+ ISNULL(CAST(@BuildingDefenseBonus_PreAttack AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BuildingDefenseBonus_PostAttack'	+ ISNULL(CAST(@BuildingDefenseBonus_PostAttack AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BuildingDefenseBonus_Delta'		+ ISNULL(CAST(@BuildingDefenseBonus_Delta AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BuildingDefenseBonus_EffectiveDelta'+ ISNULL(CAST(@BuildingDefenseBonus_EffectiveDelta AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackSpyStrength'				+ ISNULL(CAST(@AttackSpyStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetCounterSpyStrength'		+ ISNULL(CAST(@TargetCounterSpyStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpySuccessChance'				+ ISNULL(CAST(@SpySuccessChance AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpyFailureChance'				+ ISNULL(CAST(@SpyFailureChance AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpySuccessful'					+ ISNULL(CAST(@SpySuccessful AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @DefenderKnowsAttackersIdentify'	+ ISNULL(CAST(@DefenderKnowsAttackersIdentify AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @DefendersTroopsVisibleToAttacker'+ ISNULL(CAST(@DefendersTroopsVisibleToAttacker AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpyOutcome'						+ ISNULL(CAST(@SpyOutcome AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @DefendersReportSubject'			+ ISNULL(CAST(@DefendersReportSubject AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @FullReportSubject'				+ ISNULL(CAST(@FullReportSubject AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SupportAttackedReportSubject'	+ ISNULL(CAST(@SupportAttackedReportSubject AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AttackingPlayerPoints'	+ ISNULL(CAST(@AttackingPlayerPoints AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetPlayersPoints'	+ ISNULL(CAST(@TargetPlayersPoints AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BattleHandicapRealmAttrib'	+ ISNULL(CAST(@BattleHandicapRealmAttrib AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @PointsRatio'	+ ISNULL(CAST(@PointsRatio AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BattleHandicapAttackerStrengthMultilier'	+ ISNULL(CAST(@BattleHandicapAttackerStrengthMultilier AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ThisBattleHandicap'	+ ISNULL(CAST(@ThisBattleHandicap AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @RebelPlayerID'	+ ISNULL(CAST(@RebelPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AbandonedPlayerID'	+ ISNULL(CAST(@AbandonedPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @HidingSpotCapacity'	+ ISNULL(CAST(@HidingSpotCapacity AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @LootableVillageCoins'	+ ISNULL(CAST(@LootableVillageCoins AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @villageTypePercentBonus'	+ ISNULL(CAST(@villageTypePercentBonus AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @researchPercentBonus'	+ ISNULL(CAST(@researchPercentBonus AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetVillageTypeID'	+ ISNULL(CAST(@TargetVillageTypeID AS VARCHAR(max)), 'Null') + CHAR(10)

		+ '   ERROR_NUMBER():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'	+ ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'	+ ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'		+ ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'		+  ERROR_MESSAGE() + CHAR(10)
		

	--
	-- IF deadlock, rerun
	--
	IF ERROR_NUMBER() = 1205 -- deadlock occured in this SP 
		OR (ERROR_NUMBER() = 50000 AND left(ERROR_MESSAGE(), 8) = 'DEADLOCK' ) -- deadlock occured in some called SP 
	BEGIN
		INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock', @ERROR_MSG)		
		WAITFOR DELAY '00:00:05'
		GOTO __RETRY_CACHETIMESTAMPS__
	END 						
		
	--
	-- we do NOT raise an error; we dont want this to fail
	--
	insert into ErrorLog values ( getdate(), 0, 'iProcessUnitMovement_Attack - UpdateCacheTimeStamps transaction failed', @ERROR_MSG)

END CATCH

--
-- say that the villages involved have changed. this is done deliberately outside of the main tran and try 
--
UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() -- update cache stamps for existing records
	from VillageCacheTimeStamps S join #ChangedVillages C
		on S.PlayerID = C.PlayerID
		and S.VillageID = C.VillageID
	where CachedItemID = 0

insert into VillageCacheTimeStamps -- insert cache stamps for records that do not exits
select C.PlayerID, C.VillageID,  0, getdate() from VillageCacheTimeStamps S 
		right join #ChangedVillages C
		on S.PlayerID = C.PlayerID
		and S.VillageID = C.VillageID
		where S.PlayerID is null

GO