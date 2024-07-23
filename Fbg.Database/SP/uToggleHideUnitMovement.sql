 
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uToggleHideUnitMovement')
	BEGIN
		DROP  Procedure  uToggleHideUnitMovement
	END

GO

CREATE Procedure uToggleHideUnitMovement
	@EventID as bigint
	,@PlayerID int
AS

BEGIN TRY

    IF EXISTS (SELECT * FROM UnitMovements_PlayerAttributes where EventID = @EventID and PlayerID = @PlayerID and AttribID = 1) BEGIN
        delete from UnitMovements_PlayerAttributes where EventID = @EventID and PlayerID = @PlayerID and AttribID = 1
    END ELSE BEGIN
        -- we do not need to ensure that the event belongs to the player since playerid sent in is controlled by the BLL so we are 
        --  guaranteed it is the ID of the logged in player therefore at most the player can hide/unhide an event for him self that does not 
        --  belong to him but this cannot effect other players so its OK
        Insert into UnitMovements_PlayerAttributes( EventID, PlayerID, AttriBID) 
            SELECT EventID, @PlayerID, 1 from UnitMovements 
                WHERE EventID = @EventID 
                    /*
                    AND 
                    ( 
                        -- we need to ensure that the event belongs to the 
                        DestinationVillageID in (select villageId from Villages where ownerPlayerid = @PlayerID)
                        or OriginVillageID in (select villageId from Villages where ownerPlayerid = @PlayerID)
                    )
                    */
    END
    --
    -- now return the current hidden status of the troop movements.
    --  if it is hidden, we will return 1 <- AKA hidden 
    --  if it is not hidden, we will return 0 <- AKA NOT Hidden
    --
    declare @curState smallint
    
    SELECT @curState = AttribID
        FROM UnitMovements_PlayerAttributes 
            where EventID = @EventID and PlayerID = @PlayerID and AttribID = 1
    
    SELECT isnull(@curState, 0)

__DONE__:
end try
begin catch
    IF @@TRANCOUNT > 0 ROLLBACK
	DECLARE @ERROR_MSG AS VARCHAR(max)

	SET @ERROR_MSG = 'uToggleHideUnitMovement FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @EventID' + ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

GO
