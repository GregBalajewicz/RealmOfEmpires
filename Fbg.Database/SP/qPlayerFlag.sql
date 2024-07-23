IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerFlag')
	BEGIN
		DROP  Procedure  qPlayerFlag
	END

GO

CREATE Procedure qPlayerFlag
	(
		@PlayerID int
		,@FlagID int
	)

AS
	select Data, UpdatedOn from PlayerFlags where PlayerID = @PlayerID and FlagID = @FlagID
	
GO
 