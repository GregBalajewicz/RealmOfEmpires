using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Globalization;

namespace Fbg.Bll
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// THIS CLASS IS NOT thread safe UNLESS you first call Initialize by someone ensuring thread safety
    /// </remarks>
    public class BuildingTypeLevel : ISerializableToNameValueCollection
    {
        DataRow drLevel;
        DataSet dsRef;
        Realm realmRef;
        List<BuildingTypeLevel> _requirements;
        BuildingType buildingTypeRef;
        CultureInfo ciUS = new CultureInfo("en-US");


        public BuildingTypeLevel(Realm realmRef, BuildingType buildingTypeRef, DataSet dsRef, DataRow drLevel)
        {
            this.drLevel = drLevel;
            this.dsRef = dsRef;
            this.realmRef = realmRef;
            this.buildingTypeRef = buildingTypeRef;
        }

        public string LevelForDisplay
        {
            get
            {
                string l = "Level " + this.Level.ToString();

                if (LevelName != string.Empty)
                {
                    l += "-" + this.LevelName;
                }
                return l;
            }
        }
        public string LevelForDisplay_Compact
        {
            get
            {
                string l = this.Level.ToString();

                if (LevelName != string.Empty)
                {
                    l += "-" + this.LevelName;
                }
                return l;
            }
        }


        public int Level
        {
            get
            {
                return (int)drLevel[Realm.CONSTS.BuildingTypesLevelsColumnIndex.Level];
            }

        }

        /// <summary>
        /// returns String.Empty if no level name is specified for this level
        /// </summary>
        public string LevelName
        {
            get
            {
                if (drLevel[Realm.CONSTS.BuildingTypesLevelsColumnIndex.LevelName] is System.DBNull)
                {
                    return string.Empty;
                }
                else
                {
                    return (string)drLevel[Realm.CONSTS.BuildingTypesLevelsColumnIndex.LevelName];
                }
            }

        }

        public int Cost
        {
            get
            {
                return (int)drLevel[Realm.CONSTS.BuildingTypesLevelsColumnIndex.Cost];
            }

        }

        public TimeSpan BuildTime()
        {
                return new TimeSpan((Int64)drLevel[Realm.CONSTS.BuildingTypesLevelsColumnIndex.BuildTime]);
        }

        public int Population
        {
            get
            {
                return (int)drLevel[Realm.CONSTS.BuildingTypesLevelsColumnIndex.Population];
            }
        }

        public int Points
        {
            get
            {
                return (int)drLevel[Realm.CONSTS.BuildingTypesLevelsColumnIndex.Points];
            }
        }

        public int PopulationCumulative
        {
            get
            {
                return (int)drLevel[Realm.CONSTS.BuildingTypesLevelsColumnIndex.PopulationCumulative];
            }
        }
        public int CumulativeLevelStrength
        {
            get
            {
                return (int)drLevel[Realm.CONSTS.BuildingTypesLevelsColumnIndex.CumulativeLevelStrength];
            }
        }
        public int LevelStrength
        {
            get
            {
                return (int)drLevel[Realm.CONSTS.BuildingTypesLevelsColumnIndex.LevelStrength];
            }
        }

        /// <summary>
        /// get the build time based on the level of the village HQ as specified by the village parameter
        /// </summary>
        /// <param name="village"></param>
        /// <returns></returns>
        public TimeSpan BuildTime(Village village)
        {
            if (village == null)
            {
                throw new ArgumentNullException("Village village");
            }
            Int64 baseTime = (Int64)drLevel[Realm.CONSTS.BuildingTypesLevelsColumnIndex.BuildTime];

            float hqFactor = Convert.ToSingle(village.GetBuildingLevelObject(CONSTS.BuildingIDs.VillageHQ).Effect);

            float researchBonus = village.owner.MyResearch.Bonus(village.owner.Realm.BuildingType(CONSTS.BuildingIDs.VillageHQ));

            float effectiveFactor = hqFactor / (1 + researchBonus);

            return new TimeSpan((long)(baseTime * (effectiveFactor / 100)));
        }

        public object Effect
        {
            get
            {
                return drLevel[Realm.CONSTS.BuildingTypesLevelsColumnIndex.PropertyValue];
            }
        }

        public string EffectFormatted
        {
            get
            {
                return EffectFormatted2(Effect);
            }
        }

        private string EffectFormatted2(object effectValue)
        {
            if (EffectType == Realm.EffectType.Int)
            {
                return effectValue == null ? "" : String.Format("{0:0,0}", Convert.ToDouble(effectValue, ciUS.NumberFormat));
            }
            else if (EffectType == Realm.EffectType.Double)
            {
                //return Effect == null ? "" : Convert.ToDouble(Effect).ToString("#,###.##");
                return effectValue == null ? "" : String.Format("{0:N}", Convert.ToDouble(effectValue, ciUS.NumberFormat));
            }
            else if (EffectType == Realm.EffectType.Percent)
            {
                //return Effect == null ? "" : Convert.ToDouble(Effect).ToString("#,###.## %");
                return effectValue == null ? "" : String.Format("{0:N} %", Convert.ToDouble(effectValue, ciUS.NumberFormat));
            }
            else
            {
                return effectValue.ToString();
            }

        }

        public string EffectFormattedInclResearch(double researchBonusMultiplier) 
        {
            return EffectFormatted2(Convert.ToDouble(Effect, ciUS.NumberFormat) * researchBonusMultiplier);
        }


        //public string EffectFormattedInclAllBonuses(MyResearch myresearch, Village village)
        //{
        //    float factor;
        //    float buildingLevelFactor;
        //    float villageBonus;
        //    float researchBonus;
        //    BuildingTypeLevel btl;

        //    //
        //    // get the recruit time VILLAGE BONUS if any
        //    //
        //    villageBonus = village.VillageType.Bonus(buildingTypeRef);
        //    //
        //    // get the recruit time RESEARCH BONUS if any
        //    //
        //    researchBonus = myresearch.Bonus(buildingTypeRef);

        //    factor = 
        //    return EffectFormatted2(Convert.ToDouble(Effect, ciUS.NumberFormat) * (villageBonus +1) * (researchBonus + 1));
        //}
        public string EffectName
        {
            get
            {
                return (string)drLevel[Realm.CONSTS.BuildingTypesLevelsColumnIndex.EffectName];
            }
        }

        public Realm.EffectType EffectType
        {
            get
            {
                return (Realm.EffectType)drLevel[Realm.CONSTS.BuildingTypesLevelsColumnIndex.EffectType];
            }
        }

        /// <summary>
        /// will throw an exception if effect cannot be converted to int. 
        /// </summary>
        public int EffectAsInt
        {
            get
            {
                object o = this.Effect;
                double d = Convert.ToDouble(o, ciUS.NumberFormat);
                return Convert.ToInt32(Math.Round(d, 0));
            }
        }
        /// <summary>
        /// will throw an exception if effect cannot be converted to int. 
        /// </summary>
        /// <param name="researchBonusMultiplier">1 if no bonus, 1.x or higher if there is bonus </param>
        public int EffectAsIntInclResearch(double researchBonusMultiplier) 
        {
            object o = this.Effect;
            double d = Convert.ToDouble(o, ciUS.NumberFormat);
            d *= researchBonusMultiplier;
            return Convert.ToInt32(Math.Round(d, 0));
        }

        public List<BuildingTypeLevel> Requirements
        {
            get
            {
                if (_requirements == null)
                {
                    BuildingType buildingType;
                    BuildingTypeLevel level;
                    DataRow[] drs = drLevel.GetChildRows(dsRef.Relations[Realm.CONSTS.RelIndex.BuildingTypeLevelsToRequirements]);                    
                    _requirements = new List<BuildingTypeLevel>(drs.Length);
                    foreach (DataRow dr in drs)
                    {
                        buildingType = realmRef.BuildingType((int)dr[Realm.CONSTS.dtBuildingTypesLevelRequirementsColumnIndex.RequiredBuildingTypeID]);
                        level = buildingType.Level((int)dr[Realm.CONSTS.dtBuildingTypesLevelRequirementsColumnIndex.RequiredLevel]);

                        _requirements.Add(level);
                    }

                }
                return _requirements;
            }
        }

        public BuildingType Building
        {
            get
            {
                return buildingTypeRef;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelID"></param>
        /// <returns>null if this is the last level</returns>
        public BuildingTypeLevel GetNextLevel()
        {
            return GetNextLevel(this.Level);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelID"></param>
        /// <returns>null if levelID is the last level for this building type</returns>
        public BuildingTypeLevel GetNextLevel(int levelID)
        {
            return buildingTypeRef.Level(levelID + 1);
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
                unsatisfiedReq =  Requirements;
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


        #region ISerializableToNameValueCollection Members

        public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
        {
            SerializeToNameValueCollection(col, "");
        }


        public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col, string namePrePend)
        {
            try
            {
                string pre = namePrePend + "BTL[" + this.buildingTypeRef.ID.ToString() + "," + this.Level.ToString() + "]";

                if (col == null)
                {
                    ExceptionManager.Publish("Error in BuildingTypeLevel.SerializeToNameValueCollection. argument 'col' is null");
                }
                else
                {
                    col.Add(pre + "LevelName", this.LevelName);
                    col.Add(pre + "Effect", Effect == null ? "null" : this.Effect.ToString());
                    col.Add(pre + "Cost", this.Cost.ToString());
                    col.Add(pre + "Points", this.Points.ToString());
                    col.Add(pre + "EffectName", this.EffectName.ToString());
                    col.Add(pre + "EffectFormatted", this.EffectFormatted.ToString());
                    col.Add(pre + "EffectType", this.EffectType.ToString());
                    BaseApplicationException.AddAdditionalInformation(col, pre + "drLevel", drLevel);
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish("Error in BuildingTypeLevel.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + ExceptionManager.SerializeToString(e));
            }
        }

        #endregion


        /// <summary>
        /// make sure object if fully initialized, to avoid any threading issues. 
        /// this MUST BE called by someone ensuring thread safety
        /// </summary>
        internal void Initialize()
        {
            //
            // make sure object if fully initialized, to avoid any threading issues
            //
            List<BuildingTypeLevel> l = Requirements;
        }
    }
}
