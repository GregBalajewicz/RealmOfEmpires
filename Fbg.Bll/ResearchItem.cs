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
    public class ResearchItem : ISerializableToNameValueCollection
    {
        DataRow drRI;
        DataSet dsRef;
        Realm realmRef;
        List<ResearchItem> _requirements;
        ResearchItemType _ritRef;
        BuildingType _btEffectedByProperty;
        CultureInfo ciUS = new CultureInfo("en-US");

        public ResearchItem(Realm realmRef, ResearchItemType ritRef, DataSet dsRef, DataRow drRI)
        {
            this.drRI = drRI;
            this.dsRef = dsRef;
            this.realmRef = realmRef;
            this._ritRef = ritRef;

            
            if (!(drRI[Realm.CONSTS.ResearchItemsColumnIndex.BuildingTypeID] is DBNull))
            {
                _btEffectedByProperty = this.realmRef.BuildingType((int)drRI[Realm.CONSTS.ResearchItemsColumnIndex.BuildingTypeID]);
            }


            //utUnlocks = realmRef.GetUnitTypesByID(
        }


        public UnitType UnlocksUnitTye
        {
            //TODO : cache this 
            get
            {


                DataRow[] drs = drRI.GetChildRows(dsRef.Relations[Realm.CONSTS.RelIndex.RIToUTReq]);
                if (drs.Length > 0)
                {
                    return realmRef.GetUnitTypesByID((int)drs[0][Realm.CONSTS.UnitTypeRecruitResearchReqColumnIndex.UnitTypeID]);
                }
                return null;
            }
        }

        public ResearchItemType ResearchItemType
        {
            get
            {
                return _ritRef;
            }
        }

        /// <summary>
        /// full url
        /// </summary>
        public string ImageUrl
        {
            get
            {
                //string url = Properties.Research.ResourceManager.GetString(String.Format("RIT_{0}_RI_{1}_Img", _ritRef.ID, ID));
                //if (string.IsNullOrEmpty(url))
                //{
                //    return "http://static.realmofempires.com/images/stories/Story_TitleKnight2.jpg";
                //}
                //return url;
                return (string)drRI[Realm.CONSTS.ResearchItemsColumnIndex.Img1];
            }
        }
        /// <summary>
        /// full url
        /// </summary>
        public string ImageUrl2
        {
            get
            {
                //string url = Properties.Research.ResourceManager.GetString(String.Format("RIT_{0}_RI_{1}_Img", _ritRef.ID, ID));
                //if (string.IsNullOrEmpty(url))
                //{
                //    return "http://static.realmofempires.com/images/stories/Story_TitleKnight2.jpg";
                //}
                //return url;
                if (String.IsNullOrEmpty((string)drRI[Realm.CONSTS.ResearchItemsColumnIndex.Img2])) 
                {
                    return ImageUrl;
                }else {
                return (string)drRI[Realm.CONSTS.ResearchItemsColumnIndex.Img2];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SpriteSheetLocX
        {
            get
            {
                if (drRI[Realm.CONSTS.ResearchItemsColumnIndex.SpriteSheetLocX] is DBNull)
                {
                    return 0;
                }
                else
                {
                    return (int)drRI[Realm.CONSTS.ResearchItemsColumnIndex.SpriteSheetLocX];
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int SpriteSheetLocY
        {
            get
            {
                if (drRI[Realm.CONSTS.ResearchItemsColumnIndex.SpriteSheetLocY] is DBNull)
                {
                    return 0;
                }
                else
                {
                    return (int)drRI[Realm.CONSTS.ResearchItemsColumnIndex.SpriteSheetLocY];
                }
            }
        }
         

        public TimeSpan ResearchTime
        {
            get
            {
                return new TimeSpan((Int64)drRI[Realm.CONSTS.ResearchItemsColumnIndex.ResearchTime]);
            }
        }

        public int ID
        {
            get
            {
                return (int)drRI[Realm.CONSTS.ResearchItemsColumnIndex.ResearchItemID];
            }

        }

        public string Name
        {
            get
            {
                //return Properties.Research.ResourceManager.GetString(String.Format("RIT_{0}_RI_{1}", _ritRef.ID, ID));
                return (string)drRI[Realm.CONSTS.ResearchItemsColumnIndex.Name];
            }
        }
        public string DEsc
        {
            get
            {
                //return Properties.Research.ResourceManager.GetString(String.Format("RIT_{0}_RI_{1}", _ritRef.ID, ID));
                return (string)drRI[Realm.CONSTS.ResearchItemsColumnIndex.Desc];
            }
        }

        public int Cost
        {
            get
            {
                return (int)drRI[Realm.CONSTS.ResearchItemsColumnIndex.Cost];
            }

        }

        public enum TypesOfResearchItems
        {
            BuildingEffect,
            UnitUnlock,
            MiscEffect
        }

        /// <summary>
        /// can be DB NULL. 
        /// It is valid for research items that have properties (building properties, and non-building properties) 
        /// </summary>
        public TypesOfResearchItems TypesOfResearchItem
        {
            get
            {
                if (Property_BuldingItEffects != null)
                {
                    return TypesOfResearchItems.BuildingEffect;
                }
                else if (UnlocksUnitTye != null)
                {
                    return TypesOfResearchItems.UnitUnlock;
                }
                else
                {
                    return TypesOfResearchItems.MiscEffect;
                }
            }
        }


        /// <summary>
        /// can be DB NULL. 
        /// It is valid for research items that have properties (building properties, and non-building properties) 
        /// </summary>
        public int ResearchPropertyTypeID 
        {
            get
            {
                return drRI[Realm.CONSTS.ResearchItemsColumnIndex.ResearchTypePropertyID] is DBNull ? 0 : (int)drRI[Realm.CONSTS.ResearchItemsColumnIndex.ResearchTypePropertyID];
            }
        }

        public object Property
        {
            get
            {
                return drRI[Realm.CONSTS.ResearchItemsColumnIndex.PropertyValue];
            }
        }


        public string PropertyFormatted
        { 
            get
            {
                //if (EffectType == Realm.EffectType.Int)
                //{
                //    return Effect == null ? "" : String.Format("{0:0,0}", Convert.ToDouble(Effect));
                //}
                //else if (EffectType == Realm.EffectType.Double)
                //{
                //    //return Effect == null ? "" : Convert.ToDouble(Effect).ToString("#,###.##");
                //    return Effect == null ? "" : String.Format("{0:N}", Convert.ToDouble(Effect));
                //}
                //else if (EffectType == Realm.EffectType.Percent)
                //{
                    //return Effect == null ? "" : Convert.ToDouble(Effect).ToString("#,###.## %");
                return Property is DBNull ? "" : String.Format("{0}%", Convert.ToDouble(this.Property, ciUS.NumberFormat) * 100);
                //}
                //else
                //{
                //    return Effect.ToString();
                //}
            }
        }

        public string EffectName
        {
            get
            {
                if (drRI[Realm.CONSTS.ResearchItemsColumnIndex.EffectName] is DBNull)
                {
                    return string.Empty;
                }
                else
                {
                   
                    return (string)drRI[Realm.CONSTS.ResearchItemsColumnIndex.EffectName];
                }
            }
        }

        public BuildingType Property_BuldingItEffects
        {
            get
            {
               
                return _btEffectedByProperty;
            }
        }

        /// <summary>
        /// will throw an exception if effect cannot be converted to int. 
        /// </summary>
        public int PropertyAsInt
        {
            get
            {
                object o = this.Property;
                double d = Convert.ToDouble(o);
                return Convert.ToInt32(Math.Round(d, 0));
            }
        }
        /// <summary>
        /// will throw an exception if effect cannot be converted to float. 
        /// </summary>
        public float PropertyAsFloat
        {
            get
            {
                object o = this.Property;
                return Convert.ToSingle(o, CONSTS.ciUS);
            }
        }

       

        public List<ResearchItem> Requirements
        {
            get
            {
                if (_requirements == null)
                {
                    ResearchItem ri;
                    DataRow[] drs = drRI.GetChildRows(dsRef.Relations[Realm.CONSTS.RelIndex.RIToReq]);
                    _requirements = new List<ResearchItem>(drs.Length);
                    foreach (DataRow dr in drs)
                    {
                        ri = realmRef.Research.ResearchItemByID((int)dr[Realm.CONSTS.ResearchItemReqColumnIndex.RequiredResearchItemTypeID]
                            , (int)dr[Realm.CONSTS.ResearchItemReqColumnIndex.RequiredResearchItemID]);

                        _requirements.Add(ri);
                    }

                }
                return _requirements;
            }
        }

        /// <summary>
        /// returns the age number when this research item is available, and 0 if not specified
        /// </summary>
        public int AvailableInAge
        {
            get
            {
                return (drRI[Realm.CONSTS.ResearchItemsColumnIndex.AvailInAge] is DBNull ? 0 : (int)drRI[Realm.CONSTS.ResearchItemsColumnIndex.AvailInAge]);
            }
        }


        #region ISerializableToNameValueCollection Members

        public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
        {
            SerializeToNameValueCollection(col, "");
        }


        public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col, string namePrePend)
        {
            //try
            //{
            //    string pre = namePrePend + "BTL[" + this.buildingTypeRef.ID.ToString() + "," + this.Level.ToString() + "]";

            //    if (col == null)
            //    {
            //        ExceptionManager.Publish("Error in BuildingTypeLevel.SerializeToNameValueCollection. argument 'col' is null");
            //    }
            //    else
            //    {
            //        col.Add(pre + "LevelName", this.LevelName);
            //        col.Add(pre + "Effect", Effect == null ? "null" : this.Effect.ToString());
            //        col.Add(pre + "Cost", this.Cost.ToString());
            //        col.Add(pre + "Points", this.Points.ToString());
            //        col.Add(pre + "EffectName", this.EffectName.ToString());
            //        col.Add(pre + "EffectFormatted", this.EffectFormatted.ToString());
            //        col.Add(pre + "EffectType", this.EffectType.ToString());
            //        BaseApplicationException.AddAdditionalInformation(col, pre + "drLevel", drLevel);
            //    }
            //}
            //catch (Exception e)
            //{
            //    ExceptionManager.Publish("Error in BuildingTypeLevel.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + ExceptionManager.SerializeToString(e));
            //}
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
            //List<BuildingTypeLevel> l = Requirements;
        }
    }
}
