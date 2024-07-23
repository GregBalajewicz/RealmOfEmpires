IF OBJECT_ID (N'dbo.fnConvertToInt') IS NOT NULL
   DROP FUNCTION dbo.fnConvertToInt
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[fnConvertToInt]
(
	@Value1 bit
)
RETURNS int
AS
BEGIN
	declare @Ret int
	IF @Value1 = 1
		set @Ret=  1
	ELSE
		set @Ret= 0

	RETURN @Ret
END

