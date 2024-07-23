
  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uBuyResearcher')
	BEGIN
		DROP  Procedure  uBuyResearcher
	END

GO

CREATE Procedure uBuyResearcher
		@PlayerID int
		, @maxBoughtResearchers int
		, @Status int output -- 1 OK-success, 2 failed-max researchers, 3 failed-no credits
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
	IF @DEBUG = 1 SELECT 'BEGIN uBuyResearcher ' + cast(@PlayerID as varchar(10))
	
	declare @cost int
	declare @PackageID int
	declare @Result int
	
	__RETRY__:
	begin try 
		SET @PackageID = 100
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
			SET @Status = 3 -- no credits
			SET @Cost = 0
			RETURN 
		END

		begin tran
			SET @Status = 2 -- if max researchers reached, code will just finish after the next line

			insert into researchers 
				select p.playerid from players p  where p.playerid = @PlayerID and 
				exists (select count(*) from researchers r where r.playerid = @PlayerID having COUNT(*) < @maxBoughtResearchers)


			IF @@rowcount = 1 BEGIN	
				SET @Status = 1 -- so far so good, so note that we are successful. if somethin else fails later, code will be updated

       			-- log the process of extending new package
				insert into PlayerPFLog
    				(PlayerID,Time ,EventType,Credits ,Cost,notes)
					values
	    			(@PlayerID,getdate(),3,@Cost,-1,  Cast(@PackageID as varchar(max)) + ' - researcher')
	    		    
            END
		
		commit tran

		--
		-- subtract the credits
		--
		IF @Status = 1 BEGIN	    	    	
			EXEC FBGC.FBGCommon.dbo.uCredits_Subtract2 @playerID, @Cost, @Result output
			IF @Result <> 0 BEGIN 
				-- credits not deducted. this means player got this for free. we ignore this.
        		IF @DEBUG = 1 SELECT 'credits not deducted. this means player got the speed up for free. we ignore this.'   
				INSERT INTO ErrorLog VALUES (getdate(), 99, 'no-credits', 'buy researcher')		 
			END
		END
					
		IF @DEBUG = 1 SELECT 'END uBuyResearcher ' + cast(@PlayerID as varchar(10))
		
	end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uBuyResearcher FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @PlayerID'		+ ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		
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


GO