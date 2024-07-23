using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace ChatHub2
{

    public enum GroupTypeEnum
    {
        Global = 0,
        OneOnOne = 1,
        Group = 2,
        Clan = 3
    }


    public class ChatHub2
    {
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bllUserID"></param>
        /// <param name="AvatarID"></param>
        public static void updateChatUserEntityUserAvatar(Guid bllUserID, int AvatarID)
        {
        }
        public static void updateChatUserEntityPlayerAvatar(Fbg.Bll.Player bllPlayer)
        {

        }

        public static void updateChatUserEntityUserAvatarBorderID(Guid bllUserID, int updatedStatus, int userVIPLevel)
        {
           

        }

        #region joinorcreateclanchat

        /// <summary>
        /// update ChatUserEntityPlayer's clan
        /// </summary>
        /// <param name="player"></param>
        /// <param name="clanId"></param>
        /// <param name="realmId"></param>
        public static void addClanToChatUser(string playerId, int clanId, string realmId)
        {
        }

        /// <summary>
        /// static version used in joining/creating clan from asp.net form button click
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="clanId"></param>
        /// <param name="realmId"></param>
        public static void JoinOrCreateClanChat(string playerId, int clanId, string realmId)
        {
           
        }

        #endregion

        #region deleteclanchat
     
        #endregion

        #region dismissclanchat
        

        #endregion

      
    }








}
