IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qGlobalStats')
	BEGIN
		DROP  Procedure  qGlobalStats
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go


CREATE Proc [dbo].[qGlobalStats]
as
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	declare @NoOfPlayers as int
	declare @NoOfVillages as int
	declare @NoOfClans as int
	declare @NoOfSilver as bigint
	declare @SilverProduction as float

	declare @CoinMineID int 
	set @CoinMineID = 5

	declare @LevelProp_CoinMineProduction int 
	set @LevelProp_CoinMineProduction = 2

	select VU.UnitTypeID, sum(TotalCount) 'Total Count', sum(CurrentCount) 'Current Count' from VillageUnits VU
		join unittypes UT on UT.unittypeid=VU.unittypeid
		where VU.VillageID not in (select villageid from villages where ownerplayerid in (select playerid from specialplayers))
		group by VU.unittypeid,UT.sort
		order by UT.sort

	select @NoOfPlayers=count(*) from players where playerid in (select ownerplayerid from villages) and playerid not in (select playerid from specialplayers)
	select @NoOfVillages=count(*),@NoOfSilver=sum(cast(coins as bigint)) from villages where ownerplayerid not in (select playerid from specialplayers)
	select @NoOfClans=count(*) from clans

	select @SilverProduction=isnull(sum(cast(PropertyValue as float)),0) from Buildings B
		join LevelProperties LP on LP.BuildingTypeID = @CoinMineID and LP.Level = B.Level
		where B.BuildingTypeID = @CoinMineID and PropertyID = @LevelProp_CoinMineProduction
		and B.VillageID not in (select villageid from villages where ownerplayerid in (select playerid from specialplayers))

	select 'Players'=@NoOfPlayers,'Villages'=@NoOfVillages,'Silver'=@NoOfSilver,'Clans'=@NoOfClans, 'Total Silver Production Per Hour'=@SilverProduction
	 