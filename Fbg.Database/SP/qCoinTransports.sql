
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qCoinTranports')
	BEGIN
		DROP  Procedure  qCoinTranports
	END

GO
CREATE Procedure [dbo].[qCoinTranports]
		@OriginVillageID int
		,@Playerid int
AS

begin try 

IF @Playerid IS NULL BEGIN
	SELECT     VillageID as DestVillageID ,Villages.Name AS DestVillage, CoinTransports.Amount, Villages.XCord, Villages.YCord, Events.EventTime, CoinTransports.TripDuration, 
                      CoinTransports.Direction,CoinTransports.Reserved,CoinTransports.EventID
	FROM         Villages INNER JOIN
                      CoinTransports ON Villages.VillageID = CoinTransports.DestinationVillageID INNER JOIN
                      Events ON CoinTransports.EventID = Events.EventID
	WHERE     (CoinTransports.OriginVillageID = @OriginVillageID and Events.Status<>1)

END ELSE BEGIN 
	SELECT     sum(CoinTransports.Amount)
	FROM       CoinTransports 
				INNER JOIN
                      Events ON CoinTransports.EventID = Events.EventID
	WHERE     (CoinTransports.OriginVillageID = @OriginVillageID and Events.Status<>1)

	SELECT v.VillageID as DestVillageID 
		,v.Name AS DestVillage
		, CT.Amount
		, v.XCord
		, v.YCord
		, Events.EventTime
		, CT.TripDuration
		, CT.Direction
		,CT.Reserved
		,CT.EventID
		, OrgV.villageid
		,OrgP.Name AS OrgVillagePlayerName
		,OrgV.OwnerPlayerID AS OrgVillageOwnerID
		,OrgV.Name AS OrgVillage
		, OrgV.XCord as OrgVX
		, OrgV.YCord as OrgVY

	FROM         Villages V INNER JOIN
                      CoinTransports CT 
						ON v.VillageID = CT.DestinationVillageID 
					INNER JOIN Events ON CT.EventID = Events.EventID
					JOIN Villages as OrgV
						on OrgV.villageid  = CT.OriginVillageID
					JOIN Players as OrgP
						on OrgP.playerid = OrgV.ownerplayerid
	WHERE     (CT.OriginVillageID in (select villageid from villages where ownerplayerid = @Playerid)
	
				OR ( CT.DestinationVillageID in (select villageid from villages where ownerplayerid = @Playerid) and Direction = 0) 
				)
	
	 and Events.Status<>1

END


end try


begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qCoinTranports FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @OriginVillageID' + ISNULL(CAST(@OriginVillageID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	 