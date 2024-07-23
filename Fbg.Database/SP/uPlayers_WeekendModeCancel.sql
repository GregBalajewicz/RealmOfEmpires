IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayers_WeekendModeCancel')
	BEGIN
		DROP  Procedure  uPlayers_WeekendModeCancel
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--
-- cancels weekend mode: NULLS takes effect on, and sets last cancel date
--
CREATE Procedure [dbo].uPlayers_WeekendModeCancel
		@PlayerID int
AS
	
begin try 
	

	begin tran
	
		update Players set 
		WeekendModeTakesEffectOn = NULL, --end/cancel WM
		WeekendModeLastEndOn = GETDATE(), --record the date it ended/canceled
		

		SleepModeActiveFrom = DATEADD(minute,-725,GETDATE())
		WHERE PlayerID = @PlayerID;
		
        IF @@ROWCOUNT < 1 BEGIN 
            select -1;
        END
        ELSE BEGIN
			select 1;
        END

	commit tran

end try

begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'uPlayers_WeekendModeCancel FAILED! ' +  + CHAR(10)
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


   