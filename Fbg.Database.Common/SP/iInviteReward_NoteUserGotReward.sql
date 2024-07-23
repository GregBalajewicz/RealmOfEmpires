      
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iInviteReward_NoteUserGotReward')
	BEGIN
		DROP  Procedure  iInviteReward_NoteUserGotReward
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].iInviteReward_NoteUserGotReward
    @userIDGuid uniqueidentifier ,
    @numOfInvitesSent int, 
    @numOfServants int,
    @rewardNumber int
AS
set nocount on 

begin try 


    insert into InviteRewards values (@userIDGuid, getdate(), @numOfInvitesSent, @numOfServants, @rewardNumber, '')


end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iInviteReward_NoteUserGotReward FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @userIDGuid' + ISNULL(CAST(@userIDGuid AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	


 