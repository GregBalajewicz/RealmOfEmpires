 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uVillageName')
	BEGIN
		DROP  Procedure  uVillageName
	END

GO

CREATE Procedure uVillageName
	@VillageID int
	,@VillageName nvarchar(25)
AS

BEGIN TRY

	update Villages set [Name] = @VillageName
		where VillageID = @VillageID and @VillageName <> ''

	EXEC uTriggerUnitMovementCacheUpdateBasedOnVID @VillageID

end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'uVillageName FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @VillageName' + ISNULL(@VillageName, 'Null') + CHAR(10)

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
declare @PlayerID int 
select @PlayerID = OwnerPlayerID from villages where VillageID = @VillageID
UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @PlayerID and VillageID = @VillageID and CachedItemID = 0
IF (@@rowcount < 1 ) BEGIN
	INSERT INTO VillageCacheTimeStamps values (@PlayerID, @VillageID, 0, getdate())
END

