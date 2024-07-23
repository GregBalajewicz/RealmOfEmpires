    
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iRegisterInvited_Gift')
	BEGIN
		DROP  Procedure  iRegisterInvited_Gift
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].iRegisterInvited_Gift
		@PlayerID as int
		,@GiftID as int
		,@FacebookIDs as varchar(max)--this holds the ID's of the Facebook Members separated by Comma ','  ex:(1,2,3)
		,@RequestID as varchar(50)
AS
	
	
begin try 
	declare @time as datetime
	declare @userid as uniqueidentifier
	set @time =getdate();



	--
	-- insert the sent Gifts. 
	--  NOTE, we rcheck here if a player already got a gift today from the sending player 
	--  to prevent possible cheating (since we dont allow more than 1 gift per person per day) 
	--  
	-- 
	
	select ID into #FacebookIDs from fnGetIds(@FacebookIDs) IDs
	
	select top 1 @userid = userid from players where playerid = @playerid
	
	--
	-- get the players that already got gifts from sender today and remove them from list of ppl who got gifts
    CREATE TABLE #GiftsSentToPpl(
        SentTo      varchar(320)    NOT NULL
    )
    INSERT #GiftsSentToPpl (SentTo) EXEC qGift_GetSentToday @userid
	delete #FacebookIDs where ID in (select SentTO from #GiftsSentToPpl)


	--
	-- insert gifts
	--	
	insert into GiftsSent(PlayerID, GiftID, SentTo, SentOn, Type, StatusID, RequestID)
		Select @PlayerID
			, @GiftID
			, ID
			, @time
			, 1 -- means facebook gift Hardcoded for now.
			, 1 
			, @RequestID
		from #FacebookIDs IDs
		
	--
	-- now record invites if any
	--  IE, if person receiving a gift is new, (not yet playing) then this gift is also 
	--  an invite so make sure to invite him. 
	--   THIS SQL IS VERY SIMILLAR TO THE ONE IN "iRegisterInvited"
	--
	insert into Invites(PlayerID, InvitedID, Type, InvitedOn)
		Select @PlayerID
			,ID
			,1 -- means facebook invitation. Hardcoded for now.
			, @time 
		from #FacebookIDs IDs
		where 
		    not exists (
			    select * from Invites I2
				    where I2.StatusID in( 1 /*pending invitation*/, 2/*accepted invitation*/, 3/*reward claimed*/)
					    and I2.Type = 1 -- means facebook invitation
					    and I2.PlayerID = @PlayerID
					    and I2.InvitedID = IDs.ID
			    )
			AND cast(ID as varchar(320)) NOT IN (select UserName from aspnet_Users U)



end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iRegisterInvited_Gift FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @FacebookIDs' + ISNULL(CAST(@FacebookIDs AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @GiftID' + ISNULL(CAST(@GiftID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @RequestID' + ISNULL(CAST(@RequestID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 