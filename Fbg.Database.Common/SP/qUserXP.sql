IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qUserXP')
	BEGIN
		DROP  Procedure  qUserXP
	END

GO

CREATE Procedure dbo.qUserXP
	@userID uniqueidentifier
AS

select XP
	from Users 
    where UserID = @userID 
 