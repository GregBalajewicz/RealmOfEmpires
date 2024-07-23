IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerAutocomplete')
	BEGIN
		DROP Procedure qPlayerAutocomplete
	END

GO

CREATE PROCEDURE qPlayerAutocomplete
	@Term nvarchar(25),
	@RealmId int = null
AS

BEGIN
	if @RealmId is null begin
		select GlobalPlayerName as Name from Users where LOWER(GlobalPlayerName) like LOWER(@Term) + '%' order by GlobalPlayerName
	end
	else begin
		select Name as Name from vPlayers where RealmID = @RealmId and LOWER(Name) like LOWER(@Term) + '%' order by Name
	end
END