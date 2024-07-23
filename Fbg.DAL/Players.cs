using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;
namespace Fbg.DAL
{
    public class Players
    {
        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.Players");
        public static int GetPlayerIDByName(string PlayerName, string connectionStr)
        {
            TRACE.InfoLine("in 'qPlayerIDByName()'");
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                object PlayerID = db.ExecuteScalar("qPlayerIDByName", new object[] { PlayerName });
                if (PlayerID == null)
                {
                    return 0;
                }
                else
                {
                    return (int)PlayerID;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerByName", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerName", PlayerName.ToString());
                throw ex;
            }
            finally
            {
            }

        }

        public static DataSet DefinedTargets_Get(string connectionStr, int playerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qDefinedTargets2", new object[] { playerID});
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qDefinedTargets2", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());

                throw ex;
            }
            finally
            {
            }
        }

        public static DataSet  DefinedTargets_Add(string connectionStr, int playerID, int villageid, short typeID, DateTime? setTime, string note, int expiresInXDays, string assignedTo)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("iDefinedTargets2", new object[] { playerID, villageid, typeID, setTime, note, expiresInXDays, assignedTo});
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iDefinedTargets2", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());

                throw ex;
            }
            finally
            {
            }
        }
        public static DataSet DefinedTargets_Edit(string connectionStr, int playerID, int definedTargetID, DateTime? setTime, string note, int expiresInXDays, string assignedTo)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("uDefinedTargets", new object[] { playerID, definedTargetID, setTime, note, expiresInXDays, assignedTo });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uDefinedTargets", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());

                throw ex;
            }
            finally
            {
            }
        }
        public static DataSet DefinedTargets_Delete(string connectionStr, int playerID, int definedTargetID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("dDefinedTargets", new object[] { playerID, definedTargetID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dDefinedTargets", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());

                throw ex;
            }
            finally
            {
            }
        }

        public static DataSet DefinedTargets_AddEditResonse(string connectionStr, int definedTargetID, int respondingplayerID, Int16 respondTypeID, string response)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("uDefinedTargetResponse", new object[] {  definedTargetID, respondingplayerID, respondTypeID, response });                
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uDefinedTargetResponse", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("definedTargetID", definedTargetID.ToString());
                ex.AdditionalInformation.Add("respondingplayerID", respondingplayerID.ToString());

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
        /// <param name="updateLastActive"></param>
        /// <param name="forceGetResearch"></param>
        /// <param name="activeStwardsRecordID">NULL unless it is a steward who is logged in as the account owner. this pass the stewarding record id so that we can validate it is still valid</param>
        /// <returns></returns>
        public static DataSet GetPlayerExtraInfo(string connectionStr, int playerID, bool updateLastActive, bool forceGetResearch, int? activeStwardsRecordID, DateTime? LastHandledVillageCacheTimeStamp)
        {
            TRACE.InfoLine("in 'qPlayerExtraInfo()'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qPlayerExtraInfo2", new object[] { playerID, updateLastActive, forceGetResearch, activeStwardsRecordID, LastHandledVillageCacheTimeStamp });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerExtraInfo2", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AddAdditionalInformation("forceGetResearch", forceGetResearch);
                throw ex;
            }
            finally
            {
            }
        }

        public static DataTable GetIncomingAttack(string connectionStr, int playerID)
        {
            TRACE.InfoLine("in 'GetIncomingAttack()'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qIncomingAttack", new object[]
                    {
                      playerID
                    }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qIncomingAttack", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
        }

       

        public static bool IsForumChanged(string connectionStr, int playerID)
        {
            TRACE.InfoLine("in 'qPlayerForumChanged()'");
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                int Result = (int)db.ExecuteScalar("qPlayerForumChanged", new object[] { playerID });
                if (Result == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerForumChanged", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static void ForumViewed(string connectionStr, int playerID)
        {
            TRACE.InfoLine("in 'uPlayerForumViewed()'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                db.ExecuteDataSet("uPlayerForumViewed", new object[] { playerID });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uPlayerForumViewed", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }

        public static DataSet GetPlayerOtherInfo(string connectionStr, int PlayerID, string PlayerName, int BasePlayerID)
        {
            TRACE.InfoLine("in 'qPlayerOther()'");
            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("qPlayerOther", new object[] { PlayerID, PlayerName, BasePlayerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerOther", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("BasePlayerID", BasePlayerID.ToString());
                throw ex;
            }
        }
        public static int GetPlayerChests(string connectionStr, int playerID)
        {
            TRACE.InfoLine("in 'qPlayerChests()'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                object result = db.ExecuteScalar("qPlayerChests", new object[] { playerID });
                if (result == null)
                {
                    return 0;
                }
                else
                {
                    return (int)result;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerChests", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static bool BuyChests(string connectionStr, int PlayerID, int VillageID, int ChestsNo, int chestCost)
        {
            TRACE.InfoLine("in 'uBuyPlayerChests()'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                bool result = (bool)db.ExecuteScalar("uBuyPlayerChests", new object[] { PlayerID, VillageID, ChestsNo, chestCost });
                return result;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uBuyPlayerChests", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("ChestsNo", ChestsNo.ToString());
                throw ex;
            }
            finally
            {
            }
        }


        /// <summary>
        /// this function activate or extend the PF Package 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="PlayerID"></param>
        /// <param name="PackageID"></param>
        /// <returns>0 means sucess ;1 means not enough gold</returns>
        public static int ActivateExtendPackage(string connectionStr, int PlayerID, int PackageID)
        {
            return ActivateExtendPackage(connectionStr, PlayerID, PackageID, false);
        }

        /// <summary>
        /// this function activate or extend the PF Package - if you dont know the right value for Activate, 
        /// call the ActivateExtendPackage(string connectionStr, int PlayerID, int PackageID) instead
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="PlayerID"></param>
        /// <param name="PackageID"></param>
        /// <param name="Activate"></param>
        /// <returns>0 means sucess ;1 means not enough gold</returns>
        public static int ActivateExtendPackage(string connectionStr, int PlayerID, int PackageID,bool Activate)
        {
            TRACE.InfoLine("in 'ActivatePackage'.");
            Database dbCommon;
            Database dbRealm;
            int cost = 0;
            int result = 0;


            DbTransaction tran;

            try
            {
                dbRealm = new DB(connectionStr);
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling DB Constractor", e);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("PackageID", PackageID.ToString());
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                throw ex;
            }

            using (DbConnection realmConnection = dbRealm.CreateConnection())
            {
               
                try
                {
                    realmConnection.Open();
                    tran = realmConnection.BeginTransaction();
                }
                catch (Exception e)
                {
                    BaseApplicationException ex = new BaseApplicationException("Error while calling opening connection & tran on Realm DB", e);
                    ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                    ex.AdditionalInformation.Add("PackageID", PackageID.ToString());
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    throw ex;
                }


                // NOTE - this distinction between activate and extend is no longer necessary. both SPs figure out if they are called wrong and call the other one. 
                //  this is being left here ... well.. because some code does call it correctly and then its a little faster not to have to callanother SP from the called SP- althought this is probably stupid
                if (Activate)
                {
                    //
                    //add the package to the player in realm specific DB
                    // 
                    try
                    {

                        cost = (int)dbRealm.ExecuteScalar(tran, "iPlayerPFPackage", new object[] { PlayerID, PackageID });
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();

                        BaseApplicationException ex = new BaseApplicationException("Error while calling 'iPlayerPFPackage' on realm specific DB", e);
                        ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                        ex.AdditionalInformation.Add("PackageID", PackageID.ToString());
                        ex.AdditionalInformation.Add("connectionStr", connectionStr);
                        throw ex;
                    }
                }
                else
                {
                    //
                    //extend the package to the player in realm specific DB
                    // 
                    try
                    {

                        cost = (int)dbRealm.ExecuteScalar(tran, "uPlayerPFPackage", new object[] { PlayerID, PackageID });
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();

                        BaseApplicationException ex = new BaseApplicationException("Error while calling 'uPlayerPFPackage' on realm specific DB", e);
                        ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                        ex.AdditionalInformation.Add("PackageID", PackageID.ToString());
                        ex.AdditionalInformation.Add("connectionStr", connectionStr);
                        throw ex;
                    }
                }
               
                try
                {
                    dbCommon = DatabaseFactory.CreateDatabase();
                }
                catch (Exception e)
                {
                    BaseApplicationException ex = new BaseApplicationException("Error while calling DatabaseFactory.CreateDatabase()", e);
                    ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                    ex.AdditionalInformation.Add("PackageID", PackageID.ToString());
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    throw ex;
                }
                //
                // add the package to the player in the common DB and subtract credits
                //
                try
                {
                    result = (int)dbCommon.ExecuteScalar("uCredits_Subtract", new object[] { PlayerID, cost });
                    //Function fail (Player don't have enough credits ) rollbck and exit function
                    if (result == 1)
                    {
                        tran.Rollback();
                        return 1;
                    }
                   

                }
                catch (Exception e)
                {
                    try
                    {
                        if (tran != null) { tran.Rollback(); };
                    }
                    catch (Exception) { }

                    BaseApplicationException ex = new BaseApplicationException("Error while calling 'uCredits_Subtract' on common DB", e);
                    ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                    ex.AdditionalInformation.Add("PackageID", PackageID.ToString());
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    throw ex;
                }
                try
                {
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();

                    BaseApplicationException ex = new BaseApplicationException("Error while calling commiting tran", e);
                    ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                    ex.AdditionalInformation.Add("PackageID", PackageID.ToString());
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    throw ex;
                }

                realmConnection.Close();
                return result;
            }

        }

        /// <summary>
        /// this function activate or extend the PF Package - if you dont know the right value for Activate, 
        /// call the ActivateExtendPackage(string connectionStr, int PlayerID, int PackageID) instead
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="PlayerID"></param>
        /// <param name="PackageID"></param>
        /// <param name="Activate"></param>
        /// <returns>0 means sucess ;1 means not enough gold</returns>
        public static void ActivateExtendPackageFromItem2(string connectionStr, int PlayerID, int PackageID, int durationInMin)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                db.ExecuteNonQuery("uPlayerPFPackage_FromItem2", new object[] { PlayerID,PackageID, durationInMin });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uPlayerPFPackage_FromItem2", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("PackageID", PackageID.ToString());
                ex.AdditionalInformation.Add("durationInMin", durationInMin.ToString());
                throw ex;
            }
            finally
            {
            }

        }
        public static DataTable GetPlayerPackages(string connectionStr, int PlayerID)
        {
            TRACE.InfoLine("in 'qPlayerPackages()'");
            Database db;

            try
            {
                db = new DB(connectionStr);
                DataSet ds=  db.ExecuteDataSet("qPlayerPackages", new object[] { PlayerID });
                return ds.Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerPackages", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
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
        /// <param name="packageID"></param>
        /// <param name="precentage"></param>
        /// <param name="refundType">0 means DepreciatedFeature ,1 means ActiveFeature</param>
        public static void CalculateRefundAmount(string connectionStr, int playerID, int packageID, double precentage
            , Fbg.Common.PFs.RefundTypes refundType, out int servantsRefund, out double daysToCancel)
        {
            TRACE.InfoLine("in 'RefreshFriends'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                DataSet ds;
                ds = db.ExecuteDataSet("dPlayerPFPackages", new object[] { playerID
                        , packageID
                        , precentage
                        , 1
                        , (int)refundType
                        , 0});

                servantsRefund = (int)ds.Tables[0].Rows[0][0];
                daysToCancel = Convert.ToDouble(ds.Tables[0].Rows[0][1]);

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'dPlayerPFPackages' ", e);
                ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                ex.AdditionalInformation.Add("PackageID", packageID.ToString());
                ex.AdditionalInformation.Add("precentage", precentage.ToString());
                ex.AdditionalInformation.Add("refundType", refundType.ToString());
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                throw ex;
            }

 


        }
        public static int CalculateNPRefundAmount(string connectionStr, int playerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("dPlayerPFNobilityPackage");


                db.AddInParameter(cmd, "@PlayerID", System.Data.DbType.Int32, playerID);
                db.AddInParameter(cmd, "@CalculateOnly", System.Data.DbType.Int32, 1);
                db.AddOutParameter(cmd,"@RefundSum", System.Data.DbType.Int32, int.MaxValue);
                db.ExecuteNonQuery(cmd);
                int ret = (int)db.GetParameterValue(cmd, "@RefundSum");
                return ret;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'dPlayerPFNobilityPackage' ", e);
                ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                throw ex;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="playerID"></param>
        /// <param name="packageID"></param>
        /// <param name="precentage"></param>
        /// <param name="refundType">0 means DepreciatedFeature ,1 means ActiveFeature</param>
        /// <returns></returns>
        public static int  RefundPackage(string connectionStr, int playerID, int packageID, double precentage, Fbg.Common.PFs.RefundTypes refundType)
        {
            Database dbCommon;
            Database dbRealm;
            int refundedServants = 0;
            int result = 0;

            DbTransaction tran;

            //
            // create a realm DB object
            //
            #region create a realm DB object
            try
            {
                dbRealm = new DB(connectionStr);
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling DB Constractor", e);
                ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                ex.AdditionalInformation.Add("PackageID", packageID.ToString());
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                throw ex;
            }
            #endregion

            using (DbConnection realmConnection = dbRealm.CreateConnection())
            {
                //
                // open the connection to realm DB and open a transaction. 
                //
                #region open the connection to realm DB
                try
                {
                    realmConnection.Open();
                    tran = realmConnection.BeginTransaction();
                }
                catch (Exception e)
                {
                    BaseApplicationException ex = new BaseApplicationException("Error while calling opening connection & tran on Realm DB", e);
                    ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                    ex.AdditionalInformation.Add("PackageID", packageID.ToString());
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    throw ex;
                }
                #endregion
                //
                // cancel the package in the DB
                // 
                #region cancel the package in the DB
                try
                {
                    DataSet ds;
                    ds = dbRealm.ExecuteDataSet(tran, "dPlayerPFPackages", new object[] { playerID
                        , packageID
                        , precentage
                        , 0
                        , (int)refundType
                        , 0});

                    refundedServants = (int)ds.Tables[0].Rows[0][0];
                }
                catch (Exception e)
                {
                    tran.Rollback();

                    BaseApplicationException ex = new BaseApplicationException("Error while calling 'dPlayerPFPackages' on realm specific DB", e);
                    ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                    ex.AdditionalInformation.Add("PackageID", packageID.ToString());
                    ex.AdditionalInformation.Add("precentage", precentage.ToString());
                    ex.AdditionalInformation.Add("refundType", refundType.ToString());
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    throw ex;
                }
                #endregion
                //
                // NOW REFUND THE SERVANTS in common db 
                //
                #region NOW REFUND THE SERVANTS in common db
                try
                {
                    dbCommon = DatabaseFactory.CreateDatabase();
                }
                catch (Exception e)
                {
                    BaseApplicationException ex = new BaseApplicationException("Error while calling DatabaseFactory.CreateDatabase()", e);
                    ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                    ex.AdditionalInformation.Add("PackageID", packageID.ToString());
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    throw ex;
                }
                try
                {
                    result = (int)dbCommon.ExecuteScalar("uRefundCredits", new object[] { playerID, refundedServants });
                    //Function fail (Player don't have enough credits ) rollbck and exit function
                    if (result == 1)
                    {
                        tran.Rollback();
                        return 1;
                    }


                }
                catch (Exception e)
                {
                    try
                    {
                        if (tran != null) { tran.Rollback(); };
                    }
                    catch (Exception) { }

                    BaseApplicationException ex = new BaseApplicationException("Error while calling 'uRefundCredits' on common DB", e);
                    ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                    ex.AdditionalInformation.Add("PackageID", packageID.ToString());
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    throw ex;
                }
                try
                {
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();

                    BaseApplicationException ex = new BaseApplicationException("Error while calling commiting tran", e);
                    ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                    ex.AdditionalInformation.Add("PackageID", packageID.ToString());
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    throw ex;
                }
                #endregion 

                realmConnection.Close();
            }

            return result;
        }

        public static int ActivateNPackageWithRefund(string connectionStr, int playerID, int npCost)
        {
            Database dbCommon;
            Database dbRealm;
            int refundedServants = 0;
            int result = 0;

            DbTransaction tran;

            //
            // create a realm DB object
            //
            #region create a realm DB object
            try
            {
                dbRealm = new DB(connectionStr);
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling DB Constractor", e);
                ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                throw ex;
            }
            #endregion

            using (DbConnection realmConnection = dbRealm.CreateConnection())
            {
                //
                // open the connection to realm DB and open a transaction. 
                //
                #region open the connection to realm DB
                try
                {
                    realmConnection.Open();
                    tran = realmConnection.BeginTransaction();
                }
                catch (Exception e)
                {
                    BaseApplicationException ex = new BaseApplicationException("Error while calling opening connection & tran on Realm DB", e);
                    ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    throw ex;
                }
                #endregion
                //
                // cancel the package in the DB
                // 
                #region cancel the package in the DB
                try
                {                    

                    System.Data.Common.DbCommand cmd = dbRealm.GetStoredProcCommand("dPlayerPFNobilityPackage");

                    dbRealm.AddInParameter(cmd, "@PlayerID", System.Data.DbType.Int32, playerID);
                    dbRealm.AddInParameter(cmd, "@CalculateOnly", System.Data.DbType.Int32, 0);
                    dbRealm.AddOutParameter(cmd, "@RefundSum", System.Data.DbType.Int32, int.MaxValue);
                    dbRealm.ExecuteNonQuery(cmd, tran);
                    refundedServants = (int)dbRealm.GetParameterValue(cmd, "@RefundSum");
                    if (refundedServants == 0)
                    {
                        tran.Rollback();
                        return 1;
                    }
                }
                catch (Exception e)
                {
                    tran.Rollback();

                    BaseApplicationException ex = new BaseApplicationException("Error while calling 'dPlayerPFNobilityPackage' on realm specific DB", e);
                    ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    throw ex;
                }
                #endregion
                //
                // NOW REFUND THE SERVANTS in common db, and activate the NP in common
                //
                #region NOW REFUND THE SERVANTS in common db
                try
                {
                    dbCommon = DatabaseFactory.CreateDatabase();
                }
                catch (Exception e)
                {
                    BaseApplicationException ex = new BaseApplicationException("Error while calling DatabaseFactory.CreateDatabase()", e);
                    ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    throw ex;
                }
                try
                {
                    result = (int)dbCommon.ExecuteScalar("uRefundNPCredits", new object[] { playerID, refundedServants ,npCost});
                    //Function fail (Player don't have enough credits ) rollbck and exit function
                    if (result == 1)
                    {
                        tran.Rollback();
                        return 1;
                    }


                }
                catch (Exception e)
                {
                    try
                    {
                        if (tran != null) { tran.Rollback(); };
                    }
                    catch (Exception) { }

                    BaseApplicationException ex = new BaseApplicationException("Error while calling 'uRefundCredits' on common DB", e);
                    ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    throw ex;
                }
                try
                {
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();

                    BaseApplicationException ex = new BaseApplicationException("Error while calling commiting tran", e);
                    ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    throw ex;
                }
                #endregion

                realmConnection.Close();
            }

            return result;
        }

        public static DataSet GetPlayerActiveFeatures(string connectionStr, int PlayerID)
        {
            TRACE.InfoLine("in 'qPlayerActiveFeatures()'");
            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("qPlayerActiveFeatures", new object[] { PlayerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerActiveFeatures", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static DataTable ActivePFTrial(string connectionStr, int PlayerID, int PFTrialID)
        {
            TRACE.InfoLine("in 'iActivePFTrial()'");
            Database db;

            try
            {
                db = new DB(connectionStr);
                DataSet ds = db.ExecuteDataSet("iActivePFTrial", new object[] { PlayerID, PFTrialID });
                return ds.Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iActivePFTrial", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("PFTrialID", PFTrialID.ToString());
                throw ex;
            }
            finally
            {
            }
        }

        public static void Title_AcceptNext(string connectionStr, int PlayerID)
        {
            TRACE.InfoLine("in 'Title_AcceptNext'");
            Database db;

            try
            {
                db = new DB(connectionStr);
                db.ExecuteNonQuery("uPlayerTitle", new object[] { PlayerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uPlayerTitle", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
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
        /// <param name="PlayerID"></param>
        /// <param name="Anonymous"></param>
        /// <param name="sex">0,1,2</param>
        public static void Update(string connectionStr, int PlayerID, bool Anonymous, short sex)
        {
            TRACE.InfoLine("in 'Update'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                db.ExecuteDataSet("uPlayer", new object[] { PlayerID, Anonymous, sex });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uPlayerOptions", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("Anonymous", Anonymous.ToString());
                ex.AddAdditionalInformation("sex", sex);
                throw ex;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="PlayerID"></param>
        /// <param name="invitedIDs">list of facebook ids comman seperated with NO trailing comma.
        /// GOOD: "5555,444,666"
        /// BAD: "555,444,666,"
        /// GOOD: "555"
        /// BAD: "555,"</param>
        public static void Invites_RegisterInvited(int PlayerID, string invitedIDs)
        {
            TRACE.InfoLine("in 'Invites_RegisterInvited'");
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteNonQuery(db,"iRegisterInvited", new object[] { PlayerID, invitedIDs });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iRegisterInvited", e);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AddAdditionalInformation("invitedIDs", invitedIDs);
                throw ex;
            }

        }
        public static DataTable Vote(Guid  UserID, int PollID,string optionIDs)
        {
            
            Database db;

            try
            {
                db = db = DatabaseFactory.CreateDatabase();
                return db.ExecuteDataSet("iUserVote", new object[] { UserID, PollID, optionIDs }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iUserVote", e);
                ex.AdditionalInformation.Add("UserID", UserID.ToString());
                ex.AdditionalInformation.Add("PoolID", PollID.ToString());
                ex.AddAdditionalInformation("optionIDs", optionIDs);
                throw ex;
            }

        }
        public static bool AreTransportsAvailable(string connectionStr, int PlayerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                int result = 0;
                 result =(int) db.ExecuteScalar("qCoinTransports_AreTransportToVillageAvail", new object[] { PlayerID, null, result });
                if (result  == 1) 
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
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerByName", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }


        /// <summary>
        /// if activation of the night build mode was not successful then DateTime.MinValue will be returned; 
        /// otherwise this returns the exact time and date the night build mode was activated in the DB
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="PlayerID"></param>
        /// <returns></returns>
        public static DateTime ActivateNightBuildMode(string connectionStr, int PlayerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                object result;
                result = db.ExecuteScalar("uPlayers_NightBuildMode", new object[] { PlayerID});
                if (result is DBNull)
                {
                    return DateTime.MinValue;
                }
                else
                {
                    return (DateTime)result;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uPlayers_NightBuildMode", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }

        public static DataTable PlayerStatHistory(string connectionStr, int PlayerID, DateTime? date, int? statId)
        {

            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("qPlayerStatHistory", new object[] { PlayerID, date, statId }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerStatHistory", e);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("date", date.ToString());
                ex.AdditionalInformation.Add("statId", statId.ToString());
                throw ex;
            }
        }

        public static void SetSteward(string connectionStr, int accountOwnerPlayerID, int accountStewardPlayerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                db.ExecuteNonQuery("iAccountStewards", new object[] { accountOwnerPlayerID, accountStewardPlayerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iAccountStewards", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("accountOwnerPlayerID", accountOwnerPlayerID);
                ex.AddAdditionalInformation("accountStewardPlayerID", accountStewardPlayerID);
                throw ex;
            }
        }

        public static DataTable GetMySteward(string connectionStr, int accountOwnerPlayerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); 
                return db.ExecuteDataSet("qAccountStewards_MyStewards", new object[] { accountOwnerPlayerID }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qAccountStewards_MyStewards", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("accountOwnerPlayerID", accountOwnerPlayerID);
                throw ex;
            }
        }
        public static DataTable GetAccountsISteward(string connectionStr, int accountStewardPlayerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                return db.ExecuteDataSet("qAccountStewards_AccountsISteward", new object[] { accountStewardPlayerID }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qAccountStewards_AccountsISteward", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("accountStewardPlayerID", accountStewardPlayerID);
                throw ex;
            }
        }

        public static object DeleteAppointedSteward(string connectionStr, int accountOwnerPlayerID, int recordID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                return db.ExecuteDataSet("dAccountStewards", new object[] { accountOwnerPlayerID, recordID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dAccountStewards", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("accountOwnerPlayerID", accountOwnerPlayerID);
                ex.AddAdditionalInformation("recordID", recordID);
                throw ex;
            }
        }


        public static void AcceptStewardship(string connectionStr, int stewardPlayerID, int recordID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                db.ExecuteNonQuery("uAcceptAccountStewardship", new object[] { stewardPlayerID, recordID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uAcceptAccountStewardship", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("stewardPlayerID", stewardPlayerID);
                ex.AddAdditionalInformation("recordID", recordID);
                throw ex;
            }
        }


        public static void CancelStewardship(string connectionStr, int stewardPlayerID, int recordID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                db.ExecuteNonQuery("dAccountStewardship", new object[] { stewardPlayerID, recordID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dAccountStewardship", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("stewardPlayerID", stewardPlayerID);
                ex.AddAdditionalInformation("recordID", recordID);
                throw ex;
            }
        }


        public static void TransferSteward(string connectionStr, int accountOwnerPlayerID, int accountStewardPlayerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                db.ExecuteNonQuery("iAccountStewards_Transfer", new object[] { accountOwnerPlayerID, accountStewardPlayerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iAccountStewards_Transfer", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("accountOwnerPlayerID", accountOwnerPlayerID);
                ex.AddAdditionalInformation("accountStewardPlayerID", accountStewardPlayerID);
                throw ex;
            }
        }

        public static short GetIsMarkedForDeletionDueToInactivity(int playerID)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                object result = DB.ExecuteScalar(db, "qIsMarkedForDeletionDueToInactivity", new object[] { playerID});

                if (result == null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qIsMarkedForDeletionDueToInactivity", e);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
        }


        public static DataTable GetRecentTargets(string connectionStr, int PlayerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                DataSet ds = db.ExecuteDataSet("qPlayerRecentTargetStack", new object[] { PlayerID });
                return ds.Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerRecentTargetStack", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }

        public static DateTime ActivateSleepMode(string connectionStr, int PlayerID)
        {
            Database db;
             
            try
            { 
                db = new DB(connectionStr); ;
                object result;
                result = db.ExecuteScalar("uPlayers_SleepMode", new object[] { PlayerID });
                if (result is DBNull || result == null)
                {
                    return DateTime.MinValue;
                }
                else
                { 
                    return (DateTime)result;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uPlayers_SleepMode", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }

        public static DateTime CancelSleepMode(string connectionStr, int PlayerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                object result;
                result = db.ExecuteScalar("uPlayers_SleepModeCancel", new object[] { PlayerID });
                if (result is DBNull || result == null)
                {
                    return DateTime.MinValue;
                }
                else
                {
                    return (DateTime)result;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uPlayers_SleepModeCancel", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }


        public static DateTime ActivateVacationMode(string connectionStr, int PlayerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                object result;
                result = db.ExecuteScalar("uPlayers_VacationMode", new object[] { PlayerID });
                if (result is DBNull || result == null)
                {
                    return DateTime.MinValue;
                }
                else
                {
                    return (DateTime)result;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uPlayers_VacationMode", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }

        public static int CancelVacationMode(string connectionStr, int PlayerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                object result;
                result = db.ExecuteScalar("uPlayers_VacationModeCancel", new object[] { PlayerID });
                if (result is DBNull || result == null)
                {
                    return -1;
                }
                else
                {
                    return (int)result;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uPlayers_VacationModeCancel", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }

        public static DateTime ActivateWeekendMode(string connectionStr, int PlayerID, DateTime desiredTime)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                object result;
                result = db.ExecuteScalar("uPlayers_WeekendMode", new object[] { PlayerID, desiredTime });
                if (result is DBNull || result == null)
                {
                    return DateTime.MinValue;
                }
                else
                {
                    return (DateTime)result;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uPlayers_WeekendMode", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("desiredTime", desiredTime.ToString());
                
                throw ex;
            }
            finally
            {
            }
        }

        public static int CancelWeekendMode(string connectionStr, int PlayerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                object result;
                result = db.ExecuteScalar("uPlayers_WeekendModeCancel", new object[] { PlayerID });
                if (result is DBNull || result == null)
                {
                    return -1;
                }
                else
                {
                    return (int)result;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uPlayers_WeekendModeCancel", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
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
        /// <param name="PlayerID"></param>
        /// <param name="invitedIDs">list of facebook ids comman seperated with NO trailing comma.
        /// GOOD: "5555,444,666"
        /// BAD: "555,444,666,"
        /// GOOD: "555"
        /// BAD: "555,"</param>
        public static void Gifts_RegisterSentGifts(int PlayerID, int giftID, string invitedIDs, string RequestID)
        {
            TRACE.InfoLine("in 'Invites_RegisterInvited'");
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteNonQuery(db, "iRegisterInvited_Gift", new object[] { PlayerID, giftID, invitedIDs, RequestID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iRegisterInvited_Gift", e);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AddAdditionalInformation("invitedIDs", invitedIDs);
                throw ex;
            }

        }

        public static int SetAvatarID(Guid userID, int PlayerID, int AvatarID)
        {
            Database db;
            try
            {

                object ret = null;
                db = DatabaseFactory.CreateDatabase();
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("uPlayerSetAvatarID");
                db.AddInParameter(cmd, "@UserID", System.Data.DbType.Guid, userID);
                db.AddInParameter(cmd, "@PlayerID", System.Data.DbType.Int32, PlayerID);
                db.AddInParameter(cmd, "@AvatarID", System.Data.DbType.Int32, AvatarID);
                db.AddOutParameter(cmd, "@Result", System.Data.DbType.Int32, int.MaxValue);
                db.ExecuteNonQuery(cmd);
                ret = cmd.Parameters["@Result"].Value;
                return (int)ret;

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uPlayerSetAvatarID", e);
                ex.AddAdditionalInformation("userID", userID);
                ex.AddAdditionalInformation("AvatarID", AvatarID);
                throw ex;
            }
        }

        /// <summary>
        /// NEVER EVER call this method on its own!
        /// Only bll set avatr method calls this after it has succeeded
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="PlayerID"></param>
        /// <param name="AvatarID"></param>
        public static void UpdateRealmPlayerAvatarID(string connectionStr, int PlayerID, int AvatarID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                db.ExecuteNonQuery("UpdateRealmPlayerAvatarID", new object[] { PlayerID, AvatarID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling UpdateRealmPlayerAvatarID", e);
                ex.AddAdditionalInformation("connectionStr", connectionStr);
                ex.AddAdditionalInformation("PlayerID", PlayerID);
                ex.AddAdditionalInformation("AvatarID", AvatarID);
                throw ex;
            }
        }

        

        public static void Gifts_Buy(int playerID, int giftID, string facebookIDOfPlayer, int rid, int vid, string payout)
        {
            TRACE.InfoLine("in 'Gifts_Buy'");
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteNonQuery(db, "iGift_Buy", new object[] { playerID, giftID, facebookIDOfPlayer, rid, vid, payout });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iGift_Buy", e);
                ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                ex.AddAdditionalInformation("facebookIDOfPlayer", facebookIDOfPlayer);
                ex.AddAdditionalInformation("giftID", giftID);
                throw ex;
            }
        }

        public static void VillageReduction_AddPromoted(string connectionStr, int villageid, int playerid)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                db.ExecuteNonQuery("iPromotedVillage", new object[] { villageid, playerid });                
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling VillageReduction_AddPromoted", e);
                ex.AddAdditionalInformation("connectionStr", connectionStr);
                ex.AddAdditionalInformation("villageid", villageid);
                ex.AddAdditionalInformation("playerid", playerid);
                throw ex;
            }
        }
        public static void VillageReduction_RemovePromoted(string connectionStr, int villageid, int playerid)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                db.ExecuteNonQuery("dPromotedVillage", new object[] { villageid, playerid });                
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling VillageReduction_RemovePromoted", e);
                ex.AddAdditionalInformation("connectionStr", connectionStr);
                ex.AddAdditionalInformation("villageid", villageid);
                ex.AddAdditionalInformation("playerid", playerid);
                throw ex;
            }
        }
        public static DataTable VillageReduction_Villages(string connectionStr, int playerid)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("qPromotedVillages", new object[] { playerid }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling VillageReduction_Villages", e);
                ex.AddAdditionalInformation("connectionStr", connectionStr);
                ex.AddAdditionalInformation("playerid", playerid);
                throw ex;
            }
        }

        public static int GetResearchers(string connectionStr, int playerid)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                return (int)db.ExecuteScalar("qPlayersResearchers", new object[] { playerid });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayersResearchers", e);
                ex.AddAdditionalInformation("connectionStr", connectionStr);
                ex.AddAdditionalInformation("playerid", playerid);
                throw ex;
            }
        }


       
        public static Fbg.Common.BuyResearcherResult BuyResearcher(string connectionStr, int playerID, int maxBoughtResearchers)
        {
            Database db;
            object ret = null;

            try
            {
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("uBuyResearcher");

                db.AddInParameter(cmd, "@PlayerID", System.Data.DbType.Int32, playerID);
                db.AddInParameter(cmd, "@maxBoughtResearchers", System.Data.DbType.Int32, maxBoughtResearchers);
                db.AddOutParameter(cmd, "@Status", System.Data.DbType.Int32, Int32.MaxValue);
                db.AddInParameter(cmd, "@PrintDebugInfo", System.Data.DbType.Boolean, 0);
                db.ExecuteNonQuery(cmd);

                ret = cmd.Parameters["@Status"].Value;
                return (Fbg.Common.BuyResearcherResult)ret;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uBuyResearcher'", e);
                ex.AdditionalInformation.Add("maxBoughtResearchers", maxBoughtResearchers.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AddAdditionalInformation("ret", ret);
                throw ex;
            }
        }


        /// <summary>
        /// return 0 if not successful, or the ID of the research item that was speed up 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="cutMinutes"></param>
        /// <param name="playerID"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static int ResearchSpeedUpUpgradeFromItem(string connectionStr, int cutMinutes, int playerID, out DateTime completionTimeBeforeSpeedUp)
        {
            Database db;
            object ret = null;
            completionTimeBeforeSpeedUp = DateTime.MinValue;
            try
            {
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("uSpeedUpResearch_viaItem");

                db.AddInParameter(cmd, "@MinToCut", System.Data.DbType.Int32, cutMinutes);
                db.AddInParameter(cmd, "@PlayerID", System.Data.DbType.Int32, playerID);
                db.AddOutParameter(cmd, "@success", System.Data.DbType.Int32, Int32.MaxValue);
                db.AddOutParameter(cmd, "@EventTimeBeforeSpeedUp", System.Data.DbType.DateTime, Int32.MaxValue);
                db.AddInParameter(cmd, "@PrintDebugInfo", System.Data.DbType.Boolean, 0);
                db.ExecuteNonQuery(cmd);

                ret = cmd.Parameters["@success"].Value;
                if (ret is DBNull || ret == null || (int)ret == 0)
                {
                    return 0;
                }
                else
                {
                    completionTimeBeforeSpeedUp = (DateTime)cmd.Parameters["@EventTimeBeforeSpeedUp"].Value;
                    return (int)ret;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uSpeedUpUpgrade_viaItem'", e);
                ex.AdditionalInformation.Add("cutMinutes", cutMinutes.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AddAdditionalInformation("ret", ret);
                throw ex;
            }
        }

        public static double ResearchHoursBehind(string connectionStr, int playerid)
        {
            Database db;
            double hoursBehind = 0;
            try
            {
             


                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("qPlayersResearch_HourseBehind");

                db.AddInParameter(cmd, "@PlayerID", System.Data.DbType.Int32, playerid);
                db.AddOutParameter(cmd, "@HoursBehind", System.Data.DbType.Double, Int32.MaxValue);
                db.AddInParameter(cmd, "@PrintDebugInfo", System.Data.DbType.Boolean, 0);
                db.ExecuteNonQuery(cmd);

                return (double)cmd.Parameters["@HoursBehind"].Value;
               
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayersResearch_HourseBehind", e);
                ex.AddAdditionalInformation("connectionStr", connectionStr);
                ex.AddAdditionalInformation("playerid", playerid);
                throw ex;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="playerID"></param>
        /// <param name="completionTimeBeforeSpeedUp"></param>
        /// <param name="cost"></param>
        /// <returns>0 if failed or research item id that was speed up</returns>
        public static int SpeedUpCurrentlyResearchingViaCatchup(string connectionStr, int playerID, out DateTime completionTimeBeforeSpeedUp, out int cost)
        {
            Database db;
            object ret = null;
            completionTimeBeforeSpeedUp = DateTime.MinValue;
            cost = 0;
            try
            {
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("uSpeedUpResearch_viaServants");

                db.AddInParameter(cmd, "@PlayerID", System.Data.DbType.Int32, playerID);
                db.AddOutParameter(cmd, "@success", System.Data.DbType.Int32, Int32.MaxValue);
                db.AddOutParameter(cmd, "@EventTimeBeforeSpeedUp", System.Data.DbType.DateTime, Int32.MaxValue);
                db.AddOutParameter(cmd, "@Cost", System.Data.DbType.Int32, Int32.MaxValue);
                db.AddInParameter(cmd, "@PrintDebugInfo", System.Data.DbType.Boolean, 0);
                db.ExecuteNonQuery(cmd);

                ret = cmd.Parameters["@success"].Value;
                if (ret is DBNull || ret == null || (int)ret == 0)
                {
                    return 0;
                }
                else
                {
                    completionTimeBeforeSpeedUp = (DateTime)cmd.Parameters["@EventTimeBeforeSpeedUp"].Value;
                    cost = (int)cmd.Parameters["@Cost"].Value;
                    return (int)ret;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uSpeedUpResearch_viaServants'", e);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AddAdditionalInformation("ret", ret);
                throw ex;
            }
        }



        #region mapevents

        public static DataTable GetPlayerMapEvents(string connectionStr, int playerid)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("qPlayerMapEvents", new object[] { playerid }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerMapEvents", e);
                ex.AddAdditionalInformation("connectionStr", connectionStr);
                ex.AddAdditionalInformation("playerid", playerid);
                throw ex;
            }
        }

        public static DataTable MapEventsCaravanCatchup(string connectionStr, int playerid, int howManyToSpawn)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("iMapEventsCaravanCatchup", new object[] { playerid, howManyToSpawn }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iMapEventsCaravanCatchup", e);
                ex.AddAdditionalInformation("connectionStr", connectionStr);
                ex.AddAdditionalInformation("playerid", playerid);
                throw ex;
            }
        }

        public static DataRow MapEventsCaravanCardReveal(string connectionStr, int eventID, string newEventData, string newStateData, string newStateData2)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("uMapEventCardReveal", new object[] { eventID, newEventData, newStateData, newStateData2 }).Tables[0].Rows[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uMapEventCardReveal", e);
                ex.AddAdditionalInformation("connectionStr", connectionStr);
                ex.AddAdditionalInformation("eventID", eventID);
                ex.AddAdditionalInformation("newEventData", newEventData);
                ex.AddAdditionalInformation("newStateData", newStateData);
                ex.AddAdditionalInformation("newStateData2", newStateData2);
                throw ex;
            }
        }

        /// <summary>
        /// DeActivate a travelling caravan mapevent, and return all active player mapevents
        /// </summary>
        public static DataTable MapEventCardPicked(string connectionStr, int playerID, int eventID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("uMapEventCardPicked", new object[] { playerID, eventID }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uMapEventCardPicked", e);
                ex.AddAdditionalInformation("connectionStr", connectionStr);
                ex.AddAdditionalInformation("playerID", playerID);
                ex.AddAdditionalInformation("eventID", eventID);
                throw ex;
            }
        }

        public static string MapEventsActivate(string connectionStr, int playerID,int eventID, int typeID, int xCord, int yCord)
        {

            Database db;
            object ret = null;

            try
            {
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("uPlayers_MapEventsActivate");

                db.AddInParameter(cmd, "@playerID", System.Data.DbType.Int32, playerID);
                db.AddInParameter(cmd, "@eventID", System.Data.DbType.Int32, eventID);
                db.AddInParameter(cmd, "@typeID", System.Data.DbType.Int32, typeID);
                db.AddInParameter(cmd, "@xCord", System.Data.DbType.Int32, xCord);
                db.AddInParameter(cmd, "@yCord", System.Data.DbType.Int32, yCord);
                db.AddOutParameter(cmd, "@result", System.Data.DbType.String,int.MaxValue);
              
                db.ExecuteNonQuery(cmd);

                ret = cmd.Parameters["@result"].Value;
                return (string)ret;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uPlayers_MapEventsActivate'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("eventID", eventID.ToString());
                ex.AdditionalInformation.Add("typeID", typeID.ToString());
                ex.AdditionalInformation.Add("xCord", xCord.ToString());
                ex.AdditionalInformation.Add("yCord", yCord.ToString());
                ex.AddAdditionalInformation("ret", ret);
                throw ex;
            }
     
        }

        #endregion mapevents




        #region raids

        //table 0 is all of player's raids
        //table 1 is all results for those raids
        public static DataSet GetPlayerRaids(string connectionStr, int playerid)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("qPlayerRaids", new object[] { playerid });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerRaids", e);
                ex.AddAdditionalInformation("connectionStr", connectionStr);
                ex.AddAdditionalInformation("playerid", playerid);
                throw ex;
            }
        }



        /// <summary>
        /// Send a raid out, return result code
        /// </summary>
        /// <returns></returns>
        public static int SendRaid(string connectionStr, int playerID, int villageID, int raidID, List<Fbg.Common.UnitCommand.Units> attackUnits, DateTime landTime)
        {


            Database db;
            int resultCode;

            //
            // preconditions
            if (attackUnits.Count == 0)
            {
                BaseApplicationException ex = new BaseApplicationException("SendRaid has no units");
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AdditionalInformation.Add("raidID", raidID.ToString());
                throw ex;
            }

            StringBuilder unitTypes = new StringBuilder(attackUnits.Count * 3);
            //StringBuilder unitAmounts = new StringBuilder(attackUnits.Count * 5);
            //StringBuilder unitBuildingTarget = new StringBuilder(attackUnits.Count * 4);

            //
            // create the list of units string           
            //      we keep the last (trailing comma (',') on purpose!
            //
            foreach (Common.UnitCommand.Units units in attackUnits)
            {
                unitTypes.Append(units.utID);
                unitTypes.Append(",");

                //unitAmounts.Append(units.sendCount);
                //unitAmounts.Append(",");

                //unitBuildingTarget.Append(units.targetBuildingTypeID);
                //unitBuildingTarget.Append(",");
            }

            //
            // Execute the stored procedure
            //
            try
            {
               
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("iSendRaid");

                db.AddInParameter(cmd, "@playerID", System.Data.DbType.Int32, playerID);
                db.AddInParameter(cmd, "@villageID", System.Data.DbType.Int32, villageID);
                db.AddInParameter(cmd, "@raidID", System.Data.DbType.Int32, raidID);
                db.AddInParameter(cmd, "@unitTypes", System.Data.DbType.String, unitTypes.ToString());
                db.AddInParameter(cmd, "@landTime", System.Data.DbType.DateTime, landTime);

                db.AddOutParameter(cmd, "@resultCode", System.Data.DbType.Int32, 16);

                db.ExecuteNonQuery(cmd);

                resultCode = (int)db.GetParameterValue(cmd, "@resultCode");

                /*
               db = new DB(connectionStr);
               DataSet raidDataSet = db.ExecuteDataSet("iSendRaid", new object[] { playerID, villageID, raidID, unitTypes.ToString() });
               return raidDataSet;
                */


            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iSendRaid'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AdditionalInformation.Add("raidID", raidID.ToString());
                ex.AdditionalInformation.Add("unitTypes", unitTypes.ToString());
                
                throw ex;
            }

            return resultCode;

        }



        /// <summary>
        /// Get all raid movements for a player
        /// Gets two tables, one is the raid movements, and one is unit details per movement
        /// </summary>
        /// <returns>
        /// TABLE 0: Raid Details
        /// TABLE 1: player's attacks to this raid
        /// TABLE 2: unit details in raid attacks
        /// TABLE 3: all raid results for this raid
        /// TABLE 4: all rewards for this raid
        /// </returns>
        public static DataSet GetRaidDetails(string connectionStr, int playerID, int raidID)
        {

            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("qRaidDetails", new object[] { playerID, raidID });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'qRaidDetails'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("raidID", raidID.ToString());
                throw ex;
            }



        }

        

        public static bool AcceptReward(string connectionStr, int playerID, int raidID)
        {


            Database db;
            int resultCode = 1000;

            try
            {

                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("iRaidRewardAccept");

                db.AddInParameter(cmd, "@playerID", System.Data.DbType.Int32, playerID);
                db.AddInParameter(cmd, "@raidID", System.Data.DbType.Int32, raidID);
                db.AddOutParameter(cmd, "@resultCode", System.Data.DbType.Int32, 16);
                db.ExecuteNonQuery(cmd);

                resultCode = (int)db.GetParameterValue(cmd, "@resultCode");

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iRaidRewardAccept'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("raidID", raidID.ToString());

                throw ex;
            }

            return resultCode == 0; ;

        }


        #endregion raids




        public static DataTable PlayerNotificationSettings_get(string connectionStr, int playerid)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("qPlayerNotificationSettings", new object[] { playerid }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerNotificationSettings", e);
                ex.AddAdditionalInformation("connectionStr", connectionStr);
                ex.AddAdditionalInformation("playerid", playerid);
                throw ex;
            }
        }

        public static void PlayerNotificationSettings_update(string connectionStr, int playerid, int noficationID, Int16 vibrateOptionID, Int16 soundSettingID, Int16 activeStateID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                db.ExecuteNonQuery("uPlayerNotificationSettings", new object[] { playerid, noficationID, vibrateOptionID, soundSettingID, activeStateID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uPlayerNotificationSettings", e);
                ex.AddAdditionalInformation("connectionStr", connectionStr);
                ex.AddAdditionalInformation("playerid", playerid);
                ex.AddAdditionalInformation("noficationID", noficationID);
                ex.AddAdditionalInformation("vibrateOptionID", vibrateOptionID);
                ex.AddAdditionalInformation("soundSettingID", soundSettingID);
                ex.AddAdditionalInformation("activeStateID", activeStateID);
                throw ex;
            }
        }

        public static void AddPlayerNotificationToQ(string connectionStr, int PlayerID, Int16 notificationTypeID, string text)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                db.ExecuteNonQuery("iPlayerNotifications", new object[] { PlayerID, notificationTypeID, text });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iPlayerNotifications", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AddAdditionalInformation("notificationTypeID",notificationTypeID);
                ex.AddAdditionalInformation("text", text);
                throw ex;
            }
            finally
            {
            }
        }


        public static void ChooseGovType(string connectionStr, int playerID ,int govTypeID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                db.ExecuteNonQuery("iPlayerGovType", new object[] { playerID, govTypeID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iPlayerGovType", e);
                ex.AddAdditionalInformation("govTypeID", govTypeID);
                ex.AddAdditionalInformation("playerID", playerID);
                throw ex;
            }
            finally
            {
            }
        }
        /// <summary>
        /// daily rewad of gifts
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="facebookID"></param>
        /// <param name="giftID"></param>
        /// <param name="numGifts">number of gifts</param>
        /// <returns></returns>
        public static bool RewardGifts(int playerID, string facebookID, int giftID, int numGifts)
        {
            TRACE.InfoLine("in 'RewardGifts()'");
            Database db;
            bool result = false;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                result = ((int)db.ExecuteScalar("iGiveGifts", new object[] { playerID, facebookID, giftID, numGifts }))==1;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iGiveGifts", e);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AddAdditionalInformation("facebookID", facebookID);                
                ex.AddAdditionalInformation("giftID", giftID);
                ex.AddAdditionalInformation("numGifts", numGifts);
                throw ex;
            }
            return result;
        }
        /// <summary>
        /// daily reward of chests
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="playerID"></param>
        /// <param name="facebookID"></param>
        /// <param name="numChests"></param>
        /// <returns></returns>
        public static bool RewardChests(string connectionStr, int playerID, int numChests)
        {
            TRACE.InfoLine("in 'RewardChests()'");
            Database db;
            bool result = false;
            try
            {
                db = new DB(connectionStr);

                result = ((int)db.ExecuteScalar("uGiveChests", new object[] { playerID, numChests }))==1;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uGiveChests", e);
                ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                ex.AddAdditionalInformation("numChests", numChests);
                throw ex;
            }                
            return result;
        }

        public static void admin_renamePlayer(int realmID, string connectionStr, string currentName, string newName)
        {
            TRACE.InfoLine("in 'admin_renamePlayer()'");
            Database db;
            try
            {
                db = new DB(connectionStr);

                db.ExecuteNonQuery("uPlayerRenameAdmin", new object[] { currentName, newName, realmID});
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uPlayerRenameAdmin", e);
                ex.AdditionalInformation.Add("realmID", realmID.ToString());
                ex.AddAdditionalInformation("currentName", currentName);
                ex.AddAdditionalInformation("newName", newName);
                throw ex;
            }
           
        }

        public static DataSet PlayerInfo(string connectionStr,int pid)
        {
            TRACE.InfoLine("in 'PlayerInfo()'");
            Database db;
            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qPlayerInfo", new object[] { pid });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerInfo", e);
                ex.AdditionalInformation.Add("pid", pid.ToString());
                throw ex;
            }
        }
    }
}
