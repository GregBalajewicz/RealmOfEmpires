IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerRankingThroneRoom')
	BEGIN
		DROP  Procedure  qPlayerRankingThroneRoom
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE proc [dbo].qPlayerRankingThroneRoom
as
	select PlayerStats_BestOfLifeStats.*  , 
		( CASE Sex
				WHEN 0 THEN Title_Female
				WHEN 1 THEN Title_Male
				ELSE Title_Male
				END ) as TitleName
	
	from PlayerStats_BestOfLifeStats 
	join Titles on TitleID = TopTitleID
	
	order by HighestVillagePoints desc




