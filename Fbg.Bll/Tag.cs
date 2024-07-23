using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Fbg.Bll
{
    /// <summary>
    /// provides the basic functionality of tags
    /// </summary>
    public class TagBase
    {
        private class CONSTS
        {
            public class TagColumnIndex
            {
                public static int TagID = 0;
                public static int Name = 1;
               //public static int Desc = 2;
               // public static int Sort = 3;
            }

        }
        private DataRow drTag;


        protected TagBase(DataRow dr)
        {
            drTag = dr;
        }
        public int ID
        {
            get
            {
                return (int)drTag[CONSTS.TagColumnIndex.TagID];
            }
        }
        public string Name
        {
            get
            {
                return (string)drTag[CONSTS.TagColumnIndex.Name];
            }
        }
        //public string Desc
        //{
        //    get
        //    {
        //        return (string)drFilter[CONSTS.FilterColumnIndex.Desc];
        //    }
        //}
        //public Int16 Sort
        //{
        //    get
        //    {
        //        return (Int16)drFilter[CONSTS.FilterColumnIndex.Sort];
        //    }
        //}

        /// <summary>
        /// returns list of filters this player has defined. 
        /// if no filters defined, the list is empty, but it is not null.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        internal static List<TagBase> GetTags(Player owner)
        {
            List<TagBase> tags = new List<TagBase>();
            DataTable dt = Bll.Tags.GetTags(owner);
            foreach (DataRow dr in dt.Rows)
            {
                tags.Add(new TagBase(dr));
            }
            return tags;
        }

    }

}