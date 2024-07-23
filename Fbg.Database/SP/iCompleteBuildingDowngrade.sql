 
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iCompleteBuildingDowngrade')
	BEGIN
		DROP  Procedure  iCompleteBuildingDowngrade
	END

GO

CREATE Procedure iCompleteBuildingDowngrade
	@EventID as int
AS
	declare @CompletedOn as DateTime
	declare @CurrentLevel int
	declare @PlayerID int
	declare @VillageID as int
	declare @buildingID as int
	declare @MinLevel as int
__RETRY__:
begin try 
	begin tran
		--
		-- Obtain a village LOCK
		--		this will ensure that no one else will change the buildings in the village, or effect the building Q 
		--
		update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @VillageID
		
		--
		-- First we set the event as completed so that no one, in the mean time, can cancel it
		--		IF no rows where updated, then the event must have been cancelled (or something like this) therefore we do nothing
		--
		UPDATE Events SET [Status] =1 WHERE EventID = @EventID AND [Status] = 0
		IF @@rowcount = 1 BEGIN			
			select @CompletedOn = EventTime FROM Events WHERE EventID = @EventID 
			
			select 
			    @VillageID = VillageID ,
                @buildingID = BuildingTypeID 
			    FROM BuildingDowngrades where EventID = @EventID
			
			SELECT @MinLevel = MinimumLevelAllowed  
			    FROM buildingtypes where BuildingTypeID = @BuildingID
			--
			-- recruit units in case the building upgraded is a recruitment building. 
			--	its critical we call it BEFORE we do the actual upgrade so that troops recruit at the speed
			--	of the building before the upgrade. 
			--
			exec iCompleteUnitRecruitment @VillageID

			--
			-- Do the downgrade
			--
			select @CurrentLevel = [Level] from Buildings where Villageid = @VillageID and BuildingTypeID =@BuildingID
			IF 	@CurrentLevel is not null begin
			    IF @CurrentLevel - 1 >= @MinLevel BEGIN
			        IF @CurrentLevel - 1 = 0 BEGIN
			            DELETE Buildings where  VillageID = @VillageID and BuildingTypeID=@BuildingID
			        END ELSE BEGIN
                        update Buildings set Level = Level - 1 where VillageID = @VillageID and BuildingTypeID=@BuildingID
			        END
			    END
			end
			--
			-- update the village's and player's points
			--
			set @PlayerID = (select OwnerPlayerID FROM Villages where VillageID = @VillageID)
			EXEC uPoints_Village @VillageID
			EXEC uPoints_Player @PlayerID
			--
			-- grab another upgrade from the Q (if any) 
			--	
			exec iBuildingDowngrade_GetOneFromQ @VillageID, @CompletedOn, 1
		END 

	commit tran
end try
begin catch
	 IF @@TRANCOUNT > 0 ROLLBACK

	DECLARE @ERROR_MSG AS VARCHAR(max)
	
	SET @ERROR_MSG = 'iCompleteBuildingDowngrade FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @EventID' + ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @buildingID' + ISNULL(CAST(@buildingID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @CurrentLevel' + ISNULL(CAST(@CurrentLevel AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @MinLevel' + ISNULL(CAST(@MinLevel AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
		
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
		
	RAISERROR(@ERROR_MSG,11,1)	

end catch	


--
-- say that the village has changed. this is done deliberately outside of the main tran and try 
--
UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @PlayerID and VillageID = @VillageID and CachedItemID = 0
IF (@@rowcount < 1 ) BEGIN
	INSERT INTO VillageCacheTimeStamps values (@PlayerID, @VillageID, 0, getdate())
END

GO

