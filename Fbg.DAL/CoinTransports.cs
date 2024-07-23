

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
    public class CoinTransports
    {
        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.CoinTransports");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="villageID">get cois transports for my selected village</param>
        /// <returns></returns>
        public static DataSet  GetCoinTransports(string connectionStr, int villageID, int? playerid)
        {
            TRACE.InfoLine("in 'GetCoinTransports()'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qCoinTranports", new object[] { villageID, playerid });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qCoinTranports", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                ex.AdditionalInformation.Add("playerid", playerid.ToString());
                throw ex;
            }

        }
        public static Fbg.Common.TransportResult Transport(string connectionStr, int OriginVillageID
            ,int DestinationVillageID, int Amount
            ,Int64 TripDuration,int TripDirection
            ,DateTime TravelTime,bool Reserved
            , int PlayerID)
        {
            TRACE.InfoLine("in 'Transport()'");
            Database db;

            try
            {
                db = new DB(connectionStr);;

                int result = (int)db.ExecuteScalar("iTranportCoins", new object[] { PlayerID, OriginVillageID
                    , DestinationVillageID, Amount
                    , TripDuration, TripDirection
                    , TravelTime, Reserved, DateTime.Now });
                return (Fbg.Common.TransportResult)result;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iTranportCoins", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("OriginVillageID", OriginVillageID.ToString());
                ex.AdditionalInformation.Add("DestinationVillageID", DestinationVillageID.ToString());
                ex.AdditionalInformation.Add("Amount", Amount.ToString());
                ex.AdditionalInformation.Add("TripDuration", TripDuration.ToString());
                ex.AdditionalInformation.Add("TripDirection", TripDirection.ToString());
                ex.AdditionalInformation.Add("TravelTime", TravelTime.ToString());
                ex.AdditionalInformation.Add("Reserved", Reserved.ToString());
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                throw ex;
            }
        }
        public static void CancelTransport(string connectionStr, int EventID,int PlayerID)
        {
            TRACE.InfoLine("in 'Transport()'");
            Database db;

            try
            {
                db = new DB(connectionStr);;

                db.ExecuteScalar("dCancelTranportCoins", new object[] { EventID ,PlayerID });
              
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dCancelTranportCoins", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("EventID", EventID.ToString());
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="SelectedVillageID"></param>
        /// <param name="PlayerID"></param>
        /// <param name="minAmount">only take villages with this minimum of silver to transport</param>
        /// <returns></returns>
        public static DataSet GetNearestVillages(string connectionStr, int SelectedVillageID, int PlayerID, int minAmount, int firstXVillages)
        {
            TRACE.InfoLine("in 'GetNearestVillages()'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qNearestVillages", new object[] { SelectedVillageID, PlayerID, minAmount, firstXVillages });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qNearestVillages", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("SelectedVillageID", SelectedVillageID.ToString());
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                throw ex;
            }

        }
        public static DataSet GetNoQTransportCoinsVillages(string connectionStr,  int PlayerID)
        {
            TRACE.InfoLine("in 'GetNoQTransportCoins()'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qNoTransportCoinsVillages", new object[] {  PlayerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qNoTransportCoinsVillages", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);             
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                throw ex;
            }

        }
        public static void AddToNoTransportsCoinsList(string connectionStr,int PlayerID, int VillageID)
        {
            TRACE.InfoLine("in 'AddToNoTransportsCoins'.");
            Database db;
            try
            {
                db = new DB(connectionStr); ;

                db.ExecuteScalar("iNoTransportCoinsVillages", new object[] {PlayerID , VillageID });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iNoTransportCoinsVillages'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("VillageID", VillageID.ToString());

                throw ex;
            }
        }
        public static void DeleteFromNoTransportsCoinsList(string connectionStr, int PlayerID, int VillageID)
        {
            TRACE.InfoLine("in 'DeleteFromNoTransportsCoins'.");
            Database db;
            try
            {
                db = new DB(connectionStr); ;

                db.ExecuteScalar("dNoTransportCoinsVillages", new object[] { PlayerID, VillageID });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'dNoTransportCoinsVillages'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("VillageID", VillageID.ToString());

                throw ex;
            }
        }
        public static DataSet GetVillagesToSendSilverFrom(string connectionStr, int ReceiverVillageID, int PlayerID)
        {
        
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qVillagesToSendSilverFrom", new object[] { ReceiverVillageID, PlayerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling ToSendSilverFrom", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("ReceiverVillageID", ReceiverVillageID.ToString());
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                throw ex;
            }
    }
    }
}

