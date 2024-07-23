using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.DAL
{
    public class Filters
    {
        public static DataTable  GetFilters(string connectionStr,int playerID)
        {
            
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return db.ExecuteDataSet("qFilters", new object[] { playerID }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qFilters", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static DataSet GetFilterByID(string connectionStr,int playerID ,int filterID)
        {
           
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return db.ExecuteDataSet("qFilterByID", new object[] { playerID,filterID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qFilterByID", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("filterID", filterID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static void DeleteFilter(string connectionStr, int playerID, int filterID)
        {

            Database db;

            try
            {
                db = new DB(connectionStr); ;

                 db.ExecuteNonQuery("dFilter", new object[] { playerID, filterID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dFilter", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("filterID", filterID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        /// <summary>
        /// add new filter
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="playerID"></param>
        /// <param name="filterName"></param>
        /// <returns>false if the name is exists ,true if the filter name not exists </returns>
        public static bool AddFilter(string connectionStr, int playerID, string filterName)
        {

            Database db;

            try
            {
                db = new DB(connectionStr); ;

                int ret=(int)db.ExecuteScalar("iFilter", new object[] { playerID, filterName });
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
                BaseApplicationException ex = new BaseApplicationException("Error while calling iFilter", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("filterName", filterName );
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static bool UpdateFilter(string connectionStr,int filterID,string filterName,string filterDesc,Int16 sort, int playerID )
        {

            Database db;

            try
            {
                db = new DB(connectionStr); ;

                int ret = (int)db.ExecuteScalar("uFilter", new object[] { filterID, filterName, filterDesc, sort, playerID });
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
                BaseApplicationException ex = new BaseApplicationException("Error while calling uFilter", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("filterID", filterID.ToString ());
                ex.AdditionalInformation.Add("filterName", filterName);
                ex.AdditionalInformation.Add("filterDesc", filterDesc);
                ex.AdditionalInformation.Add("filterSort", sort.ToString () );
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static void UpdateFilterTags(string connectionStr,int playerID, int filterID,string tagIDs)
        {

            Database db;

            try
            {
                db = new DB(connectionStr); ;

                db.ExecuteNonQuery("iFilterTags", new object[] { playerID,filterID, tagIDs });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iFilterTags", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("filterID", filterID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("tagIDs", tagIDs);
                throw ex;
            }
            finally
            {
            }
        }
    }
}
