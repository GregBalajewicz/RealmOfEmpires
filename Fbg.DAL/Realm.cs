using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Data.Common;
namespace Fbg.DAL
{
    public class Realm
    {
        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.Realm");
        public static DataSet GetAllRealms()
        {
            TRACE.InfoLine("in 'GetAllRealms()'");
            DataSet ds;
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                ds = db.ExecuteDataSet("qRealms", new object[0]);
            }
            catch (Exception e)
            {
                throw new BaseApplicationException("Error while calling 'qRealms'", e);
            }
            finally
            {
            }

           
            return ds;
        }

        public static DataSet GetRealmInfo(string connectionStr)
        {
            TRACE.InfoLine("in 'GetRealmInfo()'");
            DataSet ds;
            Database db;

            if (string.IsNullOrEmpty(connectionStr))
            {
                throw new ArgumentNullException("connectionStr");
            }

            try
            {
                db = new DB(connectionStr);
                ds = db.ExecuteDataSet("qCompleteRealmInfo2", new object[0]);
            }
            catch (Exception e)
            {
                BaseApplicationException be = new BaseApplicationException("Error while calling 'qCompleteRealmInfo2'", e);
                be.AdditionalInformation.Add("connectionStr", connectionStr);
                throw be;
            }
            finally
            {
            }

            return ds;
        }





        
        /// <param name="UserID"></param>
        /// <param name="Name"></param>
        /// <param name="realmID"></param>
        /// <returns>0 if this player name has been taken by someone else already. anyother value if name available. </returns>
        public static int RegisterPlayer_CheckNameOnly(string Name, Guid userID)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                DataTable tbl = DB.ExecuteDataSet(db, "iRegisterPlayer_CheckPlayerName", new object[] { Name, userID }).Tables[0];
                return (int)tbl.Rows[0][0];
            }
            catch (Exception e)
            {
                throw new BaseApplicationException("Error while calling iRegisterPlayer_CheckPlayerName", e);
            }
            finally
            {
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Name"></param>
        /// <param name="realmID"></param>
        /// <param name="connectionStr"></param>
        /// <param name="intInvitaionID">pass in null if this registration is not an acceptance of an invite. Pass InviteID (int!) if it is. </param>
        /// <param name="isInvitedToClan">only valid if intInvitaionID is not null. </param>
        /// <param name="invitingPlayerID">if 0 means player did not register as part of a invitation (if intInvitaionID was specified, then it must have been invalid)</param>
        /// <returns>0 if player Alreday Exist otherwise the playerid</returns>
        public static int RegisterPlayer(Guid UserID, string Name, string facebookID, int realmID
            , string connectionStr, short sexID, object intInvitaionID
            , out bool isInvitedToClan, out int invitingPlayerID, Fbg.Common.StartInQuadrants startInQuadrant, string villageName, int avatarId
            , Fbg.Common.UserLoginType playerLoginType)
        {
            TRACE.InfoLine("in 'RegisterPlayer'.");
            Database dbCommon;
            Database dbRealm;
            int playerID = 0;
            int inviteToClanResult=0;

            invitingPlayerID = 0;
            isInvitedToClan = false;

            DbTransaction tran;

            try
            {
                dbCommon = DatabaseFactory.CreateDatabase();
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling DatabaseFactory.CreateDatabase()", e);
                ex.AdditionalInformation.Add("UserID", UserID.ToString());
                ex.AdditionalInformation.Add("Name", Name);
                ex.AdditionalInformation.Add("RealmID", realmID.ToString());
                ex.AddAdditionalInformation("facebookID", facebookID);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("sexID", sexID);
                throw ex;
            }

            using (DbConnection CommonConnection = dbCommon.CreateConnection())
            {
                //
                // Get connection and start transaction
                try
                {
                    CommonConnection.Open();
                    tran = CommonConnection.BeginTransaction();
                }
                catch (Exception e)
                {
                    BaseApplicationException ex = new BaseApplicationException("Error while calling opening connection & tran on common DB", e);
                    ex.AdditionalInformation.Add("UserID", UserID.ToString());
                    ex.AdditionalInformation.Add("Name", Name);
                    ex.AdditionalInformation.Add("RealmID", realmID.ToString());
                    ex.AddAdditionalInformation("facebookID", facebookID);
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    ex.AddAdditionalInformation("sexID", sexID);
                    ex.AddAdditionalInformation("invitingPlayerID", invitingPlayerID);
                    ex.AddAdditionalInformation("playerLoginType", ((Int16)playerLoginType));
                    throw ex;
                }
                //
                // register the player in the common DB 
                //
                try
                {
                    DataTable tbl = dbCommon.ExecuteDataSet(tran, "iRegisterPlayerInCommon", new object[] { UserID, Name, realmID, intInvitaionID, playerLoginType }).Tables[0];
                    playerID = (int)tbl.Rows[0][0];
                    invitingPlayerID = (int)tbl.Rows[0][1];
                    //Function fail (Player Alreday Exist ) rollbck and exit function
                    if (playerID == 0)
                    {
                        tran.Rollback();
                        return 0;
                    }
                    else if (playerID == -1)//max players in realm reached 
                    {
                        return -1;
                    }

                }
                catch (Exception e)
                {
                    try
                    {
                        if (tran != null) { tran.Rollback(); };
                    }
                    catch (Exception) { }

                    BaseApplicationException ex = new BaseApplicationException("Error while calling 'iRegisterPlayerInCommon' on common DB", e);
                    ex.AdditionalInformation.Add("UserID", UserID.ToString());
                    ex.AdditionalInformation.Add("Name", Name);
                    ex.AdditionalInformation.Add("RealmID", realmID.ToString());
                    ex.AddAdditionalInformation("facebookID", facebookID);
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    ex.AddAdditionalInformation("sexID", sexID);
                    ex.AddAdditionalInformation("invitingPlayerID", invitingPlayerID);
                    ex.AddAdditionalInformation("playerLoginType", ((Int16)playerLoginType));
                    throw ex;
                }
                //
                //register the player in realm specific DB
                // 
                try
                {
                    dbRealm = new DB(connectionStr);


                    System.Data.Common.DbCommand cmd = dbRealm.GetStoredProcCommand("iRegisterPlayerInRealm");

                    dbRealm.AddInParameter(cmd, "@PlayerID", System.Data.DbType.Int32, playerID);
                    dbRealm.AddInParameter(cmd, "@PlayerName", System.Data.DbType.String, Name);
                    dbRealm.AddInParameter(cmd, "@UserID", System.Data.DbType.Guid, UserID);
                    dbRealm.AddInParameter(cmd, "@SexID", System.Data.DbType.Int16, sexID);
                    dbRealm.AddInParameter(cmd, "@InvitingPlayerID", System.Data.DbType.Int32, invitingPlayerID);
                    dbRealm.AddInParameter(cmd, "@InQuadrant", System.Data.DbType.Int16, startInQuadrant == Fbg.Common.StartInQuadrants.NoneSelected ? null : (object)startInQuadrant);
                    dbRealm.AddInParameter(cmd, "@VillageName", System.Data.DbType.String, villageName);
                    dbRealm.AddInParameter(cmd, "@AvatarID", System.Data.DbType.Int16, avatarId);
                    dbRealm.AddOutParameter(cmd, "@Result", System.Data.DbType.Int32, 0);
                    dbRealm.ExecuteNonQuery(cmd);
                    if (!(dbRealm.GetParameterValue(cmd, "@Result") is DBNull))
                    {
                        inviteToClanResult = (int)dbRealm.GetParameterValue(cmd, "@Result");
                    }

                    if (inviteToClanResult == 1)
                    {
                        isInvitedToClan = true;
                    }
                    else if (inviteToClanResult == 2)
                    {
                        isInvitedToClan = false;
                    }
                }
                catch (Exception e)
                {
                    tran.Rollback();

                    BaseApplicationException ex = new BaseApplicationException("Error while calling 'iRegisterPlayerInRealm' on realm specific DB", e);
                    ex.AdditionalInformation.Add("UserID", UserID.ToString());
                    ex.AdditionalInformation.Add("Name", Name);
                    ex.AdditionalInformation.Add("RealmID", realmID.ToString());
                    ex.AddAdditionalInformation("UserID", UserID);
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    ex.AddAdditionalInformation("sexID", sexID);
                    ex.AddAdditionalInformation("invitingPlayerID", invitingPlayerID);
                    ex.AddAdditionalInformation("playerLoginType", ((Int16)playerLoginType));
                    throw ex;
                }
                //
                // Commit the register player in common DB
                //
                try
                {
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();

                    BaseApplicationException ex = new BaseApplicationException("Error while calling commiting tran", e);
                    ex.AdditionalInformation.Add("UserID", UserID.ToString());
                    ex.AdditionalInformation.Add("Name", Name);
                    ex.AdditionalInformation.Add("RealmID", realmID.ToString());
                    ex.AdditionalInformation.Add("connectionStr", connectionStr);
                    ex.AddAdditionalInformation("sexID", sexID);
                    ex.AddAdditionalInformation("playerLoginType", ((Int16)playerLoginType));
                    throw ex;
                }

                CommonConnection.Close();
            }
            //
            // handle starting levels
            //
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                db.ExecuteNonQuery("iRegisterPlayerInCommon_PlayerStartingLevels", new object[] { UserID, playerID, realmID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iRegisterPlayerInCommon_PlayerStartingLevels. Quietly logging the error only", e);
                ex.AdditionalInformation.Add("UserID", UserID.ToString());
                ex.AdditionalInformation.Add("Name", Name);
                ex.AdditionalInformation.Add("RealmID", realmID.ToString());
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AddAdditionalInformation("UserID", UserID);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("sexID", sexID);
                ex.AddAdditionalInformation("invitingPlayerID", invitingPlayerID);
                ex.AddAdditionalInformation("playerLoginType", ((Int16)playerLoginType));
                ExceptionManager.Publish(ex);
            }
            finally
            {
            }


            return playerID;

        }
        public static bool AddPlayerQ(Guid UserID, int realmID)
        {

            TRACE.InfoLine("in 'AddPlayerQ()'");
            Database db;
            bool Result;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                Result = Convert.ToBoolean ( db.ExecuteScalar("iPlayerQ", new object[] { UserID, realmID }));
            }
            catch (Exception e)
            {
                throw new BaseApplicationException("Error while calling 'AddPlayerQ'", e);
            }
            finally
            {
            }

            return Result;
        }


        //public static DataTable GetFBFriendsInRealm(string connectionStr, string listOfFriends)
        //{
        //    TRACE.InfoLine("in 'GetFBFriendsInRealm()'");
        //    Database db;

        //    if (string.IsNullOrEmpty(connectionStr))
        //    {
        //        throw new ArgumentNullException("connectionStr");
        //    }

        //    try
        //    {
        //        db = new DB(connectionStr);
        //        return db.ExecuteDataSet("qGetFBFriendsInRealm", new object[] { listOfFriends, 0 }).Tables[0];
        //    }
        //    catch (Exception e)
        //    {
        //        BaseApplicationException be = new BaseApplicationException("Error while calling 'qGetFBFriendsInRealm'", e);
        //        be.AdditionalInformation.Add("connectionStr", connectionStr);
        //        be.AdditionalInformation.Add("listOfFriends", listOfFriends);
        //        throw be;
        //    }
        //    finally
        //    {
        //    }

        //}
    


        public static int AdminStatus(string connectionStr)
        {
            Database db;

            if (string.IsNullOrEmpty(connectionStr))
            {
                throw new ArgumentNullException("connectionStr");
            }

            try
            {
                db = new DB(connectionStr);
                return (int)db.ExecuteScalar("qAdmin_EventHandlerStatus", new object[] { });
            }
            catch (Exception e)
            {
                BaseApplicationException be = new BaseApplicationException("Error while calling 'qAdmin_EventHandlerStatus'", e);
                be.AdditionalInformation.Add("connectionStr", connectionStr);
                throw be;
            }
            finally
            {
            }
        }
    }

}
    

 