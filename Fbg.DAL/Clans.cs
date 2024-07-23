using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.DAL
{
    public class Clans
    {

        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.Clans");

        public static DataSet CreateNewClan(string connectionStr, string name, string desc, int playerid)
        {

            TRACE.InfoLine("in 'Clans'.");
            Database db;

            try
            {
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("iClan");

                db.AddInParameter(cmd, "@Name", System.Data.DbType.String, name);
                db.AddInParameter(cmd, "@Desc", System.Data.DbType.String, desc);
                db.AddInParameter(cmd, "@PlayerID", System.Data.DbType.Int32, playerid);
                db.AddOutParameter(cmd, "@ClanID", System.Data.DbType.Int32, int.MaxValue);
                db.ExecuteNonQuery(cmd);
                int ClanID = (int)db.GetParameterValue(cmd, "@ClanID");
                if (ClanID == 0)
                {
                    return null;
                }
                else
                {
                    return GetClanByID(connectionStr, ClanID);
                }
               

            }
            catch (Exception e)
            {

                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iClan'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("Name", name);
                ex.AdditionalInformation.Add("Desc", desc);
                ex.AdditionalInformation.Add("PlayerID", playerid.ToString());
                throw ex;
            }
            
        }


        public static int InvitePlayer(string connectionStr, int InviterID, string PlayerName, int ClanID, out DateTime MoreInvitesAvailableOn)
        {
            TRACE.InfoLine("in 'Clans'."); 
            Database db;

            try
            {
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("iClanInvite");
                db.AddInParameter(cmd, "@InviterID", System.Data.DbType.Int32, InviterID);
                db.AddInParameter(cmd, "@PlayerName", System.Data.DbType.String, PlayerName);
                db.AddInParameter(cmd, "@ClanID", System.Data.DbType.Int32, ClanID );
                db.AddOutParameter(cmd, "@Error", System.Data.DbType.Int32, int.MaxValue);
                db.AddInParameter(cmd, "@InviteType", System.Data.DbType.Int16, 1);
                db.AddOutParameter(cmd, "@MoreInvitesAvailableOn", System.Data.DbType.DateTime, int.MaxValue);
                db.ExecuteNonQuery(cmd);
                int Error = (int)db.GetParameterValue(cmd, "@Error");

                if (db.GetParameterValue(cmd, "@MoreInvitesAvailableOn") is DBNull)
                {
                    MoreInvitesAvailableOn = DateTime.MinValue;
                }
                else
                {
                    MoreInvitesAvailableOn = (DateTime)db.GetParameterValue(cmd, "@MoreInvitesAvailableOn");
                }

                return Error;

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iClanInvite'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("inviterID", InviterID.ToString());
                ex.AdditionalInformation.Add("PlayerName", PlayerName);
                ex.AdditionalInformation.Add("clanID", ClanID.ToString ());
                throw ex;
            }
        }
 
        public static void DeleteClan(string connectionStr, int clanID,string playerNameOfOwnerDeletingTheClan)
        {

            ///this function delete specific clan and all its members and invitors
            TRACE.InfoLine("in 'Clans'.");
            Database db;

            try
            {
                db = new DB(connectionStr);

                db.ExecuteDataSet("dClan", new object[] { clanID, playerNameOfOwnerDeletingTheClan });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'dClan'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("clanID", clanID.ToString());
                ex.AddAdditionalInformation("playerNameOfOwnerDeletingTheClan", playerNameOfOwnerDeletingTheClan);
                throw ex;
            }
        }
        public static DataSet  GetClanByID(string connectionStr, int clanID)
        {
            TRACE.InfoLine("in 'Clans'");
            Database db;

            try
            {
                db = new DB(connectionStr);;

                return db.ExecuteDataSet("qClanByID", new object[] { clanID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qClanByID", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("clanID", clanID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static DataSet GetClanForPlayer(string connectionStr, int playerID)
        {
            TRACE.InfoLine("in 'Clans'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qPlayerClan", new object[] { playerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerClan", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static DataTable  ViewClanInvitations(string connectionStr, int clanID,string searchName)
        {
            TRACE.InfoLine("in 'Clans'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qClanInvites", new object[] { clanID ,searchName }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qClanInvites", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("clanID", clanID.ToString());
                ex.AdditionalInformation.Add("searchName", searchName);
                throw ex;
            }
            finally
            {
            }
        }
        public static DataSet ViewPlayerInvitations(string connectionStr, int playerID)
        {
            TRACE.InfoLine("in 'Clans'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qPlayerInvites", new object[] { playerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerInvites", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static DataSet ViewClanMembers(string connectionStr, int clanID)
        {
            TRACE.InfoLine("in 'Clans'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qClanMembers", new object[] { clanID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qClanMembers", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("clanID", clanID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static DataSet ViewClanMembersLite(string connectionStr, int clanID)
        {
            TRACE.InfoLine("in 'Clans'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qClanMembersLite", new object[] { clanID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qClanMembersLite", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("clanID", clanID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="playerID"> player to delete from clan</param>
        /// <param name="loggedInPlayerID">logged in player</param>
        /// <param name="clanID"></param>
        ///   /// <param name="Isleave">true if i leave my clan false if i  dismis a nother player</param>
        /// <param name="isLeave">true means it is player leaving on his own. then loggedInPlayerID == playerID. 
        /// false mean an admin is dismissing someone</param>
        public static Fbg.Common.Clan.DissmissFromClanResult LeaveClan(string connectionStr, int loggedInPlayerID, int playerID, int clanID,bool isLeave)
        {
            TRACE.InfoLine("in 'Clans'.");
            Database db;

            try
            {
                db = new DB(connectionStr);;
                int returnvalue = 0;
                int result = (int)db.ExecuteScalar("dClanMember", new object[] { clanID, loggedInPlayerID, playerID, isLeave, returnvalue, 0 });

                if (result > 1)
                {
                    return  Fbg.Common.Clan.DissmissFromClanResult.Success;
                }
                else if (result == 1)
                {
                    return Fbg.Common.Clan.DissmissFromClanResult.TryingToDismissLastOwner;
                }
                else
                {
                    return Fbg.Common.Clan.DissmissFromClanResult.AdminTryingToDismissOwner;
                }

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'dClanMember'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("loggedInPlayerID", loggedInPlayerID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("clanID", clanID.ToString());
                ex.AdditionalInformation.Add("isLeave", isLeave.ToString());
                throw ex;
            }
        }
        public static int JoinClan(string connectionStr, int playerID, int clanID)
        {
            TRACE.InfoLine("in 'Clans'.");
            Database db;

            try
            {
                db = new DB(connectionStr);

                int result=(int)db.ExecuteScalar("iClanJoin", new object[] { playerID, clanID  });
                
                return result;

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iClanJoin'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("clanID", clanID.ToString());
               
                throw ex;
            }
        }
        public static void CancelInvitation(string connectionStr, int playerID, int clanID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                db.ExecuteDataSet("dClanInvitation", new object[] { clanID, playerID });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'dClanInvitation'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("clanID", clanID.ToString());

                throw ex;
            }
        }
        public static void DeleteInvitation(string connectionStr, int playerID, int clanID)
        {
            TRACE.InfoLine("in 'Clans'.");
            Database db;

            try
            {
                db = new DB(connectionStr);

                db.ExecuteDataSet("dDeleteInvitation", new object[] { clanID,playerID });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'dDeleteInvitation'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("clanID", clanID.ToString());

                throw ex;
            }
        }
        /// <summary>
        /// return true if player is member of clan else retuen false
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="playerID"></param>
        public static DataSet  ViewMyClanDiplomacy(string connectionStr, int ClanID)
        {
            TRACE.InfoLine("in 'Clans'.");
            Database db;
            
            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qMyClanDiplomacy", new object[] { ClanID});


            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'qMyClanDiplomacy'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("ClanID", ClanID.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="ClanID"></param>
        /// <param name="OtherClanName"></param>
        /// <param name="DiplomacyStatus"></param>
        /// <returns>
        /// Success = 0,
        /// Diplomacy Already Exist = 1,
        /// Clan Dont Exist = 2
        /// </returns>
        public static int  AddDiplomacy(string connectionStr, int ClanID, string  OtherClanName, int DiplomacyStatus)
        {
            TRACE.InfoLine("in 'Clans'.");
            Database db;
            int result = 0;
            try
            {
                db = new DB(connectionStr);;

                 result = (int)db.ExecuteScalar("iMyClanDiplomacy", new object[] { ClanID, OtherClanName, DiplomacyStatus });

                 return result;//(the return will be 0 =Success OR 1=Diplomacy Already Exist OR 2=Clan Dont Exist)this handled in BLL function 'AddDiplomacy'
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iMyClanDiplomacy'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("ClanID", ClanID.ToString());
                ex.AdditionalInformation.Add("OtherClanName", OtherClanName);
                ex.AdditionalInformation.Add("DiplomacyStatus", DiplomacyStatus.ToString());
                throw ex;
            }
        }
        public static void  DeleteDiplomacy(string connectionStr, int ClanID, string  OtherClanName, int DiplomacyStatus)
        {
            TRACE.InfoLine("in 'Clans'.");
            Database db;
            try
            {
                db = new DB(connectionStr);

                 db.ExecuteDataSet("dMyClanDiplomacy", new object[] { ClanID, OtherClanName, DiplomacyStatus });


            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'dMyClanDiplomacy'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("ClanID", ClanID.ToString());
                ex.AdditionalInformation.Add("OtherClanName", OtherClanName);
                ex.AdditionalInformation.Add("DiplomacyStatus", DiplomacyStatus.ToString());
                throw ex;
            }
        }
        public static DataSet ViewClanPublicProfile(string connectionStr, int ClanID)
        {
            TRACE.InfoLine("in 'Clans'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qClanPublicProfile", new object[] { ClanID  });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qClanPublicProfile", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("ClanID", ClanID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static void UpdateClanPublicProfile(string connectionStr, int loggedinPlayerID, int ClanID, string PublicProfile)
        {

            TRACE.InfoLine("in 'Clans'.");
            Database db;

            try
            {

                db = new DB(connectionStr);

                db.ExecuteDataSet("uClanPublicProfile", new object[] { loggedinPlayerID, ClanID, PublicProfile });

            }
            catch (Exception e)
            {

                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uClanPublicProfile'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("ClanID", ClanID.ToString());
                ex.AdditionalInformation.Add("PublicProfile", PublicProfile);
                ex.AdditionalInformation.Add("loggedinPlayerID", loggedinPlayerID.ToString());
                throw ex;
            }
        }
        public static bool RenameClan(string connectionStr, int ClanID, string newClanName,int PlayerID)
        {

            TRACE.InfoLine("in 'Clans'.");
            Database db;

            try
            {

                db = new DB(connectionStr);

                return (bool)db.ExecuteScalar("uClanRename", new object[] { ClanID, newClanName, PlayerID });

            }
            catch (Exception e)
            {

                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uClanRename'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("ClanID", ClanID.ToString());
                ex.AdditionalInformation.Add("newClanName", newClanName);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString ());
                throw ex;
            }
        }
        public static DataSet GetClanEvents(string connectionStr, int clanID, bool topRowsOnly)
        {
            TRACE.InfoLine("in 'Clans'");
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qClanEvents", new object[] { clanID, topRowsOnly });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qClanEvents", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("clanID", clanID.ToString());
                ex.AddAdditionalInformation("topRowsOnly", topRowsOnly);
                throw ex;
            }
            finally
            {
            }
        }
        //public static void AddClanEvent(string connectionStr, int ClanID, string Message, DateTime Time)
        //{
        //    TRACE.InfoLine("in 'Clans'.");
        //    Database db;

        //    try
        //    {

        //        db = new DB(connectionStr);

        //        db.ExecuteDataSet("iClanEvents", new object[] { ClanID, Message, Time });
        //    }
        //    catch (Exception e)
        //    {
        //        BaseApplicationException ex = new BaseApplicationException("Error while calling 'iClanEvents'", e);
        //        ex.AdditionalInformation.Add("connectionStr", connectionStr);
        //        ex.AdditionalInformation.Add("ClanID", ClanID.ToString());
        //        ex.AdditionalInformation.Add("Message", Message);
        //        ex.AdditionalInformation.Add("Time", Time);
        //        throw ex;
        //    }
        //}

        public static DataSet GetClanSettings(string connectionStr, int ClanID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qClanSettings", new object[] { ClanID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qClanSettings", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("ClanID", ClanID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        public static void UpdateClanSettings(string connectionStr, int ClanID, bool inviterFlag)
        {

            Database db;

            try
            {

                db = new DB(connectionStr);

                db.ExecuteDataSet("uClanSettings", new object[] { ClanID, inviterFlag });

            }
            catch (Exception e)
            {

                BaseApplicationException ex = new BaseApplicationException("Error while calling uClanSettings", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("ClanID", ClanID.ToString());
                ex.AddAdditionalInformation("inviterFlag", inviterFlag);
                throw ex;
            }
        }



        public static void  GetInvitesLeft(string connectionStr, int InviterID, int ClanID, out int invitesLeft, out DateTime MoreInvitesAvailableOn)
        {
            Database db;
            invitesLeft = Int32.MaxValue;
            MoreInvitesAvailableOn = DateTime.MinValue;
                

            try
            {
                db = new DB(connectionStr);
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("qInvitesLeft");
                db.AddInParameter(cmd, "@ClanID", System.Data.DbType.Int32, ClanID);
                db.AddInParameter(cmd, "@InviterID", System.Data.DbType.Int32, InviterID);
                db.AddOutParameter(cmd, "@InvitesLeft", System.Data.DbType.Int32, int.MaxValue);
                db.AddOutParameter(cmd, "@InvitedAvailableOn", System.Data.DbType.DateTime, int.MaxValue);
                db.ExecuteNonQuery(cmd);
                
                if ((int)db.GetParameterValue(cmd, "@InvitesLeft") == 999999)
                {
                    invitesLeft = Int32.MaxValue;
                }
                else
                {
                    invitesLeft=(int)db.GetParameterValue(cmd, "@InvitesLeft");
                }


                if (db.GetParameterValue(cmd, "@InvitedAvailableOn") is DBNull)
                {
                    MoreInvitesAvailableOn = DateTime.MinValue;
                }
                else
                {
                    MoreInvitesAvailableOn = (DateTime)db.GetParameterValue(cmd, "@InvitedAvailableOn");
                }

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'qInvitesLeft'", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("inviterID", InviterID.ToString());
                ex.AddAdditionalInformation("invitesLeft", invitesLeft);
                ex.AddAdditionalInformation("MoreInvitesAvailableOn", MoreInvitesAvailableOn);
                ex.AdditionalInformation.Add("clanID", ClanID.ToString());
                throw ex;
            }
        }



        public static DataSet GetClanCommunicationBrief(string connectionStr, int playerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return db.ExecuteDataSet("qPlayerClanQuickComm", new object[] { playerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerClanQuickComm", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                throw ex;
            }
            finally
            {
            }
        }





        public static int ClaimVillage(string connectionStr, int playerID, int clanID, int villageID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return (int)db.ExecuteDataSet("iPlayerVillageClaims", new object[] { playerID, clanID, villageID }).Tables[0].Rows[0][0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iPlayerVillageClaims", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("clanID", clanID.ToString());
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                throw ex;
            }
            finally
            {
            }
        }

        public static int ClaimVillage_Unclaim(string connectionStr, int playerID, int clanID, int villageID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                db.ExecuteDataSet("dPlayerVillageClaims", new object[] { playerID, clanID, villageID });
                return 0;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dPlayerVillageClaims", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("clanID", clanID.ToString());
                ex.AdditionalInformation.Add("villageID", villageID.ToString());
                throw ex;
            }
            finally
            {
            }
        }
        
        public static DataSet ClaimVillage_GetSettings(string connectionStr, int clanID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                return db.ExecuteDataSet("qClanClaimSysSettings", new object[] { clanID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qClanClaimSysSettings", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("clanID", clanID.ToString());
                throw ex;
            }
            finally
            {
            }
        }

        public static void ClaimVillage_SaveSetting(string connectionStr, int clanID, int settingID, int settingInt)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                db.ExecuteNonQuery("uClanClaimSysSettings", new object[] { clanID, settingID, settingInt });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uClanClaimSysSettings", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("clanID", clanID.ToString());
                throw ex;
            }
            finally
            {
            }
        }

        
    }
}
