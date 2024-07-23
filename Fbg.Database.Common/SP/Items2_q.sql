    
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'Items2_q')
	BEGIN
		DROP  Procedure  Items2_q
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE Procedure [dbo].Items2_q
		@user as uniqueidentifier
		, @ItemID bigint = nulll

AS

begin try 

	select I.ItemID, PlayerID, ExpiresOn 
		, ID.PfPackageID, ID.DurationInMinutes, IT.UnitTypeID, IT.Amount,
		ISi.Silver, Ibs.MinutesAmount, Irs.MinutesAmount as ResearchSpeedUpMinutesAmount
		from Items I
		left join Items_PFWithDuration ID
			on I.ItemID = ID.ItemID
		left join Items_Troops IT
			on I.ItemID = IT.ItemID
		left join Items_Silver ISi
			on I.ItemID = ISi.ItemID
		left join Items_BuildingSpeedup Ibs
			on I.ItemID = Ibs.ItemID
		left join Items_ResearchSpeedup Irs
			on I.ItemID = Irs.ItemID
		where 
				UserID = @user 
			and (@ItemID is null OR  i.ItemID = @ItemID )
			and (ExpiresOn is null OR ExpiresOn >= getdate())
			and (PlayerID is null OR PlayerID in (select PlayerID from players where userid = @user))
			and UsedOn is null
		order by PfPackageID, DurationInMinutes,  IT.UnitTypeID, IT.Amount, ISi.Silver, Ibs.MinutesAmount, Irs.MinutesAmount

end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 

	IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'Items2_q FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		+ '   @@user' + ISNULL(CAST(@user AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	



 