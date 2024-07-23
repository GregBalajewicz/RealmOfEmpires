IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dDeleteMessage')
	BEGIN
		DROP  Procedure  dDeleteMessage
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE Proc [dbo].[dDeleteMessage]
	@PlayerID as int
	,@RecordIDs as varchar(max)
as

delete MessageAddressees where 
	PlayerID=@PlayerID 
	and  RecordID In (Select ID from fnGetIds(@recordIDs))

