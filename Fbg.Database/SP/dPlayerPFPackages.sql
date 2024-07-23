 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dPlayerPFPackages')
	BEGIN
		DROP  Procedure  dPlayerPFPackages
	END

GO

--
-- procedure returns just one result set with a single row, 2 columns
--	1st column the servants to refund (int)
--	2nd column the days to cancel (real)
--
CREATE Procedure [dPlayerPFPackages]
	@PlayerID as int
	,@PackageID as int
	,@Precentage as real -- number between 0 and 1
	,@CalculateOnly as bit--this value enable the SP to only calculate the refund value without doing the refund
	,@RefundType as int--0 means DepreciatedFeature ,1 means ActiveFeature, 2 means refund while activating Nobility package
	,@PrintDebugInfo BIT = null
AS
begin try 
	declare @Cost as int
	declare @Duration as int
	declare @CostPerDay as real
	declare @ServantsReturned as int
	declare @ExpiresOn as datetime
	declare @now as datetime 
	declare @DoRefundOfExtensionOnly as bit
	declare @DaysTillExpiryOfPackage as real
	declare @NoOfDaysToCancel as real
	declare @note varchar(max)
	declare @logEventType int

	DECLARE @DEBUG INT
	IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
		SET @DEBUG = 1
		SET NOCOUNT OFF
	END ELSE BEGIN
		SET @DEBUG = 0
		SET NOCOUNT ON
	END 
	IF @DEBUG = 1 SELECT 'BEGIN [dPlayerPFPackages] '

	if @Precentage > 1 or @Precentage <0  begin
		--
		-- invalid params
		--
		RAISERROR('@Precentage must be between 0 and 1',11,1)	
	end
	if @RefundType <> 1 and @RefundType <> 0 and   @RefundType <> 2 begin
		--
		-- invalid params
		--
		RAISERROR('@RefundType must be 0 , 1 or 2',11,1)	
	end

	--
	-- init values
	--
	set @DoRefundOfExtensionOnly = 0
	set @now = getdate()
	
	--
	-- get info about this package
	--
	select @Cost=Cost 
		,@Duration=Duration 
		,@ExpiresOn=ExpiresOn
		from PFPackages 
		join PlayersPFPackages 
			on PFPackages.PFPackageID=PlayersPFPackages.PFPackageID
		where 
			PlayersPFPackages.PFPackageID=@PackageID 
			and PlayerID=@PlayerID
	if @Cost is null begin
		-- package don't exists for this user
		select 0,0;
		return 
	end
	set @CostPerDay=cast(@Cost as real)/@Duration;
	
	IF @DEBUG = 1 SELECT @Cost as '@Cost'
		, @Duration  as '@Duration' 
		, @ExpiresOn as '@ExpiresOn'
		, @CostPerDay as '@CostPerDay'
        , cast(datediff(day,@now,@ExpiresOn) as float) as 'datediff(day,@now,@ExpiresOn)'
	    , datediff(minute,@now,@ExpiresOn) as 'datediff(minute,@now,@ExpiresOn)'
	    , datediff(minute,@now,@ExpiresOn) / 60.0 as 'datediff(minute,@now,@ExpiresOn) / 60.0 - AKA hours till expiry'
	    , datediff(minute,@now,@ExpiresOn) / 60.0 / 24 as 'datediff(minute,@now,@ExpiresOn) / 60.0 /24 - AKA days till expiry'
	--
	-- how many days till this package expires
	--
	IF datediff(day,@now,@ExpiresOn) <0 BEGIN
		select 0,0
		RETURN -- nothing to cancel. the package is not active.
	END ELSE IF datediff(day,@now,@ExpiresOn) = 0 BEGIN
		IF datediff(minute,@now,@ExpiresOn) > 0 BEGIN
			SET @DaysTillExpiryOfPackage = datediff(minute,@now,@ExpiresOn) / 60.0 /24.0
		END ELSE BEGIN
			select 0,0
			RETURN -- nothing to cancel. the package is not active.
		END
	END ELSE BEGIN
		DECLARE @h int
		declare @m int
		SET @DaysTillExpiryOfPackage = datediff(day,@now,@ExpiresOn) 
		set @h = datediff(hour,dateadd(d, @DaysTillExpiryOfPackage, @now),@ExpiresOn)
		set @m = datediff(minute,dateadd(hour, @h, dateadd(d, @DaysTillExpiryOfPackage, @now)),@ExpiresOn)		
		SET @DaysTillExpiryOfPackage = @DaysTillExpiryOfPackage + @h / 24.0
		SET @DaysTillExpiryOfPackage = @DaysTillExpiryOfPackage + @m / 1440.0
	END
	
	IF @DEBUG = 1 SELECT @DaysTillExpiryOfPackage as '@DaysTillExpiryOfPackage'


	--
	-- figure out how many days to refund. 
	--
	IF @RefundType = 0 or  @RefundType = 2 BEGIN
		--
		-- for depreciated features, AND for activating NP we always do refund for all remaining days
		--
		set @NoOfDaysToCancel = @DaysTillExpiryOfPackage
	END ELSE BEGIN
		--
		-- for active feature (not deprecated ones).
		--
		IF @DaysTillExpiryOfPackage > @Duration BEGIN
			--
			-- this feature is extended and the extension has not been used yet.
			--	so we do a full refund of the extension only 
			--
			set @NoOfDaysToCancel = @Duration
			set @DoRefundOfExtensionOnly = 1
		END ELSE BEGIN 
			--
			-- this feature expires in less time then its duration
			-- For this we do a partial, prorated refund
			--
			set @NoOfDaysToCancel = @DaysTillExpiryOfPackage
		END 
	END
	IF @DEBUG = 1 SELECT @NoOfDaysToCancel as '@NoOfDaysToCancel', @DoRefundOfExtensionOnly as '@DoRefundOfExtensionOnly'
	
	
	IF @NoOfDaysToCancel > 0 BEGIN
		--
		-- calculate how many servant to refund.
		--
		IF @DoRefundOfExtensionOnly = 1 BEGIN
			--
			-- this means we are cancelling just the extension of a active (not depreciated) package
			--
			set @ServantsReturned = @Cost
		END  ELSE BEGIN		
			IF @RefundType = 0 BEGIN 
				--
				-- for depreciated packages we round up 
				--			
				set @ServantsReturned=CEILING (@CostPerDay*@NoOfDaysToCancel)
			end else if @RefundType = 1  begin
				--
				-- for not deprecaite packages we round down and take a percentage
				--
				set @ServantsReturned=floor ((@CostPerDay*@NoOfDaysToCancel)*@Precentage)
			end else if @RefundType = 2 begin
				--
				-- for refunding packages as part of activaing the NP, we round down 
				--	 and we do not take a percentage
				--
				set @ServantsReturned=floor (@CostPerDay * @NoOfDaysToCancel)
			END ELSE BEGIN 
				RAISERROR('@RefundType must be 0 , 1 or 2',11,1)	
			end
			
		
		END 		
		IF @DEBUG = 1 SELECT @ServantsReturned as '@ServantsReturned'
		
		--
		-- now do the actual cancel of package unless we were asked to do calculate only.
		--
		if @CalculateOnly = 0 begin
			BEGIN TRAN
			
			IF @DoRefundOfExtensionOnly = 1 BEGIN
				--
				-- this means we are cancelling just the extension of a active (not depreciated) package
				--
				update PlayersPFPackages set ExpiresOn= DATEADD(d,-@Duration,ExpiresOn)
					where PFPackageID=@PackageID and PlayerID=@PlayerID 
			end else begin
				--
				-- we are cancelling the package completely. 
				--
				update PlayersPFPackages set ExpiresOn = @now
					where PFPackageID=@PackageID and PlayerID=@PlayerID
			end
			--			
			-- log the process of cancelling a package
			--
			set @note = '@PackageID=' + ISNULL(CAST(@PackageID AS VARCHAR(100)), 'Null') + CHAR(10)
				+ '@RefundType=' + ISNULL(CAST(@RefundType AS VARCHAR(100)), 'Null') + CHAR(10)
				+ '@Cost=' + ISNULL(CAST(@Cost AS VARCHAR(100)), 'Null') + CHAR(10)
				+ '@Duration=' + ISNULL(CAST(@Duration AS VARCHAR(100)), 'Null') + CHAR(10)
				+ '@CostPerDay=' + ISNULL(CAST(@CostPerDay AS VARCHAR(100)), 'Null') + CHAR(10)
				+ '@ExpiresOn=' + ISNULL(CAST(@ExpiresOn AS VARCHAR(100)), 'Null') + CHAR(10)
				+ '@DoRefundOfExtensionOnly=' + ISNULL(CAST(@DoRefundOfExtensionOnly AS VARCHAR(100)), 'Null') + CHAR(10)
				+ '@DaysTillExpiryOfPackage=' + ISNULL(CAST(@DaysTillExpiryOfPackage AS VARCHAR(100)), 'Null') + CHAR(10)
				+ '@NoOfDaysToCancel=' + ISNULL(CAST(@NoOfDaysToCancel AS VARCHAR(100)), 'Null') + CHAR(10)
			
			IF @RefundType = 0 BEGIN
				set @logEventType = 8						
			END ELSE IF @DoRefundOfExtensionOnly = 1 BEGIN			
				set @logEventType = 9
			END else BEGIN
				set @logEventType = 10
			END 			
			insert into PlayerPFLog
				(PlayerID,Time ,EventType,Credits ,Cost, Notes)
				values
				(@PlayerID,@now,@logEventType,@ServantsReturned,0, @note)

				
			COMMIT TRAN
		end
		select @ServantsReturned, @NoOfDaysToCancel
		return 	@ServantsReturned
	END

	IF @DEBUG = 1 SELECT 'END [dPlayerPFPackages] '
	
  end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'dPlayerPFPackages FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID'				  + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @PackageID'				  + ISNULL(CAST(@PackageID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @Cost'					+ ISNULL(CAST(@Cost AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @Duration'				  + ISNULL(CAST(@Duration AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @Precentage'				  + ISNULL(CAST(@Precentage AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @RefundType'		  + ISNULL(CAST(@RefundType AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @CalculateOnly'		  + ISNULL(CAST(@CalculateOnly AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @ServantsReturned'		  + ISNULL(CAST(@ServantsReturned AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @CostPerDay'		  + ISNULL(CAST(@CostPerDay AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @ExpiresOn'		  + ISNULL(CAST(@ExpiresOn AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @now'		  + ISNULL(CAST(@now AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @DoRefundOfExtensionOnly'		  + ISNULL(CAST(@DoRefundOfExtensionOnly AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @DaysTillExpiryOfPackage'		  + ISNULL(CAST(@DaysTillExpiryOfPackage AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @NoOfDaysToCancel'		  + ISNULL(CAST(@NoOfDaysToCancel AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @note'		  + ISNULL(CAST(@note AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @logEventType'		  + ISNULL(CAST(@logEventType AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	





