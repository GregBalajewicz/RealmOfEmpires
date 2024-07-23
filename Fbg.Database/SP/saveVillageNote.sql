 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'saveVillageNote')
	BEGIN
		DROP  Procedure  saveVillageNote
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

Create Proc [dbo].[saveVillageNote]
@VillageID as int, @OwnerPlayerID as int, @Notes as varchar(Max)
as
	if exists(select * from VillageNotes where VillageID=@VillageID and NoteOwnerPlayerID=@OwnerPlayerID)
	begin
		update VillageNotes
			set Note=@Notes
		where VillageID=@VillageID
			and NoteOwnerPlayerID=@OwnerPlayerID;
	end
	else
	begin
		insert into VillageNotes(VillageID,NoteOwnerPlayerID,Note)
		values(@VillageID,@OwnerPlayerID,@Notes)
	end