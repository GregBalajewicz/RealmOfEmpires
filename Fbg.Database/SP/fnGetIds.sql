IF OBJECT_ID (N'dbo.fnGetIds') IS NOT NULL
   DROP FUNCTION dbo.fnGetIds
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION fnGetIds
(
	@strIds varchar(max)
)
RETURNS @tblId Table (IndexValue int PRIMARY KEY IDENTITY(1,1),ID bigInt)
As  
begin

   Set @strIds =  @strIds + ',' 

   Declare @Pos1 Int
   Declare @pos2 Int
 
   Set @Pos1=1
   Set @Pos2=1

   While @Pos1<Len(@strIds)
     Begin
        Set @Pos1 = CharIndex(',',@strIds,@Pos1)
        Insert @tblId Select  Cast(Substring(@strIds,@Pos2,@Pos1-@Pos2) As bigInt)
 
        Set @Pos2=@Pos1+1
 
        Set @Pos1 = @Pos1+1
     End 
   Return
End