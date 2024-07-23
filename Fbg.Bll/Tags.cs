using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common ;
using System.Data;
namespace Fbg.Bll
{
    public class Tags
    {
        public class CONSTS
        {

            public class TagColumnIndex
            {
                public static int TagID = 0;
                public static int Name = 1;
                public static int Desc = 2;
                public static int Sort = 3;
                public static int PlayerID = 4;
            }
            
            public class TagTableIndex
            {
                public static int VillageTag = 0;
                public static int WithVillages = 1;
                public static int WithoutVillages = 2;

            }
            public class VillageTagTableIndex
            {

                public static int WithTags = 0;
                public static int WithoutTags = 1;

            }

        }
        private DataTable dtWithVillages;
        private DataTable dtWithoutVillages;
        private DataRow drVillageTag;
        public Tags(DataSet ds)
        {
            if (ds.Tables[CONSTS.TagTableIndex.VillageTag].Rows.Count  > 0)
            {
                drVillageTag = ds.Tables[CONSTS.TagTableIndex.VillageTag].Rows[0];
                dtWithVillages = ds.Tables[CONSTS.TagTableIndex.WithVillages];
                dtWithoutVillages = ds.Tables[CONSTS.TagTableIndex.WithoutVillages];
            }
        }
        public int ID
        {
            get
            {
                return (int)drVillageTag[CONSTS.TagColumnIndex.TagID];
            }
        }
        public string Name
        {
            get
            {
                return (string)drVillageTag[CONSTS.TagColumnIndex.Name];
            }
        }
        public string  Desc
        {
            get
            {
                return (string)drVillageTag[CONSTS.TagColumnIndex.Desc];
            }
        }
        public Int16   Sort
        {
            get
            {
                return (Int16) drVillageTag[CONSTS.TagColumnIndex.Sort];
            }
        }
        public int PlayerID
        {
            get
            {
                return (int)drVillageTag[CONSTS.TagColumnIndex.PlayerID];
            }
        }
        public DataTable WithVillages
        {
            get
            {
                return dtWithVillages;
            }

        }
        public DataTable WithoutVillages
        {
            get
            {
                return dtWithoutVillages;
            }

        }
        
        public static DataTable GetTags(Player owner)
        {
            return DAL.Tags.GetTags(owner.Realm.ConnectionStr,owner.ID);
        }
        public static Tags GetTagByID(Player owner, int tagID)
        {
            return new Tags(DAL.Tags.GetTagByID (owner.Realm.ConnectionStr,owner.ID , tagID ));
        }
        public static DataSet  GetTagsByVillageID(Player owner, int villageID)
        {
            return DAL.Tags.GetTagsByVillageID(owner.Realm.ConnectionStr, owner.ID, villageID);
        }
        public static void DeleteTag(Player owner, int tagID)
        {
            DAL.Tags.DeleteTag(owner.Realm.ConnectionStr, owner.ID, tagID);
            owner.Tags_Invalidate();
        }
        /// <summary>
        /// this will add unique named tag
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="tagName"></param>
        /// <returns>false if the name is exists ,true if the tag name not exists even the player have a filter name with the same name or not</returns>
        public static bool AddTag(Player owner, string tagName)
        {
            bool retval = DAL.Tags.AddTag(owner.Realm.ConnectionStr, owner.ID, tagName);
            owner.Tags_Invalidate();
            return retval;
        }
        public static void AddVillageTag(Player owner, int tagID,int villageID)
        {
            DAL.Tags.AddVillageTag(owner.Realm.ConnectionStr, owner.ID, tagID,villageID );
        }

        public static void DeleteVillageTag(Player owner, int tagID, int villageID)
        {
            DAL.Tags.DeleteVillageTag(owner.Realm.ConnectionStr, owner.ID, tagID, villageID);
        }

        public static bool UpdateTag(Player owner, int tagID, string tagName, string tagDesc, Int16 sort)
        {
            bool retVal =  DAL.Tags.UpdateTag(owner.Realm.ConnectionStr, tagID, tagName, tagDesc, sort, owner.ID);
            owner.Tags_Invalidate();
            return retVal;
        }
    }
}
