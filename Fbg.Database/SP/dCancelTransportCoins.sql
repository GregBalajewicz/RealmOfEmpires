IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dCancelTranportCoins')
	BEGIN
		DROP  Procedure  dCancelTranportCoins
	END

GO

CREATE Procedure [dbo].[dCancelTranportCoins]
		@EventID int
		,@EventsOwnerPlayerID int
		
AS
	
begin try 
	begin tran
		declare @OriginVillageID as int;
		declare @TrasuryOverflow as int;
		declare @Amount as int;
		declare @EventTime as datetime ;
		declare @TimeDiff as int;
		declare @TimeToReturn as datetime 
		declare @TripDuration as int;
		declare @TripDiff as int;
			
		--
		-- First we set the event as completed so that no one, in the mean time, can cancel it; or process it 
		--
		UPDATE Events SET [Status] =1 WHERE EventID = @EventID AND [Status] = 0
		IF @@rowcount <> 1 BEGIN
			-- IF no rows where updated, then the event must have been cancelled (or something like this) thereforecs we abort quietly
			ROLLBACK 
			INSERT INTO ErrorLog VALUES (getdate(), 0, 'dCancelTranportCoins : update UPDATE Events SET [Status] =1 resulted in no rows updated',  ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null'))
			goto __DONE__
		END
		
		
		--this check to insure that this transport is for not reserved trasport 
		if exists(select * FROM           
				CoinTransports 
				INNER JOIN Villages ON CoinTransports.OriginVillageID = Villages.VillageID
				WHERE        (CoinTransports.EventID = @EventID) AND (CoinTransports.Reserved = 0) 
				AND (CoinTransports.Direction = 0)and OwnerPlayerID=@EventsOwnerPlayerID )
		begin
						
						
			select @EventTime=EventTime 
					,@OriginVillageID=OriginVillageID 
					,@Amount=Amount
					,@TripDuration=TripDuration/10000000 
					from events 
					inner join CoinTransports
					on events.EventID=CoinTransports.EventID 
					where events.EventId =@EventID
			
			--
			-- Obtain a village LOCK
			--		this will ensure that no one else will change anything in the village, or effect the upgrade Q 
			--
			update VillageSemaphore 				
			set [TimeStamp] = getdate() 
			WHERE villageID = @OriginVillageID
			
			
			set @TimeDiff = datediff (s ,getdate(),@EventTime);
			Set @TripDiff=@TripDuration-@TimeDiff
			set @TimeToReturn =dateadd (ss ,@TripDiff,getdate());
			--select @TimeDiff,@TimeToReturn,@EventTime,getdate(),@TripDiff,@TripDuration

							
			update Events 
			set EventTime=@TimeToReturn,[Status]=0
			where EventId =@EventID
			
			-- set it as returing 
			update CoinTransports 
			set Direction=1 
			where EventID=@EventID
			
			-- return coins to village of origin. 
			exec uVillageCoins_Add @OriginVillageID,@Amount,@TrasuryOverflow
						
		end
	
commit tran
	__DONE__:
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'dCancelTranportCoins FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @EventID' + ISNULL(CAST(@EventID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

  