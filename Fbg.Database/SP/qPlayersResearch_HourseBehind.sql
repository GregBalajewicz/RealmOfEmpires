 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayersResearch_HourseBehind')
	BEGIN
		DROP  Procedure  qPlayersResearch_HourseBehind
	END

GO
CREATE Procedure qPlayersResearch_HourseBehind
	@PlayerID as int
	, @HoursBehind real OUTPUT
	, @PrintDebugInfo BIT = null


AS
begin try 
	DECLARE @DEBUG INT
	IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
		SET @DEBUG = 1
		SET NOCOUNT OFF
	END ELSE BEGIN
		SET @DEBUG = 0
		SET NOCOUNT ON
	END 
	IF @DEBUG = 1 SELECT 'BEGIN qPlayersResearch_HourseBehind ' + cast(@PlayerID as varchar(10))

	declare @RealmOpenOn datetime
	declare @HoursOfCompleteResearch real
	declare @HoursOfResearchSpeedupUsed real
	declare @HoursOfCompletedResearchCurrentlyUpgrading real
	declare @HoursOnThisRealm real
	declare @HoursNoUsedForResearch real

	select @RealmOpenOn = OpenOn from realm

	select @HoursOfCompleteResearch = sum(ResearchTime / 10000 / 1000 / 60 /60.0)  from PlayerResearchItems pri JOIN ResearchItems RI on Ri.ResearchItemID = PRI.ResearchItemID
		where playerid = @playerID and PriceInCoins = 0 
	SET @HoursOfCompleteResearch  = ISNULL(@HoursOfCompleteResearch , 0) 

	select @HoursOfResearchSpeedupUsed = cast(data as real) from PlayerFlags where playerid = @PlayerID and FlagID = 87
	SET @HoursOfResearchSpeedupUsed  = ISNULL(@HoursOfResearchSpeedupUsed , 0) 

	SET @HoursOnThisRealm = datediff(MINUTE, @RealmOpenOn, getdate()) / 60.0

	select @HoursOfCompletedResearchCurrentlyUpgrading = ( ( ResearchTime / 10000 / 1000 / 60.0) - DATEDIFF(minute, getdate(), eventtime) ) / 60.0 
		from ResearchInProgress RIP 
		JOIN Events E 
			on RIP.EventID = E.eventid 
		JOIN ResearchItems RI 
			on RI.ResearchItemID = RIP.ResearchItemID
		where Playerid = @playerid and status = 0
	SET @HoursOfCompletedResearchCurrentlyUpgrading  = ISNULL(@HoursOfCompletedResearchCurrentlyUpgrading , 0) 


	SET @HoursBehind =  @HoursOnThisRealm - ( ( @HoursOfCompleteResearch + @HoursOfCompletedResearchCurrentlyUpgrading) - @HoursOfResearchSpeedupUsed) 
	IF @HoursBehind < 0 SET @HoursBehind = 0 
	SELECT @HoursBehind
	
	IF @DEBUG = 1 select @HoursOfResearchSpeedupUsed as '@HoursOfResearchSpeedupUsed', @HoursOfCompleteResearch as '@HoursOfCompleteResearch' , @HoursOnThisRealm as '@HoursOnThisRealm'
		, @HoursOfCompletedResearchCurrentlyUpgrading as '@HoursOfCompletedResearchCurrentlyUpgrading', @HoursBehind as '@HoursBehind'
	
	IF @DEBUG = 1 SELECT 'END qPlayersResearch_HourseBehind ' + cast(@PlayerID as varchar(10))
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qPlayersResearch_HourseBehind FAILED! ' +  + CHAR(10)
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




 go

