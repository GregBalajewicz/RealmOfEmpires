 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dCancelGovernorRecruit')
	BEGIN
		DROP  Procedure  dCancelGovernorRecruit
	END
GO

CREATE Procedure dCancelGovernorRecruit
	@VillageID int
	,@PlayerID int
AS
	begin try 

		begin tran 		
			declare @UnitCost as int
			declare @EntryID as int

			--
			-- Obtain a village LOCK
			--		this will ensure that no one else will change anything in the village, or effect the upgrade Q 
			--
			update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @VillageID
			
			select top 1
			@EntryID=EntryID
			from UnitRecruitments
			inner join villages on villages.VillageID=UnitRecruitments.VillageID
			where UnitRecruitments.VillageID=@VillageID
			and villages.OwnerPlayerID=@PlayerID
			and Status = 0
			and UnitTypeID=10 /* Unit type == Lord*/
			order by EntryID desc
				
			IF @EntryID is null BEGIN
				-- this could happen if the player called the cancel twice 
				GOTO done
			END 
			
			-- no need to return coins just return chests
			exec qLordUnitTypeCost @PlayerID ,@UnitCost output, 0 /*0 means we want the cost of the last recruiting gov*/
				
			update players set Chests=Chests + @UnitCost 
			from Players
			join Villages
			on OwnerPlayerID=PlayerID
			where VillageID=@VillageID
			
			-- delete the Recruit from the DB
			delete 
			from UnitRecruitments 
			where EntryID=@EntryID;
			
			DONE:
		commit tran
	end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'dCancelGovernorRecruit FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	




GO 