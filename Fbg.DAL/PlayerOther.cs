using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;

using Fbg.Common;

namespace Fbg.DAL
{
    public class PlayerOther
    {
        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.PlayerOther");

        public class CONSTS
        {
            public class PlayerOtherColumnIndex
            {
                public static int VillageID = 0;
                public static int Name = 1;
                public static int Coins = 2;
                public static int Points = 3;
                public static int XCord = 4;
                public static int YCord = 5;
                public static int loyalty = 6;
            }
        }

        public static string getPlayerNotes(int PlayerID, int OwnerID, string ConnectionStr)
        {
            TRACE.InfoLine("in 'qPlayerNotes()'");
            Database db;

            try
            {
                db = new DB(ConnectionStr); ;
                return (string)db.ExecuteScalar("qPlayerNotes", new object[] { PlayerID, OwnerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerNotes", e);
                ex.AdditionalInformation.Add("connectionStr", ConnectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("NoteOwnerPlayerID", OwnerID.ToString());
                throw ex;
            }
        }

        public static void savePlayerNotes(int PlayerID, int OwnerID, string Notes, string ConnectionStr)
        {
            TRACE.InfoLine("in 'savePlayerNote()'");
            Database db;

            try
            {
                db = new DB(ConnectionStr); ;
                db.ExecuteNonQuery("savePlayerNote", new object[] { PlayerID, OwnerID, Notes });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling savePlayerNotes", e);
                ex.AdditionalInformation.Add("connectionStr", ConnectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("NoteOwnerPlayerID", OwnerID.ToString());
                ex.AdditionalInformation.Add("Notes", Notes);
                throw ex;
            }
        }
        public static void updatePlayerNotes(int PlayerID, int OwnerID, string Notes, string ConnectionStr)
        {
            TRACE.InfoLine("in 'uPlayerNote()'");
            Database db;

            try
            {
                db = new DB(ConnectionStr); ;
                db.ExecuteNonQuery("uPlayerNote", new object[] { PlayerID, OwnerID, Notes });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uPlayerNotes", e);
                ex.AdditionalInformation.Add("connectionStr", ConnectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("NoteOwnerPlayerID", OwnerID.ToString());
                ex.AdditionalInformation.Add("Notes", Notes);
                throw ex;
            }
        }

        /// <summary>
        /// returns 2 tables. 1st table has latest login of player. this table may have no rows if we have no login data on this player.
        /// table 2 is the number of logins we got for this person. 
        /// </summary>
        /// <param name="PlayerID"></param>
        /// <param name="ConnectionStr"></param>
        /// <returns></returns>
        public static DataSet GetPlayerActivity(int PlayerID)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return DB.ExecuteDataSet(db, "qPlayerActivity", new object[] { PlayerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerActivity", e);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                throw ex;
            }

        }


        public static void SaveProfile(int PlayerID, string profile, string ConnectionStr)
        {
            Database db;

            try
            {
                db = new DB(ConnectionStr); ;
                db.ExecuteNonQuery("uPlayerProfile", new object[] { PlayerID, profile });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uPlayerProfile", e);
                ex.AdditionalInformation.Add("connectionStr", ConnectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("profile", profile);
                throw ex;
            }
        }
    }
}
