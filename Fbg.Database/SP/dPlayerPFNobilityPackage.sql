  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dPlayerPFNobilityPackage')
	BEGIN
		DROP  Procedure  dPlayerPFNobilityPackage
	END

GO

CREATE Procedure [dPlayerPFNobilityPackage]
	@PlayerID as int
	,@CalculateOnly as bit--this value enable the SP to only calculate the refund value without doing the refund
	,@RefundSum as int output
AS
begin try 
	declare @NPackageID as int
	declare @PackageID as int
	declare @i as int;
	declare @NumFeatureID as int;
	declare @RefundServant as int;
	set @RefundSum=0;
	set @NPackageID=999;--set to nobilty package id
	
	if not exists	(select * from PlayersPFPackages where PlayerID=@PlayerID and PFPackageID=@NPackageID and ExpiresOn > getdate() )						
	begin
		--
		-- packages that 'fall' under nobility package.
		--
		create table #tmp (PackageID int,  row int identity(1,1))
		insert into #tmp values (4) -- insert into PFPackages values (4,  25,  30) -- insert into PFs values (3, 'BuildingQ')
		insert into #tmp values (5) -- insert into PFPackages values (5,  25,  30) -- insert into PFs values (4, 'LargeMap')
		insert into #tmp values (6) -- insert into PFPackages values (6,  20,  30) -- insert into PFs values (5, 'SummaryPages')
		insert into #tmp values (7) -- insert into PFPackages values (7,  10,  30) -- insert into PFs values (9, 'IncomingTroopsToFromVillagePlayer') 
		insert into #tmp values (8) -- insert into PFPackages values (8,  5,   30) -- insert into PFs values (10, 'Notes') -- Notes on village or player
		insert into #tmp values (9) -- insert into PFPackages values (9,  5,   30) -- insert into PFs values (11, 'IncomingOutgoingOnVov') -- the outgoing and incoming troops list on your village overview
		insert into #tmp values (10) -- insert into PFPackages values (10, 10,  30) -- insert into PFs values (13, 'IncomingOutgoingAllVillages') -- 
		insert into #tmp values (11) -- insert into PFPackages values (11, 5,   30) -- insert into PFs values (12, 'IncomingOutgoingFilter') -- 
		insert into #tmp values (12) -- insert into PFPackages values (12, 10,  30) -- insert into PFs values (14, 'SupportAllVillage') -- 
		insert into #tmp values (13) --insert into PFPackages values (13, 20,  30) -- insert into PFs values (15, 'TagsAndFilters') -- 
		insert into #tmp values (14) -- insert into PFPackages values (14, 5,   30) -- insert into PFs values (16, 'ImprovedVOV') --
		insert into #tmp values (15) -- insert into PFPackages values (15, 25,  30) -- insert into PFs values (17, 'ConvinientSilverTransport') -- 
		insert into #tmp values (16) -- insert into PFPackages values (16, 20,  30) -- insert into PFs values (18, 'ReportImprovements') -- 
		insert into #tmp values (17) -- insert into PFPackages values (17, 20,  30) -- insert into PFs values (19, 'MessageImprovements') -- 
		insert into #tmp values (18) -- insert into PFPackages values (18, 10,  30) -- insert into PFs values (20, 'BattleSimImprovements') -- 
		insert into #tmp values (19) -- insert into PFPackages values (19, 10,  30) --  insert into PFs values (21, 'Command Troops Enhancements')
		insert into #tmp values (32) -- insert into PFPackages values (32, 30, 30) --  insert into PFs values (23, 'Mass Upgrade and and Recruit')


		
		
		select @NumFeatureID = count(*) from #tmp	
		set @i = 1
				
		--
		-- if doing a refund, make sure everything is in a transaction
		--
		IF @CalculateOnly = 0 BEGIN
			BEGIN TRAN
		END 
		
		WHILE @i <= @NumFeatureID BEGIN
			select @PackageID = PackageID from #tmp where row = @i
			exec @RefundServant=dPlayerPFPackages @PlayerID,@PackageID,1/*this param is ignored*/,@CalculateOnly,2,null;
			set @RefundSum=@RefundSum+@RefundServant;
			set @i = @i + 1 
			
		END
		
		if @CalculateOnly=0 begin
			exec iPlayerPFPackage @PlayerID,@NPackageID
		end
			
		--
		-- if doing a refund, make sure to commit the tran
		--
		IF @CalculateOnly = 0 BEGIN
			COMMIT TRAN
		END 			
			
		return @RefundSum
	end
	return 0;
	
  end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'dPlayerPFNobilityPackage FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID'				  + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @PackageID'				  + ISNULL(CAST(@PackageID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @CalculateOnly'		  + ISNULL(CAST(@CalculateOnly AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	





