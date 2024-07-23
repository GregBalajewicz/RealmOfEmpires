
IF EXISTS (
  SELECT * 
    FROM INFORMATION_SCHEMA.ROUTINES 
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'fnRankToPoints' 
)
   DROP function dbo.fnRankToPoints
GO


CREATE FUNCTION [dbo].[fnRankToPoints]
(
	@RealmID int
	, @ranking int
)
RETURNS int
AS
BEGIN
	declare @Ret int

	-- formula for season 2 
	--(1 = 50, 2 = 45, 3 = 40, 4 = 35, 5 = 30 6-10 = 20, 11-15 = 15, 16-20 = 10, 21-25 = 5, 26+ = 1)
	IF @ranking between 1 and 5 begin
		set @Ret = 50 - 5*(@ranking-1)
	end ELSE if @ranking between 6 and 10 begin
		set @Ret= 20
	end ELSE if @ranking between 11 and 15 begin
		set @Ret= 15
	end ELSE if @ranking between 16 and 20 begin
		set @Ret= 10
	end ELSE if @ranking between 21 and 25 begin
		set @Ret= 5
	end else begin
		set @Ret= 1
	end


	/*
	this was the formulat we used in season  2 
	IF @RealmID = 0 BEGIN
		--(1 = 50, 2 = 45, 3 = 40, 4 = 35, 5 = 30 6-10 = 20, 11-15 = 15, 16-20 = 10, 21-25 = 5, 26+ = 1)
		IF @ranking between 1 and 5 begin
			set @Ret = 50 - 5*(@ranking-1)
		end ELSE if @ranking between 6 and 10 begin
			set @Ret= 20
		end ELSE if @ranking between 11 and 15 begin
			set @Ret= 15
		end ELSE if @ranking between 16 and 20 begin
			set @Ret= 10
		end ELSE if @ranking between 21 and 25 begin
			set @Ret= 5
		end else begin
			set @Ret= 1
		end
	END else IF @RealmID = 13 or @Realmid = 21 BEGIN
		-- (1 = 100, 2 = 90, 3 = 80, 4 = 70, 5 = 60 6-10 = 40, 11-15 = 30, 16-20 = 20, 21-25 = 10, 26+ = 5)
		IF @ranking between 1 and 5 begin
			set @Ret = 100 - 10*(@ranking-1)
		end ELSE if @ranking between 6 and 10 begin
			set @Ret= 40
		end ELSE if @ranking between 11 and 15 begin
			set @Ret= 30
		end ELSE if @ranking between 16 and 20 begin
			set @Ret= 20
		end ELSE if @ranking between 21 and 25 begin
			set @Ret= 10
		end else begin
			set @Ret= 5
		end
	END
	*/ 
	RETURN @Ret
END
go
