using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using Fbg.Common;

namespace Fbg.Bll
{
    public class CoinTransport
    {
        public class CONSTS
        {

            public class NearestVillagesColumnIndex
            {
                public static int VillageID = 0;
                public static int TravelTime = 1;
                public static int XCord = 2;
                public static int YCord=3;
                public static int Name=4;
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
                public static int Coins =3;
                public static int VillageXCord = 4;
                public static int VillageYCord = 5;
            }
            public class NoQTransportCoinsCloumnNames
            {
                public static string VillageID = "VillageID";
                
            }
        }

        public static DataSet GetCoinTransports(Player Player, Village SelectedVillage)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");
            }
            if (SelectedVillage == null)
            {
                throw new ArgumentNullException("SelectedVillage is null");
            }
            return DAL.CoinTransports.GetCoinTransports(Player.Realm.ConnectionStr, SelectedVillage.id, null);            

        }

        public static DataSet GetAllCoinTransports(Player Player, Village SelectedVillage)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");
            }
            if (SelectedVillage == null)
            {
                throw new ArgumentNullException("SelectedVillage is null");
            }
            return DAL.CoinTransports.GetCoinTransports(Player.Realm.ConnectionStr, SelectedVillage.id, Player.ID);

        }
        /// <summary>
        /// returns a datatable described by Fbg.Bll.CoinTransport.CONSTS.NearestVillagesColumnIndex
        /// </summary>
        /// <param name="Player"></param>
        /// <param name="SelectedVillage"></param>
        /// <param name="minAmount">only take villages with this minimum of silver to transport</param>
        /// <param name="firstXVillages">List only top X villages</param>
        /// <returns></returns>
        public static DataTable GetNearestVillages(Player player, Village selectedVillage, List<VillageBasicA> villages
            , int minAmount, int firstXVillages)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player is null");
            }

            DataTable dt;
            dt = DAL.CoinTransports.GetNearestVillages(player.Realm.ConnectionStr, selectedVillage.id, player.ID,  minAmount, firstXVillages).Tables[0];
            //
            // update the MinAmount column to hold the true minimum transport amount. 
            //  from DB, this column holds the minimum transport amount as far as the treadepost transport capacity is concenrned ONLY 
            //
            VillageBasicA v;
            int min1;
            foreach (DataRow dr in dt.Rows)
            {
                v = player.VillageBasicA((int)dr[CONSTS.NearestVillagesColumnIndex.VillageID], villages);
                min1 = Math.Min(v.coins, (int)dr[CONSTS.NearestVillagesColumnIndex.MinAmount]);
                dr[CONSTS.NearestVillagesColumnIndex.MinAmount] = Math.Min(min1, selectedVillage.TreasurySize - selectedVillage.coins);
                if ((int)dr[CONSTS.NearestVillagesColumnIndex.MinAmount] < minAmount)
                {
                    dr.Delete();
                }
            }
            dt.AcceptChanges();

            return dt;
        }
        public static TransportResult Transport(Player Player, Village originVillage
            , VillageOther destinationVillage, object Amount, int MaxAmount, bool Reserved)
        {
            TimeSpan TripDuration = GetTravelTime(Player, originVillage, destinationVillage);
            return Transport(Player, originVillage.id, destinationVillage.ID, Amount, MaxAmount, Reserved, TripDuration);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Player"></param>
        /// <param name="OriginVillageID"></param>
        /// <param name="DestVillageID"></param>
        /// <param name="OriginXCord"></param>
        /// <param name="OriginYCord"></param>
        /// <param name="DestXCord"></param>
        /// <param name="DestYCord"></param>
        /// <param name="Amount"></param>
        /// <param name="MaxAmount"></param>
        /// <param name="Reserved"></param>
        /// <param name="getMaxIfAmountNotAvail">if YES, transport will transport max possible amount if amount requested is not available for transport</param>
        /// <returns></returns>
        public static TransportResult Transport(Player Player, int OriginVillageID, int DestVillageID
            , int OriginXCord, int OriginYCord, int DestXCord
            , int DestYCord, object Amount, int MaxAmount
            , bool Reserved)
        {
            TimeSpan TripDuration = GetTravelTime(Player, OriginXCord, OriginYCord, DestXCord, DestYCord);
            return Transport(Player, OriginVillageID, DestVillageID, Amount, MaxAmount, Reserved, TripDuration);
        }

        /// <summary>
        /// private helper method
        /// </summary>
        /// <param name="Player"></param>
        /// <param name="OriginVillageID"></param>
        /// <param name="DestVillageID"></param>
        /// <param name="Amount"></param>
        /// <param name="MaxAmount"></param>
        /// <param name="Reserved"></param>
        /// <param name="tripDuration"></param>
        /// <param name="getMaxIfAmountNotAvail">if YES, transport will transport max possible amount if amount requested is not available for transport</param>
        /// <returns></returns>
        private static TransportResult Transport(Player Player
            , int OriginVillageID
            , int DestVillageID
            , object Amount
            , int MaxAmount
            , bool Reserved
            , TimeSpan tripDuration)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("owner is null");

            }
            TransportResult result = ValidateAmount(Amount, MaxAmount);
            if (result == TransportResult.Success)
            {
                DateTime TravelTime = DateTime.Now.Add(tripDuration);
                return DAL.CoinTransports.Transport(Player.Realm.ConnectionStr, OriginVillageID
                    , DestVillageID, Convert.ToInt32(Amount)
                    , tripDuration.Ticks, (int)TransportDirection.Transporting
                    , TravelTime, Reserved, Player.ID);
            }
            else
            {
                return result;
            }

        }
        public static void CancelTransport(Player Player, int EventID)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("owner is null");

            }

            DAL.CoinTransports.CancelTransport(Player.Realm.ConnectionStr, EventID,Player.ID);

        }
        private static TransportResult ValidateAmount(object Amount ,int MaxAmount)
        {
           
            int Result = 0;
            if (int.TryParse(Amount.ToString (), out Result))
            {
                if (Result > 0)
                {
                    if (Result > MaxAmount)
                    {
                        //coins transported more then allowed
                        return TransportResult.Coins_More_then_Allowed;
                    }
                    else
                    {
                        return TransportResult.Success;
                    }

                }
                else
                {

                    return TransportResult.Coins_must_be_greater_then_Zero;
                }

            }
            else
            {
               
                return TransportResult.Only_Numbers_Accepted;

            }

        }
        public static TimeSpan GetTravelTime(Player Player,Village originVillage, VillageOther destinationVillage)
        {
            double distance =  Village.CalculateDistance(originVillage.Cordinates.X, originVillage.Cordinates.Y
                , destinationVillage.Cordinates.X, destinationVillage.Cordinates.Y);

            return new TimeSpan(Convert.ToInt64(System.Math.Floor((distance /Player.Realm.CoinTransportSpeed ) * TimeSpan.TicksPerHour)));
        }
        public static TimeSpan GetTravelTime(Player Player, int OriginXCord,int OriginYCord,int DestXCord,int DestYCord)
        {
            double distance = Village.CalculateDistance(OriginXCord, OriginYCord
                , DestXCord, DestYCord);

            return new TimeSpan(Convert.ToInt64(System.Math.Floor((distance / Player.Realm.CoinTransportSpeed) * TimeSpan.TicksPerHour)));
        }
        public static void DoGetMaxCoins(Player Owner,DataView NearestVillages, Village SelectedVillage)
        {
            int treasurySize = SelectedVillage.TreasurySize;
            int neededAmount = treasurySize - SelectedVillage.coins;//this hold the amount needed by selectd village
            for (int i = 0; i < NearestVillages.Count; i++)
            {
                int amount = (int)NearestVillages[i][CONSTS.NearestVillagesColumnIndex.MinAmount];//this hold the amount avialabe in this village to give
                if (neededAmount - amount > 0)
                {
                    TransportResult result = Fbg.Bll.CoinTransport.Transport(Owner
                        ,(int)NearestVillages[i][CONSTS.NearestVillagesColumnIndex.VillageID]
                        , SelectedVillage.id
                         , (int)NearestVillages[i][CONSTS.NearestVillagesColumnIndex.XCord]
                        , (int)NearestVillages[i][CONSTS.NearestVillagesColumnIndex.YCord]
                        , SelectedVillage.Cordinates.X
                        , SelectedVillage.Cordinates.Y, amount, amount, true);
                       
                    if (result == TransportResult.Success)
                    {
                        neededAmount -= amount;
                    }

                }
                else
                {
                    if (neededAmount > 0)
                    {
                        TransportResult result = Fbg.Bll.CoinTransport.Transport(Owner
                        , (int)NearestVillages[i][CONSTS.NearestVillagesColumnIndex.VillageID]
                        , SelectedVillage.id
                         , (int)NearestVillages[i][CONSTS.NearestVillagesColumnIndex.XCord]
                        , (int)NearestVillages[i][CONSTS.NearestVillagesColumnIndex.YCord]
                        , SelectedVillage.Cordinates.X
                        , SelectedVillage.Cordinates.Y, neededAmount, neededAmount, true);
                       
                        if (result == TransportResult.Success)
                        {
                            neededAmount = 0;
                        }
                    }
                }

            }

        }
        public static int GetMaxCoins(DataView NearestVillages, Village SelectedVillage)
        {
            int treasurySize  = SelectedVillage.TreasurySize;
            int neededAmount = treasurySize - SelectedVillage.coins;//this hold the amount needed by selectd village
            int maxCoins = 0;
            for (int i = 0; i < NearestVillages.Count; i++)
            {
                int amount = (int)NearestVillages[i][CONSTS.NearestVillagesColumnIndex.MinAmount ];
                if (neededAmount  - amount > 0)
                {
                    maxCoins +=  amount;
                    neededAmount -= amount;
                }
                else
                {
                    if (treasurySize > 0)
                    {
                        maxCoins += neededAmount;
                        neededAmount = 0;

                    }
                }

            }
            return maxCoins;

        }
        /// <summary>
        /// this function return a dataset have a Yes Villages and No Villages
        /// </summary>
        /// <param name="Owner"></param>
        /// <returns> Returns a tables who's described by CoinTransport.CONSTS.NoQTransportCoinsTableIndex </returns>
        public static DataSet  GetNoTransportCoinsVillages(Player Owner)
        {
          return DAL.CoinTransports.GetNoQTransportCoinsVillages(Owner.Realm.ConnectionStr, Owner.ID);

        }
        public static void AddToNoTransportsCoinsList(Player Owner,int VillageID)
        {

            DAL.CoinTransports.AddToNoTransportsCoinsList(Owner.Realm.ConnectionStr, Owner.ID, VillageID);

        }
        public static void DeleteFromNoTransportsCoinsList(Player Owner,int VillageID)
        {
            DAL.CoinTransports.DeleteFromNoTransportsCoinsList(Owner.Realm.ConnectionStr, Owner.ID , VillageID);

        }
        public static DataSet GetVillagesToSendSilverFrom(Player Player, int ReceiverVillageID)
        {
            if (Player == null)
            {
                throw new ArgumentNullException("Player is null");
            }
            if (ReceiverVillageID == 0)
            {
                throw new ArgumentNullException("ReceiverVillageID is 0");
            }
            return DAL.CoinTransports.GetVillagesToSendSilverFrom(Player.Realm.ConnectionStr, ReceiverVillageID, Player.ID);

        }

    }


}
