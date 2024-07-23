IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uUpdatePlayerCoins')
	BEGIN
		DROP  Procedure  uUpdatePlayerCoins
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO
CREATE Procedure dbo.uUpdatePlayerCoins
	@PlayerID int
AS
	declare @villageID int
	declare @NumVillages int
	declare @i int
	set @villageID = null;
	

	
	select VillageId, ROW_NUMBER() OVER (order BY villageid) as row into #tmp from Villages where ownerPlayerId = @playerID
	select @NumVillages = count(*) from #tmp
		
	set @i = 1
	
	WHILE @i <= @NumVillages BEGIN
		select @villageID = villageid from #tmp where row = @i
		exec UpdateVillageCoins @villageID
		set @i = @i + 1 
		print @i			
	END
		
	
	GO
