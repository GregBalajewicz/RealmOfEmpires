IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qIncomingAttack')
	BEGIN
		DROP  Procedure  qIncomingAttack
	END

GO



/*
WHY the "with(nolock)" hint on all tables ?? because we really don't care about dirty reads here and this is executed so 
    often, that we want this to run as fast as possible. 
*/
CREATE Procedure qIncomingAttack
       @PlayerID int 
AS
 
select E.EventTime
	into #tmp
    from unitmovements UM  with(nolock)
    join Events E  with(nolock)
        on E.EventID = UM.EventID
    join Villages V  with(nolock)
        on V.VillageID = DestinationVillageID
    join Players P  with(nolock)
        on P.PlayerID = V.OwnerPlayerID
    where Status = 0
        and P.PlayerID =  @PlayerID
        and UM.CommandType = 1 /*1 == attack*/
		and UM.VisibleToTarget > 0   
	    and E.EventID not in (SELECT EventID FROM UnitMovements_PlayerAttributes with(nolock) WHERE playerid = @PlayerID and AttribID=1)
     
    order by EventTime asc
    
select count(*), (select top 1 EventTime from #tmp order by EventTime asc)
 from #tmp
    

GO


