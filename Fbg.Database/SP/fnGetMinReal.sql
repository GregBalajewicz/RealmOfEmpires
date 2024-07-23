
IF OBJECT_ID (N'dbo.fbGetMinReal') IS NOT NULL
   DROP FUNCTION dbo.fbGetMinReal
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION dbo.fbGetMinReal
(
	@Value1 real
	,@Value2 real
)
RETURNS real
AS
BEGIN
	declare @Ret real
	IF @Value1 < @Value2
		set @Ret=  @Value1
	ELSE
		set @Ret= @Value2

	RETURN @Ret
END
GO

 