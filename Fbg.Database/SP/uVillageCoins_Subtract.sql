IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uVillageCoins_Subtract')
	BEGIN
		DROP  Procedure  uVillageCoins_Subtract
	END

GO

--
-- The param @now tells the SP what is the autoritative time. 
--  if this is null, the sp get the time using GETDATE(). 
--  this param is left as optional for only one reason - to support code that does not send this param but eventually all code should do this. 
--
CREATE Procedure [dbo].[uVillageCoins_Subtract]
	@VillageID int
	,@CoinsToSubtract int
	, @Sucess bit OUTPUT --  1 -  coins were successfully subtracted, 0 otherwise (not enough coins)
	, @PrintDebugInfo BIT = null
	, @now DateTime = null
	
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
IF @DEBUG = 1 print @DBG + 'BEGIN uVillageCoins_Subtract ' 


begin try 

	set @Sucess = 1

	-- Update village coins, & get current
	exec UpdateVillageCoins @VillageID, @now

	UPDATE Villages SET Coins =  Coins - @CoinsToSubtract, coinslastupdates = getdate() 
		WHERE VillageID = @VillageID 
		AND Coins - @CoinsToSubtract >= 0 
	IF @@rowcount <> 1 BEGIN
		-- IF no rows where updated, then subtract failed; assuming the villageID was correct, it must have been because there is not enough coins. 
		IF @DEBUG = 1 print '@CoinsToSubtract=' + cast(@CoinsToSubtract  as varchar)+' is greater then the villages coins. '
		INSERT INTO ErrorLog VALUES (getdate(), 0, '@CoinsToSubtract=' + cast(@CoinsToSubtract  as varchar)+' is greater then the villages coins.', @VillageID)		
		set @Sucess = 0
	END 

IF @DEBUG = 1 print @DBG + 'END uVillageCoins_Subtract ' 
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)

	SET @ERROR_MSG = 'uVillageCoins_Subtract FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)

		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CoinsToSubtract' + ISNULL(CAST(@CoinsToSubtract AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @Sucess' + ISNULL(CAST(@Sucess AS VARCHAR(10)), 'Null') + CHAR(10)

		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	
 