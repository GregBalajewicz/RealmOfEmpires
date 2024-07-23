using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Fbg.Common
{
    public class Utils
    {
        public enum LoginType
        {
            FB
            ,MobileOrKindle
            ,Tactica
        }

        /// <summary>
        /// get the login type of the logged in player as identified by their username and email 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public static LoginType GetLoginType(string userName, string userEmail)
        {
            // are we limiting by login type? then see if we got a device id or facebook id 
            if (String.Equals(userName, userEmail, StringComparison.OrdinalIgnoreCase))
            {
                return Utils.LoginType.Tactica;
            }
            else if (userName.Length > 25)
            {
                return Utils.LoginType.MobileOrKindle;
            }

            return LoginType.FB;  
        }
    }
}
