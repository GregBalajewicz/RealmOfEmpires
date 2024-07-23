 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iUserLog')
	BEGIN
		DROP  Procedure  iUserLog
	END

GO

CREATE Procedure iUserLog
	(
		@UserID uniqueidentifier
		,@playerID int 
		, @eventID int
		, @message varchar(max)
		, @data varchar(max)
	)
AS
	INSERT into userLog (Time, UserID, PlayerID, EventID, Message, Data) values (getdate(), @UserID, @PlayerID, @EventID, @Message, @Data)

GO
 