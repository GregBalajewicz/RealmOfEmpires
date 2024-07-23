IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'iClanJoin')
	BEGIN
		DROP  Procedure  iClanJoin
	END

GO
CREATE Procedure [dbo].[iClanJoin]
		@PlayerID int,
		@ClanID int
		
AS
	
begin try 
	BEGIN TRAN 
		declare @EventMessage as nvarchar(max);
		declare @PlayerName as nvarchar(25);
		declare @EventTime as datetime;
		
		if exists(select * from Clans where ClanID=@ClanID) begin
			declare @ClanLimit as int
			
			select @ClanLimit = AttribValue from RealmAttributes where AttribID = 22
			
			if @ClanLimit > 0 begin
				if (select count(*) from ClanMembers where ClanID = @ClanID) = @ClanLimit begin
					ROLLBACK
					select 3; -- return 3 if clan limit
					RETURN
				end
			end
			
			--delete invitation from table 
			delete 
			from ClanInvites 
			where PlayerID=@PlayerID and ClanID=@ClanID
				and datediff(day,getdate(),dateadd(day,10,ClanInvites.InvitedOn))>0
				
			IF @@rowcount <> 1 BEGIN
				ROLLBACK
				select 2; -- return 2 if player don't have invitation to this clan
				RETURN
			END
			-- insert player in clan
			Insert into ClanMembers(ClanID,PlayerID)           
			values(@ClanID,@PlayerID)     
			
			-- handle clan events
			select @PlayerName=Name from Players where PlayerID=@PlayerID;
			set @EventMessage=@PlayerName+dbo.Translate('iClanJoin_joined');
			set @EventTime=getdate()
			exec iClanEvents @ClanID,@EventTime,@EventMessage
			
			-- give the player the default role
			INSERT INTO PlayerInRoles (PlayerID, RoleID, ClanID) 
				SELECT @PlayerID, RoleID, @ClanID
				FROM DefaultRoles DF
					WHERE DF.ClanID = @ClanID
			
			
			select 0;-- return 0 if sucess
		end	else begin
			select 1;--return 1 if clan don't exists
		end	


				
	COMMIT TRAN	
end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(8000) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'iClanJoin FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @ClanID' + ISNULL(CAST(@ClanID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	
 