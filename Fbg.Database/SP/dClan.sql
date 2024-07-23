
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dClan')
	BEGIN
		DROP  Procedure  dClan
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[dClan]
		@ClanID int
		, @DeletingOwnersPlayerName nvarchar(25) = null
		
AS
	
	
begin try 

declare @ReportID as int;
declare @ClanName as nvarchar(30);

	begin tran
    
		delete from ForumSharing where ClanID = @ClanID  
		delete from ForumSharingWhiteListedClans where ClanID = @ClanID  
		delete from ForumSharingWhiteListedClans where WhiteListClanID = @ClanID  
		delete from ForumSharing where forumid in (select forumid from tbh_Forums WHERE ClanID=@ClanID)

		delete DefaultRoles where ClanID = @ClanID
		delete ClanInviteLog where ClanID = @ClanID
        delete chat where clanid = @ClanID

		DELETE pv
		FROM PlayerPostViews AS pv 
		INNER JOIN tbh_Posts as p on pv.ThreadPostID=p.PostID
		inner join tbh_Forums AS f ON p.ForumID=f.ForumID
		INNER JOIN Clans AS c ON c.ClanID=f.ClanID
		WHERE c.ClanID=@ClanID;

		DELETE p
		FROM tbh_Posts AS p INNER JOIN tbh_Forums AS f ON p.ForumID=f.ForumID
		INNER JOIN Clans AS c ON c.ClanID=f.ClanID
		WHERE c.ClanID=@ClanID;

		DELETE f
		FROM tbh_Forums AS f INNER JOIN  Clans AS c ON c.ClanID=f.ClanID
		WHERE c.ClanID=@ClanID;
		
		
		delete from PlayerInRoles where clanid=@ClanID;
		
		delete ClanEvents where clanID = @ClanID

		delete from ClanInvites where ClanID = @ClanID  
		--get clan name
		select @ClanName=Name from Clans where ClanID=@ClanID; 
		-- insert a report  
		insert into Reports values (getdate(), dbo.Translate('dClan_theClan') +@ClanName+ dbo.Translate('dClan_hasDisbanded'), 4
		,dbo.Translate('dClan_theClan')+@ClanName+',' + dbo.Translate('dClan_wereMember') + isnull(@DeletingOwnersPlayerName, ' the owner') + '.</br>'+
dbo.Translate('dClan_autoWithoutClan'))
		set @ReportID = SCOPE_IDENTITY()
			-- send the report to all clan members to know that the clan is deleted 
		insert into ReportAddressees (PlayerID, ReportID, ForwardedByPlayerID, ForwardedOn, IsViewed) 
			select PlayerID,@ReportID,null,null,0
			from clanmembers where clanid=@ClanID
		update Players set NewReportIndicator = 1 where PlayerID in
			(select PlayerID from clanmembers where clanid=@ClanID)
			
		delete from ClanDiplomacy where ClanID=@ClanID or OtherClanID=@ClanID;
		delete from ClanMembers where ClanID = @ClanID  

		delete from Clans where ClanID = @ClanID  
		

	commit tran
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'dClan FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @ClanID' + ISNULL(CAST(@ClanID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @ClanName' + ISNULL(CAST(@ClanName AS VARCHAR(30)), 'Null') + CHAR(10)
		+ '   @ReportID' + ISNULL(CAST(@ReportID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

begin try
	declare @RealmID int = (select AttribValue from RealmAttributes where AttribID = 33)
	exec FBGC.fbgcommon.dbo.dChat_ClanChat @RealmID = @RealmID, @ClanID = @ClanID
end try

begin catch
DECLARE @ERROR_MSG2 AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG2 = 'dClan -- DELETE FROM USERSTOCHATS2 FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @ClanID' + ISNULL(CAST(@ClanID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @ClanName' + ISNULL(CAST(@ClanName AS VARCHAR(30)), 'Null') + CHAR(10)
		+ '   @ReportID' + ISNULL(CAST(@ReportID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG2,11,1)	

end catch

 