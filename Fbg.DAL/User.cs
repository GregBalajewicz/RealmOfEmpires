using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Linq;
using System.Data.Linq;


using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Collections.Concurrent;

namespace Fbg.DAL
{
    public class User
    {
        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.User");
        public static DataSet GetUser(Guid userID)
        {
            DataSet ds;
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                ds  = DB.ExecuteDataSet(db,"qUser", new object[] { userID});

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qUser", e);
                ex.AdditionalInformation.Add("UserID", userID.ToString());
                throw ex;
            }
            finally
            {
            }

            
            return ds;
        }
        public static DataSet GetUserStats(Guid userID)
        {
            DataSet ds;
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                ds = DB.ExecuteDataSet(db,"qUserStats", new object[] { userID });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qUserStats", e);
                ex.AdditionalInformation.Add("UserID", userID.ToString());
                throw ex;
            }
            finally
            {
            }


            return ds;
        }

        public static DataSet GetThroneRoomTournamentRealmStats(Guid userid)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return DB.ExecuteDataSet(db, "qThroneRoomTournamentRealmStats", new object[] { userid });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'qThroneRoomTournamentRealmStats'", e);
                ex.AddAdditionalInformation("userId", userid);                
                throw ex;
            }
        }

        public static int GetUserCredits(Guid userID)
        {
            TRACE.InfoLine("in 'GetUser()'");
            
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                object credits = DB.ExecuteScalar(db,"qUserCredits", new object[] { userID });

                return credits == null ||credits is DBNull ? 0 : (int)credits;

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qUserCredits", e);
                ex.AdditionalInformation.Add("UserID", userID.ToString());
                throw ex;
            }
            finally
            {
            }   
        }

        public static int GetUserTransferableCredits(Guid userID)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                return (int)DB.ExecuteDataSet(db,"qTransferableCredits", new object[] { userID, null }).Tables[0].Rows[0][0];

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qTransferableCredits", e);
                ex.AdditionalInformation.Add("UserID", userID.ToString());
                throw ex;
            }
            finally
            {
            }
        }



        public static void TransferCredits(int amountOfCredits, int playerToTransferToID, Guid transferingUserID)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteNonQuery(db,"uTransferCredits", new object[] { transferingUserID, amountOfCredits, playerToTransferToID});

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uTransferCredits", e);
                ex.AdditionalInformation.Add("transferingUserID", transferingUserID.ToString());
                ex.AdditionalInformation.Add("playerToTransferToID", playerToTransferToID.ToString());
                ex.AdditionalInformation.Add("amountOfCredits", amountOfCredits.ToString());
                throw ex;
            }
            finally
            {
            }
        }

        
       
        public static void BuyCredits(Guid userID,int creditsAmount)
        {
            TRACE.InfoLine("in 'BuyCredits()'");

            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteDataSet(db,"uBuyCredits", new object[] { userID, creditsAmount });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uBuyCredits", e);
                ex.AdditionalInformation.Add("UserID", userID.ToString());
                ex.AdditionalInformation.Add("CreditsAmount", creditsAmount.ToString());
                throw ex;
            }
            finally
            {
            }


        }

        public static void UseCredits(Guid userID, int creditsAmount, int eventType, int costField)
        {
            TRACE.InfoLine("in 'UseCredits()'");

            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                int result = 0;
                DB.ExecuteDataSet(db, "uCredits_Subtract3", new object[] { userID, creditsAmount, eventType, costField, result });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uCredits_Subtract3", e);
                ex.AdditionalInformation.Add("UserID", userID.ToString());
                ex.AdditionalInformation.Add("CreditsAmount", creditsAmount.ToString());
                ex.AdditionalInformation.Add("EventType", eventType.ToString());
                ex.AdditionalInformation.Add("CostField", costField.ToString());
                throw ex;
            }
            finally
            {
            }


        }

        public static void GetCreditsFromQuest(Guid userID, int creditsAmount)
        {

            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteDataSet(db, "uGetCredits", new object[] { userID, creditsAmount });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uGetCredits", e);
                ex.AdditionalInformation.Add("UserID", userID.ToString());
                ex.AdditionalInformation.Add("CreditsAmount", creditsAmount.ToString());
                throw ex;
            }
            finally
            {
            }


        }

        /// <summary>
        /// To register a donor, if already exists an update query will be executed otherwise new row will be inserted
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="runningTotalDonationAmount"></param>
        /// <param name="wantsToBeAnonymous"></param>
       

        public static DataTable GetDonors(int realmID)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return (DataTable)db.ExecuteDataSet("qDonors", new object[] { realmID }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qDonors", e);
                return null;
            }
        }
        public static void Update(Guid userID,float timezone)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                DB.ExecuteNonQuery(db,"uUser", new object[] { userID,timezone });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uUser", e);
                
            }
        }

        public static DataTable GetUsers()
        {
            TRACE.InfoLine("in 'GetUsers()'");
            DataTable  dt;
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                dt = DB.ExecuteDataSet(db, "qUsersWithoutDemographicDetailsPopulated", new object[] { }).Tables[0];

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qUsersWithoutDemographicDetailsPopulated", e);
                
                throw ex;
            }
            finally
            {
            }


            return dt;
        }
   
        public static DataTable GetMyInvites(string facebookID)
        {        
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                return DB.ExecuteDataSet(db, "qInvitesForMe", new object[] { facebookID, }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qInvitesForMe", e);
                ex.AddAdditionalInformation("facebookID", facebookID);
                throw ex;
            }
        }
        public static DataTable Invites_GetInvited(Guid userID)
        {
            TRACE.InfoLine("in 'Invites_GetInvited'");
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                return DB.ExecuteDataSet(db, "qInvites", new object[] { userID }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qInvites", e);
                ex.AdditionalInformation.Add("userID", userID.ToString());
                throw ex;
            }

        }


        public static void Invites_CancelInvite(Guid userID, int InviteID)
        {
            TRACE.InfoLine("in 'Invites_CancelInvite'");
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteNonQuery(db, "dInvites", new object[] { userID, InviteID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dInvites", e);
                ex.AdditionalInformation.Add("userID", userID.ToString());
                ex.AddAdditionalInformation("InviteID", InviteID);
                throw ex;
            }
        }

        /// <summary>
        /// returns the UserID of the invited person who is a knight or higher; the person who got 5 servants. 
        /// returns Guid.Empty if something went wrong and the reward was not given. 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="InviteID"></param>
        /// <param name="invitedPlayersFBID"></param>
        /// <returns></returns>
        public static Guid Invites_ClaimReward(Guid userID, int InviteID, string invitedPlayersFBID)
        {
            TRACE.InfoLine("in 'Invites_ClaimReward'");
            Database db;

            DataSet ds = null;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                ds = DB.ExecuteDataSet(db, "uInvites_ClaimReward", new object[] { userID, InviteID, invitedPlayersFBID });

                if (ds.Tables.Count == 0)
                {
                    return Guid.Empty;
                }
                else
                {
                    return (Guid)ds.Tables[0].Rows[0][0];
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uInvites_ClaimReward", e);
                ex.AdditionalInformation.Add("userID", userID.ToString());
                ex.AddAdditionalInformation("InviteID", InviteID);
                ex.AddAdditionalInformation("invitedPlayersFBID", invitedPlayersFBID);
                ex.AddAdditionalInformation("ds", ds);
                throw ex;
            }
        }

        


         
        public static DataTable Invites_GetAcceptedInvites(Guid userID)
        {
            TRACE.InfoLine("in 'Invites_GetAcceptedInvites'");
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                return DB.ExecuteDataSet(db, "qInvites_Accepted", new object[] { userID }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qInvites_Accepted", e);
                ex.AdditionalInformation.Add("userID", userID.ToString());
                throw ex;
            }

        }


        public static void UserDetails_Add(Guid userID, DateTime birthday, string locationCountry, string locationCity, string locationState, string Interests, byte relationshipStatus, string religion, string SignificantOtherFacebookID)
        {
            TRACE.InfoLine("in 'UserDetails_Add'");
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                 DB.ExecuteNonQuery(db, "iUserDetails", new object[] { userID,birthday,locationCountry,locationCity,locationState,Interests,relationshipStatus,religion ,SignificantOtherFacebookID  });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iUserDetails", e);
                ex.AdditionalInformation.Add("userID", userID.ToString());
                ex.AdditionalInformation.Add("birthday", birthday.ToString());
                ex.AdditionalInformation.Add("locationCountry", locationCountry);
                ex.AdditionalInformation.Add("locationCity", locationCity);
                ex.AdditionalInformation.Add("locationState", locationState);
                ex.AdditionalInformation.Add("Interests", Interests);
                ex.AdditionalInformation.Add("relationshipStatus", relationshipStatus.ToString());
                ex.AdditionalInformation.Add("religion", religion);
                ex.AdditionalInformation.Add("SignificantOtherFacebookID", SignificantOtherFacebookID);
                
                throw ex;
            }

        }

        public static void SetFlag(int flagID, Guid userID, String data)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteNonQuery(db,"iUserFlag", new object[] 
                    { 
                        userID 
                        , flagID
                        , data
                    });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iUserFlag", e);
                ex.AdditionalInformation.Add("flagID", flagID.ToString());
                ex.AdditionalInformation.Add("userID", userID.ToString());
                ex.AddAdditionalInformation("data", data);
                throw ex;
            }


        }

        public static void LogEvent(Guid userID,  int playerID, int eventID, string message, string data)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteNonQuery(db,"iUserLog", new object[] 
                    { 
                        userID
                        , playerID 
                        , eventID
                        , message
                        , data
                    });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iUserLog", e);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("eventID", eventID.ToString());
                ex.AddAdditionalInformation("message", message);
                ex.AddAdditionalInformation("data", data);
                throw ex;
            }
        }

        public static void Gift_AcceptGift(Guid userID, int giftID)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteNonQuery(db,"uGift_Accept", new object[] 
                    { 
                        userID
                        , giftID 
                    });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uGift_Accept", e);
                ex.AddAdditionalInformation("userID", userID);
                ex.AddAdditionalInformation("giftID", giftID);
                throw ex;
            }
        }

        public static int[] Gift_AcceptByRequest(Guid userID, string requestID)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                var ds = DB.ExecuteDataSet(db, "uGift_AcceptByRequest", new object[] { 
                        userID, requestID 
                    });

                return ds.Tables[0].Rows.Cast<DataRow>().Select(row => (int)row["GiftID"]).ToArray();
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uGift_AcceptByRequest", e);
                ex.AddAdditionalInformation("userID", userID);
                ex.AddAdditionalInformation("requestID", requestID);
                throw ex;
            }
        }

        public static DataTable Gift_GetMyGifts(Guid userID, int realmID)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();

               return DB.ExecuteDataSet(db,"qGift_GetMyGifts", new object[] 
                    { 
                        userID
                        , realmID
                    }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qGift_GetMyGifts", e);
                ex.AddAdditionalInformation("userID", userID);
                throw ex;
            }
        }

        public static void Gifts_DeleteGift(Guid userID, int recordID)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteNonQuery(db,"uGift_Delete", new object[] 
                    { 
                        userID
                        , recordID
                    });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uGift_Delete", e);
                ex.AddAdditionalInformation("userID", userID);
                ex.AddAdditionalInformation("recordID", recordID);
                throw ex;
            }
        }

        public static DataTable Gifts_GetTodaysGifts(Guid userID)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                return DB.ExecuteDataSet(db, "qGift_GetSentToday", new object[] { userID }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qGift_GetSentToday", e);
                ex.AdditionalInformation.Add("userID", userID.ToString());
                throw ex;
            }
        }

        public static void Gifts_UseGift(Guid userID, int giftID, int realmID)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteNonQuery(db,"uGift_Use", new object[] 
                    { 
                        userID
                        , giftID
                        , realmID
                    });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uGift_Use", e);
                ex.AddAdditionalInformation("userID", userID);
                ex.AddAdditionalInformation("giftID", giftID);
                throw ex;
            }
        }



        public static List<Fbg.Common.UsersFriend> RefreshFriends(string friendsList, Guid loggedInUserID)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                List<Fbg.Common.UsersFriend> users= new List<Common.UsersFriend>();
                using ( IDataReader reader  = db.ExecuteReader("uFBFriends", new object[] { loggedInUserID, friendsList, 0 }))
                {
                    while(reader.Read()) 
                    {
                        users.Add(new Common.UsersFriend() { FacebookID = (string)reader[1], UserID =(Guid) reader[0], NumOfRealms = (int) reader[2] });
                    }
                }
                return users;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uFBFriends", e);
                ex.AddAdditionalInformation("friendsList", friendsList);
                ex.AddAdditionalInformation("loggedInUserID", loggedInUserID);
                throw ex;
            }
        }

        public static int GetUserXP(Guid userID)
        {
            TRACE.InfoLine("in 'GetUserXP()'");

            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                
                return (int) ( DB.ExecuteScalar(db, "qUserXP", new object[] { userID }) ?? 1);

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qUserCredits", e);
                ex.AdditionalInformation.Add("UserID", userID.ToString());
                throw ex;
            }
            finally
            {
            }
        }

        public static bool PayRealmEntryFee(Guid userID, int entryCost, int realmid)
        {
           
            Database db;
            object ret;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                //DB.ExecuteDataSet(db, "uCredits_Subtract3.sql", new object[] { userID, entryCost, 25, realmid, });

                /*

                @UserID as uniqueidentifier,
	@CreditAmount as int,
	@EventTypeID as int,
	@CostField as int,
	@Result as int output

                */
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("uCredits_Subtract3");

                db.AddInParameter(cmd, "@userID", System.Data.DbType.Guid, userID);
                db.AddInParameter(cmd, "@CreditAmount", System.Data.DbType.Int32, entryCost);
                db.AddInParameter(cmd, "@EventTypeID", System.Data.DbType.Int32, 25);
                db.AddInParameter(cmd, "@CostField", System.Data.DbType.Int32, realmid);
                db.AddParameter(cmd, "@Result", System.Data.DbType.Int32, ParameterDirection.Output, "", DataRowVersion.Current, 0); 
                db.ExecuteNonQuery(cmd);

                ret = cmd.Parameters["@Result"].Value;

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uTransferCredits", e);
                ex.AddAdditionalInformation("userID", userID);
                ex.AddAdditionalInformation("entryCost", entryCost);
                ex.AddAdditionalInformation("realmid", realmid);
                throw ex;
            }
            finally
            {
            }

            if (ret == null || ret is DBNull)
            {
                return false;
            }
            else
            {
                return (int)ret == 1 ? true: false;
            }
        
        }

        public static List<Common.UsersFriend> GetFriends(Guid loggedInUserID)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                List<Fbg.Common.UsersFriend> users = new List<Common.UsersFriend>();
                using (IDataReader reader = db.ExecuteReader("qFBFriends", new object[] { loggedInUserID }))
                {
                    while (reader.Read())
                    {
                        users.Add(new Common.UsersFriend() { FacebookID = (string)reader[1], UserID = (Guid)reader[0], NumOfRealms = (int)reader[2] });
                    }
                }
                return users;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qFBFriends", e);
                ex.AddAdditionalInformation("loggedInUserID", loggedInUserID);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="email"></param>
        /// <returns>0 - all went well
        /// 7 - email already in use
        /// othervalue - other error occured</returns>
        public static int MigrateToBDAAccount(Guid userID, string email)
        {
            Database db;
            object retval = null;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                retval = db.ExecuteScalar("uMigrateToBDAAccount", new object[] { userID, email });
                return (int)(retval == null ? 0 : retval);
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uMigrateToBDAAccount", e);
                ex.AddAdditionalInformation("userID", userID);
                ex.AddAdditionalInformation("email", email);
                ex.AddAdditionalInformation("retval", retval);
                throw ex;
            }
        }

        public static string FriendCodeGet(Guid userID)
        {
            Database db;
            DataSet ds = null;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                ds = db.ExecuteDataSet("qFriendInviteCode", new object[] { userID });
                if (ds.Tables[0].Rows.Count > 0)
                {
                    return Convert.ToString( ds.Tables[0].Rows[0][0]);
                }
                return null;

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qFriendInviteCode", e);
                ex.AddAdditionalInformation("userID", userID);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="friendCode"></param>
        /// <param name="rewardTheInviterGet"></param>
        /// <param name="rewardTheInviteeGets"></param>
        /// <returns>false - something went wrong; either the friendcode is invalid, or already used, or it is the players own friend code
        /// true - all went well</returns>
        public static bool FriendCodeUse(Guid userID, string friendCode, int rewardTheInviterGet, int rewardTheInviteeGets)
        {
            Database db;
            DataSet ds = null;
            int retVal = -1;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                ds = db.ExecuteDataSet("uFriendInviteCode", new object[] { userID, friendCode, rewardTheInviterGet, rewardTheInviteeGets });
                if (ds.Tables[0].Rows.Count > 0)
                {
                    retVal = (int)ds.Tables[0].Rows[0][0];
                }
                return retVal == 0;

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uFriendInviteCode", e);
                ex.AddAdditionalInformation("userID", userID);
                ex.AddAdditionalInformation("friendCode", friendCode);
                ex.AddAdditionalInformation("rewardTheInviterGet", rewardTheInviterGet);
                ex.AddAdditionalInformation("rewardTheInviteeGets", rewardTheInviteeGets);
                throw ex;
            }
        }

        public static DataTable AllPlayers(Guid userID)
        {
            Database db;
            DataSet ds = null;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                ds = db.ExecuteDataSet("qAllPlayers", new object[] { userID});
                return ds.Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qAllPlayers", e);
                ex.AddAdditionalInformation("userID", userID);
                throw ex;
            }
        }

        public static int ChangeGlobalPlayerName(Guid userID, string newName)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                return (int)db.ExecuteDataSet("uRenameUser", new object[] { userID, newName }).Tables[0].Rows[0][0];
            
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uRenameUser", e);
                ex.AddAdditionalInformation("userID", userID);
                ex.AddAdditionalInformation("newName", newName);
                throw ex;
            }
        }

        public static DataSet AvatarData(Guid userID)
        {

            Database db;
            DataSet ds = null;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                ds = db.ExecuteDataSet("qGetUserAvatarData", new object[] { userID });
                return ds;

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qGetUserAvatarData", e);
                throw ex;
            }
        
        }

        public static int SetAvatarID(Guid userID, int avatarID)
        {
            Database db;
            try
            {

                object ret = null;
                db = DatabaseFactory.CreateDatabase();
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("uUserSetAvatarID");
                db.AddInParameter(cmd, "@UserID", System.Data.DbType.Guid, userID);
                db.AddInParameter(cmd, "@AvatarID", System.Data.DbType.Int32, avatarID);
                db.AddOutParameter(cmd, "@Result", System.Data.DbType.Int32, int.MaxValue);
                db.ExecuteNonQuery(cmd);
                ret = cmd.Parameters["@Result"].Value;
                return (int)ret;

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uUserSetAvatarID", e);
                ex.AddAdditionalInformation("userID", userID);
                ex.AddAdditionalInformation("avatarID", avatarID);
                throw ex;
            }
        }

        public static void UnlockUserAvatarByAvatarID(Guid userID, int avatarID)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                db.ExecuteDataSet("uUserUnlockAvatarID", new object[] { userID, avatarID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uUserUnlockAvatarID", e);
                ex.AddAdditionalInformation("userID", userID);
                ex.AddAdditionalInformation("avatarID", avatarID);
                throw ex;
            }
        }

        public static void SetSex(Guid userID, int sex)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                db.ExecuteDataSet("uUserSetSex", new object[] { userID, sex });
            
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uUserSetSex", e);
                ex.AddAdditionalInformation("userID", userID);
                ex.AddAdditionalInformation("sex", sex);
                throw ex;
            }
        }
        
        public static DataTable GetPlayerListSettings(Guid userID)
        {
            Database db;
            DataSet ds = null;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                ds = db.ExecuteDataSet("qtr_PlayerListSettings", new object[] { userID });
                return ds.Tables[0];

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qtr_PlayerListSettings", e);
                ex.AddAdditionalInformation("userID", userID);
                throw ex;
            }
        }

        public static void SavePlayerListSetting(Guid userID, int playerID, int statusID)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                db.ExecuteNonQuery("utr_PlayerListSettings", new object[] { userID, playerID, statusID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling utr_PlayerListSettings", e);
                ex.AddAdditionalInformation("userID", userID);
                throw ex;
            }
        }

        #region chat1
        //called on server start and anytime blocking/unblocking occurs
        public static ConcurrentDictionary<string, ConcurrentDictionary<Guid, ConcurrentHashSet<string>>> GetBlockedUsers(string realmId = null)
        {
            Database db;
            DataRowCollection collection;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                collection = DB.ExecuteDataSet(db, "qAllBlockedUsers", new object[] { realmId }).Tables[0].Rows;
                ConcurrentDictionary<string, ConcurrentDictionary<Guid, ConcurrentHashSet<string>>> results = new ConcurrentDictionary<string, ConcurrentDictionary<Guid, ConcurrentHashSet<string>>>();

                if (collection.Count > 0)
                {
                    DataRow firstRow = collection[0];
                    string currRealmId = firstRow["RealmID"].ToString();
                    string currId = firstRow["UserId"].ToString();
                    ConcurrentDictionary<Guid, ConcurrentHashSet<string>> currUsers = new ConcurrentDictionary<Guid, ConcurrentHashSet<string>>();
                    ConcurrentHashSet<string> currUserList = new ConcurrentHashSet<string> { firstRow["BlockedPlayerName"].ToString() };
                    for (int i = 1; i < collection.Count; i++)
                    {
                        DataRow row = collection[i];
                        string thisRealmId = row["RealmID"].ToString();
                        string thisId = row["UserId"].ToString();

                        if (currRealmId != thisRealmId)
                        {
                            currUsers[Guid.Parse(currId)] = currUserList;
                            results.TryAdd(string.IsNullOrWhiteSpace(currRealmId) ? "GlobalChat" : currRealmId, currUsers);
                            currUsers = new ConcurrentDictionary<Guid, ConcurrentHashSet<string>>();
                            currUserList = new ConcurrentHashSet<string>();
                            currRealmId = thisRealmId;
                            currId = thisId;
                        }

                        if (currId.ToString() != thisId)
                        {
                            currUsers.TryAdd(Guid.Parse(currId), currUserList);
                            currUserList = new ConcurrentHashSet<string>();
                            currId = thisId;
                        }
                        currUserList.Add(row["BlockedPlayerName"].ToString());
                    }
                    currUsers.TryAdd(Guid.Parse(currId), currUserList);
                    results.TryAdd(string.IsNullOrWhiteSpace(currRealmId) ? "GlobalChat" : currRealmId, currUsers);
                }
                else
                {
                    results.TryAdd("GlobalChat", new ConcurrentDictionary<Guid, ConcurrentHashSet<string>>());  //just to make sure there is an empty entry in the second dictionary for ease of null checks
                }

                return results;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qAllBlockedFromUsers", e);
                ex.AddAdditionalInformation("realmID", realmId);
                throw ex;
            }
        }

        //called whenever user logins
        public static ConcurrentDictionary<string, string> GetPlayerNames(Guid userId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                DataRowCollection collection = DB.ExecuteDataSet(db, "qPlayerNames", new object[] { userId }).Tables[0].Rows;
                ConcurrentDictionary<string, string> playerNames = new ConcurrentDictionary<string, string>();
                for (int i = 0; i < collection.Count; i++)
                {
                    DataRow row = collection[i];
                    playerNames.TryAdd(row["RealmID"].ToString(), row["Name"].ToString()); 
                }
                return playerNames;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayerNameByUserAndRealm", e);
                ex.AddAdditionalInformation("userID", userId);
                throw ex;
            }
        }

        //called when blocking/unblocking user
        public static void BlockUser(Guid userId, string name, string realmId, bool toBlock)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                DB.ExecuteNonQuery(db, toBlock ? "iBlockUser":"dUnblockUser", new object[] { userId, name, ToNullableInt(realmId) });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iBlockUser or dUnblockUser", e);
                ex.AddAdditionalInformation("user", name);
                ex.AddAdditionalInformation("toBlock", toBlock);
                throw ex;
            }
        }

        //called when initiating a 1on1 chat
        public static DataRow GetOneOnOneChat(Guid userId, string name, string realmId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                return DB.ExecuteDataSet(db, "qChat_OneOnOne", new object[] { userId, name, ToNullableInt(realmId) }).Tables[0].Rows[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qChat_OneOnOne", e);
                ex.AddAdditionalInformation("userId", userId);
                ex.AddAdditionalInformation("user to chat", name);
                ex.AddAdditionalInformation("realm", realmId);
                throw ex;
            }
        }

        //called when sending a message
        public static void SaveMessage(Guid userId, string message, DateTime date, string realmId, string groupId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                Guid realGroupId;
                DB.ExecuteScalar(db, "iChat_SaveMessage", new object[] { userId, message, date.ToString(), ToNullableInt(realmId), ToNullableGuid(groupId) });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iChat_SaveMessage'", e);
                ex.AdditionalInformation.Add("userId", userId.ToString());
                ex.AdditionalInformation.Add("message", message);
                ex.AdditionalInformation.Add("realmId", realmId);
                throw ex;
            }
        }
        


        /// <summary>
        /// returns the updateOn date for the flag or NULL if flag not set
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="playerID"></param>
        /// <param name="flagID"></param>
        /// <returns></returns>
        public static void GetFlag(Guid userID, int flagID, out string data, out object setOn)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                DataSet ds = db.ExecuteDataSet("qUserFlag", new object[]
                    {
                        userID
                        , flagID
                    });

                if (ds.Tables[0].Rows.Count == 0)
                {
                    data = null;
                    setOn = null;
                }
                else
                {
                    data = ds.Tables[0].Rows[0][0] is DBNull ? null : (string)ds.Tables[0].Rows[0][0];
                    setOn = ds.Tables[0].Rows[0][1];
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qUserFlag", e);
                ex.AdditionalInformation.Add("flagID", flagID.ToString());
                ex.AdditionalInformation.Add("userID", userID.ToString());
                throw ex;
            }


        }

        //called when loading older msgs or opening a chat (calls older msgs once)
        public static DataRowCollection GetMessages(string realmId, string groupId, DateTime lastMessageDate, int numMessages = 50)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                //int? realRealmId = realmId == -1 ? (int?)null : realmId;
                return DB.ExecuteDataSet(db, "qChat_PreviousChatMessages", new object[] { lastMessageDate, numMessages, ToNullableInt(realmId), ToNullableGuid(groupId) }).Tables[0].Rows;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'qChat_PreviousChatMessages'", e);
                ex.AdditionalInformation.Add("lastMessageDate", lastMessageDate.ToString());
                ex.AdditionalInformation.Add("numMessages", numMessages.ToString());
                ex.AdditionalInformation.Add("realmId", realmId);
                throw ex;
            }
        }

        //called on login
        public static DataRowCollection GetAllUnreadOneOnOneChats(Guid userId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return DB.ExecuteDataSet(db, "qChat_AllUnreadOneOnOneChats", new object[] { userId }).Tables[0].Rows;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'qChat_AllUnreadOneOnOneChats'", e);
                ex.AdditionalInformation.Add("userId", userId.ToString());
                throw ex;
            }
        }

        //called when opening realm profile or the shield
        public static Tuple<DateTime, int> GetNotifications(Guid userId, string realmId, string groupId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
               // int? realRealmId = realmId == -1 ? (int?)null : realmId;
                DataRow dr = DB.ExecuteDataSet(db, "qChat_Notifications", new object[] { userId, ToNullableInt(realmId), ToNullableGuid(groupId) }).Tables[0].Rows[0];
                return Tuple.Create((DateTime)dr["LastSeenDate"], (int)dr["Notifications"]);
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'qChat_Notifications'", e);
                ex.AdditionalInformation.Add("userId", userId.ToString());
                ex.AdditionalInformation.Add("realmId", realmId);
                throw ex;
            }
        }

        //called whenever a chat becomes active, or an active chat becomes inactive
        public static void UpdateLastSeenMsg(Guid userId, string realmId, string groupId, DateTime lastSeen)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                DB.ExecuteNonQuery(db, "uChat_LastSeenDate", new object[] { userId, ToNullableInt(realmId), ToNullableGuid(groupId), lastSeen });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uChat_LastSeenDate'", e);
                ex.AdditionalInformation.Add("userId", userId.ToString());
                ex.AdditionalInformation.Add("realmId", realmId);
                ex.AdditionalInformation.Add("lastSeen", lastSeen.ToString());
                throw ex;
            }
        }
        #endregion

        #region chat2  
        //called on server start to load all these chats
        /// <summary>
        /// gets all global, realm, and clan chats
        /// each row has GroupId, Name, RealmID, ClanID, GroupType
        /// </summary>
        /// <returns></returns>
        public static DataRowCollection GetAllGlobalAndClanChats()
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return DB.ExecuteDataSet(db, "qChat_AllGlobalAndClanChats").Tables[0].Rows;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qChat_AllGlobalAndClanChats", e);
                throw ex;
            }
        }

        
        /// <summary>
        /// Gets all blocked users either for a specific player, or everyone
        /// each row has UserId, PlayerId, BlockedUserId, and BlockedPlayerId
        /// called on server start to get all blocked users and id will be null
        /// also called anytime blocking/unblocking occurs and id will be id of a specific player
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ConcurrentDictionary<Guid, ConcurrentDictionary<string, ConcurrentHashSet<string>>> GetBlockedUsers2(string id = null)
        {
            Database db;
            DataRowCollection collection;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                collection = DB.ExecuteDataSet(db, "qAllBlockedUsers2", new object[] { ToNullableGuid(id), ToNullableInt(id) }).Tables[0].Rows;
                ConcurrentDictionary<Guid, ConcurrentDictionary<string, ConcurrentHashSet<string>>> results = new ConcurrentDictionary<Guid, ConcurrentDictionary<string, ConcurrentHashSet<string>>>();

                if (collection.Count > 0)
                {
                    DataRow firstRow = collection[0];
                    string currPlayerId = firstRow["PlayerId"].ToString();
                    Guid currId = (Guid)firstRow["UserId"];
                    ConcurrentDictionary<string, ConcurrentHashSet<string>> currUsers = new ConcurrentDictionary<string, ConcurrentHashSet<string>>();
                    ConcurrentHashSet<string> currUserList = new ConcurrentHashSet<string> ();
                    string blockedId = firstRow["BlockedPlayerId"].ToString();
                    currUserList.Add(string.IsNullOrWhiteSpace(blockedId) ? firstRow["BlockedUserId"].ToString() : blockedId);

                    for (int i = 1; i < collection.Count; i++)
                    {
                        DataRow row = collection[i];
                        string thisPlayerId = row["PlayerId"].ToString();
                        Guid thisId = (Guid)row["UserId"];

                        if (currId != thisId)
                        {
                            currUsers.TryAdd(string.IsNullOrWhiteSpace(currPlayerId) ? currId.ToString() : currPlayerId, currUserList);
                            results.TryAdd(currId, currUsers);
                            currUsers = new ConcurrentDictionary<string, ConcurrentHashSet<string>>();
                            currUserList = new ConcurrentHashSet<string>();
                            currPlayerId = thisPlayerId;
                            currId = thisId;
                        }

                        if (currPlayerId != thisPlayerId)
                        {
                            currUsers.TryAdd(currPlayerId, currUserList);
                            currUserList = new ConcurrentHashSet<string>();
                            currId = thisId;
                        }

                        blockedId = row["BlockedPlayerId"].ToString();
                        currUserList.Add(string.IsNullOrWhiteSpace(blockedId) ? row["BlockedUserId"].ToString() : blockedId);
                    }

                    currUsers.TryAdd(string.IsNullOrWhiteSpace(currPlayerId) ? currId.ToString() : currPlayerId, currUserList);
                    results.TryAdd(currId, currUsers);
                }
                else
                {
                    ConcurrentDictionary<string, ConcurrentHashSet<string>> emptyDictionary = new ConcurrentDictionary<string, ConcurrentHashSet<string>>();
                    emptyDictionary.TryAdd(Guid.Empty.ToString(), new ConcurrentHashSet<string>());
                    results.TryAdd(Guid.Empty, emptyDictionary);  //just to make sure there is an empty entry in the second dictionary for ease of null checks
                }

                return results;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qAllBlockedUsers2", e);
                ex.AddAdditionalInformation("id", id);
                throw ex;
            }
        }


        /// <summary>
        /// Simpler version of GetBlockedUsers2, only takes in playerId, and contains playername
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public static List<object> GetBlockedPlayersForPlayer(string playerId)
        {
            Database db;
            DataRowCollection collection;

            List<object> results = new List<object>();

            try
            {
                db = DatabaseFactory.CreateDatabase();
                collection = DB.ExecuteDataSet(db, "qAllBlockedUsers2", new object[] { ToNullableGuid(playerId), ToNullableInt(playerId) }).Tables[0].Rows;

                for (int i = 0; i < collection.Count; i++)
                {
                    DataRow row = collection[i];

                    results.Add(new {
                        //UserId = row["UserId"].ToString(),
                        //PlayerId = row["PlayerId"].ToString(),
                        //BlockedUserId = row["BlockedUserId"].ToString(),
                        BlockedPlayerId = row["BlockedPlayerId"].ToString(),
                        BlockedPlayerName = row["Name"].ToString()
                    });
                }


                return results;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling GetBlockedPlayersForPlayer", e);
                ex.AddAdditionalInformation("playerId", playerId);
                throw ex;
            }
        }

        /// <summary>
        /// Gets all chats + playerinfo for a particular client
        /// each row has PlayerID, RealmID, PlayerName, GroupId, ChatName, GroupType
        /// every player of the client is returned even though that player does not have any chats (groupid etc will be null)
        /// called on login
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataSet GetAllPlayerInfos(Guid userId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return DB.ExecuteDataSet(db, "qChat_AllClientPlayerInfos2", new object[] { userId });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qChat_AllClientPlayerInfos2", e);
                ex.AddAdditionalInformation("userID", userId);
                throw ex;
            }
        }

        /// <summary>
        /// called to add other users if exists - for optimizing group/1on1 chats
        /// not called for now because we did not optimize the memory yet
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataRowCollection GetOtherChatPlayerInfos(string groupId, Guid userId, string id)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return DB.ExecuteDataSet(db, "qChat_OtherChatPlayerInfos", new object[] { Guid.Parse(groupId), userId, ToNullableInt(id) }).Tables[0].Rows;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qChat_OtherChatPlayerInfos", e);
                ex.AddAdditionalInformation("groupId", groupId);
                ex.AddAdditionalInformation("userID", userId);
                throw ex;
            }
        }

        //called when blocking/unblocking user
        public static void BlockUser2(Guid userId, string id, string blockId, bool toBlock)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                DB.ExecuteNonQuery(db, toBlock ? "iBlockUser2" : "dUnblockUser2", new object[] { userId, ToNullableInt(id), ToNullableGuid(blockId), ToNullableInt(blockId) });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iBlockUser2 or dUnblockUser2", e);
                ex.AddAdditionalInformation("userId", userId);
                ex.AddAdditionalInformation("id", id);
                ex.AddAdditionalInformation("blockId", blockId);
                ex.AddAdditionalInformation("toBlock", toBlock);
                throw ex;
            }
        }

        /// <summary>
        /// called when initiating a 1on1 chat
        /// rows have GroupId, Name, and OtherUserId
        /// also creates the 1on1 chat if it does not exist
        /// </summary>
        /// <param name="id"></param>
        /// <param name="otherId"></param>
        /// <param name="realmId"></param>
        /// <returns></returns>
        public static DataRow GetOneOnOneChat2(string id, string otherId, string realmId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                return DB.ExecuteDataSet(db, "qChat_OneOnOne2", new object[] { ToNullableGuid(id), ToNullableInt(id), ToNullableGuid(otherId), ToNullableInt(otherId), ToNullableInt(realmId) }).Tables[0].Rows[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qChat_OneOnOne2", e);
                ex.AddAdditionalInformation("id", id);
                ex.AddAdditionalInformation("otherId", otherId);
                throw ex;
            }
        }

        /// <summary>
        /// called when initiating a 1on1 chat from the history
        /// rows have OtherUserId, Name, OtherId
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataRow GetOneOnOneChat2(string groupId, string id)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                return DB.ExecuteDataSet(db, "qChat_OneOnOneByGroupId", new object[] { Guid.Parse(groupId), ToNullableGuid(id), ToNullableInt(id) }).Tables[0].Rows[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qChat_OneOnOneByGroupId", e);
                ex.AddAdditionalInformation("groupId", groupId);
                ex.AddAdditionalInformation("id", id);
                throw ex;
            }
        }

        /// <summary>
        /// called when creating a new group
        /// returns the new groupId generated
        /// </summary>
        /// <param name="name"></param>
        /// <param name="realmId"></param>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string CreateGroupChat(string name, string realmId, Guid userId, string id)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                return DB.ExecuteScalar(db, "iChat_CreateGroup", new object[] { name, ToNullableInt(realmId), userId, ToNullableInt(id) }).ToString();
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iChat_CreateGroup", e);
                ex.AddAdditionalInformation("name", name);
                ex.AddAdditionalInformation("realmId", realmId);
                ex.AddAdditionalInformation("userId", userId.ToString());
                ex.AddAdditionalInformation("id", id);
                throw ex;
            }
        }

        /// <summary>
        /// called to update groupchat users
        /// rows have UserId, Name, Id
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataRowCollection GetGroupChatUsers(string groupId, Guid userId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                return DB.ExecuteDataSet(db, "qChat_GroupChatUsers", new object[] { Guid.Parse(groupId), userId }).Tables[0].Rows;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qChat_GroupChatUsers", e);
                ex.AddAdditionalInformation("groupId", groupId);
                throw ex;
            }
        }

        /// <summary>
        /// Gets the groupID of a private group chat that 2 given playerIDs belong to.
        /// returns top 1 row
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static DataRowCollection GetPrivateChatGroupIdByPlayerId(string playerIdOne, string playerIdTwo)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                //FIX PROBLEM HERE, IF NO ROWS, THROWS ERROR
                //maybe have it return all rows, and use it as collection in chathub, then if row count is 0 then no chat was found
                return DB.ExecuteDataSet(db, "qChat_PrivateChatGroupIdByPlayerId", new object[] { playerIdOne, playerIdTwo }).Tables[0].Rows;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qChat_PrivateChatGroupIdByPlayerId", e);
                ex.AddAdditionalInformation("playerIdOne", playerIdOne);
                ex.AddAdditionalInformation("playerIdTwo", playerIdTwo);
                throw ex;
            }
        }

        //called when adding players to a group
        /// <summary>
        /// Adds list of players to group.
        /// Returns 2 tables: First table holds the list of names that were added. Second Table holds the names that were not added, along with error messages for them
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="names"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataSet AddPlayersToGroup(string groupId, string names, string id)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                return DB.ExecuteDataSet(db, "qChat_AddPlayersToGroup", new object[] { Guid.Parse(groupId), names, ToNullableGuid(id), ToNullableInt(id) });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qChat_AddPlayersToGroup", e);
                ex.AddAdditionalInformation("groupId", groupId);
                ex.AddAdditionalInformation("names", names);
                ex.AddAdditionalInformation("id", id);
                throw ex;
            }
        }

        //called when player kicks another user
        /// <summary>
        /// Kicks a user from a chat. Returns UserId of kicked person
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public static string KickUser(string groupId, string targetId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                return DB.ExecuteScalar(db, "dChat_KickUser", new object[] { Guid.Parse(groupId), ToNullableGuid(targetId), ToNullableInt(targetId) }).ToString();
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dChat_KickUser", e);
                ex.AddAdditionalInformation("groupId", groupId);
                ex.AddAdditionalInformation("targetId", targetId);
                throw ex;
            }
        }

        //called in login when clan chat does not exist yet in the server
        /// <summary>
        /// Gets the corresponding clan chat. If the chat does not exist, it creates a new row and grabs it
        /// returns groupId of clanchat
        /// </summary>
        /// <param name="realmId"></param>
        /// <param name="clanId"></param>
        /// <returns></returns>
        public static string GetClanChat(string realmId, int clanId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                //this grabs corresponding clan chat and inserts a new row if chat does not exist yet
                return DB.ExecuteScalar(db, "qChat_GetClanChat", new object[] { int.Parse(realmId), clanId }).ToString();
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qChat_GetClanChat", e);
                ex.AddAdditionalInformation("realmId", realmId);
                ex.AddAdditionalInformation("clanId", clanId);
                throw ex;
            }
        }

        //called when player disbands a clan
        //returns groupId of clanchat
        //public static void DeleteClanChat(int realmId, int clanId)
        //{
        //    Database db;

        //    try
        //    {
        //        db = DatabaseFactory.CreateDatabase();
        //        //only deletes userstochats rows
        //        //not sure if we want to persist prev clan chat msgs so it does not delete the actual group for now
        //        DB.ExecuteNonQuery(db, "dChat_ClanChat", new object[] { realmId, clanId }).ToString();
        //    }
        //    catch (Exception e)
        //    {
        //        BaseApplicationException ex = new BaseApplicationException("Error while calling dChat_ClanChat", e);
        //        ex.AddAdditionalInformation("realmId", realmId);
        //        ex.AddAdditionalInformation("clanId", clanId);
        //        throw ex;
        //    }
        //}

        //called after deleting clan chat just to make sure the deletion was successful, and user is not trying to hack system
        /// <summary>
        /// Checks to make sure if there is a groupchat2 row for this clan
        /// </summary>
        /// <param name="realmId"></param>
        /// <param name="clanId"></param>
        /// <returns></returns>
        public static bool DoesClanChatExist(string realmId, int clanId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                //only deletes userstochats rows
                //not sure if we want to persist prev clan chat msgs so it does not delete the actual group for now
                return (bool)DB.ExecuteScalar(db, "qChat_DoesClanChatExist", new object[] { int.Parse(realmId), clanId });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qChat_DoesClanChatExist", e);
                ex.AddAdditionalInformation("realmId", realmId);
                ex.AddAdditionalInformation("clanId", clanId);
                throw ex;
            }
        }

        /// <summary>
        /// Kicks a player from a clan chat
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="realmId"></param>
        /// <param name="clanId"></param>
        public static void DismissPlayerFromClanChat(int playerId, int realmId, int clanId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                //only deletes userstochats rows
                //not sure if we want to persist prev clan chat msgs so it does not delete the actual group for now
                DB.ExecuteNonQuery(db, "dChat_UsersToChats2", new object[] { playerId, realmId, clanId }).ToString();
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dChat_UsersToChats2", e);
                ex.AddAdditionalInformation("playerId", playerId);
                ex.AddAdditionalInformation("clanId", clanId);
                throw ex;
            }
        }

        //called after player is dismissed just to make sure the deletion was successful, and user is not trying to hack system
        /// <summary>
        /// Determines if a player is actually in a clan chat or not
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="realmId"></param>
        /// <param name="clanId"></param>
        /// <returns></returns>
        public static bool IsPlayerInClanChat(string playerId, string realmId, int clanId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                //only deletes userstochats rows
                //not sure if we want to persist prev clan chat msgs so it does not delete the actual group for now
                return (bool)DB.ExecuteScalar(db, "qChat_IsPlayerInClanChat", new object[] { int.Parse(playerId), int.Parse(realmId), clanId });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qChat_IsPlayerInClanChat", e);
                ex.AddAdditionalInformation("playerId", playerId);
                ex.AddAdditionalInformation("realmId", realmId);
                ex.AddAdditionalInformation("clanId", clanId);
                throw ex;
            }
        }

        //called when sending a message
        /// <summary>
        /// Saves a chat msg.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="playerId"></param>
        /// <param name="message"></param>
        /// <param name="date"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static void SaveMessage(Guid? userId, string playerId, string message, DateTime date, string groupId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                DB.ExecuteNonQuery(db, "iChat_SaveMessage2", new object[] { userId, ToNullableInt(playerId), message, date, Guid.Parse(groupId) });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iChat_SaveMessage'", e);
                ex.AdditionalInformation.Add("userId", userId.ToString());
                ex.AdditionalInformation.Add("playerId", playerId);
                ex.AdditionalInformation.Add("message", message);
                ex.AdditionalInformation.Add("groupId", groupId);
                throw ex;
            }
        }

        //called when loading older msgs or opening a chat (calls older msgs once)
        /// <summary>
        /// Loads messages with default numMessages = 50. Returns rows that have UserId, PlayerId, Name, Text, Datetime
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="lastMessageDate"></param>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <param name="numMessages"></param>
        /// <returns></returns>
        public static DataRowCollection GetMessages(string groupId, DateTime lastMessageDate, Guid userId, string id, int numMessages = 50)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                //int? realRealmId = realmId == -1 ? (int?)null : realmId;
                return DB.ExecuteDataSet(db, "qChat_PreviousChatMessages2", new object[] { lastMessageDate, numMessages, Guid.Parse(groupId), userId, ToNullableInt(id) }).Tables[0].Rows;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'qChat_PreviousChatMessages2'", e);
                ex.AdditionalInformation.Add("lastMessageDate", lastMessageDate.ToString());
                ex.AdditionalInformation.Add("numMessages", numMessages.ToString());
                ex.AdditionalInformation.Add("groupId", groupId);
                ex.AdditionalInformation.Add("userId", userId.ToString());
                ex.AdditionalInformation.Add("id", id);
                throw ex;
            }
        }

        //called when opening realm profile or the shield
        /// <summary>
        /// Grabs notifications along with last seen msg for a player in a chat. Also inserts a new row if the userstochat2 row does not exist
        /// Returned rows have LastSeenDate, and Notifications
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public static Tuple<DateTime, int> GetNotifications2(string groupId, Guid userId, string playerId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                DataRow dr = DB.ExecuteDataSet(db, "qChat_Notifications2", new object[] { Guid.Parse(groupId), userId, ToNullableInt(playerId) }).Tables[0].Rows[0];
                return Tuple.Create((DateTime)dr["LastSeenDate"], (int)dr["Notifications"]);
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'qChat_Notifications2'", e);
                ex.AdditionalInformation.Add("userId", userId.ToString());
                ex.AdditionalInformation.Add("groupId", groupId);
                ex.AdditionalInformation.Add("playerId", playerId);
                throw ex;
            }
        }

        //called whenever a chat becomes active, or an active chat becomes inactive
        /// <summary>
        /// Updates the last seen date for a userstochats2 row
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="playerId"></param>
        /// <param name="groupId"></param>
        /// <param name="lastSeen"></param>
        public static void UpdateLastSeenMsg2(Guid userId, string playerId, string groupId, DateTime lastSeen)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                DB.ExecuteNonQuery(db, "uChat_LastSeenDate2", new object[] { userId, ToNullableInt(playerId), ToNullableGuid(groupId), lastSeen });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uChat_LastSeenDate2'", e);
                ex.AdditionalInformation.Add("userId", userId.ToString());
                ex.AdditionalInformation.Add("playerId", playerId);
                ex.AdditionalInformation.Add("lastSeen", lastSeen.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// Gets all chats for a specific player. If id is userid, it grabs all chats for all players of the user, since the user is in TR.
        /// rows have GroupId, PlayerId, RealmID, PlayerName, ChatName, Notifications, LastMsgDate, GroupType. Also, they are ordered by descending datetime,
        /// so in the chatlist, the top rows are the most recent chats
        /// </summary>
        /// <param name="id">PlayerId of player, or UserId of user</param>
        /// <returns></returns>
        public static DataRowCollection GetAllChats(string id)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return DB.ExecuteDataSet(db, "qChat_AllChats", new object[] { ToNullableGuid(id), ToNullableInt(id) }).Tables[0].Rows;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'qChat_AllChats'", e);
                ex.AdditionalInformation.Add("id", id);
                throw ex;
            }
        }

        /// <summary>
        /// Autocompletes a player or GlobalPlayer name. Rows returned just contians the full name
        /// </summary>
        /// <param name="term"></param>
        /// <param name="realmId"></param>
        /// <returns></returns>
        public static DataRowCollection PlayerAutocomplete(string term, string realmId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return DB.ExecuteDataSet(db, "qPlayerAutocomplete", new object[] { term, ToNullableInt(realmId) }).Tables[0].Rows;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'qPlayerAutocomplete'", e);
                ex.AdditionalInformation.Add("term", term);
                ex.AdditionalInformation.Add("realmId", realmId);
                throw ex;
            }
        }

        /// <summary>
        /// Changes a group name
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="name"></param>
        public static void ChangeGroupName(string groupId, string name)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                DB.ExecuteNonQuery(db, "uChat_ChangeGroupName", new object[] { Guid.Parse(groupId), name });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'uChat_ChangeGroupName'", e);
                ex.AdditionalInformation.Add("groupId", groupId);
                ex.AdditionalInformation.Add("name", name);
                throw ex;
            }
        }

        /// <summary>
        /// Gets playerId or userId by the name and realmId
        /// </summary>
        /// <param name="name"></param>
        /// <param name="realmId"></param>
        /// <returns></returns>
        public static string GetIdByNameAndRealm(string name, string realmId)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return DB.ExecuteScalar(db, "qIdByNameAndRealm", new object[] { name, ToNullableInt(realmId) }).ToString();
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'qIdByNameAndRealm'", e);
                ex.AdditionalInformation.Add("name", name);
                ex.AdditionalInformation.Add("realmId", realmId);
                throw ex;
            }
        }
        #endregion

        private static int? ToNullableInt(string s)
        {
            int i;
            return Int32.TryParse(s, out i) ? i : (int?)null; 
        }

        private static Guid? ToNullableGuid(string g)
        {
            Guid guid;
            return Guid.TryParse(g, out guid) ? guid : (Guid?)null;
        }

        public class ConcurrentHashSet<T> : IEnumerable<T>, ISet<T>, ICollection<T>
        {
            private readonly ConcurrentDictionary<T, byte> set;

            public ConcurrentHashSet()
            {
                set = new ConcurrentDictionary<T, byte>();
            }

            public bool TryRemove(T item)
            {
                byte val;
                return set.TryRemove(item, out val);
            }

            public bool TryAdd(T item)
            {
                return set.TryAdd(item, default(byte));
            }

            public bool Add(T item)
            {
                return TryAdd(item);
            }

            public bool Remove(T item)
            {
                return TryRemove(item);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return set.Keys.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int Count
            {
                get { return set.Count; }
            }


            public void ExceptWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public void IntersectWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSubsetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSupersetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSubsetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSupersetOf(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool Overlaps(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public bool SetEquals(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public void SymmetricExceptWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            public void UnionWith(IEnumerable<T> other)
            {
                throw new NotImplementedException();
            }

            void ICollection<T>.Add(T item)
            {
                TryAdd(item);
            }

            public void Clear()
            {
                set.Clear();
            }

            public bool Contains(T item)
            {
                return set.ContainsKey(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                set.Keys.CopyTo(array, arrayIndex);   
            }

            public bool IsReadOnly
            {
                get { return false; }
            }
        }

        public static int GetThroneRoomLikes(Guid? userid, int? playerID)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return (int)DB.ExecuteDataSet(db, "qThroneRoomLike", new object[] { userid, playerID }).Tables[0].Rows[0][0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'qThroneRoomLike'", e);
                ex.AddAdditionalInformation("userId", userid);
                ex.AddAdditionalInformation("playerID", playerID);              
                throw ex;
            }
        }

        public static void  SetThroneRoomLike(Guid? userid, int? playerID, string IP)
        {

            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                DB.ExecuteNonQuery(db, "iThroneRoomLike", new object[] {IP, userid, playerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'iThroneRoomLike'", e);
                ex.AddAdditionalInformation("userId", userid);
                ex.AddAdditionalInformation("playerID", playerID);
                ex.AddAdditionalInformation("IP", IP);
                throw ex;
            }
        }

        public static DataTable GetAnyEventDeletedPlayerInfo(int? playerID, string playerName)
        {

            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return DB.ExecuteDataSet(db, "qvPlayers", new object[] { playerID, playerName }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling 'qvPlayers'", e);
            
                ex.AddAdditionalInformation("playerID", playerID);
                ex.AddAdditionalInformation("playerName", playerName);
                throw ex;
            }
        }


        public static int Items2_SetAsUsed(Guid userID, int itemID)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                return (int)DB.ExecuteDataSet(db,"Items2_dItem", new object[] { userID, itemID}).Tables[0].Rows[0][0];

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling Items2_dItem", e);
                ex.AddAdditionalInformation("userID", userID);
                ex.AddAdditionalInformation("itemID", itemID);
                throw ex;
            }
        }


        public static DataTable Items2_GetItems(Guid userID)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                return DB.ExecuteDataSet(db, "Items2_q", new object[] { userID, null }).Tables[0];

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qItems2", e);
                ex.AddAdditionalInformation("userID", userID);
                throw ex;
            }
        }


        public static DataTable Items2_GetItem(Guid userID, long itemID)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                return DB.ExecuteDataSet(db, "Items2_q", new object[] { userID, itemID }).Tables[0];

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qItems2", e);
                ex.AddAdditionalInformation("userID", userID);
                throw ex;
            }
        }

        public static void Items2_MarkAsUsed(Guid userID, long itemID)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteNonQuery(db, "Items2_dItem", new object[] { userID, itemID });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling Items2_dItem", e);
                ex.AddAdditionalInformation("userID", userID);
                ex.AddAdditionalInformation("itemID", itemID);
                throw ex;
            }
        }

        public static void Items2_GiveItem(int? expiresInHours, Guid userid, int? playerID, int unitTypeID, int amount)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteNonQuery(db, "Items2_iItem_Troops", new object[] {  expiresInHours, userid,  playerID,  unitTypeID,  amount });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling Items2_iItem_Troops", e);
                throw ex;
            }
        }

        public static void Items2_GiveItem(int? expiresInHours, Guid userid, int? playerID, int amount)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                DB.ExecuteNonQuery(db, "Items2_iItem_Silver", new object[] { expiresInHours, userid, playerID, amount });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling Items2_iItem_Silver", e);
                throw ex;
            }
        }

        public static void Items2_GiveItem_pfwithduration(int? expiresInHours, Guid userid, int? pplayerID, int pfPackageID, int durationInMinutes)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteNonQuery(db, "Items2_iItem_PFWithDuration", new object[] { expiresInHours, userid, pplayerID, pfPackageID, durationInMinutes });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling Items2_iItem_PFWithDuration", e);
               
                throw ex;
            }
        }
        public static void Items2_GiveItem_BuildingSpeedup(int? expiresInHours, Guid userid, int? pplayerID, int amountInMinutes)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteNonQuery(db, "Items2_iItem_BuildingSpeedup", new object[] { expiresInHours, userid, pplayerID, amountInMinutes });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling Items2_iItem_BuildingSpeedup", e);

                throw ex;
            }
        }
        public static void Items2_GiveItem_ResearchSpeedup(int? expiresInHours, Guid userid, int? pplayerID, int amountInMinutes)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteNonQuery(db, "Items2_iItem_ResearchSpeedup", new object[] { expiresInHours, userid, pplayerID, amountInMinutes });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling Items2_iItem_ResearchSpeedup", e);

                throw ex;
            }
        }
    }
}
