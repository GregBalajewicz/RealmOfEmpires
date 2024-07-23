IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uVillageUnits')
	BEGIN
		DROP  Procedure  uVillageUnits
	END

GO

CREATE Procedure uVillageUnits
	@VillageID int
    , @UnitTypeID int
    , @NumToAdd int -- must be >0
	, @PrintDebugInfo BIT = null
AS

DECLARE @DEBUG INT, @DBG varchar(10)
IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
	SET @DEBUG = 1
	set @DBG = ':::::::'
	SET NOCOUNT OFF
END ELSE BEGIN
	SET @DEBUG = 0
	SET NOCOUNT ON
END 
IF @DEBUG = 1 print @DBG + 'BEGIN uVillageUnits ' + cast(@VillageID AS VARCHAR(100))


__RETRY__:

begin try 
	begin transaction	
	--
	-- Obtain lock on village
	--
	update VillageSemaphore set TimeStamp = getdate() 
		where VillageID = @VillageID
	
    IF @NumToAdd > 0 BEGIN 
	    if exists(select * from VillageUnits where VillageId =@VillageID and UnitTypeID = @UnitTypeID) BEGIN
		    IF @DEBUG = 1 print @DBG + 'Doing update VillageUnits '
		    update VillageUnits 
			    set TotalCount = TotalCount + @NumToAdd 
			    , CurrentCount = CurrentCount + @NumToAdd
			    where VillageId =@VillageID and UnitTypeID = @UnitTypeID
	    END ELSE BEGIN
		    IF @DEBUG = 1 print @DBG + 'Doing insert into VillageUnits '
		    insert into VillageUnits (VillageID, UnitTypeID, TotalCount, CurrentCount )
			    values(@villageID, @UnitTypeID, @NumToAdd, @NumToAdd)
	    END 
    END 
	IF @DEBUG = 1 print @DBG + 'About to commit. @@TRANCOUNT='+ cast(@@TRANCOUNT AS VARCHAR(100))
	commit transaction
	IF @DEBUG = 1 print @DBG + 'Commited. @@TRANCOUNT='+ cast(@@TRANCOUNT AS VARCHAR(100))
	
	
	IF @DEBUG = 1 print @DBG + 'END uVillageUnits ' + cast(@VillageID AS VARCHAR(100))

end try
begin catch
    IF @@TRANCOUNT > 0 ROLLBACK
	DECLARE @ERROR_MSG AS VARCHAR(max)

	
	SET @ERROR_MSG = 'uVillageUnits FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @UnitTypeID' + ISNULL(CAST(@UnitTypeID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @NumToAdd' + ISNULL(CAST(@NumToAdd AS VARCHAR(100)), 'Null') + CHAR(10)
		
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

end catch	

declare @PlayerID int 
select @PlayerID = OwnerPlayerID from villages where VillageID = @VillageID
UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @PlayerID and VillageID = @VillageID and CachedItemID = 0
IF (@@rowcount < 1 ) BEGIN
	INSERT INTO VillageCacheTimeStamps values (@PlayerID, @VillageID, 0, getdate())
END


GO
