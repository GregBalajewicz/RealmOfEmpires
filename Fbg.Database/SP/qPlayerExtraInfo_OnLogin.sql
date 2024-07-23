IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayerExtraInfo_OnLogin')
	BEGIN
		DROP  Procedure  qPlayerExtraInfo_OnLogin
	END
GO

CREATE Procedure qPlayerExtraInfo_OnLogin
	(
		@PlayerID int,
		@xpToUpdate int
	)

AS	

	IF  @xpToUpdate is not null BEGIN 
		update players set xp_cached = @xpToUpdate where PlayerID = @PlayerID
	END 

	select FlagID, UpdatedOn, Data from PlayerFlags where PlayerID = @PlayerID

	-- this code was removed as it is not used and caused perforamnce issue
	-- CODE WAS:
	--		exec FBGC.Fbgcommon.dbo.qPlayersFriends @PlayerID	
	select playerid from players where playerid = -9999
	
	select PFTrialID, ExpiresOn from PlayerPFTrials where PlayerID = @playerid
	
	select RegisteredOn, Anonymous, TitleID, Sex, NightBuildActivatedOn, AvatarID, Morale, MoraleLastUpdated from players where playerid = @playerid
	
	-- get active stewards
	SELECT StewardPlayerID from AccountStewards		
		where PlayerID = @PlayerID
		and State = 2
	
GO