IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[iPendingAccountRecovery]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[iPendingAccountRecovery]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].[iPendingAccountRecovery]
	@Email nvarchar(256)
	, @NewUserName nvarchar(256)
	, @UID nvarchar(256)
AS
	
begin try
	
	begin tran
		if CURSOR_STATUS('global', 'curActRec')>=-1
		begin
			deallocate curActRec
		end
		declare curActRec cursor for
		select U.UserId, U.UserName, F.Data from aspnet_membership M
		   join aspnet_Users U
			on U.Userid = M.Userid
		   join UserFlags F
			on U.Userid = F.Userid		
		  where 
			F.Flagid = 13 and F.Data = 2
		   and M.Email = @email
		   order by F.Data desc
			
		open curActRec
		declare @cUserId uniqueidentifier
		declare @cUserName nvarchar(256)
		declare @cFlagData int
		
		--
		-- update old requests to state -3
		--
 		update AccountRecovery 
 			set [State] = -5
 		where
			Email = @Email
		
		--
		-- iterate through the result set in the cursor
		--
		fetch next from curActRec into @cUserId, @cUserName, @cFlagData
		while (@@FETCH_STATUS = 0)
		begin		
			--	    
			-- if length is less than 20 it is probably a facebook id - in this case state will be 3
			--			
			if @cFlagData = 2
			begin 

				IF NOT EXISTS(select * from AccountRecovery where
					Email = @Email and OldUserName = @cUserName and [State] = 0)
				begin
					insert into AccountRecovery
							   ([State]
							   , Email
							   , UserId
							   , OldUserName
							   , NewUserName
							   , PlayerNames
							   , "UID")
						 values
								(case when len(@cUserName) < 20 then 3 else 0 end
								, @Email
								, @cUserId
								, @cUserName
								, @NewUserName
								, dbo.fnGetAllPlayerNamesForUserId(@cUserId)
								, @UID)
				end
			end
			fetch next from curActRec into @cUserId, @cUserName, @cFlagData			
 		end
		close curActRec
		deallocate curActRec
		--
		-- now select Action and items in AccountRecovery
		--			
 		select * from AccountRecovery where
			Email = @Email and RequestDate >= dateadd(day, -2, getdate()) and ([State] = 0 or [State] = 3)
    commit tran
    
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK
	
	SET @ERROR_MSG = 'iPendingAccountRecovery FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @Email:' + ISNULL(CAST(@Email AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @NewUserName:' + ISNULL(CAST(@NewUserName AS VARCHAR(max)), 'Null') + CHAR(10)	
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

GO