   IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayerPFPackage')
	BEGIN
		DROP  Procedure  uPlayerPFPackage
	END

GO

CREATE Procedure dbo.uPlayerPFPackage
	@PlayerID as int,
	@PackageID as int
AS
begin try  
	declare @Cost as int
	declare @Duration as real
	
	IF not exists (select * from PlayersPFPackages where PlayerID = @PlayerID and  PFPackageID = @PackageID ) BEGIN
		EXEC iPlayerPFPackage @PlayerID, @PackageID
	END ELSE BEGIN

		select @Cost=Cost ,
			   @Duration=Duration 
		from PFPackages 
		where PFPackageID=@PackageID
		
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
		
		if @Duration < 1 BEGIN
			update PlayersPFPackages 
				set ExpiresOn= DATEADD(hour,round(@Duration  * 24,0),dbo.fnGetMax(ExpiresOn,getdate()))
				where PFPackageID=@PackageID
					and PlayerID = @PlayerID
		END ELSE BEGIN 
			update PlayersPFPackages 
				set ExpiresOn= DATEADD(day,@Duration,dbo.fnGetMax(ExpiresOn,getdate()))
				where PFPackageID=@PackageID
					and PlayerID = @PlayerID
		END

	
			
		-- log the process of extending new package
			insert into PlayerPFLog
			(PlayerID,Time ,EventType,Credits ,Cost,notes)
			values
			(@PlayerID,getdate(),3,@Cost,-1, @PackageID)
			
		select @Cost
	END
  end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uPlayerPFPackage FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID'				  + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @PackageID'				  + ISNULL(CAST(@PackageID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO