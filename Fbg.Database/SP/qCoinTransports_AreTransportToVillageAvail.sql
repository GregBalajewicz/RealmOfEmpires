
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qCoinTransports_AreTransportToVillageAvail')
	BEGIN
		DROP  Procedure  qCoinTransports_AreTransportToVillageAvail
	END

GO

-- returns single result set, one int value: 0 no transports avail, 1 means there are transports available
-- if you pass a valid villge ID for @SelectedVillageID then this SP will exlude this village from list of possible villages that can send silver. 
--	ie, it is then assumed that you are asking ' are there transports available TO THIS SELECTED village'. It is assumed the selectd village id is your village. 
--	You may also pass in NULL for @SelectedVillageID which case, you are asking ' are there transports available from ANY of my villages'
CREATE Procedure [dbo].qCoinTransports_AreTransportToVillageAvail
	@PlayerID int,
	@SelectedVillageID int,-- can pass in null. 
	@ReturnValue int output -- 0 no transports avail, 1 means there are transports available
AS

begin try 


	if exists (
		select *
		from Villages V
		where OwnerPlayerID=@PlayerID 
			and ( @SelectedVillageID is null OR V.VillageID<>@SelectedVillageID) 
			and  V.VillageID not in(select VillageID from NoTransportVillages)
			and dbo.fnGetMinAmountToTransport(@SelectedVillageID,V.VillageID,@PlayerID) > 0
	) BEGIN
		select 1
		set @ReturnValue= 1
	END ELSE BEGIN
		select 0
		SET @ReturnValue = 0
	END
		
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qCoinTransports_AreTransportToVillageAvail FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @SelectedVillageID'	+ ISNULL(CAST(@SelectedVillageID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PlayerID'			+ ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @ReturnValue'			+ ISNULL(CAST(@ReturnValue AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'	+ ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
	
end catch	 