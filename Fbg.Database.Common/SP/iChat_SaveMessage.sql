IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iSaveMessage')
	BEGIN
		DROP  Procedure  iSaveMessage
	END

GO

IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iChat_SaveMessage')
	BEGIN
		DROP  Procedure  iChat_SaveMessage
	END

GO

CREATE PROCEDURE iChat_SaveMessage
	@UserId as uniqueidentifier,
	@Text as nvarchar(256),
	@Date as datetime,
	@RealmID as int = null,
	@GroupId as uniqueidentifier = null
AS

BEGIN

	insert into ChatMsgs(UserId, Text, DateTime, RealmID, GroupId) values (@UserId, @Text, @Date, @RealmID, @GroupId)

	if (select COUNT(*) from ChatMsgs where UserId = @UserId
	and ((RealmID is NULL and @RealmID is NULL) or RealmID = @RealmID) 
	and ((GroupId is NULL and @GroupId is NULL) or GroupId = @GroupId) 
	and [DateTime] > Dateadd(ss, -10, @Date) and [Text] = @Text) >= 5 begin
		select 1; -- Too many repeated posts
	end

	if (select COUNT(*) from ChatMsgs where UserId = @UserId 
	and ((RealmID is NULL and @RealmID is NULL) or RealmID = @RealmID) 
	and ((GroupId is NULL and @GroupId is NULL) or GroupId = @GroupId) 
	and [DateTime] > Dateadd(ss, -5, @Date)) >= 5 begin
		select 2; -- Too many posts
	end

	select 0

END