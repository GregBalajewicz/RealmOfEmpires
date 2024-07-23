

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uMessageMove')
	BEGIN
		DROP  Procedure  uMessageMove
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE Proc [dbo].uMessageMove
	@PlayerID as int
	,@RecordIDs as varchar(max)
	,@FolderID as int 
as

update MessageAddressees 
	set FolderID = @FolderID 
	where PlayerID=@PlayerID 
		and RecordID In (Select ID from fnGetIds(@RecordIDs))
		and Type =1 /*1 means inbox. we do not allow moving of sent items*/

