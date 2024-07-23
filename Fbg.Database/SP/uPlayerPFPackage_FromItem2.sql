   IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayerPFPackage_FromItem2')
	BEGIN
		DROP  Procedure  uPlayerPFPackage_FromItem2
	END

GO

CREATE Procedure dbo.uPlayerPFPackage_FromItem2
	@PlayerID as int,
	@PackageID as int,
	@DurationInMin as int
AS
begin try  
		
	--
	-- before extending the PF, update the players coins IF the package we are extending 
	--  has the BONUS SILVER PF. WHY?!? so that all village's coins are up to date before the bonus goes 
	--  into effect - all coin calculation code depends on this!
	--				
	IF EXISTS
		(select * 
	        from PFsInPackage 
	        join PFPackages on
		        PFPackages.PFPackageID=PFsInPackage.PFPackageID
	        where PFsInPackage.FeatureID = 24 -- BONUS SILVER PF
	            AND PFPackages.PFPackageID = @PackageID
		) 
	BEGIN		    
		exec uUpdatePlayerCoins @PlayerID
	END
		

	IF not exists (select * from PlayersPFPackages where PlayerID = @PlayerID and  PFPackageID = @PackageID ) BEGIN		
		insert into PlayersPFPackages 
			(PlayerID,PFPackageID,ExpiresOn) 
		values
			(@PlayerID,@PackageID, DATEADD(minute, round(@DurationInMin,0),GETDATE()))
	END ELSE BEGIN
		update PlayersPFPackages 
			set ExpiresOn= DATEADD(minute,round(@DurationInMin,0),dbo.fnGetMax(ExpiresOn,getdate()))
			where PFPackageID=@PackageID
				and PlayerID = @PlayerID
	END
	
	
  end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uPlayerPFPackage_FromItem2 FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID'				  + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @PackageID'				  + ISNULL(CAST(@PackageID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @DurationInMin'			  + ISNULL(CAST(@DurationInMin AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)

	
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO