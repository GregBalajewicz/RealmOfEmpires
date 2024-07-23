IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qClanMembers')
	BEGIN
		DROP  Procedure  qClanMembers
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[qClanMembers]
		@ClanID int
AS

begin try 
		select  P.Points as villagepoints
			, p.PlayerID
			, p.Name
			, (select count(villageID)   from villages where	OwnerPlayerID = p.PlayerID) as VillagesCount
			, p.LastActivity as LastLoginTime
			, ACS.StewardPlayerID
			, ACS_P.Name as StewardPlayerName
			--, ACS_C.Name as StewardClanName
			--, ACS_C.ClanID as StewardClanID
			, P.SleepModeActiveFrom
			, P.VacationModeRequestOn --this is just request date,not the time it takes effect on, that happens X days later
			, P.WeekendModetakesEffectOn
			from Players  p
			join ClanMembers  cm on cm.PlayerID = p.PlayerID
			left join AccountStewards ACS
				on ACS.PlayerID = cm.PlayerID 
				and ACS.State = 2 /*active/accepted stewards*/
			--left join ClanMembers ACS_CM
			--	on ACS_CM.PlayerID = ACS.StewardPlayerID
			--left join Clans ACS_C
			--	on ACS_C.ClanID = ACS_CM.ClanID
			left join Players ACS_P
				on ACS_P.PlayerID = ACS.StewardPlayerID
		where cm.clanid=@ClanID
		order by villagepoints desc



		----------select Players Roles
		SELECT    PlayerInRoles.PlayerID,PlayerInRoles.ClanID,PlayerInRoles.RoleID
		FROM         Clans INNER JOIN
                      PlayerInRoles ON Clans.ClanID = PlayerInRoles.ClanID
		where Clans.ClanID=@ClanID;
		
		
	
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qClanMembers FAILED! ' +  + CHAR(10)
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




 