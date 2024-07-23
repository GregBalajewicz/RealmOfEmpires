
IF OBJECT_ID (N'dbo.fnGetTravelTime') IS NOT NULL
   DROP FUNCTION dbo.fnGetTravelTime
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION fnGetTravelTime
(
	@originX int 
	,@originY int 
	,@destinationX int 
	,@destinationY int 
	,@UnitSpeed int
)
RETURNS BigInt
AS
BEGIN

	declare @distX real
	declare @distY real
	declare @distance real
	declare @TimeIntTicks BigInt 

	set @distX = Abs(@originX - @destinationX)
	set @distY = Abs(@originY - @destinationY)

	set @distX = @distX * @distX;
	set @distY = @distY * @distY;

	set @Distance =  Sqrt(@distX + @distY)
	set @TimeIntTicks =  Floor( (@Distance / @UnitSpeed) * 36000000000/*ticks per hour*/)

	RETURN @TimeIntTicks
END
GO

  