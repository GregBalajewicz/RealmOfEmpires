IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uClanPublicProfile')
	BEGIN
		DROP  Procedure  uClanPublicProfile
	END

GO
CREATE Procedure [dbo].[uClanPublicProfile]
		@loggedInPlayerID int,
		@ClanID int,
		@PublicProfile text
		
AS
	
begin try 
			
		declare @EventMessage as nvarchar(max);
		declare @LoggedInPlayerName as nvarchar(max);
		declare @EventTime as datetime;
		
		
		update Clans set PublicProfile=@PublicProfile
		where ClanID=@ClanID
		
		--
		-- handle clan events		  
		--
		set @EventTime=getdate()
		select @LoggedInPlayerName=Name from Players where PlayerID=@loggedInPlayerID;
		
		set @EventMessage= @LoggedInPlayerName + dbo.Translate('uClanPP_changedPM')
		exec iClanEvents @ClanID,@EventTime,@EventMessage
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uClanPublicProfile FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @ClanID' + ISNULL(CAST(@ClanID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PublicProfile' + ISNULL(CAST(@PublicProfile AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	
  