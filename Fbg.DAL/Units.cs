

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
    public  class Units
    {
        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.Units");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="playerID">always must be specified and must be the owner of the villageID if specified</param>
        /// <param name="villageID">set to null if getting getting for all villages for this player. </param>
        /// <param name="supportedVillageID">set to null if getting getting troops abroad in any village, otherwise will get troops supporting just this village</param>
        /// <returns></returns>
        public static DataTable GetMyUnitsAbroad(string connectionStr, int playerID, int? villageID, int? supportedVillageID)
        {
            TRACE.InfoLine("in 'GetMyUnitsAbroad()'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qUnitsAbroad", new object[] 
                    { 
                        playerID 
                        , villageID 
                        , supportedVillageID
                    }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qUnitsAbroad", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("playerID", playerID);
                ex.AddAdditionalInformation("villageID", villageID);
                ex.AddAdditionalInformation("supportedVillageID", supportedVillageID);
                throw ex;
            }

        }
        /// <summary>
        /// recall some of troops
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="command">use this value to troops(pass null to recall all or command to recall some)</param>
        /// <returns></returns>
        public static void RecallUnits(string connectionStr, int supportingPlayerID, int supportingVillageID
            , int supportedVillageID, UnitCommand command)
        {
            TRACE.InfoLine("in 'RecallSomeUnits()'");
            Database db;

            StringBuilder unitTypes = null;
            StringBuilder unitAmounts = null;

            //
            // preconditions
            try
            {

                if (command != null)
                {
                    if (command.unitsSent != null)
                    {
                        unitTypes = new StringBuilder(command.unitsSent.Count * 3);
                        unitAmounts = new StringBuilder(command.unitsSent.Count * 5);

                        //
                        // create the list of units string           
                        //      we keep the last (trailing comma (',') on purpose!
                        //
                        foreach (UnitCommand.Units units in command.unitsSent)
                        {
                            if (units.sendCount > 0)
                            {
                                unitTypes.Append(units.utID);
                                unitTypes.Append(",");

                                unitAmounts.Append(units.sendCount);
                                unitAmounts.Append(",");
                            }
                        }
                        if (String.IsNullOrEmpty(unitTypes.ToString()) || String.IsNullOrEmpty(unitAmounts.ToString()))
                        {
                            //this means recall some has no value to recall--throw exception
                            BaseApplicationException ex = new BaseApplicationException("Error while recall units amount and types string");
                            ex.AdditionalInformation.Add("connectionStr", connectionStr);
                            ex.AdditionalInformation.Add("supportingPlayerID", supportingPlayerID.ToString());
                            ex.AdditionalInformation.Add("supportingVillageID", supportingVillageID.ToString());
                            ex.AdditionalInformation.Add("supportedVillageID", supportedVillageID.ToString());
                            ex.AddAdditionalInformation("unitTypes", unitTypes);
                            ex.AddAdditionalInformation("unitAmounts", unitAmounts);
                            ex.AddAdditionalInformation("command", command);
                            throw ex;
                        }
                    }

                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while recall units amount and types string", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("supportingPlayerID", supportingPlayerID.ToString());
                ex.AdditionalInformation.Add("supportingVillageID", supportingVillageID.ToString());
                ex.AdditionalInformation.Add("supportedVillageID", supportedVillageID.ToString());
                ex.AddAdditionalInformation("unitTypes", unitTypes);
                ex.AddAdditionalInformation("unitAmounts", unitAmounts);
                ex.AddAdditionalInformation("command", command);
                throw ex;
            }


            try
            {
                db = new DB(connectionStr); ;

                db.ExecuteNonQuery("uRecallSomeUnits", new object[] 
                    { 
                        supportingPlayerID 
                        , null
                        , supportingVillageID 
                        , supportedVillageID
                        , unitTypes==null?null : unitTypes.ToString ()
                        , unitAmounts==null?null:unitAmounts.ToString()
                        ,0
                    });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uRecallSomeUnits", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("supportingPlayerID", supportingPlayerID.ToString());
                ex.AdditionalInformation.Add("supportingVillageID", supportingVillageID.ToString());
                ex.AdditionalInformation.Add("supportedVillageID", supportedVillageID.ToString());
                ex.AddAdditionalInformation("unitTypes", unitTypes);
                ex.AddAdditionalInformation("unitAmounts", unitAmounts);
                ex.AddAdditionalInformation("command", command);
                throw ex;
            }


        }


        public static DataTable GetUnitMovementDetails(string connectionStr, int playerID, long EventID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qUnitMovementDetails", new object[] 
                    { 
                        EventID
                        ,playerID
                    }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qUnitMovementDetails", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("playerID", playerID);
                ex.AdditionalInformation.Add("EventID", EventID.ToString());
                throw ex;
            }

        }

        /// <summary>
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="loggedInPlayerID">cannot be null!! Represents the ID of the player viewing these troops 
        /// movements to ensure he can actually see them</param>
        /// <param name="fromPlayerID">can be null</param>
        /// <param name="destVillageID">can be -1</param>
        /// <param name="originVillageID">can be -1</param>
        /// <returns></returns>
        public static DataTable GetOutgoingTroops(string connectionStr
            ,  long loggedInPlayerID
            , long fromPlayerID
            , int destVillageID
            , int originVillageID
            , bool showAttacks
            , bool showSupports
            , bool showHidden)
        {
            TRACE.InfoLine("in 'GetOutgoingTroops()'");
            Database db;
            object fromPlayerIDOject = fromPlayerID == -1 ? null : (object)fromPlayerID;
            object destinationVillageIDOject = destVillageID == -1 ? null : (object)destVillageID;
            object originVillageIDOject = originVillageID == -1 ? null : (object)originVillageID;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qOutgoingTroops", new object[] 
                    { 
                        loggedInPlayerID
                        , fromPlayerIDOject
                        ,destinationVillageIDOject
                        ,originVillageIDOject
                        , showAttacks
                        , showSupports
                        , 1
                        , showHidden
                    }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qOutgoingTroops", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("loggedInPlayerID", loggedInPlayerID);
                ex.AdditionalInformation.Add("fromPlayerID", fromPlayerID.ToString());
                ex.AdditionalInformation.Add("destVillageID", destVillageID.ToString());
                ex.AdditionalInformation.Add("originVillageID", originVillageID.ToString());
                ex.AddAdditionalInformation("showHidden", showHidden);
                throw ex;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="loggedInPlayerID">cannot be null!! Represents the ID of the player viewing these troops 
        /// movements to ensure he can actually see them</param>
        /// <returns></returns>
        public static DataSet GetOutgoingTroops2(string connectionStr
            , long loggedInPlayerID)
        {
            Database db;
           

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qOutgoingTroops2", new object[] 
                    { 
                        loggedInPlayerID
                        , null
                        , null
                        , null
                        , true
                        , true
                        , 1
                        , true
                    });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qOutgoingTroops", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("loggedInPlayerID", loggedInPlayerID);               
                throw ex;
            }

        }

        public static DataTable GetIncomingTroops(string connectionStr
            , int toPlayerID
            , int destVillageID
            , int originVillageID
            , int originPlayerID
            , bool showAttacks
            , bool showSupports
            , bool showReturns
             , bool showHidden)

        {
            TRACE.InfoLine("in 'GetIncomingTroops()'");
            Database db;
           
            object destinationVillageIDOject = destVillageID == -1 ? null : (object)destVillageID;
            object toPlayerIDOject = toPlayerID == -1 ? null : (object)toPlayerID;
            object originVillageIDOject = originVillageID == -1 ? null : (object)originVillageID;
            object originPlayerIDOject = originPlayerID == -1 ? null : (object)originPlayerID;


            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qIncomingTroops", new object[] 
                    { 
                        toPlayerIDOject
                        ,destinationVillageIDOject
                        , originVillageIDOject
                        , originPlayerIDOject
                        , showAttacks
                        , showSupports
                        , showReturns
                        , 1
                        , showHidden
                    }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qIncomingTroops", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("toPlayerID", toPlayerID.ToString());
                ex.AdditionalInformation.Add("destVillageID", destVillageID.ToString());
                ex.AdditionalInformation.Add("originVillageID", originVillageID.ToString());
                ex.AdditionalInformation.Add("originPlayerID", originPlayerID.ToString());
                ex.AddAdditionalInformation("showHidden", showHidden);
                throw ex;
            }
        }

        public static DataSet GetIncomingTroops2(string connectionStr
           , int toPlayerID, string sp)
        {
            Database db;


            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet(sp, new object[] 
                    { 
                        toPlayerID
                        , null        // ignored
                        , null        // ignored
                        , null        // ignored
                        , true      // ignored
                        , true      // ignored
                        , true      // ignored
                        , 1         // ignored
                        , true      // ignored
                    });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling " + sp, e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("toPlayerID", toPlayerID.ToString());
                throw ex;
            }
        }
        public static void CancelUnitMovement(string connectionStr, long EventID, DateTime newEventTime, int eventsOwnerPlayerID)
        {
            TRACE.InfoLine("in 'CancelUnitMovement()'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                db.ExecuteDataSet("dCancelUnitMovement", new object[] 
                    { 
                        EventID 
                        , newEventTime
                        , eventsOwnerPlayerID
                    });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dCancelUnitMovement", e);
                ex.AddAdditionalInformation("connectionStr", connectionStr);
                ex.AddAdditionalInformation("EventID", EventID);
                ex.AddAdditionalInformation("eventsOwnerPlayerID", eventsOwnerPlayerID);
                throw ex;
            }

        }


        public static int GetLordCostMultiplier(string connectionStr, int playerID)
        {
            TRACE.InfoLine("in 'qLordUnitTypeCost()'");
            Database db;

            try
            {
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("qLordUnitTypeCost");

                db.AddInParameter(cmd, "@PlayerID", System.Data.DbType.String, playerID);
                db.AddOutParameter(cmd, "@GovCost", System.Data.DbType.Int32, int.MaxValue);
                db.ExecuteNonQuery(cmd);
                int Multiplier = (int)db.GetParameterValue(cmd, "@GovCost");

                return Multiplier;

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qLordUnitTypeCost", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
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
        /// <param name="QEntryID"></param>
        /// <param name="VillageID"></param>
        /// <returns>the TrasuryOverflow</returns>
        public static int  CancelRecruit(string connectionStr, int QEntryID,int VillageID)
        {
            TRACE.InfoLine("in 'CancelRecruit'.");
            Database db;
            int TrasuryOverflow = 0;
            try
            {
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("dCancelRecruit");

                db.AddInParameter(cmd, "@QEntryID", System.Data.DbType.String, QEntryID);
                db.AddInParameter(cmd, "@VillageID", System.Data.DbType.String, VillageID);
                db.AddOutParameter(cmd, "@TrasuryOverflow", System.Data.DbType.Int32, int.MaxValue);
                db.ExecuteNonQuery(cmd);
                if ( !(db.GetParameterValue(cmd, "@TrasuryOverflow") is DBNull) )
                {
                    TrasuryOverflow = (int)db.GetParameterValue(cmd, "@TrasuryOverflow");
                }
                return TrasuryOverflow;
            }
            ///this function Cancel recruit 

            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'dCancelRecruit'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("QEntryID", QEntryID.ToString());
                ex.AdditionalInformation.Add("VillageID", VillageID.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="VillageID"></param>
        public static void CancelGovernerRecruit(string connectionStr,  int VillageID,int PlayerID)
        {
            TRACE.InfoLine("in 'CancelGovernorRecruit'.");
            Database db;

            try
            {
                db = new DB(connectionStr);
                db.ExecuteDataSet("dCancelGovernorRecruit", new object[] { VillageID,PlayerID });
            }
            ///this function Cancel recruit 

            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'dCancelGovernorRecruit'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("VillageID", VillageID.ToString());
                ex.AdditionalInformation.Add("PlayerID", VillageID.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="playerID">cannot be null</param>
        /// <param name="villageID">can be -1</param>
        /// <returns></returns>
        public static DataTable GetSupportingTroops(string connectionStr, long playerID, int villageID)
        {
            TRACE.InfoLine("in 'GetSupportingTroops()'");
            Database db;
            object villageIDOject = villageID == -1 ? null : (object)villageID;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qSupportingTroops", new object[] 
                    { 
                        playerID
                        ,villageIDOject
                    }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qSupportingTroops", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AdditionalInformation.Add("villageIDOject", villageIDOject == null ? "null" : villageIDOject.ToString());
                throw ex;
            }

        }

        public static void SendBackSupport(string connectionStr, int supportedPlayerID,  int supportingVillageID, int supportedVillageID)
        {
            TRACE.InfoLine("in 'SendBackSupport()'");
            Database db;

            try
            {
                db = new DB(connectionStr);;

                db.ExecuteNonQuery("uRecallSomeUnits", new object[] 
                    { 
                        null
                        , supportedPlayerID
                        , supportingVillageID 
                        , supportedVillageID
                        ,null 
                        ,null
                        ,0
                    });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uRecallSomeUnits", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("supportedPlayerID", supportedPlayerID.ToString());
                ex.AdditionalInformation.Add("supportingVillageID", supportingVillageID.ToString());
                ex.AdditionalInformation.Add("supportedVillageID", supportedVillageID.ToString());
                throw ex;
            }
        }


        /// <summary>
        /// returns 0 if now the event is not hidden 
        /// returns 1 if now the event is hidden 
        /// </summary>
        public static short ToggleHideUnitMovement(string connectionStr, long EventID, int eventsOwnerPlayerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                return (short)db.ExecuteDataSet("uToggleHideUnitMovement", new object[] 
                    { 
                        EventID 
                        , eventsOwnerPlayerID
                    }).Tables[0].Rows[0][0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uToggleHideUnitMovement", e);
                ex.AddAdditionalInformation("connectionStr", connectionStr);
                ex.AddAdditionalInformation("eventsOwnerPlayerID", eventsOwnerPlayerID);
                throw ex;
            }

        }

 
    }
}

