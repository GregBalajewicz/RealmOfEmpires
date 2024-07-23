using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.DAL
{
    public class Tags
    {
        public static DataTable  GetTags(string connectionStr,int playerID)
        {
            
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return db.ExecuteDataSet("qTags", new object[] { playerID }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qTags", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static DataSet GetTagByID(string connectionStr,int playerID ,int tagID)
        {
           
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return db.ExecuteDataSet("qTagByID", new object[] { playerID,tagID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qTagByID", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("tagID", tagID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static DataSet GetTagsByVillageID(string connectionStr, int playerID, int villageID)
        {

            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return db.ExecuteDataSet("qTagsByVillageID", new object[] { playerID, villageID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qTagsByVillageID", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static void DeleteTag(string connectionStr, int playerID, int tagID)
        {

            Database db;

            try
            {
                db = new DB(connectionStr); ;

                 db.ExecuteNonQuery("dTag", new object[] { playerID, tagID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dTag", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("tagID", tagID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="playerID"></param>
        /// <param name="tagName"></param>
        /// <returns>false if the name is exists ,true if the tag name not exists even the player have a filter name with the same name or not</returns>
        public static bool AddTag(string connectionStr, int playerID, string tagName)
        {

            Database db;

            try
            {
                db = new DB(connectionStr); ;

                int ret =(int)db.ExecuteScalar("iTag", new object[] { playerID, tagName });
                if (ret == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iTag", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("tagName", tagName);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static void AddVillageTag(string connectionStr, int playerID, int tagID,int villageID)
        {

            Database db;

            try
            {
                db = new DB(connectionStr); ;

                db.ExecuteNonQuery("iVillageTag", new object[] { playerID, tagID,villageID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iVillageTag", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("tagID", tagID.ToString ());
                ex.AdditionalInformation.Add("villageID", villageID.ToString ());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static void DeleteVillageTag(string connectionStr, int playerID, int tagID, int villageID)
        {

            Database db;

            try
            {
                db = new DB(connectionStr); ;

                db.ExecuteNonQuery("dVillageTag", new object[] { playerID, tagID, villageID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dVillageTag", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("tagID", tagID.ToString());
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static bool UpdateTag(string connectionStr,int tagID,string tagName,string tagDesc,Int16 sort, int playerID )
        {

            Database db;

            try
            {
                db = new DB(connectionStr); ;

                int ret = (int)db.ExecuteScalar("uTag", new object[] { tagID, tagName, tagDesc, sort, playerID });
                if (ret == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uTag", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("tagID", tagID.ToString ());
                ex.AdditionalInformation.Add("tagName", tagName);
                ex.AdditionalInformation.Add("tagDesc", tagDesc);
                ex.AdditionalInformation.Add("tagSort", sort.ToString () );
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
    }
}
