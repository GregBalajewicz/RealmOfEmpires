IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iLandMarks')
	BEGIN
		DROP  Procedure  iLandMarks
	END

GO

CREATE proc [dbo].[iLandMarks]
	@SizeX as int = null -- max size of realm X wise
	,@SizeY as int = null-- max size of realm Y wise
	,@IncreaseChanceOfLandmarksBeingPlace as bit = null -- set true if re-populating landmarks on a realm with villages, hence we want more chances that a landmark is going to be placed
	,@doOnlyLandmarksWithThisManyPartsOrMore as int = null -- allows you to limit the landmars used to only those that have at least this number of parts
	,@MinDistanceBetweenLandmarks as int = null -- if specified, then we will not allow any landmark to be repeated within that distance in squares 
	,@DoOnlyLandmarksLikeThis as varchar(max) = null -- if specified, then we grab landmarks where  "NAME LIKE @DoOnlyLandmarksLikeThis" 
	, @PrintDebugInfo BIT = null

as
	Declare @Size as int
	Declare @X1 as int
	Declare @X2 as int
	Declare @Y1 as int
	Declare @Y2 as int
	Declare @X as int
	Declare @Y as int

	
	DECLARE @DEBUG INT, @DBG varchar(10)
	IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
		SET @DEBUG = 1
		set @DBG = ':::::::'
		SET NOCOUNT OFF
	END ELSE BEGIN
		SET @DEBUG = 0
		SET NOCOUNT ON
	END 
	IF @DEBUG = 1 SELECT 'BEGIN [iLandMarks] ' 


	-- figure out the size of the realm; if not specified in params, go with size in Realm table

	select @Size=RealmSize from Realm
	set @X1= isnull(@SizeX,@Size)
	set @X2= (0 - isnull(@SizeX,@Size))
	set @Y1= isnull(@SizeY,@Size)
	set @Y2=(0- isnull(@SizeY,@Size))

	set @X=1
	set @Y=0

	IF @DEBUG = 1 select @x,@x1,@y,@y2

	--
	-- this temp table will be used by iLandMark
	--	getting the landmarks we want to place
	create table #LandmarksTypesNormalized(RowID int identity(1,1), LandmarkTypeID int)	
	insert into #LandmarksTypesNormalized 
		select LandmarkTypeID from LandmarkTypeParts 
			where LandmarkTypeID in (select LandmarkTypeID from LandmarkTypes where name LIKE @DoOnlyLandmarksLikeThis OR @DoOnlyLandmarksLikeThis is null )
			group by LandmarkTypeID having count(*) >= isnull(@doOnlyLandmarksWithThisManyPartsOrMore, 1)

	IF @DEBUG = 1 select * from #LandmarksTypesNormalized


	while((@X<=@X1 and @X>=@Y2) and (@Y>=@Y2 and @Y<=@X1))
	begin

		-- doing this IF as perforamnce improvement. no point trying to place a landmakr when even the start x,y is taken
		if not exists(select * from Landmarks where xcord = @x and ycord = @y)
			and not exists(select * from Villages where xcord = @x and ycord = @y)
			and not exists(select * from NewVillageQ where xcord = @x and ycord = @y)
		BEGIN
			exec iLandMark @X,@Y, @IncreaseChanceOfLandmarksBeingPlace, @MinDistanceBetweenLandmarks, @PrintDebugInfo
		END
			

		-- For ++
		if(@x>=0 and @y>=0)
		begin
			if(@x>@y)
			begin
				set @y=@y+1
			end
			else if(@x<=@y)
			begin
				set @x=@x-1
			end
		end
		-- For --
		else if(@x<0 and @y<0)
		begin
			if(@x>=@y)
			begin
				set @x=@x+1
			end
			else if(@x<@y)
			begin
				set @y=@y-1
			end
		end
		-- For +-
		else if(@x>=0 and @y<0)
		begin
			if(abs(@x)<=abs(@y))
			begin
				set @x=@x+1
			end
			else if(abs(@x)>abs(@y))
			begin
				set @y=@y+1
			end
		end
		-- For -+
		else if(@x<0 and @y>=0)
		begin
			if(abs(@x)<abs(@y))
			begin
				set @x=@x-1
			end
			else if(abs(@x)>=abs(@y))
			begin
				set @y=@y-1
			end
		end
	end


	IF @DEBUG = 1 SELECT 'END [iLandMarks] ' 
