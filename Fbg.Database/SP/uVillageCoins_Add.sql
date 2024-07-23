IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uVillageCoins_Add')
	BEGIN
		DROP  Procedure  uVillageCoins_Add
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


CREATE Procedure dbo.uVillageCoins_Add
	@VillageID int
	,@CoinsToAdd int
	,@CoinsOverflow int out	
	, @PrintDebugInfo BIT = null
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
IF @DEBUG = 1 print @DBG + 'BEGIN uVillageCoins_Add ' 

declare @VillageCoins int
declare @NewVillageCoins int
declare @TrasurySize int
declare @TrasurySizeFloat float
declare @TreasuryCurrentCoinDiff int

declare @LevelProp_TreasuryCapacity int 
set @LevelProp_TreasuryCapacity = 3

declare @treasuryID int 
set @treasuryID = 6

begin try 

	--
	-- get trasury size
	-- 
	select @TrasurySizeFloat = PropertyValue from Buildings B
		join LevelProperties LP on LP.BuildingTypeID = @treasuryID and LP.Level = B.Level 
		where B.VillageID = @VillageID and B.BuildingTypeID = @treasuryID and PropertyID = @LevelProp_TreasuryCapacity

    declare @researchTreasuryBonus float
    -- get research bonus. will be like 0.1 meaing 10%, which will turn to 1.1
    select @researchTreasuryBonus = sum(cast(PropertyValue as float))
        from ResearchItemPropertyTypes PT 
        join ResearchItemProperties P on P.ResearchItemPropertyID = PT.ResearchItemPropertyID
        join ResearchItems RI on RI.ResearchItemTypeID = P.ResearchItemTypeID AND RI.ResearchItemID = P.ResearchItemID
        join PlayerResearchItems PRI ON RI.ResearchItemTypeID = PRI.ResearchItemTypeID AND RI.ResearchItemID = PRI.ResearchItemID
        where PT.PropertyID = @LevelProp_TreasuryCapacity
            AND PlayerID = (select OwnerPlayerID from Villages where VillageID = @VillageID)
            AND PT.Type = 3 			            
    SET @researchTreasuryBonus = isnull(@researchTreasuryBonus,0) + 1

	set @TrasurySize = Round(@TrasurySizeFloat * @researchTreasuryBonus, 0)

	IF @DEBUG = 1 print '@TrasurySizeFloat=' + cast(@TrasurySize  as varchar)

	-- Update village coins, & get current
	exec @VillageCoins = UpdateVillageCoins @VillageID
	IF @DEBUG = 1 print '@VillageCoins=' + cast(@VillageCoins  as varchar)

	--
	-- 
	set @TreasuryCurrentCoinDiff = @TrasurySize - @VillageCoins
	IF @DEBUG = 1 print '@TreasuryCurrentCoinDiff=' + cast(@TreasuryCurrentCoinDiff  as varchar)

	IF @CoinsToAdd > @TreasuryCurrentCoinDiff BEGIN
		set @CoinsOverflow =  @CoinsToAdd - @TreasuryCurrentCoinDiff 
		set @NewVillageCoins = @TrasurySize
	END ELSE BEGIN
		set @CoinsOverflow =  0
		set @NewVillageCoins = @VillageCoins + @CoinsToAdd
	END 
	IF @DEBUG = 1 print '@CoinsOverflow=' + cast(@CoinsOverflow  as varchar)
	IF @DEBUG = 1 print '@NewVillageCoins=' + cast(@NewVillageCoins  as varchar)

	Update Villages set coins = @NewVillageCoins, coinslastupdates = getdate() where VillageID = @VillageID

	IF @DEBUG = 1 print @DBG + 'END uVillageCoins_Add ' 
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max)

	SET @ERROR_MSG = 'uVillageCoins_Add FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CoinsToAdd' + ISNULL(CAST(@CoinsToAdd AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CoinsOverflow' + ISNULL(CAST(@CoinsOverflow AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @VillageCoins' + ISNULL(CAST(@VillageCoins AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @NewVillageCoins' + ISNULL(CAST(@NewVillageCoins AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @TrasurySize' + ISNULL(CAST(@TrasurySize AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @TrasurySizeFloat' + ISNULL(CAST(@TrasurySizeFloat AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @TreasuryCurrentCoinDiff' + ISNULL(CAST(@TreasuryCurrentCoinDiff AS VARCHAR(30)), 'Null') + CHAR(10)
		
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














