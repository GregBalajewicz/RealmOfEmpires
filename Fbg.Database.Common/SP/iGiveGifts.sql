IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[iGiveGifts]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[iGiveGifts]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[iGiveGifts]
		@playerID as int
		,@facebookID as varchar(256)
		,@giftID as int
		,@numGifts as int		
		
AS	
	
begin try 
	declare @time as datetime
	declare @useridOfRoeTeam as uniqueidentifier
	set @time =getdate();
    declare @Result int
    declare @i int

	set @useridOfRoeTeam = '00000000-0000-0000-0000-000000000001'
	
	
	BEGIN TRAN
           
		set @i = 0
        while @i<@numGifts
        begin
			--
			-- insert gifts
			--	
			insert into GiftsSent(PlayerID, GiftID, SentTo, SentOn, Type, StatusID)
				values (@playerID
					, @giftID
					, @facebookID
					, @time
					, 1 -- means facebook gift Hardcoded for now.
					, 2 )
			set @i=@i+1
		end	
		select 1
    COMMIT TRAN

end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iGiftsSent FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)		
		+ '   @playerID' + ISNULL(CAST(@playerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @facebookID' + ISNULL(CAST(@facebookID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @giftID' + ISNULL(CAST(@giftID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @numGifts' + ISNULL(CAST(@playerID AS VARCHAR(max)), 'Null') + CHAR(10)		
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch


 
GO


