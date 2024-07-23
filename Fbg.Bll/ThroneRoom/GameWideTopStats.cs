using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fbg.Bll.ThroneRoom
{
    public class GameWideTopStats
    {
        public class CONSTS
        {
            public class Stats
            {
                internal static int maxStatsID = 5;
                public enum IDs
                {
                    HighestNumOfVillages = 1,
                    VillagePoints = 2,
                    PointsAsAttacker = 3,
                    PointsAsDefender = 4,
                    GovKilledAsDefender = 5
                }
            }
        }

        public TopStat HighestNumOfVillages
        {
            get
            {
                return _stats[(int)CONSTS.Stats.IDs.HighestNumOfVillages];
            }
        }
        public TopStat VillagePoints
        {
            get
            {
                return _stats[(int)CONSTS.Stats.IDs.VillagePoints];
            }
        }
        public TopStat PointsAsAttacker
        {
            get
            {
                return _stats[(int)CONSTS.Stats.IDs.PointsAsAttacker];
            }
        }
        public TopStat PointsAsDefender
        {
            get
            {
                return _stats[(int)CONSTS.Stats.IDs.PointsAsDefender];
            }
        }
        public TopStat GovKilledAsDefender
        {
            get
            {
                return _stats[(int)CONSTS.Stats.IDs.GovKilledAsDefender];
            }
        }



        private TopStat[] _stats;

        public GameWideTopStats() {
            _stats = new TopStat[CONSTS.Stats.maxStatsID + 1];
        }

        internal void UpdateIfBetter(GameWideTopStats.CONSTS.Stats.IDs statID, int statValue, int pID)
        {
            if ((int)_stats[(int)statID].Value < (int)statValue) {
                _stats[(int)statID].Value = statValue;
                _stats[(int)statID].AchievedByPlayerID = pID;
            }
        }


        public struct  TopStat
        {
            public int Value { get; internal set; }
            public int AchievedByPlayerID { get; internal set; }
        }
    }
}
