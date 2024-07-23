using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common ;
using System.Data;
namespace Fbg.Bll
{

    /// <summary>
    /// provides the basic functionality of filters - just its basic properties. 
    /// FOr more functional object, see the fully functional "Filter" class
    /// </summary>
    public class FilterBase
    {
        private class CONSTS
        {
            public class FilterColumnIndex
            {
                public static int FilterID = 0;
                public static int Name = 1;
                public static int Desc = 2;
                public static int Sort = 3;
            }

        }
        private DataRow drFilter;

        
        protected FilterBase(DataRow dr)
        {
                drFilter = dr;
        }
        public int ID
        {
            get
            {
                return (int)drFilter[CONSTS.FilterColumnIndex.FilterID];
            }
        }
        public string Name
        {
            get
            {
                return (string)drFilter[CONSTS.FilterColumnIndex.Name];
            }
        }
        public string Desc
        {
            get
            {
                return (string)drFilter[CONSTS.FilterColumnIndex.Desc];
            }
        }
        public Int16 Sort
        {
            get
            {
                return (Int16)drFilter[CONSTS.FilterColumnIndex.Sort];
            }
        }

        /// <summary>
        /// returns list of filters this player has defined. 
        /// if no filters defined, the list is empty, but it is not null.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        internal static List<FilterBase> GetFilters(Player owner)
        {
            List<FilterBase> filters = new List<FilterBase>();
            DataTable dt  = DAL.Filters.GetFilters(owner.Realm.ConnectionStr, owner.ID);
            foreach (DataRow dr in dt.Rows) 
            {
                filters.Add(new FilterBase(dr));
            }
            return filters;
        }

    }

    public class Filter :FilterBase
    {
        public class CONSTS
        {
            internal class FilterTableIndex
            {
                public static int Filter = 0;
                public static int SelectedTags = 1;
            }
            public class SelectedTagColumnIndex
            {
                public static int TagID = 0;
                public static int TagName = 1;
                /// <summary>
                /// this column hold the filterID or 0 meaning this filter does not have this tag checked
                /// </summary>
                public static int FilterID = 2;
            }
            public class SelectedTagColumnNames
            {
                /// <summary>
                /// this column hold the filterID or 0 meaning this filter does not have this tag checked
                /// </summary>
                public static string FilterID = "checked";
            }
        }
        private DataTable dtSelectedTags;
        private Filter(DataSet ds)
            : base(ds.Tables[CONSTS.FilterTableIndex.Filter].Rows[0])
        {
             dtSelectedTags = ds.Tables[CONSTS.FilterTableIndex.SelectedTags];
        }
 
        /// <summary>
        /// This returns a list of ALL of players tags. 
        /// The table is described by Filter.CONSTS.SelectedTagColumnIndex
        /// The records with the Filter.CONSTS.SelectedTagColumnIndex.FilterID == 0 are tags that are NOT 
        /// selected for this filter, others are
        /// </summary>
        public DataTable Tags
        {
            get
            {
                return dtSelectedTags;
            }
        }

        /// <summary>
        /// may return null if so a filter is not found
        /// </summary>
        /// <param name="owner">cannot be null</param>
        /// <returns></returns>
        public static Filter GetFilterByID(Player owner, int filterID)
        {
            DataSet ds = DAL.Filters.GetFilterByID(owner.Realm.ConnectionStr, owner.ID, filterID);
            if (ds.Tables[CONSTS.FilterTableIndex.Filter].Rows.Count  > 0)
            {
                return new Filter(ds);
            }
            return null;
        }
        public static void DeleteFilter(Player owner, int filterID)
        {
             DAL.Filters.DeleteFilter(owner.Realm.ConnectionStr, owner.ID,filterID );
        }
        /// <summary>
        /// add new filter 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="filterName"></param>
        /// <returns>false if the name is exists ,true if the filter name not exists </returns>
        public static bool AddFilter(Player owner, string filterName)
        {
            bool retVal = DAL.Filters.AddFilter(owner.Realm.ConnectionStr, owner.ID, filterName);
            owner.Filters_Invalidate();
            return retVal;
        }
        public static bool UpdateFilter(Player owner, int filterID, string filterName, string filterDesc, Int16 sort)
        {
            bool retVal = DAL.Filters.UpdateFilter(owner.Realm.ConnectionStr, filterID, filterName, filterDesc, sort, owner.ID);
           owner.Filters_Invalidate();
           return retVal;
       }
        public static void UpdateFilterTags(Player owner, int filterID, string tagIDs)
        {
            DAL.Filters.UpdateFilterTags(owner.Realm.ConnectionStr, owner.ID, filterID, tagIDs);
            owner.Filters_Invalidate();
        }
    }
}
