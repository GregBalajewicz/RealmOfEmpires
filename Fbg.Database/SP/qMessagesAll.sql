IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qMessagesAll')
	BEGIN
		DROP  Procedure  qMessagesAll
	END

GO

--
-- returns messages addressed to player (@RecipientID) 
--
--	THIS IS IGNORED as we get all messages cause we want to know whos in a folder
--  or not for the starred feature. Left for compatibility :
--	@FolderID - if specified, will return messages in this folder. If set to NULL, will return all messages in inbox that are not in some custom folder
--
CREATE Procedure qMessagesAll
	@RecipientID as int
	,@MaxDaysOld as int
	,@FolderID as int = null 
as
	declare @OldestToRetrieve DateTime
	
	set @MaxDaysOld = 0 - @MaxDaysOld
	set @OldestToRetrieve = Dateadd(day, @MaxDaysOld, getdate())
	
	select 0 -- left for compatibility
		,m.subject
		,m.timesent
		,P_SENDER.playerid
		,P_SENDER.name as 'Sender'
		,ma.isviewed
		,ma.RecordID 
		,ma.FolderID
	from messageaddressees ma
	join messages m 
		on m.messageid=ma.messageid
	join players P_SENDER 
		on P_SENDER.PlayerID = m.PlayerID
	where 
		ma.playerid=@RecipientID 
		and ma.type=1 -- this means its a inbox/inbound message, not a sent message
		--and (
		--		(@FolderID is null AND ma.FolderID is null) -- folder is not specified, so get all uncategorized messages
		--		OR (@FolderID is not null AND ma.FolderID = @FolderID) -- folder is specified so get all messages in this folder
		--	)
	and (m.timesent > @OldestToRetrieve or ma.FolderID is not null)
	order by m.timesent desc
	
	update players set MessagesCheckedOn=getDate(), NewMessageIndicator=0 where PlayerID=@RecipientID

go