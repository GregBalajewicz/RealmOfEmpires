using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fbg.Common.Entities
{
    public class UnitType
    {
        public int ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int Pop
        {
            get;
            set;
        }


        /// <summary>
        /// Fields per hour
        /// </summary>
        public int Speed
        {
            get;
            set;
        }

        /// <summary>
        /// How much silver (plunder) the unit can carry
        /// </summary>
        public int CarryCapacity
        {
            get;
            set;
        }

        public int AttackStrength
        {
            get;
            set;
        }

        public int AvgDefenseStrength
        {
            get;
            set;
        }

        public string IconUrl
        {
            get;
            set;
        }
        public string LargeIconUrl
        {
            get;
            set;
        }
        public string IconUrl_ThemeM
        {
            get;
            set;
        }

        public string Image
        {
            get;
            set;
        }
        public string LargeImage
        {
            get;
            set;
        }
        public int SpyAbility
        {
            get;
            set;
        }
        public int CounterSpyAbility
        {
            get;
            set;
        }

        public List<int> AttackableBuildingIDs
        {
            get;
            set;
        }


        public string description
        {
            get;
            set;
        }
        public Dictionary<string, int> DefenceStrengths
        {
            get;
            set;
        }

        public TimeSpan baseRecruitSpeed
        {
            get;
            set;
        }
        public int costInCoins
        {
            get;
            set;
        }
    }

}
