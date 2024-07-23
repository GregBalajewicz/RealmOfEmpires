IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qMap')
	BEGIN
		DROP  Procedure  qMap
	END

GO 

create Procedure [dbo].[qMap]
	@regularMapBottomLeftX INTEGER,        
	@regularMapBottomLeftY INTEGER,      
	@regularMapSizeX INTEGER,       
	@regularMapSizeY INTEGER,        
	@ClanID int,
	@PlayerID int,
	@HasPF bit
AS   
begin try

    
    declare @NullIfPlayerHasPF smallint
    SET @NullIfPlayerHasPF = 0 -- do not change this default; code depends on it 
    IF @HasPF = 1 BEGIN
        SET @NullIfPlayerHasPF = null
    END

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
		,isnull(substring(PN.Note, 1, 10),'0')
		,isnull(substring(VN.Note, 1, 10), '0')
		,isnull(PF.FriendPlayerID,'0')
		,( CASE RA_SleepModeFlag.AttribValue WHEN 1 THEN null
			ELSE P.SleepModeActiveFrom END
			)
		,P.Points
		, V.VillageTypeID
		FROM Villages as v WITH (NOLOCK) 
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
	WHERE 
		v.XCord>=@regularMapBottomLeftX AND v.XCord < @regularMapBottomLeftX + @regularMapSizeX
		and v.YCord>=@regularMapBottomLeftY AND v.YCord < @regularMapBottomLeftY + @regularMapSizeY
		and V.OwnerPlayerID not in (select PlayerID from SpecialPlayers where type = -1) -- exclude special player roe_team		
	
	SELECT
		lm.XCord
		, lm.YCord
		, lmtp.ImageURL
		FROM LandMarks lm
		join LandMarkTypeParts lmtp 
			on lmtp.LandMarkTypePartID=lm.LandMarkTypePartID
	WHERE 
		lm.XCord>=@regularMapBottomLeftX 
		AND lm.XCord < @regularMapBottomLeftX + @regularMapSizeX
		and lm.YCord>=@regularMapBottomLeftY 
		AND lm.YCord < @regularMapBottomLeftY + @regularMapSizeY
      and lm.xcord is null 
      
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
	
	
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK
	
	SET @ERROR_MSG = 'qMap FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @regularMapBottomLeftX' + ISNULL(CAST(@regularMapBottomLeftX AS VARCHAR(11)), 'Null') + CHAR(10)
		+ '   @regularMapBottomLeftY' + ISNULL(CAST(@regularMapBottomLeftY AS VARCHAR(11)), 'Null') + CHAR(10)
		+ '   @regularMapSizeX' + ISNULL(CAST(@regularMapSizeX AS VARCHAR(11)), 'Null') + CHAR(10)
		+ '   @regularMapSizeY' + ISNULL(CAST(@regularMapSizeY AS VARCHAR(11)), 'Null') + CHAR(10)
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
 