using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Threading;

namespace Fbg.DAL
{
    public class EventHandler
    {
        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.EventHandler");

        public static DataTable GetDueEvents(string connectionStr)
        {
            TRACE.InfoLine("in 'GetDueEvents'.");
            DataSet ds=null;
            Database db;
            try
            {
                db = new DB(connectionStr);

                ds = DB.ExecuteDataSet(db, "qEventsDue", new object[] { });
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                if (string.Compare(e.Message, "Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.", false) == 0)
                {
                    throw new Fbg.DAL.SqlTimeOutException(e);
                }
                else
                {
                    throw new BaseApplicationException("Error while calling or preparing to call qEventsDue stored proc", e);
                }
            }
            catch (ThreadAbortException){}
            catch (Exception e)
            {
                throw new BaseApplicationException("Error while calling or preparing to call qEventsDue stored proc", e);
            }
            finally
            {
            }

            if (ds.Tables.Count != 1) 
            {
                throw new BaseApplicationException("Expected to get exactly one table in dataset but instead got:" + ds.Tables.Count);
            }
            return ds.Tables[0];            
        }

        public static void  SetEventAsProcessed(string connectionStr, Int64 eventID)
        {
            TRACE.InfoLine("in 'SetEventAsProcessed'.");
            Database db;
            try
            {
                db = new DB(connectionStr);

                System.Data.Common.DbCommand cmd = db.GetSqlStringCommand("Update Events set status = 1 where EventID = @EventID");

                db.AddInParameter(cmd, "@EventID", System.Data.DbType.Int64, eventID);

                DB.ExecuteNonQuery(db,cmd);
            }
            catch (ThreadAbortException) { }
            catch (System.Data.SqlClient.SqlException e)
            {
                if (string.Compare(e.Message, "Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.", false) == 0)
                {
                    throw new Fbg.DAL.SqlTimeOutException(e);
                }
                else
                {
                    throw new BaseApplicationException("Error in SetEventAsProcessed", e);
                }
            }
            catch (Exception e)
            {
                throw new BaseApplicationException("Error in SetEventAsProcessed", e);
            }
            finally
            {
            }
        }


        public static void CompleteBuilingUpgrade(string connectionStr, long eventid, int villageID, int bid, int level)
        {
            TRACE.InfoLine("in 'CompleteBuilingUpgrade'.");
            Database db;

            try
            {
                db = new DB(connectionStr);
                DB.ExecuteNonQuery(db, "iCompleteBuildingUpgrade", new object[] { eventid, villageID, bid, level });
            }
            catch (ThreadAbortException) { }
            catch (System.Data.SqlClient.SqlException e)
            {
                if (string.Compare(e.Message, "Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.", false) == 0)
                {
                    throw new Fbg.DAL.SqlTimeOutException(e);
                }
                else
                {
                    BaseApplicationException ex = new BaseApplicationException("Error while calling 'iCompleteBuildingUpgrade'", e);
                    ex.AdditionalInformation.Add("villageID", villageID.ToString());
                    ex.AdditionalInformation.Add("bid", bid.ToString());
                    ex.AdditionalInformation.Add("level", level.ToString());
                    ex.AdditionalInformation.Add("eventid", eventid.ToString());
                    throw ex;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iCompleteBuildingUpgrade'", e);
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AdditionalInformation.Add("bid", bid.ToString());
                ex.AdditionalInformation.Add("level", level.ToString());
                ex.AdditionalInformation.Add("eventid", eventid.ToString());
                throw ex;
            }

        }



        public static void CompleteUnitRecruitment(string connectionStr, long eventid)
        {
            TRACE.InfoLine("in 'CompleteUnitRecruitment'.");
            Database db;

            try
            {
                db = new DB(connectionStr);
                DB.ExecuteNonQuery(db, "iCompleteUnitRecruitment", new object[] { eventid, 0 });
            }
            catch (ThreadAbortException) { }
            catch (System.Data.SqlClient.SqlException e)
            {
                if (string.Compare(e.Message, "Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.", false) == 0)
                {
                    throw new Fbg.DAL.SqlTimeOutException(e);
                }
                else
                {
                    BaseApplicationException ex = new BaseApplicationException("Error while calling 'iCompleteUnitRecruitment'", e);
                    ex.AdditionalInformation.Add("eventid", eventid.ToString());
                    throw ex;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iCompleteUnitRecruitment'", e);
                ex.AdditionalInformation.Add("eventid", eventid.ToString());
                throw ex;
            }
        }

        public static void ProcessUnitMovement(string connectionStr, long eventid, Fbg.Common.UnitCommand.CommandType commandType)
        {
            TRACE.InfoLine("in 'ProcessUnitMovement'.");
            Database db=null;
            DataSet dsForLog=null;
            bool logToFile = !String.IsNullOrEmpty(Config.UMLOG_Dir);

            try
            {
                db = new DB(connectionStr);
            }
            catch (ThreadAbortException) { }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'CreateDatabase'", e);
                ex.AdditionalInformation.Add("eventid", eventid.ToString());
                ex.AdditionalInformation.Add("commandType", commandType.ToString());
                throw ex;
            }


            try
            {
                switch (commandType)
                {
                    case Fbg.Common.UnitCommand.CommandType.Attack:
                        dsForLog = DB.ExecuteDataSet(db, "iProcessUnitMovement_Attack", new object[] { eventid, logToFile });
                        break;
                    case Fbg.Common.UnitCommand.CommandType.Return:
                        dsForLog = DB.ExecuteDataSet(db, "iProcessUnitMovement_Return", new object[] { eventid, logToFile });
                        break;
                    case Fbg.Common.UnitCommand.CommandType.Support:
                        dsForLog = DB.ExecuteDataSet(db, "iProcessUnitMovement_Support", new object[] { eventid, 1, logToFile });
                        break;
                    case Fbg.Common.UnitCommand.CommandType.Recall:
                        dsForLog = DB.ExecuteDataSet(db, "iProcessUnitMovement_Recall", new object[] { eventid, logToFile });
                        break;
                    default:
                        throw new ArgumentException("Unexpected value for commandType:"  + commandType.ToString());
                }
            }
            catch (ThreadAbortException) { }
            catch (System.Data.SqlClient.SqlException e)
            {
                if (string.Compare(e.Message, "Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.", false) == 0)
                {
                    throw new Fbg.DAL.SqlTimeOutException(e);
                }
                else
                {
                    BaseApplicationException ex = new BaseApplicationException("Error while calling 'iProcessUnitMovement'", e);
                    ex.AdditionalInformation.Add("eventid", eventid.ToString());
                    ex.AdditionalInformation.Add("commandType", commandType.ToString());
                    throw ex;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iProcessUnitMovement'", e);
                ex.AdditionalInformation.Add("eventid", eventid.ToString());
                ex.AdditionalInformation.Add("commandType", commandType.ToString());
                throw ex;
            }
            //
            // log to file if required
            //
            if (logToFile)
            {
                LogUM(eventid, commandType, dsForLog, true);
            }
  
        }

        public static void TransportCoinsCompleted(string connectionStr, long eventid)
        {
            TRACE.InfoLine("in 'TransportCoinsCompleted'.");
            Database db;

            try
            {
                db = new DB(connectionStr);
                DB.ExecuteNonQuery(db, "uTransportCoinsCompleted", new object[] { eventid, 0 });
            }
            catch (ThreadAbortException) { }
            catch (System.Data.SqlClient.SqlException e)
            {
                if (string.Compare(e.Message, "Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.", false) == 0)
                {
                    throw new Fbg.DAL.SqlTimeOutException(e);
                }
                else
                {
                    BaseApplicationException ex = new BaseApplicationException("Error while calling 'uTransportCoinsCompleted'", e);
                    ex.AdditionalInformation.Add("eventid", eventid.ToString());
                    throw ex;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uTransportCoinsCompleted'", e);
                ex.AdditionalInformation.Add("eventid", eventid.ToString());
                throw ex;
            }
        }

        private static void LogUM(long eventid, Fbg.Common.UnitCommand.CommandType commandType, DataSet dsForLog, bool createDirIfNotFound)
        {
            string dir=null;
            string fileName = null;
            DateTime today = DateTime.Now;
            try
            {
                dir = String.Format(Config.UMLOG_Dir, today.Year, today.Month, today.Day);
                fileName = dir + "\\UMLog-" + eventid.ToString() + "-" + commandType.ToString() + "-" + DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss.ffff") + "-" + DateTime.Now.Ticks.ToString() + ".xml";
                dsForLog.WriteXml(fileName, XmlWriteMode.IgnoreSchema);
            }
            catch (System.IO.DirectoryNotFoundException exDir)
            {
                #region since no dir, then create it 
                if (createDirIfNotFound)
                {
                    //must create a directory for this day
                    try
                    {
                        System.IO.Directory.CreateDirectory(dir);
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            BaseApplicationException bex = new BaseApplicationException("Erorr while loging this movement to a file", e);
                            bex.AddAdditionalInformation("dir", dir);
                            bex.AddAdditionalInformation("fileName", fileName);
                            bex.AddAdditionalInformation("dsForLog", dsForLog);
                            ExceptionManager.Publish(bex);
                        }
                        catch
                        {
                            //we never want this to fail... 
                        }
                    }
                    //
                    // ok, call me again to try to write the log again. but this time, do nto attempt to creat a dir if not found. 
                    //
                    LogUM(eventid, commandType, dsForLog, false);
                }
                else
                {
                    //
                    // we are asked not to creat a dir, yet dir does not exist. log this situation and exit quietly
                    //
                    try
                    {
                        BaseApplicationException bex = new BaseApplicationException("dir not found and we are not creating one", exDir);
                        bex.AddAdditionalInformation("dir", dir);
                        bex.AddAdditionalInformation("fileName", fileName);
                        bex.AddAdditionalInformation("dsForLog", dsForLog);
                        ExceptionManager.Publish(bex);
                    }
                    catch
                    {
                        //we never want this to fail... 
                    }
                }

                #endregion
            }
            catch (Exception e)
            {
                try
                {
                    BaseApplicationException bex = new BaseApplicationException("Erorr while loging this movement to a file", e);
                    bex.AddAdditionalInformation("dir", dir);
                    bex.AddAdditionalInformation("fileName", fileName);
                    bex.AddAdditionalInformation("today", today);
                    bex.AddAdditionalInformation("dsForLog", dsForLog);
                    ExceptionManager.Publish(bex);
                }
                catch
                {
                    //we never want this to fail... 
                }

            }
        }

        public static void CompleteBuilingDowngrade(string connectionStr, long eventID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                DB.ExecuteNonQuery(db, "iCompleteBuildingDowngrade", new object[] { eventID });
            }
            catch (ThreadAbortException) { }
            catch (System.Data.SqlClient.SqlException e)
            {
                if (string.Compare(e.Message, "Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.", false) == 0)
                {
                    throw new Fbg.DAL.SqlTimeOutException(e);
                }
                else
                {
                    BaseApplicationException ex = new BaseApplicationException("Error while calling 'CompleteBuilingDowngrade'", e);
                    ex.AdditionalInformation.Add("eventID", eventID.ToString());
                    throw ex;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'CompleteBuilingDowngrade'", e);
                ex.AdditionalInformation.Add("eventID", eventID.ToString());
                throw ex;
            }
        }

        public static void CompleteResearch(string connectionStr, long eventID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                DB.ExecuteNonQuery(db, "iCompleteResearch", new object[] { eventID });
            }
            catch (ThreadAbortException) { }
            catch (System.Data.SqlClient.SqlException e)
            {
                if (string.Compare(e.Message, "Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.", false) == 0)
                {
                    throw new Fbg.DAL.SqlTimeOutException(e);
                }
                else
                {
                    BaseApplicationException ex = new BaseApplicationException("Error while calling 'CompleteResearch'", e);
                    ex.AdditionalInformation.Add("eventID", eventID.ToString());
                    throw ex;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'CompleteResearch'", e);
                ex.AdditionalInformation.Add("eventID", eventID.ToString());
                throw ex;
            }
        }

        public static void ProcessRaidMovement(string connectionStr, long eventID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                DB.ExecuteNonQuery(db, "iProcessRaidMovement", new object[] { eventID });
            }
            catch (ThreadAbortException) { }
            catch (System.Data.SqlClient.SqlException e)
            {
                if (string.Compare(e.Message, "Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.", false) == 0)
                {
                    throw new Fbg.DAL.SqlTimeOutException(e);
                }
                else
                {
                    BaseApplicationException ex = new BaseApplicationException("Error while calling 'ProcessRaidMovement'", e);
                    ex.AdditionalInformation.Add("eventID", eventID.ToString());
                    throw ex;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'ProcessRaidMovement'", e);
                ex.AdditionalInformation.Add("eventID", eventID.ToString());
                throw ex;
            }
        }
        //public static DateTime? GetRealmEndingTime()
        //{
        //    TRACE.InfoLine("in 'GetRealmEndingTime'.");
        //    DataSet ds;
        //    Database db;
        //    try
        //    {
        //        db = DatabaseFactory.CreateDatabase();

        //        ds = DB.ExecuteDataSet(db, "qRealmEndingDate", new object[] { });
        //    }
        //    catch (System.Data.SqlClient.SqlException e)
        //    {
        //        if (string.Compare(e.Message, "Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.", false) == 0)
        //        {
        //            throw new Fbg.DAL.SqlTimeOutException(e);
        //        }
        //        else
        //        {
        //            throw new BaseApplicationException("Error while calling or preparing to call qEventsDue stored proc", e);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new BaseApplicationException("Error while calling or preparing to call qEventsDue stored proc", e);
        //    }
        //    finally
        //    {
        //    }

        //    if (ds.Tables.Count != 1)
        //    {
        //        throw new BaseApplicationException("Expected to get exactly one table in dataset but instead got:" + ds.Tables.Count);
        //    }
        //    return ds.Tables[0];
        //}
    }
}
