IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iTranportCoins')
	BEGIN
		DROP  Procedure  iTranportCoins
	END

GO

CREATE Procedure [dbo].[iTranportCoins]
		@PlayerID int,
		@OriginVillageID int,
		@DestinationVillageID int,
		@Amount int,
		@TripDuration bigint,
		@TripDirection smallint,
		@TravelTime datetime,
		@Reserved bit,
		@Now datetime -- this is SP what it the authoritative time. 
		
		--@Error int output -- 0 means all good; 1 means village don't exist;2 means Coins More then Allowed ;3 means Coins must be greater then Zero
		-- 4 means Only_Numbers_Accepted ;5 means Same_Village
AS
	
begin try 
	begin tran
		declare @Sucess as bit;
		declare @Error as int;
		set @Error=0;
		declare @EventID as int;
		declare @Time as datetime;
		declare @TrasuryOverflow as int;
		declare @AmtCurrentlyInTransport as int;
		declare @LevelLimit as int;
		declare @LevelLimitFloat as float;
		declare @MaxAmountToTransport as int;
		declare @VillageCoins as int;
        declare @researchPercentBonus as real
		declare @villageTypeBonus as real

        declare @LevelProp_TradePostCapacity int 
        set @LevelProp_TradePostCapacity = 10

		--
		-- Obtain a village LOCK
		--		this will ensure that no one else will change anything in the village, or effect the upgrade Q 
		--
		update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @DestinationVillageID or villageID = @OriginVillageID
		
		
		SELECT    @AmtCurrentlyInTransport= sum(Amount)
		FROM         CoinTransports INNER JOIN
                      Events ON CoinTransports.EventID = Events.EventID
		WHERE     (CoinTransports.OriginVillageID = @OriginVillageID and Events.Status<>1)

		select @LevelLimitFloat=PropertyValue from LevelProperties lp inner join Buildings b on b.BuildingTypeID=lp.BuildingTypeID and b.level=lp.level
		where b.buildingtypeid=11 and b.Villageid=@OriginVillageID ;
	
		set @LevelLimit = Round(@LevelLimitFloat, 0)
        	        		
        --
        -- obtain research bonus if any. will be in a format like 0.1 meaning 10%
        -- 
        select @researchPercentBonus = sum(cast(PropertyValue as float))
            from ResearchItemPropertyTypes PT 
            join ResearchItemProperties P on P.ResearchItemPropertyID = PT.ResearchItemPropertyID
            join ResearchItems RI on RI.ResearchItemTypeID = P.ResearchItemTypeID AND RI.ResearchItemID = P.ResearchItemID
            join PlayerResearchItems PRI ON RI.ResearchItemTypeID = PRI.ResearchItemTypeID AND RI.ResearchItemID = PRI.ResearchItemID
            where PT.PropertyID = @LevelProp_TradePostCapacity
                AND PlayerID = @PlayerID
                AND PT.Type = 3 	
        SET @researchPercentBonus = isnull(@researchPercentBonus,0)

		--
		-- get the possible trasnport capacity bonus from bonus village type. 
		--
		select @villageTypeBonus = PropertyValue from villages V join VillageTypeProperties VP
			on v.VillageTypeID = VP.VillageTypeID
			and VillageTypePropertyTypeID = 8 
		where villageid = @OriginVillageID
		set @villageTypeBonus = isnull(@villageTypeBonus, 0) + 1;


		set @MaxAmountToTransport= (@LevelLimit * (@researchPercentBonus+1) * @villageTypeBonus) - @AmtCurrentlyInTransport

	
		if (@Amount>@MaxAmountToTransport)--check for Transport avialabilty
        begin
			set @Error  = 2
			select @Error;
			rollback
			return  
		end
		
		set @Time=getdate();
		-- check for existance of destination village 
		IF not exists (select * from villages where villageid=@DestinationVillageID) BEGIN
			set @Error  = 1
			select @Error;
			rollback
			return  
		END	 
		if @DestinationVillageID<>@OriginVillageID
			begin
				-- subtract the amountfrom th original village
				exec uVillageCoins_Subtract @OriginVillageID,@Amount,@Sucess output, 0 , @Now
				if @Sucess<>1 begin
					set @Error  = 2
					select @Error;
					rollback
					return  
				end

				-- insert an event for the Transport
				insert into events (EventTime,Status) values(@TravelTime,0)
				set @EventID = SCOPE_IDENTITY()
				--insert the transport
				insert into CoinTransports (EventID,OriginVillageID,DestinationVillageID,Amount,TripDuration,Direction,Reserved)values(@EventID,@OriginVillageID,@DestinationVillageID,@Amount,@TripDuration,@TripDirection,@Reserved )	
				if @Reserved =1
					begin 
						exec uVillageCoins_Add @DestinationVillageID, @Amount, @TrasuryOverflow output
					end
				
				set @Error=0;   
				select @Error;	
			end
		else
			begin
				set @Error=5
				select @Error;
				rollback
				return
			end
	commit tran
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iTranportCoins FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @OriginVillageID' + ISNULL(CAST(@OriginVillageID AS VARCHAR(25)), 'Null') + CHAR(10)
		+ '   @DesctinationVillageID' + ISNULL(CAST(@DestinationVillageID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @Amount' + ISNULL(CAST(@Amount AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @TripDuration' + ISNULL(CAST(@TripDuration AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @TripDirection' + ISNULL(CAST(@TripDirection AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @TravelTime' + ISNULL(CAST(@TravelTime AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @EventID' + ISNULL(CAST(@EventID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @Error' + ISNULL(CAST(@Error AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

--
-- say that the village has changed. this is done deliberately outside of the main tran and try 
--
UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @PlayerID and VillageID = @OriginVillageID and CachedItemID = 0
IF (@@rowcount < 1 ) BEGIN
	INSERT INTO VillageCacheTimeStamps values (@PlayerID, @OriginVillageID, 0, getdate())
END

if @Reserved =1 begin 
	-- if reserved, the coins are there immediatelly, so trigger refresh of this village
	UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @PlayerID and VillageID = @DestinationVillageID and CachedItemID = 0
	IF (@@rowcount < 1 ) BEGIN
		INSERT INTO VillageCacheTimeStamps values (@PlayerID, @DestinationVillageID, 0, getdate())
	END
END

GO