IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qDonors')
	BEGIN
		DROP  Procedure  qDonors
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE proc [dbo].[qDonors]
@RealmID as int
as
	select P.PlayerID,P.Name
		from donors d
	join players p on p.userid=d.userid and p.RealmID=@RealmID
	where d.WantsToBeAnonymous=0
	order by lastdonatedon desc