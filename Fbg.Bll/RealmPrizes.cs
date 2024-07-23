using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fbg.Bll.Items2;

namespace Fbg.Bll
{
    /// <summary>
    /// TODO : this was never finished, in favor of hardcoding 
    /// </summary>
    class RealmPrizes
    {
        
        public abstract class Placement { }

        public class VillageCountPlacement
        {
            public int fromRank { protected set; get; }
            public int toRank { protected set; get; }
        }

        public abstract class Prize { }
        public class Item2Prize {
            Item2 item;
        }

        class RealmPrize
        {
            public Placement place { protected set; get; }

        }
    }
}
