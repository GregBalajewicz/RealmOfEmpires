IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qIdByNameAndRealm')
	BEGIN
		DROP Procedure qIdByNameAndRealm
	END

GO

CREATE PROCEDURE qIdByNameAndRealm
	@Name nvarchar(25),
	@RealmId int = null
AS

BEGIN
	if @RealmId is null begin
		select UserId as Id from Users where GlobalPlayerName = @Name
	end
	else begin
		select PlayerID as Id from vPlayers where RealmID = @RealmId and Name = @Name
	end
END