IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qUserCredits')
	BEGIN
		DROP  Procedure  qUserCredits
	END

GO

CREATE Procedure dbo.qUserCredits
	@userID uniqueidentifier
AS

select isnull(Credits,0)
	from Users 
    where UserID = @userID 
 