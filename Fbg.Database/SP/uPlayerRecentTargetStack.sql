 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayerRecentTargetStack')
	BEGIN
		DROP  Procedure  uPlayerRecentTargetStack
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE proc [dbo].uPlayerRecentTargetStack
    @PlayerID int
    , @VillageID int
as

    IF exists(select * from PlayerRecentTargetStack where PlayerID = @PlayerID and TargetVillageID = @VillageID) BEGIN
        update PlayerRecentTargetStack SET TargetLastUpdate = getdate()
            where PlayerID = @PlayerID and TargetVillageID = @VillageID
    END ELSE BEGIN
        INSERT INTO PlayerRecentTargetStack(PlayerID, TargetVillageID, TargetLastUpdate)
            values (@PlayerID, @VillageID, getdate())    
            
        WHILE EXISTS (SELECT count(*) from PlayerRecentTargetStack where PlayerID = @PlayerID having count(*) >10) BEGIN
            delete PlayerRecentTargetStack where PlayerID = @PlayerID and TargetVillageID 
                = (SELECT Top 1 TargetVillageID from PlayerRecentTargetStack where PlayerID = @PlayerID Order by TargetLastUpdate ASC)
        END
    END

