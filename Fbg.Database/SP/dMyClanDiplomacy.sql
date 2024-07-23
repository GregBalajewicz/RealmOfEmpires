 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'dMyClanDiplomacy')
	BEGIN
		DROP  Procedure  dMyClanDiplomacy
	END

GO
 
 CREATE Procedure [dbo].[dMyClanDiplomacy]
		
		@ClanID int  ,
		@OtherClanName nvarchar(30),
		@DiplomacyStatus int
		
AS
	
begin try 
		begin tran
			declare @OtherClanID as int;
			set @OtherClanID=0;
			
			declare @EventMessage as nvarchar(max);
			declare @EventTime as datetime;
			set @EventTime=getdate()
			
			SELECT @EventMessage= dbo.Translate( 'dMyClanDiplomacy_diploWith') + '''' + @OtherClanName+ '''' + dbo.Translate('dMyClanDiplomacy_endChange')			
			

			select @OtherClanID=ClanID from Clans where Name=@OtherClanName	
			if @OtherClanID!=0  begin
				delete from ClanDiplomacy where ClanID=@ClanID and OtherClanID=@OtherClanID and StatusID=@DiplomacyStatus;
            	IF @@rowcount >= 1 BEGIN
			    	exec iClanEvents @ClanID,@EventTime,@EventMessage
			    END
			end 
		commit tran
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'dMyClanDiplomacy FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @ClanID:' + ISNULL(@ClanID , 'Null') + CHAR(10)
		+ '   @OtherClanName:' + ISNULL(@OtherClanName , 'Null') + CHAR(10)
		+ '   @DiplomacyStatus:' + ISNULL(CAST(@DiplomacyStatus AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	

 
