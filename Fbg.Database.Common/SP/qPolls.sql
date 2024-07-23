IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPolls')
	BEGIN
		DROP  Procedure  qPolls
	END

GO

CREATE Procedure dbo.qPolls
AS

select PollID,Title,Description,OfferType,OfferAmount,RealmID,PollType, Run_StartOn, Run_ForHours from Polls

select PollID,PollOptionID,[Text] from PollOptions