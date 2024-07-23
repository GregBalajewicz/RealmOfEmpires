

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

using Gmbc.Common.Diagnostics.ExceptionManagement;

using Fbg.Common;

namespace Fbg.DAL
{
    public  class Quests
    {

        public static bool HasPlayerMadeAForumPost(string connectionStr, string playerName)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                int numPosts = (int)db.ExecuteScalar("qHasPlayerMadeAForumPost", new object[] 
                    { 
                        playerName 
                    });

                return (numPosts > 0 ? true: false);
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qHasPlayerMadeAForumPost", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("playerName", playerName);
                throw ex;
            }

        }
 
    }
}

