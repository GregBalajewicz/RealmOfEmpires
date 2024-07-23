 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iPromotedVillage')
	BEGIN
		DROP  Procedure  iPromotedVillage
	END

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].iPromotedVillage
		@villageid int,
		@playerid int
AS

declare @x int
declare @y int
declare @numAbsorbed int
declare @numToAbsorb int -- constant value - tells you the # of villages to absorb

select @x = xcord, @y=ycord from villages where villageid = @villageid

select @numToAbsorb = AttribValue from RealmAttributes where AttribID = 41

BEGIN TRAN 

	insert into Villages_promoted (PromotedvillageID ) 
		(select villageid from villages where villageid = @VillageID and ownerplayerid = @playerid
			and not exists (select * from villages_promoted where promotedvillageid = @villageid)
			and not exists (select * from villages_absorbed where absorbedvillageid = @villageid))

	IF @@rowcount > 0 BEGIN

		insert into villages_absorbed (AbsorbedVillageID, PromotedVillageID)
			select top (@numToAbsorb) villageid, @VillageID from villages V 
				left join villages_promoted VP on promotedvillageid = V.villageid
				left join villages_absorbed VA on absorbedvillageid = V.villageid
			where 
				ownerplayerid = @playerid  
				and VP.promotedvillageid is null 
				and VA.absorbedVillageid is null
				and ( villagetypeid = 0 OR villagetypeid between 8 and 12) -- we only absorb regular villages and legendary bonus villages 
			order by power(abs(@x - xcord),2) + power(abs(@y - ycord),2)

		set @numAbsorbed = @@rowcount 
		--
		-- if less then @numToAbsorb village were absorbed, then we either are finished, OR we dont have any more non-bonus villages to take. 
		--	so here we try to absorb the reminder of villages
		--
		IF @numAbsorbed < @numToAbsorb BEGIN
			insert into villages_absorbed (AbsorbedVillageID, PromotedVillageID)
				select top (@numToAbsorb - @numAbsorbed) villageid, @VillageID from villages V 
					left join villages_promoted VP on promotedvillageid = V.villageid
					left join villages_absorbed VA on absorbedvillageid = V.villageid
				where 
					ownerplayerid = @playerid  
					and VP.promotedvillageid is null 
					and VA.absorbedVillageid is null
				order by power(abs(@x - xcord),2) + power(abs(@y - ycord),2)
		END 
	END
COMMIT
