using System;
using System.Collections.Generic;
using System.Text;
using System.Data;


namespace Fbg.Bll
{
    public class UnitInVillage
    {
        private Village _villageRef;
        private UnitType _ut;
        private int _yourUnitsCurrentlyInVillageCount;
        private int _yourUnitsTotalCount;
        private int _supportUnitCount;
        private int _currentlyRecruiting;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="villageRef"></param>
        /// <param name="drVillageUnits">This may be in null if there are no units of this type</param>
        /// <param name="drSupportingUnits">This may be in null if there are no units of this type supporting</param>
        public UnitInVillage(Village villageRef, UnitType ut, DataRow drVillageUnits
            , DataRow drSupportingUnits, Dictionary<BuildingType, List<UnitRecruitQEntry>> qEntriedByBuildingType)
        {
            _villageRef = villageRef;
            _ut = ut;

            _yourUnitsCurrentlyInVillageCount = 0;
            _yourUnitsTotalCount = 0;
            _supportUnitCount = 0;

            if (drVillageUnits != null)
            {
                _yourUnitsCurrentlyInVillageCount = (int)drVillageUnits[Village.CONSTS.UnitsColumnIndex.CurrentCount];
                _yourUnitsTotalCount = (int)drVillageUnits[Village.CONSTS.UnitsColumnIndex.TotalCount];
            }
            if (drSupportingUnits != null)
            {
                _supportUnitCount = (int)drSupportingUnits[Village.CONSTS.SuppportUnitsColumnIndex.Count]; ;
            }

            _currentlyRecruiting = AddUpCurrentlyRecruiting(qEntriedByBuildingType, ut);
        }

        private int AddUpCurrentlyRecruiting(Dictionary<BuildingType, List<UnitRecruitQEntry>> qEntriedByBuildingType, UnitType ut)
        {
            int count = 0;
            foreach (KeyValuePair<BuildingType, List<UnitRecruitQEntry>> kvp in qEntriedByBuildingType)
            {
                if (kvp.Key == ut.RecruitmentBuilding)
                {
                    foreach (UnitRecruitQEntry entry in kvp.Value)
                    {
                        if (entry.UnitType  == ut)
                        {
                            count += entry.Count;
                        }
                    }
                }
            }
            return count;
        }


        public UnitType UnitType
        {
            get
            {
                return _ut;
            }
        }

        public int YourUnitsCurrentlyInVillageCount
        {
            get
            {
                return _yourUnitsCurrentlyInVillageCount;
            }
        }
        public int YourUnitsTotalCount
        {
            get
            {
                return _yourUnitsTotalCount;
            }
        }
        public int SupportCount
        {
            get
            {
                return _supportUnitCount;
            }
        }
        public int TotalNowInVillageCount
        {
            get
            {
                return SupportCount + YourUnitsCurrentlyInVillageCount;
            }
        }

        /// <summary>
        /// number of units currently being recruited.
        /// </summary>
        public int CurrentlyRecruiting
        {
            get
            {
                return _currentlyRecruiting;
            }
        }

    }
}
