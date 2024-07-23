  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qIsMarkedForDeletionDueToInactivity')
	BEGIN
		DROP  Procedure  qIsMarkedForDeletionDueToInactivity
	END

GO

CREATE Procedure qIsMarkedForDeletionDueToInactivity
	(
		@PlayerID int
	)

AS
	
	select PlayerID from InactivePlayersToBeWarned where PlayerID = @PlayerID
GO
