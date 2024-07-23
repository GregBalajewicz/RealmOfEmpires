if exists (select * from sysobjects where type = 'P' and name = 'qGetCordinatesForNewVillage')
begin
	drop procedure qGetCordinatesForNewVillage
end
go

/*

@CloseToX, @CloseToY and @InQuadrant are ONLY RELEVANT for Algorithm Version 3

IF new village placement algotihm version = 3 
    IF @CloseToX is not null THEN algorithm finds the  village in the 
        new village Q that is closes to (@CloseToX ,@CloseToY)
    IF @CloseToX is null AND @InQuadrant is not null THEN algorithm 
        find the village in the new village Q that is in this quadrant. 
        
        @InQuadrant = 1 -> North East
        @InQuadrant = 2 -> South East
        @InQuadrant = 3 -> South West
        @InQuadrant = 4 -> North West
        

*/
create procedure  qGetCordinatesForNewVillage
	@x int output
	,@y int output
	, @CloseToX int = null
	, @CloseToY int = null
	, @InQuadrant smallint = null 
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

declare @quadrantAngle int -- used if placing new village in a quadrant
declare @CenterOfQuadrantX int
declare @CenterOfQuadrantY int

declare @algorithmVersion varchar(1)
select @algorithmVersion  = AttribValue from RealmAttributes where AttribID = 3

IF @algorithmVersion is NULL OR @algorithmVersion  = 1 BEGIN
	--
	-- **************************************************
	-- algorithm version 2 
	-- **************************************************
	--
	Declare @RANDOM as integer

	set @RANDOM=-1
	while(@RANDOM < 35)
	begin
		-- For 0,0
		if(@x=0 and @y=0)
		begin
			set @x=1
			set @y=0
		end
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
		set @RANDOM = (select cast(rand()*100 as int))
	end
END ELSE IF @algorithmVersion  = 2 BEGIN 
	--
	-- **************************************************
	-- algorithm version 2 
	-- **************************************************
	--
    exec  qGetCordinatesForNewVillage_Generate @x out,@y out	
END ELSE IF @algorithmVersion  = 3 BEGIN 
	--
	-- **************************************************
	-- algorithm version 3
	-- **************************************************
	--
	SET @X = null
	WHILE @X is null BEGIN
	    IF @CloseToX IS NOT NULL BEGIN
	        --
	        -- find village in the Q that is closest to specified cords
	        --
	        SELECT top 1 @X = XCord, @Y = YCord 
              FROM NewVillageQ 
              WHERE DateTaken is null
              order by power((@CloseToX - XCord),2) + power((@CloseToY - YCord),2) asc      
	    END ELSE IF @InQuadrant IS NOT NULL BEGIN
	        --
	        -- find village in a quadrant
	        --
	        IF @InQuadrant = 1 BEGIN 
	            -- North East
	            select top 1 @X = XCord, @Y = YCord 
	                FROM NewVillageQ 
	                WHERE DateTaken is null
	                    and XCord>=0 and YCord>0
	                order by NewVillageID asc 
	            SET @quadrantAngle = 45       
	        END ELSE IF @InQuadrant = 2 BEGIN 
	            -- South East
	            select top 1 @X = XCord, @Y = YCord 
	                FROM NewVillageQ 
	                WHERE DateTaken is null
	                    and XCord>0 and YCord<=0
	                order by NewVillageID asc 		        
	            SET @quadrantAngle = 135	        	                		        
	        END ELSE IF @InQuadrant = 3 BEGIN 
	            --South West
	            select top 1 @X = XCord, @Y = YCord 
	                FROM NewVillageQ 
	                WHERE DateTaken is null
	                    and XCord<=0 and YCord<0
	                order by NewVillageID asc 		        
	            SET @quadrantAngle = 225	        	                		        
	        END ELSE BEGIN 
	            -- (@InQuadrant = 4 -> North West
	            select top 1 @X = XCord, @Y = YCord 
	                FROM NewVillageQ 
	                WHERE DateTaken is null
	                    and XCord<0 and YCord>=0
	                order by NewVillageID asc 
	            SET @quadrantAngle = 315	        	                		        
	        END 
	        --
	        -- if we did NOT GET a village, that means no village in this quadrant 
	        --  exist in the Q therefore we find a village closest to the
	        --  center of this quadrant. 
	        --
	        IF @X is null BEGIN 
	            --
	            -- get a point that would represent center of the quadrant on the rim.
	            --  the code here is very simillar to qGetCordinatesForNewVillage_Generate except that we dont care if this point is
	            --  on the landmark or existing village since it is just the center of the quadrant. 
	            --
	            declare @Density real
	            declare @DistanceVer real
	            declare @ActualDistance real
	            declare @CurVillageCount int
            	
	            select @Density  = cast(AttribValue as real) from RealmAttributes where AttribID = 1
	            select @DistanceVer  = cast(AttribValue as real) from RealmAttributes where AttribID = 2            	            		
	            SELECT @CurVillageCount = Count(*) from Villages 
	            SET @ActualDistance = sqrt(cast(@CurVillageCount as real) / @Density) + @DistanceVer * Rand() - @DistanceVer/2
	            SET @ActualDistance = @ActualDistance /2 -- we dont want a point right on the rim so that the algorithm prefers new village closest to the rim
	            SET @CenterOfQuadrantY = CEILING(@ActualDistance * COS(RADIANS(@quadrantAngle)))
	            SET @CenterOfQuadrantX = CEILING(@ActualDistance * SIN(RADIANS(@quadrantAngle)))
                --
                -- Now that we know the center of the quadarnt, simply take village from the Q that is closest to this cord
                --            	
	            SELECT top 1 @X = XCord, @Y = YCord 
                  FROM NewVillageQ 
                  WHERE DateTaken is null
                  order by power((@CenterOfQuadrantX - XCord),2) + power((@CenterOfQuadrantY - YCord),2) asc      	            
	        END 		        
	    END ELSE BEGIN 
	        --
	        -- Pick the first village from the Q
	        --
	        select top 1 @X = XCord, @Y = YCord 
	            FROM NewVillageQ 
	            WHERE DateTaken is null
	            order by NewVillageID asc 		        
	    END 
	    
	    --
	    -- reserve the village by updating this row saying its taken
	    --  if no rows updated then we happen to have lost the village and must try again.
	    --
	    UPDATE NewVillageQ SET DateTaken = getdate() 
	        where XCord = @X 
	            AND YCord = @Y
	            AND DateTaken is null
	    IF @@rowcount <> 1 BEGIN 
	        SET @X = null  
	    END 
	END 
	
END ELSE IF @algorithmVersion  = 4 BEGIN 
	--
	-- **************************************************
	-- algorithm version 4
	-- **************************************************
	--
	SET @X = null
	WHILE @X is null BEGIN
	    IF @CloseToX IS NOT NULL BEGIN
	        --
	        -- find village in the Q that is closest to specified cords
	        --
	        SELECT top 1 @X = XCord, @Y = YCord 
              FROM NewVillageQ 
              WHERE DateTaken is null
              order by power((@CloseToX - XCord),2) + power((@CloseToY - YCord),2) asc      
	    END ELSE IF @InQuadrant IS NOT NULL BEGIN
	        --
	        -- find village in a quadrant
	        --
	        IF @InQuadrant = 1 BEGIN 
	            -- North East
	            select top 1 @X = XCord, @Y = YCord 
	                FROM NewVillageQ 
	                WHERE DateTaken is null
	                    and XCord>=0 and YCord>0
	                order by NewVillageID asc 
	            SET @quadrantAngle = 45       
	        END ELSE IF @InQuadrant = 2 BEGIN 
	            -- South East
	            select top 1 @X = XCord, @Y = YCord 
	                FROM NewVillageQ 
	                WHERE DateTaken is null
	                    and XCord>0 and YCord<=0
	                order by NewVillageID asc 		        
	            SET @quadrantAngle = 135	        	                		        
	        END ELSE IF @InQuadrant = 3 BEGIN 
	            --South West
	            select top 1 @X = XCord, @Y = YCord 
	                FROM NewVillageQ 
	                WHERE DateTaken is null
	                    and XCord<=0 and YCord<0
	                order by NewVillageID asc 		        
	            SET @quadrantAngle = 225	        	                		        
	        END ELSE BEGIN 
	            -- (@InQuadrant = 4 -> North West
	            select top 1 @X = XCord, @Y = YCord 
	                FROM NewVillageQ 
	                WHERE DateTaken is null
	                    and XCord<0 and YCord>=0
	                order by NewVillageID asc 
	            SET @quadrantAngle = 315	        	                		        
	        END 
	        
	    END 
		
		--
		-- if we are not starting close to someone, or in some quadrant, then pick just any village from the Q 
		--
		IF @X is null BEGIN 
	        --
	        -- Pick the first village from the Q
	        --
	        select top 1 @X = XCord, @Y = YCord 
	            FROM NewVillageQ 
	            WHERE DateTaken is null
	            order by NewVillageID asc 		        
	    END 
	    
	    --
	    -- reserve the village by updating this row saying its taken
	    --  if no rows updated then we happen to have lost the village and must try again.
	    --
	    UPDATE NewVillageQ SET DateTaken = getdate() 
	        where XCord = @X 
	            AND YCord = @Y
	            AND DateTaken is null
	    IF @@rowcount <> 1 BEGIN 
	        SET @X = null  
	    END 
	END 
	
END 