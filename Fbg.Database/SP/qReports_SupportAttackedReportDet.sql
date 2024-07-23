 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qReports_SupportAttackedReportDet')
	BEGIN
		DROP  Procedure  qReports_SupportAttackedReportDet
	END

GO

CREATE Procedure qReports_SupportAttackedReportDet
	@PlayerID as int
	,@RecordID as int
	
AS
begin try

	Declare @ReportID as int

	select @ReportID=ReportID from ReportAddressees where RecordID=@RecordID

	select R.Time
		, R.Subject
		, RT.Name as ReportType
		, SP.Name as SupportingPlayerName
		, SP.PlayerID as SupportingPlayerID
		, SV.VillageID as SupportingVillageID
		, SV.Name as SupportingVillageName
		, SV.XCord as SupportingVillageXCord
		, SV.YCord as SupportingVillageYCord
		, isnull(RA.ForwardedByPlayerID,0) as ForwardedByPlayerID
		, RA.ForwardedOn
		, RA.RecordID
		, FP.Name
		, BR.DefenderPlayerID as SupportedPlayerID
		, DP.AvatarID as SupportedAvatarID
		, DP.Name as SupportedPlayerName
		, DV.Name as SupportedVillageName
		, DV.VillageTypeID as SupportedVillageTypeID
		, DV.Points as SupportedVillagePoints
		, DV.XCord as SupportedVillageX
		, DV.YCord as SupportedVillageY
		, DV.VillageID as SupportedVillageID
		, FP.AvatarID as ForwardedByPlayerAvatarID
		from reports R
		join ReportTypes RT
			on R.ReportTypeID = RT.ReportTypeID
		join SupportAttackedReports SAR
			on SAR.ReportID = R.ReportID
		join BattleReports BR
			on BR.ReportID = SAR.BattleReportID
		join BattleReportSupport BRS
			on BRS.ReportID = BR.ReportID
			and BRS.SupportingPlayerID = SAR.SupportingPlayerID			
		join Villages SV
			on SV.VillageID = BRS.SupportingVillageID
		join Villages DV
			on DV.VillageID = BR.DefenderVillageID
		join Players SP
			on SP.PlayerID = BRS.SupportingPlayerID
		join Players DP			
			on DP.PlayerID = BR.DefenderPlayerID
		join ReportAddressees RA
			on RA.PlayerID = @PlayerID
			and RA.RecordID = @RecordID
		left join Players FP
			on FP.PlayerID=RA.ForwardedByPlayerID
		where R.ReportID = RA.ReportID

	select
		BRSU.SupportingVillageID 
		, BRSU.UnitTypeID 
		, DeployedUnitCount
		, KilledUnitCount
		, ReaminingUnitCount
		FROM SupportAttackedReports SAR
		JOIN BattleReportSupport BRS
			on BRS.ReportID = SAR.BattleREportID
			and BRS.SupportingPlayerID = SAR.SupportingPlayerID
		JOIN BattleReportSupportUnits BRSU
			on BRSU.ReportID = SAR.BattleREportID
			and BRSU.SupportingVillageID = BRS.SupportingVillageID
		join UnitTypes UT
			on BRSU.UnitTypeID = UT.UnitTypeID
		where SAR.ReportID = @ReportID
		order by SupportingVillageID, UT.sort -- Sort has to be this way. the code relies on this!!
		
	--
	-- Mark report as read/viewed
	update ReportAddressees
		set IsViewed = 1
		where PlayerID = @PlayerID
			and RecordID = @RecordID



end try

begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK
	
	SET @ERROR_MSG = 'qReports_SupportAttackedReportDet FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
        + '   @ReportID' + ISNULL(CAST(@ReportID AS VARCHAR(20)), 'Null') + CHAR(10)
        + '   @RecordID' + ISNULL(CAST(@RecordID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch
