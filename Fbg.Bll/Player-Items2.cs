using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fbg.Bll.Items2;

namespace Fbg.Bll
{
    partial class Player
    {


        private int _items2_cacheversionGotten=0;
        List<Fbg.Bll.Items2.Item2> _items2;
        List<Fbg.Bll.Items2.Item2ItemsGroup> _items2_ItemGroups;
        public List<Fbg.Bll.Items2.Item2> Items2
        {

            get
            {
                if (_items2 == null || _user.Items2_cacheversion != _items2_cacheversionGotten)
                {
                    List<Fbg.Bll.Items2.Item2> temp;
                    _items2_ItemGroups = new List<Item2ItemsGroup>();
                    int indexOfGroupElement;

                    temp = _user.Items2.FindAll(s => s.PlayerID == this.ID || s.PlayerID == 0);
                    _items2 = new List<Item2>(temp.Count);
                    foreach (Item2 item in temp)
                    {
                        if (item.isCompatibleWithThisPlayer(this))
                        {
                            _items2.Add(item);

                            //
                            // create groups
                            //

                            // first see if we already have a group for this type of item. if not, add it, if not up the count
                            indexOfGroupElement = _items2_ItemGroups.FindIndex(s=> s._firstItem.specialEquals(item));
                            if (indexOfGroupElement == -1)
                            {
                                _items2_ItemGroups.Add(new Item2ItemsGroup(item));
                            }
                            else
                            {
                                _items2_ItemGroups[indexOfGroupElement].addItem(item.ID);
                            }
                        }
                    }
                    _items2_cacheversionGotten = _user.Items2_cacheversion;
                }

                return _items2;

            }
        }


        public List<Fbg.Bll.Items2.Item2ItemsGroup> Items2ItemGRoups
        {

            get
            {
                if (_items2_ItemGroups == null)
                {
                    var o = this.Items2;
                }

                return _items2_ItemGroups;

            }
        }


        public bool Items2_Use(int villageID, int itemID)
        {
            bool useSucess = false;
            Item2 item = Items2.FirstOrDefault(i => i.ID == itemID);
            if (item != null)
            {
                useSucess = item.Use(this, villageID);
                if (useSucess)
                {
                    this.Items2.Remove(item);
                    this.User.Items2.Remove(item);

                    // remove it from the item groupings
                    int indexOfGroupElement = Items2ItemGRoups.FindIndex(s => s._firstItem.specialEquals(item));
                    if (indexOfGroupElement != -1)
                    {                       
                        Items2ItemGRoups[indexOfGroupElement].removeItem(item.ID);
                        if (Items2ItemGRoups[indexOfGroupElement].count < 1)
                        {
                            Items2ItemGRoups.RemoveAt(indexOfGroupElement);
                        }
                    }
                }

                // QUESTS
                if (item is Item2_Troops)
                {
                    if (((Item2_Troops)item).UnitType.ID == Fbg.Bll.CONSTS.UnitIDs.LC)
                    {

                        Quests2.SetQuestAsCompleted("AttackRebel1");

                    }
                }
                if (item is Item2_PFWithDuration)
                {
                    if (((Item2_PFWithDuration)item).PFPackageID == 32)
                    {
                            Quests2.SetQuestAsCompleted("AttackRebel2");
                    }
                }
            }

            return useSucess;
        }


        /// <summary>
        /// give a troop item 
        /// </summary>
        /// <param name="expiresInHours"></param>
        /// <param name="unitType"></param>
        /// <param name="amount"></param>
        public void Items2_Give(int? expiresInHours, UnitType unitType, int amount)
        {
            Fbg.DAL.User.Items2_GiveItem(expiresInHours, this.User.ID, this.ID, unitType.ID, amount);
            Items2_Inalidate();
        }
        /// <summary>
        /// give a Silver Item 
        /// </summary>
        /// <param name="expiresInHours"></param>
        /// <param name="silveramount"></param>
        public void Items2_Give(int? expiresInHours, int silveramount)
        {
            Fbg.DAL.User.Items2_GiveItem(expiresInHours, this.User.ID, this.ID, silveramount);
            Items2_Inalidate();
        }
        /// <summary>
        /// give a PF Package item 
        /// </summary>
        /// <param name="expiresInHours"></param>
        /// <param name="pfPackageID"></param>
        /// <param name="durationInMinutes"></param>
        public void Items2_Give(int? expiresInHours, int pfPackageID, int durationInMinutes)
        {
            Fbg.DAL.User.Items2_GiveItem_pfwithduration(expiresInHours, this.User.ID, this.ID, pfPackageID, durationInMinutes);
            Items2_Inalidate();
        }
        /// <summary>
        /// give a building speed up item 
        /// </summary>
        /// <param name="expiresInHours"></param>
        /// <param name="pfPackageID"></param>
        /// <param name="durationInMinutes"></param>
        public void Items2_Give_BuildingSpeedup(int? expiresInHours, int amountInMinutes)
        {
            Fbg.DAL.User.Items2_GiveItem_BuildingSpeedup(expiresInHours, this.User.ID, this.ID, amountInMinutes);
            Items2_Inalidate();
        }
        /// <summary>
        /// give a research speed up item 
        /// </summary>
        /// <param name="expiresInHours"></param>
        /// <param name="pfPackageID"></param>
        /// <param name="durationInMinutes"></param>
        public void Items2_Give_ResearchSpeedup(int? expiresInHours, int amountInMinutes)
        {
            Fbg.DAL.User.Items2_GiveItem_ResearchSpeedup(expiresInHours, this.User.ID, this.ID, amountInMinutes);
            Items2_Inalidate();
        }
        public void Items2_Inalidate()
        {
            _items2 = null;
            _items2_ItemGroups = null;
            this._user.Items2_Inalidate();
        }
    }
}
