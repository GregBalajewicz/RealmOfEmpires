 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uCredits_Subtract2')
	BEGIN
		DROP  Procedure  uCredits_Subtract2
	END

GO

CREATE Procedure dbo.uCredits_Subtract2
	@PlayerID as int,
	@CreditAmount as int,
	@Result as int output
AS
	declare @UserID as uniqueidentifier
		
	select @UserID=UserId from Players where PlayerID=@PlayerID

EXEC uCredits_Subtract3 @UserID, @CreditAmount, 6, -1, @Result output
GO