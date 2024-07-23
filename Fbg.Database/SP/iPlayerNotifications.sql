 
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iPlayerNotifications')
	BEGIN
		DROP  Procedure  iPlayerNotifications
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].iPlayerNotifications
		@PlayerID int,
		@NotificationTypeID smallint,
		@Text nvarchar(max)
AS
	
INSERT INTO PlayerNotifications(
    NotificationTypeID    ,
    PlayerID              ,
    Text)
	values (@NotificationTypeID,@PlayerID,@Text)
go

