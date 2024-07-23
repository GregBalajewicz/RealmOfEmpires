    
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'VillageStartLevels_ResearchSpeedup_i')
	BEGIN
		DROP  Procedure  VillageStartLevels_ResearchSpeedup_i
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].VillageStartLevels_ResearchSpeedup_i
		 @userid uniqueidentifier 
	, @PlayerID int
	, @StartLevelID int
	, @RealmID int

AS

begin try 

	declare @receivedOn datetime
	set @receivedOn = getdate()

	-- get the building speedups to give
	select * into #T from VillageStartLevels_ResearchSpeedup where StartLevelID <= @StartLevelID and realmid = @RealmID

	-- create Items 
	insert into Items (userid, playerid, receivedon) select @userid, @PlayerID, @receivedOn  from #T 

	-- get temp tables with dummy key so that we can join them 
	select *, row_number() over (order by StartLevelID) tempID into #T2 from #t 
	select itemid, row_number() over (order by itemid) tempID into #I2 from Items where ReceivedOn = @receivedOn

	insert into Items_ResearchSpeedup (itemID, MinutesAmount) select I.ItemID, minuntesOfSpeedup from #T2 T join #I2 I on I.tempID = T.tempID


end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'VillageStartLevels_ResearchSpeedup_i FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @userid' + ISNULL(CAST(@userid AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 



--delete VillageStartLevels_ResearchSpeedup 

--drop table #T
--drop table #t2
--drop table #i2

--/*
--drop table VillageStartLevels_BuildingSpeedup
--CREATE TABLE VillageStartLevels_BuildingSpeedup(
--    StartLevelID		int    NOT NULL,
--    minuntesOfSpeedup   int    NOT NULL,
--	realmID int not null    
--)
--go
--CREATE TABLE VillageStartLevels_ResearchSpeedup(
--    StartLevelID		int    NOT NULL,
--    minuntesOfSpeedup   int    NOT NULL,
--	realmID int not null    
--)
--go
--*/
--declare @userid uniqueidentifier 
--declare @receivedOn datetime
--declare @StartLevelID int
--declare @PlayerID int
--declare @RealmID int

--set @receivedOn = getdate()
--set @StartLevelID = 2
--set @userid = 'BC0C1732-6CDB-4277-A9F8-19EA62C56265'
--set @PlayerID = 21
--set @RealmID = 70


--insert into VillageStartLevels_ResearchSpeedup values (1,15,70)
--insert into VillageStartLevels_ResearchSpeedup values (1,15,70)
--insert into VillageStartLevels_ResearchSpeedup values (1,15,70)
--insert into VillageStartLevels_ResearchSpeedup values (2,60,70)
--insert into VillageStartLevels_ResearchSpeedup values (2,60,70)

--select * into #T from VillageStartLevels_ResearchSpeedup where StartLevelID <= @StartLevelID and realmid = @RealmID
--insert into Items (userid, playerid, receivedon) select @userid, @PlayerID, @receivedOn  from #T 


--select *, row_number() over (order by StartLevelID) tempID into #T2 from #t 
--select itemid, row_number() over (order by itemid) tempID into #I2 from Items where ReceivedOn = @receivedOn


--insert into Items_ResearchSpeedup (itemID, MinutesAmount) 
--select I.ItemID, minuntesOfSpeedup from #T2 T join #I2 I on I.tempID = T.tempID



--exec Items2_q 'BC0C1732-6CDB-4277-A9F8-19EA62C56265', null




