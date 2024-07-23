IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qVillageBuildingInfo')
	BEGIN
		DROP  Procedure  qVillageBuildingInfo
	END

GO

CREATE Procedure qVillageBuildingInfo
	@LoggedInPlayerID int
	, @VillageID int 
    , @GetTroopMovements bit = 0     
    , @GetAreTransportsAvailable bit = 0 
AS


begin try 
	--
	-- Get buildings in the village
	--
	select v.villageid, v.name, B.BuildingTypeID, B.Level from  Villages V 
		join Buildings B on B.VillageID = V.VillageID 
	where
		 V.VillageID = @VillageId
		 and  OwnerPlayerID = @LoggedInPlayerID
	IF @@rowcount = 0 BEGIN
		INSERT INTO ErrorLog VALUES (getdate(), 2, 'Attempt to access someone elses village:' + ISNULL(CAST(@VillageID AS VARCHAR(10)),'null' ), ISNULL(CAST(@LoggedInPlayerID AS VARCHAR(10)),'null' ))		
		RETURN
	END
	--
	-- Get building upgrades
	--
	select BU.EventID, E.EventTime, BU.BuildingTypeID, BU.Level  from BuildingUpgrades BU
		join Events E on BU.EventID = E.EventID
	where VillageID = @VillageId
	and status = 0

	SELECT QEntryID
		, BUQ.BuildingTypeID
		, BUQ.DateAdded -- for testing and debugging only 
		, ROW_NUMBER() OVER (PARTITION BY BUQ.BuildingTypeID order by BUQ.DateAdded ) AS 'RANK' -- for testing and debugging only 
		, isnull(B.Level, 0)  AS VillageBuildingLevel -- for testing and debugging only 
		, BU.Level	AS BuildingUpgradesLevel -- for testing and debugging only 
		, isnull( ROW_NUMBER() OVER (PARTITION BY BUQ.BuildingTypeID order by BUQ.DateAdded ) + isnull(BU.Level, isnull(B.Level, 0)), 1) as UpgradeLevel
		FROM BuildingUpgradeQEntries BUQ
		left JOIN Buildings B
			on BUQ.VillageID = B.VillageID
			and BUQ.BuildingTypeID = B.BuildingTypeID
		left join buildingupgrades BU
			on BU.VillageID = BUQ.VillageID
			and BUQ.BuildingTypeID = BU.BuildingTypeID
			and BU.EventID = (SELECT EventID FROM Events WHERE EventID = BU.EventID AND Status <> 1)
		WHERE BUQ.VillageID = @VillageId	
		ORDER BY DateAdded asc	
		
	--
	-- Get max population
	--
	select dbo.fnGetMaxPopulation(@VillageId)

	--
	-- Get the current population
	--
	declare @pop int
	set @pop = dbo.fnGetCurrentPopulation(@VillageID)
	select @pop

	--
	-- ensure VillageUnits table is up-to-date
	--
	begin tran 
		--
		-- Obtain a village LOCK
		--		this will ensure that no one else will change the buildings in the village, or effect the building Q, unit req etc
		--
		update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @VillageID

		exec iCompleteUnitRecruitment @villageId
	commit tran 
	--
	-- Get Units in the village
	--
	select UnitTypeID, TotalCount, CurrentCount from VillageUnits
	where
		VillageID = @villageId

	--
	-- Get Supporting Units in the village
	--
	select UnitTypeID, sum(UnitCount )
	from VillageSupportUnits
	where
		SupportedVillageID = @villageId
		and UnitCount >0
	group by UnitTypeID


	--
	-- Get units in the Q
	--
	-- NOTE - order by MUST BE first by BuildingTypeID as the code replies on that!
	select EntryID
		, UR.UnitTypeID
		, UR.BuildingTypeID
		, [Count]
		, [count] * UT.Population as TotalPopulation
		, UR.UnitCost 
		,  isnull(UR.DateLastUpdated, DateAdded)
		from UnitRecruitments UR
		join UnitTypes UT 
			on UT.UnitTypeId = UR.UnitTypeID
		where
			VillageID = @villageId
			AND status = 0 
		order by UR.BuildingTypeID, UR.DateAdded ASC
	--
	-- Incoming attacks
	--
	EXEC qIncomingTroops @LoggedInPlayerID, @VillageID, null,null,1, 1, 1, @GetTroopMovements
	--
	-- outgoing attacks
	--
	exec qOutgoingTroops @LoggedInPlayerID, null, null, @VillageID, 1, 1, @GetTroopMovements
	--
	-- get indicator if transports to this village are available
	--
	--	we've put the check for number of villages, when we have learned that this runs very very slow for players with many vilages 
	--		(we've tested with players with 4000 village, and this could run for good 4 seconds) 
	--		hence we put this optimizer; 
	--
	--	long term solution shoudl be that we cache the max transport capacity of a village, so that we do not need to calculate it each time; 
	--		opther potimizations may be needed
	--
	IF @GetAreTransportsAvailable = 1 and exists (select count(*) from villages with(nolock) where ownerplayerid = @LoggedInPlayerID having count(*) < 50) BEGIN
	    declare @retval int
	    exec qCoinTransports_AreTransportToVillageAvail @LoggedInPlayerID, @VillageID, @retval output
	END ELSE BEGIN 
	    select 1
	END
	--
	-- get basic info
	--
	select 
		v.Name
		, v.Coins
		, V.points as villagepoints
		, XCord
		, YCord 
		, cast(loyalty + floor(cast(datediff(minute, LoyaltyLastUpdated, getdate()) as real) /
			(60.0 / (select cast(AttribValue as real) FROM RealmAttributes where attribid =8))) as integer) 
		, V.CoinsLastUpdates
		, VillageTypeID
		from villages v 
	where VillageID = @VillageID
	--
	-- get the tags for this village
	--
	select TagID from villagetags where VillageID = @VillageID
	
	--
	-- get the building downgrades
	--
	select BU.EventID, E.EventTime, BU.BuildingTypeID  from BuildingDowngrades BU
		join Events E on BU.EventID = E.EventID
	where VillageID = @VillageId
	and status = 0

	SELECT QEntryID
		, BUQ.BuildingTypeID
		, BUQ.DateAdded -- for testing and debugging only 
--		, ROW_NUMBER() OVER (PARTITION BY BUQ.BuildingTypeID order by BUQ.DateAdded ) AS 'RANK' -- for testing and debugging only 
		FROM BuildingDowngradeQEntries BUQ
		WHERE BUQ.VillageID = @VillageId	
		ORDER BY DateAdded asc	
		
	declare @now datetime
	set @now = getdate()
	select isnull(sum(SpeedUpAmountInMin),0) from VillageSpeedUpUsage with(nolock)
		where 
		PlayerID = @LoggedInPlayerID
		and villageID = @VillageID
		and TimeOfSpeedup  > (cast(datename(year, @now) +'-'+ cast(month(@now)as varchar(2))+'-'+ datename( dd, @now) as datetime) )


end try
begin catch
	IF @@TRANCOUNT > 0 ROLLBACK

	DECLARE @ERROR_MSG AS VARCHAR(max)
	
	SET @ERROR_MSG = 'qVillageBuildingInfo FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @LoggedInPlayerID'+ ISNULL(CAST(@LoggedInPlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @VillageID'		+ ISNULL(CAST(@VillageID AS VARCHAR(100)), 'Null') + CHAR(10)

		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)

/*		
	--
	-- IF deadlock, rerun
	--
	IF ERROR_NUMBER() = 1205 -- deadlock occured in this SP 
		OR (ERROR_NUMBER() = 50000 AND left(ERROR_MESSAGE(), 8) = 'DEADLOCK' ) -- deadlock occured in some called SP 
	BEGIN
		INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock', @ERROR_MSG)		
		WAITFOR DELAY '00:00:05'
		GOTO __RETRY__
	END 
*/
		
	RAISERROR(@ERROR_MSG,11,1)	

end catch	


