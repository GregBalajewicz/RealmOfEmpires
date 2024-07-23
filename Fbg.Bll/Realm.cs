using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Globalization;

namespace Fbg.Bll
{ 
    [Serializable]
    public partial class Realm : ISerializableToNameValueCollection2
    {
        public class CONSTS
        {
            internal class ResearchItemTypesColumnIndex
            {
                public static int ResearchItemTypeID = 0;
            }
            internal class ResearchItemsColumnIndex
            {
                public static int ResearchItemTypeID = 0;
                public static int ResearchItemID = 1;
                public static int Cost = 2;
                public static int Name = 3;
                public static int Desc = 4;
                public static int Img1 = 5;
                public static int Img2 = 6;
                public static int PropertyValue = 7;
                public static int PropertyType = 8;
                public static int PropertyID = 9;
                public static int ResearchTime = 10;
                /// <summary>
                /// the building that this RI's property effects
                /// </summary>
                public static int BuildingTypeID = 11;
                public static int EffectName = 12;
                public static int AvailInAge = 13;
                public static int SpriteSheetLocX = 14;
                public static int SpriteSheetLocY = 15;
                public static int ResearchTypePropertyID = 16;
            }
            internal class ResearchItemReqColumnIndex
            {
                public static int ResearchItemTypeID = 0;
                public static int ResearchItemID = 1;
                public static int RequiredResearchItemTypeID = 2;
                public static int RequiredResearchItemID = 3;
            }
            internal class UnitTypeRecruitResearchReqColumnIndex
            {
                public static int UnitTypeID = 0;
                public static int ResearchItemTypeID = 1;
                public static int ResearchItemID = 2;
            }
            internal class BuildingTypesColumnIndex
            {
                public static int BuildingTypeID = 0;
                public static int BuildingName = 1;
                public static int MinimumLevelAllowed = 2;
            }
            internal class BuildingTypesLevelsColumnIndex
            {
                public static int BuildingTypeID = 0;
                public static int Level = 1;
                public static int Cost = 2;
                public static int BuildTime = 3;
                public static int LevelName = 4;
                public static int PropertyID = 5;
                public static int PropertyValue = 6;
                public static int Population = 7;
                public static int PopulationCumulative = 8;
                public static int Points = 9;
                public static int LevelStrength = 10;
                public static int CumulativeLevelStrength = 11;
                public static int EffectName = 12;
                public static int EffectType = 13;
            }
            internal class dtBuildingTypesLevelRequirementsColumnIndex
            {
                public static int BuildingTypeID = 0;
                public static int Level = 1;
                public static int RequiredBuildingTypeID = 2;
                public static int RequiredLevel = 3;
            }
            internal class UnitTypesColumnIndex
            {
                public static int UnitTypeID = 0;
                public static int Name = 1;
                public static int Description = 2;
                public static int Cost = 3;
                public static int Population = 4;
                public static int RecruitmentTime = 5;
                public static int BuildingTypeID = 6;
                public static int Speed = 7;
                public static int CarryCapacity = 8;
                public static int AttackStrength = 9;
                public static int ImageIcon = 10;
                public static int Image = 11;
                public static int SpyAbility = 12;
                public static int CounterSpyAbility = 13;
            }
            internal class UnitTypesRecruitReqColumnIndex
            {
                public static int UnitTypeID = 0;
                public static int BuildingTypeID = 1;
                public static int Level = 2;
            }
            internal class MapColumnIndex
            {
                public static int IconUrl = 0;
                public static int VillageTypeName = 1;
                public static int MaxVillagePoints = 2;
                public static int VillageTypeID = 3;
            }
            internal class MaxUnitTypeIDColumnIndex
            {
                public static int MaxUnitTypeID = 0;
            }

            internal class UnitTypeDefenseColIndex
            {
                public static int UnitTypeID = 0;
                public static int DefendAgainstUnitTypeID = 1;
                public static int DefenseStrength = 2;
            }
            internal class UnitOnBuildingAttackColIndex
            {
                public static int BuildingTypeID = 0;
                public static int UnitTypeID = 1;
                public static int AttackStrength = 2;
            }
            internal class RealmColIndex
            {
                public static int CancelAttackInMin = 0;
                public static int CoinTransportSpeed = 1;
                public static int BeginnerProtectionDays = 2;
            }
            internal class LevelPropertiesColIndex
            {
                public static int BuildingTypeID = 0;
                public static int Level = 1;
                public static int PropertyValue = 2;
                public static int PropertyID = 3;

            }
            internal class PFTrailsColIndex
            {
                public static int PFTrialID = 0;
                public static int FeatureID = 1;
                public static int Description = 2;
                public static int Duration = 3;

            }
            internal class TableIndex
            {
                public const int BuildingTypes = 0;
                public const int BuildingTypesLevels = 1;
                public const int dtBuildingTypesLevelRequirements = 2;
                public const int UnitTypes = 3;
                public const int UnitTypeRecruitReq = 4;
                public const int MaxUnitTypeID = 5;
                public const int Map = 6;
                public const int UnitTypeDefense = 7;
                public const int UnitOnBuildingAttack = 8;
                public const int Realm = 9;
                public const int LevelProperties = 10;
                public const int PFs = 11;
                public const int PFPackages = 12;
                public const int PFsInPackage = 13;
                public const int PFTrails = 14;
                public const int Titles = 15;
                public const int RealmAttributes = 16;
                public const int ResearchItemTypes = 17;
                public const int ResearchItems = 18;
                public const int ResearchItemRequirements = 19;
                public const int Avatars = 20;
                public const int VTS = 21;
                public const int VT_PropTypes = 22;
                public const int VT_Props = 23;
                public const int UnitTypeRecruitResearchReq = 24;
                public const int QuestTemplates = 25;
                public const int QuestTemplates_reward_troops = 26;
                public const int QuestTemplates_reward_pfwithduratio = 27;
                public const int QuestDesc = 28;
                public const int QuestProgression = 29;
                public const int RealmAges = 30;
                public const int MoraleRanges = 31;
                public const int Morale = 32;
            }

            internal class RelIndex
            {
                public static int BuildingTypesToLevels = 0;
                public static int BuildingTypeLevelsToRequirements = 1;
                public static int UnitTypesToReq = 2;
                public static int UnitTypesToDefense = 3;
                public static int UnitTypesToBuildingAttack = 4;
                // 5th one is not defined but exists
                public static int RITypesToRIs = 6;
                public static int RIToReq = 7;
                public static int RIToUTReq = 8;


            }

            internal class OtherVillageOverviewIndex
            {
                public static int VillageInfoMin = 0;
            }

            /// <summary>
            /// describes the columns of a RealmAges
            /// </summary>
            public class RealmAgesColIndex
            {
                public static int AgeNum = 0;
                public static int Desc = 1;
                public static int Untill = 2;
                public static int InfoUrl = 3;
                public static int InfoText = 4;
            }

            /// <summary>
            /// describes the columns of a table returned from GetFBFriendsInRealm
            /// </summary>
            public class FBFriendsInRealmColIndex
            {
                public static int PlayerID = 0;
                public static int Name = 1;
                public static int NumOfVillages = 2;
                public static int TotalPoints = 3;
                public static int ClanId = 4;
                public static int ClanName = 5;
                public static int FBID = 6;
            }
            /// <summary>
            /// describes the columns of a table returned from PFPackages
            /// </summary>
            public class PFPackagesColIndex
            {
                public static int PFPackageID = 0;
                public static int Cost = 1;
                public static int Duration = 2;
            }
            /// <summary>
            /// describes the columns name of a table returned from PFPackages
            /// </summary>
            public class PFPackgesPrimaryColName
            {
                public static string PackageID = "PFPackageID";
            }
            /// <summary>
            /// describes the columns of a Titles table 
            /// </summary>
            public class TitlesColIndex
            {
                public static int TitleID = 0;
                public static int Title_Male = 1;
                public static int Title_Female = 2;
                public static int Desc = 3;
                public static int MaxPoints = 4;
                public static int Level = 5;
                public static int XP = 6;
                public static int XP_Cumulative = 7;
            }
            /// <summary>
            /// describes the columns of a RealmAttributes table 
            /// </summary>
            internal class RealmAttribsColIndex
            {
                public static int AttribID = 0;
                public static int AttribValue = 1;
                public static int AttribDesc = 2;
            }
            /// <summary>
            /// describes the columns of a RealmAttributes table 
            /// </summary>
            internal class RealmAttribsColNames
            {
                public static string AttribID = "AttribID";
                public static string AttribValue = "AttribValue";
                public static string AttribDesc = "AttribDesc";
            }

            /// <summary>
            /// describes the realm attribute IDs - see RealmAttributes table,  RealmAttribsColIndex.AttribID column
            /// </summary>
            internal enum RealmAttributeIDs
            {
                ClanIniteLimit_MinID = 1000,
                ClanInviteLimit_MaxIS = 1010,
                BattleHandicap = 5,
                VillagePlacementAlgorithmVersion = 3,
                SpyAlgorithmVersion = 4,
                RealmClosedToNewPlayerIndicator = 7,
                OverAllSpeedFactor = 8,
                RealmEntryLimitations = 9,
                AttackFreeze_StartsOn = 10,
                AttackFreeze_EndOn = 11,
                AttackFreeze_Msg_WhenAttacking = 12,
                AttackFreeze_Msg_Report = 13,
                TemporaryTournamentRealm = 14,
                NeedNInSomeOtherRealmToEnterThisOne = 15,
                SleepMode_Duration = 16,
                SleepMode_HrsTillActive = 17,
                SleepMode_AvailableOnceXHours = 18,
                Gifts_areActive = 19,
                IsVPRealm = 20,
                UnitDisertionScalingFactor = 21,
                ClanLimit = 22,
                StewardshipType = 23,
                AreCapitalVillagesActive = 27,
                BattleHandicapParam_StartRatio = 28,
                BattleHandicapParam_MaxHandicap = 29,
                BattleHandicapParam_Steepness = 30,
                AreQuestsDisabled = 32,
                EntryCost = 34,
                TournamentRealm_PrizeType = 37,
                TournamentRealm_GamePlayType = 38,
                MaxGiftsPerDayUse = 39,
                Consolidation_NumVillagesAbsorbed = 41,
                GovTypesEnabled = 42,
                Consolidation_time_of_consolidation = 43,
                Consolidation_AttackFrezeStartsOn = 44,
                Consolidation_AttackFrezeStopsOn = 45,
                NewDailyReward = 46,
                AccessDeviceTypeLimitation = 47,
                RealmEntryLimitationMaxXP = 48,
                BonusVillChange = 49,
                LocalDBVersion = 50,
                CapitalVillageProtectionDurationInDays = 51,
                RealmType = 2000,
                RealmSubType = 2001,
                Vacation_baseDays = 52, //number of base vacation days a realm provides, player xp increases it
                Vacation_activationDelayDays = 53, //number of days it takes for vacation mode to kick in after being requested
                Vacation_perUseMin = 54, //minimum number of days per activation, if canceled early, it gets deducted by this anyway
                CreditFarm_allowed = 55, //does this realm allow credit farming or not
                Vacation_reactivationDelay = 56, //number of days to pass since last end date to allow reactivation
                CreditFarm_bonusDate = 57, //if date in future, a bonus event is active
                CreditFarm_bonusMultiplier = 58, //if a bonus event is active, instead of 1 each farm grants this many amounts instead
                CreditFarm_bonusEventDesc = 59, //description of the event, can be formatted with data
                CreditFarm_bonusEventIcon = 60, //icon of the event
                BattleAlgorithmVersion = 61,
                UnitDisertionMaxPopulation = 62,
                UnitDisertionMinDistance = 63,
                HoursBeforeCloseRealmToDisableClanChanges = 64,
                Vacation_realmMaxDays = 65, //max number of days allowed on this realm, will even override player days 
                Vacation_perUseMax = 66, //maximum consecutive days per single usage, on this realm
                RealmTheme = 67, //desert | Europe
                LimitAccessToVIPsOnly = 68, // 1|0
                MaxUpgradeSpeedupsInMinutes = 69, // 1|0
                // ID 70 to 90 reserved for morale system
                Morale_isActive = 70,
                Morale_decrease_normal = 71,
                Morale_decrease_npc = 72,
                Morale_minMorale = 73,
                Morale_maxMorale = 74,
                Morale_maxMorale_Normal = 75,
                Morale_minMorale_Normal = 76,
                Morale_increasePerHour = 77,
                // ID 70 to 90 reserved for morale system
                AgeFromWhichToDisableClanChanges = 91,
                MapEvents_CaravanSpawnCap = 92, //if present, will govern how many spawns a day, if not preset, it defaults to X in mapevents.cs
                MaxGiftsDaysCarryOver = 93,

                WeekendMode_baseDays = 94, //how many days WeeendMode lasts upto
                WeekendMode_activationDelayMinimumHours = 95,   //minimum time takes for WM to activate
                WeekendMode_reactivationDelayDays = 96, //how many days after last end/cancel before reactivation



            }


        }



        /// <summary>
        /// Attack freeze will be in effect, in specified time, 
        ///     if the starts on& ends on parameters are found in the realm attributes table.
        /// </summary>
        public class AttackFreeze  
        {
            DateTime _startsOn=DateTime.MinValue;
            DateTime _endsOn = DateTime.MinValue;
            String _msgWhenAttacking=string.Empty;


            public AttackFreeze(DataTable dt)
            {
                DataRow[] drs;
                //
                // get the starts on attrib. if not found, then no attack freeze. 
                //
                drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.AttackFreeze_StartsOn));
                if (drs.Length > 0)
                {
                    _startsOn = Convert.ToDateTime(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                    //
                    // get the ends on attrib. if not found, something is wrong. cancel the freeze
                    //
                    drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.AttackFreeze_EndOn));
                    if (drs.Length > 0)
                    {
                        _endsOn = Convert.ToDateTime(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                    }
                    else
                    {
                        _startsOn = DateTime.MinValue;
                        ExceptionManager.Publish("UNREPORTED ERROR - error building AttackFreeze. no AttackFreeze_EndOn found. Freeze is NOT ACTIVE");
                        return;
                    }
                    //
                    // get the about to attack message
                    //
                    drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.AttackFreeze_Msg_WhenAttacking));
                    if (drs.Length > 0)
                    {
                        _msgWhenAttacking= drs[0][CONSTS.RealmAttribsColIndex.AttribValue].ToString();
                    }
                    else
                    {
                        ExceptionManager.Publish("UNREPORTED EXCEPTION - WARNING, no AttackFreeze_Msg_WhenAttacking found. Using a generic message instead.");
                        _msgWhenAttacking = "No attacks allowed during the attack freeze.";
                    }
                }
                else
                {
                    _startsOn = DateTime.MinValue;
                }
            }

            public bool IsActiveNow
            {
                get
                {
                    return IsInFreezeTime(DateTime.Now);
                }
            }

            public bool IsInFreezeTime(DateTime date)   
            {
                if (_startsOn == DateTime.MinValue)
                {
                    return false;
                }
                else
                {
                    return (date >= _startsOn && date <= _endsOn) ? true : false;
                }

            }

            public string MessageWhenTryingToAttack
            {
                get
                {
                    return _msgWhenAttacking;
                }
            }

            public TimeSpan AttackFreezeStartIn
            {
                get
                {
                    return _startsOn.Subtract(DateTime.Now);
                }
            }
        }

        

        /// <summary>
        /// 
        /// </summary>
        public class Consolidation  
        {
            DateTime _attackFreestartsOn=DateTime.MinValue;
            DateTime _attackFreeEndsOn = DateTime.MinValue;
           

            public Consolidation(DataTable dt)
            {
                DataRow[] drs;
                //
                // get the starts on attrib. if not found, then no attack freeze. 
                //
                drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.Consolidation_AttackFrezeStartsOn));
                if (drs.Length > 0)
                {
                    _attackFreestartsOn = Convert.ToDateTime(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                    //
                    // get the ends on attrib. if not found, something is wrong. cancel the freeze
                    //
                    drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.Consolidation_AttackFrezeStopsOn));
                    if (drs.Length > 0)
                    {
                        _attackFreeEndsOn = Convert.ToDateTime(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                    }
                    else
                    {
                        _attackFreestartsOn = DateTime.MinValue;
                        ExceptionManager.Publish("UNREPORTED ERROR - error building AttackFreeze for Consolidation. no AttackFreeze_EndOn found. Freeze is NOT ACTIVE");
                        return;
                    }                   
                    NumVillagesAbsorbed = RealmAttributesGetHelper(5, dt, CONSTS.RealmAttributeIDs.Consolidation_NumVillagesAbsorbed);
                    TimeOfConsolidation = RealmAttributesGetHelper(DateTime.MinValue, dt, CONSTS.RealmAttributeIDs.Consolidation_time_of_consolidation);

                }
                else
                {
                    _attackFreestartsOn = DateTime.MinValue;
                }
            }
            public bool isActiveOnThisRealm { get { return TimeOfConsolidation != DateTime.MinValue; } }
            public int NumVillagesAbsorbed { get; private set; }
            public DateTime TimeOfConsolidation { get; private set; }

            public bool IsAttackFreezeActiveNow
            {
                get
                {
                    return IsInFreezeTime(DateTime.Now);
                }
            }

            public bool IsInFreezeTime(DateTime date)   
            {
                if (_attackFreestartsOn == DateTime.MinValue)
                {
                    return false;
                }
                else
                {
                    return (date >= _attackFreestartsOn && date <= _attackFreeEndsOn) ? true : false;
                }

            }
          
            public TimeSpan AttackFreezeStartIn
            {
                get
                {
                    return _attackFreestartsOn.Subtract(DateTime.Now);
                }
            }
            public DateTime AttackFreezeStartOn
            {
                get
                {
                    return _attackFreestartsOn;
                }
            }
            public DateTime AttackFreezeeEndsOn
            {
                get
                {
                    return _attackFreeEndsOn;
                }
            }

            public TimeSpan TimeForPromotion
            {
                get
                {
                    return _attackFreeEndsOn.Subtract(TimeOfConsolidation);
                }
            }

            public bool IsConsolidationDone {
                get
                {
                    // this is a hack introduced march 8 2017 to allow us to signal that consolidation has been done. 
                    return _attackFreestartsOn == TimeOfConsolidation;
                }
            }

        }

        /// <summary>
        /// Sleep Mode
        /// </summary>
        public class SleepMode
        {
            double _durationInHours = 0;
            double _timeTillActiveInHours = 0;
            double _availableOnceEveryXHours = 0;


            public SleepMode(DataTable dt)
            {
                DataRow[] drs; 
                //
                // get the duration attrib. if not found, or 0 then sleep mode on this realm. 
                //
                drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.SleepMode_Duration));
                if (drs.Length > 0)
                {
                    _durationInHours = Convert.ToDouble(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                    if (_durationInHours != 0)
                    {
                        //
                        // get the time till actrive. if not found, something is wrong. cancel the sleepmode
                        //
                        drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.SleepMode_HrsTillActive));
                        if (drs.Length > 0)
                        {
                            _timeTillActiveInHours = Convert.ToDouble(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                        }
                        else
                        {
                            _durationInHours = 0;
                            ExceptionManager.Publish("UNREPORTED ERROR - error building SleepMode. no SleepMode_MinTillActive found. sleepmode is NOT ACTIVE");
                            return;
                        }
                        //
                        // get the available once X hours. if not found, something is wrong, cancel sleep mode
                        //
                        drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.SleepMode_AvailableOnceXHours));
                        if (drs.Length > 0)
                        {
                            _availableOnceEveryXHours = Convert.ToDouble(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                        }
                        else
                        {
                            _durationInHours = 0;
                            ExceptionManager.Publish("UNREPORTED ERROR - no SleepMode_AvailableOnceXHours found. sleepmode is NOT ACTIVE");
                        }
                        if (_availableOnceEveryXHours <= 0 || _availableOnceEveryXHours < _durationInHours)
                        {
                            _durationInHours = 0;
                            ExceptionManager.Publish("UNREPORTED ERROR - SleepMode_AvailableOnceXHours either 0 or less than duration. sleepmode is NOT ACTIVE");
                        }
                    }
                }
            }

            public bool IsAvailableOnThisRealm
            {
                get
                {
                    return _durationInHours != 0;
                }
            }

            /// <summary>
            /// in hours
            /// </summary>
            public double Duration
            {
                get
                {
                    return _durationInHours;
                }
            }

            /// <summary>
            /// in hours
            /// </summary>
            public double TimeTillActive
            {
                get
                {
                    return _timeTillActiveInHours;
                }
            }

            /// <summary>
            /// in hours
            /// </summary>
            public double AavailableOnceEveryXHours
            {
                get
                {
                    return _availableOnceEveryXHours; 
                }
            }

            public bool IsPlayerInSleepMode(DateTime sleepModeActiveFrom)
            {
                if (!IsAvailableOnThisRealm)
                { 
                    return false;
                }
                if (sleepModeActiveFrom <= DateTime.Now
                    && sleepModeActiveFrom.AddHours(Duration) > DateTime.Now)
                {
                    return true;
                }
                return false;
            }
            public bool IsPlayerInSleepMode(Player player)
            {
                return IsPlayerInSleepMode(player.SleepMode_ActiveOn);
            }
          }

        /// <summary>
        /// Vacation Mode
        /// </summary>
        public class VacationMode
        {

            int _realmBaseDays = 0;
            int _activationDelayDays = 0;
            int _perUseMin = 0;
            int _reactivationDelay = 0;

            int _realmMaxDays = 0;
            int _perUseMax = 0; 

            public VacationMode(DataTable dt)
            {
                DataRow[] drs;

                drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.Vacation_baseDays));

                if (drs.Length > 0)
                {
                    _realmBaseDays = Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                    if (_realmBaseDays != 0)
                    {
                        //
                        // get the time till active. if not found, something is wrong. cancel the vacation
                        //
                        drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.Vacation_activationDelayDays));
                        if (drs.Length > 0)
                        {
                            _activationDelayDays = Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                        }
                        else
                        {
                            _realmBaseDays = 0;
                            ExceptionManager.Publish("UNREPORTED ERROR - error building VacationMode. no Vacation_activationDelayDays found. VacationMode is NOT ACTIVE");
                            return;
                        }

                        //
                        // get the minimum active time. if not found, something is wrong. cancel the vacation
                        //
                        drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.Vacation_perUseMin));
                        if (drs.Length > 0)
                        {
                            _perUseMin = Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                        }
                        else
                        {
                            _realmBaseDays = 0;
                            ExceptionManager.Publish("UNREPORTED ERROR - error building VacationMode. no Vacation_minimumDays found. VacationMode is NOT ACTIVE");
                            return;
                        }

                        //
                        // get the reactivation delay
                        //
                        drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.Vacation_reactivationDelay));
                        if (drs.Length > 0)
                        {
                            _reactivationDelay = Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                        }
                        else
                        {
                            _realmBaseDays = 0;
                            ExceptionManager.Publish("UNREPORTED ERROR - error building VacationMode. no Vacation_reactivationDelay found. VacationMode is NOT ACTIVE");
                            return;
                        }

                        //
                        // get the maximum realm VM days allowed, if non found, set to 9999
                        //
                        drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.Vacation_realmMaxDays));
                        if (drs.Length > 0)
                        {
                            _realmMaxDays = Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                        }
                        else
                        {
                            _realmMaxDays = 9999;
                        }

                        //
                        // get the maximum active time per use, if non found, set to 9999
                        //
                        drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.Vacation_perUseMax));
                        if (drs.Length > 0)
                        {
                            _perUseMax = Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                        }
                        else
                        {
                            _perUseMax = 9999;
                        }

                        
                    }
                }
            }

            /// <summary>
            /// Is vacation mode allowed in this realm, if baseMaxDays comes out as 0 for any reason, its not allowed in realm
            /// </summary>
            public bool Allowed
            {
                get
                {
                    return _realmBaseDays != 0;
                }
            }

            /// <summary>
            /// Base MIN vacation days in a realm
            /// </summary>
            public int RealmBaseDays
            {
                get
                {
                    return _realmBaseDays;
                }
            }

            /// <summary>
            /// Minimum days used per activation
            /// </summary>
            public int PerUseMinimum
            {
                get
                {
                    return _perUseMin;
                }
            }

            /// <summary>
            /// Days it takes for vacation mode to turn on in a realm
            /// </summary>
            public int ActivationDelayDays
            {
                get
                {
                    return _activationDelayDays;
                }
            }

            /// <summary>
            /// Days it takes to enable reactivation since last vacation end
            /// </summary>
            public int ReactivationDelayDays
            {
                get
                {
                    return _reactivationDelay;
                }
            }

            /// <summary>
            /// Maximum days of VM allowed on this realm, can potentially override player days
            /// </summary>
            public int RealmMaxDays
            {
                get
                {
                    return _realmMaxDays;
                }
            }

            /// <summary>
            /// Maximum consecutive days used per activation
            /// </summary>
            public int PerUseMaximum
            {
                get
                {
                    return _perUseMax;
                }
            }

            /// <summary>
            /// Given a date time of VM request, ESTIMATE if player would be in VM
            /// NOTE: this is an estimate because we dont knwo ho wmany days the player has
            /// NOTE: DO NOT USE THIS AS A SUREFIRE WAY OF DETERMING VM, this is just for ClanMembers page
            /// for surefire way, get fbgplayer and get their vacation status
            /// </summary>
            /// <param name="vacationModeRequested"></param>
            /// <returns></returns>
            public bool IsPlayerInVacationMode(DateTime vacationModeRequested)
            {
                if (!Allowed)
                {
                    return false;
                }
                if (vacationModeRequested.AddDays(ActivationDelayDays).Ticks < DateTime.Now.Ticks)
                {
                    return true;
                }
                return false;
            }


        }


        /// <summary>
        /// Weekend Mode
        /// </summary>
        public class WeekendMode
        {

            int _realmBaseDays = 0; //how many days per week, if ZERO, it will mean WM is off on realm
            int _activationDelayMinimumHours = 0; //minimum time takes for WM to activate
            int _reactivationDelayDays = 0; //how many days after last end/cancel before reactivation

            public WeekendMode(DataTable dt)
            {
                DataRow[] drs;

                //
                // get the reactivation delay  - if param not found, should not be in WM
                //
                drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.WeekendMode_baseDays));

                if (drs.Length > 0)
                {
                    _realmBaseDays = Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                    if (_realmBaseDays != 0)
                    {
                        //
                        // activation delay - if param not found, should not be in WM
                        //
                        drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.WeekendMode_activationDelayMinimumHours));
                        if (drs.Length > 0)
                        {
                            _activationDelayMinimumHours = Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                        }
                        else
                        {
                            _realmBaseDays = 0;
                            return;
                        }

                        //
                        // get the reactivation delay  - if param not found, should not be in WM
                        //
                        drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.WeekendMode_reactivationDelayDays));
                        if (drs.Length > 0)
                        {
                            _reactivationDelayDays = Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                        }
                        else
                        {
                            _realmBaseDays = 0;
                            return;
                        }


                    }
                }
            }

            /// <summary>
            /// Is Weekend mode allowed in this realm, if baseMaxDays comes out as 0 for any reason, its not allowed in realm
            /// </summary>
            public bool Allowed
            {
                get
                {
                    return _realmBaseDays != 0;
                }
            }

            /// <summary>
            /// How many days per week allowed
            /// </summary>
            public int RealmBaseDays
            {
                get
                {
                    return _realmBaseDays;
                }
            }

            /// <summary>
            /// Time it takes for weekend mode to turn on in a realm
            /// </summary>
            public int ActivationDelayMinimumHours
            {
                get
                {
                    return _activationDelayMinimumHours;
                }
            }

            /// <summary>
            /// Days it takes to enable reactivation since last WM end
            /// </summary>
            public int ReactivationDelayDays
            {
                get
                {
                    return _reactivationDelayDays;
                }
            }

            public bool IsPlayerInWeekendMode(DateTime weekendModeActiveFrom)
            {
                if (!Allowed)
                {
                    return false;
                }

                //if activated in the past, and still active beyond now
                if (weekendModeActiveFrom.Ticks <= DateTime.Now.Ticks && 
                    weekendModeActiveFrom.AddDays(RealmBaseDays).Ticks > DateTime.Now.Ticks)
                {
                    return true;
                }
                return false;
            }



        }



        public enum AccessDeviceTypeLimitations : int
        {
            OpenToAll = 0,
            MobDevicesOnly = 1,
            OpenToAll_ButRegisterViaDesktopOnly= 2
        }

        public enum EffectType : int
        {
            Int = 1,
            Double = 2,
            Percent = 3
        }

        public enum StewardshipTypes
        {
            classic = 0,
            DefenceOnly=1,
            NoStewardShip=2,
            DefenceOnlyNoSupportingOthers=3
        }

        public enum Themes
        {
            europe=0,
            desert=1,
            highlands=2

        }
       
        public Themes Theme
        {
            private set;
            get;
        }

        private void Theme_set(int themeID)
        {
            if (themeID == 0)
            {
                Theme = Themes.europe;
            }
            else if (themeID == 1)
            {
                Theme = Themes.desert;
            }
            else if (themeID == 2)
            {
                Theme = Themes.highlands;
            }
            else
            {
                Theme = Themes.europe;
            }
        }

        public bool isLimitAccessToVIPsOnly
        {
            private set;
            get;
        }
        public int maxUpgradeSpeedupInMinutes
        {
            private set;
            get;
        }

        DataRow drRealm;
        DataSet ds;
        private DataTable dtBuildingTypes;
        private DataTable dtBuildingTypesLevels;
        private DataTable dtBuildingTypesLevelRequirements;
        private DataTable dtUnitTypes;
        private DataTable dtUnitTypesRecruitReq;
        private DataTable dtMap;
        private DataTable dtUnitTypeDefense;
        private DataTable dtUnitOnBuildingAttack;
        private DataTable dtRealm;
        private DataTable dtLevelProperties;
        private DataTable dtPFTrials;
        private DataTable dtPFPackages;
        private DataTable dtPFs;
        private DataTable dtTitles;
        private DataTable dtRealmAttributes;
        private DataTable dtResearchItemTypes;
        private DataTable dtResearchItems;
        private DataTable dtResearchItemRequirements;
        private DataTable dtUnitTypesRecruitResReq;
        private DataTable dtRealmAges;
       

        List<PFPackage> _PFPackages;
        SortedList<int, PF> _PFs;

        List<BuildingType> _buildingTypes;
        List<UnitType> _unitTypes;
        Dictionary<BuildingType, List<UnitType>> _unitTypesByBuildingType;
        List<Title> _titles;
        List<Gift> _gifts;


        string _connectionStr;
        string _name;
        string _desc;
        int _id;
        int _population;
        int _maxplayers;
        DateTime _openOn;
        DateTime _closingOn;
        bool _allowPreReg;
        Realms.ActiveStatus _activeStatus;
        int _clanLimit;
        int _spyAlgorithmVersion;
        int _villagePlacementAlgorithmVersion;
        float _closesToNewPlayersInHours;
        float _overAllSpeedFactor=1;
        Int16 _realmTitleEntryLimitations = Int16.MinValue;
        AttackFreeze _attackFreeze;
        SleepMode _sleepMode;
        VacationMode _vacationMode;
        WeekendMode _weekendMode;
        Consolidation _consolidation;
        bool _isTemporaryTournamentRealm = false;
        bool _needNPInSomeOtherRealmToEnterThisOne = false;
        bool _gifts_areActive = true;
        bool _isVPRealm = false;
        StewardshipTypes _stewardshipType = StewardshipTypes.classic;
        bool _areCapitalVillagesActive;
        Research _research;        

        int _maxUnitTypeID; // the largest unit type ID
        // both of these arrays must be first initialized using BuildQuickLookups() before use
        UnitType[] _unitTypeQuickLookup;
        int[] _normalizedUnitTypeQuickLookup;

        //will need to come from realm attr, for now its temp 30
        public int travellingCaravanLimitDaily { get; set; }

        Dictionary<int, VillageImgPack> _villageImagePacks;

        /// <summary>
        /// NOT THREAD SAFE. Assuming this is called from a thread safe location, the Realms constructor. 
        /// </summary>
        /// <param name="drRealm"></param>
        public Realm(DataRow drRealm)
        {
            this.drRealm = drRealm;

            _connectionStr = (string)drRealm[Realms.CONSTS.RealmsColumnIndex.ConnectionStr];
            _name       = (string)drRealm[Realms.CONSTS.RealmsColumnIndex.Name];
            _desc       = (string)drRealm[Realms.CONSTS.RealmsColumnIndex.Description];
            _id         = (int)drRealm[Realms.CONSTS.RealmsColumnIndex.RealmID];
            _population = (int)drRealm[Realms.CONSTS.RealmsColumnIndex.PlayerCount];
            _maxplayers = (int)drRealm[Realms.CONSTS.RealmsColumnIndex.MaxPlayers];
            _openOn     = (DateTime)drRealm[Realms.CONSTS.RealmsColumnIndex.OpenOn];
            _allowPreReg = (bool)drRealm[Realms.CONSTS.RealmsColumnIndex.AllowPrereg];
            _activeStatus= (Realms.ActiveStatus)drRealm[Realms.CONSTS.RealmsColumnIndex.ActivitStatus];
            ExtendedDesc = (drRealm[Realms.CONSTS.RealmsColumnIndex.ExtendedDesc] is System.DBNull ? String.Empty: (string)drRealm[Realms.CONSTS.RealmsColumnIndex.ExtendedDesc]);

            if (!(drRealm[Realms.CONSTS.RealmsColumnIndex.ClosingOn] is DBNull))
            {
                _closingOn = (DateTime)drRealm[Realms.CONSTS.RealmsColumnIndex.ClosingOn];
            }
            else
            {
                _closingOn = DateTime.MaxValue;
            }

            PlayerGenerated = new PlayerGeneratedSettings(drRealm[Realms.CONSTS.RealmsColumnIndex.PlayerGenerated_ByUserID], drRealm[Realms.CONSTS.RealmsColumnIndex.PlayerGenerated_Password]);

            LoadData();
        }

        /// <summary>
        /// the maximum number of researchers allowed 
        /// </summary>
        public int Research_MaxResearchersAllowed
        {
            get
            {
                    return 1;
            }
        }
        public Research Research
        {
            get
            {
                if (_research == null)
                {
                    _research = new Research(this, ds, dtResearchItemTypes);
                }                
                return _research;
            }
        }

        public VillageImgPack VillageImagePack(int id)
        {
            return _villageImagePacks[id];
        }

        public Dictionary<int, VillageImgPack> VillageImagePacks
        {
            get
            {
                return _villageImagePacks;
            }
        }

        #region properties
        public BattleHandicap BattleHandicap { get; private set; }
        public string ExtendedDesc { get; private set; }

        public bool AreCapitalVillagesActive
        {
            get
            {
                return _areCapitalVillagesActive;
            }
        }
        private int _capitalVillageProtectionDurationInDays;
        /// <summary>
        /// 60 by default, other number is explicitly set in realm parameters. 0 if !AreCapitalVillagesActive
        /// </summary>
        public int CapitalVillageProtectionDurationInDays
        {
            get
            {
                return AreCapitalVillagesActive ? _capitalVillageProtectionDurationInDays :  0;
            }
            private set
            {
                _capitalVillageProtectionDurationInDays = value;
            }
        }


        public int ClanLimit
        {
            get { return _clanLimit; }
        }

        public bool IsTemporaryTournamentRealm
        {
            get { return _isTemporaryTournamentRealm; }
        }
        public bool NeedNPInSomeOtherRealmToEnterThisOne
        {
            get { return _needNPInSomeOtherRealmToEnterThisOne; }
        }
        public int TemporaryTournamentRealm_PrizeType { get; private set; }
        public int TemporaryTournamentRealm_GamePlayType { get; private set; }
        public int MaxNumberOfGiftsPerDayThatCanBeUsed { get; private set; }
        public int MaxNumberOfGiftsThatCanBeUsed_CarryOverDays { get; private set; }
      //  public int MaxTotalNumberOfGiftsThatCanBeUsedIncludingCarrriedOver { get { return MaxNumberOfGiftsPerDayThatCanBeUsed * MaxNumberOfGiftsThatCanBeUsed_CarryOverDays; } }
        //public int Consolidation_NumVillagesAbsorbed { get; private set; }
        /// <summary>
        /// cost of entering this realm, in credits
        /// </summary>
        public int EntryCost
        {
            get;
            set;
        }
        public bool NewDailyReward
        {
            get;
            set;
        }
        public AttackFreeze AttackFreezeGet
        {
            get
            {
                return _attackFreeze;
            }
        }
        public SleepMode SleepModeGet
        {
            get
            {
                return _sleepMode;
            }
        }

        public VacationMode VacationModeGet
        {
            get
            {
                return _vacationMode;
            }
        }

        public WeekendMode WeekendModeGet
        {
            get
            {
                return _weekendMode;
            }
        }

        public Consolidation ConsolidationGet
        {
            get
            {
                return _consolidation;
            }
        }

        public string ConnectionStr
        {
            get
            {
                return _connectionStr;
            }
        }
        public string Tag
        {
            get
            {
                return _name;
            }
        }
        public string Name
        {
            get
            {
                return "Realm " + Tag ;
            }
        }
        public string Name2
        {
            get
            {
                return Tag;
            }
        }
        public string Desc
        {
            get
            {
                return _desc;
            }
        }
        public int ID
        {
            get
            {
                return _id;
            }
        }
        public int Population
        {
            get
            {
                return _population;
            }
        }
        public int MaxPlayers
        {
            get
            {
                return _maxplayers;
            }
        }
        public DateTime OpenOn
        {
            get
            {
                return _openOn;
            }
        }
        public DateTime ClosingOn
        {
            get
            {
                return _closingOn;
            }
        }
        public bool BonusVillChange
        {
            get;
            set;
        }
             
        public int LocalDBVersion
        {
            get;
            private set;
        }

        public DateTime CreditFarmBonusDateEnds
        {
            get;
            set;
        }

        public int CreditFarmBonusMultiplier
        {
            get;
            set;
        }

        public string CreditFarmBonusDesc
        {
            get;
            set;
        }

        public string CreditFarmBonusIcon
        {
            get;
            set;
        }
        

        #region Realm Type - a bit of a hack
        public string RealmType
        {
            get;
            private set;
        }
        public bool RealmType_isNoob
        {
            get
            {
                return String.Compare(RealmType, "NOOB", true) == 0;
            }
        }

        public enum RealmSubTypes { Subscription, Unknown, NoClans, Retro}
        string _realmSubType;
        public RealmSubTypes RealmSubType
        {
            get
            {
                switch(_realmSubType)
                {
                    case "Subscription":
                        return RealmSubTypes.Subscription;
                    case "NOClans":
                        return RealmSubTypes.NoClans;
                    case "Retro":
                        return RealmSubTypes.Retro;
                    default:
                        return RealmSubTypes.Unknown;
                }
            }
           
        }
        #endregion 

        public enum RealmState
        {
            /// <summary>
            /// realm is not yet opened and prereg not allowed
            /// </summary>
            NoPreRegistration,
            PreRegistration,
            Running_OpenToAll,
            Running_ClosedToNewPlayer
        }
        /// <summary>
        /// returns NULL if realm is not set to close to new players at the moment, or date when it is to be closed. remember that the date may be in the past already
        /// </summary>
        public DateTime? ClosesToNewPlayersOn
        {
            get {
                if (_closesToNewPlayersInHours != 0)
                {
                    return _openOn.AddHours(_closesToNewPlayersInHours);
                }
                else
                {
                    return null;
                }
            }
        }


        
        /// <summary>
        /// Tells you if the realm is now running - events are completing etc. 
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return (IsOpen == RealmState.Running_ClosedToNewPlayer || IsOpen == RealmState.Running_OpenToAll) && ClosingOn > DateTime.Now;
            }
        }
        /// <summary>
        /// signals if this realm is already opened. and running. Otherwise only preregistration is available
        /// </summary>
        public RealmState IsOpen
        {  
            get
            {
                if (ClosesToNewPlayersOn != null && ClosesToNewPlayersOn < DateTime.Now)
                {
                    return RealmState.Running_ClosedToNewPlayer;
                }
                else
                {
                    if (_openOn <= DateTime.Now)
                    {
                        return RealmState.Running_OpenToAll;
                    }
                    else
                    {
                        if (_allowPreReg)
                        {
                            return RealmState.PreRegistration;
                        }
                        else
                        {
                            return RealmState.NoPreRegistration;
                        }
                    }
                }
            }
        }

        public bool AreGiftsActive
        {
            get
            {
                return _gifts_areActive;
            }
        }
        /// <summary>
        /// is this VersionP realm?
        /// </summary>
        public bool IsVPrealm
        {
            get
            {
                return _isVPRealm;
            }
        }
        public StewardshipTypes StewardshipType
        {
            get
            {
                return _stewardshipType;
            }
        }

        /// <summary>
        /// if 0, that means this realm has not desertion
        /// </summary>
        public int UnitDesertionScalingFactor
        {
             get;
            private set;
        }
        public int UnitDesertionMaxPopulation
        {
             get;
            private set;
        }
        public int UnitDesertionMinDistance
        {
             get;
            private set;
        }

        DateTime? _sinceThenDoNotAllowClanChanges;
        int _sinceThisAgeDoNotAllowClanChanges;
        /// <summary>
        /// if true, do not allow leaving or joining a clan, because we are close to the end of the realm, and no clan changes are allowed
        /// </summary>
        public bool AllowClanChanges
        {
            get
            {
                if (_sinceThenDoNotAllowClanChanges != null)
                {
                    return DateTime.Now < _sinceThenDoNotAllowClanChanges;
                }
                else if (_sinceThisAgeDoNotAllowClanChanges > 0 && this.Age.CurrentAge.AgeNumber >= _sinceThisAgeDoNotAllowClanChanges)
                {
                    return false;
                }
                return true;
            }
        }



        //TreasuryBuildingType _trasuryBuildingType;
        public BuildingType BuildingType_Treasury
        {
            get
            {
                return BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Treasury);
            }
        }
        public BuildingType BuildingType_CoinMine
        {
            get
            {
                return BuildingType(Fbg.Bll.CONSTS.BuildingIDs.CoinMine);
            }
        }
        public BuildingType BuildingType_HQ
        {
            get
            {
                return BuildingType(Fbg.Bll.CONSTS.BuildingIDs.VillageHQ);
            }
        }
        public BuildingType BuildingType_Stable
        {
            get
            {
                return BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Stable);
            }
        }
        public BuildingType BuildingType_TradePost
        {
            get
            {
                return BuildingType(Fbg.Bll.CONSTS.BuildingIDs.TradePost);
            }
        }
        public BuildingType BuildingType_Wall
        {
            get
            {
                return BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Wall);
            }
        }
        public BuildingType BuildingType_DefenseTower
        {
            get
            {
                return BuildingType(Fbg.Bll.CONSTS.BuildingIDs.DefenseTower);
            }
        }
        public BuildingType BuildingType_Barracks
        {
            get
            {
                return BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Barracks);
            }
        }
        public BuildingType BuildingType_HidingSpot
        {
            get
            {
                return BuildingType(Fbg.Bll.CONSTS.BuildingIDs.HidingSpot);
            }
        }
        public BuildingType BuildingType_Tavern
        {
            get
            {
                return BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Tavern);
            }
        }
        public BuildingType BuildingType_Workshop
        {
            get
            {
                return BuildingType(Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop);
            }
        }
        public BuildingType BuildingType_Farm
        {
            get
            {
                return BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Farmland);
            }
        }
        public BuildingType BuildingType_Palace
        {
            get
            {
                return BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Palace);
            }
        }

        public List<BuildingType> BuildingTypes
        {
            get
            {
                // MAKE SURE THIS property is called first from LoadData() to ensure it get populated in thread safe environment

                if (_buildingTypes == null)
                {
                    _buildingTypes = new List<BuildingType>(dtBuildingTypes.Rows.Count);
                    foreach (DataRow dr in dtBuildingTypes.Rows)
                    {
                        if ((int)dr[Realm.CONSTS.BuildingTypesColumnIndex.BuildingTypeID] == Fbg.Bll.CONSTS.BuildingIDs.VillageHQ)
                        {
                            _buildingTypes.Add(new BuildingTypeHQ(this, ds, dr));
                        }
                        else if ((int)dr[Realm.CONSTS.BuildingTypesColumnIndex.BuildingTypeID] == Fbg.Bll.CONSTS.BuildingIDs.TradePost)
                        {
                            _buildingTypes.Add(new BuildingTypeTP(this, ds, dr));
                        }
                        else
                        {
                            _buildingTypes.Add(new BuildingType(this, ds, dr));
                        }
                    }
                }
                return _buildingTypes;
            }
        }


        Dictionary<string, Fbg.Common.Entities.BuildingType> _buildingTypesEntities;
        public Dictionary<string, Fbg.Common.Entities.BuildingType> BuildingTypesEntities
        {
            get
            {
                if (_buildingTypesEntities == null)
                {
                    _buildingTypesEntities = BuildingTypes.ToDictionary(bt => bt.ID.ToString()
                    , bt => new Fbg.Common.Entities.BuildingType()
                    {
                       ID = bt.ID,
                       MaxLevel = bt.MaxLevel,
                       MinimumLevelAllowed = bt.MinimumLevelAllowed,
                       Name = bt.Name
                    }
                    );
                }
                return _buildingTypesEntities;
            }
        }


        /// <summary>
        /// The speed of coin/silver transport
        /// </summary>
        public int CoinTransportSpeed 
        {
           get
           {
               return (int)dtRealm.Rows[0][CONSTS.RealmColIndex.CoinTransportSpeed];
           }
        }

        public float BeginnerRegistrationDays
        {
            get
            {
                return (float)dtRealm.Rows[0][CONSTS.RealmColIndex.BeginnerProtectionDays];
            }
        }

        /// <summary>
        /// Number of minutes after the attack was sent that it can be cancelled
        /// </summary>
        public int CancelAttackTimeLimit
        {
           get
           {
               return (int)dtRealm.Rows[0][CONSTS.RealmColIndex.CancelAttackInMin];
           }
        }

        //public int BattleHandicap
        //{
        //    get
        //    {
        //        return _battleHandicap;
        //    }
        //}

        public int SpyAlgorithmVersion
        {
            get
            {
                return _spyAlgorithmVersion;
            }
        }
        public int VillagePlacementAlgorithmVersion
        {
            get
            {
                return _villagePlacementAlgorithmVersion;
            }
        }
        public float OverAllSpeedFactor
        {
            get
            {
                return _overAllSpeedFactor;
            }
        }
        /// <summary>
        /// - Int16.MinValue means there is no entry limitations on this realm. 
        /// - 0 means this realm if opened for new players only (who are notplaying on any realm) 
        /// - >0 means player must have achieved this title before entering this realm
        /// </summary>
        public Int16 RealmTitleEntryLimitations
        {
            get
            {
                return _realmTitleEntryLimitations;
            }
        }

        /// <summary>
        /// - Int32.MaxValue means there is no entry limitations on this realm. 
        /// </summary>
        public Int32 RealmEntryLimitations_MaxXP { get; private set; }        

        /// <summary>
        /// given a player, it tells you if this player can enter the realm SOLY based on RealmEntryLimitations. No other 
        /// conditions are checked!!!
        /// 
        /// This does NOT return a proper value if player is already playing on this realm. 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool RealmEntryLimitations_AllowEntryForNewPlayer(User user)
        {
            bool allowEntry = true;
            if (RealmTitleEntryLimitations == 0 && user.Players.Count != 0)
            {
                allowEntry = false;
            }
            else if (RealmTitleEntryLimitations > 0 && user.MyHigestAchievedTitle < RealmTitleEntryLimitations)
            {
                allowEntry = false;
            }
            return allowEntry;
        }
        

        #endregion

        /// <summary>
        /// Tells you if this realm has this particular paid feature trial. 
        /// </summary>
        public bool PF_HasPFTrial(Fbg.Bll.CONSTS.PFTrials pfTrialID)
        {
            return PF_PFTrial(pfTrialID) != null;
        }

        /// <summary>
        /// returns null if such a trial does not exits in this realm. 
        /// </summary>
        public PFTrial PF_PFTrial(Fbg.Bll.CONSTS.PFTrials pfTrialID)
        {
            DataRow dr = dtPFTrials.Rows.Find(pfTrialID);
            if (dr == null)
            {
                return null;
            }
            else
            {
                return new PFTrial(dr);
            }
        }

        Dictionary<short, List<MapVillageIcon>> _mapVillageIcons;
        public Dictionary<short, List<MapVillageIcon>> GetMapVillageIconUrls
        {
            get
            {
                if (_mapVillageIcons == null)
                {
                    _mapVillageIcons = new Dictionary<short, List<MapVillageIcon>>();
                    List<MapVillageIcon> iconsPerVillageType;
                    foreach (DataRow dr in dtMap.Rows)
                    {
                        _mapVillageIcons.TryGetValue((short)dr[CONSTS.MapColumnIndex.VillageTypeID], out iconsPerVillageType);
                        if (iconsPerVillageType == null)
                        {
                            iconsPerVillageType = new List<MapVillageIcon>();
                            _mapVillageIcons.Add((short)dr[CONSTS.MapColumnIndex.VillageTypeID], iconsPerVillageType);
                        }
                        iconsPerVillageType.Add(new MapVillageIcon()
                        {
                            IconUrl = (string)dr[CONSTS.MapColumnIndex.IconUrl]
                            ,
                            MaxVillagePoints = (int)dr[CONSTS.MapColumnIndex.MaxVillagePoints]
                        });
                    }
                }
                return  _mapVillageIcons;
            }
        }

        public string GetMapVillageIconUrl(int points, short villageTypeID)
        {           
           

            List<MapVillageIcon> icons;
            GetMapVillageIconUrls.TryGetValue(villageTypeID, out icons);
            if (icons == null)
            {
                icons = GetMapVillageIconUrls[0];
            }
            //This can probably be improved performance wise considering how many times this will be called ... 
            // This code relies on the fact that the map rows are sorted on MaxVillagePoints asc
            foreach (MapVillageIcon icon in icons)
            {
                if (points <= icon.MaxVillagePoints) {
                    return icon.IconUrl;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the units in this Realm
        /// </summary>
        public List<UnitType> GetUnitTypes()
        {
            DataRow[] drsDefenses;
            DataRow[] drsAttackableBuildings;
            UnitType ut;
            if (_unitTypes == null)
            {
                _unitTypes = new List<UnitType>(dtUnitTypes.Rows.Count);
                foreach (DataRow dr in dtUnitTypes.Rows)
                {
                    drsDefenses             = dr.GetChildRows(ds.Relations[CONSTS.RelIndex.UnitTypesToDefense]);
                    drsAttackableBuildings  = dr.GetChildRows(ds.Relations[CONSTS.RelIndex.UnitTypesToBuildingAttack]);

                    ut = new UnitType(this, ds, dr, drsDefenses, drsAttackableBuildings);
                    if (ut.ID == UnitTypeLord.LordUnitID)
                    {
                        ut = new UnitTypeLord(this, ds, dr, drsDefenses, drsAttackableBuildings);
                    }
                    _unitTypes.Add(ut);
                }
            }
            return _unitTypes;
        }


        Dictionary<string, Fbg.Common.Entities.UnitType> _unitTypesEntities;
        /// <summary>
        /// Get the units in this Realm
        /// </summary>
        public Dictionary<string, Fbg.Common.Entities.UnitType> GetUnitTypesEntities()
        {
            if (_unitTypesEntities == null)
            {
                _unitTypesEntities = GetUnitTypes().ToDictionary(unit => unit.ID.ToString()
                , unit => new Fbg.Common.Entities.UnitType()
                {
                    AttackStrength = unit.AttackStrength
                    ,
                    AvgDefenseStrength = unit.DefenseStrength_Avg
                    ,
                    CarryCapacity = unit.CarryCapacity
                    ,
                    CounterSpyAbility = unit.CarryCapacity
                    ,
                    IconUrl = unit.IconUrl
                    ,
                    LargeIconUrl = unit.LargeIconUrl
                    ,
                    IconUrl_ThemeM = unit.IconUrl_ThemeM
                    ,
                    ID = unit.ID
                    ,
                    Name = unit.Name
                    ,
                    Image = unit.Image
                    ,
                    Pop = unit.Pop
                    ,
                    Speed = unit.Speed
                    ,
                    SpyAbility = unit.SpyAbility
                    ,
                    AttackableBuildingIDs = ( from a in unit.AttackableBuildings select a.ID ).ToList<int>()
                    , description = unit.Desc
                    , DefenceStrengths = unit.DefenseStrength.ToDictionary(r => r.Key.ID.ToString(), r => r.Value)
                    ,
                    baseRecruitSpeed = unit.RecruitmentTime()
                    , costInCoins = unit.Cost(null)
                }
                );
            }
            return _unitTypesEntities;
        }
        public DataTable  GetBuildingTypes()
        {
            return dtBuildingTypes;
        }
        public DataTable GetLevelProperties()
        {
            return dtLevelProperties;
        }

        /// <summary>
        /// Get list of units per building they are built in. So, keys are the buildings, and vales are the lists of 
        /// units types that are recruited at this building
        /// </summary>
        /// <returns></returns>
        public Dictionary<BuildingType, List<UnitType>> GetUnitTypesByBuildingType()
        {
            List<UnitType> unitTypeList;
            if (_unitTypesByBuildingType == null)
            {
                _unitTypesByBuildingType = new Dictionary<BuildingType, List<UnitType>>(3);
                foreach (UnitType ut in GetUnitTypes())
                {
                    if (_unitTypesByBuildingType.TryGetValue(ut.RecruitmentBuilding, out unitTypeList))
                    {
                        unitTypeList.Add(ut);
                    }
                    else
                    {
                        unitTypeList = new List<UnitType>(2);
                        unitTypeList.Add(ut);
                        _unitTypesByBuildingType.Add(ut.RecruitmentBuilding, unitTypeList);
                    }
                }
            }
            return _unitTypesByBuildingType;
        }

        /// <summary>
        /// will return null if no such unit type is found
        /// </summary>
        /// <param name="unitTypeID"></param>
        /// <returns></returns>
        public UnitType GetUnitTypesByID(int unitTypeID)
        {
            UnitType unitTypeToReturn = null;

            if (_unitTypeQuickLookup == null)
            {
                //this.BuildQuickLookups();
                throw new InvalidOperationException("_unitTypeQuickLookup == null");
            }

            if (unitTypeID >= 0 && unitTypeID < _unitTypeQuickLookup.Length) {
                unitTypeToReturn = _unitTypeQuickLookup[unitTypeID];
            }

            return unitTypeToReturn;
        }

        /// <summary>
        /// returns the location of this type by sort order starting with first unit type having location 0
        /// ie, UnitType which is first by sort order, gets 0, second one gets 1 etc
        /// </summary>
        /// <param name="unitTypeID"></param>
        /// <returns></returns>
        public int GetNormalizedUnitTypeLocation(int unitTypeID)
        {
            int loc = 0; 

            if (_normalizedUnitTypeQuickLookup == null)
            {
//                this.BuildQuickLookups();
                throw new InvalidOperationException("_unitTypeQuickLookup == null");
            }

            if (unitTypeID >= 0 && unitTypeID < _normalizedUnitTypeQuickLookup.Length)
            {
                loc = _normalizedUnitTypeQuickLookup[unitTypeID];
            }

            return loc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="name"></param>
        /// <param name="facebookID"></param>
        /// <param name="sex"></param>
        /// <param name="intInvitaionID">pass in null if this registration is not an acceptance of an invite. Pass InviteID (int!) if it is. </param>
        /// <param name="isInvitedToClan">only valid if intInvitaionID is not null. </param>
        /// <returns></returns>
        public int RegisterPlayer(Guid userID, string name, string facebookID, Fbg.Common.Sex sex, object intInvitaionID, out bool isInvitedToClan, out int invitingPlayerID
            , Fbg.Common.StartInQuadrants startInQuadrant, string villageName, int avatarId, Fbg.Common.UserLoginType playerLoginType)
        {
            if (this._villagePlacementAlgorithmVersion != 3)
            {
                startInQuadrant = Fbg.Common.StartInQuadrants.NoneSelected;
            }
            return DAL.Realm.RegisterPlayer(userID, name, facebookID, ID, _connectionStr, (short)sex
                , intInvitaionID, out isInvitedToClan, out invitingPlayerID, startInQuadrant, villageName, avatarId, playerLoginType);
        }
        public bool AddPlayerQ(Guid userID,int Realm) 
        {
            return DAL.Realm.AddPlayerQ(userID, Realm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buildingTypeID"></param>
        /// <returns></returns>
        public BuildingType BuildingType(int buildingTypeID)
        {
            //TODO: cash the building types for faster access
            foreach (BuildingType b in BuildingTypes)
            {
                if (b.ID == buildingTypeID)
                {
                    return b;
                }
            }
            return null;
        }

        private void BuildQuickLookups()
        {
            if (_unitTypeQuickLookup == null || _normalizedUnitTypeQuickLookup == null)
            {
                _unitTypeQuickLookup = new UnitType[_maxUnitTypeID+ 1];
                _normalizedUnitTypeQuickLookup = new int[_maxUnitTypeID+ 1];

                int counter = 0;
                foreach(UnitType ut in this.GetUnitTypes())
                {
                    _unitTypeQuickLookup[ut.ID] = ut;
                    _normalizedUnitTypeQuickLookup[ut.ID] = counter;
                    counter++;
                }
            }
        }

        public VillageTypes VillageTypes { get; internal set; }

        /// <summary>
        /// NOT THREAD SAFE. 
        /// assume this is called by code that ensures thread safety
        /// </summary>
        private  void LoadData()
        {
            try
            {
                CultureInfo ciUS = new CultureInfo("en-US");
                if (ds != null)
                {
                    ds = null;
                }


                ds = DAL.Realm.GetRealmInfo(_connectionStr);

                dtBuildingTypes         = ds.Tables[CONSTS.TableIndex.BuildingTypes];
                dtBuildingTypesLevels   = ds.Tables[CONSTS.TableIndex.BuildingTypesLevels];
                dtBuildingTypesLevelRequirements = ds.Tables[CONSTS.TableIndex.dtBuildingTypesLevelRequirements];
                dtUnitTypes             = ds.Tables[CONSTS.TableIndex.UnitTypes];
                dtUnitTypesRecruitReq   = ds.Tables[CONSTS.TableIndex.UnitTypeRecruitReq];
                _maxUnitTypeID          =(int)ds.Tables[CONSTS.TableIndex.MaxUnitTypeID].Rows[0][CONSTS.MaxUnitTypeIDColumnIndex.MaxUnitTypeID];
                dtMap                   = ds.Tables[CONSTS.TableIndex.Map];
                dtUnitTypeDefense       = ds.Tables[CONSTS.TableIndex.UnitTypeDefense];
                dtUnitOnBuildingAttack  = ds.Tables[CONSTS.TableIndex.UnitOnBuildingAttack];
                dtRealm                 = ds.Tables[CONSTS.TableIndex.Realm];
                dtLevelProperties       = ds.Tables[CONSTS.TableIndex.LevelProperties];
                dtPFTrials              = ds.Tables[CONSTS.TableIndex.PFTrails];
                dtPFPackages            = ds.Tables[CONSTS.TableIndex.PFPackages];
                dtPFs                   = ds.Tables[CONSTS.TableIndex.PFs]; 
                dtTitles                = ds.Tables[CONSTS.TableIndex.Titles];
                dtRealmAttributes       = ds.Tables[CONSTS.TableIndex.RealmAttributes];
                dtResearchItemTypes     = ds.Tables[CONSTS.TableIndex.ResearchItemTypes];
                dtResearchItems         = ds.Tables[CONSTS.TableIndex.ResearchItems];
                dtResearchItemRequirements = ds.Tables[CONSTS.TableIndex.ResearchItemRequirements];
                dtUnitTypesRecruitResReq = ds.Tables[CONSTS.TableIndex.UnitTypeRecruitResearchReq];
                dtRealmAges             = ds.Tables[CONSTS.TableIndex.RealmAges];

                this.Morale = new PlayerMorale(ds.Tables[CONSTS.TableIndex.MoraleRanges], ds.Tables[CONSTS.TableIndex.Morale], dtRealmAttributes);

                LoadVillageImagePacks();

                //_avatars = new List<Avatar>();
                //foreach (DataRow dr in ds.Tables[CONSTS.TableIndex.Avatars].Rows)
                //{
                //    _avatars.Add(new Avatar(dr));
                //}

                //
                // init realm attributes
                this.BattleHandicap = new BattleHandicap();
               
                DataRow[] drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.BattleHandicap ));
                if (drs.Length > 0 ) 
                {
                    if (Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]) == 1)
                    {
                        BattleHandicap.IsActive = true;
                         drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.BattleHandicapParam_MaxHandicap));
                        if (drs.Length > 0)
                        {
                            BattleHandicap.Param_MaxHandicap = Convert.ToDouble(drs[0][CONSTS.RealmAttribsColIndex.AttribValue], ciUS);
                        }
                        drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.BattleHandicapParam_StartRatio));
                        if (drs.Length > 0)
                        {
                            BattleHandicap.Param_StartRatio = Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                        }
                         drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.BattleHandicapParam_Steepness));
                        if (drs.Length > 0)
                        {
                            BattleHandicap.Param_Steepness = Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                        }

                    }
                }
                _spyAlgorithmVersion = 1;
                drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.SpyAlgorithmVersion));
                if (drs.Length > 0)
                {
                    _spyAlgorithmVersion = Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                }
                _closesToNewPlayersInHours = 0;
                drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.RealmClosedToNewPlayerIndicator));
                if (drs.Length > 0)
                {
                    _closesToNewPlayersInHours = Convert.ToSingle(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                }
                _villagePlacementAlgorithmVersion = 1;
                drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.VillagePlacementAlgorithmVersion));
                if (drs.Length > 0)
                {
                    _villagePlacementAlgorithmVersion = Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                }
                _overAllSpeedFactor = 1; 
                drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.OverAllSpeedFactor));
                if (drs.Length > 0)
                {
                    _overAllSpeedFactor = Convert.ToSingle(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                }
                _realmTitleEntryLimitations = Int16.MinValue;
                drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.RealmEntryLimitations));
                if (drs.Length > 0)
                {
                    _realmTitleEntryLimitations = Convert.ToInt16(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
                }

                _attackFreeze = new AttackFreeze(dtRealmAttributes);
                _sleepMode = new SleepMode(dtRealmAttributes);
                _vacationMode = new VacationMode(dtRealmAttributes);
                _weekendMode = new WeekendMode(dtRealmAttributes);
                _consolidation = new Consolidation(dtRealmAttributes);

                _isTemporaryTournamentRealm = false;
                drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.TemporaryTournamentRealm));
                if (drs.Length > 0)
                {
                    _isTemporaryTournamentRealm = Convert.ToBoolean(Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]));
                }
                _needNPInSomeOtherRealmToEnterThisOne = false;
                drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.NeedNInSomeOtherRealmToEnterThisOne));
                if (drs.Length > 0)
                {
                    _needNPInSomeOtherRealmToEnterThisOne = Convert.ToBoolean(Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]));
                }
                _gifts_areActive = true;
                drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.Gifts_areActive));
                if (drs.Length > 0)
                {
                    _gifts_areActive = Convert.ToBoolean(Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]));
                }
                _isVPRealm = false;
                drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.IsVPRealm));
                if (drs.Length > 0)
                {
                    _isVPRealm = Convert.ToInt32(Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue])) == 1;
                }
                _clanLimit = 0;
                drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.ClanLimit));
                if (drs.Length > 0)
                {
                    _clanLimit = Convert.ToInt32(Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]));
                }
                _stewardshipType = StewardshipTypes.classic;
                drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.StewardshipType));
                if (drs.Length > 0)
                {
                    _stewardshipType = (StewardshipTypes)(Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]));
                }
                _areCapitalVillagesActive = false;
                drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.AreCapitalVillagesActive));
                if (drs.Length > 0)
                {
                    _areCapitalVillagesActive = Convert.ToBoolean(Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]));
                }

                AreQuestsDisabled= false;
                drs = dtRealmAttributes.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)CONSTS.RealmAttributeIDs.AreQuestsDisabled));
                if (drs.Length > 0)
                {
                    AreQuestsDisabled = Convert.ToBoolean(Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]));
                }
                EntryCost = RealmAttributesGetHelper(0, dtRealmAttributes, CONSTS.RealmAttributeIDs.EntryCost);
                NewDailyReward = RealmAttributesGetHelper(0, dtRealmAttributes, CONSTS.RealmAttributeIDs.NewDailyReward) == 1 && (IsOpen == Realm.RealmState.Running_OpenToAll || IsOpen == Realm.RealmState.Running_ClosedToNewPlayer);
                TemporaryTournamentRealm_GamePlayType = RealmAttributesGetHelper(0, dtRealmAttributes, CONSTS.RealmAttributeIDs.TournamentRealm_GamePlayType);                
                TemporaryTournamentRealm_PrizeType = RealmAttributesGetHelper(0, dtRealmAttributes, CONSTS.RealmAttributeIDs.TournamentRealm_PrizeType);
                MaxNumberOfGiftsPerDayThatCanBeUsed = RealmAttributesGetHelper(40, dtRealmAttributes, CONSTS.RealmAttributeIDs.MaxGiftsPerDayUse);
                MaxNumberOfGiftsThatCanBeUsed_CarryOverDays = RealmAttributesGetHelper(0, dtRealmAttributes, CONSTS.RealmAttributeIDs.MaxGiftsDaysCarryOver);
                GovernmentTypesEnabled = RealmAttributesGetHelper(0, dtRealmAttributes, CONSTS.RealmAttributeIDs.GovTypesEnabled);
                AccessDeviceTypeLimitation = (AccessDeviceTypeLimitations)RealmAttributesGetHelper(0, dtRealmAttributes, CONSTS.RealmAttributeIDs.AccessDeviceTypeLimitation);
                RealmEntryLimitations_MaxXP = RealmAttributesGetHelper(0, dtRealmAttributes, CONSTS.RealmAttributeIDs.RealmEntryLimitationMaxXP);
                BonusVillChange = RealmAttributesGetHelper(0, dtRealmAttributes, CONSTS.RealmAttributeIDs.BonusVillChange) == 1;
                LocalDBVersion = RealmAttributesGetHelper(1, dtRealmAttributes, CONSTS.RealmAttributeIDs.LocalDBVersion);
                _capitalVillageProtectionDurationInDays = RealmAttributesGetHelper(60, dtRealmAttributes, CONSTS.RealmAttributeIDs.CapitalVillageProtectionDurationInDays);
                RealmType = RealmAttributesGetHelper("", dtRealmAttributes, CONSTS.RealmAttributeIDs.RealmType);
                _realmSubType = RealmAttributesGetHelper("", dtRealmAttributes, CONSTS.RealmAttributeIDs.RealmSubType);
                CreditFarmBonusDateEnds = RealmAttributesGetHelper(DateTime.Now, dtRealmAttributes, CONSTS.RealmAttributeIDs.CreditFarm_bonusDate);
                CreditFarmBonusMultiplier = RealmAttributesGetHelper(1, dtRealmAttributes, CONSTS.RealmAttributeIDs.CreditFarm_bonusMultiplier);
                CreditFarmBonusDesc = RealmAttributesGetHelper("", dtRealmAttributes, CONSTS.RealmAttributeIDs.CreditFarm_bonusEventDesc);
                CreditFarmBonusIcon = RealmAttributesGetHelper("", dtRealmAttributes, CONSTS.RealmAttributeIDs.CreditFarm_bonusEventIcon);
                BattleAlgorithmVersion = RealmAttributesGetHelper(1, dtRealmAttributes, CONSTS.RealmAttributeIDs.BattleAlgorithmVersion);
                UnitDesertionScalingFactor = RealmAttributesGetHelper(9, dtRealmAttributes, CONSTS.RealmAttributeIDs.UnitDisertionScalingFactor);
                UnitDesertionMaxPopulation = RealmAttributesGetHelper(300, dtRealmAttributes, CONSTS.RealmAttributeIDs.UnitDisertionMaxPopulation);
                UnitDesertionMinDistance = RealmAttributesGetHelper(13, dtRealmAttributes, CONSTS.RealmAttributeIDs.UnitDisertionMinDistance);
                Theme_set(RealmAttributesGetHelper(0, dtRealmAttributes, CONSTS.RealmAttributeIDs.RealmTheme));
                isLimitAccessToVIPsOnly = RealmAttributesGetHelper(false, dtRealmAttributes, CONSTS.RealmAttributeIDs.LimitAccessToVIPsOnly);
                maxUpgradeSpeedupInMinutes = RealmAttributesGetHelper(Int32.MaxValue, dtRealmAttributes, CONSTS.RealmAttributeIDs.MaxUpgradeSpeedupsInMinutes);

                travellingCaravanLimitDaily = RealmAttributesGetHelper(30, dtRealmAttributes, CONSTS.RealmAttributeIDs.MapEvents_CaravanSpawnCap);
                

                if (RealmAttributesGetHelper(0, dtRealmAttributes, CONSTS.RealmAttributeIDs.HoursBeforeCloseRealmToDisableClanChanges) != 0 && this.ClosingOn != DateTime.MaxValue)
                {
                    _sinceThenDoNotAllowClanChanges = ClosingOn.Subtract(new TimeSpan(RealmAttributesGetHelper(0, dtRealmAttributes, CONSTS.RealmAttributeIDs.HoursBeforeCloseRealmToDisableClanChanges), 0, 0));
                }
                _sinceThisAgeDoNotAllowClanChanges = RealmAttributesGetHelper(0, dtRealmAttributes, CONSTS.RealmAttributeIDs.AgeFromWhichToDisableClanChanges);

                ds.Relations.Add("BuildingTypesToLevels"
                    , dtBuildingTypes.Columns[CONSTS.BuildingTypesColumnIndex.BuildingTypeID]
                    , dtBuildingTypesLevels.Columns[CONSTS.BuildingTypesLevelsColumnIndex.BuildingTypeID]);
                ds.Relations.Add("BuildingTypeLevelsToRequirements"
                    , new DataColumn[2]{dtBuildingTypesLevels.Columns[CONSTS.BuildingTypesLevelsColumnIndex.BuildingTypeID]
                    ,dtBuildingTypesLevels.Columns[CONSTS.BuildingTypesLevelsColumnIndex.Level]}
                , new DataColumn[2]{dtBuildingTypesLevelRequirements.Columns[CONSTS.dtBuildingTypesLevelRequirementsColumnIndex.BuildingTypeID]
                    ,dtBuildingTypesLevelRequirements.Columns[CONSTS.dtBuildingTypesLevelRequirementsColumnIndex.Level]});

                ds.Relations.Add("UnitTypesToReq"
                    , dtUnitTypes.Columns[CONSTS.UnitTypesColumnIndex.UnitTypeID]
                    , dtUnitTypesRecruitReq.Columns[CONSTS.UnitTypesRecruitReqColumnIndex.UnitTypeID]);

                ds.Relations.Add("UnitTypesToDefense"
                    , dtUnitTypes.Columns[CONSTS.UnitTypesColumnIndex.UnitTypeID]
                    , dtUnitTypeDefense.Columns[CONSTS.UnitTypeDefenseColIndex.UnitTypeID]);

                ds.Relations.Add("UnitTypesToBuildingAttack"
                    , dtUnitTypes.Columns[CONSTS.UnitTypesColumnIndex.UnitTypeID]
                    , dtUnitOnBuildingAttack.Columns[CONSTS.UnitOnBuildingAttackColIndex.UnitTypeID]);
                ds.Relations.Add("LevelProperties", dtBuildingTypes.Columns[CONSTS.BuildingTypesColumnIndex.BuildingTypeID]
                    , dtLevelProperties.Columns[CONSTS.LevelPropertiesColIndex.BuildingTypeID]);

                ds.Relations.Add("RITypesToRIs"
                  , dtResearchItemTypes.Columns[CONSTS.ResearchItemTypesColumnIndex.ResearchItemTypeID]
                  , dtResearchItems.Columns[CONSTS.ResearchItemsColumnIndex.ResearchItemTypeID]);

                ds.Relations.Add("RIToReq"
                  , new DataColumn[2]{dtResearchItems.Columns[CONSTS.ResearchItemsColumnIndex.ResearchItemTypeID], dtResearchItems.Columns[CONSTS.ResearchItemsColumnIndex.ResearchItemID]}
                  , new DataColumn[2]{dtResearchItemRequirements.Columns[CONSTS.ResearchItemReqColumnIndex.ResearchItemTypeID], dtResearchItemRequirements.Columns[CONSTS.ResearchItemReqColumnIndex.ResearchItemID]});

                ds.Relations.Add("RIToUnitReq"
                  , new DataColumn[2] { dtResearchItems.Columns[CONSTS.ResearchItemsColumnIndex.ResearchItemTypeID], dtResearchItems.Columns[CONSTS.ResearchItemsColumnIndex.ResearchItemID] }
                  , new DataColumn[2] { dtUnitTypesRecruitResReq.Columns[CONSTS.UnitTypeRecruitResearchReqColumnIndex.ResearchItemTypeID], dtUnitTypesRecruitResReq.Columns[CONSTS.UnitTypeRecruitResearchReqColumnIndex.ResearchItemID] });

                dtPFTrials.PrimaryKey = new DataColumn[] {dtPFTrials.Columns[CONSTS.PFTrailsColIndex.PFTrialID]};


                //
                // calls to these is necessary in a thread safe environment (which we assume this method is called in) 
                //  to make sure the object is properly initialized
                // Ideally, these properties and method below should be stripped of their populate code and everything should be
                //  initizlied in one place/method... 
                //
                object o;
                o = this.GetUnitTypes();
                o = this.GetUnitTypesByBuildingType();
                this.BuildQuickLookups();
                o = Titles;
                o = this.PFPackages_Object;
                o = this.PFs;

                Age_Init(dtRealmAges); //must be initialized before research

                Research.Init();

                List<BuildingType> buildingTypes = this.BuildingTypes;
                foreach (BuildingType bt in buildingTypes)
                {
                    bt.Initialize();
                }
                o = BuildingTypesEntities;

                VillageTypes = new VillageTypes(ds.Tables[CONSTS.TableIndex.VTS], ds.Tables[CONSTS.TableIndex.VT_PropTypes], ds.Tables[CONSTS.TableIndex.VT_Props], this);

                o = QuestTemplates;
                
                o = this.GetUnitTypesEntities();
                o = GetMapVillageIconUrls;
                o = AreGiftsActive ? this.Gifts : null;

                PopupateQuestTemplates(ds.Tables[CONSTS.TableIndex.QuestTemplates]
                    , ds.Tables[CONSTS.TableIndex.QuestProgression]
                    , ds.Tables[CONSTS.TableIndex.QuestTemplates_reward_troops]
                    , ds.Tables[CONSTS.TableIndex.QuestTemplates_reward_pfwithduratio]
                    , ds.Tables[CONSTS.TableIndex.QuestDesc]);

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error in LoadData()", e);
                // be careful here when writing out context info to avoid circular references/execution
                throw ex;
            }
        }

        private static int RealmAttributesGetHelper(int defaultValue, DataTable dt, CONSTS.RealmAttributeIDs attribID)
        {
            int val = defaultValue;
            DataRow[] drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)attribID));
            if (drs.Length > 0) {
                val = Convert.ToInt32(Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]));
            }
            return val;
        }
        private static bool RealmAttributesGetHelper(bool defaultValue, DataTable dt, CONSTS.RealmAttributeIDs attribID)
        {
            bool val = defaultValue;
            DataRow[] drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)attribID));
            if (drs.Length > 0)
            {
                val = Convert.ToBoolean(Convert.ToInt32(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]));
            }
            return val;
        }
        private static string RealmAttributesGetHelper(string defaultValue, DataTable dt, CONSTS.RealmAttributeIDs attribID)
        {
            string val = defaultValue;
            DataRow[] drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)attribID));
            if (drs.Length > 0)
            {
                val = drs[0][CONSTS.RealmAttribsColIndex.AttribValue].ToString();
            }
            return val;
        }
        private static DateTime RealmAttributesGetHelper(DateTime defaultValue, DataTable dt, CONSTS.RealmAttributeIDs attribID)
        {
            DateTime val = defaultValue;
            DataRow[] drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)attribID));
            if (drs.Length > 0)
            {
                val = Convert.ToDateTime(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
            }
            return val;
        }
        private static float RealmAttributesGetHelper(float defaultValue, DataTable dt, CONSTS.RealmAttributeIDs attribID)
        {
            float val = defaultValue;
            DataRow[] drs = dt.Select(String.Format("{0} = {1}", CONSTS.RealmAttribsColNames.AttribID, (int)attribID));
            if (drs.Length > 0)
            {
                val = Convert.ToSingle(drs[0][CONSTS.RealmAttribsColIndex.AttribValue]);
            }
            return val;
        }

        private void LoadVillageImagePacks()
        {
            //
            // this is hardcoded for testing. eventually, defintion of the image packs should be stored in the DB
            //
            _villageImagePacks = new Dictionary<int, VillageImgPack>(2);
            _villageImagePacks.Add(1, new VillageImgPack() { _images = new Dictionary<string, string>(0) { } });
            _villageImagePacks.Add(2, new VillageImgPack()
            {
                _images = new Dictionary<string, string>(10) { 
            { "https://static.realmofempires.com/images/vov/d_bg1.jpg", "https://static.realmofempires.com/images/vov/d_bg_age2.jpg" }
            , { "https://static.realmofempires.com/images/vov/d_bg2.jpg", "https://static.realmofempires.com/images/vov/d_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/d_bg3.jpg", "https://static.realmofempires.com/images/vov/d_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/d_bg4.jpg", "https://static.realmofempires.com/images/vov/d_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/d_bg5.jpg", "https://static.realmofempires.com/images/vov/d_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/d_bg6.jpg", "https://static.realmofempires.com/images/vov/d_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/d_bg7.jpg", "https://static.realmofempires.com/images/vov/d_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/d_bg8.jpg", "https://static.realmofempires.com/images/vov/d_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/d_bg9.jpg", "https://static.realmofempires.com/images/vov/d_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/d_bg10.jpg", "https://static.realmofempires.com/images/vov/d_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/n_bg1.jpg", "https://static.realmofempires.com/images/vov/n_bg_age2.jpg" }
            , { "https://static.realmofempires.com/images/vov/n_bg2.jpg", "https://static.realmofempires.com/images/vov/n_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/n_bg3.jpg", "https://static.realmofempires.com/images/vov/n_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/n_bg4.jpg", "https://static.realmofempires.com/images/vov/n_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/n_bg5.jpg", "https://static.realmofempires.com/images/vov/n_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/n_bg6.jpg", "https://static.realmofempires.com/images/vov/n_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/n_bg7.jpg", "https://static.realmofempires.com/images/vov/n_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/n_bg8.jpg", "https://static.realmofempires.com/images/vov/n_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/n_bg9.jpg", "https://static.realmofempires.com/images/vov/n_bg_age2.jpg" } 
            , { "https://static.realmofempires.com/images/vov/n_bg10.jpg", "https://static.realmofempires.com/images/vov/n_bg_age2.jpg" } 

            , { "https://static.realmofempires.com/images/vov/n_SilverMine1.png", "https://static.realmofempires.com/images/vov/n_SilverMine_age2.png" } 
            , { "https://static.realmofempires.com/images/vov/n_SilverMine2.png", "https://static.realmofempires.com/images/vov/n_SilverMine_age2.png" } 
            , { "https://static.realmofempires.com/images/vov/n_SilverMine3.png", "https://static.realmofempires.com/images/vov/n_SilverMine_age2.png" } 
            , { "https://static.realmofempires.com/images/vov/n_SilverMine4.png", "https://static.realmofempires.com/images/vov/n_SilverMine_age2.png" } 
            , { "https://static.realmofempires.com/images/vov/n_SilverMine5.png", "https://static.realmofempires.com/images/vov/n_SilverMine_age2.png" } 
            , { "https://static.realmofempires.com/images/vov/n_SilverMine6.png", "https://static.realmofempires.com/images/vov/n_SilverMine_age2.png" } 
             , { "https://static.realmofempires.com/images/vov/n_SilverMine7.png", "https://static.realmofempires.com/images/vov/n_SilverMine_age2.png" } 
             , { "https://static.realmofempires.com/images/vov/n_SilverMine8.png", "https://static.realmofempires.com/images/vov/n_SilverMine_age2.png" } 
             , { "https://static.realmofempires.com/images/vov/n_SilverMine9.png", "https://static.realmofempires.com/images/vov/n_SilverMine_age2.png" } 
             , { "https://static.realmofempires.com/images/vov/n_SilverMine10.png", "https://static.realmofempires.com/images/vov/n_SilverMine_age2.png" } 
            , { "https://static.realmofempires.com/images/vov/d_SilverMine1.png", "https://static.realmofempires.com/images/vov/d_SilverMine_age2.png" } 
            , { "https://static.realmofempires.com/images/vov/d_SilverMine2.png", "https://static.realmofempires.com/images/vov/d_SilverMine_age2.png" } 
            , { "https://static.realmofempires.com/images/vov/d_SilverMine3.png", "https://static.realmofempires.com/images/vov/d_SilverMine_age2.png" } 
            , { "https://static.realmofempires.com/images/vov/d_SilverMine4.png", "https://static.realmofempires.com/images/vov/d_SilverMine_age2.png" } 
            , { "https://static.realmofempires.com/images/vov/d_SilverMine5.png", "https://static.realmofempires.com/images/vov/d_SilverMine_age2.png" } 
            , { "https://static.realmofempires.com/images/vov/d_SilverMine6.png", "https://static.realmofempires.com/images/vov/d_SilverMine_age2.png" } 
             , { "https://static.realmofempires.com/images/vov/d_SilverMine7.png", "https://static.realmofempires.com/images/vov/d_SilverMine_age2.png" } 
             , { "https://static.realmofempires.com/images/vov/d_SilverMine8.png", "https://static.realmofempires.com/images/vov/d_SilverMine_age2.png" } 
             , { "https://static.realmofempires.com/images/vov/d_SilverMine9.png", "https://static.realmofempires.com/images/vov/d_SilverMine_age2.png" } 
             , { "https://static.realmofempires.com/images/vov/d_SilverMine10.png", "https://static.realmofempires.com/images/vov/d_SilverMine_age2.png" } 
            

            , { "https://static.realmofempires.com/images/vov/d_TreesR_4e_10.png", "https://static.realmofempires.com/images/vov/d_TreesR_4e_10_age2.png" } 
            , { "https://static.realmofempires.com/images/vov/d_TreesL_4e_10.png", "https://static.realmofempires.com/images/vov/d_TreesL_4e_10_age2.png" } 
            , { "https://static.realmofempires.com/images/vov/d_TreesL_3e_10.png", "https://static.realmofempires.com/images/vov/d_TreesL_3e_10_age2.png" } 
            , { "https://static.realmofempires.com/images/vov/n_TreesR_4e_10.png", "https://static.realmofempires.com/images/vov/n_TreesR_4e_10_age2.png" } 
            , { "https://static.realmofempires.com/images/vov/n_TreesL_4e_10.png", "https://static.realmofempires.com/images/vov/n_TreesL_4e_10_age2.png" } 
            , { "https://static.realmofempires.com/images/vov/n_TreesL_3e_10.png", "https://static.realmofempires.com/images/vov/n_TreesL_3e_10_age2.png" } 

            }
            });
        }

   
        /// <summary>
        ///  Returns a table who's columns are described by Realm.CONSTS.PFPackagesColIndex.
        ///  THIS IS DEPRECIATED. Use PFPackages_Object instead
        /// </summary>
        public DataTable PFPackages
        {
            get
            {
                return dtPFPackages;
            }
        }

        /// <summary>
        ///  Returns a table who's columns are described by Realm.CONSTS.PFPackagesColIndex
        ///  This is not thread safe. It shoudl be called at least once so that it initializes from a thread safe env.
        /// </summary>
        public List<PFPackage> PFPackages_Object
        {
            get
            {
                if (_PFPackages == null)
                {
                    _PFPackages = new List<PFPackage>(dtPFPackages.Rows.Count);
                    foreach (DataRow dr in dtPFPackages.Rows)
                    {
                        _PFPackages.Add(new PFPackage((int)dr[CONSTS.PFPackagesColIndex.PFPackageID]
                            , (int)dr[CONSTS.PFPackagesColIndex.Cost], Convert.ToSingle(dr[CONSTS.PFPackagesColIndex.Duration])));
                    }
                }
                return _PFPackages;
            }
        }
        /// <summary>
        ///  Returns pf package by id. returns null if not found
        /// </summary>
        public PFPackage PFPackage(int id)
        {
               return PFPackages_Object.Find(delegate(PFPackage p){return p.Id == id;} );
        }


        /// <summary>
        ///  Returns all the PFs on this realm 
        ///  This is not thread safe. It shoudl be called at least once so that it initializes from a thread safe env.
        /// </summary>
        public SortedList<int, PF> PFs
        {
            get
            {
                if (_PFs == null)
                {
                    _PFs = new SortedList<int, PF>(dtPFs.Rows.Count);
                    foreach (DataRow dr in dtPFs.Rows)
                    {

                        _PFs.Add((int)dr[0], new PF((int)dr[0], (string)dr[1]));
                    }
                }
                return _PFs;
            }
        }


        public bool hasPF(Fbg.Bll.CONSTS.PFs pf)
        {
            return hasPF((int)pf);
        }
        public bool hasPF(int pf)
        {
            Fbg.Bll.PF pfOUT;
            PFs.TryGetValue((int)pf, out pfOUT);
            return pfOUT != null;
        }


        /// <summary>
        /// will return null if package with this ID does not exist
        /// </summary>
        public CreditPackage PFCreditPackage(int creditPackageID)
        {
            return Realms.CreditPackage(creditPackageID);
        }

        /// <summary>
        /// may return null if ID Not found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Title TitleByID(int id)
        {
            ///TODO:improve performance by creating an array for titles allowing a quick lookup
            /// like for unit types
            foreach (Title title in Titles)
            {
                if (title.ID == id)
                {
                    return title;
                }
            }
            return null;
        }
        /// <summary>
        /// may return null if level Not found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Title TitleByLevel(long level)
        {
            ///TODO:improve performance by creating an array for titles allowing a quick lookup
            /// like for unit types
            foreach (Title title in Titles)
            {
                if (title.Level == level)
                {
                    return title;
                }
            }
            return null;
        }


        public List<Title> Titles
        {
            get
            {
                if (_titles == null)
                {
                    Title title;
                    _titles = new List<Title>(dtTitles.Rows.Count);
                    foreach (DataRow dr in dtTitles.Rows)
                    {
                        title = new Title(dr, this);
                        _titles.Add(title);
                    }
                }
                return _titles;
            }
        }

        public Poll GetPoll(int pollID)
        {
            foreach (Poll _poll in Realms.Polls)
            {
                if (_poll.ID == pollID)
                {
                    return _poll;
                }
            }
            return null;
            
        }

        private Poll _activePoll;
        private bool _activePoll_retrived = false;

        /// <summary>
        /// returns the active poll. 
        ///     active poll, is the first poll that is for this realm and its RunUntill properly is still in the future
        ///     if not found, an active poll is a game wide poll that has its RunUntill properly still in the future
        /// </summary>
        public Poll ActivePoll
        {
            get
            {
                if (_activePoll == null && !_activePoll_retrived) {
                    // if we dont have an active poll, and we did not look for one, then look for one
                    _activePoll_retrived = true;
                    _activePoll = ActivePoll_Retrieve();
                } else if (_activePoll != null && _activePoll.RunUntill < DateTime.Now) {
                    //if we had a poll, but it expired, the look for it again
                    _activePoll_retrived = true;
                    _activePoll = ActivePoll_Retrieve();
                }

                return _activePoll;
            }
        }

        private Poll ActivePoll_Retrieve()
        {
            // find a poll for this realm, that expires in the future. 
            //  if not found, look for a general poll (not realm specific) that expires in the future
            Poll poll = Realms.Polls.FindAll(p => p.RealmID == this.ID).Find(p2 => p2.RunUntill >= DateTime.Now);

            if (poll == null)
            {
                poll = Realms.Polls.FindAll(p => p.RealmID == null).Find(p2 => p2.RunUntill >= DateTime.Now);
            }

            return poll;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attackersPoints"></param>
        /// <param name="defendersPoints"></param>
        /// <param name="villageType"></param>
        /// <returns>number between 0 and 1 </returns>
        public double CalcBattleHandicap(int attackersPoints, int defendersPoints, Fbg.Bll.CONSTS.VillageType villageType)
        {
            return this.BattleHandicap.CalcBattleHandicap(attackersPoints, defendersPoints, villageType);
        }


        #region ISerializableToNameValueCollection2 Members

        public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col, string prefix )
        {
            try
            {
                string pre = prefix + "Realm[" + this.ID.ToString() + "]";

                if (col == null)
                {
                    ExceptionManager.Publish("Error in Player.SerializeToNameValueCollection. argument 'col' is null");
                }
                else
                {
                    BaseApplicationException.AddAdditionalInformation(col, pre + "Name", this.Name);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "dtRealm", this.dtRealm);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "dtPFPackages", this.dtPFPackages);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "dtPFTrials", this.dtPFTrials);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "dtRealmAttributes", this.dtRealmAttributes);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_maxplayers", this._maxplayers);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_openOn", this._openOn);
                    BaseApplicationException.AddAdditionalInformation(col, pre + "_buildingTypes.Count"
                        , _buildingTypes == null ? "null" : _buildingTypes.Count.ToString());
                    
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish("Error in Player.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + ExceptionManager.SerializeToString(e));
            }
        }
        public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
        {
            SerializeToNameValueCollection(col, String.Empty);
        }

        #endregion

        public enum AdminStatus
        {
            AllOK,
            EventHandlerDown

        }
        public AdminStatus Status()
        { 
            if (DAL.Realm.AdminStatus(this.ConnectionStr) > 0)
            {
                return AdminStatus.EventHandlerDown;
            }
            else
            {
                return AdminStatus.AllOK;
            }
        }


        /// <summary>
        /// may return null if no such gift on this realm
        /// </summary>
        /// <param name="giftID"></param>
        /// <returns></returns>
        public Gift GiftByID(int giftID)
        {
            foreach (Gift g in Gifts)
            {
                if (g.Id == giftID)
                {
                    return g;
                }
            }
            return null;
        }

        

        /// <summary>
        /// this tells you what level HQ you need to get unlimited building Q with all other factors reamining the same. 
        /// ie, on a non-vp realm, you can get this if you unlock the feature - this properly does not look at this. 
        /// </summary>
        public int HQLevelNeededForUnlimitedQ
        {
            get
            {
                if (IsVPrealm)
                {
                    return 15;
                }
                else
                {
                    return 10;
                }
            }
        }

        public bool AreQuestsDisabled { get; set; }

        public int GovernmentTypesEnabled { get; private set; }
        public AccessDeviceTypeLimitations AccessDeviceTypeLimitation { get; private set; }

        /// <summary>
        /// CURRENTLY NOT USED FOR ANYTHING
        /// </summary>
        public int BattleAlgorithmVersion { get; private set; }


        public DataTable Villages
        {
            get
            {
                return DAL.Villages.GetAllVillages(this.ConnectionStr);
            }
        }
    }
}
