

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uReportMove')
	BEGIN
		DROP  Procedure  uReportMove
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE Proc [dbo].uReportMove
	@PlayerID as int
	,@RecordIDs as varchar(max)
	,@FolderID as int 
as

update ReportAddressees 
	set FolderID = @FolderID 
	where PlayerID=@PlayerID 
		and RecordID In (Select ID from fnGetIds(@RecordIDs))

