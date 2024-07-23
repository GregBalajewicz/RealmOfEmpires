IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iClan')
	BEGIN
		DROP  Procedure  iClan
	END

GO

CREATE Procedure [dbo].[iClan]
		
		@Name nvarchar(30),
		@Desc nvarchar(200),
		@ClanID int output ,
		@PlayerID int
		
AS
	
	
begin try 
		begin tran
		set @ClanID=0;

		if not exists(select * from Clans where Name=@Name) begin 
			 Insert into Clans([Name],[Description],PublicProfile)           
				values(@Name,@Desc,'')     ;  
			select @ClanID=@@identity ;

			--put the player as member of clan 
			 Insert into ClanMembers(ClanID,PlayerID)           
			values(@ClanID,@PlayerID) 			
			
			-- put the player as in owner role of the forum 
			exec [iPlayerRole]  @ClanID,@PlayerID,0

			-- put a general Forum
			declare @ForumID as int
			declare @Date as datetime
			set @Date=GETDATE()
			exec [tbh_Forums_InsertForum] @Date,'System','Default Forum',0,1,'Automatically generated forum','http://static.realmofempires.com/images/Folder.gif',@ClanID,1,0,@ForumID
			
			-- by default, new members have invite rights / Inviter Role
			insert into DefaultRoles values (@ClanID, 2)
		end
		else
			Select @ClanID;

			commit tran
end try


begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iClan FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @Name:' + ISNULL(@Name , 'Null') + CHAR(10)
		+ '   @Desc:' + ISNULL(@Desc , 'Null') + CHAR(10)
		+ '   @ClanID:' + ISNULL(CAST(@ClanID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PlayerID:' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @Role:' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

 