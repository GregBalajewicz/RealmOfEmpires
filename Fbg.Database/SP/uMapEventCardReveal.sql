IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uMapEventCardReveal')
	BEGIN
		DROP  Procedure  uMapEventCardReveal
	END
GO

CREATE Procedure [dbo].uMapEventCardReveal
	@eventID int,
	@newEventData varchar(250),
	@newStateData varchar(250),
	@newStateData2 varchar(250)
AS

declare @playerID int;
declare @typeID int;

select @playerID = PlayerID, @typeID = typeID from playerMapEvents where EventID = @eventID;


--@newStateData: 'collectedTotal,collectedThisLevel,level'
--@newStateData2: 'collectedTotal'
update playerMapEventsStates set StateData = @newStateData, StateData2 = @newStateData2 where PlayerID = @playerID and TypeID = @typeID;
IF @@ROWCOUNT = 0 BEGIN
	INSERT into playerMapEventsStates values (@playerID,@typeID,@newStateData,@newStateData2);
END

--@newEventData:  'itemID1,itemID2,itemID3,reroll'
update playerMapEvents set Data = @newEventData where EventID = @eventID;

--return the event row
select PE.EventID, PE.PlayerID, PE.TypeID, PE.Data, PE.AddedOn, PE.XCord, PE.YCord, PES.StateData, PES.StateData2 
	from playerMapEvents PE 
	left join playerMapEventsStates PES
		on PE.TypeID = PES.TypeID
		and PE.PlayerID = PES.PlayerID
	where PE.IsActive = 1 and PE.PlayerID = @playerid and  PE.EventID = @eventID;