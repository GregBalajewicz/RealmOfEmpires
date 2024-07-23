using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.Bll
{

    [Serializable]
    public class utils
    {
        public class CONSTS
        {
            public enum RecordPaymentTransactionReturnValue
            {
                Failure_SuchTranAndStatusExists = 0,
                Success = 1
            }


            public class GetPlayersByNameColIndex
            {
                public static int PlayerID = 0;
                public static int Name = 1;
            }

            public class GetClansByXColIndex
            {
                public static int ID = 0;
                public static int Name = 1;
            }
            public class GetVillagesByXColIndex
            {
                public static int ID = 0;
                public static int Name = 1;
                public static int XCord = 2;
                public static int YCord = 3;
                public static int OwnerPlayerID = 4;
            }

        }

        public static T[] InitializeArray<T>(int length) where T : new()
        {
            T[] array = new T[length];
            for (int i = 0; i < length; ++i)
            {
                array[i] = new T();
            }

            return array;
        }


        public static bool LogLogin(Player player, string remoteAddress, long port, string agent)
        {
            try
            {
                return DAL.utils.LogLogin(player.Realm.ConnectionStr, player.ID, remoteAddress, port, agent);
            }
            catch (Exception e)
            {
                System.Collections.Specialized.NameValueCollection col = new System.Collections.Specialized.NameValueCollection();
                BaseApplicationException.AddAdditionalInformation(col, "player", player);
                Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(new Exception("In LogLogin", e), col);

                //
                // we eat the exception on purpose. do not want failure if login cannot be logged. 
                //
            }
            return false;
        }


        public static int Admin_BanPlayer(Player player, Guid userID, string publicMsg, string notes, int? hoursTillExpiry)
        {

            return DAL.utils.Admin_BanPlayer(player.Name, userID, publicMsg, notes, hoursTillExpiry);
        }

        public static DataTable Admin_FindUserByUserName(string username)
        {
            return Fbg.DAL.utils.Admin_FindUser(null, null, username, null);
        }
        public static DataTable Admin_FindUserByName(string PlayerName)
        {
            return Fbg.DAL.utils.Admin_FindUser(PlayerName, null, null, null);
        }
        public static DataTable Admin_FindUserByEmail(string email)
        {
            return Fbg.DAL.utils.Admin_FindUser(null, email, null, null);
        }

        public enum GiveServantsReason
        {
            ManualAdminFunctionAdd,
            RewardOrPromo
        }
        public static bool Admin_GiveServants(Guid UserID, int ServantsAmount, GiveServantsReason reason)
        {
            int eventType = -1;
            switch (reason)
            {
                case GiveServantsReason.ManualAdminFunctionAdd:
                    eventType = 4;
                    break;
                case GiveServantsReason.RewardOrPromo:
                    eventType = 5;
                    break;
                default:
                    throw new Exception("unrecognized reason:" + reason.ToString());
            }
            return Fbg.DAL.utils.Admin_GiveServants(UserID, ServantsAmount, eventType);
        }
        public static int Admin_UnBanPlayer(Guid UserID)
        {

            return DAL.utils.Admin_UnBanPlayer(UserID);
        }
        public static DataTable Admin_GetPlayersByIP(Player player, string ip)
        {
            return DAL.utils.Admin_GetPlayersByIP(player.Realm.ConnectionStr, ip);
        }

        /// <summary>
        /// returns 1 if all is well, something else otherwise (see iBanPlayer for more info)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static int Admin_Cofiscate(Player player, Guid UserID)
        {

            User user = new User(UserID);

            foreach (Player p in user.Players)
            {

                int result = DAL.utils.Admin_Cofiscate(p.Realm.ConnectionStr, p.Name);
                if (result == 0)
                {
                    return result;

                }
            }
            return 1;

        }


        public static CONSTS.RecordPaymentTransactionReturnValue RecordPaymentTransaction(string tranID, Guid userID, short status, string rawData
             , string payment_status, string payment_date, string parent_txn_id
             , string paymentGross, string paymentFee, string payer_email)
        {
            return (CONSTS.RecordPaymentTransactionReturnValue)DAL.utils.RecordPaymentTransaction(tranID
                , userID, status, rawData, payment_status, payment_date
                , parent_txn_id, paymentGross, paymentFee, payer_email);
        }

        public static int RecordFbCreditTransaction(long order_id, string fb_sig_user, int price, int servants)
        {
            return Fbg.DAL.utils.RecordFbCreditTransaction(order_id, fb_sig_user, price, servants);
        }

        public static int RecordFbCreditTransaction(long order_id, string fb_sig_user, int price, int servants, float amount, string currency)
        {
            return Fbg.DAL.utils.RecordFbCreditTransaction(order_id, fb_sig_user, price, servants, amount, currency);
        }

        public static int RecordKongregateCreditTransaction(string kongreagateUserID, string packageID, string kongregateItemID, string kongregateUsageID, string rawListOfItems, string rawThisItem)
        {
            return Fbg.DAL.utils.RecordKongregateCreditTransaction(kongreagateUserID, packageID, kongregateItemID, kongregateUsageID,rawThisItem,  rawListOfItems);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="androidID"></param>
        /// <param name="purchaseState">0 (purchased), 1 (canceled), 2 (refunded), or 3 (expired, for subscription purchases only)</param>
        /// <param name="notificationId"></param>
        /// <param name="productId"></param>
        /// <param name="orderId"></param>
        /// <param name="purchaseTime"></param>
        /// <param name="developerPayload"></param>
        /// <param name="purchaseToken">Unique transaction ID</param>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public static Fbg.Common.RecordPaymentTransaction_ReturnValue RecordCreditTransaction_android(string androidID,
            int purchaseState,
            string notificationId,
            string productId,
            string orderId,
            long purchaseTime,
            string developerPayload,
            string purchaseToken,
            string packageName,
            string rawData)
        {
            Fbg.DAL.utils.RecordCreditTransaction_android(
                androidID,
                purchaseState,
                notificationId,
                productId,
                orderId,
                purchaseTime,
                developerPayload,
                purchaseToken,
                packageName,
                rawData);

            return Common.RecordPaymentTransaction_ReturnValue.Success;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="quantity"></param>
        /// <param name="product_id"></param>
        /// <param name="transaction_id"></param>
        /// <param name="purchase_date"></param>
        /// <param name="original_transaction_id"></param>
        /// <param name="original_purchase_date"></param>
        /// <param name="app_item_id"></param>
        /// <param name="version_external_identifier"></param>
        /// <param name="bid"></param>
        /// <param name="bvrs"></param>
        /// <param name="rawdata"></param>
        /// <returns></returns>
        public static Fbg.Common.RecordPaymentTransaction_ReturnValue RecordCreditTransaction_iOS(
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
            Fbg.DAL.utils.RecordCreditTransaction_ios(
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
                rawdata);

            return Common.RecordPaymentTransaction_ReturnValue.Success;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="status"></param>
        /// <param name="sku"></param>
        /// <param name="purchaseToken"></param>
        /// <param name="rawdata"></param>
        /// <returns></returns>
        public static Fbg.Common.RecordPaymentTransaction_ReturnValue RecordCreditTransaction_amazon(
            string uid,
            int status,
            string sku,
            string purchaseToken,
            string rawdata)
        {
            Fbg.DAL.utils.RecordCreditTransaction_amazon(
                uid,
                status,
                sku,
                purchaseToken,
                rawdata);

            return Common.RecordPaymentTransaction_ReturnValue.Success;
        }

        public static DataSet Notifications_GetRecipients(int notificationID, bool isTest)
        {
            return DAL.utils.Notifications_GetRecipients(notificationID, isTest);
        }
        public static DataSet Notifications_MarkSuccessfulSend(int notificationID, string facebookIDs)
        {
            return DAL.utils.Notifications_MarkSuccessfulSend(notificationID, facebookIDs);
        }

        public static void Stories_StoryPublished(int playerID, int storyID, string body, bool isSuccessful)
        {
            DAL.utils.Stories_StoryPublished(playerID, storyID, body, isSuccessful);
        }



        public static bool IsSpecialPlayer(string playername)
        {
            if (String.Compare(playername.Trim(), Fbg.Bll.CONSTS.SpecialPlayers.Abandonedl_PlayerName, true) == 0
                || String.Compare(playername.Trim(), Fbg.Bll.CONSTS.SpecialPlayers.roe_team_PlayerName, true) == 0
                || String.Compare(playername.Trim(), Fbg.Bll.CONSTS.SpecialPlayers.Rebel_PlayerName, true) == 0)
            {
                return true;
            }
            return false;
        }


        public static bool IsSpecialPlayer(Guid userID)
        {
            if (userID == Fbg.Bll.CONSTS.SpecialPlayers.roe_team_UserID
                || userID == Fbg.Bll.CONSTS.SpecialPlayers.Abandoned_UserID
                || userID == Fbg.Bll.CONSTS.SpecialPlayers.Rebel_UserID)
            {
                return true;
            }
            return false;
        }
        public static bool IsSpecialPlayer(int playerID, Realm realm)
        {
            if (playerID == Fbg.Bll.CONSTS.SpecialPlayers.Rebels_PlayerId(realm)
                || playerID == Fbg.Bll.CONSTS.SpecialPlayers.Abandoned_PlayerId(realm)
                || playerID == Fbg.Bll.CONSTS.SpecialPlayers.roe_team_PlayerId(realm))
            {
                return true;
            }
            return false;
        }



        public static DataSet Admin_GetStats(int s)
        {
            return DAL.utils.Admin_GetStats(s);
        }

        public static DataSet Admin_GetUserInfo(Guid userID)
        {
            return DAL.utils.Admin_GetUserInfo(userID);
        }


        public static Fbg.Bll.CONSTS.VillageType GetVillageType(Realm realm, int ownerPlayerID)
        {
            if (ownerPlayerID == Fbg.Bll.CONSTS.SpecialPlayers.Abandoned_PlayerId(realm))
            {
                return Fbg.Bll.CONSTS.VillageType.Abandoned;
            }
            else if (ownerPlayerID == Fbg.Bll.CONSTS.SpecialPlayers.Rebels_PlayerId(realm))
            {
                return Fbg.Bll.CONSTS.VillageType.Rebel;
            }
            else
            {
                return Fbg.Bll.CONSTS.VillageType.Normal;
            }
        }


        public static DataSet Admin_GetPlayerInfo(string connectionStr, int playerID)
        {
            return DAL.utils.Admin_GetPlayerInfo(connectionStr, playerID);
        }

        public static void Admin_ChangeVillageOwnership(string connectionStr, int villageID, int toPlayerID, string newVillageName)
        {
            DAL.utils.Admin_ChangeVillageOwnership(connectionStr, villageID, toPlayerID, newVillageName);
        }

        public static DataTable Admin_GetUsersForInviteReward(int numDaysBack
            , int numLastRewards, int minNumberOfInvites)
        {
            return DAL.utils.Admin_GetUsersForInviteReward(numDaysBack, numLastRewards, minNumberOfInvites);
        }

        public static void Admin_NoteUserGotInviteReward(Guid userIDGuid, int numOfInvitesSent, int numOfServants, int rewardNumber)
        {
            DAL.utils.Admin_NoteUserGotInviteReward(userIDGuid, numOfInvitesSent, numOfServants, rewardNumber);
        }

        /// <summary>
        /// Described by utils.CONSTS.GetPlayersByNameColIndex
        /// </summary>
        /// <param name="players">list of player names comma seperated with NO trailing comma.
        /// GOOD: "greg,bill,mark"
        /// BAD: "greg,bill,mark,"
        /// GOOD: "bill"
        /// BAD: "bill,"</param>
        public static DataTable GetPlayersByName(Realm realm, string players)
        {
            DataTable dt = DAL.utils.GetPlayersByName(realm.ConnectionStr, players);
            return dt;
        }



        /// <summary>
        /// Described by utils.CONSTS.GetClansByXColIndex
        /// </summary>
        /// <param name="players">list of clan names comma seperated with NO trailing comma.
        /// GOOD: "clana,clanb,clanC"
        /// BAD: "clanA,clanB,clanC,"
        /// GOOD: "clan"
        /// BAD: "clan,"</param>
        public static DataTable GetClansByName(Realm realm, string clans)
        {
            DataTable dt = DAL.utils.GetClansByName(realm.ConnectionStr, clans);
            return dt;
        }

        /// <summary>
        /// Described by utils.CONSTS.GetClansByXColIndex
        /// </summary>
        /// <param name="clans">list of clan IDs comma seperated with NO trailing comma.
        /// GOOD: "11,12,13"
        /// BAD: "11,12,13,"
        /// GOOD: "33"
        /// BAD: "33,"</param>
        public static DataTable GetClansByIDs(Realm realm, string clans)
        {
            DataTable dt = DAL.utils.GetClansByIDs(realm.ConnectionStr, clans);
            return dt;
        }

        /// <summary>
        /// Described by utils.CONSTS.GetVillagesByXColIndex
        /// </summary>
        /// <param name="whereClause">something like this: (xcord=-10 and ycord=-6) or (xcord = 0 and ycord=-20)</param>
        internal static DataTable CheckVillagesByCords(Realm realm, string whereClause)
        {
            DataTable dt = DAL.utils.CheckVillagesByCords(realm.ConnectionStr, whereClause);
            return dt;
        }

        /// <summary>
        /// Described by utils.CONSTS.GetVillagesByXColIndex
        /// </summary>
        /// <param name="players">list of village IDs comma seperated with NO trailing comma.
        /// GOOD: "11,12,13"
        /// BAD: "11,12,13,"
        /// GOOD: "33"
        /// BAD: "33,"</param>
        internal static DataTable GetVillagesByIDs(Realm realm, string list)
        {
            DataTable dt = DAL.utils.GetVillagesByIDs(realm.ConnectionStr, list);
            return dt;

        }

        public static string Ordinal(int number)
        {
            string suffix = String.Empty;

            int ones = number % 10;
            int tens = (int)Math.Floor(number / 10M) % 10;

            if (tens == 1)
            {
                suffix = "th";
            }
            else
            {
                switch (ones)
                {
                    case 1:
                        suffix = "st";
                        break;

                    case 2:
                        suffix = "nd";
                        break;

                    case 3:
                        suffix = "rd";
                        break;

                    default:
                        suffix = "th";
                        break;
                }
            }
            return String.Format("{0}{1}", number, suffix);
        }

        public static bool isBadEmailException(Exception e)
        {
            /*
            trying to caputre this error:
             *     
                Exception #3 Information
                *********************************************
                [[ Exception Type ]] : Amazon.SimpleEmail.Model.MessageRejectedException
                [[ ErrorType ]] : Sender
                [[ ErrorCode ]] : MessageRejected
                [[ RequestId ]] : e4266232-a835-11e2-9dc6-d96e1bcc3a58
                [[ StatusCode ]] : BadRequest
                [[ Message ]] : Address blacklisted.
             
             */
            if (e != null)
            {
                if (e is Amazon.SimpleEmail.Model.MessageRejectedException)
                {
                    if (((Amazon.SimpleEmail.Model.MessageRejectedException)e).ErrorCode == "MessageRejected"
                        && ((Amazon.SimpleEmail.Model.MessageRejectedException)e).Message.Contains("Address blacklisted"))
                    {
                        return true;
                    }
                }

                return isBadEmailException(e.InnerException);
            }

            return false;

        }
        /// <summary>
        /// 
        /// </summary>
        const int MSInAnHour = 3600000;
        /// <summary>
        /// converts a localtime based on the user timezone offset
        /// </summary>
        /// <param name="utc">universal time</param>
        /// <param name="timeZone">timezone offset in hours. eg: -4.0, 5.5</param>
        /// <returns></returns>
        public static DateTime ToLocalTime(DateTime utc, float timeZone)
        {
            return utc.AddMilliseconds(timeZone * MSInAnHour);
        }
        /// <summary>
        /// account recovery information
        /// </summary>
        public class AccountRecoveryInfo
        {
            List<AccountRecovery> _arl = new List<AccountRecovery>();
            public void Add(AccountRecovery ar)
            {
                _arl.Add(ar);
            }
            public List<AccountRecovery> AccountRecoveryList
            {
                get
                {
                    return _arl;
                }
            }
        }
        /// <summary>
        /// entry in AccountRecovery table
        /// </summary>
        public class AccountRecovery
        {
            public enum eState
            {
                AnotherRequestMade = -5,
                UpdateFailed = -4,
                RequestExpired = -3,
                AccountRecoveryLookupFailed = -2,
                Exception = -1,
                Initial = 0,
                AccountRecoveredOld2New = 1,
                AccountRecovered2New = 2,
                FacebookAccount = 3,
            }
            DataRow _dr;
            public AccountRecovery(DataRow dr)
            {
                _dr = dr;
            }
            public int ID
            {
                get
                {
                    return Convert.ToInt32(_dr["ID"].ToString());
                }
            }
            public eState State
            {
                get
                {
                    return (eState)Convert.ToInt32(_dr["State"].ToString());
                }
            }
            public string UserId
            {
                get
                {
                    return _dr["UserId"].ToString();
                }
            }
            public string UID
            {
                get
                {
                    return _dr["UID"].ToString();
                }
            }
            public string OldUserName
            {
                get
                {
                    return _dr["OldUserName"].ToString();
                }
            }
            public string NewUserName
            {
                get
                {
                    return _dr["NewUserName"].ToString();
                }
            }
            public DateTime RequestDate
            {
                get
                {
                    return (DateTime.Parse(_dr["RequestDate"].ToString()));
                }
            }
            public string PlayerNames
            {
                get
                {
                    return _dr["PlayerNames"].ToString();
                }
            }
            //public string ToDebugString()
            //{
            //    return string.Format("ID:{0} State:{1} UserId:{2} OldUserName:{3} NewUserName:{4} RequestDate:{5} PlayerNames:{6}",
            //        ID, State, UserId, OldUserName, NewUserName, RequestDate, PlayerNames);
            //}
        }
        /// <summary>
        /// Add entry to AccountRecovery table
        /// </summary>
        /// <param name="email"></param>
        /// <param name="uid"></param>
        /// <param name="encuid"></param>
        /// <returns></returns>
        public static AccountRecoveryInfo PendingAccountRecovery(string email, string uid, string encuid)
        {
            AccountRecoveryInfo ari = new AccountRecoveryInfo();
            DataSet ds = DAL.utils.PendingAccountRecovery(email, uid, encuid);
            DataTable pending = ds.Tables[0];

            foreach (DataRow dr in pending.Rows)
            {
                ari.Add(new AccountRecovery(dr));
            }            
            return ari;
        }
        /// <summary>
        /// Perform the account recovery
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static int DoAccountRecovery(int id, Guid uid)
        {
            return DAL.utils.DoAccountRecovery(id, uid);
        }
        /// <summary>
        /// returns the start page for recount recovery redirect
        /// </summary>
        /// <param name="page"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string AccountRecoveryLogin(string page, string email)
        {
            string login = "chooselogintype.aspx";
            try
            {
                DataTable dt = DAL.utils.AccountRecoveryLogin(email);
                // if there are no rows then this was a facebook account so
                string uid = dt.Rows[0]["UID"].ToString();
                string lt = dt.Rows[0]["LT"].ToString();
                login = string.Format("{0}?lt={1}&uid={2}", page, lt, uid);
            }
            catch (Exception x)
            {
                System.Diagnostics.Debug.WriteLine("AccountRecoveryLogin: " + x.Message);
            }
            return login;
        }




        public static string FormatDuration(TimeSpan time)
        {
            //if (time.Milliseconds >= 500)
            //{
            //    time = time.Add(new TimeSpan(0,0,0,1));
            //}

            string s = "";
            if (time.TotalHours > 0)
            {
                s += Math.Floor(time.TotalHours) + ":";
            }
            s += (time.Minutes < 10 ? "0" + time.Minutes.ToString() : time.Minutes.ToString())
                + ":" + (time.Seconds < 10 ? "0" + time.Seconds.ToString() : time.Seconds.ToString());

            return s;
        }
        public static string FormatDuration_Long(TimeSpan time)
        {
            string s = "";

            if (time.TotalHours >= 1)
            {
                s = Convert.ToInt32(Math.Floor(time.TotalHours)).ToString() + " hours, ";
            }
            if (time.TotalMinutes >= 1)
            {
                s += time.Minutes.ToString() + " min, ";
            }

            if (time.TotalSeconds >= 1)
            {
                s += time.Seconds.ToString() + " sec";
            }
            return s;
        }
        public static string FormatDuration_Long2(TimeSpan time)
        {
            string s = "";

            if (time.TotalHours >= 1)
            {
                s = Convert.ToInt32(Math.Floor(time.TotalHours)).ToString() + "h";
            }
            if (time.Minutes.ToString() != "0")
            {
                s += (s == "" ? "" : ", ") + time.Minutes.ToString() + "m";
            }

            if (time.Seconds.ToString() != "0")
            {
                s += (s == "" ? "" : ", ") + time.Seconds.ToString() + "s";
            }
            return s;
        }
        public static string FormatDuration_Short(TimeSpan time)
        {
            string s = "";

            if (time.TotalSeconds < 60)
            {
                s = time.Seconds.ToString() + " s";
            }
            else if (time.TotalMinutes < 60)
            {
                s = time.TotalMinutes.ToString("#.0") + " m";
            }
            else
            {
                s = time.TotalHours.ToString("#.0") + " h";
            }


            return s;
        }



        public static string FormatCost(int cost)
        {
            return cost.ToString("#,###0");
        }

        public static string FormatCost(long cost)
        {
            return cost.ToString("#,###0");
        }

        public static string FormatCost(double cost)
        {
            return cost.ToString("#,###0.#0");
        }

    }
}

