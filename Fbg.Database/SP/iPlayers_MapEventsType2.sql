IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iPlayers_MapEventsType2')
	BEGIN
		DROP  Procedure  iPlayers_MapEventsType2
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--
-- activate a map event
--
CREATE Procedure iPlayers_MapEventsType2
	
AS
	
begin try 
	set nocount on
	
	--
	-- delete OLD caravan events for players who were active after the event was created.
	--	why? in case hte player does not see the caravan, in case they restarted etc
	--
	delete from PlayerMapEvents where EventID in (
		select eventid from PlayerMapEvents E
		join players P  on e.PlayerID = p.PlayerID
		where  TYPEid = 2 and IsActive = 1 and  AddedOn <  DATEADD(day, -1, getdate()) and dateadd(hour,-1, LastActivity) > AddedOn
	)

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

	--select * from #calcs

	Declare @now datetime
	declare @today datetime

	set @now = getdate()
	set @today = cast(datename(year, @now) +'-'+ cast(month(@now)as varchar(2))+'-'+ datename( dd, @now) as datetime)

	declare @MaxCaravansToday int
	declare @CaravansPerDay int 
	select @CaravansPerDay = AttribValue from RealmAttributes where attribid = 92 -- Nubmer of caravans per day'
	SET @CaravansPerDay = isnull(@CaravansPerDay , 30)

	select @MaxCaravansToday = datediff(day,cast(openon as date), cast(getdate() as date))+1 from realm
	set @MaxCaravansToday = @MaxCaravansToday * @CaravansPerDay
	--
	-- get players that should get a rebel camp / caravan
	--
	--delete PlayerMapEvents
	--drop table #Villages 
	create table #Villages (playerid int, villageid int )
	insert into #Villages 
		select OwnerPlayerID, min(villageid) as villageID 
		from villages V 
			join Players P
				on P.playerid = V.ownerplayerid 
		where OwnerPlayerID not in (select playerid from SpecialPlayers)
			and not exists (select * From PlayerMapEvents PE where PE.TypeID = 2 and pe.PlayerID = OwnerPlayerID and IsActive = 1)
			and not exists (select * from playerMapEventsStates PES where PES.TypeID = 2 and PES.PlayerID = OwnerPlayerID and StateData2 >= @MaxCaravansToday)
			and P.RegisteredOn < dateadd(minute, -10, getdate()) -- 10 min from when person stared playing
	group by OwnerPlayerID



	declare @Y int 
	declare @X int 
	declare @VID int
	declare @OPID int 
	declare @rand int 
	select @VID = V.villageid, @X = XCord, @y=YCord, @OPID =OwnerPlayerID from #Villages VT join villages V on v.VillageID = Vt.villageid
	while (@vid is not null) BEGIN 
	
		-- ///NOTE: TODO: need to make sure 'DATA' field from previous camp is carried over if possible
		SET @currentStep = 2
		WHILE (1=1) BEGIN 
			insert into PlayerMapEvents(PlayerID, typeid, Data, addedon, xcord, ycord, IsActive) 
			select top 1 @OPID, 2, 'L1',getdate(), @X + c.x, @y + c.y, 1 from #calcs C
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
			order by NEWID()

			-- if not inserted, then widen the field we look in for empty spot
			IF @@ROWCOUNT = 0 BEGIN 
				SET @currentStep = @currentStep + 1
			END ELSE BEGIN
				BREAK
			END
		END



		delete #villages where villageid = @vid 
		set @vid = null 
		select @VID = V.villageid, @X = XCord, @y=YCord, @OPID =OwnerPlayerID from #Villages VT join villages V on v.VillageID = Vt.villageid
	END
    
end try

begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'iPlayers_MapEventsType2 FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)

		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

