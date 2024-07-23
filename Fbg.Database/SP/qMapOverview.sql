 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qMapOverview')
	BEGIN
		DROP  Procedure  qMapOverview
	END

GO 
  
Create Procedure dbo.qMapOverview        
	@overviewMapBottomLeftX INTEGER,        
	@overviewMapBottomLeftY INTEGER,      
	@overviewMapSize INTEGER  ,	
	@ClanID int,
	@PlayerID int
AS   
begin try
	
	SELECT
		cast(isnull( cast(SPECIALP.Type as int), v.OwnerPlayerID) as int) as PlayerID		
		, v.XCord
		, v.YCord 
		, CM.ClanID
		, CD.StatusID
		, PF.FriendPlayerID
		FROM Villages as v 
		left JOIN ClanMembers CM
			ON V.OwnerPlayerID = CM.PlayerID
		left JOIN Clans C
			ON C.ClanID = CM.ClanID
		left JOIN ClanDiplomacy CD
			ON CD.ClanID =@ClanID
			AND CD.OtherClanID = CM.ClanID
		left JOIN PlayersFriends PF
			ON PF.FriendPlayerID=V.OwnerPlayerID
			AND PF.PlayerID=@PlayerID
		LEFT JOIN SpecialPlayers SPECIALP
			on SPECIALP.PlayerID = V.OwnerPlayerID
			and SPECIALP.Type in (0,-2) -- abandoned or rebel village
	WHERE 
		v.XCord>=@overviewMapBottomLeftX 
		AND v.XCord < @overviewMapBottomLeftX + @overviewMapSize
		and v.YCord>=@overviewMapBottomLeftY 
		AND v.YCord < @overviewMapBottomLeftY + @overviewMapSize
		and V.OwnerPlayerID not in (select PlayerID from SpecialPlayers where type = -1) -- exclude special player roe_team		
	
	
	
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK
	
	SET @ERROR_MSG = 'qMapOverview FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @overviewMapBottomLeftX' + ISNULL(CAST(@overviewMapBottomLeftX AS VARCHAR(11)), 'Null') + CHAR(10)
		+ '   @overviewMapBottomLeftY' + ISNULL(CAST(@overviewMapBottomLeftY AS VARCHAR(11)), 'Null') + CHAR(10)
		+ '   @overviewMapSize' + ISNULL(CAST(@overviewMapSize AS VARCHAR(11)), 'Null') + CHAR(10)
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
 