 IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iMyClanDiplomacy')
	BEGIN
		DROP  Procedure  iMyClanDiplomacy
	END

GO
 
 SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[iMyClanDiplomacy]
		
		@ClanID int  ,
		@OtherClanName nvarchar(30),
		@DiplomacyStatus int
		
AS
	
begin try 
		begin tran
			declare @OtherClanID as int;
			declare @EventMessage as nvarchar(max);
			declare @InviterName as nvarchar(25);
			declare @EventTime as datetime;
			set @EventTime=getdate()
			
			SELECT @EventMessage=
			CASE @DiplomacyStatus
			WHEN 0 THEN @OtherClanName+dbo.Translate('iMyClanDip_setAlly')
			WHEN 1 THEN @OtherClanName+dbo.Translate('iMyClanDip_setEnemy')
			WHEN 2 THEN @OtherClanName+dbo.Translate('iMyClanDip_setNAP')
			ELSE dbo.Translate('iMyClanDip_setOther')
			END
			
			set @OtherClanID=0;

			select @OtherClanID=ClanID from Clans where Name=@OtherClanName
				
				if @OtherClanID!=0 and @ClanID!=@OtherClanID begin
					if not exists(select * from ClanDiplomacy where ClanID=@ClanID and OtherClanID=@OtherClanID  )begin
						insert into ClanDiplomacy (ClanID,OtherClanID,StatusID)values (@ClanID,@OtherClanID,@DiplomacyStatus)
						-- handle clan events
						exec iClanEvents @ClanID,@EventTime,@EventMessage
						select 0;--return 0 if Sucess 
					end
					else 
						select 1;--retuen 1 if a diplomacy already exist 
				end
				else
					 select 2;--return 2 if the clan Don't exist or try to add the player clan 
		commit tran
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iMyClanDiplomacy FAILED! ' +  + CHAR(10)
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

 
