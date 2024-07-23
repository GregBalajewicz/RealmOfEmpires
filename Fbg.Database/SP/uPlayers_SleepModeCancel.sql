IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayers_SleepModeCancel')
	BEGIN
		DROP  Procedure  uPlayers_SleepModeCancel
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--
-- return value is a single row, single column - either NULL if sleep mode was not cancelled
--	or the SleepModeActiveFrom after the cancel
--
CREATE Procedure [dbo].uPlayers_SleepModeCancel
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
        
	begin tran
	    declare @now datetime
	    set @now = getdate()
	
        update Players set SleepModeActiveFrom = dateadd(minute, -(@duration*60), @now)
            WHERE PlayerID = @PlayerID
                and 
                (
                    SleepModeActiveFrom > @now -- NOTE: See (1) below
                    OR dateadd(minute, @duration*60, SleepModeActiveFrom) > @now
                )
        IF @@ROWCOUNT = 1 BEGIN 
            select  SleepModeActiveFrom from players WHERE PlayerID = @PlayerID
        END
	commit tran
end try

--(1) I dont remember what this check " SleepModeActiveFrom > @now " is for. i think it was 
--	intended to be used to cancel sleep mode that is not yet active because of activation delay but 
--	we dont allow cancelling untill it is active so not sure if this is used. leaving it as is 

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uPlayers_SleepModeCancel FAILED! ' +  + CHAR(10)
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



    