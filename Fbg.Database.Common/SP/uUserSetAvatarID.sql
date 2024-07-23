IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uUserSetAvatarID')
	BEGIN
		DROP  Procedure  uUserSetAvatarID
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO
CREATE Procedure dbo.uUserSetAvatarID
	@UserID as uniqueidentifier ,
	@AvatarID as int,
	@Result int output
	
AS
-- 
--
--
begin try 
	begin tran
		
		declare @avatarType int;
		declare @canSet tinyint;
		
		set @canSet = 0;
		
		select @avatarType = AvatarType from Avatars2 where AvatarID = @AvatarID;
		if @@rowcount < 1 begin --avatar wasnt found
			set @canSet = 0;
			set @Result = 1;
		end 
		else begin
			
			--for type 2 and 3 check user ownership
			if @avatarType = 2 or @avatarType = 3 begin
				select top 1 AvatarID from UsersUnlockedAvatars where UserID = @UserID and AvatarID = @AvatarID ;
				if @@rowcount < 1 begin 
					set @canSet = 0;
				end
				else begin
					set @canSet = 1;
				end
			end 
			else begin
				set @canSet = 1;
			end
				
				
			if @canSet = 1 begin

				Update Users set AvatarID = @AvatarID where UserID = @UserID;	
				if @@rowcount < 1 begin 
					--
					-- the above update is know to fail for one one reason - this is a user who never registerd on a realm, therefore has 
					--	no record in users table; see iRegisterPlayerInCommon
					--
					insert into Users(UserID, Credits ,GlobalPlayerName, AvatarID) values (@UserID, 25, null, @AvatarID);
				END
				set @Result = 0;
			
			end
			else begin
				set @Result = 1;
			end
			
		end

		
	commit tran
	
	
end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'uUserSetAvatarID FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserID:'					  + ISNULL(CAST(@UserID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AvatarID:'					  + ISNULL(@AvatarID, 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



