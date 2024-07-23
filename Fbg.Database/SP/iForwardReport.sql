IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iForwardReport')
	BEGIN
		DROP  Procedure  iForwardReport
	END

GO

CREATE Procedure iForwardReport
		@recordIDs varchar(2000)
    	,@playerID int
	    ,@PlayerName varchar(25)
	    ,@Error int output
AS

       set @Error=0
		declare @To_PlayerID as int 
		declare @Count as int 
		declare @Counter as int 
		declare @Value as bigint
		declare @RecValue as bigint
        declare @RecordID as bigint
begin Try         

         --- Temperary Table To store IDs
        DECLARE @TempTable TABLE (IndexValue int PRIMARY KEY IDENTITY(1,1),ID bigint )

         --set IDENTITY_INSERT @TempTable ON
        insert @TempTable(ID) select ID from fnGetIds(@recordIDs)
        
        Select @Count = count(*) From @TempTable;  

		SELECT    @To_PlayerID = Players.PlayerID 
		FROM      Players
		where Players.Name=@PlayerName;
		
		--
		-- if recipient blocked the sender, don't allow the forward
		IF exists (select * from MessagesBlockedPlayers where PlayerID=@To_PlayerID and BlockedPlayerID=@playerID) begin
		    RETURN
		END
		
		
	
		IF @To_PlayerID is null BEGIN
			set @Error  = 1
			RETURN
	    End
	    
	    set @Counter = 1
        while @Counter <= @Count 
		begin
            select @RecValue = ID from @TempTable IDs where IDs.IndexValue = @Counter


	        Insert into ReportAddressees(
				PlayerID
				,ReportID
				,ForwardedByPlayerID
				,ForwardedOn
				,IsViewed
				,PointOfView
				,AlternateSubject)            
		        SELECT @To_PlayerID
					, ReportID
					, @playerID
					, getdate()
					,0
					, PointOfView
					, AlternateSubject 
					FROM ReportAddressees
		        	WHERE PlayerID = @playerID AND RecordID=@RecValue
            SET @RecordID = scope_identity()
	        Insert into ReportInfoFlag(
				RecordID
				,FlagID
				,FlagValue)            
		        SELECT @RecordID
				,FlagID
				,FlagValue
					FROM ReportInfoFlag
		        	WHERE RecordID=@RecValue


			update Players set NewReportIndicator = 1 where PlayerID = @To_PlayerID

            set @Counter = @Counter + 1
        end
	
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK
	
	SET @ERROR_MSG = 'iForwardReport FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @recordIDs' + ISNULL(CAST(@recordIDs AS VARCHAR(2000)), 'Null') + CHAR(10)
		+ '   @playerID' + ISNULL(CAST(@playerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @To_PlayerID' + ISNULL(CAST(@To_PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @Error' + ISNULL(CAST(@Error AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	


