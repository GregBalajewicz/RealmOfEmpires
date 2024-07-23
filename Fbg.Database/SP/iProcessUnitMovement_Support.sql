IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iProcessUnitMovement_Support')
	BEGIN
		DROP  Procedure  iProcessUnitMovement_Support
	END

GO

CREATE Procedure iProcessUnitMovement_Support
	@EventID as int
	, @RunMode as int 	-- 1 == executed by the event handler to process a support event. 
						-- 2 == executed by the attack event when a village attacked is taken over and the attacking troops stay as support. 
	, @PrintDebugInfo BIT = null
AS
declare @OriginVillageName varchar(max)
declare @TargetVillageName varchar(max)
declare @TargetVillageXCord int
declare @TargetVillageYCord int
declare @OriginVillageID int
declare @TargetVillageID int
declare @OriginPlayerName varchar(max)
declare @OriginPlayerID int
declare @OriginVillageXCord int
declare @OriginVillageYCord int
declare @TargetPlayerID int
declare @TargetPlayerName nvarchar(25)
declare @ExpectedEventStatus int
declare @ReportID int
declare @IsNestedTransaction Bit
declare @SupportOKDoReport bit
DECLARE @ERROR_MSG AS VARCHAR(max)

declare @CONST_YourSupportArrived as int
set @CONST_YourSupportArrived =5
declare @CONST_SupportArrived as int
set @CONST_SupportArrived =6

DECLARE @DEBUG INT, @DBG varchar(10)
IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
	SET @DEBUG = 1
	set @DBG = ':::::::'
	SET NOCOUNT OFF
END ELSE BEGIN
	SET @DEBUG = 0
	SET NOCOUNT ON
END 
IF @DEBUG = 1 print @DBG + 'BEGIN iProcessUnitMovement_Support ' + cast(@EventID as varchar(10))

--
-- Is this SP called by some other SP, as part of a nested transaction?
--
IF @@TRANCOUNT > 0 BEGIN 
	set @IsNestedTransaction = 1 --Yes!
END ELSE BEGIN
	set @IsNestedTransaction = 0 --no
END

__RETRY__:

begin try 
	begin tran SUPPORT	
	--
	-- get some details of the event we are processing
	--
	IF @RunMode = 1 BEGIN
		--
		-- Set the event as completed so that no one, in the mean time, can cancel it
		--
		UPDATE Events SET [Status] =1
		FROM Events E
		join UnitMovements UM
			on UM.EventID = E.EventID
		WHERE E.EventID = @EventID
			AND [Status] = 0		
			AND CommandType in (0) -- MAKE sure this is SUPPORT.

		IF @@rowcount <> 1 BEGIN
			-- IF no rows where updated, then the event must have been cancelled (or something like this) thereforecs we abort quietly
			IF @DEBUG = 1 SELECT 'Event is no longer valid. Exit quietly' 			
			INSERT INTO ErrorLog VALUES (getdate(), 0, 'iProcessUnitMovement_Support, UPDATE Events SET [Status] =1 resulted in no rows', @EventID)		
			GOTO DONE
		END 
		
		-- 
		-- If we are processing the actual support event, then we expect an event like that in the database
		-- 
		select @OriginVillageID = UM.OriginVillageID
			, @TargetVillageID = UM.DestinationVillageID
		from UnitMovements UM
		join Events E
			on UM.EventID = E.EventID
		where UM.EventID = @EventID
			and E.Status = 1
			
										
		--
		-- Obtain locks on target & attacking villages
		--
		update VillageSemaphore set TimeStamp = getdate() 
			where VillageID in (@OriginVillageID, @TargetVillageID)
						
	END ELSE BEGIN 
		--	
		-- If this stored proc was called by the attack event (to leave troops in the village that was just taken over)
		--	then we expect to find an attack event that was already marked as completed. 
				
		select @OriginVillageID = UM.OriginVillageID
			, @TargetVillageID = UM.DestinationVillageID
		from UnitMovements UM
		join Events E
			on UM.EventID = E.EventID
		where UM.EventID = @EventID
			and E.Status = 1
	END 

	
	IF @OriginVillageID is null BEGIN
		--
		-- This can occur in case the event got processed already but its unlikely. 
		--	So this is just a sanity check. 
		INSERT INTO ErrorLog VALUES (getdate(), 0, 'iProcessUnitMovement_Support, @OriginVillageID is null', @EventID)		
		GOTO DONE
	END

	--
	-- get some additional info needed later
	--
	SELECT @OriginVillageName = V.Name
			, @OriginPlayerName = P.Name
			, @OriginPlayerID = V.OwnerPlayerID
			, @OriginVillageXCord = V.XCord
			, @OriginVillageYCord = V.YCord
		from Villages V
		join Players P
			on V.OwnerPlayerID = P.PlayerID 
		where V.VillageID = @OriginVillageID

	SELECT @TargetVillageName = V.Name
			, @TargetPlayerID = V.OwnerPlayerID
			, @TargetVillageXCord = V.XCord
			, @TargetVillageYCord = V.YCord
		from Villages V
		where V.VillageID = @TargetVillageID


	--
	-- check if there is already support from this village in this dest village
	--	if not, enter a empty record 
	--
	IF not exists(select * from VillageSupportUnits 
		where SupportedVillageID = @TargetVillageID 
		AND SupportingVillageID = @OriginVillageID) 
	BEGIN
		insert into VillageSupportUnits 
		select @TargetVillageID, @OriginVillageID, UnitTypeID,0 from UnitTypes
	END
	
	IF @DEBUG = 1 select * from VillageSupportUnits where SupportedVillageID = @TargetVillageID AND SupportingVillageID = @OriginVillageID
	--
	-- Update the support in the target village from the supporting village but the amounts in this 
	--	support event
	--
	update VillageSupportUnits set UnitCount = VSU.UnitCount + UMing.UnitCount
	from VillageSupportUnits VSU
	join UnitsMoving UMing 
		on VSU.UnitTypeID = UMing.UnitTypeID
		and UMing.EventID = @EventID
	where SupportedVillageID = @TargetVillageID 
	AND SupportingVillageID = @OriginVillageID
	IF @DEBUG = 1 select * from VillageSupportUnits where SupportedVillageID = @TargetVillageID AND SupportingVillageID = @OriginVillageID


	SET @SupportOKDoReport = 1
	
	--
	-- update cache time stamps
	--
	-- outgoing cache item (2) for sender of the support
	UPDATE PlayerCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @OriginPlayerID and CachedItemID = 2  
	IF (@@rowcount < 1 ) BEGIN
		INSERT INTO PlayerCacheTimeStamps values (@OriginPlayerID, 2, getdate())
	END
	-- incoming cache item (1) for receiver of the support
	UPDATE PlayerCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @TargetPlayerID and CachedItemID = 1  
	IF (@@rowcount < 1 ) BEGIN
		INSERT INTO PlayerCacheTimeStamps values (@TargetPlayerID, 1, getdate())
	END

		



DONE: 
	COMMIT TRAN SUPPORT
	
	IF @DEBUG = 1 print @DBG + 'END iProcessUnitMovement_Support Support part' + cast(@EventID as varchar(10))
end try
begin catch
    IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iProcessUnitMovement_Support FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @EventID'			+ ISNULL(CAST(@EventID AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @OriginVillageID' + ISNULL(CAST(@OriginVillageID AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @TargetVillageID' + ISNULL(CAST(@TargetVillageID AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @OriginVillageName' + ISNULL(CAST(@OriginVillageName AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @TargetVillageName' + ISNULL(CAST(@TargetVillageName AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @RunMode'			+ ISNULL(CAST(@RunMode AS VARCHAR (33)), 'Null') + CHAR(10)		
		+ '   @OriginPlayerID'	+ ISNULL(CAST(@OriginPlayerID AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @TargetPlayerID'	+ ISNULL(CAST(@TargetPlayerID AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @OriginPlayerName'+ ISNULL(CAST(@OriginPlayerName AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @IsNestedTransaction'+ ISNULL(CAST(@IsNestedTransaction AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @TargetVillageXCord'+ ISNULL(CAST(@TargetVillageXCord AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @TargetVillageYCord'+ ISNULL(CAST(@TargetVillageYCord AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @OriginVillageXCord'+ ISNULL(CAST(@OriginVillageXCord AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @OriginVillageYCord'+ ISNULL(CAST(@OriginVillageYCord AS VARCHAR(33)), 'Null') + CHAR(10)
		
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
			
	--
	-- IF deadlock, then rerun if not in nested tran, return with propert error message if nested 
	--
	IF	ERROR_NUMBER() = 1205 -- deadlock occured in this SP 
		OR (ERROR_NUMBER() = 50000 AND left(ERROR_MESSAGE(), 8) = 'DEADLOCK' ) -- deadlock occured in some called SP 
	BEGIN	
		IF @IsNestedTransaction = 1 BEGIN 
			--
			-- Nested tran. no rerurn, just let caller know what happend. 
			-- 
			INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock, no rerun', @ERROR_MSG)		
			SET @ERROR_MSG = 'DEADLOCK  -- ' + @ERROR_MSG 		
			RAISERROR(@ERROR_MSG,11,1)	
		END ELSE BEGIN 
			--
			-- NOT Nested tran. rerurn
			-- 
			INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock', @ERROR_MSG)		
			WAITFOR DELAY '00:00:05'
			GOTO __RETRY__		
		END
	END ELSE BEGIN
		--
		-- Some other error, not deadlock
		RAISERROR(@ERROR_MSG,11,1)	
	END
		

end catch	




__RETRY__REPORT__:

begin try 
	begin tran SUPPORT	
	--
	-- Create a report
	-- 

	IF @SupportOKDoReport = 1 BEGIN


		IF @RunMode = 1 BEGIN
			

			SELECT @TargetPlayerName = Name 
			FROM Players where PlayerID = @TargetPlayerID

			IF @DEBUG = 1 select @OriginVillageName as '@OriginVillageName', @OriginPlayerName as '@OriginPlayerName', @OriginPlayerID as '@OriginPlayerID', @TargetVillageName as '@TargetVillageName', @TargetPlayerID as '@TargetPlayerID'		
		
			--
			-- create a 'your support arrived at destination report' IF @OriginPlayerID <> @TargetPlayerID BEGIN 
			--
			IF @OriginPlayerID <> @TargetPlayerID BEGIN 
				INSERT INTO Reports (Time, Subject, ReportTypeID, ReportTypeSpecificData)
					VALUES(getdate()
						, dbo.Translate('iPUMSup_arrAt') + cast(@TargetVillageName as varchar) + '(' 
							+ cast(@TargetVillageXCord as varchar(10))+ ',' 
							+ cast(@TargetVillageYCord as varchar(10)) + ')'				
						, @CONST_YourSupportArrived 
				
						, dbo.Translate('iPUMSup_yourSup') +'<a href=VillageOverviewOther.aspx?ovid='
							+ cast(@OriginVillageID as varchar) + '>'
							+ cast(@OriginVillageName as varchar) + '(' 
							+ cast(@OriginVillageXCord as varchar(10))+ ',' 
							+ cast(@OriginVillageYCord as varchar(10)) + ')'							
							+ '</a>' + dbo.Translate('iPUMSup_destVill')+'<a href=VillageOverviewOther.aspx?ovid=' 
							+ cast(@TargetVillageID as varchar) + '>'
							+ cast(@TargetVillageName as varchar) + '(' 
							+ cast(@TargetVillageXCord as varchar(10))+ ',' 
							+ cast(@TargetVillageYCord as varchar(10)) + ')'				
							+ '</a>, owned by ' + @TargetPlayerName )
				set @ReportID = SCOPE_IDENTITY()
			
				insert into ReportAddressees (PlayerID, ReportID, ForwardedByPlayerID, ForwardedOn, IsViewed) 
					values (@OriginPlayerID, @ReportID, null, null, 0)	
				update Players set NewReportIndicator = 1 where PlayerID = @OriginPlayerID
		
				IF @DEBUG = 1 select 'Reports' , * from Reports where ReportID = @ReportID
				IF @DEBUG = 1 select 'ReportAddressees', * from ReportAddressees where ReportID = @ReportID
			END
			--
			-- create a 'you got supported report' - if you are not supporting your self
			--
			IF @OriginPlayerID <> @TargetPlayerID BEGIN 

				INSERT INTO Reports (Time, Subject, ReportTypeID, ReportTypeSpecificData)
					VALUES(getdate()
						, dbo.Translate('iPUMSup_supAt') + cast(@TargetVillageName as varchar)+ '(' 
							+ cast(@TargetVillageXCord as varchar(10))+ ',' 
							+ cast(@TargetVillageYCord as varchar(10)) + ')'
							+ dbo.Translate('iPUMSup_by') + cast(@OriginPlayerName as varchar(10))
						, @CONST_SupportArrived 
						, dbo.Translate('iPUMSup_supFrom') + '<a href=player.aspx?pname=' 
							+ cast(@OriginPlayerName as varchar) +'>'
							+ cast(@OriginPlayerName as varchar) 
							+ '</a> - <a href=VillageOverviewOther.aspx?ovid=' 
							+ cast(@OriginVillageID as varchar) + '>'
							+ cast(@OriginVillageName as varchar) + '(' 
							+ cast(@OriginVillageXCord as varchar(10))+ ',' 
							+ cast(@OriginVillageYCord as varchar(10)) + ')</a>'		
							+ dbo.Translate('iPUMSup_arrVill') + '<a href=VillageOverviewOther.aspx?ovid=' 
							+ cast(@TargetVillageID as varchar) + '>'
							+ cast(@TargetVillageName as varchar) + '(' 
							+ cast(@TargetVillageXCord as varchar(10))+ ',' 
							+ cast(@TargetVillageYCord as varchar(10)) + ')</a>')			
				set @ReportID = SCOPE_IDENTITY()
				
				insert into ReportAddressees (PlayerID, ReportID, ForwardedByPlayerID, ForwardedOn, IsViewed) 
					values (@TargetPlayerID, @ReportID, null, null, 0)	
				update Players set NewReportIndicator = 1 where PlayerID = @TargetPlayerID

				IF @DEBUG = 1 select 'Reports' , * from Reports where ReportID = @ReportID
				IF @DEBUG = 1 select 'ReportAddressees', * from ReportAddressees where ReportID = @ReportID
			END
		END	

	END -- IF @SupportOKDoReport = 1 BEGIN

	COMMIT TRAN SUPPORT
	
	IF @DEBUG = 1 print @DBG + 'END iProcessUnitMovement_Support Report part' + cast(@EventID as varchar(10))
end try
begin catch
    IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iProcessUnitMovement_Support Report FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @EventID'			+ ISNULL(CAST(@EventID AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @OriginVillageID' + ISNULL(CAST(@OriginVillageID AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @TargetVillageID' + ISNULL(CAST(@TargetVillageID AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @OriginVillageName' + ISNULL(CAST(@OriginVillageName AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @TargetVillageName' + ISNULL(CAST(@TargetVillageName AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @RunMode'			+ ISNULL(CAST(@RunMode AS VARCHAR (33)), 'Null') + CHAR(10)		
		+ '   @OriginPlayerID'	+ ISNULL(CAST(@OriginPlayerID AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @TargetPlayerID'	+ ISNULL(CAST(@TargetPlayerID AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @OriginPlayerName'+ ISNULL(CAST(@OriginPlayerName AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @IsNestedTransaction'+ ISNULL(CAST(@IsNestedTransaction AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @TargetVillageXCord'+ ISNULL(CAST(@TargetVillageXCord AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @TargetVillageYCord'+ ISNULL(CAST(@TargetVillageYCord AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @OriginVillageXCord'+ ISNULL(CAST(@OriginVillageXCord AS VARCHAR(33)), 'Null') + CHAR(10)
		+ '   @OriginVillageYCord'+ ISNULL(CAST(@OriginVillageYCord AS VARCHAR(33)), 'Null') + CHAR(10)
		
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
			
	--
	-- IF deadlock, then rerun if not in nested tran, return with propert error message if nested 
	--
	IF	ERROR_NUMBER() = 1205 -- deadlock occured in this SP 
		OR (ERROR_NUMBER() = 50000 AND left(ERROR_MESSAGE(), 8) = 'DEADLOCK' ) -- deadlock occured in some called SP 
	BEGIN	
		IF @IsNestedTransaction = 1 BEGIN 
			--
			-- Nested tran. no rerurn, just let caller know what happend. 
			-- 
			INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock, no rerun', @ERROR_MSG)		
			SET @ERROR_MSG = 'DEADLOCK  -- ' + @ERROR_MSG 		
			RAISERROR(@ERROR_MSG,11,1)	
		END ELSE BEGIN 
			--
			-- NOT Nested tran. rerurn
			-- 
			INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock', @ERROR_MSG)		
			WAITFOR DELAY '00:00:05'
			GOTO __RETRY__REPORT__	
		END
	END ELSE BEGIN
		--
		-- Some other error, not deadlock
		RAISERROR(@ERROR_MSG,11,1)	
	END
		

end catch	



--
-- say that the village has changed. this is done deliberately outside of the main tran and try 
--
UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @TargetPlayerID and VillageID = @TargetVillageID and CachedItemID = 0
IF (@@rowcount < 1 ) BEGIN
	INSERT INTO VillageCacheTimeStamps values (@TargetPlayerID, @TargetVillageID, 0, getdate())
END

GO