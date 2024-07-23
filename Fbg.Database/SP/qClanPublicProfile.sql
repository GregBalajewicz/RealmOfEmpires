IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qClanPublicProfile')
	BEGIN
		DROP  Procedure  qClanPublicProfile
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[qClanPublicProfile]
		@ClanID int
AS

begin try 
			--select Point and villages #
			select  isnull(sum(V.points),0) as villagepoints
				,(select count(villageID)   from villages ) as VillagesCount 
			from villages v 
			join ClanMembers  cm on cm.PlayerID = v.OwnerPlayerID 

			where cm.clanid=@ClanID
		

			--select Players #
			select count(*) as PlayersCount from ClanMembers where clanid=@ClanID
			-- select Clan Information
			select * from Clans where clanid=@ClanID
	
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qClanPublicProfile FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @ClanID' + ISNULL(CAST(@ClanID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	




 