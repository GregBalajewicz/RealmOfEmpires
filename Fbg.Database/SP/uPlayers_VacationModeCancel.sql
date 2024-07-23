IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayers_VacationModeCancel')
	BEGIN
		DROP  Procedure  uPlayers_VacationModeCancel
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--
-- cancels vacation mode: NULLs request date, calculates days used, updates it and returns it
--
CREATE Procedure [dbo].uPlayers_VacationModeCancel
		@PlayerID int
AS
	
begin try 
	
	declare @now datetime2;
	declare @requestDate datetime2;
    declare @daysUsed int;
    declare @daysUsedNewSum int;   
    declare @realmDelayDays int;
    declare @perUseMin int;
    declare @perUseMax int;
    
    --the number of days before vacation becomes effective, after it is requested, determined by realm attribute
    select @realmDelayDays =cast(attribValue as int) FROM RealmAttributes where attribID = 53
    IF @realmDelayDays is null BEGIN 
        select -1;
        RETURN -1;
    END
    
    --minimum number of days vacation mode uses up from player's days, even if canceled early, determined by realm attribute
    select @perUseMin =cast(attribValue as int) FROM RealmAttributes where attribID = 54
    IF @perUseMin is null BEGIN 
        select -1;
        RETURN -1;
    END
    
    --request date is when player clicked to activate vacation mode, it becomes effective after a delay
	--if request date is already null then it was already canceled or something is wrong, abort.
	select @requestDate = VacationModeRequestOn FROM Players WHERE PlayerID = @PlayerID
    IF @requestDate IS NULL BEGIN
		select -1;
        RETURN -1;
    END
    
    --get the currently used player days
	select @daysUsed = VacationModeDaysUsed FROM Players WHERE PlayerID = @PlayerID
    IF @daysUsed IS NULL BEGIN --should never be null, but its a sanity check
		select -1;
        RETURN -1;
    END

	--the number of days VM can be on consecutivley, in a single use
    select @perUseMax =cast(attribValue as int) FROM RealmAttributes where attribID = 66
    IF @perUseMax is null BEGIN 
        set @perUseMax = 9999;
    END
      
    --calculates days used, difference of request date and now, minus the realm vacation activiation delay
    set @now = getdate();
    set @daysUsedNewSum = (DATEDIFF(day, @requestDate, @now)) - @realmDelayDays;
    
    --if days used are less than minimum per use, deduct the minimum
    IF @daysUsedNewSum < @perUseMin BEGIN
		set @daysUsedNewSum = @perUseMin;
    END
    
    --if days used are more than maximum per use, cap usage at maximum
    IF @daysUsedNewSum > @perUseMax BEGIN
		set @daysUsedNewSum = @perUseMax;
    END
    
    set @daysUsedNewSum = @daysUsed + @daysUsedNewSum;
	
	begin tran
	
		update Players set VacationModeRequestOn = NULL, VacationModeLastEndOn = @now, VacationModeDaysUsed = @daysUsedNewSum WHERE PlayerID = @PlayerID;
		
        IF @@ROWCOUNT = 1 BEGIN 
            select VacationModeDaysUsed from players WHERE PlayerID = @PlayerID
        END
        
	commit tran

end try

begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'uPlayers_VacationModeCancel FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @realmDelayDays' + ISNULL(CAST(@realmDelayDays AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @perUseMin' + ISNULL(CAST(@perUseMin AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @perUseMax' + ISNULL(CAST(@perUseMax AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @requestDate' + ISNULL(CAST(@requestDate AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @daysUsed' + ISNULL(CAST(@daysUsed AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	


   