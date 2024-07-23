IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uTriggerUnitMovementCacheUpdateBasedOnVID')
	BEGIN
		DROP  Procedure  uTriggerUnitMovementCacheUpdateBasedOnVID
	END

GO

CREATE Procedure uTriggerUnitMovementCacheUpdateBasedOnVID
	(
		@VID int
	)
AS

--
-- get the unitmovement events that involve this village
--
select e.eventid into #events from UnitMovements UM
	JOIN Events E
		on E.EventID = UM.EventID
where (OriginVillageID = @VID or DestinationVillageID = @VID)
	and E.Status = 0

--
-- now get the list of players that are involved in unitmovements for which the village has changed
--
select distinct ownerplayerid into #P from villages where villageid in (select originVillageID from UnitMovements where EventID in (select eventid from #events))
insert into #p select distinct ownerplayerid from villages where villageid in (select destinationVillageID from UnitMovements where EventID in (select eventid from #events))

--
-- now update the cache for these players. 
--	 this is a bit of a hammer approach since we update both incoming and outgoing cache for all players, but htis is easier then trying to figure things out in more detail
--

UPDATE PlayerCacheTimeStamps SET TimeStamp = getdate() where PlayerID in (select distinct ownerplayerid from #p) and CachedItemID = 2
INSERT INTO PlayerCacheTimeStamps select ownerplayerid, 2, getdate() from #p where not exists (select * from PlayerCacheTimeStamps where PlayerCacheTimeStamps.PlayerID = #p.OwnerPlayerID and CachedItemID =2)
  
UPDATE PlayerCacheTimeStamps SET TimeStamp = getdate() where PlayerID in (select distinct ownerplayerid from #p) and CachedItemID = 1
INSERT INTO PlayerCacheTimeStamps select ownerplayerid, 1, getdate() from #p where not exists (select * from PlayerCacheTimeStamps where PlayerCacheTimeStamps.PlayerID = #p.OwnerPlayerID and CachedItemID =1)


