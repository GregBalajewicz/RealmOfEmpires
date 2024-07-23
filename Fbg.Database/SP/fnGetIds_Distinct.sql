IF OBJECT_ID (N'dbo.fnGetIds_Distinct') IS NOT NULL
   DROP FUNCTION dbo.fnGetIds_Distinct
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION fnGetIds_Distinct
(
	@strIds varchar(max)
)
RETURNS @tblId Table (IndexValue int IDENTITY(1,1),ID bigInt PRIMARY KEY )
As  
begin

   Set @strIds =  @strIds + ',' 

   Declare @Pos1 Int
   Declare @pos2 Int
   declare @ID bigint

   Set @Pos1=1
   Set @Pos2=1

   While @Pos1<Len(@strIds)
     Begin
        Set @Pos1 = CharIndex(',',@strIds,@Pos1)
        
        SET @ID = Cast(Substring(@strIds,@Pos2,@Pos1-@Pos2) As bigInt)
        IF not exists(select * from @tblId where ID = @ID) BEGIN

            Insert @tblId Select  @ID
 
        END 
        Set @Pos2=@Pos1+1
 
        Set @Pos1 = @Pos1+1
     End 
   Return
End 