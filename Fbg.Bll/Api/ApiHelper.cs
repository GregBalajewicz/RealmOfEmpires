using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Fbg.Bll.Api
{
    public class ApiHelper
    {

        public static System.Web.Script.Serialization.JavaScriptSerializer  GetJsonSerializer
        {
            get
            {
                System.Web.Script.Serialization.JavaScriptSerializer json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                json_serializer.MaxJsonLength = 10000000;
                return json_serializer;
            }
        }


        public static string RETURN_REDIRECT(string url)
        {
            System.Web.Script.Serialization.JavaScriptSerializer json_serializer = GetJsonSerializer;

            return json_serializer.Serialize(new
            {
                success = false,
                redirect_url = url,
            });

        }
        public static string RETURN_FAILURE(object o)
        {
            System.Web.Script.Serialization.JavaScriptSerializer json_serializer = GetJsonSerializer;

            return json_serializer.Serialize(new
            {
                success = false,
                @object = o
            });

        }
        public static string RETURN_SUCCESS(object o)
        {
            System.Web.Script.Serialization.JavaScriptSerializer json_serializer = GetJsonSerializer;

            return json_serializer.Serialize(new
            {
                success = true,
                @object = o
            });

        }

        public static string RETURN_SUCCESS(object o, System.Web.Script.Serialization.JavaScriptConverter converter)
        {
            System.Web.Script.Serialization.JavaScriptSerializer json_serializer = GetJsonSerializer;
            json_serializer.RegisterConverters(new JavaScriptConverter[] { converter});

            return json_serializer.Serialize(new
            {
                success = true,
                @object = o
            });

        }


        public class Converter : System.Web.Script.Serialization.JavaScriptConverter
        {
            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();

                if (obj is Village)
                {
                    Village v = (Village)obj;

                    dic.Add("name", v.name);
                    dic.Add("coins", v.coins);
                    dic.Add("loyalty", v.Loyalty);
                    dic.Add("x", v.Cordinates.X);
                    dic.Add("y", v.Cordinates.Y);
                    dic.Add("popCur", v.CurrentPopulation);
                    dic.Add("id", v.id);
                    dic.Add("yey", v.Loyalty);
                    dic.Add("popMax", v.MaxPopulation);
                    dic.Add("coinsPH", v.PerHourCoinIncome);
                    dic.Add("points", v.Points);
                    dic.Add("coinsTresMax", v.TreasurySize);
                    dic.Add("type", v.VillageType.ID);
                    dic.Add("areTranspAvail", v.AreTransportsToThisVillageAvailable);
                    dic.Add("villageTypeName", (v.VillageType != null && v.VillageType.IsBonus) ? v.VillageType.Name : "");


                    dic.Add("Units", from u in v.GetVillageUnits()
                                     select new
                                     {
                                         id = u.UnitType.ID
                                         ,
                                         YourUnitsCurrentlyInVillageCount = u.YourUnitsCurrentlyInVillageCount
                                         ,
                                         CurrentlyRecruiting = u.CurrentlyRecruiting
                                         ,
                                         SupportCount = u.SupportCount
                                         ,
                                         TotalNowInVillageCount = u.TotalNowInVillageCount
                                         ,
                                         YourUnitsTotalCount = u.YourUnitsTotalCount
                                     });

                }
                else if (obj is VillageBasicB)
                {
                    VillageBasicB v = (VillageBasicB)obj;

                    dic.Add("name", v.name);
                    dic.Add("coins", v.coins);
                    dic.Add("x", v.Cordinates.X);
                    dic.Add("y", v.Cordinates.Y);
                    dic.Add("popCur", v.CurrentPopulation);
                    dic.Add("id", v.id);
                    dic.Add("yey", v.Loyalty);
                    dic.Add("popMax", v.MaxPopulation);
                    dic.Add("coinsPH", v.PerHourCoinIncome);
                    dic.Add("points", v.Points);
                    dic.Add("coinsTresMax", v.TreasurySize);
                    dic.Add("type", v.VillageType.ID);
                }
                else if (obj is MyResearch.ResearchItemInProgress)
                {
                    MyResearch.ResearchItemInProgress riip = (MyResearch.ResearchItemInProgress)obj;
                    dic.Add("riid", riip.ri.ID);
                    dic.Add("eventID", riip.eventID);
                    dic.Add("completesOn", SerializeDate(riip.completesOn));


                }
                else if (obj is BuildingType)
                {
                    BuildingType bt = (BuildingType)obj;
                    dic.Add("buildingTypeID", bt.ID.ToString());
                }
                else if (obj is UnitType)
                {
                    UnitType ut = (UnitType)obj;
                    dic.Add("unitTypeID", ut.ID);
                }
             

                return dic;
            }


            public override IEnumerable<Type> SupportedTypes
            {
                get { return new System.Collections.ObjectModel.ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(VillageBasicB), typeof(MyResearch.ResearchItemInProgress), typeof(BuildingType), typeof(UnitType)  })); }
            }
        }


        public class Items2Converter : System.Web.Script.Serialization.JavaScriptConverter
        {
            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();

                if (obj is User)
                {
                    User u = (User)obj;

                    dic.Add("userID", u.ID);
                   

                }               
                else if (obj is UnitType)
                {
                    UnitType ut = (UnitType)obj;
                    dic.Add("unitTypeID", ut.ID);
                }
                else if (obj is TimeSpan)
                {                   
                    dic.Add("minutes", Convert.ToInt32( ((TimeSpan)obj).TotalMinutes));
                }
                else if (obj is DateTime)
                {

                    dic.Add("time", ApiHelper.SerializeDate((DateTime)obj));
                }
                return dic;
            }


            public override IEnumerable<Type> SupportedTypes
            {
                get { return new System.Collections.ObjectModel.ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(UnitType), typeof(User), typeof(TimeSpan), typeof(DateTime) })); }
            }
        }


        public static double SerializeDate(DateTime date)
        {
            return date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }


        public static double SerializeTimeSpan(TimeSpan timespan)
        {
            return timespan.TotalMilliseconds;
        }

    }
}
