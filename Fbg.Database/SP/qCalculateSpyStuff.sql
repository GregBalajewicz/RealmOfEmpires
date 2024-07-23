  IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'qCalculateSpyStuff')
	BEGIN
		DROP  Procedure  qCalculateSpyStuff
	END

GO
CREATE Procedure [dbo].qCalculateSpyStuff
	@AttackSpyStrength real
	,@TargetCounterSpyStrength real
	,@SpySuccessChance as real output-- the % chance that the spies are succesful;
	,@SpySuccessChance_AlgA as real output-- the % chance that the spies are succesful - algorithm A number
	,@SpySuccessChance_AlgB as real output-- the % chance that the spies are succesful - algorithm B number
	,@spyIdentityKnownChance as real output
	,@spyAttackVisibleChance as real output

AS
	
declare @AlgVersion smallint
declare @SpyFailureChance real
declare @SpySpyAbilty int

	
begin try 
	SET @AlgVersion = 1 -- default value. 
	SELECT @AlgVersion = cast(AttribValue as smallint) FROM RealmAttributes where AttribID = 4
	SELECT @SpySpyAbilty = SpyAbility from UnitTypes where UnitTypeID = 12 /*12 == spy.*/

	SET @SpySuccessChance_AlgA = @AttackSpyStrength / (@AttackSpyStrength + @TargetCounterSpyStrength)
	SET @SpySuccessChance_AlgB = 1 - power(1 - (@SpySpyAbilty / @TargetCounterSpyStrength), @AttackSpyStrength / @SpySpyAbilty);
	
	IF @AlgVersion = 1 BEGIN	
		SET @SpySuccessChance = @SpySuccessChance_AlgA
				
        SET @SpyFailureChance = 1 - @SpySuccessChance;
        SET @spyIdentityKnownChance = @SpyFailureChance * @SpyFailureChance;
        SET @spyAttackVisibleChance = @SpyFailureChance;		
	END ELSE IF @AlgVersion = 2 BEGIN
		SET @SpySuccessChance = @SpySuccessChance_AlgB
		
		SET @SpyFailureChance = 1 - @SpySuccessChance;
		SET @spyIdentityKnownChance = 0.1 + (0.8 * power(@SpyFailureChance, 0.66666));
		SET @spyAttackVisibleChance = @spyIdentityKnownChance;
	END ELSE BEGIN 
		RAISERROR('@AlgVersion unrecognized',11,1)	
	END 
	

end try

begin catch
	DECLARE @ERROR_MSG AS VARCHAR(max) 
	
	SET @ERROR_MSG = 'qCalculateSpyStuff FAILED! ' +  + CHAR(10)
		+ '   SOME PARAMETERS/VARIABLES:' + CHAR(10)
		
		
		+ '   @AttackSpyStrength' + ISNULL(CAST(@AttackSpyStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @TargetCounterSpyStrength' + ISNULL(CAST(@TargetCounterSpyStrength AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpySuccessChance' + ISNULL(CAST(@SpySuccessChance AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpySuccessChance_AlgA' + ISNULL(CAST(@SpySuccessChance_AlgA AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpySuccessChance_AlgB' + ISNULL(CAST(@SpySuccessChance_AlgB AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @spyIdentityKnownChance' + ISNULL(CAST(@spyIdentityKnownChance AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @spyAttackVisibleChance' + ISNULL(CAST(@spyAttackVisibleChance AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @AlgVersion' + ISNULL(CAST(@AlgVersion AS VARCHAR(max)), 'Null') + CHAR(10)
		+ '   @SpyFailureChance' + ISNULL(CAST(@SpyFailureChance AS VARCHAR(max)), 'Null') + CHAR(10)
		
		
		+ '   ERROR_NUMBER():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_SEVERITY():' + ISNULL(CAST(ERROR_SEVERITY() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_STATE():' + ISNULL(CAST(ERROR_NUMBER() AS VARCHAR(100)), 'Null')+ CHAR(10)
		+ '   ERROR_PROCEDURE():' + ERROR_PROCEDURE()  + CHAR(10)
		+ '   ERROR_LINE():' + ISNULL(CAST(ERROR_LINE() AS VARCHAR(100)), 'Null') + CHAR(10)
		+ '   ERROR_MESSAGE():' +  ERROR_MESSAGE() + CHAR(10)
	RAISERROR(@ERROR_MSG,11,1)	

end catch	 