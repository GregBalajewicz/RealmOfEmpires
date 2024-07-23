 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iLandMark')
	BEGIN
		DROP  Procedure  iLandMark
	END

GO

CREATE procedure [dbo].[iLandMark]
	@XCord as int
	,@YCord as int
	,@IncreaseChanceOfLandmarksBeingPlace as bit = null
	,@MinDistanceBetweenLandmarks as int = null -- if specified, then we will not allow any landmark to be repeated within that distance in squares 
	, @PrintDebugInfo BIT = null

as

	declare @COUNT as int
	declare @OBJECTID as int
	declare @X as int
	declare @Y as int
	declare @RCount as int
	declare @Chance as int
	declare @RANDOM as int
	declare @LandmarkPlaced as bit
	declare @ROWCOUNT as int

	DECLARE @DEBUG INT, @DBG varchar(50)
	IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
		SET @DEBUG = 1
		set @DBG = ':::iLandMark:'
		SET NOCOUNT ON
	END ELSE BEGIN
		SET @DEBUG = 0
		SET NOCOUNT ON
	END 

	set @LandmarkPlaced= 0

	declare @i int
	declare @MaxTries int

	

	set @MaxTries = 3
	set @i=1
	while(@i<=@MaxTries) begin
		set @COUNT=0
		while(@COUNT=0)
		begin
			--getting counting of records in ObjectTypes and multiplying with RAND() to pick ObjectID dynamically.
			set @COUNT=(select cast(rand()*count(*) as int)+1 from #LandmarksTypesNormalized)
		end
		select @OBJECTID = LandMarkTypeID,@Chance=isnull(ChanceCurrent, Chance) from LandMarkTypes where LandMarkTypeID= (select LandmarkTypeID from #LandmarksTypesNormalized where RowID = @COUNT)
		set @RANDOM = (select cast(rand()*100 as int))
		if(@RANDOM <= @Chance) begin
			--------------------------------------------------
			set @X=@XCord
			set @Y=@YCord		
		
			--
			-- if within the realm, try to add the landmark if it is abstructed by anything on the map
			--
			insert into LandMarks(XCord,YCord,LandMarkTypePartID)
				select  xcord+@x, ycord+@y, LandMarkTypePartID  from LandmarkTypeParts LT
					where LandmarkTypeID = @OBJECTID
						and not exists (
							select * from villages V join LandmarkTypeParts L
								on V.XCord = L.xcord+@x and V.YCord = L.ycord+@y 
								where L.LandmarkTypeID = @OBJECTID
						)
						and not exists (
							select * from NewVillageQ Q join LandmarkTypeParts L
								on Q.XCord = L.xcord+@x and Q.YCord = L.ycord+@y 
								where L.LandmarkTypeID = @OBJECTID
						)
						and not exists (
							select * from Landmarks LM join LandmarkTypeParts L
								on LM.XCord = L.xcord+@x and LM.YCord = L.ycord+@y 
								where L.LandmarkTypeID = @OBJECTID
						)
						-- and not close to another version of it self
						and not exists (
							select * from LandmarkTypeParts LTP
								where LTP.LandmarkTypeID = @OBJECTID
								and exists (
									select * from Landmarks LM
										where LM.LandmarkTypePartID in (select LandmarkTypePartID from LandmarkTypeParts where LandmarkTypeID = @OBJECTID)
										AND SQRT(power(abs(LM.XCord - (LTP.xcord+@x)),2) + power( abs(LM.YCord - (LTP.ycord+@y)),2) ) < @MinDistanceBetweenLandmarks 
								)							 							
						)

			SET @ROWCOUNT = @@ROWCOUNT;
			IF @ROWCOUNT > 0 BEGIN
				set @LandmarkPlaced = 1 
				SET @i = @MaxTries
			END		

			IF @DEBUG = 1 print @DBG + '@x :'+ CAST(@x AS varchar(max)) + ', @Y :'+ CAST(@y AS varchar(max)) +', @OBJECTID:' + CAST(@OBJECTID AS varchar(max))+ ', @ROWCOUNT :'+ CAST(@ROWCOUNT AS varchar(max))+ ', @LandmarkPlaced :'+ CAST(@LandmarkPlaced AS varchar(max)) 
		END
		SET @i = @i + 1
	END


	IF @LandmarkPlaced =1  BEGIN
		--print 'landmarkID ' + cast(@OBJECTID as varchar(max)) + 'PLACED!' + cast(@X as varchar(max)) + ',' + cast(@Y as varchar(max))
		--
		-- reset its chance of being set next time
		--
		update LandMarkTypes set ChanceCurrent = null where LandMarkTypeID= @OBJECTID
	END ELSE BEGIN
		--print 'landmarkID ' + cast(@OBJECTID as varchar(max)) + 'no placed'
		if @IncreaseChanceOfLandmarksBeingPlace = 1 BEGIN
			--
			-- increase the chance of it being placed next time
			--
			update LandMarkTypes set ChanceCurrent = isnull(ChanceCurrent, Chance) + 10  where LandMarkTypeID= @OBJECTID and (isnull(ChanceCurrent, Chance) + 10 <= 100)
		END
	END

