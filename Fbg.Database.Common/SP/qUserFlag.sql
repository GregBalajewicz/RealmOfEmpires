IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qUserFlag')
	BEGIN
		DROP  Procedure  qUserFlag
	END

GO

CREATE Procedure qUserFlag
	(
		@UserID uniqueidentifier 
		,@FlagID int
	)

AS
	select Data, UpdatedOn from UserFlags where UserID = @UserID and FlagID = @FlagID
	
GO
 