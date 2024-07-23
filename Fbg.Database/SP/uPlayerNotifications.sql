 
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayerNotifications')
	BEGIN
		DROP  Procedure  uPlayerNotifications
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].uPlayerNotifications
		@RecordID int
AS
	
update PlayerNotifications set TimeSent=getdate() where RecordId = @RecordID
go

