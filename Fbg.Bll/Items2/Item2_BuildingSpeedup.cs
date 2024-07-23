using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fbg.Bll.Items2
{
    public class Item2_BuildingSpeedup : Item2
    {
        public int AmountOfMinutesToSpeedUp { get; private set; }
        public TimeSpan AmountOfTimeToSpeedUp { get; private set; }

        public Item2_BuildingSpeedup(long id, Fbg.Bll.User user, int playerID, DateTime? expiresOn, int amount)
            : base(id, user, playerID, expiresOn)
        {
            AmountOfMinutesToSpeedUp = amount;
            AmountOfTimeToSpeedUp = new TimeSpan(0, AmountOfMinutesToSpeedUp, 0);
        }

        override  public string Type
        {
            get
            {
                return "buildingspeedup";
            }
        }
        
        override public string Text
        {
            get
            {
                return String.Format("{0} building speedup", Fbg.Bll.utils.FormatDuration_Long2(AmountOfTimeToSpeedUp));
            }
        }

        override public bool Use(Fbg.Bll.Player player, int villageID)
        {
            Village v = player.Village(villageID);
            if (v != null)
            {
                List<UpgradeEvent> list = v.GetAllUpgradingBuildings();

                if (list.Count > 0 && list[0] is UpgradeEvent_CurrentlyUpgrading)
                {
                    UpgradeEvent_CurrentlyUpgrading upgradeEvent = (UpgradeEvent_CurrentlyUpgrading)list[0];
                    if (base.isItemAvailable())
                    {
                        if (v.SpeedUpUpgrade(upgradeEvent.eventID, this))
                        {
                            player.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.Item2UsageLog, "Item2_BuildingSpeedup", String.Format("ItemID:{0} used on VID:{1}, BuildingID:{2}, upgrading to level:{3}, completion time before speedup:{4},  speeding up {5} minutes"
                                , this.ID, villageID, upgradeEvent.upgradeToLevel.Building.ID, upgradeEvent.upgradeToLevel.Level, upgradeEvent.completionTime, AmountOfMinutesToSpeedUp));
                            base.MarkItemAsUsed();
                            return true;
                        }                       
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
                Item2_BuildingSpeedup itemOfThisType =  (Item2_BuildingSpeedup ) item;
                return itemOfThisType.AmountOfTimeToSpeedUp == this.AmountOfTimeToSpeedUp;
            }
            return false;
        }

    }
}
