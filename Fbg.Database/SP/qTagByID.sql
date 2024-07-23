  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qTagByID')
	BEGIN
		DROP  Procedure  qTagByID
	END

GO
CREATE Procedure qTagByID
	@PlayerID as int,
	@TagID as int
AS
begin try 

			select TagID,Name,Description,Sort,PlayerID from Tags 
			where PlayerID=@PlayerID and TagID=@TagID
			order by [Sort] asc
			
			select villages.VillageID,OwnerPlayerID,Name,Coins,CoinsLastUpdates,XCord,YCord,Loyalty,LoyaltyLastUpdated from villages  
			join villagetags on villages.VillageID=villagetags.VillageID
			where tagid=@TagID and ownerplayerID=@PlayerID
			order by Name
			
			select villages.VillageID,OwnerPlayerID,Name,Coins,CoinsLastUpdates,XCord,YCord,Loyalty,LoyaltyLastUpdated from villages  
			where villages.VillageID not in 
				(select villagetags.villageid from villagetags 
					join villages on villagetags.VillageID=villages.VillageID 
				 where  ownerplayerid=@PlayerID and tagid=@TagID)
			and ownerplayerID=@PlayerID
			order by Name
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qTagByID FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @TagID' + ISNULL(CAST(@TagID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	




 go

