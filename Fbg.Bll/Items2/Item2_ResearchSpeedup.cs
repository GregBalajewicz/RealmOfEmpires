using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fbg.Bll.Items2
{
    public class Item2_ResearchSpeedup : Item2
    {
        public int AmountOfMinutesToSpeedUp { get; private set; }
        public TimeSpan AmountOfTimeToSpeedUp { get; private set; }

        public Item2_ResearchSpeedup(long id, Fbg.Bll.User user, int playerID, DateTime? expiresOn, int amount)
            : base(id, user, playerID, expiresOn)
        {
            AmountOfMinutesToSpeedUp = amount;
            AmountOfTimeToSpeedUp = new TimeSpan(0, AmountOfMinutesToSpeedUp, 0);
        }

        override  public string Type
        {
            get
            {
                return "researchspeedup";
            }
        }
        
        override public string Text
        {
            get
            {
                return String.Format("{0} research speedup", Fbg.Bll.utils.FormatDuration_Long2(AmountOfTimeToSpeedUp));
            }
        }
        override public bool Use(Fbg.Bll.Player player, int vid)
        {
        
            if (player.MyResearch.ResearchInProgress.Count > 0)
            {
                DateTime eventTimeBeforeSpeedUp;
                int researchItemSpeedUp = player.MyResearch.SpeedUp(this, out eventTimeBeforeSpeedUp);
                if (researchItemSpeedUp != 0)
                {
                    player.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.Item2UsageLog, "Item2_ResearchSpeedup", String.Format("ItemID:{0} used to speed up research ID:{1}, completion time before speedup:{2},  speeding up {3} minutes"
                        , this.ID, researchItemSpeedUp, eventTimeBeforeSpeedUp, AmountOfMinutesToSpeedUp));
                    base.MarkItemAsUsed();
                    return true;
                }
            }
            

            return false;
        }

        override internal bool isCompatibleWithThisPlayer(Player player)
        {

            return true;

        }


        override public bool specialEquals(Item2 item)
        {
            if (item.GetType() == this.GetType())
            {
                Item2_ResearchSpeedup itemOfThisType = (Item2_ResearchSpeedup)item;
                return itemOfThisType.AmountOfTimeToSpeedUp == this.AmountOfTimeToSpeedUp;
            }
            return false;
        }
    }
}
