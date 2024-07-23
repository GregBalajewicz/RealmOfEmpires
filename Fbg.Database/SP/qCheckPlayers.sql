IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qCheckPlayers')
	BEGIN
		DROP Procedure qCheckPlayers
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE proc [dbo].qCheckPlayers
	@PlayerID as int -- ID of the player checking the other players 
    , @PlayerName as varchar(max)
    , @DetailLevel smallint
    , @HasPF bit= null
as
	Create table #TempUsersForEmail
	(
		PlayerName varchar(max)
	)

	Declare @Name as varchar(max)
	Declare @Pos int

	SET @PlayerName = LTRIM(RTRIM(@PlayerName))+ ','
	SET @Pos = CHARINDEX(',', @PlayerName, 1)

	while(@Pos>0)
	begin
		SET @Name = LTRIM(RTRIM(LEFT(@PlayerName, @Pos - 1)))
		IF @Name <> ''	
		BEGIN
			insert into #TempUsersForEmail values(@Name)
		END
		SET @PlayerName = RIGHT(@PlayerName, LEN(@PlayerName) - @Pos)
		SET @Pos = CHARINDEX(',', @PlayerName, 1)
	end

	IF @DetailLevel = 2 BEGIN
	    --
	    -- Detailed view
	    --   the 2 qierries are EXACTLY the same except for the "TOP 2" in one
	    --
	    IF @HasPF is null or @HasPF = 0 BEGIN
		    select 		       
		        distinct 
		        TOP 2 
		        P.PlayerID
			    ,P.Name
			    , P.points as 'TotalPoints'
			    , isnull(C.Name,'') as 'ClanName'
			    , isnull(C.ClanID,0) as 'ClanID'
			    , isnull(CD.StatusID, -1) as StatusID 
			    , isnull(substring(PN.Note, 1, 51),'') as 'PartialNote'
			    , P.TitleID 
			    , P.Sex
			    from players P
			    join #TempUsersforEmail t 
				    on p.name=t.playername		
			    left join ClanMembers CM
				    on cm.PlayerID=p.PlayerID
			    left join Clans C
				    on CM.ClanID=C.ClanID
			    left join PlayerNotes PN
				    on PN.PlayerID=P.PlayerID and PN.NoteOwnerPlayerID=@PlayerID
			    left JOIN ClanDiplomacy CD
				    ON CD.ClanID =(select clanid from clanmembers where playerid = @PlayerID)
				    AND CD.OtherClanID = CM.ClanID
			    where P.playerid not in (select PlayerID from SpecialPlayers ) -- exclude special players 
				    and PlayerStatus = 1
			    order by StatusID, ClanName	
	    END ELSE BEGIN 
		    select 
		        distinct
		         P.PlayerID
			    ,P.Name
			    , P.points as 'TotalPoints'
			    , isnull(C.Name,'') as 'ClanName'
			    , isnull(C.ClanID,0) as 'ClanID'
			    , isnull(CD.StatusID, -1) as StatusID 
			    , isnull(substring(PN.Note, 1, 71),'') as 'PartialNote'
			    , P.TitleID 
			    , P.Sex
			    from players P
			    join #TempUsersforEmail t 
				    on p.name=t.playername		
			    left join ClanMembers CM
				    on cm.PlayerID=p.PlayerID
			    left join Clans C
				    on CM.ClanID=C.ClanID
			    left join PlayerNotes PN
				    on PN.PlayerID=P.PlayerID and PN.NoteOwnerPlayerID=@PlayerID
			    left JOIN ClanDiplomacy CD
				    ON CD.ClanID =(select clanid from clanmembers where playerid = @PlayerID)
				    AND CD.OtherClanID = CM.ClanID
			    where P.playerid not in (select PlayerID from SpecialPlayers ) -- exclude special players 
				    and PlayerStatus = 1
			    order by StatusID, ClanName
	    END
	END ELSE BEGIN
	    --
	    -- Simple view only 
	    -- 
		select distinct p.PlayerID,p.Name from players p join #TempUsersforEmail t on p.name=t.playername
	END 

	select playername from #TempUsersforEmail where playername not in(select name from players)

	drop table #TempUsersForEmail