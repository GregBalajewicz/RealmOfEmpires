IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'savePlayerNote')
	BEGIN
		DROP  Procedure  savePlayerNote
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create Proc savePlayerNote
@PlayerID as int, @OwnerID as int, @Notes as varchar(Max)
as
	if exists(select * from PlayerNotes where PlayerID=@PlayerID and NoteOwnerPlayerID=@OwnerID)
	begin
		update PlayerNotes 
			set Note=@Notes
		where PlayerID=@PlayerID
			AND NoteOwnerPlayerID=@OwnerID
	end
	else
	begin
		insert into PlayerNotes(PlayerID,NoteOwnerPlayerID,Note)
		values(@PlayerID,@OwnerID,@Notes)
	end