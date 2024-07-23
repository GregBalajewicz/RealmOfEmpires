    
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uMigrateToBDAAccount')
	BEGIN
		DROP  Procedure  uMigrateToBDAAccount
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].uMigrateToBDAAccount
    @UserID             uniqueidentifier,
    @Email                nvarchar(256)
AS
BEGIN
declare @i int
END