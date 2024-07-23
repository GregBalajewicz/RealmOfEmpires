IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qUnitsAbroad')
	BEGIN
		DROP  Procedure  qUnitsAbroad
	END

GO

CREATE Procedure qUnitsAbroad
	@PlayerID int
	, @VillageID int  -- Suporting village id. null if all
	, @SupportVillageID int -- suported village id. null if all 
AS

BEGIN TRY

	-- My troops that are supporting other villages. 
	--	either get all my troops (if @VillageID is null), or get only troops from one village, identified by @VillageID
	--
	select VSU.SupportedVillageID
		, VSU.SupportingVillageID
		, VSU.UnitTypeID
		, UnitCount
		, V1.Name as SupportedVillageName
		, V1.XCord as SupportedVillageXCord
		, V1.YCord as SupportedVillageYCord
		, V2.Name as SupportingVillageName
		, V2.XCord as SupportingVillageXCord
		, V2.YCord as SupportingVillageYCord
		, P1.Name as SupportedPlayerName
		, P1.PlayerID as SupportedPlayerID
	from VillageSupportUnits VSU
	join Villages V1
		on VSU.SupportedVillageID = V1.VillageID
	join Villages V2
		on VSU.SupportingVillageID = V2.VillageID
	join Players P1
		on V1.OwnerPLayerID = P1.PLayerID
	where
		( SupportingVillageID  in 
			(select VillageID from Villages where OwnerPlayerID = @PlayerID and (VillageID = @VillageID OR @VillageID  is null) )) 
		and ( VSU.SupportedVillageID = @SupportVillageID or @SupportVillageID is null)
		and UnitCount <> 0 
	order by SupportingVillageID, SupportedVillageID

END TRY
begin catch
    IF @@TRANCOUNT > 0 ROLLBACK
	DECLARE @ERROR_MSG AS VARCHAR(8000)

	SET @ERROR_MSG = 'qUnitsAbroad FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



GO
 