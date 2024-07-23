using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;
using System.Collections;

namespace Fbg.Bll
{
    public class VillageTypes :IEnumerable
    {
        DataTable _dtVTS;
        DataTable _dtVTSPTS;
        DataTable _dtVTSPS;
        List<VillageType> _villageTypes;
        public VillageTypes(DataTable dtVTS, DataTable dtVTSPTS, DataTable dtVTSPS, Realm realm)
        {
            _dtVTS = dtVTS;
            _dtVTSPS = dtVTSPS;
            _dtVTSPTS = dtVTSPTS;

            _villageTypes = new List<VillageType>();
            foreach (DataRow dr in dtVTS.Rows)
            {

                _villageTypes.Add(new VillageType((short)dr[0], dtVTSPTS, dtVTSPS, realm) { Name = (string)dr[1], rawDesc = (string)dr[2] });
            }
        }

        public VillageType this[int villageTypeID]
        {
            get
            {
                return _villageTypes.Find(delegate(VillageType vt) { return vt.ID == villageTypeID; });
            }
        }

        public bool AreBonusVillagesActive
        {
            get
            {
                return (_villageTypes.Count > 1);
            }
        }

        public List<VillageType> Types
        {
            get
            {
                return _villageTypes;
            }
        }

        public class VillageType
        {
            public string Name { get; internal set; }
            public short ID { get; internal set; }
            public Realm Realm { get; set; }
            public List<Property> Properties { get; internal set; }
            internal string rawDesc {  get; set; }

            public VillageType(short ID, DataTable dtVTSPTS, DataTable dtVTSPS, Realm realm)
            {
                Realm = realm;
                this.ID = ID;
                Properties = new List<Property>();
                foreach (DataRow dr in dtVTSPS.Select("VillageTypeID = " + ID.ToString()))
                {
                    Properties.Add(new Property((int)dr[1], (string)dr[2], dtVTSPTS, Realm));
                }
            }

            public string LargeIconUrl
            {
                get
                {
                    return rawDesc + ".png";
                }
            }
            public bool IsBonus
            {
                get
                {
                    return !String.IsNullOrEmpty(Name);
                }
            }

            /// <summary>
            /// 0 then no bonus, 0.1 means 10% bonus
            /// </summary>
            public float Bonus(BuildingType bt)
            {
                //TODO: would be good to cache this 
                float _bonus = 0;
                List<Property> ps = Properties.FindAll(delegate(Property p) { return p.BuldingItEffects == bt; });

                foreach (Property p in ps)
                {
                    _bonus += p.Value;
                }


                return _bonus;
            }


            /// <summary>
            /// 0 then no bonus, 0.1 means 10% bonus
            /// </summary>
            public float Bonus_DefenceFactor()
            {
                return Bonus(Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.DefenseTower)) + Bonus(Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Wall));
            }


            public class Property
            {
                Realm realmRef;
                BuildingType _btEffectedByProperty;


                CultureInfo ciUS = new CultureInfo("en-US");
                int _levelPropertyID;
                public float Value { get; internal set; }
                public Property(int propertyID, string value, DataTable dtVTSPTS, Realm realm)
                {
                    realmRef = realm;
                    _levelPropertyID = propertyID;
                    Value = Convert.ToSingle(value, ciUS); ;
                    DataRow drPropetyType = dtVTSPTS.Select("VillageTypePropertyTypeID = " + propertyID.ToString())[0];

                    _btEffectedByProperty = this.realmRef.BuildingType((int)drPropetyType[4]);
                }


                public BuildingType BuldingItEffects
                {
                    get
                    {

                        return _btEffectedByProperty;
                    }
                }
            }
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _villageTypes.GetEnumerator();
        }

        #endregion
    }
}
