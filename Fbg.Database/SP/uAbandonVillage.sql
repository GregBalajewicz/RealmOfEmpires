IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[uAbandonVillage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[uAbandonVillage]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO


CREATE Procedure [dbo].[uAbandonVillage]
	@PlayerID int
	, @VID int
	, @RestartCost int 
AS

BEGIN TRY
	declare @AbandonedPID int	
	  
	select @AbandonedPID = playerid from specialPlayers where Type=0

	update villages set Name = dbo.Translate('dIP_abandoned') where villageid = @VID
	update villages set ownerplayerid = @AbandonedPID where villageid = @VID

	delete CapitalVillages where villageid = @VID


	 -- note this change of ownership in the history table

	INSERT INTO VillageOwnershipHistory( VillageID, PreviousOwnerPlayerID, CurrentOwnerPlayerID, date) 
		VALUES (@VID,@PlayerID ,@AbandonedPID, getdate())


	-- delete this village from  NoTransportCoins if exists
	delete from NoTransportVillages where VillageID=@VID

	--
	-- delete any recruiting troops
	--
	update UnitRecruitments set status = 1 where villageid = @VID and status = 0

	    
	-- remove any support from this village in other villages
	delete from VillageSupportUnits	where SupportingVillageID = @VID
	    
	-- remove all troops from the target village
	update VillageUnits set CurrentCount = 0, TotalCount = 0 where VillageID = @VID 

	     

	-- delete any troops in transit from this village

	Update Events Set Status = 1
		from UnitMovements UM

		where Events.EventID = UM.EventID
	and 
	( 
		-- get rid of all movements from the taken over village UNLESS this movements it an attack returning to someother village
		-- OR unless this movement is a recall of support from the target village. 
		( UM.OriginVillageID = @VID  AND CommandType not in (2 /*return*/, 3/*recall*/) ) 

		OR 	     

		-- get rid of all movements to this village if this movements is a returning attack 
		-- OR this movements is a support recall
		( UM.DestinationVillageID = @VID AND CommandType in (2,3))  -- 2==return, 3==recall

	) 

	--
	-- note the transaction
	--	
	insert into PlayerPFLog
    	(PlayerID,Time ,EventType,Credits ,Cost,notes)
		values
	    (@PlayerID,getdate(),3,@RestartCost,-1
	    	, 'VID=' + Cast(@VID as varchar(max)) + ' - restart'
	    )
	    		    

end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'uVillageName FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @VID' + ISNULL(@VID, 'Null') + CHAR(10)

		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	




GO


