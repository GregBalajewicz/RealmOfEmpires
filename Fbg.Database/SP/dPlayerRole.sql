 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dPlayerRole')
	BEGIN
		DROP  Procedure  dPlayerRole
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[dPlayerRole]
		@ClanID int,
		@PlayerID int,
		@RoleID int
AS

begin try 
	begin tran
		-- see if more then one owner 
		declare @Count as int;
			if @RoleID=0
				begin
					select @Count =count(PlayerInRoles.RoleID)FROM    PlayerInRoles
					where PlayerInRoles.RoleID=0 and PlayerInRoles.ClanID=@ClanID
				end
			else
			     set @Count=2;
		 
		--delete Player
		if @Count>1
		begin
			delete from PlayerInRoles where ClanID = @ClanID and PlayerID=@PlayerID and RoleID=@RoleID
		end

		select @Count

	commit tran
end try
	

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'dPlayerRole FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @ClanID' + ISNULL(CAST(@ClanID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @Count' + ISNULL(CAST(@Count AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	
 