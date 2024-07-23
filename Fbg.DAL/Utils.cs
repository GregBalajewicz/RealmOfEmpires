using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;
namespace Fbg.DAL
{
    public class utils
    {
        public static DataSet InvestigateFakeAccount(string connectionStr, string fakePlayerName, string activePlayerName,int daysOld, int realmID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);;

                return db.ExecuteDataSet("qFakeAccountInvestigation", new object[] { activePlayerName, fakePlayerName, daysOld, realmID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qFakeAccountInvestigation", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("fakePlayerName", fakePlayerName.ToString());
                ex.AdditionalInformation.Add("activePlayerName", activePlayerName.ToString());
                ex.AdditionalInformation.Add("daysOld", daysOld.ToString());
                throw ex;
            }
            finally
            {
            }

        }
        public static DataSet InvestigateFakeAccountDetails(string connectionStr, string fakePlayerName, string activePlayerName, int daysOld, string selectedPlayerName, int selectedVillageID, int selectedReport, int realmID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return db.ExecuteDataSet("qFakeAccountInvestigationDetails", new object[] { activePlayerName, fakePlayerName, daysOld, selectedPlayerName,selectedVillageID, selectedReport, realmID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qFakeAccountInvestigation", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("fakePlayerName", fakePlayerName.ToString());
                ex.AdditionalInformation.Add("activePlayerName", activePlayerName.ToString());
                ex.AdditionalInformation.Add("selectedPlayerName", selectedPlayerName.ToString());
                ex.AdditionalInformation.Add("selectedReport", selectedReport.ToString());
                throw ex;
            }
            finally
            {
            }

        }


        public static DataSet InvestigateFakeAccount_commonattacks(string connectionStr, string playerA, string playerB, int daysOld)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);

                return db.ExecuteDataSet("qFakeAccountInvestigation_commonattacks", new object[] { playerA, playerB, daysOld });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qFakeAccountInvestigation", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("daysOld", daysOld.ToString());
                throw ex;
            }
            finally
            {
            }

        }

        public static bool LogLogin(string connectionStr, int playerID , string remoteAddress, long port, string agent)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return Convert.ToBoolean(db.ExecuteScalar("iLoginLog"
                    , new object[] { 
                        playerID
                        , remoteAddress
                        , port
                        , agent}));
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling LogLogin", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("playerID", playerID);
                ex.AddAdditionalInformation("remoteAddress", remoteAddress);
                ex.AddAdditionalInformation("port", port);
                ex.AddAdditionalInformation("agent", agent);
                throw ex;
            }
            finally
            {
            }

        }

        

        public static int Admin_BanPlayer(string adminPlayerName, Guid  userID, string publicMsg, string notes, int? hoursTillExpiry)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return (int)db.ExecuteDataSet("iBanPlayer", new object[] { userID , adminPlayerName, publicMsg, notes, hoursTillExpiry }).Tables[0].Rows[0][0];

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling Admin_BanPlayer", e);
                ex.AddAdditionalInformation("adminPlayerName", adminPlayerName);
                ex.AddAdditionalInformation("userID", userID);
                ex.AddAdditionalInformation("publicMsg", publicMsg);
                ex.AddAdditionalInformation("notes", notes);
                throw ex;

            }

        }
        public static DataTable  Admin_GetBanPlayerInfo( string planerNameToBan)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return db.ExecuteDataSet("qBanPlayerInfo", new object[] { planerNameToBan,}).Tables[0];

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling Admin_GetBanPlayerInfo", e);
                ex.AddAdditionalInformation("planerNameToBan", planerNameToBan);
                throw ex;

            }

        }
        public static DataTable Admin_FindUser(string playerName, string email, string username ,string globalName)
        {
           
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                return db.ExecuteDataSet("qUserAdmin", new object[] { playerName, email, username, globalName }).Tables[0];

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling Admin_FindUser", e);
                ex.AddAdditionalInformation("playerName", playerName);
                throw ex;

            }
        }
        //public static DataTable Admin_FindUserByEmail(string email)
        //{

        //    Database db;
        //    try
        //    {
        //        db = DatabaseFactory.CreateDatabase();
        //        return db.ExecuteDataSet("qUserByEmail", new object[] { email }).Tables[0];

        //    }
        //    catch (Exception e)
        //    {
        //        BaseApplicationException ex = new BaseApplicationException("Error while calling Admin_FindUserByEmail", e);
        //        ex.AddAdditionalInformation("email", email);
        //        throw ex;

        //    }
        //}
        public static int Admin_UnBanPlayer( Guid UserID)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return (int)db.ExecuteScalar("dUnBanPlayer", new object[] { UserID});

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling Admin_UnBanPlayer", e);
                ex.AddAdditionalInformation("UserID", UserID.ToString());

                throw ex;

            }

        }
        //public static DataTable Admin_FindUserByFBID(string facebookID)
        //{
        //    Database db;
        //    try
        //    {
        //        db = DatabaseFactory.CreateDatabase();
        //        return db.ExecuteDataSet("qUserByFacebookID", new object[] { facebookID }).Tables[0];

        //    }
        //    catch (Exception e)
        //    {
        //        BaseApplicationException ex = new BaseApplicationException("Error while calling Admin_FindUser", e);
        //        ex.AddAdditionalInformation("facebookID", facebookID.ToString());
        //        throw ex;

        //    }
        //}
        public static bool Admin_GiveServants(Guid UserID, int ServantsAmount, int eventType)
        {
            Database db;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                int ret = (int)db.ExecuteScalar("uGiveServants", new object[] { UserID, ServantsAmount, eventType });
               if (ret == 0)
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
                BaseApplicationException ex = new BaseApplicationException("Error while calling Admin_GiveServants", e);
                ex.AddAdditionalInformation("UserID", UserID.ToString());
                ex.AddAdditionalInformation("ServantsAmount", ServantsAmount.ToString());
                ex.AddAdditionalInformation("eventType", eventType.ToString());

                throw ex;

            }
        }
        public static DataTable  Admin_GetPlayersByIP(string connectionStr, string ip)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return db.ExecuteDataSet("qPlayersByIP"
                    , new object[] { ip }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayersByIP", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("ip", ip);

                throw ex;
            }
            finally
            {
            }

        }


        public static int Admin_Cofiscate(string connectionStr, string planerNameToBan)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return (int)db.ExecuteDataSet("iBanPlayer"
                    , new object[] {planerNameToBan }).Tables[0].Rows[0][0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iBanPlayer", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("planerNameToBan", planerNameToBan);
                throw ex;
            }
            finally
            {
            }

        }


        public static int RecordFbCreditTransaction(long orderId, string FbUserId, int price, int servants, float? amount, string currency)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return (int)DB.ExecuteScalar(db, "iPaymentFbCreditTransaction", new object[] { 
                    orderId, FbUserId, price, servants, amount, currency });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iPaymentFbCreditTransaction", e);
                ex.AddAdditionalInformation("orderId", orderId);
                ex.AddAdditionalInformation("fbUserID", FbUserId);
                ex.AddAdditionalInformation("price", price);
                ex.AddAdditionalInformation("servants", servants);
                ex.AddAdditionalInformation("amount", amount);
                ex.AddAdditionalInformation("currency", currency);
                throw ex;
            }
        }

        public static int RecordFbCreditTransaction(long orderId, string FbUserId, int price, int servants)
        {
            return RecordFbCreditTransaction(orderId, FbUserId, price, servants, null, null);
        }

           public static int RecordKongregateCreditTransaction(string kongreagateUserID, string packageID, string kongregateItemID, string kongregateUsageID, string rawListOfItems, string rawThisItem)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return (int)DB.ExecuteScalar(db, "iPaymentKongregateCreditTransaction", new object[] { 
                    packageID, kongreagateUserID, kongregateItemID, kongregateUsageID, rawThisItem, rawListOfItems});
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iPaymentKongregateCreditTransaction", e);
                ex.AddAdditionalInformation("kongreagateUserID", kongreagateUserID);
                ex.AddAdditionalInformation("packageID", packageID);
                ex.AddAdditionalInformation("kongregateItemID", kongregateItemID);
                ex.AddAdditionalInformation("kongregateUsageID", kongregateUsageID);
                ex.AddAdditionalInformation("rawThisItem", rawThisItem);
                ex.AddAdditionalInformation("rawListOfItems", rawListOfItems);
                throw ex;
            }
        }



        /// <returns>
        /// 0 - meaning there already is such a transaction and status. 
        /// 1 - meanning there was not such tran & status and it has been added to the log
        /// </returns>
        public static int RecordPaymentTransaction(string tranID, Guid userID, short status, string rawData
            , string payment_status, string payment_date, string parent_txn_id
           , string paymentGross, string paymentFee, string payer_email)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return (int)DB.ExecuteDataSet(db, "iPaymentTransaction", new object[] { 
                    tranID
                    , userID
                    , status
                    , rawData
                    , payment_status
                    , payment_date
                    , parent_txn_id
                    , paymentGross
                    , paymentFee
                    ,payer_email}).Tables[0].Rows[0][0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iPaymentTransaction", e);
                ex.AddAdditionalInformation("tranID", tranID);
                ex.AddAdditionalInformation("userID", userID);
                ex.AddAdditionalInformation("status", status);
                ex.AddAdditionalInformation("rawData", rawData);
                ex.AddAdditionalInformation("payment_status", payment_status);
                ex.AddAdditionalInformation("payment_date", payment_date);
                ex.AddAdditionalInformation("parent_txn_id", parent_txn_id);
                ex.AddAdditionalInformation("paymentGross", paymentGross);
                ex.AddAdditionalInformation("paymentFee", paymentFee);
                ex.AddAdditionalInformation("payer_email", payer_email);
                throw ex;
            }
        }



        /// <returns>
        /// </returns>
        public static DataSet Notifications_GetRecipients(int notificationID, bool isTest)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return db.ExecuteDataSet("qNotificationRecipients", new object[] { (int)notificationID , isTest});
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qNotificationRecipients", e);
                throw ex;
            }
        }

        /// <returns>
        /// </returns>
        public static DataSet Notifications_MarkSuccessfulSend(int notificationID, string facebookIDs)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return db.ExecuteDataSet("iNotificationSent", new object[] { facebookIDs, (int)notificationID});
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iNotificationSent", e);
                throw ex;
            }
        }

        /// <returns>
        /// </returns>
        public static void Stories_StoryPublished(int playerID, int storyID, string body, bool isSuccessful)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                DB.ExecuteNonQuery(db, "iStoryPublished", new object[] { playerID, storyID, body, isSuccessful });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iStoryPublished", e);
                throw ex;
            }
        }


        public static DataSet Admin_GetStats(int s)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                switch (s)
                {
                    case 1:
                        return db.ExecuteDataSet("qAdminStats", new object[] { });
                    default:
                        return db.ExecuteDataSet("qAdminStats" + s.ToString(), new object[] { });

                }
            

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qAdminStats", e);
                throw ex;
            }
        }

        public static DataSet Admin_GetUserInfo(Guid userID)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return db.ExecuteDataSet("qAdminUserInfo", new object[] { userID });

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qAdminUserInfo", e);
                ex.AddAdditionalInformation("userID", userID);
                throw ex;

            }

        }

        public static DataSet Admin_GetPlayerInfo(string connectionStr, int playerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return db.ExecuteDataSet("qAdminPlayerInfo"
                    , new object[] { playerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qAdminPlayerInfo", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("playerID", playerID);
                throw ex;
            }
            finally
            {
            }            
        }

        public static void Admin_ChangeVillageOwnership(string connectionStr, int villageID, int toPlayerID, string newVillageName)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                db.ExecuteNonQuery("uAdmin_ChangeVillageOwnership"
                    , new object[] { villageID, toPlayerID, newVillageName });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uAdmin_ChangeVillageOwnership", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("villageID", villageID);
                ex.AddAdditionalInformation("toPlayerID", toPlayerID);
                ex.AddAdditionalInformation("newVillageName", newVillageName);
                throw ex;
            }
            finally
            {
            }                        
        }

        public static DataTable Admin_GetUsersForInviteReward(int numDaysBack, int numLastRewards, int minNumberOfInvites)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                return db.ExecuteDataSet("qInviteReward_GetUsers"
                    , new object[] { numDaysBack, numLastRewards, minNumberOfInvites }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qInviteReward_GetUsers", e);
                ex.AddAdditionalInformation("numDaysBack", numDaysBack);
                ex.AddAdditionalInformation("numLastRewards", numLastRewards);
                ex.AddAdditionalInformation("minNumberOfInvites", minNumberOfInvites);
                throw ex;
            }
            finally
            {
            }
        }

        public static void Admin_NoteUserGotInviteReward(Guid userIDGuid, int numOfInvitesSent, int numOfServants, int rewardNumber)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                db.ExecuteNonQuery("iInviteReward_NoteUserGotReward"
                    , new object[] { userIDGuid, numOfInvitesSent, numOfServants, rewardNumber });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iInviteReward_NoteUserGotReward", e);
                ex.AddAdditionalInformation("userIDGuid", userIDGuid);
                ex.AddAdditionalInformation("numOfInvitesSent", numOfInvitesSent);
                ex.AddAdditionalInformation("numOfServants", numOfServants);
                ex.AddAdditionalInformation("rewardNumber", rewardNumber);
                throw ex;
            }
            finally
            {
            }
        }

        /// <summary>
        /// returns table of (PlayerID, Name)
        /// </summary>
        /// <param name="players">list of player names comma seperated with NO trailing comma.
        /// GOOD: "greg,bill,mark"
        /// BAD: "greg,bill,mark,"
        /// GOOD: "bill"
        /// BAD: "bill,"</param>
        public static DataTable GetPlayersByName(string connectionStr, string players)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return DB.ExecuteDataSet(db, "qPlayersByName"
                    , new object[] { players}).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qPlayersByName", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("players", players);
                throw ex;
            }
            finally
            {
            }
        }

        /// <summary>
        /// returns table of clans (ID, Name)
        /// </summary>
        /// <param name="players">list of clan names comma seperated with NO trailing comma.
        /// GOOD: "clana,clanb,clanC"
        /// BAD: "clanA,clanB,clanC,"
        /// GOOD: "clan"
        /// BAD: "clan,"</param>
        public static DataTable GetClansByName(string connectionStr, string clans)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return DB.ExecuteDataSet(db, "qClansByName"
                    , new object[] { clans }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qClansByName", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("clans", clans);
                throw ex;
            }
            finally
            {
            }
        }
        /// <summary>
        /// returns table of clans (ID, Name)
        /// </summary>
        /// <param name="players">list of clan IDs comma seperated with NO trailing comma.
        /// GOOD: "11,12,13"
        /// BAD: "11,12,13,"
        /// GOOD: "33"
        /// BAD: "33,"</param>
        public static DataTable GetClansByIDs(string connectionStr, string clans)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return db.ExecuteDataSet("qClansByID"
                    , new object[] { clans }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qClansByID", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("clans", clans);
                throw ex;
            }
            finally
            {
            }
        }



        /// <summary>
        /// returns table of villages (ID, Name, xcord, ycord)
        /// </summary>
        public static DataTable CheckVillagesByCords(string connectionStr, string whereClause)
        {
            Database db;

            string sqlQuerry = "select  distinct villageid, name, xcord, ycord,ownerplayerid from villages where " + whereClause;
            try
            {
                db = new DB(connectionStr); ;
                

                return db.ExecuteDataSet(CommandType.Text, sqlQuerry).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while in CheckVillagesByCords", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("whereClause", whereClause);
                ex.AddAdditionalInformation("sqlQuerry", sqlQuerry);
                throw ex;
            }
            finally
            {
            }
        }

        /// <summary>
        /// returns table of villages (ID, Name, xcord, ycord)
        /// </summary>
        /// <param name="players">list of clan IDs comma seperated with NO trailing comma.
        /// GOOD: "11,12,13"
        /// BAD: "11,12,13,"
        /// GOOD: "33"
        /// BAD: "33,"</param>
        public static DataTable GetVillagesByIDs(string connectionStr, string list)
        {
            Database db;

            try
            {
                db = new DB(connectionStr); ;

                return db.ExecuteDataSet("qVillagesByID"
                    , new object[] { list }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qVillagesByID", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AddAdditionalInformation("list", list);
                throw ex;
            }
            finally
            {
            }
        }

        public static void
            RecordCreditTransaction_android(string androidID, int purchaseState, string notificationId, string productId
            , string orderId, long purchaseTime, string developerPayload, string purchaseToken, string packageName, string rawData)
        {
            Database db;

            try {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteDataSet(db, "iPaymentTransaction_android", new object[] { 
                    androidID
                    , purchaseState
                    , notificationId
                    , productId
                    , orderId
                    , purchaseTime
                    , developerPayload
                    , purchaseToken
                    , packageName
                    ,rawData});
            }
            catch (Exception e) {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iPaymentTransaction", e);

                throw ex;
            }
        }
        //CREATE Procedure [dbo].[iPaymentTransaction_ios]
        //      @uid							varchar(max)
        //    , @quantity						varchar(max)
        //    , @product_id					varchar(max)
        //    , @transaction_id				varchar(max)
        //    , @purchase_date				varchar(max)
        //    , @original_transaction_id		varchar(max)
        //    , @original_purchase_date		varchar(max)
        //    , @app_item_id					varchar(max)
        //    , @version_external_identifier	varchar(max)
        //    , @bid							varchar(max)
        //    , @bvrs							varchar(max)
        //    , @rawdata						varchar(max)
        public static void RecordCreditTransaction_ios(            
            string uid,
            string quantity,
            string product_id,
            string transaction_id,
            string purchase_date,
            string original_transaction_id,
            string original_purchase_date,
            string app_item_id,
            string version_external_identifier,
            string bid,
            string bvrs,
            string rawdata)
        {
            Database db;

            try {
                db = DatabaseFactory.CreateDatabase();

                //System.Diagnostics.Debug.WriteLine(
                //    "RecordCreditTransaction_ios(" + uid
                //    + ", " + quantity
                //    + ", " + product_id
                //    + ", " + transaction_id
                //    + ", " + purchase_date
                //    + ", " + original_transaction_id
                //    + ", " + original_purchase_date
                //    + ", " + app_item_id
                //    + ", " + version_external_identifier
                //    + ", " + bid
                //    + ", " + bvrs
                //    + ", " + rawdata);

                DB.ExecuteDataSet(db, "iPaymentTransaction_ios", new object[] { 
                    uid,
                    quantity,
                    product_id,
                    transaction_id,
                    purchase_date,
                    original_transaction_id,
                    original_purchase_date,
                    app_item_id,
                    version_external_identifier,
                    bid,
                    bvrs,
                    rawdata});
            }
            catch (Exception e) 
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iPaymentTransaction_ios", e);
                throw ex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="status"></param>
        /// <param name="sku"></param>
        /// <param name="purchaseToken"></param>
        /// <param name="rawdata"></param>
        public static void RecordCreditTransaction_amazon(
            string uid,            
            int status,
            string sku,
            string purchaseToken,
            string rawdata)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                System.Diagnostics.Debug.WriteLine(
                    "RecordCreditTransaction_amazon(" + uid
                    + ", " + sku
                    + ", " + purchaseToken
                    + ", " + rawdata);

                DB.ExecuteDataSet(db, "iPaymentTransaction_amazon", new object[] {
                    uid,                    
                    status,
                    sku,
                    purchaseToken,
                    rawdata});
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iPaymentTransaction_amazon", e);
                throw ex;
            }
        }


        public static void SetInvalidAddress_bounce(string toemail, string bounceType, string bounceSubType)
        {
            SetInvalidAddress(toemail, bounceType, bounceSubType);
        }
        public static void SetInvalidAddress_complaint(string toemail)
        {
            SetInvalidAddress(toemail, null, null);
        }
        /// <summary>
        /// DO NOT USE IF YOU DONT NEED TO. instead use the SetInvalidAddress_bounce or SetInvalidAddress_complaint
        /// 
        /// if bounce, you can set the optional bouceType and bounceSubType params. 
        /// if complaint, leave this as null
        /// </summary>
        /// <param name="toemail"></param>
        /// <param name="bounceType"></param>
        public static void SetInvalidAddress(string toemail, string bounceType, string bounceSubType)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();

                DB.ExecuteDataSet(db, "iBadEmail", new object[] { 
                    toemail, bounceType, bounceSubType});
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iBadEmail", e);
                ex.AddAdditionalInformation("toemail", toemail);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>
        /// [0] - is bad email (1/0)
        /// [1] - money made
        /// 
        /// </returns>
        public static DataTable Admin_GetUserMiscInfoForEmail(Guid userID)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return db.ExecuteDataSet("qAdminUserInfo2", new object[] { userID }).Tables[0];

            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qAdminUserInfo2", e);
                ex.AddAdditionalInformation("userID", userID);
                throw ex;

            }

        }

        public static DataTable Admin_GetUserByLastActiveOn(DateTime lastActiveMin, DateTime lastActiveMax, int? mod_div, int? mod_rem, string batchTag)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return db.ExecuteDataSet("qUsersForMassMailer", new object[] { lastActiveMin, lastActiveMax, mod_div, mod_rem, string.IsNullOrWhiteSpace(batchTag) ? null : batchTag }).Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qUsersForMassMailer", e);
                throw ex;

            }
        }


        public static bool Admin_SetUserAsNewsletterSent(string batchTag, Guid UserID, int ccid, string ccm, string ccd)
        {
            Database db;

            try
            {
                db = DatabaseFactory.CreateDatabase();
                return Convert.ToBoolean( db.ExecuteDataSet("uUserForMassMailer", new object[] { batchTag, UserID, ccid, ccm, ccd }).Tables[0].Rows[0][0]);
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uUserForMassMailer", e);
                throw ex;

            }

        }
        /// <summary>
        /// Add an entry in the AccountRecovery table
        /// </summary>
        /// <param name="email"></param>
        /// <param name="uid"></param>
        /// <param name="encuid"></param>/// 
        /// <returns>
        /// DataTable of all userids that have a 
        /// verified emails that match the provided email - facebook accounts will be set to State=3
        /// </returns>data
        public static DataSet PendingAccountRecovery(string email, string uid, string encuid)
        {
            Database db;
            DataSet ds;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("iPendingAccountRecovery");
                db.AddInParameter(cmd, "@Email", System.Data.DbType.String, email);
                db.AddInParameter(cmd, "@NewUserName", System.Data.DbType.String, encuid);
                db.AddInParameter(cmd, "@UID", System.Data.DbType.String, uid);
                ds = db.ExecuteDataSet(cmd);
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iPendingAccountRecovery", e);
                ex.AddAdditionalInformation("email", email);
                ex.AddAdditionalInformation("uid", uid);
                throw ex;

            }
            return ds;
        }
        /// <summary>
        /// process an entry in the Account Recovery table
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns>
        ///-4: couldn't update old user in Users table
        ///-3: entry has expired (> 48 hrs)
        ///-2: entry not found in AR table
        ///-1: exception
        /// 0: nothing happened - undefined behavior
        /// 1: Account was recovered and the "new" account was cleaned up
        /// 2: Account was recovered and there was no "new" account to clean up
        /// </returns>
        public static int DoAccountRecovery(int id, Guid userId)
        {
            Database db;
            int state = 0;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("uDoAccountRecovery");
                db.AddInParameter(cmd, "@ID", System.Data.DbType.Int32, id);
                db.AddInParameter(cmd, "@UserId", System.Data.DbType.Guid, userId);
                db.AddOutParameter(cmd, "@State", System.Data.DbType.Int32, int.MaxValue);
                db.ExecuteNonQuery(cmd);
                state = (int)db.GetParameterValue(cmd, "@State");
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uDoAccountRecovery", e);
                ex.AddAdditionalInformation("id", id);
                ex.AddAdditionalInformation("uid", userId);
                throw ex;
            }
            return state;
        }
        /// <summary>
        /// returns the uid and lt
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static DataTable AccountRecoveryLogin(string email)
        {
            Database db;
            DataTable dt;
            try
            {
                db = DatabaseFactory.CreateDatabase();
                System.Data.Common.DbCommand cmd = db.GetStoredProcCommand("qAccountRecoveryLogin");
                db.AddInParameter(cmd, "@Email", System.Data.DbType.String, email);
                DataSet ds = db.ExecuteDataSet(cmd);
                dt = ds.Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qAccountRecoveryLogin", e);
                ex.AddAdditionalInformation("email", email);
                throw ex;

            }
            return dt;
        }
    }
}