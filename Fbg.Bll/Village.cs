using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

using Gmbc.Common.Diagnostics.ExceptionManagement;

using Fbg.Common;

namespace Fbg.Bll
{
    public class Village : VillageBasicB, ISerializableToNameValueCollection
    {
        new internal class CONSTS
        {
            public class TableIndex
            {
                public static int Buildings = 0;
                public static int Upgrades = 1;
                public static int UpgradesQ = 2;
                public static int MaxPop = 3;
                public static int CurPop = 4;
                public static int Units = 5;
                public static int SupportUnits = 6;
                public static int UnitsInRecruitQ = 7;
                public static int IncomingUnits = 8;
                public static int OutgoingUnits = 9;
                public static int AreCoinTransportsAvail = 10;
                public static int VillageBasicInfo = 11;
                public static int Tags = 12;
                public static int Downgrades = 13;
                public static int DowngradesQ = 14;
                public static int SpeedupsAllowed = 15;
            }


            public class BuildingsColumnIndex
            {
                public static int VillageID = 0;
                public static int Name = 1;
                public static int BuildingTypeID = 2;
                public static int BuildingLevel = 3;
            }
            public class UpgradesColumnIndex
            {
                public static int EventID = 0;
                public static int EventTime = 1;
                public static int BuildingTypeID = 2;
                public static int BuildingLevel = 3;
            }
            public class UpgradesQColumnIndex
            {
                public static int QEntryID = 0;
                public static int BuildingTypeID = 1;
                public static int UpgradeLevel = 6;
            }
            public class UpgradesQColumnNames
            {
                public static string QEntryID = "EntryID";
                public static string BuildingTypeID = "BuildingTypeID";
                public static string UpgradeLevel = "UpgradeLevel";
            }

            public class UpgradesColumnName
            {
                public static string EventID = "EventID";
                public static string EventTime = "EventTime";
                public static string BuildingTypeID = "BuildingTypeID";
                public static string BuildingLevel = "Level";
            }
            public class UnitRecruitQEntreyColumnName
            {
                public static string EntryID = "EntryID";
                public static string UnitTypeID = "UnitTypeID";
                public static string BuildingTypeID = "BuildingTypeID";
                public static string Count = "Count";
                public static string UnitCost = "UnitCost";
            }
            public class UnitRecruitQEntreyColumnIndexes
            {
                public static int EntryID = 0;
                public static int UnitTypeID = 1;
                public static int BuildingTypeID = 2;
                public static int Count = 3;
                public static int TotalPop = 4;
                public static int UnitCost = 5;
                public static int DateStarted = 6;
            }

            public class UnitsColumnIndex
            {
                public static int UnitTypeId = 0;
                public static int TotalCount = 1;
                public static int CurrentCount = 2;
            }
            public class UnitsColumnName
            {
                public static string UnitTypeId = "UnitTypeID";
                public static string TotalCount = "TotalCount";
                public static string CurrentCount = "CurrentCount";

            }
            public class SuppportUnitsColumnIndex
            {
                public static int UnitTypeId = 0;
                public static int Count = 1;
            }
            public class SuppportUnitsColumnName
            {
                public static string UnitTypeId = "UnitTypeID";
            } 

            public class IncomingUnitsColumnIndex
            {
                public static int EventID = 0;
                public static int OriginVillageID = 1;
                public static int DestinationVillageID = 2;
                public static int CommandType = 3;
                public static int EventTime = 4;
                public static int VillageName = 5;
                public static int PlayerName = 6;
                public static int PlayerID = 7;
            } 
            public class OutgoingUnitsColumnIndex
            {
                public static int EventID = 0;
                public static int OriginVillageID = 1;
                public static int DestinationVillageID = 2;
                public static int CommandType = 3;
                public static int EventTime = 4;
                public static int VillageName = 5;
                public static int PlayerName = 6;
                public static int PlayerID = 7;
            }
            public class VillageBasicInfo
            {
                public static int Name = 0;
                public static int Coins = 1;
                public static int Points = 2;
                public static int XCord = 3;
                public static int YCord = 4;
                public static int loyalty = 5;
                public static int CoinsLastUpdates = 6;
                public static int VillageTypeID = 7;
            }
            public class DowngradesColumnIndex
            {
                public static int EventID = 0;
                public static int EventTime = 1;
                public static int BuildingTypeID = 2;
            }
            public class DowngradesQColumnIndex
            {
                public static int QEntryID = 0;
                public static int BuildingTypeID = 1;
            }

        }

        //bool areTransportsToThisVillageAvailable;
        //private  int _maxPopulation = 0;
        //private int _currentPopulation = 0;
        private Dictionary<BuildingType, List<UnitRecruitQEntry>> _unitRecruitmentQEntries;
        private List<UnitInVillage> _villageUnits;
        private List<UpgradeEvent> _upgrades;
        private List<DowngradeEvent> _downgrades;

        DataTable dtBuildings;
        DataTable dtUpgrades;
        DataTable dtUpgradesQ;
        DataTable dtUnits;
        DataTable dtSupportUnits;
        DataTable dtUnitsInRecruitQ;
        DataTable dtIncomingUnits;
        DataTable dtOutgoingUnits;
        DataTable dtDowngrades;
        DataTable dtDowngradesQ;

        /// <summary>
        /// speeds ups for servants allowed for this village at this time. 
        /// </summary>
        public int minutesOfSpeedupsAllowed { get; private set; }
        private int minutesOfSpeedupsUsedToday { get; set; }


        /// <summary>
        /// may return null if no such village for this owner
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="villageID"></param>
        /// <returns></returns>
        public static Village GetVillage(Player owner, int villageID, bool getTroopMovements, bool getAreTransportsAvail) 
        {
            DataSet ds = DAL.Villages.VillageBuildingInfo(owner.Realm.ConnectionStr, owner.ID, villageID, getTroopMovements, getAreTransportsAvail);

            if (ds.Tables.Count <= 1)
            {
                return null;
            }

            return new Village(owner, villageID, ds);
        }

        private Village(Player owner, int id, DataSet ds)
            : base(owner, id,false)
        {
            LoadData(ds);
            DataTable basicInfo = ds.Tables[CONSTS.TableIndex.VillageBasicInfo];

            Init((string)basicInfo.Rows[0][CONSTS.VillageBasicInfo.Name]
                , (int)basicInfo.Rows[0][CONSTS.VillageBasicInfo.Coins]
                , (int)basicInfo.Rows[0][CONSTS.VillageBasicInfo.Points]
                , (int)basicInfo.Rows[0][CONSTS.VillageBasicInfo.XCord]
                , (int)basicInfo.Rows[0][CONSTS.VillageBasicInfo.YCord]
                , (int)basicInfo.Rows[0][CONSTS.VillageBasicInfo.loyalty]
                , this.GetBuildingLevel(Fbg.Bll.CONSTS.BuildingIDs.CoinMine)
                , this.GetBuildingLevel(Fbg.Bll.CONSTS.BuildingIDs.Treasury)
                , (DateTime)basicInfo.Rows[0][CONSTS.VillageBasicInfo.CoinsLastUpdates]
                , Convert.ToInt32(Math.Floor((Convert.ToDouble(ds.Tables[CONSTS.TableIndex.MaxPop].Rows[0][0]))))
                , Convert.ToInt32(ds.Tables[CONSTS.TableIndex.CurPop].Rows[0][0])
                , Convert.ToBoolean(ds.Tables[CONSTS.TableIndex.AreCoinTransportsAvail].Rows[0][0])
                , (short)basicInfo.Rows[0][CONSTS.VillageBasicInfo.VillageTypeID]
                , ds.Tables[CONSTS.TableIndex.Tags]);
        }

        private DataSet LoadData(DataSet ds)
        {
            if (dtBuildings == null)
            {
                if (ds.Tables.Count <= 1)
                {
                    // this is if there is no such village for such a player
                    return null;
                }

                dtBuildings = ds.Tables[CONSTS.TableIndex.Buildings];
                dtUpgrades = ds.Tables[CONSTS.TableIndex.Upgrades];
                dtUpgradesQ = ds.Tables[CONSTS.TableIndex.UpgradesQ];


                dtUnits = ds.Tables[CONSTS.TableIndex.Units];
                dtSupportUnits = ds.Tables[CONSTS.TableIndex.SupportUnits];
                dtUnitsInRecruitQ = ds.Tables[CONSTS.TableIndex.UnitsInRecruitQ];
                dtIncomingUnits = ds.Tables[CONSTS.TableIndex.IncomingUnits];
                dtOutgoingUnits = ds.Tables[CONSTS.TableIndex.OutgoingUnits];

                dtDowngrades = ds.Tables[CONSTS.TableIndex.Downgrades];
                dtDowngradesQ = ds.Tables[CONSTS.TableIndex.DowngradesQ];
                minutesOfSpeedupsUsedToday = (int) ds.Tables[CONSTS.TableIndex.SpeedupsAllowed].Rows[0][0];
                minutesOfSpeedupsAllowed = _owner.Realm.maxUpgradeSpeedupInMinutes - minutesOfSpeedupsUsedToday;

                return ds;
            }
            return null;
        }

        public DataTable GetIncomingUnits()
        {   
            return dtIncomingUnits;
        }

        public DataTable OutgoingUnits
        {
            get {
                

                return dtOutgoingUnits;
            }
        }


  

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns>0 if no building is present,its level otherwise</returns>
        public int GetBuildingLevel(int buildingID) 
        {
            

            string select = CONSTS.UpgradesColumnName.BuildingTypeID + " = " + buildingID;
            DataRow[] drs = dtBuildings.Select(select, CONSTS.UpgradesColumnName.BuildingLevel + " desc");
            if (drs.Length == 0)
            {
                return 0;
            }
            else if (drs.Length != 1)
            {
                BaseApplicationException e = new BaseApplicationException("Exepecting one row and got:" + drs.Length.ToString());
                this.SerializeToNameValueCollection(e.AdditionalInformation);
                throw e;
            }
            {
                return (int)drs[0][CONSTS.BuildingsColumnIndex.BuildingLevel];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns>0 if no building is present,its level otherwise</returns>
        public int GetBuildingLevel(BuildingType bt)
        {
            // no call to LoadDate intentional! since this is just an overload
            if (bt == null)
            {
                throw new ArgumentNullException("BuildingType bt");
            }
            return GetBuildingLevel(bt.ID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns>0 if no building is present,its level otherwise</returns>
        public BuildingTypeLevel GetBuildingLevelObject(int buildingID) 
        {
            

            //TODO good to cash all this 

            return this.owner.Realm.BuildingType(buildingID).Level(GetBuildingLevel(buildingID));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns>0 if no such building is being upgraded, otherwise the highest level its being upgraded to</returns>
        public int GetUpgradingBuildingLevel(int buildingID)
        {
            

            int upgradingLevel = 0;
            string select = String.Empty;
            DataRow[] drs=null;

            try
            {
                //
                // get highest level in the Q 
                //
                select = CONSTS.UpgradesQColumnNames.BuildingTypeID + " = " + buildingID;
                drs = dtUpgradesQ.Select(select, CONSTS.UpgradesQColumnNames.UpgradeLevel + " desc");
                if (drs.Length != 0)
                {
                    upgradingLevel = Convert.ToInt32(drs[0][CONSTS.UpgradesQColumnIndex.UpgradeLevel]);
                }
                else
                {
                    // since we found nothing in the Q, then lets look the at the currently upgrading list
                    //  btw, there is no point looking at this list if we found an upgrade in the Q since the Q 
                    //  will always have a higher level then the currently upgrading list

                    select = CONSTS.UpgradesColumnName.BuildingTypeID + " = " + buildingID;
                    drs = dtUpgrades.Select(select, CONSTS.UpgradesColumnName.BuildingLevel + " desc");
                    if (drs.Length != 0)
                    {
                        upgradingLevel = (int)drs[0][CONSTS.UpgradesColumnIndex.BuildingLevel];
                    }
                }
            }
            catch (Exception e) 
            {
                BaseApplicationException bex = new BaseApplicationException("error in GetUpgradingBuildingLevel", e);
                bex.AdditionalInformation.Add("buildingID", buildingID.ToString());
                bex.AdditionalInformation.Add("upgradingLevel", upgradingLevel.ToString());
                bex.AdditionalInformation.Add("select", select.ToString());
                bex.AdditionalInformation.Add("drs.Length", drs == null ? "null" : drs.Length.ToString());
                this.SerializeToNameValueCollection(bex.AdditionalInformation);
                throw bex;
            }

            return upgradingLevel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns>the level of a building after all downgrade events (if any) complete. 
        /// Example:current level is 10. 2 downgrade orders/events are active. therefore this will return 8. if no downgrades are active, it will return 10
        /// This does not take any active upgrades into consideration!</returns>
        public int GetDowngradingBuildingLevel(int buildingID)
        {
            int downgradescount = 0;
            int currentLevel=0;

            try
            {
                foreach (DowngradeEvent e in this.GetAllDowngradingBuildings())
                {
                    if (e.bt.ID == buildingID)
                    {
                        downgradescount++;
                    }
                }

                currentLevel = this.GetBuildingLevel(buildingID);

                return currentLevel - downgradescount;

            }
            catch (Exception e)
            {
                BaseApplicationException bex = new BaseApplicationException("error in GetDowngradingBuildingLevel", e);
                bex.AdditionalInformation.Add("buildingID", buildingID.ToString());
                bex.AdditionalInformation.Add("downgradescount", downgradescount.ToString());
                bex.AdditionalInformation.Add("currentLevel", currentLevel.ToString());
                this.SerializeToNameValueCollection(bex.AdditionalInformation);
                throw bex;
            }

        }


        /// <summary>
        /// same as calling GetAllUpgradingBuildings().Count but more efficient
        /// </summary>
        /// <returns></returns>
        public int GetAllUpgradingBuildingsCount()
        {
            

            return dtUpgrades.Rows.Count + dtUpgradesQ.Rows.Count;
        }

        public List<UpgradeEvent> GetAllUpgradingBuildings()
        {
            

            DateTime lastCompletionTime = DateTime.Now;
            TimeSpan upgradeDuration;
            BuildingType building;
            BuildingTypeLevel buildingLevel;

            if (_upgrades == null)
            {

                _upgrades = new List<UpgradeEvent>(dtUpgrades.Rows.Count + dtUpgradesQ.Rows.Count);

                foreach (DataRow dr in dtUpgrades.Rows)
                {
                    building = owner.Realm.BuildingType((int)dr[CONSTS.UpgradesColumnIndex.BuildingTypeID]);

                    lastCompletionTime = (DateTime)dr[CONSTS.UpgradesColumnName.EventTime];

                    _upgrades.Add(new UpgradeEvent_CurrentlyUpgrading(
                        (long)dr[CONSTS.UpgradesColumnIndex.EventID]
                        , building.Level((int)dr[CONSTS.UpgradesColumnIndex.BuildingLevel])
                        , lastCompletionTime));
                }
                foreach (DataRow dr in dtUpgradesQ.Rows)
                {
                    building = owner.Realm.BuildingType((int)dr[CONSTS.UpgradesQColumnIndex.BuildingTypeID]);
                    buildingLevel = building.Level(Convert.ToInt32(dr[CONSTS.UpgradesQColumnIndex.UpgradeLevel]));

                    upgradeDuration = buildingLevel.BuildTime(this);
                    lastCompletionTime = lastCompletionTime + upgradeDuration;

                    _upgrades.Add(new UpgradeEvent_UpgradeInQ(
                        (long)dr[CONSTS.UpgradesQColumnIndex.QEntryID]
                        , buildingLevel
                        , lastCompletionTime));
                }
            }

            return _upgrades;
        }

        public List<DowngradeEvent> GetAllDowngradingBuildings()
        {
            DateTime lastCompletionTime = DateTime.Now;
            BuildingType building;

            if (_downgrades == null)
            {

                _downgrades = new List<DowngradeEvent>(dtDowngrades.Rows.Count + dtDowngradesQ.Rows.Count);

                foreach (DataRow dr in dtDowngrades.Rows)
                {
                    building = owner.Realm.BuildingType((int)dr[CONSTS.DowngradesColumnIndex.BuildingTypeID]);

                    lastCompletionTime = (DateTime)dr[CONSTS.DowngradesColumnIndex.EventTime];

                    _downgrades.Add(new DowngradeEvent_CurrentlyDowngrading(
                        (long)dr[CONSTS.DowngradesColumnIndex.EventID]
                        , building
                        , lastCompletionTime));
                }
                foreach (DataRow dr in dtDowngradesQ.Rows)
                {
                    building = owner.Realm.BuildingType((int)dr[CONSTS.DowngradesQColumnIndex.BuildingTypeID]);

                    //upgradeDuration = buildingLevel.BuildTime(this);
                    //lastCompletionTime = lastCompletionTime + upgradeDuration;

                    _downgrades.Add(new DowngradeEvent_DowngradeInQ(
                        (long)dr[CONSTS.DowngradesQColumnIndex.QEntryID]
                        , building
                        , lastCompletionTime));
                }
            }

            return _downgrades;
        }

        /// <summary>
        /// same as calling GetAllDowngradingBuildings().Count but more efficient
        /// </summary>
        /// <returns></returns>
        public int GetAllDowngradingBuildingsCount()
        {
            return dtDowngrades.Rows.Count + dtDowngradesQ.Rows.Count;
        }

        public Dictionary<BuildingType, List<UnitRecruitQEntry>> GetAllRecruitmentQEntries()
        {
            

            List<UnitRecruitQEntry> qEntryList;
            BuildingType recruitmentBuilding=null;
            if (_unitRecruitmentQEntries == null)
            {
                _unitRecruitmentQEntries = new Dictionary<BuildingType, List<UnitRecruitQEntry>>();
                foreach (DataRow row in dtUnitsInRecruitQ.Rows)
                {
                    if (recruitmentBuilding == null || recruitmentBuilding.ID != (int)row[CONSTS.UnitRecruitQEntreyColumnName.BuildingTypeID]) {
                        recruitmentBuilding = owner.Realm.BuildingType((int)row[CONSTS.UnitRecruitQEntreyColumnName.BuildingTypeID]);
                    }
                    if (_unitRecruitmentQEntries.TryGetValue(recruitmentBuilding, out qEntryList))
                    {
                        qEntryList.Add(new UnitRecruitQEntry(this, row));
                    }
                    else
                    {
                        qEntryList = new List<UnitRecruitQEntry>();
                        qEntryList.Add(new UnitRecruitQEntry_First(this, row));
                        _unitRecruitmentQEntries.Add(recruitmentBuilding, qEntryList);
                    }
                }
            }
            return _unitRecruitmentQEntries;
        }



        public enum CanRecruitResult
        {
            /// <summary>
            /// maxNumberToRecruit holds the max number to recruit
            /// </summary>
            Yes,
            /// <summary>
            /// no
            /// </summary>
            No_RecruitmentBuildingNotBuilt,
            /// <summary>
            /// no, see unsatisfiedReq. means that building level requirements not satisfied
            /// </summary>
            No_RequirementsNotSatisfied,
            /// <summary>
            /// no
            /// </summary>
            No_LackSilver,
            /// <summary>
            /// no
            /// </summary>
            No_LackFood
        }

        /// <summary>
        /// tells you if you can recruit this unit in the village and if not, then why not. 
        /// </summary>
        /// <param name="unit">Unit to recruit</param>
        /// <param name="maxNumberToRecruit">max amount that can be recruited. </param>
        /// <returns>in order:
        /// (*) No_RecruitmentBuildingNotBuilt. no out put param is consiten
        /// (*) No_RequirementsNotSatisfied. Recrutment building is built but not all building OR research req satisfied. see  unsatisfiedReq and unsatisfiedReqRes
        /// (*) No_LackFood. no food
        /// (*) No_LackSilver. no silver
        /// (*) Yes - all is well !
        /// </returns>
        public CanRecruitResult CanRecruit(UnitType ut
            , out int maxNumberToRecruit
            , out int maxNumberToRecruitByPop
            , out int maxNumberToRecruitByCost
            , out List<BuildingTypeLevel> unsatisfiedReq
            , out List<ResearchItem> unsatisfiedReqRes
            )
        {
            int unitCost;

            unsatisfiedReq = null;
            unsatisfiedReqRes = null;
            maxNumberToRecruit = 0;
            maxNumberToRecruitByPop = 0;
            maxNumberToRecruitByCost = 0;
            //
            // is the recruitment building built?
            //
            if (GetBuildingLevel(ut.RecruitmentBuilding) == 0)
            {
                return CanRecruitResult.No_RecruitmentBuildingNotBuilt;
            }
            //
            // are requiements satisfied?
            //
            unsatisfiedReq = ut.GetUnsatisfiedRequirements(this);
            unsatisfiedReqRes = ut.GetUnsatisfiedResearchRequirements(this);

            if (unsatisfiedReq.Count != 0 || unsatisfiedReqRes.Count != 0)
            {
                return CanRecruitResult.No_RequirementsNotSatisfied;
            }
           

            unitCost = ut.Cost(owner); //cache the cost since this could result in multiple DB calls

            //
            // Get the recruit maximum 
            maxNumberToRecruitByPop = RemainingPopulation / ut.Pop;
            maxNumberToRecruitByCost = coins / unitCost;

            if (maxNumberToRecruitByPop <= 0)
            {
                return CanRecruitResult.No_LackFood;
            }
            else if (maxNumberToRecruitByCost <= 0)
            {
                return CanRecruitResult.No_LackSilver;
            }
            else
            {
                if (ut is Fbg.Bll.UnitTypeLord)
                {
                    maxNumberToRecruit = 1;
                }
                else
                {
                    maxNumberToRecruit = maxNumberToRecruitByCost < maxNumberToRecruitByPop ? maxNumberToRecruitByCost : maxNumberToRecruitByPop;
                }
                return CanRecruitResult.Yes;
            }


        }

        /// <summary>
        /// After this is called, the object is left in inconsistent state since the 
        ///  recruitment will not be reflected in the object. You must disacrd the object after calling this 
        /// </summary>
        public string Recruit(UnitType unitType, int recruitCount)
        {
            string returnMsg = null;
            int cost = unitType.Cost(this.owner);//cache this since all calls may call DB. 

            
            if (recruitCount < 1 || recruitCount > 100000) // this is to avoid possibily of overflow
            {
                return "recruit count invalid";
            }

            //
            // validate count. 
            if (recruitCount * unitType.Pop > this.RemainingPopulation) 
            {
                returnMsg = "Lacking food"; 
            }
            else if (recruitCount * cost > this.coins)
            {
                returnMsg = "Lacking silver";
            }
            else
            {
                DAL.Villages.Recruit(this.owner.Realm.ConnectionStr, id, unitType.ID
                    , unitType.RecruitmentBuilding.ID, recruitCount
                    , recruitCount * unitType.Pop, recruitCount * cost,cost );  //TODO -- this is WRONG!! what is a person decides to recruit 2 lords???? he will get them at the same price

                _currentPopulation += recruitCount * unitType.Pop;
                this.coins = this.coins - recruitCount * cost;
                
            }

            return returnMsg;
        }


          /// <summary>
        /// the list MUST be already sorted by priority!!
        /// </summary>
        /// <param name="massRecruitRules"></param>
        public void Recruit(List<MassRecruitRule> massRecruitRules)
        {
            Recruit(massRecruitRules, false);
        }
        /// <summary>
        /// the list MUST be already sorted by priority!!
        /// </summary>
        /// <param name="massRecruitRules"></param>
        public void Recruit(List<MassRecruitRule> massRecruitRules, bool previewOnly)
        {
            foreach (MassRecruitRule rule in massRecruitRules)
            {
                if (rule is MassRecruitRule_RecruitMaxUpTo)
                {
                    RecruitViaRule((MassRecruitRule_RecruitMaxUpTo)rule, previewOnly);
                }
                else if (rule is MassRecruitRule_KeepBusy)
                {
                    RecruitViaRule((MassRecruitRule_KeepBusy)rule, previewOnly);
                }
                else if (rule is MassRecruitRule_RecruitX)
                {
                    RecruitViaRule((MassRecruitRule_RecruitX)rule, previewOnly);
                }
            }            
        }
        /// <summary>
        /// the list MUST be already sorted by priority!!
        /// </summary>
        /// <param name="massRecruitRules"></param>
        public void RecruitPreview(List<MassRecruitRule> massRecruitRules)
        {
            Recruit(massRecruitRules, true);

        }

        /* this was used in a n attempt to balance recuit Q in 2 builings (part of MassRecruitRule_KeepBusy) 
         * but got to so complicated that it was not finised
        private int RecruitViaRuleHelper_AddUpCurrentlyRecruiting(Dictionary<BuildingType, List<UnitRecruitQEntry>> qEntriedByBuildingType, UnitType ut)
        {
            int count = 0;
            foreach (KeyValuePair<BuildingType, List<UnitRecruitQEntry>> kvp in myDictionary)
            {
                if (kvp.Key == ut.RecruitmentBuilding)
                {
                    foreach (UnitRecruitQEntry entry in kvp.Value)
                    {
                        if (ut == rule.FirstUnit)
                        {
                            count += entry.Count;
                        }
                    }
                }
            }
            return count;
        }
         */
        private void RecruitViaRule(MassRecruitRule_KeepBusy rule, bool previewOnly)
        {
            List<BuildingTypeLevel> unsatisfiedReq;
            List<ResearchItem> unsatisfiedResReq;

            int maxByPop;
            int maxByCost;
            int maxRecruitA;
            int maxRecruitB;

            CanRecruitResult resultA = this.CanRecruit(rule.FirstUnit, out maxRecruitA, out maxByPop, out maxByCost, out unsatisfiedReq, out unsatisfiedResReq);
            CanRecruitResult resultB = this.CanRecruit(rule.SecondUnit, out maxRecruitB, out maxByPop, out maxByCost, out unsatisfiedReq, out unsatisfiedResReq);

            rule.Result_FirstUnit_RecruitResult = resultA;
            rule.Result_SecondUnit_RecruitResult = resultB;
            if (resultA == CanRecruitResult.Yes)
            {
                if (maxRecruitA > 1) { maxRecruitA = maxRecruitA / 2; }

                rule.Result_FirstUnit_AmountRecruited = maxRecruitA;
                if (!previewOnly)
                {
                    this.Recruit(rule.FirstUnit, maxRecruitA);
                }
                
            }
            if (resultB == CanRecruitResult.Yes)
            {
                if (maxRecruitB > 1) { maxRecruitB = maxRecruitB / 2; }
                if (maxRecruitB > 1) maxRecruitB -= 1; //why ? to prevent the possibility of actually not having enough since we just recruited units

                rule.Result_SecondUnit_AmountRecruited= maxRecruitB;

                if (!previewOnly)
                {
                    this.Recruit(rule.SecondUnit, maxRecruitB);
                }

            }

            /* removed due to complications. See RecruitViaRuleHelper_AddUpCurrentlyRecruiting for more info
             * 
             * 
            if (resultA == CanRecruitResult.Yes &&
                resultB == CanRecruitResult.Yes)
            {
                //
                // both are recruitable so do then both. 
                //

                // first get the # that are currently recruiting of each 
                Dictionary<BuildingType, List<UnitRecruitQEntry>> qEntriedByBuildingType = this.GetAllRecruitmentQEntries();
                int numCurrentlyRecruitingA = RecruitViaRuleHelper_AddUpCurrentlyRecruiting(qEntriedByBuildingType, rule.FirstUnit);
                int numCurrentlyRecruitingB = RecruitViaRuleHelper_AddUpCurrentlyRecruiting(qEntriedByBuildingType, rule.SecondUnit);

                
            }
            else
            {
                UnitType unitToRecruit = resultA == CanRecruitResult.Yes ?  rule.FirstUnit : rule.SecondUnit;
                int maxToRecruit = resultA == CanRecruitResult.Yes ? maxRecruitA : maxRecruitB;

                if (maxToRecruit > 1) { maxToRecruit = maxToRecruit / 2; }

                this.Recruit(unitToRecruit, maxToRecruit);
            }
                */

        }
        private void RecruitViaRule(MassRecruitRule_RecruitMaxUpTo rule, bool previewOnly)
        {
            UnitInVillage units = GetVillageUnit(rule.Unit);
            int currentlyOwned = units.YourUnitsTotalCount;
            currentlyOwned += units.CurrentlyRecruiting;
            if (rule.MaxToHaveInVillage > currentlyOwned)
            {
                int maxToRecruitDueToRuleCap = rule.MaxToHaveInVillage - currentlyOwned;
                List<BuildingTypeLevel> unsatisfiedReq;
                List<ResearchItem> unsatisfiedResReq;
                int maxByPop;
                int maxByCost;
                int maxRecruit;

                CanRecruitResult result = this.CanRecruit(rule.Unit, out maxRecruit, out maxByPop, out maxByCost, out unsatisfiedReq, out unsatisfiedResReq);
                rule.Result_RecruitResult = result;
                if (result == CanRecruitResult.Yes)
                {
                    maxRecruit = maxToRecruitDueToRuleCap < maxRecruit ? maxToRecruitDueToRuleCap : maxRecruit;
                    rule.Result_AmountRecruited = maxRecruit;
                    if (!previewOnly)
                    {
                        this.Recruit(rule.Unit, maxRecruit);
                    }
                }
            }
            else {
                rule.Result_MaxAlreadyReached = true;
            }
        }
        private void RecruitViaRule(MassRecruitRule_RecruitX rule, bool previewOnly)
        {
            List<BuildingTypeLevel> unsatisfiedReq;
            List<ResearchItem> unsatisfiedResReq;

            int maxByPop;
            int maxByCost;
            int maxRecruit;

            CanRecruitResult result = this.CanRecruit(rule.Unit, out maxRecruit, out maxByPop, out maxByCost, out unsatisfiedReq, out unsatisfiedResReq);
            rule.Result_RecruitResult = result;

            if (result == CanRecruitResult.Yes)
            {
                maxRecruit = maxRecruit < rule.NumToRecruit ? maxRecruit : rule.NumToRecruit;
                rule.Result_AmountRecruited = maxRecruit;
                if (!previewOnly)
                {
                    this.Recruit(rule.Unit, maxRecruit);
                }
            }

        }
        /// <summary>
        /// After this is called, the object is left in inconsistent state since the 
        ///  disbanding will not be reflected in the object. You must disacrd the object after calling this 
        /// </summary>
        public string Disband(UnitType unitType, int disbandCount)
        {
            return Disband(unitType, disbandCount, false);
        }

        /// <summary>
        /// After this is called, the object is left in inconsistent state since the 
        ///  disbanding will not be reflected in the object. You must disacrd the object after calling this 
        /// </summary>
        /// <param name="disbandCountOrFewer">id true, will disband "disbandCount" or if there are fewer troops, will disband all </param>
        public string Disband(UnitType unitType, int disbandCount, bool disbandCountOrFewer)
        {
            string returnMsg = null;

            if (_owner.Stewardship_IsLoggedInAsSteward)
            {
                return "Stewards cannot do this";
            }

            if (unitType.ID == Fbg.Bll.CONSTS.UnitIDs.Gov)
            {
                returnMsg = "you can't disband governers";
                return returnMsg;
            }

            int currentlyInVillage = this.GetVillageUnit(unitType).YourUnitsCurrentlyInVillageCount;
            if (disbandCountOrFewer && disbandCount > currentlyInVillage)
            {
                disbandCount = currentlyInVillage;
            }

            if (disbandCount > currentlyInVillage)
            {
                returnMsg = "You do not have this many troops to disband";
            }
            else
            {
                //
                // invalidate this village from the cache
                //
                _owner.VillagesCache.InvalidateVillage(id);

                if (DAL.Villages.Disband(this.owner.Realm.ConnectionStr, owner.ID, id, unitType.ID
                    , disbandCount) != 0)
                {
                    returnMsg = "You do not have this many troops to disband";
                }
            }


            return returnMsg;
        }

        /// <summary>
        /// DEPRECIATED. use RecruitGoverner2 instead.
        /// 
        /// After this is called, the object is left in inconsistent state since the 
        ///  recruitment will not be reflected in the object. You must disacrd the object after calling this 
        /// </summary>
        public string RecruitGoverner()
        {
            RecruitGovernerResult result = this.RecruitGoverner2();
            switch (result) {
                case RecruitGovernerResult.OK:
                    return null;
                case RecruitGovernerResult.FAILED_NoFood:
                    return "No Food. Upgrade FarmLand";
                case RecruitGovernerResult.FAILED_NoChests:
                    return "Not enough chests";
                case RecruitGovernerResult.FAILED_NoPalace:
                    return "No palace built";
                default:
                    throw new Exception("RecruitGoverner: Unhandeled Error return from the SP");
            }
        }


        public enum RecruitGovernerResult
        {
            OK = 0 
            , FAILED_NoFood
            , FAILED_NoChests
            , FAILED_NoPalace
        }

        /// <summary>
        /// After this is called, the object is left in inconsistent state since the 
        ///  recruitment will not be reflected in the object. You must disacrd the object after calling this 
        /// </summary>
        public RecruitGovernerResult RecruitGoverner2()
        {

            if (GetBuildingLevel(Fbg.Bll.CONSTS.BuildingIDs.Palace) > 0) {
                int result = DAL.Villages.RecruitGoverner(this.owner.Realm.ConnectionStr, this.owner.ID, id);
                switch (result) {
                    case 0:
                        owner.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.GovRecruit, string.Empty, string.Format("Recruited governor in {0}({1},{2}) (Village ID:{3})"
                            , name, xcord, ycord, this.id));
                        return RecruitGovernerResult.OK;
                    case 1:
                        return RecruitGovernerResult.FAILED_NoFood;
                    case 2:
                        return RecruitGovernerResult.FAILED_NoChests;
                    default:
                        throw new Exception("RecruitGoverner: Unhandeled Error return from the SP");
                }
            }
            else {
                return RecruitGovernerResult.FAILED_NoPalace;
            }

            
        }


        /// <summary>
        /// Get the units in this village
        /// </summary>
        /// <returns></returns>
        public List<UnitInVillage> GetVillageUnits()
        {
            

            if (_villageUnits == null)
            {
                _villageUnits = new List<UnitInVillage>(dtUnits.Rows.Count);

                DataRow[] drSupportUnits;
                DataRow[] drVillageUnits;
                foreach (UnitType ut in this.owner.Realm.GetUnitTypes())
                {
                    drVillageUnits = dtUnits.Select(String.Format("{0}={1}"
                        , CONSTS.UnitsColumnName.UnitTypeId
                        , ut.ID));

                    drSupportUnits = dtSupportUnits.Select(String.Format("{0}={1}"
                        , CONSTS.SuppportUnitsColumnName.UnitTypeId
                        , ut.ID));

                    _villageUnits.Add(new UnitInVillage(this
                        , ut
                        , drVillageUnits.Length == 0 ? null : drVillageUnits[0]
                        , drSupportUnits.Length == 0 ? null : drSupportUnits[0]
                        , this.GetAllRecruitmentQEntries()));
                }
            }
            return _villageUnits;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originVillage">cannot be null</param>
        /// <param name="destinationVillage">cannot be null</param>
        /// <param name="unitType">cannot be null</param>
        /// <returns></returns>
        public static TimeSpan GetTravelTime(Village originVillage, VillageOther destinationVillage, UnitType unitType)
        {
            if (originVillage == null)
            {
                throw new ArgumentNullException("Village originVillage");
            }
            if (destinationVillage == null)
            {
                throw new ArgumentNullException("VillageOther destinationVillage");
            }
            if (unitType == null)
            {
                throw new ArgumentNullException("UnitType unitType");
            }
            return GetTravelTime(originVillage, destinationVillage.XCord, destinationVillage.YCord, unitType);
        }
       
        public static TimeSpan GetTravelTime(Village originVillage, int destinationVillageXCord, int destinationVillageYCord, UnitType unitType)
        {
            if (originVillage == null)
            {
                throw new ArgumentNullException("Village originVillage");
            }
           
            if (unitType == null)
            {
                throw new ArgumentNullException("UnitType unitType");
            }
            double distance = Village.CalculateDistance(originVillage.Cordinates.X, originVillage.Cordinates.Y
                , destinationVillageXCord, destinationVillageYCord);

            return new TimeSpan(Convert.ToInt64(System.Math.Floor((distance / unitType.Speed) * TimeSpan.TicksPerHour)));
        }

        public TimeSpan GetTravelTime(VillageOther destinationVillage, UnitType unitType)
        {
            return Village.GetTravelTime(this, destinationVillage, unitType);
        }

        /// <summary>
        /// Get the # of units in this village for the specified unitType.
        /// returns null if not found (ie, if this village has no troops of this type)
        /// </summary>
        /// <returns></returns>
        public UnitInVillage GetVillageUnit(UnitType unitType)
        {
            return GetVillageUnits().Find(delegate(UnitInVillage uiv) { return uiv.UnitType == unitType; });
        }





        public enum CanCommandUnitsResult
        {          
            Yes=0,
            No_troopsSpecifiedNotInVillage,     //1       
            No_TargetUnderBeginnerProtection,   //2
            No_TargetUnderSleepMode,            //3
            No_CannotSupportRebels,             //4
            No_CannotAttackRebelsMoreThan22Away,//5
            No_AttackFreeze,                    //6
            No_UnknownError,                    //7
            No_TargetVillageSameAsOrigin,       //8
            /// <summary>
            /// Cannot send this attack since all troops would desert. need to send more troops
            /// </summary>
            No_AllTroopsDesert,                 //9
            /// <summary>
            /// must send at least 1 of some troop type. cannot send an empty support/attack
            /// </summary>
            No_NoTroopsSelected,                //10
            No_StewardAttackingOnDefenceOnlyStewardshipRealm,   //11
            No_StewardSupportingSomeoneElseInRealmWhereThisIsNotAllowed, //12
            No_TargetUnderVacationMode, //13 
            No_targetUnderWeekendMode, // 14
            No_NoSubscription, //15
            No_NoSupportingOthers //16
        }



        /// <summary>
        /// tells you if you can issue this command
        /// </summary>
        /// <returns>you will never get a return value of "No_UnknownError"</returns>
        public CanCommandUnitsResult CanCommandUnits(
            VillageOther targetVillage
            , UnitCommand command
            , bool isLoggedInAsSteward
            , out TimeSpan travelTime
            , out DateTime arrivalTime
            , out UnitMovements.Desertion desertion
            , out UnitType slowestUnitType
            , bool hasSpeedUpAttacksOnRebelsSpell
            , bool hasSubscriptionPF
            )
        {
            //
            // find slowest unit
            //
            slowestUnitType=null;
            UnitType ut;
            foreach (UnitCommand.Units u in command.unitsSent)
            {
                ut = owner.Realm.GetUnitTypesByID(u.utID);
                if (slowestUnitType == null || slowestUnitType.Speed > ut.Speed)
                {
                    slowestUnitType = ut;
                }
            }

            return CanCommandUnits(
                targetVillage
                , slowestUnitType
                , command
                , isLoggedInAsSteward
                , out travelTime
                , out arrivalTime
                , out desertion
                , hasSpeedUpAttacksOnRebelsSpell
                , hasSubscriptionPF);
        }

        /// <summary>
        /// tells you if you can issue this command
        /// </summary>
        /// <returns>you will never get a return value of "No_UnknownError"</returns>
        public CanCommandUnitsResult CanCommandUnits(
            VillageOther targetVillage
            , UnitType slowestUnitType
            , UnitCommand command
            , bool isLoggedInAsSteward
            , out TimeSpan travelTime
            , out DateTime arrivalTime
            , out UnitMovements.Desertion desertion
            , bool hasSpeedUpAttacksOnRebelsSpell
             , bool hasSubscriptionPF
            )
        {
            List<KeyValuePair<UnitType, int>> unitsToSend2 = new List<KeyValuePair<UnitType, int>>(command.unitsSent.Count);
            foreach (UnitCommand.Units u in command.unitsSent)
            {
                unitsToSend2.Add(new KeyValuePair<UnitType, int>(this.owner.Realm.GetUnitTypesByID(u.utID), u.sendCount));
            }
            return CanCommandUnits(
                targetVillage
                , slowestUnitType
                , command.command
                , unitsToSend2
                , isLoggedInAsSteward
                , out travelTime
                , out arrivalTime
                , out desertion
                , hasSpeedUpAttacksOnRebelsSpell
                , hasSubscriptionPF);
        }

         /// <summary>
        /// tells you if you can issue this command
        /// </summary>
        /// <returns>you will never get a return value of "No_UnknownError"</returns>
        public CanCommandUnitsResult CanCommandUnits(
            VillageOther targetVillage
            , UnitType slowestUnitType
            , UnitCommand.CommandType commandType
            , List<UnitCommand.Units> unitsToSend
            , bool isLoggedInAsSteward
            , out TimeSpan travelTime
            , out DateTime arrivalTime
            , out UnitMovements.Desertion desertion
            )
        {
           
            return CanCommandUnits(targetVillage, slowestUnitType, commandType, unitsToSend, isLoggedInAsSteward, out travelTime, out arrivalTime, out desertion);
        }

        /// <summary>
        /// tells you if you can issue this command
        /// </summary>
        /// <param name="travelTime">returns the travel time to target. always valid, no matter what the return value</param>
        /// <param name="arrivalTime">returns the arrival time to target. always valid, no matter what the return value. based on "slowestUnitType" param</param>
        /// <param name="desertion">Desertion may or may not be valid. it is guaranteed to be non-null IF return value is either Yes OR No_AllTroopsDesert</param>
        /// <returns>you will never get a return value of "No_UnknownError"</returns>
        public CanCommandUnitsResult CanCommandUnits(
            VillageOther targetVillage
            , UnitType slowestUnitType
            , UnitCommand.CommandType commandType
            , List<KeyValuePair<UnitType, int>> unitsToSend
            , bool isLoggedInAsSteward
            , out TimeSpan travelTime
            , out DateTime arrivalTime
            , out UnitMovements.Desertion desertion
            , bool hasSpeedUpAttacksOnRebelsSpell
            , bool hasSubscriptionPF
            )
        {
           
            // check to make sure at least some troops have been selected
            bool someTroopsBeingSent = false;
            bool isGovernorPresent = false;
            bool areSpiesPresent= false;
            bool areNonSpiesPresent = false;
            foreach (KeyValuePair<UnitType, int> u in unitsToSend)
            {
                if (u.Value  > 0 )
                {
                    someTroopsBeingSent = true;

                    if (u.Key.ID == Fbg.Bll.CONSTS.UnitIDs.Gov)
                    {
                        isGovernorPresent = true;
                    }
                    if (u.Key.ID == Fbg.Bll.CONSTS.UnitIDs.Spy)
                    {
                        areSpiesPresent = true;
                    }
                    else
                    {
                        areNonSpiesPresent = true;
                    }
                }
            }
            

            travelTime = this.GetTravelTime(targetVillage, slowestUnitType);
            travelTime = AdjustTravelTimeBasedOnPFs(targetVillage, isGovernorPresent, travelTime, hasSpeedUpAttacksOnRebelsSpell, commandType);
            travelTime = AdjustTravelTimeBasedOnMorale(targetVillage, isGovernorPresent, travelTime, areSpiesPresent && !areNonSpiesPresent, commandType);
            arrivalTime = DateTime.Now.Add(travelTime);
            desertion = null;

            if (this.owner.Realm.RealmSubType == Realm.RealmSubTypes.NoClans 
                && commandType == UnitCommand.CommandType.Support
                && targetVillage.OwnerPlayerID != this.owner.ID)
            {
                return CanCommandUnitsResult.No_NoSupportingOthers;
            }

            if (this.owner.Realm.hasPF(Fbg.Bll.CONSTS.PFs.subscription) && !hasSubscriptionPF)
            {
                return CanCommandUnitsResult.No_NoSubscription;
            }

            if (!someTroopsBeingSent)
            {
                return CanCommandUnitsResult.No_NoTroopsSelected;
            }

            // make sure we are not attacking itself
            if (this.id == targetVillage.ID)
            {
                return CanCommandUnitsResult.No_TargetVillageSameAsOrigin;
            }


            // make sure stewards is not supporting other players on realm where this it not allowed
            if (commandType == UnitCommand.CommandType.Support
                && isLoggedInAsSteward
                && this.owner.Realm.StewardshipType == Realm.StewardshipTypes.DefenceOnlyNoSupportingOthers
                && targetVillage.OwnerPlayerID != owner.ID)
            {
                return CanCommandUnitsResult.No_StewardSupportingSomeoneElseInRealmWhereThisIsNotAllowed;
            }

            // make sure stewards is not attacking on realm with defence only stewardship
            if (commandType == UnitCommand.CommandType.Attack
                && isLoggedInAsSteward
                && (this.owner.Realm.StewardshipType == Realm.StewardshipTypes.DefenceOnly || this.owner.Realm.StewardshipType == Realm.StewardshipTypes.DefenceOnlyNoSupportingOthers))
            {
                return CanCommandUnitsResult.No_StewardAttackingOnDefenceOnlyStewardshipRealm;
            }

            // make sure player is not new and has passed his Protection period
            if (commandType == UnitCommand.CommandType.Attack
                && targetVillage.IsUnderBeginnerProtection
                && targetVillage.OwnerPlayerID != Fbg.Bll.CONSTS.SpecialPlayers.Rebels_PlayerId(owner.Realm))
            {
                return CanCommandUnitsResult.No_TargetUnderBeginnerProtection;
            }

            // make sure player is not in sleep mode
            if (commandType == UnitCommand.CommandType.Attack
                && targetVillage.IsInSleepMode)
            {
                return CanCommandUnitsResult.No_TargetUnderSleepMode;
            }

            // make sure target player is not in weekend mode
            if (commandType == UnitCommand.CommandType.Attack
                && targetVillage.IsInWeekendModeUntill != DateTime.MinValue)
            {
                return CanCommandUnitsResult.No_targetUnderWeekendMode;
            }

            // make sure target player is not in vacation mode
            if (commandType == UnitCommand.CommandType.Attack
                && targetVillage.IsInVacationModeUntill != DateTime.MinValue)
            {
                return CanCommandUnitsResult.No_TargetUnderVacationMode;
            }

            //
            // cannot support rebels
            if (commandType == UnitCommand.CommandType.Support
                && targetVillage.OwnerPlayerID == Fbg.Bll.CONSTS.SpecialPlayers.Rebels_PlayerId(owner.Realm))
            {
                return CanCommandUnitsResult.No_CannotSupportRebels;
            }

            //
            // cannot attack rebels more than 22 away
            if (commandType == UnitCommand.CommandType.Attack
               && targetVillage.OwnerPlayerID == Fbg.Bll.CONSTS.SpecialPlayers.Rebels_PlayerId(owner.Realm)
               && Village.CalculateDistance(this.xcord, this.ycord, targetVillage.XCord, targetVillage.YCord) > 22
                   )
            {
                return CanCommandUnitsResult.No_CannotAttackRebelsMoreThan22Away;
            }

            //
            // check if attack is happening during the attack freeze time OR will land in this time
            if (commandType == UnitCommand.CommandType.Attack
                 && ( (owner.Realm.AttackFreezeGet.IsActiveNow || owner.Realm.AttackFreezeGet.IsInFreezeTime(arrivalTime))
                        ||
                      (owner.Realm.ConsolidationGet.IsAttackFreezeActiveNow || owner.Realm.ConsolidationGet.IsInFreezeTime(arrivalTime))
                    )
                 && targetVillage.OwnerPlayerID != Fbg.Bll.CONSTS.SpecialPlayers.Rebels_PlayerId(owner.Realm)
                 && targetVillage.OwnerPlayerID != Fbg.Bll.CONSTS.SpecialPlayers.Abandoned_PlayerId(owner.Realm)
                )
            {
                return CanCommandUnitsResult.No_AttackFreeze;
            }
            //
            // check desertion - check if all troops desert
            //
            desertion = UnitMovements.Desertion.GetUnitDesertion(commandType, this.owner.Realm, unitsToSend, targetVillage, this);
            if (desertion.IsThereAnyDesertion)
            {
                // go through all troops, and  make sure that at least some troops did not desert
                bool notAllTroopDesert = false;

                foreach (KeyValuePair<UnitType, int> u in unitsToSend)
                {
                    if (desertion.GetUnitsDeserted(u.Value) < u.Value) 
                    {
                        notAllTroopDesert = true;
                        break;
                    }
                }
                if (!notAllTroopDesert)
                {
                    return CanCommandUnitsResult.No_AllTroopsDesert;
                }
            }

            //
            // All good! command ok.
            //
            return CanCommandUnitsResult.Yes;

        }

        private TimeSpan AdjustTravelTimeBasedOnMorale(VillageOther targetVillage, bool isGovPresent, TimeSpan travelTime, bool isOnlySpies, UnitCommand.CommandType commandType)
        {
            if (this.owner.Realm.Morale.IsActiveOnThisRealm)
            {
                if (commandType == UnitCommand.CommandType.Attack)
                {
                    if (!isGovPresent && !isOnlySpies)
                    {
                        if (targetVillage.OwnerPlayerID == Fbg.Bll.CONSTS.SpecialPlayers.Rebels_PlayerId(owner.Realm)
                        || targetVillage.OwnerPlayerID == Fbg.Bll.CONSTS.SpecialPlayers.Abandoned_PlayerId(owner.Realm)
                        )
                        {
                            return new TimeSpan(Convert.ToInt64(travelTime.Ticks / owner.Morale.Effect.MoveSpeedMultiplier));
                        }
                    }
                }
            }

            return travelTime;
        }

        private TimeSpan AdjustTravelTimeBasedOnPFs(VillageOther targetVillage, bool  isGovPresent, TimeSpan travelTime, bool hasSpeedUpSpell, UnitCommand.CommandType commandType)
        {
            if (this.owner.Realm.hasPF(Fbg.Bll.CONSTS.PFs.attackSpeedUp) && hasSpeedUpSpell && !isGovPresent)
            {
                if (commandType == UnitCommand.CommandType.Attack)
                {
                    if (targetVillage.OwnerPlayerID == Fbg.Bll.CONSTS.SpecialPlayers.Rebels_PlayerId(owner.Realm)
                        || targetVillage.OwnerPlayerID == Fbg.Bll.CONSTS.SpecialPlayers.Abandoned_PlayerId(owner.Realm)
                        )
                    {
                        return new TimeSpan(travelTime.Ticks / 20);
                    }
                }
            }

            return travelTime;
        }


        /// <summary>
        /// </summary>
        /// <returns>you may get a return value of "No_UnknownError" in case the command failed when making the call to the DB
        /// Return value of CanCommandUnitsResult.Yes means the command was successful</returns>
        public CanCommandUnitsResult CommandUnits(VillageOther targetVillage, UnitType slowestUnitType, UnitCommand command, bool hasCommandTroopsPF, bool isLoggedInAsSteward, bool hasSpeedUpAttacksOnRebelsSpell)
        {
            int playerMoraleAfterCmd;
            DateTime playerMoraleLastUpdatedAfterCmd;

            return this.CommandUnits(targetVillage, slowestUnitType, command, hasCommandTroopsPF, isLoggedInAsSteward, hasSpeedUpAttacksOnRebelsSpell, out playerMoraleAfterCmd, out playerMoraleLastUpdatedAfterCmd);
        }
        /// <summary>
        /// </summary>
        /// <returns>you may get a return value of "No_UnknownError" in case the command failed when making the call to the DB
        /// Return value of CanCommandUnitsResult.Yes means the command was successful</returns>
        public CanCommandUnitsResult CommandUnits(VillageOther targetVillage, UnitType slowestUnitType, UnitCommand command, bool hasCommandTroopsPF, bool isLoggedInAsSteward, bool hasSpeedUpAttacksOnRebelsSpell, out int playerMoraleAfterCmd, out DateTime playerMoraleLastUpdatedAfterCmd)
        {
            // no call to LoadDate intentional! no need
            playerMoraleAfterCmd = this.owner.Morale.Morale;
            playerMoraleLastUpdatedAfterCmd = this.owner.Morale.MoraleLastUpdatedOn;

            TimeSpan travelTime;
            DateTime arrivalTime;

            UnitMovements.Desertion desertion;
            CanCommandUnitsResult result = CanCommandUnits(targetVillage, slowestUnitType, command, isLoggedInAsSteward, out travelTime, out arrivalTime, out desertion
                , hasSpeedUpAttacksOnRebelsSpell, owner.PF_HasPF(Fbg.Bll.CONSTS.PFs.subscription));
            if (result != CanCommandUnitsResult.Yes)
            {
                return result;
            }
            command.UnitDesertionFactor = desertion.factor;
            //
            // invalidate this village from the cache
            //
            _owner.VillagesCache.InvalidateVillage(id);
            //
            // EXECUTE THE COMMAND
            //
            int retVal = DAL.Villages.CommandUnits(this.owner.Realm.ConnectionStr
                , arrivalTime
                , travelTime
                , command
                , _owner.ID
                , hasCommandTroopsPF
                , out playerMoraleAfterCmd, out playerMoraleLastUpdatedAfterCmd);


            //
            // examine the return value;
            //
            if (retVal == 0)
            {
                _owner.RecentTargets_Invalidate();
                return CanCommandUnitsResult.Yes;
            }
            else if (retVal == -1)
            {
                return CanCommandUnitsResult.No_troopsSpecifiedNotInVillage;
            }
            else
            {
                return CanCommandUnitsResult.No_UnknownError;
            }
        }
        public UnitsAbroad GetUnitsAbroad()
        {
            return owner.GetUnitsAbroad(id, null);
        }


        public enum CanUpgradeResult 
        {
            /// <summary>
            /// next level is available and you can do it. 
            /// </summary>
            Yes,
            /// <summary>
            /// next level is available, but you do not have enough food
            /// </summary>
            No_LackFood,
            /// <summary>
            /// next level is available, but you do not have enough silver
            /// </summary>
            No_LackSilver,
            /// <summary>
            /// no next level available since building is fully upgraded. 
            /// </summary>
            No_BuildingFullyUpgraded,
            /// <summary>
            /// you can build next level, you have enough silver and food but you cannot due to locked feature being unavailable
            /// </summary>
            No_LockedFeature,
            /// <summary>
            /// IF can build next level but you already have 1 item being built and at the moment, only 1 item in Q is allowed; 
            /// see code for all conditions
            /// </summary>
            No_Busy,
            /// <summary>
            /// IF next level is available but the requirements have not been met
            /// </summary>
            No_UnsatisfiedReq,
            /// <summary>
            /// IF there are downgrades currently active
            /// </summary>
            No_DowngradesInProgress
        }


        /// <summary>
        /// tells you if you can upgrade this building in the village
        /// </summary>
        /// <param name="bt">building to upgrade</param>
        /// <param name="nextBuildingTypeLevel">Next building level or NULL</param>
        /// <param name="curBuildingTypeLevel">The current level of this building in the village. Null if no building at this village</param>
        /// <param name="maxNextBuildingTypeLevel">The maximum level this village can afford to upgrade to. If equals to nextBuildingTypeLevel then no more upgrades are possible
        /// ie, this means that nextBuildingTypeLevel is already max upgrade level. ONLY valid if return is CanUpgradeResult.YES</param>
        /// <param name="unsatisfiedReqs">If no unsatisfied requirements, returned list is not null but 0 legth</param>
        /// <param name="notEnoughSilver">true if next upgrade level is available and village does not have enough silver for upgrade</param>
        /// <param name="notEnoughFood">true if next upgrade level is available and village does not have food for upgrade</param>
        /// <param name="pfSystemActive">true if premium system is active</param>
        /// <returns>in order: 
        /// (*) No_DowngradesInProgress -  nextBuildingTypeLevel, unsatisfiedReqs, notEnoughSilver & notEnoughFood is valid
        /// (*) ELSE No_UnsatisfiedReq -  nextBuildingTypeLevel, unsatisfiedReqs, notEnoughSilver & notEnoughFood is valid
        /// (*) ELSE, No_BuildingFullyUpgraded -  nextBuildingTypeLevel is null. ignore unsatisfiedReqs, notEnoughSilver & notEnoughFood 
        /// (*) ELSE, No_LackFood -  nextBuildingTypeLevel, unsatisfiedReqs(empty list), notEnoughSilver & notEnoughFood(will be true) is valid
        /// (*) ELSE, No_LackSilver -  nextBuildingTypeLevel, unsatisfiedReqs(empty list), notEnoughSilver(will be true) & notEnoughFood is valid
        /// (*) ELSE, No_Busy -  nextBuildingTypeLevel, unsatisfiedReqs(empty list), notEnoughSilver(is false) & notEnoughFood(is false) is valid
        /// (*) ELSE, No_LockedFeature -  nextBuildingTypeLevel, unsatisfiedReqs(empty list), notEnoughSilver(is false) & notEnoughFood(is false) is valid
        /// (*) ELSE, Yes -  nextBuildingTypeLevel, unsatisfiedReqs(empty list), notEnoughSilver(is false) & notEnoughFood(is false), maxNextBuildingTypeLevel is valid
        /// </returns>
        public CanUpgradeResult CanUpgrade(BuildingType bt
            , out BuildingTypeLevel nextBuildingTypeLevel
            , out BuildingTypeLevel curBuildingTypeLevel
            , out BuildingTypeLevel maxNextBuildingTypeLevel
            , out List<BuildingTypeLevel> unsatisfiedReqs
            , out bool notEnoughSilver
            , out bool notEnoughFood
            , bool isNightBuildModeActive)
        {
            notEnoughSilver = false;
            notEnoughFood = false;
            CanUpgradeResult returnValue=CanUpgradeResult.Yes;
            int itemsInUpgradeQAlready = 0;
            bool playerHasUmlimitedBuildingQPF = false;

            int curLevel=0;
            int nextPotentialUpgradeLevel=0;
            int upgradeLevel=0;

            //
            // for calculating max next upgrade level
            int silverLeft=0; 
            int foodRemaining=0; 
            BuildingTypeLevel potentialMaxNextBuildingTypeLevel =null; 
            List<BuildingTypeLevel> maxNextunsatisfiedReqs = null;

            maxNextBuildingTypeLevel = null;
            nextBuildingTypeLevel = null;
            curBuildingTypeLevel = null;
            unsatisfiedReqs = null;

            try
            {


                curBuildingTypeLevel = bt.Level(this.GetBuildingLevel(bt.ID));
                curLevel = curBuildingTypeLevel == null ? 0 : curBuildingTypeLevel.Level;
                upgradeLevel = this.GetUpgradingBuildingLevel(bt.ID);

                // get the next upgrade level
                nextPotentialUpgradeLevel = upgradeLevel > curLevel ? upgradeLevel : curLevel; // get the larger of the two
                nextPotentialUpgradeLevel++;

                //this may be NULL! if nextPotentialUpgradeLevel > then max level
                nextBuildingTypeLevel = bt.Level(nextPotentialUpgradeLevel);

                // get a list of unsatisfied req. no req if list is empty            
                if (nextBuildingTypeLevel != null)
                {
                    unsatisfiedReqs = nextBuildingTypeLevel.GetUnsatisfiedRequirements(this);
                }
                else
                {
                    unsatisfiedReqs = new List<BuildingTypeLevel>(0);
                }

                //
                // Do we have enough silver??
                if (nextBuildingTypeLevel != null
                    && nextBuildingTypeLevel.Cost > this.coins)
                {
                    notEnoughSilver = true;
                }
                // 
                // Do we have enough food ??
                if (nextBuildingTypeLevel != null
                    && nextBuildingTypeLevel.Population > this.RemainingPopulation)
                {
                    notEnoughFood = true;
                }
                //
                // do not allow upgrades if downgrades are active
                if (this.GetAllDowngradingBuildingsCount() != 0)
                {
                    return CanUpgradeResult.No_DowngradesInProgress;
                }
                //
                // now see if we can do the upgrade
                //
                if (unsatisfiedReqs.Count != 0)
                {
                    returnValue = CanUpgradeResult.No_UnsatisfiedReq;
                }
                else if (nextBuildingTypeLevel == null)
                {
                    returnValue = CanUpgradeResult.No_BuildingFullyUpgraded;
                }
                else if (nextBuildingTypeLevel.Population > this.RemainingPopulation
                     && nextBuildingTypeLevel.Population != 0)
                {
                    returnValue = CanUpgradeResult.No_LackFood;
                }
                else if (nextBuildingTypeLevel.Cost > this.coins)
                {
                    returnValue = CanUpgradeResult.No_LackSilver;
                }
                else
                {
                    playerHasUmlimitedBuildingQPF = owner.PF_HasPF(Fbg.Bll.CONSTS.PFs.BuildingQ);
                    itemsInUpgradeQAlready = this.GetAllUpgradingBuildings().Count;

                    //
                    // check if only one item in Q is allowed at this time 
                    //
                    if (this.IsOnlyOneItemUpgradeLimitationNowActive
                        && itemsInUpgradeQAlready >= 1 ) // has at least one item upgrading
                    {
                        returnValue = CanUpgradeResult.No_Busy;
                    }
                    else
                    {
                        //
                        // See if this upgrade is possible from the PF point of view. 
                        //  if player has no PF and there are more than 2 buildings in Q 
                        //  and the night build 
                        //
                        if (!playerHasUmlimitedBuildingQPF)
                        {
                            if (itemsInUpgradeQAlready >= Fbg.Bll.CONSTS.PFInfo.BuildQueueLimit)
                            {
                                //
                                // person has more than 2 buildings in Q. 
                                // So now check if perhaps he has night mode activated. If so, and if total duration of 
                                //  all upgrades in build Q exceed 8 hours, then do not allow it
                                //
                                if (!isNightBuildModeActive
                                    || TotalQueueBuildTime(this.GetAllUpgradingBuildings()).TotalHours >= Fbg.Bll.CONSTS.PFInfo.NightBuilModeMaxQueueTimeInHours)
                                {
                                    returnValue = CanUpgradeResult.No_LockedFeature;
                                }
                            }
                        }
                        //
                        // check if we cannot upgrade due to a PF. if not, then just continue to the upgrade.
                        //
                        if (returnValue == CanUpgradeResult.Yes)
                        {
                            #region Can build. Figure out some details
                            //
                            // now lets calculate the max next level building this village can afford. 
                            //  after this loop, maxNextBuildingTypeLevel will point to this max level. 
                            //      maxNextBuildingTypeLevel can be just = nextBuildingTypeLevel meaning no more levels are possible
                            //      or maxNextBuildingTypeLevel.Level > nextBuildingTypeLevel.Level
                            //  
                            //  we only do this if player has the PF.
                            //
                            if (owner.PF_HasPF(Fbg.Bll.CONSTS.PFs.BuildingQ))
                            {
                                silverLeft = coins;
                                foodRemaining = RemainingPopulation;
                                silverLeft = silverLeft - nextBuildingTypeLevel.Cost; // we know silverLeft will be >= 0 since we know nextBuildingTypeLevel upgrade is possible
                                foodRemaining = foodRemaining - nextBuildingTypeLevel.Population; // we know foodRemaining will be >= 0 since we know nextBuildingTypeLevel upgrade is possible
                                potentialMaxNextBuildingTypeLevel = nextBuildingTypeLevel;
                                do
                                {
                                    maxNextBuildingTypeLevel = potentialMaxNextBuildingTypeLevel;
                                    potentialMaxNextBuildingTypeLevel = bt.Level(potentialMaxNextBuildingTypeLevel.Level + 1);
                                    if (potentialMaxNextBuildingTypeLevel != null)
                                    {
                                        silverLeft = silverLeft - potentialMaxNextBuildingTypeLevel.Cost;
                                        foodRemaining = foodRemaining - potentialMaxNextBuildingTypeLevel.Population;
                                        maxNextunsatisfiedReqs = potentialMaxNextBuildingTypeLevel.GetUnsatisfiedRequirements(this);
                                    }
                                } while (silverLeft >= 0 && foodRemaining >= 0 && potentialMaxNextBuildingTypeLevel != null && maxNextunsatisfiedReqs.Count == 0);
                            }
                            else
                            {
                                maxNextBuildingTypeLevel = nextBuildingTypeLevel;
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BaseApplicationException bex = new BaseApplicationException("Error in CanUpgrade", ex);
                bex.AddAdditionalInformation("bt", bt);
                bex.AddAdditionalInformation("nextBuildingTypeLevel", nextBuildingTypeLevel);
                bex.AddAdditionalInformation("curBuildingTypeLevel", curBuildingTypeLevel);
                bex.AddAdditionalInformation("maxNextBuildingTypeLevel", maxNextBuildingTypeLevel);
                bex.AddAdditionalInformation("unsatisfiedReqs", unsatisfiedReqs);
                bex.AddAdditionalInformation("notEnoughSilver", notEnoughSilver);
                bex.AddAdditionalInformation("notEnoughFood", notEnoughFood);
                bex.AddAdditionalInformation("returnValue", returnValue);
                bex.AddAdditionalInformation("curLevel",curLevel);
                bex.AddAdditionalInformation("nextPotentialUpgradeLevel",nextPotentialUpgradeLevel);
                bex.AddAdditionalInformation("upgradeLevel",upgradeLevel);
                bex.AddAdditionalInformation("silverLeft",silverLeft);
                bex.AddAdditionalInformation("foodRemaining",foodRemaining);
                bex.AddAdditionalInformation("potentialMaxNextBuildingTypeLevel",potentialMaxNextBuildingTypeLevel);
                bex.AddAdditionalInformation("maxNextunsatisfiedReqs",maxNextunsatisfiedReqs);
                bex.AddAdditionalInformation("itemsInUpgradeQAlready", itemsInUpgradeQAlready);
                throw bex;
            }
            return returnValue;         
        }

        /// <summary>
        /// tells if you this village currently is limited to 1 item upgrading
        /// This does not check if there is one item upgrading already, just tells if you this village is limited to this in principle
        /// </summary>
        /// <returns></returns>
        public bool IsOnlyOneItemUpgradeLimitationNowActive
        {
            get
            {
                bool playerHasUmlimitedBuildingQPF = owner.PF_HasPF(Fbg.Bll.CONSTS.PFs.BuildingQ);
                if (!owner.Realm.IsTemporaryTournamentRealm) // only for none tournament realms
                {
                    if ((!playerHasUmlimitedBuildingQPF || owner.Realm.IsVPrealm) // if player does not have unlimited building Q feature OR is on VP Realm
                        && owner.NumberOfVillages == 1  // has only one village
                        )
                    {
                        int levelOfHQ = this.GetBuildingLevel(Fbg.Bll.CONSTS.BuildingIDs.VillageHQ);
                        // for VP realm, need HQ level 15
                        // for none VP Realms, need HQ level 10
                        if (levelOfHQ < owner.Realm.HQLevelNeededForUnlimitedQ)
                        {
                            //
                            // player with no PF, 1 village and HQ less then level 10/15 is limited to 1 items in the building Q
                            // 
                            return true;
                        }
                    }
                }
                return false;
            }
        }



        public enum CanDowngradeResult
        {
            /// <summary>
            /// at least one downgrade order can be made
            /// </summary>
            Yes,
            /// <summary>
            /// 
            /// </summary>
            No_UpgradesInProgress,
            /// <summary>
            /// 
            /// </summary>
            No_NoSuchBuildingInVillage,
            /// <summary>
            /// 
            /// </summary>
            No_BuildingBeingDowngradedToMinLevel,
            /// <summary>
            /// you can build next level, you have enough silver and food but you cannot due to locked feature being unavailable
            /// </summary>
            No_LockedFeature,
            /// <summary>
            /// if player is logged in as steward then this is not allowed
            /// </summary>
            No_StwardsCannotDoThis

        }


        /// <summary>
        /// tells you if you can downgrade this building in the village
        /// </summary>
        public CanDowngradeResult CanDowngrade(BuildingType bt)
        {
            int itemsInUpgradeQAlready = 0;
            bool playerHasUmlimitedBuildingQPF = false;
            BuildingTypeLevel curBuildingTypeLevel=null;
            int levelOfBuildingAfterAllDowngrades=0;

            try
            {
                if (_owner.Stewardship_IsLoggedInAsSteward)
                {
                    return CanDowngradeResult.No_StwardsCannotDoThis;
                }
                if (this.GetAllUpgradingBuildingsCount() != 0)
                {
                    return CanDowngradeResult.No_UpgradesInProgress;
                }

                curBuildingTypeLevel = bt.Level(this.GetBuildingLevel(bt.ID));
                if (curBuildingTypeLevel == null)
                {
                    return CanDowngradeResult.No_NoSuchBuildingInVillage;
                }

                levelOfBuildingAfterAllDowngrades = this.GetDowngradingBuildingLevel(bt.ID);
                if (levelOfBuildingAfterAllDowngrades <= bt.MinimumLevelAllowed)
                {
                    return CanDowngradeResult.No_BuildingBeingDowngradedToMinLevel;
                }

                playerHasUmlimitedBuildingQPF = owner.PF_HasPF(Fbg.Bll.CONSTS.PFs.BuildingQ);
                itemsInUpgradeQAlready = this.GetAllDowngradingBuildingsCount();
                //
                // See if this upgrade is possible from the PF point of view. 
                //  if player has no PF and there are more than 2 buildings in Q 
                //
                if (!playerHasUmlimitedBuildingQPF)
                {
                    if (itemsInUpgradeQAlready >= Fbg.Bll.CONSTS.PFInfo.BuildQueueLimit)
                    {
                        return CanDowngradeResult.No_LockedFeature;
                    }
                }

                return CanDowngradeResult.Yes;


            }
            catch (Exception ex)
            {
                BaseApplicationException bex = new BaseApplicationException("Error in CanUpgrade", ex);
                bex.AddAdditionalInformation("bt", bt);
                bex.AddAdditionalInformation("curBuildingTypeLevel", curBuildingTypeLevel);
                bex.AddAdditionalInformation("itemsInUpgradeQAlready", itemsInUpgradeQAlready);
                bex.AddAdditionalInformation("levelOfBuildingAfterAllDowngrades", levelOfBuildingAfterAllDowngrades);
                throw bex;
            }
        }


        public static double CalculateDistance(int originX, int originY, int destinationX, int destinationY)
        {
            double x, y;
            x = System.Math.Abs(originX - destinationX);
            y = System.Math.Abs(originY - destinationY);

            x = x * x;
            y = y * y;

            return  System.Math.Sqrt(x + y);
        }


        #region ISerializableToNameValueCollection Members

        new public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
        {
            try
            {
                string pre = "Village[" + id.ToString() + "]";

                if (col == null)
                {
                    ExceptionManager.Publish("Error in Village.SerializeToNameValueCollection. argument 'col' is null");
                }
                else
                {

                    #region GetBuildingLevelObject(....)
                    BuildingTypeLevel btl=null;
                    string pre2;
                    foreach(BuildingType bt in this.owner.Realm.BuildingTypes ) 
                    {
                        pre2 = pre + "BT[" + bt.ID + "]";
                        try
                        {
                            btl = this.GetBuildingLevelObject(bt.ID);
                        } catch(Exception e) 
                        {
                            col.Add(pre2,"Error calling GetBuildingLevelObject(). Message: " + e.Message);
                        }

                        if (btl != null)
                        {
                            btl.SerializeToNameValueCollection(col, pre2);
                        }
                        else
                        {
                            col.Add(pre2, "null");
                        }
                    }
                    #endregion

                    #region serialize all datatables
                    col.Add(pre + "dtBuildings", Gmbc.Common.Data.DataSetUtilities.WriteXml(dtBuildings, true));
                    col.Add(pre + "dtUpgrades", Gmbc.Common.Data.DataSetUtilities.WriteXml(dtUpgrades, true));
                    col.Add(pre + "dtUpgradesQ", Gmbc.Common.Data.DataSetUtilities.WriteXml(dtUpgradesQ, true));
                    col.Add(pre + "dtUnits", Gmbc.Common.Data.DataSetUtilities.WriteXml(dtUnits, true));
                    col.Add(pre + "dtSupportUnits", Gmbc.Common.Data.DataSetUtilities.WriteXml(dtSupportUnits, true));
                    col.Add(pre + "dtUnitsInRecruitQ", Gmbc.Common.Data.DataSetUtilities.WriteXml(dtUnitsInRecruitQ, true));
                    col.Add(pre + "dtIncomingUnits", Gmbc.Common.Data.DataSetUtilities.WriteXml(dtIncomingUnits, true));
                    col.Add(pre + "dtOutgoingUnits", Gmbc.Common.Data.DataSetUtilities.WriteXml(dtOutgoingUnits, true));
                    #endregion                
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish("Error in Village.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + ExceptionManager.SerializeToString(e));
            }
        }

        #endregion


        public void SetName(string newVillageName)
        {
            DAL.Villages.SetVillageName(owner.Realm.ConnectionStr, id, newVillageName);
            this._name = newVillageName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="villageTypeID"></param>
        /// <param name="cost"> cost in servants for this action </param>
        public bool SetType(int villageTypeID, int cost)
        {
            //must be a bonus village to begin with, to allow bonus change.
            if (this.VillageType.IsBonus){
                DAL.Villages.SetVillageType(owner.Realm.ConnectionStr, id, villageTypeID, cost, this.owner.ID);
                this.VillageType = owner.Realm.VillageTypes[villageTypeID];
                return true;
            }
            return false;
        }

        public void saveVillageNotes( string Notes)
        {
            
            Fbg.DAL.VillageOther.saveVillageNotes(_id, _owner.ID , Notes, owner.Realm.ConnectionStr);
        }
        /// <summary>
        /// same as calling UpdateCoins(coins, out coinsOverflow)
        /// </summary>
        /// <param name="coins">amount of coins to add. must be > 0</param>
        public void UpdateCoins(int coins)
        {
            int coinsOverflow;
            UpdateCoins(coins, out coinsOverflow);
        }



        /// <summary>
        /// Returns TimeSpan.MinValue if this building does not do anything, ever. 
        /// Returns TimeSpan.Zero if this building is idle
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        public TimeSpan GetBuildingWorkTimeRemaining(BuildingType bt)
        {
            TimeSpan totalTime = TimeSpan.MinValue;
            if (bt is BuildingTypeHQ)
            {
                totalTime = TimeSpan.Zero;

                bool firstUpgrade = true;
                foreach (UpgradeEvent upgrade in this.GetAllUpgradingBuildings())
                {
                    if (firstUpgrade)
                    {
                        totalTime += upgrade.completionTime.Subtract(DateTime.Now);

                        firstUpgrade = false;
                    }
                    else
                    {
                        totalTime += upgrade.upgradeToLevel.BuildTime(this);
                    }
                }
            }
            else if (bt.ID == Fbg.Bll.CONSTS.BuildingIDs.Stable
                || bt.ID == Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop
                || bt.ID == Fbg.Bll.CONSTS.BuildingIDs.Tavern
                || bt.ID == Fbg.Bll.CONSTS.BuildingIDs.Barracks
                || bt.ID == Fbg.Bll.CONSTS.BuildingIDs.Palace)
            {
                totalTime = TimeSpan.Zero;

                if (this.GetAllRecruitmentQEntries().Count > 0)
                {
                    if (this.GetAllRecruitmentQEntries().ContainsKey(bt))
                    {
                        foreach (UnitRecruitQEntry ur in this.GetAllRecruitmentQEntries()[bt])
                        {
                            if (ur is UnitRecruitQEntry_First)
                            {
                                totalTime += ((UnitRecruitQEntry_First)ur).RecruitmentCompletedOn.Subtract(DateTime.Now);
                            }
                            else
                            {
                                totalTime += ur.TotalRecruitTime;
                            }
                        }
                    }

                }
            }


            return totalTime;
        }


        private TimeSpan TotalQueueBuildTime(List<UpgradeEvent> upgrades)
        {
            TimeSpan totalQueueBuildTime = TimeSpan.Zero;

            foreach (UpgradeEvent upgrade in upgrades)
            {

                if (upgrade is UpgradeEvent_CurrentlyUpgrading)
                {
                    totalQueueBuildTime += ((UpgradeEvent_CurrentlyUpgrading)upgrade).completionTime.Subtract(DateTime.Now);
                }
                else
                {
                    totalQueueBuildTime += ((UpgradeEvent_UpgradeInQ)upgrade).upgradeToLevel.BuildTime(this);
                }

            }

            return totalQueueBuildTime;
        }

        public void BuildingDowngrade(int btID)
        {
            CanDowngradeResult result = this.CanDowngrade(this.owner.Realm.BuildingType(btID));

            if (result == CanDowngradeResult.Yes)
            {
                DAL.Villages.DoDowngrade(owner.Realm.ConnectionStr, this.id, btID);
            }
        }

        public void CancelDowngrade(int eid, bool isQ)
        {
            DAL.Villages.CancelDowngrade(owner.Realm.ConnectionStr, eid, isQ);

        }

        /// <summary>
        /// returns 0 if failed, or cost of the action if successful
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="cutMinutes">set to 9999 if doing "finish now"</param>
        /// <returns></returns>
        public int SpeedUpUpgrade(long eventID, int cutMinutes)
        {
 
            List<UpgradeEvent> upgrades = this.GetAllUpgradingBuildings();
            if (upgrades.Count > 0
                && upgrades[0] is UpgradeEvent_CurrentlyUpgrading
                && upgrades[0].ID == eventID
                )
            {
                TimeSpan timeLeft = upgrades[0].completionTime.Subtract(DateTime.Now);

                if (cutMinutes != 9999)
                {
                    //
                    // if the requested cut time would complete the building, see if this is the most efficient cut time or if perhaps
                    //  a cheaper one would do the job instead.
                    //
                    //  why do we do this? to prevent support calls that says 'I clicked cut 4 hours by mistake but i only had 55 min left. why i got charged so much, blah, blah'
                    //
                    if (timeLeft.TotalMinutes < cutMinutes)
                    {
                        if (timeLeft.TotalMinutes < 1)
                        {
                            cutMinutes = 1;
                        }
                        else if (timeLeft.TotalMinutes < 15)
                        {
                            cutMinutes = 15;
                        }
                        else if (timeLeft.TotalMinutes < 60)
                        {
                            cutMinutes = 60;
                        }
                    }
                    return DAL.Villages.SpeedUpUpgrade(owner.Realm.ConnectionStr, eventID, cutMinutes, this.owner.ID);
                }
                else
                {
                    //
                    // doing finish now
                    //
                    List<int> listOfTimeCuts;
                    int actualCost=0;
                    bool allowFinishNow=false;
                    UpgradeSpeedUp_CalculateFinishNow(timeLeft.TotalMinutes, out listOfTimeCuts, this.minutesOfSpeedupsAllowed, ref allowFinishNow);
                    foreach (int cutTime in listOfTimeCuts)
                    {
                        actualCost += DAL.Villages.SpeedUpUpgrade(owner.Realm.ConnectionStr, eventID, cutTime, this.owner.ID);
                    }
                    return actualCost;
                }
            }

            return 0;
        }

        /// <summary>
        /// returns 0 if failed, or cost of the action if successful
        /// </summary>
        /// <param name="eventID"></param>
        /// <param name="cutMinutes">set to 9999 if doing "finish now"</param>
        /// <returns></returns>
        public bool SpeedUpUpgrade(long eventID, Fbg.Bll.Items2.Item2_BuildingSpeedup item)
        {

            List<UpgradeEvent> upgrades = this.GetAllUpgradingBuildings();
            if (upgrades.Count > 0
                && upgrades[0] is UpgradeEvent_CurrentlyUpgrading
                && upgrades[0].ID == eventID
                )
            {
                return DAL.Villages.SpeedUpUpgradeFromItem(owner.Realm.ConnectionStr, eventID, item.AmountOfMinutesToSpeedUp, this.owner.ID);
            }

            return false;
        }

        public static int[] timeCuts = new int[4] { 240, 60, 15, 1 };
        public static int[] timeCutCosts = new int[4] { 30, 10, 5, 1 };
        public static int CostOfTimeCut(int minutesToCut)
        {
            for (int i = 0; i < timeCuts.Length; i++ )
            {
                if (timeCuts[i] == minutesToCut)
                {
                    return timeCutCosts[i];
                }
            }
            return 0;
        }

        /// <summary>
        /// returns the total cost of finish now. 
        /// </summary>
        /// <param name="minutesLeft"></param>
        /// <param name="listOfTimeCuts">list of time cuts, in minute, from longest to highest. example, 17.1 minutes left will return {15,1,1,1}</param>
        /// <returns></returns>
        public static int UpgradeSpeedUp_CalculateFinishNow(double minutesLeft, out List<int> listOfTimeCuts, int minutesOfSpeedupsAllowed, ref bool allowFinishNow)
        {
            int cost = 0;
            listOfTimeCuts = new List<int>();

            // if totaal time left more than what we can do now
            if (minutesLeft > minutesOfSpeedupsAllowed)
            {
                minutesLeft = minutesOfSpeedupsAllowed;
                allowFinishNow = false;
            }
            else
            {
                allowFinishNow = true;
            }

            UpgradeSpeedUp_CalculateFinishNow(minutesLeft, 0, ref cost, ref listOfTimeCuts);
            return cost;

        }
        private static void UpgradeSpeedUp_CalculateFinishNow(double minutesLeft, int loc, ref int cost, ref List<int> listOfTimeCuts)
        {
            int nextLowerCut;
            int nextLowerCost;

           

            if (loc < timeCuts.Length - 1)
            {
                nextLowerCut = timeCuts[loc + 1];
                nextLowerCost = timeCutCosts[loc + 1];
            }
            else
            {
                nextLowerCut = 0;
                nextLowerCost = 1;
            }

            while (minutesLeft > nextLowerCut
                && (Math.Ceiling(minutesLeft) / nextLowerCut * nextLowerCost) >= timeCutCosts[loc]) 
            {
                listOfTimeCuts.Add(timeCuts[loc]);
                minutesLeft -= timeCuts[loc];
                cost += timeCutCosts[loc];
            }

            if (loc < timeCutCosts.Length - 1)
            {
                UpgradeSpeedUp_CalculateFinishNow(minutesLeft, loc + 1, ref cost, ref listOfTimeCuts);
            }

        }

        //this spell allows players to speed up a building (to Finish) for free, if its below X time left 
        public bool SpeedUpUpgradeFree(long eventID)
        {

            List<UpgradeEvent> upgrades = this.GetAllUpgradingBuildings();
            if (upgrades.Count > 0 && upgrades[0] is UpgradeEvent_CurrentlyUpgrading && upgrades[0].ID == eventID)
            {
                TimeSpan timeLeft = upgrades[0].completionTime.Subtract(DateTime.Now);

                //only allowed to do this if upgrade has 30 seconds or less left.
                if (timeLeft.TotalSeconds < 31)
                {
                    return DAL.Villages.SpeedUpUpgradeFree(owner.Realm.ConnectionStr, eventID, this.owner.ID);
                }
            }

            return false;
        }




        public int SpeedUpDowngrade(long eventID)
        {
            List<DowngradeEvent> downgrades = this.GetAllDowngradingBuildings();
            if (downgrades.Count > 0
                && downgrades[0] is DowngradeEvent_CurrentlyDowngrading
                && ((DowngradeEvent_CurrentlyDowngrading)downgrades[0]).eventID == eventID
                )
            {
                return DAL.Villages.SpeedUpDowngrade(owner.Realm.ConnectionStr, eventID, this.owner.ID);
            }

            return 0;
        }

        public enum BoostApprovalResults
        {
            boosted,
            /// <summary>
            /// either loyalty 100 or over already, or not a vp realm
            /// </summary>
            notboosted,
            notboosted_cooldownperiod
        }
       
        public BoostApprovalResults BoostApproval()
        {
            if (Loyalty < 100 && _owner.Realm.IsVPrealm)
            {
                // if used before, make sure 24 passed since last use
                object dateLastSpellUsed = this.owner.HasFlag(Player.Flags.Misc_BoostedApproval, false);
                if (dateLastSpellUsed != null)
                {
                    if (((DateTime)dateLastSpellUsed).AddDays(1) > DateTime.Now)
                    {
                        return BoostApprovalResults.notboosted_cooldownperiod;
                    }
                }

                // go ahead and boost it 
                if (DAL.Villages.BoostApproval(_owner.Realm.ConnectionStr, this.id, this.owner.ID))
                {
                    this.owner.SetFlag(Player.Flags.Misc_BoostedApproval);
                    this._loyalty = 100;
                    return BoostApprovalResults.boosted;
                }

            }
            return BoostApprovalResults.notboosted ;
        }

        /// <summary>
        /// max amount that you can transport from this village NOT including any possible moving transports. this is simply based
        /// on building level, and any village and research bonuses
        /// </summary>
        /// <returns></returns>
        public int MaxCoinTransportAmount()
        {
            BuildingType tradepost = this.owner.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.TradePost);
            int MaxAmount = (int)this.GetBuildingLevelObject(tradepost.ID).EffectAsInt;
            float villageBonus = this.VillageType.Bonus(tradepost);
            float researchBonus = this.owner.MyResearch.Bonus(tradepost);
            return Convert.ToInt32(MaxAmount * (villageBonus + 1) * (researchBonus + 1));
        }

        public bool isCity()
        {
            return (this.owner.Realm.Age.CurrentAge != null ? this.owner.Realm.Age.CurrentAge.AgeNumber >=3 : false);
        }

        VillageImgPack _imgPack;
        public VillageImgPack ImagePack
        {
            get
            {
                if (_imgPack == null)
                {
                    _imgPack = this.owner.Realm.VillageImagePack(this.isCity() ? 2 : 1);
                }
                return _imgPack;
            }
        }

        public VillageBasicB CloneandGetVillageBasicB
        {
            get
            {
                VillageBasicB thisClassVBVersion = (VillageBasicB) this;
                VillageBasicB vb= new Fbg.Bll.VillageBasicB(this.owner, this.id, false);

                vb.Init(thisClassVBVersion);


                return vb;

            }
        }
    }


    

    

    
}
