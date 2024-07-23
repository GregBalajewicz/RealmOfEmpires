using Gmbc.Common.Diagnostics.ExceptionManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fbg.Bll.ThroneRoom
{
    public abstract class PlayerRealmInfo
    {
        public int PlayerID { get; private set; }
        public Realm Realm { get; private set; }
        public string Playername { get; private set; }
        public Title highestTitleAchieved { get; protected set; }
        public DateTime registeredOn { get; protected set; }
        public int HighestNumOfVillages { get; protected set; }
        public int HighestNumOfVillages_rank { get; protected set; }
        public int HighestVillagePoints { get; protected set; }
        public int HighestVillagePoints_rank { get; protected set; }
        public int PointsAsAttacker { get; protected set; }
        public int PointsAsAttacker_rank { get; protected set; }
        public int PointsAsDefender { get; protected set; }
        public int PointsAsDefender_rank { get; protected set; }
        public int GovKilledAsDefender { get; protected set; }
        public int GovKilledAsDefender_rank { get; protected set; }



        public PlayerRealmInfo(int pID, string name, Realm r, DataTable dtPlayerInfo, DataTable dtPlayerStats)
        {
            PlayerID = pID;
            Playername = name;
            Realm = r;


            try
            {
                highestTitleAchieved = r.TitleByID((int)dtPlayerInfo.Rows[0][0]);
                registeredOn = (DateTime)dtPlayerInfo.Rows[0][1];

                dtPlayerStats.PrimaryKey = new DataColumn[] { dtPlayerStats.Columns[0] };

                HighestNumOfVillages = setStat(dtPlayerStats, 1, 0);
                HighestVillagePoints = setStat(dtPlayerStats, 2, 0);
                PointsAsAttacker = setStat(dtPlayerStats, 3, 0);
                PointsAsDefender = setStat(dtPlayerStats, 4, 0);
                GovKilledAsDefender = setStat(dtPlayerStats, 5, 0);

                DataTable dt;
                dt = Fbg.Bll.Stats.GetThroneRoomPlayerRanking(r, Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking_SortExp.HighestNumOfVillages);
                HighestNumOfVillages_rank = dt.Rows.IndexOf(dt.Rows.Find(PlayerID)) + 1;

                dt = Fbg.Bll.Stats.GetThroneRoomPlayerRanking(r, Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking_SortExp.GovKilledAsDefender);
                GovKilledAsDefender_rank = dt.Rows.IndexOf(dt.Rows.Find(PlayerID)) + 1;

                dt = Fbg.Bll.Stats.GetThroneRoomPlayerRanking(r, Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking_SortExp.HighestVillagePoints);
                HighestVillagePoints_rank = dt.Rows.IndexOf(dt.Rows.Find(PlayerID)) + 1;

                dt = Fbg.Bll.Stats.GetThroneRoomPlayerRanking(r, Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking_SortExp.PointsAsAttacker);
                PointsAsAttacker_rank = dt.Rows.IndexOf(dt.Rows.Find(PlayerID)) + 1;

                dt = Fbg.Bll.Stats.GetThroneRoomPlayerRanking(r, Fbg.Bll.Stats.CONSTS.ThroneRoomPlayerRanking_SortExp.PointsAsDefender);
                PointsAsDefender_rank = dt.Rows.IndexOf(dt.Rows.Find(PlayerID)) + 1;
            }
            catch (Exception e)
            {
                BaseApplicationException bex = new BaseApplicationException("failed getting player info", e);
                bex.AddAdditionalInformation("dtPlayerStats", dtPlayerStats);
                bex.AddAdditionalInformation("pID", pID);
                throw bex;
            }
        }

        private int setStat(DataTable dtPlayerStats, int statID, int defaultValue)
        {
            DataRow drStat;
            drStat = dtPlayerStats.Rows.Find(statID);
            if (drStat != null)
            {
                return (int)drStat[1];
            }
            else
            {
                return defaultValue;
            }
        }
    }


    public abstract class MyPlayerRealmInfo : PlayerRealmInfo
    {
        public ThroneRoom.PlayerInfoDisplayStatus displayStatus { get; protected set; }

        public MyPlayerRealmInfo(int pID, string name, Realm r, DataTable dtPlayerInfo, DataTable dtPlayerStats, ThroneRoom.PlayerInfoDisplayStatus displayStatus)
            : base(pID, name, r, dtPlayerInfo, dtPlayerStats)
        {
            this.displayStatus = displayStatus;
        }
    }

    public abstract class SomeElsesPlayerRealmInfo : PlayerRealmInfo
    {

        public SomeElsesPlayerRealmInfo(int pID, string name, Realm r, DataTable dtPlayerInfo, DataTable dtPlayerStats)
            : base(pID, name, r, dtPlayerInfo, dtPlayerStats)
        {
            
        }

    }


    public class SomeElsesInactivePlayerInfo : SomeElsesPlayerRealmInfo
    {
        public SomeElsesInactivePlayerInfo(int pID, string name, Realm r, DataSet dsInfo)
            : base(pID, name, r, dsInfo.Tables[0], dsInfo.Tables[1])
        {
            
        }
    }


    public class SomeElsesActivePlayerInfo : SomeElsesPlayerRealmInfo
    {
        public SomeElsesActivePlayerInfo(int pID, string name, Realm r, DataSet dsInfo)
            : base(pID, name, r, dsInfo.Tables[0], dsInfo.Tables[1])
        {
            
        }

    }


    public class MyInactivePlayerInfo : MyPlayerRealmInfo
    {
        public MyInactivePlayerInfo(int pID, string name, Realm r, DataSet dsInfo, ThroneRoom.PlayerInfoDisplayStatus displayStatus)
            : base(pID, name, r, dsInfo.Tables[0], dsInfo.Tables[1], displayStatus)
        {

        }
    }


    public class MyActivePlayerInfo : MyPlayerRealmInfo
    {
        public MyActivePlayerInfo(int pID, string name, Realm r, DataSet dsInfo, ThroneRoom.PlayerInfoDisplayStatus displayStatus)
            : base(pID, name, r, dsInfo.Tables[0], dsInfo.Tables[1], displayStatus)
        {

        }

    }

}
