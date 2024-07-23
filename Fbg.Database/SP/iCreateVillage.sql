if exists (select * from sysobjects where type = 'P' and name = 'iCreateVillage')
begin
	drop procedure iCreateVillage
end
go

/*

@StartCloseToPlayerID and @InQuadrant are ONLY RELEVANT for Algorithm Version 3

IF new village placement algotihm version = 3 
    IF @StartCloseToPlayerID is not null THEN algorithm finds the  village in the 
        new village Q that is closes to the village closes to the rim owned by this player
    IF @CloseToX is null AND @InQuadrant is not null THEN algorithm 
        find the village in the new village Q that is in this quadrant. 
        
        @InQuadrant = 1 -> North East
        @InQuadrant = 2 -> South East
        @InQuadrant = 3 -> South West
        @InQuadrant = 4 -> North West        
*/
create Procedure iCreateVillage
	@PlayerID int
	,@PlayerName varchar(50)
	,@StartCloseToPlayerID int = null -- only used for village creation algorithm v3	
	,@InQuadrant smallint = null 	
    ,@VillageName	nvarchar(25) -- set to null or empty string if a default name is to be generated
	,@ForRebellVillage bit = 0 -- if 1, indicates this is creating rebel village
	,@AllowMultipleVillage bit = 0 -- if 1, SP will not check if player already has a village
as
	declare @IsNestedTransaction Bit

	Declare @x as integer
	Declare @y as integer
	Declare @RANDOM as integer
	Declare @CheckValue as integer
	Declare @VillageID  as integer

	Declare @ERROR_MSG as varchar(max)

	Declare @count as integer
	declare @DaysOld int 
	declare @StartLevelID int
	declare @StartCoins int
	declare @ChanceOfRebelVillage real
	declare @RebelPlayerID int
    declare @algorithmVersion varchar(1)
    
    declare @CloseToX int
	declare @CloseToY int
	
	declare @now Datetime
	set @now = getdate() 
	declare @RealmType varchar(100)

	SELECT @RealmType = attribvalue FROM RealmAttributes WHERE attribID = 2000 

	select @algorithmVersion  = AttribValue from RealmAttributes where AttribID = 3

    --
    -- sanity check - make sure this player exists and is active.  
	IF not exists (select * from players where playerid = @PlayerID and PlayerStatus = 1) BEGIN
	    RETURN;
	END 

	--
	-- ensure player has no villages, unless we are to allow multiple villages
	--
	IF @AllowMultipleVillage = 0 BEGIN
		IF exists (select * from villages where ownerplayerid = @PlayerID) BEGIN
			RETURN;
		END
	END 
	
	--
	-- create a village name if not specified. 
	--
	IF @VillageName is null OR @VillageName = '' BEGIN
	    IF  len(@PlayerName) >= 21  BEGIN
		    set @VillageName = @PlayerName
	    END ELSE IF len(@PlayerName) >= 17  BEGIN		
		    set @VillageName = @PlayerName + dbo.Translate('iCreateVillage_vill')
	    END ELSE BEGIN
		    set @VillageName = @PlayerName+ dbo.Translate('iCreateVillage_village')
	    END
	END


	--
	-- Is this SP called by some other SP, as part of a nested transaction?
	--
	IF @@TRANCOUNT > 0 BEGIN 
		set @IsNestedTransaction = 1 --Yes!
	END ELSE BEGIN
		set @IsNestedTransaction = 0 --no
	END

	__RETRY__:
	
	begin try
		Select @x=XCord,@y=YCord from Villages where VillageID=(Select isnull(max(VillageID),0) as MaxVillageID from villages)
		select @x=isnull(@x,0),@y=isnull(@y,0)
		--
		-- what is the 'starting level' of this village
		--
		select @daysold = datediff(day, OpenOn, @now)  from realm
		select top 1 @StartLevelID = StartLevelID, @StartCoins = coins
			from VillageStartLevels 
			where @DaysOld <= realmmaxAgeInDays 
			order by realmmaxAgeInDays asc		
	    --
	    -- for alg version 3 or later, we see if we are to start next to someone
	    --
		IF @algorithmVersion >= 3 BEGIN
		    IF @StartCloseToPlayerID IS NOT NULL AND @StartCloseToPlayerID <>0  BEGIN
		        SELECT TOP 1 @CloseToX = XCord, @CloseToY = YCord 
	                FROM Villages 
	                WHERE OwnerPlayerID = @StartCloseToPlayerID
                    order by power(XCord,2) + power(YCord,2) desc
		    END		
		END
		
			
		
		begin tran
		
		    --
		    -- IN THIS LOOP, we actually create the new village
		    --
			set @count=0
			while(@count=0)
			begin
				exec qGetCordinatesForNewVillage @x out,@y out, @CloseToX, @CloseToY, @InQuadrant

				Insert into Villages(OwnerPlayerID,Name, Coins, XCord,YCord)
				    select @PlayerID,@VillageName,@StartCoins, @x,@y
				    where 
				            not exists(select * from villages where XCord=@x and YCord=@y) 
							and ( 
								not exists(select * from landmarks L where L.XCord=@X and L.YCord=@Y )	
								OR exists(select * from landmarks L JOIN LandmarkTypeParts LTP on L.LandmarkTypePartID = LTP.LandmarkTypePartID where L.XCord=@X and L.YCord=@Y and AllowVillage =1  )	
								) 
				
				set @count=@@rowcount
				
				SET @VillageID = SCOPE_IDENTITY()
			end
            --
            -- do a few more things once the village is created
            --
            IF @algorithmVersion >= 3 BEGIN
	            --
	            -- note the real village id for the village taken off the Q
	            --
	            UPDATE NewVillageQ SET VillageID = @VillageID
		            where XCord = @X 
		                AND YCord = @Y		             
		        --
		        -- add one more village to the Q
		        --
		        declare @X1 int
		        declare @Y2 int
	            exec  qGetCordinatesForNewVillage_Generate @X1 out,@Y2 out, 0
	            insert into NewVillageQ (XCord, YCord) values (@X1, @Y2)
				delete AvailableVillageCords where x = @x and y = @y  -- this spot is taken, so remove it from available spots
				--
	            -- note if player started in a quadrant or not
	            --
	            IF @InQuadrant is not null and @InQuadrant > 0 BEGIN 
                    EXEC iPlayerFlag @PlayerID, 75, @now, @InQuadrant
	            END
            END

			--
			-- insert the buildings for this village
			--
			insert into buildings 
				select @VillageID, BuildingTypeID, Level 
				from VillageStartLevels_Buildings 
				where StartLevelID = @StartLevelID
			--
			-- insert any starting reearch items (could be none)
			--
			--	we join with playerresearchitems to make sure we only give player the research they dont have
			--	we join ResearchInProgress to make sure we only give player the research they dont have currently in progress. 
			--
			insert into playerresearchitems 
				select @PlayerID, SL.ResearchItemTypeID, SL.ResearchItemID
				from VillageStartLevels_ResearchItems SL 
					left join playerresearchitems PR 
						on SL.ResearchItemTypeID = PR.ResearchItemTypeID 
						and SL.ResearchItemID = PR.ResearchItemID 					
						and PR.PlayerID = @PlayerID
					left join ResearchInProgress RIIP
						on SL.ResearchItemTypeID = RIIP.ResearchItemTypeID 
						and SL.ResearchItemID = RIIP.ResearchItemID 					
						and RIIP.PlayerID = @PlayerID
				where StartLevelID = @StartLevelID
					and PR.ResearchItemID is null -- only add those that player does not alreayd have. important in case of adding a rebel village and respawn
					and RIIP.ResearchItemID is null -- only add those that player does not alreayd have in progress important in case respawn				
			--
			-- Your starting units and capital
			--
			IF @ForRebellVillage = 0 BEGIN 
				--
				-- for regular village
				--
				insert into VillageUnits (VillageID, UnitTypeID, TotalCount, CurrentCount) 
					SELECT @VillageID, UnitTypeID, Count,Count  
					FROM VillageStartLevels_Units 
					where StartLevelID = @StartLevelID
					
				INSERT INTO CapitalVillages VALUES (@VillageID)
			END ELSE BEGIN 
				--
				-- do something special for rebel villages 
				--
				declare @CMForRebels int

				

				--
				-- we change all rebels, in all realms, to have few starting troops  because we have one quest system that 
				--	tells them to farm rebels right away
				--
				SET @CMForRebels = 1 + rand()*14					
				insert into VillageUnits  (VillageID, UnitTypeID, TotalCount, CurrentCount) 
					values (@VillageID, 11, @CMForRebels, @CMForRebels) 				
				
				--
				-- handle bonus villages if active on this realm
				--	
				exec uBonusVillageInit @VillageID
			END 

				
			--
			-- enter empty records for missing units; improves performance. 
			--
			insert into VillageUnits (VillageID, UnitTypeID, TotalCount, CurrentCount) 
				SELECT @VillageID, UnitTypeID, 0,0 
				FROM UnitTypes 
				where UnitTypeID not in (select unittypeid from VillageUnits where villageid = @VillageID)

			
			insert into VillageSemaphore (villageID, timestamp) values (@VillageID, @now)


			
			--
			---- In case this player is 'restarting' make sure he has no chests. 
			----
			--update players set chests = 0 where playerID = @PlayerID


			--
			-- village ownership record history. 
			--
			INSERT INTO VillageOwnershipHistory( VillageID, PreviousOwnerPlayerID, CurrentOwnerPlayerID, date) 
				VALUES (@VillageID,-1,@PlayerID, @now)	
			--
			-- update the village's and player's points
			--
			EXEC uPoints_Village @VillageID
			EXEC uPoints_Player @PlayerID
			
		commit tran
		
		--
		-- create a rebel village if needed. 
		--
		DECLARE @RebelName AS varchar(10)
		SET @RebelName = dbo.Translate('iCreateVillage_rebel') --Needed to create a string variable and set it like this because inserting dbo.Translate(...) directly into EXEC caused an error
		IF @ForRebellVillage = 0 BEGIN 
			SELECT @ChanceOfRebelVillage = RebelVillageCreationChance FROM Realm

            IF @ChanceOfRebelVillage <= 1 BEGIN
                --
                -- for values 0-1, we consider this a % chance. 
                --
			    IF rand() < @ChanceOfRebelVillage BEGIN
				    select @RebelPlayerID = PlayerID FROM SpecialPlayers where Type = -2
    			
				    EXEC iCreateVillage @RebelPlayerID, @RebelName, null, null, null, 1, 1
			    END
			END ELSE BEGIN
			    --
			    -- for values > 1, we assume that the number is an integer and we create this many rebels			    
			    --
    	        select @RebelPlayerID = PlayerID FROM SpecialPlayers where Type = -2
			    declare @counter int
			    set @counter = 0 
			    WHILE (@counter < @ChanceOfRebelVillage) BEGIN
			        set @Counter = @counter + 1
				    EXEC iCreateVillage @RebelPlayerID, @RebelName, null, null, null, 1, 1	        
			    END
			END
		END
	end try

	begin catch
		IF @@TRANCOUNT > 0 ROLLBACK
		
		SET @ERROR_MSG = 'CreateVillage FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID'		+ ISNULL(CAST(@PlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @PlayerName'		+ ISNULL(CAST(@PlayerName AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @x'				+ ISNULL(CAST(@x AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @y'				+ ISNULL(CAST(@y AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @RANDOM'			+ ISNULL(CAST(@RANDOM AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @CheckValue'		+ ISNULL(CAST(@CheckValue AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @VillageID'		+ ISNULL(CAST(@VillageID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @VillageName'		+ ISNULL(@VillageName, 'Null') + CHAR(10)
		--+ '   @GoldMineID'		+ ISNULL(CAST(@GoldMineID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ForRebellVillage'	+ ISNULL(CAST(@ForRebellVillage AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @ChanceOfRebelVillage'+ ISNULL(CAST(@ChanceOfRebelVillage AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @DaysOld'			+ ISNULL(CAST(@DaysOld AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @StartLevelID'	+ ISNULL(CAST(@StartLevelID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @StartCoins'		+ ISNULL(CAST(@StartCoins AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'+ ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'   + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'   + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'    + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'   + ERROR_MESSAGE() + CHAR(10)
		
		--
		-- IF deadlock, then rerun if not in nested tran, return with propert error message if nested 
		--
		IF ERROR_NUMBER() = 1205 -- deadlock occured in this SP 
			OR (ERROR_NUMBER() = 50000 AND left(ERROR_MESSAGE(), 8) = 'DEADLOCK' ) -- deadlock occured in some called SP 
		BEGIN	
			IF @IsNestedTransaction = 1 BEGIN 
				--
				-- Nested tran. no rerurn, just let caller know what happend. 
				-- 
				INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock, no rerun', @ERROR_MSG)		
				SET @ERROR_MSG = 'DEADLOCK  -- ' + @ERROR_MSG 		
				RAISERROR(@ERROR_MSG,11,1)	
			END ELSE BEGIN 
				--
				-- NOT Nested tran. rerurn
				-- 
				INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock', @ERROR_MSG)		
				WAITFOR DELAY '00:00:05'
				GOTO __RETRY__		
			END
		END ELSE BEGIN
			--
			-- Some other error, not deadlock
			RAISERROR(@ERROR_MSG,11,1)	
		END
		
	end catch