using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.DAL
{
    public class Stats
    {
        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.Stats");

        public static DataTable GetThroneRoomPlayerRanking(string ConnectionStr)
        {
            Database db;
            try
            {
                db = new DB(ConnectionStr);
                return db.ExecuteDataSet("qPlayerRankingThroneRoom", new object[] { }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerRankingThroneRoom", e);
                ex.AdditionalInformation.Add("connectionStr", ConnectionStr);
 
                throw ex;
            }
        }
        public static DataTable GetPlayerRanking(string ConnectionStr, int areaNumber)
        {
            Database db;
            Fbg.Common.Area a=null;
            try
            {
                db = new DB(ConnectionStr);
                if (areaNumber != 0)
                {
                    a = Fbg.Common.Area.GetArea(areaNumber);
                    return db.ExecuteDataSet("qPlayerRanking", new object[] { a.StartOfArea.X, a.StartOfArea.Y, a.EndOfArea.X, a.EndOfArea.Y }).Tables[0];
                }
                else
                {
                    return db.ExecuteDataSet("qPlayerRanking", new object[] { null, null, null, null }).Tables[0];
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerRanking", e);
                ex.AdditionalInformation.Add("connectionStr", ConnectionStr);
                ex.AddAdditionalInformation("a", a);
                throw ex;
            }
        }


        public static DataTable GetClanRanking(string ConnectionStr, int areaNumber)
        {
            TRACE.InfoLine("in 'GetClanRanking()'");
            Database db;
            Fbg.Common.Area a = null;


            try
            {
                db = new DB(ConnectionStr);


                if (areaNumber != 0)
                {
                    a = Fbg.Common.Area.GetArea(areaNumber);
                    return db.ExecuteDataSet("qClanRanking", new object[] { a.StartOfArea.X, a.StartOfArea.Y, a.EndOfArea.X, a.EndOfArea.Y }).Tables[0];
                }
                else
                {
                    return db.ExecuteDataSet("qClanRanking", new object[] { null, null, null, null }).Tables[0];
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qClanRanking", e);
                ex.AdditionalInformation.Add("connectionStr", ConnectionStr);
                throw ex;
            }
        }
        public static DataSet GetGlobalStats(string ConnectinStr)
        {
            TRACE.InfoLine("in 'GetGlobalStats()'");
            Database db;

            try
            {
                db = new DB(ConnectinStr);
                return db.ExecuteDataSet("qGlobalStats", new object[] { });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qGlobalStats", e);
                ex.AdditionalInformation.Add("connectionStr", ConnectinStr);
                throw ex;
            }
        }
        public static DataTable GetPlayersFriendRanking(string ConnectionStr)
        {
            TRACE.InfoLine("in 'GetPlayersFriendRanking()'");
            Database db;

            try
            {
                db = new DB(ConnectionStr);
                return db.ExecuteDataSet("qPlayersFriendRanking", new object[] { }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayersFriendRanking", e);
                ex.AdditionalInformation.Add("connectionStr", ConnectionStr);
                throw ex;
            }
        }
        public static DataTable GetTitlesRanking(string ConnectionStr)
        {
            TRACE.InfoLine("in 'GetTitlesRanking()'");
            Database db;

            try
            {
                db = new DB(ConnectionStr);
                return db.ExecuteDataSet("qTitlesRanking", new object[] { }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qTitlesRanking", e);
                ex.AdditionalInformation.Add("connectionStr", ConnectionStr);
                throw ex;
            }
        }        
    }
}
