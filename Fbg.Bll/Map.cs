using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Linq;

namespace Fbg.Bll
{
    public class Map
    {

        public class CONSTS
        {
            public class GetMapInfoTables
            {
                /// <summary>
                /// columns defined  by VillagesColIndex
                /// </summary>
                public static int regularMap= 0;
                /// <summary>
                /// Columns defined by LandMarkColIndex
                /// </summary>
                public static int landmarks= 1;
                public static int tags = 2;
                public static int landtypes = 3;
            }
            public class GetOverviewMapInfoTables
            {
                /// <summary>
                /// columns defined by OverviewMapColIndex
                /// </summary>
                public static int overviewMap = 0;
            }

            public class VillagesColIndex
            {
                public static int VillageID = 0;
                //public static int OwnerPlayerID = 1;
                public static int VillageName = 1;
                public static int XCord = 2;
                public static int YCord = 3;
                public static int villagepoints = 4;
                public static int PlayerName = 5;
                public static int PlayerID = 6;
                public static int ClanID = 7;
                public static int ClanName = 8;
                public static int StatusID = 9;
                public static int PlayerNote = 10;
                public static int VillageNote = 11;
                public static int FriendPlayer = 12;
                public static int SleepModeActiveFrom = 13;
                public static int PlayerPoints = 14;
                public static int VillageTypeID = 15;
            }

            public class OverviewMapColIndex
            {
                public static int OwnerPlayerID = 0;
                public static int XCord = 1;
                public static int YCord = 2;
                public static int ClanID = 3;
                public static int StatusID = 4;
                public static int FriendPlayerID = 5;
            }

            public class LandMarkColIndex
            {
                public static int XCord = 0;
                public static int YCord = 1;
                public static int ImageURL = 2;
            }
        }

        public enum MovementType { Incoming = 1, Outgoing = 2 }

        public class Movement
        {
            public int VillageID { get; set; }
            public int SupportCount { get; set; }
            public int AttackCount { get; set; }
        }

        /// <summary>
        /// returns the map
        /// The returned dataset is defined by Map.CONSTS.GetMapInfoTables
        /// </summary>
        public static DataSet GetMapInfo(Realm realm, int bottomLeftX, int bottomLeftY, int mapSize, int MyClanID, int PlayerID, bool hasPF)
        {
            return GetMapInfo(realm, bottomLeftX, bottomLeftY, mapSize, mapSize, MyClanID, PlayerID, hasPF);
        }

        public static DataSet GetMapInfo(Realm realm, int bottomLeftX, int bottomLeftY, int mapSizeX, int mapSizeY, int MyClanID, int PlayerID, bool hasPF)
        {
            return DAL.Map.GetMapInfo(realm.ConnectionStr, bottomLeftX, bottomLeftY, mapSizeX, mapSizeY, MyClanID, PlayerID, hasPF);
        }

        public class MapRectangle
        {
            public int lux = 0, luy = 0, rdx = 0, rdy = 0;
            public int blx = 0, bly = 0, msx = 0, msy = 0;

            public MapRectangle Find(string coords)
            {
                if (String.IsNullOrEmpty(coords)) return this;

                var cc = coords.Split(',').Select(i =>
                    new { x = Convert.ToInt32(i.Split('_')[0]), y = Convert.ToInt32(i.Split('_')[1]) }).ToArray();

                lux = rdx = cc[0].x; luy = rdy = cc[0].y;

                for(int i = 0; i < cc.Length; i++) {
                    if (cc[i].x > lux) { lux = cc[i].x; }
                    if (cc[i].y > luy) { luy = cc[i].y; }
                    if (cc[i].x < rdx) { rdx = cc[i].x; }
                    if (cc[i].y < rdy) { rdy = cc[i].y; }
                }

                blx = rdx;
                bly = rdy;

                msx = (rdx > lux ? rdx - lux : lux - rdx) + 6;
                msy = (rdy > luy ? rdy - luy : luy - rdy) + 6;

                return this;
            }
        }

        public static DataSet GetMapInfo(Realm realm, 
            string MapCoords, string LandCoords,
            int MyClanID, int PlayerID, bool hasPF, bool hasAllLandmarkTypes)
        {
            var mc = new MapRectangle().Find(MapCoords);
            var lc = new MapRectangle().Find(LandCoords);

            return DAL.Map.GetMapBySquares(realm.ConnectionStr,
                mc.blx, mc.bly, mc.msx, mc.msy,
                lc.blx, lc.bly, lc.msx, lc.msy, MyClanID, PlayerID, hasPF, hasAllLandmarkTypes);
        }
        
        public static Movement[] GetMapTroopsMove(Realm realm, MovementType type, int bottomLeftX, int bottomLeftY, int mapSize, int PlayerID)
        {
            
            var rows = type == MovementType.Incoming ?
                DAL.Map.GetMapTroopsMoveIncoming(realm.ConnectionStr, bottomLeftX, bottomLeftY, mapSize, PlayerID)
                            .Tables[0].Rows
                            : 
                DAL.Map.GetMapTroopsMoveOutgoing(realm.ConnectionStr, bottomLeftX, bottomLeftY, mapSize, PlayerID)
                            .Tables[0].Rows            
                            ;

            Movement[] mv = new Movement[rows.Count];

            for(int i = 0; i < rows.Count; i++){
                mv[i] = new Movement();
                mv[i].VillageID = (int)rows[i].ItemArray[0];
                mv[i].AttackCount = (int)rows[i].ItemArray[2];
                mv[i].SupportCount = (int)rows[i].ItemArray[1];
            }

            return mv;
        }

        /// <summary>
        /// returns the overview map
        /// The returned dataset is defined by Map.CONSTS.GetOverviewMapInfoTables
        /// </summary>
        public static DataSet GetOverviewMapInfo(Realm realm, int bottomLeftX_ov, int bottomLeftY_ov, int overviewMapSize, int MyClanID, int PlayerID)
        {
            return DAL.Map.GetOverviewMapInfo(realm.ConnectionStr, bottomLeftX_ov, bottomLeftY_ov, overviewMapSize, MyClanID, PlayerID);
        }

     }
}
