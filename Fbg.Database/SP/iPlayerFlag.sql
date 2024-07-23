IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iPlayerFlag')
	BEGIN
		DROP  Procedure  iPlayerFlag
	END

GO

CREATE Procedure iPlayerFlag
	(
		@PlayerID int,
		@FlagID int,
		@FlagUpdatedOn DateTime,
		@Data varchar(max)
	)
AS
	IF exists(select * from playerflags where playerID = @PlayerID AND FlagID = @FlagID) BEGIN
		UPDATE PlayerFlags set UpdatedOn = @FlagUpdatedOn, Data=@Data where playerID = @PlayerID AND FlagID = @FlagID
	END ELSE BEGIN
		INSERT into PlayerFlags (PlayerID, FlagID, UpdatedOn, Data) values (@PlayerID, @FlagID, @FlagUpdatedOn, @Data)
	END 

GO
