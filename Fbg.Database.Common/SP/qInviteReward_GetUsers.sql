     
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qInviteReward_GetUsers')
	BEGIN
		DROP  Procedure  qInviteReward_GetUsers
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].qInviteReward_GetUsers
    @NumDaysBack int ,
    @NumLastRewards int, 
    @MinNumberOfInvites int
AS
set nocount on 

begin try 

create table #UsersWithInvites(UserID uniqueidentifier,InviteCount int, LastTimeRewardGiven datetime )


IF @NumLastRewards = 0 BEGIN
    /*
    ----------
    Case 1 - player never received a reward. 
    ----------
      IF you have never received this reward
      AND invited at least @MinNumberOfInvites person in the last @NumDaysBack days 
    */
    insert into #UsersWithInvites
    select P.userid, count(distinct invitedid) as InviteCount, getdate()
	    from invites I
	    join players P
		    on I.PlayerID = P.PlayerID
	    join aspnet_users U
		    on p.UserID = U.UserID
		join aspnet_membership M
		    on U.userid = M.userid
	    where invitedon > dateadd(day, -@NumDaysBack,getdate())
		    and P.UserID not in (select userid from InviteRewards) -- did not get a reward yet 
		    and P.UserID not in (select userid from dbo.PaymentTransactionLog where amount_gross > 0) -- no paypal trans
		    and U.UserName not in (select FacebookID from dbo.OfferCompletedTransactionLog where credits > 0) -- no offers
		    and dateadd(month, 1, M.CreateDate) < getdate() -- entered the game at least 1 months ago
	    group by P.userid
	    having count(distinct invitedid) >= @MinNumberOfInvites
	    order by count(distinct invitedid) desc
END ELSE BEGIN 
    /*
    ----------
    Case X - player received the reward X times
    ----------  
    */
    insert into #UsersWithInvites
    select P.userid
	    , count(distinct invitedid) as InviteCount
	    , (select Time from InviteRewards where userID = P.UserID and RewardNumber = @NumLastRewards) as 'LastTimeRewardGiven'
	    from invites I
	    join players P
		    on I.PlayerID = P.PlayerID
	    join aspnet_users U
		    on p.UserID = U.UserID
	    where 
		    invitedon > (select Time from InviteRewards where userID = P.UserID and RewardNumber = @NumLastRewards)
		    and @NumLastRewards = (select top 1 RewardNumber from InviteRewards where userID = P.UserID order by RewardNumber desc )
		    and getdate() >= dateadd(day, @NumDaysBack, (select Time from InviteRewards where userID = P.UserID and RewardNumber = @NumLastRewards))
	    group by P.userid
	    having count(distinct invitedid) >= @MinNumberOfInvites
	    order by count(distinct invitedid) desc

END

select UserID, InviteCount from #UsersWithInvites


end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qInviteReward_GetUsers FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @NumDaysBack' + ISNULL(CAST(@NumDaysBack AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @NumLastRewards' + ISNULL(CAST(@NumLastRewards AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @MinNumberOfInvites' + ISNULL(CAST(@MinNumberOfInvites AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	


 