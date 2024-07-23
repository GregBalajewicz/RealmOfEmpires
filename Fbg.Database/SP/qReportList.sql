IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qReportList')
	BEGIN
		DROP  Procedure  qReportList
	END

GO

CREATE Procedure qReportList
                  @PlayerID as int
                 ,@ReportTypeID as int
                 ,@InVillageXCord as int
                 ,@InVillageYCord as int
                 ,@SearchTxt as varchar(250)
                 ,@FolderID as int 
                 ,@MaxDaysOld as int 
AS

BEGIN
	begin try

		Declare @Status as int
		Set @Status = 0
		Declare @vID as int
		
		declare @OldestToRetrieve DateTime		
		set @MaxDaysOld = 0 - @MaxDaysOld
		set @OldestToRetrieve = Dateadd(day, @MaxDaysOld, getdate())
	

		if @InVillageXCord is not Null begin     
		   -- Set Village ID
		   Select @vID = V.VillageID from Villages V where V.XCord = @InVillageXCord and V.YCord = @InVillageYCord;

		   if @vID is null begin
    			set @Status = 1    /* to check village name not found */                   
		   end
		end

		SELECT  ISNULL(RA.ForwardedOn, R.Time) AS ReportTime
				 , ISNULL(RA.AlternateSubject, R.Subject) AS Subject
				 , RT.Name as ReportType
				 , RA.IsViewed
				 , RT.ReportTypeID  
				 , isNull(RA.ForwardedByPlayerID,0) as IsForwarded
				 , RA.RecordID
				 , (Select FlagValue from ReportInfoFlag where RecordID = RA.RecordID and FlagID = 1)
				 , (Select FlagValue from ReportInfoFlag where RecordID = RA.RecordID and FlagID = 2)
				 , (Select FlagValue from ReportInfoFlag where RecordID = RA.RecordID and FlagID = 3)
				 , case when BR.AttackerPlayerID = @PlayerID then 'Attack' 
						when BR.DefenderPlayerID = @PlayerID then 'Defend' 
						else 'NoSide' end as WhatSide
		  FROM  Reports R
				join ReportTypes RT
					 on R.ReportTypeID = RT.ReportTypeID
				join ReportAddressees RA
					 on RA.ReportID = R.ReportID
					 and RA.PlayerID = @PlayerID
				left join BattleReports BR
					 on BR.ReportID = R.ReportID
		   where
				-- Check For ReportTypeID
				(R.ReportTypeID = @ReportTypeID or @ReportTypeID is null)

				-- Check For Search String
				and (ISNULL(RA.AlternateSubject, R.Subject) like '%' + @SearchTxt + '%' or @SearchTxt is null)

				-- Check For VillageID
				and ( ( BR.AttackerVillageID = @vID or BR.DefenderVillageID = @vID ) or @vID is null)

				-- select reports in folder unless not specified
				and (
						(@FolderID is null AND RA.FolderID is null) -- folder is not specified, so get all uncategorized messages
						OR (@FolderID is not null AND RA.FolderID = @FolderID) -- folder is specified so get all messages in this folder
					)
					
				-- only X days old
				and ISNULL(RA.ForwardedOn, R.Time) > @OldestToRetrieve
				
			order by ReportTime desc

			Select @Status as Status;

		Update Players set ReportsCheckedOn = getdate(), NewReportIndicator=0 where PlayerID = @PlayerID
     end try
     begin catch
         DECLARE @ERROR_MSG AS VARCHAR(max) 

	     IF @@TRANCOUNT > 0 ROLLBACK
	
	     SET @ERROR_MSG = 'qReportList FAILED! ' +  + CHAR(10)
	     	+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
	     	
	    	+ '   @PlayerID' + ISNULL(CAST(@PlayerID AS VARCHAR(20)), 'Null') + CHAR(10)
	    	+ '   @ReportTypeID' + ISNULL(CAST(@ReportTypeID AS VARCHAR(20)), 'Null') + CHAR(10)
	    	+ '   @InVillageXCord' + ISNULL(CAST(@InVillageXCord AS VARCHAR(20)), 'Null') + CHAR(10)
	    	+ '   @InVillageYCord' + ISNULL(CAST(@InVillageYCord AS VARCHAR(20)), 'Null') + CHAR(10)
	    	+ '   @SearchTxt' + ISNULL(CAST(@SearchTxt AS VARCHAR(20)), 'Null') + CHAR(10)
	    	+ '   @FolderID ' + ISNULL(CAST(@FolderID  AS VARCHAR(20)), 'Null') + CHAR(10)
	    	+ '   @MaxDaysOld' + ISNULL(CAST(@MaxDaysOld AS VARCHAR(20)), 'Null') + CHAR(10)
	    	+ '   @OldestToRetrieve' + ISNULL(CAST(@OldestToRetrieve AS VARCHAR(20)), 'Null') + CHAR(10)
	    	
           	+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
	    	+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
	    	+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
	    	+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
	    	+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
	    	+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	     RAISERROR(@ERROR_MSG,11,1)	

     end catch

End