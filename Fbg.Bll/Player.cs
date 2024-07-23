using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Linq;
using System.Dynamic;
//using System.Web.Script.Serialization;

namespace Fbg.Bll
{
    [Serializable]
    public partial class Player : Gmbc.Common.GmbcBaseClass, Gmbc.Common.Diagnostics.ExceptionManagement.ISerializableToNameValueCollection2
    {
        public class CONSTS
        {
            public class IncomingAttackColIndex
            {
                public static int count = 0;
                public static int EventTime = 1;
            }

            public class FlagsColIndex
            {
                public static int FlagID = 0;
                public static int UpdatedOn = 1;
                public static int Data = 2;
            }

            public class ActivatedPFTrialsColIndex
            {
                public static int PFTrialID = 0;
                public static int ExpiresOn = 1;
            }

            public class ExtraInfoTableIndex
            {
                public const int Flags = 0;
                public const int Friends = 1;
                public const int ActivatedPFTrials = 2;
                public const int PlayerInfo = 3;
                public const int ActiveStewardInfo = 4;
                public const int CommonPlayerInfo = 5;

            }
            public class PlayerPFPackageColIndex
            {
                public static int PFPackageID = 0;
                public static int Cost = 1;
                public static int Duration = 2;
                public static int ExpiresOn = 3;

            }
            public class PlayerActiveFeaturesColIndex
            {
                public static int FeatureID = 0;
                public static int ExpiresOn = 1;

            }
            public class PlayerActiveFeaturesColName
            {
                public static string FeatureID = "FeatureID";

            }
            public class PlayerAllFeaturesColIndex
            {
                public static int FeatureID = 0;
                public static int ExpiresOn = 1;

            }
            public class PlayerAllFeaturesColName
            {
                public static string FeatureID = "FeatureID";

            }
            public class PlayerFriendsColName
            {
                public static string FriendPlayerID = "PlayerID";

            }
            public class PlayerInfoColIndex
            {
                /// <summary>
                /// column of datatype DateTime
                /// </summary>
                public static int RegisteredOn = 0;
                /// <summary>
                /// column of Datatype bool
                /// </summary>
                public static int Anonymous = 1;
                /// <summary>
                /// column of datatype int
                /// </summary>
                public static int TitleID = 2;
                /// <summary>
                /// column datatype smallint /short/int16
                /// </summary>
                public static int Sex = 3;
                /// <summary>
                /// datatype DateTime
                /// </summary>
                public static int NightBuildActivatedOn = 4;
                public static int AvatarID = 5;
                public static int Morale = 6;
                public static int MoraleLastUpdated = 7;


            }

            public class CommonPlayerInfoColIndex
            {
                /// <summary>
                /// avatarID from Common.players table
                /// </summary>
                public static int AvatarID = 0;


            }         

            public class PlayerExtreaInfoColIndex
            {
                public static int MiscInfo = 0;
                public static int VillCount = 1;
                public static int CacheStamps = 2;
                public static int VillageCacheStamps = 3;
                public static int Raids = 4;
                public static int Research_1_Optional = 5;
                public static int Research_2_Optional = 6;
                public static int NUM_TABLES_INDICATING_RESERARCH_RETRIVED = 6;
            }

            /// <summary>
            /// Descibes the table returned from GetMyStewards()
            /// </summary>
            public class MyStewardsColIndex
            {
                public static int StewardPlayerID = 0;
                public static int StewardPlayerName = 1;
                public static int RecordID = 2;
                public static int State = 3;
            }

            /// <summary>
            /// Describes the table returned from GetMyStewardship()
            /// </summary>
            public class MyStewardshipColIndex
            {
                public static int AccountOwnerPlayerID = 0;
                public static int AccountOwnerPlayerName = 1;
                public static int RecordID = 2;
                public static int State = 3;
                public static int AccountOwnerUserID = 4;
            }

            public static int NightBuildDurationInMinutes = 15;
            public static int NightBuildCycleInHours = 24;
            /// <summary>
            /// Start Flag IDs that belong to achievements at this number
            /// </summary>
            public static int AchievementsFlagStartOffset = 1000;
            /// <summary>
            /// Start Flag IDs that belong to achievements at this number
            /// </summary>
            public static int QuestsFlagStartOffset = 2000;

            /// <summary>
            /// number between 0 and 1
            /// </summary>
            public static double PFPackageRefundPercentage = 0.75;//this value determine the precentage cutoff from the servants returned

            public class RecentTargetsColIndex
            {
                public static int OwnerName = 0;
                public static int VillageName = 1;
                public static int XCord = 2;
                public static int YCord = 3;
                /// <summary>
                /// if null, then not special player. otherwise, the type of special player
                /// </summary>
                public static int SpecialPlayerType = 4;
            }

            public const int RESTART_COST = 50;
        }
        

        public enum StatIDTypes
        {
            NumberOfVillages = 1,
            TotalVillagePoints = 2,
            AttackPoints = 3,
            DefencePoints = 4
        }

        public enum CacheItemIDs
        {
            incomingTroops = 1,
            outgoingTroops = 2
        }

        public enum Flags
        {
            Notification_NewMsgSeen = 0
            ,
            Notification_NewReportSeen = 1
          ,
            Notification_NewClanForumPostSeen = 2
          ,
            Advisor_BeginnerTutorialCompleted = 3
          ,
            Advisor_SilverMine10 = 4
          ,
            Advisor_Barracks1 = 5
          ,
            Advisor_RecruitCMSeen = 6
          ,
            Advisor_Clan = 7
          ,
            Advisor_SilverMine15 = 8
          ,
            Advisor_Tavern = 9
          ,
            Advisor_RecruitSpies = 10
          ,
            Advisor_BuildStable = 11
          ,
            Advisor_RecruitLC = 12
          ,
            Advisor_RecruitSpies10 = 13
          ,
            Advisor_SpyonVillages = 14
          ,
            Advisor_Wall = 15
          ,
            Advisor_DefensiveTowers = 16
          ,
            Advisor_Barracks15 = 17
          ,
            Advisor_Stable = 18
          ,
            Advisor_HQLevelUpgrade = 19
          ,
            Advisor_SiegeWorkshopNotBuilt = 20
          ,
            Advisor_SiegeWorkshopBuilt = 21
          ,
            Advisor_SilverMine35 = 22
          ,
            Advisor_RecruitEnoughTroops = 23
          ,
            Advisor_Palace = 24
          ,
            Advisor_Governor = 25
          ,
            Advisor_TradePost = 26
          ,
            Advisor_Specialize = 27
          ,
            Advisor_End = 28

            ,
            /// <summary>
            /// Tutorial quest completed
            /// </summary>
            Quests_TutorialQ = 29,
            /// <summary>
            /// Advisor quest completed
            /// </summary>
            Quests_SMLvl3Q = 30,
            /// <summary>
            /// advisor button was clicked
            /// </summary>
            Quests_AdvisorAccessed = 31,
            /// <summary>
            /// Invite button was clicked
            /// </summary>
            Quests_InviteClicked = 32,
            /// <summary>
            /// invite quest completed
            /// </summary>
            Quests_InviteQ = 33,
            /// <summary>
            /// how many friends do i have quest complete
            /// </summary>
            Quests_FriendsQ = 34,
            /// <summary>
            /// how many points do i have quest completed
            /// </summary>
            Quests_MyPointsQ = 35,
            /// <summary>
            /// how much silver completed
            /// </summary>
            Quests_SilverQ = 36,
            /// <summary>
            /// how much food quest completed.
            /// </summary>
            Quests_FoodQ = 37,
            /// <summary>
            /// join a clan quest completed.
            /// </summary>
            Quests_JoinClanQ = 38,
            Quests_SilverProduction = 39,
            Quests_DefenseFactor = 40,
            Quests_BarracksRecruitTime = 41,
            Quests_InfantryRecruitTime = 42,
            Quests_BattleSimSpies = 43,
            Quests_BattleSimCM = 44,
            Quests_BattleSimLC = 45,
            Quests_BattleSimLCWithWall = 46,
            Quests_BattleSimRamAndTreb = 47,
            Quests_Support = 48,
            Quests_TOU = 49,
            /// <summary>
            /// all quests have been completed and person clicked OK on this message
            /// </summary>
            Quests_alldone = 50,
            /// <summary>
            /// this tells us if player has gotten the 20 serants when accepting a title and has 2 villages at least
            /// </summary>
            Misc_Got2VillagePromo = 51,
            /// <summary>
            /// this tells us if player has gotten the 10 serants reward for posting a story about becoming a knight
            /// </summary>
            Misc_GotKightStoryReward = 52,
            /// <summary>
            /// all quests "2" for tags have been completed and person clicked OK on this message
            /// </summary>
            Quest2_Tags_Alldone = 53,
            Quest2_CreateTags = 54,
            Quest2_ApplyTags = 55,
            Quest2_Filters = 56,
            Quest2_FiltersSummaryPages = 57,
            Quest2_MultiFilter = 58,
            /// <summary>
            /// this tells us the last time the player accepted the daily gift
            /// </summary>
            Misc_GiftAccepted = 59,
            Quests_SMLvl5 = 60,
            Quests_ClanLeaders = 61,
            Quests_ClanAllies = 62,
            Quests_ClanMates = 63,
            Quests_Map1 = 64,
            Quests_Map2 = 65,
            Quests_OtherRealms = 66,
            Quests_Tavern = 67,
            Quests_Stable = 68,
            Quests_FindInactives = 69,
            Quests_RecruitSpies = 70,
            Quests_LootingInactives = 71,
            Quests_Mail = 72,
            Quests_Reports = 73,
            Quests_Advisor2 = 74,
            /// <summary>
            /// this flag keep track of where a player started, ie if player chose a quadrant to start in and which one. 
            /// </summary>
            Misc_StartedIn = 75,
            Misc_OptOutOfEmails = 76,
            Misc_AdvancedHQ = 77,
            Quests_BrrcksLvl1 = 78,
            Quests_Research = 79,
            NumSendableGiftsUsed = 80,
            NumEmailsToClamMembersSentToday = 81,
            Misc_OptOutOfNotification = 82,
            Misc_GovTypeChosen = 83,
            Misc_NumBonusVillChange = 84,
            Misc_Restart = 85,
            Misc_BoostedApproval= 86,
            Misc_ResearchItemsUsedInH = 87,
            NumSendableGiftsUsedV2 = 88,
        }

        VillagesCache _villagesCache;
        MyResearch _myResearch;
        Realm realm;
        Role _role;
        Clan _clan;
        DataRow drPlayers;
        DateTime _lastTimePlayerExtraInfoRetrieved;
        DateTime _lastTimePlayerForumInfoRetrieved;
        DateTime _lastTimePlayerRoleInfoRetrieved;
        DateTime _lastTimePlayerClanInfoRetrieved;
        DataSet dsExtraInfoOnLogin;
        DataTable dtFlags;
        DataTable dtFriends;
        DataTable dtActivatedPFTrials;
        DataTable dtPlayerActiveFeatures;//Returns a table who's columns are described by Player.CONSTS.PlayerActiveFeaturesColIndex 
        DataTable dtPlayerAllPFFeatures;//Returns a table who's columns are described by Player.CONSTS.PlayerActiveFeaturesColIndex 
        DataTable _dtRecentTargets = null;
        bool _reportInfo;
        bool _messageInfo;
        DateTime _sleepModeActiveOn;
        DateTime _vacationModeRequestOn;
        DateTime _vacationModeLastEndOn;

        DateTime _weekendModeTakesEffectOn; //player requests WM to be active on this date (not when reuqest itself was issued)
        DateTime _weekendModeLastEndOn;

        int _vacationDaysUsed;
        bool _forumchanged = false;
        int _points;
        int _numOfVillages;
        
        DateTime _registeredOn;
        bool _annonymous;
        Quests _quests_old;
        Title _title;
        User _user;
        Fbg.Common.Sex _sex;
        object _optOutOfEmails; //type bool
        List<FilterBase> _filters;
        private FilterBase _selectedFilter = null;
        Folders _folders;
        private DateTime _nightBuildActivatedOn;
        private Int16 _isMarkedForDelition = Int16.MinValue;
        private object _activeStewardPlayerID = null;
        /// <summary>
        /// if true, the object will consider the player as activly playing/personally logged into the game
        /// and will update the LastActive flag the database periodically (whenever player extra info is retreived) 
        /// </summary>
        bool _updateLastActivity = false;
        Avatar _avatar;
        public DateTime Raids_TopRaidActByTime { get; private set; }
        public int Raids_NumRaidsToCollectReward { get; private set; }

       
        static int[] xpSteps = new int[] { 
            //less than 5000 gets 0 days from xp
            5000, //1 day
            10000, //2 days
            20000, //3 days
            30000, //4 days
            40000, //5 days
            50000, //6 days
            60000, //7 days
            70000, //8 days
            80000, //9 days
            90000, //10 days
            100000, //11 days
            125000, //12 days
            150000, //13 days
            175000, //14 days
            200000, //15 days
            400000, //16 days
            600000, //17 days
            800000, //18 days
            1000000, //19 days
            1200000, //20 days 
            1500000, //21 days
            2000000, //22 days
            3000000 //23 days
        };
        System.Web.Script.Serialization.JavaScriptSerializer json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();


        public Player(DataRow dr, int xpTopdate, User user)
        {
            try
            {
                _user = user;
                drPlayers = dr;
                realm = Realms.Realm((int)drPlayers[User.CONSTS.PlayerBasicInfoColumnIndex.RealmID]);
                DefinedTargets = new DefinedTargets(this);

                PopulateExtraInfoOnLogin(xpTopdate);

                _quests_old = new Quests(this);
                _villagesCache = new VillagesCache(this);
            }
            catch (Exception ex)
            {
                BaseApplicationException bex = new BaseApplicationException("Error in player constructor", ex);
                bex.AddAdditionalInformation("dr", dr);
                bex.AddAdditionalInformation("realm", realm);
                bex.AddAdditionalInformation("dtFlags", dtFlags);
                throw bex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpToUpdate">set this only if you want to update the cached XP of this player</param>
        internal void PopulateExtraInfoOnLogin(int? xpToUpdate)
        {
            try
            {
                //returns table with PlayerInfo
                //we pass avatarID (the one from common players) here,
                //to playerLoginInfo to update realm player's avatarID duplicate, for player other purposes
                dsExtraInfoOnLogin = DAL.PlayerFlags.GetExtraInfo_OnLogin(realm.ConnectionStr
                    , (int)drPlayers[User.CONSTS.PlayerBasicInfoColumnIndex.playerID], xpToUpdate);

                dtFlags = dsExtraInfoOnLogin.Tables[Player.CONSTS.ExtraInfoTableIndex.Flags];
                
                // THIS IS DEAD, RETURNS AN EMPTY TABLE!
                dtFriends = dsExtraInfoOnLogin.Tables[Player.CONSTS.ExtraInfoTableIndex.Friends];

                dtActivatedPFTrials = dsExtraInfoOnLogin.Tables[Player.CONSTS.ExtraInfoTableIndex.ActivatedPFTrials];
                _annonymous = (bool)dsExtraInfoOnLogin.Tables[Player.CONSTS.ExtraInfoTableIndex.PlayerInfo]
                    .Rows[0][Player.CONSTS.PlayerInfoColIndex.Anonymous];
                _registeredOn = (DateTime)dsExtraInfoOnLogin.Tables[Player.CONSTS.ExtraInfoTableIndex.PlayerInfo]
                    .Rows[0][Player.CONSTS.PlayerInfoColIndex.RegisteredOn];
                _title = realm.TitleByID((int)dsExtraInfoOnLogin.Tables[Player.CONSTS.ExtraInfoTableIndex.PlayerInfo]
                    .Rows[0][Player.CONSTS.PlayerInfoColIndex.TitleID]);
                _sex = (Fbg.Common.Sex)(short)dsExtraInfoOnLogin.Tables[Player.CONSTS.ExtraInfoTableIndex.PlayerInfo]
                    .Rows[0][Player.CONSTS.PlayerInfoColIndex.Sex];
                _nightBuildActivatedOn = (DateTime)dsExtraInfoOnLogin.Tables[Player.CONSTS.ExtraInfoTableIndex.PlayerInfo]
                    .Rows[0][Player.CONSTS.PlayerInfoColIndex.NightBuildActivatedOn];
                _avatar = Realms.Avatars.GetAvatar(Convert.ToInt32(drPlayers[User.CONSTS.PlayerBasicInfoColumnIndex.AvatarID]));
                if (_avatar == null)
                {
                    _avatar = Realms.Avatars.GetAvatar(2);
                }
                this.Morale = new PlayerMorale(this,
                    (int)dsExtraInfoOnLogin.Tables[Player.CONSTS.ExtraInfoTableIndex.PlayerInfo]
                    .Rows[0][Player.CONSTS.PlayerInfoColIndex.Morale]
                    , (DateTime)dsExtraInfoOnLogin.Tables[Player.CONSTS.ExtraInfoTableIndex.PlayerInfo]
                    .Rows[0][Player.CONSTS.PlayerInfoColIndex.MoraleLastUpdated]);


                if (dsExtraInfoOnLogin.Tables[Player.CONSTS.ExtraInfoTableIndex.ActiveStewardInfo].Rows.Count > 0)
                {
                    _activeStewardPlayerID = (int)dsExtraInfoOnLogin.Tables[Player.CONSTS.ExtraInfoTableIndex.ActiveStewardInfo].Rows[0][0];
                }
                else
                {
                    _activeStewardPlayerID = null;
                }

                dtFlags.PrimaryKey = new DataColumn[] { dtFlags.Columns[CONSTS.FlagsColIndex.FlagID] };
                dtActivatedPFTrials.PrimaryKey = new DataColumn[] { dtActivatedPFTrials.Columns[CONSTS.ActivatedPFTrialsColIndex.PFTrialID] };

                PopulatePlayersPFFeatures();

               
            }
            catch (Exception ex)
            {
                BaseApplicationException bex = new BaseApplicationException("Error in PopulateExtraInfoOnLogin", ex);
                bex.AddAdditionalInformation("dsExtraInfoOnLogin", dsExtraInfoOnLogin);
                bex.AddAdditionalInformation("dtFlags", dtFlags);
                bex.AddAdditionalInformation("dtFriends", dtFriends);
                bex.AddAdditionalInformation("dtActivatedPFTrials", dtActivatedPFTrials);
                throw bex;
            }
        }

        internal void PopulatePlayersPFFeatures()
        {
            DataSet ds = DAL.Players.GetPlayerActiveFeatures(this.Realm.ConnectionStr, ID);
            dtPlayerActiveFeatures = ds.Tables[0];
            dtPlayerAllPFFeatures = ds.Tables[1];
        }

        #region SleepMode stuff
        /// <summary>
        /// returns DateTime.MinValue if it was never activated. Otherwise, returns the date the last time the sleep mode was 
        /// active from. So note that it was activated on ( SleepMode_ActiveOn - realm.SleepModeGet.TimeTillActive)
        /// </summary>
        public DateTime SleepMode_ActiveOn
        {
            get
            {
                RetrievePlayerExtraInfoIfNeeded();
                return _sleepModeActiveOn;
            }
        }

        /// <summary>
        /// tells if if this player is currently in sleep mode
        /// </summary>
        public bool SleepMode_IsActiveNow
        {
            get
            {
                if (realm.SleepModeGet.IsAvailableOnThisRealm)
                {
                    if (SleepMode_ActiveOn <= DateTime.Now
                        && SleepMode_ActiveOn.AddHours(realm.SleepModeGet.Duration) > DateTime.Now)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// Tells you if player activated sleep mode but it is still not yet taken effect. 
        /// ie, if the time (realm.SleepModeGet.TimeTillActive) to activate has not yet passed and returns you the time it will be active on
        /// Otherwise, returns DateTime.MinValue
        /// </summary>
        public DateTime SleepMode_ActivatingOn
        {
            get
            {
                if (realm.SleepModeGet.IsAvailableOnThisRealm)
                {
                    if (SleepMode_ActiveOn.AddHours(realm.SleepModeGet.TimeTillActive) > DateTime.Now)
                    {
                        return SleepMode_ActiveOn;
                    }
                }
                return DateTime.MinValue;
            }
        }
        /// <summary>
        /// Tells you if player can activate sleep mode.
        /// IF returns DateTime.MinValue, that means player can activate it now
        /// IF returns DateTime.MaxValue, that mean player cannot activate it at all (no sleep mode on this realm) 
        /// ELSE returns the date and time when player can activate the sleep mode again
        /// </summary> 
        public DateTime SleepMode_CanActivateIn
        {
            get
            {

                if (realm.SleepModeGet.IsAvailableOnThisRealm)
                {
                    if (SleepMode_ActiveOn == DateTime.MinValue)
                    {
                        return DateTime.MinValue;
                    }
                    else
                    {
                        return SleepMode_ActiveOn.AddHours(realm.SleepModeGet.AavailableOnceEveryXHours
                            - realm.SleepModeGet.TimeTillActive);
                    }
                }
                return DateTime.MaxValue;
            }
        }

        public bool SleepMode_Activate()
        {
            if (realm.SleepModeGet.IsAvailableOnThisRealm
                && !SleepMode_IsActiveNow
                && SleepMode_ActivatingOn == DateTime.MinValue)
            {

                //here we make sure if realm has WM, that WM isnt set to kick in within 16hours from now.
                bool canEnterSleepMode = true;
                /* -remove WM / SM interaction
                if (realm.WeekendModeGet.Allowed) {
                    WeekendModeStatus wmStatus = this.GetWeekendModeStatus();
                    if (wmStatus.requested && wmStatus.takesEffectOn.Ticks < DateTime.Now.AddHours(16).Ticks) {
                        return false;
                    } 
                }
                */

                if (canEnterSleepMode) {
                    DateTime activeOn = DAL.Players.ActivateSleepMode(realm.ConnectionStr, ID);
                    _sleepModeActiveOn = activeOn == DateTime.MinValue ? _sleepModeActiveOn : activeOn;
                    this.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.SleepModeLog, "activated", 
                        String.Format("Activated On: {0}, Takes effect On:{1}. Will last until:{2}, unless cancelled earlier. (all times in game-time)", 
                        DateTime.Now.ToUniversalTime(), _sleepModeActiveOn.ToUniversalTime(), 
                        _sleepModeActiveOn.ToUniversalTime().AddHours(this.realm.SleepModeGet.Duration)));
                    return true;
                }
            }

            return false;
        }


        public void SleepMode_Cancel()
        {
            if (realm.SleepModeGet.IsAvailableOnThisRealm)
            {
                DateTime activeOn = DAL.Players.CancelSleepMode(realm.ConnectionStr, ID);
                _sleepModeActiveOn = activeOn == DateTime.MinValue ? _sleepModeActiveOn : activeOn;
                this.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.SleepModeLog, "cancelled", String.Format("Cancelled On:{0} Game Time", DateTime.Now.ToUniversalTime()));
            }
        }
        #endregion


        #region VacationMode stuff

        public class VacationModeStatus
        {
            public int daysMax { get; set; } //how many total days a player has (base realm days + player xp conversion)
            public int daysUsed { get; set; } //how many days a player has used
            public bool active { get; set; } //is vacation mode currently in effect
            public bool requested { get; set; } //is there a vacation mode request awaiting activation
            public DateTime requestedOn { get; set; } //when vacation mode was last requested by user
            public DateTime takesEffectOn { get; set; } //when vacation mode takes effect (requested date + activativation delay)
            public DateTime endsOn { get; set; } //when vacation mode ends automatically (takes effect date + days left)
            public DateTime lastEndOn { get; set; } //the last time vacation mode ended for a player

            public double requestedOnMilli
            {
                get
                {
                    return Api.ApiHelper.SerializeDate(requestedOn);
                }
            }
            public double takesEffectOnMilli
            {
                get
                {
                    return Api.ApiHelper.SerializeDate(takesEffectOn);
                }
            }
            public double endsOnMilli
            {
                get
                {
                    return Api.ApiHelper.SerializeDate(endsOn);
                }
            }
            public double lastEndOnOnMilli
            {
                get
                {
                    return Api.ApiHelper.SerializeDate(lastEndOn);
                }
            }
        }

        /// <summary>
        /// Gets the player's vacation mode details.
        /// Automatically updates player table if vacation has ran out of days.
        /// </summary>
        /// <returns></returns>
        public VacationModeStatus GetVacationModeStatus()
        {
            VacationModeStatus vacationStatus = new VacationModeStatus();

            if (!realm.VacationModeGet.Allowed)
            {
                vacationStatus.active = false;
                vacationStatus.requested = false;
                return vacationStatus;
            }

            DateTime now = DateTime.Now;
            RetrievePlayerExtraInfoIfNeeded();

            vacationStatus.requestedOn = _vacationModeRequestOn;
            vacationStatus.lastEndOn = _vacationModeLastEndOn;
            vacationStatus.daysMax = GetMaxVacationDays();
            vacationStatus.daysUsed = _vacationDaysUsed;
            int vacationDaysLeft = vacationStatus.daysMax - vacationStatus.daysUsed;
            
            if (vacationStatus.requestedOn == DateTime.MinValue) {
                vacationStatus.active = false;
                vacationStatus.requested = false;
                vacationStatus.takesEffectOn = DateTime.MinValue;
                vacationStatus.endsOn = DateTime.MinValue;
                return vacationStatus;
            }

            vacationStatus.takesEffectOn = _vacationModeRequestOn.AddDays(realm.VacationModeGet.ActivationDelayDays);

            //set end date to effect + days left
            //IF days left is more than perUseMaximum, then set end date to effect + PerUseMaximum    
            vacationStatus.endsOn = vacationStatus.takesEffectOn.AddDays(Math.Min(vacationDaysLeft, realm.VacationModeGet.PerUseMaximum));
            

            //if vacation mode has ended naturally, cancel vacation mode and update player table
            if (vacationStatus.endsOn.Ticks <= now.Ticks)
            {
                VacationMode_Cancel();
                vacationStatus.active = false;
                vacationStatus.requested = false;
                vacationStatus.requestedOn = DateTime.MinValue;
                vacationStatus.takesEffectOn = DateTime.MinValue;
                vacationStatus.endsOn = DateTime.MinValue;
                vacationStatus.daysUsed = _vacationDaysUsed;               
                return vacationStatus;
            }

            vacationStatus.requested = true;

            //if vacation mode has taken effect, then it is active
            if (vacationStatus.takesEffectOn.Ticks <= now.Ticks)
            {
                vacationStatus.active = true;
            }
            else {
                vacationStatus.active = false;
            }

            return vacationStatus;
        }

        /// <summary>
        /// Gets a player Max vacation days by converting their xp points to additional vacation days, and adding it to realm base vacation days
        /// </summary>
        public int GetMaxVacationDays() {
            if (realm.VacationModeGet.Allowed)
            {
                int additionalDaysFromXp = Fbg.Bll.Player.convertXpToVacationDays(this.XP.XP);
                int maxPlayerVacationDays = realm.VacationModeGet.RealmBaseDays + additionalDaysFromXp;

                //if a realm Maximum exists, enforce it
                if (realm.VacationModeGet.RealmMaxDays > 0)
                {
                    maxPlayerVacationDays = maxPlayerVacationDays > realm.VacationModeGet.RealmMaxDays ? realm.VacationModeGet.RealmMaxDays : maxPlayerVacationDays;
                }

                return maxPlayerVacationDays;
            }
            return 0;
        }


        /// <summary>
        /// Acivates Vacation mode, sets the player's vacation request date to now
        /// </summary>
        public bool VacationMode_Activate()
        {
            if (this.Stewardship_IsActive) {
                return false;
            }

            VacationModeStatus VacationModeStatus = GetVacationModeStatus();

            //if realm allows vacation, vacation not currently active, request date is null, reactivation is allowed, and at least one vacation day available
            if (realm.VacationModeGet.Allowed && 
                !VacationModeStatus.active && 
                VacationModeStatus.requestedOn == DateTime.MinValue &&
                VacationModeStatus.lastEndOn.AddDays(realm.VacationModeGet.ReactivationDelayDays) < DateTime.Now &&
                VacationModeStatus.daysUsed < VacationModeStatus.daysMax)
            {               
                _vacationModeRequestOn = DAL.Players.ActivateVacationMode(realm.ConnectionStr, ID);
                VacationModeStatus = GetVacationModeStatus(); //get status again, since a new request was set
                this.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.VacationModeLog, "requested", String.Format("Activated On: {2}, Takes effect On:{3}.  Will last until:{4}, unless cancelled earlier. (all times in game-time) [ max days available:{0}, days used so far:{1}]"
                    , GetMaxVacationDays() 
                    , _vacationDaysUsed
                    , DateTime.Now.ToUniversalTime()
                    , VacationModeStatus.takesEffectOn.ToUniversalTime()
                    , VacationModeStatus.endsOn.ToUniversalTime()
                    ));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Cancels VM, calculates days used
        /// </summary>
        public bool VacationMode_Cancel()
        {
            if (this.Stewardship_IsActive)
            {
                return false;
            }
            int resultDays = DAL.Players.CancelVacationMode(realm.ConnectionStr, ID);
            if(resultDays == -1){
                //something went wrong with canceling
                return false;
            }
            _vacationDaysUsed = resultDays;
            this.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.VacationModeLog, "cancel/end", " max days:" + GetMaxVacationDays() + " _ used days:" + _vacationDaysUsed );
            return true;
        }

        #endregion



        #region WeekendMode stuff

        public class WeekendModeStatus
        {
            public bool active { get; set; } //is weekend mode currently in effect
            public bool requested { get; set; } //is there a weekend mode request awaiting activation
            public DateTime takesEffectOn { get; set; } //when weekend mode takes effect
            public DateTime endsOn { get; set; } //when weekend mode ends naturally
            public DateTime lastEndOn { get; set; } //the last time weekend mode ended for a player

            public double takesEffectOnMilli
            {
                get
                {
                    return Api.ApiHelper.SerializeDate(takesEffectOn);
                }
            }
            public double endsOnMilli
            {
                get
                {
                    return Api.ApiHelper.SerializeDate(endsOn);
                }
            }
            public double lastEndOnOnMilli
            {
                get
                {
                    return Api.ApiHelper.SerializeDate(lastEndOn);
                }
            }
        }

        /// <summary>
        /// Gets the player's weekend mode details.
        /// Automatically updates player table if weekend mode exhausted
        /// </summary>
        /// <returns></returns>
        public WeekendModeStatus GetWeekendModeStatus()
        {
            WeekendModeStatus weekendStatus = new WeekendModeStatus();

            if (!realm.WeekendModeGet.Allowed)
            {
                weekendStatus.active = false;
                weekendStatus.requested = false;
                return weekendStatus;
            }

            DateTime now = DateTime.Now;
            RetrievePlayerExtraInfoIfNeeded();

            weekendStatus.takesEffectOn = _weekendModeTakesEffectOn;  //player requests WM to be active on this date (not when reuqest itself was issued)
            weekendStatus.lastEndOn = _weekendModeLastEndOn;

            //if there is no date of effect set, then its not active and will not be, we return a base weekend Status
            if (weekendStatus.takesEffectOn == DateTime.MinValue)
            {
                weekendStatus.active = false;
                weekendStatus.requested = false;
                weekendStatus.takesEffectOn = DateTime.MinValue;
                weekendStatus.endsOn = DateTime.MinValue;
                return weekendStatus;
            }

            //set end date to effect date + realm wm days
            weekendStatus.endsOn = weekendStatus.takesEffectOn.AddDays(realm.WeekendModeGet.RealmBaseDays);

            //if weekend mode has ended naturally, cancel it (updates player table)
            if (weekendStatus.endsOn <= now)
            {
                WeekendMode_Cancel();
                weekendStatus.active = false;
                weekendStatus.requested = false;
                weekendStatus.takesEffectOn = DateTime.MinValue;
                weekendStatus.endsOn = DateTime.MinValue;
                return weekendStatus;
            }

            //if weekend mode has a take effect date, and hasnt ended here, then have an active request
            weekendStatus.requested = true;

            //if weekend mode has taken effect, then it is active
            if (weekendStatus.takesEffectOn <= now)
            {
                weekendStatus.active = true;
            }
            else
            {
                weekendStatus.active = false;
            }

            return weekendStatus;
        }


        /// <summary>
        /// Acivates Vacation mode, sets the player's vacation request date to now
        /// </summary>
        public bool WeekendMode_Request(DateTime desiredActivationDateInServerTime)
        {
            if (this.Stewardship_IsActive)
            {
                return false;
            }

            WeekendModeStatus weekendModeStatus = GetWeekendModeStatus();

            bool canReactivateNow = weekendModeStatus.lastEndOn.AddDays(realm.WeekendModeGet.ReactivationDelayDays).Ticks < DateTime.Now.Ticks;
            bool canActivateNow = DateTime.Now.AddHours(realm.WeekendModeGet.ActivationDelayMinimumHours).Ticks < desiredActivationDateInServerTime.Ticks;

            //if realm has sleepmode, make sure sleepmode isnt being activated within 16 hours
            bool sleepModeInteractionOk = true;
            /*
             * ///we are allowing WM regardless of SM
            if (realm.SleepModeGet.IsAvailableOnThisRealm)
            {
                //make sure SM is not currently active
                if (this.SleepMode_IsActiveNow ||
                    //or if SM is being activated and the desired WM activation time is less 16 hours AFTER SM taking effect
                    (this.SleepMode_ActivatingOn > DateTime.MinValue && desiredActivationDateInServerTime.Ticks < this.SleepMode_ActivatingOn.AddHours(16).Ticks))
                {
                    sleepModeInteractionOk = false;
                }
            }
            */


            if (realm.WeekendModeGet.Allowed && //if realm allows WeekendMode, 
                !weekendModeStatus.active && //it is not currently active
                !weekendModeStatus.requested && // not currently requested,
                canReactivateNow && //and reactivation is allowed
                canActivateNow && //and desired time beyond minimum wait
                sleepModeInteractionOk //WM is allowed in terms of SM interference
                )
            {

                _weekendModeTakesEffectOn = DAL.Players.ActivateWeekendMode(realm.ConnectionStr, ID, desiredActivationDateInServerTime);
                weekendModeStatus = GetWeekendModeStatus(); //get status again, since a new request was set

                this.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.WeekendModeLog, "wm-requested", 
                    String.Format("Request On: {0}, Takes effect On:{1}. Will last until:{2}, unless cancelled earlier. (all times UTC)"
                    , DateTime.Now.ToUniversalTime()
                    , weekendModeStatus.takesEffectOn.ToUniversalTime()
                    , weekendModeStatus.endsOn.ToUniversalTime()
                ));

                return true;
            }
            return false;
        }

        /// <summary>
        /// Cancels sleepmode, calculates days used
        /// </summary>
        public bool WeekendMode_Cancel()
        {
            if (this.Stewardship_IsActive)
            {
                return false;
            }

            int result = DAL.Players.CancelWeekendMode(realm.ConnectionStr, ID);
            if (result == -1)
            {
                //something went wrong with canceling
                return false;
            }

            _weekendModeTakesEffectOn = DateTime.MinValue;
            _weekendModeLastEndOn = DateTime.Now;

            this.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.WeekendModeLog, "cancel/end", "Ended/Canceled on " + DateTime.Now.ToUniversalTime() + " (UTC)");
            return true;
        }

        #endregion





        UsersXP _xp;
        public UsersXP XP { get { RetrievePlayerExtraInfoIfNeeded(); return _xp; } private set { _xp = value; } }

        public MyResearch MyResearch
        {
            get
            {
                RetrievePlayerExtraInfoIfNeeded();

                return _myResearch;
            }
        }
        public bool IsMarkedForDeletionDueToInactivity
        {
            get
            {
                if (_isMarkedForDelition == Int16.MinValue)
                {
                    _isMarkedForDelition = DAL.Players.GetIsMarkedForDeletionDueToInactivity(ID);
                }

                return _isMarkedForDelition == 1;
            }
        }

        public DataTable RecentTargets
        {
            get
            {
                if (_dtRecentTargets == null)
                {
                    _dtRecentTargets = Fbg.DAL.Players.GetRecentTargets(this.realm.ConnectionStr, this.ID);
                }
                return _dtRecentTargets;
            }
        }

        public DataTable GetStatHistory(int playerYouLook, DateTime? from)
        {
            return Fbg.DAL.Players.PlayerStatHistory(Realm.ConnectionStr, playerYouLook, from, null);
        }

        public void RecentTargets_Invalidate()
        {
            _dtRecentTargets = null;
        }

        /// <summary>
        /// number of villages this player has 
        /// </summary>
        public int NumberOfVillages
        {
            get
            {
                RetrievePlayerExtraInfoIfNeeded();
                return _numOfVillages;
            }
        }
        public int Points
        {
            get
            {
                RetrievePlayerExtraInfoIfNeeded();
                return _points;
            }
        }
        public int ID
        {
            get
            {
                return (int)drPlayers[User.CONSTS.PlayerBasicInfoColumnIndex.playerID];
            }
        }


        /// <summary>
        /// THIS IS DEAD! Returns an empty table!
        /// </summary>
        public DataTable Friends
        {
            get
            {
                return (DataTable)dtFriends;
            }
        }

        public string Name
        {
            get
            {
                return (string)drPlayers[User.CONSTS.PlayerBasicInfoColumnIndex.Name];
            }
        }
        public bool IsSuspended
        {
            get
            {
                return User.IsSuspended;
            }
        }

        /// <summary>
        /// will return null if IsSuspended is false
        /// </summary>
        public string SuspensionReason
        {
            get
            {
                return User.SuspensionReason;
            }
        }

        public Realm Realm
        {
            get
            {
                if (realm == null)
                {
                    realm = Realms.Realm((int)drPlayers[User.CONSTS.PlayerBasicInfoColumnIndex.RealmID]);
                }
                return realm;
            }
        }
        public DateTime RegisteredOn
        {
            get
            {
                return _registeredOn;
            }
        }
        public string FacebookID
        {
            get
            {
                return _user.UserName;
            }
        }
        /// <summary>
        /// if true, the object will consider the player as activly playing/personally logged into the game
        /// and will update the LastActive flag the database periodically (whenever player extra info is retreived) 
        /// </summary>
        public bool UpdateLastActivity
        {
            get { return _updateLastActivity; }
            set { _updateLastActivity = value; }
        }

        /// <summary>
        /// Indicates is new report is waiting
        /// </summary>
        public bool ReportInfo
        {
            get
            {
                RetrievePlayerExtraInfoIfNeeded();
                return _reportInfo;
            }
        }

        /// <summary>
        /// indicates if new message is waiting
        /// </summary>
        public bool MessageInfo
        {
            get
            {
                RetrievePlayerExtraInfoIfNeeded();
                return _messageInfo;
            }
        }


        /// <summary>
        /// gets the player avialabe chests
        /// this proberty call the DB each time
        /// </summary>
        public int Chests
        {
            get
            {
                return DAL.Players.GetPlayerChests(realm.ConnectionStr, this.ID);
            }
        }

        /// <summary>
        /// Get the clan this player is part of. Returns NULL if
        /// player is not part of a clan. 
        /// 
        /// Please note that clan is cached for 1 seconds so if you retrieve this once, it will not trigger refresh for 1 seconds,
        /// unless you set Clan=null which will trigger retrieve from database again you access Clan. 
        /// </summary>
        public Clan Clan
        {
            get
            {
                if (_clan == null || ((TimeSpan)DateTime.Now.Subtract(_lastTimePlayerClanInfoRetrieved)).TotalSeconds > 1)
                {
                    _clan = null;
                    DataSet ds = Fbg.DAL.Clans.GetClanForPlayer(Realm.ConnectionStr, ID);
                    _lastTimePlayerClanInfoRetrieved = DateTime.Now;
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            _clan = new Clan(ds.Tables[0].Rows[0], this);
                        }
                    }
                }
                return _clan;
            }
            set
            {
                _clan = value;
            }
        }

        public Role Role
        {
            get
            {
                if (Clan != null)
                {
                    if (_role == null || ((TimeSpan)DateTime.Now.Subtract(_lastTimePlayerRoleInfoRetrieved)).TotalSeconds > 3)
                    {

                        DataSet ds = Fbg.DAL.Roles.GetPlayerRoles(Realm.ConnectionStr, Clan.ID, ID);
                        _lastTimePlayerRoleInfoRetrieved = DateTime.Now;
                        if (ds.Tables.Count > 0)
                        {

                            _role = new Role(ds.Tables[0]);
                            return _role;
                        }

                    }
                    else
                    {
                        return _role;
                    }
                }
                return null;
            }
            set
            {
                _role = value;
            }
        }

        DateTime _lastTimeVillages_TempCacheAccessed;
        List<VillageBase> villages_TempCache;
        /// <summary>
        /// same as Villages but this is cached for 1 second so that it is safe to all this multiple times in a single request (while processing the same page) 
        /// </summary>
        /// 
        public List<VillageBase> Villages_TempCache()
        {
            if (_lastTimeVillages_TempCacheAccessed.AddMinutes(1) < DateTime.Now
                || villages_TempCache == null)
            {
                _lastTimeVillages_TempCacheAccessed = DateTime.Now;
                villages_TempCache = Villages;
            }

            return villages_TempCache;
        }

        /// <summary>
        /// Get list of villages for this player. the list of villages is limited to the currently selected
        /// filter. Use GetVillages(null) to get all villages, not limited by a filter
        /// This is an EXPENSIVE DB CALL. when possible, cache this list. do not do multiple calls. 
        /// </summary>
        public List<VillageBase> Villages
        {
            get
            {
                return GetVillages(SelectedFilter);
            }
        }

        /// <summary>
        /// Get list of villages for this player. 
        /// This is an EXPENSIVE DB CALL. when possible, cache this list. do not do multiple calls. 
        /// </summary>
        public List<VillageBase> GetVillages(FilterBase filter)
        {
            return GetVillages(filter, false);
        }

        /// <summary>
        /// Get list of villages for this player. 
        /// This is an EXPENSIVE DB CALL. when possible, cache this list. do not do multiple calls. 
        /// </summary>
        public List<VillageBase> GetVillages(FilterBase filter, bool allowReturnOfEmptyList)
        {

            List<VillageBase> villages = new List<VillageBase>();
            IDataReader reader;

            int selectedFilterID = -1;
            selectedFilterID = filter == null ? -1 : filter.ID;
            //
            // get the most basic list of villages
            //
            reader = Fbg.DAL.Villages.GetVillagesForPlayer(Realm.ConnectionStr, ID, false
                , selectedFilterID);
            using (reader)
            {
                while (reader.Read())
                {
                    villages.Add(new VillageBase(this
                        , (int)reader[DAL.Villages.CONSTS.VillagesReader.VillageID]));
                }
            }

            //
            // handle the situation if a filter returns a null result. 
            //  99% of the time this is handled by the UI but sometimes it is not. 
            //
            if (!allowReturnOfEmptyList)
            {
                if (villages.Count == 0
                    && this.NumberOfVillages > 0
                    && filter != null)
                {
                    this.SelectedFilter = null;
                    return GetVillages(null, false);
                }
            }



            return villages;
        }



        /// <summary>
        /// Get list of villages for this player. the list of villages is limited to the currently selected
        /// filter. Use GetVillages_BasicA(null) to get all villages, not limited by a filter
        /// This is an EXPENSIVE DB CALL. when possible, cache this list. do not do multiple calls. 
        /// </summary>
        public List<VillageBasicA> Villages_BasicA
        {
            get
            {
                return GetVillages_BasicA(SelectedFilter, false);
            }
        }
        /// <summary>
        /// Get list of villages for this player. 
        /// This is an EXPENSIVE DB CALL. when possible, cache this list. do not do multiple calls. 
        /// </summary>
        public List<VillageBasicA> GetVillages_BasicA(FilterBase filter)
        {
            return GetVillages_BasicA(filter, false);
        }
        /// <summary>
        /// Get list of villages for this player. 
        /// This is an EXPENSIVE DB CALL. when possible, cache this list. do not do multiple calls. 
        /// </summary>
        public List<VillageBasicA> GetVillages_BasicA(FilterBase filter, bool allowReturnOfEmptyList)
        {
            List<VillageBasicA> villages = new List<VillageBasicA>();
            IDataReader reader;

            int selectedFilterID = -1;
            selectedFilterID = filter == null ? -1 : filter.ID;

            //
            // get the second level of details list of villages
            //
            reader = Fbg.DAL.Villages.GetVillagesForPlayer(Realm.ConnectionStr, ID, true
                , selectedFilterID);
            using (reader)
            {
                while (reader.Read())
                {
                    villages.Add(new VillageBasicA(this
                        , (string)reader[DAL.Villages.CONSTS.VillagesReader.Name]
                        , (int)reader[DAL.Villages.CONSTS.VillagesReader.VillageID]
                        , (int)reader[DAL.Villages.CONSTS.VillagesReader.Coins]
                        , (int)reader[DAL.Villages.CONSTS.VillagesReader.Points]
                        , (int)reader[DAL.Villages.CONSTS.VillagesReader.XCord]
                        , (int)reader[DAL.Villages.CONSTS.VillagesReader.YCord]
                        , (int)reader[DAL.Villages.CONSTS.VillagesReader.loyalty]
                        , (int)reader[DAL.Villages.CONSTS.VillagesReader.CoinMineLevel]
                        , (int)reader[DAL.Villages.CONSTS.VillagesReader.TreasuryLevel]
                        , (DateTime)reader[DAL.Villages.CONSTS.VillagesReader.CoinsLastUpdates]
                        , (short)reader[DAL.Villages.CONSTS.VillagesReader.VillageType]
                        ));
                }
            }
            //
            // handle the situation if a filter returns a null result. 
            //  99% of the time this is handled by the UI but sometimes it is not. 
            //
            if (!allowReturnOfEmptyList)
            {
                if (villages.Count == 0
                    && this.NumberOfVillages > 0
                    && filter != null)
                {
                    this.SelectedFilter = null;
                    return GetVillages_BasicA(null, false);
                }
            }

            return villages;
        }




        /// <summary>
        /// Get a village base on villageID. 
        /// </summary>
        public VillageBase VillageBase(int villageID, List<VillageBase> villages)
        {
            foreach (VillageBase vil in villages)
            {
                if (vil.id == villageID)
                {
                    return vil;
                }
            }
            return null;
        }
        /// <summary>
        /// Get a village base on villageID. 
        /// </summary>
        public Village Village(int villageID)
        {
            return Fbg.Bll.Village.GetVillage(this, villageID
                , PF_HasPF(Fbg.Bll.CONSTS.PFs.ImprovedVOV)
                , PF_HasPF(Fbg.Bll.CONSTS.PFs.ConvenientSilverTransport));
        }

        /// <summary>
        /// Get a VillageBaseB by villageID. 
        /// </summary>
        public VillageBasicB VillageBasicB(int villageID)
        {
            return Fbg.Bll.VillageBasicB.GetVillage(this, villageID, PF_HasPF(Fbg.Bll.CONSTS.PFs.ConvenientSilverTransport));
        }

        /// <summary>
        /// Get a VillageBaseA by villageID from the provided list. 
        /// </summary>
        public VillageBasicA VillageBasicA(int villageID, List<VillageBasicA> villages)
        {
            foreach (VillageBasicA vil in villages)
            {
                if (vil.id == villageID)
                {
                    return vil;
                }
            }
            return null;
        }

        /// <summary>
        /// Create a new clan
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public Clan CreateClan(string name, string desc)
        {
            return Clan.CreateNewClan(name, desc, this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="isQ">true if this event is a entry in the Q; false if it is a currently upgrading upgrade</param>
        public void CancelEvent(long eventID, bool isQ)
        {
            DAL.Villages.CancelUpgrade(Realm.ConnectionStr, eventID, isQ);
        }

        /// <summary>
        /// upgrade building
        /// </summary>
        /// <param name="bid">BuildingID</param>
        public void DoUpgrade(int villageID, int bid, int level)
        {

            //DAL.Villages.DoUpgrade(Realm.ConnectionStr
            //    , villageID
            //    , bid
            //    , level
            //    , (pfSystemActive && !PF_HasPF(Fbg.Bll.CONSTS.PFs.BuildingQ)) ? Fbg.Bll.CONSTS.PFInfo.BuildQueueLimit : 9999);

            DAL.Villages.DoUpgrade(Realm.ConnectionStr
                , villageID
                , bid
                , level
                , 0 /*depreciated param*/);
        }

        public void FoundAVillage()
        {
            DAL.Villages.FoundVillage(Realm.ConnectionStr, ID, Name, null);
        }
        /// <summary>
        /// same as above but specifies a quad
        /// </summary>
        /// <param name="quad"></param>
        public void FoundAVillage(int? quad)
        {
            DAL.Villages.FoundVillage(Realm.ConnectionStr, ID, Name, quad);
        }




        ///// <summary>
        ///// get my troops abroad, from any village, supporting any village
        ///// </summary>
        ///// <returns></returns>
        //public UnitsAbroad GetUnitsAbroad()
        //{
        //    return new UnitsAbroad(DAL.Units.GetMyUnitsAbroad(Realm.ConnectionStr, this.ID, null, null));
        //}
        //public UnitsAbroad GetUnitsAbroad(int? supportingVillageID)
        //{
        //    return new UnitsAbroad(DAL.Units.GetMyUnitsAbroad(Realm.ConnectionStr, ID, supportingVillageID, null));
        //}
        /// <summary>
        /// get my troops supporting other villages
        /// </summary>
        /// <param name="supportingVillageID">set to null to see troops from all my villages. set to villageid of 
        /// one of my village and will see troops from this village only</param>
        /// <param name="supportedVillageID">set to some village id and will see troops supporting only this village. Set to null not to limit on this</param>
        /// <returns></returns>
        public UnitsAbroad GetUnitsAbroad(int? supportingVillageID, int? supportedVillageID)
        {
            return new UnitsAbroad(DAL.Units.GetMyUnitsAbroad(Realm.ConnectionStr, ID, supportingVillageID, supportedVillageID));
        }


        public static int GetPlayerByName(string PlayerName, Player owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("Player is null");

            }

            return DAL.Players.GetPlayerIDByName(PlayerName, owner.Realm.ConnectionStr);
        }


        public static int convertXpToVacationDays(double xpPoints)
        {           
            Int32 days = 0;

            for (int i = 0; i < xpSteps.Length; i++)
            {
                if (xpPoints < xpSteps[i])
                {
                    days = i;
                    break;
                }
            }

            //in the unlikely event that some xp busts out of our imagination, give max days
            if (days == 0 && xpPoints >= xpSteps[xpSteps.Length - 1]) { days = xpSteps.Length; }

            return days;
        }

        public static int nextVacationXP(int currentXP)
        {

            int theIndex = 0;

            for (int i = 0; i < xpSteps.Length; i++)
            {
                if (currentXP < xpSteps[i])
                {
                    theIndex = i;
                    break;
                }
            }

            return xpSteps[theIndex];
        }

        /// <summary>
        /// this function allow the player to buy more chests using selectd village coins
        /// </summary>
        /// <param name="numberOfChests">number of chests palyer wants to buy</param>
        /// <param name="selectedVillage">selected villagethat the player wants to use its coins</param>
        /// <returns>false means not enough silver </returns>
        public bool BuyChests(int numberOfChests, Village selectedVillage)
        {
            bool retVal = false;
            // Prevent the player from cashing out chests. Doing so with allow the player to hide
            // silver in chests, protecting it till needed later.
            if (numberOfChests <=0)
                return retVal;

            //((UnitTypeLord)FbgPlayer.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Gov)).Cost(null);
            int chestCost = ((UnitTypeLord)Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Gov)).Cost(null);
            int maxChests = selectedVillage.coins / chestCost;
            if (numberOfChests <= maxChests)
            {
                retVal = DAL.Players.BuyChests(Realm.ConnectionStr, ID, selectedVillage.id, numberOfChests, chestCost);

                selectedVillage.coins -= numberOfChests * chestCost;

                if (Chests >= 1)
                {
                    Quests2.SetQuestAsCompleted("CaptureAVillage1");
                }
              
                this.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.ChestBuy, string.Empty, string.Format("bought {3} chest(s) from {0}({1},{2})"
                    , selectedVillage.name, selectedVillage.xcord, selectedVillage.ycord, numberOfChests));
            }

            return retVal;
        }

        /// <summary>
        /// This will ensure that we retrieve MyResearch next time it is called. 
        /// </summary>
        public void MyResearch_ForceRefresh()
        {
            // this will ensure that we retrieve MyResearch next time it is called
            _lastTimePlayerExtraInfoRetrieved = DateTime.MinValue;
            _myResearch = null;
        }

        /// <summary>
        /// Use CONSTS.CacheItemIDs for the defintion of the IDS
        /// </summary>
        public Fbg.Common.CacheTimeStamps CacheItemInfo { get; private set; }

        public void ForceRetrievePlayerExtraInfoNextTime()
        {
            _lastTimePlayerExtraInfoRetrieved = DateTime.Now.AddMinutes(-1); // this will ensure that RetrievePlayerExtraInfoIfNeeded(), when called next time, will get latest data
        }
        private void RetrievePlayerExtraInfoIfNeeded()
        {
            //cache for only 1 seconds - this will allow multiple accesses during the same processing to not 
            //  result in multiple calls. 
            if (((TimeSpan)DateTime.Now.Subtract(_lastTimePlayerExtraInfoRetrieved)).TotalSeconds > 1)
            {
                TRACE.VerboseLine("RetrievePlayerExtraInfoIfNeeded() - retrieving");
                DataSet ds = DAL.Players.GetPlayerExtraInfo(realm.ConnectionStr
                    , this.ID
                    , _updateLastActivity
                    , _myResearch == null
                    , this.Stewardship_IsLoggedInAsSteward ? (int?)this._loggedInAsStewardRecID : null
                    , LastHandledVillageCacheTimeStamp);
                DataRow drMiscInfo = ds.Tables[CONSTS.PlayerExtreaInfoColIndex.MiscInfo].Rows[0];

                _points = (int)drMiscInfo[0];
                _messageInfo = (bool)drMiscInfo[1];
                _reportInfo = (bool)drMiscInfo[2];
                _sleepModeActiveOn = drMiscInfo[3] is DBNull ? DateTime.MinValue : (DateTime)drMiscInfo[3];
                _vacationModeRequestOn = drMiscInfo[8] is DBNull ? DateTime.MinValue : (DateTime)drMiscInfo[8];
                _vacationDaysUsed = drMiscInfo[9] is DBNull ? 0 : (int)drMiscInfo[9];
                _vacationModeLastEndOn = drMiscInfo[10] is DBNull ? DateTime.MinValue : (DateTime)drMiscInfo[10];

                _numOfVillages = (int)ds.Tables[CONSTS.PlayerExtreaInfoColIndex.VillCount].Rows[0][0];
                _lastTimePlayerExtraInfoRetrieved = DateTime.Now;
                XP = new UsersXP((int)drMiscInfo[6]);
                ListOfVillagesChangedSinceLastHandledVillageCacheTimeStamp = ds.Tables[CONSTS.PlayerExtreaInfoColIndex.VillageCacheStamps];

                if (Stewardship_IsLoggedInAsSteward)
                {
                    Stewardship_IsActive = (bool)drMiscInfo[7];
                }
                else
                {
                    Stewardship_HasStewardAccepted = (bool)drMiscInfo[7];
                }
                //
                //
                //
                CacheItemInfo = new Common.CacheTimeStamps(ds.Tables[CONSTS.PlayerExtreaInfoColIndex.CacheStamps]);

                //
                //
                //
                if (ds.Tables.Count >= CONSTS.PlayerExtreaInfoColIndex.NUM_TABLES_INDICATING_RESERARCH_RETRIVED)
                {
                    _myResearch = new MyResearch(realm, this, ds.Tables[CONSTS.PlayerExtreaInfoColIndex.Research_1_Optional], ds.Tables[CONSTS.PlayerExtreaInfoColIndex.Research_2_Optional]);
                }

                //
                //
                //
                if ((bool)drMiscInfo[5])
                {
                    this.Quests2.CompletedQuests_RewardNotClaimed_Invalidate();
                }
                this.Morale = new PlayerMorale(this,
                   (int)drMiscInfo[11]
                   , (DateTime)drMiscInfo[12]);

                //weekend mode stuff
                _weekendModeTakesEffectOn = drMiscInfo[13] is DBNull ? DateTime.MinValue : (DateTime)drMiscInfo[13];
                _weekendModeLastEndOn = drMiscInfo[14] is DBNull ? DateTime.MinValue : (DateTime)drMiscInfo[14];

                // raids 
                Raids_TopRaidActByTime = DateTime.MinValue;
                Raids_NumRaidsToCollectReward = 0;
                if (ds.Tables[CONSTS.PlayerExtreaInfoColIndex.Raids].Rows.Count > 0 )
                {
                    if (!(ds.Tables[CONSTS.PlayerExtreaInfoColIndex.Raids].Rows[0][0] is DBNull))
                    {
                        Raids_TopRaidActByTime = (DateTime)ds.Tables[CONSTS.PlayerExtreaInfoColIndex.Raids].Rows[0][0];
                    }
                    if (!(ds.Tables[CONSTS.PlayerExtreaInfoColIndex.Raids].Rows[0][1] is DBNull))
                    {
                        Raids_NumRaidsToCollectReward = (int)ds.Tables[CONSTS.PlayerExtreaInfoColIndex.Raids].Rows[0][1];
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if there are new post on the forum accessible by this player</returns>
        private bool RetrievePlayerForumLastViewedIfNeeded(bool forceChange)
        {
            //cache for 15 seconds - this will allow multiple accesses during the same processing to not 
            //  result in multiple calls. 
            // also, if forum is changed, no point going to the DB since we will continue saying it has 
            //  
            if (forceChange || !_forumchanged)
            {
                if (((TimeSpan)DateTime.Now.Subtract(_lastTimePlayerForumInfoRetrieved)).TotalSeconds > 15)
                {
                    _lastTimePlayerForumInfoRetrieved = DateTime.Now;
                    _forumchanged = DAL.Players.IsForumChanged(realm.ConnectionStr, this.ID);
                }
            }
            return _forumchanged;
        }

        /// <returns>true if there are new post on the forum accessible by this player</returns>
        public bool ForumChanged
        {
            get
            {
                return RetrievePlayerForumLastViewedIfNeeded(false);
            }
        }

        /// <summary>
        /// call this to set the ForumChanged indicator to false.
        /// </summary>
        public void ForumChanged_SetAsChecked()
        {
            _forumchanged = false;
        }

        /////<summary>
        ///// Get the number of incoming attacks. repreciated. use IncomingAttack() 
        /////</summary>        
        //private DataTable tblIncomingAttack()
        //{
        //    dtIncomingAttack = DAL.Players.GetIncomingAttack(Realm.ConnectionStr, this.ID);

        //    return dtIncomingAttack;
        //}


        public struct IncomingAttackInfo
        {
            /// <summary>
            /// only valid if NumAttacks > 0 
            /// </summary>
            public DateTime FirstAttackArrivalTime { get; set; }
            /// <summary>
            /// only valid if NumAttacks > 0 
            /// </summary>
            public TimeSpan FirstAttackArrivalIn { get { return FirstAttackArrivalTime.Subtract(DateTime.Now); } }
            public int NumAttacks { get; set; }
        }
        /// <summary>
        /// be careful, this makes a call to the DB every time
        /// </summary>
        public IncomingAttackInfo IncomingAttack()
        {
            IncomingAttackInfo ret = new IncomingAttackInfo();
            DataTable tblAttack = DAL.Players.GetIncomingAttack(Realm.ConnectionStr, this.ID);
            ret.NumAttacks = (int)tblAttack.Rows[0][Player.CONSTS.IncomingAttackColIndex.count];
            if (ret.NumAttacks > 0)
            {
                ret.FirstAttackArrivalTime = (DateTime)tblAttack.Rows[0][Player.CONSTS.IncomingAttackColIndex.EventTime];
            }

            return ret;
        }

        #region Player Flags


        /// <summary>
        /// returns null if no flag, UpdatedOn value if it does
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="useCache">if true, we use the value from caches in session, if false, we go to the DB and refresh this flag info</param>
        /// <returns></returns>
        public object HasFlag(Flags flag, bool useCache)
        {
            return HasFlag((int)flag, useCache);
        }

        /// <summary>
        /// returns null if no flag, UpdatedOn value if it does
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="useCache">if true, we use the value from caches in session, if false, we go to the DB and refresh this flag info</param>
        /// <returns></returns>
        private object HasFlag(int flag, bool useCache)
        {
            DataRow dr = dtFlags.Rows.Find(flag);

            if (!useCache)
            {
                object updatedOn;
                string data;
                DAL.PlayerFlags.GetFlag(this.realm.ConnectionStr, ID, (int)flag, out data, out updatedOn);
                Flag_Update(flag, updatedOn, data, ref dr);
            }

            if (dr != null)
            {
                return dr[CONSTS.FlagsColIndex.UpdatedOn];
            }
            return null;
        }

        /// <summary>
        /// returns null if no flag, UpdatedOn value if it does.
        /// 
        /// Same as calling HasFlag(flag, true)
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public object HasFlag(Flags flag)
        {
            return HasFlag(flag, true);
        }
        public object HasFlag_GetData(Flags flag)
        {
            DataRow dr = dtFlags.Rows.Find(flag);
            if (dr != null)
            {
                return dr[CONSTS.FlagsColIndex.Data];
            }
            return null;
        }
        public object HasFlag_GetData(int flag)
        {
            DataRow dr = dtFlags.Rows.Find(flag);
            if (dr != null)
            {
                return dr[CONSTS.FlagsColIndex.Data];
            }
            return null;
        }
        public void SetFlag(Flags flag)
        {
            SetFlag((int)flag);
        }
        public void SetFlag(int flag)
        {
            SetFlag(flag, null);
        }
        public void SetFlag(Flags flag, String data)
        {
            SetFlag((int)flag, data);
        }
        private void SetFlag(int flag, String data)
        {
            DataRow dr = dtFlags.Rows.Find(flag);
            DateTime now = DateTime.Now;
            DAL.PlayerFlags.SetFlag(realm.ConnectionStr, (int)flag, ID, now, data);
            Flag_Update(flag, now, data, ref dr);
        }

        /// <summary>
        /// internal helper function
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="updateOn"></param>
        /// <param name="dr"></param>
        private void Flag_Update(Flags flag, object updatedOn, string data, ref DataRow dr)
        {
            Flag_Update((int)flag, updatedOn, data, ref dr);
        }
        /// <summary>
        /// internal helper function
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="updateOn"></param>
        /// <param name="dr"></param>
        private void Flag_Update(int flag, object updatedOn, string data, ref DataRow dr)
        {
            if (updatedOn == null)
            {
                // player does not have this flag at all so remove it from cache if it was there 
                if (dr != null)
                {
                    dr.Delete();
                    dtFlags.AcceptChanges();
                    dr = null;
                }
            }
            else
            {
                //
                // update the flag 
                if (dr == null)
                {
                    dr = dtFlags.Rows.Add(new object[] { flag, updatedOn, data });
                    dtFlags.AcceptChanges();
                }
                else
                {
                    dr[CONSTS.FlagsColIndex.UpdatedOn] = updatedOn;
                    dr[CONSTS.FlagsColIndex.Data] = data;
                    dtFlags.AcceptChanges();
                }

            }
        }
        #endregion

        #region Paid Features

        internal void PF_UseViaItem2(int pfPackageID, TimeSpan duration)
        {
            DAL.Players.ActivateExtendPackageFromItem2(this.Realm.ConnectionStr, this.ID, pfPackageID, Convert.ToInt32(duration.TotalMinutes));
            PopulatePlayersPFFeatures();
            PF_PlayerPFPackages2_Invalidate();            
        }

        /// <summary>
        /// returns true if trial was already used; false otherwise. 
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool PF_WasTrialActivated(Fbg.Bll.CONSTS.PFTrials trialID)
        {
            DataRow dr = dtActivatedPFTrials.Rows.Find(trialID);
            if (dr != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// returns true if trial is CURRENTLY active; false otherwise. 
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool PF_IsTrialActive(Fbg.Bll.CONSTS.PFTrials trialID)
        {
            DataRow dr = dtActivatedPFTrials.Rows.Find(trialID);
            if (dr != null)
            {
                if ((DateTime)dr[CONSTS.ActivatedPFTrialsColIndex.ExpiresOn] > DateTime.Now)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// returns true if this PF is active 
        /// </summary>
        /// <param name="PFID">Values from Fbg.Bll.CONSTS.PFs enum</param>
        public bool PF_HasPF(Fbg.Bll.CONSTS.PFs PFID)
        {
            if (realm.PFs.ContainsKey((int)PFID))
            {
                DataRow[] drs = dtPlayerActiveFeatures.Select(Bll.Player.CONSTS.PlayerActiveFeaturesColName.FeatureID + "=" + (int)PFID);
                if (drs.Length != 0 && (DateTime)drs[0][1] >  DateTime.Now)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                // no such PF on this realm so therefore by default we consider the player as having it. 
                //  ie - no PF in the database, then we consider the PF non-existant
                return true;
            }
        }
        /// <summary>
        /// returns when a feature expires (even if it expired long ago) as long as the player had the feature active at least once. 
        /// If not, returns DateTime.MinValue
        /// </summary>
        /// <param name="PFID">Values from Fbg.Bll.CONSTS.PFs enum</param>
        public DateTime PF_PFExpiresOn(Fbg.Bll.CONSTS.PFs PFID)
        {
            DataRow[] drs = dtPlayerAllPFFeatures.Select(Bll.Player.CONSTS.PlayerAllFeaturesColName.FeatureID + "=" + (int)PFID);
            if (drs.Length != 0)
            {
                return (DateTime)drs[0][CONSTS.PlayerAllFeaturesColIndex.ExpiresOn];
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        /// <summary>
        ///  this function activate or extend the PF Package
        /// </summary>
        /// <remarks>DEPRECIATED. USE PF_ActivatePackage(int packageID)</remarks>
        /// <param name="packageID"></param>
        /// <param name="activate">True if you want to Activate PF Package False if you want to Extend PF Package</param>
        /// <returns>0 means sucess ;1 means not enough gold</returns>
        public int PF_ActivatePackage(int packageID, bool activate)
        {
            int retVal = DAL.Players.ActivateExtendPackage(this.Realm.ConnectionStr, this.ID, packageID, activate);
            PopulatePlayersPFFeatures();
            PF_PlayerPFPackages2_Invalidate();
            return retVal;
        }
        /// <summary>
        ///  this function activate or extend the PF Package 
        /// </summary>
        /// <param name="packageID"></param>
        /// <param name="activate">True if you want to Activate PF Package False if you want to Extend PF Package</param>
        /// <returns>0 means sucess ;1 means not enough gold</returns>
        public int PF_ActivatePackage(int packageID)
        {
            return PF_ActivatePackage(packageID, false);
        }

        /// <summary>
        /// this function cancels a package and refunds the servants, if any
        /// </summary>
        /// <param name="packageID"></param>
        /// <param name="refundtype"></param>
        /// <returns></returns>
        public void PF_RefundPackage(int packageID, Fbg.Common.PFs.RefundTypes refundtype)
        {
            int retVal = 0;

            retVal = DAL.Players.RefundPackage(this.Realm.ConnectionStr, this.ID, packageID, CONSTS.PFPackageRefundPercentage, refundtype);
            PopulatePlayersPFFeatures();
            PF_PlayerPFPackages2_Invalidate();
        }
        /// <summary>
        /// this function calculates the refund one can get for an active package. 
        /// </summary>
        /// <param name="packageID"></param>
        /// <param name="refundtype"></param>
        /// <returns></returns>
        public void PF_CalculatePackageRefund(int packageID, Fbg.Common.PFs.RefundTypes refundtype
            , out int servantsRefund, out double daysToCancel)
        {
            DAL.Players.CalculateRefundAmount(this.Realm.ConnectionStr, this.ID, packageID
                , CONSTS.PFPackageRefundPercentage, refundtype, out servantsRefund, out daysToCancel);
        }

        /// <summary>
        /// calculate the refund when you would activate the NP while cancelling the packages belonging to NP
        /// </summary>
        /// <returns>servants returned</returns>
        public int PF_CalculateNPRefund()
        {
            return DAL.Players.CalculateNPRefundAmount(this.Realm.ConnectionStr, this.ID);
        }

        /// <summary>
        /// Activate the NP while also cancelling any active features that fall under it
        /// </summary>
        /// <returns>servants returned</returns>
        public int PF_ActivateNPackageWithRefund()
        {
            int NPPackageID = 999;
            DataRow[] drPF = this.Realm.PFPackages.Select(Fbg.Bll.Realm.CONSTS.PFPackgesPrimaryColName.PackageID + "=" + NPPackageID.ToString());
            int npCost = Convert.ToInt32(drPF[0][Realm.CONSTS.PFPackagesColIndex.Cost]);
            int ret = DAL.Players.ActivateNPackageWithRefund(this.Realm.ConnectionStr, this.ID, npCost);
            PopulatePlayersPFFeatures();
            return ret;
        }


        public void PF_ActivatePFTrail(Fbg.Bll.CONSTS.PFTrials pfTrailID)
        {
            dtActivatedPFTrials = DAL.Players.ActivePFTrial(this.Realm.ConnectionStr, ID, (int)pfTrailID);
            PopulatePlayersPFFeatures();
            dtActivatedPFTrials.PrimaryKey = new DataColumn[] { dtActivatedPFTrials.Columns[CONSTS.ActivatedPFTrialsColIndex.PFTrialID] };
        }


        List<PlayerPFPackage> _playerPFPackages;
        /// <summary>
        /// this property gets the PF Player Packges. 
        /// This is cached version of PF_PlayerPFPackages
        /// </summary>
        public List<PlayerPFPackage> PF_PlayerPFPackages2
        {
            get
            {
                if (_playerPFPackages == null)
                {
                    DataTable dt = PF_PlayerPFPackages;
                    _playerPFPackages = new List<PlayerPFPackage>(dt.Rows.Count);
                    foreach (DataRow dr in dt.Rows)
                    {
                        _playerPFPackages.Add(new PlayerPFPackage(dr, this));
                    }
                }
                return _playerPFPackages;
            }
        }
        public void PF_PlayerPFPackages2_Invalidate()
        {
            _playerPFPackages = null;
        }

        /// <summary>
        /// this property gets the PF Player Packges --BECARFUL -- this function call the DB each time its called 
        ///  Returns a table who's columns are described by Player.CONSTS.PlayerPFPackageColIndex 
        /// </summary>
        public DataTable PF_PlayerPFPackages
        {
            get
            {
                return DAL.Players.GetPlayerPackages(this.Realm.ConnectionStr, ID);
            }
        }
        /// <summary>
        /// this function activate the NP for the admins on the fly so this function don't afect in the original player data
        /// </summary>
        public void PF_ActivateNPForAdmin()
        {
            dtPlayerActiveFeatures.Rows.Add(3, DateTime.Now.AddDays(1));
            dtPlayerActiveFeatures.Rows.Add(4, DateTime.Now.AddDays(1));
            dtPlayerActiveFeatures.Rows.Add(5, DateTime.Now.AddDays(1));
            dtPlayerActiveFeatures.Rows.Add(9, DateTime.Now.AddDays(1));
            dtPlayerActiveFeatures.Rows.Add(10, DateTime.Now.AddDays(1));
            dtPlayerActiveFeatures.Rows.Add(12, DateTime.Now.AddDays(1));
            dtPlayerActiveFeatures.Rows.Add(13, DateTime.Now.AddDays(1));
            dtPlayerActiveFeatures.Rows.Add(14, DateTime.Now.AddDays(1));
            dtPlayerActiveFeatures.Rows.Add(15, DateTime.Now.AddDays(1));
            dtPlayerActiveFeatures.Rows.Add(16, DateTime.Now.AddDays(1));
            dtPlayerActiveFeatures.Rows.Add(17, DateTime.Now.AddDays(1));
            dtPlayerActiveFeatures.Rows.Add(18, DateTime.Now.AddDays(1));
            dtPlayerActiveFeatures.Rows.Add(19, DateTime.Now.AddDays(1));
            dtPlayerActiveFeatures.Rows.Add(20, DateTime.Now.AddDays(1));
            dtPlayerActiveFeatures.Rows.Add(21, DateTime.Now.AddDays(1));
            dtPlayerActiveFeatures.Rows.Add(22, DateTime.Now.AddDays(1));
        }

        /// <summary>
        /// returns null if no trial is available
        /// </summary>
        /// <returns></returns>
        public PFTrial PF_GetAvailableTrial(Fbg.Bll.CONSTS.PFs pf, List<VillageBase> villages)
        {
            switch (pf)
            {
                case Fbg.Bll.CONSTS.PFs.BuildingQ:
                    return GetAvailableBuildingQTrail();
                case Fbg.Bll.CONSTS.PFs.LargeMap:
                    return GetGenericTrail(Fbg.Bll.CONSTS.PFTrials.LargeMap);
                case Fbg.Bll.CONSTS.PFs.GiantMap:
                    return GetGenericTrail(Fbg.Bll.CONSTS.PFTrials.GiantMap);
                case Fbg.Bll.CONSTS.PFs.ConvenientSilverTransport:
                    return GetGenericTrail(Fbg.Bll.CONSTS.PFTrials.ConvinientSilverTransport);
                case Fbg.Bll.CONSTS.PFs.IncomingTroopsToFromVillagePlayer:
                    return GetGenericTrail(Fbg.Bll.CONSTS.PFTrials.IncomingTroopsToFromVillagePlayer);
                case Fbg.Bll.CONSTS.PFs.Notes:
                    return GetGenericTrail(Fbg.Bll.CONSTS.PFTrials.Notes);
                //case Fbg.Bll.CONSTS.PFs.IncomingOutgoingOnVov:
                //    return GetGenericTrail(Fbg.Bll.CONSTS.PFTrials.IncomingOutgoingOnVov);
                case Fbg.Bll.CONSTS.PFs.IncomingOutgoingEnhacements:
                    return GetGenericTrail(Fbg.Bll.CONSTS.PFTrials.IncomingOutgoingEnhacements);
                case Fbg.Bll.CONSTS.PFs.IncomingOutgoingAllVillages:
                    return GetGenericTrail(Fbg.Bll.CONSTS.PFTrials.IncomingOutgoingAllVillages);
                case Fbg.Bll.CONSTS.PFs.SupportAllVillage:
                    return GetGenericTrail(Fbg.Bll.CONSTS.PFTrials.SupportAllVillage);
                case Fbg.Bll.CONSTS.PFs.SummaryPages:
                    return GetGenericTrail(Fbg.Bll.CONSTS.PFTrials.SummaryPages);
                case Fbg.Bll.CONSTS.PFs.TagsAndFilters:
                    return GetAvailableTagsAndFiltersTrail();
                case Fbg.Bll.CONSTS.PFs.ImprovedVOV:
                    return GetGenericTrail(Fbg.Bll.CONSTS.PFTrials.ImprovedVOV);
                case Fbg.Bll.CONSTS.PFs.ReportImprovements:
                    return GetGenericTrail(Fbg.Bll.CONSTS.PFTrials.ReportImprovements);
                case Fbg.Bll.CONSTS.PFs.MessageImprovements:
                    return GetGenericTrail(Fbg.Bll.CONSTS.PFTrials.MessageImprovements);
                case Fbg.Bll.CONSTS.PFs.BattleSimImprovements:
                    return GetGenericTrail(Fbg.Bll.CONSTS.PFTrials.BattleSimImprovements);
                case Fbg.Bll.CONSTS.PFs.CommandTroopsEnhancements:
                    return GetGenericTrail(Fbg.Bll.CONSTS.PFTrials.CommandTroopsEnhancements);
                default:
                    return null;
            }
        }

        #region GetAvailable...Trial private functions
        /// <summary>
        /// a generic way to get a trial of a one time trial feature - ie, feature that
        /// has just one trial. 
        /// </summary>
        /// <returns></returns>
        private PFTrial GetGenericTrail(Fbg.Bll.CONSTS.PFTrials trial)
        {
            if (realm.PF_HasPFTrial(trial)
                && !PF_WasTrialActivated(trial))
            {
                return realm.PF_PFTrial(trial);
            }
            return null;
        }
        private PFTrial GetAvailableGiantMapTrail()
        {
            if (realm.PF_HasPFTrial(Fbg.Bll.CONSTS.PFTrials.GiantMap)
                && !PF_WasTrialActivated(Fbg.Bll.CONSTS.PFTrials.GiantMap))
            {
                return realm.PF_PFTrial(Fbg.Bll.CONSTS.PFTrials.GiantMap);
            }
            return null;
        }


        /// <summary>
        /// returns null if no trial is available
        /// </summary>
        /// <returns></returns>
        public Fbg.Bll.PFTrial GetAvailableBuildingQTrail()
        {
            //
            // 1st trial
            if (realm.PF_HasPFTrial(Fbg.Bll.CONSTS.PFTrials.BuildingQ_1st)
                    && !PF_WasTrialActivated(Fbg.Bll.CONSTS.PFTrials.BuildingQ_1st))
            {
                return realm.PF_PFTrial(Fbg.Bll.CONSTS.PFTrials.BuildingQ_1st);
            }
            //
            // 2nd village
            else if (realm.PF_HasPFTrial(Fbg.Bll.CONSTS.PFTrials.BuildingQ_3rdVillage)
                && NumberOfVillages >= 2 && NumberOfVillages < 3
                && !PF_WasTrialActivated(Fbg.Bll.CONSTS.PFTrials.BuildingQ_3rdVillage))
            {
                return realm.PF_PFTrial(Fbg.Bll.CONSTS.PFTrials.BuildingQ_3rdVillage);
            }
            //
            // 3rd village
            else if (realm.PF_HasPFTrial(Fbg.Bll.CONSTS.PFTrials.BuildingQ_3rdVillage)
                && NumberOfVillages >= 3 && NumberOfVillages < 5
                && !PF_WasTrialActivated(Fbg.Bll.CONSTS.PFTrials.BuildingQ_3rdVillage))
            {
                return realm.PF_PFTrial(Fbg.Bll.CONSTS.PFTrials.BuildingQ_3rdVillage);
            }
            //
            // 5th village
            else if (realm.PF_HasPFTrial(Fbg.Bll.CONSTS.PFTrials.BuildingQ_5rdVillage)
                && NumberOfVillages >= 3 && NumberOfVillages < 5
                && !PF_WasTrialActivated(Fbg.Bll.CONSTS.PFTrials.BuildingQ_5rdVillage))
            {
                return realm.PF_PFTrial(Fbg.Bll.CONSTS.PFTrials.BuildingQ_5rdVillage);
            }
            //
            // 10th village
            else if (realm.PF_HasPFTrial(Fbg.Bll.CONSTS.PFTrials.BuildingQ_10rdVillage)
                && NumberOfVillages >= 10
                && !PF_WasTrialActivated(Fbg.Bll.CONSTS.PFTrials.BuildingQ_10rdVillage))
            {
                return realm.PF_PFTrial(Fbg.Bll.CONSTS.PFTrials.BuildingQ_10rdVillage);
            }

            return null;
        }
        /// <summary>
        /// returns null if no trial is available
        /// </summary>
        /// <returns></returns>
        public Fbg.Bll.PFTrial GetAvailableTagsAndFiltersTrail()
        {
            // first trial. available anytime. 
            if (realm.PF_HasPFTrial(Fbg.Bll.CONSTS.PFTrials.TagsAndFilters)
                && !PF_WasTrialActivated(Fbg.Bll.CONSTS.PFTrials.TagsAndFilters))
            {
                return realm.PF_PFTrial(Fbg.Bll.CONSTS.PFTrials.TagsAndFilters);
            }
            //
            // 5th village
            else if (realm.PF_HasPFTrial(Fbg.Bll.CONSTS.PFTrials.TagsAndFilters_5thVillage)
               && NumberOfVillages >= 5
               && !PF_WasTrialActivated(Fbg.Bll.CONSTS.PFTrials.TagsAndFilters_5thVillage))
            {
                return realm.PF_PFTrial(Fbg.Bll.CONSTS.PFTrials.TagsAndFilters_5thVillage);
            }

            return null;
        }
        #endregion

        #endregion

        //public void RefreshFriends(string friendsList)
        //{
        //    dtFriends = DAL.Players.RefreshFriends(friendsList, this.Realm.ConnectionStr, this.ID);
        //}

        #region ISerializableToNameValueCollection2 Members

        public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
        {
            SerializeToNameValueCollection(col, String.Empty);
        }
        public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col, string prefix)
        {
            try
            {
                string pre = prefix + "Player[" + this.ID.ToString() + "]";

                if (col == null)
                {
                    ExceptionManager.Publish("Error in Player.SerializeToNameValueCollection. argument 'col' is null");
                }
                else
                {
                    col.Add(pre + "Name", this.Name);
                    //col.Add(pre + "Effect", Effect == null ? "null" : this.Effect.ToString());
                    col.Add(pre + "ReportInfo", this.ReportInfo.ToString());
                    col.Add(pre + "MessageInfo", this.MessageInfo.ToString());
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish("Error in Player.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + ExceptionManager.SerializeToString(e));
            }
        }

        #endregion



        public Quests Quests
        {
            get
            {
                return _quests_old;
            }
        }
        public string TitleName
        {
            get
            {
                return _title.TitleName(Sex);
            }
        }
        public Title Title
        {
            get
            {
                return _title;
            }
        }
        public User User
        {
            get
            {
                return _user;
            }
        }
        public Fbg.Common.Sex Sex
        {
            get
            {
                return _sex;
            }
        }
        public bool Anonymous
        {
            get
            {
                return _annonymous;
            }

        }
        public void Update(bool anonymous, Fbg.Common.Sex sex)
        {
            DAL.Players.Update(Realm.ConnectionStr, ID, anonymous, (short)sex);
            PopulateExtraInfoOnLogin(null);
        }

        public bool OptOutOfEmails
        {
            get
            {
                if (_optOutOfEmails == null)
                {
                    _optOutOfEmails = false;
                    object o = HasFlag_GetData(Flags.Misc_OptOutOfEmails);
                    if (o != null)
                    {
                        _optOutOfEmails = Convert.ToBoolean(o);
                    }
                }
                return (bool)_optOutOfEmails;
            }

            set
            {
                _optOutOfEmails = value;
                SetFlag(Flags.Misc_OptOutOfEmails, _optOutOfEmails.ToString());
            }
        }

        object _optOutOfNotifications; //type bool
        /// <summary>
        /// controls if you want mobile app based notification on this realm
        /// </summary>
        public bool OptOutOfNotifications
        {
            get
            {
                if (_optOutOfNotifications == null)
                {
                    _optOutOfNotifications = false;
                    object o = HasFlag_GetData(Flags.Misc_OptOutOfNotification);
                    if (o != null)
                    {
                        _optOutOfNotifications = Convert.ToBoolean(o);
                    }
                }
                return (bool)_optOutOfNotifications;
            }

            set
            {
                _optOutOfNotifications = value;
                SetFlag(Flags.Misc_OptOutOfNotification, _optOutOfNotifications.ToString());
            }
        }


        public void Title_AcceptNext()
        {
            DAL.Players.Title_AcceptNext(this.realm.ConnectionStr, this.ID);
            Quests2.CompletedQuests_RewardNotClaimed_Invalidate();
            PopulateExtraInfoOnLogin(null); //need to invalidate the object basically
            _giftsAvailability = null; // invalidate gifts available

            // record the highest title reached
            if (this.Title != null)
            {
                if (!realm.IsTemporaryTournamentRealm) // we dont do this for tournament realms
                {
                    int highestTitleID = _user.MyHigestAchievedTitle;
                    if (highestTitleID < this.Title.ID)
                    {
                        _user.SetFlag(User.Flags.Misc_HighestTitleAchieved, this.Title.ID.ToString());
                    }
                }
            }

           
            //@title count, r109,110 give user special morale avatars 34/35 (friar guy / joan lady)
            if (this.Title.ID == 13 && (this.realm.ID == 109 || this.realm.ID == 110))
            {
                Realms.Avatars.unlockUserAvatarByTag(this.User, Avatars.unlockUserAvatarByTag_AvatarTags.moraleRealmSpecial);
            }

            //@title prince, in r117 or 120 give user special donald series avatars 37(king donaldious) / 38(queen mel)
            if (this.Title.ID == 18 && (this.realm.ID == 117 || this.realm.ID == 120))
            {
                Realms.Avatars.unlockUserAvatarByTag(this.User, Avatars.unlockUserAvatarByTag_AvatarTags.donaldSeries);
            }

        }

        /// <summary>
        /// return the poll result if u pass it a vote or not
        /// </summary>
        /// <param name="pollID"></param>
        /// <param name="optionIDs">options ID's separated by comma ex: 1,2,</param>
        /// <returns></returns>
        public DataTable Vote(int pollID, string optionIDs)
        {
            return DAL.Players.Vote(this.User.ID, pollID, optionIDs);
        }

        /// <summary>
        /// Helper overload for Invites_RegisterInvited(string giftID, string invitedIDs).
        /// Whenever possible, use Invites_RegisterInvited(string giftID, string invitedIDs) instead of this. 
        /// </summary>
        /// <param name="giftID">must be of type int or null</param>
        /// <param name="invitedIDs">must be of type String or null</param>
        public void Invites_RegisterInvited(object giftID, object invitedIDs)
        {
            if (invitedIDs != null)
            {
                Invites_RegisterInvited(giftID == null ? String.Empty : giftID.ToString(), (string)invitedIDs);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="giftID">send null or empty string if this is not a gift sent request</param>
        /// <param name="invitedIDs">list of facebook ids comman seperated with NO trailing comma. If null or empty, nothing is done
        /// GOOD: "5555,444,666"
        /// GOOD: "" or null
        /// BAD: "555,444,666,"
        /// GOOD: "555"
        /// BAD: "555,"</param>
        public void Invites_RegisterInvited(string giftID, string invitedIDs, string requestId)
        {
            if (String.IsNullOrEmpty(invitedIDs))
            {
                return;
            }
            if (string.IsNullOrEmpty(giftID))
            {
                DAL.Players.Invites_RegisterInvited(ID, invitedIDs);
            }
            else
            {
                int giftIDint;
                if (Int32.TryParse(giftID, out giftIDint))
                {
                    DAL.Players.Gifts_RegisterSentGifts(ID, giftIDint, invitedIDs, requestId);
                }
            }
        }


        /// <summary>
        /// returns a list of filters this player has defined. 
        /// This list is cached for the lifetime of the object. Call InvalidateFilters() to force refresh 
        /// </summary>
        public List<FilterBase> Filters
        {
            get
            {
                if (_filters == null)
                {
                    _filters = FilterBase.GetFilters(this);
                }
                return _filters;
            }
        }

        List<TagBase> _tags = null;
        public List<TagBase> Tags
        {
            get
            {
                if (_tags == null)
                {
                    _tags = TagBase.GetTags(this);
                }
                return _tags;

            }
        }

        /// <summary>
        /// invalidate the cached filters obtainable by the property 'Filters' forcing a refreh
        /// </summary>
        public void Filters_Invalidate()
        {
            _filters = null;
        }
        /// <summary>
        /// invalidate the cached tags obtainable by the property 'Tags' forcing a refreh
        /// </summary>
        public void Tags_Invalidate()
        {
            _tags = null;
        }


        /// <summary>
        /// may return null if no filter is selected.
        /// If player has 1 village, then this always returns null.
        /// </summary>
        public FilterBase SelectedFilter
        {
            get
            {
                if (NumberOfVillages == 1 && _selectedFilter != null)
                {
                    _selectedFilter = null;
                }
                return _selectedFilter;
            }
            set
            {
                _selectedFilter = value;
            }
        }

        public Folders Folders
        {
            get
            {
                if (_folders == null)
                {
                    _folders = new Folders(this);
                }
                return _folders;
            }
        }

        public bool AreTransportsAvailable
        {
            get
            {
                return DAL.Players.AreTransportsAvailable(this.Realm.ConnectionStr, ID);
            }
        }

        public bool NightBuild_IsActive
        {
            get
            {
                if (NightBuild_ExpiresOn > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public DateTime NightBuild_ExpiresOn
        {
            get
            {
                return _nightBuildActivatedOn.AddMinutes(CONSTS.NightBuildDurationInMinutes);
            }
        }
        public DateTime NightBuild_CanActivateAgainOn
        {
            get
            {
                return _nightBuildActivatedOn.AddHours(CONSTS.NightBuildCycleInHours);
            }
        }
        public void NightBuild_Activate()
        {
            if (NightBuild_CanActivateAgainOn <= DateTime.Now)
            {
                DateTime activatedON = DAL.Players.ActivateNightBuildMode(realm.ConnectionStr, ID);
                _nightBuildActivatedOn = activatedON;
            }
        }

        #region STEWARDSHIP
        public struct SetStewardResult
        {
            /// <summary>
            /// guaranteed not null only if result == ResultCode.OK
            /// </summary>
            public PlayerOther steward;
            public ResultCode result;

            public enum ResultCode
            {
                ok,
                fail_playerNotFound,
                fail_alreadyOneStewardAppointed
            }
        }
        public SetStewardResult Stewardship_SetSteward_Try(string playerName)
        {
            SetStewardResult result = new SetStewardResult();
            result.steward = PlayerOther.GetPlayer(this.realm, playerName, this.ID);
            if (result.steward == null)
            {
                result.result = SetStewardResult.ResultCode.fail_playerNotFound;
            }
            else if (result.steward.PlayerID == this.ID)
            {
                result.result = SetStewardResult.ResultCode.fail_playerNotFound;
            }
            else if (this.Stewardship_GetMyStewards().Rows.Count > 0)
            {
                result.result = SetStewardResult.ResultCode.fail_alreadyOneStewardAppointed;
            }
            else
            {
                result.result = SetStewardResult.ResultCode.ok;
            }

            return result;
        }

        public SetStewardResult Stewardship_SetSteward(string playerName)
        {
            SetStewardResult result = this.Stewardship_SetSteward_Try(playerName);
            if (result.result == SetStewardResult.ResultCode.ok)
            {
                DAL.Players.SetSteward(this.realm.ConnectionStr, this.ID, result.steward.PlayerID);
                Fbg.Bll.Mail.sendEmail(this.ID, result.steward.PlayerID.ToString()
                    , String.Format("{0} appointed you as a steward", this.Name)
                    , String.Format("{0} appointed you as a steward.<BR>You may accept/reject the appointment <a href='accountstewards.aspx' target='_parent' > here</a>. You may also accept the appoitment, login as {0} and then transfer stewardship to someone else", this.Name)
                    , result.steward.PlayerName, this.realm.ConnectionStr);
            }
            return result;
        }
        public struct TransferStewardResult
        {
            /// <summary>
            /// guaranteed not null only if result == ResultCode.OK
            /// </summary>
            public PlayerOther steward;
            public ResultCode result;

            public enum ResultCode
            {
                ok,
                fail_playerNotFound,
                fail_triedTransferToSelf
            }
        }
        public TransferStewardResult Stewardship_TransferSteward_Try(string playerName)
        {
            TransferStewardResult result = new TransferStewardResult();
            result.steward = PlayerOther.GetPlayer(this.realm, playerName, this.ID);
            if (result.steward != null && result.steward.PlayerID == this.ID)
            {
                result.steward = null;
                result.result = result.steward != null ? TransferStewardResult.ResultCode.ok : TransferStewardResult.ResultCode.fail_playerNotFound;
            }
            else if (result.steward.PlayerID == (int)Stewardship_ActiveStewardPlayerID) //disallow transfer to myself
            {
                result.result = TransferStewardResult.ResultCode.fail_triedTransferToSelf;
            }
            else
            {
                result.result = TransferStewardResult.ResultCode.ok;
            }
            return result;
        }

        public TransferStewardResult Stewardship_TransferSteward(string playerName)
        {
            TransferStewardResult result = this.Stewardship_TransferSteward_Try(playerName);
            if (result.result == TransferStewardResult.ResultCode.ok)
            {
                DAL.Players.TransferSteward(this.realm.ConnectionStr, this.ID, result.steward.PlayerID);
                Fbg.Bll.Mail.sendEmail(this.ID, result.steward.PlayerID.ToString()
                    , String.Format("{0} appointed you as a steward", this.Name)
                    , String.Format("{0} appointed you as a steward.<BR>You may accept/reject the appointment <a href='accountstewards.aspx' target='_parent' > here</a>. You may also accept the appoitment, login as {0} and then transfer stewardship to someone else", this.Name)
                    , result.steward.PlayerName, this.realm.ConnectionStr);

            }
            return result;
        }
        /// <summary>
        /// returned table described by Player.CONSTS.MyStewardsColIndex
        /// </summary>
        /// <returns></returns>
        public DataTable Stewardship_GetMyStewards()
        {
            return DAL.Players.GetMySteward(this.realm.ConnectionStr, this.ID);
        }
        /// <summary>
        /// gives you the kingdoms you are a steward of. 
        /// returned table described by Player.CONSTS.MyStewardshipColIndex
        /// </summary>
        public DataTable Stewardship_GetMyStewardship()
        {
            return DAL.Players.GetAccountsISteward(this.realm.ConnectionStr, this.ID);
        }

        public void Stewardship_DeleteAppointedSteward(int recordID)
        {
            DAL.Players.DeleteAppointedSteward(this.realm.ConnectionStr, this.ID, recordID);
        }

        public void Stewardship_AcceptStewardship(int recordID)
        {
            DAL.Players.AcceptStewardship(this.realm.ConnectionStr, this.ID, recordID);
        }

        public void Stewardship_CancelStewardship(int recordID)
        {
            DAL.Players.CancelStewardship(this.realm.ConnectionStr, this.ID, recordID);
        }


        /// <summary>
        /// returns null if player has no active steward, or steward's player ID is he does.
        /// This is retrieved only once, opon login. To find out if a player has a steward that accepted stwarding (ie, an active steward) 
        /// use Stewardship_CanMyActivityContinue
        /// </summary>
        /// <remarks>This is very much related to Stewardship_HasStewardAccepted and holds the same value but it is retrieved at different times
        /// </remarks>
        public object Stewardship_ActiveStewardPlayerID
        {
            get
            {
                return _activeStewardPlayerID;
            }
        }

        private int _loggedInAsStewardRecID = int.MinValue;
        /// <summary>
        /// if true, this means that this is not the actual player but in fact this is a steward logged in as this player
        /// </summary>
        public bool Stewardship_IsLoggedInAsSteward
        {
            get
            {
                return _loggedInAsStewardRecID != int.MinValue;
            }
        }
        /// <summary>
        /// if a steward loggs in as this player, then call this to make sure we know it.
        /// </summary>
        public void Stewardship_SetIsLoggedInAsSteward(int stewardshipRecordID)
        {
            _loggedInAsStewardRecID = stewardshipRecordID;
        }

        /// <summary>
        /// returns true if the current player has continue playing, fro the point of view of stewardship. 
        /// so if this is me playing my own account, this will be true as long as i have not appoointed a steward that has accepted. 
        ///     the moment the steward accepts, this will return false, and that player gets logged out
        /// If this is steward who logged in to account he is stewarding, this will return true as long as the owner does not cancel stewardship
        ///     if onwer canceles, this will return false and player should be logged out. 
        /// </summary>
        public bool Stewardship_CanMyActivityContinue
        {
            get
            {
                RetrievePlayerExtraInfoIfNeeded();
                if (Stewardship_IsLoggedInAsSteward)
                {
                    return Stewardship_IsActive;
                }
                else
                {
                    return !Stewardship_HasStewardAccepted;
                }
            }
        }
        /// <summary>
        /// tells you if a steward that this player has set, has accepted the stewardship. 
        /// player should not be allowed to continue in this case.
        /// This is only valid and hold a valid inforation if this is the player logged in to his account. not if this is a steward logged in to account
        /// he is stewarding
        /// </summary>
        private bool Stewardship_HasStewardAccepted
        {
            get;
            set;
        }
        /// <summary>
        /// if this is a steward logged in to the account he is stewarding, this tells you if the stewardship
        ///     is still valid. ie, if owner cancels stewardship, this will return false and the steward should be immediatelly kicked out of the account
        /// This is only valid and hold valid inforation if this is a steward logged in to account
        /// he is stewarding
        /// </summary>
        private bool Stewardship_IsActive
        {
            get;
            set;
        }


        #endregion

        #region LOGEVENT
        /// <summary>
        /// returns null on success, Exception otherwise
        /// </summary>
        /// <param name="eventID">Try to use a value from Fbg.Bll.CONSTS.UserLogEvents. If no suitable event id exists, 
        /// and you need to use one not listed there, always use number 10,000 or above</param>
        public object LogEvent(int eventID, string message, string data)
        {
            return this.User.LogEvent(this, eventID, message, data);
        }
        /// <summary>
        /// returns null on success, Exception otherwise
        /// </summary>
        public object LogEvent(string message, string data)
        {
            return this.User.LogEvent(this, message, data);
        }
        #endregion

        

        List<GiftAvailability> _giftsAvailability;
        public List<GiftAvailability> GiftsAvailability
        {
            get
            {
                if (_giftsAvailability == null)
                {
                    _giftsAvailability = new List<GiftAvailability>(realm.Gifts.Count);
                    foreach (Gift gift in realm.Gifts)
                    {
                        _giftsAvailability.Add(new GiftAvailability(gift, this));

                    }
                }
                return _giftsAvailability;
            }
        }
        /// <summary>
        /// may return null if not found
        /// </summary>
        public GiftAvailability GiftsAvailabilityByGiftID(int giftID)
        {
            foreach (GiftAvailability ga in GiftsAvailability)
            {
                if (ga.Gift.Id == giftID)
                {
                    return ga;
                }
            }
            return null;
        }


        int _Gifts_NumberOfGiftsICanUseToday_cached = -1;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="useCache">set true if you want the result to be calculated from the session, pass false if you want the check to be made against the database</param>
        /// <returns></returns>
        public int Gifts_NumberOfGiftsICanUseToday(bool useCache)
        {
            if (!useCache)
            {
                _Gifts_NumberOfGiftsICanUseToday_cached = -1; // reset cache
            }

            if (_Gifts_NumberOfGiftsICanUseToday_cached == -1)
            {
                GiftUsageHistory history;
                history = Gifts_GetGiftUsageHistory(useCache);

                //
                // count the # of gifts used in the last X days. X being the carry over param of the realm.
                //  we skip here the gifts used today, as this is done differently. 
                //
                int numGiftsUsedInCarryOverDays = 0;
                DateTime cutOffDate = DateTime.Now.ToUniversalTime().Date.AddDays(0 - realm.MaxNumberOfGiftsThatCanBeUsed_CarryOverDays).Date;
                int i = history.history.Count - 1;
                while (i >= 0)
                {
                    if (history.history[i].date.Date.ToUniversalTime() >= cutOffDate
                        && history.history[i].date.Date.ToUniversalTime() != DateTime.Now.ToUniversalTime().Date)
                    {
                        numGiftsUsedInCarryOverDays += history.history[i].numUsed;
                    }
                    i--;
                }
                //
                // figure out the # of gifts left from carry over days
                //
                          
                int maxCarryOverDays; // Row 1 in excel
                int maxCanUseInCarryOverDays; //row 2 
                int carriedOverUnusedGifts; //row 4

                maxCarryOverDays = Math.Min(Convert.ToInt32(Math.Floor(DateTime.Now.ToUniversalTime().Date.Subtract(realm.OpenOn.ToUniversalTime().Date).TotalDays)), realm.MaxNumberOfGiftsThatCanBeUsed_CarryOverDays);
                maxCanUseInCarryOverDays = (realm.MaxNumberOfGiftsPerDayThatCanBeUsed * (maxCarryOverDays));
                carriedOverUnusedGifts = Math.Max(maxCanUseInCarryOverDays - numGiftsUsedInCarryOverDays, 0);

                //
                // now deal with gifts used today
                //
                int numGiftsUsedToday=0; // row 7 in excel
                if (history.history.Count > 0)
                {
                    if (history.history[history.history.Count - 1].date.Date.ToUniversalTime() == DateTime.Now.ToUniversalTime().Date)
                    {
                        numGiftsUsedToday = history.history[history.history.Count - 1].numUsed;
                    }
                }
                int unusedGiftsToday = realm.MaxNumberOfGiftsPerDayThatCanBeUsed - numGiftsUsedToday;
                //
                // combine today's gifts left ,with carried over gifts left
                //
                _Gifts_NumberOfGiftsICanUseToday_cached = unusedGiftsToday + carriedOverUnusedGifts;
            }

            return _Gifts_NumberOfGiftsICanUseToday_cached;
        }

        public string Gifts_GetGiftUsageHistoryAdmin
        {
            get
            {
                string result="";

                GiftUsageHistory his = Gifts_GetGiftUsageHistory(false);

                foreach(GiftUsageHistory.OnDayGiftUsage one in his.history)
                {
                    result += string.Format("Day:{0} {1} numb used:{2}<BR>", one.date.ToUniversalTime().ToLongDateString(), one.date.ToUniversalTime().ToLongTimeString(), one.numUsed);
                }
                result += string.Format("Now cap:{0}", Gifts_NumberOfGiftsICanUseToday(false));

                return result;
            }
        }


        private GiftUsageHistory Gifts_GetGiftUsageHistory(bool useCache)
        {
            DateTime now = DateTime.Now;

            if (!useCache)
            {
                //trigger getting the data from DB if requested
                object junk = HasFlag(Player.Flags.NumSendableGiftsUsedV2, useCache); 
            }
            object flag = HasFlag_GetData( Player.Flags.NumSendableGiftsUsedV2); 

            GiftUsageHistory history;
            if (flag == null)
            {
                history = new GiftUsageHistory();
                history.history = new List<GiftUsageHistory.OnDayGiftUsage>();
            }
            else
            {
                history = new GiftUsageHistory() { history = json_serializer.Deserialize<List<GiftUsageHistory.OnDayGiftUsage>>((string)flag) };
            }
            _Gifts_NumberOfGiftsICanUseToday_cached = -1; // reset cache

            return history;
        }


        /// <summary>
        ///  consider storing the UTC date instead of server date
        /// </summary>
        private class GiftUsageHistory
        {
            public class OnDayGiftUsage {
                /// <summary>
                /// this is always UTC date. make sure you alwas set it to UTC date and compared with UTC dates!
                /// </summary>
                public DateTime date;
                public int numUsed; }
            public List<OnDayGiftUsage> history;  
            
            public OnDayGiftUsage MostRecentDay

            {
                get
                {
                    if (history == null || history.Count == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return history[history.Count - 1];
                    }
                }
            }
        }


        public void Gifts_RegisterdThatGiftWasUsed()
        {
            //int numGiftsLeft;
            GiftUsageHistory history = Gifts_GetGiftUsageHistory(false);

            // add the one gift use for today
            GiftUsageHistory.OnDayGiftUsage today = history.MostRecentDay;
            if (today == null || today.date.Date != DateTime.Now.ToUniversalTime().Date) {
                today = new GiftUsageHistory.OnDayGiftUsage() { date = DateTime.Now.ToUniversalTime().Date, numUsed = 1 };
                history.history.Add(today);
            } else
            {
                today.numUsed++;
            }

            // clean up any days from history that we no longer need to keep track of 
            while(history.history.Count > realm.MaxNumberOfGiftsThatCanBeUsed_CarryOverDays+1)
            {
                history.history.RemoveAt(0);
            }

            SetFlag(Player.Flags.NumSendableGiftsUsedV2, json_serializer.Serialize(history.history));
        }

        public VillagesCache VillagesCache
        {
            get
            {
                return _villagesCache;
            }
        }

        /// <summary>
        /// Tells you when this player's beginner proteciton expires. only valid if BeginnerProtection_IsNowActive is true
        /// </summary>
        public DateTime BeginnerProtection_ExpiresOn
        {
            get
            {
                TimeSpan beginnerProtLenght = new TimeSpan(Convert.ToInt64(this.Realm.BeginnerRegistrationDays * TimeSpan.TicksPerDay));
                return this.RegisteredOn.Add(beginnerProtLenght);
            }
        }


        /// <summary>
        /// Tells you in how many days this players beginner protection expired only valid if BeginnerProtection_IsNowActive is true
        /// </summary>
        public double BeginnerProtection_ExpiresInDays
        {
            get
            {
                return BeginnerProtection_ExpiresOn.Subtract(DateTime.Now).TotalDays;

            }
        }
        public bool BeginnerProtection_IsNowActive
        {
            get
            {
                return BeginnerProtection_ExpiresOn > DateTime.Now;
            }
        }

        /// <summary>
        /// get or set the avatar. Warning! TO SAVE: call SetAvatarID(int)
        /// </summary>
        public Avatar Avatar
        {
            get
            {
                return _avatar;
            }
            private set
            {
                if (value == null) { throw new Exception("cannot set as null"); }
                _avatar = value;
            }
        }

        public bool SetAvatarID(int AvatarID) {
            int result = DAL.Players.SetAvatarID(this.User.ID, this.ID, AvatarID);
            bool success;
            if (result == 0)
            { //successfully set new avatar

                //update/duplicate common player avatar id into realm player table
                DAL.Players.UpdateRealmPlayerAvatarID(this.realm.ConnectionStr, this.ID, AvatarID);
                this.Avatar = Realms.Avatars.GetAvatar(AvatarID);
                drPlayers[User.CONSTS.PlayerBasicInfoColumnIndex.AvatarID] = AvatarID;
                success = true;

            }
            else {
                success = false;
            }
            return success;
        }

        public void Gifts_Buy(Gift gift, int rid, int vid, string payout)
        {
            DAL.Players.Gifts_Buy(ID, gift.Id, this.FacebookID, rid, vid, payout);
        }
        public void VillageReduction_AddPromoted(int villageid)
        {
            DAL.Players.VillageReduction_AddPromoted(this.realm.ConnectionStr, villageid, ID);
        }
        public void VillageReduction_RemovePromoted(int villageid)
        {
            DAL.Players.VillageReduction_RemovePromoted(this.realm.ConnectionStr, villageid, ID);
        }


        public class VillageReduction
        {
            public VillageReduction(DataRow vill)
            {
                VillageID = vill.Field<int>("VillageID");
                Name = vill.Field<string>("Name");
                XCord = vill.Field<int>("Xcord");
                YCord = vill.Field<int>("Ycord");
                Status = (vill.Field<Object>("PromotedVillageID") != null ? Player.VillageReduction.PromotionStatus.promoted : vill.Field<Object>("AbsorbedVillageID") != null ? Player.VillageReduction.PromotionStatus.absorbed : Player.VillageReduction.PromotionStatus.normal);
            }

            public enum PromotionStatus
            {
                normal,
                promoted,
                absorbed
            }
            public int VillageID { get; set; }
            public int YCord { get; set; }
            public int XCord { get; set; }
            public PromotionStatus Status { get; set; }
            public string Name { get; set; }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public System.Data.EnumerableRowCollection<Player.VillageReduction> VillageReduction_Villages()
        {
            DataTable dt = DAL.Players.VillageReduction_Villages(this.realm.ConnectionStr, ID);
            var query =
                from vill in dt.AsEnumerable()
                select new Player.VillageReduction(vill);

            return query;
        }


        int _numberOfBoughtReearchers = Int32.MinValue;
        /// <summary>
        /// returns the number of bought researchers player has. 
        /// </summary>
        /// <param name="refreshCache">pass in true to reload the datafrom the databae.</param>
        /// <returns></returns>
        /// <remarks>this only returns the researchers that you have purchased. under Facebook, you also need to get friends</remarks>
        public int Researchers_Bought(bool refreshCache)
        {
            if (_numberOfBoughtReearchers == Int32.MinValue || refreshCache)
            {
                _numberOfBoughtReearchers = DAL.Players.GetResearchers(this.realm.ConnectionStr, this.ID);
            }
            return _numberOfBoughtReearchers;
        }
        /// <summary>
        /// same as calling Researchers(false) 
        /// </summary>
        /// <returns></returns>
        public int Researchers_Bought()
        {
            return Researchers_Bought(false);
        }

        List<Fbg.Common.UsersFriend> _friendsResearchers = null;
        /// <summary>
        /// gives you facebook friends who are researchers. note, passing in refreshCache only refreshes the researchers if User.Friends has changed. it does not
        ///     trigger User.Friends to refresh
        /// </summary>
        /// <param name="refreshCache"></param>
        /// <returns></returns>
        public List<Fbg.Common.UsersFriend> Researchers_Friends(bool refreshCache)
        {
            if (_friendsResearchers == null || refreshCache)
            {
                int minRealms = Realm.ID < 24 ? 0 : 1;
                _friendsResearchers = User.Friends.FindAll(a => a.NumOfRealms >= minRealms);
            }
            return _friendsResearchers;
        }

        /// <summary>
        /// returns the number of all researchers player has, including himself.
        /// if isInFacebook, that means we count friends as well 
        /// </summary>
        /// <param name="isInFacebook"></param>
        /// <returns></returns>
        public int Researchers_All(bool isInFacebook)
        {
            return Math.Min(realm.Research_MaxResearchersAllowed, (1 + (isInFacebook ? (Researchers_Bought() + Researchers_Friends(false).Count) : Researchers_Bought())));
        }

        public Fbg.Common.BuyResearcherResult Researchers_Buy(bool isInFacebook)
        {
            // the last param MaxBoughtResearcher is calculatd this wayL
            //  
            return DAL.Players.BuyResearcher(this.Realm.ConnectionStr, ID, (realm.Research_MaxResearchersAllowed - Researchers_All(isInFacebook)) + Researchers_Bought(true));
        }



        public enum NotificationTypes : short
        {
            ClanInvite = 6,
            BuildingUpgradeCompleted = 7, /*never used by the BLL but listed here for documentation purpose*/
            ResearchCompleted = 8, /*never used by the BLL but listed here for documentation purpose*/
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationTypeID">6 - clan invite, 7 building upgrade complete, 8 research completed</param>
        /// <param name="text"></param>
        public void AddNotificationToQ(NotificationTypes notificationTypeID, string text)
        {
            Fbg.DAL.Players.AddPlayerNotificationToQ(this.realm.ConnectionStr, this.ID, (short)notificationTypeID, text);
        }


        public void ChooseGovType(int govTypeID)
        {
            Fbg.DAL.Players.ChooseGovType(this.realm.ConnectionStr, this.ID, govTypeID);
        }

        /// <summary>
        /// calculates the daily reward day (player has accepted every day)
        /// </summary>
        /// <returns></returns>
        public int GetDailyRewardLevel()
        {
            int dailyRewardLevel = 1;
            object flag = HasFlag(Player.Flags.Misc_GiftAccepted, false);
            if (flag != null)
            {
                TimeSpan timeSinceLastAccept = DateTime.Now - (DateTime)flag;
                if (timeSinceLastAccept.TotalDays < 2)
                {
                    object data = HasFlag_GetData(Player.Flags.Misc_GiftAccepted);
                    if (data != null && !(data is DBNull))
                    {
                        dailyRewardLevel = Convert.ToInt32(data);
                    }
                }
                else {
                    object data = HasFlag_GetData(Player.Flags.Misc_GiftAccepted);
                    if (data != null && !(data is DBNull))
                    {
                        dailyRewardLevel = Convert.ToInt32(data);
                        dailyRewardLevel = Math.Min(dailyRewardLevel, 14); //cap the level at 14 before deducting penalty
                        dailyRewardLevel -= Convert.ToInt32(timeSinceLastAccept.TotalDays); //decay 1 level per day since last accepted
                        dailyRewardLevel = Math.Max(dailyRewardLevel, 1);
                    }
                }

            }
            return dailyRewardLevel;
        }

        public DateTime GetDailyRewardNextTime(DateTime flag, int level)
        {

            DateTime timeSinceLastAccept = flag;
            TimeSpan[] levelTimes = new TimeSpan[] { 
                TimeSpan.FromMinutes(0),    //N/A
                TimeSpan.FromMinutes(0),    //to level 1 - N/A - min level is 1
                TimeSpan.FromMinutes(1),    //to level 2
                TimeSpan.FromMinutes(1),    //to level 3
                TimeSpan.FromMinutes(2),    //to level 4
                TimeSpan.FromMinutes(2),    //to level 5
                TimeSpan.FromMinutes(2),    //to level 6
                TimeSpan.FromMinutes(3),    //to level 7
                TimeSpan.FromMinutes(5),    //to level 8
                TimeSpan.FromMinutes(10),   //to level 9
                TimeSpan.FromMinutes(10),   //to level 10
                TimeSpan.FromHours(12),     //to level 11
                TimeSpan.FromHours(22),     //to level 12
                TimeSpan.FromHours(22),     //to level 13
                TimeSpan.FromHours(22)      //to level 14
                //last level is set to 22 and not 24, to give players some lost-hours buffer
            };

            if (level >= levelTimes.Length)
            {
                return timeSinceLastAccept.Add(levelTimes[levelTimes.Length - 1]);
            }
            else {
                return timeSinceLastAccept.Add(levelTimes[level]);  
            }


        }

        public class DailyRewardStatus
        {
            public bool available {get; set;}
            public DateTime nextTime { get; set; }
            public int level { get; set; }
        }

        /// <summary>
        /// calculates the daily reward day (player has accepted every day)
        /// </summary>
        /// <returns></returns>
        public DailyRewardStatus GetDailyRewardStatus()
        {
            DateTime now = DateTime.Now;
            DailyRewardStatus status = new DailyRewardStatus();
            status.available = false;
            status.level = 1;
            status.nextTime = now;

            if (Realm.NewDailyReward)
            {
                if (DateTime.Now < Realm.ClosingOn) // not available on closed realms 
                {
                    object flag = HasFlag(Player.Flags.Misc_GiftAccepted, false);

                    if (flag == null)
                    {
                        status.available = true;
                        status.level = 1;
                    }
                    else
                    {
                        status.level = GetDailyRewardLevel();
                        status.nextTime = GetDailyRewardNextTime((DateTime)flag, status.level);
                        if (now > status.nextTime)
                        {
                            status.available = true;
                        }
                        else
                        {
                            status.available = false;
                        }
                    }
                }
            }

            return status;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="giftID"></param>
        /// <param name="numGifts"></param>
        /// <returns></returns>
        public bool RewardGifts(int giftID, int numGifts)
        {
            bool retVal = false;

            retVal = DAL.Players.RewardGifts(ID, FacebookID, giftID, numGifts);
            this.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.DailyRewardLog, this.realm.GiftByID(giftID).Title, numGifts.ToString());
            return retVal;
        }
        /// <summary>
        /// rewards player chests as part of daily reward
        /// </summary>
        /// <param name="numberOfChests">number of chests palyer wants to buy</param>
        /// <returns></returns>
        public bool RewardChests(int numberOfChests)
        {
            bool retVal = false;

            retVal = DAL.Players.RewardChests(Realm.ConnectionStr, ID, numberOfChests);
            this.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.DailyRewardLog, "Chests", numberOfChests.ToString());
            return retVal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfServants"></param>
        /// <returns></returns>
        public bool RewardServants(int numberOfServants)
        {
            bool retVal = false;

            retVal = utils.Admin_GiveServants(User.ID, numberOfServants, utils.GiveServantsReason.RewardOrPromo);
            this.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.DailyRewardLog, "servants", numberOfServants.ToString());

            return retVal;
        }

        #region Restart
        /// <summary>
        /// Fbg.Bll.Player.Flags.Misc_Restart
        /// </summary>
        /// <returns></returns>
        public int GetRestartCost()
        {
            int restart = 0;
            if (HasFlag(Fbg.Bll.Player.Flags.Misc_Restart, false) != null)
            {
                object data = HasFlag_GetData(Fbg.Bll.Player.Flags.Misc_Restart);
                restart = Convert.ToInt32((string)data) + 1;
            }
            // cap at 100 servants
            return Math.Min(restart * CONSTS.RESTART_COST, 100);
        }

        public enum RestartOnRealmReturnVal
        {
            failed_notEnoughServangs,
            failed_notLeftWithOneVillage,
            failed_noRespawnsOnRealm,
            success
        }
        /// <summary>
        /// this method INITIATES the process of restarting, in that it abandons the player's village. 
        /// it does not give the player a new village, this will be done when when player goes to ChooseQuad.aspx
        /// </summary>
        /// <returns></returns>
        public RestartOnRealmReturnVal RestartOnRealm_abandon()
        {
            if (realm.IsOpen != Bll.Realm.RealmState.Running_OpenToAll)
            {
                return RestartOnRealmReturnVal.failed_noRespawnsOnRealm;
            }
            int cost = GetRestartCost();
            if (Villages.Count != 1)
            {
                return RestartOnRealmReturnVal.failed_notLeftWithOneVillage;
            }

            if (User.Credits < cost)
            {
                return RestartOnRealmReturnVal.failed_notEnoughServangs;
            }

            User.UseCredits(cost, 28, Realm.ID);

            //
            // update the flag that remembers how often this has happened
            //
            int restart = 0;
            if (HasFlag(Fbg.Bll.Player.Flags.Misc_Restart, false) != null)
            {
                object data = HasFlag_GetData(Fbg.Bll.Player.Flags.Misc_Restart);
                restart = Convert.ToInt32((string)data) + 1;
            }
            SetFlag(Fbg.Bll.Player.Flags.Misc_Restart, restart.ToString());
            //
            // abandon the village
            //
            DAL.Villages.AbandonVillage(Realm.ConnectionStr, ID, Villages[0].id, cost);

            return RestartOnRealmReturnVal.success;
        }

        public void Restart_GetNewVillage(int? quad)
        {
            DAL.Villages.Restart_GetNewVillage(Realm.ConnectionStr, ID, Name, quad);
        }


        #endregion

        // used by RetrievePlayerExtraInfoIfNeeded to decide which villages changed after this date
        public DateTime? LastHandledVillageCacheTimeStamp { set; private get; }

        DataTable _listOfVillagesChangedSinceLastHandledVillageCacheTimeStamp;
        public DataTable ListOfVillagesChangedSinceLastHandledVillageCacheTimeStamp
        {
            get{
                RetrievePlayerExtraInfoIfNeeded();
                return _listOfVillagesChangedSinceLastHandledVillageCacheTimeStamp;
            }
            private set
            {
                _listOfVillagesChangedSinceLastHandledVillageCacheTimeStamp = value;
            }
        }



        public DefinedTargets DefinedTargets { get; private set; }
    }
}
