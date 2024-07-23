SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO


IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'tbh_Forums_InsertWhiteListClan')
	BEGIN
		DROP  Procedure  [tbh_Forums_InsertWhiteListClan]
	END

GO

create PROCEDURE [dbo].[tbh_Forums_InsertWhiteListClan]
(
   @WhiteListClanName nvarchar(30),
   @ClanID  int,
   @PlayerID int 
)
AS
SET NOCOUNT ON

	declare @WhiteListClanID as int ;
	set @WhiteListClanID=0;
	select @WhiteListClanID=ClanID from Clans where Name=@WhiteListClanName
	if @WhiteListClanID <>0 and not exists (select * from ForumSharingWhiteListedClans where ClanID=@ClanID  and WhiteListClanID=@WhiteListClanID)
		begin
			insert into ForumSharingWhiteListedClans (ClanID,WhiteListClanID)values(@ClanID,@WhiteListClanID);
			select 1;
		end
	else
		select 0;
GO