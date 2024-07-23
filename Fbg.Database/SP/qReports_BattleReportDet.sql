IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qReports_BattleReportDet')
	BEGIN
		DROP  Procedure  qReports_BattleReportDet
	END

GO

CREATE Procedure qReports_BattleReportDet
	@PlayerID as int
	,@RecordID as int
AS
begin
begin try

	Declare @ReportID as int
	select @ReportID=ReportID from ReportAddressees where RecordID=@RecordID

select R.Time
	, ISNULL(RA.AlternateSubject, R.Subject) AS Subject
	, RT.Name as ReportType
	, AP.Name as AttackerPlayerName
	, AP.PlayerID as AttackerPlayerID
	, AV.VillageID as AttackerVillageID
	, AV.Name as AttackerVillageName
	, DP.Name as DefenderPlayerName
	, DP.PlayerID as DefenderPlayerID
	, DV.VillageID as DefenderVillageID
	, DV.Name as DefenderVillageName
	--, Cast(DV.XCord as varchar(max)) + ', ' + Cast(DV.YCord as varchar(max)) as DefenderVillageCords
	, BR.AttackersLoot
	, BR.LoyaltyBeforeAttack
	, BR.LoyaltyChange
	, RA.PointOfView
	, BR.CanAttackerSeeDefTroops
	, BR.DefendersCoins
	, BR.DefenderKnowsAttackersIdentity
	, BR.SpyOutcome
	, isnull(RA.ForwardedByPlayerID,0) as IsForwarded
	, RA.ForwardedOn
	, RA.RecordID
	, FP.Name
	, DV.XCord as DVX
	, DV.YCord as DVY
	, AV.XCord as AVX
	, AV.YCord as AVY
	, isnull(substring(PN.Note, 1, 71),'') as 'AttackerPlayerPN'
	, isnull(substring(VN.Note, 1, 71),'') as 'AttackerVillagePN'
	, isnull(substring(DPN.Note, 1, 71),'') as 'DefenderPlayerPN'
	, isnull(substring(DVN.Note, 1, 71),'') as 'DefenderVillagePN'
    , (Select FlagValue from ReportInfoFlag where RecordID = @RecordID and FlagID = 1)
    , (Select FlagValue from ReportInfoFlag where RecordID = @RecordID and FlagID = 2)	
	-- Added new columns here so column indexes don't get messed up in code
	, AP.AvatarID as AttackerAvatarID
	, AV.Points as AttackerVillagePoints
	, AV.VillageTypeID as AttackerVillageTypeID
	, DP.AvatarID as DefenderAvatarID
	, DV.Points as DefenderVillagePoints
	, DV.VillageTypeID as DefenderVillageTypeID
	, FP.AvatarID as ForwardedByPlayerAvatarID
    , (Select FlagValue from ReportInfoFlag where RecordID = @RecordID and FlagID = 4) as Morale -- may be null!	
	from reports R
	join ReportTypes RT
		on R.ReportTypeID = RT.ReportTypeID
	left join BattleReports BR
		on R.ReportID = BR.ReportID
	join Villages AV
		on AV.VillageID = BR.AttackerVillageID
	left join VillageNotes VN
	    on VN.VillageID=AV.VillageID and VN.NoteOwnerPlayerID=@PlayerID	
	join Villages DV
		on DV.VillageID = BR.DefenderVillageID
	left join VillageNotes DVN
	    on DVN.VillageID=DV.VillageID and DVN.NoteOwnerPlayerID=@PlayerID	
	join Players AP
		on AP.PlayerID = BR.AttackerPlayerID
	left join PlayerNotes PN
	    on PN.PlayerID=AP.PlayerID and PN.NoteOwnerPlayerID=@PlayerID	
	join Players DP
		on DP.PlayerID = BR.DefenderPlayerID
	left join PlayerNotes DPN
	    on DPN.PlayerID=DP.PlayerID and DPN.NoteOwnerPlayerID=@PlayerID
	join ReportAddressees RA
		on RA.PlayerID = @PlayerID
		and RA.RecordID = @RecordID
	left join Players FP
		on RA.ForwardedByPlayerID=FP.PlayerID
	where R.ReportID = @ReportID 
	order by R.Time desc

select BR.ReportID
	, Party
	, BRU.UnitTypeID 
	, DeployedUnitCount
	, KilledUnitCount
	, ReaminingUnitCount
	from BattleReports BR		
	join BattleReportUnits BRU
		on BR.ReportID = BRU.ReportID
	join UnitTypes UT
		on BRU.UnitTypeID = UT.UnitTypeID
	where BR.ReportID = @ReportID
	order by Party, sort -- Sort has to be this way. the code relies on this!!
	
SELECT BT.Name
	, BeforeAttackLevel
	, AfterAttackLevel
	, BT.BuildingTypeID
	FROM BattleReportBuildings BRB
	JOIN BuildingTypes BT
		ON BT.BuildingTypeID = BRB.BuildingTypeID 
	WHERE BRB.ReportID = @ReportID
	ORDER BY BT.Sort
	

SELECT BT.Name, I.BuildingTypeID, [Level] 
	FROM BattleReportBuildingIntel I
	JOIN BuildingTypes BT
		ON BT.BuildingTypeID = I.BuildingTypeID 
	WHERE I.ReportID = @ReportID
	ORDER BY BT.Sort

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
	
	SET @ERROR_MSG = 'qReports_BattleReportDet FAILED! ' +  + CHAR(10)
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
end