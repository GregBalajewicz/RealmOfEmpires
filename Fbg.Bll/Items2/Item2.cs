using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fbg.Bll.Items2
{
    public abstract class Item2
    {
        public long ID   {get;set;}
	    public Fbg.Bll.User User {get; private set;}
	    public int PlayerID {get; private set;}
	    public DateTime? ExpiresOn {get; private set;}
        abstract public string Type { get; }

        public Item2(long id, Fbg.Bll.User user, int playerID, DateTime? expiresOn)
        {
            ID = id;
            User = user;
            PlayerID = playerID;
            ExpiresOn = expiresOn;
        }

        abstract public string Text { get; }
        abstract public bool Use(Fbg.Bll.Player player, int villageID);


        protected void MarkItemAsUsed()
        {
            Fbg.DAL.User.Items2_MarkAsUsed(User.ID, ID);
        }

        abstract internal bool isCompatibleWithThisPlayer(Player player);

        /// <summary>
        /// makes a call to DB to check if this item is still available
        /// </summary>
        /// <returns></returns>
        internal bool isItemAvailable()
        {
            return Fbg.DAL.User.Items2_GetItem(this.User.ID, this.ID).Rows.Count == 1;
        }



        /// <summary>
        /// compares two items, and returns true if they are off the same time AND have some, but not all properties the same. See implementation for each item type
        ///     This is used to create groupings of simillar items. like a group of all silver items with the same amount of silver
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        abstract public bool specialEquals(Item2 item);
    }



    public class Item2ItemsGroup
    {
        public string groupID { get; set; }
        public Item2 _firstItem { get; set; }
        public List<long> itemIDs { get; set; }

        public Item2ItemsGroup(Item2 firstItem)
        {
            _firstItem = firstItem;
            groupID = firstItem.Type + firstItem.Text;
            itemIDs = new List<long>();
            addItem(firstItem.ID);
            
        }

        public void addItem(long itemID)
        {
            count++;
            itemIDs.Add(itemID);
        }
        public void removeItem(long itemID)
        {
            count--;
            itemIDs.Remove(itemID);
        }


        public int count { get; private set;}
    }
}
