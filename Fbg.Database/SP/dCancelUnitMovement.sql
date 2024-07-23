IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dCancelUnitMovement')
	BEGIN
		DROP  Procedure  dCancelUnitMovement
	END

GO

CREATE Procedure dCancelUnitMovement
	@EventID as bigint
	,@NewEventTime DateTime
	,@EventsOwnerPlayerID int
AS


declare @morale_isactive as bit
declare @morale_returnAmount as int
declare @DestinationPlayerID as int 
declare @junk int


BEGIN TRY
	begin tran 
		--
		-- First we set the event as completed so that no one, in the mean time, can cancel it, or do anything else with it
		--
		UPDATE Events SET [Status] =1 WHERE EventID = @EventID AND [Status] = 0
		IF @@rowcount <> 1 BEGIN
			-- IF no rows where updated, then the event must have been cancelled (or something like this) thereforecs we abort quietly
			ROLLBACK 
			INSERT INTO ErrorLog VALUES (getdate(), 0, 'dCancelUnitMovement : update UPDATE Events SET [Status] =1 resulted in no rows updated',  ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null'))
			goto __DONE__
		END 
		
		--
		-- return morale if 
		--	- this is a morale enabled realm
		--  - this attack took morale (say, like spies) 
		--	- was cancelled within 3 minutes (that is, new event time is less than 3 minutes from now) 
		--
		IF dateadd(minute, 3, getdate()) >= @NewEventTime BEGIN
			select @morale_isactive = AttribValue from RealmAttributes where attribid = 70
			if @morale_isactive=1 BEGIN			
				-- if this command took morale, then we'll have a record of it here. if not, this will not return any rows
				select @morale_returnAmount = AttribValue from  UnitMovements_Attributes where AttribID = 2 and EventID = @EventID
			END
		END
		SET @morale_returnAmount  = isnull(@morale_returnAmount ,0) -- making sure this is never null, for easier checking later on. 

		
		--
		-- Update the command type to return 
		--	make sure this command is an attack or support, make sure this player belongs to this player
		update UnitMovements 
			set CommandType = dbo.fnGetUnitMovementReturnType(CommandType) --Return type depends on original command type
			, OriginVillageID = DestinationVillageID
			, DestinationVillageID = OriginVillageID 
			, VisibleToTarget = 2 -- this make the return visible, just in case the visibility of the attack was 0 or 1
			where EventID = @EventID
			
			AND CommandType in (0,1) -- MAKE sure we only cancel attack or support. If this event is not one of those, then something is wrong, perhaps this was called twice for the same event id
			AND OriginVillageID in (select VillageID from Villages where OwnerPlayerID = @EventsOwnerPlayerID) -- For security. 
		IF @@rowcount <> 1 BEGIN
			-- IF no rows where updated, then the event must have been cancelled already
			ROLLBACK 
			INSERT INTO ErrorLog VALUES (getdate(), 0, 'dCancelUnitMovement : update UnitMovements resulted in no rows updated',  ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null'))		
			goto __DONE__
		END 

		-- Update the event time to be battle time + travel time; Set status to not process, ie 0
		update Events set EventTime = @NewEventTime
			, Status = 0
			where Events.EventID = @EventID

		--
		-- return morale
		--
		IF @morale_returnAmount > 0 BEGIN 
			EXEC uPlayerMorale_SubtractAdd_Unsafe @EventsOwnerPlayerID, @morale_returnAmount, @junk out		
		END 
		

		--
		-- update cache time stamps
		--

		-- outgoing cache item (2) for owner of the command
		UPDATE PlayerCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @EventsOwnerPlayerID and CachedItemID = 2  
		IF (@@rowcount < 1 ) BEGIN
			INSERT INTO PlayerCacheTimeStamps values (@EventsOwnerPlayerID, 2, getdate())
		END
		-- incoming cache item (1) for owner of the command
		UPDATE PlayerCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @EventsOwnerPlayerID and CachedItemID = 1  
		IF (@@rowcount < 1 ) BEGIN
			INSERT INTO PlayerCacheTimeStamps values (@EventsOwnerPlayerID, 1, getdate())
		END
		-- incoming cache item (1) for the original destination of the command
		UPDATE PlayerCacheTimeStamps SET TimeStamp = getdate() 
			where PlayerID = (select ownerplayerid from villages where villageid = (select OriginVillageID from UnitMovements UnitMovements where EventID = @EventID)) 
			and CachedItemID = 1  
		IF (@@rowcount < 1 ) BEGIN
			INSERT INTO PlayerCacheTimeStamps select ownerplayerid, 1, getdate() from villages where villageid = (select OriginVillageID from UnitMovements UnitMovements where EventID = @EventID)											  
		END


	COMMIT tran 

__DONE__:
end try
begin catch
    IF @@TRANCOUNT > 0 ROLLBACK
	DECLARE @ERROR_MSG AS VARCHAR(8000)

	SET @ERROR_MSG = 'dCancelUnitMovement FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @EventID' + ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @NewEventTime' + ISNULL(CAST(@NewEventTime AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

GO
