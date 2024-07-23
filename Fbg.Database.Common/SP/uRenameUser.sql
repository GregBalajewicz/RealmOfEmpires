IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uRenameUser')
	BEGIN
		DROP  Procedure  uRenameUser
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO
CREATE Procedure dbo.uRenameUser
	@UserID uniqueidentifier ,
	@NewName varchar(25)
	
	
AS
-- 
--
--
begin try 
	begin tran

		declare @retval int
		--
		-- check if this player name is already used by someone else 
		--	
		-- TODO: THIS IS NOT THREAD SAFE!!! 
		--
		IF     NOT EXISTS (select PlayerID from Players where  [Name]=@NewName and UserID <> @UserID
					 UNION select PlayerID from DeletedPlayers where [Name]=@NewName and UserID <> @UserID)
		   AND NOT EXISTS (select userID from Users where GlobalPlayerName=@NewName and UserID <> @UserID)
		BEGIN 
			Update Users set GlobalPlayerName = @NewName where UserID = @UserID	
			if @@rowcount < 1 begin 
				--
				-- the above update is know to fail for one one reason - this is a user who never registerd on a realm, therefore has 
				--	no record in users table; see iRegisterPlayerInCommon
				--
				insert into Users(UserID, Credits ,GlobalPlayerName) values (@UserID, 25, @NewName)
			END
			set @retval =  0
		END 
		ELSE BEGIN 
			--
			-- player name already taken!
			--
			set @retval =  2
		END 
		
	commit tran

	select @retval
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'uRenameUser FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserID:'					  + ISNULL(CAST(@UserID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @NewName:'				  + ISNULL(@NewName, 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



