IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iUserFlag')
	BEGIN
		DROP  Procedure  iUserFlag
	END

GO

CREATE Procedure iUserFlag
	(
		@userID uniqueidentifier,
		@FlagID int,
		@Data varchar(max)
	)
AS
	IF exists(select * from userflags where UserID = @userID AND FlagID = @FlagID) BEGIN
		UPDATE userflags set UpdatedOn = getdate(), Data = @Data where userID = @userID AND FlagID = @FlagID
	END ELSE BEGIN
		INSERT into userflags (userID, FlagID, UpdatedOn, Data) values (@userID, @FlagID, getdate(), @Data)
	END 

GO
 