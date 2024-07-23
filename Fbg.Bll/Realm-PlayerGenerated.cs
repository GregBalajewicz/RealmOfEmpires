using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Globalization;

namespace Fbg.Bll
{ 
   
    public partial class Realm : ISerializableToNameValueCollection2
    {

        public PlayerGeneratedSettings PlayerGenerated { get; private set; }

        public class PlayerGeneratedSettings
        {

            public bool IsPlayerGenerated { get; private set; }
            public Guid GeneratedByUserID { get; private set; }

            public PrivateSettings Private {get; private set; }


            public PlayerGeneratedSettings(object userid, object password)
            {
                if (IsThisAPrivateRealm(userid))
                {
                    IsPlayerGenerated = false;
                    Private = new PrivateSettings(string.Empty);
                }
                else
                {
                    IsPlayerGenerated = true;
                    GeneratedByUserID = (Guid)userid;
                    Private = new PrivateSettings(password);
                }
            }

            private static bool IsThisAPrivateRealm(object userid)
            {
                return userid == null || userid is DBNull;
            }

            public class PrivateSettings
            {                
                public PrivateSettings(object password)
                {
                    EntryPasscode = password is DBNull ? string.Empty : (string)password;
                    isPrivate = !(EntryPasscode.Trim() == string.Empty);
                }
                public string EntryPasscode { get; private set; }
                public readonly bool isPrivate;
            }
        }

    }
}
