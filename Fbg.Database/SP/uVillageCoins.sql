IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uVillageCoins')
	BEGIN
		DROP  Procedure  uVillageCoins
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


/*

*/
CREATE Procedure dbo.uVillageCoins
	@VillageID int
	,@CoinsToAdd int -- MUST BE A positive NUMBER!!
	,@CoinsOverflow int out	-- valid only if @CoinsToAdd > 0 and indicates the coins lost due to treasury limit
	, @CoinsAfter int out -- number of coins in the village after the update
	, @PrintDebugInfo BIT = null
AS

DECLARE @DEBUG INT
IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
	SET @DEBUG = 1
	SET NOCOUNT OFF
END ELSE BEGIN
	SET @DEBUG = 0
	SET NOCOUNT ON
END 
IF @DEBUG = 1 print 'BEGIN uVillageCoins ' 

__RETRY__:
begin try  
	begin tran 
		--
		-- Obtain a village LOCK
		--		this will ensure that no one else will change the buildings in the village, or effect the building Q 
		--
		update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @VillageID

		--
		-- IF COINS < 0 then subtract - NOT YET IMPLEMENTED  
		--
		IF @CoinsToAdd < 0 BEGIN
			RAISERROR('NOT YET IMPLEMENTED  - @CoinsToAdd < 0 ',11,1)	
		END ELSE BEGIN
			EXEC dbo.uVillageCoins_Add
				@VillageID
				, @CoinsToAdd
				, @CoinsOverflow out	
				, @DEBUG
		END
		
		select @CoinsAfter = coins FROM Villages where VillageID = @VillageID
		
	COMMIT TRAN 
	IF @DEBUG = 1 print 'END uVillageCoins ' 
end try
begin catch
	IF @@TRANCOUNT > 0 ROLLBACK

	DECLARE @ERROR_MSG AS VARCHAR(max)

	SET @ERROR_MSG = 'uVillageCoins FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CoinsToAdd' + ISNULL(CAST(@CoinsToAdd AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CoinsOverflow' + ISNULL(CAST(@CoinsOverflow AS VARCHAR(10)), 'Null') + CHAR(10)
		
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














 