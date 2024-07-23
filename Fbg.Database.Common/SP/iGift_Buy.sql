     
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iGift_Buy')
	BEGIN
		DROP  Procedure  iGift_Buy
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].iGift_Buy
		@PlayerID as int
		,@GiftID as int
		,@FacebookIDOfPlayer as varchar(256)
		,@rid int
		,@vid int
		,@payout varchar(max)
AS
	
	
begin try 
	declare @time as datetime
	declare @useridOfRoeTeam as uniqueidentifier
	set @time =getdate();
    declare @Result int

	set @useridOfRoeTeam = '00000000-0000-0000-0000-000000000001'
	
	
	BEGIN TRAN
        --
        -- subtract the credits, make sure this worked
        --	    	    	
        EXEC uCredits_Subtract_BuyGift @playerID, 1,@GiftID, @rid ,@vid ,@payout, @Result output
        IF @Result <> 0 BEGIN 
            --
            -- credits not deducted. abort. 
            --
            IF @@TRANCOUNT > 0 ROLLBACK
            RETURN 
        END
              
	    --
	    -- insert gifts
	    --	
	    insert into GiftsSent(PlayerID, GiftID, SentTo, SentOn, Type, StatusID)
		    values (@PlayerID
			    , @GiftID
			    , @FacebookIDOfPlayer
			    , @time
			    , 1 -- means facebook gift Hardcoded for now.
			    , 2 )
		
    COMMIT TRAN

end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iGift_Buy FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @FacebookIDOfPlayer' + ISNULL(CAST(@FacebookIDOfPlayer AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @GiftID' + ISNULL(CAST(@GiftID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 