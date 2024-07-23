IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iChat_SaveMessage2')
	BEGIN
		DROP  Procedure  iChat_SaveMessage2
	END

GO

CREATE PROCEDURE iChat_SaveMessage2
	@UserId as uniqueidentifier,
	@PlayerId as int = null,
	@Text as nvarchar(256),
	@Date as datetime,
	@GroupId as uniqueidentifier
AS

BEGIN

	insert into ChatMsgs2 values (@UserId, @PlayerId, @Text, @Date, @GroupId)

	/*
	if (select COUNT(*) from ChatMsgs2 where UserId = @UserId
	and ((PlayerId is NULL and @PlayerId is NULL) or PlayerId = @PlayerId) 
	and GroupId = @GroupId
	and [DateTime] > Dateadd(ss, -10, @Date) and [Text] = @Text) >= 5 begin
		select 1; -- Too many repeated posts
	end

	if (select COUNT(*) from ChatMsgs2 where UserId = @UserId 
	and ((PlayerId is NULL and @PlayerId is NULL) or PlayerId = @PlayerId) 
	and GroupId = @GroupId
	and [DateTime] > Dateadd(ss, -5, @Date)) >= 5 begin
		select 2; -- Too many posts
	end


	select 0
		*/

END