IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qEventsDue')
	BEGIN
		DROP  Procedure  qEventsDue
	END

GO

CREATE Procedure qEventsDue

AS


select top 100 
	E.EventID
	, E.EventTime
	, E.Status
	, BU.VillageID
	, BU.BuildingTypeID
	, BU.Level 
	, UM.CommandType
	,CT.Amount
	, BD.EventID
	, RIP.ResearchItemId
	, RM.RaidID
from events E 
	left join BuildingUpgrades BU on E.EventID = BU.EventID
	left join UnitMovements UM on UM.EventID = E.EventID
	left join CoinTransports CT on CT.EventID = E.EventID 
	left join BuildingDowngrades BD on BD.EventID = E.EventID 
	left join ResearchInProgress RIP on RIP.EventID = E.EventID 
	left join RaidUnitMovements RM on RM.EventID = E.EventID 
where 
	EventTime < dateadd(second, 5, getdate())
	and Status = 0 
	order by E.EventTime

GO


