IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerForumChanged')
	BEGIN
		DROP  Procedure  qPlayerForumChanged
	END

GO


/*
WHY the "with(nolock)" hint on all tables ?? because we really don't care about dirty reads here and this is executed so 
    often, that we want this to run as fast as possible. 
*/
CREATE Procedure [dbo].[qPlayerForumChanged]
		@PlayerID int
AS
	
	
begin try 
	declare @ClanForumCheckedOn as datetime ;
	declare @LastPost as datetime ;
	declare @Result as int;
	declare @PlayerName as nvarchar(25);
	SELECT     @ClanForumCheckedOn=ClanForumCheckedOn,@PlayerName=Name
        FROM         Players with(nolock)
        where PlayerID=@PlayerID

	SELECT     @LastPost=tbh_Posts.LastPostDate
        FROM
            tbh_Posts with(nolock)
            INNER JOIN tbh_Forums with(nolock)
                ON tbh_Posts.ForumID = tbh_Forums.ForumID 
            INNER JOIN Clans with(nolock)
                ON tbh_Forums.ClanID = Clans.ClanID                     
            INNER JOIN ClanMembers with(nolock)
                ON Clans.ClanID = ClanMembers.ClanID
				
        where ClanMembers.PlayerID=@PlayerID and tbh_Posts.lastpostby<>@PlayerName
		and (SecurityLevel = 0
				OR exists (select * from PlayerInRoles PIR WHERE PIR.PlayerID = @PlayerID AND RoleID in (select ID from dbo.fnGetPlayerRolesFromSecurityLevel(SecurityLevel)))
			) 
        and AlertClanMembers=1
        order by tbh_Posts.LastPostDate asc
	if( @ClanForumCheckedOn < @LastPost)
		begin
			set @Result=1; -- there are new post since player last checekd the forum 
		end
	else
		begin
			set @Result=0; -- there are NO new post since player last checekd the forum 
		end
    select @Result;
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qPlayerForumChanged FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @ClanForumCheckedOn' + ISNULL(CAST(@ClanForumCheckedOn AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @LastPost' + ISNULL(CAST(@LastPost AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @Result' + ISNULL(CAST(@Result AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	  