  
  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uBoostLoyalty')
	BEGIN
		DROP  Procedure  uBoostLoyalty
	END

GO

--
-- If successful, this returns the amount of credits used up for this action in the OUTPUT param @Cost
--  if not succesful, @Cost is zero or null
--
CREATE Procedure uBoostLoyalty
		@VillageID  int
		, @PlayerID int 
        , @Cost int OUTPUT
		, @PrintDebugInfo BIT = null

AS
	DECLARE @DEBUG INT
	IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
		SET @DEBUG = 1
		SET NOCOUNT OFF
	END ELSE BEGIN
		SET @DEBUG = 0
		SET NOCOUNT ON
	END 
	IF @DEBUG = 1 SELECT 'BEGIN uBoostLoyalty ' + cast(@VillageID as varchar(10))
	
	declare @PackageID int
	declare @result int
	declare @LoyaltyBefore int
	declare @success_subtractCredits bit
	
	__RETRY__:
	begin try 
		set @success_subtractCredits = 0 --default val
		--
		-- figure out what PF we are using. we note the PackageID, not PF id 
		--
        SET @PackageID = 31
		
		SELECT @Cost = Cost FROM PFPackages where PFPackageID = @PackageID
		IF @Cost is null BEGIN
		    RAISERROR('No cost found',11,1)			
		END

		declare @HasEnoughCredits int 
		exec @HasEnoughCredits = FBGC.FBGCommon.dbo.qDoesPlayerHaveEnoughCredits @PlayerID, @Cost
		IF @HasEnoughCredits != 1 BEGIN 
			--
			-- not enough credits. abort. 
			--
        	IF @DEBUG = 1 SELECT 'insufficient funds. Abort'        
			SET @Cost = 0
			RETURN 
		END

		begin tran
			--
			-- Obtain a village LOCK
			--		this will ensure that no one else will change the buildings in the village, or effect the building Q 
			--
			update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @VillageID
			IF @@rowcount <> 1 BEGIN	
				IF @DEBUG = 1 SELECT 'No village semaphore taken!'
				RAISERROR('No village semaphore taken!',11,1)	
			END							

			select 
				@LoyaltyBefore = cast(loyalty + floor(cast(datediff(minute, LoyaltyLastUpdated, getdate()) as real) /(60.0 / (select cast(AttribValue as real) FROM RealmAttributes where attribid =8))) as integer) 
				from villages v 
				where VillageID = @VillageID

            
			update villages set LoyaltyLastUpdated = getdate(), Loyalty=100 
				where villageid = @VillageID 
				and OwnerPlayerID = @PlayerID
				and @LoyaltyBefore < 100 -- why? well, just in case there was some problem - we dont want the player to be charged twice or when not necessary
                       
			IF @@rowcount = 1 BEGIN								
				set @success_subtractCredits = 1

   				-- log the process of extending new package
				insert into PlayerPFLog
					(PlayerID,Time ,EventType,Credits ,Cost,notes)
					values
    				(@PlayerID,getdate(),3,@Cost,-1
    					, Cast(@PackageID as varchar(max)) + '- VID=' + Cast(@VillageID as varchar(max)) + ', ApprovalBefore=' + Cast(@LoyaltyBefore as varchar(max))  
    				)
			END					
		commit tran
		
		--
		-- subtract the credits
		--
		IF @success_subtractCredits = 1 BEGIN	    	    	
			EXEC FBGC.FBGCommon.dbo.uCredits_Subtract2 @playerID, @Cost, @Result output
			IF @Result <> 0 BEGIN 
				-- credits not deducted. this means player got this for free. we ignore this.
        		IF @DEBUG = 1 SELECT 'credits not deducted. this means player got the speed up for free. we ignore this.'   
				INSERT INTO ErrorLog VALUES (getdate(), 99, 'no-credits', 'boost loyalty')		 
			END
		END
					      
		
		IF @DEBUG = 1 SELECT 'END uBoostLoyalty ' + cast(@VillageID as varchar(10))
		
	end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uBoostLoyalty FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @VillageID'		+ ISNULL(CAST(@VillageID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PlayerID'		+ ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PackageID'		+ ISNULL(CAST(@PackageID AS VARCHAR(20)), 'Null') + CHAR(10)
		
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
		
	--
	-- IF deadlock, rerun
	--
	IF ERROR_NUMBER() = 1205 -- deadlock occured in this SP 
		OR (ERROR_NUMBER() = 50000 AND left(ERROR_MESSAGE(), 8) = 'DEADLOCK' ) -- deadlock occured in some called SP 
	BEGIN
		INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock', @ERROR_MSG)		
		WAITFOR DELAY '00:00:05'
		GOTO __RETRY__
	END 		
		
	RAISERROR(@ERROR_MSG,11,1)	

end catch	


--
-- say that the village has changed. this is done deliberately outside of the main tran and try 
--
UPDATE VillageCacheTimeStamps SET TimeStamp = getdate() where PlayerID = @PlayerID and VillageID = @VillageID and CachedItemID = 0
IF (@@rowcount < 1 ) BEGIN
	INSERT INTO VillageCacheTimeStamps values (@PlayerID, @VillageID, 0, getdate())
END

GO 