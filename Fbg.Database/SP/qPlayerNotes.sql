IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerNotes')
	BEGIN
		DROP  Procedure  qPlayerNotes
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create Proc qPlayerNotes
@PlayerID as int, @OwnerID as int
as
	select isnull(Note,'') from PlayerNotes
	where Playerid=@PlayerID and NoteOwnerPlayerID=@OwnerID 