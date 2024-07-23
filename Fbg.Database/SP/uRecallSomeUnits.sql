IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uRecallSomeUnits')
	BEGIN
		DROP  Procedure  uRecallSomeUnits
	END

GO

CREATE Procedure [dbo].[uRecallSomeUnits]
	@SupportingPlayerID int -- for security. set this or @SupportedPlayerID to not  null
	,@SupportedPlayerID int -- for security. set this or @SupportingPlayerID to not  null
	,@SupportingVillageID int
	,@SupportedVillageID int
	,@ReturnUnitTypes varchar(100) --type of units that will be recalled
	,@ReturnUnitAmounts varchar(100) --# of units that will be recalled
	,@PrintDebugInfo BIT =null 
AS
declare @EventID int
declare @TripDurationInTicks Bigint
declare	@originX int 
declare	@originY int 
declare	@destinationX int 
declare	@destinationY int 
declare @UnitSpeed int
declare @ReportID bigint
declare @TravelTimeBonusMultipier real -- if supporting player has this bonus PF, then 0.1, otherwise 1. 
--- part for parsing units

declare @UnitTypeID int
declare @UnitAmount int
declare @UnitTypes_CommaLoc int
declare @UnitTypes_CurLoc int
declare @UnitAmounts_CommaLoc int
declare @UnitAmounts_CurLoc int

---
declare @ForReport_SupportingPlayerID int
declare @ForReport_SupportedPlayerID int
declare @ForReport_SupportingPlayerName varchar(max)
declare @ForReport_SupportedPlayerName varchar(max)
declare @ForReport_SupportingVillageName varchar(max)
declare @ForReport_SupportedVillageName varchar(max)
declare @For_Report_SupportingTroops varchar(max)
DECLARE @CONST_SupportSentBack int
DECLARE @CONST_SupportPulledBack int
SET @CONST_SupportSentBack = 7
SET @CONST_SupportPulledBack = 8

DECLARE @DEBUG INT, @DBG varchar(10)
IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
	SET @DEBUG = 1
	set @DBG = ':::::::'
	SET NOCOUNT OFF
END ELSE BEGIN
	SET @DEBUG = 0
	SET NOCOUNT ON
END 
IF @DEBUG = 1 print @DBG + 'BEGIN uRecallUnits ' 


BEGIN TRY
-------------------------------------------------------------
	--
	-- validate preconditions
	--
	IF ( @SupportingPlayerID is null and  @SupportedPlayerID is null) BEGIN
		RAISERROR('both @SupportingPlayerID and @SupportedPlayerID are null',11,1)	
	END
	
	--	
	-- Temp table to hold the units to be sent. 
	create table #temp 
	(
		UnitTypeID int,
		UnitAmount int
	)
	-- -----recall Type= all
	if (@ReturnUnitTypes is null or @ReturnUnitAmounts is null)begin
			insert into 	#temp	
			select  VSU.UnitTypeID, VSU.UnitCount
			from VillageSupportUnits VSU
			where 
				VSU.SupportedVillageID = @SupportedVillageID
				and VSU.SupportingVillageID = @SupportingVillageID
				and VSU.UnitCount > 0
				and ( VSU.SupportingVillageID in (select VillageID from Villages where OwnerPlayerID = @SupportingPlayerID) OR @SupportingPlayerID is null) -- security check
				and ( VSU.SupportedVillageID in (select VillageID from Villages where OwnerPlayerID = @SupportedPlayerID) OR @SupportedPlayerID is null)-- security check
				-- this part to handle report text depend on recall type
				set @For_Report_SupportingTroops=dbo.Translate('uRSU_pulledAll') ;
		goto __DoRecall
	end
	
	-- -----recall Type= Some
	--
	-- Validate preconditions - last char must be ','
	--
	IF substring(@ReturnUnitTypes, len(@ReturnUnitTypes),1) <> ',' BEGIN
		RAISERROR('@ReturnUnitTypes must end with a comma',11,1)	
	END 
	IF substring(@ReturnUnitAmounts, len(@ReturnUnitAmounts),1) <> ',' BEGIN
		RAISERROR('@ReturnUnitAmount must end with a comma',11,1)	
	END
	
	IF @DEBUG = 1 print @DBG + '@UnitTypes:' + @ReturnUnitTypes 
	IF @DEBUG = 1 print @DBG + '@UnitAmounts:' + @ReturnUnitAmounts

	set @UnitTypes_CurLoc			= 1
	set @UnitAmounts_CurLoc			= 1

	set @UnitTypes_CommaLoc = charindex( ',', @ReturnUnitTypes)
	IF @DEBUG = 1 print @DBG + '@UnitTypes_CommaLoc:' + cast(@UnitTypes_CommaLoc as varchar(10))

	while (@UnitTypes_CommaLoc > 0) BEGIN
		IF @DEBUG = 1 print @DBG + 'WHILE'
		-- 
		-- read the unit type ID 
		--
		set @UnitTypeID	= substring(@ReturnUnitTypes, @UnitTypes_CurLoc ,@UnitTypes_CommaLoc -  @UnitTypes_CurLoc)
		IF @DEBUG = 1 print @DBG + '  *@UnitTypeID:' + cast(@UnitTypeID as varchar(10))
		--
		-- read the unit amount
		--
		set @UnitAmounts_CommaLoc = charindex( ',', @ReturnUnitAmounts, @UnitAmounts_CurLoc)
		IF @DEBUG = 1 print @DBG + '  @UnitAmounts_CommaLoc:' + cast(@UnitAmounts_CommaLoc as varchar(10))

		IF @UnitAmounts_CommaLoc is null OR @UnitAmounts_CommaLoc <= 0 BEGIN
			RAISERROR('Got different number of unitTypeIDs in @UnitTypes and unit amounts in @UnitAmounts. Check the commas',11,1)	
		END

		set @UnitAmount	= substring(@ReturnUnitAmounts, @UnitAmounts_CurLoc ,@UnitAmounts_CommaLoc -  @UnitAmounts_CurLoc)
		IF @DEBUG = 1 print @DBG + '  *@UnitAmount:' + cast(@UnitAmount as varchar(10))
		

		--
		-- Update CurLoc
		set @UnitTypes_CurLoc = @UnitTypes_CommaLoc + 1
		set @UnitAmounts_CurLoc = @UnitAmounts_CommaLoc + 1
		
		IF @DEBUG = 1 print @DBG + '  @UnitTypes_CurLoc :' + cast(@UnitTypes_CurLoc as varchar(10))
		IF @DEBUG = 1 print @DBG + '  @UnitAmounts_CurLoc :' + cast(@UnitAmounts_CurLoc as varchar(10))

		-- get next comma location
		set @UnitTypes_CommaLoc = charindex(',', @ReturnUnitTypes,@UnitTypes_CurLoc)
		IF @DEBUG = 1 print @DBG + '  @UnitTypes_CommaLoc:' + cast(@UnitTypes_CommaLoc as varchar(10))

		insert into #temp values (@UnitTypeID, @UnitAmount)
		IF @DEBUG = 1 print @DBG + 'END (while)'
	END
	-- this part to handle report text depend on recall type
	set @For_Report_SupportingTroops=dbo.Translate('uRSU_pulledSome') ;
	
	
	-- this part don't depend on recall type its common for both types
	__DoRecall:

	IF @DEBUG = 1 select * from #temp T 


--------------------------------------------------
	IF @DEBUG = 1 select * from VillageSupportUnits where SupportedVillageID = @SupportedVillageID and SupportingVillageID = @SupportingVillageID

    --
    -- does the player 
    --
	SET @TravelTimeBonusMultipier = 1 -- default; no bonus since X * 1 = X
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
		            PlayerID= (select ownerplayerID from Villages where VillageID = @SupportingVillageID )
	            AND PFs.FeatureID = 32 -- DEFENCE BONUS
	            AND ExpiresOn>getdate()	
	    )
	BEGIN
	    SET @TravelTimeBonusMultipier = 0.1	--10 times bonus
	END

	--
	-- calculate the travel time.
	--	first get the slowest unit & village cordinates, then calc the time
	-- 
	select @originX = OV.Xcord
		,@originY = OV.Ycord
		,@destinationX = DV.Xcord
		,@destinationY = DV.Ycord
		
		, @ForReport_SupportingVillageName = OV.Name
		, @ForReport_SupportingPlayerID = OV.OwnerPlayerID
		
		, @ForReport_SupportedVillageName = DV.Name
		, @ForReport_SupportedPlayerID = DV.OwnerPlayerID
	from VillageSupportUnits VSU
	join Villages OV
		on OV.VillageID = VSU.SupportingVillageID
	join Villages DV
		on DV.VillageID = VSU.SupportedVillageID
	where 
		VSU.SupportedVillageID = @SupportedVillageID
		and VSU.SupportingVillageID = @SupportingVillageID

	IF @DEBUG = 1 print @DBG + '  @originX:' + cast(@originX AS VARCHAR(max))
	IF @DEBUG = 1 print @DBG + '  @originY:' + cast(@originY AS VARCHAR(max))
	IF @DEBUG = 1 print @DBG + '  @destinationX:' + cast(@destinationX AS VARCHAR(max))
	IF @DEBUG = 1 print @DBG + '  @destinationY:' + cast(@destinationY AS VARCHAR(max))

	select @UnitSpeed = min(speed)
	from #temp VSU
	join UnitTypes UT
		on VSU.UnitTypeID = UT.UnitTypeID 
	where 
		UnitAmount > 0
	IF @DEBUG = 1 print @DBG + '  @UnitSpeed:' + cast(@UnitSpeed AS VARCHAR(max))
	--if this @UnitSpeed is null that means no units need to recalled as all units have count 0 
	IF @UnitSpeed is null  BEGIN
			IF @DEBUG = 1 print @DBG + '  @UnitSpeed = null ,  return quietly '
			RETURN 
	END
	set @TripDurationInTicks = dbo.fnGetTravelTime(@originX, @originY, @destinationX, @destinationY, @UnitSpeed) * @TravelTimeBonusMultipier
	--
	-- From now on, everything must be in a transaction. 
	--
	BEGIN TRAN CommandTroops
		--
		-- Obtain locks on effected villages
		--
		update VillageSemaphore set TimeStamp = getdate() where VillageID in (@SupportingVillageID, @SupportedVillageID)
	

		insert into Events (EventTime) values ( dateadd(millisecond, @TripDurationInTicks/10000, GetDate())  )	
		set @EventID = SCOPE_IDENTITY() 
		IF @DEBUG = 1 print @DBG + '  @EventID:' + cast(@EventID AS VARCHAR(max))

		insert into UnitMovements 
			values (@EventID, @SupportedVillageID, @SupportingVillageID, 3 /*3-> support recall*/, @TripDurationInTicks,0,2)

		insert into UnitsMoving 
			select @EventID, VSU.UnitTypeID, T.UnitAmount, null
			from VillageSupportUnits VSU inner join #temp T on T.UnitTypeID=VSU.UnitTypeID
			where 
				VSU.SupportedVillageID = @SupportedVillageID
				and VSU.SupportingVillageID = @SupportingVillageID
				and VSU.UnitCount > 0
				and ( VSU.SupportingVillageID in (select VillageID from Villages where OwnerPlayerID = @SupportingPlayerID) OR @SupportingPlayerID is null) -- security check
				and ( VSU.SupportedVillageID in (select VillageID from Villages where OwnerPlayerID = @SupportedPlayerID) OR @SupportedPlayerID is null)-- security check
		
		--
		-- if above call resulted in at leats 1 row copied, that means it was successful
		--	otherwise, either playerID was wrong (security check) or perhaps the support was already removed. 
		--
		IF @@rowcount = 0 BEGIN
			IF @DEBUG = 1 print @DBG + '  @@rowcount = 0, rollback and return quietly '
			ROLLBACk TRAN CommandTroops
			RETURN 
		END
		--
		-- Remove the support from the supported village 
		--
		update VillageSupportUnits set UnitCount =  UnitCount - T.UnitAmount
		from VillageSupportUnits VSU 
			join #temp T 
			on T.UnitTypeID = VSU.UnitTypeID
			where 
				SupportedVillageID = @SupportedVillageID
				and SupportingVillageID = @SupportingVillageID
				and  UnitCount - T.UnitAmount>=0
	
		IF @@rowcount <> (select count(*)from VillageSupportUnits VSU
								join #temp T 
								on T.UnitTypeID = VSU.UnitTypeID
								where 
							SupportedVillageID = @SupportedVillageID
							and SupportingVillageID = @SupportingVillageID) BEGIN

			IF @DEBUG = 1 print @DBG + '  @@rowcount <> Count from VillageSupportUnits, rollback and return quietly '
			ROLLBACk TRAN CommandTroops
			RETURN 
		END	

		IF @DEBUG = 1 select * from VillageSupportUnits where SupportedVillageID = @SupportedVillageID and SupportingVillageID = @SupportingVillageID
		IF @DEBUG = 1 select * from UnitMovements where EventID = @EventID
		IF @DEBUG = 1 select * from UnitsMoving where EventID = @EventID
		IF @DEBUG = 1 select * from Events where EventID = @EventID

	COMMIT TRAN CommandTroops
	
	
	--
	-- Create a report.  we do this outside of the transaction on purpose because IF or some reason creation of the report fails,
	--	then we can live with that
	-- 
	IF @SupportingPlayerID is not null BEGIN
		--
		-- since supporting player is not null, this means that this is a recall since it is the supporting player 
		--	who initiated this. So create a report for the supported player, a support pulled back report
		--
		-- However, only create the report if the village the troops are recalled from is NOT owned by the supporting player since
		--	there is no need to tell a player that he just did this. 
		
		IF NOT EXISTS (SELECT * FROM Villages where VillageID = @SupportedVillageID AND OwnerPlayerID = @SupportingPlayerID) BEGIN
		
			SELECT @ForReport_SupportingPlayerName = Name FROM Players where PlayerID = @SupportingPlayerID
			
			IF @DEBUG = 1 select @ForReport_SupportingPlayerID as '@ForReport_SupportingPlayerID'
				, @ForReport_SupportedPlayerID as '@ForReport_SupportedPlayerID'
				, @ForReport_SupportingPlayerName as '@ForReport_SupportingPlayerName'
				, @ForReport_SupportedPlayerName as '@ForReport_SupportedPlayerName'
				, @ForReport_SupportingVillageName as '@ForReport_SupportingVillageName'		
				, @ForReport_SupportedVillageName as '@ForReport_SupportedVillageName'		
			
			INSERT INTO Reports (Time, Subject, ReportTypeID, ReportTypeSpecificData)
				VALUES(getdate()
					, dbo.Translate('uRSU_supFrom') + cast(@ForReport_SupportedVillageName AS VARCHAR(max))
					, @CONST_SupportPulledBack 
					, '<a href=player.aspx?pname=' + @ForReport_SupportingPlayerName + '>'+ @ForReport_SupportingPlayerName + '</a>'
					    + @For_Report_SupportingTroops
						+ '<a href=VillageOverviewOther.aspx?ovid=' + cast(@SupportedVillageID as varchar)+ '>' + @ForReport_SupportedVillageName + '</a>'
						+ '.')
			set @ReportID = SCOPE_IDENTITY()
				
			insert into ReportAddressees (PlayerID, ReportID, ForwardedByPlayerID, ForwardedOn, IsViewed) 
				values (@ForReport_SupportedPlayerID, @ReportID, null, null, 0)	
			update Players set NewReportIndicator = 1 where PlayerID = @ForReport_SupportedPlayerID
			
		END 
	END ELSE BEGIN -- ie, @SupportingPlayerID IS null HENCE @SupportedPlayerID IS NOT null
		--
		-- since supporting player is  null, this means that this is a send back since it is the supported player 
		--	who initiated this. So create a report for the suporting player, a support sent back report
		--
		--
		-- However, only create the report if the village the troops are send back from is NOT owned by the supporting player since
		--	there is no need to tell a player that he just did this. 

		IF NOT EXISTS (SELECT * FROM Villages where VillageID = @SupportedVillageID AND OwnerPlayerID = @ForReport_SupportingPlayerID) BEGIN

			SELECT @ForReport_SupportingPlayerName = Name FROM Players where PlayerID = @ForReport_SupportingPlayerID
			SELECT @ForReport_SupportedPlayerName = Name FROM Players where PlayerID = @ForReport_SupportedPlayerID
			
			IF @DEBUG = 1 select @ForReport_SupportingPlayerID as '@ForReport_SupportingPlayerID'
				, @ForReport_SupportedPlayerID as '@ForReport_SupportedPlayerID'
				, @ForReport_SupportingPlayerName as '@ForReport_SupportingPlayerName'
				, @ForReport_SupportedPlayerName as '@ForReport_SupportedPlayerName'
				, @ForReport_SupportingVillageName as '@ForReport_SupportingVillageName'		
				, @ForReport_SupportedVillageName as '@ForReport_SupportedVillageName'		
			
			INSERT INTO Reports (Time, Subject, ReportTypeID, ReportTypeSpecificData)
				VALUES(getdate()
					, dbo.Translate('uRSU_supBack') + cast(@ForReport_SupportedVillageName AS VARCHAR(max))
					, @CONST_SupportSentBack 
					, '<a href=player.aspx?pname=' + @ForReport_SupportedPlayerName +'>' + @ForReport_SupportedPlayerName+  '</a>' + dbo.Translate('uRSU_yourSupBack')
						+ '<a href=VillageOverviewOther.aspx?ovid=' + cast(@SupportedVillageID as varchar) + '>' + @ForReport_SupportedVillageName + '</a>.<BR><BR>'
						+ dbo.Translate('uRSU_headingBack') + '<a href=VillageOverviewOther.aspx?ovid=' + cast(@SupportingVillageID as varchar)+ '>' + @ForReport_SupportingVillageName  + '</a>.')
			set @ReportID = SCOPE_IDENTITY()
				
			insert into ReportAddressees (PlayerID, ReportID, ForwardedByPlayerID, ForwardedOn, IsViewed) 
				values (@ForReport_SupportingPlayerID, @ReportID, null, null, 0)	
			update Players set NewReportIndicator = 1 where PlayerID = @ForReport_SupportingPlayerID
					
		END
	END
	IF @DEBUG = 1 select 'Reports' , * from Reports where ReportID = @ReportID
	IF @DEBUG = 1 select 'ReportAddressees', * from ReportAddressees where ReportID = @ReportID

	
	--
	-- update cache time stamps
	--
	-- incoming cache item (1) 
	UPDATE PlayerCacheTimeStamps SET TimeStamp = getdate() where PlayerID = (select ownerplayerid from villages where villageid = @SupportingVillageID) and CachedItemID = 1  
	IF (@@rowcount < 1 ) BEGIN
		INSERT INTO PlayerCacheTimeStamps select ownerplayerid, 1, getdate() from villages where villageid = @SupportingVillageID
	END


IF @DEBUG = 1 print @DBG + 'END uRecallUnits ' 
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max)
	 IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'uRecallSomeUnits FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @SupportingPlayerID'	+ ISNULL(CAST(@SupportingPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SupportingVillageID' + ISNULL(CAST(@SupportingVillageID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SupportedVillageID'	+ ISNULL(CAST(@SupportedVillageID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TripDurationInTicks' + ISNULL(CAST(@TripDurationInTicks AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @EventID'				+ ISNULL(CAST(@EventID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @originX'				+ ISNULL(CAST(@originX AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @originY'				+ ISNULL(CAST(@originY AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @destinationX'		+ ISNULL(CAST(@destinationX AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @destinationY'		+ ISNULL(CAST(@destinationY AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ReportID'			+ ISNULL(CAST(@ReportID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @UnitSpeed'			+ ISNULL(CAST(@UnitSpeed AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ForReport_SupportingPlayerID'	+ ISNULL(CAST(@ForReport_SupportingPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ForReport_SupportedPlayerID'		+ ISNULL(CAST(@ForReport_SupportedPlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ForReport_SupportingPlayerName'	+ ISNULL(CAST(@ForReport_SupportingPlayerName AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ForReport_SupportedPlayerName'	+ ISNULL(CAST(@ForReport_SupportedPlayerName AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ForReport_SupportingVillageName' + ISNULL(CAST(@ForReport_SupportingVillageName AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ForReport_SupportedVillageName'	+ ISNULL(CAST(@ForReport_SupportedVillageName AS VARCHAR(max)), 'Null') + CHAR(10)
		
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(max)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	


