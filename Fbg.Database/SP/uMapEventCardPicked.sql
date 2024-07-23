 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uMapEventCardPicked')
	BEGIN
		DROP  Procedure  uMapEventCardPicked
	END

GO


CREATE Procedure [dbo].uMapEventCardPicked	
	@playerID int,
	@eventID int
AS

--decativate event
update playerMapEvents set IsActive = 0 where EventID = @eventID;

--return all active player's mapevents
EXEC qPlayerMapEvents @playerID