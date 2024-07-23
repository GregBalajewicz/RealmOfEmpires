  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uClanRename')
	BEGIN
		DROP  Procedure  uClanRename
	END

GO

CREATE Procedure uClanRename
	@ClanID int
	,@newClanName nvarchar(30)
	,@PlayerID int
AS

BEGIN TRY
declare @Result as bit
set @Result = 0 -- false means clan already exists


if not exists(select * from Clans where Name=@newClanName) begin 
	update Clans set [Name] = @newClanName
	from Clans 
	join PlayerInRoles
	on Clans.ClanID=PlayerInRoles.ClanID
		where Clans.ClanID = @ClanID
		and PlayerID=@PlayerID -- security check to validate that the only owner can change the clan name 
		and RoleID=0 --Owner Role 
		
	set @Result=1;
	select @Result
end 
	select @Result

end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'uClanRename FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @ClanID' + ISNULL(CAST(@ClanID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @newClanName' + ISNULL(@newClanName, 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



