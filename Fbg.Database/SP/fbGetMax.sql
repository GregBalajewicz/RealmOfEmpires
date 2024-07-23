
IF OBJECT_ID (N'dbo.fbGetMax') IS NOT NULL
   DROP FUNCTION dbo.fbGetMax
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION dbo.fbGetMax
(
	@Value1 int
	,@Value2 int
)
RETURNS int
AS
BEGIN
	declare @Ret int
	IF @Value1 > @Value2
		set @Ret=  @Value1
	ELSE
		set @Ret= @Value2

	RETURN @Ret
END
GO

 