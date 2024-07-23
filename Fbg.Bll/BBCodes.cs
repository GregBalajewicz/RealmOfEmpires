using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Fbg.Bll
{
    public class BBCodes
    {
        /// <summary>
        /// Described by utils.CONSTS.GetPlayersByNameColIndex
        /// Column CONSTS.GetPlayersByNameColIndex.Name is setup as primary key
        /// </summary>
        /// <param name="players">list of player names comma seperated with NO trailing comma.
        /// GOOD: "greg,bill,mark"
        /// BAD: "greg,bill,mark,"
        /// GOOD: "bill"
        /// BAD: "bill,"</param>
        public static DataTable CheckPlayerNames(Realm realm, string players)
        {
            DataTable dt = Bll.utils.GetPlayersByName(realm, players);
            dt.PrimaryKey = new DataColumn[] { dt.Columns[utils.CONSTS.GetPlayersByNameColIndex.Name] };
            return dt;
        }
        /// <summary>
        /// Described by utils.CONSTS.GetClansByNameColIndex
        /// Column CONSTS.GetClansByNameColIndex.Name is setup as primary key
        /// </summary>
        /// <param name="players">list of player names comma seperated with NO trailing comma.
        /// GOOD: "clana,clanb,clanC"
        /// BAD: "clanA,clanB,clanC,"
        /// GOOD: "clan"
        /// BAD: "clan,"</param>
        public static DataTable CheckClanNames(Realm realm, string clans)
        {
            DataTable dt = Bll.utils.GetClansByName(realm, clans);
            dt.PrimaryKey = new DataColumn[] { dt.Columns[utils.CONSTS.GetClansByXColIndex.Name] };
            return dt;
        }

        /// <summary>
        /// Described by utils.CONSTS.GetClansByXColIndex
        /// Column CONSTS.GetClansByXColIndex.ID is setup as primary key
        /// </summary>
        /// <param name="players">list of clan IDs comma seperated with NO trailing comma.
        /// GOOD: "11,12,13"
        /// BAD: "11,12,13,"
        /// GOOD: "33"
        /// BAD: "33,"</param>
        public static DataTable CheckClanIDs(Realm realm, string clans)
        {
            DataTable dt = Bll.utils.GetClansByIDs(realm, clans);
            dt.PrimaryKey = new DataColumn[] { dt.Columns[utils.CONSTS.GetClansByXColIndex.ID] };
            return dt;
        }

        /// <summary>
        /// Described by utils.CONSTS.GetVillagesByXColIndex
        /// (CONSTS.GetVillagesByXColIndex.XCord, GetVillagesByXColIndex.YCord) is setup as primary key
        /// </summary>
        /// <param name="whereClause">something like this: (xcord=-10 and ycord=-6) or (xcord = 0 and ycord=-20)</param>
        public static DataTable CheckVillageCords(Realm realm, string whereClause)
        {
            DataTable dt = Bll.utils.CheckVillagesByCords(realm, whereClause);
            dt.PrimaryKey = new DataColumn[] { dt.Columns[utils.CONSTS.GetVillagesByXColIndex.XCord], dt.Columns[utils.CONSTS.GetVillagesByXColIndex.YCord] };
            return dt;
        }

        /// <summary>
        /// Described by utils.CONSTS.GetVillagesByXColIndex
        /// Column CONSTS.GetVillagesByXColIndex.ID is setup as primary key
        /// </summary>
        /// <param name="players">list of village IDs comma seperated with NO trailing comma.
        /// GOOD: "11,12,13"
        /// BAD: "11,12,13,"
        /// GOOD: "33"
        /// BAD: "33,"</param>
        public static DataTable CheckVillageIDs(Realm realm, string list)
        {
            DataTable dt = Bll.utils.GetVillagesByIDs(realm, list);
            dt.PrimaryKey = new DataColumn[] { dt.Columns[utils.CONSTS.GetVillagesByXColIndex.ID] };
            return dt;
        }
    }
}
