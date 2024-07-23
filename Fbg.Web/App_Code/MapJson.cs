using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Script.Serialization;
using Fbg.Bll;
using Gmbc.Common.Diagnostics.ExceptionManagement;

/// <summary>
/// Summary description for MapJson
/// </summary>
public class MapJson
{

    public class CONSTS
    {

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
            public static int Avatar = 15;
            public static int VillageTypeID = 16;
            public static int claimedBy = 17;
            public static int claimedBy_otherclan = 18;
        }
        public class GetMapInfoTables
        {
            /// <summary>
            /// columns defined  by VillagesColIndex
            /// </summary>
            public static int regularMap = 0;
            /// <summary>
            /// Columns defined by LandMarkColIndex
            /// </summary>
            public static int landmarks = 1;
            public static int tags = 2;
            public static int landtypes = 3;
        }
        public class LandMarkColIndex
        {
            public static int XCord = 0;
            public static int YCord = 1;
            public static int ImageURL = 2;
        }
    }

    private Player _player;
    private Clan _clan;
    public MapJson(DataSet dsMap, Player fbgPlayer)
    {
        try
        {
            _player = fbgPlayer;
            _clan = fbgPlayer.Clan;

            var dtLandmarks = dsMap.Tables[CONSTS.GetMapInfoTables.landmarks];

            land = dtLandmarks.AsEnumerable().ToDictionary(
                    r => r[CONSTS.LandMarkColIndex.XCord] + "_" + r[CONSTS.LandMarkColIndex.YCord],
                    r => new
                    {
                        x = r[CONSTS.LandMarkColIndex.XCord],
                        y = r[CONSTS.LandMarkColIndex.YCord],
                        image = r[CONSTS.LandMarkColIndex.ImageURL]
                    }
                );

            var dtMap = dsMap.Tables[CONSTS.GetMapInfoTables.regularMap];
            var dtTags = dsMap.Tables[CONSTS.GetMapInfoTables.tags];

            villages =
                dtMap.AsEnumerable().ToDictionary(
                    r => r[CONSTS.VillagesColIndex.XCord] + "_" + r[CONSTS.VillagesColIndex.YCord],
                    r => new
                    {
                        x = (int)r[CONSTS.VillagesColIndex.XCord],
                        y = (int)r[CONSTS.VillagesColIndex.YCord],
                        name = (string)r[CONSTS.VillagesColIndex.VillageName],
                        id = (int)r[CONSTS.VillagesColIndex.VillageID],
                        points = (int)r[CONSTS.VillagesColIndex.villagepoints],
                        pid = (int)r[CONSTS.VillagesColIndex.PlayerID],
                        type = (short)r[CONSTS.VillagesColIndex.VillageTypeID],
                        note = (string)r[CONSTS.VillagesColIndex.VillageNote],
                        friend = r[CONSTS.VillagesColIndex.FriendPlayer].ToString() == "0" ? "0" : "1",
                        tags = dtTags.AsEnumerable().Where(t => t.Field<int>("VillageID") == (int)r[CONSTS.VillagesColIndex.VillageID]).Select(t => t.Field<int>("TagID").ToString()).Aggregate("", (c, s) => c + s + ",").TrimEnd(','),
                        area = Fbg.Common.Area.GetAreaNumberFromCords((int)r[CONSTS.VillagesColIndex.XCord], (int)r[CONSTS.VillagesColIndex.YCord]),
                        flag = VillageFlag(r),
                        claimedStatus = r[CONSTS.VillagesColIndex.claimedBy] is DBNull ? 0
                            : ((int)r[CONSTS.VillagesColIndex.claimedBy] == fbgPlayer.ID ? 1 : 2),
                        claimedStatus_otherClan = r[CONSTS.VillagesColIndex.claimedBy_otherclan] is DBNull ? 0 : 1
                    }
                );

            players =
                dtMap.AsEnumerable().GroupBy(m => m.Field<int>(CONSTS.VillagesColIndex.PlayerID)).ToDictionary(
                    r => r.Key.ToString(),
                    r => new
                    {
                        id = r.First()[CONSTS.VillagesColIndex.PlayerID],
                        PN = r.First()[CONSTS.VillagesColIndex.PlayerName].ToString(),
                        CID = r.First()[CONSTS.VillagesColIndex.ClanID],
                        Pe = r.First()[CONSTS.VillagesColIndex.PlayerNote].ToString(),
                        PF = r.First()[CONSTS.VillagesColIndex.FriendPlayer].ToString() == "0" ? "0" : "1",
                        PP = Utils.FormatCost((int)r.First()[CONSTS.VillagesColIndex.PlayerPoints]),
                        Av = r.First()["AvatarID"].ToString()
                    }
                );

            clans =
                dtMap.AsEnumerable().Where(m => !(m[CONSTS.VillagesColIndex.ClanID] is DBNull)).GroupBy(m => m.Field<int>(CONSTS.VillagesColIndex.ClanID)).ToDictionary(
                    r => r.Key.ToString(),
                    r => new
                    {
                        id = r.Key.ToString(),
                        CN = r.First()[CONSTS.VillagesColIndex.ClanName].ToString(),
                        CP = ClanPoints(r.Key)
                    }
                );

            if (dsMap.Tables.Contains("Table3"))
            {
                var dtLandTypes = dsMap.Tables[CONSTS.GetMapInfoTables.landtypes];

                landtypes = dtLandTypes.AsEnumerable().ToDictionary(
                        r => r[1].ToString(),
                        r => new
                        {
                            idt = r[0],
                            idtp = r[1],
                            image = r[2]
                        }
                    );
            }
        }
        catch (Exception e)
        {
            BaseApplicationException bex = new BaseApplicationException("error in MapJson", e);
            bex.AddAdditionalInformation("dsMap", dsMap);
            throw bex;
        }
    }

    public string ClanPoints(int clanId) {
        var dr = Fbg.Bll.Stats.GetOneClanRanking(_player.Realm, clanId);

        return Utils.FormatCost((int)(dr == null ? 0 : dr[Fbg.Bll.Stats.CONSTS.ClanRanking.TotalPoints]));
    }

    public string VillageFlag(DataRow r){
        string flagImg = null;
        if (((int)r[CONSTS.VillagesColIndex.PlayerID]) == _player.ID)
        {
            // this village is my village                     
            flagImg = "fme.png"; //"Barracks.png"; 
        }
        else if (((int)r[CONSTS.VillagesColIndex.PlayerID]) == Fbg.Bll.CONSTS.SpecialPlayers.Abandoned_PlayerId(_player.Realm))
        {
            // this village is 'abandoned'
            flagImg = "fabd.png";
        }
        else if (((int)r[CONSTS.VillagesColIndex.PlayerID]) == Fbg.Bll.CONSTS.SpecialPlayers.Rebels_PlayerId(_player.Realm))
        {
            // this village is 'abandoned'
            flagImg = "freb.png";
        }
        else if (!(r[CONSTS.VillagesColIndex.ClanID] is DBNull))
        {
            if (_clan != null && ((int)r[CONSTS.VillagesColIndex.ClanID]) == _clan.ID)
            {
                // This village is part of the same clan as mine
                flagImg = "fc.png";
            }
            else if ((r[CONSTS.VillagesColIndex.StatusID]) is DBNull)
            {
                // my clan has no diplomatic relation with this village's clan
                flagImg = null;
            }
            else if (((int)r[CONSTS.VillagesColIndex.StatusID]) == (int)Fbg.Bll.Clan.Diplomacy.Ally)
            {
                // this is y ally
                flagImg = "fa.png";
            }
            else if (((int)r[CONSTS.VillagesColIndex.StatusID]) == (int)Fbg.Bll.Clan.Diplomacy.Enemy)
            {
                // this is y enemy
                flagImg = "fe.png";
            }
            else if (((int)r[CONSTS.VillagesColIndex.StatusID]) == (int)Fbg.Bll.Clan.Diplomacy.NAP)
            {
                // this is my NAP
                flagImg = "fn.png";
            }
        }

        return flagImg;
    }

    public Object land { get; set; }
    public Object landtypes { get; set; }
    public Object villages { get; set; }
    public Object players { get; set; }
    public Object clans { get; set; }
}