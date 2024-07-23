  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iNoTransportCoinsVillages')
	BEGIN
		DROP  Procedure  iNoTransportCoinsVillages
	END

GO
 
 SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[iNoTransportCoinsVillages]
		
		@PlayerID int  ,
		@VillageID int
		
AS
	
begin try 
	insert into NoTransportVillages (VillageID) 
		select VillageID 
			from Villages 
			where VillageID=@VillageID 
				-- ensure this is the owner of this village
				and OwnerPlayerID=@PlayerID 
				-- ensure this village is not already in on transport list
				and villageid not in (select villageid from NoTransportVillages where VillageID=@VillageID)
				
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iNoTransportCoinsVillages FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(@PlayerID , 'Null') + CHAR(10)
		+ '   @VillageID' + ISNULL(@VillageID , 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

 
