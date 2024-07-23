IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayerMorale_SubtractAdd_Unsafe')
	BEGIN
		DROP  Procedure  uPlayerMorale_SubtractAdd_Unsafe
	END

GO

--
--  THIS DOES NOT CALL uPlayerMorale to get the proper morale!! you must call it befor calling this!!
--
CREATE Procedure [dbo].[uPlayerMorale_SubtractAdd_Unsafe]
	@PlayerID int
	,@MoraleToSubtractOrAdd int
	, @ActualMoraleDifference int output -- this is the actual morale difference that occured. If, for example marale is at max nagative, subtracting will not actually subtract anythjing
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
IF @DEBUG = 1 print @DBG + 'BEGIN uPlayerMorale_SubtractAdd_Unsafe ' 


begin try 

	IF @MoraleToSubtractOrAdd = 0 BEGIN 
		RETURN 
	END

	declare @morale_max as int
	declare @morale_min as int
	select @morale_min = AttribValue from RealmAttributes where attribid = 73 -- 'Morale System - min morale') 
	select @morale_max = AttribValue from RealmAttributes where attribid = 74 -- Morale System - max morale') 
	declare @morale_beforeUpdate as int 
	declare @morale_afterUpdate as int 
	SELECT @morale_beforeUpdate = morale from Players where PlayerID = @PlayerID

	UPDATE Players SET Morale = morale + @MoraleToSubtractOrAdd, moraleLastUpdated = getdate() 
		WHERE PlayerID = @PlayerID
		AND Morale +  @MoraleToSubtractOrAdd >=  @morale_min
		AND Morale +  @MoraleToSubtractOrAdd <=  @morale_max

	IF @@rowcount <> 1 BEGIN
		--
		-- we got over the max or min
		--
		if @MoraleToSubtractOrAdd < 0 BEGIN 
			UPDATE Players SET Morale = @morale_min, moraleLastUpdated = getdate() WHERE PlayerID = @PlayerID
		END ELSE BEGIN 
			UPDATE Players SET Morale = @morale_max, moraleLastUpdated = getdate() WHERE PlayerID = @PlayerID
		END
	END 

	SELECT @morale_afterUpdate = morale from Players where PlayerID = @PlayerID
	SET @ActualMoraleDifference = @morale_afterUpdate - @morale_beforeUpdate



IF @DEBUG = 1 print @DBG + 'END uPlayerMorale_SubtractAdd_Unsafe ' 
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)

	SET @ERROR_MSG = 'uPlayerMorale_SubtractAdd_Unsafe FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)

		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	
 