using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Fbg.Common;

namespace Fbg.Bll.Api.SilverTransport
{
    /// <summary>
    /// Summary description for Report
    /// </summary>

    public class SilverTransport
    {
        public class CONSTS
        {

            public class NearestVillagesColumnIndex
            {
                public static int VillageID = 0;
                public static int TravelTime = 1;
                public static int XCord = 2;
                public static int YCord = 3;
                public static int Name = 4;
                public static int MinAmount = 5;
            }
            public class NoQTransportCoinsTableIndex
            {
                public static int YesVillages = 0;
                public static int NoVillages = 1;
            }
            public class YesVillagesCloumnIndex
            {
                public static int VillageID = 0;
                public static int OwnerPlayerID = 1;
                public static int VillageName = 2;
                public static int Coins = 3;
                public static int VillageXCord = 4;
                public static int VillageYCord = 5;
            }
            public class NoQTransportCoinsCloumnNames
            {
                public static string VillageID = "VillageID";

            }
        }

        Fbg.Bll.Player _player;
        List<VillageBasicA> _villages = null;
        DataView _dv_QTransports;

        public SilverTransport(Fbg.Bll.Player player)
        {
            _player = player;
        }

        //get info on nearest friendly villages to a given friendly village
        public string GetNearestVillages(int targetVillageId, int minAmount, int xNumVillages)
        {

            DataTable dt = new DataTable();
            Village targetVillage = Fbg.Bll.Village.GetVillage(_player, targetVillageId, false, false);
            dt = Fbg.Bll.CoinTransport.GetNearestVillages(_player, targetVillage, _player.GetVillages_BasicA(null), minAmount, xNumVillages);
  
            System.Web.Script.Serialization.JavaScriptSerializer json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();           
            return json_serializer.Serialize(new
            {
                success = true,
                @object = dt.AsEnumerable().Select(
                    row => new
                    {
                  
                        VillageID = row.Field<Int32>(CONSTS.NearestVillagesColumnIndex.VillageID),
                        TravelTime = getFormattedTime(row.Field<Int64>(CONSTS.NearestVillagesColumnIndex.TravelTime)),
                        XCord = row.Field<Int32>(CONSTS.NearestVillagesColumnIndex.XCord),
                        YCord = row.Field<Int32>(CONSTS.NearestVillagesColumnIndex.YCord),
                        Name = row.Field<String>(CONSTS.NearestVillagesColumnIndex.Name),
                        MinAmount = row.Field<Int32>(CONSTS.NearestVillagesColumnIndex.MinAmount)
                       
                    }
                )
            });

        }

        //get info on nearest friendly villages to a given foreign village
        public string GetNearestVillagesForeign(int targetVillageId)
        {

            DataTable dt = new DataTable();
            Village targetVillage = Fbg.Bll.Village.GetVillage(_player, targetVillageId, false, false);
            dt = Fbg.Bll.CoinTransport.GetVillagesToSendSilverFrom(_player, targetVillageId).Tables[0];

            System.Web.Script.Serialization.JavaScriptSerializer json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return json_serializer.Serialize(new
            {
                success = true,
                @object = dt.AsEnumerable().Select(
                    row => new
                    {

                        VillageID = row.Field<Int32>(CONSTS.NearestVillagesColumnIndex.VillageID),
                        TravelTime = getFormattedTime(row.Field<Int64>(CONSTS.NearestVillagesColumnIndex.TravelTime)),
                        XCord = row.Field<Int32>(CONSTS.NearestVillagesColumnIndex.XCord),
                        YCord = row.Field<Int32>(CONSTS.NearestVillagesColumnIndex.YCord),
                        Name = row.Field<String>(CONSTS.NearestVillagesColumnIndex.Name),
                        MinAmount = row.Field<Int32>(CONSTS.NearestVillagesColumnIndex.MinAmount)

                    }
                )
            });

        }

        private string getFormattedTime(long duration)
        {
            TimeSpan TravelTime = new TimeSpan(duration);
            string Time;
            if (TravelTime.TotalSeconds <= 0)
            {
                Time = "0:00:00";
            }else{
                Time = Utils.FormatDuration(TravelTime);
            }

            return Time;
        }


        public string SendAmount(int vidFrom, int vidTo, int vToX, int vToY, int amountToTransport, bool isMine)
        {

            Village fromVillage = Fbg.Bll.Village.GetVillage(_player, vidFrom, false, false);
            //Village toVillage = Fbg.Bll.Village.GetVillage(_player, vidTo, false, false);

            TransportResult result = Fbg.Bll.CoinTransport.Transport(_player
                , fromVillage.id
                , vidTo
                , fromVillage.Cordinates.X
                , fromVillage.Cordinates.Y
                , vToX
                , vToY
                , amountToTransport
                , amountToTransport
                , isMine);

            if (result == TransportResult.Success)
            {
                return Fbg.Bll.Api.ApiHelper.RETURN_SUCCESS(new
                {
                    success = true,
                    vFromName = fromVillage.name,
                    vFromId = fromVillage.id,
                    vToId = vidTo,
                    amountToTransport = amountToTransport
                });
            }
            else
            {
                return Fbg.Bll.Api.ApiHelper.RETURN_SUCCESS(new
                {
                    success = false,
                    reason = "transport failed."
                });
            }

        }


        public string GetMaxSilverFromNearestVillages(int targetVillageId, int minAmount, int xNumVillages)
        {
            Village toVillage = Fbg.Bll.Village.GetVillage(_player, targetVillageId, false, false);
            DataTable dt = Fbg.Bll.CoinTransport.GetNearestVillages(_player, toVillage, _player.GetVillages_BasicA(null), minAmount, xNumVillages);
            DataView _dv_QTransports = new DataView(dt);
            //Fbg.Bll.CoinTransport.DoGetMaxCoins(_player, _dv_QTransports, toVillage);

            int treasurySize = toVillage.TreasurySize;
            int neededAmount = treasurySize - toVillage.coins;
            for (int i = 0; i < _dv_QTransports.Count; i++)
            {
                int amount = (int)_dv_QTransports[i][CONSTS.NearestVillagesColumnIndex.MinAmount];
                if (neededAmount - amount > 0)
                {
                    TransportResult result = Fbg.Bll.CoinTransport.Transport(_player
                        , (int)_dv_QTransports[i][CONSTS.NearestVillagesColumnIndex.VillageID]
                        , toVillage.id
                         , (int)_dv_QTransports[i][CONSTS.NearestVillagesColumnIndex.XCord]
                        , (int)_dv_QTransports[i][CONSTS.NearestVillagesColumnIndex.YCord]
                        , toVillage.Cordinates.X
                        , toVillage.Cordinates.Y, amount, amount, true);

                    if (result == TransportResult.Success)
                    {
                        neededAmount -= amount;
                    }

                }
                else
                {
                    if (neededAmount > 0)
                    {
                        TransportResult result = Fbg.Bll.CoinTransport.Transport(_player
                        , (int)_dv_QTransports[i][CONSTS.NearestVillagesColumnIndex.VillageID]
                        , toVillage.id
                         , (int)_dv_QTransports[i][CONSTS.NearestVillagesColumnIndex.XCord]
                        , (int)_dv_QTransports[i][CONSTS.NearestVillagesColumnIndex.YCord]
                        , toVillage.Cordinates.X
                        , toVillage.Cordinates.Y, neededAmount, neededAmount, true);

                        if (result == TransportResult.Success)
                        {
                            neededAmount = 0;
                        }
                    }
                }

            }


            return Fbg.Bll.Api.ApiHelper.RETURN_SUCCESS(new
            {
                success = true,
                reason = "Max transport completed"
            });

        }







    }
    

}