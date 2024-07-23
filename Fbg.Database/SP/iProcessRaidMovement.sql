 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iProcessRaidMovement')
	BEGIN
		DROP  Procedure  iProcessRaidMovement
	END

GO



--Send out a raiding party!
CREATE Procedure [dbo].iProcessRaidMovement
	@EventID as int
AS

--additional declares

declare @playerID int --player ID of attacker
declare @villageID int --village ID of attack origin
declare @raidID int
declare @raidCurrentHealth int
declare @attackStrength int --the damage the attacking troops CAN do to raid monster
declare @damageDone int --the actual final damage done to monster
declare @damageDoneRatio float --the ratio of how much damage from the actual attack sent
declare @casultyPerc float --percantage of casulties from raidmonster (0 to 1)

--declare @AttackCasulties int --the # of troops lost upon attacking

--------
--------
--- MULTIPLE RAIDS LANDING AT THE SAEM TIME CAN GO WRONG
--- this SP is not multii thread safe
--- might be OK because EH is handling each event 1 at a time,
--- BUT STILL!
--------
--------


begin try 

	begin tran 	

	
	

		-- First we set the event as completed so that no one, in the mean time, can cancel it
		UPDATE Events SET [Status] = 1 WHERE EventID = @EventID AND [Status] = 0;

		--IF no rows where updated, then the event must have been cancelled (or something like this) therefore we do nothing
		IF @@rowcount <> 1 BEGIN
			RETURN
		END 

		--store Player ID, Village ID, and Raid ID
		select @playerID = PlayerID, @villageID = OriginVillageID, @raidID = RaidID from RaidUnitMovements where EventID = @EventID;

		--store Raid Monster Current Health
		select @raidCurrentHealth = M.CurrentHealth, @casultyPerc = (RMT.CasultyPerc / 100.00) from Raids R
		left join RaidMonster M on M.RaidID = @raidID
		left join RaidMonsterTemplate RMT on RMT.MonsterTemplateID = M.MonsterTemplateID
		where R.RaidID = @raidID;

		--	
		-- Obtain locks on villages
		--
		update VillageSemaphore set TimeStamp = getdate() 
			where VillageID = @villageID

		--if raid has any health, then we do the attack (and possibly incur casualties)
		IF @raidCurrentHealth > 0 BEGIN

			--calculate the dmg done
			select @attackStrength= sum(RUMing.UnitCount * UT.AttackStrength) from RaidUnitsMoving RUMing
			join UnitTypes UT
			on UT.UnitTypeID = RUMing.UnitTypeID and UT.AttackStrength > 0
			where RUMing.EventID = @EventID
			group by EventID

			--raid still has some health left
			IF(@raidCurrentHealth > @attackStrength) BEGIN
				set @damageDone = @attackStrength;
				set @raidCurrentHealth = @raidCurrentHealth - @damageDone;
			END
			
			--if attack was more than or equal to health left, raid will be beaten!
			--damage will be partial however, as it cant be more than there was health
			--casualties will be partial as well
			ELSE BEGIN
				--record actual damage done (without overkill)
				set @damageDone = @attackStrength + (@raidCurrentHealth - @attackStrength);
				set @raidCurrentHealth = 0; --set to 0 to make sure it doesnt show negative
				set @damageDoneRatio = cast(@damageDone as float) / cast(@attackStrength as float);
				set @casultyPerc = @damageDoneRatio * @casultyPerc;
				--THE RAID IS DEFEATED
				--we do raid defeat stuff here, if any
			END

			--update raid HP
			update RaidMonster set CurrentHealth = @raidCurrentHealth where RaidID = @raidID;
	
			--make a results entry (with actual damage done, not attack damage sent)
			insert into RaidResults values (@raidID, @playerID, GETDATE(), @damageDone);
	

			----put the troops back home (CURRENT COUNT) (incurr casualty)
			UPDATE VillageUnits 
			SET VillageUnits.CurrentCount = VillageUnits.CurrentCount + (RaidUnitsMoving.UnitCount - CEILING(RaidUnitsMoving.UnitCount * @casultyPerc))
			, VillageUnits.TotalCount = VillageUnits.TotalCount - (CEILING(RaidUnitsMoving.UnitCount * @casultyPerc))
			FROM RaidUnitsMoving, VillageUnits 
			WHERE RaidUnitsMoving.UnitTypeID = VillageUnits.UnitTypeID
			and RaidUnitsMoving.EventID = @EventID 
			and VillageUnits.VillageID = @villageID;
			
			------put the troops back home (TOTAL COUNT) (incurr casualty)
			--UPDATE VillageUnits 
			--SET VillageUnits.TotalCount = VillageUnits.TotalCount - (CEILING(RaidUnitsMoving.UnitCount * @casultyPerc))
			--FROM RaidUnitsMoving, VillageUnits 
			--WHERE RaidUnitsMoving.UnitTypeID = VillageUnits.UnitTypeID
			--and RaidUnitsMoving.EventID = @EventID 
			--and VillageUnits.VillageID = @villageID;


		END

		 --if raid was dead by the time we process this event, we just skip the fight part, and put units back home
		 --
		 -- CRITICAL!! Note that the system relies on this acting this way. Beause, when raid ends and user accepts the reward, 
		 --	all outgoing troops to this raid (that may be still outgoing) are set to process immediatelly using the event handler, so that they 
		 --	immediatelly return home. In such a case, they cannot take any casulties etc. 
		 --
		ELSE BEGIN

			--put the troops back home
			UPDATE VillageUnits 
			SET VillageUnits.CurrentCount = VillageUnits.CurrentCount + RaidUnitsMoving.UnitCount
			FROM RaidUnitsMoving, VillageUnits 
			WHERE RaidUnitsMoving.UnitTypeID = VillageUnits.UnitTypeID
			and RaidUnitsMoving.EventID = @EventID 
			and VillageUnits.VillageID = @villageID;

		END
	
	commit tran
	
end try

begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'iProcessRaidMovement FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @EventID' + ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @raidID' + ISNULL(CAST(@raidID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @attackStrength' + ISNULL(CAST(@attackStrength AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	


			
			
			

