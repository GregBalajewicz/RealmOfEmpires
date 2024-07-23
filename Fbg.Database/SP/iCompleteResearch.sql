if exists (select * from sysobjects where type = 'P' and name = 'iCompleteResearch')
begin
	drop procedure iCompleteResearch
end
go


CREATE Procedure iCompleteResearch
	@EventID as int
AS
	declare @RITID int
	declare @PlayerID int
	declare @RIID as int
		DECLARE @ERROR_MSG AS VARCHAR(max)

__RETRY__:
begin try 
	begin tran
		
		
		--
		-- First we set the event as completed so that no one, in the mean time, can cancel it
		--		IF no rows where updated, then the event must have been cancelled (or something like this) therefore we do nothing
		--
		UPDATE Events SET [Status] =1 WHERE EventID = @EventID AND [Status] = 0
		IF @@rowcount = 1 BEGIN			
			
			select 
			    @PlayerID = PlayerID ,
                @RITID = ResearchItemTypeID,
                @RIID = ResearchItemID
			    FROM ResearchInProgress where EventID = @EventID
			
			--
			-- Do the research, if players does not have it alreayd. how could this happen?? not likely but its possible since there is no check like this in SP when initiating research
			--
			IF NOT EXISTS (SELECT * from PlayerResearchItems WHERE PlayerID  = @PlayerID AND ResearchItemTypeID = @RITID and ResearchItemID = @RIID ) BEGIN
			    INSERT into PlayerResearchItems ( PlayerID, ResearchItemTypeID, ResearchItemID ) 
			        Values (@PlayerID, @RITID, @RIID )
			
			    UPDATE Players SET ResearchUpdated = 1, UpdateCompletedQuests = 1 WHERE PlayerID = @PlayerID
			END 
			
		END 

	commit tran
end try
begin catch
	 IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iCompleteResearch FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @EventID' + ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null') + CHAR(10)


		+ '   @RITID' + ISNULL(CAST(@RITID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @RIID' + ISNULL(CAST(@RIID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
		
	--
	-- IF deadlock, rerun
	--
	IF ERROR_NUMBER() = 1205 -- deadlock occured in this SP 
		OR (ERROR_NUMBER() = 50000 AND left(ERROR_MESSAGE(), 8) = 'DEADLOCK' ) -- deadlock occured in some called SP 
	BEGIN
		INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock', @ERROR_MSG)		
		WAITFOR DELAY '00:00:05'
		GOTO __RETRY__
	END 
		
	RAISERROR(@ERROR_MSG,11,1)	

end catch	




--
-- **************************************************************
--
-- **************************************************************
--
-- Send notification if needed. 
-- THIS IS DONE DELIBERATELY outside of transaction and first try block because we dont want this to fail the entire SP
--
-- **************************************************************
--
-- **************************************************************

begin try 
		
	if exists(select * from vPlayerNotificationSettings PNS where PNS.PlayerID = @PlayerID and NotificationID = 5/*research completed notiftype*/ and PNS.isActive = 1 ) BEGIN
		INSERT INTO PlayerNotifications(
			NotificationTypeID    ,
			PlayerID              ,
			Text)
			select 8,@PlayerID, name  + dbo.Translate('iCompleteResearch_notif')															
				from researchItems where researchitemid = @RIID
	END

end try
begin catch
	
	
	SET @ERROR_MSG = 'iCompleteResearch FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @EventID' + ISNULL(CAST(@EventID AS VARCHAR(10)), 'Null') + CHAR(10)


		+ '   @RITID' + ISNULL(CAST(@RITID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @RIID' + ISNULL(CAST(@RIID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)

	INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock', @ERROR_MSG)		

end catch	

GO

