using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fbg.Common;
using Fbg.Bll;

namespace Fbg.Bll.Api
{
    public class VillageApi
    {
        Fbg.Bll.Player _player;
        public VillageApi(Fbg.Bll.Player player)
        {
            _player = player;
        }




        /// <summary>
        /// (almost ) same as Upgrade_GetUpgradeInfo(int vid, int bid) except that this returns a list of all building upgrade info, plus ypgrade Q
        /// This also does not return recruitment, or building effect. 
        /// </summary>
        /// <returns>
        /// </returns>
        public string Upgrade_GetUpgradeInfo(int vid)
        {
           
            return ApiHelper.RETURN_SUCCESS(
                Upgrade_GetUpgradeInfo_raw(vid)
            ); 
        }



        public string GetVillage(int vid)
        {
            return ApiHelper.RETURN_SUCCESS(
               GetVillage_raw(vid), new Fbg.Bll.Api.ApiHelper.Converter()
           );
        }

        public object GetVillage_raw(int vid)
        {
            Village v = Fbg.Bll.Village.GetVillage(_player, vid, false, false);
            return GetVillage_raw(v);
        }
        public object GetVillage_raw(Village v)
        {
            // grabbing now timestamp as the first thing we do; this will let us remember when was the last time this was called. 
            DateTime timeStampWhenExtendedPropertiesLastRetrieved = DateTime.Now;

            if (v != null)
            {
               // VillageBasicB vb = v.CloneandGetVillageBasicB;


                object Village = new
                {                   
                    timeStampWhenExtendedPropertiesLastRetrieved =  ApiHelper.SerializeDate(timeStampWhenExtendedPropertiesLastRetrieved)
                    , Village = v
                    , VOV = GetVOVInfo(v) 
                    , upgrade = Upgrade_GetUpgradeInfo_raw(v)
                    , recruit = Recruit_GetRecruitInfo_raw(v)
                    , Buildings = GetBuildingsInVillage(v)
                };

                //return ApiHelper.RETURN_SUCCESS(
                //    villageUnitInfo, new Fbg.Bll.Api.ApiHelper.Converter()
                //);
                return Village;
            }
            else
            {
                //return ApiHelper.RETURN_SUCCESS(null);
                return null;
            }
        }


        public object Upgrade_GetUpgradeInfo_raw(int vid)
        {
            Village v = _player.Village(vid);
            return Upgrade_GetUpgradeInfo_raw(v);
        }
        /// <summary>
        /// (almost ) same as Upgrade_GetUpgradeInfo(int vid, int bid) except that this returns a list of all building upgrade info, plus ypgrade Q
        /// This also does not return recruitment, or building effect. 
        /// </summary>
        /// <returns>
        /// </returns>
        public object Upgrade_GetUpgradeInfo_raw(Village v)
        {            
            TimeSpan totalUpTime;
            TimeSpan totalDownTime;

            List<UpgradeEventSimple> q = GetUpgradingQ(v, out totalUpTime);
            List<DowngradeEventSimple> q_downgrades = GetDowngradingQ(v, out totalDownTime);

            return
                new
                {
                    villageinfo_food = v.RemainingPopulation,
                    villageinfo_silver = v.coins,
                    villageinfo_CurPop = v.CurrentPopulation,
                    villageinfo_RemPop = v.RemainingPopulation,
                    villageinfo_name = v.name,
                    villageinfo_x = v.xcord,
                    villageinfo_y = v.ycord,
                    villageinfo_id = v.id,
                    villageinfo_minSpeedUpAllowed = v.minutesOfSpeedupsAllowed,

                    Buildings = Upgrade_GetUpgradeInfo_GetBuildings(v),
                    Q = new
                    {
                        totalUpgradeTime = ApiHelper.SerializeTimeSpan(totalUpTime),
                        UpgradeEvents = q,
                        finishesOn = ApiHelper.SerializeDate(DateTime.Now + totalUpTime)
                    }
                    ,
                    DowngradeQ = q_downgrades

                };
            
        }

         List<UpgradeEventSimple> GetUpgradingQ(Village v, out TimeSpan totalTime)
        {
            List<UpgradeEvent> list = v.GetAllUpgradingBuildings();
            List<UpgradeEventSimple> q = new List<UpgradeEventSimple>(list.Count);
            totalTime = TimeSpan.Zero;
            foreach (UpgradeEvent e in list)
            {
                if (e is UpgradeEvent_UpgradeInQ)
                {
                    q.Add(new UpgradeEventSimple_UpgradeInQ((UpgradeEvent_UpgradeInQ)e, v));
                    totalTime += e.upgradeToLevel.BuildTime(v);
                }
                else
                {
                    q.Add(new UpgradeEventSimple_CurrentlyUpgrading((UpgradeEvent_CurrentlyUpgrading)e, v));
                    totalTime += e.completionTime.Subtract(DateTime.Now);
                }
            }



            return q;
        }


         protected List<DowngradeEventSimple> GetDowngradingQ(Village v, out TimeSpan totalTime)
        {
            List<DowngradeEvent> list = v.GetAllDowngradingBuildings();
            List<DowngradeEventSimple> q = new List<DowngradeEventSimple>(list.Count);
            totalTime = TimeSpan.Zero;
            foreach (DowngradeEvent e in list)
            {
                if (e is DowngradeEvent_DowngradeInQ)
                {
                    q.Add(new DowngradeEventSimple_DowngradeInQ((DowngradeEvent_DowngradeInQ)e));
                }
                else
                {
                    q.Add(new DowngradeEventSimple_CurrentlyDowngrading((DowngradeEvent_CurrentlyDowngrading)e));
                }
            }



            return q;
        }

         #region inner public classes
         public abstract class UpgradeEventSimple
        {
            public UpgradeEventSimple(UpgradeEvent e, Village v)
            {
                this.upgradeToLevel = e.upgradeToLevel.Level;
                this.bid = e.upgradeToLevel.Building.ID;
                this.completionTime = ApiHelper.SerializeDate(e.completionTime);
                this.upgradeDuration = ApiHelper.SerializeTimeSpan(e.upgradeToLevel.BuildTime(v));
                this.ID = e.ID;
            }
            public int upgradeToLevel;
            public int bid;
            public double completionTime;
            public long ID;
            public double upgradeDuration;
        }


        public class UpgradeEventSimple_CurrentlyUpgrading : UpgradeEventSimple
        {
            public UpgradeEventSimple_CurrentlyUpgrading(UpgradeEvent_CurrentlyUpgrading e, Village v)
                : base((UpgradeEvent)e, v)
            {
                this.eventID = e.eventID;
            }
            public long eventID;
        }

        public class UpgradeEventSimple_UpgradeInQ : UpgradeEventSimple
        {
            public UpgradeEventSimple_UpgradeInQ(UpgradeEvent_UpgradeInQ e, Village v)
                : base((UpgradeEvent)e, v)
            {
                this.qEntryID = e.qEntryID;
            }
            public long qEntryID;
        }



        public abstract class DowngradeEventSimple
        {
            public DowngradeEventSimple(BuildingType bt, DateTime completionTime)
            {
                this.buildingID = bt.ID;
                this.completionTime = ApiHelper.SerializeDate(completionTime);
            }
            public int buildingID;
            public double completionTime;
        }

        public class DowngradeEventSimple_CurrentlyDowngrading : DowngradeEventSimple
        {
            public DowngradeEventSimple_CurrentlyDowngrading(DowngradeEvent_CurrentlyDowngrading d)
                : base(d.bt, d.completionTime)
            {
                this.eventID = d.eventID;
            }
            public long eventID;
        }

        public class DowngradeEventSimple_DowngradeInQ : DowngradeEventSimple
        {
            public DowngradeEventSimple_DowngradeInQ(DowngradeEvent_DowngradeInQ d)
                : base(d.bt, d.completionTime)
            {
                this.qEntryID = d.qEntryID;
            }
            public long qEntryID;
        }
        #endregion

        Dictionary<string, object> GetBuildingsInVillage(Village v)
        {

            Dictionary<string, object> buildings = new Dictionary<string, object>(_player.Realm.BuildingTypes.Count);

            BuildingTypeLevel btl;
            foreach (BuildingType bt in _player.Realm.BuildingTypes)
            {
                btl = v.GetBuildingLevelObject(bt.ID);
                OneBuildingUpgradeInfo up = Upgrade_GetUpgradeInfo_Raw_CurLevelAndUpgradeInfo(v, bt);

                buildings.Add(bt.ID.ToString(), 
                    new
                    {
                        id = bt.ID,
                        curLevel = btl == null ? 0 : btl.Level,                        
                        Upgrade = new
                        {
                            canUpgrade = (int)up.canUpgrade,
                            canDowngrade = v.CanDowngrade(bt),
                            nextLevel = up.nextLevel,
                            nextLevel_max = up.nextLevel_Max,
                            unsatisfiedRequirementsIfAny = from x in up.unsatisfiedReqs select new { btid = x.Building.ID, level = x.Level }
                        },
                    }
                );
            }
            return buildings;
        }


        List<object> Upgrade_GetUpgradeInfo_GetBuildings(Village v)
        {

            List<object> allBuildingUpgrades = new List<object>(_player.Realm.BuildingTypes.Count);

            OneBuildingUpgradeInfo up;
            foreach (BuildingType bt in _player.Realm.BuildingTypes)
            {
                up = Upgrade_GetUpgradeInfo_Raw_CurLevelAndUpgradeInfo(v, bt);

                allBuildingUpgrades.Add(
                    new
                    {
                        buildingID = up.buildingType.ID,
                        curLevel = up.curBuildingLevel,
                        Upgrade = new
                        {
                            canUpgrade = (int)up.canUpgrade,
                            canDowngrade = v.CanDowngrade(bt),
                            nextLevel = up.nextLevel,
                            nextLevel_max = up.nextLevel_Max,
                            unsatisfiedRequirementsIfAny = from btl in up.unsatisfiedReqs select new { btid = btl.Building.ID, level = btl.Level }
                        },

                        EffectInfo = GetEffectInfo(bt, v, up.curBuildingTypeLevel),
                        EffectInfoNextLevel = up.nextBuildingTypeLevel == null ? null : GetEffectInfo(bt, v, up.nextBuildingTypeLevel)

                    }
                );
            }
            return allBuildingUpgrades;
        }


        /// <summary>
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="bid"></param>
        /// <returns>
        /// upgradeInfo :
        ///      **** curLevel **** current building level OR 0 if not built
        ///      **** villageinfo_food ****  food remaining in villagd 
        ///      **** villageinfo_silver **** silver in village
        ///      
        ///      **** Upgrade **** Class:
        ///          canUpgrade is the INT version of Fbg.Bll.Village.CanUpgradeResult.
        ///          possible values of canUpgrade, and you shoudl check in this order: 
        ///          (7) No_DowngradesInProgress   - nextLevel and unsatisfiedRequirementsIfAny are valid
        ///          (6) No_UnsatisfiedReq         - nextLevel and unsatisfiedRequirementsIfAny are valid
        ///          (3) No_BuildingFullyUpgraded  - nextLevel and unsatisfiedRequirementsIfAny are NOT valid
        ///          (1) No_LackFood               - nextLevel and unsatisfiedRequirementsIfAny are valid
        ///          (2) No_LackSilver             - nextLevel and unsatisfiedRequirementsIfAny are valid
        ///          (5) No_Busy                   - nextLevel and unsatisfiedRequirementsIfAny are valid
        ///          (4) No_LockedFeature          - nextLevel and unsatisfiedRequirementsIfAny are valid
        ///          (0) Yes                       - nextLevel and unsatisfiedRequirementsIfAny are valid
        ///      
        ///          unsatisfiedRequirementsIfAny is an array of "btid" (building ID) and "level" (building level) 
        ///      
        ///      **** Upgrading **** Class - see code
        ///      **** UpgradesQueuedUp **** Class - see code
        ///      
        ///  recruitInfo : 
        ///       **** unitRecruitInfo *** 
        ///        - an array of objects that holds the units that can be recruited at thie building
        ///        unitRecruitInfo[x].unitCost - cost in silver for the unit
        ///        unitRecruitInfo[x].uid - unit id
        ///        unitRecruitInfo[x].canRecruit - enum telling you if you can recruit this unit time now and if not, why not. 
        ///          (1) No_RecruitmentBuildingNotBuilt. no out put param is consiten
        ///          (2) No_RequirementsNotSatisfied. Recrutment building is built but not all building OR research req satisfied. see  unsatisfiedReq and unsatisfiedReqRes
        ///          (4) No_LackFood. no food
        ///          (3) No_LackSilver. no silver
        ///          (0) Yes - all is well !
        ///        unitRecruitInfo[x].unsatisfiedReq - array of building and level ids [{btid=x, level=y} , ...] that are necessary in order to be able to recruit this unit type. THIS can be NULL or empty array if no req
        ///        unitRecruitInfo[x].unsatisfiedReqRes - list of research item ID [1, 5, ...] that are necessary in order to be able to recruit this unit type. THIS can be NULL or empty array if no req
        ///        unitRecruitInfo[x].time - time it takes to recruit this unit type
        ///        unitRecruitInfo[x].currentCount - count of troops of this unit type that belong to this villag - that were recruited there. 
        ///        unitRecruitInfo[x].maxToRecruit - max that can be recruited of this unit type now
        /// </returns>
        public string Upgrade_GetUpgradeInfo(int vid, int bid)
        {
            return ApiHelper.RETURN_SUCCESS(
                Upgrade_GetUpgradeInfo_Raw(vid, bid)
               );
        }


        protected object Upgrade_GetUpgradeInfo_Raw(int vid, int bid)
        {
            Village v = _player.Village(vid);
            BuildingType bt = _player.Realm.BuildingType(bid);
            object upgradeInfo = Upgrade_GetUpgradeInfo_Raw(v, bt);
            object recruitInfo = Recruit_GetRecruitInfo_Raw(v, bt);
            object govInfo = null;
            object result = null;
            if (bid == Fbg.Bll.CONSTS.BuildingIDs.Palace)
            {
                govInfo = Gov_GetInfo_Raw(v);
                result = new { upgradeInfo, recruitInfo, govInfo };
            }
            else
            {
                result = new { upgradeInfo, recruitInfo };
            }
            return result;
        }
        protected object Upgrade_GetUpgradeInfo_Raw(Village v, BuildingType bt)
        {

            #region current level, and upgrade to next level info
            OneBuildingUpgradeInfo up = Upgrade_GetUpgradeInfo_Raw_CurLevelAndUpgradeInfo(v, bt);
            #endregion


            #region currently upgrading or queued up level info
            //
            // currently upgrading or queued up level info
            //
            object upgrading = null;
            object upgradesQueuedUp = null;
            List<UpgradeEvent> upgrades = v.GetAllUpgradingBuildings();
            UpgradeEvent_CurrentlyUpgrading currentlyUpgradingUpgrade;

            if (upgrades.Count <= 0)
            {
                upgrading = null;
            }
            else
            {
                //
                // if currently upgrading item is the current building, then grab info about this upgrade
                //
                currentlyUpgradingUpgrade = (UpgradeEvent_CurrentlyUpgrading)upgrades[0]; ;
                if (currentlyUpgradingUpgrade.upgradeToLevel.Building == bt)
                {

                    upgrading = new
                    {
                        timeLeft = ApiHelper.SerializeTimeSpan(currentlyUpgradingUpgrade.completionTime.Subtract(DateTime.Now)),
                        totalTime = ApiHelper.SerializeTimeSpan(currentlyUpgradingUpgrade.upgradeToLevel.BuildTime(v))
                        ,
                        totalTimeFormatted = utils.FormatDuration(currentlyUpgradingUpgrade.upgradeToLevel.BuildTime(v))
                        ,
                        completesOn = ApiHelper.SerializeDate(currentlyUpgradingUpgrade.completionTime)
                        ,
                        eventID = currentlyUpgradingUpgrade.eventID
                    };

                    // TODO: if VP realm, return info about speed up  -- how to handle this ???
                }
                //
                // display queded up items if any
                //
                int upgradingBuildingLevel = v.GetUpgradingBuildingLevel(bt.ID);
                if ((currentlyUpgradingUpgrade.upgradeToLevel.Building == bt && upgradingBuildingLevel > up.curBuildingLevel + 1)
                    || (currentlyUpgradingUpgrade.upgradeToLevel.Building != bt && upgradingBuildingLevel > up.curBuildingLevel))
                {

                    upgradesQueuedUp = new
                    {
                        toLevel = upgradingBuildingLevel
                    };
                }

            }

            #endregion



            return
                new
                {
                    curLevel = up.curBuildingLevel,
                    villageinfo_food = v.RemainingPopulation,
                    villageinfo_silver = v.coins,
                    villageinfo_CurPop = v.CurrentPopulation,
                    villageinfo_RemPop = v.RemainingPopulation,
                    villageinfo_name = v.name,
                    villageinfo_x = v.xcord,
                    villageinfo_y = v.ycord,
                    villageinfo_id = v.id,

                    Upgrade = new
                    {
                        canUpgrade = (int)up.canUpgrade,
                        nextLevel = up.nextLevel,
                        nextLevel_Max = up.nextLevel_Max,
                        unsatisfiedRequirementsIfAny = from btl in up.unsatisfiedReqs select new { btid = btl.Building.ID, level = btl.Level }
                    },

                    Upgrading = upgrading,

                    UpgradesQueuedUp = upgradesQueuedUp,

                    EffectInfo = GetEffectInfo(bt, v, up.curBuildingTypeLevel),
                    EffectInfoNextLevel = up.nextBuildingTypeLevel == null ? null : GetEffectInfo(bt, v, up.nextBuildingTypeLevel)

                };
        }

        protected class OneBuildingUpgradeInfo
        {
            public BuildingType buildingType = null;
            public BuildingTypeLevel nextBuildingTypeLevel = null;
            public BuildingTypeLevel curBuildingTypeLevel = null;
            public int curBuildingLevel;
            public BuildingTypeLevel maxNextBuildingTypeLevel = null;
            public List<BuildingTypeLevel> unsatisfiedReqs = null;
            public bool notEnoughSilver = false;
            public bool notEnoughFood = false;
            public Village.CanUpgradeResult canUpgrade = Village.CanUpgradeResult.No_BuildingFullyUpgraded;
            public bool maxLevelReached;
            public object nextLevel = null;
            public object nextLevel_Max = null;
            public OneBuildingUpgradeInfo(BuildingType bt)
            {
                buildingType = bt;
            }
        }
        protected class AllBuildingRecruitInfo
        {
            public List<OneBuildingRecruitInfo> recruitInfoList;
            public object govInfo;

            public AllBuildingRecruitInfo()
            {
                recruitInfoList = new List<OneBuildingRecruitInfo>();
            }
        }

        protected class OneBuildingRecruitInfo
        {
            public int buildingTypeID = 0;
            public OneBuildingRecruitInfo(int btid)
            {
                buildingTypeID = btid;
                q = new List<object>();
                unitsRecruited = new List<OneUnitRecruitInfo>(2);

            }

            public List<object> q;
            public List<OneUnitRecruitInfo> unitsRecruited;

            public class OneUnitRecruitInfo
            {
                public OneUnitRecruitInfo(int unitID)
                {
                    uid = unitID;
                }

                public int unitCost = 0;
                public int uid;
                public Fbg.Bll.Village.CanRecruitResult canRecruit;
                public object unsatisfiedReq;
                public object unsatisfiedReqRes;
                public double time;
                public int currentCount;
                public int maxToRecruit;
            }

        }





        protected OneBuildingUpgradeInfo Upgrade_GetUpgradeInfo_Raw_CurLevelAndUpgradeInfo(Village v, BuildingType bt)
        {

            //
            //current level, and upgrade to next level info
            //
            OneBuildingUpgradeInfo up = new OneBuildingUpgradeInfo(bt);

            up.canUpgrade = Village.CanUpgradeResult.No_BuildingFullyUpgraded;

            try
            {

                up.canUpgrade = v.CanUpgrade(bt
                    , out up.nextBuildingTypeLevel
                    , out up.curBuildingTypeLevel
                    , out up.maxNextBuildingTypeLevel
                    , out up.unsatisfiedReqs
                    , out up.notEnoughSilver
                    , out up.notEnoughFood
                    , _player.NightBuild_IsActive);


                if (up.curBuildingTypeLevel == null)
                {
                    up.curBuildingLevel = 0;
                    up.maxLevelReached = false;
                }
                else
                {
                    up.curBuildingLevel = up.curBuildingTypeLevel.Level;
                    up.maxLevelReached = up.curBuildingTypeLevel.GetNextLevel() == null ? true : false;
                }

                if (!up.maxLevelReached)
                {
                    if (up.nextBuildingTypeLevel != null)
                    {
                        up.nextLevel = new
                        {
                            cost = up.nextBuildingTypeLevel.Cost,
                            food = up.nextBuildingTypeLevel.Population,
                            time = ApiHelper.SerializeTimeSpan(up.nextBuildingTypeLevel.BuildTime(v)),
                            timeFormatted = utils.FormatDuration_Long2(up.nextBuildingTypeLevel.BuildTime(v)),
                            levelNum = up.nextBuildingTypeLevel.Level
                        };
                    }
                    if (up.maxNextBuildingTypeLevel != null)
                    {
                        up.nextLevel_Max = new
                        {
                            levelNum = up.maxNextBuildingTypeLevel.Level
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                Gmbc.Common.Diagnostics.ExceptionManagement.BaseApplicationException bex = new Gmbc.Common.Diagnostics.ExceptionManagement.BaseApplicationException("eror in Upgrade_GetUpgradeInfo_Raw_CurLevelAndUpgradeInfo", ex);
                bex.AddAdditionalInformation("nextBuildingTypeLevel", up.nextBuildingTypeLevel);
                bex.AddAdditionalInformation("curBuildingTypeLevel", up.curBuildingTypeLevel);
                bex.AddAdditionalInformation("maxNextBuildingTypeLevel", up.maxNextBuildingTypeLevel);
                bex.AddAdditionalInformation("unsatisfiedReqs", up.unsatisfiedReqs);
                bex.AddAdditionalInformation("notEnoughSilver", up.notEnoughSilver);
                bex.AddAdditionalInformation("notEnoughFood", up.notEnoughFood);
                bex.AddAdditionalInformation("canUpgrade", up.canUpgrade);
                throw bex;
            }


            return up;
        }


        /// <summary>

        /// </summary>
        /// <param name="vid"></param>
        /// <param name="bid"></param>
        /// <returns>
        /// IF could not upgrade for some reason, then return the reason for the failed upgrade in property called "cannotUpgrade"
        ///     it is the exact same value as Upgrade.canUpgrade returned from Upgrade_GetUpgradeInfo
        /// 
        /// ELSE IF upgrade was sucessful, then the return is EXACTLY the same as from "Upgrade_GetUpgradeInfo"
        /// 
        /// </returns>
        public string Upgrade_DoUpgrade(int vid, int bid)
        {
            Village v = _player.Village(vid);
            BuildingType bt = _player.Realm.BuildingType(bid);

            BuildingTypeLevel nextBuildingTypeLevel = null;
            BuildingTypeLevel curBuildingTypeLevel = null;
            BuildingTypeLevel maxNextBuildingTypeLevel = null;
            List<BuildingTypeLevel> unsatisfiedReqs = null;
            bool notEnoughSilver = false;
            bool notEnoughFood = false;
            Village.CanUpgradeResult canUpgrade = Village.CanUpgradeResult.No_BuildingFullyUpgraded;


            try
            {

                canUpgrade = v.CanUpgrade(bt
                    , out nextBuildingTypeLevel
                    , out curBuildingTypeLevel
                    , out maxNextBuildingTypeLevel
                    , out unsatisfiedReqs
                    , out notEnoughSilver
                    , out notEnoughFood
                    , _player.NightBuild_IsActive);


                if (canUpgrade == Village.CanUpgradeResult.Yes)
                {
                    _player.DoUpgrade(v.id, bt.ID, nextBuildingTypeLevel.Level);

                    return Upgrade_GetUpgradeInfo(v.id, bt.ID);
                }
                else
                {
                    return ApiHelper.RETURN_SUCCESS(
                      new
                      {
                          cannotUpgrade = canUpgrade
                          ,
                          upgradeInfo = Upgrade_GetUpgradeInfo_Raw(v.id, bt.ID)
                      });
                }
            }
            catch (Exception ex)
            {
                Gmbc.Common.Diagnostics.ExceptionManagement.BaseApplicationException bex = new Gmbc.Common.Diagnostics.ExceptionManagement.BaseApplicationException("Error in Do Upgrade", ex);
                bex.AddAdditionalInformation("nextBuildingTypeLevel", nextBuildingTypeLevel);
                bex.AddAdditionalInformation("curBuildingTypeLevel", curBuildingTypeLevel);
                bex.AddAdditionalInformation("maxNextBuildingTypeLevel", maxNextBuildingTypeLevel);
                bex.AddAdditionalInformation("unsatisfiedReqs", unsatisfiedReqs);
                bex.AddAdditionalInformation("notEnoughSilver", notEnoughSilver);
                bex.AddAdditionalInformation("notEnoughFood", notEnoughFood);
                bex.AddAdditionalInformation("canUpgrade", canUpgrade);
                throw bex;
            }

        }

        /// <summary>

        /// </summary>
        /// <param name="vid"></param>
        /// <param name="bid"></param>
        /// <returns>
        /// IF could not upgrade for some reason, then return the reason for the failed upgrade in property called "cannotUpgrade"
        ///     it is the exact same value as Upgrade.canUpgrade returned from Upgrade_GetUpgradeInfo
        /// 
        /// ELSE IF upgrade was sucessful, then the return is EXACTLY the same as from "Upgrade_GetUpgradeInfo"
        /// 
        /// </returns>
        public string Upgrade_DoUpgrade2(int vid, int bid, int levelToUpgradeTo)
        {
            Village v = _player.Village(vid);
            BuildingType bt = _player.Realm.BuildingType(bid);

            BuildingTypeLevel nextBuildingTypeLevel = null;
            BuildingTypeLevel curBuildingTypeLevel = null;
            BuildingTypeLevel maxNextBuildingTypeLevel = null;
            List<BuildingTypeLevel> unsatisfiedReqs = null;
            bool notEnoughSilver = false;
            bool notEnoughFood = false;
            Village.CanUpgradeResult canUpgrade = Village.CanUpgradeResult.No_BuildingFullyUpgraded;


            try
            {

                canUpgrade = v.CanUpgrade(bt
                    , out nextBuildingTypeLevel
                    , out curBuildingTypeLevel
                    , out maxNextBuildingTypeLevel
                    , out unsatisfiedReqs
                    , out notEnoughSilver
                    , out notEnoughFood
                    , _player.NightBuild_IsActive);


                if (canUpgrade == Village.CanUpgradeResult.Yes)
                {
                    if (levelToUpgradeTo == nextBuildingTypeLevel.Level)
                    {
                        //
                        // if requested upgrade to level is simply the next level, then we know its a simple upgrade to next level
                        //
                        _player.DoUpgrade(v.id, bid, nextBuildingTypeLevel.Level);

                    }
                    else if (levelToUpgradeTo > nextBuildingTypeLevel.Level)
                    {
                        //
                        // upgrade to level higher then next level so we know its a upgrade max function
                        //
                        levelToUpgradeTo = levelToUpgradeTo < maxNextBuildingTypeLevel.Level ? levelToUpgradeTo : maxNextBuildingTypeLevel.Level;


                        for (int i = nextBuildingTypeLevel.Level; i <= levelToUpgradeTo; i++)
                        {
                            _player.DoUpgrade(v.id, bid, i);
                        }
                    }

                    //return Upgrade_GetUpgradeInfo(v.id);
                    return GetVillage(v.id);                      
                }
                else
                {
                    return ApiHelper.RETURN_SUCCESS(
                      new
                      {
                          cannotUpgrade = canUpgrade
                          //,upgradeInfo = Upgrade_GetUpgradeInfo(v.id)
                          , Village = GetVillage_raw(v.id)
                      }, new Fbg.Bll.Api.ApiHelper.Converter());
                }
            }
            catch (Exception ex)
            {
                Gmbc.Common.Diagnostics.ExceptionManagement.BaseApplicationException bex = new Gmbc.Common.Diagnostics.ExceptionManagement.BaseApplicationException("Error in Do Upgrade2", ex);
                bex.AddAdditionalInformation("nextBuildingTypeLevel", nextBuildingTypeLevel);
                bex.AddAdditionalInformation("curBuildingTypeLevel", curBuildingTypeLevel);
                bex.AddAdditionalInformation("maxNextBuildingTypeLevel", maxNextBuildingTypeLevel);
                bex.AddAdditionalInformation("unsatisfiedReqs", unsatisfiedReqs);
                bex.AddAdditionalInformation("notEnoughSilver", notEnoughSilver);
                bex.AddAdditionalInformation("notEnoughFood", notEnoughFood);
                bex.AddAdditionalInformation("canUpgrade", canUpgrade);
                bex.AddAdditionalInformation("levelToUpgradeTo", levelToUpgradeTo);

                throw bex;
            }

        }



        /// <summary>
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="bid"></param>
        /// <returns>
        /// tries to cancel the passed in event and the return is EXACTLY the same as from "Upgrade_GetUpgradeInfo, with building id passed in "
        /// 
        /// </returns>
        public string Upgrade_Cancel(int vid, int bid, long eventID, bool isQ)
        {
            _player.CancelEvent(eventID, isQ);

            return GetVillage(vid);
        }


        /// <summary>
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="bid"></param>
        /// <returns>
        /// tries to cancel the passed in event and the return is EXACTLY the same as from "Upgrade_GetUpgradeInfo, without building ID passed in "
        /// 
        /// </returns>
        public string Upgrade_Cancel(int vid, long eventID, bool isQ)
        {
            _player.CancelEvent(eventID, isQ);

            return GetVillage(vid);
        }



        /// <summary>
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="bid"></param>
        /// <returns>
        /// tries to cancel the passed in event and the return is EXACTLY the same as from "Upgrade_GetUpgradeInfo"
        /// 
        /// </returns>
        public string Upgrade_SpeedUp(int vid, int? bid, long eventID, int cutTimeInMin)
        {
            Village v = _player.Village(vid);
            //BuildingType bt = FbgPlayer.Realm.BuildingType(bid);

            int cost = v.SpeedUpUpgrade(eventID, cutTimeInMin);
            int costOfThisCut = 0;
            bool cutSuccessful = false;
            bool failed_lackServants = false;
            int creditsNow;
            // HACK: we want to note that player has used the cut by 1 minute spell but we consider the cut by one minute 
            //  AND the Finish Now spell (if cut less then 1 minute) to be the same thing. 
            if (cost == 1)
            {
                _player.Quests2.SetQuestAsCompleted(Fbg.Bll.Player.QuestTags.BuildingSpeedUp1.ToString());
            }

            switch (cutTimeInMin)
            {
                case 1:
                    costOfThisCut = Fbg.Bll.CONSTS.PFCosts.CutBuildTime1Min;
                    break;
                case 15:
                    costOfThisCut = Fbg.Bll.CONSTS.PFCosts.CutBuildTime15Min;
                    _player.Quests2.SetQuestAsCompleted(Fbg.Bll.Player.QuestTags.BuildingSpeedUp2.ToString());
                    break;
                case 60:
                    costOfThisCut = Fbg.Bll.CONSTS.PFCosts.CutBuildTime1H;
                    break;
                case 240:
                    costOfThisCut = Fbg.Bll.CONSTS.PFCosts.CutBuildTime4H;
                    break;
                case 9999:
                    //Do nothing on purpose.            
                    break;
            }
            if (cost > 0)
            {
                creditsNow = _player.User.Credits;
                cutSuccessful = true;
            }
            else
            {
                creditsNow = _player.User.Credits;
                if (creditsNow < costOfThisCut || cutTimeInMin == 9999)
                {
                    // not, for 9999, we assume this is the case, we dont have a way to validate if credits were missing
                    failed_lackServants = true;
                }
            }


            return ApiHelper.RETURN_SUCCESS(
                     new
                     {
                         playersCreditsNow = creditsNow,
                         cutSuccessful = cutSuccessful,
                         failed_lackServants = failed_lackServants,
                         //UpgradeInfo = (bid == null ? Upgrade_GetUpgradeInfo_raw(vid) : Upgrade_GetUpgradeInfo_Raw(vid, (int)bid))
                         Village = GetVillage_raw(vid)

                     }, new Fbg.Bll.Api.ApiHelper.Converter());
        }

        public string Upgrade_SpeedUpFree(int vid, long eventID)
        {
            if (_player.Realm.RealmType == "X") {
                return "feature not allowed";
            }
            
            Village v = _player.Village(vid);
            bool cutSuccessful = v.SpeedUpUpgradeFree(eventID);

            return ApiHelper.RETURN_SUCCESS(
                     new
                     {
                         playersCreditsNow = _player.User.Credits, //its free, but keep this here for strcuture standard
                         cutSuccessful = cutSuccessful, //if false, something went wrong DB / sql
                         failed_lackServants = false, //its free, but keep this here for strcuture standard
                         Village = GetVillage_raw(vid) //standard village object
                     }, new Fbg.Bll.Api.ApiHelper.Converter());
        }
        


        /// <summary>

        /// </summary>
        /// <param name="vid"></param>
        /// <param name="bid"></param>
        /// <returns>
        /// 
        /// </returns>
        public string Downgrade_DoDwngrade(int vid, int bid)
        {

            Village village = null;
            BuildingType bt = null;


            try
            {
                village = _player.Village(vid);
                bt = _player.Realm.BuildingType(bid);

                //
                // Not accessible by stewards -- NOT SURE HOW TO HANDLE THIS ON MOBILE, IF EVEN NEEDED... 
                //
                //if (FbgPlayer.Stewardship_IsLoggedInAsSteward)
                //{
                //    Response.Redirect(NavigationHelper.StewardAccesDenied());
                //    return;
                //}


                village.BuildingDowngrade(bt.ID);
            }
            catch (Exception ex)
            {
                Gmbc.Common.Diagnostics.ExceptionManagement.BaseApplicationException bex = new Gmbc.Common.Diagnostics.ExceptionManagement.BaseApplicationException("Error in Downgrade_DoDwngrade", ex);
                bex.AddAdditionalInformation("village", village);
                bex.AddAdditionalInformation("bt", bt);

                throw bex;
            }

            return GetVillage(village.id);
        }



        /// <summary>
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="bid"></param>
        /// <returns>
        /// tries to cancel the passed in event and the return is EXACTLY the same as from "Upgrade_GetUpgradeInfo"
        /// 
        /// </returns>
        public string Downgrade_Cancel(int vid, int eventID, bool isQ)
        {
            Village village = null;

            village = _player.Village(vid);

            village.CancelDowngrade(eventID, isQ);


            return GetVillage(village.id);
        }

        /// <summary>
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="bid"></param>
        /// <returns>
        /// 
        /// 
        /// </returns>
        public string Downgrade_SpeedUp(int vid, long eventID)
        {
            Village village = _player.Village(vid);


            int cost = village.SpeedUpDowngrade(eventID);
            bool cutSuccessful;
            int creditsNow;

            if (cost > 0)
            {
                cutSuccessful = true;
            }
            else
            {
                cutSuccessful = false;
            }

            creditsNow = _player.User.Credits;
            return ApiHelper.RETURN_SUCCESS(
                     new
                     {
                         playersCreditsNow = creditsNow,
                         cutSuccessful = cutSuccessful,
                         //UpgradeInfo = Upgrade_GetUpgradeInfo_raw(village.id)
                         Village = GetVillage_raw(village.id)
                     }, new Fbg.Bll.Api.ApiHelper.Converter());
        }

        protected object Recruit_GetRecruitInfo_Raw(int vid, int bid)
        {
            Village village = _player.Village(vid);
            BuildingType bt = _player.Realm.BuildingType(bid);
            return Recruit_GetRecruitInfo_Raw(village, bt);
        }

        public string Recruit_GetRecruitInfo(int vid)
        {
            return ApiHelper.RETURN_SUCCESS(
                         Recruit_GetRecruitInfo_raw(vid)
                      );
        }

        public object Recruit_GetRecruitInfo_raw(int vid)
        {
            Village village = _player.Village(vid);
            return Recruit_GetRecruitInfo_raw(village);
        }

        public object Recruit_GetRecruitInfo_raw(Village village)
        {
            

            AllBuildingRecruitInfo recruitInfo = new AllBuildingRecruitInfo();
            foreach (BuildingType bt in _player.Realm.BuildingTypes)
            {
                recruitInfo.recruitInfoList.Add(Recruit_GetRecruitInfo_Raw2(village, bt));
            }

            recruitInfo.govInfo = Gov_GetInfo_Raw(village);

            return new
            {
                villageinfo_food = village.RemainingPopulation,
                villageinfo_silver = village.coins,
                villageinfo_CurPop = village.CurrentPopulation,
                villageinfo_RemPop = village.RemainingPopulation,
                villageinfo_name = village.name,
                villageinfo_x = village.xcord,
                villageinfo_y = village.ycord,
                villageinfo_id = village.id,
                recruitInfo
            };
        }

        protected object Recruit_GetRecruitInfo_Raw(Village village, BuildingType bt)
        {
            OneBuildingRecruitInfo info = Recruit_GetRecruitInfo_Raw2(village, bt);
            return new
            {
                unitRecruitInfo = info.unitsRecruited
                , recruitingQ = info.q
            };
        }

        protected OneBuildingRecruitInfo Recruit_GetRecruitInfo_Raw2(Village village, BuildingType bt)
        {
            bool canRecruit;
            bool recruitBuildingBuilt;
            List<BuildingTypeLevel> unsatisfiedReq;
            List<ResearchItem> unsatisfiedReqRes;
            int maxByPop;
            int maxByCost;
            int unitCost;
            int unitBaseCost;
            int maxToRecruit;
            Village.CanRecruitResult canRecruitResult;
            Dictionary<BuildingType, List<Fbg.Bll.UnitType>>.KeyCollection keys = _player.Realm.GetUnitTypesByBuildingType().Keys;
            List<Fbg.Bll.UnitType> units;
            OneBuildingRecruitInfo oneBuildingRecruitInfo;
            List<Object> recruitingQ = new List<object>();

            //
            // get units that can be recruited in this building
            //
            recruitBuildingBuilt = village.GetBuildingLevel(bt) == 0 ? false : true;
            canRecruit = recruitBuildingBuilt;
            oneBuildingRecruitInfo = new OneBuildingRecruitInfo(bt.ID);
            _player.Realm.GetUnitTypesByBuildingType().TryGetValue(bt, out units);

            if (units != null)
            {
                foreach (Fbg.Bll.UnitType ut in units)
                {
                    unitCost = ut.Cost(_player); //cache the cost since this could result in multiple DB calls
                    unitBaseCost = ut.Cost(null);

                    CanRecruit(village, ut, out unsatisfiedReq, out unsatisfiedReqRes, out maxByPop, out maxByCost, out maxToRecruit, out canRecruitResult);
                    canRecruit = canRecruitResult == Village.CanRecruitResult.Yes;

                    oneBuildingRecruitInfo.unitsRecruited.Add(new OneBuildingRecruitInfo.OneUnitRecruitInfo(ut.ID)
                     {
                         unitCost = unitCost,
                         canRecruit = canRecruitResult
                        ,
                         unsatisfiedReq = unsatisfiedReq == null ? null : from r in unsatisfiedReq select new { btid = r.Building.ID, level = r.Level }
                        ,
                         unsatisfiedReqRes = unsatisfiedReqRes == null ? null : from r in unsatisfiedReqRes select r.ID
                        ,
                         time = ApiHelper.SerializeTimeSpan(ut.RecruitmentTime(village))
                        ,
                         currentCount = village.GetVillageUnit(ut).YourUnitsTotalCount
                        ,
                         maxToRecruit = maxToRecruit
                     });
                    
                }
            }

            //
            // Get Recruitment Q
            //
            List<UnitRecruitQEntry> qEntries;

            village.GetAllRecruitmentQEntries().TryGetValue(bt, out qEntries);

            if (qEntries != null)
            {
                foreach (UnitRecruitQEntry qEntry in qEntries)
                {

                    if ((qEntry is UnitRecruitQEntry_First))
                    {
                        oneBuildingRecruitInfo.q.Add(
                            new
                            {
                                uid = qEntry.UnitType.ID
                                ,
                                cost = (qEntry.UnitType.ID == Fbg.Bll.CONSTS.UnitIDs.Gov) ? 0 : qEntry.TotalCost
                                ,
                                timeLeft = ApiHelper.SerializeDate(((UnitRecruitQEntry_First)qEntry).RecruitmentCompletedOn)
                                ,
                                qEntryID = qEntry.QEntryID
                                ,
                                count = qEntry.Count
                            }

                        );
                    }
                    else
                    {
                        oneBuildingRecruitInfo.q.Add(
                           new
                           {
                               uid = qEntry.UnitType.ID
                               ,
                               cost = (qEntry.UnitType.ID == Fbg.Bll.CONSTS.UnitIDs.Gov) ? 0 : qEntry.TotalCost
                               ,
                               time = ApiHelper.SerializeTimeSpan(qEntry.TotalRecruitTime)
                               ,
                               timeFormatted = utils.FormatDuration(qEntry.TotalRecruitTime)
                               ,
                               qEntryID = qEntry.QEntryID
                               ,
                               count = qEntry.Count
                           }
                       );
                    }
                }
            }


            return oneBuildingRecruitInfo;
        }

        private void CanRecruit(Fbg.Bll.Village village, Fbg.Bll.UnitType ut, out List<BuildingTypeLevel> unsatisfiedReq, out List<ResearchItem> unsatisfiedReqRes, out int maxByPop, out int maxByCost, out int maxToRecruit, out Village.CanRecruitResult canRecruitResult)
        {
            unsatisfiedReq = null;
            maxToRecruit = 0;
            maxByPop = 0;
            maxByCost = 0;
            unsatisfiedReqRes = null;
            canRecruitResult = Village.CanRecruitResult.No_LackFood;
            if (ut is Fbg.Bll.UnitTypeLord)
            {
                if (village.GetBuildingLevel(ut.RecruitmentBuilding) < 1)
                {
                    canRecruitResult = Village.CanRecruitResult.No_RecruitmentBuildingNotBuilt;
                }
                else if (ut.Pop > (village.MaxPopulation - village.CurrentPopulation))
                {
                    canRecruitResult = Village.CanRecruitResult.No_LackFood;
                }
                // TODO : do this still. see Controls/UnitRecuruit for example
                //else if (chestCostForNextGoverner > avilableChests) {
                //    canRecruitResult = Village.CanRecruitResult.No_LackSilver;
                //}
                else
                {
                    canRecruitResult = Village.CanRecruitResult.Yes;
                }

            }
            else
            {
                canRecruitResult = village.CanRecruit(ut, out maxToRecruit, out maxByPop, out maxByCost, out unsatisfiedReq, out unsatisfiedReqRes);
            }
        }
        public struct RecruitCommand
        {
            public int uid;
            public int count;
        }

        /// <summary>
        /// recruit troopa and return building info for the building specified
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="bid"></param>
        /// <param name="data"></param>
        /// <returns>upgrade info for building page</returns>
        public string Recruit_DoRecruit(int vid, int bid, List<RecruitCommand> data)
        {
            Village village = _player.Village(vid);
            Recruit_DoRecruit_justrecruit(village, data);


            return Upgrade_GetUpgradeInfo(vid, bid);
        }
        /// <summary>
        /// recruit troops and return all recruit info 
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="data"></param>
        /// <returns>recruit info</returns>
        public string Recruit_DoRecruit(int vid, List<RecruitCommand> data)
        {
            Village village = _player.Village(vid);
            Recruit_DoRecruit_justrecruit(village, data);

            return GetVillage(vid);
        }



        private void Recruit_DoRecruit_justrecruit(Village village, List<RecruitCommand> data)
        {           
            foreach (RecruitCommand cmd in data)
            {
                if (cmd.count > 0)
                {
                    if (cmd.uid == Fbg.Bll.CONSTS.UnitIDs.Gov)
                    {
                        village.RecruitGoverner();
                    }
                    else
                    {
                        village.Recruit(_player.Realm.GetUnitTypesByID(cmd.uid), cmd.count);
                    }
                }
            }
        }


        public enum Recruit_CancelReturnType
        {
            buildinginfo,
            recruitinfo
        }
        public string Recruit_Cancel(int vid, int bid, int eventID, Recruit_CancelReturnType returnType)
        {
            if (bid == Fbg.Bll.CONSTS.BuildingIDs.Palace)
            {
                Fbg.Bll.UnitRecruitQEntry.CancelGovernerRecruit(_player, vid);
            }
            else
            {
                Fbg.Bll.UnitRecruitQEntry.CancelRecruit(_player, eventID, vid);
            }

            if (returnType == Recruit_CancelReturnType.buildinginfo)
            {
                return Upgrade_GetUpgradeInfo(vid, bid);
            }
            else
            {
                return GetVillage(vid);
            }
        }

        /// <summary>
        /// disbands units in given village by ID
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="data"></param>
        /// <returns>recruit info</returns>
        public string Disband(int vid, List<RecruitCommand> data)
        {
            Village village = _player.Village(vid);

            foreach (RecruitCommand cmd in data)
            {
                if (cmd.count > 0)
                {
                    village.Disband(_player.Realm.GetUnitTypesByID(cmd.uid), cmd.count);
                }
            }

            return GetVillage(vid);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="vid"></param>
        /// <returns>
        /// *** q *** is an array of object that is a list of recruiting govs. 
        ///     IF the object has a property "timeLeft" then it is the 
        ///         currently recruting gov and this represent the time left before it is done. 
        ///     IF this object has a property totalRecuitTime then this is a queued up recruitment of a gov.; 
        /// </returns>
        public string Gov_GetInfo(int vid)
        {
            Fbg.Bll.UnitType unit = _player.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Gov);

            return ApiHelper.RETURN_SUCCESS(new
            {
                govInfo = Gov_GetInfo_Raw(_player.Village(vid))
            });
        }

        //protected object Gov_GetInfo_Raw(int vid)
        //{
        //    Village village = _player.Village(vid);
        //    return Gov_GetInfo_Raw(village);
        //}

        protected object Gov_GetInfo_Raw(Village village)
        {
            Fbg.Bll.UnitType unit = _player.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Gov);

            int chestCostForNextGoverner = ((UnitTypeLord)unit).CostInChests(_player);
            int chestCost = unit.Cost(null);
            int foodNeeded = unit.Pop;
            int availableChests = _player.Chests;
            int maxChests = village.coins / chestCost;
            string chestIcon = "https://static.realmofempires.com/images/ChestIcon.png"; // TODO - why the hell is this here ??
            string chestIconThemeM = "https://static.realmofempires.com/images/icons/M_PF_Silver.png"; // TODO - why the hell is this here ??
            List<Object> q = new List<object>();

            List<UnitRecruitQEntry> qEntries;
            village.GetAllRecruitmentQEntries().TryGetValue(_player.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Palace), out qEntries);

            if (qEntries != null)
            {
                for (int i = 0; i < qEntries.Count; i++)
                {


                    if (qEntries[i] is UnitRecruitQEntry_First)
                    {
                        q.Add(new
                        {
                            timeLeft = ApiHelper.SerializeTimeSpan(((UnitRecruitQEntry_First)qEntries[i]).RecruitmentCompletedOn.Subtract(DateTime.Now))
                        });
                    }
                    else
                    {
                        q.Add(new
                        {
                            totalRecuitTime = ApiHelper.SerializeTimeSpan(qEntries[i].TotalRecruitTime)
                        });

                    }
                }
            }

            return new
            {
                isPalaceBuilt = village.GetBuildingLevel(Fbg.Bll.CONSTS.BuildingIDs.Palace) > 0 ?  true: false,
                chestCostForNextGoverner,
                recruitTime = ApiHelper.SerializeTimeSpan(unit.RecruitmentTime()),
                chestCost,
                foodNeeded,
                availableChests,
                maxChests,
                currentCoinsInVillage = village.coins,
                govsAtThisVillage = village.GetVillageUnit(_player.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Gov)).YourUnitsTotalCount,
                chestIcon,
                chestIconThemeM,
                q
            };
        }

        public string Gov_CancelRecruit(int vid)
        {
            Fbg.Bll.UnitRecruitQEntry.CancelGovernerRecruit(_player, vid);

            return ApiHelper.RETURN_SUCCESS(new
            {
                govInfo = Gov_GetInfo_Raw(_player.Village(vid))
            });
        }


        /// <summary>
        /// buy a max number of chests possible in the village
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="numChests"></param>
        /// <returns></returns>
        public string Gov_BuyChests(int vid, bool returnRecruitInfoInsteadOfJustGovInfo)
        {
            Village village = _player.Village(vid);
            int silverToLeave = 0; //hmm doesnt seem to do anything?
                       
            int chestCost;
            chestCost = _player.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Gov).Cost(null);

            int numChestsToBuy = (village.coins - silverToLeave) / chestCost;

            return Gov_BuyChests(village, numChestsToBuy, returnRecruitInfoInsteadOfJustGovInfo);
        }

        /// <summary>
        /// leave an amount of silver in village, then spend the rest on chests
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="numChests"></param>
        /// <returns></returns>
        public string Gov_BuyChests_LeaveSilver(int vid, int silverToLeave, bool returnRecruitInfoInsteadOfJustGovInfo)
        {
            Village village = _player.Village(vid);

            int chestCost;
            chestCost = _player.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Gov).Cost(null);

            int numChestsToBuy = (village.coins - silverToLeave) / chestCost;

            return Gov_BuyChests(village, numChestsToBuy, returnRecruitInfoInsteadOfJustGovInfo);
        }


        /// <summary>
        /// buy a specific number of chests
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="numChests"></param>
        /// <returns></returns>
        public string Gov_BuyChests(int vid, int numChests, bool returnRecruitInfoInsteadOfJustGovInfo)
        {
            Village village = _player.Village(vid);
            return Gov_BuyChests(village, numChests, returnRecruitInfoInsteadOfJustGovInfo);
        }


        public string Gov_BuyChests(Village village, int numChests, bool returnRecruitInfoInsteadOfJustGovInfo)
        {
            bool wasBuySuccessful = false;

            if (village.GetBuildingLevel(Fbg.Bll.CONSTS.BuildingIDs.Palace) > 0)
            {
                wasBuySuccessful = _player.BuyChests(numChests, village);
            }

            if (returnRecruitInfoInsteadOfJustGovInfo)
            {
                return GetVillage(village.id);
            }
            else
            {
                return ApiHelper.RETURN_SUCCESS(new
                {
                    wasBuySuccessful,
                    govInfo = Gov_GetInfo_Raw(_player.Village(village.id))
                });
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="vid"></param>
        /// <returns>
        ///  **** resultCode *** 
        ///      OK = 0 
        ///      FAILED_NoFood = 1
        ///      FAILED_NoChests = 2 
        ///      FAILED_NoPalace = 3
        /// </returns>
        public string Gov_DoRecruit(int vid)
        {
            Village village = _player.Village(vid);

            Village.RecruitGovernerResult result = village.RecruitGoverner2();

            return ApiHelper.RETURN_SUCCESS(new
            {
                resultCode = result,
                govInfo = Gov_GetInfo_Raw(_player.Village(vid))
            });
        }



        /// <summary>
        /// </summary>
        /// <param name="bt"></param>
        /// <param name="village"></param>
        /// <returns>
        /// { 
        ///     type : 0 (cur level info) | 1 (misc text) | 2 (text with value) | 3 (research info) | 4 (total effect) | 
        ///         5 (pf bonus) | 6 bonus village | 7 warning text | 8 treasury full | 9 treasury not full 
        /// 
        /// }
        /// </returns>
        private Dictionary<string, string> GetEffectInfo(BuildingType bt, Village village, BuildingTypeLevel curBuildingTypeLevel)
        {
            Dictionary<string, string> entries = new Dictionary<string, string>(5);


            switch (bt.ID)
            {
                case Fbg.Bll.CONSTS.BuildingIDs.VillageHQ:
                    GetEffectInfo_HelperRecruitTime(bt, village, curBuildingTypeLevel, entries);
                    break;
                case Fbg.Bll.CONSTS.BuildingIDs.HidingSpot:
                    if (curBuildingTypeLevel != null)
                    {
                        GetEffectInfo_Helper(bt, village, curBuildingTypeLevel, entries, false, 0);
                    }
                    break;
                case Fbg.Bll.CONSTS.BuildingIDs.CoinMine:
                    GetEffectInfo_Helper(bt, village, curBuildingTypeLevel, entries, _player.PF_HasPF(Fbg.Bll.CONSTS.PFs.SilverBonus), Fbg.Bll.CONSTS.PF_SilverBonusPercent);
                    break;
                case Fbg.Bll.CONSTS.BuildingIDs.TradePost:
                    GetEffectInfo_Helper(bt, village, curBuildingTypeLevel, entries, false, 0);
                    break;
                case Fbg.Bll.CONSTS.BuildingIDs.Treasury:
                    GetEffectInfo_Helper(bt, village, curBuildingTypeLevel, entries, false, 0);

                    int roomInTreasury = village.TreasurySize - village.coins;
                    if (roomInTreasury > 0)
                    {
                        float hoursTillFull = roomInTreasury / village.PerHourCoinIncome;
                        TimeSpan time = new TimeSpan(Convert.ToInt64(Math.Round(hoursTillFull * TimeSpan.TicksPerHour)));

                        entries.Add("timeTillTreasuryFull", utils.FormatDuration(time));

                        //entries.Add(new
                        //{
                        //    type = 9,
                        //    timeTillFull = helper.SerializeTimeSpan(time)
                        //});
                    }
                    else
                    {
                        entries.Add("timeTillTreasuryFull", "0");
                    }

                    break;
                case Fbg.Bll.CONSTS.BuildingIDs.Farmland:
                    GetEffectInfo_Helper(bt, village, curBuildingTypeLevel, entries, false, 0);
                    break;
                case Fbg.Bll.CONSTS.BuildingIDs.Barracks:
                case Fbg.Bll.CONSTS.BuildingIDs.Stable:
                case Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop:
                case Fbg.Bll.CONSTS.BuildingIDs.Tavern:
                case Fbg.Bll.CONSTS.BuildingIDs.Palace:
                    GetEffectInfo_HelperRecruitTime(bt, village, curBuildingTypeLevel, entries);
                    break;
                case Fbg.Bll.CONSTS.BuildingIDs.Wall:
                case Fbg.Bll.CONSTS.BuildingIDs.DefenseTower:
                    GetEffectInfo_Helper_Defense(village, entries);
                    break;
            }

            return entries;
        }


        private void GetEffectInfo_Helper_Defense(Village village, Dictionary<string, string> entries)
        {
            bool hasDefenceBonusPF = _player.PF_HasPF(Fbg.Bll.CONSTS.PFs.DefenseBonus);
            double wallBonus = village.GetBuildingLevelObject(Fbg.Bll.CONSTS.BuildingIDs.Wall) == null ? 0 : Convert.ToDouble(village.GetBuildingLevelObject(Fbg.Bll.CONSTS.BuildingIDs.Wall).Effect) - 100;
            double towerBonus = village.GetBuildingLevelObject(Fbg.Bll.CONSTS.BuildingIDs.DefenseTower) == null ? 0 : Convert.ToDouble(village.GetBuildingLevelObject(Fbg.Bll.CONSTS.BuildingIDs.DefenseTower).Effect) - 100;
            double villageBonus = village.VillageType.Bonus_DefenceFactor();
            double researchBonus = village.owner.MyResearch.Bonus_DefenceFactor();
            double possiblePFBonus;
            double actualPFBonus;
            if (!_player.Realm.IsVPrealm)
            {
                possiblePFBonus = 0;
                actualPFBonus = 0;
            }
            else
            {
                possiblePFBonus = Fbg.Bll.CONSTS.PF_DefenceBonusPercent * 100;
                actualPFBonus = hasDefenceBonusPF ? Fbg.Bll.CONSTS.PF_DefenceBonusPercent * 100 : 0;
            }

            double totalBonus = (wallBonus + towerBonus) * (researchBonus + 1) * (villageBonus + 1) * (1 + actualPFBonus / 100);
            BuildingType wall = _player.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Wall);
            BuildingType tower = _player.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.DefenseTower);
            double maxBonusWall = Convert.ToDouble(wall.Level(wall.MaxLevel).Effect) - 100;
            double maxBonustower = Convert.ToDouble(tower.Level(tower.MaxLevel).Effect) - 100;
            double maxBonusResearch = _player.Realm.Research.MaxBonus_DefenceFactor();

            double maxBonus = (maxBonusWall + maxBonustower) * (maxBonusResearch + 1) * (villageBonus + 1) * (1 + possiblePFBonus / 100);

            //// order of these entries is very important! it is assume that the wall always comes before tower
            entries.Add("levelW_effect", String.Format("{0:N}", wallBonus));
            entries.Add("levelW_max", String.Format("{0:N}", maxBonusWall));
            entries.Add("levelW_perc", GetPercentageComplete(wallBonus, maxBonusWall).ToString());

            entries.Add("levelT_effect", String.Format("{0:N}", towerBonus));
            entries.Add("levelT_max", String.Format("{0:N}", maxBonustower));
            entries.Add("levelT_perc", GetPercentageComplete(towerBonus, maxBonustower).ToString());


            if (possiblePFBonus > 0)
            {
                entries.Add("pf_effect", String.Format("{0:0}", actualPFBonus));
                entries.Add("pf_max", String.Format("{0:N}", possiblePFBonus));
                entries.Add("pf_perc", GetPercentageComplete(actualPFBonus, possiblePFBonus).ToString());

            }
            entries.Add("res_effect", String.Format("{0:0}", researchBonus * 100));
            entries.Add("res_max", String.Format("{0:N}", maxBonusResearch * 100));
            entries.Add("res_perc", GetPercentageComplete(researchBonus, maxBonusResearch).ToString());

            if (villageBonus > 0)
            {
                entries.Add("bonus_effect", Convert.ToInt32((villageBonus * 100)).ToString());

            }

            entries.Add("total_effect", String.Format("{0:N}", totalBonus));
            entries.Add("total_max", String.Format("{0:N}", maxBonus));
            entries.Add("total_perc", GetPercentageComplete(totalBonus, maxBonus).ToString());

        }



        private void GetEffectInfo_Helper(BuildingType bt, Village village, BuildingTypeLevel curBuildingTypeLevel, Dictionary<string, string> entries, bool hasPF, float maxPFBonus)
        {
            float maxResearchBonus = 0.0F;
            float curResearchBonus = 0.0F;

            int totalMax = 0;
            int totalCurrent = 0;
            //
            // level
            //
            totalCurrent += curBuildingTypeLevel == null ? 0 : curBuildingTypeLevel.EffectAsInt;
            totalMax += bt.Level(bt.MaxLevel).EffectAsInt;
            entries.Add("level_effect", curBuildingTypeLevel == null ? "0" : curBuildingTypeLevel.EffectFormatted);
            entries.Add("level_max", bt.Level(bt.MaxLevel).EffectFormatted);
            entries.Add("level_perc", GetPercentageComplete(curBuildingTypeLevel == null ? 0 : curBuildingTypeLevel.EffectAsInt, bt.Level(bt.MaxLevel).EffectAsInt).ToString());

            //
            // research
            //
            maxResearchBonus = village.owner.Realm.Research.MaxBonus(bt);
            curResearchBonus = village.owner.MyResearch.Bonus(bt);
            totalCurrent += Convert.ToInt32(totalCurrent * curResearchBonus);
            totalMax += Convert.ToInt32(totalMax * maxResearchBonus);
            if (maxResearchBonus > 0)
            {
                entries.Add("res_effect", String.Format("{0:0}", curResearchBonus * 100));
                entries.Add("res_max", String.Format("{0:N}", maxResearchBonus * 100));
                entries.Add("res_perc", GetPercentageComplete(curResearchBonus, maxResearchBonus).ToString());

            }
            //
            // PF bonus
            //             
            if (maxPFBonus > 0)
            {
                float bonusAmount = hasPF ? maxPFBonus : 0;
                totalCurrent += Convert.ToInt32(totalCurrent * (bonusAmount));
                totalMax += Convert.ToInt32(totalMax * (maxPFBonus));
                entries.Add("pf_effect", String.Format("{0:0}", bonusAmount));
                entries.Add("pf_max", String.Format("{0:N}", maxPFBonus));
                entries.Add("pf_perc", GetPercentageComplete(bonusAmount, maxPFBonus).ToString());

            }
            //
            // bonus village
            //
            float bonusFromVillageType = village.VillageType.Bonus(bt);

            if (bonusFromVillageType > 0)
            {
                totalCurrent += Convert.ToInt32(totalCurrent * bonusFromVillageType);
                totalMax += Convert.ToInt32(totalMax * bonusFromVillageType);
                entries.Add("bonus_effect", String.Format("{0:N}", bonusFromVillageType));

            }
            //
            // totals
            //
            entries.Add("total_effect", utils.FormatCost(totalCurrent));
            entries.Add("total_max", utils.FormatCost(totalMax));
            entries.Add("total_perc", GetPercentageComplete(totalCurrent, totalMax).ToString());

        }


        private void GetEffectInfo_HelperRecruitTime(BuildingType bt, Village village, BuildingTypeLevel curBuildingTypeLevel, Dictionary<string, string> entries)
        {
            float maxResearchBonus = 0.0F;
            float curResearchBonus = 0.0F;

            float totalMax = 0;
            float totalCurrent = 0;
            //
            // level
            //
            float curLevelEffect;
            float maxLevelEffect;
            if (curBuildingTypeLevel == null)
            {
                //
                // if this village has no recruitment building, then assume 100% recruitment factor
                //  this may happen in case of a recruitment building begin destroyed but 
                //  some units are still recruiting. 
                //
                curLevelEffect = 100;
            }
            else
            {
                curLevelEffect = Convert.ToSingle(curBuildingTypeLevel.Effect);
            }
            maxLevelEffect = bt.Level(bt.MaxLevel).EffectAsInt;
            totalCurrent = curLevelEffect;
            totalMax = maxLevelEffect;

            entries.Add("level_effect", String.Format("{0:N}", curLevelEffect));
            entries.Add("level_max", String.Format("{0:N}", maxLevelEffect));
            entries.Add("level_perc", GetPercentageComplete(100 - curLevelEffect, 100 - maxLevelEffect).ToString());
            //
            // research
            //
            maxResearchBonus = village.owner.Realm.Research.MaxBonus(bt);
            curResearchBonus = village.owner.MyResearch.Bonus(bt);
            if (maxResearchBonus > 0)
            {
                entries.Add("res_effect", String.Format("{0:0}", curResearchBonus * 100));
                entries.Add("res_max", String.Format("{0:N}", maxResearchBonus * 100));
                entries.Add("res_perc", GetPercentageComplete(curResearchBonus, maxResearchBonus).ToString());
            }
            //
            // bonus village
            //
            float bonusFromVillageType = village.VillageType.Bonus(bt);

            if (bonusFromVillageType > 0)
            {
                entries.Add("bonus_effect", String.Format("{0:N}", bonusFromVillageType));               
            }
            totalCurrent = totalCurrent / ((1 + bonusFromVillageType) * (1 + curResearchBonus) * 100) * 100;
            totalMax = totalMax / ((1 + bonusFromVillageType) * (1 + maxResearchBonus) * 100) * 100;

            //
            // totals
            //
            entries.Add("total_effect", String.Format("{0:N}", totalCurrent));
            entries.Add("total_max", String.Format("{0:N}", totalMax));
            entries.Add("total_perc", GetPercentageComplete(100 - totalCurrent, 100 - totalMax).ToString());

        }





        static private int GetPercentageComplete(int cur, int max)
        {
            return GetPercentageComplete((float)cur, (float)max);
        }

        static private int GetPercentageComplete(float cur, float max)
        {
            if (max > 0)
            {
                return Convert.ToInt32(Math.Ceiling((Convert.ToDouble(cur) / max) * 100));
            }
            else
            {
                return 100;
            }
        }
        static private int GetPercentageComplete(double cur, double max)
        {
            if (max > 0)
            {
                return Convert.ToInt32(Math.Ceiling((Convert.ToDouble(cur) / max) * 100));
            }
            else
            {
                return 100;
            }
        }




        public class VillageBuilding
        {
            public int id { get; set; }
            public int level { get; set; }
            public string image { get; set; }
            public string[] images { get; set; }
            public string image_c { get; set; }
            public bool built { get; set; }
            public bool cnstr { get; set; }
            public string buildcount { get; set; }
            public string recruitcount { get; set; }
            public bool areRequirementsSatisfied { get; set; }
        }
        public  object GetVOVInfo(Village village)
        {
            DateTime now = DateTime.UtcNow.AddHours(_player.User.TimeZone);

            // hack : night mode only for europe theme for now
            TimeOfDay _timeOfDay = TimeOfDay.day;
            if (village.owner.Realm.Theme == Realm.Themes.europe || village.owner.Realm.Theme == Realm.Themes.highlands)
            {
                _timeOfDay = now.Hour > 6 && now.Hour < 22 ? TimeOfDay.day : TimeOfDay.night;
            }

            var levelTowers = village.GetBuildingLevel(Fbg.Bll.CONSTS.BuildingIDs.DefenseTower);
            var levelWall = village.GetBuildingLevel(Fbg.Bll.CONSTS.BuildingIDs.Wall);
            var villageImages = new VillageOverviewImages(levelTowers, levelWall);
            var villageBackgroundLevel = villageImages.GetVillageBackgroundIndex(village.Points);
            bool isCity = village.isCity();

            Func<UInt32, Func<UInt16>> makeLCG = delegate(UInt32 seed)
            {
                // We use a linear congruential generator to generate random numbers, to decide which
                // animations will show up. The seed is mixed up a bit, since otherwise, small changes
                // in the low digits of the seed would correspond closely to small changes in the
                // generator. This isn't anything cryptographically-based, it's just to shift around
                // the low digits in a way that "looks" random, which is good enough for our purposes.
                UInt32 multiplier = 1664525;
                UInt32 increment = 1013904223;
                UInt32 state = seed ^ (seed >> 5 + 0x358259) ^ (seed << 10 + 0x2585a1);
                return delegate()
                {
                    state = multiplier * state + increment;
                    // We only use the high 16 bits because they have better randomness properties.
                    return (UInt16)(state >> 16);
                };
            };

            // Generates a boolean from the generator that is true chance% of the time, false (100-chance)% of the time. 
            Func<Func<UInt16>, float, bool> probability = delegate(Func<UInt16> generator, float chance)
            {
                return (float)generator() <= (chance * 0xFFFF / 100.0f);
            };

            // Generators seeded with specific values. Note, if you generate more random numbers between the ones
            // that already exist (by adding new animations, or whatever) it will change the random numbers that
            // are generated afterward. This might be sad for, for example, a player who enjoyed that the lute
            // player always showed up at 5 o'clock in his favourite village -- now he doesn't, because you
            // threw off the state of the RNG with the newest server update. So if you want to add in more
            // animations, I recommend adding them after all the animations that already exist, and things will
            // work as they always have.
            var byVillage = makeLCG((UInt32)village.id);
            var byHour = makeLCG((UInt32)(village.id ^ now.Hour));
            var byDay = makeLCG((UInt32)(village.id ^ now.DayOfYear));

            // We generate all of the probabilities up front because generating them conditionally
            // can cause previous conditions to change subsequent conditions. E.g., a village that
            // does not normally have a preacher might end up having a preacher if the byCity generator
            // isn't incremented in the previous block.
            // Don't use short-circuit boolean operators like && and || here! They can do the same thing.
            #region civilians
            var showChickens = probability(byHour, 100);
            var showCiviliansChat = probability(byHour, 75);
            var showSittingAtWell = probability(byHour, 60);
            var showLutePlayer = probability(byHour, 20);
            var townHasPreacher = probability(byVillage, 90);
            var showPreacher = probability(byHour, 80);
            var townHasGreyD = probability(byVillage, 70);
            var showGreyD = probability(byHour, 80);
            var townHasBrownS = probability(byVillage, 70);
            var showBrownS = probability(byHour, 80);
            var townHasGreyS = probability(byVillage, 70);
            var showGreyS = probability(byHour, 80);
            var townHasPurpleS = probability(byVillage, 70);
            var showPurpleS = probability(byHour, 80);
            var showRidgeCivilianReds = probability(byHour, 40);
            var showGuyByWater = probability(byHour, 40);
            var showBonfire = probability(byDay, 50);
            if (_timeOfDay == TimeOfDay.day)
            {
                if (showChickens) animations.Add(new VOVAnimationInfo() { type = VOVAnimations.chickens01, stage = 1 });
                if (showCiviliansChat) animations.Add(new VOVAnimationInfo() { type = VOVAnimations.civiliansChat, stage = 1 });
                if (villageBackgroundLevel >= 3)
                {
                    if (!isCity)
                    {
                        if (showSittingAtWell) animations.Add(new VOVAnimationInfo() { type = VOVAnimations.sitting_at_well, stage = 1 });
                    }
                    if (showLutePlayer) animations.Add(new VOVAnimationInfo() { type = VOVAnimations.lute_player, stage = 1 });
                }
                if (villageBackgroundLevel >= 6 && townHasPreacher && showPreacher)
                {
                    // Uncomment this conditional if you decide that you don't want the preacher
                    // to show up when there's no one there to listen to him. At the moment, the
                    // creative decision is that it is hilarious. So he still shows up.
                    // if ((townHasGreyD && showGreyD) || (townHasBrownS && showBrownS) || (townHasGreyS && showGreyS) || (townHasPurpleS && showPurpleS))
                    // {
                    animations.Add(new VOVAnimationInfo() { type = VOVAnimations.preacher, stage = 1 });
                    // }
                    // Grey dress should appear behind brown shirt; sending them in this order is the easiest way.
                    if (townHasGreyD && showGreyD) animations.Add(new VOVAnimationInfo() { type = VOVAnimations.follower_grey_d, stage = 1 });
                    if (townHasBrownS && showBrownS) animations.Add(new VOVAnimationInfo() { type = VOVAnimations.follower_brown_s, stage = 1 });
                    if (townHasGreyS && showGreyS) animations.Add(new VOVAnimationInfo() { type = VOVAnimations.follower_grey_s, stage = 1 });
                    if (townHasPurpleS && showPurpleS) animations.Add(new VOVAnimationInfo() { type = VOVAnimations.follower_purple_s, stage = 1 });
                }
                if (villageBackgroundLevel >= 7)
                {
                    if (!isCity)
                    {
                        if (showRidgeCivilianReds)
                        {
                            animations.Add(new VOVAnimationInfo() { type = VOVAnimations.ridge_civilian_red_d, stage = 1 });
                            animations.Add(new VOVAnimationInfo() { type = VOVAnimations.ridge_civilian_red_s, stage = 1 });
                        }

                        if (showGuyByWater) animations.Add(new VOVAnimationInfo() { type = VOVAnimations.guyByWater, stage = 1 });
                    }
                }
            }
            else if (!isCity)
            {
                if (villageBackgroundLevel >= 2)
                {
                    animations.Add(new VOVAnimationInfo() { type = VOVAnimations.torches, stage = 1 });
                }
                if (villageBackgroundLevel >= 5 && showBonfire)
                {
                    animations.Add(new VOVAnimationInfo() { type = VOVAnimations.bonfire, stage = 1 });
                }
            }
            #endregion

            var imgPack = village.ImagePack;
            var imgDirPrefix = VillageOverviewImages.ImageDirPrefix(_timeOfDay, village.owner.Realm);

            #region trees and troops
            Dictionary<string, string> trees = new Dictionary<string, string>(){
            {"imgTreesR_4e", imgPack.Image(imgDirPrefix + "TreesR_4e_10.png", imgDirPrefix + "TreesR_4e_10.png") },
            {"imgTreesL_4e", imgPack.Image(imgDirPrefix + "TreesL_4e_10.png", imgDirPrefix + "TreesL_4e_10.png") },
            {"imgTreesR_6e", imgDirPrefix + "TreesR_6e_10.png"},
            {"imgTreesL_6e", imgDirPrefix + "TreesL_6e_10.png"},
            {"imgTrees_2e", imgDirPrefix + "Trees_2e_10.png"},
            {"imgTrees_3e", imgPack.Image(imgDirPrefix + "TreesL_3e_10.png", imgDirPrefix + "TreesL_3e_10.png") },
            {"imgTrees_5e", imgDirPrefix + "TreesL_5e_10.png" }
        };

            var knight = _player.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Knight);
            var lc = _player.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.LC);

            UnitInVillage infantryInVillage = village.GetVillageUnit(village.owner.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Infantry));
            UnitInVillage cmInVillage = village.GetVillageUnit(village.owner.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.CM));
            UnitInVillage knightsInVillage = village.GetVillageUnit(village.owner.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Knight));
            UnitInVillage lcInVillage = village.GetVillageUnit(village.owner.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.LC));

            var _troopCount_CM = cmInVillage.TotalNowInVillageCount;
            var _troopCount_I = infantryInVillage.TotalNowInVillageCount;
            var _troopCount_K = knightsInVillage.TotalNowInVillageCount;
            var _troopCount_LC = lcInVillage.TotalNowInVillageCount;
            var _KbyPop = _troopCount_K * knight.Pop;
            var _LCByPop = _troopCount_LC * lc.Pop;

            if ((_troopCount_I +
                _troopCount_CM +
                _KbyPop +
                _LCByPop) > 1000)
            {
                if (levelWall == 9)
                {
                    trees.Add("imgWallBuildings", imgDirPrefix + "WallBuildings_A.png");
                }
                else if (levelWall == 10)
                {
                    trees.Add("imgWallBuildings", imgDirPrefix + "WallBuildings_B.png");
                }

                trees.Add("imgTroops4e_A", imgDirPrefix + "Troops4e_A.png");
            }

            if (_troopCount_I >= 1)
            {
                animations.Add(new VOVAnimationInfo() { type = VOVAnimations.chat, stage = 1 });
            }

            if (_troopCount_I > 18000)
            {
                trees.Add("imgTroops5e_A_L", imgDirPrefix + "Troops5e_A_L.png");
            }
            if (_troopCount_I > 9000)
            {
                trees.Add("imgTroops4e_B_L", imgDirPrefix + "Troops4e_B_L.png");
            }
            //
            // Knights
            //
            if (_LCByPop + _KbyPop >= 1)
            {
                animations.Add(new VOVAnimationInfo() { type = VOVAnimations.knightHorse, stage = 1 });
            }
            if (_LCByPop + _KbyPop > 9000)
            {
                trees.Add("imgKnights3e_A_R", imgDirPrefix + "Knights3e_A_R.png");
            }
            if (_LCByPop + _KbyPop > 14000)
            {
                trees.Add("imgKnights5e_A", imgDirPrefix + "Knights5e_A.png");
            }
            if (_LCByPop + _KbyPop > 18000)
            {
                trees.Add("imgKnights5e_B", imgDirPrefix + "Knights5e_B.png");
            }
            //
            // units on walls. 
            //
            if (levelWall == 7)
            {
                trees.Add("imgWallTroops", imgDirPrefix + "troopsWall7.png");
            }
            else if (levelWall == 8)
            {
                trees.Add("imgWallTroops", imgDirPrefix + "troopsWall8.png");
            }
            else if (levelWall >= 9)
            {
                trees.Add("imgWallTroops", imgDirPrefix + "troopsWall9.png");
            }

            #endregion

            bool hasWater = false;
            string bg = villageImages.GetVillageBackground(village.Points, _timeOfDay, imgPack, ref hasWater ,village.owner.Realm);

            List<VillageBuilding> villBuildings = new List<VillageBuilding>();
            List<UpgradeEvent> upgrades = village.GetAllUpgradingBuildings();

            bool areBuildingRequirementsSatisfied = false;
            BuildingTypeLevel btl;
            int level;
            foreach (BuildingType bt in _player.Realm.BuildingTypes)
            {
                string htmlMapAreaCords = null;
                string leftMapAreaCords = "";
                string rightMapAreaCords = "";
                int buildingStage = 0;

                btl = village.GetBuildingLevelObject(bt.ID);
                if (btl == null)
                {
                    level = 0;

                    areBuildingRequirementsSatisfied = bt.Level(1).GetUnsatisfiedRequirements(village).Count == 0;
                }
                else
                {
                    level = btl.Level;

                    areBuildingRequirementsSatisfied = btl.GetUnsatisfiedRequirements(village).Count == 0;
                }
                string im = "";
                string[] ims = null;
                string im_c = null;

                string buildcount = null;
                string recruitcount = null;

                bool built = true;
                bool cnstr = false;

                if (bt.ID == Fbg.Bll.CONSTS.BuildingIDs.Wall)
                {
                    ims = villageImages.GetWallImageUrl(level, _timeOfDay, out htmlMapAreaCords, village.owner.Realm);
                    im = ims[1];
                }
                else if (bt.ID == Fbg.Bll.CONSTS.BuildingIDs.DefenseTower)
                {
                    ims = villageImages.GetTowersImageUrl(level, _timeOfDay, out leftMapAreaCords, out rightMapAreaCords, village.owner.Realm);
                    im = ims[0];
                }
                else
                {
                    im = villageImages.GetBuildingImageUrl(bt, level, _timeOfDay, out htmlMapAreaCords,
                                    village.ImagePack, ref buildingStage, true, village.owner.Realm);
                }

                TimeSpan recruitingTime = TimeSpan.Zero;
                recruitingTime = village.GetBuildingWorkTimeRemaining(bt);
                if (recruitingTime > TimeSpan.Zero)
                {
                    //recruitcount = utils.FormatDuration(recruitingTime);
                    recruitcount = ApiHelper.SerializeDate(DateTime.Now + recruitingTime).ToString();

                    if (bt.ID == Fbg.Bll.CONSTS.BuildingIDs.Barracks)
                    {
                        animations.Add(new VOVAnimationInfo() { type = VOVAnimations.infantrySlash, stage = buildingStage });
                    }
                    else if (bt.ID == Fbg.Bll.CONSTS.BuildingIDs.Stable)
                    {
                        animations.Add(new VOVAnimationInfo() { type = VOVAnimations.hay_prod, stage = buildingStage });
                        animations.Add(new VOVAnimationInfo() { type = VOVAnimations.leatherArmorGuy, stage = buildingStage });

                        if (buildingStage >= 1)
                        {
                            animations.Add(new VOVAnimationInfo() { type = VOVAnimations.horseTail, stage = buildingStage });
                        }
                    }
                    else if (bt.ID == Fbg.Bll.CONSTS.BuildingIDs.Tavern && !isCity)
                    {
                        animations.Add(new VOVAnimationInfo() { type = VOVAnimations.crone, stage = 1 });
                    }
                }

                if (upgrades.Count > 0 && upgrades[0].upgradeToLevel.Building == bt)
                {
                    im_c = im.Replace("anim_", "").Replace(".png", "U.png");

                    //upgradeTime = upgrades[0].completionTime.Subtract(DateTime.Now);

                    /*
                    if (upgradeTime > TimeSpan.Zero){
                        buildcount = utils.FormatDuration(upgradeTime);                     
                    }
                    */

                    buildcount = ApiHelper.SerializeDate(upgrades[0].completionTime).ToString();

                    cnstr = true;
                }

                if (!cnstr && level == 0)
                {
                    built = false;
                    buildingStage = 0;
                }

                #region Building animation
                if (bt is BuildingTypeHQ)
                {
                    animations.Add(new VOVAnimationInfo() { type = VOVAnimations.flag01, stage = buildingStage });
                    if (buildingStage == 3)
                    {
                        animations.Add(new VOVAnimationInfo() { type = VOVAnimations.flag02, stage = buildingStage });
                        animations.Add(new VOVAnimationInfo() { type = VOVAnimations.flag03, stage = buildingStage });
                    }
                }
                else if (bt.ID == Fbg.Bll.CONSTS.BuildingIDs.Barracks)
                {
                    animations.Add(new VOVAnimationInfo() { type = VOVAnimations.barracks_sign, stage = buildingStage });
                }
                else if (bt.ID == Fbg.Bll.CONSTS.BuildingIDs.Stable)
                {
                    animations.Add(new VOVAnimationInfo() { type = VOVAnimations.stables_sign, stage = buildingStage });
                }
                else if (bt.ID == Fbg.Bll.CONSTS.BuildingIDs.Tavern)
                {
                    animations.Add(new VOVAnimationInfo() { type = VOVAnimations.tavern_sign, stage = buildingStage });
                    if (buildingStage < 3)
                    {
                        animations.Add(new VOVAnimationInfo() { type = VOVAnimations.tavern_glow_1, stage = buildingStage });
                    }
                    else if (buildingStage < 5)
                    {
                        animations.Add(new VOVAnimationInfo() { type = VOVAnimations.tavern_glow_3, stage = buildingStage });
                    }
                    else
                    {
                        animations.Add(new VOVAnimationInfo() { type = VOVAnimations.tavern_glow_5, stage = buildingStage });
                    }
                }
                else if (bt.ID == Fbg.Bll.CONSTS.BuildingIDs.Treasury)
                {
                    animations.Add(new VOVAnimationInfo() { type = VOVAnimations.treasury_sign, stage = buildingStage });
                }
                else if (bt.ID == Fbg.Bll.CONSTS.BuildingIDs.Farmland)
                {
                    animations.Add(new VOVAnimationInfo() { type = VOVAnimations.scarecrow, stage = 1 });
                }
                #endregion

                villBuildings.Add(new VillageBuilding
                {
                    id = bt.ID,
                    image = im,
                    images = ims,
                    image_c = im_c,
                    built = built,
                    level = level,
                    cnstr = cnstr,
                    buildcount = buildcount,
                    recruitcount = recruitcount,
                    areRequirementsSatisfied = areBuildingRequirementsSatisfied
                });
            }

            List<VOVAnimationInfo> animations2 = new List<VOVAnimationInfo>();

            for (int i = 0; i < animations.Count; i++)
            {
                VOVAnimationInfo ai = animations[i];
                if (ai.stage > 0)
                {
                    var loc = GetAnimLocation(ai.type, ai.stage);
                    ai.x = loc.X; ai.y = loc.Y;

                    animations2.Add(ai);
                }
            }

            return
                    new
                    {
                        isday = _timeOfDay == TimeOfDay.day ? 1 : 0,
                        bg = bg,
                        buildings = villBuildings,
                        animations = animations2.Select(a => new { type = a.type.ToString(), x = a.x, y = a.y }),
                        trees = trees
                    };
            
        }



        #region Village Detail animations

        public enum VOVAnimations
        {
            MonsterARun,
            hqFlags,
            barracks_sign,
            chickens01,
            guyByWater,
            stables_sign,
            treasury_sign,
            tavern_sign,
            scarecrow,
            civiliansChat,
            knightHorse,
            chat,
            infantrySlash,
            hay_prod,
            horseTail,
            leatherArmorGuy,
            flag01,
            flag02,
            flag03,
            bonfire,
            tavern_glow_1,
            tavern_glow_3,
            tavern_glow_5,
            torches,
            ridge_civilian_red_d,
            ridge_civilian_red_s,
            sitting_at_well,
            lute_player,
            preacher,
            follower_brown_s,
            follower_grey_d,
            follower_grey_s,
            follower_purple_s,
            crone
        }

        public class VOVAnimationInfo
        {
            public VOVAnimations type;
            public int stage;
            public int x;
            public int y;
        }

        List<VOVAnimationInfo> animations = new List<VOVAnimationInfo>();

        private System.Drawing.Point GetAnimLocation(VOVAnimations type, int stage)
        {
            System.Drawing.Point loc = new System.Drawing.Point();
            switch (type)
            {
                case VOVAnimations.barracks_sign:
                    switch (stage)
                    {
                        case 1: loc.X = 150; loc.Y = 303; break;
                        case 2: loc.X = 177; loc.Y = 306; break;
                        case 3: loc.X = 181; loc.Y = 310; break;
                        default: loc.X = 200; loc.Y = 200; break;
                    }
                    break;
                case VOVAnimations.stables_sign:
                    switch (stage)
                    {
                        case 1: loc.X = 117; loc.Y = 200; break;
                        case 2: loc.X = 119; loc.Y = 196; break;
                        case 3: loc.X = 117; loc.Y = 197; break;
                        default: loc.X = 200; loc.Y = 200; break;
                    }
                    break;

                case VOVAnimations.chickens01: loc.X = 60; loc.Y = 260; break;
                case VOVAnimations.guyByWater: loc.X = 333; loc.Y = 279; break;
                case VOVAnimations.tavern_sign:
                    switch (stage)
                    {
                        case 1: loc.X = 239; loc.Y = 330; break;
                        case 2: loc.X = 239; loc.Y = 330; break;
                        case 3: loc.X = 239; loc.Y = 330; break;
                        default: loc.X = 200; loc.Y = 200; break;
                    }
                    break;
                case VOVAnimations.treasury_sign:
                    switch (stage)
                    {
                        case 1: loc.X = 324; loc.Y = 193; break;
                        case 2: loc.X = 342; loc.Y = 196; break;
                        case 3: loc.X = 343; loc.Y = 183; break;
                        default: loc.X = 200; loc.Y = 200; break;
                    }
                    break;
                case VOVAnimations.scarecrow: loc.X = 406; loc.Y = 114; break;
                case VOVAnimations.civiliansChat: loc.X = 250; loc.Y = 198; break;
                case VOVAnimations.knightHorse: loc.X = 311; loc.Y = 337; break;
                case VOVAnimations.chat: loc.X = 307; loc.Y = 207; break;
                case VOVAnimations.flag01:
                    switch (stage)
                    {
                        case 1: loc.X = 235; loc.Y = 99; break;
                        case 2: loc.X = 236; loc.Y = 80; break;
                        case 3: loc.X = 162; loc.Y = 76; break;
                        default: loc.X = 200; loc.Y = 200; break;
                    }
                    break;
                case VOVAnimations.flag02: loc.X = 201; loc.Y = 75; break;
                case VOVAnimations.flag03: loc.X = 246; loc.Y = 120; break;
                case VOVAnimations.infantrySlash:
                    switch (stage)
                    {
                        case 1: loc.X = 144; loc.Y = 335; break;
                        case 2: loc.X = 151; loc.Y = 335; break;
                        case 3: loc.X = 149; loc.Y = 345; break;
                        default: loc.X = 200; loc.Y = 200; break;
                    }

                    break;
                case VOVAnimations.hay_prod:
                    switch (stage)
                    {
                        case 1: loc.X = 76; loc.Y = 233; break;
                        case 2: loc.X = 84; loc.Y = 234; break;
                        case 3: loc.X = 89; loc.Y = 231; break;
                        default: loc.X = 200; loc.Y = 200; break;
                    }
                    break;
                case VOVAnimations.horseTail:
                    switch (stage)
                    {
                        case 2: loc.X = 83; loc.Y = 213; break;
                        case 3: loc.X = 72; loc.Y = 215; break;
                        default: loc.X = 200; loc.Y = 200; break;
                    }
                    break;
                case VOVAnimations.leatherArmorGuy:
                    switch (stage)
                    {
                        case 1: loc.X = 105; loc.Y = 211; break;
                        case 2: loc.X = 106; loc.Y = 225; break;
                        case 3: loc.X = 99; loc.Y = 225; break;
                        default: loc.X = 200; loc.Y = 200; break;
                    }
                    break;
                case VOVAnimations.bonfire: loc.X = 140; loc.Y = 350; break;
                case VOVAnimations.tavern_glow_1: loc.X = 223; loc.Y = 260; break;
                case VOVAnimations.tavern_glow_3: loc.X = 223; loc.Y = 260; break;
                case VOVAnimations.tavern_glow_5: loc.X = 223; loc.Y = 260; break;
                case VOVAnimations.torches: loc.X = 139; loc.Y = 191; break;
                case VOVAnimations.ridge_civilian_red_s: loc.X = 162; loc.Y = 221; break;
                case VOVAnimations.ridge_civilian_red_d: loc.X = 183; loc.Y = 214; break;
                case VOVAnimations.sitting_at_well: loc.X = 233; loc.Y = 213; break;
                case VOVAnimations.lute_player: loc.X = 174; loc.Y = 374; break;
                case VOVAnimations.preacher: loc.X = 281; loc.Y = 233; break;
                case VOVAnimations.follower_brown_s: loc.X = 262; loc.Y = 248; break;
                case VOVAnimations.follower_grey_d: loc.X = 262; loc.Y = 231; break;
                case VOVAnimations.follower_grey_s: loc.X = 297; loc.Y = 251; break;
                case VOVAnimations.follower_purple_s: loc.X = 278; loc.Y = 250; break;
                case VOVAnimations.crone: loc.X = 388; loc.Y = 298; break;
                default: loc.X = 150; loc.Y = 50; break;
            }

            return loc;
        }

        #endregion
    }
}
