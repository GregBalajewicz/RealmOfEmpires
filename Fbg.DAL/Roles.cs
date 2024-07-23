using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.DAL
{

    public class Roles
    {
        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.Roles");
        public static DataSet GetPlayerRoles(string connectionStr, int ClanID, int PlayerID)
        {
            TRACE.InfoLine("in 'Roles'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qPlayerRoles", new object[] { ClanID,PlayerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerRoles", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("clanID", ClanID.ToString());
                ex.AdditionalInformation.Add("PlayerID", ClanID.ToString());
                throw ex;
            }
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="ClanID"></param>
        /// <param name="PlayerID"></param>
        /// <param name="RoleID"></param>
        /// <returns>true if success false if failed </returns>
        public static bool  RemovePlayerRole(string connectionStr, int ClanID, int PlayerID,int RoleID)
        {
            TRACE.InfoLine("in 'Roles'");
            Database db;

            try
            {
                db = new DB(connectionStr);;

                int Return= (int)db.ExecuteScalar("dPlayerRole", new object[] { ClanID, PlayerID,RoleID });
                if (Return > 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dPlayerRole", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("clanID", ClanID.ToString());
                ex.AdditionalInformation.Add("PlayerID", ClanID.ToString());
                ex.AdditionalInformation.Add("RoleID", ClanID.ToString());
                throw ex;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="ClanID"></param>
        /// <param name="PlayerID"></param>
        /// <param name="RoleID"></param>
        /// <returns>true if success false if failed </returns>
        public static bool  AddPlayerRole(string connectionStr, int ClanID, int PlayerID, int RoleID)
        {
            TRACE.InfoLine("in 'Roles'");
            Database db;

            try
            {
                db = new DB(connectionStr);;

                int Return =(int)db.ExecuteScalar("iPlayerRole", new object[] { ClanID, PlayerID, RoleID });
                if (Return == 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iPlayerRole", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("clanID", ClanID.ToString());
                ex.AdditionalInformation.Add("PlayerID", ClanID.ToString());
                ex.AdditionalInformation.Add("RoleID", ClanID.ToString());
                throw ex;
            }

        }
    }
}
