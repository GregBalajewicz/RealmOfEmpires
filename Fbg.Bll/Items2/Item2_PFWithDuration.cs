using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fbg.Bll.Items2
{
    public class Item2_PFWithDuration : Item2
    {
        public int PFPackageID
        {
            get
            {
                return _pfPackageID;
            }
        }
        private  int _pfPackageID;
       // public Fbg.Bll.CONSTS.PFs PF {get; private set;}
        public TimeSpan Duration { get; private set; }

        public Item2_PFWithDuration(long id, Fbg.Bll.User user, int playerID, DateTime? expiresOn, int pfPackageID, TimeSpan duration)
            : base(id, user, playerID, expiresOn)
        {
            _pfPackageID = pfPackageID;
            Duration = duration;
        }

        override public string Type
        {
            get
            {
                return "pfd";
            }
        }
        //override public string IconURL
        //{
        //    get
        //    {
        //        return "";
        //    }
        //}
        override public string Text
        {
            get
            {
                return String.Format("{1} {0}", Fbg.Bll.CONSTS.PF_NameForPackage(this._pfPackageID), GetProperDuration(Duration));
            }
        }

        private string GetProperDuration(TimeSpan d)
        {
            return Fbg.Bll.utils.FormatDuration_Long2(d);
        }

        override public bool Use(Fbg.Bll.Player player, int villageID)
        {
            if (base.isItemAvailable())
            {
                player.PF_UseViaItem2(_pfPackageID, Duration);
                player.LogEvent(Fbg.Bll.CONSTS.UserLogEvents.Item2UsageLog, "Item2_PFWithDuration", String.Format("ItemID:{0}. enabled PFPckg:{1} (ID:{2}) for {3}min", this.ID, Fbg.Bll.CONSTS.PF_NameForPackage(this._pfPackageID), this._pfPackageID, Duration.TotalMinutes)); 
                base.MarkItemAsUsed();
                return true;
            }

            return false;
        }

        override internal bool isCompatibleWithThisPlayer(Player player)
        {
            if (player.Realm.PFPackage(this._pfPackageID) != null) {

               
                return true;
            }

            return false;
        }


        override public bool specialEquals(Item2 item)
        {
            if (item.GetType() == this.GetType())
            {
                Item2_PFWithDuration itemOfThisType = (Item2_PFWithDuration)item;
                return itemOfThisType.PFPackageID == this.PFPackageID && itemOfThisType.Duration == this.Duration;
            }
            return false;
        }
    }
}
