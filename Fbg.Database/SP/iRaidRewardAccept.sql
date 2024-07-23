 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iRaidRewardAccept')
	BEGIN
		DROP  Procedure  iRaidRewardAccept
	END

GO

--Send out a raiding party!
CREATE Procedure [dbo].iRaidRewardAccept
	@playerID int,
	@raidID int,
	@resultCode int output 
AS

begin try 

BEGIN TRAN 


	--Check if already accepted the reward for a reward acceptance row
	IF NOT EXISTS(select * from RaidRewardAcceptanceRecord RRA where RRA.PlayerID = @playerID and RRA.RaidID = @raidID) 
	BEGIN
		
		-- Insert a raid reward acceptance row 
		insert into RaidRewardAcceptanceRecord values (@raidID, @playerID, GETDATE());
		set @resultCode = 0;
		
	END
	ELSE BEGIN
		set @resultCode = 1; --this would mean its been accepted before
	END
	
COMMIT TRAN 

IF @resultCode = 0 BEGIN 
	--
	-- Since we've accepted the reward, make sure to complete any possible outgoing attack to it.
	--	this is outside of the transaction on purpose. 
	--
	update events set EventTime = getdate() 
		where eventID in (select E.eventid from RaidUnitMovements RUM join Events E on E.EventID = RUM.EventID where status = 0 and RaidID = @raidID) 
END 
end try

begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'iRaidRewardAccept FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @playerID' + ISNULL(CAST(@playerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @raidID' + ISNULL(CAST(@raidID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	











