

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
    public class PlayerFlags
    {
        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.PlayerFlags");

        public static DataSet GetExtraInfo_OnLogin(string connectionStr, int playerID, int? xpToUpdate)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                //returns dataset, 1st table flaginfo, 2nd table FB Friends
                return db.ExecuteDataSet("qPlayerExtraInfo_OnLogin", new object[] { playerID, xpToUpdate });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerExtraInfo_OnLogin", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }

        }

        
        public static void SetFlag(string connectionStr, int flagID, int playerID, DateTime flagUpdatedOn,String data)
        {
            Database db;
            try
            {
                db = new DB(connectionStr);;

                db.ExecuteNonQuery("iPlayerFlag", new object[] 
                    { 
                        playerID 
                        , flagID
                        , flagUpdatedOn
                        , data
                    });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iPlayerFlag", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("flagID", flagID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AddAdditionalInformation("flagUpdatedOn",flagUpdatedOn);
                ex.AddAdditionalInformation("data", data);
                throw ex;
            }


        }

        /// <summary>
        /// returns the updateOn date for the flag or NULL if flag not set
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="playerID"></param>
        /// <param name="flagID"></param>
        /// <returns></returns>
        public static void GetFlag(string connectionStr, int playerID, int flagID, out string data, out object setOn)
        {
            Database db;
            try
            {
                db = new DB(connectionStr);;

                DataSet ds = db.ExecuteDataSet("qPlayerFlag", new object[] 
                    { 
                        playerID 
                        , flagID
                    });

                if (ds.Tables[0].Rows.Count == 0)
                {
                    data = null;
                    setOn = null;
                }
                else
                {
                    data = ds.Tables[0].Rows[0][0] is DBNull ? null : (string)ds.Tables[0].Rows[0][0];
                    setOn = ds.Tables[0].Rows[0][1];
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerFlag", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("flagID", flagID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }


        }

        
    }
}

