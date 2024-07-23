IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayers_SleepMode')
	BEGIN
		DROP  Procedure  uPlayers_SleepMode
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--
-- return value is a single row, single column - either NULL if sleep mode was not activated
--	or the date that the sleep mode will go active on.
--
CREATE Procedure [dbo].uPlayers_SleepMode
		@PlayerID int
AS
	
begin try 
    declare @duration real
    declare @timeTillActiveFromActivation real 
    declare @AvailOnceEveryXHours real
    
    select @duration=cast(attribValue as real) FROM RealmAttributes where attribID = 16
    IF @duration is null OR @duration = 0 BEGIN 
        RETURN;
    END
    select @timeTillActiveFromActivation=cast(attribValue as real) FROM RealmAttributes where attribID = 17
    select @AvailOnceEveryXHours=cast(attribValue as real) FROM RealmAttributes where attribID = 18
    IF @timeTillActiveFromActivation is null OR @AvailOnceEveryXHours is null BEGIN 
        RETURN;
    END
    
	begin tran
	    declare @now datetime
	    set @now = getdate()
	
        update Players set SleepModeActiveFrom = dateadd(minute, @timeTillActiveFromActivation*60, @now)
            WHERE PlayerID = @PlayerID
                and 
                (
                    SleepModeActiveFrom is null 
                    OR dateadd(hour, 0-@timeTillActiveFromActivation+@AvailOnceEveryXHours, SleepModeActiveFrom) < @now
                )
        IF @@ROWCOUNT = 1 BEGIN 
            select  SleepModeActiveFrom from players WHERE PlayerID = @PlayerID
        END
	commit tran
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uPlayers_SleepMode FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	


/*

ALTERNATIVE VERSION that I worked on, but did not finish, that allows setting sleep mode much further ahead of time 

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayers_SleepMode')
	BEGIN
		DROP  Procedure  uPlayers_SleepMode
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--
-- return value is a single row, single column - either NULL if sleep mode was not activated
--	or the date that the sleep mode will go active on.
--
-- NOTE - no business rules are checked here, by this SP
--
CREATE Procedure [dbo].uPlayers_SleepMode
		@PlayerID int
		, @ActivateOn int -- leave blank if you want it to activate asap from now
AS
	
begin try 
	
    declare @duration real
    declare @timeTillActiveFromActivation real 
    declare @AvailOnceEveryXHours real
    
    select @duration=cast(attribValue as real) FROM RealmAttributes where attribID = 16
    IF @duration is null OR @duration = 0 BEGIN 
        RETURN;
    END
    select @timeTillActiveFromActivation=cast(attribValue as real) FROM RealmAttributes where attribID = 17
    select @AvailOnceEveryXHours=cast(attribValue as real) FROM RealmAttributes where attribID = 18
    IF @timeTillActiveFromActivation is null OR @AvailOnceEveryXHours is null BEGIN 
        RETURN;
    END
    	
	IF @ActivateOn is null BEGIN
		SET @ActivateOn = dateadd(minute, @timeTillActiveFromActivation*60, getdate())
	END
	
    update Players set SleepModeActiveFrom = @ActivateOn
        WHERE PlayerID = @PlayerID
              
    IF @@ROWCOUNT = 1 BEGIN 
        select  SleepModeActiveFrom from players WHERE PlayerID = @PlayerID
    END
	
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uPlayers_SleepMode FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



   

*/
   