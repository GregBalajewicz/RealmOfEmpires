IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uTransportCoinsCompleted')
	BEGIN
		DROP  Procedure  uTransportCoinsCompleted
	END

GO

CREATE Procedure [dbo].[uTransportCoinsCompleted]
	@EventID as int
	, @PrintDebugInfo BIT = null
AS
declare @DestinationVillageID int
declare @Amount int
declare @TrasuryOverflow int
declare @Direction smallint
declare @TripDuration bigint
declare @TimeToReturn datetime
declare @Reserved bit
declare @VillageName as nvarchar(20)
declare @DestinationVillagePlayerID as int
declare @OriginVillageID as int
declare @ReportID as int
declare @ReportBody as varchar(max)
declare @ReportSubject as varchar(max)
declare @SenderPlayerName as varchar(25);
declare @DestinationVillageName as varchar(25);
declare @DestinationPlayerName as varchar(25);
declare @DestinationVillageX as int;
declare @DestinationVillageY as int;
declare @OriginVillagePlayerID as int;
declare @SenderReportBody as varchar(max)
declare @SenderReportSubject as varchar(max)

declare @CONST_SilverTransportReportType int
set @CONST_SilverTransportReportType  = 3

DECLARE @DEBUG INT, @DBG varchar(10)
IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
	SET @DEBUG = 1
	set @DBG = ':::::::'
	SET NOCOUNT OFF
END ELSE BEGIN
	SET @DEBUG = 0
	SET NOCOUNT ON
END 
IF @DEBUG = 1 print @DBG + 'BEGIN uTransportCoinsCompleted ' + cast(@EventID as varchar(10))

begin try 
	begin transaction tran_uTransportCoinsCompleted

	
		IF @DEBUG = 1 print @DBG + 'Doing update Event '
		select @OriginVillageID=OriginVillageID
			,@DestinationVillageID=DestinationVillageID
			,@Amount=Amount 
			,@Direction=Direction
			,@TripDuration=TripDuration 
			,@Reserved=Reserved 
			from CoinTransports where EventID=@EventID
	
		--
	-- Obtain a village LOCK
	--		this will ensure that no one else will change anything in the village, or effect the upgrade Q 
	--
	update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @DestinationVillageID
	
	
		if (@Direction=0)--already reach the  village
			begin
				-- begin to update the event to the return time
				
				set @TimeToReturn = dateadd (ss ,@TripDuration/10000000,getdate());

				
				update Events 
				set EventTime=@TimeToReturn
				where EventId =@EventID

				-- set it as returing 
				update CoinTransports set Direction=1 where EventID=@EventID
				-- update the destination village coins
				if @Reserved=0
					begin
						exec uVillageCoins_Add @DestinationVillageID, @Amount, @TrasuryOverflow output
					end
				--get village name
				select @VillageName=Villages.Name ,@SenderPlayerName=Players.Name,@OriginVillagePlayerID=OwnerPlayerID from Villages inner join Players on Villages.OwnerPlayerID=Players.PlayerID	where VillageID=@OriginVillageID
				--getplayerid for destination village
				select @DestinationVillagePlayerID=OwnerPlayerID ,@DestinationVillageName=villages.Name ,@DestinationVillageX =XCord ,@DestinationVillageY=YCord,@DestinationPlayerName=Players.Name from villages inner join Players on Villages.OwnerPlayerID=Players.PlayerID where villageid=@DestinationVillageID
				
				
				-- Create  reciver report
				SET @ReportSubject = dbo.Translate('uTCC_arrAt')+@DestinationVillageName
				SET @ReportBody = @SenderPlayerName+dbo.Translate('uTCC_hasSent')+CAST(@Amount AS VARCHAR(10)) 
					+dbo.Translate('uTCC_silToYour')+@DestinationVillageName+'.' +dbo.Translate('uTCC_transArr')
				IF @TrasuryOverflow>0 BEGIN
					SET @ReportBody  = @ReportBody + '<br><br>' + dbo.Translate('uTCC_notStoreAll') 
						+ CAST(@TrasuryOverflow AS VARCHAR(10))+dbo.Translate('uTCC_silLootLost')
				END
				--- create sender report 
				SET @SenderReportSubject =dbo.Translate('uTCC_yourArrAt')+@DestinationVillageName
				SET @SenderReportBody =dbo.Translate('uTCC_youSent')+CAST(@Amount AS VARCHAR(10)) 
					+dbo.Translate('uTCC_silTo')+@DestinationPlayerName+'.' + dbo.Translate('uTCC_transArrAt')+@DestinationVillageName+' ('+CAST(@DestinationVillageX AS VARCHAR(10)) +','+CAST(@DestinationVillageY AS VARCHAR(10)) +')'
				if @Reserved=0
					begin
						INSERT INTO Reports (Time, Subject, ReportTypeID, ReportTypeSpecificData)
							VALUES(getdate()
								, @ReportSubject
								, @CONST_SilverTransportReportType 
								, @ReportBody)					
						set @ReportID = SCOPE_IDENTITY()
							
						insert into ReportAddressees (PlayerID, ReportID, ForwardedByPlayerID, ForwardedOn, IsViewed) 
						values (@DestinationVillagePlayerID, @ReportID, null, null, 0)	
						update players set NewReportIndicator = 1 where playerid = @DestinationVillagePlayerID
						
						
						----------------------sender report 
							INSERT INTO Reports (Time, Subject, ReportTypeID, ReportTypeSpecificData)
							VALUES(getdate()
								, @SenderReportSubject
								, @CONST_SilverTransportReportType 
								, @SenderReportBody)					
						set @ReportID = SCOPE_IDENTITY()
							
						insert into ReportAddressees (PlayerID, ReportID, ForwardedByPlayerID, ForwardedOn, IsViewed) 
						values (@OriginVillagePlayerID, @ReportID, null, null, 0)	
						update players set NewReportIndicator = 1 where playerid = @OriginVillagePlayerID
					end
			end
		else
			begin --alreday back to village 
				update Events 
				set Status =  1 
				where EventId =@EventID
				-- set it as finished
				update CoinTransports set Direction=2 where EventID=@EventID
			end
			--
			-- Update Destination Village Amount
			--
			IF @DEBUG = 1 print @DBG + 'Doing Update Destination Village '
							
commit transaction tran_uTransportCoinsCompleted
	
end try
begin catch
    IF @@TRANCOUNT > 0 ROLLBACK
	DECLARE @ERROR_MSG AS VARCHAR(max)

	
	SET @ERROR_MSG = 'uTransportCoinsCompleted FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @EventID'			+ ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @DestinationVillageID' + ISNULL(CAST(@DestinationVillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @Amount'			+ ISNULL(CAST(@Amount AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @Direction' + ISNULL(CAST(@Direction AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @TripDuration' + ISNULL(CAST(@TripDuration AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @TimeToReturn' + ISNULL(CAST(@TimeToReturn AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @Reserved' + ISNULL(CAST(@Reserved AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @VillageName' + ISNULL(CAST(@VillageName AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @DestinationVillagePlayerID' + ISNULL(CAST(@DestinationVillagePlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @OriginVillageID' + ISNULL(CAST(@OriginVillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @ReportID' + ISNULL(CAST(@ReportID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @ReportBody'		+ ISNULL(@ReportBody , 'Null') + CHAR(10)
		+ '   @ReportSubject'	+ ISNULL(@ReportSubject, 'Null') + CHAR(10)
		+ '   @SenderPlayerName'	+ ISNULL(@ReportSubject, 'Null') + CHAR(10)
		+ '   @DestinationVillageName'	+ ISNULL(@ReportSubject, 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



if @DestinationVillagePlayerID is not null BEGIN -- if null, that means we did not actually do anythig requiring update
	--
	-- say that the village has changed. this is done deliberately outside of the main tran and try 
	--
	UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @DestinationVillagePlayerID and VillageID = @DestinationVillageID and CachedItemID = 0
	IF (@@rowcount < 1 ) BEGIN
		INSERT INTO VillageCacheTimeStamps values (@DestinationVillagePlayerID, @DestinationVillageID, 0, getdate())
	END
END
GO