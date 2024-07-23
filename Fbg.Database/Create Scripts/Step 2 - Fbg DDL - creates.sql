use fbg1
go
/* 
 * TABLE: AccountStewardLog 
 */

CREATE TABLE AccountStewardLog(
    ActingPlayerID    int             NOT NULL,
    EventTypeID       smallint        NOT NULL,
    Time              datetime        NOT NULL,
    Notes             varchar(max)    NULL
)
go



IF OBJECT_ID('AccountStewardLog') IS NOT NULL
    PRINT '<<< CREATED TABLE AccountStewardLog >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE AccountStewardLog >>>'
go

/* 
 * TABLE: AccountStewards 
 */

CREATE TABLE AccountStewards(
    PlayerID           int         NOT NULL,
    StewardPlayerID    int         NOT NULL,
    RecordID           int         IDENTITY(1,1),
    State              smallint    DEFAULT 1 NOT NULL,
    CONSTRAINT PK227 PRIMARY KEY NONCLUSTERED (PlayerID, StewardPlayerID)
)
go



IF OBJECT_ID('AccountStewards') IS NOT NULL
    PRINT '<<< CREATED TABLE AccountStewards >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE AccountStewards >>>'
go

/* 
 * TABLE: admin_attackLog 
 */

CREATE TABLE admin_attackLog(
    eventID                 int             NULL,
    attackerPID             int             NOT NULL,
    attackerVID             int             NOT NULL,
    defenderPID             int             NOT NULL,
    defenderVID             int             NOT NULL,
    launchTime              datetime        NOT NULL,
    landTime                datetime        NULL,
    attackTroops            varchar(max)    NULL,
    attackTroops_isGov      bit             NULL,
    defenderTroops          varchar(max)    NULL,
    loyalty_beforeAttack    smallint        NULL,
    loyalty_change          smallint        NULL,
    supportDetailedInfo     varchar(max)    NULL,
    AttackStrength          int             NULL,
    CreditFarmed            int             NULL,
    morale					int             NULL
)
go



IF OBJECT_ID('admin_attackLog') IS NOT NULL
    PRINT '<<< CREATED TABLE admin_attackLog >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE admin_attackLog >>>'
go

/* 
 * TABLE: BattleReportBuildingIntel 
 */

CREATE TABLE BattleReportBuildingIntel(
    ReportID          bigint    NOT NULL,
    BuildingTypeID    int       NOT NULL,
    Level             int       NOT NULL,
    CONSTRAINT PK114 PRIMARY KEY NONCLUSTERED (ReportID, BuildingTypeID, Level)
)
go



IF OBJECT_ID('BattleReportBuildingIntel') IS NOT NULL
    PRINT '<<< CREATED TABLE BattleReportBuildingIntel >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE BattleReportBuildingIntel >>>'
go

/* 
 * TABLE: BattleReportBuildings 
 */

CREATE TABLE BattleReportBuildings(
    ReportID             bigint    NOT NULL,
    BuildingTypeID       int       NOT NULL,
    BeforeAttackLevel    int       NOT NULL,
    AfterAttackLevel     int       NOT NULL,
    CONSTRAINT PK80 PRIMARY KEY NONCLUSTERED (ReportID, BuildingTypeID)
)
go



IF OBJECT_ID('BattleReportBuildings') IS NOT NULL
    PRINT '<<< CREATED TABLE BattleReportBuildings >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE BattleReportBuildings >>>'
go

/* 
 * TABLE: BattleReports 
 */

CREATE TABLE BattleReports(
    ReportID                          bigint      NOT NULL,
    AttackerVillageID                 int         NOT NULL,
    DefenderVillageID                 int         NOT NULL,
    AttackerPlayerID                  int         NOT NULL,
    DefenderPlayerID                  int         NOT NULL,
    AttackersLoot                     int         NOT NULL,
    LoyaltyBeforeAttack               int         NULL,
    LoyaltyChange                     int         NULL,
    CanAttackerSeeDefTroops           smallint    DEFAULT 0 NOT NULL,
    DefendersCoins                    int         NOT NULL,
    DefenderKnowsAttackersIdentity    smallint    NOT NULL,
    SpyOutcome                        smallint    NOT NULL,
    SpySuccessChance                  real        NULL,
    CONSTRAINT PK59 PRIMARY KEY NONCLUSTERED (ReportID)
)
go



IF OBJECT_ID('BattleReports') IS NOT NULL
    PRINT '<<< CREATED TABLE BattleReports >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE BattleReports >>>'
go

/* 
 * TABLE: BattleReportSupport 
 */

CREATE TABLE BattleReportSupport(
    ReportID               bigint      NOT NULL,
    SupportingVillageID    int         NOT NULL,
    SupportingPlayerID     int         NOT NULL,
    ViewAccessLevel        smallint    NOT NULL,
    CONSTRAINT PK83 PRIMARY KEY NONCLUSTERED (ReportID, SupportingVillageID)
)
go



IF OBJECT_ID('BattleReportSupport') IS NOT NULL
    PRINT '<<< CREATED TABLE BattleReportSupport >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE BattleReportSupport >>>'
go

/* 
 * TABLE: BattleReportSupportUnits 
 */

CREATE TABLE BattleReportSupportUnits(
    ReportID               bigint    NOT NULL,
    UnitTypeID             int       NOT NULL,
    DeployedUnitCount      int       NOT NULL,
    KilledUnitCount        int       NOT NULL,
    ReaminingUnitCount     int       NOT NULL,
    SupportingVillageID    int       NOT NULL,
    CONSTRAINT PK61_1 PRIMARY KEY NONCLUSTERED (ReportID, UnitTypeID, SupportingVillageID)
)
go



IF OBJECT_ID('BattleReportSupportUnits') IS NOT NULL
    PRINT '<<< CREATED TABLE BattleReportSupportUnits >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE BattleReportSupportUnits >>>'
go

/* 
 * TABLE: BattleReportUnits 
 */

CREATE TABLE BattleReportUnits(
    ReportID              bigint      NOT NULL,
    Party                 smallint    NOT NULL,
    UnitTypeID            int         NOT NULL,
    DeployedUnitCount     int         NOT NULL,
    KilledUnitCount       int         NOT NULL,
    ReaminingUnitCount    int         NOT NULL,
    CONSTRAINT PK61 PRIMARY KEY NONCLUSTERED (ReportID, Party, UnitTypeID)
)
go



IF OBJECT_ID('BattleReportUnits') IS NOT NULL
    PRINT '<<< CREATED TABLE BattleReportUnits >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE BattleReportUnits >>>'
go

/* 
 * TABLE: BuildingDowngradeQEntries 
 */

CREATE TABLE BuildingDowngradeQEntries(
    QEntryID          bigint      IDENTITY(1,1),
    VillageID         int         NOT NULL,
    BuildingTypeID    int         NOT NULL,
    DateAdded         datetime    NOT NULL,
    CONSTRAINT PK103_1 PRIMARY KEY NONCLUSTERED (QEntryID)
)
go



IF OBJECT_ID('BuildingDowngradeQEntries') IS NOT NULL
    PRINT '<<< CREATED TABLE BuildingDowngradeQEntries >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE BuildingDowngradeQEntries >>>'
go

/* 
 * TABLE: BuildingDowngrades 
 */

CREATE TABLE BuildingDowngrades(
    EventID             bigint      NOT NULL,
    VillageID           int         NOT NULL,
    BuildingTypeID      int         NOT NULL,
    OriginalDuration    bigint      NOT NULL,
    InitiatedOn         datetime    NOT NULL,
    CONSTRAINT PK19_1 PRIMARY KEY NONCLUSTERED (EventID)
)
go



IF OBJECT_ID('BuildingDowngrades') IS NOT NULL
    PRINT '<<< CREATED TABLE BuildingDowngrades >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE BuildingDowngrades >>>'
go

/* 
 * TABLE: BuildingLevels 
 */

CREATE TABLE BuildingLevels(
    BuildingTypeID             int             NOT NULL,
    Level                      int             NOT NULL,
    Cost                       int             NOT NULL,
    BuildTime                  bigint          NOT NULL,
    CumulativeBuildTime        bigint          NOT NULL,
    LevelName                  nvarchar(50)    NULL,
    Points                     int             NOT NULL,
    Population                 int             NOT NULL,
    CumulativePopulation       int             NOT NULL,
    LevelStrength              int             NOT NULL,
    CumulativeLevelStrength    int             NOT NULL,
    CONSTRAINT PK10 PRIMARY KEY NONCLUSTERED (BuildingTypeID, Level)
)
go



IF OBJECT_ID('BuildingLevels') IS NOT NULL
    PRINT '<<< CREATED TABLE BuildingLevels >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE BuildingLevels >>>'
go

/* 
 * TABLE: Buildings 
 */

CREATE TABLE Buildings(
    VillageID         int    NOT NULL,
    BuildingTypeID    int    NOT NULL,
    Level             int    NOT NULL,
    CONSTRAINT PK11 PRIMARY KEY NONCLUSTERED (VillageID, BuildingTypeID)
)
go



IF OBJECT_ID('Buildings') IS NOT NULL
    PRINT '<<< CREATED TABLE Buildings >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Buildings >>>'
go

/* 
 * TABLE: BuildingTypeRequirements 
 */

CREATE TABLE BuildingTypeRequirements(
    BuildingTypeID            int    NULL,
    Level                     int    NULL,
    RequiredBuildingTypeID    int    NOT NULL,
    RequiredLevel             int    NOT NULL
)
go



IF OBJECT_ID('BuildingTypeRequirements') IS NOT NULL
    PRINT '<<< CREATED TABLE BuildingTypeRequirements >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE BuildingTypeRequirements >>>'
go

/* 
 * TABLE: BuildingTypes 
 */

CREATE TABLE BuildingTypes(
    BuildingTypeID         int              NOT NULL,
    Name                   nvarchar(100)    NOT NULL,
    Sort                   int              NOT NULL,
    MinimumLevelAllowed    smallint         NOT NULL,
    Description            nvarchar(max)    NOT NULL,
    CONSTRAINT PK_BuildingTypes PRIMARY KEY CLUSTERED (BuildingTypeID)
)
go



IF OBJECT_ID('BuildingTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE BuildingTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE BuildingTypes >>>'
go

/* 
 * TABLE: BuildingUpgradeQEntries 
 */

CREATE TABLE BuildingUpgradeQEntries(
    QEntryID          bigint      IDENTITY(1,1),
    VillageID         int         NULL,
    BuildingTypeID    int         NULL,
    DateAdded         datetime    NOT NULL,
    CONSTRAINT PK103 PRIMARY KEY NONCLUSTERED (QEntryID)
)
go



IF OBJECT_ID('BuildingUpgradeQEntries') IS NOT NULL
    PRINT '<<< CREATED TABLE BuildingUpgradeQEntries >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE BuildingUpgradeQEntries >>>'
go

/* 
 * TABLE: BuildingUpgrades 
 */

CREATE TABLE BuildingUpgrades(
    VillageID         int       NOT NULL,
    BuildingTypeID    int       NOT NULL,
    Level             int       NOT NULL,
    EventID           bigint    NOT NULL,
    CONSTRAINT PK19 PRIMARY KEY NONCLUSTERED (VillageID, EventID)
)
go



IF OBJECT_ID('BuildingUpgrades') IS NOT NULL
    PRINT '<<< CREATED TABLE BuildingUpgrades >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE BuildingUpgrades >>>'
go

/* 
 * TABLE: CapitalVillages 
 */

CREATE TABLE CapitalVillages(
    VillageID    int    NOT NULL,
    CONSTRAINT PK305 PRIMARY KEY NONCLUSTERED (VillageID)
)
go



IF OBJECT_ID('CapitalVillages') IS NOT NULL
    PRINT '<<< CREATED TABLE CapitalVillages >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE CapitalVillages >>>'
go

/* 
 * TABLE: Chat 
 */

CREATE TABLE Chat(
    PlayerID    int              NOT NULL,
    ClanID      int              NULL,
    Time        datetime         DEFAULT getdate() NOT NULL,
    Msg         nvarchar(max)    NOT NULL
)
go



IF OBJECT_ID('Chat') IS NOT NULL
    PRINT '<<< CREATED TABLE Chat >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Chat >>>'
go

/* 
 * TABLE: ClanDiplomacy 
 */

CREATE TABLE ClanDiplomacy(
    ClanID         int    NOT NULL,
    OtherClanID    int    NOT NULL,
    StatusID       int    NOT NULL,
    CONSTRAINT PK72 PRIMARY KEY NONCLUSTERED (ClanID, OtherClanID, StatusID)
)
go



IF OBJECT_ID('ClanDiplomacy') IS NOT NULL
    PRINT '<<< CREATED TABLE ClanDiplomacy >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ClanDiplomacy >>>'
go

/* 
 * TABLE: ClanEvents 
 */

CREATE TABLE ClanEvents(
    ClanID     int              NOT NULL,
    Time       datetime         NOT NULL,
    Message    nvarchar(max)    NOT NULL
)
go



IF OBJECT_ID('ClanEvents') IS NOT NULL
    PRINT '<<< CREATED TABLE ClanEvents >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ClanEvents >>>'
go

/* 
 * TABLE: ClanInviteLog 
 */

CREATE TABLE ClanInviteLog(
    ClanID             int         NOT NULL,
    PlayerID           int         NOT NULL,
    InvitedPlayerID    int         NOT NULL,
    InvitedOn          datetime    NOT NULL
)
go



IF OBJECT_ID('ClanInviteLog') IS NOT NULL
    PRINT '<<< CREATED TABLE ClanInviteLog >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ClanInviteLog >>>'
go

/* 
 * TABLE: ClanInvites 
 */

CREATE TABLE ClanInvites(
    ClanID       int         NOT NULL,
    PlayerID     int         NOT NULL,
    InvitedOn    datetime    DEFAULT getdate() NOT NULL,
    CONSTRAINT PK48 PRIMARY KEY NONCLUSTERED (ClanID, PlayerID)
)
go



IF OBJECT_ID('ClanInvites') IS NOT NULL
    PRINT '<<< CREATED TABLE ClanInvites >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ClanInvites >>>'
go

/* 
 * TABLE: ClanMembers 
 */

CREATE TABLE ClanMembers(
    ClanID      int    NOT NULL,
    PlayerID    int    NOT NULL,
    CONSTRAINT PK7 PRIMARY KEY NONCLUSTERED (ClanID, PlayerID)
)
go



IF OBJECT_ID('ClanMembers') IS NOT NULL
    PRINT '<<< CREATED TABLE ClanMembers >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ClanMembers >>>'
go

/* 
 * TABLE: Clans 
 */

CREATE TABLE Clans(
    ClanID            int              IDENTITY(1,1),
    Name              nvarchar(30)     NOT NULL,
    Description       nvarchar(200)    NOT NULL,
    PublicProfile     nvarchar(max)    NOT NULL,
    WelcomeMessage    nvarchar(max)    DEFAULT '' NOT NULL,
    Annocuncement     nvarchar(250)    DEFAULT '' NOT NULL,
    CONSTRAINT PK_Tribes PRIMARY KEY NONCLUSTERED (ClanID)
)
go



IF OBJECT_ID('Clans') IS NOT NULL
    PRINT '<<< CREATED TABLE Clans >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Clans >>>'
go

/* 
 * TABLE: ClanStatHistory 
 */

CREATE TABLE ClanStatHistory(
    ClanID       int         NOT NULL,
    Date         datetime    NOT NULL,
    StatID       smallint    NOT NULL,
    StatValue    int         NOT NULL
)
go



IF OBJECT_ID('ClanStatHistory') IS NOT NULL
    PRINT '<<< CREATED TABLE ClanStatHistory >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ClanStatHistory >>>'
go

/* 
 * TABLE: CoinTransports 
 */

CREATE TABLE CoinTransports(
    EventID                 bigint      NOT NULL,
    OriginVillageID         int         NULL,
    DestinationVillageID    int         NULL,
    Amount                  int         NOT NULL
                            CHECK (Amount >= 0),
    TripDuration            bigint      NOT NULL,
    Direction               smallint    NOT NULL,
    Reserved                bit         NOT NULL,
    CONSTRAINT PK90 PRIMARY KEY NONCLUSTERED (EventID)
)
go



IF OBJECT_ID('CoinTransports') IS NOT NULL
    PRINT '<<< CREATED TABLE CoinTransports >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE CoinTransports >>>'
go

/* 
 * TABLE: CreditFarm_PlayerFoodChanceModifierFactor 
 */

CREATE TABLE CreditFarm_PlayerFoodChanceModifierFactor(
    PlayerID          int     NOT NULL,
    modifierFactor    real    NOT NULL,
    CONSTRAINT PK347 PRIMARY KEY NONCLUSTERED (PlayerID)
)
go



IF OBJECT_ID('CreditFarm_PlayerFoodChanceModifierFactor') IS NOT NULL
    PRINT '<<< CREATED TABLE CreditFarm_PlayerFoodChanceModifierFactor >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE CreditFarm_PlayerFoodChanceModifierFactor >>>'
go

/* 
 * TABLE: CreditFarm_TodaysChance 
 */

CREATE TABLE CreditFarm_TodaysChance(
    CreditsFarmedToday    int     NOT NULL,
    TodaysChance          real    NOT NULL,
    CONSTRAINT PK348 PRIMARY KEY NONCLUSTERED (CreditsFarmedToday)
)
go



IF OBJECT_ID('CreditFarm_TodaysChance') IS NOT NULL
    PRINT '<<< CREATED TABLE CreditFarm_TodaysChance >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE CreditFarm_TodaysChance >>>'
go

/* 
 * TABLE: CreditFarmLogTable 
 */

CREATE TABLE CreditFarmLogTable(
    AttackerPID                  int         NOT NULL,
    AttackerStrength             int         NULL,
    AttackerLoss                 int         NULL,
    FarmedThisAttack             smallint    NULL,
    TodayFarmed                  smallint    NULL,
    TodayMax                     smallint    NULL,
    TodayChance                  real        NULL,
    FoodNow                      smallint    NULL,
    FoodCap                      smallint    NULL,
    FoodChance                   real        NULL,
    TotalChance                  real        NULL,
    Roll                         real        NULL,
    Date                         datetime    NULL,
    TargetVID                    int         NULL,
    TargetVPoints                int         NULL,
    TargetVX                     smallint    NULL,
    TargetVY                     smallint    NULL,
    ID                           bigint      IDENTITY(1,1),
    PlayerFoodChanceModifier     real        NULL,
    PlayerTodayChanceModifier    real        NULL
)
go



IF OBJECT_ID('CreditFarmLogTable') IS NOT NULL
    PRINT '<<< CREATED TABLE CreditFarmLogTable >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE CreditFarmLogTable >>>'
go

/* 
 * TABLE: DefaultRoles 
 */

CREATE TABLE DefaultRoles(
    ClanID    int    NOT NULL,
    RoleID    int    NOT NULL,
    CONSTRAINT PK233 PRIMARY KEY NONCLUSTERED (ClanID, RoleID)
)
go



IF OBJECT_ID('DefaultRoles') IS NOT NULL
    PRINT '<<< CREATED TABLE DefaultRoles >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE DefaultRoles >>>'
go

/* 
 * TABLE: ErrorLog 
 */

CREATE TABLE ErrorLog(
    Time       datetime        NOT NULL,
    Number     int             NULL,
    Message    varchar(max)    NOT NULL,
    Data       varchar(max)    NULL
)
go



IF OBJECT_ID('ErrorLog') IS NOT NULL
    PRINT '<<< CREATED TABLE ErrorLog >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ErrorLog >>>'
go

/* 
 * TABLE: Events 
 */

CREATE TABLE Events(
    EventID      bigint      IDENTITY(1,1),
    EventTime    datetime    NOT NULL,
    Status       int         DEFAULT 0 NOT NULL,
    CONSTRAINT PK20 PRIMARY KEY NONCLUSTERED (EventID)
)
go



IF OBJECT_ID('Events') IS NOT NULL
    PRINT '<<< CREATED TABLE Events >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Events >>>'
go

/* 
 * TABLE: Filters 
 */

CREATE TABLE Filters(
    FilterID       int             IDENTITY(1,1),
    Name           nvarchar(30)    NOT NULL,
    Description    nvarchar(75)    NOT NULL,
    Sort           smallint        NOT NULL,
    PlayerID       int             NOT NULL,
    CONSTRAINT PK_Filters PRIMARY KEY NONCLUSTERED (FilterID)
)
go



IF OBJECT_ID('Filters') IS NOT NULL
    PRINT '<<< CREATED TABLE Filters >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Filters >>>'
go

/* 
 * TABLE: FilterTags 
 */

CREATE TABLE FilterTags(
    FilterID    int    NOT NULL,
    TagID       int    NOT NULL,
    CONSTRAINT PK_FilterTags PRIMARY KEY NONCLUSTERED (FilterID, TagID)
)
go



IF OBJECT_ID('FilterTags') IS NOT NULL
    PRINT '<<< CREATED TABLE FilterTags >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE FilterTags >>>'
go

/* 
 * TABLE: Folders 
 */

CREATE TABLE Folders(
    FolderID     int             IDENTITY(1,1),
    PlayerID     int             NOT NULL,
    Name         nvarchar(30)    NOT NULL,
    FolderFor    smallint        NOT NULL,
    CONSTRAINT PK_Folders PRIMARY KEY NONCLUSTERED (FolderID)
)
go



IF OBJECT_ID('Folders') IS NOT NULL
    PRINT '<<< CREATED TABLE Folders >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Folders >>>'
go

/* 
 * TABLE: ForumSharing 
 */

CREATE TABLE ForumSharing(
    ForumID    int    NOT NULL,
    ClanID     int    NOT NULL,
    CONSTRAINT PK302 PRIMARY KEY NONCLUSTERED (ForumID, ClanID)
)
go



IF OBJECT_ID('ForumSharing') IS NOT NULL
    PRINT '<<< CREATED TABLE ForumSharing >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ForumSharing >>>'
go

/* 
 * TABLE: ForumSharingWhiteListedClans 
 */

CREATE TABLE ForumSharingWhiteListedClans(
    ClanID             int    NOT NULL,
    WhiteListClanID    int    NOT NULL,
    CONSTRAINT PK311 PRIMARY KEY NONCLUSTERED (ClanID, WhiteListClanID)
)
go



IF OBJECT_ID('ForumSharingWhiteListedClans') IS NOT NULL
    PRINT '<<< CREATED TABLE ForumSharingWhiteListedClans >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ForumSharingWhiteListedClans >>>'
go

/* 
 * TABLE: Landmarks 
 */

CREATE TABLE Landmarks(
    XCord                 int    NOT NULL,
    YCord                 int    NOT NULL,
    LandmarkTypePartID    int    NOT NULL,
    CONSTRAINT PK28 PRIMARY KEY CLUSTERED (XCord, YCord)
)
go



IF OBJECT_ID('Landmarks') IS NOT NULL
    PRINT '<<< CREATED TABLE Landmarks >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Landmarks >>>'
go

/* 
 * TABLE: LandmarkTypeParts 
 */

CREATE TABLE LandmarkTypeParts(
    LandmarkTypeID        int             NOT NULL,
    LandmarkTypePartID    int             IDENTITY(1,1),
    ImageUrl              varchar(max)    NOT NULL,
    XCord                 int             NOT NULL,
    YCord                 int             NOT NULL,
	AllowVillage		bit				null
    CONSTRAINT PK95 PRIMARY KEY NONCLUSTERED (LandmarkTypePartID)
)
go



IF OBJECT_ID('LandmarkTypeParts') IS NOT NULL
    PRINT '<<< CREATED TABLE LandmarkTypeParts >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE LandmarkTypeParts >>>'
go

/* 
 * TABLE: LandmarkTypes 
 */

CREATE TABLE LandmarkTypes(
    LandmarkTypeID    int              NOT NULL,
    Name              nvarchar(100)    NOT NULL,
    Chance            int              NOT NULL,
    CheckPosition     bit              NOT NULL,
    ChanceCurrent     int              NULL,
    CONSTRAINT PK27 PRIMARY KEY NONCLUSTERED (LandmarkTypeID)
)
go



IF OBJECT_ID('LandmarkTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE LandmarkTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE LandmarkTypes >>>'
go

/* 
 * TABLE: LevelProperties 
 */

CREATE TABLE LevelProperties(
    BuildingTypeID    int             NOT NULL,
    Level             int             NOT NULL,
    PropertyValue     varchar(100)    NOT NULL,
    PropertyID        int             NOT NULL,
    CONSTRAINT PK23 PRIMARY KEY NONCLUSTERED (BuildingTypeID, Level, PropertyID)
)
go



IF OBJECT_ID('LevelProperties') IS NOT NULL
    PRINT '<<< CREATED TABLE LevelProperties >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE LevelProperties >>>'
go

/* 
 * TABLE: LevelPropertyTypes 
 */

CREATE TABLE LevelPropertyTypes(
    PropertyID    int              NOT NULL,
    Name          nvarchar(100)    NOT NULL,
    Type          int              NOT NULL,
    CONSTRAINT PK24 PRIMARY KEY NONCLUSTERED (PropertyID)
)
go



IF OBJECT_ID('LevelPropertyTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE LevelPropertyTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE LevelPropertyTypes >>>'
go

/* 
 * TABLE: LordUnitTypeCostMultiplier 
 */

CREATE TABLE LordUnitTypeCostMultiplier(
    LordCountStart        int    NOT NULL,
    LordCountStartCost    int    NOT NULL,
    LordCostMultiplier    int    NOT NULL,
    CONSTRAINT PK211 PRIMARY KEY NONCLUSTERED (LordCountStart)
)
go



IF OBJECT_ID('LordUnitTypeCostMultiplier') IS NOT NULL
    PRINT '<<< CREATED TABLE LordUnitTypeCostMultiplier >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE LordUnitTypeCostMultiplier >>>'
go

/* 
 * TABLE: Map 
 */

CREATE TABLE Map(
    IconUrl             varchar(100)    NOT NULL,
    VillageTypeName     varchar(100)    NOT NULL,
    MaxVillagePoints    int             NOT NULL,
    VillageTypeID       smallint        DEFAULT 0 NOT NULL
)
go



IF OBJECT_ID('Map') IS NOT NULL
    PRINT '<<< CREATED TABLE Map >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Map >>>'
go

/* 
 * TABLE: MessageAddressees 
 */

CREATE TABLE MessageAddressees(
    PlayerID     int    NOT NULL,
    Type         bit    NOT NULL,
    MessageID    int    NOT NULL,
    IsViewed     bit    DEFAULT 0 NOT NULL,
    FolderID     int    DEFAULT null NULL,
    RecordID     int    IDENTITY(1,1),
    CONSTRAINT PK107 PRIMARY KEY NONCLUSTERED (RecordID)
)
go



IF OBJECT_ID('MessageAddressees') IS NOT NULL
    PRINT '<<< CREATED TABLE MessageAddressees >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE MessageAddressees >>>'
go

/* 
 * TABLE: Messages 
 */

CREATE TABLE Messages(
    MessageID         int              IDENTITY(1,1),
    PlayerID          int              NOT NULL,
    Subject           nvarchar(90)     NOT NULL,
    Body              nvarchar(max)    NOT NULL,
    TimeSent          datetime         NOT NULL,
    RecipientNames    varchar(max)     NOT NULL,
    CONSTRAINT PK106 PRIMARY KEY NONCLUSTERED (MessageID)
)
go



IF OBJECT_ID('Messages') IS NOT NULL
    PRINT '<<< CREATED TABLE Messages >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Messages >>>'
go

/* 
 * TABLE: MessagesBlockedPlayers 
 */

CREATE TABLE MessagesBlockedPlayers(
    PlayerID           int    NOT NULL,
    BlockedPlayerID    int    NOT NULL,
    CONSTRAINT PK198 PRIMARY KEY NONCLUSTERED (PlayerID, BlockedPlayerID)
)
go



IF OBJECT_ID('MessagesBlockedPlayers') IS NOT NULL
    PRINT '<<< CREATED TABLE MessagesBlockedPlayers >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE MessagesBlockedPlayers >>>'
go

/* 
 * TABLE: NewVillageQ 
 */

CREATE TABLE NewVillageQ(
    NewVillageID        int         IDENTITY(1,1),
    XCord               int         NOT NULL,
    YCord               int         NOT NULL,
    DateTaken           datetime    NULL,
    VillageID           int         NULL,
    NumTimesReturned    int         DEFAULT 0 NOT NULL,
    CONSTRAINT PK241 PRIMARY KEY NONCLUSTERED (NewVillageID)
)
go



IF OBJECT_ID('NewVillageQ') IS NOT NULL
    PRINT '<<< CREATED TABLE NewVillageQ >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE NewVillageQ >>>'
go

/* 
 * TABLE: NotificationSettings_Template 
 */

CREATE TABLE NotificationSettings_Template(
    NotificationID    int              NOT NULL,
    Vibrate           smallint         NOT NULL,
    SoundID           smallint         NOT NULL,
    isActive          smallint         NOT NULL,
    Name              nvarchar(100)    NOT NULL,
    Description       nvarchar(max)    NOT NULL,
    CONSTRAINT PK328_1 PRIMARY KEY NONCLUSTERED (NotificationID)
)
go



IF OBJECT_ID('NotificationSettings_Template') IS NOT NULL
    PRINT '<<< CREATED TABLE NotificationSettings_Template >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE NotificationSettings_Template >>>'
go

/* 
 * TABLE: NoTransportVillages 
 */

CREATE TABLE NoTransportVillages(
    VillageID    int    NOT NULL,
    CONSTRAINT PK195 PRIMARY KEY NONCLUSTERED (VillageID)
)
go



IF OBJECT_ID('NoTransportVillages') IS NOT NULL
    PRINT '<<< CREATED TABLE NoTransportVillages >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE NoTransportVillages >>>'
go

/* 
 * TABLE: PFEventTypes 
 */

CREATE TABLE PFEventTypes(
    EventType      smallint         NOT NULL,
    Name           nvarchar(100)    NOT NULL,
    Description    nvarchar(max)    NOT NULL,
    CONSTRAINT PK239 PRIMARY KEY NONCLUSTERED (EventType)
)
go



IF OBJECT_ID('PFEventTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE PFEventTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PFEventTypes >>>'
go

/* 
 * TABLE: PFPackages 
 */

CREATE TABLE PFPackages(
    PFPackageID    int     NOT NULL,
    Cost           int     NOT NULL,
    Duration       real    NOT NULL,
    CONSTRAINT PK175 PRIMARY KEY NONCLUSTERED (PFPackageID)
)
go



IF OBJECT_ID('PFPackages') IS NOT NULL
    PRINT '<<< CREATED TABLE PFPackages >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PFPackages >>>'
go

/* 
 * TABLE: PFs 
 */

CREATE TABLE PFs(
    FeatureID      int              NOT NULL,
    Description    nvarchar(200)    NOT NULL,
    CONSTRAINT PK99 PRIMARY KEY NONCLUSTERED (FeatureID)
)
go



IF OBJECT_ID('PFs') IS NOT NULL
    PRINT '<<< CREATED TABLE PFs >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PFs >>>'
go

/* 
 * TABLE: PFsInPackage 
 */

CREATE TABLE PFsInPackage(
    PFPackageID    int    NOT NULL,
    FeatureID      int    NOT NULL,
    CONSTRAINT PK176 PRIMARY KEY NONCLUSTERED (PFPackageID, FeatureID)
)
go



IF OBJECT_ID('PFsInPackage') IS NOT NULL
    PRINT '<<< CREATED TABLE PFsInPackage >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PFsInPackage >>>'
go

/* 
 * TABLE: PFTrails 
 */

CREATE TABLE PFTrails(
    PFTrialID      int              NOT NULL,
    FeatureID      int              NOT NULL,
    Description    nvarchar(200)    NOT NULL,
    Duration       int              NOT NULL,
    IsActive       smallint         NOT NULL,
    CONSTRAINT PK188 PRIMARY KEY NONCLUSTERED (PFTrialID)
)
go



IF OBJECT_ID('PFTrails') IS NOT NULL
    PRINT '<<< CREATED TABLE PFTrails >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PFTrails >>>'
go

/* 
 * TABLE: PlayerCacheTimeStamps 
 */

CREATE TABLE PlayerCacheTimeStamps(
    PlayerID        int         NOT NULL,
    CachedItemID    smallint    NOT NULL,
    TimeStamp       datetime    NOT NULL,
    CONSTRAINT PK324 PRIMARY KEY NONCLUSTERED (PlayerID, CachedItemID)
)
go



IF OBJECT_ID('PlayerCacheTimeStamps') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerCacheTimeStamps >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerCacheTimeStamps >>>'
go

/* 
 * TABLE: PlayerFlags 
 */

CREATE TABLE PlayerFlags(
    PlayerID     int             NOT NULL,
    FlagID       smallint        NOT NULL,
    UpdatedOn    datetime        NOT NULL,
    Data         varchar(max)    NULL,
    CONSTRAINT PK124 PRIMARY KEY NONCLUSTERED (PlayerID, FlagID)
)
go



IF OBJECT_ID('PlayerFlags') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerFlags >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerFlags >>>'
go

/* 
 * TABLE: PlayerInRoles 
 */

CREATE TABLE PlayerInRoles(
    PlayerID    int    NOT NULL,
    RoleID      int    NOT NULL,
    ClanID      int    NOT NULL,
    CONSTRAINT PK_aspnet_UserInRoles PRIMARY KEY NONCLUSTERED (PlayerID, RoleID, ClanID)
)
go



IF OBJECT_ID('PlayerInRoles') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerInRoles >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerInRoles >>>'
go

/* 
 * TABLE: PlayerMapEvents 
 */

CREATE TABLE PlayerMapEvents(
    EventID     int             IDENTITY(1,1),
    PlayerID    int             NOT NULL,
    TypeID      int             NOT NULL,
    Data        varchar(256)    NULL,
    AddedOn     datetime        DEFAULT getdate() NOT NULL,
    XCord       int             NOT NULL,
    YCord       int             NOT NULL,
    IsActive    bit             DEFAULT 1 NOT NULL,
    CONSTRAINT PK343 PRIMARY KEY NONCLUSTERED (EventID)
)
go



IF OBJECT_ID('PlayerMapEvents') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerMapEvents >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerMapEvents >>>'
go

/* 
 * TABLE: PlayerNotes 
 */

CREATE TABLE PlayerNotes(
    PlayerID             int              NOT NULL,
    NoteOwnerPlayerID    int              NOT NULL,
    Note                 nvarchar(max)    NOT NULL,
    CONSTRAINT PK110 PRIMARY KEY NONCLUSTERED (PlayerID, NoteOwnerPlayerID)
)
go



IF OBJECT_ID('PlayerNotes') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerNotes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerNotes >>>'
go

/* 
 * TABLE: PlayerNotifications 
 */

CREATE TABLE PlayerNotifications(
    RecordID              bigint           IDENTITY(1,1),
    NotificationTypeID    smallint         NOT NULL,
    PlayerID              int              NOT NULL,
    Text                  nvarchar(max)    NOT NULL,
    TimeCreated           datetime         DEFAULT getdate() NOT NULL,
    TimeSent              datetime         NULL,
    CONSTRAINT PK333 PRIMARY KEY NONCLUSTERED (RecordID)
)
go



IF OBJECT_ID('PlayerNotifications') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerNotifications >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerNotifications >>>'
go

/* 
 * TABLE: PlayerNotificationSettings 
 */

CREATE TABLE PlayerNotificationSettings(
    PlayerID          int         NOT NULL,
    NotificationID    int         NOT NULL,
    Vibrate           smallint    NOT NULL,
    SoundID           smallint    NOT NULL,
    isActive          smallint    NOT NULL,
    CONSTRAINT PK328 PRIMARY KEY NONCLUSTERED (PlayerID, NotificationID)
)
go



IF OBJECT_ID('PlayerNotificationSettings') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerNotificationSettings >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerNotificationSettings >>>'
go

/* 
 * TABLE: PlayerNotificationsNotSent 
 */

CREATE TABLE PlayerNotificationsNotSent(
    NotifyTypeID    int         NOT NULL,
    PlayerID        int         NOT NULL,
    Reason          smallint    DEFAULT 0 NOT NULL,
    Time            datetime    DEFAULT getdate() NULL
)
go



IF OBJECT_ID('PlayerNotificationsNotSent') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerNotificationsNotSent >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerNotificationsNotSent >>>'
go

/* 
 * TABLE: PlayerNotificationsSent 
 */

CREATE TABLE PlayerNotificationsSent(
    NotifyTypeID    int         NOT NULL,
    PlayerID        int         NOT NULL,
    Time            datetime    DEFAULT getdate() NULL
)
go



IF OBJECT_ID('PlayerNotificationsSent') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerNotificationsSent >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerNotificationsSent >>>'
go

/* 
 * TABLE: PlayerPFLog 
 */

CREATE TABLE PlayerPFLog(
    PlayerID     int             NOT NULL,
    Time         datetime        NOT NULL,
    EventType    smallint        NOT NULL,
    Credits      int             NULL,
    Cost         int             NULL,
    Notes        varchar(max)    DEFAULT null NULL
)
go



IF OBJECT_ID('PlayerPFLog') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerPFLog >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerPFLog >>>'
go

/* 
 * TABLE: PlayerPFTrials 
 */

CREATE TABLE PlayerPFTrials(
    PlayerID     int         NOT NULL,
    PFTrialID    int         NOT NULL,
    ExpiresOn    datetime    NOT NULL,
    CONSTRAINT PK191 PRIMARY KEY NONCLUSTERED (PlayerID, PFTrialID)
)
go



IF OBJECT_ID('PlayerPFTrials') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerPFTrials >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerPFTrials >>>'
go

/* 
 * TABLE: PlayerPostViews 
 */

CREATE TABLE PlayerPostViews(
    PlayerID        int    NOT NULL,
    ThreadPostID    int    NOT NULL,
    IsViewed        bit    DEFAULT 1 NOT NULL,
    CONSTRAINT PK85 PRIMARY KEY NONCLUSTERED (PlayerID, ThreadPostID)
)
go



IF OBJECT_ID('PlayerPostViews') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerPostViews >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerPostViews >>>'
go

/* 
 * TABLE: PlayerProfile 
 */

CREATE TABLE PlayerProfile(
    PlayerID    int      NOT NULL,
    Text        ntext    NOT NULL,
    CONSTRAINT PK283 PRIMARY KEY NONCLUSTERED (PlayerID)
)
go



IF OBJECT_ID('PlayerProfile') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerProfile >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerProfile >>>'
go

/* 
 * TABLE: PlayerRecentTargetStack 
 */

CREATE TABLE PlayerRecentTargetStack(
    PlayerID            int         NOT NULL,
    TargetVillageID     int         NOT NULL,
    TargetLastUpdate    datetime    NULL,
    CONSTRAINT PK237 PRIMARY KEY NONCLUSTERED (PlayerID, TargetVillageID)
)
go



IF OBJECT_ID('PlayerRecentTargetStack') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerRecentTargetStack >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerRecentTargetStack >>>'
go

/* 
 * TABLE: PlayerResearchItems 
 */

CREATE TABLE PlayerResearchItems(
    PlayerID              int    NOT NULL,
    ResearchItemTypeID    int    NOT NULL,
    ResearchItemID        int    NOT NULL,
    CONSTRAINT PK260_1 PRIMARY KEY NONCLUSTERED (PlayerID, ResearchItemTypeID, ResearchItemID)
)
go



IF OBJECT_ID('PlayerResearchItems') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerResearchItems >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerResearchItems >>>'
go

/* 
 * TABLE: Players 
 */

CREATE TABLE Players(
    PlayerID                 int                 NOT NULL,
    Name                     nvarchar(25)        NOT NULL,
    ReportsCheckedOn         datetime            DEFAULT getdate() NOT NULL,
    ClanForumCheckedOn       datetime            DEFAULT getdate() NOT NULL,
    MessagesCheckedOn        datetime            DEFAULT getdate() NOT NULL,
    RegisteredOn             datetime            NOT NULL,
    Chests                   int                 NOT NULL
                             CHECK (chests>=0),
    Anonymous                bit                 DEFAULT 0 NOT NULL,
    TitleID                  int                 NOT NULL,
    Sex                      smallint            NOT NULL,
    Points                   int                 DEFAULT 0 NOT NULL,
    NightBuildActivatedOn    datetime            DEFAULT '2007 January 2' NOT NULL,
    NewReportIndicator       bit                 DEFAULT 0 NOT NULL,
    NewMessageIndicator      bit                 DEFAULT 0 NOT NULL,
    PlayerStatus             tinyint             DEFAULT 1 NOT NULL,
    SleepModeActiveFrom      datetime            NULL,
    LastActivity             datetime            DEFAULT getdate() NOT NULL,
    ResearchUpdated          bit                 DEFAULT 1 NOT NULL,
    AvatarID                 smallint            NULL,
    UpdateCompletedQuests    bit                 DEFAULT 1 NOT NULL,
    XP_Cached                int                 DEFAULT 1 NOT NULL,
    UserID                   uniqueidentifier    NOT NULL,
    VacationModeRequestOn    datetime            NULL,
    VacationModeDaysUsed     int                 DEFAULT 0 NOT NULL,
    VacationModeLastEndOn    datetime            NULL,
    Morale		             int             DEFAULT 100 NOT NULL,
    MoraleLastUpdated		 datetime        DEFAULT getdate() NOT NULL,
	WeekendModeTakesEffectOn datetime			null,
	WeekendModeLastEndOn datetime				null

    CONSTRAINT PK_Players PRIMARY KEY CLUSTERED (PlayerID)
)
go



IF OBJECT_ID('Players') IS NOT NULL
    PRINT '<<< CREATED TABLE Players >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Players >>>'
go

/* 
 * TABLE: PlayersFriends 
 */

CREATE TABLE PlayersFriends(
    PlayerID          int    NOT NULL,
    FriendPlayerID    int    NOT NULL,
    CONSTRAINT PK170 PRIMARY KEY NONCLUSTERED (PlayerID, FriendPlayerID)
)
go



IF OBJECT_ID('PlayersFriends') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayersFriends >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayersFriends >>>'
go

/* 
 * TABLE: PlayersPFPackages 
 */

CREATE TABLE PlayersPFPackages(
    PlayerID       int         NOT NULL,
    PFPackageID    int         NOT NULL,
    ExpiresOn      datetime    NOT NULL,
    CONSTRAINT PK100 PRIMARY KEY NONCLUSTERED (PlayerID, PFPackageID)
)
go



IF OBJECT_ID('PlayersPFPackages') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayersPFPackages >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayersPFPackages >>>'
go

/* 
 * TABLE: PlayerStatHistory 
 */

CREATE TABLE PlayerStatHistory(
    PlayerID     int         NOT NULL,
    Date         datetime    NOT NULL,
    StatID       smallint    NOT NULL,
    StatValue    int         NOT NULL
)
go



IF OBJECT_ID('PlayerStatHistory') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerStatHistory >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerStatHistory >>>'
go

/* 
 * TABLE: PlayerStats 
 */

CREATE TABLE PlayerStats(
    PlayerID     int         NOT NULL,
    StatID       smallint    NOT NULL,
    StatValue    bigint      NULL,
    CONSTRAINT PK250 PRIMARY KEY NONCLUSTERED (PlayerID, StatID)
)
go



IF OBJECT_ID('PlayerStats') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerStats >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerStats >>>'
go

/* 
 * TABLE: PlayerSuspensions 
 */

CREATE TABLE PlayerSuspensions(
    PlayerID       int    NOT NULL,
    SupensionID    int    NOT NULL,
    CONSTRAINT PK284 PRIMARY KEY NONCLUSTERED (PlayerID, SupensionID)
)
go



IF OBJECT_ID('PlayerSuspensions') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerSuspensions >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerSuspensions >>>'
go

/* 
 * TABLE: PlayerTitleHistory 
 */

CREATE TABLE PlayerTitleHistory(
    PlayerID      int         NOT NULL,
    TitleID       int         NOT NULL,
    AcceptedOn    datetime    DEFAULT getdate() NOT NULL,
    CONSTRAINT PK210 PRIMARY KEY NONCLUSTERED (PlayerID, TitleID)
)
go



IF OBJECT_ID('PlayerTitleHistory') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerTitleHistory >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerTitleHistory >>>'
go

/* 
 * TABLE: PlayerVillageClaims 
 */

CREATE TABLE PlayerVillageClaims(
    PlayerID       int         NOT NULL,
    ClanID         int         NOT NULL,
    ClaimedVID     int         NOT NULL,
    TimeOfClaim    datetime    NOT NULL,
    CONSTRAINT PK_PlayerVillageClaims PRIMARY KEY NONCLUSTERED (ClanID, ClaimedVID)
)
go


/* 
 * TABLE: PlayerVillageClaims_ClanSetting 

 -- who can see all claim info details (int)
 ----- SettingID : 1. SettingInt == 999 > every clan member, otherwise, minumum role (example 5 means inviter or above, 0 means ower only) 
 -- max number of claims per clan member (int)
 ----- SettingID : 2. 
 -- max claim duration (int, hours) 
 ----- SettingID : 3.
 -- share with allies Y|N (int)
 ----- SettingID : 4.
 -- share with NAP Y|N (int)
 ----- SettingID : 5.
 */

CREATE TABLE PlayerVillageClaims_ClanSetting(
    ClanID         int         NOT NULL,
	SettingID		int		not null,
	SettingInt	int	null,
	SettingStr	varchar(100) null,
  CONSTRAINT PK_PlayerVillageClaims_ClanSetting PRIMARY KEY NONCLUSTERED (ClanID, SettingID)
)
go



IF OBJECT_ID('PlayerVillageClaims') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerVillageClaims >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerVillageClaims >>>'
go

/* 
 * TABLE: QuestProgression 
 */

CREATE TABLE QuestProgression(
    Step           int            NOT NULL,
    TagName        varchar(50)    NOT NULL,
    isMandatory    smallint       DEFAULT 1 NOT NULL
)
go



IF OBJECT_ID('QuestProgression') IS NOT NULL
    PRINT '<<< CREATED TABLE QuestProgression >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE QuestProgression >>>'
go

/* 
 * TABLE: QuestTemplates 
 */

CREATE TABLE QuestTemplates(
    RowID                               int             IDENTITY(1,1),
    TagName                             varchar(50)     NOT NULL,
    DependantQuestTagName               varchar(50)     NULL,
    reward_silver                       int             NULL,
    reward_credits                      int             NULL,
    CompleteCondition_Building_ID       int             NULL,
    CompleteCondition_Building_Level    int             NULL,
    CompleteCondition_TitleLevel        int             NULL,
    CompleteCondition_NumVillages       int             NULL,
    CompleteCondition_ResearchItemID    int             NULL,
    Title                               varchar(max)    NULL,
    Goal                                varchar(max)    NULL,
    Description                         varchar(max)    NULL,
    CONSTRAINT PK315 PRIMARY KEY NONCLUSTERED (TagName)
)
go



IF OBJECT_ID('QuestTemplates') IS NOT NULL
    PRINT '<<< CREATED TABLE QuestTemplates >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE QuestTemplates >>>'
go

/* 
 * TABLE: Realm 
 */

CREATE TABLE Realm(
    CancelAttackInMin             int         DEFAULT 20 NOT NULL,
    CoinTransportSpeed            int         DEFAULT 10 NOT NULL,
    RealmSize                     int         NOT NULL,
    MaxPlayers                    int         DEFAULT 100 NOT NULL,
    BeginnerProtectionDays        real        NOT NULL,
    OpenOn                        datetime    NOT NULL,
    RebelVillageCreationChance    real        DEFAULT 0 NOT NULL
)
go



IF OBJECT_ID('Realm') IS NOT NULL
    PRINT '<<< CREATED TABLE Realm >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Realm >>>'
go

/* 
 * TABLE: RealmAges 
 */

CREATE TABLE RealmAges(
    AgeNumber      int              NOT NULL,
    Description    nvarchar(max)    NOT NULL,
    Until          datetime         NOT NULL,
    InfoUrl        varchar(512)     NOT NULL,
    InfoText       nvarchar(max)    NULL,
    CONSTRAINT PK241_1 PRIMARY KEY NONCLUSTERED (AgeNumber)
)
go



IF OBJECT_ID('RealmAges') IS NOT NULL
    PRINT '<<< CREATED TABLE RealmAges >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE RealmAges >>>'
go

/* 
 * TABLE: RealmAttributes 
 */

CREATE TABLE RealmAttributes(
    AttribID       int             NOT NULL,
    AttribValue    varchar(max)    NOT NULL,
    AttribDesc     varchar(max)    NULL,
    CONSTRAINT PK231 PRIMARY KEY NONCLUSTERED (AttribID)
)
go



IF OBJECT_ID('RealmAttributes') IS NOT NULL
    PRINT '<<< CREATED TABLE RealmAttributes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE RealmAttributes >>>'
go

/* 
 * TABLE: ReportAddressees 
 */

CREATE TABLE ReportAddressees(
    PlayerID               int              NULL,
    ReportID               bigint           NOT NULL,
    ForwardedByPlayerID    int              NULL,
    ForwardedOn            datetime         NULL,
    IsViewed               bit              DEFAULT 0 NOT NULL,
    PointOfView            smallint         NULL,
    AlternateSubject       nvarchar(200)    NULL,
    RecordID               bigint           IDENTITY(1,1),
    FolderID               int              DEFAULT null NULL,
    CONSTRAINT PK69 PRIMARY KEY NONCLUSTERED (RecordID)
)
go



IF OBJECT_ID('ReportAddressees') IS NOT NULL
    PRINT '<<< CREATED TABLE ReportAddressees >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ReportAddressees >>>'
go

/* 
 * TABLE: ReportInfoFlag 
 */

CREATE TABLE ReportInfoFlag(
    RecordID     bigint      NOT NULL,
    FlagID       smallint    NOT NULL,
    FlagValue    smallint    NOT NULL,
    CONSTRAINT PK60_1 PRIMARY KEY NONCLUSTERED (RecordID, FlagID)
)
go



IF OBJECT_ID('ReportInfoFlag') IS NOT NULL
    PRINT '<<< CREATED TABLE ReportInfoFlag >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ReportInfoFlag >>>'
go

/* 
 * TABLE: Reports 
 */

CREATE TABLE Reports(
    ReportID                  bigint           IDENTITY(1,1),
    Time                      datetime         NOT NULL,
    Subject                   nvarchar(200)    NOT NULL,
    ReportTypeID              int              NOT NULL,
    ReportTypeSpecificData    nvarchar(max)    NULL,
    CONSTRAINT PK58 PRIMARY KEY NONCLUSTERED (ReportID)
)
go



IF OBJECT_ID('Reports') IS NOT NULL
    PRINT '<<< CREATED TABLE Reports >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Reports >>>'
go

/* 
 * TABLE: ReportTypes 
 */

CREATE TABLE ReportTypes(
    ReportTypeID    int              NOT NULL,
    Name            nvarchar(100)    NOT NULL,
    Description     nvarchar(200)    NOT NULL,
    Sort            int              NOT NULL,
    CONSTRAINT PK60 PRIMARY KEY NONCLUSTERED (ReportTypeID)
)
go



IF OBJECT_ID('ReportTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE ReportTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ReportTypes >>>'
go

/* 
 * TABLE: Researchers 
 */

CREATE TABLE Researchers(
    PlayerID        int    NOT NULL,
    ResearcherID    int    IDENTITY(1,1),
    CONSTRAINT PK313 PRIMARY KEY NONCLUSTERED (PlayerID, ResearcherID)
)
go



IF OBJECT_ID('Researchers') IS NOT NULL
    PRINT '<<< CREATED TABLE Researchers >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Researchers >>>'
go

/* 
 * TABLE: ResearchInProgress 
 */

CREATE TABLE ResearchInProgress(
    PlayerID              int       NOT NULL,
    EventID               bigint    NOT NULL,
    ResearchItemTypeID    int       NOT NULL,
    ResearchItemID        int       NOT NULL,
    CONSTRAINT PK19_2 PRIMARY KEY NONCLUSTERED (PlayerID, EventID, ResearchItemTypeID, ResearchItemID)
)
go



IF OBJECT_ID('ResearchInProgress') IS NOT NULL
    PRINT '<<< CREATED TABLE ResearchInProgress >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ResearchInProgress >>>'
go

/* 
 * TABLE: ResearchItemProperties 
 */

CREATE TABLE ResearchItemProperties(
    ResearchItemTypeID        int             NOT NULL,
    ResearchItemID            int             NOT NULL,
    ResearchItemPropertyID    int             NOT NULL,
    PropertyValue             varchar(100)    NOT NULL,
    CONSTRAINT PK23_1 PRIMARY KEY NONCLUSTERED (ResearchItemTypeID, ResearchItemID, ResearchItemPropertyID)
)
go



IF OBJECT_ID('ResearchItemProperties') IS NOT NULL
    PRINT '<<< CREATED TABLE ResearchItemProperties >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ResearchItemProperties >>>'
go

/* 
 * TABLE: ResearchItemPropertyTypes 
 */

CREATE TABLE ResearchItemPropertyTypes(
    ResearchItemPropertyID    int              NOT NULL,
    Name                      nvarchar(100)    NOT NULL,
    Type                      int              NOT NULL,
    PropertyID                int              NULL,			-- PROPERTY was changed to NULL when doing research changes for R100
    CONSTRAINT PK24_1 PRIMARY KEY NONCLUSTERED (ResearchItemPropertyID)
)
go



IF OBJECT_ID('ResearchItemPropertyTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE ResearchItemPropertyTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ResearchItemPropertyTypes >>>'
go

/* 
 * TABLE: ResearchItemRequirements 
 */

CREATE TABLE ResearchItemRequirements(
    ResearchItemTypeID            int    NULL,
    ResearchItemID                int    NULL,
    RequiredResearchItemTypeID    int    NOT NULL,
    RequiredResearchItemID        int    NOT NULL
)
go



IF OBJECT_ID('ResearchItemRequirements') IS NOT NULL
    PRINT '<<< CREATED TABLE ResearchItemRequirements >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ResearchItemRequirements >>>'
go

/* 
 * TABLE: ResearchItems 
 */

CREATE TABLE ResearchItems(
    ResearchItemTypeID    int               NOT NULL,
    ResearchItemID        int               NOT NULL,
    PriceInCoins          int               NOT NULL,
    Name                  nvarchar(200)     NOT NULL,
    ResearchTime          bigint            NOT NULL,
    Description           nvarchar(1024)    NOT NULL,
    Image1Url             varchar(250)      NULL,
    Image2Url             varchar(250)      NULL,
    AgeNumber             int               NULL,
    CONSTRAINT PK260_3 PRIMARY KEY NONCLUSTERED (ResearchItemTypeID, ResearchItemID)
)
go



IF OBJECT_ID('ResearchItems') IS NOT NULL
    PRINT '<<< CREATED TABLE ResearchItems >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ResearchItems >>>'
go

/* 
 * TABLE: ResearchItemSpriteLocation 
 */

CREATE TABLE ResearchItemSpriteLocation(
    ResearchItemTypeID    int    NOT NULL,
    ResearchItemID        int    NOT NULL,
    X                     int    NOT NULL,
    Y                     int    NOT NULL,
    CONSTRAINT PK260_3_1 PRIMARY KEY NONCLUSTERED (ResearchItemTypeID, ResearchItemID)
)
go



IF OBJECT_ID('ResearchItemSpriteLocation') IS NOT NULL
    PRINT '<<< CREATED TABLE ResearchItemSpriteLocation >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ResearchItemSpriteLocation >>>'
go

/* 
 * TABLE: ResearchItemTypes 
 */

CREATE TABLE ResearchItemTypes(
    ResearchItemTypeID    int              NOT NULL,
    Description           nvarchar(200)    NOT NULL,
    CONSTRAINT PK260 PRIMARY KEY NONCLUSTERED (ResearchItemTypeID)
)
go



IF OBJECT_ID('ResearchItemTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE ResearchItemTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ResearchItemTypes >>>'
go

/* 
 * TABLE: Roles 
 */

CREATE TABLE Roles(
    RoleID    int    NOT NULL,
    CONSTRAINT PK_aspnet_Roles PRIMARY KEY NONCLUSTERED (RoleID)
)
go

/*
 * TABLE: Roles 
 */

CREATE TABLE SecurityLevelToRoles(
    SecurityLevel    int    NOT NULL,
    RoleID    int    NOT NULL,
    Description varchar(max) NULL,
    CONSTRAINT PK_SecurityLevelToRoles PRIMARY KEY NONCLUSTERED (SecurityLevel, RoleID)
)
go



IF OBJECT_ID('Roles') IS NOT NULL
    PRINT '<<< CREATED TABLE Roles >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Roles >>>'
go

/* 
 * TABLE: SpecialPlayers 
 */

CREATE TABLE SpecialPlayers(
    PlayerID    int         NOT NULL,
    Type        smallint    NOT NULL,
    CONSTRAINT PK225 PRIMARY KEY NONCLUSTERED (PlayerID)
)
go



IF OBJECT_ID('SpecialPlayers') IS NOT NULL
    PRINT '<<< CREATED TABLE SpecialPlayers >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE SpecialPlayers >>>'
go

/* 
 * TABLE: SupportAttackedReports 
 */

CREATE TABLE SupportAttackedReports(
    ReportID              bigint    NOT NULL,
    BattleReportID        bigint    NOT NULL,
    SupportingPlayerID    int       NOT NULL,
    CONSTRAINT PK82 PRIMARY KEY NONCLUSTERED (ReportID, BattleReportID)
)
go



IF OBJECT_ID('SupportAttackedReports') IS NOT NULL
    PRINT '<<< CREATED TABLE SupportAttackedReports >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE SupportAttackedReports >>>'
go

/* 
 * TABLE: Tags 
 */

CREATE TABLE Tags(
    TagID          int             IDENTITY(1,1),
    Name           nvarchar(30)    NOT NULL,
    Description    nvarchar(75)    NOT NULL,
    Sort           smallint        NOT NULL,
    PlayerID       int             NOT NULL,
    CONSTRAINT PK_Tags PRIMARY KEY NONCLUSTERED (TagID)
)
go



IF OBJECT_ID('Tags') IS NOT NULL
    PRINT '<<< CREATED TABLE Tags >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Tags >>>'
go

/* 
 * TABLE: tbh_Forums 
 */

CREATE TABLE tbh_Forums(
    ForumID             int               IDENTITY(1,1),
    AddedDate           datetime          DEFAULT getdate() NOT NULL,
    AddedBy             nvarchar(256)     NOT NULL,
    Title               nvarchar(256)     NOT NULL,
    Moderated           bit               DEFAULT 0 NOT NULL,
    Importance          int               DEFAULT 0 NOT NULL,
    Description         nvarchar(4000)    NULL,
    ImageUrl            nvarchar(256)     NULL,
    ClanID              int               NOT NULL,
    Deleted             bit               DEFAULT 0 NOT NULL,
    AlertClanMembers    bit               DEFAULT 1 NOT NULL,
    SecurityLevel       tinyint           DEFAULT 0 NOT NULL,
    CONSTRAINT PK_tbh_Forums PRIMARY KEY NONCLUSTERED (ForumID)
)
go



IF OBJECT_ID('tbh_Forums') IS NOT NULL
    PRINT '<<< CREATED TABLE tbh_Forums >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE tbh_Forums >>>'
go

/* 
 * TABLE: tbh_Posts 
 */

CREATE TABLE tbh_Posts(
    PostID          int              IDENTITY(1,1),
    AddedDate       datetime         DEFAULT getdate() NOT NULL,
    AddedBy         nvarchar(256)    NOT NULL,
    AddedByIP       nchar(15)        NOT NULL,
    ForumID         int              NOT NULL,
    ParentPostID    int              DEFAULT 0 NOT NULL,
    Title           nvarchar(256)    NOT NULL,
    Body            ntext            NOT NULL,
    Approved        bit              DEFAULT 1 NOT NULL,
    Closed          bit              DEFAULT 0 NOT NULL,
    ViewCount       int              DEFAULT 0 NOT NULL,
    ReplyCount      int              DEFAULT 0 NOT NULL,
    LastPostBy      nvarchar(256)    NOT NULL,
    LastPostDate    datetime         NOT NULL,
    Sticky          bit              NOT NULL,
    CONSTRAINT PK_tbh_Posts PRIMARY KEY NONCLUSTERED (PostID)
)
go



IF OBJECT_ID('tbh_Posts') IS NOT NULL
    PRINT '<<< CREATED TABLE tbh_Posts >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE tbh_Posts >>>'
go

/* 
 * TABLE: Titles 
 */

CREATE TABLE Titles(
    TitleID         int              NOT NULL,
    Title_Male      nvarchar(100)    NOT NULL,
    Title_Female    nvarchar(100)    NOT NULL,
    Description     nvarchar(max)    NOT NULL,
    MaxPoints       int              NOT NULL,
    XP              int              DEFAULT 0 NOT NULL,
    CONSTRAINT PK209 PRIMARY KEY NONCLUSTERED (TitleID)
)
go



IF OBJECT_ID('Titles') IS NOT NULL
    PRINT '<<< CREATED TABLE Titles >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Titles >>>'
go

/* 
 * TABLE: UnitMovements 
 */

CREATE TABLE UnitMovements(
    EventID                 bigint      NOT NULL,
    OriginVillageID         int         NOT NULL,
    DestinationVillageID    int         NOT NULL,
    CommandType             smallint    NOT NULL,
    TripDuration            bigint      NOT NULL,
    Loot                    int         DEFAULT 0 NOT NULL,
    VisibleToTarget         smallint    DEFAULT 2 NOT NULL,
    CONSTRAINT PK12 PRIMARY KEY NONCLUSTERED (EventID)
)
go



IF OBJECT_ID('UnitMovements') IS NOT NULL
    PRINT '<<< CREATED TABLE UnitMovements >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UnitMovements >>>'
go

/* 
 * TABLE: UnitMovements_Attributes 
 */
 --
 -- AttribID:26 - blood lust spell is on
 --			: 1 - the morale with which this attack was launched with
 --			: 2 - the morale this attack took
 --
CREATE TABLE UnitMovements_Attributes(
    EventID        bigint          NOT NULL,
    AttribID       int             NOT NULL,
    AttribValue    varchar(max)    NULL,
    AttribDesc     varchar(max)    NULL,
    CONSTRAINT PK248 PRIMARY KEY NONCLUSTERED (EventID, AttribID)
)
go



IF OBJECT_ID('UnitMovements_Attributes') IS NOT NULL
    PRINT '<<< CREATED TABLE UnitMovements_Attributes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UnitMovements_Attributes >>>'
go

/* 
 * TABLE: UnitMovements_PlayerAttributes 
 */

CREATE TABLE UnitMovements_PlayerAttributes(
    EventID        bigint      NOT NULL,
    PlayerID       int         NOT NULL,
    AttribID       smallint    NOT NULL,
    AttribValue    int         NULL,
    CONSTRAINT PK248_1 PRIMARY KEY NONCLUSTERED (EventID, PlayerID, AttribID)
)
go



IF OBJECT_ID('UnitMovements_PlayerAttributes') IS NOT NULL
    PRINT '<<< CREATED TABLE UnitMovements_PlayerAttributes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UnitMovements_PlayerAttributes >>>'
go

/* 
 * TABLE: UnitOnBuildingAttack 
 */

CREATE TABLE UnitOnBuildingAttack(
    BuildingTypeID    int    NOT NULL,
    UnitTypeID        int    NOT NULL,
    AttackStrength    int    NULL,
    CONSTRAINT PK78 PRIMARY KEY NONCLUSTERED (BuildingTypeID, UnitTypeID)
)
go



IF OBJECT_ID('UnitOnBuildingAttack') IS NOT NULL
    PRINT '<<< CREATED TABLE UnitOnBuildingAttack >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UnitOnBuildingAttack >>>'
go

/* 
 * TABLE: UnitRecruitments 
 */

CREATE TABLE UnitRecruitments(
    EntryID            bigint      IDENTITY(1,1),
    VillageID          int         NOT NULL,
    BuildingTypeID     int         NOT NULL,
    UnitTypeID         int         NOT NULL,
    Count              int         DEFAULT 1 NOT NULL,
    UnitCost           int         NOT NULL,
    DateAdded          datetime    NOT NULL,
    DateLastUpdated    datetime    NULL,
    Status             smallint    DEFAULT 0 NOT NULL,
    CONSTRAINT PK33_1 PRIMARY KEY NONCLUSTERED (EntryID)
)
go



IF OBJECT_ID('UnitRecruitments') IS NOT NULL
    PRINT '<<< CREATED TABLE UnitRecruitments >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UnitRecruitments >>>'
go

/* 
 * TABLE: UnitsMoving 
 */

CREATE TABLE UnitsMoving(
    EventID                 bigint    NOT NULL,
    UnitTypeID              int       NOT NULL,
    UnitCount               int       NOT NULL,
    TargetBuildingTypeID    int       NULL,
    CONSTRAINT PK16 PRIMARY KEY NONCLUSTERED (EventID, UnitTypeID)
)
go



IF OBJECT_ID('UnitsMoving') IS NOT NULL
    PRINT '<<< CREATED TABLE UnitsMoving >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UnitsMoving >>>'
go

/* 
 * TABLE: UnitTypeDefense 
 */

CREATE TABLE UnitTypeDefense(
    UnitTypeID                 int    NOT NULL,
    DefendAgainstUnitTypeID    int    NOT NULL,
    DefenseStrength            int    NOT NULL,
    CONSTRAINT PK76 PRIMARY KEY NONCLUSTERED (UnitTypeID, DefendAgainstUnitTypeID)
)
go



IF OBJECT_ID('UnitTypeDefense') IS NOT NULL
    PRINT '<<< CREATED TABLE UnitTypeDefense >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UnitTypeDefense >>>'
go

/* 
 * TABLE: UnitTypeRecruitmentRequirements 
 */

CREATE TABLE UnitTypeRecruitmentRequirements(
    UnitTypeID        int              NOT NULL,
    BuildingTypeID    int              NOT NULL,
    Level             int              NOT NULL,
    Description       nvarchar(200)    NOT NULL,
    CONSTRAINT PK14 PRIMARY KEY NONCLUSTERED (UnitTypeID, BuildingTypeID, Level)
)
go



IF OBJECT_ID('UnitTypeRecruitmentRequirements') IS NOT NULL
    PRINT '<<< CREATED TABLE UnitTypeRecruitmentRequirements >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UnitTypeRecruitmentRequirements >>>'
go

/* 
 * TABLE: UnitTypeRecruitmentResearchRequirements 
 */

CREATE TABLE UnitTypeRecruitmentResearchRequirements(
    UnitTypeID            int    NOT NULL,
    ResearchItemTypeID    int    NOT NULL,
    ResearchItemID        int    NOT NULL,
    CONSTRAINT PK14_1 PRIMARY KEY NONCLUSTERED (UnitTypeID, ResearchItemTypeID, ResearchItemID)
)
go



IF OBJECT_ID('UnitTypeRecruitmentResearchRequirements') IS NOT NULL
    PRINT '<<< CREATED TABLE UnitTypeRecruitmentResearchRequirements >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UnitTypeRecruitmentResearchRequirements >>>'
go

/* 
 * TABLE: UnitTypes 
 */

CREATE TABLE UnitTypes(
    UnitTypeID           int              NOT NULL,
    Name                 nvarchar(100)    NOT NULL,
    Description          nvarchar(max)    NOT NULL,
    Sort                 int              NOT NULL,
    Cost                 int              NOT NULL,
    Population           int              NOT NULL,
    BuildingTypeID       int              NOT NULL,
    RecruitmentTime      bigint           NOT NULL,
    PropertyID           int              NOT NULL,
    Speed                int              NOT NULL,
    CarryCapacity        int              NOT NULL,
    AttackStrength       int              DEFAULT 0 NOT NULL,
    SurvivalFactor       real             DEFAULT 1.5 NOT NULL,
    ImageIcon            varchar(200)     NOT NULL,
    Image                nvarchar(200)    NOT NULL,
    SpyAbility           int              NOT NULL,
    CounterSpyAbility    int              NOT NULL,
    CONSTRAINT PK13 PRIMARY KEY NONCLUSTERED (UnitTypeID)
)
go



IF OBJECT_ID('UnitTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE UnitTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UnitTypes >>>'
go

/* 
 * TABLE: VillageNotes 
 */

CREATE TABLE VillageNotes(
    VillageID            int             NOT NULL,
    Note                 varchar(max)    NOT NULL,
    NoteOwnerPlayerID    int             NOT NULL,
    CONSTRAINT PK111 PRIMARY KEY NONCLUSTERED (VillageID, NoteOwnerPlayerID)
)
go



IF OBJECT_ID('VillageNotes') IS NOT NULL
    PRINT '<<< CREATED TABLE VillageNotes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE VillageNotes >>>'
go

/* 
 * TABLE: VillageOwnershipHistory 
 */

CREATE TABLE VillageOwnershipHistory(
    VillageID                int         NOT NULL,
    PreviousOwnerPlayerID    int         NOT NULL,
    CurrentOwnerPlayerID     int         NOT NULL,
    Date                     datetime    NOT NULL
)
go



IF OBJECT_ID('VillageOwnershipHistory') IS NOT NULL
    PRINT '<<< CREATED TABLE VillageOwnershipHistory >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE VillageOwnershipHistory >>>'
go

/* 
 * TABLE: Villages 
 */

CREATE TABLE Villages(
    VillageID             int             IDENTITY(1,1),
    Points                int             DEFAULT 0 NOT NULL,
    OwnerPlayerID         int             NOT NULL,
    Name                  nvarchar(25)    NOT NULL,
    Coins                 int             DEFAULT 1500 NOT NULL
                          CHECK (Coins >=0),
    CoinsLastUpdates      datetime        DEFAULT getdate() NOT NULL,
    XCord                 int             NOT NULL,
    YCord                 int             NOT NULL,
    Loyalty               int             DEFAULT 100 NOT NULL,
    LoyaltyLastUpdated    datetime        DEFAULT getdate() NOT NULL,
    VillageTypeID         smallint        DEFAULT 0 NOT NULL,
    CONSTRAINT PK_Villages PRIMARY KEY NONCLUSTERED (VillageID)
)
go



IF OBJECT_ID('Villages') IS NOT NULL
    PRINT '<<< CREATED TABLE Villages >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Villages >>>'
go

/* 
 * TABLE: Villages_Absorbed 
 */

CREATE TABLE Villages_Absorbed(
    AbsorbedVillageID    int    NOT NULL,
    PromotedVillageID    int    NULL,
    CONSTRAINT PK308 PRIMARY KEY NONCLUSTERED (AbsorbedVillageID)
)
go



IF OBJECT_ID('Villages_Absorbed') IS NOT NULL
    PRINT '<<< CREATED TABLE Villages_Absorbed >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Villages_Absorbed >>>'
go

/* 
 * TABLE: Villages_Promoted 
 */

CREATE TABLE Villages_Promoted(
    PromotedVillageID    int    NOT NULL,
    CONSTRAINT PK307 PRIMARY KEY NONCLUSTERED (PromotedVillageID)
)
go



IF OBJECT_ID('Villages_Promoted') IS NOT NULL
    PRINT '<<< CREATED TABLE Villages_Promoted >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Villages_Promoted >>>'
go

/* 
 * TABLE: VillageSemaphore 
 */

CREATE TABLE VillageSemaphore(
    VillageID    int         NOT NULL,
    TimeStamp    datetime    NOT NULL,
    CONSTRAINT PK97 PRIMARY KEY CLUSTERED (VillageID)
)
go



IF OBJECT_ID('VillageSemaphore') IS NOT NULL
    PRINT '<<< CREATED TABLE VillageSemaphore >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE VillageSemaphore >>>'
go

/* 
 * TABLE: VillageStartLevels 
 */

CREATE TABLE VillageStartLevels(
    StartLevelID         int         NOT NULL,
    RealmMaxAgeInDays    smallint    NOT NULL,
    Coins                int         NOT NULL,
    CONSTRAINT PK202 PRIMARY KEY NONCLUSTERED (StartLevelID)
)
go



IF OBJECT_ID('VillageStartLevels') IS NOT NULL
    PRINT '<<< CREATED TABLE VillageStartLevels >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE VillageStartLevels >>>'
go

/* 
 * TABLE: VillageStartLevels_Buildings 
 */

CREATE TABLE VillageStartLevels_Buildings(
    StartLevelID      int    NOT NULL,
    BuildingTypeID    int    NOT NULL,
    Level             int    NOT NULL,
    CONSTRAINT PK11_1 PRIMARY KEY NONCLUSTERED (StartLevelID, BuildingTypeID)
)
go



IF OBJECT_ID('VillageStartLevels_Buildings') IS NOT NULL
    PRINT '<<< CREATED TABLE VillageStartLevels_Buildings >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE VillageStartLevels_Buildings >>>'
go

/* 
 * TABLE: VillageStartLevels_ResearchItems 
 */

CREATE TABLE VillageStartLevels_ResearchItems(
    StartLevelID          int    NOT NULL,
    ResearchItemTypeID    int    NOT NULL,
    ResearchItemID        int    NOT NULL,
    CONSTRAINT PK11_1_1 PRIMARY KEY NONCLUSTERED (StartLevelID, ResearchItemTypeID, ResearchItemID)
)
go



IF OBJECT_ID('VillageStartLevels_ResearchItems') IS NOT NULL
    PRINT '<<< CREATED TABLE VillageStartLevels_ResearchItems >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE VillageStartLevels_ResearchItems >>>'
go

/* 
 * TABLE: VillageStartLevels_Units 
 */

CREATE TABLE VillageStartLevels_Units(
    StartLevelID    int    NOT NULL,
    UnitTypeID      int    NOT NULL,
    Count           int    DEFAULT 0 NOT NULL,
    CONSTRAINT PK31_1 PRIMARY KEY NONCLUSTERED (StartLevelID, UnitTypeID)
)
go



IF OBJECT_ID('VillageStartLevels_Units') IS NOT NULL
    PRINT '<<< CREATED TABLE VillageStartLevels_Units >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE VillageStartLevels_Units >>>'
go

/* 
 * TABLE: VillageSupportUnits 
 */

CREATE TABLE VillageSupportUnits(
    SupportedVillageID     int    NOT NULL,
    SupportingVillageID    int    NOT NULL,
    UnitTypeID             int    NOT NULL,
    UnitCount              int    NOT NULL,
    CONSTRAINT PK54 PRIMARY KEY NONCLUSTERED (SupportedVillageID, SupportingVillageID, UnitTypeID)
)
go



IF OBJECT_ID('VillageSupportUnits') IS NOT NULL
    PRINT '<<< CREATED TABLE VillageSupportUnits >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE VillageSupportUnits >>>'
go

/* 
 * TABLE: VillageTags 
 */

CREATE TABLE VillageTags(
    VillageID    int    NOT NULL,
    TagID        int    NOT NULL,
    CONSTRAINT PK_VillageTags PRIMARY KEY NONCLUSTERED (VillageID, TagID)
)
go



IF OBJECT_ID('VillageTags') IS NOT NULL
    PRINT '<<< CREATED TABLE VillageTags >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE VillageTags >>>'
go

/* 
 * TABLE: VillageTypeProperties 
 */

CREATE TABLE VillageTypeProperties(
    VillageTypeID                smallint       NOT NULL,
    VillageTypePropertyTypeID    int            NOT NULL,
    PropertyValue                varchar(50)    NOT NULL,
    CONSTRAINT PK296 PRIMARY KEY NONCLUSTERED (VillageTypeID, VillageTypePropertyTypeID)
)
go



IF OBJECT_ID('VillageTypeProperties') IS NOT NULL
    PRINT '<<< CREATED TABLE VillageTypeProperties >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE VillageTypeProperties >>>'
go

/* 
 * TABLE: VillageTypePropertyTypes 
 */

CREATE TABLE VillageTypePropertyTypes(
    VillageTypePropertyTypeID    int              NOT NULL,
    Name                         nvarchar(100)    NOT NULL,
    Type                         smallint         NOT NULL,
    PropertyID                   int              NOT NULL,
    CONSTRAINT PK295 PRIMARY KEY NONCLUSTERED (VillageTypePropertyTypeID)
)
go



IF OBJECT_ID('VillageTypePropertyTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE VillageTypePropertyTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE VillageTypePropertyTypes >>>'
go

/* 
 * TABLE: VillageTypes 
 */

CREATE TABLE VillageTypes(
    VillageTypeID    smallint         NOT NULL,
    Name             nvarchar(100)    NOT NULL,
    Description      nvarchar(200)    NOT NULL,
    CONSTRAINT PK252 PRIMARY KEY NONCLUSTERED (VillageTypeID)
)
go



IF OBJECT_ID('VillageTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE VillageTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE VillageTypes >>>'
go

/* 
 * TABLE: VillageUnits 
 */

CREATE TABLE VillageUnits(
    VillageID       int    NOT NULL,
    UnitTypeID      int    NOT NULL,
    TotalCount      int    DEFAULT 0 NOT NULL
                    CHECK (TotalCount >=0),
    CurrentCount    int    DEFAULT 0 NOT NULL
                    CHECK (CurrentCount >=0),
    CONSTRAINT PK31 PRIMARY KEY NONCLUSTERED (VillageID, UnitTypeID)
)
go

create table PlayerMoraleEffectsTemplate (
    MoraleMax		int    NOT NULL,
    MoraleMin		int    NOT NULL,
    AttackAdj		real NOT NULL,
    DesertionAdj		real NOT NULL,
    CarryCapAdj		real NOT NULL,
    MoveSpeedAdj		real NOT NULL,
)
CREATE TABLE PlayerMoraleEffects(
    Morale			int    NOT NULL,
    AttackAdj		real NOT NULL,
    DesertionAdj		real NOT NULL,
    CarryCapAdj		real NOT NULL,
    MoveSpeedAdj		real NOT NULL,
    CONSTRAINT PK_PlayerMoraleEffects PRIMARY KEY NONCLUSTERED (Morale)
)
go




create table AvailableVillageCords (x smallint, y smallint, xplusy real CONSTRAINT PK_AvailableVillageCords PRIMARY KEY CLUSTERED (x,y))

CREATE NONCLUSTERED INDEX IDX_AvailableVillageCords ON AvailableVillageCords 
(
	xplusy ASC
)
INCLUDE (x,y)


IF OBJECT_ID('VillageUnits') IS NOT NULL
    PRINT '<<< CREATED TABLE VillageUnits >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE VillageUnits >>>'
go

/* 
 * INDEX: UK_Clans 
 */

CREATE UNIQUE INDEX UK_Clans ON Clans(Name)
go
IF EXISTS (SELECT * FROM sysindexes WHERE id=OBJECT_ID('Clans') AND name='UK_Clans')
    PRINT '<<< CREATED INDEX Clans.UK_Clans >>>'
ELSE
    PRINT '<<< FAILED CREATING INDEX Clans.UK_Clans >>>'
go

/* 
 * INDEX: UK_NewVillageQ_Cords 
 */

CREATE UNIQUE INDEX UK_NewVillageQ_Cords ON NewVillageQ(XCord, YCord)
go
IF EXISTS (SELECT * FROM sysindexes WHERE id=OBJECT_ID('NewVillageQ') AND name='UK_NewVillageQ_Cords')
    PRINT '<<< CREATED INDEX NewVillageQ.UK_NewVillageQ_Cords >>>'
ELSE
    PRINT '<<< FAILED CREATING INDEX NewVillageQ.UK_NewVillageQ_Cords >>>'
go

/* 
 * INDEX: UK_Players_Name 
 */

CREATE UNIQUE INDEX UK_Players_Name ON Players(Name)
go
IF EXISTS (SELECT * FROM sysindexes WHERE id=OBJECT_ID('Players') AND name='UK_Players_Name')
    PRINT '<<< CREATED INDEX Players.UK_Players_Name >>>'
ELSE
    PRINT '<<< FAILED CREATING INDEX Players.UK_Players_Name >>>'
go

/* 
 * INDEX: uk_VillageStartLevels 
 */

CREATE UNIQUE INDEX uk_VillageStartLevels ON VillageStartLevels(RealmMaxAgeInDays, StartLevelID)
go
IF EXISTS (SELECT * FROM sysindexes WHERE id=OBJECT_ID('VillageStartLevels') AND name='uk_VillageStartLevels')
    PRINT '<<< CREATED INDEX VillageStartLevels.uk_VillageStartLevels >>>'
ELSE
    PRINT '<<< FAILED CREATING INDEX VillageStartLevels.uk_VillageStartLevels >>>'
go

/* 
 * TABLE: AccountStewards 
 */

ALTER TABLE AccountStewards ADD CONSTRAINT RefPlayers289 
    FOREIGN KEY (StewardPlayerID)
    REFERENCES Players(PlayerID)
go

ALTER TABLE AccountStewards ADD CONSTRAINT RefPlayers290 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: BattleReportBuildingIntel 
 */

ALTER TABLE BattleReportBuildingIntel ADD CONSTRAINT FK_BattleReports_BattleReportBuildingIntel 
    FOREIGN KEY (ReportID)
    REFERENCES BattleReports(ReportID)
go

ALTER TABLE BattleReportBuildingIntel ADD CONSTRAINT FK_BuildingLevels_BattleReportBuildingIntel 
    FOREIGN KEY (BuildingTypeID, Level)
    REFERENCES BuildingLevels(BuildingTypeID, Level)
go


/* 
 * TABLE: BattleReportBuildings 
 */

ALTER TABLE BattleReportBuildings ADD CONSTRAINT FK_BattleReports_BattleReportBuildings 
    FOREIGN KEY (ReportID)
    REFERENCES BattleReports(ReportID)
go

ALTER TABLE BattleReportBuildings ADD CONSTRAINT FK_BuildingTypes_BattleReportBuildings 
    FOREIGN KEY (BuildingTypeID)
    REFERENCES BuildingTypes(BuildingTypeID)
go


/* 
 * TABLE: BattleReports 
 */

ALTER TABLE BattleReports ADD CONSTRAINT FK_Players_BattleRepo92 
    FOREIGN KEY (DefenderPlayerID)
    REFERENCES Players(PlayerID)
go

ALTER TABLE BattleReports ADD CONSTRAINT FK_Players_BattleReports 
    FOREIGN KEY (AttackerPlayerID)
    REFERENCES Players(PlayerID)
go

ALTER TABLE BattleReports ADD CONSTRAINT FK_Reports_BattleReports 
    FOREIGN KEY (ReportID)
    REFERENCES Reports(ReportID)
go

ALTER TABLE BattleReports ADD CONSTRAINT FK_Villages_BattleRepo90 
    FOREIGN KEY (DefenderVillageID)
    REFERENCES Villages(VillageID)
go

ALTER TABLE BattleReports ADD CONSTRAINT FK_Villages_BattleReports 
    FOREIGN KEY (AttackerVillageID)
    REFERENCES Villages(VillageID)
go


/* 
 * TABLE: BattleReportSupport 
 */

ALTER TABLE BattleReportSupport ADD CONSTRAINT FK_BattleReports_BattleReportSupport 
    FOREIGN KEY (ReportID)
    REFERENCES BattleReports(ReportID)
go


/* 
 * TABLE: BattleReportSupportUnits 
 */

ALTER TABLE BattleReportSupportUnits ADD CONSTRAINT FK_BattleReportSupport_BattleReportSupportUnits 
    FOREIGN KEY (ReportID, SupportingVillageID)
    REFERENCES BattleReportSupport(ReportID, SupportingVillageID)
go

ALTER TABLE BattleReportSupportUnits ADD CONSTRAINT FK_UnitTypes_BattleReportSupportUnits 
    FOREIGN KEY (UnitTypeID)
    REFERENCES UnitTypes(UnitTypeID)
go


/* 
 * TABLE: BattleReportUnits 
 */

ALTER TABLE BattleReportUnits ADD CONSTRAINT FK_BattleReports_BattleReportUnits 
    FOREIGN KEY (ReportID)
    REFERENCES BattleReports(ReportID)
go

ALTER TABLE BattleReportUnits ADD CONSTRAINT FK_UnitTypes_BattleReportUnits 
    FOREIGN KEY (UnitTypeID)
    REFERENCES UnitTypes(UnitTypeID)
go


/* 
 * TABLE: BuildingDowngradeQEntries 
 */

ALTER TABLE BuildingDowngradeQEntries ADD CONSTRAINT RefBuildingTypes312 
    FOREIGN KEY (BuildingTypeID)
    REFERENCES BuildingTypes(BuildingTypeID)
go

ALTER TABLE BuildingDowngradeQEntries ADD CONSTRAINT RefVillages313 
    FOREIGN KEY (VillageID)
    REFERENCES Villages(VillageID)
go


/* 
 * TABLE: BuildingDowngrades 
 */

ALTER TABLE BuildingDowngrades ADD CONSTRAINT RefVillages307 
    FOREIGN KEY (VillageID)
    REFERENCES Villages(VillageID)
go

ALTER TABLE BuildingDowngrades ADD CONSTRAINT RefBuildingTypes308 
    FOREIGN KEY (BuildingTypeID)
    REFERENCES BuildingTypes(BuildingTypeID)
go

ALTER TABLE BuildingDowngrades ADD CONSTRAINT RefEvents309 
    FOREIGN KEY (EventID)
    REFERENCES Events(EventID)
go


/* 
 * TABLE: BuildingLevels 
 */

ALTER TABLE BuildingLevels ADD CONSTRAINT FK_BuildingTypes_BuildingLevels 
    FOREIGN KEY (BuildingTypeID)
    REFERENCES BuildingTypes(BuildingTypeID)
go


/* 
 * TABLE: Buildings 
 */

ALTER TABLE Buildings ADD CONSTRAINT FK_BuildingLevels_Buildings 
    FOREIGN KEY (BuildingTypeID, Level)
    REFERENCES BuildingLevels(BuildingTypeID, Level)
go

ALTER TABLE Buildings ADD CONSTRAINT FK_Villages_Buildings 
    FOREIGN KEY (VillageID)
    REFERENCES Villages(VillageID)
go


/* 
 * TABLE: BuildingTypeRequirements 
 */

ALTER TABLE BuildingTypeRequirements ADD CONSTRAINT FK_BuildingLevels_BuildingTypeRequireme63 
    FOREIGN KEY (RequiredBuildingTypeID, RequiredLevel)
    REFERENCES BuildingLevels(BuildingTypeID, Level)
go

ALTER TABLE BuildingTypeRequirements ADD CONSTRAINT FK_BuildingLevels_BuildingTypeRequirements 
    FOREIGN KEY (BuildingTypeID, Level)
    REFERENCES BuildingLevels(BuildingTypeID, Level)
go


/* 
 * TABLE: BuildingUpgradeQEntries 
 */

ALTER TABLE BuildingUpgradeQEntries ADD CONSTRAINT FK_BuildingTypes_BuildingUpgradeQEntries 
    FOREIGN KEY (BuildingTypeID)
    REFERENCES BuildingTypes(BuildingTypeID)
go

ALTER TABLE BuildingUpgradeQEntries ADD CONSTRAINT FK_Villages_BuildingUpgradeQEntries 
    FOREIGN KEY (VillageID)
    REFERENCES Villages(VillageID)
go


/* 
 * TABLE: BuildingUpgrades 
 */

ALTER TABLE BuildingUpgrades ADD CONSTRAINT FK_BuildingLevels_BuildingUpgrades 
    FOREIGN KEY (BuildingTypeID, Level)
    REFERENCES BuildingLevels(BuildingTypeID, Level)
go

ALTER TABLE BuildingUpgrades ADD CONSTRAINT FK_Events_BuildingUpgrades 
    FOREIGN KEY (EventID)
    REFERENCES Events(EventID)
go

ALTER TABLE BuildingUpgrades ADD CONSTRAINT FK_Villages_BuildingUpgrades 
    FOREIGN KEY (VillageID)
    REFERENCES Villages(VillageID)
go


/* 
 * TABLE: CapitalVillages 
 */

ALTER TABLE CapitalVillages ADD CONSTRAINT RefVillages395 
    FOREIGN KEY (VillageID)
    REFERENCES Villages(VillageID)
go


/* 
 * TABLE: Chat 
 */

ALTER TABLE Chat ADD CONSTRAINT RefClans326 
    FOREIGN KEY (ClanID)
    REFERENCES Clans(ClanID)
go

ALTER TABLE Chat ADD CONSTRAINT RefPlayers327 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: ClanDiplomacy 
 */

ALTER TABLE ClanDiplomacy ADD CONSTRAINT FK_Clans_ClanDiplo101 
    FOREIGN KEY (OtherClanID)
    REFERENCES Clans(ClanID)
go

ALTER TABLE ClanDiplomacy ADD CONSTRAINT FK_Clans_ClanDiplomacy 
    FOREIGN KEY (ClanID)
    REFERENCES Clans(ClanID)
go


/* 
 * TABLE: ClanEvents 
 */

ALTER TABLE ClanEvents ADD CONSTRAINT FK_Clans_ClanEvents 
    FOREIGN KEY (ClanID)
    REFERENCES Clans(ClanID)
go


/* 
 * TABLE: ClanInviteLog 
 */

ALTER TABLE ClanInviteLog ADD CONSTRAINT RefPlayers298 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go

ALTER TABLE ClanInviteLog ADD CONSTRAINT RefClans299 
    FOREIGN KEY (ClanID)
    REFERENCES Clans(ClanID)
go


/* 
 * TABLE: ClanInvites 
 */

ALTER TABLE ClanInvites ADD CONSTRAINT FK_Clans_ClanInvites 
    FOREIGN KEY (ClanID)
    REFERENCES Clans(ClanID)
go

ALTER TABLE ClanInvites ADD CONSTRAINT FK_Players_ClanInvites 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: ClanMembers 
 */

ALTER TABLE ClanMembers ADD CONSTRAINT FK_Clans_ClanMembers 
    FOREIGN KEY (ClanID)
    REFERENCES Clans(ClanID)
go

ALTER TABLE ClanMembers ADD CONSTRAINT FK_Players_ClanMembers 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: CoinTransports 
 */

ALTER TABLE CoinTransports ADD CONSTRAINT FK_Events_CoinTransports 
    FOREIGN KEY (EventID)
    REFERENCES Events(EventID)
go

ALTER TABLE CoinTransports ADD CONSTRAINT FK_Villages_CoinTranspo17 
    FOREIGN KEY (DestinationVillageID)
    REFERENCES Villages(VillageID)
go

ALTER TABLE CoinTransports ADD CONSTRAINT FK_Villages_CoinTransports 
    FOREIGN KEY (OriginVillageID)
    REFERENCES Villages(VillageID)
go


/* 
 * TABLE: CreditFarm_PlayerFoodChanceModifierFactor 
 */

ALTER TABLE CreditFarm_PlayerFoodChanceModifierFactor ADD CONSTRAINT RefPlayers437 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: CreditFarmLogTable 
 */

ALTER TABLE CreditFarmLogTable ADD CONSTRAINT RefPlayers438 
    FOREIGN KEY (AttackerPID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: DefaultRoles 
 */

ALTER TABLE DefaultRoles ADD CONSTRAINT RefRoles293 
    FOREIGN KEY (RoleID)
    REFERENCES Roles(RoleID)
go

ALTER TABLE DefaultRoles ADD CONSTRAINT RefClans294 
    FOREIGN KEY (ClanID)
    REFERENCES Clans(ClanID)
go


/* 
 * TABLE: Filters 
 */

ALTER TABLE Filters ADD CONSTRAINT FK_Players_Filters 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: FilterTags 
 */

ALTER TABLE FilterTags ADD CONSTRAINT FK_Filters_FilterTags 
    FOREIGN KEY (FilterID)
    REFERENCES Filters(FilterID)
go

ALTER TABLE FilterTags ADD CONSTRAINT FK_Tags_FilterTags 
    FOREIGN KEY (TagID)
    REFERENCES Tags(TagID)
go


/* 
 * TABLE: Folders 
 */

ALTER TABLE Folders ADD CONSTRAINT FK_Folders_Players 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: ForumSharing 
 */

ALTER TABLE ForumSharing ADD CONSTRAINT RefClans392 
    FOREIGN KEY (ClanID)
    REFERENCES Clans(ClanID)
go

ALTER TABLE ForumSharing ADD CONSTRAINT Reftbh_Forums393 
    FOREIGN KEY (ForumID)
    REFERENCES tbh_Forums(ForumID)
go


/* 
 * TABLE: ForumSharingWhiteListedClans 
 */

ALTER TABLE ForumSharingWhiteListedClans ADD CONSTRAINT RefClans406 
    FOREIGN KEY (WhiteListClanID)
    REFERENCES Clans(ClanID)
go

ALTER TABLE ForumSharingWhiteListedClans ADD CONSTRAINT RefClans407 
    FOREIGN KEY (ClanID)
    REFERENCES Clans(ClanID)
go


/* 
 * TABLE: Landmarks 
 */

ALTER TABLE Landmarks ADD CONSTRAINT FK_LandmarkTypeParts_Landmarks 
    FOREIGN KEY (LandmarkTypePartID)
    REFERENCES LandmarkTypeParts(LandmarkTypePartID)
go


/* 
 * TABLE: LandmarkTypeParts 
 */

ALTER TABLE LandmarkTypeParts ADD CONSTRAINT FK_LandmarkTypes_LandmarkTypeParts 
    FOREIGN KEY (LandmarkTypeID)
    REFERENCES LandmarkTypes(LandmarkTypeID)
go


/* 
 * TABLE: LevelProperties 
 */

ALTER TABLE LevelProperties ADD CONSTRAINT FK_BuildingLevels_LevelProperties 
    FOREIGN KEY (BuildingTypeID, Level)
    REFERENCES BuildingLevels(BuildingTypeID, Level)
go

ALTER TABLE LevelProperties ADD CONSTRAINT FK_LevelPropertyTypes_LevelProperties 
    FOREIGN KEY (PropertyID)
    REFERENCES LevelPropertyTypes(PropertyID)
go


/* 
 * TABLE: MessageAddressees 
 */

ALTER TABLE MessageAddressees ADD CONSTRAINT FK_Folders_MessageAddressees 
    FOREIGN KEY (FolderID)
    REFERENCES Folders(FolderID)
go

ALTER TABLE MessageAddressees ADD CONSTRAINT FK_Messages_MessageAddressees 
    FOREIGN KEY (MessageID)
    REFERENCES Messages(MessageID)
go

ALTER TABLE MessageAddressees ADD CONSTRAINT FK_Players_MessageAddressees 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: Messages 
 */

ALTER TABLE Messages ADD CONSTRAINT FK_Players_Messages 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: MessagesBlockedPlayers 
 */

ALTER TABLE MessagesBlockedPlayers ADD CONSTRAINT FK_Players_MessagesBlockedPlay47 
    FOREIGN KEY (BlockedPlayerID)
    REFERENCES Players(PlayerID)
go

ALTER TABLE MessagesBlockedPlayers ADD CONSTRAINT FK_Players_MessagesBlockedPlayers 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: NoTransportVillages 
 */

ALTER TABLE NoTransportVillages ADD CONSTRAINT FK_Villages_NoTransportVillages 
    FOREIGN KEY (VillageID)
    REFERENCES Villages(VillageID)
go


/* 
 * TABLE: PFsInPackage 
 */

ALTER TABLE PFsInPackage ADD CONSTRAINT FK_PFPackages_PFsInPackage 
    FOREIGN KEY (PFPackageID)
    REFERENCES PFPackages(PFPackageID)
go

ALTER TABLE PFsInPackage ADD CONSTRAINT FK_PFs_PFsInPackage 
    FOREIGN KEY (FeatureID)
    REFERENCES PFs(FeatureID)
go


/* 
 * TABLE: PFTrails 
 */

ALTER TABLE PFTrails ADD CONSTRAINT FK_PFs_PFTrails 
    FOREIGN KEY (FeatureID)
    REFERENCES PFs(FeatureID)
go


/* 
 * TABLE: PlayerCacheTimeStamps 
 */

ALTER TABLE PlayerCacheTimeStamps ADD CONSTRAINT RefPlayers417 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: PlayerFlags 
 */

ALTER TABLE PlayerFlags ADD CONSTRAINT FK_Players_PlayerFlags 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: PlayerInRoles 
 */

ALTER TABLE PlayerInRoles ADD CONSTRAINT FK_ClanMembers_PlayerInRoles 
    FOREIGN KEY (ClanID, PlayerID)
    REFERENCES ClanMembers(ClanID, PlayerID)
go

ALTER TABLE PlayerInRoles ADD CONSTRAINT FK_Roles_PlayerInRoles 
    FOREIGN KEY (RoleID)
    REFERENCES Roles(RoleID)
go


/* 
 * TABLE: PlayerMapEvents 
 */

ALTER TABLE PlayerMapEvents ADD CONSTRAINT RefPlayers434 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: PlayerNotes 
 */

ALTER TABLE PlayerNotes ADD CONSTRAINT FK_Players_PlayerNo27 
    FOREIGN KEY (NoteOwnerPlayerID)
    REFERENCES Players(PlayerID)
go

ALTER TABLE PlayerNotes ADD CONSTRAINT FK_Players_PlayerNotes 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: PlayerNotifications 
 */

ALTER TABLE PlayerNotifications ADD CONSTRAINT RefPlayers425 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: PlayerNotificationSettings 
 */

ALTER TABLE PlayerNotificationSettings ADD CONSTRAINT RefPlayers420 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go

ALTER TABLE PlayerNotificationSettings ADD CONSTRAINT RefNotificationSettings_Template422 
    FOREIGN KEY (NotificationID)
    REFERENCES NotificationSettings_Template(NotificationID)
go


/* 
 * TABLE: PlayerNotificationsNotSent 
 */

ALTER TABLE PlayerNotificationsNotSent ADD CONSTRAINT RefPlayers431 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: PlayerNotificationsSent 
 */

ALTER TABLE PlayerNotificationsSent ADD CONSTRAINT RefPlayers432 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: PlayerPFTrials 
 */

ALTER TABLE PlayerPFTrials ADD CONSTRAINT FK_PFTrails_PlayerPFTrials 
    FOREIGN KEY (PFTrialID)
    REFERENCES PFTrails(PFTrialID)
go

ALTER TABLE PlayerPFTrials ADD CONSTRAINT FK_Players_PlayerPFTrials 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: PlayerPostViews 
 */

ALTER TABLE PlayerPostViews ADD CONSTRAINT FK_Players_PlayerPostViews 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go

ALTER TABLE PlayerPostViews ADD CONSTRAINT FK_tbh_Posts_PlayerPostViews 
    FOREIGN KEY (ThreadPostID)
    REFERENCES tbh_Posts(PostID)
go


/* 
 * TABLE: PlayerProfile 
 */

ALTER TABLE PlayerProfile ADD CONSTRAINT RefPlayers374 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: PlayerRecentTargetStack 
 */

ALTER TABLE PlayerRecentTargetStack ADD CONSTRAINT RefVillages302 
    FOREIGN KEY (TargetVillageID)
    REFERENCES Villages(VillageID)
go

ALTER TABLE PlayerRecentTargetStack ADD CONSTRAINT RefPlayers303 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: PlayerResearchItems 
 */

ALTER TABLE PlayerResearchItems ADD CONSTRAINT RefPlayers336 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go

ALTER TABLE PlayerResearchItems ADD CONSTRAINT RefResearchItems355 
    FOREIGN KEY (ResearchItemTypeID, ResearchItemID)
    REFERENCES ResearchItems(ResearchItemTypeID, ResearchItemID)
go


/* 
 * TABLE: Players 
 */

ALTER TABLE Players ADD CONSTRAINT FK_Titles_Players 
    FOREIGN KEY (TitleID)
    REFERENCES Titles(TitleID)
go


/* 
 * TABLE: PlayersFriends 
 */

ALTER TABLE PlayersFriends ADD CONSTRAINT FK_Players_PlayersFrie37 
    FOREIGN KEY (FriendPlayerID)
    REFERENCES Players(PlayerID)
go

ALTER TABLE PlayersFriends ADD CONSTRAINT FK_Players_PlayersFriends 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: PlayersPFPackages 
 */

ALTER TABLE PlayersPFPackages ADD CONSTRAINT FK_PFPackages_PlayersPFPackages 
    FOREIGN KEY (PFPackageID)
    REFERENCES PFPackages(PFPackageID)
go

ALTER TABLE PlayersPFPackages ADD CONSTRAINT FK_Players_PlayersPFPackages 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: PlayerStats 
 */

ALTER TABLE PlayerStats ADD CONSTRAINT RefPlayers317 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: PlayerSuspensions 
 */

ALTER TABLE PlayerSuspensions ADD CONSTRAINT RefPlayers375 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: PlayerTitleHistory 
 */

ALTER TABLE PlayerTitleHistory ADD CONSTRAINT FK_Players_PlayerTitleHistory 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go

ALTER TABLE PlayerTitleHistory ADD CONSTRAINT FK_Titles_PlayerTitleHistory 
    FOREIGN KEY (TitleID)
    REFERENCES Titles(TitleID)
go


/* 
 * TABLE: QuestProgression 
 */

ALTER TABLE QuestProgression ADD CONSTRAINT RefQuestTemplates412 
    FOREIGN KEY (TagName)
    REFERENCES QuestTemplates(TagName)
go


/* 
 * TABLE: QuestTemplates 
 */

ALTER TABLE QuestTemplates ADD CONSTRAINT RefQuestTemplates413 
    FOREIGN KEY (DependantQuestTagName)
    REFERENCES QuestTemplates(TagName)
go


/* 
 * TABLE: ReportAddressees 
 */

ALTER TABLE ReportAddressees ADD CONSTRAINT FK_Folders_ReportAddressees 
    FOREIGN KEY (FolderID)
    REFERENCES Folders(FolderID)
go

ALTER TABLE ReportAddressees ADD CONSTRAINT FK_Players_ReportAddress98 
    FOREIGN KEY (ForwardedByPlayerID)
    REFERENCES Players(PlayerID)
go

ALTER TABLE ReportAddressees ADD CONSTRAINT FK_Players_ReportAddressees 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go

ALTER TABLE ReportAddressees ADD CONSTRAINT FK_Reports_ReportAddressees 
    FOREIGN KEY (ReportID)
    REFERENCES Reports(ReportID)
go


/* 
 * TABLE: ReportInfoFlag 
 */

ALTER TABLE ReportInfoFlag ADD CONSTRAINT RefReportAddressees377 
    FOREIGN KEY (RecordID)
    REFERENCES ReportAddressees(RecordID)
go


/* 
 * TABLE: Reports 
 */

ALTER TABLE Reports ADD CONSTRAINT FK_ReportTypes_Reports 
    FOREIGN KEY (ReportTypeID)
    REFERENCES ReportTypes(ReportTypeID)
go


/* 
 * TABLE: Researchers 
 */

ALTER TABLE Researchers ADD CONSTRAINT RefPlayers409 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: ResearchInProgress 
 */

ALTER TABLE ResearchInProgress ADD CONSTRAINT RefPlayers367 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go

ALTER TABLE ResearchInProgress ADD CONSTRAINT RefEvents368 
    FOREIGN KEY (EventID)
    REFERENCES Events(EventID)
go

ALTER TABLE ResearchInProgress ADD CONSTRAINT RefResearchItems369 
    FOREIGN KEY (ResearchItemTypeID, ResearchItemID)
    REFERENCES ResearchItems(ResearchItemTypeID, ResearchItemID)
go


/* 
 * TABLE: ResearchItemProperties 
 */

ALTER TABLE ResearchItemProperties ADD CONSTRAINT RefResearchItems362 
    FOREIGN KEY (ResearchItemTypeID, ResearchItemID)
    REFERENCES ResearchItems(ResearchItemTypeID, ResearchItemID)
go

ALTER TABLE ResearchItemProperties ADD CONSTRAINT FK_ResearchItemLevelPropertyTypes_ResearchItemLevelProperties 
    FOREIGN KEY (ResearchItemPropertyID)
    REFERENCES ResearchItemPropertyTypes(ResearchItemPropertyID)
go


/* 
 * TABLE: ResearchItemPropertyTypes 
 */

ALTER TABLE ResearchItemPropertyTypes ADD CONSTRAINT RefLevelPropertyTypes361 
    FOREIGN KEY (PropertyID)
    REFERENCES LevelPropertyTypes(PropertyID)
go


/* 
 * TABLE: ResearchItemRequirements 
 */

ALTER TABLE ResearchItemRequirements ADD CONSTRAINT RefResearchItems357 
    FOREIGN KEY (RequiredResearchItemTypeID, RequiredResearchItemID)
    REFERENCES ResearchItems(ResearchItemTypeID, ResearchItemID)
go

ALTER TABLE ResearchItemRequirements ADD CONSTRAINT RefResearchItems358 
    FOREIGN KEY (ResearchItemTypeID, ResearchItemID)
    REFERENCES ResearchItems(ResearchItemTypeID, ResearchItemID)
go


/* 
 * TABLE: ResearchItems 
 */

ALTER TABLE ResearchItems ADD CONSTRAINT RefResearchItemTypes359 
    FOREIGN KEY (ResearchItemTypeID)
    REFERENCES ResearchItemTypes(ResearchItemTypeID)
go


/* 
 * TABLE: ResearchItemSpriteLocation 
 */

ALTER TABLE ResearchItemSpriteLocation ADD CONSTRAINT RefResearchItems427 
    FOREIGN KEY (ResearchItemTypeID, ResearchItemID)
    REFERENCES ResearchItems(ResearchItemTypeID, ResearchItemID)
go


/* 
 * TABLE: SupportAttackedReports 
 */

ALTER TABLE SupportAttackedReports ADD CONSTRAINT FK_BattleReports_SupportAttackedReports 
    FOREIGN KEY (BattleReportID)
    REFERENCES BattleReports(ReportID)
go

ALTER TABLE SupportAttackedReports ADD CONSTRAINT FK_Reports_SupportAttackedReports 
    FOREIGN KEY (ReportID)
    REFERENCES Reports(ReportID)
go


/* 
 * TABLE: Tags 
 */

ALTER TABLE Tags ADD CONSTRAINT FK_Players_Tags 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: tbh_Forums 
 */

ALTER TABLE tbh_Forums ADD CONSTRAINT FK_Clans_tbh_Forums 
    FOREIGN KEY (ClanID)
    REFERENCES Clans(ClanID)
go


/* 
 * TABLE: tbh_Posts 
 */

ALTER TABLE tbh_Posts ADD CONSTRAINT FK_tbh_Forums_tbh_Posts 
    FOREIGN KEY (ForumID)
    REFERENCES tbh_Forums(ForumID)
go


/* 
 * TABLE: UnitMovements 
 */

ALTER TABLE UnitMovements ADD CONSTRAINT FK_Events_UnitMovements 
    FOREIGN KEY (EventID)
    REFERENCES Events(EventID)
go

ALTER TABLE UnitMovements ADD CONSTRAINT FK_Villages_UnitMoveme65 
    FOREIGN KEY (OriginVillageID)
    REFERENCES Villages(VillageID)
go

ALTER TABLE UnitMovements ADD CONSTRAINT FK_Villages_UnitMovements 
    FOREIGN KEY (DestinationVillageID)
    REFERENCES Villages(VillageID)
go


/* 
 * TABLE: UnitMovements_Attributes 
 */

ALTER TABLE UnitMovements_Attributes ADD CONSTRAINT RefEvents315 
    FOREIGN KEY (EventID)
    REFERENCES Events(EventID)
go


/* 
 * TABLE: UnitOnBuildingAttack 
 */

ALTER TABLE UnitOnBuildingAttack ADD CONSTRAINT FK_BuildingTypes_UnitOnBuildingAttack 
    FOREIGN KEY (BuildingTypeID)
    REFERENCES BuildingTypes(BuildingTypeID)
go

ALTER TABLE UnitOnBuildingAttack ADD CONSTRAINT FK_UnitTypes_UnitOnBuildingAttack 
    FOREIGN KEY (UnitTypeID)
    REFERENCES UnitTypes(UnitTypeID)
go


/* 
 * TABLE: UnitRecruitments 
 */

ALTER TABLE UnitRecruitments ADD CONSTRAINT FK_BuildingTypes_UnitRecruitments 
    FOREIGN KEY (BuildingTypeID)
    REFERENCES BuildingTypes(BuildingTypeID)
go

ALTER TABLE UnitRecruitments ADD CONSTRAINT FK_UnitTypes_UnitRecruitments 
    FOREIGN KEY (UnitTypeID)
    REFERENCES UnitTypes(UnitTypeID)
go

ALTER TABLE UnitRecruitments ADD CONSTRAINT FK_Villages_UnitRecruitments 
    FOREIGN KEY (VillageID)
    REFERENCES Villages(VillageID)
go


/* 
 * TABLE: UnitsMoving 
 */

ALTER TABLE UnitsMoving ADD CONSTRAINT FK_BuildingTypes_UnitsMoving 
    FOREIGN KEY (TargetBuildingTypeID)
    REFERENCES BuildingTypes(BuildingTypeID)
go

ALTER TABLE UnitsMoving ADD CONSTRAINT FK_UnitMovements_UnitsMoving 
    FOREIGN KEY (EventID)
    REFERENCES UnitMovements(EventID)
go

ALTER TABLE UnitsMoving ADD CONSTRAINT FK_UnitTypes_UnitsMoving 
    FOREIGN KEY (UnitTypeID)
    REFERENCES UnitTypes(UnitTypeID)
go


/* 
 * TABLE: UnitTypeDefense 
 */

ALTER TABLE UnitTypeDefense ADD CONSTRAINT FK_UnitTypes_UnitTypeDefen2 
    FOREIGN KEY (DefendAgainstUnitTypeID)
    REFERENCES UnitTypes(UnitTypeID)
go

ALTER TABLE UnitTypeDefense ADD CONSTRAINT FK_UnitTypes_UnitTypeDefense 
    FOREIGN KEY (UnitTypeID)
    REFERENCES UnitTypes(UnitTypeID)
go


/* 
 * TABLE: UnitTypeRecruitmentRequirements 
 */

ALTER TABLE UnitTypeRecruitmentRequirements ADD CONSTRAINT FK_BuildingLevels_UnitTypeRecruitmentRequirements 
    FOREIGN KEY (BuildingTypeID, Level)
    REFERENCES BuildingLevels(BuildingTypeID, Level)
go

ALTER TABLE UnitTypeRecruitmentRequirements ADD CONSTRAINT FK_UnitTypes_UnitTypeRecruitmentRequirements 
    FOREIGN KEY (UnitTypeID)
    REFERENCES UnitTypes(UnitTypeID)
go


/* 
 * TABLE: UnitTypeRecruitmentResearchRequirements 
 */

ALTER TABLE UnitTypeRecruitmentResearchRequirements ADD CONSTRAINT RefResearchItems388 
    FOREIGN KEY (ResearchItemTypeID, ResearchItemID)
    REFERENCES ResearchItems(ResearchItemTypeID, ResearchItemID)
go

ALTER TABLE UnitTypeRecruitmentResearchRequirements ADD CONSTRAINT RefUnitTypes389 
    FOREIGN KEY (UnitTypeID)
    REFERENCES UnitTypes(UnitTypeID)
go


/* 
 * TABLE: UnitTypes 
 */

ALTER TABLE UnitTypes ADD CONSTRAINT FK_BuildingTypes_UnitTypes 
    FOREIGN KEY (BuildingTypeID)
    REFERENCES BuildingTypes(BuildingTypeID)
go

ALTER TABLE UnitTypes ADD CONSTRAINT FK_LevelPropertyTypes_UnitTypes 
    FOREIGN KEY (PropertyID)
    REFERENCES LevelPropertyTypes(PropertyID)
go


/* 
 * TABLE: VillageNotes 
 */

ALTER TABLE VillageNotes ADD CONSTRAINT FK_Players_VillageNotes 
    FOREIGN KEY (NoteOwnerPlayerID)
    REFERENCES Players(PlayerID)
go

ALTER TABLE VillageNotes ADD CONSTRAINT FK_Villages_VillageNotes 
    FOREIGN KEY (VillageID)
    REFERENCES Villages(VillageID)
go


/* 
 * TABLE: Villages 
 */

ALTER TABLE Villages ADD CONSTRAINT RefVillageTypes382 
    FOREIGN KEY (VillageTypeID)
    REFERENCES VillageTypes(VillageTypeID)
go

ALTER TABLE Villages ADD CONSTRAINT FK_Players_Villages 
    FOREIGN KEY (OwnerPlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: Villages_Absorbed 
 */

ALTER TABLE Villages_Absorbed ADD CONSTRAINT RefVillages_Promoted401 
    FOREIGN KEY (PromotedVillageID)
    REFERENCES Villages_Promoted(PromotedVillageID)
go

ALTER TABLE Villages_Absorbed ADD CONSTRAINT RefVillages402 
    FOREIGN KEY (AbsorbedVillageID)
    REFERENCES Villages(VillageID)
go


/* 
 * TABLE: Villages_Promoted 
 */

ALTER TABLE Villages_Promoted ADD CONSTRAINT RefVillages403 
    FOREIGN KEY (PromotedVillageID)
    REFERENCES Villages(VillageID)
go


/* 
 * TABLE: VillageStartLevels_Buildings 
 */

ALTER TABLE VillageStartLevels_Buildings ADD CONSTRAINT FK_VillageStartLevels_VillageStartLevels_Buildings 
    FOREIGN KEY (StartLevelID)
    REFERENCES VillageStartLevels(StartLevelID)
go


/* 
 * TABLE: VillageStartLevels_ResearchItems 
 */

ALTER TABLE VillageStartLevels_ResearchItems ADD CONSTRAINT RefResearchItems415 
    FOREIGN KEY (ResearchItemTypeID, ResearchItemID)
    REFERENCES ResearchItems(ResearchItemTypeID, ResearchItemID)
go


/* 
 * TABLE: VillageStartLevels_Units 
 */

ALTER TABLE VillageStartLevels_Units ADD CONSTRAINT FK_VillageStartLevels_VillageStartLevels_Units 
    FOREIGN KEY (StartLevelID)
    REFERENCES VillageStartLevels(StartLevelID)
go


/* 
 * TABLE: VillageSupportUnits 
 */

ALTER TABLE VillageSupportUnits ADD CONSTRAINT FK_UnitTypes_VillageSupportUnits 
    FOREIGN KEY (UnitTypeID)
    REFERENCES UnitTypes(UnitTypeID)
go

ALTER TABLE VillageSupportUnits ADD CONSTRAINT FK_Villages_VillageSupportUn86 
    FOREIGN KEY (SupportingVillageID)
    REFERENCES Villages(VillageID)
go

ALTER TABLE VillageSupportUnits ADD CONSTRAINT FK_Villages_VillageSupportUnits 
    FOREIGN KEY (SupportedVillageID)
    REFERENCES Villages(VillageID)
go


/* 
 * TABLE: VillageTags 
 */

ALTER TABLE VillageTags ADD CONSTRAINT FK_Tags_VillageTags 
    FOREIGN KEY (TagID)
    REFERENCES Tags(TagID)
go

ALTER TABLE VillageTags ADD CONSTRAINT FK_Villages_VillageTags 
    FOREIGN KEY (VillageID)
    REFERENCES Villages(VillageID)
go


/* 
 * TABLE: VillageTypeProperties 
 */

ALTER TABLE VillageTypeProperties ADD CONSTRAINT RefVillageTypePropertyTypes383 
    FOREIGN KEY (VillageTypePropertyTypeID)
    REFERENCES VillageTypePropertyTypes(VillageTypePropertyTypeID)
go

ALTER TABLE VillageTypeProperties ADD CONSTRAINT RefVillageTypes384 
    FOREIGN KEY (VillageTypeID)
    REFERENCES VillageTypes(VillageTypeID)
go


/* 
 * TABLE: VillageTypePropertyTypes 
 */

ALTER TABLE VillageTypePropertyTypes ADD CONSTRAINT RefLevelPropertyTypes385 
    FOREIGN KEY (PropertyID)
    REFERENCES LevelPropertyTypes(PropertyID)
go


/* 
 * TABLE: VillageUnits 
 */

ALTER TABLE VillageUnits ADD CONSTRAINT FK_UnitTypes_VillageUnits 
    FOREIGN KEY (UnitTypeID)
    REFERENCES UnitTypes(UnitTypeID)
go

ALTER TABLE VillageUnits ADD CONSTRAINT FK_Villages_VillageUnits 
    FOREIGN KEY (VillageID)
    REFERENCES Villages(VillageID)
go





CREATE TABLE playerMapEventsStates(
    PlayerID    int    NOT NULL,
    TypeID     int    NOT NULL,
    StateData   varchar(max)    null, --not the same as a single event's data but rather the state data of that TYPE for that player
	StateData2 varchar(250),
	CONSTRAINT PK_playerMapEventsStates PRIMARY KEY NONCLUSTERED (PlayerID, TypeID)
)


CREATE TABLE VillageSpeedUpUsage(
    PlayerID    int    NOT NULL,
    VillageID    int    NOT NULL,
    TimeOfSpeedup     DateTime    NOT NULL,
    SpeedUpAmountInMin   int    NOT null
)

CREATE TABLE DefinedTargets(
	DefinedTargetID		int   IDENTITY(1,1),    
	PlayerID			int         NOT NULL,
    VillageID			int         NOT NULL,
    TimeCreated			datetime    NOT NULL,
	SetTime				datetime	null,
	DefinedTargetTypeID	smallint	not null,
	Note				varchar(max) null,
	ExpiresOn			datetime not null,
	AssignedTo			int	 null
    CONSTRAINT PK_DefinedTargets PRIMARY KEY NONCLUSTERED (DefinedTargetID)
)
go

CREATE TABLE DefinedTargetResponses(
	DefinedTargetID				int   not null,    
	PlayerID					int         NOT NULL,
	ResponseTypeID				smallint	not null,
	Response					varchar(max) null,
    TimeLastUpdated				datetime    NOT NULL,
    CONSTRAINT PK_DefinedTargetResponses PRIMARY KEY NONCLUSTERED (DefinedTargetID, PlayerID)
)
go



/*This view ensures that all players are listed with default values for the settings if not explicitly set*/
IF OBJECT_ID('vPlayerNotificationSettings') IS NOT NULL BEGIN
	drop view  vPlayerNotificationSettings
END 

go
create view  vPlayerNotificationSettings AS 
	SELECT		
		p.PlayerID,
	    t.NotificationID,
	    isnull(s.Vibrate, t.vibrate) as SilentAtNight,
	    isnull(s.SoundID, t.SoundID) SoundID,
		isnull(s.isActive, t.isActive) isActive
			
	from players p 	
	full join NotificationSettings_template t on t.NotificationID is not null
	full join  PlayerNotificationSettings S  on P.PlayerID = s.PlayerID and  S.NotificationID = t.NotificationID
	
	where p.PlayerID is not null 


go


--
-- translation table
--
IF OBJECT_ID('Translations') IS NOT NULL
BEGIN
    DROP TABLE Translations
END
go
Create Table Translations (Lang tinyint,Theme tinyint, [key] varchar(100), [value] nvarchar (MAX),  CONSTRAINT TranslationsPK19 PRIMARY KEY CLUSTERED ([key], Lang, Theme))


CREATE TABLE VillageCacheTimeStamps(
    PlayerID        int         NOT NULL,
    VillageID        int         NOT NULL,
    CachedItemID    smallint    NOT NULL, /*0 -- means something in village changed*/
    TimeStamp       datetime    NOT NULL,
    CONSTRAINT PK_VillageCacheTimeStamps PRIMARY KEY NONCLUSTERED (PlayerID, VillageID, CachedItemID)
)
go


CREATE TABLE CreditFarm_PlayerDailyChanceModifierFactor(
    PlayerID          int     NOT NULL,
    modifierFactor    real    NOT NULL,
    CONSTRAINT PK_CreditFarm_PlayerDailyChanceModifierFactor PRIMARY KEY NONCLUSTERED (PlayerID)
)
go



create table UsersToRealmChats
(
UserId uniqueidentifier not null,
RealmID int,
LastSeenMsg datetime not null,
)


GO



CREATE TABLE QuestTemplates_Reward_Items_Troops(
    TagName varchar(50)     NOT NULL,
	UnitTypeID int,
	Amount int
) ON [PRIMARY]


CREATE TABLE QuestTemplates_Reward_Items_PFWithDuration(
    TagName varchar(50)     NOT NULL,
	PFPackageID int,
	DurationInMinutes int
) ON [PRIMARY]

CREATE TABLE QuestTemplates_Descriptions(
    TagName varchar(50)     NOT NULL,
	UITypeID int,
    Description varchar(max) NULL
) ON [PRIMARY]


----
-- This view gives you claims that have not expired
----
IF OBJECT_ID('vPlayerVillageClaims_Active') IS NOT NULL BEGIN
	drop view  vPlayerVillageClaims_Active
END 
go
create view  [vPlayerVillageClaims_Active] AS 
    
select * from PlayerVillageClaims C
        where dateadd(hour, isnull((select settingint from PlayerVillageClaims_ClanSetting S where S.ClanID = C.ClanID and SettingID =3 ),999999), TimeOfClaim) > getdate()

go





--
-- RAIDS 
--


/* Raids table holds instances of actual raids attached to players or clans etc; this is an actual raid that is created, and once completed is removed */
CREATE TABLE Raids (
	RaidID			int not null  IDENTITY(1,1),   /*you can get the ID created, using @@IDENTITY in the next statement - I THINK!! See creating events, into Events table. insert into raids .... set @raidid = @@IDENTITY*/
	CreatedOn		DateTime not null,
	RaidTemplateID  int not null,
	Size			int null, /* Sum of all villages of players participating in this particular raid. null for solo raids */	
	PlayerCount		int not null, /* Count of how many players involved, 1 for solo, and X for clan or global */
	CONSTRAINT PK_RAIDS PRIMARY KEY NONCLUSTERED (RaidID)
)

CREATE TABLE PrivateRaids (
	RaidID			int not null,		
	PlayerID		int not null,		
)

ALTER TABLE PrivateRaids ADD CONSTRAINT FK_Raids_PrivateRaids 
    FOREIGN KEY (RaidID)
    REFERENCES Raids(RaidID)
go

ALTER TABLE PrivateRaids ADD CONSTRAINT FK_Players_PrivateRaids 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go

CREATE TABLE ClanRaids (
	RaidID			int not null,		
	PlayerID		int not null,		
	PlayerVillageCount		int not null,		
)

ALTER TABLE ClanRaids ADD CONSTRAINT FK_Raids_ClanRaids 
    FOREIGN KEY (RaidID)
    REFERENCES Raids(RaidID)
go

ALTER TABLE ClanRaids ADD CONSTRAINT FK_Players_ClanRaids 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/*
-- IF ONE TABLE
SELECT RaidID, PLyaerID, ClanID from RAIDS
IF PlayuerID NULL >> private
IF ClanDI NULL >> clan
ELSE >> global


-- IF muktiple RAID TABLEs
SELECT RaidID, PLyaerID, ClanID from RAIDS left join PrivateRaids left join ClanRaids
IF PlayuerID NULL >> private
IF ClanDI NULL >> clan
ELSE >> global
*/


CREATE TABLE RaidTemplate (
	RaidTemplateID 	INT NOT NULL,
	SpawnRate		INT NOT NULL, -- in minutes. time when it can spawn agai, since last time it was spawned. 
	Distance		float NOT NULL,
	RarityID		INT NOT NULL,
	ActByDuration	INT NULL ,	-- In minutes
	AvailableFromDay real NULL, 	-- both "available" params say when a raid shoudl be available, based on realm age 
	AvailableToDay real NULL, 	-- 
	TypeID			int null, -- 1 -- private-early, 2 - private-ongoing, 3 - clan
	CONSTRAINT PK_RaidTemplate PRIMARY KEY NONCLUSTERED (RaidTemplateID)
	
)

/*THIIS IS NOT FINISED!*/
CREATE TABLE RaidReward (
	RaidTemplateID 	INT NOT NULL,
	TypeID			INT NOT NULL,
	Count			INT NOT NULL
)

CREATE TABLE RaidMonsterTemplate (
	MonsterTemplateID 	INT NOT NULL,
	Name varchar(max) not null ,
	[Desc] varchar(max) not null,
	ImageUrlBG  varchar(max) not null,
	ImageUrlIcon  varchar(max) not null,
	CasultyPerc		int not null,
	BaseMaxHealth		int not null
	CONSTRAINT PK_RaidMonsterTemplate PRIMARY KEY NONCLUSTERED (MonsterTemplateID)
)



CREATE TABLE RaidMonster (
	MonsterTemplateID 	INT NOT NULL,
	RaidID				int not null,		
	CurrentHealth		int not null,
	MaxHealth			int not null
	CONSTRAINT PK_RaidMonster PRIMARY KEY NONCLUSTERED (MonsterTemplateID, RaidID)

)

ALTER TABLE RaidMonster ADD CONSTRAINT FK_Raids_RaidMonster
    FOREIGN KEY (RaidID)
    REFERENCES Raids(RaidID)
go

ALTER TABLE RaidMonster ADD CONSTRAINT FK_RaidMonsterTemplate_RaidMonster
    FOREIGN KEY (MonsterTemplateID)
    REFERENCES RaidMonsterTemplate(MonsterTemplateID)
go


CREATE TABLE RaidTemplatePossibleMonsters (
	RaidTemplateID 	INT NOT NULL,
	MonsterTemplateID 	INT NOT NULL,
)

ALTER TABLE RaidTemplatePossibleMonsters ADD CONSTRAINT FK_RaidMonsterTemplate_RaidTemplatePossibleMonsters
    FOREIGN KEY (MonsterTemplateID)
    REFERENCES RaidMonsterTemplate(MonsterTemplateID)
go

ALTER TABLE RaidTemplatePossibleMonsters ADD CONSTRAINT FK_RaidTemplate_RaidTemplatePossibleMonsters
    FOREIGN KEY (RaidTemplateID)
    REFERENCES RaidTemplate(RaidTemplateID)
go


CREATE TABLE RaidUnitMovements(
    EventID                 bigint      NOT NULL,
    PlayerID				int         NOT NULL,
    OriginVillageID         int         NOT NULL,
	RaidID					int not null,
	StartTime				datetime not null,
	LandTime				datetime not null
    CONSTRAINT PK_RaidUnitMovements PRIMARY KEY NONCLUSTERED (EventID)
)
/* TODO : add foreign keys for village, raid, eventid  */

CREATE TABLE RaidUnitsMoving(
    EventID                 bigint    NOT NULL,
    UnitTypeID              int       NOT NULL,
    UnitCount               int       NOT NULL,
    CONSTRAINT PK_RaidUnitsMoving PRIMARY KEY NONCLUSTERED (EventID, UnitTypeID)
)
/* TODO : add foreign keys for raid, unittypeid */


CREATE TABLE RaidResults (
	RaidID			int not null,
	PlayerID		int not null, /*TODO : add a foreign key*/
	LandTime		datetime not null, 
	DamageHP		int not null	
)
ALTER TABLE RaidResults ADD CONSTRAINT FK_Raids_RaidResults 
    FOREIGN KEY (RaidID)
    REFERENCES Raids(RaidID)
go


CREATE TABLE RaidRewardAcceptanceRecord (
	RaidID			int not null, /*TODO : add a foreign key*/
	PlayerID		int not null, /*TODO : add a foreign key*/
	AcceptedOn		datetime not null default (getdate())
)

create table ConsolidationLog 
	(
		EventTime DateTime not null default (getdate())
		, RowID int not null  IDENTITY(1,1)
		, consolidationNum	int  null
		, EventDesc varchar(max) not null 
		, EventDetails varchar(max) null
	)



CREATE TABLE [dbo].[PlayerStats_BestOfLifeStats](
	[PlayerID] [int] NOT NULL,
	[Name] [nvarchar](25) NULL,
	[HighestNumOfVillages] [int] NOT NULL,
	[HighestVillagePoints] [int] NOT NULL,
	[PointsAsAttacker] [int] NOT NULL,
	[PointsAsDefender] [int] NOT NULL,
	[GovKilledAsDefender] [int] NOT NULL,
	[ClanID] [int] NOT NULL,
	[ClanName] [nvarchar](30) NULL,
	[TopTitleID] [int] NOT NULL,
	[Sex] [smallint] NOT NULL,
	[PlayerStatus] [tinyint] NOT NULL,
	[LastActivity] [datetime] NOT NULL
) ON [PRIMARY]

GO
