IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iPlayerRole')
	BEGIN
		DROP  Procedure  iPlayerRole
	END

GO

CREATE Procedure [dbo].[iPlayerRole]
		@ClanID int,
		@PlayerID int,
		@RoleID int
AS
	
	
begin try 
declare @Error as int ;
set @Error=1;--set as error 
		if not exists(select * from  PlayerInRoles where ClanID=@ClanID  and PlayerID=@PlayerID and RoleID=@RoleID)
			begin
				Insert into PlayerInRoles(ClanID,PlayerID,RoleID)           
				values(@ClanID,@PlayerID,@RoleID)       
				set @Error=0;-- remove any error
				Select @Error;
			end
		else
				select @Error;
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iPlayerRole FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @ClanID' + ISNULL(CAST(@ClanID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @RoleID' + ISNULL(CAST(@RoleID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

 