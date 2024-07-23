    IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uPoints_Player')
	BEGIN
		DROP  Procedure  uPoints_Player
	END

GO

CREATE Procedure dbo.uPoints_Player
	@PlayerID as int
AS
declare @IsNestedTransaction Bit
--
-- Is this SP called by some other SP, as part of a nested transaction?
--
IF @@TRANCOUNT > 0 BEGIN 
	set @IsNestedTransaction = 1 --Yes!
END ELSE BEGIN
	set @IsNestedTransaction = 0 --no
END

IF exists (select * from specialplayers where playerid = @PlayerID) BEGIN 
	return
END


__RETRY__:
begin try 
	
	--
	-- if not villages, then player gets 0 points
	--
	update Players set Points = 
		isnull((
			select sum(points)
			from villages v WITH (NOLOCK)
			where V.OwnerPlayerID  = @playerID
		),0)
	where PlayerID  = @PlayerID

end try
begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max)
    IF @@TRANCOUNT > 0 ROLLBACK

	
	SET @ERROR_MSG = 'uPoints_Player FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		+ '   @PlayerID'			+ ISNULL(CAST(@PlayerID AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   ERROR_NUMBER():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():'	+ ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():'		+ ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():'	+ ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():'		+ ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():'		+  ERROR_MESSAGE() + CHAR(10)
		
	
	--
	-- IF deadlock, rerun
	--
	IF ERROR_NUMBER() = 1205 -- deadlock occured in this SP 
		OR (ERROR_NUMBER() = 50000 AND left(ERROR_MESSAGE(), 8) = 'DEADLOCK' ) -- deadlock occured in some called SP 
	BEGIN
		IF @IsNestedTransaction = 1 BEGIN 
			--
			-- Nested tran. no rerurn, just let caller know what happend. 
			-- 
			INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock, no rerun', @ERROR_MSG)		
			SET @ERROR_MSG = 'DEADLOCK  -- ' + @ERROR_MSG 		
			RAISERROR(@ERROR_MSG,11,1)	
		END ELSE BEGIN 
			--
			-- NOT Nested tran. rerurn
			-- 
			INSERT INTO ErrorLog VALUES (getdate(), ERROR_NUMBER(), 'deadlock', @ERROR_MSG)		
			WAITFOR DELAY '00:00:05'
			GOTO __RETRY__		
		END
	END 		
		
	RAISERROR(@ERROR_MSG,11,1)	
end catch	


