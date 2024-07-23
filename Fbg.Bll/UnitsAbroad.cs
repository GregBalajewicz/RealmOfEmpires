using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Fbg.Common;
namespace Fbg.Bll
{

    /// <summary>
    /// TODO: this is currently terribly inneficient since it brings back much duplicated data such as SupportingPlayerName, SupportedVillageName etc
    ///     This should be optimized some day. 
    /// </summary>
    public class UnitsAbroad
    {
        public class CONSTS
        {
            public class ColIndex
            {
                public static int SupportedVillageID = 0;
                public static int SupportingVillageID = 1;
                public static int UnitTypeID = 2;
                public static int UnitCount = 3;
                public static int SupportedVillageName = 4;
                public static int SupportedVillageXCord = 5;
                public static int SupportedVillageYCord = 6;
                public static int SupportingVillageName = 7;
                public static int SupportingVillageXCord = 8;
                public static int SupportingVillageYCord = 9;
                public static int SupportedPlayerName = 10;
                public static int SupportedPlayerID = 11;
            }
        }
      
        DataTable _dtData;
        public UnitsAbroad(DataTable data)
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

        public static void RecallUnits(Player supportingPlayer, int supportingVillageID, int supportedVillageID, UnitCommand command)
        {
            DAL.Units.RecallUnits(supportingPlayer.Realm.ConnectionStr, supportingPlayer.ID
                , supportingVillageID, supportedVillageID,command );
        }


        public class UnitWithCount
        {
            public int unitID;
            public int count;
        }

        public List<UnitWithCount> GetUnitsSupporting(int supportingVillageID, int supportedVillageID)
        {
            List<UnitWithCount> list = new List<UnitWithCount>();
            foreach (DataRow row in _dtData.Rows)
            {
                if ((int)row[CONSTS.ColIndex.SupportingVillageID] == supportingVillageID
                    && (int)row[CONSTS.ColIndex.SupportedVillageID] == supportedVillageID)
                {
                    list.Add(new UnitWithCount() { unitID = (int)row[CONSTS.ColIndex.UnitTypeID]
                    , count = (int)row[CONSTS.ColIndex.UnitCount]});
                }
            }

            return list;
        }

        //public static void RecallUnits(Player supportingPlayer, int supportingVillageID, int supportedVillageID)
        //{
        //    DAL.Units.RecallUnits(supportingPlayer.Realm.ConnectionStr, supportingPlayer.ID
        //        , supportingVillageID, supportedVillageID);
        //}
    }
}
