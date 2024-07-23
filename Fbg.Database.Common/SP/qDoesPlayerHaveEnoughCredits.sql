IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qDoesPlayerHaveEnoughCredits')
	BEGIN
		DROP  Procedure  qDoesPlayerHaveEnoughCredits
	END

GO

CREATE Procedure dbo.qDoesPlayerHaveEnoughCredits
	@PlayerID int
	, @Credits int
AS
	--
	-- returns -1 if this playuer does not have at least this many credits, 1 otherwise
	--
	IF not exists (select * from users 
		where UserID= (select UserId from Players where PlayerID=@PlayerID) 
		and Credits - @Credits >= 0)
	BEGIN 
		RETURN -1
	END

	RETURN 1 