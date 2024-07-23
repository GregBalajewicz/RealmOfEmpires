IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iCompleteBuildingUpgrade')
	BEGIN
		DROP  Procedure  iCompleteBuildingUpgrade
	END

GO

CREATE Procedure iCompleteBuildingUpgrade
	@EventID as int
	,@VillageID as int
	,@buildingID as int
	,@level as int -- THIS IS IGNORED!! the SP now just upgrade the building to the NEXT level, no matter what is passed
AS
	declare @CompletedOn as DateTime
	declare @CurrentLevel int
	declare @PlayerID int
	DECLARE @ERROR_MSG AS VARCHAR(max)
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
			
			--
			-- recruit units in case the building upgraded is a recruitment building. 
			--	its critical we call it BEFORE we do the actual upgrade so that troops recruit at the speed
			--	of the building before the upgrade. 
			--
			exec iCompleteUnitRecruitment @VillageID

			--
			-- Do the upgrade
			--
			select @CurrentLevel = [Level] from Buildings where @VillageID = Villageid and BuildingTypeID =@BuildingID
			if 	not @CurrentLevel is null begin
				update Buildings set Level = @CurrentLevel + 1 where VillageID = @VillageID and BuildingTypeID=@BuildingID
			end else begin 
				insert into Buildings values (@Villageid, @BuildingID, 1)
			end
			--
			-- update the village's and player's points
			--
			set @PlayerID = (select OwnerPlayerID FROM Villages where VillageID = @VillageID)
			EXEC uPoints_Village @VillageID
			EXEC uPoints_Player @PlayerID
			update players set UpdateCompletedQuests = 1 where playerid = @PlayerID -- invalidate quests			

			--
			-- grab another upgrade from the Q (if any) 
			--	
			exec iBuildingUpgrade_GetOneFromQ @VillageID, @CompletedOn, 0
		END 

	commit tran
end try
begin catch
	 IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iCompleteBuildingUpgrade FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @EventID' + ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @buildingID' + ISNULL(CAST(@buildingID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @level' + ISNULL(CAST(@level AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @CurrentLevel' + ISNULL(CAST(@CurrentLevel AS VARCHAR(100)), 'Null') + CHAR(10)
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
-- **************************************************************
--
-- **************************************************************
--
-- Send notification if needed. 
-- THIS IS DONE DELIBERATELY outside of transaction and first try block because we dont want this to fail the entire SP
--
-- **************************************************************
--
-- **************************************************************

begin try 

	--
	-- if this village is not upgrading anything now
	--
	if not exists(select * from events E join BuildingUpgrades BU on E.EventID = BU.EventID and e.Status = 0 and VillageID = @VillageID) BEGIN		

		if exists(select * from vPlayerNotificationSettings PNS where PNS.PlayerID = @PlayerID and NotificationID = 4/*upgrade completed notif type*/ and PNS.isActive = 1 ) BEGIN
			INSERT INTO PlayerNotifications(
				NotificationTypeID    ,
				PlayerID              ,
				Text)
				select 7,@PlayerID
					, name 
						+ dbo.Translate('iCompleteBuildingUpgrade_notif_Part1')
						+ cast(isnull(@CurrentLevel,0)+1 as varchar(11)) 
						+ dbo.Translate('iCompleteBuildingUpgrade_notif_Part2') from BuildingTypes where BuildingTypeID = @buildingID	
		END
	END 

end try
begin catch
	
	SET @ERROR_MSG = 'iCompleteBuildingUpgrade - notification insert FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @EventID' + ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @buildingID' + ISNULL(CAST(@buildingID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @level' + ISNULL(CAST(@level AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @CurrentLevel' + ISNULL(CAST(@CurrentLevel AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)

	INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'iCompleteBuildingUpgrade - notification insert FAILED', @ERROR_MSG)		

end catch	

if @PlayerID is not null BEGIN -- if null, that means we did not actually do anythig
	--
	-- say that the village has changed. this is done deliberately outside of the main tran and try 
	--
	UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @PlayerID and VillageID = @VillageID and CachedItemID = 0
	IF (@@rowcount < 1 ) BEGIN
		INSERT INTO VillageCacheTimeStamps values (@PlayerID, @VillageID, 0, getdate())
	END
END

GO

