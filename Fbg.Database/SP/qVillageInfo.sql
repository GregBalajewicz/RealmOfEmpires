 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qVillageInfo')
	BEGIN
		DROP  Procedure  qVillageInfo
	END

GO

CREATE Procedure qVillageInfo
	@LoggedInPlayerID int
	, @VillageID int 
    , @GetAreTransportsAvailable bit = 0 

AS

begin try 		
	--
	-- get basic info
	--
	select 
		v.Name
		, v.Coins
		, V.points as villagepoints
		, XCord
		, YCord 
		, cast(loyalty + floor(cast(datediff(minute, LoyaltyLastUpdated, getdate()) as real) /(60.0 / (select cast(AttribValue as real) FROM RealmAttributes where attribid =8))) as integer) 
		, b_CoinMine.Level as CoinMineLevel
		, b_Treasury.Level as TreasuryLevel
		, V.CoinsLastUpdates
		, VillageTypeID
		from villages v 
		join buildings b_CoinMine on V.villageId = b_CoinMine.villageID 
			and b_CoinMine.BuildingTypeID = 5
		join buildings b_Treasury on V.villageId = b_Treasury.villageID 	
			and b_Treasury.BuildingTypeID = 6
	where V.VillageID = @VillageID
		 and  OwnerPlayerID = @LoggedInPlayerID
	IF @@rowcount = 0 BEGIN
		INSERT INTO ErrorLog VALUES (getdate(), 1, 'Attempt to access someone elses village:' + ISNULL(CAST(@VillageID AS VARCHAR(10)),'null' ), ISNULL(CAST(@LoggedInPlayerID AS VARCHAR(10)),'null' ))		
		RETURN
	END
	
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
	-- get indicator if transports to this village are available
	--
	
	IF @GetAreTransportsAvailable = 1 and exists (select count(*) from villages with(nolock) where ownerplayerid = @LoggedInPlayerID having count(*) < 50) BEGIN
	    declare @retval int
	    exec qCoinTransports_AreTransportToVillageAvail @LoggedInPlayerID, @VillageID, @retval output
	END ELSE BEGIN 
	    select 1
	END
	
	--
	-- get the tags for this village
	--
	select TagID from villagetags where VillageID = @VillageID

end try
begin catch
	 IF @@TRANCOUNT > 0 ROLLBACK

	DECLARE @ERROR_MSG AS VARCHAR(max)
	
	SET @ERROR_MSG = 'qVillageInfo FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @LoggedInPlayerID'+ ISNULL(CAST(@LoggedInPlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @VillageID'		+ ISNULL(CAST(@VillageID AS VARCHAR(100)), 'Null') + CHAR(10)

		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
		
	RAISERROR(@ERROR_MSG,11,1)	

end catch	


