using System;
using System.Collections.Generic;
using System.Text;
using System.Data;


namespace Fbg.Bll
{
    public class UnitsSupporting
    {
        public class CONSTS
        {
            public class ColIndex
            {
                public static int UnitTypeID = 0;
                public static int UnitCount = 1;
                public static int SupportedVillageID = 2;
                public static int SupportingVillageID = 3;
                public static int SupportedVillageName = 4;
                public static int SupportedVillageXCord = 5;
                public static int SupportedVillageYCord = 6;
                public static int SupportingVillageName = 7;
                public static int SupportingVillageXCord = 8;
                public static int SupportingVillageYCord = 9;
                public static int SupportingPlayerName = 10;
                public static int SupportingPlayerID = 11;
            }
        }

        DataTable _dtData;
        private UnitsSupporting(DataTable data)
        {
            _dtData = data;
        }

        public DataTable Data
        {
            get
            {
                return _dtData;
            }
        }

        /// <summary>
        /// Get the supporting troops either at all of the player's villages (if -1 is passed for villageID ) 
        /// or at a specific village if villageID is specified
        /// </summary>
        /// <param name="realm"></param>
        /// <param name="player"></param>
        /// <param name="villageID"></param>
        /// <returns></returns>
        public static UnitsSupporting GetSupportingTroops(Realm realm, Player player, int villageID)
        {
            if (realm == null)
            {
                throw new ArgumentNullException("realm");
            }
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            DataTable dt;

            /// TODO: this is currently terribly inneficient since it brings back much duplicated data such as SupportingPlayerName, SupportedVillageName etc
            ///     This should be optimized some day. 
            dt =  DAL.Units.GetSupportingTroops(realm.ConnectionStr, player.ID, villageID);

            return new UnitsSupporting(dt);
        }

        public static void SendBack(Player CurrentlyLoggedInPlayer, int supportingVillageID, int supportedVillageID)
        {
            DAL.Units.SendBackSupport(CurrentlyLoggedInPlayer.Realm.ConnectionStr, CurrentlyLoggedInPlayer.ID, supportingVillageID, supportedVillageID);
        }
    }
}
