
IF EXISTS (
  SELECT * 
    FROM INFORMATION_SCHEMA.ROUTINES 
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'Translate' 
)
   DROP function dbo.Translate
GO
CREATE FUNCTION Translate
(
	@Key nvarchar(100)
)
RETURNS varchar(MAX)
As  
begin
    return (select Value from Translations where rtrim([key]) = rtrim(@key) 
        and lang = (select attribValue from GameAttributes where AttribID = 24)
        and Theme = (select attribValue from GameAttributes where AttribID = 25))
END
go 