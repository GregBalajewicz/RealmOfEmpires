IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qInbox')
	BEGIN
		DROP  Procedure  qInbox
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

