IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dCancelRecruit')
	BEGIN
		DROP  Procedure  dCancelRecruit
	END
GO

CREATE Procedure dCancelRecruit
	@QEntryID int,
	@VillageID int,
	@TrasuryOverflow int output
AS
	begin try 

		begin tran 		
			declare @UnitCost as int
			declare @UnitCount as int
			declare @TotalCost as int 
			--
			-- Obtain a village LOCK
			--		this will ensure that no one else will change anything in the village, or effect the upgrade Q 
			--
			update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @VillageID
			
			--
			-- ensure any recruitment up-to-date has occured
			--
			exec iCompleteUnitRecruitment @villageId

			-- get the amount of Units that need to canceled 
			select 
				@UnitCount=Count 
				,@UnitCost=UnitCost 
				from UnitRecruitments
				where EntryID=@QEntryID
				and Status = 0;
			
			IF @UnitCount is null BEGIN
				-- this could happen if the call to iCompleteUnitRecruitment already completed this recruitment 
				GOTO done
			END 
			
			-- delete the entry
			update UnitRecruitments set Status = 1 where EntryID=@QEntryID and Status = 0
			IF @@rowcount <> 1 BEGIN
				-- IF no rows where updated, then the qentry must have been remove already therefore we abort quietly
				GOTO done
			END 

			--Calculate the Cost of the units
			set	@TotalCost = @UnitCount * @UnitCost;
			
			--
			-- return the coins to the Trasury 
			--
			exec uVillageCoins_Add @VillageID, @TotalCost, @TrasuryOverflow output
			
			DONE:
		commit tran
	end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'dCancelRecruit FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @QEntryID' + ISNULL(CAST(@QEntryID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

--
-- say that the village has changed. this is done deliberately outside of the main tran and try 
--
declare @PlayerID int 
select @PlayerID = OwnerPlayerID from villages where VillageID = @VillageID
UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @PlayerID and VillageID = @VillageID and CachedItemID = 0
IF (@@rowcount < 1 ) BEGIN
	INSERT INTO VillageCacheTimeStamps values (@PlayerID, @VillageID, 0, getdate())
END

GO 