IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qMessageDetail')
	BEGIN
		DROP  Procedure  qMessageDetail
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go



CREATE Proc [dbo].[qMessageDetail]
	@PlayerID as int
	,@RecordID as int
as
	select 
		ma.MessageID
		,P_SENDER.playerid
		,P_SENDER.name
		,m.Subject
		,m.Body
		,m.TimeSent
		,m.RecipientNames
		,ma.RecordID 
		,ma.Type
		, isnull(substring(PN.Note, 1, 71),'') as 'SenderPN'
		, (select top 1 MNA.RecordID from Messageaddressees MNA join Messages MN on MNA.MessageID = MN.MessageID where MNA.[Type]=1 and MNA.PlayerID=@PlayerID and MN.TimeSent > M.TimeSent and MNA.FolderID is null order by MN.TimeSent asc) as NextRec
		, (select top 1 MPA.RecordID from Messageaddressees MPA join Messages MP on MPA.MessageID = MP.MessageID where MPA.[Type]=1 and MPA.PlayerID=@PlayerID and MP.TimeSent < M.TimeSent and MPA.FolderID is null order by MP.TimeSent desc) as PrevRec
	from Messageaddressees MA
	join Messages M
		on MA.messageid = M.messageid
	join players P_SENDER 
		on P_SENDER.playerid=M.playerid
	left join PlayerNotes PN
	    on PN.PlayerID=P_SENDER.PlayerID and PN.NoteOwnerPlayerID=@PlayerID
	where 
		MA.PlayerID=@PlayerID 
		and MA.RecordID=@RecordID

	if(@@ROWCOUNT>0)
	begin
		update MessageAddressees set IsViewed=1 where RecordID=@RecordID
	end