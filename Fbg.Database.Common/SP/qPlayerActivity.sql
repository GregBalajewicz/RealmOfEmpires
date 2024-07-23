 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerActivity')
	BEGIN
		DROP  Procedure  qPlayerActivity
	END

GO
CREATE Procedure [dbo].qPlayerActivity
		@PlayerID int
AS
	
	
select top 1 [time] from loginlog where playerid = @PlayerID order by time desc
select count(*) from loginlog where playerid = @PlayerID 