using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Fbg.Bll.Api
{
    public static class PlayerNotificationSettings
    {

        public class NotificationSetting
        {
            public int notificationID;
            public string name;
            public string desc;
            public Int16 MuteAtNightSettingID;
            public Int16 soundSettingID;
            public Int16 activeStateID;
        }

       
        public static List<NotificationSetting> Get(Fbg.Bll.Player player)
        {
            List<NotificationSetting> list;
            var myGifts = from r in Fbg.DAL.Players.PlayerNotificationSettings_get(player.Realm.ConnectionStr, player.ID).AsEnumerable()
                          select new NotificationSetting() { 
                              notificationID = r.Field<int>(0),
                              MuteAtNightSettingID = r.Field<Int16>(1),
                              soundSettingID= r.Field<Int16>(2),
                              activeStateID = r.Field<Int16>(3),
                              name = r.Field<string>(4),
                              desc = r.Field<string>(5),
                          };
            list = myGifts.ToList();
            

            return list;
        }



        public static List<NotificationSetting> Update(Fbg.Bll.Player player, int noficationID, Int16 vibrateOptionID, Int16 soundSettingID, Int16 activeStateID)
        {
            Fbg.DAL.Players.PlayerNotificationSettings_update(player.Realm.ConnectionStr, player.ID, noficationID, vibrateOptionID, soundSettingID, activeStateID);

            return Get(player);
         }

    }
}
