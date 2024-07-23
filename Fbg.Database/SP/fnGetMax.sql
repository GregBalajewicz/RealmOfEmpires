 
IF OBJECT_ID (N'dbo.fnGetMax') IS NOT NULL
   DROP FUNCTION dbo.fnGetMax
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION dbo.fnGetMax
(
	@Value1 datetime
	,@Value2 datetime
)
RETURNS datetime
AS
BEGIN
	declare @Ret datetime
	IF @Value1 > @Value2
		set @Ret=  @Value1
	ELSE
		set @Ret= @Value2

	RETURN @Ret
END
GO

 