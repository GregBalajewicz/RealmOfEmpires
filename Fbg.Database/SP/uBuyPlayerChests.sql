IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uBuyPlayerChests')
	BEGIN
		DROP  Procedure  uBuyPlayerChests
	END
GO
CREATE Procedure uBuyPlayerChests
	@PlayerID int
	,@VillageID int
	,@ChestsNo int
	,@ChestCost int
AS
	declare @TotalCost int
	declare @Sucess int
	declare @Result bit
	set @Result=1;

	begin try 

		begin tran 		
			--
			-- Obtain a village LOCK
			--		this will ensure that no one else can effect the village in any way.
			--
			update VillageSemaphore set [TimeStamp] = getdate() WHERE villageID = @VillageID
			set @TotalCost=@ChestsNo*@ChestCost
			exec uVillageCoins_Subtract @VillageID,@TotalCost,@Sucess output
			
			if @Sucess<>1 begin
				-- verify the cost does not exceeed assets. ( this can happen for the same reason the population
				--	check above)
				IF @@TRANCOUNT > 0 ROLLBACK
				INSERT INTO ErrorLog VALUES (getdate(), 0, 'uHirePlayerChests : @TotalCost exceedes assets', ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null'))		
				set @Result=0;
				select @Result
				return 	
			end
			
			update players set Chests=isnull(Chests,0)+@ChestsNo 
			where playerid=@PlayerID 

			select @Result
			
		
			
		commit tran
	end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000)
	 IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uBuyPlayerChests FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @VillageID' + ISNULL(CAST(@VillageID AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @ChestsNo' + ISNULL(CAST(@ChestsNo AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @ChestCost' + ISNULL(CAST(@ChestCost AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @Sucess' + ISNULL(CAST(@Sucess AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   @TotalCost' + ISNULL(CAST(@TotalCost AS VARCHAR(10)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	




GO  