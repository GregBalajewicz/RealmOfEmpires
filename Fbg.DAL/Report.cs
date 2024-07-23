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
    public class Report
    {
        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.Villages");

        //public static DataTable GetReports(string connectionStr, int playerID)
        //{
        //    TRACE.InfoLine("in 'GetReport()'");
        //    Database db;

        //    try
        //    {
        //        db = new DB(connectionStr);

        //        return db.ExecuteDataSet("qReports", new object[] { playerID }).Tables[0];
        //    }
        //    catch (Exception e)
        //    {
        //        BaseApplicationException ex = new BaseApplicationException("Error while calling qReports", e);
        //        ex.AdditionalInformation.Add("connectionStr", connectionStr);
        //        ex.AdditionalInformation.Add("playerID", playerID.ToString());
        //        throw ex;
        //    }
        //}

        

        public static DataSet GetSupportAttackedReportDetails(string connectionStr, int playerID, int recordID)
        {
            TRACE.InfoLine("in 'GetSupportAttackedReportDetails()'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qReports_SupportAttackedReportDet", new object[] { playerID, recordID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qReports_SupportAttackedReportDet", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("recordID", recordID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
        }


        public static DataSet GetBattleReportDetails(string connectionStr, int playerID, int recordID)
        {
            TRACE.InfoLine("in 'GetBattleReportDetails()'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qReports_BattleReportDet", new object[] { playerID, recordID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qReports_BattleReportDet", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("recordID", recordID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
        }



        
        public static void DeleteReport(string connectionStr, int playerID, List<Int64> recordIDs)
        {
            TRACE.InfoLine("in 'DeleteReport()'");
            Database db;

            string record_IDs;
            if (recordIDs != null)
            {
                //string report_IDs = string.Empty;
                record_IDs = string.Empty;
                //report_IDs = ListToString(reportIDs);
                record_IDs = ListToString(recordIDs);
            } else
            {
                record_IDs = null;
            }
            try
            {
                db = new DB(connectionStr);
                db.ExecuteDataSet("dDeleteReport", new object[] { playerID, record_IDs });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dDeleteReport", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("recordIDs", recordIDs.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
        }

        public static void MoveReportsToFolder(string connectionStr, int playerID, List<Int64> recordIDs, int folderID)
        {
            Database db;

            string record_IDs = string.Empty;
            record_IDs = ListToString(recordIDs);
            object fid=null;
            try
            {
                db = new DB(connectionStr);
                fid = folderID == -1 ? null : (object)folderID;
                db.ExecuteDataSet("uReportMove", new object[] { playerID, record_IDs, fid });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uReportMove", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("recordIDs", recordIDs.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("folderID", folderID.ToString());
                ex.AdditionalInformation.Add("fid", fid.ToString());
                throw ex;
            }
        }

        public static int ForwardReport(string connectionStr, int playerID, string playerName, List<Int64> recordIDs)
        {
            TRACE.InfoLine("in 'ForwardReport()'");
            Database db;
            string report_IDs = string.Empty;
            string record_IDs = string.Empty;

            //report_IDs = ListToString(reportIDs);
            record_IDs = ListToString(recordIDs);

            try
            {               
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("iForwardReport");

                // Add parameters
                //db.AddInParameter(cmd, "@reportIDs", System.Data.DbType.String, report_IDs);
                db.AddInParameter(cmd, "@recordIDs", System.Data.DbType.String, record_IDs);
                db.AddInParameter(cmd, "@PlayerName", System.Data.DbType.String, playerName);
                db.AddInParameter(cmd, "@playerID", System.Data.DbType.Int32, playerID);
                db.AddOutParameter(cmd, "@Error", System.Data.DbType.Int32, int.MaxValue);
                db.ExecuteNonQuery(cmd);
                int Error = (int)db.GetParameterValue(cmd, "@Error");

                return Error;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iForwardReport", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                //ex.AdditionalInformation.Add("reportIDs", report_IDs.ToString());
                ex.AdditionalInformation.Add("recordIDs", record_IDs.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
        }

        public static DataSet GetReportTypeDetails(string connectionStr, int playerID, int recordID)
        {
            TRACE.InfoLine("in 'GetReportTypeDetails()'");
            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("qReportDetails", new object[] { playerID, recordID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qReportDetails", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("recordID", recordID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
        }

        ///<summary>
        /// To get comma seperated string of ids from List
        /// </summary>
        public static string ListToString(List<Int64> ListTemp)
        {
            StringBuilder ReportIds = new StringBuilder();

            foreach (Int64 id in ListTemp)
            {
                if (ReportIds.Length > 0)
                {
                    ReportIds.Append(',');
                    ReportIds.Append(id);
                }
                else
                {
                    ReportIds.Append(id);
                }
            }
            return ReportIds.ToString();
        }
        ///<summary>
        /// To get comma seperated string of ids from List
        /// </summary>
        public static string ListToString(List<int> ListTemp)
        {
            StringBuilder ReportIds = new StringBuilder();

            foreach (int id in ListTemp)
            {
                if (ReportIds.Length > 0)
                {
                    ReportIds.Append(',');
                    ReportIds.Append(id);
                }
                else
                {
                    ReportIds.Append(id);
                }
            }
            return ReportIds.ToString();
        }

        public static DataTable GetReportType(string connectionStr)
        {
            TRACE.InfoLine("in 'GetReportType'");
            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("qReportType").Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qReportType", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="PlayerID"></param>
        /// <param name="ReportTypeID"></param>
        /// <param name="villageXCord"></param>
        /// <param name="villageYCord"></param>
        /// <param name="SearchTxt"></param>
        /// <param name="selectedFolderID">can be set to -1 if showing inbox</param>
        /// <returns></returns>
        public static DataSet GetReportList(string connectionStr, int PlayerID, int ReportTypeID
            , int villageXCord, int villageYCord, string SearchTxt, int selectedFolderID, int maxDaysOld, bool retrieveAllData)
        {
            TRACE.InfoLine("in 'GetReportList()'");
            Database db;

            // Check null Parameters
            object ReportTypeIDObject = ReportTypeID == -1 ? null : (object)ReportTypeID;
            object SearchTxtObject = SearchTxt == "" ? null : (object)SearchTxt;
            object villageXCordObject = villageXCord == Int32.MinValue ? null : (object)villageXCord;
            object villageYCordObject = villageYCord == Int32.MinValue ? null : (object)villageYCord;
            object selectedFolderIDObject = selectedFolderID == -1 ? null : (object)selectedFolderID;

            try
            {
                db = new DB(connectionStr);

        

                return db.ExecuteDataSet("qReportList", new object[] { PlayerID
                    , ReportTypeIDObject
                    , villageXCordObject
                    , villageYCordObject
                    , SearchTxtObject
                    , selectedFolderIDObject
                    , maxDaysOld});
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qReportList", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("ReportTypeID", ReportTypeID.ToString());
                ex.AddAdditionalInformation("villageXCord", villageXCord);
                ex.AddAdditionalInformation("villageYCord", villageYCord);
                ex.AddAdditionalInformation("selectedFolderID", selectedFolderID);
                ex.AdditionalInformation.Add("SearchTxt", SearchTxt.ToString());
                ex.AddAdditionalInformation("maxDaysOld", maxDaysOld);
                ex.AddAdditionalInformation("ReportTypeIDObject", ReportTypeIDObject);
                ex.AddAdditionalInformation("villageXCordObject", villageXCordObject);
                ex.AddAdditionalInformation("villageYCordObject", villageYCordObject);
                ex.AddAdditionalInformation("SearchTxtObject", SearchTxtObject);
                ex.AddAdditionalInformation("selectedFolderIDObject", selectedFolderIDObject);
                ex.AddAdditionalInformation("retrieveAllData", retrieveAllData);
                throw ex;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="PlayerID"></param>
        /// <param name="ReportTypeID"></param>
        /// <param name="villageXCord"></param>
        /// <param name="villageYCord"></param>
        /// <param name="SearchTxt"></param>
        /// <param name="selectedFolderID">can be set to -1 if showing inbox</param>
        /// <returns></returns>
        public static DataSet GetReportListAll(string connectionStr, int PlayerID, int ReportTypeID
            , int villageXCord, int villageYCord, string SearchTxt, int maxDaysOld, bool retrieveAllData)
        {
            TRACE.InfoLine("in 'GetReportListAll()'");
            Database db;

            // Check null Parameters
            object ReportTypeIDObject = ReportTypeID == -1 ? null : (object)ReportTypeID;
            object SearchTxtObject = SearchTxt == "" ? null : (object)SearchTxt;
            object villageXCordObject = villageXCord == Int32.MinValue ? null : (object)villageXCord;
            object villageYCordObject = villageYCord == Int32.MinValue ? null : (object)villageYCord;           

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("qReportListAll", new object[] { PlayerID
                    , ReportTypeIDObject
                    , villageXCordObject
                    , villageYCordObject
                    , SearchTxtObject                    
                    , maxDaysOld});
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qReportListAll", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("ReportTypeID", ReportTypeID.ToString());
                ex.AddAdditionalInformation("villageXCord", villageXCord);
                ex.AddAdditionalInformation("villageYCord", villageYCord);                
                ex.AdditionalInformation.Add("SearchTxt", SearchTxt.ToString());
                ex.AddAdditionalInformation("maxDaysOld", maxDaysOld);
                ex.AddAdditionalInformation("ReportTypeIDObject", ReportTypeIDObject);
                ex.AddAdditionalInformation("villageXCordObject", villageXCordObject);
                ex.AddAdditionalInformation("villageYCordObject", villageYCordObject);
                ex.AddAdditionalInformation("SearchTxtObject", SearchTxtObject);                
                ex.AddAdditionalInformation("retrieveAllData", retrieveAllData);
                throw ex;
            }
        }




    }
}
