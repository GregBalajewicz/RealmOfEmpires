    
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qFriendInviteCode')
	BEGIN
		DROP  Procedure  qFriendInviteCode
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].qFriendInviteCode
		@UserID as uniqueidentifier
AS
	
	
begin try 

	declare @friendinviteCode varchar(max)
	select @friendinviteCode = friendinviteCode from FriendInviteCodes where userid = @UserID

	if @friendinviteCode is null  begin 

		while (0=0) BEGIN
			--
			-- try to generate a random friend invite code and break from the loop once a unique one was created
			--
			SELECT @friendinviteCode = SUBSTRING(CONVERT(varchar(40), NEWID()),0,9)

			insert into FriendInviteCodes (Userid, friendinviteCode) 
				(select userid, @friendinviteCode 
					from Users 
					where userid = @UserID
					and @friendinviteCode not in (select friendinviteCode from FriendInviteCodes )
				)
			
			select @friendinviteCode = friendinviteCode from FriendInviteCodes where userid = @UserID
			if not @friendinviteCode is null BEGIN
				break
			END
		END 
	end 


	select friendinviteCode, FICU.FriendWhoGotRewardUserID, FICU.RewardStage 
		from FriendInviteCodes FIC 
		left join  FriendInviteCodeUses FICU 
			on FIC.userid = FICU.UserID
			where FIC.userid = @UserID
			and FICU.userid is null 

end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qFriendInviteCode FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserID' + ISNULL(CAST(@UserID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 