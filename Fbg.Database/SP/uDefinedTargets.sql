
if exists (select * from sysobjects where type = 'P' and name = 'uDefinedTargets')
begin
	drop procedure uDefinedTargets
end
go

create Procedure uDefinedTargets
	@PlayerID int
	, @DefinedTargetID int
	, @setTime DateTime
	, @note varchar(max)
	, @ExpiresInXDays int
	, @assignedTo varchar(max) = null

as
set nocount on

update DefinedTargets SET SetTime = @setTime, Note = @note, ExpiresOn = DATEADD(day, @ExpiresInXDays, getdate()) 
	where definedTargetID = @DefinedTargetID and PlayerID = @PlayerID 

IF @assignedTo is not null BEGIN
	update DefinedTargets SET assignedTo = 	(select playerid from players where name = @assignedTo)
		where definedTargetID = @DefinedTargetID and PlayerID = @PlayerID 
END

 EXEC qDefinedTargets2 @PlayerID