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
    public class Villages
    {
        public class CONSTS
        {
            public class VillagesReader
            {
                public static int VillageID = 0;
                public static int Name = 1;
                public static int Coins = 2;
                public static int Points = 3;
                public static int XCord = 4;
                public static int YCord = 5;
                public static int loyalty = 6;
                public static int CoinMineLevel=7;
                public static int TreasuryLevel=8;
                public static int CoinsLastUpdates = 9;
                public static int VillageType = 10;
            }
        }

     

        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.Villages");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="playerID"></param>
        /// <param name="detailedList"></param>
        /// <param name="selectedFilter">pass in filter ID or -1 if no filter selected</param>
        /// <returns></returns>
        public static IDataReader GetVillagesForPlayer(string connectionStr, int playerID, bool detailedList
            , int selectedFilter)
        {
            TRACE.InfoLine("in 'GetVillagesForPlayer()'");
            DB db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteReader("qPlayerVillages", new object[] { playerID
                    , detailedList
                    , selectedFilter == -1 ? null : (object)selectedFilter });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerVillages", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("detailedList", detailedList.ToString());
                ex.AddAdditionalInformation("selectedFilter", selectedFilter);
                
                throw ex;
            }
            finally
            {
            }
        }

        public static IDataReader GetChatByPlayer(string connectionStr, int? playerID, bool isClan, DateTime time, int count, bool older)
        {
            TRACE.InfoLine("in 'GetVillagesForPlayer()'");
            DB db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteReader("qChatByPlayer", new object[] { playerID, isClan, time, count, older });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qChatByPlayer", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("clanID", isClan.ToString());
                ex.AdditionalInformation.Add("time", time.ToString());

                throw ex;
            }
            finally
            {
            }
        }

        public static int DoChatMessage(string connectionStr, int? playerID, string message, bool isClan)
        {
            TRACE.InfoLine("in 'DoChatMessage'.");
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                return (int)db.ExecuteScalar("iChatMessage", new object[] { playerID, message, isClan });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iChatMessage'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("message", message);
                ex.AdditionalInformation.Add("isClan", isClan.ToString());
                throw ex;
            }
        }

        public static DataSet GetOtherVillage(string connectionStr, int villageID,int OwnerID)
        {
            TRACE.InfoLine("in 'GetOtherVillage()'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qVillageInfo_Min", new object[] { villageID, null,null,OwnerID ,null});
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qVillageInfo_Min", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                throw ex;
            }
            finally
            {
            }
        }

  
        public static DataSet VillageBuildingInfo(string connectionStr, int loggedInPlayerID, int villageID
            , bool getTroopMovements, bool getAreTransportsAvail)
        {
            TRACE.InfoLine("in 'VillageBuildingInfo()'");
            Database db;            

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qVillageBuildingInfo", new object[] { loggedInPlayerID, villageID, getTroopMovements, getAreTransportsAvail });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qVillageBuildingInfo", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("loggedInPlayerID", loggedInPlayerID.ToString());
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AddAdditionalInformation("getTroopMovements", getTroopMovements);
                ex.AddAdditionalInformation("getAreTransportsAvail", getAreTransportsAvail);
                throw ex;
            }
        }


        public static DataSet VillageInfo(string connectionStr, int loggedInPlayerID, int villageID,  bool getAreTransportsAvail)
        {
            TRACE.InfoLine("in 'VillageInfo()'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qVillageInfo", new object[] { loggedInPlayerID, villageID, getAreTransportsAvail });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qVillageInfo", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("loggedInPlayerID", loggedInPlayerID.ToString());
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AddAdditionalInformation("getAreTransportsAvail", getAreTransportsAvail);
                throw ex;
            }
        }

        /// <summary>
        /// get the info of the minimal village info - usually for a village 
        ///     other then the logged in player
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="villageID"></param>
        /// <returns></returns>
        public static DataSet VillageInfo_min(string connectionStr, int x, int y)
        {
            TRACE.InfoLine("in 'qVillageInfo_Min()'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qVillageInfo_Min", new object[] { null, x, y, null ,null });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qVillageInfo_Min", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("x", x.ToString());
                ex.AdditionalInformation.Add("y", y.ToString());
                throw ex;
            }
        }
        public static DataSet VillageInfo_min(string connectionStr, string villageName)
        {
            TRACE.InfoLine("in 'qVillageInfo_Min()'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qVillageInfo_Min", new object[] { null, null, null, null,villageName });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qVillageInfo_Min", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("villageName", villageName );

                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="villageID"></param>
        /// <param name="bid"></param>
        /// <param name="level"></param>
        /// <param name="maxQLength">set to 9999 is no limit. Used to enforce q length paid feature.</param>
        public static void DoUpgrade(string connectionStr, int villageID, int bid, int level, int maxQLength)
        {
            TRACE.InfoLine("in 'DoUpgrade'.");
            Database db;

            try
            {
                db = new DB(connectionStr);;
                db.ExecuteNonQuery("iBuildingUpgrade", new object[] { villageID, bid, level, maxQLength, null });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iBuildingUpgrade'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AdditionalInformation.Add("bid", bid.ToString());
                ex.AdditionalInformation.Add("level", level.ToString());
                throw ex;
            }
 
        }

        public static void CancelUpgrade(string connectionStr, long eventID, bool isQ)
        {
            TRACE.InfoLine("in 'CancelUpgrade'.");
            Database db;

            try
            {
                db = new DB(connectionStr);;
                db.ExecuteNonQuery("dCancelEvent", new object[] { eventID, isQ , 0});
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'dCancelEvent'", e);
                ex.AdditionalInformation.Add("eventID", eventID.ToString());
                throw ex;
            }

        }

        
        public static void FoundVillage(string connectionStr, int playerID, string playerName, int? quad)
        {
          
            Database db;

            try 
            {
                db = new DB(connectionStr); ;
                db.ExecuteNonQuery("iCreateVillage", new object[] { playerID, playerName, null, quad , "", 0, 1 });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iCreateVillage'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("playerName", playerName.ToString());
                throw ex;
            }

        }
        public static void Restart_GetNewVillage(string connectionStr, int playerID, string playerName, int? quad)
        {          
            Database db;

            try 
            {
                db = new DB(connectionStr); ;
                db.ExecuteNonQuery("iCreateVillage", new object[] { playerID, playerName, null, quad , "", 0, 0 });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iCreateVillage'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("playerName", playerName.ToString());
                throw ex;
            }

        }

        public static bool AbandonVillage(string connectionStr, int playerID, int villageID, int restartCost)
        {

            Database db;
            bool success = false;

            try
            {
                db = new DB(connectionStr); ;
                db.ExecuteNonQuery("uAbandonVillage", new object[] { playerID, villageID, restartCost });
                //TODO: add return code from insert
                success = true;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uAbandonVillage'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AdditionalInformation.Add("restartCost", restartCost.ToString());
                throw ex;
            }
            return success;
        }
        
        public static void Recruit(string connectionStr, int villageID, int unitTypeID
            , int recruitmentBuildingID, int recruitCount, int totalPopulation, int totalCost,int UnitCost)
        {
            TRACE.InfoLine("in 'Recruit'.");
            Database db;

            try
            {
                db = new DB(connectionStr);;
                db.ExecuteNonQuery("iRecruitUnits", new object[] { villageID, unitTypeID, recruitCount, recruitmentBuildingID, totalPopulation, totalCost ,UnitCost});
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iRecruitUnits'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AdditionalInformation.Add("unitTypeID", unitTypeID.ToString());
                ex.AdditionalInformation.Add("recruitCount", recruitCount.ToString());
                ex.AdditionalInformation.Add("recruitmentBuildingID", recruitmentBuildingID.ToString());
                ex.AdditionalInformation.Add("totalPopulation", totalPopulation.ToString());
                ex.AdditionalInformation.Add("totalCost", totalCost.ToString());
                ex.AdditionalInformation.Add("unitCost", UnitCost.ToString());
                //ex.AdditionalInformation.Add("durationInTicks", durationInTicks.ToString());
                throw ex;
            }
        }

        public static int Disband(string connectionStr,int playerID, int villageID, int unitTypeID,int disbandCount)
        {
            TRACE.InfoLine("in 'Disband'.");
            Database db;

            try
            {
                db = new DB(connectionStr); ;
               return(int) db.ExecuteScalar("uDisbandUnits", new object[] {playerID, villageID, unitTypeID,disbandCount });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uDisbandUnits'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AdditionalInformation.Add("unitTypeID", unitTypeID.ToString());
                ex.AdditionalInformation.Add("disbandCount", disbandCount.ToString());
                throw ex;
            }
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="playerID"></param>
        /// <param name="villageID"></param>
        /// <returns>0 sucess ;1 not enough food ; 2 not enough chests</returns>
        
        public static int RecruitGoverner(string connectionStr,int playerID, int villageID)
        {
            TRACE.InfoLine("in 'Recruit'.");
            Database db;

            try
            {
                db = new DB(connectionStr); ;
               int result=(int) db.ExecuteScalar("iRecruitGoverner", new object[] { playerID,villageID });
               return result;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iRecruitGoverner'", e);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
        
                throw ex;
            }
        }
        /// <summary>
        /// Returns 0 if all was successfull, returns -1 if troops specified are not in the village. 
        /// </summary>
        /// <returns></returns>
        public static int CommandUnits(string connectionStr, DateTime arrivalTime, TimeSpan travelTime
            , UnitCommand command, int originPlayerID, bool hasCommandTroopsPF
            , out int playerMoraleAfterCmd, out DateTime playerMoraleLastUpdatedAfterCmd)
        {
            TRACE.InfoLine("in 'CommandUnits'.");
            Database db;
            object ret;

            StringBuilder unitTypes = new StringBuilder(command.unitsSent.Count * 3);
            StringBuilder unitAmounts = new StringBuilder(command.unitsSent.Count * 5);
            StringBuilder unitBuildingTarget = new StringBuilder(command.unitsSent.Count * 4);

            //
            // preconditions
            if (command.unitsSent == null || command.unitsSent.Count == 0)
            {
                BaseApplicationException ex = new BaseApplicationException("command.unitsSent has no units");
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("arrivalTime", arrivalTime.ToString());
                ex.AdditionalInformation.Add("travelTime", travelTime.ToString());
                command.SerializeToNameValueCollection(ex.AdditionalInformation);
                throw ex;
            }
            //
            // create the list of units string           
            //      we keep the last (trailing comma (',') on purpose!
            //
            foreach (UnitCommand.Units units in command.unitsSent)
            {
                unitTypes.Append(units.utID);
                unitTypes.Append(",");

                unitAmounts.Append(units.sendCount);
                unitAmounts.Append(",");

                unitBuildingTarget.Append(units.targetBuildingTypeID);
                unitBuildingTarget.Append(",");
            }
            //
            // Execute the stored procedure
            //
            try
            {
                /*
                 SqlCommand cmd = connection.CreateCommand();cmd.CommandType = CommandType.StoredProcedure;cmd.CommandText = 
                 * "Test"cmd.Parameters.Add("@x", SqlDbType.Int).Direction = ParameterDirection.Output;
                 * cmd.Parameters.Add("@retval", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;cmd.Execute();int? x = cmd.Parameters["@x"].Value is DBNull ? null : (int?)cmd.Parameters["@x"].Value;
                 */
                 
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("iCommandUnits");

                db.AddInParameter(cmd, "@UnitTypes", System.Data.DbType.String, unitTypes.ToString());
                db.AddInParameter(cmd, "@UnitAmounts", System.Data.DbType.String, unitAmounts.ToString());
                db.AddInParameter(cmd, "@UnitBuildingTargets", System.Data.DbType.String, unitBuildingTarget.ToString());
                db.AddInParameter(cmd, "@OriginVillageID", System.Data.DbType.Int32, command.originVillageID);
                db.AddInParameter(cmd, "@DestVillageID", System.Data.DbType.Int32, command.targetVillageID);
                db.AddInParameter(cmd, "@CommandType", System.Data.DbType.Int32, (int)command.command);
                db.AddInParameter(cmd, "@CompletedOn", System.Data.DbType.DateTime, arrivalTime);
                db.AddInParameter(cmd, "@TripDurationInTicks", System.Data.DbType.Int64, travelTime.Ticks);
                db.AddInParameter(cmd, "@originPlayerID", System.Data.DbType.Int32, originPlayerID);
                db.AddInParameter(cmd, "@hasCommandTroopsPF", System.Data.DbType.Boolean, hasCommandTroopsPF);
                db.AddInParameter(cmd, "@DesertionFactor", System.Data.DbType.Double, command.UnitDesertionFactor);
                db.AddInParameter(cmd, "@PrintDebugInfo", System.Data.DbType.Boolean, 0);
                db.AddOutParameter(cmd, "@playerMoraleAfterCmd", System.Data.DbType.Int32, 16);
                db.AddOutParameter(cmd, "@playerMoraleLastUpdatedAfterCmd", System.Data.DbType.DateTime, 8);
                db.AddParameter(cmd, "@retval", System.Data.DbType.Int32, ParameterDirection.ReturnValue,"", DataRowVersion.Current,0 );
                db.ExecuteNonQuery(cmd);

                playerMoraleAfterCmd = (int)db.GetParameterValue(cmd, "@playerMoraleAfterCmd");
                playerMoraleLastUpdatedAfterCmd = (DateTime)db.GetParameterValue(cmd, "@playerMoraleLastUpdatedAfterCmd");

                ret = cmd.Parameters["@retval"].Value;

                //db = new DB(connectionStr);;
                //ret = db.ExecuteScalar("iCommandUnits", new object[] { 
                //    unitTypes.ToString()
                //    , unitAmounts.ToString()
                //    , unitBuildingTarget.ToString()
                //    , command.originVillageID
                //    , command.targetVillageID
                //    , (int)command.command
                //    , arrivalTime
                //    , travelTime.Ticks
                //    , 0 });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iCommandUnits'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("unitTypes", unitTypes.ToString());
                ex.AdditionalInformation.Add("unitAmounts", unitAmounts.ToString());
                ex.AdditionalInformation.Add("unitBuildingTarget", unitBuildingTarget.ToString());
                ex.AdditionalInformation.Add("arrivalTime", arrivalTime.ToString());
                ex.AdditionalInformation.Add("travelTime", travelTime.ToString());
                ex.AdditionalInformation.Add("hasCommandTroopsPF", hasCommandTroopsPF.ToString());
                command.SerializeToNameValueCollection(ex.AdditionalInformation);
                throw ex;
            }

            if (ret == null || ret is DBNull)
            {
                return 0;
            }
            else
            {
                return (int)ret;
            }
        }

        public static void SetVillageName(string connectionStr, int villageID, string newVillageName)
        {
            TRACE.InfoLine("in 'SetVillageName'.");
            Database db;

            try
            {
                db = new DB(connectionStr);;
                db.ExecuteNonQuery("uVillageName", new object[] { villageID, newVillageName});
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uVillageName'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AdditionalInformation.Add("unitTypeID", newVillageName);
                throw ex;
            }            
        }

        public static void SetVillageType(string connectionStr, int villageID, int villageType, int cost , int playerid)
        {
            TRACE.InfoLine("in 'SetVillageType'.");
            Database db;

            try
            {
                db = new DB(connectionStr); 
                db.ExecuteNonQuery("uVillageType", new object[] { villageID, villageType, cost, playerid });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uVillageType'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AdditionalInformation.Add("villageType", villageType.ToString());
                throw ex;
            }
        }

        public static int UpdateCoins(string connectionStr
            , int villageID
            , int coinsToUpdate
            , out int coinsOverflow)
        {
            Database db;

            coinsOverflow = 0;
            int coinsAfterUpdate=0;
            try
            {
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("uVillageCoins");

                db.AddInParameter(cmd, "@VillageID", System.Data.DbType.Int32, villageID);
                db.AddInParameter(cmd, "@CoinsToAdd", System.Data.DbType.Int32, coinsToUpdate);
                db.AddOutParameter(cmd, "@CoinsOverflow", System.Data.DbType.Int32, int.MaxValue);
                db.AddOutParameter(cmd, "@CoinsAfter", System.Data.DbType.Int32, int.MaxValue);
                db.AddInParameter(cmd, "@PrintDebugInfo", System.Data.DbType.Int32, 0);
                db.ExecuteNonQuery(cmd);

                coinsOverflow = (int)db.GetParameterValue(cmd, "@CoinsOverflow");
                coinsAfterUpdate =  (int)db.GetParameterValue(cmd, "@CoinsAfter");

                return coinsAfterUpdate;

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uVillageCoins'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AddAdditionalInformation("coinsToUpdate", coinsToUpdate);
                ex.AddAdditionalInformation("coinsOverflow", coinsOverflow);
                ex.AddAdditionalInformation("coinsAfterUpdate", coinsAfterUpdate);
                throw ex;
            }            
        }

        public static void AddUnits(string connectionStr, int villageID, int unitTypeID, int amount)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                db.ExecuteNonQuery("uVillageUnits", new object[] { villageID, unitTypeID, amount, 0 });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uVillageUnits'", e);
                ex.AddAdditionalInformation("connectionStr", connectionStr);
                ex.AddAdditionalInformation("villageID", villageID);
                ex.AddAdditionalInformation("unitTypeID", unitTypeID);
                ex.AddAdditionalInformation("amount", amount);
                throw ex;
            }            
            
        }

        public static void DoDowngrade(string connectionStr, int villageID, int bid)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                db.ExecuteNonQuery("iBuildingDowngrade", new object[] { villageID, bid, null });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iBuildingDowngrade'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AdditionalInformation.Add("bid", bid.ToString());
                throw ex;
            }
        }

        public static void CancelDowngrade(string connectionStr, int eventID, bool isQ)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                db.ExecuteNonQuery("dCancelBuildingDowngrade", new object[] { eventID, isQ, 0 });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'dCancelBuildingDowngrade'", e);
                ex.AdditionalInformation.Add("eventID", eventID.ToString());
                throw ex;
            }
        }

        public static int SpeedUpUpgrade(string connectionStr, long eventID, int cutMinutes, int playerID)
        {
            Database db;
            object ret=null;

            try
            {
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("uSpeedUpUpgrade");

                db.AddInParameter(cmd, "@eventID", System.Data.DbType.Int64, eventID);
                db.AddInParameter(cmd, "@MinToCut", System.Data.DbType.Int32, cutMinutes);
                db.AddInParameter(cmd, "@PlayerID", System.Data.DbType.Int32, playerID);
                db.AddOutParameter(cmd, "@Cost", System.Data.DbType.Int32, Int32.MaxValue);
                db.AddInParameter(cmd, "@PrintDebugInfo", System.Data.DbType.Boolean, 0);
                db.ExecuteNonQuery(cmd);

                ret = cmd.Parameters["@Cost"].Value;
                if (ret is DBNull || ret == null)
                {
                    return 0;
                }
                else
                {
                    return (Int32)ret;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uSpeedUpUpgrade'", e);
                ex.AdditionalInformation.Add("eventID", eventID.ToString());
                ex.AdditionalInformation.Add("cutMinutes", cutMinutes.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AddAdditionalInformation("ret", ret);
                throw ex;
            }
        }

        public static bool SpeedUpUpgradeFromItem(string connectionStr, long eventID, int cutMinutes, int playerID)
        {
            Database db;
            object ret=null;

            try
            {
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("uSpeedUpUpgrade_viaItem");

                db.AddInParameter(cmd, "@eventID", System.Data.DbType.Int64, eventID);
                db.AddInParameter(cmd, "@MinToCut", System.Data.DbType.Int32, cutMinutes);
                db.AddInParameter(cmd, "@PlayerID", System.Data.DbType.Int32, playerID);
                db.AddOutParameter(cmd, "@success", System.Data.DbType.Int32, Int32.MaxValue);
                db.AddInParameter(cmd, "@PrintDebugInfo", System.Data.DbType.Boolean, 0);
                db.ExecuteNonQuery(cmd);

                ret = cmd.Parameters["@success"].Value;
                if (ret is DBNull || ret == null || (int)ret == 0)
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
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uSpeedUpUpgrade_viaItem'", e);
                ex.AdditionalInformation.Add("eventID", eventID.ToString());
                ex.AdditionalInformation.Add("cutMinutes", cutMinutes.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AddAdditionalInformation("ret", ret);
                throw ex;
            }
        }

        

        public static bool SpeedUpUpgradeFree(string connectionStr, long eventID, int playerID)
        {
            Database db;
            object ret = null;

            try
            {
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("uSpeedUpUpgradeFree");

                db.AddInParameter(cmd, "@eventID", System.Data.DbType.Int64, eventID);
                db.AddInParameter(cmd, "@PlayerID", System.Data.DbType.Int32, playerID);
                db.AddInParameter(cmd, "@Success", System.Data.DbType.Boolean, 0);
                db.AddInParameter(cmd, "@PrintDebugInfo", System.Data.DbType.Boolean, 0);
                db.ExecuteNonQuery(cmd);

                ret = cmd.Parameters["@Success"].Value;
                if (ret is DBNull || ret == null)
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
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uSpeedUpUpgrade'", e);
                ex.AdditionalInformation.Add("eventID", eventID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AddAdditionalInformation("ret", ret);
                throw ex;
            }
        }

        

        public static int SpeedUpDowngrade(string connectionStr, long eventID, int playerID)
        {
            Database db;
            object ret = null;

            try
            {
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("uSpeedUpDowngrade");

                db.AddInParameter(cmd, "@eventID", System.Data.DbType.Int64, eventID);
                db.AddInParameter(cmd, "@PlayerID", System.Data.DbType.Int32, playerID);
                db.AddOutParameter(cmd, "@Cost", System.Data.DbType.Int32, Int32.MaxValue);
                db.AddInParameter(cmd, "@PrintDebugInfo", System.Data.DbType.Boolean, 0);
                db.ExecuteNonQuery(cmd);

                ret = cmd.Parameters["@Cost"].Value;
                if (ret is DBNull || ret == null)
                {
                    return 0;
                }
                else
                {
                    return (Int32)ret;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uSpeedUpDowngrade'", e);
                ex.AdditionalInformation.Add("eventID", eventID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AddAdditionalInformation("ret", ret);
                throw ex;
            }
        }

        public static bool BoostApproval(string connectionStr, long villageID, int playerID)
        {
            Database db;
            object ret = null;

            try
            {
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("uBoostLoyalty");

                db.AddInParameter(cmd, "@villageID", System.Data.DbType.Int64, villageID);
                db.AddInParameter(cmd, "@PlayerID", System.Data.DbType.Int32, playerID);
                db.AddOutParameter(cmd, "@Cost", System.Data.DbType.Int32, Int32.MaxValue);
                db.AddInParameter(cmd, "@PrintDebugInfo", System.Data.DbType.Boolean, 0);
                db.ExecuteNonQuery(cmd);

                ret = cmd.Parameters["@Cost"].Value;
                if (ret is DBNull || ret == null || (Int32)ret == 0)
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
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uBoostLoyalty'", e);
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AddAdditionalInformation("ret", ret);
                throw ex;
            }
        }


        public static void DoResearch(string connectionStr, int playerid, int villageid, int ritid, int riid, int maxItemsResearching)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                db.ExecuteNonQuery("iResearchInProgress", new object[] { playerid, villageid, ritid, riid, maxItemsResearching, 0 });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iResearchInProgress'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("playerid", playerid);
                ex.AddAdditionalInformation("ritid", ritid);
                ex.AddAdditionalInformation("riid", riid);
                ex.AddAdditionalInformation("villageid", villageid);
                ex.AddAdditionalInformation("maxItemsResearching", maxItemsResearching);
                throw ex;
            }
        }

        public static DataTable GetAllVillages(string connectionStr)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                return db.ExecuteDataSet("qDataFiles_Village").Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'qDataFiles_Village'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                throw ex;
            }
        }
    }
}
