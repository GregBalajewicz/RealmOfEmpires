 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iSendRaid')
	BEGIN
		DROP  Procedure  iSendRaid
	END

GO

--Send out a raiding party!
CREATE Procedure [dbo].iSendRaid
	@playerID int,
	@villageID int,
	@raidID int,
	@unitTypes varchar(100),
	@landTime datetime,
	@resultCode int output 
AS

--additional declares
declare @UnitTypeID int;
declare @UnitTypes_CommaLoc int;
declare @UnitTypes_CurLoc int;
declare @EventID int;


-- Temp table to hold the units to be sent. 
create table #temp 
(
	UnitTypeID int,
	UnitSendCount int
)



set @UnitTypes_CurLoc = 1;
set @UnitTypes_CommaLoc = charindex( ',', @UnitTypes);


while (@UnitTypes_CommaLoc > 0) BEGIN


	-- read the unit type ID 
	set @UnitTypeID	= substring(@UnitTypes, @UnitTypes_CurLoc , @UnitTypes_CommaLoc -  @UnitTypes_CurLoc);

	-- Update CurLoc
	set @UnitTypes_CurLoc = @UnitTypes_CommaLoc + 1;

	-- get next comma location
	set @UnitTypes_CommaLoc = charindex(',', @UnitTypes, @UnitTypes_CurLoc)

    -- remember this unit
	insert into #temp values (@UnitTypeID, 0)


END --end of while


begin try 

BEGIN TRAN 
	--
	-- Obtain a village LOCK
	--		this will ensure that no one else will change anything in the village, or effect the upgrade Q 
	--
	
	--commented out for now
	update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @villageID

	exec iCompleteUnitRecruitment @villageID

	--Put village current count of units into the temp table
	--were gonna send all of the units found, fo the types we are sending
	update T 
	set T.UnitSendCount = UV.CurrentCount 
	from #temp T
	left join VillageUnits UV
	on T.UnitTypeID = UV.UnitTypeID and UV.VillageID = @villageID


	--
	-- if sending a 0 amount of a unit, village didnt have it, something is wrong	
	IF EXISTS(select * from #temp T where T.UnitSendCount is null or T.UnitSendCount < 1) 
	BEGIN
		SET @resultCode = 1; --code one means missing unit problem
		GOTO DONE
	END
	

	--
	-- Finally do the thing - insert the necessary stuff to DB to execute the command
	--
	insert into Events (EventTime) values ( @landTime )	
	set @EventID = SCOPE_IDENTITY() 

	insert into RaidUnitMovements values (@EventID, @playerID, @villageID, @raidID, GETDATE(), @landTime)
	insert into RaidUnitsMoving select @EventID, T.UnitTypeID, T.UnitSendCount from #temp T
	
	--update the units count in village
	update VillageUnits set CurrentCount = CurrentCount - T.UnitSendCOunt
		from VillageUnits UV 
		join #temp T 
			on T.UnitTypeID = UV.UnitTypeID
			and UV.VillageID = @villageID

	-- say that troops in the village of origin have changed
	UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @playerID and VillageID = @villageID and CachedItemID = 0
	IF (@@rowcount < 1 ) BEGIN
		INSERT INTO VillageCacheTimeStamps values (@playerID, @villageID, 0, getdate())
	END
	
	set @resultCode = 0; --code 0 is all OK

DONE:
COMMIT TRAN 

end try

begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'iSendRaid FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @EventID' + ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @villageID' + ISNULL(CAST(@villageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @raidID' + ISNULL(CAST(@raidID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @unitTypes' + ISNULL(CAST(@unitTypes AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @landTime' + ISNULL(CAST(@landTime AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	











