 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uCredits_Subtract')
	BEGIN
		DROP  Procedure  uCredits_Subtract
	END

GO

CREATE Procedure dbo.uCredits_Subtract
	@PlayerID as int,
	@CreditAmount as int
AS
declare @Result int

EXEC uCredits_Subtract2 @playerID, @CreditAmount, @Result output
GO