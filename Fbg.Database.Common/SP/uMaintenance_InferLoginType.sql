    
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uMaintenance_InferLoginType')
	BEGIN
		DROP  Procedure  uMaintenance_InferLoginType
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].uMaintenance_InferLoginType
AS
begin
declare @i int
END