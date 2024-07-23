using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fbg.Bll;
using Fbg.Bll.Items2;
using System.Dynamic; 


namespace Fbg.Bll.Api.Items2
{
    /// <summary>
    /// Summary description for Items
    /// </summary>
    public class Items2
    {

        public static string GetAllItems(Fbg.Bll.Player p)
        {
            return ApiHelper.RETURN_SUCCESS(p.Items2, new Fbg.Bll.Api.ApiHelper.Items2Converter());
        }

        public static string GetAllItemGroups(Fbg.Bll.Player p, bool refreshFromFB)
        {
            if (refreshFromFB)
            {
                p.Items2_Inalidate();
            }
            return ApiHelper.RETURN_SUCCESS(p.Items2ItemGRoups, new Fbg.Bll.Api.ApiHelper.Items2Converter());
        }


        public static string UseItem(Player player, int villageID, int itemID, string groupID = null)
        {
            bool useSucess = false;
               
            //maybe only return pfpckgs if type was pfd? for performance?
            Fbg.Bll.Items2.Item2 item = player.Items2.Find(i => i.ID == itemID);

            if (item == null)
            {
                return ApiHelper.RETURN_SUCCESS(new
                {
                    villageID = villageID,
                    itemID = itemID,
                    groupID = groupID,
                    wasUsed = false
                });
            }

            string itemType = player.Items2.Find(i => i.ID == itemID).Type;

            useSucess = player.Items2_Use(villageID, itemID);
           
            return ApiHelper.RETURN_SUCCESS(new { 
                villageID = villageID,
                itemID = itemID,
                groupID = groupID,
                itemType = itemType,
                wasUsed = useSucess, 
                //myItemGroups = player.Items2ItemGRoups,
                PFPckgs = player.PF_PlayerPFPackages2.FindAll(s => { return s.Package.Id != 1; }).ToDictionary(p => p.Package.Id.ToString(),
                    p => new { id = p.Package.Id, ExpiresOn = Fbg.Bll.Api.ApiHelper.SerializeDate(p.ExpiresOn) })
 
            },
                new Fbg.Bll.Api.ApiHelper.Items2Converter());

        }

      
    }
}