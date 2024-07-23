
if exists (select * from sysobjects where type = 'P' and name = 'iDefinedTargets2')
begin
	drop procedure iDefinedTargets2
end
go

create Procedure iDefinedTargets2
	@PlayerID int
	, @VillageID int
	, @TypeID smallint
	, @setTime DateTime
	, @note varchar(max)
	, @ExpiresInXDays int
	, @assignedTo varchar(max)
as
set nocount on


insert into DefinedTargets
	(PlayerID			,
    VillageID			,
    TimeCreated			,
	SetTime				,
	DefinedTargetTypeID	,
	Note				,
	ExpiresOn			,
	AssignedTo) 				
 values (@PlayerID,@VillageID, getdate(), @setTime, @TypeID,@note, dateadd(day, @ExpiresInXDays, getdate()), (select playerid from players where name = @assignedTo) )
 
 declare @ID int
 SELECT @ID = @@IDENTITY

 EXEC qDefinedTargets2 @PlayerID

 SELECT @ID