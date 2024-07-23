IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iBlockPlayer')
	BEGIN
		DROP  Procedure  iBlockPlayer
	END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure dbo.iBlockPlayer
	@PlayerID int,
	@PlayerNameToBlock varchar(25)

AS
-- return =0 ;sucess
-- return =1 ;BlockedPlayer not exists
-- return =2 ;this player Already Blocked
-- return =3 ;player Block himself
begin try 
	declare @BlockedPlayerID as int
	select @BlockedPlayerID=PlayerID from Players where Name=@PlayerNameToBlock
	if @BlockedPlayerID is not null
		begin 
			if not exists(select * from MessagesBlockedPlayers 
					  join Players on MessagesBlockedPlayers.BlockedPlayerID=Players.PlayerID
					  where Players.Name=@PlayerNameToBlock and MessagesBlockedPlayers.PlayerID=@PlayerID)
				begin
					if @BlockedPlayerID<>@PlayerID
						begin
							insert into MessagesBlockedPlayers(PlayerID,BlockedPlayerID) 
							values (@PlayerID,@BlockedPlayerID)
						end
					else
						begin
							select 3;-- return =3 ;player Block himself
							return;
						end
				end
			else
				begin
					select 2;-- return =2 ;this player Already Blocked
					return;
				end
		end
	else 
		begin
			select 1;-- return =1 ;BlockedPlayer not exists
			return;
		end
	
	select 0;-- return =0 ;sucess
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iBlockPlayer FAILED! '	+  CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:'	+ CHAR(10)
		+ '   @PlayerNameToBlock'				+ ISNULL(CAST(@PlayerNameToBlock AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @PlayerID'		+ ISNULL(CAST(@PlayerID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @BlockedPlayerID'		+ ISNULL(CAST(@BlockedPlayerID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



 