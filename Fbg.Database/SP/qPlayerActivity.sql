 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerActivity')
	BEGIN
		DROP  Procedure  qPlayerActivity
	END

GO
-- procedure deleted