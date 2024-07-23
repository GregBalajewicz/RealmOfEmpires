
IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = 'uCreditFarm_PlayerModifier')
	BEGIN
		DROP  Procedure  uCreditFarm_PlayerModifier
	END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--
-- Credit Farm
--
create Procedure [dbo].uCreditFarm_PlayerModifier
	@PrintDebugInfo BIT = null
as

DECLARE @DEBUG INT, @DBG varchar(10)
IF ((@PrintDebugInfo = 1) OR (0 = 1)) BEGIN
	SET @DEBUG = 1
	set @DBG = ':::::::'
	SET NOCOUNT OFF
END ELSE BEGIN
	SET @DEBUG = 0
	SET NOCOUNT ON
END

--
-- only run on a realm that has servant farming enabled
--
IF (not EXISTS (SELECT * FROM RealmAttributes WHERE attribID = 55 AND AttribValue = '1'))  BEGIN
	return
END 

 
--drop table #adjust
--drop table #adjust2
--go

declare @CMRecruitInSec int

select @CMRecruitInSec = RecruitmentTime /10000 /1000/10 from UnitTypes where UnitTypeID  = 11

--
-- clean up
--
-- since successful farms cut this by 10%, we coudl end up with some stupidly small numbers; remove them if found. 
delete from CreditFarm_PlayerFoodChanceModifierFactor where modifierFactor < 0.01


select AttackerPID, avg(FoodChance) FC ,count(*) NumAttacks, avg(FoodChance) + isnull((1 - avg(FoodChance)) * modifierFactor, 0) as FoodChanceWithModifier into #adjust
	from CreditFarmLogTable 
	left join CreditFarm_PlayerFoodChanceModifierFactor FM
		on FM.playerid = AttackerPID		
	where Date > DATEADD(minute, -180, GETDATE()) 
	and ID not in 
		(
			select ID from  CreditFarmLogTable CL  where FoodNow < FoodCap 
			and exists 
				(
				select * from CreditFarmLogTable CL2 where CL2.AttackerPID = CL.AttackerPID and CL2.TargetVID = CL.TargetVID 
					and (
							( dateadd(second, @CMRecruitInSec*CL.FoodCap, CL.[Date]) > CL2.[Date] and CL.[Date] < CL2.Date )
							or
							( dateadd(second, -(@CMRecruitInSec*CL.FoodCap), CL.[Date]) < CL2.[Date] and CL.[Date] > CL2.Date )
						)
				)
			and Date > DATEADD(minute, -180, GETDATE())
			--and  AttackerPID = 1485310
			-- order by CL.[Date] desc
		)

	group by AttackerPID ,modifierFactor
	having 
	-- we only look at attacks, where the attacker had food chance less then 95% with any modifier this player may already have. 
	avg(FoodChance) + isnull((1 - avg(FoodChance)) * modifierFactor, 0) < 0.95 
	
IF @DEBUG = 1 select * from #adjust  order by AttackerPID


select *, 1- FoodChanceWithModifier as DiffFrom1, (1- FoodChanceWithModifier) * dbo.fbGetMinReal((cast(NumAttacks as real)/3.0),1) modifierAdjustment 
into #adjust2
from #adjust 
where 1- FoodChanceWithModifier > 0.05 -- this is redundant since we alreay check for this in previous select but adding it here just case something changes about the select above

IF @DEBUG = 1 select * from #adjust2  order by AttackerPID

IF @DEBUG = 1 select * from CreditFarm_PlayerFoodChanceModifierFactor where playerid in (select AttackerPID from #adjust2) 
--
update CreditFarm_PlayerFoodChanceModifierFactor set modifierFactor = dbo.fbGetMinReal(0.9, modifierFactor + modifierAdjustment)
from #adjust2 where  CreditFarm_PlayerFoodChanceModifierFactor.playerid = #adjust2.AttackerPID and modifierFactor < 0.9

IF @DEBUG = 1 select * from CreditFarm_PlayerFoodChanceModifierFactor where playerid in (select AttackerPID from #adjust2) 

insert into CreditFarm_PlayerFoodChanceModifierFactor select AttackerPID, modifierAdjustment from #adjust2 where AttackerPID not in (select playerid from CreditFarm_PlayerFoodChanceModifierFactor)

IF @DEBUG = 1 select * from CreditFarm_PlayerFoodChanceModifierFactor where playerid in (select AttackerPID from #adjust2) 
