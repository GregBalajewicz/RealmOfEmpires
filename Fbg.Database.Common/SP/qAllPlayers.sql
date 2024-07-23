    
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qAllPlayers')
	BEGIN
		DROP  Procedure  qAllPlayers
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].qAllPlayers
		@UserID as uniqueidentifier
		--, @view int -- 0 - my view of my throne room, 1 - viewing someone elses throne room
AS
	
	
begin try 
	select distinct PLayers.Playerid, RealmID, Name, isActive, isnull(trp.displayStatus,1) from 
	(
		select Playerid, RealmID, Name, 1 as isActive from players where userid = @userid union all
		select Playerid, RealmID, isnull(OriginalName, name), 0 as isActive from DeletedPlayers where userid = @userid 
	) as PLayers 
	left join tr_PlayerListSettings trp 
		on PLayers.playerid = trp.playerid 
	--where (@view = 0 or  isnull(trp.displayStatus,1) = 1 )
	where realmid >= 0 -- ADDED THIS because th TR woudl fail if you were in any of the new RXs with nagative realm IDs
	order by RealmID desc, Playerid 
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qAllPlayers FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserID' + ISNULL(CAST(@UserID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 