IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iSendMail')
	BEGIN
		DROP  Procedure  iSendMail
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go


CREATE proc [dbo].[iSendMail]
@SenderID as int,@RecipientIDs as varchar(max),@Subject as varchar(90),@Message as varchar(max),@RecipientNames as varchar(max)
as
begin try

	IF exists (select * From playersuspensions where playerid = @SenderID and SupensionID = 3) BEGIN 
		RETURN 
	END

	begin tran
		Declare @MsgID as int

		insert into Messages(PlayerID,Subject,Body,TimeSent,RecipientNames)
		values(@SenderID,@Subject,@Message,getDate(),@RecipientNames)

		set @MsgID=@@Identity

		Declare @RecID int
		Declare @Pos int

		SET @RecipientIDs = LTRIM(RTRIM(@RecipientIDs))+ ','
		SET @Pos = CHARINDEX(',', @RecipientIDs, 1)

		INSERT INTO MessageAddressees (MessageID,PlayerID,IsViewed,Type) VALUES (@MsgID,@SenderID,0,0)

		while(@Pos>0)
		begin
			SET @RecID = LTRIM(RTRIM(LEFT(@RecipientIDs, @Pos - 1)))
			IF @RecID <> ''
			BEGIN
				if not exists (select * from MessagesBlockedPlayers where PlayerID=@RecID and BlockedPlayerID=@SenderID)
					begin
						INSERT INTO MessageAddressees (MessageID,PlayerID,IsViewed,Type) VALUES (@MsgID,CAST(@RecID AS int),0,1) --Use Appropriate conversion
						update Players set NewMessageIndicator = 1 where PlayerID = CAST(@RecID AS int)
					end
			END
			SET @RecipientIDs = RIGHT(@RecipientIDs, LEN(@RecipientIDs) - @Pos)
			SET @Pos = CHARINDEX(',', @RecipientIDs, 1)
		end
	commit tran
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	SET @ERROR_MSG = 'iSendMail FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @SenderID' + ISNULL(CAST(@SenderID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @RecipientIDs' + ISNULL(CAST(@RecipientIDs AS VARCHAR(600)), 'Null') + CHAR(10)
		+ '   @Subject' + ISNULL(CAST(@Subject AS VARCHAR(50)), 'Null') + CHAR(10)
		+ '   @Message' + ISNULL(CAST(@Message AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @RecipientNames' + ISNULL(CAST(@RecipientNames AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch