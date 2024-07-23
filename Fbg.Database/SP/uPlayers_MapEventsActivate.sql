IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPlayers_MapEventsActivate')
	BEGIN
		DROP  Procedure  uPlayers_MapEventsActivate
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--
-- activate a map event
--
CREATE Procedure [dbo].uPlayers_MapEventsActivate
		@playerID as int,
		@eventID as int, 
		@typeID as int, 
		@xCord as int, 
		@yCord as int,
		@result varchar(MAX) output
AS
	
begin try 
	
	begin tran
        
        declare @totalCount int;
					
        if (@typeID = 1) begin --Credit Farm Claim
			
			declare @EffectedEvents table(data int)

			update PlayerMapEvents set isActive = 0 
			output inserted.data as data
			into @EffectedEvents
			where IsActive = 1 and PlayerID = @PlayerID and XCord = @xCord and YCord = @yCord and TypeID = @typeID;
 
			select @totalCount = sum(CAST(Data AS int)) from @EffectedEvents;
			set @totalCount = ISNULL(@totalCount,0);
			
			-- There is still more to be done outside, because we can't do FBGCommon.dbo.uGiveServants in this tran, it would be a distributed transaction.
        end
        else if (@typeID = 2) begin
        	update PlayerMapEvents set isActive = 0 where IsActive = 1 and PlayerID = @PlayerID and XCord = @xCord and YCord = @yCord and TypeID = @typeID;
        	--need a check here to make sure update worked, before setting success
        	set @result ='success';
        end
        else begin
			set @result = 'fail';
        end
		
	commit tran

	if (@typeID = 1 and @totalCount > 0) begin --Credit Farm Claim
						
		declare @playerUserID uniqueidentifier;	
		declare @creditsUpdated int;
		declare @res int;
				
		select @playerUserID = UserID From Players where PlayerID = @playerID; --get players user ID		
		exec @res = FBGCommon.dbo.uGiveServants @playerUserID, @totalCount, 32; --give player servants
		select @creditsUpdated = Credits from FBGCommon.dbo.Users where UserId = @playerUserID; --get latest servant count
			
		--output data seperated by commas
		set @result = CAST(@creditsUpdated AS VARCHAR(20))+','+CAST(@totalCount AS VARCHAR(20));
        
    end
    else if (@typeID = 2) begin
    	-- as above, need to make sure update succeeded before setting a success
    	set @result ='success';
    end
	else begin
		set @result = 'fail';
    end

end try

begin catch

	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'uPlayers_MapEventsActivate FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @typeID' + ISNULL(CAST(@typeID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @eventID' + ISNULL(CAST(@eventID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @xCord' + ISNULL(CAST(@xCord AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @yCord' + ISNULL(CAST(@yCord AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	


   