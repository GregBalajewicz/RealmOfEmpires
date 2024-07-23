    
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iStartLevel_Items2')
	BEGIN
		DROP  Procedure  iStartLevel_Items2
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].iStartLevel_Items2
		 @userid uniqueidentifier 
	, @PlayerID int
	, @StartLevelID int
	, @RealmID int

AS

begin try 

	EXEC VillageStartLevels_BuildingSpeedup_i @userid, @PlayerID, @StartLevelID, @RealmID
	EXEC VillageStartLevels_ResearchSpeedup_i @userid, @PlayerID, @StartLevelID, @RealmID

end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iStartLevel_Items2 FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @userid' + ISNULL(CAST(@userid AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 