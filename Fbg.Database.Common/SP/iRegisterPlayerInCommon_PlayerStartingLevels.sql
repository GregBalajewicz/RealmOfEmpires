IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iRegisterPlayerInCommon_PlayerStartingLevels')
	BEGIN
		DROP  Procedure  iRegisterPlayerInCommon_PlayerStartingLevels
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO
CREATE Procedure dbo.iRegisterPlayerInCommon_PlayerStartingLevels
	@UserID uniqueidentifier ,
	@PlayerID varchar(50),
	@RealmID integer

AS

-- 
--
--
begin try 
	
						
		--
		-- HANDLING Start Levels Items / reward
		--
		-- this is happening outside of tran on purpose, because we are calling linked server
		--
		declare @StartLevelID int
		declare @DaysOld int 
		declare @now Datetime
		set @now = getdate() 


		select @daysold = datediff(day, OpenOn, @now)  from realms where realmid = @RealmID
		
		select top 1 @StartLevelID = StartLevelID
				from PlayerStartLevels 
				where @DaysOld <= realmmaxAgeInDays 
				order by realmmaxAgeInDays asc		

		if @StartLevelID is not null BEGIN 
			EXEC iStartLevel_Items2 @UserID, @PlayerID, @StartLevelID, @RealmID
		END
	
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iRegisterPlayerInCommon_PlayerStartingLevels FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @RealmID:'				  + ISNULL(CAST(@RealmID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @UserID:'					  + ISNULL(CAST(@UserID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @PlayerID:'				  + ISNULL(CAST(@PlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



