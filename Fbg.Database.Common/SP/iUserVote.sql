 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iUserVote')
	BEGIN
		DROP  Procedure  iUserVote
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

/*
Returns one result set with vote result fot specific poll
this SP used for 2 perposes 
1 - to insert vote for player and return voting result(if the OptionsIDs have values)
2- just get voting result for specific poll id (if the optionIDs is null)

*/
CREATE Procedure dbo.iUserVote
	@UserID uniqueidentifier ,
	@PollID int,
	@OptionsIDs varchar(max)

AS
set nocount on 
begin try 
	if @OptionsIDs is not null begin
		begin tran 

		--clear the db form other choices done by this user .
		
		delete PollResponses from PollResponses
			join polloptions 
				on pollresponses.PollOptionID=PollOptions.PollOptionID
			where polloptions.PollID=@PollID 
				and PollResponses.PollOptionID not in (select ID from fnGetIds(@OptionsIDs))
				and PollResponses.Userid=@UserID
		
		Declare @RecID int
		Declare @Pos int

		SET @OptionsIDs = LTRIM(RTRIM(@OptionsIDs))+ ','
		SET @Pos = CHARINDEX(',', @OptionsIDs, 1)

		while(@Pos>0)
		begin
			SET @RecID = LTRIM(RTRIM(LEFT(@OptionsIDs, @Pos - 1)))
			IF @RecID <> ''
			BEGIN
			-- insert the value of the new user choice
				if not exists (select * from PollResponses where Userid=@UserID and PollOptionID =@RecID)
				begin
					insert into PollResponses (UserID,PollOptionID) 
					values (@UserID,@RecID)
					
				end		
				
			END
			SET @OptionsIDs = RIGHT(@OptionsIDs, LEN(@OptionsIDs) - @Pos)
			SET @Pos = CHARINDEX(',', @OptionsIDs, 1)
		end
	
		commit tran
	end

	--	
	-- return the result of votes
	--	there is a stlighly different sql depending on poll type
	--
	declare @PollType smallint
	select @PollType = PollType from Polls where pollid = @PollID
	
	if @PollType = 0 BEGIN
		select  PollOptions.PollOptionID
				,PollOptions.Text
				,count(pollresponses.PollOptionID) as NoOfVotes
				,(select count(*) 
					from pollResponses 
					join PollOptions 
						on PollOptions.PollOptionID=pollResponses.PollOptionID
					where pollid=@PollID) as TotalVotes
			from pollresponses 
			right outer join polloptions 
				on PollOptions.PollOptionID=pollResponses.PollOptionID
			where pollid=@PollID
			group by PollOptions.PollOptionID,PollOptions.Text
	END ELSE IF @PollType = 1 BEGIN
		select  PollOptions.PollOptionID
				,PollOptions.Text
				,count(pollresponses.PollOptionID) as NoOfVotes
				,(select count(distinct userid) 
					from pollResponses 
					join PollOptions 
						on PollOptions.PollOptionID=pollResponses.PollOptionID
					where pollid=@PollID) as TotalVotes
			from pollresponses 
			right outer join polloptions 
				on PollOptions.PollOptionID=pollResponses.PollOptionID
			where pollid=@PollID
			group by PollOptions.PollOptionID,PollOptions.Text
	
	END

end try
begin catch

	DECLARE @ERROR_MSG AS VARCHAR(max)
	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iUserVote FAILED! '	+  CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:'	+ CHAR(10)
		+ '   @UserID'				+ ISNULL(CAST(@UserID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @PollID'		+ ISNULL(CAST(@PollID AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   @OptionsIDs'		+ ISNULL(CAST(@OptionsIDs AS VARCHAR(100)), 'Null') + CHAR(10)

		+ '   ERROR_NUMBER():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'		  + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'			  + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'		  + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'			  + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'           + ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch	



GO



