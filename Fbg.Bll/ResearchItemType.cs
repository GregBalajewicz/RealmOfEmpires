using Gmbc.Common.Diagnostics.ExceptionManagement;
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
    public class ResearchItemType
    {
        DataRow drRIType;
        DataSet dsRef;
        Realm realmRef;
        List<ResearchItem> _researchItems;

        /// <summary>
        /// this MUST BE called by someone ensuring thread safety
        /// </summary>
        public ResearchItemType(Realm realmRef, DataSet dsRef, DataRow drRIType)
        {
            this.drRIType = drRIType;
            this.dsRef = dsRef;
            this.realmRef = realmRef;

            //
            // This is CRITICAL. Why do we do this here and not in Initialize()? because BuildingTypeLevel has a Requirements property
            //  so we want to make sure that first all levels are populated/constructed, and then we initialize them.
            //
           // List<BuildingTypeLevel> levels = Levels;
        }

        public int ID
        {
            get
            {
                return (int)drRIType[Realm.CONSTS.ResearchItemTypesColumnIndex.ResearchItemTypeID];
            }
        }
        //public int MinimumLevelAllowed
        //{
        //    get
        //    {
        //        return Convert.ToInt32( drBuildingType[Realm.CONSTS.BuildingTypesColumnIndex.MinimumLevelAllowed]);
        //    }
        //}

        public List<ResearchItem> ResearchItems
        {
            get
            {
                if (_researchItems == null)
                {
                    DataRow[] drs = drRIType.GetChildRows(dsRef.Relations[Realm.CONSTS.RelIndex.RITypesToRIs]);
                    _researchItems = new List<ResearchItem>(drs.Length);
                    ResearchItem ri=null;
                    foreach (DataRow dr in drs)
                    {
                        try
                        {

                            ri = new ResearchItem(realmRef, this, dsRef, dr);
                            if (realmRef.Age != null && realmRef.Age.isFeatureActive && realmRef.Age.CurrentAge.AgeNumber < ri.AvailableInAge)
                            {
                                // only get the research items that are in the age
                                continue;
                            }

                            _researchItems.Add(ri);


                            
                        }
                        catch (Exception e)
                        {
                            BaseApplicationException bex = new BaseApplicationException("error in ResearchItems GET", e);
                            bex.AddAdditionalInformation("dr", dr);
                            bex.AddAdditionalInformation("_researchItems.Count", _researchItems.Count);
                            bex.AddAdditionalInformation("ri", ri);
                            throw bex;
                        }
                    }
                }
                return _researchItems;
            }
        }
       

        //public  BuildingTypeLevel Level(int level)
        //{
        //    foreach (BuildingTypeLevel l in Levels)
        //    {
        //        if (l.Level == level)
        //        {
        //            return l;
        //        }
        //    }
        //    return null;
        //}
        //public int  MaxLevel
        //{
        //    get
        //    {
        //        int MaxLevel = 0;
        //        foreach (BuildingTypeLevel l in Levels)
        //        {
        //            if (l.Level > MaxLevel)
        //            {
        //                MaxLevel = l.Level;
        //            }
        //        }
        //        return MaxLevel;
        //    }
        //}


        /// <summary>
        /// make sure object if fully initialized, to avoid any threading issues. 
        /// this MUST BE called by someone ensuring thread safety
        /// </summary>
        internal void Initialize()
        {
            Object o = ResearchItems;
        }
    }

}
