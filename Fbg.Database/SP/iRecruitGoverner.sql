IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iRecruitGoverner')
	BEGIN
		DROP  Procedure  iRecruitGoverner
	END
GO
CREATE Procedure iRecruitGoverner
	@PlayerID int
	,@VillageID int
AS
	declare @CurPop int
	declare @MaxPop int
	declare @RemPop int
	declare @UnitPop int
	declare @ChestsRequired int
	declare @Result int -- 0 sucess;1 not enough food ;2 not enough chests
	set @Result=0;


	begin try 

		begin tran 		
			--
			-- Obtain a village LOCK
			--		this will ensure that no one else can effect the village in any way.
			--
			update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @VillageID
		
			-- 
			-- validating population
			-- 
			select @UnitPop=Population from UnitTypes where UnitTypeID=10 -- unit type lord
			set @MaxPop = dbo.fnGetMaxPopulation(@VillageId)
			set @CurPop = dbo.fnGetCurrentPopulation(@VillageID)
			set @RemPop = @MaxPop - @CurPop
			
			IF @UnitPop > @RemPop BEGIN
				-- the recruit request we got is inconsistent. This can happen, for example, when the recruit button is quickly clicked twice 
				--	and get end up with two identical calls. the first one comepltes, adds the recruitment to the Q, 
				-- and then the other one will fail here
				--	This is an expected behavour so we just exit quietly
				IF @@TRANCOUNT > 0 ROLLBACK
				INSERT INTO ErrorLog VALUES (getdate(), 0, 'iRecruitGoverner : @RemPop < 0',  ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null'))		
				
				set @Result=1;
				select @Result;
				return 
			END			
						

			exec  qLordUnitTypeCost @PlayerID,@ChestsRequired output, 1
			
			update players set Chests=Chests-@ChestsRequired 
			where playerid=@PlayerID 
			and Chests-@ChestsRequired>=0
			
			IF  @@rowcount <> 1  BEGIN
				-- the recruit request we got is inconsistent. This can happen, for example, when the recruit button is quickly clicked twice 
				--	and get end up with two identical calls. the first one comepltes, adds the recruitment to the Q, 
				-- and then the other one will fail here
				--	This is an expected behavour so we just exit quietly
				IF @@TRANCOUNT > 0 ROLLBACK
				INSERT INTO ErrorLog VALUES (getdate(), 0, 'iRecruitGoverner : @ChestsRequired > Chests',  ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null'))		
				
				set @Result=2;
				select @Result;
				return 
			END		
		
			--
			-- do the recruitment
			-- 
			insert into UnitRecruitments
					(VillageID
					,BuildingTypeID
					,UnitTypeID
					,Count
					, UnitCost
					, DateAdded
					, DateLastUpdated
					, status)
				 values 
					(@VillageID 
					, 9 /*Palace Building ID */
					, 10 /*Lord Unit Type*/
					, 1
					, @ChestsRequired
					, getdate()
					, null
					, 0)
					
					
			select @Result;
		commit tran
	end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iRecruitGoverner FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @CurPop' + ISNULL(CAST(@CurPop AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @RemPop' + ISNULL(CAST(@RemPop AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @ChestsRequired' + ISNULL(CAST(@ChestsRequired AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @UnitPop' + ISNULL(CAST(@UnitPop AS VARCHAR(10)), 'Null') + CHAR(10)
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
UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @PlayerID and VillageID = @VillageID and CachedItemID = 0
IF (@@rowcount < 1 ) BEGIN
	INSERT INTO VillageCacheTimeStamps values (@PlayerID, @VillageID, 0, getdate())
END

GO 