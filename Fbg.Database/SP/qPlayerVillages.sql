IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerVillages')
	BEGIN
		DROP  Procedure  qPlayerVillages
	END

GO
CREATE Procedure qPlayerVillages
	@PlayerID as int
	,@DetailedList as bit
	,@FilterID as int -- may pass in null
AS

IF @DetailedList = 1 BEGIN
	select v.VillageID
		, v.Name
		, v.Coins
		, V.points as villagepoints
		, XCord
		, YCord 
		, cast(loyalty + floor(cast(datediff(minute, LoyaltyLastUpdated, getdate()) as real) /(60.0 / (select cast(AttribValue as real) FROM RealmAttributes where attribid =8))) as integer) 
		, b_CoinMine.Level as CoinMineLevel
		, b_Treasury.Level as TreasuryLevel
		, V.CoinsLastUpdates
		, VillageTypeID
			from villages v 
			join buildings b_CoinMine on V.villageId = b_CoinMine.villageID 
				and b_CoinMine.BuildingTypeID = 5
			join buildings b_Treasury on V.villageId = b_Treasury.villageID 	
				and b_Treasury.BuildingTypeID = 6
		where ownerPlayerID = @PlayerID
			 and (
					@FilterID is null 
					OR V.VillageID in 
						(
						    select villageid
						    from villagetags VT
						    join filterTags FT
							    on VT.TagID = FT.TagID
						    join Filters F
							    on F.FilterID = FT.FilterID
						    where F.FilterID = @FilterID
						    group by villageid
						    having count(*) = (
							    select count(*) from 
								    filterTags FT
								    join Filters F
									    on F.FilterID = FT.FilterID
								    where F.FilterID = @FilterID
								)
						)
				) 
		order by name
END ELSE BEGIN 
	select v.VillageID
		, VillageTypeID
		from villages v 		
		where ownerPlayerID = @PlayerID
			 and (
					@FilterID is null 
					OR V.VillageID in 
						(							
						    select villageid
						    from villagetags VT
						    join filterTags FT
							    on VT.TagID = FT.TagID
						    join Filters F
							    on F.FilterID = FT.FilterID
						    where F.FilterID = @FilterID
						    group by villageid
						    having count(*) = (
							    select count(*) from 
								    filterTags FT
								    join Filters F
									    on F.FilterID = FT.FilterID
								    where F.FilterID = @FilterID
							    )
						)
				) 
		
		order by name
END


GO

