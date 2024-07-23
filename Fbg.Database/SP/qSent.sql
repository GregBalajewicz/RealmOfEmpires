IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qSent')
	BEGIN
		DROP  Procedure  qSent
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go


CREATE proc [dbo].[qSent]
	@PlayerID as int
	,@MaxDaysOld as int

as

	declare @OldestToRetrieve DateTime
	
	set @MaxDaysOld = 0 - @MaxDaysOld
	set @OldestToRetrieve = Dateadd(day, @MaxDaysOld, getdate())

	select  0 -- left for compatibility
		,p.playerid
		,p.name
		,m.subject
		,m.timesent
		,m.recipientnames
		,ma.isviewed
		,ma.RecordID 
	from messages m
	join messageaddressees ma 
		on ma.messageid=m.messageid 
		and ma.type=0
	join players p 
		on p.playerid=m.playerid
	where m.playerid=@PlayerID
		and m.timesent > @OldestToRetrieve	
	order by m.timesent desc