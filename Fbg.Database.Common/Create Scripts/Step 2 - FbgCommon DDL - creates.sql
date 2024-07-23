use FbgCommon   
/* 
 * TABLE: AccountRecovery 
 */

CREATE TABLE AccountRecovery(
    ID             int                 IDENTITY(1,1),
    State          int                 NOT NULL,
    Email          nvarchar(256)       NOT NULL,
    UserId         uniqueidentifier    DEFAULT newid() NOT NULL,
    OldUserName    nvarchar(256)       NOT NULL,
    NewUserName    nvarchar(256)       NOT NULL,
    RequestDate    datetime            DEFAULT getdate() NOT NULL,
    ChangeDate     datetime            NULL,
    PlayerNames    nvarchar(max)       NULL,
    UID            nvarchar(256)       NOT NULL,
    CONSTRAINT PK113 PRIMARY KEY NONCLUSTERED (ID)
)
go



IF OBJECT_ID('AccountRecovery') IS NOT NULL
    PRINT '<<< CREATED TABLE AccountRecovery >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE AccountRecovery >>>'
go

/* 
 * TABLE: Avatars2 
 */

CREATE TABLE Avatars2(
    AvatarID      smallint        NOT NULL,
    AvatarType    int             DEFAULT 1 NOT NULL,
    ImageUrlS     varchar(256)    NOT NULL,
    ImageUrlL     varchar(256)    NOT NULL,
    Info          varchar(512)    NULL,
    cost          int             NULL,
    CONSTRAINT PK_Avatars2 PRIMARY KEY NONCLUSTERED (AvatarID)
)
go



IF OBJECT_ID('Avatars2') IS NOT NULL
    PRINT '<<< CREATED TABLE Avatars2 >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Avatars2 >>>'
go

/* 
 * TABLE: BadEmails 
 */

CREATE TABLE BadEmails(
    email             varchar(256)    NOT NULL,
    firstBouncedOn    datetime        DEFAULT getdate() NOT NULL,
    bounceType        varchar(20)     NULL,
    bounceTypeSub     varchar(20)     NULL,
    CONSTRAINT PK109 PRIMARY KEY NONCLUSTERED (email)
)
go



IF OBJECT_ID('BadEmails') IS NOT NULL
    PRINT '<<< CREATED TABLE BadEmails >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE BadEmails >>>'
go

/* 
 * TABLE: ChatMsgs2 
 */

CREATE TABLE ChatMsgs2(
    MsgID       bigint              IDENTITY(1,1),
    UserId      uniqueidentifier    NULL,
    PlayerID    int                 NULL,
    Text        nvarchar(256)       NULL,
    DateTime    datetime            NOT NULL,
    GroupId     uniqueidentifier    NOT NULL,
    CONSTRAINT PK_ChatMsgs2 PRIMARY KEY NONCLUSTERED (MsgID)
)
go



IF OBJECT_ID('ChatMsgs2') IS NOT NULL
    PRINT '<<< CREATED TABLE ChatMsgs2 >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ChatMsgs2 >>>'
go

/* 
 * TABLE: ChatMsgsBlockedUsers2 
 */

CREATE TABLE ChatMsgsBlockedUsers2(
    UserId             uniqueidentifier    DEFAULT newid() NULL,
    PlayerID           int                 NULL,
    BlockedUserID      uniqueidentifier    DEFAULT newid() NULL,
    BlockedPlayerId    int                 NULL
)
go



IF OBJECT_ID('ChatMsgsBlockedUsers2') IS NOT NULL
    PRINT '<<< CREATED TABLE ChatMsgsBlockedUsers2 >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ChatMsgsBlockedUsers2 >>>'
go

/* 
 * TABLE: CreditPackages 
 */

CREATE TABLE CreditPackages(
    CreditPackageID    int              NOT NULL,
    Credits            int              NOT NULL,
    RealCost           decimal(4, 2)    NULL,
    CONSTRAINT PK_CreditPackages1 PRIMARY KEY NONCLUSTERED (CreditPackageID)
)
go



IF OBJECT_ID('CreditPackages') IS NOT NULL
    PRINT '<<< CREATED TABLE CreditPackages >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE CreditPackages >>>'
go

/* 
 * TABLE: CreditPackages_Device 
 */

CREATE TABLE CreditPackages_Device(
    ProductID     varchar(256)    NOT NULL,
    Credits       int             NOT NULL,
    SaleType      int             DEFAULT 0 NOT NULL,
    Price         float           NOT NULL,
    DeviceType    int             NOT NULL,
    CONSTRAINT PK_CreditPackages1_1 PRIMARY KEY NONCLUSTERED (ProductID, SaleType)
)
go



IF OBJECT_ID('CreditPackages_Device') IS NOT NULL
    PRINT '<<< CREATED TABLE CreditPackages_Device >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE CreditPackages_Device >>>'
go

/* 
 * TABLE: CustomIDToUserID 
 */

CREATE TABLE CustomIDToUserID(
    CustomID    varchar(100)        NOT NULL,
    UserId      uniqueidentifier    DEFAULT newid() NOT NULL,
    CONSTRAINT PK78 PRIMARY KEY NONCLUSTERED (CustomID)
)
go



IF OBJECT_ID('CustomIDToUserID') IS NOT NULL
    PRINT '<<< CREATED TABLE CustomIDToUserID >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE CustomIDToUserID >>>'
go

/* 
 * TABLE: DeletedPlayers 
 */

CREATE TABLE DeletedPlayers(
    PlayerID        int                 NOT NULL,
    RealmID         int                 NOT NULL,
    UserId          uniqueidentifier    NOT NULL,
    Name            nvarchar(25)        NOT NULL,
    OriginalName    nvarchar(25)        NULL,
    CONSTRAINT PK_Players1_1 PRIMARY KEY NONCLUSTERED (PlayerID)
)
go



IF OBJECT_ID('DeletedPlayers') IS NOT NULL
    PRINT '<<< CREATED TABLE DeletedPlayers >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE DeletedPlayers >>>'
go

/* 
 * TABLE: Donors 
 */

CREATE TABLE Donors(
    UserId                        uniqueidentifier    DEFAULT newid() NOT NULL,
    RunningTotalDonationAmount    money               NOT NULL,
    WantsToBeAnonymous            smallint            NOT NULL,
    LastDonatedOn                 datetime            NOT NULL,
    CONSTRAINT PK_Donors1 PRIMARY KEY NONCLUSTERED (UserId)
)
go



IF OBJECT_ID('Donors') IS NOT NULL
    PRINT '<<< CREATED TABLE Donors >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Donors >>>'
go

/* 
 * TABLE: DummyPlayerNames 
 */

CREATE TABLE DummyPlayerNames(
    Name    bigint    IDENTITY(1,1)
)
go



IF OBJECT_ID('DummyPlayerNames') IS NOT NULL
    PRINT '<<< CREATED TABLE DummyPlayerNames >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE DummyPlayerNames >>>'
go

/* 
 * TABLE: EmailsSent 
 */

CREATE TABLE EmailsSent(
    EmailTypeID    int                 NOT NULL,
    UserID         uniqueidentifier    NOT NULL,
    RealmID        int                 NULL,
    Time           datetime            DEFAULT getdate() NOT NULL,
    email          varchar(max)        NOT NULL
)
go



IF OBJECT_ID('EmailsSent') IS NOT NULL
    PRINT '<<< CREATED TABLE EmailsSent >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE EmailsSent >>>'
go

/* 
 * TABLE: ErrorLog 
 */

CREATE TABLE ErrorLog(
    Time       datetime        NOT NULL,
    Number     int             NULL,
    Message    varchar(max)    NOT NULL,
    Date       varchar(max)    NULL
)
go



IF OBJECT_ID('ErrorLog') IS NOT NULL
    PRINT '<<< CREATED TABLE ErrorLog >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE ErrorLog >>>'
go

/* 
 * TABLE: FbCreditsTransactionLog 
 */

CREATE TABLE FbCreditsTransactionLog(
    RecordID       bigint              IDENTITY(1,1),
    OrderID        bigint              NOT NULL,
    Price          int                 NOT NULL,
    Credits        int                 NOT NULL,
    UserId         uniqueidentifier    DEFAULT newid() NOT NULL,
    DateCreated    datetime            DEFAULT getdate() NOT NULL,
    CONSTRAINT PK70 PRIMARY KEY NONCLUSTERED (RecordID)
)
go



IF OBJECT_ID('FbCreditsTransactionLog') IS NOT NULL
    PRINT '<<< CREATED TABLE FbCreditsTransactionLog >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE FbCreditsTransactionLog >>>'
go

/* 
 * TABLE: FriendInviteCodes 
 */

CREATE TABLE FriendInviteCodes(
    UserId              uniqueidentifier    DEFAULT newid() NOT NULL,
    friendinviteCode    varchar(10)         NOT NULL,
    CONSTRAINT PK128 PRIMARY KEY NONCLUSTERED (UserId)
)
go



IF OBJECT_ID('FriendInviteCodes') IS NOT NULL
    PRINT '<<< CREATED TABLE FriendInviteCodes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE FriendInviteCodes >>>'
go

/* 
 * TABLE: FriendInviteCodeUses 
 */

CREATE TABLE FriendInviteCodeUses(
    UserId                      uniqueidentifier    DEFAULT newid() NOT NULL,
    FriendWhoGotRewardUserID    uniqueidentifier    DEFAULT newid() NULL,
    RewardStage                 int                 NOT NULL,
    TimeUsed                    datetime            NOT NULL,
    CONSTRAINT PK129 PRIMARY KEY NONCLUSTERED (UserId)
)
go



IF OBJECT_ID('FriendInviteCodeUses') IS NOT NULL
    PRINT '<<< CREATED TABLE FriendInviteCodeUses >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE FriendInviteCodeUses >>>'
go

/* 
 * TABLE: Gifts 
 */

CREATE TABLE Gifts(
    GiftID        int    NOT NULL,
    GiftTypeID    int    NOT NULL,
    CONSTRAINT PK64 PRIMARY KEY NONCLUSTERED (GiftID)
)
go



IF OBJECT_ID('Gifts') IS NOT NULL
    PRINT '<<< CREATED TABLE Gifts >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Gifts >>>'
go

/* 
 * TABLE: GiftsRealmAvailability 
 */

CREATE TABLE GiftsRealmAvailability(
    RealmID    int    NOT NULL,
    GiftID     int    NOT NULL,
    CONSTRAINT PK63 PRIMARY KEY NONCLUSTERED (RealmID, GiftID)
)
go



IF OBJECT_ID('GiftsRealmAvailability') IS NOT NULL
    PRINT '<<< CREATED TABLE GiftsRealmAvailability >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE GiftsRealmAvailability >>>'
go

/* 
 * TABLE: GiftsSent 
 */

CREATE TABLE GiftsSent(
    RecordID     int             IDENTITY(1,1),
    PlayerID     int             NULL,
    GiftID       int             NOT NULL,
    SentTo       varchar(320)    NOT NULL,
    SentOn       datetime        NOT NULL,
    Type         smallint        NOT NULL,
    StatusID     smallint        NOT NULL,
    RequestID    varchar(25)     NULL,
    CONSTRAINT PK62 PRIMARY KEY NONCLUSTERED (RecordID)
)
go



IF OBJECT_ID('GiftsSent') IS NOT NULL
    PRINT '<<< CREATED TABLE GiftsSent >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE GiftsSent >>>'
go

/* 
 * TABLE: GroupChat2 
 */

CREATE TABLE GroupChat2(
    GroupId      uniqueidentifier    NOT NULL,
    Name         nvarchar(256)       DEFAULT '' NOT NULL,
    RealmID      int                 NULL,
    GroupType    tinyint             DEFAULT 0 NOT NULL,
    ClanID       int                 NULL,
    CONSTRAINT PK143 PRIMARY KEY NONCLUSTERED (GroupId)
)
go



IF OBJECT_ID('GroupChat2') IS NOT NULL
    PRINT '<<< CREATED TABLE GroupChat2 >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE GroupChat2 >>>'
go

/* 
 * TABLE: HTTP_USER_AGENTs 
 */

CREATE TABLE HTTP_USER_AGENTs(
    HTTP_USER_AGENT_ID    int             IDENTITY(1,1),
    HTTP_USER_AGENT       varchar(500)    NOT NULL,
    CONSTRAINT PK185 PRIMARY KEY NONCLUSTERED (HTTP_USER_AGENT_ID)
)
go



IF OBJECT_ID('HTTP_USER_AGENTs') IS NOT NULL
    PRINT '<<< CREATED TABLE HTTP_USER_AGENTs >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE HTTP_USER_AGENTs >>>'
go

/* 
 * TABLE: InactivePlayers_TempListFor_dInactivePlayers 
 */

CREATE TABLE InactivePlayers_TempListFor_dInactivePlayers(
    playerid    int    NOT NULL,
    realmid     int    NULL,
    CONSTRAINT PK117 PRIMARY KEY NONCLUSTERED (playerid)
)
go



IF OBJECT_ID('InactivePlayers_TempListFor_dInactivePlayers') IS NOT NULL
    PRINT '<<< CREATED TABLE InactivePlayers_TempListFor_dInactivePlayers >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE InactivePlayers_TempListFor_dInactivePlayers >>>'
go

/* 
 * TABLE: InactivePlayersToBeWarned 
 */

CREATE TABLE InactivePlayersToBeWarned(
    PlayerID           int         NOT NULL,
    FoundInactiveOn    datetime    NOT NULL,
    CONSTRAINT PK47 PRIMARY KEY NONCLUSTERED (PlayerID)
)
go



IF OBJECT_ID('InactivePlayersToBeWarned') IS NOT NULL
    PRINT '<<< CREATED TABLE InactivePlayersToBeWarned >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE InactivePlayersToBeWarned >>>'
go

/* 
 * TABLE: InviteRewards 
 */

CREATE TABLE InviteRewards(
    UserId           uniqueidentifier    DEFAULT newid() NOT NULL,
    Time             datetime            NOT NULL,
    NumInvites       int                 NOT NULL,
    InviteRewards    int                 NOT NULL,
    RewardNumber     int                 NOT NULL,
    Notes            varchar(max)        NULL
)
go



IF OBJECT_ID('InviteRewards') IS NOT NULL
    PRINT '<<< CREATED TABLE InviteRewards >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE InviteRewards >>>'
go

/* 
 * TABLE: Invites 
 */

CREATE TABLE Invites(
    InviteID     int             IDENTITY(1,1),
    PlayerID     int             NOT NULL,
    InvitedID    varchar(320)    NULL,
    Type         smallint        NOT NULL,
    InvitedOn    datetime        NOT NULL,
    StatusID     smallint        DEFAULT 1 NOT NULL,
    CONSTRAINT PK39 PRIMARY KEY NONCLUSTERED (InviteID)
)
go



IF OBJECT_ID('Invites') IS NOT NULL
    PRINT '<<< CREATED TABLE Invites >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Invites >>>'
go

/* 
 * TABLE: Items 
 */

CREATE TABLE Items(
    ItemId        bigint              IDENTITY(1,1),
    UserId        uniqueidentifier    DEFAULT newid() NOT NULL,
    PlayerID      int            NULL,
    ReceivedOn    datetime            NOT NULL,
    ExpiresOn     datetime            NULL,
    UsedOn        datetime            NULL,
    CONSTRAINT PK148 PRIMARY KEY NONCLUSTERED (ItemId)
)
go



IF OBJECT_ID('Items') IS NOT NULL
    PRINT '<<< CREATED TABLE Items >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Items >>>'
go

/* 
 * TABLE: Items_BuildingSpeedup 
 */

CREATE TABLE Items_BuildingSpeedup(
    ItemId           bigint    NOT NULL,
    MinutesAmount    int       NOT NULL,
    CONSTRAINT PK_Items_BuildingSpeedup PRIMARY KEY NONCLUSTERED (ItemId)
)
go



IF OBJECT_ID('Items_BuildingSpeedup') IS NOT NULL
    PRINT '<<< CREATED TABLE Items_BuildingSpeedup >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Items_BuildingSpeedup >>>'
go

/* 
 * TABLE: Items_PFWithDuration 
 */

CREATE TABLE Items_PFWithDuration(
    ItemId               bigint    NOT NULL,
    PFPackageID          int       NOT NULL,
    DurationInMinutes    int       NOT NULL,
    CONSTRAINT PK149 PRIMARY KEY NONCLUSTERED (ItemId)
)
go



IF OBJECT_ID('Items_PFWithDuration') IS NOT NULL
    PRINT '<<< CREATED TABLE Items_PFWithDuration >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Items_PFWithDuration >>>'
go

/* 
 * TABLE: Items_ResearchSpeedup 
 */

CREATE TABLE Items_ResearchSpeedup(
    ItemId           bigint    NOT NULL,
    MinutesAmount    int       NOT NULL,
    CONSTRAINT PK_Items_ResearchSpeedup PRIMARY KEY NONCLUSTERED (ItemId)
)
go



IF OBJECT_ID('Items_ResearchSpeedup') IS NOT NULL
    PRINT '<<< CREATED TABLE Items_ResearchSpeedup >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Items_ResearchSpeedup >>>'
go

/* 
 * TABLE: Items_Silver 
 */

CREATE TABLE Items_Silver(
    ItemId    bigint    NOT NULL,
    Silver    int       NOT NULL,
    CONSTRAINT PK150 PRIMARY KEY NONCLUSTERED (ItemId)
)
go



IF OBJECT_ID('Items_Silver') IS NOT NULL
    PRINT '<<< CREATED TABLE Items_Silver >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Items_Silver >>>'
go

/* 
 * TABLE: Items_Troops 
 */

CREATE TABLE Items_Troops(
    ItemId        bigint    NOT NULL,
    UnitTypeID    int       NOT NULL,
    Amount        int       NOT NULL,
    CONSTRAINT PK151 PRIMARY KEY NONCLUSTERED (ItemId)
)
go



IF OBJECT_ID('Items_Troops') IS NOT NULL
    PRINT '<<< CREATED TABLE Items_Troops >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Items_Troops >>>'
go

/* 
 * TABLE: LoginLog 
 */

CREATE TABLE LoginLog(
    RealmID               int            NOT NULL,
    PlayerID              int            NOT NULL,
    Time                  datetime       NOT NULL,
    REMOTE_ADDR           varchar(15)    NOT NULL,
    HTTP_USER_AGENT_ID    int            NOT NULL,
    REMOTE_PORT           bigint         NULL
)
go



IF OBJECT_ID('LoginLog') IS NOT NULL
    PRINT '<<< CREATED TABLE LoginLog >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE LoginLog >>>'
go

/* 
 * TABLE: OfferCompletedTransactionLog 
 */

CREATE TABLE OfferCompletedTransactionLog(
    RecordID          bigint          IDENTITY(1,1),
    ProviderID        smallint        NOT NULL,
    Date              datetime        NOT NULL,
    FacebookID        varchar(15)     NOT NULL,
    Credits           int             NOT NULL,
    AdditionalData    varchar(max)    NULL,
    CONSTRAINT PK43 PRIMARY KEY NONCLUSTERED (RecordID)
)
go



IF OBJECT_ID('OfferCompletedTransactionLog') IS NOT NULL
    PRINT '<<< CREATED TABLE OfferCompletedTransactionLog >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE OfferCompletedTransactionLog >>>'
go

/* 
 * TABLE: PaymentTransactionLog 
 */

CREATE TABLE PaymentTransactionLog(
    RecordID          bigint              IDENTITY(1,1),
    txn_id            varchar(17)         NOT NULL,
    UserId            uniqueidentifier    DEFAULT newid() NULL,
    Status            smallint            NOT NULL,
    RawData           varchar(max)        NOT NULL,
    Date              datetime            NOT NULL,
    payment_status    varchar(25)         NOT NULL,
    payment_date      varchar(30)         NOT NULL,
    parent_txn_id     varchar(17)         NULL,
    amount_gross      money               NOT NULL,
    amount_fee        money               NOT NULL,
    payer_email       varchar(255)        NULL,
    CONSTRAINT PK_PaymentTransactionLog1 PRIMARY KEY NONCLUSTERED (RecordID)
)
go



IF OBJECT_ID('PaymentTransactionLog') IS NOT NULL
    PRINT '<<< CREATED TABLE PaymentTransactionLog >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PaymentTransactionLog >>>'
go

/* 
 * TABLE: PaymentTransactionLog_amazon 
 */

CREATE TABLE PaymentTransactionLog_amazon(
    RecordID         bigint              IDENTITY(1,1),
    UserId           uniqueidentifier    DEFAULT newid() NOT NULL,
    status           int                 NOT NULL,
    sku              varchar(100)        NOT NULL,
    purchaseToken    varchar(max)        NOT NULL,
    DateCreated      datetime            DEFAULT getdate() NOT NULL,
    RawData          varchar(max)        NOT NULL,
    CONSTRAINT PK70_1_1 PRIMARY KEY NONCLUSTERED (RecordID)
)
go



IF OBJECT_ID('PaymentTransactionLog_amazon') IS NOT NULL
    PRINT '<<< CREATED TABLE PaymentTransactionLog_amazon >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PaymentTransactionLog_amazon >>>'
go

/* 
 * TABLE: PaymentTransactionLog_android 
 */

CREATE TABLE PaymentTransactionLog_android(
    RecordID            bigint              IDENTITY(1,1),
    UserId              uniqueidentifier    NOT NULL,
    purchaseState       tinyint             NOT NULL,
    notificationId      varchar(256)        NOT NULL,
    productId           varchar(256)        NOT NULL,
    orderId             varchar(256)        NOT NULL,
    purchaseTime        bigint              NOT NULL,
    developerPayload    varchar(max)        NULL,
    purchaseToken       varchar(256)        NULL,
    packageName         varchar(256)        NOT NULL,
    DateCreated         datetime            DEFAULT getdate() NOT NULL,
    RawData             varchar(max)        NOT NULL,
    CONSTRAINT PK70_1 PRIMARY KEY NONCLUSTERED (RecordID)
)
go



IF OBJECT_ID('PaymentTransactionLog_android') IS NOT NULL
    PRINT '<<< CREATED TABLE PaymentTransactionLog_android >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PaymentTransactionLog_android >>>'
go

/* 
 * TABLE: PaymentTransactionLog_ios 
 */

CREATE TABLE PaymentTransactionLog_ios(
    RecordID                       bigint              IDENTITY(1,1),
    UserId                         uniqueidentifier    DEFAULT newid() NOT NULL,
    quantity                       varchar(100)        NOT NULL,
    product_id                     varchar(100)        NOT NULL,
    transaction_id                 varchar(100)        NOT NULL,
    purchase_date                  varchar(100)        NOT NULL,
    original_transaction_id        varchar(100)        NOT NULL,
    original_purchase_date         varchar(100)        NOT NULL,
    app_item_id                    varchar(100)        NOT NULL,
    version_external_identifier    varchar(100)        NOT NULL,
    bid                            varchar(100)        NOT NULL,
    bvrs                           varchar(100)        NOT NULL,
    DateCreated                    datetime            DEFAULT getdate() NOT NULL,
    RawData                        varchar(max)        NOT NULL,
    CONSTRAINT PK93 PRIMARY KEY NONCLUSTERED (RecordID)
)
go



IF OBJECT_ID('PaymentTransactionLog_ios') IS NOT NULL
    PRINT '<<< CREATED TABLE PaymentTransactionLog_ios >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PaymentTransactionLog_ios >>>'
go

/* 
 * TABLE: PFEventTypes 
 */

CREATE TABLE PFEventTypes(
    EventType      smallint         NOT NULL,
    Name           nvarchar(100)    NOT NULL,
    Description    nvarchar(max)    NOT NULL,
    CONSTRAINT PK56 PRIMARY KEY NONCLUSTERED (EventType)
)
go



IF OBJECT_ID('PFEventTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE PFEventTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PFEventTypes >>>'
go

/* 
 * TABLE: PlayerQ 
 */

CREATE TABLE PlayerQ(
    UserId         uniqueidentifier    DEFAULT newid() NOT NULL,
    RealmID        int                 NOT NULL,
    DateEntered    datetime            NOT NULL,
    CONSTRAINT PK_PlayerQ1 PRIMARY KEY NONCLUSTERED (UserId, RealmID)
)
go



IF OBJECT_ID('PlayerQ') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerQ >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerQ >>>'
go

/* 
 * TABLE: Players 
 */

CREATE TABLE Players(
    PlayerID    int                 IDENTITY(1,1),
    RealmID     int                 NOT NULL,
    UserId      uniqueidentifier    NOT NULL,
    Name        nvarchar(25)        NOT NULL,
    AvatarID    smallint            NOT NULL,
    CONSTRAINT PK_Players1 PRIMARY KEY NONCLUSTERED (PlayerID)
)
go



IF OBJECT_ID('Players') IS NOT NULL
    PRINT '<<< CREATED TABLE Players >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Players >>>'
go

/* 
 * TABLE: PlayerSuspensions 
 */

CREATE TABLE PlayerSuspensions(
    UserId                uniqueidentifier    DEFAULT newid() NOT NULL,
    IsSuspensionActive    tinyint             NOT NULL,
    SuspendedUntil        datetime            NULL,
    SuspensionType        tinyint             NOT NULL,
    PublicReason          varchar(max)        NOT NULL,
    InternalNotes         varchar(max)        NOT NULL,
    SuspendedOn           datetime            DEFAULT getdate() NOT NULL,
    SuspendedBy           varchar(max)        NOT NULL,
    CONSTRAINT PK_PlayerSuspensions1 PRIMARY KEY NONCLUSTERED (UserId)
)
go



IF OBJECT_ID('PlayerSuspensions') IS NOT NULL
    PRINT '<<< CREATED TABLE PlayerSuspensions >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PlayerSuspensions >>>'
go

/* 
 * TABLE: PollOptions 
 */

CREATE TABLE PollOptions(
    PollID          int             NULL,
    PollOptionID    int             IDENTITY(1,1),
    Text            varchar(max)    NOT NULL,
    CONSTRAINT PK28 PRIMARY KEY NONCLUSTERED (PollOptionID)
)
go



IF OBJECT_ID('PollOptions') IS NOT NULL
    PRINT '<<< CREATED TABLE PollOptions >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PollOptions >>>'
go

/* 
 * TABLE: PollResponses 
 */

CREATE TABLE PollResponses(
    UserId          uniqueidentifier    DEFAULT newid() NOT NULL,
    PollOptionID    int                 NOT NULL,
    CONSTRAINT PK29 PRIMARY KEY NONCLUSTERED (UserId, PollOptionID)
)
go



IF OBJECT_ID('PollResponses') IS NOT NULL
    PRINT '<<< CREATED TABLE PollResponses >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE PollResponses >>>'
go

/* 
 * TABLE: Polls 
 */

CREATE TABLE Polls(
    PollID          int              NOT NULL,
    Title           nvarchar(max)    NOT NULL,
    Description     nvarchar(max)    NOT NULL,
    OfferType       int              NOT NULL,
    OfferAmount     int              NOT NULL,
    RealmID         int              NULL,
    PollType        smallint         NOT NULL,
    Run_StartOn     datetime         DEFAULT getdate() NOT NULL,
    Run_ForHours    int              DEFAULT 0 NOT NULL,
    CONSTRAINT PK27 PRIMARY KEY NONCLUSTERED (PollID)
)
go



IF OBJECT_ID('Polls') IS NOT NULL
    PRINT '<<< CREATED TABLE Polls >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Polls >>>'
go

/* 
 * TABLE: Realms 
 */

CREATE TABLE Realms(
    RealmID          int              NOT NULL,
    Name             nvarchar(100)    NOT NULL,
    Description      nvarchar(200)    NOT NULL,
    Version          varchar(50)      NOT NULL,
    SizeX            int              DEFAULT 100 NOT NULL,
    SizeY            int              DEFAULT 100 NOT NULL,
    ConnectionStr    varchar(500)     NOT NULL,
    MaxPlayers       int              DEFAULT 500 NOT NULL,
    OpenOn           datetime         NOT NULL,
    AllowPrereg      bit              NOT NULL,
    ActiveStatus     smallint         NOT NULL,
    ExtendedDesc     nvarchar(max)    NULL,
    EndsOn           datetime         NULL,
    CONSTRAINT PK_Realms1 PRIMARY KEY NONCLUSTERED (RealmID)
)
go



IF OBJECT_ID('Realms') IS NOT NULL
    PRINT '<<< CREATED TABLE Realms >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Realms >>>'
go

/* 
 * TABLE: StoriesPublished 
 */

CREATE TABLE StoriesPublished(
    PlayerID         int              NOT NULL,
    StoryID          smallint         NOT NULL,
    Body             nvarchar(max)    NULL,
    PublishedOn      datetime         DEFAULT getdate() NOT NULL,
    PublishStatus    smallint         NOT NULL
)
go



IF OBJECT_ID('StoriesPublished') IS NOT NULL
    PRINT '<<< CREATED TABLE StoriesPublished >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE StoriesPublished >>>'
go

/* 
 * TABLE: tr_PlayerListSettings 
 */

CREATE TABLE tr_PlayerListSettings(
    UserId           uniqueidentifier    DEFAULT newid() NOT NULL,
    PlayerID         int                 NOT NULL,
    displayStatus    int                 NOT NULL,
    CONSTRAINT PK138 PRIMARY KEY NONCLUSTERED (UserId, PlayerID)
)
go



IF OBJECT_ID('tr_PlayerListSettings') IS NOT NULL
    PRINT '<<< CREATED TABLE tr_PlayerListSettings >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE tr_PlayerListSettings >>>'
go

/* 
 * TABLE: UserDetails 
 */

CREATE TABLE UserDetails(
    UserId                        uniqueidentifier    DEFAULT newid() NOT NULL,
    Birthday                      datetime            NULL,
    Location_Country              varchar(max)        NULL,
    Location_City                 nvarchar(max)       NULL,
    Location_State                nvarchar(max)       NULL,
    Interests                     nvarchar(max)       NULL,
    Relationship_Status           tinyint             NULL,
    Religion                      nvarchar(100)       NULL,
    SignificantOtherFacebookID    varchar(20)         NULL,
    CONSTRAINT PK58 PRIMARY KEY NONCLUSTERED (UserId)
)
go



IF OBJECT_ID('UserDetails') IS NOT NULL
    PRINT '<<< CREATED TABLE UserDetails >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UserDetails >>>'
go

/* 
 * TABLE: UserFlags 
 */

CREATE TABLE UserFlags(
    UserId       uniqueidentifier    DEFAULT newid() NOT NULL,
    FlagID       smallint            NOT NULL,
    UpdatedOn    datetime            NOT NULL,
    Data         varchar(max)        NULL,
    CONSTRAINT PK124 PRIMARY KEY NONCLUSTERED (UserId, FlagID)
)
go



IF OBJECT_ID('UserFlags') IS NOT NULL
    PRINT '<<< CREATED TABLE UserFlags >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UserFlags >>>'
go

/* 
 * TABLE: UserLog 
 */

CREATE TABLE UserLog(
    Time        datetime            NOT NULL,
    UserID      uniqueidentifier    NULL,
    PlayerID    int                 NULL,
    EventID     smallint            NULL,
    Message     varchar(max)        NULL,
    Data        varchar(max)        NULL
)
go



IF OBJECT_ID('UserLog') IS NOT NULL
    PRINT '<<< CREATED TABLE UserLog >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UserLog >>>'
go

/* 
 * TABLE: UserLoginTypeIDs 
 */

CREATE TABLE UserLoginTypeIDs(
    UserLoginTypeID    smallint         NOT NULL,
    Name               nvarchar(100)    NOT NULL,
    CONSTRAINT PK125 PRIMARY KEY NONCLUSTERED (UserLoginTypeID)
)
go



IF OBJECT_ID('UserLoginTypeIDs') IS NOT NULL
    PRINT '<<< CREATED TABLE UserLoginTypeIDs >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UserLoginTypeIDs >>>'
go

/* 
 * TABLE: UserLoginTypes 
 */

CREATE TABLE UserLoginTypes(
    UserId             uniqueidentifier    DEFAULT newid() NOT NULL,
    UserLoginTypeID    smallint            NOT NULL,
    CONSTRAINT PK123 PRIMARY KEY NONCLUSTERED (UserId, UserLoginTypeID)
)
go



IF OBJECT_ID('UserLoginTypes') IS NOT NULL
    PRINT '<<< CREATED TABLE UserLoginTypes >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UserLoginTypes >>>'
go

/* 
 * TABLE: UserNotifications 
 */

CREATE TABLE UserNotifications(
    UserId            uniqueidentifier    DEFAULT newid() NOT NULL,
    Notifications     bit                 NOT NULL,
    RegistrationID    varchar(max)        NULL,
    DeviceType        int                 NOT NULL,
    CONSTRAINT PK85 PRIMARY KEY NONCLUSTERED (UserId)
)
go



IF OBJECT_ID('UserNotifications') IS NOT NULL
    PRINT '<<< CREATED TABLE UserNotifications >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UserNotifications >>>'
go

/* 
 * TABLE: UserNotificationsSent 
 */

CREATE TABLE UserNotificationsSent(
    NotifyTypeID      int                 NOT NULL,
    UserId            uniqueidentifier    DEFAULT newid() NOT NULL,
    RealmID           int                 NULL,
    Time              datetime            DEFAULT getdate() NOT NULL,
    RegistrationID    varchar(max)        NOT NULL,
    DeviceType        int                 NOT NULL
)
go



IF OBJECT_ID('UserNotificationsSent') IS NOT NULL
    PRINT '<<< CREATED TABLE UserNotificationsSent >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UserNotificationsSent >>>'
go

/* 
 * TABLE: UserNotificationsToSend 
 */

CREATE TABLE UserNotificationsToSend(
    RecordID              bigint              IDENTITY(1,1),
    NotificationTypeID    smallint            NOT NULL,
    UserId                uniqueidentifier    DEFAULT newid() NOT NULL,
    Title                 nvarchar(max)       NOT NULL,
    Text                  nvarchar(max)       NOT NULL,
    TimeCreated           datetime            DEFAULT getdate() NOT NULL,
    TimeSent              datetime            NULL,
    CONSTRAINT PK119 PRIMARY KEY NONCLUSTERED (RecordID)
)
go



IF OBJECT_ID('UserNotificationsToSend') IS NOT NULL
    PRINT '<<< CREATED TABLE UserNotificationsToSend >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UserNotificationsToSend >>>'
go

/* 
 * TABLE: UserPFLog 
 */

CREATE TABLE UserPFLog(
    UserID       uniqueidentifier    NOT NULL,
    Time         datetime            NOT NULL,
    EventType    smallint            NOT NULL,
    Credits      int                 NULL,
    Cost         int                 NULL,
    realmid      int                 NULL,
    VillageID    int                 NULL,
    Data         varchar(max)        NULL
)
go



IF OBJECT_ID('UserPFLog') IS NOT NULL
    PRINT '<<< CREATED TABLE UserPFLog >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UserPFLog >>>'
go

/* 
 * TABLE: Users 
 */

CREATE TABLE Users(
    UserId              uniqueidentifier    DEFAULT newid() NOT NULL,
    Credits             int                 NOT NULL,
    TimeZone            real                DEFAULT 0 NOT NULL,
    XP                  int                 DEFAULT 1 NOT NULL,
    GlobalPlayerName    nvarchar(25)        NULL,
    AvatarID            int                 NULL,
    Sex                 smallint            NULL,
    CONSTRAINT PK_Users1 PRIMARY KEY NONCLUSTERED (UserId)
)
go



IF OBJECT_ID('Users') IS NOT NULL
    PRINT '<<< CREATED TABLE Users >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE Users >>>'
go

/* 
 * TABLE: UsersFriends 
 */

CREATE TABLE UsersFriends(
    UserId          uniqueidentifier    DEFAULT newid() NOT NULL,
    FriendUserId    uniqueidentifier    DEFAULT newid() NOT NULL,
    CONSTRAINT PK74 PRIMARY KEY NONCLUSTERED (UserId, FriendUserId)
)
go



IF OBJECT_ID('UsersFriends') IS NOT NULL
    PRINT '<<< CREATED TABLE UsersFriends >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UsersFriends >>>'
go

/* 
 * TABLE: UsersToChats2 
 */

CREATE TABLE UsersToChats2(
    UserId         uniqueidentifier    DEFAULT newid() NOT NULL,
    PlayerID       int                 NULL,
    GroupId        uniqueidentifier    NOT NULL,
    LastSeenMsg    datetime            NOT NULL
)
go



IF OBJECT_ID('UsersToChats2') IS NOT NULL
    PRINT '<<< CREATED TABLE UsersToChats2 >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UsersToChats2 >>>'
go

/* 
 * TABLE: UsersUnlockedAvatars 
 */

CREATE TABLE UsersUnlockedAvatars(
    UserId      uniqueidentifier    DEFAULT newid() NOT NULL,
    AvatarID    smallint            NOT NULL,
    CONSTRAINT PK_UsersUnlockedAvatars PRIMARY KEY NONCLUSTERED (UserId, AvatarID)
)
go



IF OBJECT_ID('UsersUnlockedAvatars') IS NOT NULL
    PRINT '<<< CREATED TABLE UsersUnlockedAvatars >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UsersUnlockedAvatars >>>'
go

/* 
 * TABLE: UserXPHistory 
 */

CREATE TABLE UserXPHistory(
    UserId           uniqueidentifier    DEFAULT newid() NULL,
    Time             datetime            DEFAULT getdate() NOT NULL,
    XPReceived       int                 NOT NULL,
    XPFromTypeID     smallint            NOT NULL,
    XPFromRealmID    int                 NULL
)
go



IF OBJECT_ID('UserXPHistory') IS NOT NULL
    PRINT '<<< CREATED TABLE UserXPHistory >>>'
ELSE
    PRINT '<<< FAILED CREATING TABLE UserXPHistory >>>'
go

/* 
 * INDEX: UK_TranID 
 */

CREATE UNIQUE INDEX UK_TranID ON PaymentTransactionLog(txn_id, payment_status)
go
IF EXISTS (SELECT * FROM sysindexes WHERE id=OBJECT_ID('PaymentTransactionLog') AND name='UK_TranID')
    PRINT '<<< CREATED INDEX PaymentTransactionLog.UK_TranID >>>'
ELSE
    PRINT '<<< FAILED CREATING INDEX PaymentTransactionLog.UK_TranID >>>'
go

/* 
 * INDEX: UK_RealmID_UserID 
 */

CREATE UNIQUE INDEX UK_RealmID_UserID ON Players(RealmID, UserId)
go
IF EXISTS (SELECT * FROM sysindexes WHERE id=OBJECT_ID('Players') AND name='UK_RealmID_UserID')
    PRINT '<<< CREATED INDEX Players.UK_RealmID_UserID >>>'
ELSE
    PRINT '<<< FAILED CREATING INDEX Players.UK_RealmID_UserID >>>'
go

/* 
 * TABLE: AccountRecovery 
 */

ALTER TABLE AccountRecovery ADD CONSTRAINT Refaspnet_Users89 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go


/* 
 * TABLE: ChatMsgs2 
 */

ALTER TABLE ChatMsgs2 ADD CONSTRAINT RefUsers120 
    FOREIGN KEY (UserId)
    REFERENCES Users(UserId)
go

ALTER TABLE ChatMsgs2 ADD CONSTRAINT RefGroupChat2128 
    FOREIGN KEY (GroupId)
    REFERENCES GroupChat2(GroupId)
go


/* 
 * TABLE: ChatMsgsBlockedUsers2 
 */

ALTER TABLE ChatMsgsBlockedUsers2 ADD CONSTRAINT RefUsers129 
    FOREIGN KEY (BlockedUserID)
    REFERENCES Users(UserId)
go

ALTER TABLE ChatMsgsBlockedUsers2 ADD CONSTRAINT RefPlayers130 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go

ALTER TABLE ChatMsgsBlockedUsers2 ADD CONSTRAINT RefPlayers131 
    FOREIGN KEY (BlockedPlayerId)
    REFERENCES Players(PlayerID)
go

ALTER TABLE ChatMsgsBlockedUsers2 ADD CONSTRAINT RefUsers132 
    FOREIGN KEY (UserId)
    REFERENCES Users(UserId)
go


/* 
 * TABLE: CustomIDToUserID 
 */

ALTER TABLE CustomIDToUserID ADD CONSTRAINT Refaspnet_Users76 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go


/* 
 * TABLE: Donors 
 */

ALTER TABLE Donors ADD CONSTRAINT FK_aspnet_Users_Donors 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go


/* 
 * TABLE: FbCreditsTransactionLog 
 */

ALTER TABLE FbCreditsTransactionLog ADD CONSTRAINT Refaspnet_Users64 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go


/* 
 * TABLE: FriendInviteCodes 
 */

ALTER TABLE FriendInviteCodes ADD CONSTRAINT RefUsers106 
    FOREIGN KEY (UserId)
    REFERENCES Users(UserId)
go


/* 
 * TABLE: FriendInviteCodeUses 
 */

ALTER TABLE FriendInviteCodeUses ADD CONSTRAINT RefUsers107 
    FOREIGN KEY (FriendWhoGotRewardUserID)
    REFERENCES Users(UserId)
go

ALTER TABLE FriendInviteCodeUses ADD CONSTRAINT RefUsers108 
    FOREIGN KEY (UserId)
    REFERENCES Users(UserId)
go


/* 
 * TABLE: GiftsRealmAvailability 
 */

ALTER TABLE GiftsRealmAvailability ADD CONSTRAINT RefGifts59 
    FOREIGN KEY (GiftID)
    REFERENCES Gifts(GiftID)
go

ALTER TABLE GiftsRealmAvailability ADD CONSTRAINT RefRealms60 
    FOREIGN KEY (RealmID)
    REFERENCES Realms(RealmID)
go


/* 
 * TABLE: GiftsSent 
 */

ALTER TABLE GiftsSent ADD CONSTRAINT RefGifts61 
    FOREIGN KEY (GiftID)
    REFERENCES Gifts(GiftID)
go

ALTER TABLE GiftsSent ADD CONSTRAINT RefPlayers62 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: GroupChat2 
 */

ALTER TABLE GroupChat2 ADD CONSTRAINT RefRealms133 
    FOREIGN KEY (RealmID)
    REFERENCES Realms(RealmID)
go


/* 
 * TABLE: InactivePlayersToBeWarned 
 */

ALTER TABLE InactivePlayersToBeWarned ADD CONSTRAINT RefPlayers47 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: InviteRewards 
 */

ALTER TABLE InviteRewards ADD CONSTRAINT Refaspnet_Users53 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go


/* 
 * TABLE: Invites 
 */

ALTER TABLE Invites ADD CONSTRAINT FK_Players_Invites 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: Items 
 */

ALTER TABLE Items ADD CONSTRAINT RefUsers140 
    FOREIGN KEY (UserId)
    REFERENCES Users(UserId)
go


/* 
 * TABLE: Items_BuildingSpeedup 
 */

ALTER TABLE Items_BuildingSpeedup ADD CONSTRAINT RefItems149 
    FOREIGN KEY (ItemId)
    REFERENCES Items(ItemId)
go


/* 
 * TABLE: Items_PFWithDuration 
 */

ALTER TABLE Items_PFWithDuration ADD CONSTRAINT RefItems141 
    FOREIGN KEY (ItemId)
    REFERENCES Items(ItemId)
go


/* 
 * TABLE: Items_ResearchSpeedup 
 */

ALTER TABLE Items_ResearchSpeedup ADD CONSTRAINT RefItems150 
    FOREIGN KEY (ItemId)
    REFERENCES Items(ItemId)
go


/* 
 * TABLE: Items_Silver 
 */

ALTER TABLE Items_Silver ADD CONSTRAINT RefItems142 
    FOREIGN KEY (ItemId)
    REFERENCES Items(ItemId)
go


/* 
 * TABLE: Items_Troops 
 */

ALTER TABLE Items_Troops ADD CONSTRAINT RefItems143 
    FOREIGN KEY (ItemId)
    REFERENCES Items(ItemId)
go


/* 
 * TABLE: PaymentTransactionLog 
 */

ALTER TABLE PaymentTransactionLog ADD CONSTRAINT FK_aspnet_Users_PaymentTransactionLog 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go


/* 
 * TABLE: PaymentTransactionLog_amazon 
 */

ALTER TABLE PaymentTransactionLog_amazon ADD CONSTRAINT Refaspnet_Users91 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go


/* 
 * TABLE: PaymentTransactionLog_android 
 */

ALTER TABLE PaymentTransactionLog_android ADD CONSTRAINT Refaspnet_Users78 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go


/* 
 * TABLE: PaymentTransactionLog_ios 
 */

ALTER TABLE PaymentTransactionLog_ios ADD CONSTRAINT Refaspnet_Users84 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go


/* 
 * TABLE: PlayerQ 
 */

ALTER TABLE PlayerQ ADD CONSTRAINT FK_aspnet_Users_PlayerQ 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go

ALTER TABLE PlayerQ ADD CONSTRAINT FK_Realms_PlayerQ 
    FOREIGN KEY (RealmID)
    REFERENCES Realms(RealmID)
go


/* 
 * TABLE: Players 
 */

ALTER TABLE Players ADD CONSTRAINT RefAvatars2151 
    FOREIGN KEY (AvatarID)
    REFERENCES Avatars2(AvatarID)
go

ALTER TABLE Players ADD CONSTRAINT FK_aspnet_Users_Players 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go

ALTER TABLE Players ADD CONSTRAINT FK_Realms_Players 
    FOREIGN KEY (RealmID)
    REFERENCES Realms(RealmID)
go


/* 
 * TABLE: PlayerSuspensions 
 */

ALTER TABLE PlayerSuspensions ADD CONSTRAINT FK_aspnet_Users_PlayerSuspensions 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go


/* 
 * TABLE: PollOptions 
 */

ALTER TABLE PollOptions ADD CONSTRAINT FK_Polls_PollOptions 
    FOREIGN KEY (PollID)
    REFERENCES Polls(PollID)
go


/* 
 * TABLE: PollResponses 
 */

ALTER TABLE PollResponses ADD CONSTRAINT FK_aspnet_Users_PollResponses 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go

ALTER TABLE PollResponses ADD CONSTRAINT FK_PollOptions_PollResponses 
    FOREIGN KEY (PollOptionID)
    REFERENCES PollOptions(PollOptionID)
go


/* 
 * TABLE: Polls 
 */

ALTER TABLE Polls ADD CONSTRAINT FK_Realms_Polls 
    FOREIGN KEY (RealmID)
    REFERENCES Realms(RealmID)
go


/* 
 * TABLE: StoriesPublished 
 */

ALTER TABLE StoriesPublished ADD CONSTRAINT FK_Players_StoriesPublished 
    FOREIGN KEY (PlayerID)
    REFERENCES Players(PlayerID)
go


/* 
 * TABLE: tr_PlayerListSettings 
 */

ALTER TABLE tr_PlayerListSettings ADD CONSTRAINT RefUsers118 
    FOREIGN KEY (UserId)
    REFERENCES Users(UserId)
go


/* 
 * TABLE: UserDetails 
 */

ALTER TABLE UserDetails ADD CONSTRAINT RefUsers51 
    FOREIGN KEY (UserId)
    REFERENCES Users(UserId)
go


/* 
 * TABLE: UserFlags 
 */

ALTER TABLE UserFlags ADD CONSTRAINT Refaspnet_Users49 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go


/* 
 * TABLE: UserLoginTypes 
 */

ALTER TABLE UserLoginTypes ADD CONSTRAINT RefUserLoginTypeIDs96 
    FOREIGN KEY (UserLoginTypeID)
    REFERENCES UserLoginTypeIDs(UserLoginTypeID)
go

ALTER TABLE UserLoginTypes ADD CONSTRAINT Refaspnet_Users97 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go


/* 
 * TABLE: UserNotifications 
 */

ALTER TABLE UserNotifications ADD CONSTRAINT RefUsers86 
    FOREIGN KEY (UserId)
    REFERENCES Users(UserId)
go


/* 
 * TABLE: UserNotificationsSent 
 */

ALTER TABLE UserNotificationsSent ADD CONSTRAINT Refaspnet_Users87 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go


/* 
 * TABLE: UserNotificationsToSend 
 */

ALTER TABLE UserNotificationsToSend ADD CONSTRAINT RefUsers93 
    FOREIGN KEY (UserId)
    REFERENCES Users(UserId)
go


/* 
 * TABLE: Users 
 */

ALTER TABLE Users ADD CONSTRAINT FK_aspnet_Users_Users 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go


/* 
 * TABLE: UsersFriends 
 */

ALTER TABLE UsersFriends ADD CONSTRAINT Refaspnet_Users71 
    FOREIGN KEY (FriendUserId)
    REFERENCES aspnet_Users(UserId)
go

ALTER TABLE UsersFriends ADD CONSTRAINT Refaspnet_Users72 
    FOREIGN KEY (UserId)
    REFERENCES aspnet_Users(UserId)
go


/* 
 * TABLE: UsersToChats2 
 */

ALTER TABLE UsersToChats2 ADD CONSTRAINT RefGroupChat2134 
    FOREIGN KEY (GroupId)
    REFERENCES GroupChat2(GroupId)
go

ALTER TABLE UsersToChats2 ADD CONSTRAINT RefUsers135 
    FOREIGN KEY (UserId)
    REFERENCES Users(UserId)
go


/* 
 * TABLE: UsersUnlockedAvatars 
 */

ALTER TABLE UsersUnlockedAvatars ADD CONSTRAINT RefAvatars2152 
    FOREIGN KEY (AvatarID)
    REFERENCES Avatars2(AvatarID)
go

ALTER TABLE UsersUnlockedAvatars ADD CONSTRAINT RefUsers153 
    FOREIGN KEY (UserId)
    REFERENCES Users(UserId)
go


/* 
 * TABLE: UserXPHistory 
 */

ALTER TABLE UserXPHistory ADD CONSTRAINT RefUsers74 
    FOREIGN KEY (UserId)
    REFERENCES Users(UserId)
go



CREATE TABLE VillageStartLevels_BuildingSpeedup -- MUST BE ADDED TO THE MODEL!!
( 
    StartLevelID		int    NOT NULL,
    minuntesOfSpeedup   int    NOT NULL,
	realmID int not null    
)
go
CREATE TABLE VillageStartLevels_ResearchSpeedup -- MUST BE ADDED TO THE MODEL!!
(
    StartLevelID		int    NOT NULL,
    minuntesOfSpeedup   int    NOT NULL,
	realmID int not null    
)
go
CREATE TABLE PlayerStartLevels(
    StartLevelID         int         NOT NULL,
    RealmMaxAgeInDays    smallint    NOT NULL,
		realmID int not null    
    CONSTRAINT PK_PlayerStartLevels PRIMARY KEY NONCLUSTERED (StartLevelID, realmID)
)
go


use FbgCommon
go
create view vPlayers as 
select playerid, realmid, userid, name from players 
union
select playerid, realmid, userid, isnull(OriginalName, name)  from deletedplayers
go


IF OBJECT_ID('vFor_qGetPPLToNotify_Abandoned') IS NOT NULL BEGIN
	drop view  vFor_qGetPPLToNotify_Abandoned
END 

go
create view  vFor_qGetPPLToNotify_Abandoned AS 
	select 
	    U.UserID
		, M.loweredemail
		, UN.RegistrationID
		, UN.DeviceType	    
		, u.timezone
		, Cp.PlayerId
		, CP.RealmID
    from Players CP with(nolock) 
	    join aspnet_membership M with(nolock) on M.UserID = CP.UserID 
	    join users U with(nolock) on U.UserID = CP.UserID
		join UserNotifications UN with(nolock) on UN.UserId = U.UserId
		join InactivePlayersToBeWarned W with(nolock) on CP.PlayerID = W.PlayerID
    where 	
        -- Send this email only once every 30 days
		UN.Notifications = 1

		and not exists (select * from UserNotificationsSent with(nolock) where NotifyTypeID = 3 and UserID = U.UserID and realmID = CP.RealmID and dateadd(day, 30, [time]) > getdate()) 

GO





IF OBJECT_ID('GlobalPlayerRanking ') IS NOT NULL BEGIN
	drop table GlobalPlayerRanking 
END 
go
create table GlobalPlayerRanking 
(
	RealmID int
	, RealmOpenOn DateTime
	, RealmCloseOn DateTime
	, DateStatsCaptured DateTime
	, userid uniqueidentifier
	, PlayerName nvarchar(25)
	, NumberOfVillages int
	, TotalPoint int
	, AveragePoints int
	, ClanName nvarchar(30)
	, TitleID int
	, PointsAsAttacker int
	, PointsAsDefender int 
	, GovKilledAsDefender int
	, NumBonusVillages int
	, RankByNumberOfVillages int
)
go


IF OBJECT_ID('GlobalPlayerRanking_TotalSeasonPoints ') IS NOT NULL BEGIN
	drop table GlobalPlayerRanking_TotalSeasonPoints
END 
go
create table GlobalPlayerRanking_TotalSeasonPoints 
(
	season_id varchar(100)
	, season_start DateTime
	, season_end DateTime
	, DateStatsCaptured DateTime
	, userid uniqueidentifier
	, PlayerName nvarchar(25)
	, TotalPoints int
	 CONSTRAINT PK_GlobalPlayerRanking_TotalSeasonPoints PRIMARY KEY NONCLUSTERED (season_id, userid)
)
go


IF OBJECT_ID('OfferCompletedTransactionLog2') IS NOT NULL
BEGIN
    DROP TABLE OfferCompletedTransactionLog2
END
go
CREATE TABLE OfferCompletedTransactionLog2(
    RecordID          bigint          IDENTITY(1,1),
    ProviderID        smallint        NOT NULL,
    Date              datetime        NOT NULL,
    UserID			  uniqueidentifier     NOT NULL,
    Credits           int             NOT NULL,
    TransactionID     varchar(max)        NOT NULL,
    AdditionalData    varchar(max)    NULL,
    TransactionStatus    smallint    NULL, -- 1 OK, -1 userid wrong, -2 duplicate transaction
    CONSTRAINT PK_OfferCompletedTransactionLog2 PRIMARY KEY NONCLUSTERED (RecordID)
)
go


create table tr_likes 
(
	UserID uniqueidentifier not null ,
	PlayerID int  null ,
	LikerIP varchar(25)
)

IF OBJECT_ID('NewsletterRecipientCandidates') IS NOT NULL
BEGIN
    DROP TABLE NewsletterRecipientCandidates
END
go
create table NewsletterRecipientCandidates (
	BatchTag varchar(50) not null
	, UserID uniqueidentifier not null
	, Name varchar(25) null 
	, IgnoreOptOut bit not null default(0)
	, TimeCreated Datetime not null default(getdate())
	, SentOn DateTime null 
	, CCID int null
	, CCM varchar(max) null 
	, CCD varchar(max) null 
	, Data1 varchar(max) null 
	, Data2 varchar(max) null 
	, Data3 varchar(max) null 
	, Data4 varchar(max) null 
	, Data5 varchar(max) null 
)



--
-- translation table
--
IF OBJECT_ID('Translations') IS NOT NULL
BEGIN
    DROP TABLE Translations
END
go
Create Table Translations (Lang tinyint,Theme tinyint, [key] varchar(100), [value] nvarchar (MAX),  CONSTRAINT TranslationsPK19 PRIMARY KEY CLUSTERED ([key], Lang, Theme))


CREATE TABLE GameAttributes(
    AttribID       int             NOT NULL,
    AttribValue    varchar(max)    NOT NULL,
    AttribDesc     varchar(max)    NULL,
    CONSTRAINT PK231 PRIMARY KEY NONCLUSTERED (AttribID)
)
go



CREATE TABLE UserSuspensions(
    UserID		   uniqueidentifier    NOT NULL,
    PlayerID       int    not NULL,
    SupensionID    int    NOT NULL,
    CONSTRAINT PK_UserSuspensions PRIMARY KEY NONCLUSTERED (UserID, PlayerID, SupensionID)
)
go




CREATE TABLE Realms_PlayerGenerated (
    RealmID          int              NOT NULL,
    userid			uniqueidentifier NOT NULL,
    entrypasscode     nvarchar(max)    NULL,
    CONSTRAINT PK_Realms_PlayerGenerated1 PRIMARY KEY NONCLUSTERED (RealmID)
)
go

ALTER TABLE Realms_PlayerGenerated ADD CONSTRAINT FK_Realms_PlayerGenerated_Realms 
    FOREIGN KEY (RealmID)
    REFERENCES Realms(RealmID)
go


create table NPCs 
(
	userid			uniqueidentifier NOT NULL,
	tsWhenUserLastTriedToRegisteredOnRealm DateTime not null default(getdate()),
	CONSTRAINT PK_NPCs PRIMARY KEY NONCLUSTERED (userid)
)