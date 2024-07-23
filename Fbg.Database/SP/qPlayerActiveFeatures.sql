IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerActiveFeatures')
	BEGIN
		DROP  Procedure  qPlayerActiveFeatures
	END

GO
CREATE Procedure [dbo].[qPlayerActiveFeatures]
		@PlayerID int
AS
	
	
begin try 
	--drop table #temp
	create table #temp(FeatureID  int, Duration int,ExpiresOn datetime)
	insert into #temp

	select PFs.FeatureID ,
		PFPackages.Duration ,
		PlayersPFPackages.ExpiresOn
	from PFs
	join PFsInPackage on
		PFsInPackage.FeatureID=PFs.FeatureID
	join PFPackages on
		PFPackages.PFPackageID=PFsInPackage.PFPackageID
	join PlayersPFPackages on
		PlayersPFPackages.PFPackageID=PFsInPackage.PFPackageID
	where PlayerID=@PlayerID 
	union all
	select PFs.FeatureID ,
		PFTrails.Duration,
		PlayerPFTrials.ExpiresOn
	from PFs 
	join PFTrails on
		PFTrails.FeatureID=PFs.FeatureID
	join PlayerPFTrials on
		PlayerPFTrials.PFTrialID=PFTrails.PFTrialID
	where playerid=@PlayerID

    --
    -- get PF that are not yet expired 
    --
	select distinct 
	FeatureID ,
	max(ExpiresOn) 
	from #temp
	where ExpiresOn>getdate()
	group by 
	FeatureID

    --
    -- get PF 
    --
	select distinct 
	FeatureID ,
	max(ExpiresOn) 
	from #temp
	group by 
	FeatureID

end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qPlayerActiveFeatures FAILED! ' +  + CHAR(10)
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