 
IF OBJECT_ID (N'dbo.fnGetUnitMovementReturnType') IS NOT NULL
   DROP FUNCTION dbo.fnGetUnitMovementReturnType
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--
-- if 0 passed, returns 3 (ie, if command is support then return type is recall)
-- if 1 passed, returns 2 (ie, if command is attack then return type is attack return )
--
CREATE FUNCTION fnGetUnitMovementReturnType 
(
	--
	-- 0 - suport
	-- 1 - attack
	--
	@commandType int
)
RETURNS int
AS
BEGIN
	declare @returnType int
	if @commandType = 0 begin
		set @returnType= 3
	end else if @commandType = 1 begin
		set @returnType= 2
	end

	return @returnType
END
GO