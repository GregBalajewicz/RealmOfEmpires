  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qTagsByVillageID')
	BEGIN
		DROP  Procedure  qTagsByVillageID
	END

GO
CREATE Procedure qTagsByVillageID
	@PlayerID as int,
	@VillageID as int
AS
begin try 

		select distinct Tags.TagID,Tags.Name,[Sort] from Tags  
			join villagetags on Tags.TagID=villagetags.TagID
			join villages on villagetags.VillageID=villagetags.VillageID
		where villagetags.VillageID=@VillageID 
				and ownerplayerID=@PlayerID and playerid=@PlayerID
		order by [Sort] asc
		
		select  Tags.TagID,Tags.Name from Tags  
		where Tags.tagid not in 
			(select villagetags.tagid from villagetags 
				join villages on villagetags.VillageID=villages.VillageID 
			where villagetags.villageid=@VillageID and ownerplayerid=@PlayerID)
		and playerid=@PlayerID
		order by [Sort] asc
		
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qTagsByVillageID FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	




 go

 