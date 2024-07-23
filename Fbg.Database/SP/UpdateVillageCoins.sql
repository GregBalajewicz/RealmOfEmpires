IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'UpdateVillageCoins')
	BEGIN
		DROP  Procedure  UpdateVillageCoins
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

--
-- The param @now tells the SP what is the autoritative time. 
--  if this is null, the sp get the time using GETDATE(). 
--  this param is left as optional for only one reason - to support code that does not send this param but eventually all code should do this. 
--
CREATE Procedure dbo.UpdateVillageCoins
	@VillageID int,
	@now DateTime = null
AS




    declare @villageTypePercentBonus float
    declare @researchPercentBonus float
	declare @coins int
	declare @lastUpdate datetime
	declare @seconds int
	declare @change int
	declare @Newcoins int
	declare @PerHourIncome float
	declare @TrasurySize int
	declare @TrasurySizeFloat float
	declare @researchTreasuryBonus float
	declare @bonusSilverPFExpiresOn datetime
	declare @PlayerID int
	declare @VillageTypeID smallint

	declare @LevelProp_CoinMineProduction int 
	set @LevelProp_CoinMineProduction = 2

	declare @CoinMineID int 
	set @CoinMineID = 5

	declare @LevelProp_TreasuryCapacity int 
	set @LevelProp_TreasuryCapacity = 3

	declare @treasuryID int 
	set @treasuryID = 6
	
	set @change = 0 -- critical init. do not remove

	select @TrasurySizeFloat = PropertyValue from Buildings B
		join LevelProperties LP on LP.BuildingTypeID = @treasuryID and LP.Level = B.Level 
		where B.VillageID = @VillageID and B.BuildingTypeID = @treasuryID and PropertyID = @LevelProp_TreasuryCapacity

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

	select @PerHourIncome = PropertyValue from Buildings B
	join LevelProperties LP on LP.BuildingTypeID = @CoinMineID and LP.Level = B.Level 
	where B.VillageID = @VillageID and B.BuildingTypeID = @CoinMineID and PropertyID = @LevelProp_CoinMineProduction
	
	IF @Now is null BEGIN 
	    set @now =  getdate()
	END
	
	select @coins= coins, @lastUpdate = coinsLastUpdates, @PlayerID = OwnerPlayerID, @VillageTypeID = VillageTypeID from villages where villageid = @villageID
	
	
    if @coins < @TrasurySize begin
        --
        -- get research bonus
        --                
        select @researchPercentBonus = sum(cast(PropertyValue as float))
	        from ResearchItemPropertyTypes PT 
	        join ResearchItemProperties P on P.ResearchItemPropertyID = PT.ResearchItemPropertyID
	        join ResearchItems RI on RI.ResearchItemTypeID = P.ResearchItemTypeID AND RI.ResearchItemID = P.ResearchItemID
	        join PlayerResearchItems PRI ON RI.ResearchItemTypeID = PRI.ResearchItemTypeID AND RI.ResearchItemID = PRI.ResearchItemID
	        where PT.PropertyID = @LevelProp_CoinMineProduction
		        AND PlayerID = @playerID
		        AND PT.Type = 3 	
        SET @researchPercentBonus = isnull(@researchPercentBonus,0) + 1
        SET @PerHourIncome = @PerHourIncome * @researchPercentBonus
	    --
	    -- Get village Type bonus, if any
	    --
	    select @villageTypePercentBonus = sum(cast(PropertyValue as float)) 
	        from VillageTypeProperties VTP 
	        join VillageTypePropertyTypes VTPT on VTP.VillageTypePropertyTypeID = VTPT.VillageTypePropertyTypeID
	        where 
		        VTP.VillageTypeID = @VillageTypeID
		        and PropertyID = @LevelProp_CoinMineProduction
		        and type = 3
        SET @villageTypePercentBonus = isnull(@villageTypePercentBonus,0) + 1
        SET @PerHourIncome = @PerHourIncome * @villageTypePercentBonus
	    --
	    -- Find out of if person has the bonus silver PF active now, or ever
	    --	
	    select @bonusSilverPFExpiresOn = isnull(max(expiresOn), 'Jan 1 1900')
	        from PFs
	        join PFsInPackage on
		        PFsInPackage.FeatureID=PFs.FeatureID
	        join PFPackages on
		        PFPackages.PFPackageID=PFsInPackage.PFPackageID
	        join PlayersPFPackages on
		        PlayersPFPackages.PFPackageID=PFsInPackage.PFPackageID
	        where PlayerID=@PlayerID 
		        AND PFs.FeatureID = 24 -- == Bonus Silver Premium Feature
	        group by PFs.FeatureID

        --
        -- If person HAD the Bonus Silver PF active but it expired AND silver was last updated BEFORE the bonus silver PF expired
        --  then must do 2 calculations. 
        --
        IF @bonusSilverPFExpiresOn < @now AND @lastUpdate < @bonusSilverPFExpiresOn BEGIN 
            --
            -- CALC 1 - update coins for the duration: FROM (last coins updated) TO (PF expiry date)
            --
            set @change = 0
		    set @seconds = datediff(second, @lastUpdate, @bonusSilverPFExpiresOn)
		    if @seconds > 0 begin
			    set @change = ((@PerHourIncome * 1.25 ) / 3600)*@seconds 
		    end 
            --
            -- CALC 2 - update coins for the duration: FROM (PF expiry date) TO (now)
            --
		    set @seconds = datediff(second, @bonusSilverPFExpiresOn, @now)
		    if @seconds > 0 begin
			    set @change = @change + (@PerHourIncome / 3600)*@seconds 
		    end 
		    --
		    -- now that we have the sum of both calcs in @change, then just update the coins 
		    --
            if @change > 0 begin
                set @Newcoins = @coins + @change 
                if @Newcoins > @TrasurySize begin
	                set @Newcoins = @TrasurySize 
                end 
                update villages set coinsLastUpdates=@now, coins = @Newcoins where villageid = @villageID			
            end 
		    
        END ELSE BEGIN 
            --
            -- THIS ELSE handles 2 situtations:
            --
            --  (1) Bonus Silver PF is active in which case we increase the income and update the coins accordingly
            --  (2) Bonus Silver PF is not active in which case we update the coins normally
            --
        
            --
            -- If person has the Bonus Silver PF active now, then add the bonus to per hour income
            --
            IF @bonusSilverPFExpiresOn >= @Now BEGIN
                SET @PerHourIncome = @PerHourIncome * 1.25
            END 
		
		    --
		    -- update the coins base on time passed and current per hour income. 
		    --
	        set @seconds = datediff(second, @lastUpdate, @now)

	        if @seconds > 0 begin
		        set @change = (@PerHourIncome / 3600)*@seconds 
	            if @change > 0 begin
	                set @Newcoins = @coins + @change 
	                if @Newcoins > @TrasurySize begin
		                set @Newcoins = @TrasurySize 
	                end 
	                update villages set coinsLastUpdates=@now, coins = @Newcoins where villageid = @villageID			
                end 
	        end 		
        END

	end

	if @Newcoins is null SET @Newcoins = @coins

    RETURN(@Newcoins)

GO
