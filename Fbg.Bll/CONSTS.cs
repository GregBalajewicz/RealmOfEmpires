using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Globalization;

namespace Fbg.Bll
{
    public static class CONSTS
    {
        public class LevelPropertyIDs
        {
            public const int TreasuryCapacity = 3;
            public const int CoinMineProduction = 2;
            public const int HQBuildFactor = 1;
        }

        public class BuildingIDs
        {
            public const int Barracks = 1;
            public const int Stable = 2;
            public const int SiegeWorkshop = 10;
            public const int VillageHQ = 3;
            public const int Wall = 4;
            public const int DefenseTower = 7;
            public const int CoinMine = 5;
            public const int Treasury = 6;
            public const int TradePost = 11;
            public const int Palace = 9;
            public const int Tavern = 12;
            public const int Farmland = 8;
            public const int HidingSpot = 13;
        }

        public class UnitIDs
        {
            public const int Infantry = 2;
            public const int LC = 5;
            public const int Knight = 6;
            public const int Ram = 7;
            public const int Treb = 8;
            public const int Gov = 10;
            public const int CM = 11;
            public const int Spy = 12;
        }

        public class Cache
        {
            public static string ThroneRoomPlayerCache_BaseSort = "{0}_tr";
            public static string ThroneRoomPlayerCache = "{0}_tr_{1}";
            public static string PlayerCache = "{0}_P_{1}_{2}";
            public static string PlayersFriendRankingCache = "_Pf";
            public static string ClanCache = "{0}_C_{1}_{2}";
            public static string GlobalStats = "_G";
            public static string Titles = "_Ts";
            public static string Title = "{0}_T_{1}";
        }

        public enum PFs
        {
            GiantMap = 1,
            /// <summary>
            /// Ability to quickly transport silver TO my current villge from other villages and to other players villages. 
            /// </summary>
            ConvenientSilverTransport = 17,
            /// <summary>
            /// Ability to have more than PFInfo.BuildQueueLimit upgrades in build Q
            /// </summary>
            BuildingQ = 3,
            /// <summary>
            /// Map up to 19x19
            /// </summary>
            LargeMap = 4,
            /// <summary>
            /// Building and units summary pages improvements
            /// </summary>
            SummaryPages = 5,           
            /// <summary>
            /// list of troops incoming to/from village/player. Displayed on various pages
            /// </summary>
            IncomingTroopsToFromVillagePlayer =9,
            /// <summary>
            /// Notes on player overview or other village overview
            /// </summary>
            Notes = 10,
            ///// <summary>
            ///// the outgoing and incoming troops list on your village overview
            ///// </summary>
            //IncomingOutgoingOnVov=11,
            /// <summary>
            /// outgoing and incoming troops enhacements
            /// </summary>
            IncomingOutgoingEnhacements = 12,
            /// <summary>
            /// outgoing and incoming troops enhacements
            /// </summary>
            IncomingOutgoingAllVillages = 13,
            /// <summary>
            /// support and troops abroad page able to show all villages 
            /// </summary>
            SupportAllVillage=14,
            /// <summary>
            /// Allows you to access tags and filters
            /// </summary>
            TagsAndFilters=15,
            /// <summary>
            /// various improvements to VOV like counters above buildings etc. 
            /// </summary>
            ImprovedVOV=16,
            ReportImprovements=18, 
            MessageImprovements=19,
            BattleSimImprovements = 20,
            CommandTroopsEnhancements = 21,
            /// <summary>
            /// allows you to cheat with quests
            /// </summary>
            RewardNow= 22,
            MassRecruitAndUpgrade = 23,
            SilverBonus=24,
            DefenseBonus=25,
            AttackBonus=26,
            CutBuildTime1Min=27,
            CutBuildTime15Min=28,
            CutBuildTime1H=29,
            CutBuildTime4H=30,
            DowngradeBuildingLevelNow= 31,
            SupportReturnsFaster =32,
            BoostLoyalty = 33,
            attackSpeedUp= 34,
            subscription= 1000,
        }


        /// <summary>
        /// actual silver bonus PF bonus. ie, if %25, this will return 0.25
        /// </summary>
        public const float PF_SilverBonusPercent = 0.25F;
        /// <summary>
        /// actual defence bonus PF bonus. ie, if %20, this will return 0.20
        /// </summary>
        public const double PF_DefenceBonusPercent = 0.20;

        ///<summary>Cost of various PFs</summary>
        /// <remarks>this is not a good way of implementing this but is better than hardcoding in code all over the place</remarks>
        public class PFCosts
        {
            /// <summary>
            /// cost of downgrade now functionaly. 
            /// </summary>
            public const int DowngradeBuildingLevelNow = 15;
            /// <summary>
            /// cost of upgrade now functionaly. 
            /// </summary>
            public const int CutBuildTime15Min = 5;
            /// <summary>
            /// cost of upgrade now functionaly. 
            /// </summary>
            public const int CutBuildTime1H = 15;
            /// <summary>
            /// cost of upgrade now functionaly. 
            /// </summary>
            public const int CutBuildTime4H = 30;
            /// <summary>
            /// cost of upgrade now functionaly. 
            /// </summary>
            public const int CutBuildTime1Min = 1;
            public const int BoostApproval = 200;
        }
        public enum PFTrials
        {
            //BuildingQ_B2Constant = 1,
            BuildingQ_1st = 2,
            BuildingQ_2ndVillage = 3,
            BuildingQ_3rdVillage = 4,
            BuildingQ_5rdVillage = 5,
            BuildingQ_10rdVillage = 6,
            //UpgradeMax = 7,
            //LargeMap_B2Constant = 8,
            GiantMap = 9,
            IncomingTroopsToFromVillagePlayer = 11,
            Notes = 12,
            IncomingOutgoingOnVov=13,
            IncomingOutgoingEnhacements=14,
            IncomingOutgoingAllVillages=15,
            SupportAllVillage=16,
            SummaryPages=17,
            TagsAndFilters=18, // more trials for this feature available
            ImprovedVOV=19,
            ConvinientSilverTransport=20,
            ReportImprovements=21,
            MessageImprovements=22,
            LargeMap = 23,
            BattleSimImprovements = 24,
            TagsAndFilters_5thVillage = 25,  // more trials for this feature available
            CommandTroopsEnhancements = 26
        }

        public class PFInfo
        {
            public const int BuildQueueLimit = 2;
            public const int NightBuilModeMaxQueueTimeInHours = 8;
        }


        public class TitleLevels
        {
            public const int Peasant = 1;
            public const int Freeman= 3;
            public const int FreePeasant = 4;            
            public const int Merchant = 5;
            public const int GMerchant = 6;
            public const int KnightS= 7;
            public const int Knight = 8;
            public const int GKnight = 9;
            public const int Baronet= 10;
            public const int Baron = 11;
            public const int Lord = 12;
            public const int Viscount = 13;
            public const int King = 21;
            public const int Emperor = 23;
            public const int HEmperor= 24;
        }

        /// <summary>
        /// our internal IDS of the different stories we publish. these are not FB template IDs
        /// </summary>
        public enum Stories
        {
            ClanCreate=1,
            ClanJoined=2,
            TitlesAccepted=3,
            JoinedARealm=4,
            /// <summary>
            /// below Knight
            /// </summary>
            Title1=5,
            /// <summary>
            /// knight or above
            /// </summary>
            Title2=6,
            VillageTakenOver = 7,
            /// <summary>
            /// knight 
            /// </summary>
            TitleK = 8,
            JoinedRealm3
        }

        /// <summary>
        /// used in User.LogEvent and Player.LogEvent
        /// values 1000-5000 are reserved for CommunicationChannel event
        /// values of 10,000 + are reserved for ad hock event
        /// 
        /// WHEN YOU ADD A NEW ID, add some kind of a clean up of old events, if applicable, in dCleanUpDB.sql
        /// </summary>
        public class UserLogEvents
        {
            public const int AllowShortStoriesPromo = 1;
            /// <summary>
            /// depreciated...  use CommunicationChannel from now on
            /// </summary>
            public const int ComingFromFacebookStory = 2; 
            public const int PublishStory= 3;
            public const int LoginAsAdminToolUsed = 4;
            public const int RoePowerToolsDownloaded= 5;
            public const int SupectionsPageAccessed = 6;
            public const int SomeoneHasCookiesDisabled = 7;
            /// <summary>
            /// used for a not previously defined admin function. Examine the message column for info what admin function this was. 
            /// </summary>
            public const int MiscAdminFunctionUsed = 8;
            /// <summary>
            /// Used for different ways we try to encourage the user to invite friends.  
            /// </summary>
            public const int InviteEncouragement = 9;
            public const int DailyRewardLog = 10;
            public const int SleepModeLog = 11;
            public const int VacationModeLog = 12;
            public const int Item2UsageLog = 13;
            /// <summary>
            /// admin notes on this player or user
            /// </summary>
            public const int AdminNotes = 14;
            public const int ChestBuy = 15;
            public const int GovRecruit = 16;

            public const int WeekendModeLog = 17;
            public const int AttackScriptPreventionParamMismatch = 18;
            public const int AttackScriptPrevention_challenge = 19;
            public const int AttackScriptPrevention_channenge_answered = 20;
            public const int AttackScriptPrevention_channenge_shown = 21;

            /// <summary>
            /// values FROM 1000 to 5000 are for communication Chanel - user click an ad, user click an notification etc
            /// </summary>
            public const int CommunicationChannel = 1000;
        }



        public static class SpecialPlayers
        {
            public static Guid roe_team_UserID = new Guid("00000000-0000-0000-0000-000000000001");
            public static string roe_team_PlayerName = "roe_team";
            public static string roe_team_FacebookID = "-1";
            public static Guid Abandoned_UserID = new Guid("00000000-0000-0000-0000-000000000000");
            public static string Abandonedl_PlayerName = "Abandoned";
            public static Guid Rebel_UserID = new Guid("00000000-0000-0000-0000-000000000002");
            public static string Rebel_PlayerName = "*Rebels*";
            private static Dictionary<int, int> _roe_team_PlayerIds;
            private static Dictionary<int, int> _Abandoned_PlayerIds;
            private static Dictionary<int, int> _Rebels_PlayerIds;

            static SpecialPlayers() 
            {
                //
                // populate the doctionary that will hold the playerID for special players for all realms
                _roe_team_PlayerIds = new Dictionary<int,int>(Realms.AllRealms.Count);
                _Abandoned_PlayerIds = new Dictionary<int, int>(Realms.AllRealms.Count);
                _Rebels_PlayerIds = new Dictionary<int, int>(Realms.AllRealms.Count);
                foreach(Realm r in Realms.AllRealms) 
                {
                    _roe_team_PlayerIds.Add(r.ID, Int32.MinValue);
                    _Abandoned_PlayerIds.Add(r.ID, Int32.MinValue);
                    _Rebels_PlayerIds.Add(r.ID, Int32.MinValue);
                }
            }
            public static int roe_team_PlayerId(Realm realm)
            {
                int playerID=Int32.MinValue;
                if (_roe_team_PlayerIds.TryGetValue(realm.ID, out playerID))
                {
                    if (playerID == Int32.MinValue)
                    {
                        User user = new User(roe_team_UserID);
                        playerID = user.PlayerByRealmID(realm.ID).ID;
                        _roe_team_PlayerIds[realm.ID] = playerID;
                    }
                }
                else
                {
                    BaseApplicationException bex = new BaseApplicationException("cannot find realm id in _roe_team_PlayerIds");
                    bex.AddAdditionalInformation("realm", realm);
                    bex.AddAdditionalInformation("_roe_team_PlayerIds", _roe_team_PlayerIds);
                    throw bex;
                }
                return playerID;
            }
            public static int Abandoned_PlayerId(Realm realm)
            {

                int playerID = Int32.MinValue;
                if (_Abandoned_PlayerIds.TryGetValue(realm.ID, out playerID))
                {
                    if (playerID == Int32.MinValue)
                    {
                        User user = new User(Abandoned_UserID);
                        playerID = user.PlayerByRealmID(realm.ID).ID;
                        _Abandoned_PlayerIds[realm.ID] = playerID;
                    }
                }
                else
                {
                    BaseApplicationException bex = new BaseApplicationException("cannot find realm id in _Abandoned_PlayerIds");
                    bex.AddAdditionalInformation("realm", realm);
                    bex.AddAdditionalInformation("_Abandoned_PlayerIds", _Abandoned_PlayerIds);
                    throw bex;
                }
                return playerID;
            }
            public static int Rebels_PlayerId(Realm realm)
            {

                int playerID = Int32.MinValue;
                if (_Rebels_PlayerIds.TryGetValue(realm.ID, out playerID))
                {
                    if (playerID == Int32.MinValue)
                    {
                        User user = new User(Rebel_UserID);
                        if (user.PlayerByRealmID(realm.ID) != null)
                        {
                            playerID = user.PlayerByRealmID(realm.ID).ID;
                        }
                        else
                        {
                            playerID = -1; // no rebels on this realm
                        }
                        _Rebels_PlayerIds[realm.ID] = playerID;
                    }
                }
                else
                {
                    BaseApplicationException bex = new BaseApplicationException("cannot find realm id in _Rebels_PlayerIds");
                    bex.AddAdditionalInformation("realm", realm);
                    bex.AddAdditionalInformation("_Rebels_PlayerIds", _Abandoned_PlayerIds);
                    throw bex;
                }
                return playerID;
            }
        }

        /// <summary>
        /// type of a village based on the owner.
        /// See Fbg.Bll.Utils GetVIllageType to get village type from owner id
        /// </summary>
        public enum VillageType
        {
            Normal,
            Rebel,
            Abandoned
        }

        /// <summary>
        /// this is the email all users get even if we don't have their email
        /// </summary>
        public static string DummyEmail = "nobody@facebook.user";

        /// <summary>
        /// US CultureInfo for converting numbers and such
        /// </summary>
        public  static CultureInfo ciUS = new CultureInfo("en-US");



        public static string PF_NameForPackage(int id)
        {
            switch (id)
            {
                case 999:
                    return "Nobility Package";
                case 1:
                    return "Giant Map";
                case 22:
                    return "Elven Efficiency Spell";
                case 23:
                    return "Bravery Spell";
                case 24:
                    return "Blood Lust Spell";
                case 30:
                    return "God Speed Spell";
                case 32:
                    return "Rebel Rush Spell";
                case 1000:
                    return "Subscription";
                default:
                    return "";
            }
        }
        public static string PF_DescForPackage(int id)
        {
            switch (id)
            {
                case 999:
                    return "";
                case 1:
                    return "";
                case 22:
                    return "(25% More Silver)";
                case 23:
                    return "(20% Defense Bonus)";
                case 24:
                    return "(20% Attack Bonus)";
                case 30:
                    return "(Support Returns 10X Faster)";
                case 32:
                    return "(Attack REBELS 20x faster, excluding governor)";
                case 1000:
                    return "(Gives you full access to your empire)";
                default:
                    return "";
            }
        }





        public static string PremiumFeaturePackageIcon(int packageID, bool isActive)
        {
            switch (packageID)
            {
                case 1:
                    return isActive ? "https://static.realmofempires.com/images/pf_map2.png" : "https://static.realmofempires.com/images/pf_map1.png";
                case 999:
                    return isActive ? "https://static.realmofempires.com/images/pf_np_sml2.png" : "https://static.realmofempires.com/images/pf_np_sml1.png";
                case 22:
                    return isActive ? "https://static.realmofempires.com/images/pf_silvermore2.png" : "https://static.realmofempires.com/images/pf_silvermore1.png";
                case 23:
                    return isActive ? "https://static.realmofempires.com/images/pf_defence2.png" : "https://static.realmofempires.com/images/pf_defence1.png";
                case 24:
                    return isActive ? "https://static.realmofempires.com/images/pf_attack2.png" : "https://static.realmofempires.com/images/pf_attack1.png";
                case 30:
                    return isActive ? "https://static.realmofempires.com/images/pf_fasterreturn2.png" : "https://static.realmofempires.com/images/pf_fasterreturn1.png";
                default:
                    return "";
                case 1000:
                    return isActive ? "https://static.realmofempires.com/images/pf_np_sml2.png" : "https://static.realmofempires.com/images/pf_np_sml1.png";

            }
        }


        public static string PremiumFeaturePackageIcon_Large(int packageID, bool isActive)
        {
            switch (packageID)
            {
                case 1:
                    return isActive ? "https://static.realmofempires.com/images/icons/M_PF_GiantMap.png" : "https://static.realmofempires.com/images/icons/M_PF_GiantMapInactive.png";
                case 999:
                    return isActive ? "https://static.realmofempires.com/images/icons/M_PF_NP.png" : "https://static.realmofempires.com/images/icons/M_PF_NPInactive.png";
                case 22:
                    return isActive ? "https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiency.png" : "https://static.realmofempires.com/images/icons/M_PF_ElvenEfficiencyInactive.png";
                case 23:
                    return isActive ? "https://static.realmofempires.com/images/icons/M_PF_Defense.png" : "https://static.realmofempires.com/images/icons/M_PF_DefenseInactive.png";
                case 24:
                    return isActive ? "https://static.realmofempires.com/images/icons/M_PF_Attack.png" : "https://static.realmofempires.com/images/icons/M_PF_AttackInactive.png";
                case 30:
                    return isActive ? "https://static.realmofempires.com/images/icons/M_PF_Return.png" : "https://static.realmofempires.com/images/icons/M_PF_ReturnInactive.png";
                case 32:
                    return isActive ? "https://static.realmofempires.com/images/icons/M_PF_RebelRush.png" : "https://static.realmofempires.com/images/icons/M_PF_RebelRushInactive.png";
                case 1000:
                    return isActive ? "https://static.realmofempires.com/images/icons/M_PF_NP.png" : "https://static.realmofempires.com/images/icons/M_PF_NPInactive.png";
                default:
                    return "";
            }
        }

    }
}