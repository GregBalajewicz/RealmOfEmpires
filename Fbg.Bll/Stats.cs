using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web.Caching;
using System.Web;

namespace Fbg.Bll
{
    public class Stats
    {

        public class CONSTS
        {
           
            public class PlayerRanking
            {
                public static int PlayerID = 0;
                public static int PlayerName = 1;
                public static int VillageCount = 2;
                public static int TotalPoints = 3;
                public static int AveragePoints = 4;
                public static int ClanID = 5;
                public static int ClanName = 6;
                public static int PlayerRank = 7;
                public static int TitleID = 8;
                public static int Sex = 9;
                public static int AttackPoints = 10;
                public static int DefencePoints = 11;
                public static int GovKilled = 12;
                public static int NumBonusVillages = 13;
            }
            public class PlayerRankingColNames
            {
                public static string TitleID = "TitleID";
            }
            public class PlayerRanking_SortExp
            {
                public const string NumVillages = "NumberOfVillages";
                public const string Points = "Total Points";
                public const string AveragePoints = "AveragePoints";
                public const string AttackPoints = "PointsAsAttacker";
                public const string Defencepoints = "PointsAsDefender";
                public const string GovKilled = "GovKilledAsDefender";
            }
            public class ClanRanking
            {
                public static int ClanID = 0;
                public static int ClanName = 1;
                public static int PlayerCount = 2;
                public static int VillageCount = 3;
                public static int TotalPoints = 4;
                public static int AttackPts = 5;
                public static int DefencePts = 6;
                public static int NumBonusVillages = 7;
            }
            public class ClanRanking_SortExp
            {
                public const string Points = "Total Points";
                public const string NumVillages = "NumOfVillages";
                public const string NumBonusVillages = "NumOfBonusVillages";
            }
            public class PlayersFriendRanking
            {
                public static int PlayerID = 0;
                public static int PlayerName = 1;
                public static int ClanID = 2;
                public static int ClanName = 3;
                public static int FriendCount = 4;
                public static int VillageCount = 5;
                public static int TotalPoints = 6;
                public static int AveragePoints = 7;
            }
            public class TitlesRanking
            {
                public static int TitleID = 0;
                public static int Title_Male = 1;
                public static int Title_Female = 2;
                public static int Description = 3;
                public static int MaxPoints = 4;
                public static int PlayerCount = 5;
                public static int Level = 6;
            }

            
            public class CacheData
            {
                public static int Seconds = 180;
                public static int Friends_Seconds = 1800;
                public static int ThroneRoomPlayerRankingSeconds = 3600; // 60 min
            }
            public class GlobalStatsTable
            {
                public static int Troops = 0;
                public static int Players = 1;
            }


            public class GlobalStatsTotalTroopsColumns
            {
                public static int UnitTypeID = 0;
                public static int TotalCount = 1;
                public static int CurrentCount = 2;
            }

            public class GlobalStatsNumberOfColumns
            {
                public static int NoOfPlayers = 0;
                public static int NoOfVillages = 1;
                public static int NoOfSilver = 2;
                public static int NoOfPClans = 3;
                public static int SilverProduction = 4;
            }
            public class ThroneRoomPlayerRanking
            {
                public static int PlayerID = 0;
                public static int PlayerName = 1;
                public static int HighestNumOfVillages = 2;
                public static int HighestVillagePoints = 3;
                public static int PointsAsAttacker = 4;
                public static int PointsAsDefender = 5;
                public static int GovKilledAsDefender = 6;
                public static int ClanID = 7;
                public static int ClanName = 8;
                public static int TopTitleID = 9;
                public static int Sex = 10;
                public static int PlayerStatus = 11;
                public static int LastActivity = 12;
                public static int TitleName = 13;
            }
            public class ThroneRoomPlayerRanking_SortExp
            {
                public const string HighestNumOfVillages = "HighestNumOfVillages";
                public const string HighestVillagePoints = "HighestVillagePoints";
                public const string PointsAsAttacker = "PointsAsAttacker";
                public const string PointsAsDefender = "PointsAsDefender";
                public const string GovKilledAsDefender = "GovKilledAsDefender";
            }

        }


        static Stats()
        {
            getThroneRoomPlayerRankingPadlock = utils.InitializeArray<object>(Realms.AllRealmsReversed[0].ID+1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="areaNumber">pass 0 to get all ranking, not limited by area</param>
        /// <param name="sortExpression">One of Fbg.Bll.CONSTS.PlayerRanking_SortExp OR String.Empty if you want default sort (ie, by building points)</param>
        /// <returns></returns>
        public static DataTable GetPlayerRanking(Realm realm, int areaNumber, string sortExpression)
        {
            DataTable dt;
            if (sortExpression == null) { sortExpression = String.Empty; }
            if (sortExpression == CONSTS.PlayerRanking_SortExp.Points) { sortExpression = String.Empty; }

            string relKey = String.Format(Bll.CONSTS.Cache.PlayerCache, realm.ID, areaNumber, sortExpression);

            if (HttpContext.Current.Cache[relKey] == null)
            {
                dt = Fbg.DAL.Stats.GetPlayerRanking(realm.ConnectionStr, areaNumber);
                if (!String.IsNullOrEmpty(sortExpression))
                {
                    DataView dv = new DataView(dt);
                    dv.Sort = sortExpression + " DESC";
                    dt = dv.ToTable();
                }
                dt.Constraints.Add("PKey_PlayerID", dt.Columns[Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerID], true);
                HttpContext.Current.Cache.Insert(relKey
                    , dt
                    , null
                    , DateTime.Now.AddSeconds(CONSTS.CacheData.Seconds)
                    , Cache.NoSlidingExpiration
                    , CacheItemPriority.NotRemovable
                    , null);
            }
            else
            {
                dt = (DataTable)HttpContext.Current.Cache[relKey];
            }
            return dt;
        }

        /// <summary>
        /// same as calling GetPlayerRanking(realm, 0)
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        public static DataTable GetPlayerRanking(Realm realm)
        {
            return GetPlayerRanking(realm, 0, string.Empty);
        }

        public static DataTable GetClanRanking(Realm realm, int areaNumber, string sortExpression)
        {
            DataTable dt;
            if (sortExpression == null) { sortExpression = String.Empty; }
            if (sortExpression == CONSTS.ClanRanking_SortExp.Points) { sortExpression = String.Empty; }

            string relKey = String.Format(Bll.CONSTS.Cache.ClanCache, realm.ID, areaNumber, sortExpression);

            if (HttpContext.Current.Cache[relKey] == null)
            {
                dt = Fbg.DAL.Stats.GetClanRanking(realm.ConnectionStr, areaNumber);
                if (!String.IsNullOrEmpty(sortExpression))
                {
                    DataView dv = new DataView(dt);
                    dv.Sort = sortExpression + " DESC";
                    dt = dv.ToTable();
                }
                dt.Constraints.Add("PKey_ClanID", dt.Columns[Fbg.Bll.Stats.CONSTS.ClanRanking.ClanID], true);
                HttpContext.Current.Cache.Insert(relKey
                    , dt
                    , null
                    , DateTime.Now.AddSeconds(CONSTS.CacheData.Seconds)
                    , Cache.NoSlidingExpiration
                    , CacheItemPriority.NotRemovable
                    , null);
            }
            else
            {
                dt = (DataTable)HttpContext.Current.Cache[relKey];
            }
            return dt;
        }
        public static DataTable GetClanRanking(Realm realm, int areaNumber)
        {
            return GetClanRanking(realm, areaNumber, string.Empty);
        }

        /// <summary>
        /// get ranking for the specified clan.
        /// MAY return null!!
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="clanID"></param>
        /// <returns></returns>
        public static DataRow GetOneClanRanking(Realm realm, int clanID)
        {
            DataTable dt;
            string relKey = realm.ID.ToString() + Bll.CONSTS.Cache.ClanCache;

            if (HttpContext.Current.Cache[relKey] == null)
            {
                dt = Fbg.DAL.Stats.GetClanRanking(realm.ConnectionStr,0);
                dt.Constraints.Add("PKey_ClanID", dt.Columns[Fbg.Bll.Stats.CONSTS.ClanRanking.ClanID], true);
                HttpContext.Current.Cache.Insert(relKey
                    , dt
                    , null
                    , DateTime.Now.AddSeconds(CONSTS.CacheData.Seconds)
                    , Cache.NoSlidingExpiration
                    , CacheItemPriority.NotRemovable
                    , null);
            }
            else
            {
                dt = (DataTable)HttpContext.Current.Cache[relKey];
            }

            return dt.Rows.Find(clanID);
        }

        public static DataSet GetGlobalStats(Realm realm)
        {
            DataSet ds;
            string relKey = realm.ID.ToString() + Bll.CONSTS.Cache.GlobalStats;

            if (HttpContext.Current.Cache[relKey] == null)
            {
                ds = Fbg.DAL.Stats.GetGlobalStats(realm.ConnectionStr);
                HttpContext.Current.Cache.Insert(relKey
                    , ds
                    , null
                    , DateTime.Now.AddSeconds(CONSTS.CacheData.Seconds)
                    , Cache.NoSlidingExpiration
                    , CacheItemPriority.NotRemovable
                    , null);
            }
            else
            {
                ds = (DataSet)HttpContext.Current.Cache[relKey];
            }
            return ds;
        }
        public static DataTable GetPlayersFriendRanking(Realm realm)
        {
            DataTable dt;

            string relKey = realm.ID.ToString() + Bll.CONSTS.Cache.PlayersFriendRankingCache;
            if (HttpContext.Current.Cache[relKey] == null)
            {
                dt = Fbg.DAL.Stats.GetPlayersFriendRanking(realm.ConnectionStr);
                dt.Constraints.Add("PKey_PlayerID", dt.Columns[Fbg.Bll.Stats.CONSTS.PlayersFriendRanking.PlayerID], true);
                dt.AcceptChanges();

                HttpContext.Current.Cache.Insert(relKey
                    , dt
                    , null
                    , DateTime.Now.AddSeconds(CONSTS.CacheData.Friends_Seconds)
                    , Cache.NoSlidingExpiration
                    , CacheItemPriority.NotRemovable
                    , null);
            }
            else
            {
                dt = (DataTable)HttpContext.Current.Cache[relKey];
            }
            return dt;
        }
        public static DataTable GetPlayersFriendRanking(Realm realm, string sortExpression)
        {
            DataTable dt;

            string relKey = realm.ID.ToString() + Bll.CONSTS.Cache.PlayersFriendRankingCache + sortExpression;
            if (HttpContext.Current.Cache[relKey] == null)
            {
                dt = GetPlayersFriendRanking(realm);
                DataView dv = new DataView(dt);
                dv.Sort = sortExpression + " DESC";  
                dt=dv.ToTable();
                dt.Constraints.Add("PKey_PlayerID", dt.Columns[Fbg.Bll.Stats.CONSTS.PlayersFriendRanking.PlayerID], true);
                
                dt.AcceptChanges();

                HttpContext.Current.Cache.Insert(relKey
                    , dt 
                    , null
                    , DateTime.Now.AddSeconds(CONSTS.CacheData.Friends_Seconds)
                    , Cache.NoSlidingExpiration
                    , CacheItemPriority.NotRemovable
                    , null);
            }
            else
            {
                dt = (DataTable)HttpContext.Current.Cache[relKey];
            }
            return dt;
        }
        /// <summary>
        /// the datarow returned is described by Fbg.Bll.Stats.CONSTS.PlayersFriendRanking.
        /// Will return null if player is not found.
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="playerID"></param>  
        /// <returns></returns>
        public static DataRow GetPlayersFriendRanking_ByPlayer(Realm realm, int playerID)
        {
            DataTable dt;
            dt = Stats.GetPlayersFriendRanking(realm);
            return dt.Rows.Find(playerID);
        }
        public static DataTable GetTitlesRanking(Realm realm)
        {
            DataTable dt;

            string relKey = realm.ID.ToString() + Bll.CONSTS.Cache.Titles;
            if (HttpContext.Current.Cache[relKey] == null)
            {
                dt = Fbg.DAL.Stats.GetTitlesRanking(realm.ConnectionStr);
                HttpContext.Current.Cache.Insert(relKey
                    , dt
                    , null
                    , DateTime.Now.AddSeconds(CONSTS.CacheData.Seconds)
                    , Cache.NoSlidingExpiration
                    , CacheItemPriority.NotRemovable
                    , null);
            }
            else
            {
                dt = (DataTable)HttpContext.Current.Cache[relKey];
            }
            return dt;
        }
        public static DataTable GetTitleRanking(Realm realm, Title title)
        {
            DataTable dtPlayers;
            DataView dvTitle;
            DataTable dtTitle;

            string relKey = String.Format(Bll.CONSTS.Cache.Title, realm.ID.ToString(), title.ID.ToString());
            if (HttpContext.Current.Cache[relKey] == null)
            {
                dtPlayers = Stats.GetPlayerRanking(realm);
                dvTitle = new DataView(dtPlayers
                    , CONSTS.PlayerRankingColNames.TitleID + " = " + title.ID.ToString()
                    , ""
                    ,DataViewRowState.CurrentRows);

                dtTitle = dvTitle.ToTable();
                dtTitle.AcceptChanges();
                dtTitle.Constraints.Add("pid", dtTitle.Columns[Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerID], true);

                HttpContext.Current.Cache.Insert(relKey
                    , dtTitle
                    , null
                    , DateTime.Now.AddSeconds(CONSTS.CacheData.Seconds)
                    , Cache.NoSlidingExpiration
                    , CacheItemPriority.NotRemovable
                    , null);
            }
            else
            {
                dtTitle = (DataTable)HttpContext.Current.Cache[relKey];
            }
            return dtTitle;
        }


        static readonly object[] getThroneRoomPlayerRankingPadlock;

          /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <returns></returns>
        public static DataTable GetThroneRoomPlayerRanking(Realm realm)
        {
            return GetThroneRoomPlayerRanking(realm, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="sortExpression">one of Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking_SortExp</param>
        /// <returns></returns>
        public static DataTable GetThroneRoomPlayerRanking(Realm realm, string sortExpression)
        {
            DataTable dt;

            string relKey = String.Format(Bll.CONSTS.Cache.ThroneRoomPlayerCache, realm.ID, sortExpression);
            string baserelKey = String.Format(Bll.CONSTS.Cache.ThroneRoomPlayerCache_BaseSort, realm.ID, sortExpression);

            if (HttpContext.Current.Cache[relKey] == null)
            {
                //
                // lock to make sure only one thread to operate here. 
                lock (getThroneRoomPlayerRankingPadlock[realm.ID])
                {

                  

                    //
                    // double check if we already have it loaded; otherwise, multiple threads will reload stats one by one
                    //  
                    if (HttpContext.Current.Cache[relKey] == null)
                    {

                        //
                        // if we dont have the base ranking DT, then get it from the DB
                        if (HttpContext.Current.Cache[baserelKey] == null)
                        {
                            dt = Fbg.DAL.Stats.GetThroneRoomPlayerRanking(realm.ConnectionStr);
                            HttpContext.Current.Cache.Insert(baserelKey
                               , dt
                               , null
                               , DateTime.Now.AddSeconds(CONSTS.CacheData.ThroneRoomPlayerRankingSeconds)
                               , Cache.NoSlidingExpiration
                               , CacheItemPriority.NotRemovable
                               , null);
                        }

                        //
                        // now take the base table, put the right sort on it, and save it in cache
                        //
                        dt = (DataTable)HttpContext.Current.Cache[baserelKey];
                        if (!String.IsNullOrEmpty(sortExpression))
                        {
                            DataView dv = new DataView(dt);
                            dv.Sort = sortExpression + " DESC";
                            dt = dv.ToTable();
                        }
                        dt.Constraints.Add("PKey_PlayerID", dt.Columns[Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking.PlayerID], true);
                        HttpContext.Current.Cache.Insert(relKey
                            , dt
                            , null
                            , DateTime.Now.AddSeconds(CONSTS.CacheData.ThroneRoomPlayerRankingSeconds)
                            , Cache.NoSlidingExpiration
                            , CacheItemPriority.NotRemovable
                            , null);
                    }
                }
            }

            dt = (DataTable)HttpContext.Current.Cache[relKey];

            return dt;
        }
    }
}
