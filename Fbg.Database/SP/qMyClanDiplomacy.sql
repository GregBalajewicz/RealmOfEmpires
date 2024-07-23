IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qMyClanDiplomacy')
	BEGIN
		DROP  Procedure  qMyClanDiplomacy
	END

GO

CREATE Procedure [dbo].[qMyClanDiplomacy]
		
		@ClanID int  
		
AS
	
begin try 
		--select Ally Clans
			SELECT     Clans.Name, ClanDiplomacy.ClanID, ClanDiplomacy.OtherClanID, ClanDiplomacy.StatusID
			FROM         Clans INNER JOIN
                      ClanDiplomacy ON Clans.ClanID = ClanDiplomacy.OtherClanID 
			where ClanDiplomacy.ClanID=@ClanID and StatusID=0
			
			--select Enmy Clans
			SELECT     Clans.Name, ClanDiplomacy.ClanID, ClanDiplomacy.OtherClanID, ClanDiplomacy.StatusID
			FROM         Clans INNER JOIN
                      ClanDiplomacy ON Clans.ClanID = ClanDiplomacy.OtherClanID 
			where ClanDiplomacy.ClanID=@ClanID and StatusID=1
			--select NAP Clans
			SELECT     Clans.Name, ClanDiplomacy.ClanID, ClanDiplomacy.OtherClanID, ClanDiplomacy.StatusID
			FROM         Clans INNER JOIN
                      ClanDiplomacy ON Clans.ClanID = ClanDiplomacy.OtherClanID 
			where ClanDiplomacy.ClanID=@ClanID and StatusID=2

		
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qMyClanDiplomacy FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @ClanID:' + ISNULL(@ClanID , 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

 
 