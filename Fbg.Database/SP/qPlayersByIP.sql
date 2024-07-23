
 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qPlayersByIP')
	BEGIN
		DROP  Procedure  qPlayersByIP
	END

GO 
  
Create Procedure dbo.qPlayersByIP        
	@IP varchar(max)        
      
AS   
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

begin try
	select 
		distinct CP.PlayerID
		,CP.Name 
		,P.Anonymous
		,P.reportsCheckedon
		,P.ClanForumCheckedOn
		,P.MessagesCheckedOn
		,P.RegisteredOn
		,0 
		,P.Chests 
		, c.name as ClanName
		, isnull(points, -1) as Points
		, count(*) as '#TimesLoggedInFromIP'
		, (select data from playerflags where flagid = 75 and playerid = P.PlayerID) 'StartedInQuad'
		, null --, 
		, LL.RealmID
		from FBGC.FBGCommon.dbo.LoginLog LL
		join FBGC.FBGCommon.dbo.players CP
			on CP.PlayerID=LL.PlayerID
		left join players P
			on P.PlayerID=LL.PlayerID
		left join clanmembers CM
			on P.PlayerID = CM.PlayerID
		left join clans C	
			on C.ClanID = CM.ClanID
		where REMOTE_ADDR=@IP  
		group by P.PlayerID, cP.PlayerID,P.Name , CP.Name,P.Anonymous,P.reportsCheckedon,P.ClanForumCheckedOn
				,P.MessagesCheckedOn,P.RegisteredOn,P.Chests , c.name	, points, LL.RealmID
		order by ll.realmid
	    
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'qPlayersByIP FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @IP' + ISNULL(CAST(@IP AS VARCHAR(25)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	
end catch