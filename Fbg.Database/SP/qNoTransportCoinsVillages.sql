 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qNoTransportCoinsVillages')
	BEGIN
		DROP  Procedure  qNoTransportCoinsVillages
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[qNoTransportCoinsVillages]
		@PlayerID int
AS

begin try 
		-- select villages that player can use in transports
		select VillageID,OwnerPlayerID,Name,Coins,XCord,YCord from villages 
		where ownerplayerid=@PlayerID 
			and villageid not in (select Villageid from NoTransportVillages)
		order by Name

		-- select villages that player put in No Transport List
		select v.VillageID,OwnerPlayerID,Name,Coins,XCord,YCord from villages v 
		join NoTransportVillages nv
		on v.VillageID=nv.VillageID
		where ownerplayerid=@PlayerID
		order by Name
		
		
	
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qNoTransportCoinsVillages FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	




 