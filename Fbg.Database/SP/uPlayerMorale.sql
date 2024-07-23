IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayerMorale')
	BEGIN
		DROP  Procedure  uPlayerMorale
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

--
-- The param @now tells the SP what is the autoritative time. 
--  if this is null, the sp get the time using GETDATE(). 
--
CREATE Procedure dbo.uPlayerMorale
	@PlayerID int,
	@now DateTime = null
AS


	/*
	* A few general comments
	*
	- this SP is not in transaction!
	*/

    declare @researchPercentBonus float
	declare @morale  int
	declare @lastUpdate datetime
	declare @seconds int
	declare @change int
	declare @NewMorale int
	declare @PerHourIncrease float
	declare @MaxMorale int
	declare @MinMorale float
	declare @researchTreasuryBonus float
	declare @bonusMoraleExpiresOn datetime

	set @change = 0 -- critical init. do not remove

	
	declare @morale_isactive as bit
	declare @morale_maxNormal as int


	--
	-- Morale system params
	--
	/*
				insert into RealmAttributes values (70, 1, 'Morale System - Is Morale system active on this realm ? 1 means yes') 
				insert into RealmAttributes values (71, 5, 'Morale System - amount to deduce for attacks on real players') 
				insert into RealmAttributes values (72, 3, 'Morale System - amount to deduce for attacks on NPC') 
				insert into RealmAttributes values (73, -50, 'Morale System - min morale') 
				insert into RealmAttributes values (74, 150, 'Morale System - max morale') 
				insert into RealmAttributes values (75, 100, 'Morale System - max normal morale') 
				insert into RealmAttributes values (76, 1, 'Morale System - min normal morale') 
				insert into RealmAttributes values (77, 60, 'Morale System - increase per hour') 
	*/
	select @morale_isactive = AttribValue from RealmAttributes where attribid = 70	
	select @MinMorale = AttribValue from RealmAttributes where attribid = 73		
	select @morale_maxNormal = AttribValue from RealmAttributes where attribid = 75 -- 'Morale System - max normal morale') 
	select @MaxMorale = AttribValue from RealmAttributes where attribid = 74 -- 'Morale System - max morale') 
	select @PerHourIncrease = AttribValue from RealmAttributes where attribid = 77 -- 'Morale System - increase per hour') 

	if @morale_isactive <> 1 BEGIN
		RETURN(@morale_maxNormal)
	END



    -- get research bonus. will be like 0.1 meaing 10%, which will turn to 1.1
    select @researchTreasuryBonus = sum(cast(PropertyValue as float))
        from ResearchItemPropertyTypes PT 
        join ResearchItemProperties P on P.ResearchItemPropertyID = PT.ResearchItemPropertyID
        join ResearchItems RI on RI.ResearchItemTypeID = P.ResearchItemTypeID AND RI.ResearchItemID = P.ResearchItemID
        join PlayerResearchItems PRI ON RI.ResearchItemTypeID = PRI.ResearchItemTypeID AND RI.ResearchItemID = PRI.ResearchItemID
        where PT.PropertyID = 1234567890 -- MAKE IT RETURN NOTHING FOR NOW!! 	
            AND PlayerID = @PlayerID
            AND PT.Type = 3		            
    SET @researchTreasuryBonus = isnull(@researchTreasuryBonus,0) + 1

	set @MaxMorale = Round(@MaxMorale * @researchTreasuryBonus, 0)


	
	IF @Now is null BEGIN 
	    set @now =  getdate()
	END
	
	select @morale= morale, @lastUpdate = moralelastupdated from Players where PlayerID = @PlayerID
	
	
    if @morale  < @MaxMorale begin
        --
        -- get research bonus
        --                
        select @researchPercentBonus = sum(cast(PropertyValue as float))
	        from ResearchItemPropertyTypes PT 
	        join ResearchItemProperties P on P.ResearchItemPropertyID = PT.ResearchItemPropertyID
	        join ResearchItems RI on RI.ResearchItemTypeID = P.ResearchItemTypeID AND RI.ResearchItemID = P.ResearchItemID
	        join PlayerResearchItems PRI ON RI.ResearchItemTypeID = PRI.ResearchItemTypeID AND RI.ResearchItemID = PRI.ResearchItemID
	        where PT.PropertyID = 1234567890 -- MAKE IT RETURN NOTHING FOR NOW!! 	
		        AND PlayerID = @playerID
		        AND PT.Type = 3 	
        SET @researchPercentBonus = isnull(@researchPercentBonus,0) + 1
        SET @PerHourIncrease = @PerHourIncrease * @researchPercentBonus
	   
	    --
	    -- Find out of if person has the bonus morale PF active now, or ever
	    --	
	    select @bonusMoraleExpiresOn = isnull(max(expiresOn), 'Jan 1 1900')
	        from PFs
	        join PFsInPackage on
		        PFsInPackage.FeatureID=PFs.FeatureID
	        join PFPackages on
		        PFPackages.PFPackageID=PFsInPackage.PFPackageID
	        join PlayersPFPackages on
		        PlayersPFPackages.PFPackageID=PFsInPackage.PFPackageID
	        where PlayerID=@PlayerID 
		        AND PFs.FeatureID = 1234567890 --MAKE IT RETURN NOTHING - DO NOT HAVE THIS PF Yet
	        group by PFs.FeatureID

        --
        -- If person HAD the morale PF active but it expired AND morale was last updated BEFORE the bonus PF expired
        --  then must do 2 calculations. 
        --
        IF @bonusMoraleExpiresOn < @now AND @lastUpdate < @bonusMoraleExpiresOn BEGIN 
            --
            -- CALC 1 - update morale for the duration: FROM (last coins updated) TO (PF expiry date)
            --
            set @change = 0
		    set @seconds = datediff(second, @lastUpdate, @bonusMoraleExpiresOn)
		    if @seconds > 0 begin
			    set @change = ((@PerHourIncrease * 1.25 ) / 3600)*@seconds 
		    end 
            --
            -- CALC 2 - update morale  for the duration: FROM (PF expiry date) TO (now)
            --
		    set @seconds = datediff(second, @bonusMoraleExpiresOn, @now)
		    if @seconds > 0 begin
			    set @change = @change + (@PerHourIncrease / 3600)*@seconds 
		    end 
		    --
		    -- now that we have the sum of both calcs in @change, then just update the coins 
		    --
            if @change > 0 begin
                set @NewMorale = @morale  + @change 
                if @NewMorale > @MaxMorale begin
	                set @NewMorale = @MaxMorale 
                end 
                update Players set MoraleLastUpdated=@now, morale = @NewMorale where Playerid = @PlayerID			
            end 
		    
        END ELSE BEGIN 
            --
            -- THIS ELSE handles 2 situtations:
            --
            --  (1) Bonus morale PF is active in which case we increase the income and update the morale accordingly
            --  (2) Bonus morale PF is not active in which case we update the morale normally
            --
        
            --
            -- If person has the Bonus Morale PF active now, then add the bonus to per hour income
            --
            IF @bonusMoraleExpiresOn >= @Now BEGIN
                SET @PerHourIncrease = @PerHourIncrease * 1.25
            END 
		
		    --
		    -- update the coins base on time passed and current per hour income. 
		    --
	        set @seconds = datediff(second, @lastUpdate, @now)

	        if @seconds > 0 begin
		        set @change = (@PerHourIncrease / 3600)*@seconds 
	            if @change > 0 begin
	                set @NewMorale = @morale  + @change 
	                if @NewMorale > @MaxMorale begin
		                set @NewMorale = @MaxMorale 
	                end 
	                update Players set MoraleLastUpdated=@now, morale = @NewMorale where Playerid = @PlayerID			
				                	
                end 
	        end 		
        END

	end


	if @NewMorale is null SET @NewMorale = @morale 

    RETURN(@NewMorale)

GO
