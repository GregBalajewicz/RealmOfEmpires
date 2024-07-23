IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerStatHistory')
	BEGIN
		DROP  Procedure  qPlayerStatHistory
	END

GO

CREATE PROCEDURE [dbo].[qPlayerStatHistory]
	@playerId int,
	@date datetime = null,
	@statId int = null
AS
BEGIN
	select PlayerID, Date, StatID, StatValue from PlayerStatHistory 
	where PlayerID = @playerId and (StatID = @statId or @statId is null)
							   and (Date > @date and Date < GETDATE() or @date is null)
	order by date asc 
							   
END
GO

