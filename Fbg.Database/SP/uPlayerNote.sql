IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayerNote')
	BEGIN
		DROP  Procedure  uPlayerNote
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create Proc uPlayerNote
@PlayerID as int, @OwnerID as int, @Note as varchar(Max)
as
	update PlayerNotes
	set NoteOwnerPlayerID=@OwnerID, Note=@Note 