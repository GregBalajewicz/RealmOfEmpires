 IF OBJECT_ID (N'dbo.fnGetStringIDs') IS NOT NULL
   DROP FUNCTION dbo.fnGetStringIDs
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

--
-- returns a list of UNIQUE strings from the passed in string. if the same string ID is in the string more than once, the return
--  table will only have it once
--
-- MAX length of each STRING ID is 50 CHARs
--
CREATE FUNCTION fnGetStringIDs
(
	@strIds varchar(max)
)
RETURNS @tblId Table (IIndexValue int IDENTITY(1,1), ID varchar(50) PRIMARY KEY)
As  
begin

   Set @strIds =  @strIds + ',' 

   Declare @Pos1 Int
   Declare @pos2 Int
   declare @StrID varchar(50)
 
   Set @Pos1=1
   Set @Pos2=1

   While @Pos1<Len(@strIds)
     Begin
        Set @Pos1 = CharIndex(',',@strIds,@Pos1)
        
        SET @StrID = Substring(@strIds,@Pos2,@Pos1-@Pos2) 
        IF not exists(select * from @tblId where ID = @StrID) BEGIN
            Insert @tblId Select  (@StrID)
        END 
 
        Set @Pos2=@Pos1+1 
        Set @Pos1 = @Pos1+1
     End 
   Return
End