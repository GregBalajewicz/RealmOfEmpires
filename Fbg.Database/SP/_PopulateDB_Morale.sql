IF EXISTS (SELECT * FROM sysobjects WHERE type = 'P' AND name = '_PopulateDB_Morale')
BEGIN
	DROP  Procedure  _PopulateDB_Morale
END

GO


CREATE Procedure dbo._PopulateDB_Morale
	@Morale varchar(10) = null -- options 'OFF', '30min', '1h', '2h', 'bonusonly', 'NOOB'

	AS
set nocount on

declare @RealmType varchar(100) -- NOOB, MC, HC, X etc
declare @RealmSubType varchar(100) -- Holiday14d etc
select @RealmType =  attribvalue from RealmAttributes where attribid =2000
select @RealmSubType =  attribvalue from RealmAttributes where attribid =2001
declare @RealmID int
select @RealmID = AttribValue from RealmAttributes where AttribID = 33

/*

this script is generated from a spreadsheet: https://docs.google.com/spreadsheets/d/1I7gDGSHJYo6jLQj9GQAp3OzIz1Zn3VmTCAdcJwE_2Z4/edit#gid=0 

*/


IF @Morale is null BEGIN
	
		SET @Morale = 'NOOB'
	
END 


if @Morale <> 'OFF' BEGIN
	insert into RealmAttributes values (70, 1, 'Morale System - Is Morale system active on this realm ? 1 means yes') 
END 


delete PlayerMoraleEffectsTemplate
	insert into RealmAttributes values (71, 5, 'Morale System - amount to deduce for attacks on real players') 
	insert into RealmAttributes values (72, 2, 'Morale System - amount to deduce for attacks on NPC') 
	insert into RealmAttributes values (73, -20, 'Morale System - min morale') 
	insert into RealmAttributes values (74, 145, 'Morale System - max morale') 
	insert into RealmAttributes values (75, 100, 'Morale System - max normal morale') 
	insert into RealmAttributes values (76, 1, 'Morale System - min normal morale') 
	insert into RealmAttributes values (77, 4.5, 'Morale System - increase per hour') 
	
	INSERT into PlayerMoraleEffectsTemplate values (145,141,1.2,0.5,15,5)
	INSERT into PlayerMoraleEffectsTemplate values (140,131,1.1,0.6,10,2)
	INSERT into PlayerMoraleEffectsTemplate values (130,121,1.07,0.7,7,1.7)
	INSERT into PlayerMoraleEffectsTemplate values (120,111,1.03,0.8,5,1.5)
	INSERT into PlayerMoraleEffectsTemplate values (110,101,1.01,0.9,2,1.1)
	INSERT into PlayerMoraleEffectsTemplate values (100,1,1,0,1,1)
	INSERT into PlayerMoraleEffectsTemplate values (0,-9,0.9,2,0.5,0.5)
	INSERT into PlayerMoraleEffectsTemplate values (-10,-19,0.8,3,0.3,0.3)
	INSERT into PlayerMoraleEffectsTemplate values (-20,-20,0.7,4,0.1,0.2)



IF not exists (select * from PlayerMoraleEffectsTemplate) BEGIN

	insert into RealmAttributes values (71, 5, 'Morale System - amount to deduce for attacks on real players') 
	insert into RealmAttributes values (72, 2, 'Morale System - amount to deduce for attacks on NPC') 
	insert into RealmAttributes values (73, -20, 'Morale System - min morale') 
	insert into RealmAttributes values (74, 145, 'Morale System - max morale') 
	insert into RealmAttributes values (75, 100, 'Morale System - max normal morale') 
	insert into RealmAttributes values (76, 1, 'Morale System - min normal morale') 
	insert into RealmAttributes values (77, 4.5, 'Morale System - increase per hour') 

	INSERT into PlayerMoraleEffectsTemplate values (100,1,1,0,1,1)
END



truncate table PlayerMoraleEffects

declare @m int
select top 1 @m = MoraleMax from PlayerMoraleEffectsTemplate order by MoraleMax desc
while @m >= ( select top 1 MoraleMin from PlayerMoraleEffectsTemplate order by MoraleMax asc) BEGIN
	insert into PlayerMoraleEffects select top 1 
			@m  
			,AttackAdj
			,DesertionAdj
			,CarryCapAdj
			,MoveSpeedAdj	
			from PlayerMoraleEffectsTemplate 
		where MoraleMax >= @m and MoraleMin <= @m
		order by MoraleMax 


	SET @m = @m -1 
END

