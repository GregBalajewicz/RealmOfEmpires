if exists (select * from sysobjects where type = 'P' and name = 'qGetCordinatesForNewVillage_Generate')
begin
	drop procedure qGetCordinatesForNewVillage_Generate
end
go


/*
This is a support SP that generates a new/next village cordinates for 
    Village Creation algorithm version 2 & 3. 
    
It is called by qGetCordinatesForNewVillage in 2 places therefore the code got its own SP

This will return the X & Y cords of an empty space for a new village. It will also check that the 
village does not exist in the new village Q
*/
create procedure  qGetCordinatesForNewVillage_Generate
	@x int output
	,@y int output
	, @PrintDebugInfo BIT = null
as

    DECLARE @DEBUG INT 
    IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
	    SET @DEBUG = 1
	    SET NOCOUNT OFF
    END ELSE BEGIN
	    SET @DEBUG = 0
	    SET NOCOUNT ON
    END 
	--
	-- algorithm version 2 & 3
	--
	declare @Density real
	declare @DistanceVer real
	declare @ActualDistance real
	declare @Angle real
	declare @CurVillageCount int
	declare @GoEmptySpace bit
	declare @CordTryCount int
	declare @NumOfCordsInCordsTable int
	declare @AlgV4_MaxBorderingVillages int
	declare @AlgV4_ChanceForVillage real 
	select @Density  = cast(AttribValue as real) from RealmAttributes where AttribID = 1
	select @DistanceVer  = cast(AttribValue as real) from RealmAttributes where AttribID = 2
	select @AlgV4_MaxBorderingVillages  = cast(AttribValue as int) from RealmAttributes where AttribID = 1
	select @AlgV4_ChanceForVillage  = cast(AttribValue as real) from RealmAttributes where AttribID = 2

	declare @algorithmVersion varchar(1)
	select @algorithmVersion  = AttribValue from RealmAttributes where AttribID = 3

	SET @CordTryCount = 0 


	IF @algorithmVersion = 4 BEGIN 

		--
		--
		-- NEW ALGORITHM
		--
		--
		--

		--
		-- Fill in more cords if we are getting to the end 
		--
		select @NumOfCordsInCordsTable = count(*) from AvailableVillageCords 
		IF @NumOfCordsInCordsTable <  100 BEGIN 
			declare @xc int 
			declare @yc int
			declare @x_max int 
			declare @y_max int
			declare @currentStep int


			set @x_max = (select top 1 x from AvailableVillageCords order by x desc) 
			set @y_max = (select top 1 y from AvailableVillageCords order by y desc) 
			
			set @xc = @x_max + 5 
			set @yc = @y_max + 5 

			set @currentStep = @xc
			IF @DEBUG = 1 select  'adding more cords to the cords table' msg, @NumOfCordsInCordsTable NumOfCordsInCordsTable, @xc xc, @yc yc, @currentStep currentStep, @x_max x_max, @y_max y_max
			while (1=1) BEGIN   
				if not (@xc = 0 and @yc=0) BEGIN
					if not exists (select * from AvailableVillageCords where x = @xc and y = @yc) BEGIN 
						insert into AvailableVillageCords values (@xc, @yc, abs(@xc) + abs(@yc)) 
					END 
				END
				if @xc = -@x_max AND @yc = -@y_max BEGIN 
		
					break;
				END ELSE IF @yc = -@currentStep BEGIN 
					SET @xc = @xc - 1
					SET @yc = @currentStep
				END ELSE BEGIN 
					SET @yc = @yc -1
				END
			END
		END 
		
		--
		-- Try to find an empty space
		--
		SET @GoEmptySpace = 0
		while(@GoEmptySpace =0)
		begin			
				/*
				 this is a self improving apglrithm. 
				 - it takes the first cord to the center. 
					- if that happens to be taken, which it normallyu will not be, then it deletes this cords and tries the next one
				 - if list of cords runs low, then the algorithm adds more 
				*/

				SET @CordTryCount = @CordTryCount + 1
					
				--
				-- get the next closed cord to center
				--								
				select top 1 @X=x, @y=y from AvailableVillageCords C order by xplusy asc 

				--
				-- check if this spot it taken. this is a performance enhancing check 
				--  but not an autoritative one since after this finishes, some other village could have 
				--  taken the stop of this one hence check again before you do anything with this village
				--
				IF not exists(select * from villages where XCord=@x and YCord=@y) 
					and ( 
						not exists(select * from landmarks L where L.XCord=@X and L.YCord=@Y )	
						OR exists(select * from landmarks L JOIN LandmarkTypeParts LTP on L.LandmarkTypePartID = LTP.LandmarkTypePartID where L.XCord=@X and L.YCord=@Y and AllowVillage =1  )	
						) 

					

					AND NOT EXISTS ( 
						select * from 
						( 
							select xcord, ycord from NewVillageQ
							union
							select @x, @y
						)  as NVQ2
							where SQRT(power(abs(XCord - @x),2) + power( abs(YCord - @Y),2) ) < 2
							having count(*) > @AlgV4_MaxBorderingVillages
					)


					and not exists(select * from NewVillageQ where XCord=@X and YCord=@Y)
					and rand() <= @AlgV4_ChanceForVillage	
				BEGIN
					SET @GoEmptySpace = 1
					IF @DEBUG = 1 select 'Empty spot!' i, @x x, @y y , @CordTryCount CordTryCount

				END ELSE BEGIN					
					SET @GoEmptySpace = 0

					-- spot is not empty, so remove it from empty cords table
					delete AvailableVillageCords where x = @x and y = @y 
					IF @DEBUG = 1 select 'taken spot' i, @x x, @y y , @CordTryCount CordTryCount
				END

			
		END
	END ELSE BEGIN 

		--
		--
		-- OLD ALGORITHM, algorithm version less than 4 
		--
		--
		--


		SET @GoEmptySpace = 0
		while(@GoEmptySpace =0)
		begin
			
			SET @CordTryCount = @CordTryCount + 1
		
			SET @Angle = cast(360 as real)*rand()
			SELECT @CurVillageCount = Count(*) + 1 from Villages -- doing + 1 since this would sometimes get stuck not able to find an empty spot when # of villages is 0, so this crude change lessens the chance of it getting stuck
			SELECT @CurVillageCount = @CurVillageCount + count(*) from NewVillageQ where DateTaken is null
	
							--	 (SQRT(A2/$I$1))+($M$1*RAND()-$M$1/2)
			SET @ActualDistance = sqrt(cast(@CurVillageCount as real) / @Density) + @DistanceVer * Rand() - @DistanceVer/2
			SET @y = CEILING(@ActualDistance * COS(RADIANS(@Angle)))
			SET @x = CEILING(@ActualDistance * SIN(RADIANS(@Angle)))
			IF @DEBUG = 1 select @x x, @y y, @ActualDistance Disc, @Angle angle, @CurVillageCount VilCnt, @Density dens, @DistanceVer disver
			--
			-- check if this spot it taken. this is a performance enhancing check 
			--  but not an autoritative one since after this finishes, some other village could have 
			--  taken the stop of this one hence check again before you do anything with this village
			--
			IF not exists(select * from villages where XCord=@x and YCord=@y) 
				and not exists(select * from landmarks L where L.XCord=@X and L.YCord=@Y)	
				and not exists(select * from NewVillageQ where XCord=@X and YCord=@Y)	BEGIN
				SET @GoEmptySpace = 1
			END ELSE BEGIN
				SET @GoEmptySpace = 0
			END
			
		END




	END