 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iMapEventsCaravanCatchup')
	BEGIN
		DROP  Procedure  iMapEventsCaravanCatchup
	END

GO

--this SP spawns more camps for player
CREATE Procedure [dbo].iMapEventsCaravanCatchup	
	@playerid int,
	@howManyToSpawn int --tells sp how many new camps to spawn
AS

	--
	-- create a temp table that will help us look for empty spot
	--
	create table #calcs (x int, y int)

	declare @xc int 
	declare @yc int
	declare @currentStep int

	set @xc = 10
	set @yc = 10
	set @currentStep = 10
	while (@xc < 11) BEGIN   
		if not (@xc = 0 and @yc=0) BEGIN
			insert into #calcs values (@xc, @yc) 
		END
		if @xc = -@currentStep AND @yc = -@currentStep BEGIN 
		
			break;
		END ELSE IF @yc = -@currentStep BEGIN 
			SET @xc = @xc - 1
			SET @yc = @currentStep
		END ELSE BEGIN 
			SET @yc = @yc -1
		END
	
	END


	Declare @now datetime
	declare @today datetime

	set @now = getdate()
	set @today = cast(datename(year, @now) +'-'+ cast(month(@now)as varchar(2))+'-'+ datename( dd, @now) as datetime)
	--
	-- get the village by which to place the caravan 
	--	
	declare @Y int 
	declare @X int 
	declare @VID int
	declare @OPID int 
	declare @rand int 

	
	create table #Villages (playerid int, villageid int )
	insert into #Villages 
		select OwnerPlayerID, min(villageid) as villageID 
		from villages V 
			join Players P
				on P.playerid = V.ownerplayerid 
		where OwnerPlayerID = @playerid
		group by OwnerPlayerID

	select @VID = V.villageid, @X = XCord, @y=YCord, @OPID =OwnerPlayerID from #Villages VT join villages V on v.VillageID = Vt.villageid


	while (@howManyToSpawn >0 ) BEGIN 
	
		-- ///NOTE: TODO: need to make sure 'DATA' field from previous camp is carried over if possible
		SET @currentStep = 2
		WHILE (1=1) BEGIN 
			insert into PlayerMapEvents(PlayerID, typeid, Data, addedon, xcord, ycord, IsActive) 
			select top 1 @OPID, 2, ''  ,getdate(), @X + c.x, @y + c.y, 1 from #calcs C
			left join villages V 
				on V.XCord = @X + c.x
					and V.YCord = @Y + c.y
			--left join Landmarks L 
			--	on L.XCord = @X + c.x
			--		and L.YCord = @Y + c.y
			left join NewVillageQ Q 
				on Q.XCord = @X + c.x
					and Q.YCord = @Y + c.y
					and Q.DateTaken is null			
			where abs(C.x) <= @currentStep and abs(C.y) <= @currentStep 
				and v.VillageID is null
				--and L.LandmarkTypePartID is null
				and Q.XCord is null 
				and not exists(select * from landmarks L JOIN LandmarkTypeParts LTP on L.LandmarkTypePartID = LTP.LandmarkTypePartID where L.XCord=@X + c.x and L.YCord=@Y + c.y and AllowVillage = 0 )	
			--order by NEWID()

			-- if not inserted, then widen the field we look in for empty spot
			IF @@ROWCOUNT = 0 BEGIN 
				SET @currentStep = @currentStep + 1
			END ELSE BEGIN
				BREAK
			END
		END



		SET @howManyToSpawn = @howManyToSpawn - 1
	END

--return all active player map events
exec qPlayerMapEvents @playerid
