using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.Bll
{
    public class UnitType :Gmbc.Common.Diagnostics.ExceptionManagement.ISerializableToNameValueCollection2
    {
        protected List<BuildingTypeLevel>   _requirements;
        protected List<ResearchItem> _requirementsResearch;
        protected BuildingType _recruitmentBuilding;
        protected List<BuildingType>        _attackableBuildings;
        protected Dictionary<UnitType, int> _defenseStrength;

        protected DataRow _drUnitType;
        protected DataSet _dsRef;
        protected Realm _realmRef;

        protected DataRow[] _drsDefenseStrength;
        protected DataRow[] _drsAttackableBuildings;



        public UnitType(Realm realmRef, DataSet dsRef, DataRow drUnitType, DataRow[] drsDefense, DataRow[] drsAttackableBuildings)
        {
            this._drUnitType             = drUnitType;
            this._dsRef                  = dsRef;
            this._realmRef               = realmRef;
            this._drsDefenseStrength     = drsDefense;
            this._drsAttackableBuildings = drsAttackableBuildings;
        }

        public int ID
        {
            get
            {
                return (int)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.UnitTypeID];
            }
        }

        public string Name
        {
            get
            {
                return (string)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.Name];
            }
        }
        public string ShortName
        {
            get
            {
                switch(ID) {
                    case 6:
                        return "Knight";
                    case 5 :
                        return "Lt Cav";
                    case 2:
                        return "Inf";
                    case 11:
                        return "Militia";
                    case 8:
                        return "Treb";
                    default:
                        return Name;
                }
            }
        }

        public string Desc
        {
            get
            {
                return (string)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.Description];
            }
        }

        public int Pop
        {
            get
            {
                return (int)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.Population];
            }
        }

        /// <summary>
        /// calling this property may cause the database to be called everytime as some unit prices
        /// (ex, lord) depend on the state of your villages. so cache this if needed multiple time in quick succession. 
        /// NOTE - you can pass in NULL for the player parameter; in this case you will get the BASE cost of a unit. 
        /// </summary>
        virtual public int Cost(Player player)
        {
            return (int)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.Cost];
        }


        /// <summary>
        /// Fields per hour
        /// </summary>
        public int Speed
        {
            get
            {
                return (int)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.Speed];
            }
        }

        /// <summary>
        /// How much silver (plunder) the unit can carry
        /// </summary>
        public int CarryCapacity
        {
            get
            {
                return (int)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.CarryCapacity];
            }
        }

        /// <summary>
        /// Fields per hour
        /// </summary>
        public int AttackStrength
        {
            get
            {
                return (int)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.AttackStrength];
            }
        }

        /// <summary>
        /// converts img name to a long img name ... this should be moved into a Util file ...
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string Img2LImg(string url)
        {
        //#if (DEBUG)
        //    url = url.Replace("https://static.realmofempires.com", ".");
        //#endif
            return url.Replace(".png", "Large.png");
        }
        public string IconUrl_ThemeM
        {
            get
            {
                return ((string)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.ImageIcon]).Replace(".png", "_M.png");;
            }
        }
        public string IconUrl
        {
            get
            {
                return ((string)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.ImageIcon]);
            }
        }
        public string LargeIconUrl
        {
            get
            {
                return Img2LImg((string)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.ImageIcon]);
            }
        }
        public string Image
        {
            get
            {
                return ((string)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.Image]);
            }
        }
        public string LargeImage
        {
            get
            {
                return Img2LImg((string)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.Image]);
            }
        }
        public int SpyAbility
        {
            get
            {
                return (int)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.SpyAbility];
            }
        }
        public int CounterSpyAbility
        {
            get
            {
                return (int)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.CounterSpyAbility ];
            }
        }

        /// <summary>
        /// This property give you defense strength of this unit when attacked by other units. 
        /// </summary>
        public Dictionary<UnitType, int> DefenseStrength
        {
            get
            {
                if (_defenseStrength == null)
                {
                    UnitType unitType;
                    _defenseStrength = new Dictionary<UnitType, int>(_drsDefenseStrength.Length);
                    foreach (DataRow dr in _drsDefenseStrength)
                    {
                        unitType=  _realmRef.GetUnitTypesByID((int)dr[Realm.CONSTS.UnitTypeDefenseColIndex.DefendAgainstUnitTypeID]);
                        _defenseStrength.Add(unitType, (int)dr[Realm.CONSTS.UnitTypeDefenseColIndex.DefenseStrength]);
                    }

                }
                return _defenseStrength;
            }
        }

        int _defenseStrengthAvg = Int32.MinValue;
        /// <summary>
        /// This property give you average defense strength of this unit when attacked. 
        /// </summary>
        public int DefenseStrength_Avg
        {
            get
            {
                if (_defenseStrengthAvg == Int32.MinValue) {
                    int[] temp = new int[DefenseStrength.Values.Count()];
                    DefenseStrength.Values.CopyTo(temp, 0);
                    _defenseStrengthAvg = Convert.ToInt32(temp.Where(s=> s >0).Average());
                }
                return _defenseStrengthAvg;
            }
        }
        /// <summary>
        /// This property give you the list of buildings this unit can attack. 
        /// This may be an empty list
        /// </summary>
        public List<BuildingType> AttackableBuildings
        {
            get
            {
                if (_attackableBuildings == null)
                {
                    BuildingType buildingType;
                    _attackableBuildings = new List<BuildingType>(_drsAttackableBuildings.Length);
                    foreach (DataRow dr in _drsAttackableBuildings)
                    {
                        buildingType = _realmRef.BuildingType((int)dr[Realm.CONSTS.UnitOnBuildingAttackColIndex.BuildingTypeID]);

                        _attackableBuildings.Add(buildingType);
                    }
                }
                return _attackableBuildings;
            }
        }

        public BuildingType RecruitmentBuilding
        {
            get
            {
                if (_recruitmentBuilding == null ) {
                    _recruitmentBuilding = _realmRef.BuildingType((int)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.BuildingTypeID]);
                } 
                return _recruitmentBuilding;
            }
        }

        public List<BuildingTypeLevel> Requirements
        {
            get
            {
                if (_requirements == null)
                {
                    BuildingType buildingType;
                    BuildingTypeLevel level;
                    DataRow[] drs = _drUnitType.GetChildRows(_dsRef.Relations[Realm.CONSTS.RelIndex.UnitTypesToReq]);
                    _requirements = new List<BuildingTypeLevel>(drs.Length);
                    foreach (DataRow dr in drs)
                    {
                        buildingType = _realmRef.BuildingType((int)dr[Realm.CONSTS.UnitTypesRecruitReqColumnIndex.BuildingTypeID]);
                        level = buildingType.Level((int)dr[Realm.CONSTS.UnitTypesRecruitReqColumnIndex.Level]);

                        _requirements.Add(level);
                    }

                }
                return _requirements;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>NOTE: be careful how this is initialized. this needs research to be initalized. Make sure research is initialized before calling this </remarks>
        public List<ResearchItem> Requirements_Research
        {    
            get
            {
                if (_requirementsResearch == null)
                {
                    _requirementsResearch = _realmRef.Research.AllResearchItems.FindAll(delegate(ResearchItem ri) { return ri.UnlocksUnitTye == this; });
                }
                return _requirementsResearch;
            }
        }


        /// <summary>
        /// get the base recruitment time for this unit
        /// </summary>
        /// <returns></returns>
        public TimeSpan RecruitmentTime()
        {
            return new TimeSpan((Int64)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.RecruitmentTime]);
        }
        /// <summary>
        /// get the build time based on the level of the building as specified by the village parameter.
        /// if no reruitment building, returns based on level 1 building
        /// </summary>
        /// <param name="village"></param>
        /// <returns></returns>
        public TimeSpan RecruitmentTime(Village village)
        {
            if (village == null)
            {
                throw new ArgumentNullException("Village village");
            }
            try
            {
                float factor;
                float buildingLevelFactor;
                float villageBonus;
                float researchBonus;
                BuildingTypeLevel btl;
                Int64 baseTime;
                //
                // get base time for unit recuitment
                baseTime = (Int64)_drUnitType[Realm.CONSTS.UnitTypesColumnIndex.RecruitmentTime];
                //
                // get the recruit time VILLAGE BONUS if any
                //
                villageBonus = village.VillageType.Bonus(this.RecruitmentBuilding);
                //
                // get the recruit time RESEARCH BONUS if any
                //
                researchBonus = village.owner.MyResearch.Bonus(this.RecruitmentBuilding);
                //
                // get the level of the recruitment building in this village
                //
                btl = village.GetBuildingLevelObject(this.RecruitmentBuilding.ID);
                if (btl == null)
                {
                    //
                    // if this village has no recruitment building, then assume 100% recruitment factor
                    //  this may happen in case of a recruitment building begin destroyed but 
                    //  some units are still recruiting. 
                    //
                    buildingLevelFactor = 100;
                }
                else
                {
                    //
                    // get the effect of this building (recruit time factory)
                    //
                    try
                    {
                        buildingLevelFactor = Convert.ToSingle(btl.Effect);
                    }
                    catch (Exception e)
                    {
                        BaseApplicationException be = new BaseApplicationException("Cannot get effect of this building", e);
                        be.AdditionalInformation.Add("this.RecruitmentBuilding.Name", this.RecruitmentBuilding.Name.ToString());
                        btl.SerializeToNameValueCollection(be.AdditionalInformation, "Btl");
                        this.SerializeToNameValueCollection(be.AdditionalInformation);
                        village.SerializeToNameValueCollection(be.AdditionalInformation);
                        throw be;
                    }
                }

                factor = buildingLevelFactor / ((1 + villageBonus) * (1 + researchBonus)*100);
                //
                // calculate the recruitment time
                //
                return new TimeSpan(Convert.ToInt64(Math.Ceiling(baseTime * (decimal)(factor))));
            }
            catch (Exception e)
            {
                BaseApplicationException be = new BaseApplicationException("Error in RecruitmentTime(Village village)", e);
                this.SerializeToNameValueCollection(be.AdditionalInformation);
                throw be;
            }
        }
        /// <summary>
        /// Get the unitt attack stregnth againest  building 
        /// </summary>
        /// <param name="BuildingID"></param>
        /// <returns></returns>
        public int GetBuildingAttackStrength(int BuildingID)
        {

            foreach (DataRow dr in _drsAttackableBuildings)
            {
                if ((int)dr[Realm.CONSTS.UnitOnBuildingAttackColIndex.BuildingTypeID] == BuildingID)
                {
                    return (int)dr[Realm.CONSTS.UnitOnBuildingAttackColIndex.AttackStrength];
                }
                
            }
            return 0;
        }

        /// <summary>
        /// returns a list of Unsatisfied requirements.
        ///  if list if empty, then there is either no requirements or all requirements are already satisfied. 
        /// </summary>
        /// <param name="village"></param>
        /// <returns></returns>
        public List<BuildingTypeLevel> GetUnsatisfiedRequirements(Village village)
        {
            List<BuildingTypeLevel> unsatisfiedReq;
            if (Requirements.Count == 0)
            {
                unsatisfiedReq = Requirements;
            }
            else
            {
                unsatisfiedReq = new List<BuildingTypeLevel>(Requirements.Count);
                foreach (BuildingTypeLevel level in Requirements)
                {
                    if (village.GetBuildingLevel(level.Building.ID) < level.Level)
                    {
                        unsatisfiedReq.Add(level);
                    }
                }
            }

            return unsatisfiedReq;
        }

        /// <summary>
        /// returns a list of Unsatisfied reqsearch requirements.
        ///  if list if empty, then there is either no requirements or all requirements are already satisfied. 
        /// </summary>
        /// <param name="village"></param>
        /// <returns></returns>
        public List<ResearchItem> GetUnsatisfiedResearchRequirements(Village village)
        {
            List<ResearchItem> unsatisfiedReq;
            if (Requirements_Research.Count == 0)
            {
                unsatisfiedReq = Requirements_Research;
            }
            else
            {
                unsatisfiedReq = new List<ResearchItem>(Requirements_Research.Count);
                foreach (ResearchItem ri in Requirements_Research)
                {
                    if (!village.owner.MyResearch.IsCompleted(ri))
                    {
                        unsatisfiedReq.Add(ri);
                    }
                }
            }

            return unsatisfiedReq;
        }

        #region ISerializableToNameValueCollection2 Members

        public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col, string namePrePend)
        {
            if (col == null)
            {
                ExceptionManager.Publish("In Fbg.Bll.UnitType.SerializeToNameValueCollection and 'col' param is null");
            }
            else
            {
                try
                {
                    string pre = namePrePend + "UT[" + this.ID.ToString() + "," + this.Name + "]";
                    col.Add(pre + "Cost", this.Cost(null).ToString());
                    col.Add(pre + "Pop", this.Pop.ToString());
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish("Error in Fbg.Bll.UnitType.SerializeToNameValueCollection.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + ExceptionManager.SerializeToString(e));
                    col.Add("Fbg.Bll.UnitType.SerializeToNameValueCollection.SerializeToNameValueCollection", "Error:" + ExceptionManager.SerializeToString(e));
                }
            }
        }

        public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
        {
            SerializeToNameValueCollection(col, string.Empty);
        }

        #endregion
    }
}
