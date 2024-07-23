 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qvPlayers')
	BEGIN
		DROP  Procedure  qvPlayers
	END

GO
CREATE Procedure [dbo].qvPlayers
		@PlayerID int,
		@PlayerName varchar(25)

AS
select * from vplayers where (@PlayerID is null OR PlayerID = @PlayerID) AND (@PlayerName is null OR Name = @PlayerName)