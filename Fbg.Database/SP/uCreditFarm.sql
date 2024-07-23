
---THIS SP IS MEANT TO BE CALLED FROM ATTACK SP, NEEDS #TargetUnits and #AttackerUnits TEMP Tables


IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uCreditFarm')
	BEGIN
		DROP  Procedure  uCreditFarm
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--
-- Credit Farm
--
CREATE Procedure [dbo].uCreditFarm
		@AttackingPlayerID as int,
		@targetVillageID as int,
		@creditFarmThisAttack as int output
AS
	
begin try 			
		
		declare @AttackerUserID uniqueidentifier; --userID of the Player that is attacking (farming)
		declare @creditFarmFlagDate datetime; --date of last successful farm
		declare @creditFarmFlagData int; --how many credits user has farmed today
		declare @farmDate datetime; --now
		declare @creditFarmMaxDaily int; --limit of how many credits a user can farm per day
		declare @creditFarmAllowedToday bit; --can player farm more today?
				
		set @farmDate = getdate();
		set @creditFarmMaxDaily = 35;
		set @creditFarmAllowedToday = 0;
			
		SELECT @AttackerUserID = UserID FROM Players where PlayerID = @AttackingPlayerID;
		SELECT @creditFarmFlagDate = UpdatedOn, @creditFarmFlagData = Data from FBGCommon.dbo.UserFlags where UserId = @AttackerUserID and FlagID = 20;
			
		--if we have a flag date, and it is today, we look at the data, otherwise reset it
		IF ( (@creditFarmFlagDate IS NOT NULL) AND (CONVERT(date, @farmDate) = CONVERT(date, @creditFarmFlagDate)))BEGIN	
 			IF ( @creditFarmFlagData < @creditFarmMaxDaily) 
				BEGIN
					set @creditFarmAllowedToday = 1;
				END 
			ELSE
				BEGIN
					set @creditFarmAllowedToday = 0;
				END 		 
		END 
		ELSE
			BEGIN
				set @creditFarmFlagDate = @farmDate;
				set @creditFarmFlagData = 0;
				set @creditFarmAllowedToday = 1;
			END 
		
		--set expected food cap based on target village points
		declare @targetVillagePoints real;
		declare @targetTroopFoodCap real;
		declare @TargetVillageXCord int;
		declare @TargetVillageYCord int;
		select @targetVillagePoints = V.Points, @TargetVillageXCord = V.XCord, @TargetVillageYCord = V.YCord from Villages V where V.VillageID = @targetVillageID;


		SET @targetTroopFoodCap = @targetVillagePoints  * 0.03
		IF (@targetTroopFoodCap < 15)		BEGIN 
			set @targetTroopFoodCap = 15; 
		END ELSE IF (@targetTroopFoodCap  > 30) BEGIN 
			set @targetTroopFoodCap = 30; 
		END
		set @targetTroopFoodCap = FLOOR(@targetTroopFoodCap)
		
		IF (@creditFarmAllowedToday = 1)BEGIN
					
			declare @targetTroopFoodNow real;
			declare @targetTroopFoodRatio real;
			declare @todayChance real;
			declare @todayChanceWithModifier real;
			declare @totalFarmChance real;
			declare @randy real;
			declare @farmDataUpdated int;
			declare @creditFarmFlagConverted real;
			declare @PlayerFoodChanceModifier real;
			declare @PlayerFoodChanceModifierFactor real 
			declare @PlayerTodayChanceModifier real
			declare @PreviousFailedAttacks int  
			declare @ExpectedNumOfAttacksToGetServant int  
			declare @PowOrExp int  


			
			set @randy = RAND();
			set @creditFarmFlagConverted = cast(@creditFarmFlagData as real);
						
			Select @targetTroopFoodNow = sum(UT.Population * T.DeployedUnitCount) from #TargetUnits T JOIN UnitTypes UT on T.UnitTypeID = UT.UnitTypeID;
			
			-- set troop ratio portion of total farm chance. in case 30769, this is the FC param. 
			--		@targetTroopFoodRatio always betwen 0 and 1
			set @targetTroopFoodRatio = @targetTroopFoodNow / @targetTroopFoodCap
			if (@targetTroopFoodRatio > 1 ) BEGIN SET @targetTroopFoodRatio = 1 END 
			
			--set today farm chance portion of total farm chance, based on how many successful farms today AND we add to it, player's modifier
			-- in case 30769, this is the TC param; BUT, se must also add to it the today chance player modifier
			--		@todayChance always betwen 0 and 1
			select @todayChance = TodaysChance from CreditFarm_TodaysChance where CreditsFarmedToday = @creditFarmFlagConverted + 1
			set @todayChance = ISNULL(@todayChance, 0.001)

			-- get the number of attack, that player is supposed to take, to get a servants given his current today chance
			SET @ExpectedNumOfAttacksToGetServant = CEILING( 1.0 / @todayChance )

			-- get number of previous failed attacks
			select @PreviousFailedAttacks = isnull(modifierFactor,0) 
					from CreditFarm_PlayerDailyChanceModifierFactor TCM
					where TCM.PlayerID = @AttackingPlayerID
			set @PreviousFailedAttacks = ISNULL(@PreviousFailedAttacks, 0)

			--
			-- calculate the today chance bonus / modifier based on https://roe.fogbugz.com/f/cases/31278/fix-servant-farming-servant-rescue-daily-chance-player-bonus
			--
			SET @todayChanceWithModifier =  @todayChance 
			--
			-- if today's chance is < 0.25 or num of attacks exceeded expected 
			if @ExpectedNumOfAttacksToGetServant > 4 OR @PreviousFailedAttacks >= @ExpectedNumOfAttacksToGetServant BEGIN 
				if (@PreviousFailedAttacks >= @ExpectedNumOfAttacksToGetServant) BEGIN 
					SET @PowOrExp = 1 -- we must do this or otherwise we risk getting overflow errors
				END ELSE BEGIN 
					SET @PowOrExp = @PreviousFailedAttacks - @ExpectedNumOfAttacksToGetServant
				END
				if @ExpectedNumOfAttacksToGetServant > 20 BEGIN 
					SET @PlayerTodayChanceModifier = exp (0.2 *( @PowOrExp ) )  -- this algorithm is a bit better for smaller todaychance. see case 31287
				END ELSE BEGIN 
					SET @PlayerTodayChanceModifier = power (1.7, ( @PowOrExp ) ) -- this algorithm is a bit better for larder todaychance. see case 31287
				END 

				-- in case 30769, this is the TC param; which now includes today chance player modifier
				--		@todayChanceWithModifier always betwen 0 and 1
				if (@todayChance > @PlayerTodayChanceModifier ) BEGIN 
					set @todayChanceWithModifier =  @todayChance 
				END ELSE BEGIN
					set @todayChanceWithModifier =  @PlayerTodayChanceModifier
				END
			END 

			-- in case 30769, this is the PFCM param. 
			select @PlayerFoodChanceModifierFactor = modifierFactor from CreditFarm_PlayerFoodChanceModifierFactor  where playerid = @AttackingPlayerID
			set @PlayerFoodChanceModifierFactor  = isnull(@PlayerFoodChanceModifierFactor, 0)
			set @PlayerFoodChanceModifier = (1 - @targetTroopFoodRatio) * @PlayerFoodChanceModifierFactor
			
			-- set the actual chance that you get servants out of this attack. in case 30769, this is the TotC param
			--		@totalFarmChance always betwen 0 and 1
			--		(case 30769) formula is TotC = TC * (FC + PFCM); (FC + PFCM) should always be between 0 and 1
			set @totalFarmChance = @todayChanceWithModifier * (@targetTroopFoodRatio + @PlayerFoodChanceModifier);
				
			--if player wins the roll
			IF(@totalFarmChance > @randy)BEGIN
				--set and correct if flag data is above limit
				set @creditFarmThisAttack = 1;
				set @farmDataUpdated = @creditFarmFlagData + @creditFarmThisAttack;
				IF(@farmDataUpdated > @creditFarmMaxDaily)BEGIN
					set @farmDataUpdated = @creditFarmMaxDaily;
				END
							
							
				--credit farm bonus events
				declare @bonusUntillDate datetime;
				declare @bonusMultiplier int;
				SET @bonusMultiplier = 1;
								
				SELECT @bonusUntillDate = AttribValue from RealmAttributes Where AttribID = 57;		
				IF ( (@bonusUntillDate IS NOT NULL) AND @bonusUntillDate > GETDATE()) BEGIN	
		 			SELECT @bonusMultiplier = AttribValue from RealmAttributes Where AttribID = 58;
					SET @bonusMultiplier = isnull(@bonusMultiplier, 1);
				END 
							
				--add a player map event				
				insert into PlayerMapEvents values (@AttackingPlayerID, 1, @bonusMultiplier, @farmDate, @TargetVillageXCord,@TargetVillageYCord, 1);				
				
				--update User's Farm Flag, if flag doesnt exist, insert. Thread safe method.
				UPDATE FBGCommon.dbo.UserFlags set UpdatedOn  = @farmDate, Data= @farmDataUpdated where UserId = @attackerUserId AND FlagID = 20;
				IF @@ROWCOUNT = 0 BEGIN
					INSERT into FBGCommon.dbo.UserFlags select top 1 @attackerUserId, 20, @farmDate, @farmDataUpdated from FBGCommon.dbo.UserFlags
					where not exists (select * from FBGCommon.dbo.UserFlags where UserId = @attackerUserId AND FlagID = 20);
				END

				-- decrease the food bonus for next time
				update CreditFarm_PlayerFoodChanceModifierFactor set modifierFactor = modifierFactor - (modifierFactor * 0.1) where playerid = @AttackingPlayerID 
				-- reset any daily chance modifier
				update CreditFarm_PlayerDailyChanceModifierFactor set modifierFactor = 0 where playerid = @AttackingPlayerID 
			END ELSE BEGIN 
				--
				-- attack not successful, give the player a chance bonus for next time. 
				--	but only if they attack food chance was very high 
				--
				if (@targetTroopFoodRatio + @PlayerFoodChanceModifier) >=0.8 BEGIN
					update CreditFarm_PlayerDailyChanceModifierFactor set modifierFactor = modifierFactor + 1  where playerid = @AttackingPlayerID
					IF @@ROWCOUNT = 0 BEGIN
						INSERT into CreditFarm_PlayerDailyChanceModifierFactor values (@AttackingPlayerID, 1)
					END
				end 
			END	
			
			--local log table
			declare @attackerStrength int;
			declare @attackerLoss int;
			--select @attackerFood = sum(UT.Population * T.DeployedUnitCount) from #AttackerUnits T JOIN UnitTypes UT on T.UnitTypeID = UT.UnitTypeID;			
			select @attackerStrength = sum( A.DeployedUnitCount * UT.AttackStrength) from #AttackerUnits A JOIN UnitTypes UT on A.UnitTypeID = UT.UnitTypeID; 				
			select @attackerLoss = KilledUnitCount from #AttackerUnits;
			
			insert into CreditFarmLogTable values ( 
				@AttackingPlayerID, 
				@attackerStrength,
				@attackerLoss,
				@creditFarmThisAttack,
				@creditFarmFlagData,
				@creditFarmMaxDaily,
				@todayChance,
				@targetTroopFoodNow,
				@targetTroopFoodCap,
				@targetTroopFoodRatio,
				@totalFarmChance,
				@randy,   
				@farmDate,
				@targetVillageID,
				@targetVillagePoints,
				@TargetVillageXCord,
				@TargetVillageYCord,
				@PlayerFoodChanceModifier,
				@PlayerTodayChanceModifier
			)
									
		END

		--Since we are here, it means the village forces were wiped, so we need to recruit some
		--cancel all current recruitment, and initiate recruitment back to @targetTroopFoodCap, this prevents stacked recruitment
		update UnitRecruitments set Status = 1 where VillageID = @targetVillageID and Status = 0;
		EXEC iRecruitUnits @targetVillageID, 11, @targetTroopFoodCap, 1, 0, 0, 0;
		

end try

begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'uCreditFarm FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @AttackingPlayerID' + ISNULL(CAST(@AttackingPlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @attackerStrength' + ISNULL(CAST(@attackerStrength AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @attackerLoss' + ISNULL(CAST(@attackerLoss AS VARCHAR(20)), 'Null') + CHAR(10)
		
		+ '   @targetVillageID' + ISNULL(CAST(@targetVillageID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @targetVillagePoints' + ISNULL(CAST(@targetVillagePoints AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @targetTroopFoodCap' + ISNULL(CAST(@targetTroopFoodCap AS VARCHAR(20)), 'Null') + CHAR(10)
			
		+ '   @totalFarmChance' + ISNULL(CAST(@totalFarmChance AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @creditFarmFlagData' + ISNULL(CAST(@creditFarmFlagData AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @creditFarmMaxDaily' + ISNULL(CAST(@creditFarmMaxDaily AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @creditFarmAllowedToday' + ISNULL(CAST(@creditFarmAllowedToday AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @creditFarmThisAttack' + ISNULL(CAST(@creditFarmThisAttack AS VARCHAR(20)), 'Null') + CHAR(10)

		+ '   @PlayerFoodChanceModifier' + ISNULL(CAST(@PlayerFoodChanceModifier AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PreviousFailedAttacks' + ISNULL(CAST(@PreviousFailedAttacks AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @ExpectedNumOfAttacksToGetServant' + ISNULL(CAST(@ExpectedNumOfAttacksToGetServant AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PlayerTodayChanceModifier' + ISNULL(CAST(@PlayerTodayChanceModifier AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @todayChance' + ISNULL(CAST(@todayChance AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @@todayChanceWithModifier' + ISNULL(CAST(@todayChanceWithModifier AS VARCHAR(20)), 'Null') + CHAR(10)

		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	
	RAISERROR(@ERROR_MSG,11,1)	
	return	
			
end catch	


   