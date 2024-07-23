 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iUserDetails')
	BEGIN
		DROP  Procedure  iUserDetails
	END

GO

set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE PROC iUserDetails
	@UserId                        uniqueidentifier    ,
    @Birthday                      datetime            ,
    @Location_Country              varchar(max)        ,
    @Location_City                 nvarchar(max)       ,
    @Location_State                nvarchar(max)       ,
    @Interests                     nvarchar(max)       ,
    @Relationship_Status           tinyint             ,
    @Religion                      nvarchar(100)       ,
    @SignificantOtherFacebookID    varchar(20)         
as
begin try
	if not exists (select * from UserDetails where UserID=@UserID)and @UserID is not null
		begin--insert part
			
			insert into UserDetails 
				([UserId]
			   ,[Birthday]
			   ,[Location_Country]
			   ,[Location_City]
			   ,[Location_State]
			   ,[Interests]
			   ,[Relationship_Status]
			   ,[Religion]
			   ,[SignificantOtherFacebookID]) 
			values 
			   (@UserId
			   ,@Birthday
			   ,@Location_Country
			   ,@Location_City
			   ,@Location_State
			   ,@Interests
			   ,@Relationship_Status
			   ,@Religion
			   ,@SignificantOtherFacebookID)
		end
	else
		begin--update part
			
			update UserDetails set 
				[Birthday]=  isnull(@Birthday,[Birthday])
			   ,[Location_Country]=isnull(@Location_Country,[Location_Country])
			   ,[Location_City]=isnull(@Location_City,Location_City)
			   ,[Location_State]=isnull(@Location_State,Location_State)
			   ,[Interests]=isnull(@Interests,Interests)
			   ,[Relationship_Status]=isnull(@Relationship_Status,Relationship_Status)
			   ,[Religion]=isnull(@Religion,Religion)
			   ,[SignificantOtherFacebookID]=isnull(@SignificantOtherFacebookID,SignificantOtherFacebookID)
			where UserId=@UserId
		end
end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iUserDetails FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @UserID' + ISNULL(CAST(@UserID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @Birthday' + ISNULL(CAST(@Birthday AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @Location_Country' + ISNULL(CAST(@Location_Country AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @Location_City' + ISNULL(CAST(@Location_City AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @Location_State' + ISNULL(CAST(@Location_State AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @Interests' + ISNULL(CAST(@Interests AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @Relationship_Status' + ISNULL(CAST(@Relationship_Status AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @Religion' + ISNULL(CAST(@Religion AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SignificantOtherFacebookID' + ISNULL(CAST(@SignificantOtherFacebookID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)
end catch