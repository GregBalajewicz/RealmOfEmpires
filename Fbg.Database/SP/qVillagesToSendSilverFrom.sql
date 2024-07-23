  
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qVillagesToSendSilverFrom')
	BEGIN
		DROP  Procedure  qVillagesToSendSilverFrom
	END

GO

 CREATE Procedure [dbo].[qVillagesToSendSilverFrom]
		@ReceiverVillageID int,
		@PlayerID int
AS

begin try 

declare @SelectedXCord as int
declare @SelectedYCord as int
declare @CoinTransportSpeed as int 
declare @researchPercentBonus as real

declare @LevelProp_TradePostCapacity int 
set @LevelProp_TradePostCapacity = 10


SELECT    @SelectedXCord= XCord,@SelectedYCord= YCord
FROM         Villages
where VillageID=@ReceiverVillageID			;

select @CoinTransportSpeed=CoinTransportSpeed from Realm


select top 20 
	Villages.VillageID
	,dbo.fnGetTravelTime(@SelectedXCord,@SelectedYCord,XCord,YCord,@CoinTransportSpeed)as TravelTime
	,XCord
	,YCord
	,Name
	,dbo.fnGetMinAmountToTransport(@ReceiverVillageID,Villages.VillageID, @PlayerID) as MinAmount 
	from Villages 
	where OwnerPlayerID=@PlayerID 
		and Villages.VillageID<>@ReceiverVillageID 
		and  Villages.VillageID not in(select VillageID from NoTransportVillages)
		and dbo.fnGetMinAmountToTransport(@ReceiverVillageID,Villages.VillageID, @PlayerID) > 0
	order by TravelTime asc

end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qVillagesToSendSilverFrom FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @ReceiverVillageID' + ISNULL(CAST(@ReceiverVillageID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	
