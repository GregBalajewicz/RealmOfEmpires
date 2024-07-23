
if exists (select * from sysobjects where type = 'P' and name = 'dDefinedTargets')
begin
	drop procedure dDefinedTargets
end
go

create Procedure dDefinedTargets
	@PlayerID int
	, @DefinedTargetID  int
	
as
set nocount on

delete  DefinedTargets where definedTargetID = @DefinedTargetID 
	and @PlayerID = @PlayerID

 EXEC qDefinedTargets2 @PlayerID
