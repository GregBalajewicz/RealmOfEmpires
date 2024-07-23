 
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dVillageTag')
	BEGIN
		DROP  Procedure  dVillageTag
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[dVillageTag]
		@PlayerID int,
		@TagID int,
		@VillageID int
AS
	
	
begin try 

	begin tran
		if exists (select * from Tags join villages 
					on tags.playerid=villages.ownerplayerid 
					where TagID=@TagID and PlayerID=@PlayerID and villageid=@VillageId)
		begin
			delete from VillageTags
			where TagID=@TagID and VillageID=@VillageID
		end

	commit tran
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'dVillageTag FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @TagID' + ISNULL(CAST(@TagID AS VARCHAR(30)), 'Null') + CHAR(10)
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(30)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



  