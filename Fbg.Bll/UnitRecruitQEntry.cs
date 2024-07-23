using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Fbg.Bll
{

    /// <summary>
    /// First entry in a recruitment Q for a building
    /// </summary>
    public class UnitRecruitQEntry_First : UnitRecruitQEntry
    {
        public UnitRecruitQEntry_First(Village villageRef, DataRow drQEntry) : base(villageRef, drQEntry)
        {
        }


        public DateTime RecruitmentCompletedOn
        {
            get
            {
                if (_recruitmentCompletedOn == DateTime.MinValue)
                {
                    DateTime recruitmentBegunOn = (DateTime)_dr[Village.CONSTS.UnitRecruitQEntreyColumnIndexes.DateStarted];

                    _recruitmentCompletedOn = recruitmentBegunOn.Add(new TimeSpan(UnitType.RecruitmentTime(_villageRef).Ticks * this.Count));
                }
                return _recruitmentCompletedOn;
            }
        }
    }

    /// <summary>
    /// entry in a recruitment Q for a building.
    /// </summary>
    public class UnitRecruitQEntry
    {
        protected Village _villageRef;
        protected DataRow _dr;
        protected UnitType _unitType;
        protected BuildingType _recruitmentBuilding;
        protected TimeSpan _totalRecruitTime = TimeSpan.MinValue;
        protected DateTime _recruitmentCompletedOn = DateTime.MinValue;

        public UnitRecruitQEntry(Village villageRef, DataRow drQEntry)
        {
            _villageRef = villageRef;
            _dr = drQEntry;
        }


        public BuildingType RecruitmentBuilding
        {
            get
            {
                if (_recruitmentBuilding == null)
                {
                    _recruitmentBuilding = _villageRef.owner.Realm.BuildingType((int)_dr[Village.CONSTS.UnitRecruitQEntreyColumnName.BuildingTypeID]);
                }
                return _recruitmentBuilding;
            }
        }

        public Int64 QEntryID
        {
            get
            {
                return (Int64)_dr[Village.CONSTS.UnitRecruitQEntreyColumnName.EntryID];
            }
        }

        public UnitType UnitType
        {
            get
            {
                if (_unitType == null)
                {
                    _unitType = _villageRef.owner.Realm.GetUnitTypesByID((int)_dr[Village.CONSTS.UnitRecruitQEntreyColumnName.UnitTypeID]);
                }
                return _unitType;
            }
            
        }

        public int TotalCost
        {
            get
            {
                return (int)_dr[Village.CONSTS.UnitRecruitQEntreyColumnName.UnitCost] * this.Count;
            }
        }


        public TimeSpan TotalRecruitTime
        {
            get
            {
                if (_totalRecruitTime == TimeSpan.MinValue)
                {
                    _totalRecruitTime = new TimeSpan(UnitType.RecruitmentTime(_villageRef).Ticks * this.Count);
                }
                return _totalRecruitTime;
            }
        }


        

        public int Count
        {
            get
            {
                return (int)_dr[Village.CONSTS.UnitRecruitQEntreyColumnName.Count];
            }
        }
        public static int CancelRecruit(Player owner, int QEntryID,int VillageID)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner is null");

            }


            return DAL.Units.CancelRecruit (owner.Realm.ConnectionStr, QEntryID, VillageID);
        }
        public static void CancelGovernerRecruit(Player owner,  int VillageID)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner is null");

            }
             DAL.Units.CancelGovernerRecruit(owner.Realm.ConnectionStr, VillageID,owner.ID);
             owner.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.GovRecruit, string.Empty, string.Format("Cancel of governor recruitment in VillageID:{0} requested", VillageID));

        }
    }
}
