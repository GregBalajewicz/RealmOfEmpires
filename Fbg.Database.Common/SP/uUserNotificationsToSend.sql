 
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uUserNotificationsToSend')
	BEGIN
		DROP  Procedure  uUserNotificationsToSend
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].uUserNotificationsToSend
		@RecordID int
AS
	
update UserNotificationsToSend set TimeSent=getdate() where RecordId = @RecordID
go

