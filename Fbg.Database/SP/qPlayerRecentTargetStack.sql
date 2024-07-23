IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerRecentTargetStack')
	BEGIN
		DROP  Procedure  qPlayerRecentTargetStack
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE proc [dbo].qPlayerRecentTargetStack
    @PlayerID int
as

    SELECT TOP 10 P.Name as OwnerPlayerName
        , V.Name as VillageName
        , V.Xcord
        , V.YCord
		, SP.Type -- may be null
    FROM  PlayerRecentTargetStack S
    JOIN Villages V
        on S.TargetVillageID = V.VillageID
    JOIN Players P
        on V.OwnerPlayerID = P.PlayerID
    left join SpecialPlayers SP
        on SP.PlayerID = P.PlayerID
    WHERE S.PlayerID = @PlayerID
    Order by TargetLastUpdate DESC

