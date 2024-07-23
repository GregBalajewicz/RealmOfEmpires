using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Fbg.Bll
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// THIS CLASS IS NOT thread safe UNLESS you first call Initialize by someone ensuring thread safety
    /// </remarks>
    public class BuildingType
    {
        DataRow drBuildingType;
        DataSet dsRef;
        Realm realmRef;
        List<BuildingTypeLevel> _Levels;

        /// <summary>
        /// this MUST BE called by someone ensuring thread safety
        /// </summary>
        public BuildingType(Realm realmRef, DataSet dsRef, DataRow drBuildingType)
        {
            this.drBuildingType = drBuildingType;
            this.dsRef = dsRef;
            this.realmRef = realmRef;

            //
            // This is CRITICAL. Why do we do this here and not in Initialize()? because BuildingTypeLevel has a Requirements property
            //  so we want to make sure that first all levels are populated/constructed, and then we initialize them.
            //
            List<BuildingTypeLevel> levels = Levels;
        }
        public string Name
        {
            get
            {
                return (string)drBuildingType[Realm.CONSTS.BuildingTypesColumnIndex.BuildingName];
            }
        }

        public int ID
        {
            get
            {
                return (int)drBuildingType[Realm.CONSTS.BuildingTypesColumnIndex.BuildingTypeID];
            }
        }
        public int MinimumLevelAllowed
        {
            get
            {
                return Convert.ToInt32( drBuildingType[Realm.CONSTS.BuildingTypesColumnIndex.MinimumLevelAllowed]);
            }
        }

        public List<BuildingTypeLevel> Levels
        {
            get
            {
                if (_Levels == null)
                {
                    DataRow[] drs = drBuildingType.GetChildRows(dsRef.Relations[Realm.CONSTS.RelIndex.BuildingTypesToLevels]);
                    _Levels = new List<BuildingTypeLevel>(drs.Length);
                    foreach (DataRow dr in drs)
                    {
                        _Levels.Add(new BuildingTypeLevel(realmRef, this, dsRef, dr));
                    }
                }
                return _Levels;
            }
        }

        public  BuildingTypeLevel Level(int level)
        {
            foreach (BuildingTypeLevel l in Levels)
            {
                if (l.Level == level)
                {
                    return l;
                }
            }
            return null;
        }
        public int  MaxLevel
        {
            get
            {
                int MaxLevel = 0;
                foreach (BuildingTypeLevel l in Levels)
                {
                    if (l.Level > MaxLevel)
                    {
                        MaxLevel = l.Level;
                    }
                }
                return MaxLevel;
            }
        }


        /// <summary>
        /// make sure object if fully initialized, to avoid any threading issues. 
        /// this MUST BE called by someone ensuring thread safety
        /// </summary>
        internal void Initialize()
        {
            foreach (BuildingTypeLevel l in Levels)
            {
                l.Initialize();
            }
        }
    }

}
