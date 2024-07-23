using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fbg.Bll.Items2
{
    public class Item2_Silver : Item2
    {
        public int Amount { get; private set; }

        public Item2_Silver(long id, Fbg.Bll.User user, int playerID, DateTime? expiresOn, int amount)
            : base(id, user, playerID, expiresOn)
        {
            Amount = amount;
        }

        override  public string Type
        {
            get
            {
                return "silver";
            }
        }

        //override public string IconURL
        //{
        //    get
        //    {
        //        return "https://static.realmofempires.com/images/gifts/d2_Gift_sack_of_silver.png";
        //    }
        //}
        override public string Text
        {
            get
            {
                return String.Format("{0} Silver", utils.FormatCost(Amount));
            }
        }

        override public bool Use(Fbg.Bll.Player player, int villageID)
        {
            Village v = player.Village(villageID);
            if (v != null)
            {
                int treasurySize = v.TreasurySize;
                if (treasurySize - v.coins >= this.Amount)
                {
                    if (base.isItemAvailable())
                    {
                        v.UpdateCoins(Amount);
                        player.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.Item2UsageLog, "Item2_Silver", String.Format("ItemID:{0} used on VID:{1}. given {2} silver", this.ID, villageID, Amount));
                        base.MarkItemAsUsed();
                        return true;
                    }
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
                Item2_Silver itemOfThisType = (Item2_Silver)item;
                return itemOfThisType.Amount == this.Amount ;
            }
            return false;
        }
    }
}
