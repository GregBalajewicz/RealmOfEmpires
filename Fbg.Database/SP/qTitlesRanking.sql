IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qTitlesRanking')
	BEGIN
		DROP  Procedure  qTitlesRanking
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE proc [dbo].qTitlesRanking
as

select T.TitleID
	, T.Title_Male
	, T.Title_Female
	, Description
	, MaxPoints
	, count(P.PlayerID) 
	, ROW_NUMBER() OVER (order by T.MaxPoints) as Level
	from Titles T
	left join players P
		on P.TitleID = T.TitleID 
		and P.PlayerID not in (select PlayerID from SpecialPlayers ) -- exclude special players
	where P.PlayerStatus = 1
group by T.TitleID, T.Title_Male, T.Title_Female,Description, MaxPoints
order by MaxPoints desc