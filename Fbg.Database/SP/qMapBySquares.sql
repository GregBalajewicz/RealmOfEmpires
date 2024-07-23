IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qMapBySquares')
	BEGIN
		DROP  Procedure  [qMapBySquares]
	END
go

create Procedure [dbo].[qMapBySquares]
	@regularMapBottomLeftX INTEGER,      
	@regularMapBottomLeftY INTEGER,      
	@regularMapSizeX INTEGER,       
	@regularMapSizeY INTEGER,      
	@landmarkMapBottomLeftX INTEGER,      
	@landmarkMapBottomLeftY INTEGER,      
	@landmarkMapSizeX INTEGER,       
	@landmarkMapSizeY INTEGER,        
	@ClanID int,
	@PlayerID int,
	@HasPF bit,
	@HasAllLandmarkTypes bit = 0
AS   
begin try

    
    declare @NullIfPlayerHasPF smallint
    SET @NullIfPlayerHasPF = 0 -- do not change this default; code depends on it 
    IF @HasPF = 1 BEGIN
        SET @NullIfPlayerHasPF = null
    END


	
	
	--
	-- Get cliams of other clans 
	--
	select * into #OtherClanCliams from vPlayerVillageClaims_Active 
	where clanid in 
		( 
			-- get all clans, that and I set as allies, that also set me as an ally
			select ClanID as ClanIDOfOtherClanSharingClaimsWithMe from ClanDiplomacy CD
				where 
					clanid in (
						-- select all clans, that i am allied with
						select OtherClanID from ClanDiplomacy  where clanid = @ClanID and StatusID = 0 				
						) 
				and OtherClanID=@ClanID  
				and StatusID = 0
				and exists(select * from PlayerVillageClaims_ClanSetting S where S.ClanID = CD.ClanID and SettingID =4 and SettingInt = 1)
		) 
		OR clanid in 
		( 
			-- get all clans, that and I set as NAP, that also set me as an NAP OR ally
			select ClanID as ClanIDOfOtherClanSharingClaimsWithMe from ClanDiplomacy CD
				where 
					clanid in (
						-- select all clans, that i am NAP-ed or allied with
						select OtherClanID from ClanDiplomacy  where clanid = @ClanID and StatusID in(2,0)		
						) 
				and OtherClanID=@ClanID  
				and StatusID in (0,2) -- nap or alliance
				and exists(select * from PlayerVillageClaims_ClanSetting S where S.ClanID = CD.ClanID and SettingID =5 and SettingInt = 1)
		) 

	



	 -- Select statement for selecting the co-ordenates of village(s) and Name of players.
	SELECT
		 v.VillageID
		, V.Name as VillageName
		, v.XCord
		, v.YCord 
		, V.points as villagepoints
		, P.Name as PlayerName
		--, cast(isnull( cast(SPECIALP.Type as int), P.PlayerID) as int) as PlayerID
		, P.PlayerID
		, CM.ClanID
		, isnull(C.Name ,'')
		,CD.StatusID
		,isnull(substring(PN.Note, 1, 30),'0')
		,isnull(substring(VN.Note, 1, 30), '0')
		,isnull(PF.FriendPlayerID,'0')
		,( CASE RA_SleepModeFlag.AttribValue WHEN 1 THEN null
			ELSE P.SleepModeActiveFrom END
			)
		,P.Points
		,P.AvatarID
		, V.VillageTypeID
		,PVC.PlayerID
		--, OCPVC.PlayerID
		,( select top 1 PlayerID from #OtherClanCliams OCPVC where OCPVC.ClaimedVID = v.villageID) 
		FROM Villages as v  WITH (NOLOCK)
		JOIN Players P  WITH (NOLOCK)
			ON V.OwnerPlayerID = P.PlayerID
		left JOIN ClanMembers CM
			ON P.PlayerID = CM.PlayerID
		left JOIN Clans C
			ON C.ClanID = CM.ClanID
		LEFT JOIN PlayerNotes PN on PN.PlayerID=P.PlayerID and PN.NoteOwnerPlayerID=@PlayerID and PN.Note<>''
		LEFT JOIN VillageNotes VN on VN.VillageID=v.VillageID and VN.NoteOwnerPlayerID=@PlayerID and VN.Note<>''
		LEFT JOIN PlayersFriends PF on PF.FriendPlayerID=P.PlayerID and PF.PlayerID=@PlayerID
		left JOIN ClanDiplomacy CD
			ON CD.ClanID =@ClanID
			AND CD.OtherClanID = CM.ClanID
		left join RealmAttributes RA_SleepModeFlag on RA_SleepModeFlag.AttribID = 40 and RA_SleepModeFlag.AttribValue = 1
		left join vPlayerVillageClaims_Active PVC on PVC.ClaimedVID = v.villageID and PVC.ClanID = @ClanID
		--left join #OtherClanCliams OCPVC on OCPVC.ClaimedVID = v.villageID 
	WHERE 
		v.XCord>=@regularMapBottomLeftX AND v.XCord < @regularMapBottomLeftX + @regularMapSizeX
		and v.YCord>=@regularMapBottomLeftY AND v.YCord < @regularMapBottomLeftY + @regularMapSizeY
		and V.OwnerPlayerID not in (select PlayerID from SpecialPlayers where type = -1) -- exclude special player roe_team		
	
	SELECT
		lm.XCord
		, lm.YCord
		, lm.LandMarkTypePartID
		FROM LandMarks lm
	WHERE 
		lm.XCord>=@regularMapBottomLeftX 
		AND lm.XCord < @regularMapBottomLeftX + @regularMapSizeX
		and lm.YCord>=@regularMapBottomLeftY 
		AND lm.YCord < @regularMapBottomLeftY + @regularMapSizeY

	-- get the tags of these villages
	--
	select villageid, TagID from villagetags 
	    where VillageID IN (
	        SELECT VillageID	    
    		FROM Villages as v 		       
            WHERE 
	            v.XCord>=@regularMapBottomLeftX AND v.XCord < @regularMapBottomLeftX + @regularMapSizeX
	            and v.YCord>=@regularMapBottomLeftY AND v.YCord < @regularMapBottomLeftY + @regularMapSizeY
				and ownerplayerid = @PlayerID
        )
      
	if @HasAllLandmarkTypes = 1 begin
		select * from LandMarkTypeParts lmtp
	end
	
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK
	
	SET @ERROR_MSG = 'qMapBySquares FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @regularMapBottomLeftX' + ISNULL(CAST(@regularMapBottomLeftX AS VARCHAR(11)), 'Null') + CHAR(10)
		+ '   @regularMapBottomLeftY' + ISNULL(CAST(@regularMapBottomLeftY AS VARCHAR(11)), 'Null') + CHAR(10)
		+ '   @regularMapSizeX' + ISNULL(CAST(@regularMapSizeX AS VARCHAR(11)), 'Null') + CHAR(10)
		+ '   @regularMapSizeY' + ISNULL(CAST(@regularMapSizeY AS VARCHAR(11)), 'Null') + CHAR(10)
		+ '   @landmarkMapBottomLeftX' + ISNULL(CAST(@landmarkMapBottomLeftX AS VARCHAR(11)), 'Null') + CHAR(10)
		+ '   @landmarkMapBottomLeftY' + ISNULL(CAST(@landmarkMapBottomLeftY AS VARCHAR(11)), 'Null') + CHAR(10)
		+ '   @landmarkMapSizeX' + ISNULL(CAST(@landmarkMapSizeX AS VARCHAR(11)), 'Null') + CHAR(10)
		+ '   @landmarkMapSizeY' + ISNULL(CAST(@landmarkMapSizeY AS VARCHAR(11)), 'Null') + CHAR(10)
		+ '   @ClanID' + ISNULL(CAST(@ClanID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch