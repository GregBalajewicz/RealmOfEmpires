  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uDisbandUnits')
	BEGIN
		DROP  Procedure  uDisbandUnits
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[uDisbandUnits]
		
		@PlayerID int,
		@VillageID int,
		@UnitTypeID int,
		@DisbandCount int
		
AS
-- this SP returns 0 on sucess 
-- 1 if the amount if the disband troops greater then the units amount in village
begin try 
begin tran 		
	declare @UnitCost as int
	declare @UnitPopulation as int
	declare @CoinsToReturn as int
	declare @TreasuryOverflow as int
	
	
	update VillageUnits 
		set 
		CurrentCount=CurrentCount-@DisbandCount 
		,TotalCount=TotalCount-@DisbandCount
	from VillageUnits
	join Villages
	on Villages.VillageID=VillageUnits.VillageID
	where
		VillageUnits.VillageID=@VillageID
		and UnitTypeID=@UnitTypeID
		and CurrentCount-@DisbandCount>=0
		and TotalCount-@DisbandCount>=0
		and OwnerPlayerid=@PlayerID
	
	IF @@rowcount <> 1 BEGIN
		-- IF no rows where updated, then subtract failed; assuming the villageID was correct, it must have been because unit type amount. 
		INSERT INTO ErrorLog VALUES (getdate(), 0, '@DisbandCount=' + cast(@DisbandCount  as varchar)+' is greater then the villages unit type amount.', @VillageID)		
		select 1;--error
		return ;
	END 
	
	select 0;--sucess
	commit tran
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uDisbandUnits FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @UnitTypeID' + ISNULL(CAST(@UnitTypeID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @DisbandCount' + ISNULL(CAST(@DisbandCount AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @UnitCost' + ISNULL(CAST(@UnitCost AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @UnitPopulation' + ISNULL(CAST(@UnitPopulation AS VARCHAR(20)), 'Null') + CHAR(10)
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