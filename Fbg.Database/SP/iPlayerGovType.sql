if exists (select * from sysobjects where type = 'P' and name = 'iPlayerGovType')
begin
	drop procedure iPlayerGovType
end
go


CREATE Procedure iPlayerGovType
	@PlayerID as int,
	@GovTypeID as int
AS
		DECLARE @ERROR_MSG AS VARCHAR(max)

__RETRY__:
begin try 
	begin tran
		
		create table #t (ResearchItemTypeID int, ResearchItemID int) 
		
		/*
		Government Name - Starting Techs  
		Monarchy 341,342,343,344  
		Republic 345,346,347,61,62,81  
		Barbarians 348,349,60,169,170  
		Merchant House 350,351,355  
		Theocracy 352,353,354,52,53,55
		*/


		if @GovTypeID =1 BEGIN
			insert into #t values (1, 341)
			insert into #t values (1, 342)
			insert into #t values (1, 343)
			insert into #t values (1, 344)
		END ELSE if @GovTypeID =2 BEGIN
			insert into #t values (1, 345)
			insert into #t values (1, 346)
			insert into #t values (1, 347)
			insert into #t values (1, 61)
			insert into #t values (1, 62)
			insert into #t values (1, 81)
		END ELSE if @GovTypeID =3 BEGIN
			insert into #t values (1, 348)
			insert into #t values (1, 349)
			insert into #t values (1, 60)
			insert into #t values (1, 169)
			insert into #t values (1, 170)
		END ELSE if @GovTypeID =4 BEGIN
			insert into #t values (1, 350)
			insert into #t values (1, 351)
			insert into #t values (1, 355)
		END ELSE if @GovTypeID =5 BEGIN
			insert into #t values (1, 352)
			insert into #t values (1, 353)
			insert into #t values (1, 354)
			insert into #t values (1, 52)
			insert into #t values (1, 53)
			insert into #t values (1, 55)
		END
		
			
			
		insert into playerresearchitems 
			select @PlayerID, SL.ResearchItemTypeID, SL.ResearchItemID
			from #t SL 
				left join playerresearchitems PR 
					on SL.ResearchItemTypeID = PR.ResearchItemTypeID 
					and SL.ResearchItemID = PR.ResearchItemID 					
					and PR.PlayerID = @PlayerID
				left join ResearchInProgress RIIP
					on SL.ResearchItemTypeID = RIIP.ResearchItemTypeID 
					and SL.ResearchItemID = RIIP.ResearchItemID 					
					and RIIP.PlayerID = @PlayerID
			where PR.ResearchItemID is null -- only add those that player does not alreayd have. 
				and RIIP.ResearchItemID is null -- only add those that player does not alreayd have in progress 

		UPDATE Players SET ResearchUpdated = 1, UpdateCompletedQuests = 1 WHERE PlayerID = @PlayerID
			
		
	commit tran
end try
begin catch
	 IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iPlayerGovType FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @@PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @@GovTypeID' + ISNULL(CAST(@GovTypeID AS VARCHAR(100)), 'Null') + CHAR(10)
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
-- say that the villages involved have changed. this is done deliberately outside of the main tran and try 
-- since at the time of writing this, gov selection only happend early on, its ok to invalidate all villages;
--	if this changes, we might have to change this as we might not want all villages to be invalidated
--
UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() -- update cache stamps for existing records
	from VillageCacheTimeStamps S join Villages V
		on S.PlayerID = V.OwnerPlayerID
		and S.VillageID = V.VillageID
	where CachedItemID = 0

insert into VillageCacheTimeStamps -- insert cache stamps for records that do not exits
select V.OwnerPlayerID, V.VillageID,  0, getdate() from VillageCacheTimeStamps S 
		right join Villages V
		on S.PlayerID = V.OwnerPlayerID
		and S.VillageID = V.VillageID
		where S.PlayerID is null
