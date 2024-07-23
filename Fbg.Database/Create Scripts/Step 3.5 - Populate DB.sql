use fbg1

insert into fbgcommon..realms values (1, '1', 'Ancient Glory',1,1,1, 'Data Source=localhost;Initial Catalog=fbg1;Integrated Security=True;Max Pool Size=600;', 1000000, '2011-4-15 13:00:00', 0, 1, '', null )

declare @RealmID int
declare @OpenOn DateTime
declare @EndsOn DateTime
declare @IsProductionRun bit 
set @OpenOn = getdate()
set @RealmID = 1

-- figure out if we are on prod or not
set @IsProductionRun = 0 



use fbg1
exec _PopulateDB_Translations
exec _PopulateDB 
	1				-- @IsP2Prealm bit
	, @IsProductionRun	-- ,@IsProductionRun bit
	, @RealmID		-- ,@RealmID int
	, 'NOOB'		-- ,@RealmType varchar(100) -- NOOB, MC, HC, X, 6M
	, ''			-- ,@RealmXType varchar(100) -- Holiday14d
	, 0			-- ,@IsMobileOnlyRealm bit
	, 1000			-- ,@MaxGiftPerDay int	
	, 0

exec _PopulateDB_Quests
exec _PopulateDB_Research
exec _PopulateDB_StartingLevels
exec _PopulateDB_OpenRealm @RealmID
	, '1'
	, 'My realm'
	, ''
	, 1
	, @OpenOn
	, @EndsOn



