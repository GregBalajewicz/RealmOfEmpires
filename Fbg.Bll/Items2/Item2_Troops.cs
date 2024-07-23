using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fbg.Bll.Items2
{
    public class Item2_Troops : Item2
    {
        public UnitType UnitType { get; private set; }
        private int unitTypeID;
        public int Amount { get; private set; }

        public Item2_Troops(long id, Fbg.Bll.User user, int playerID, DateTime? expiresOn, int unitTypeID, int amount) : base(id, user,playerID,expiresOn)
        {
            this.unitTypeID = unitTypeID;
            Amount = amount;
        }

        override public string Type
        {
            get
            {
                return "troops";
            }
        }
        //override public string IconURL
        //{
        //    get
        //    {
        //        return UnitType.IconUrl_ThemeM;
        //    }
        //}
        override public string Text
        {
            get
            {
                return String.Format("{0} {1}", utils.FormatCost(Amount), UnitType.Name);
            }
        }

        override public bool Use(Fbg.Bll.Player player, int villageID)
        {
            Village v = player.Village(villageID);

            if (v != null)
            {
                if (v.RemainingPopulation >= this.UnitType.Pop * Amount)
                {
                    if (base.isItemAvailable())
                    {
                        v.AddUnitsToVillage(UnitType, Amount);
                        //v.CommandUnits()
                        player.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.Item2UsageLog, "Item2_Troops", String.Format("ItemID:{0} used on VID:{1}. given {2} of {3}", this.ID, villageID, Amount, UnitType.Name));
                        base.MarkItemAsUsed();
                        return true;
                    }
                }
            }
            return false;
        }

        override internal bool isCompatibleWithThisPlayer(Player player)
        {
            UnitType ut = player.Realm.GetUnitTypesByID(unitTypeID);
            if (ut != null)
            {
                UnitType = ut;
                return true;
            }

            return false;
        }



        override public bool specialEquals(Item2 item)
        {
            if (item.GetType() == this.GetType())
            {
                Item2_Troops itemOfThisType = (Item2_Troops)item;
                return itemOfThisType.unitTypeID == this.unitTypeID && itemOfThisType.Amount == this.Amount;
            }
            return false;
        }
    }
}
