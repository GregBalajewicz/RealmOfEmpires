   
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iRegisterInvited')
	BEGIN
		DROP  Procedure  iRegisterInvited
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].iRegisterInvited
		@PlayerID as int
		,@FacebookIDs as varchar(max)--this holds the ID's of the Facebook Members separated by Comma ','  ex:(1,2,3)
AS
	
	
begin try 
	declare @time as datetime
	set @time =getdate();


	--
	-- insert an invite unless it is already there. 
	--  it should  not be there however. Unless there was some weird problem ... 
	--	either case, we want to prevent duplicate records so that selects are easier
	-- 
	--   THIS SQL IS VERY SIMILLAR TO THE ONE IN "iRegisterInvited_Gift" so if you make any changes
	--      here, check out the version in iRegisterInvited_Gift to see if this one needs changes as this
	--      SP also inserts into Invites table
    --
	insert into Invites(PlayerID, InvitedID, Type, InvitedOn)
		Select @PlayerID
			,ID
			,1 -- means facebook invitation. Hardcoded for now.
			, @time 
		from fnGetIds(@FacebookIDs) IDs
		where not exists (
			select * from Invites I2
				where I2.StatusID = 1 -- status 1==pending invitation
					and I2.Type = 1 -- means facebook invitation
					and I2.PlayerID = @PlayerID
					and I2.InvitedID = IDs.ID
			)			


end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iRegisterInvited FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @FacebookIDs' + ISNULL(CAST(@FacebookIDs AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 