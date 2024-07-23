IF OBJECT_ID (N'dbo.fnGetPlayerRolesFromSecurityLevel') IS NOT NULL
   DROP FUNCTION dbo.fnGetPlayerRolesFromSecurityLevel
GO

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
GO

create FUNCTION [dbo].[fnGetPlayerRolesFromSecurityLevel]
(
	@SecurityLevel tinyint
)
RETURNS @tblId Table (ID Int)
As  
begin

	/*

		DEPRECIATED!!! 

		DO NOT USE ANYMORE. 

		Instead, just do "select roleID from SecurityLevelToRoles where SecurityLevel = @SecurityLevel"

	*/


	insert into @tblId select roleID from SecurityLevelToRoles where SecurityLevel = @SecurityLevel


	--if @SecurityLevel=0--All Members
	--	begin 
	--		insert  @tblId  Select 0 as int 
	--		insert  @tblId  Select 2 as int 
	--		insert  @tblId  Select 3 as int 
	--		insert  @tblId  Select 4 as int 
	--		insert  @tblId  Select 5 as int 
			
	--	end
	--else if @SecurityLevel=1--Only Owners and Admins			
	--	begin 
	--		insert @tblId  Select 0 as int  
	--		insert @tblId  Select 3 as int  
	--	end
	--else if @SecurityLevel=2--Only Owners, Admins & Diplomats		
	--	begin 
	--		insert @tblId  Select 0 as int 
	--		insert @tblId  Select 3 as int 
	--		insert @tblId  Select 5 as int 
	--	end
	--else if @SecurityLevel=3--Only Owners, Admins, Diplomats & Forum Admins
	--	begin 
			
	--		insert  @tblId  Select 0 as int
	--		insert  @tblId  Select 3 as int
	--		insert  @tblId  Select 4 as int
	--		insert  @tblId  Select 5 as int
	--	end
	--else if @SecurityLevel=4-- Only Owners, Admins, Diplomats, Forum Admins & Inviters
	--	begin 
	--		insert  @tblId  Select 0 as int
	--		insert  @tblId  Select 2 as int
	--		insert  @tblId  Select 3 as int
	--		insert  @tblId  Select 4 as int
	--		insert  @tblId  Select 5 as int
	--	end


		Return;
End

 