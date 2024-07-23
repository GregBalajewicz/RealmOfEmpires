 if exists (select * from sysobjects where type = 'P' and name = 'uBonusVillageInit')
begin
	drop procedure uBonusVillageInit
end
go


/*
it is assumed that this SP is always called within an active transaction
*/
create Procedure uBonusVillageInit
	@VID int
AS
    declare @BonusVillageCreationPerc int
    declare @VillageTypeID smallint 
	Declare @ERROR_MSG as varchar(max)
    
    begin try
        select @BonusVillageCreationPerc = cast (isnull(attribValue,0) as int) from RealmAttributes where AttribID = 26 
	    IF @BonusVillageCreationPerc > 0  BEGIN 	    
	        IF (rand() * 100)  <= @BonusVillageCreationPerc BEGIN
				--
				-- get the bonus village ID that this village should get 
				--
				-- HACK HACK HACK HACK HACK HACK 
				-- NOTE : Legnedary bonus villages, that is, village types with ID 8 or higher, have a hardcoded, smaller chance of appearing on the map!!
				-- HACK HACK HACK HACK HACK HACK 
				--
				IF (rand() * 100)  <= 15 BEGIN
					-- 1 in 5 chance that village will be a legendary bonus village
					SELECT TOP 1 @VillageTypeID = VillageTypeID FROM VillageTypes where villagetypeid <> 0 and VillageTypeID >= 8 ORDER BY NEWID()
				END ELSE BEGIN
					-- 4 in 5 chance that village will be a non-legendary bonus village
					SELECT TOP 1 @VillageTypeID = VillageTypeID FROM VillageTypes where villagetypeid <> 0 and VillageTypeID < 8 ORDER BY NEWID()
				END 

				-- in case there are no lengendary bonus villages, and ID returned null
				SET @VillageTypeID = ISNULL ( @VillageTypeID , (SELECT TOP 1 VillageTypeID FROM VillageTypes where villagetypeid <> 0 ORDER BY NEWID()))


		        Update Villages set VillageTypeID = @VillageTypeID where villageid = @VID
		    END 
		END
	end try
	begin catch
		IF @@TRANCOUNT > 0 ROLLBACK
		
		SET @ERROR_MSG = 'uBonusVillageInit FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @VID'		+ ISNULL(CAST(@VID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @BonusVillageCreationPerc'		+ ISNULL(CAST(@BonusVillageCreationPerc AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @VillageTypeID'		+ ISNULL(CAST(@VillageTypeID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'+ ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'   + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'   + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'    + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'   + ERROR_MESSAGE() + CHAR(10)
		
		--
		-- IF deadlock, then rerun if not in nested tran, return with propert error message if nested 
		--
		IF ERROR_NUMBER() = 1205 -- deadlock occured in this SP 
			OR (ERROR_NUMBER() = 50000 AND left(ERROR_MESSAGE(), 8) = 'DEADLOCK' ) -- deadlock occured in some called SP 
		BEGIN	
			
			INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock, no rerun', @ERROR_MSG)		
			SET @ERROR_MSG = 'DEADLOCK  -- ' + @ERROR_MSG 		
			RAISERROR(@ERROR_MSG,11,1)	
		END ELSE BEGIN
			--
			-- Some other error, not deadlock
			RAISERROR(@ERROR_MSG,11,1)	
		END
		
	end catch