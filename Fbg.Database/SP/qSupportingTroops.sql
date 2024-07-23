IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qSupportingTroops')
	BEGIN
		DROP  Procedure  qSupportingTroops
	END

GO

CREATE Procedure qSupportingTroops
	@PlayerID as int
	, @VillageID as int -- pass in null if you do not want to limit result set on this param
AS 

select VSU.UnitTypeID
	, VSU.UnitCount
	, VSU.SupportedVillageID
	, VSU.SupportingVillageID
	, StVID.Name as SupportedVillageName
	, StVID.XCord as SupportedVillageXCord
	, StVID.YCord as SupportedVillageYCord
	, SiVID.Name as SupportingVillageName
	, SiVID.XCord as SupportingVillageXCord
	, SiVID.YCord as SupportingVillageYCord
	, SiPID.Name as SupportingPlayerName
	, SiPID.PlayerID as SupportingPlayerID
	from villagesupportunits VSU
	join Villages StVID --SupportedVillage
		on VSU.SupportedVillageID = StVID.VillageID
	join Villages SiVID -- Supporting village
		on VSU.SupportingVillageID = SiVID.VillageID
	join Players SiPID -- suporting player
		on SiVID.OwnerPlayerID = SiPID.PlayerID
	WHERE 
		VSU.UnitCount != 0
		and StVID.OwnerPlayerID = @PlayerID
		and ( StVID.VillageID = @VillageID OR @VillageID is null ) 
	order by SupportedVillageID, SiPID.PlayerID, SupportingVillageID
		