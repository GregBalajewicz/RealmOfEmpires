 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerMapEvents')
	BEGIN
		DROP  Procedure  qPlayerMapEvents
	END

GO

--this SP gets all map events that are active for a player in this realm, 
--also left joined with playerMapEventsStates to get more details / state info 
CREATE Procedure [dbo].qPlayerMapEvents	
	@playerid int
AS

select PE.EventID, PE.PlayerID, PE.TypeID, PE.Data, PE.AddedOn, PE.XCord, PE.YCord, PES.StateData, PES.StateData2
	from playerMapEvents PE 
	left join playerMapEventsStates PES
		on PE.TypeID = PES.TypeID
		and PE.PlayerID = PES.PlayerID
where PE.IsActive = 1 and PE.PlayerID = @playerid;
