using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Linq;

namespace Fbg.Bll
{
    public class User : Gmbc.Common.Diagnostics.ExceptionManagement.ISerializableToNameValueCollection2
    {
        public class CONSTS
        {
            public class UserTableIndex
            {
                public static int UserBasicInfo = 0;
                public static int PlayerBasicInfo = 1;
                public static int Flags = 2;
            }
            public class UserStatsTableIndex
            {
                public static int ServantsBoughtVIAPaypal = 0;
                public static int ServantsBoughtVIAOfferProviders = 1;
                public static int ServantsGiven = 2;
                public static int ServantsFromPromos = 3;
                public static int InviteStats = 4;
                public static int NumberOfServantsBought = 5;


            }
            public class UserBasicInfoColumnIndex
            {
                public static int TimeZone = 0;
                public static int UserName = 1;
                public static int LoginType = 2;
                public static int Name = 3;
                public static int AvatarID = 4;
                public static int Sex = 5;
                public static int SuspensionReason = 6;
                public static int SuspensionUntil = 7;
            }
            public class PlayerBasicInfoColumnIndex
            {
                public static int playerID = 0;
                public static int RealmID = 1;
                public static int UserID = 2;
                public static int Name = 3;
                public static int NoLongerUsed = 4;
                public static int AvatarID = 5;
                public static int Morale= 6;
                public static int MoraleLastUpdated = 7;
            } 
            public class UsersColumnIndex
            {
                public static int playerID = 0;
                public static int RealmID = 1;
                public static int UserID = 2;
                public static int Name = 3;
                public static int FaceBookID = 4;
                
            }

            public class DonorColumnIndex
            {
                public static int playerID = 0;
                public static int playerName = 1;
            }

            public class MyInvitesColIndex
            {
                public static int InviteID = 0;
                public static int PlayerID= 1;
                public static int InvitedOn= 2;
                public static int FacebookID = 3;
                public static int RealmID = 4;
                public static int PlayerName = 5;
            }
            public class Items2ColIndes
            {
                public static int ItemID = 0;
                public static int PlayerID = 1;
                public static int ExpiresOn = 2;
                public static int PFFeatureID = 3;
                public static int DurationInMin = 4;
                public static int UnitTypeID = 5;
                public static int Amount = 6;
                public static int SilverAmount = 7;
                public static int MinutesAmount = 8;
                public static int ResearchSpeedUpMinutesAmount = 9;
            }
            public class AcceptedInvitesColIndex
            {
                /// <summary>
                /// column of datatype int
                /// </summary>
                public static int InviteID = 0;
                /// <summary>
                /// column of Datatype string
                /// </summary>
                public static int InvitedID = 1;
                /// <summary>
                /// column of Datatype int
                /// </summary>
                public static int PlayerID = 2;
                /// <summary>
                /// column of Datatype int
                /// </summary>
                public static int RealmID = 3;
                /// <summary>
                /// column of Datatype int.
                /// 1==pending invite
                /// 2==accepted invite
                /// 3==accepted invite & reward claimed
                /// </summary>
                public static int StatusID= 4;
                /// <summary>
                /// column of Datatype Guid.
                /// </summary>
                public static int UserID = 5;
            }
            public class PendingInvitesColIndex
            {
                /// <summary>
                /// column of datatype int
                /// </summary>
                public static int InviteID = 0;
                /// <summary>
                /// column of Datatype string
                /// </summary>
                public static int InvitedID = 1;
                /// <summary>
                /// column of Datatype DateTime
                /// </summary>
                public static int InvitedOn = 2;
            }
            /// <summary>
            /// describes the table returned by Gifts_GetTodaysGifts
            /// </summary>
            public class TodaysGiftsColIndex
            {
                /// <summary>
                /// column of Datatype string
                /// </summary>
                public static int SentTo = 0;
            }
            public class FlagsColIndex
            {
                public static int FlagID = 0;
                public static int UpdatedOn = 1;
                public static int Data = 2;
            }
            public class FriendsColName
            {
                public static string FriendUserID = "FriendUserID";

            }
        }



        public enum Flags
        {
            Misc = 0,
            /// <summary>
            /// this tells us if player has gotten the 20 serants when accepting a title and has 2 villages at least
            /// </summary>
            Misc_Got2VillagePromo = 1,
            /// <summary>
            /// tells us if player has either denied allowing short stories (no reward) or said he allowed short stories and hence got a reward of X servants
            /// </summary>
            Misc_UsedUpAllowShortStoriesPromo = 2,
            /// <summary>
            /// This flag is set everytime user registers at some realm.
            /// It can be used to find out if player has registed at atleast one realm
            /// </summary>
            Misc_RegistredAtARealm = 3,
            /// <summary>
            /// Data field of this flag holds the ID of the highest title this player ever achieved in some realm. 
            /// </summary>
            Misc_HighestTitleAchieved = 4,
            /// <summary>
            /// this is set when a player see the page that encourages them to invite their friends and chooses to invite
            /// </summary>
            Misc_EncouragedInvite_ChoiceMade = 5,
            /// <summary>
            /// this is set when a player see the page that encourages them to invite their friends
            /// </summary>
            Misc_EncouragedInvite_PageSeen = 6,
            /// <summary>
            /// means that this player has already taken advantage of the 100 servants for 2.95 set of offers
            /// </summary>
            Offers_HasUsedServantOfferNumber1 = 7,
            /// <summary>
            /// means that this player has been offered offer number 1 - ie, was told about it somehow and is aware that he has it
            /// </summary>
            Offers_HasBeenOfferedOfferNumber1 = 8,
            /// <summary>
            /// used for the friend network ranking reward
            /// </summary>
            FriendNetworkRankingReward = 9,
            /// <summary>
            /// if user has this flag, user has no right to send emails to clan members
            /// </summary>
            BannedFromSendingClamEmails = 10,
            /// <summary>
            /// tells us if user has got the reward for rating the mobile app. 
            /// </summary>
            Reward_MobileAppRate = 11,
            /// <summary>
            /// if player has this flag, means he as free servants waiting
            /// </summary>
            Offers_FreeServants = 12,
            /// <summary>
            /// set once a player gives us their email address and verifies it. 
            /// </summary>
            Misc_VerifiedEmail=13,
            /// <summary>
            /// means that this player has already taken advantage of the time time servants offer
            /// </summary>
            Offers_HasUsedServantOfferNumber2 = 14,
            /// <summary>
            /// means that this player has been offered offer number 2 - ie, was told about it somehow and is aware that he has it
            /// </summary>
            Offers_HasBeenOfferedOfferNumber2 = 15,
            LocalStorageOnServer=16,

            PartnerEntry_partner = 17,
            PartnerEntry_brand = 18,
            PartnerEntry_locale = 19,

            /// <summary>
            /// Credit farming: stores number of servants farmed today by the user.
            /// Example of usage would be if last update isnt today, reset data to 0, otherwise add to data (check against a max)
            /// </summary>
            FarmedCreditsToday = 20,
            HideMobileIconFromMainUI = 21,
            DisableRenameGlobalPlayerName = 22,
            VIPLevel = 23,
            VIPLevel_ShowInChat = 24,
            VideAd_IsAvailable = 25,
            VideAd_Checked = 26,
            AI_ServantsGiven = 27
        }

        public enum Offers
        {
            /// <summary>
            /// the 100 servants for 2.95 set of offers
            /// </summary>
            Number1,
            FreeServants,
            /// <summary>
            /// deep discount offer
            /// </summary>
            Number2 
        }

        DataTable dtFlags;
        List<Fbg.Common.UsersFriend> friends = null;
        List<Player> players;
        ThroneRoom.MyThroneRoom _throneRoom;
        DataTable drPlayers;
        Guid _userID;
        float _timeZone=0;
        Stats _stats;
        int _giftRecentlyAccepted=Int32.MinValue;
        int[] _giftsRecentlyAccepted;

        #region Suspensions
        DateTime lastTimeSuspensionStatusChecked;
        public bool IsSuspended
        {
            get
            {
                GetLatestSuspensionStatusIfNeeded();
                return 
                    drUser ==null ? false // this is needed as newly registered player will not be able to get in 
                    : !(drUser[User.CONSTS.UserBasicInfoColumnIndex.SuspensionReason] is DBNull);
            }
        }

        /// <summary>
        /// will return null if IsSuspended is false
        /// </summary>
        public string SuspensionReason
        {
            get
            {
                if (IsSuspended)
                {
                    return (string)drUser[User.CONSTS.UserBasicInfoColumnIndex.SuspensionReason];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// will return null if IsSuspended is false. if not specified, or not suspended, will return DateTime.MinValue
        /// </summary>
        public DateTime SuspensionUntil
        {
            get
            {
                if (IsSuspended)
                {
                    if (drUser[User.CONSTS.UserBasicInfoColumnIndex.SuspensionUntil] is DBNull) {
                        return DateTime.MinValue;
                    }
                    return (DateTime)drUser[User.CONSTS.UserBasicInfoColumnIndex.SuspensionUntil];
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }


        public void GetLatestSuspensionStatusIfNeeded()
        {
            //
            // update the suspension status ever few minute (15 now) 
            //
            // NOTE - this is an unecessarily heavy implemetnation as it calls DAL.User.GetUser(ID) which gets more than just 
            //  suspension status. However, it was done like this in a hurry. Because it does not happen all the time, it shoudl be OK
            //
            if (lastTimeSuspensionStatusChecked.AddMinutes(15) < DateTime.Now)
            {
                lastTimeSuspensionStatusChecked = DateTime.Now;
                DataSet ds = DAL.User.GetUser(ID);
                drUser = ds.Tables[CONSTS.UserTableIndex.UserBasicInfo].Rows[0];
            }
        }


        #endregion 


      

        public bool VIP_isVIP
        {
            get { return HasFlag(Flags.VIPLevel) != null; }
        }

        /// <summary>
        /// return of 0 means no vip status
        /// </summary>
        public int VIP_vipLevel
        {
            get
            {
                if (VIP_isVIP)
                {
                    return Convert.ToInt32( HasFlag_GetData(Flags.VIPLevel));
                }
                return 0;
            }
        }


        DataRow drUser;
        public User(Guid userID)
        {
            _userID = userID;

            lastTimeSuspensionStatusChecked = DateTime.Now;
            DataSet ds = DAL.User.GetUser(userID);
            drPlayers = ds.Tables[CONSTS.UserTableIndex.PlayerBasicInfo];
            dtFlags = ds.Tables[CONSTS.UserTableIndex.Flags];
            dtFlags.PrimaryKey = new DataColumn[] { dtFlags.Columns[CONSTS.FlagsColIndex.FlagID] };


            if (ds.Tables[CONSTS.UserTableIndex.UserBasicInfo].Rows.Count > 0)
            {
                drUser = ds.Tables[CONSTS.UserTableIndex.UserBasicInfo].Rows[0];
                _timeZone = Convert.ToSingle(ds.Tables[CONSTS.UserTableIndex.UserBasicInfo].Rows[0][CONSTS.UserBasicInfoColumnIndex.TimeZone]);
                UserName = Convert.ToString(ds.Tables[CONSTS.UserTableIndex.UserBasicInfo].Rows[0][CONSTS.UserBasicInfoColumnIndex.UserName]);

                if (ds.Tables[CONSTS.UserTableIndex.UserBasicInfo].Rows[0][CONSTS.UserBasicInfoColumnIndex.AvatarID] is DBNull)
                {
                    UserAvatarID = 1; 
                }
                else {
                    UserAvatarID = Convert.ToInt32(ds.Tables[CONSTS.UserTableIndex.UserBasicInfo].Rows[0][CONSTS.UserBasicInfoColumnIndex.AvatarID]);
                }

                if (ds.Tables[CONSTS.UserTableIndex.UserBasicInfo].Rows[0][CONSTS.UserBasicInfoColumnIndex.Sex] is DBNull)
                {
                    UserSex = 0;
                }
                else {
                    UserSex = Convert.ToInt32(ds.Tables[CONSTS.UserTableIndex.UserBasicInfo].Rows[0][CONSTS.UserBasicInfoColumnIndex.Sex]);
                }

                if (ds.Tables[CONSTS.UserTableIndex.UserBasicInfo].Rows[0][CONSTS.UserBasicInfoColumnIndex.LoginType] is DBNull)
                {
                    LoginType = Common.UserLoginType.Unknown;
                }
                else
                {
                    LoginType = (Fbg.Common.UserLoginType)((Int16)ds.Tables[CONSTS.UserTableIndex.UserBasicInfo].Rows[0][CONSTS.UserBasicInfoColumnIndex.LoginType]);
                }

                if (ds.Tables[CONSTS.UserTableIndex.UserBasicInfo].Rows[0][CONSTS.UserBasicInfoColumnIndex.Name] is DBNull)
                {
                    GlobalPlayerName = string.Empty;
                }
                else
                {
                    GlobalPlayerName = Convert.ToString(ds.Tables[CONSTS.UserTableIndex.UserBasicInfo].Rows[0][CONSTS.UserBasicInfoColumnIndex.Name]);
                }
            }

        }


        #region Offers_ stuff
        /// <summary>
        /// tells you if user has taken advantage of an offer made for him 
        /// </summary>
        public bool Offers_HasCompletedOffer(Offers offer)
        {
            switch (offer)
            {
                case Offers.Number1:
                    return HasFlag(Flags.Offers_HasUsedServantOfferNumber1) != null;
                case Offers.Number2:
                    return HasFlag(Flags.Offers_HasUsedServantOfferNumber2) != null;
                default:
                    return false;
            }
        }
        /// <summary>
        /// for some offers, returns the actual amount
        /// </summary>
        public int Offers_GetServantOfferAmount(Offers offer)
        {
            object offerAmount;
            switch (offer)
            {
                case Offers.FreeServants:
                    offerAmount = HasFlag_GetData(Fbg.Bll.User.Flags.Offers_FreeServants);
                    if (offerAmount != null)
                    {
                        int amount = Convert.ToInt32(offerAmount);

                        if (amount > 0)
                        {
                            return amount;
                        }
                    }
                    return 0;
                case Offers.Number2:
                    offerAmount = HasFlag_GetData(Fbg.Bll.User.Flags.Offers_HasBeenOfferedOfferNumber2);
                    if (offerAmount != null)
                    {
                        int amount = Convert.ToInt32(offerAmount);

                        if (amount > 0)
                        {
                            return amount;
                        }
                    }
                    return 500;
                default:
                    throw new NotImplementedException(offer.ToString());
            }
        }
        /// <summary>
        /// tells you if user has been offered this offer and hence has it available to him. Ie, it tells us that this
        /// user has been made aware of this offer in some way and hence knows its available to him.
        /// If offer is available only once, this will return false if it has already been used up. 
        /// </summary>
        public bool Offers_HasOffer(Offers offer)
        {
            switch (offer)
            {
                case Offers.Number1:
                    if (HasFlag(Flags.Offers_HasBeenOfferedOfferNumber1) != null)
                    {
                        return HasFlag(Flags.Offers_HasUsedServantOfferNumber1) == null;
                    }
                    return false;
                case Offers.Number2:
                    if (HasFlag(Flags.Offers_HasBeenOfferedOfferNumber2) != null)
                    {
                        return HasFlag(Flags.Offers_HasUsedServantOfferNumber2) == null;
                    }
                    return false;
                case Offers.FreeServants:
                    object offerAmount = HasFlag_GetData(Fbg.Bll.User.Flags.Offers_FreeServants);
                    if (offerAmount != null)
                    {
                        int amount = Convert.ToInt32(offerAmount);

                        if (amount > 0)
                        {
                            return true;
                        }
                    }
                    return false;
                default:
                    throw new NotImplementedException(offer.ToString());
            }
        }

        /// <summary>
        /// tells if is particular user/player is eligible for this offer right now - ie, have all requirements been satisfied
        ///     for this offer. It does not mean the user has already been presented with this offer. For this, see Offers_HasOffer()
        /// </summary>
        public bool Offers_IsOfferAvailable(Offers offer, Player player)
        {
            return Offers_IsOfferAvailable(offer, player.RegisteredOn);
        }
        /// <summary>
        /// tells if is particular user/player is eligible for this offer right now - ie, have all requirements been satisfied
        ///     for this offer. It does not mean the user has already been presented with this offer. For this, see Offers_HasOffer()
        /// </summary>
        public bool Offers_IsOfferAvailable(Offers offer, DateTime registeredOn)
        {
            return false;
        }

        /// <summary>
        /// log the fact that user has been offered this offer
        /// </summary>
        /// <param name="offers"></param>
        public void Offers_SetOfferAsOffered(Offers offer)
        {
            Offers_SetOfferAsOffered(offer, 0);
        }
        public void Offers_SetOfferAsOffered(Offers offer, int servantAmount)
        {
            switch (offer)
            {
                case Offers.Number1:
                    SetFlag(Flags.Offers_HasBeenOfferedOfferNumber1);
                    break;
                case Offers.Number2:
                    SetFlag(Flags.Offers_HasBeenOfferedOfferNumber2, servantAmount.ToString());
                    break;
                case Offers.FreeServants:
                    SetFlag(Flags.Offers_FreeServants, servantAmount.ToString());
                    break;
                default:
                    throw new NotImplementedException(offer.ToString());
            }
        }

        /// <summary>
        /// log the fact that user has taken advantage of an offer
        /// </summary>
        /// <param name="offers"></param>
        public void Offers_SetOfferCompleted(Offers offer)
        {
            switch (offer)
            {
                case Offers.Number1:
                    SetFlag(Flags.Offers_HasUsedServantOfferNumber1);
                    break;
                case Offers.Number2:
                    SetFlag(Flags.Offers_HasUsedServantOfferNumber2);
                    break;
                case Offers.FreeServants:
                    SetFlag(Flags.Offers_FreeServants,"0");
                    break;
                default:
                    throw new NotImplementedException(offer.ToString());
            }
        }
        #endregion

        #region  LoginType_ stuff
        public bool LoginType_isKong
        {
            get
            {
                return LoginType == Common.UserLoginType.Kong || LoginType == Common.UserLoginType.Kong_inferred;
            }
        }
        /// <summary>
        /// device ID login type via android
        /// </summary>
        public bool LoginType_isMobileDeviceIDLoginAndroid
        {
            get
            {
                return LoginType == Common.UserLoginType.Mobile_Android || LoginType == Common.UserLoginType.Mobile_Android_inferred;
            }
        }
        /// <summary>
        /// device ID login type via iOS
        /// </summary>
        public bool LoginType_isMobileDeviceIDLoginiOS
        {
            get
            {
                return LoginType == Common.UserLoginType.Mobile_iOS || LoginType == Common.UserLoginType.Mobile_iOS_inferred;
            }
        }
        /// <summary>
        /// amazon login on mobile device
        /// </summary>
        public bool LoginType_isAmazon
        {
            get
            {
                return LoginType == Common.UserLoginType.Mobile_Amazon || LoginType == Common.UserLoginType.Mobile_Amazon_inferred;
            }
        }
       
        public bool LoginType_isArmoredGames
        {
            get
            {
                return LoginType == Common.UserLoginType.ArmoredGames;
            }
        }
        /// <summary>
        /// device ID login type via android or iOS
        /// </summary>
        public bool LoginType_isMobileDeviceIDLogin
        {
            get
            {
                return LoginType_isMobileDeviceIDLoginAndroid || LoginType_isMobileDeviceIDLoginiOS;
            }
        }
        public Fbg.Common.UserLoginType LoginType {get; private set;}
        #endregion

        public String UserName { get; private set; }
        public Guid ID
        {
            get
            {
                return _userID;
            }
        }
        public float TimeZone
        {
            get
            {
                return _timeZone;
            }
        }
        public List<Player> Players
        {
            get
            {
                if (players == null)
                {
                    players = new List<Player>(drPlayers.Rows.Count);
                    int xp = this.XP;
                    foreach (DataRow dr in drPlayers.Rows)
                    {
                        

                        if (Realms.Realm((int)dr[User.CONSTS.PlayerBasicInfoColumnIndex.RealmID], true) != null)
                        {
                            players.Add(new Player(dr, xp, this));
                        }
                    }
                }
                return players;
            }
        }
        public Player PlayerByRealmID(int realmID)
        {
            foreach (Player p in Players)
            {
                if (p.Realm.ID == realmID)
                {
                    return p;
                }
            }
            return null;
        }
        public Player PlayerByID(int playerID)
        {
            return PlayerByID(playerID, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="rePopulateExtraInfp">if set to true, we will make a call to common db, to get the global XP, and then get the player object to repopulate it self, including updateing the XP</param>
        /// <returns></returns>
        public Player PlayerByID(int playerID, bool rePopulateExtraInfp)
        {
            foreach (Player p in Players)
            {
                if (p.ID == playerID)
                {
                    p.PopulateExtraInfoOnLogin(this.XP);
                    return p;
                }
            }
            return null;
        }

        public void RemovePlayerExcept(Player loginAsPlayer)
        {
            Players.RemoveAll(delegate(Player p) { return p.ID != loginAsPlayer.ID; });
        }

        /// <summary>
        /// note, this make a call to the db each time!! Use the Fbg.Bll.Player.XP which is cached
        /// </summary>
        public int XP
        {
            get
            {
                return DAL.User.GetUserXP(_userID);
            }
        }

        /// <summary>
        /// User selectable name to appear in the Thorne Room and to default to when registering at a realm. 
        /// Maybe String.Empty if it was never set. 
        /// </summary>
        public string GlobalPlayerName
        {
            get;
            private set;
        }

        /// <summary>
        /// User Avatar ID
        /// </summary>
        public int UserAvatarID
        {
            get;
            private set;
        }

        /// <summary>
        /// User Sex: Male is 1, Female is 0
        /// </summary>
        public int UserSex
        {
            get;
            private set;
        }

        /// <summary>
        /// note, this make a call to the db each time
        /// </summary>
        public int Credits
        {
            get
            {
                return DAL.User.GetUserCredits(_userID);
            }
        }
        public int TransferableCredits
        {
            get
            {
                return DAL.User.GetUserTransferableCredits(_userID);
            }
        }


        /// <summary>
        /// My return 0 if player never accepted any title on any realm, otherwise title ID. 
        /// </summary>
        public int MyHigestAchievedTitle
        {
            get 
            {
                int highestTitleID = 0;
                string highestTitleStr = (string)HasFlag_GetData(User.Flags.Misc_HighestTitleAchieved);

                if (!String.IsNullOrEmpty(highestTitleStr))
                {
                    highestTitleID = Convert.ToInt32(highestTitleStr);
                }

                return highestTitleID;
            }
        }

        /// <summary>
        /// Tells you if this player has a NP enabled in some realm. 
        /// NOTE!! This is a bit of a hack. Since we dont actually know, from Bll.Player object if player has NP enabled, 
        /// we check for a features that only comes with the NP, ie, the Fbg.Bll.CONSTS.PFs.RewardNow feature.
        /// </summary>
        public bool HasNPInSomeRealm
        {
            get
            {
                foreach (Player p in Players)
                {
                    if (p.PF_HasPF(Fbg.Bll.CONSTS.PFs.RewardNow))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        #region ISerializableToNameValueCollection2 Members

        public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
        {
            SerializeToNameValueCollection(col, String.Empty);
        }
        public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col, string prefix)
        {
            try
            {
                string pre = prefix ;

                if (col == null)
                {
                    ExceptionManager.Publish("Error in Player.SerializeToNameValueCollection. argument 'col' is null");
                }
                else
                {
                    col.Add(pre + "_userID", this._userID.ToString());
                    BaseApplicationException.AddAdditionalInformation(col, pre + "drPlayers.Count", drPlayers == null ? "null" : drPlayers.Rows.Count.ToString());
                    BaseApplicationException.AddAdditionalInformation(col, pre + "drPlayers", drPlayers);
                }

            }
            catch (Exception e)
            {
                ExceptionManager.Publish("Error in User.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + ExceptionManager.SerializeToString(e));
            }
        }

        #endregion
        public void BuyCredits(int creditsAmount)
        {
            DAL.User.BuyCredits(_userID, creditsAmount);
        }

        public void UseCredits(int creditsAmount, int eventType, int costField)
        {
            DAL.User.UseCredits(_userID, creditsAmount, eventType, costField);
        }

        public static void BuyCredits(Guid userID, int creditsAmount)
        {
            DAL.User.BuyCredits(userID, creditsAmount);
        }

        public void GetCreditsFromQuest(int creditsAmount)
        {
            DAL.User.GetCreditsFromQuest(_userID, creditsAmount);
        }


        /// <summary>
        /// returns false in case of some failure
        /// </summary>
        /// <param name="amountOfCredits"></param>
        /// <param name="playerToTransferToID"></param>
        /// <returns></returns>
        public bool TransferCredits(int amountOfCredits, int playerToTransferToID)
        {
            DAL.User.TransferCredits(amountOfCredits, playerToTransferToID, this.ID);
            return true;
        }

     

        /// <summary>
        /// Returns the list of player's invits from his firneds
        /// returned table is described by User.CONSTS.MyInvitesColIndex
        /// </summary>
        /// <returns></returns>
        public DataTable Invites_GetMyInvites(string facebookID)
        {
            return DAL.User.GetMyInvites(facebookID);
        }

        /// <summary>
        /// Returns the list of player's invited facebook friends (pending invitations).
        /// returned table is described by User.CONSTS.PendingInvitesColIndex
        /// </summary>
        /// <returns></returns>
        public DataTable Invites_GetPendingInvites()
        {
            return DAL.User.Invites_GetInvited(_userID);
        }



        public DataTable Invites_GetAcceptedInvites()
        {
            return DAL.User.Invites_GetAcceptedInvites(ID);
        }

        public void Invites_CancelInvite(int inviteID)
        {
            DAL.User.Invites_CancelInvite(ID, inviteID);
        }

        public void Invites_ClaimReward(int inviteID, string invitedPlayersFBID, PlayerOther invitedPlayer, Realm invitedPlayerRealm)
        {
            Guid userID = DAL.User.Invites_ClaimReward(ID, inviteID, invitedPlayersFBID);

            if (userID != Guid.Empty)
            {
                //
                // ok, so we gave 5 servants to the invited person - send him a message
                //
                Fbg.Bll.Mail.sendEmail(Fbg.Bll.CONSTS.SpecialPlayers.roe_team_PlayerId(invitedPlayerRealm)
                    , invitedPlayer.PlayerID.ToString()
                    , Properties.Misc.email2_subject
                    , String.Format(Properties.Misc.email2_body, invitedPlayer.TitleName)
                    , invitedPlayer.PlayerName, invitedPlayerRealm.ConnectionStr);
            }
        }
        public void Update(float timezone)
        {
            _timeZone = timezone;
            DAL.User.Update(ID ,timezone);
        }


        #region LogEvent
        /// <summary>
        /// returns null on success, Exception otherwise
        /// </summary>
        public Exception LogEvent(string message, string data)
        {
            return LogEvent(null, -99, message, data);
        }
        /// <summary>
        /// returns null on success, Exception otherwise
        /// </summary>
        /// <param name="eventID">Try to use a value from Fbg.Bll.CONSTS.UserLogEvents. If no suitable event id exists, 
        /// and you need to use one not listed there, always use number 10,000 or above</param>
        public Exception LogEvent(int eventID, string message, string data)
        {
            return LogEvent(null, eventID, message, data);
        }

        /// <summary>
        /// returns null on success, Exception otherwise
        /// </summary>
        /// <param name="eventID">Try to use a value from Fbg.Bll.CONSTS.UserLogEvents. If no suitable event id exists, 
        /// and you need to use one not listed there, always use number 10,000 or above</param>
        public Exception LogEvent(Player player, int eventID, string message, string data)
        {
            return User.LogEvent(this.ID, player, eventID, message, data);
        }

        /// <summary>
        /// returns null on success, Exception otherwise
        /// </summary>
        /// <param name="eventID">Try to use a value from Fbg.Bll.CONSTS.UserLogEvents. If no suitable event id exists, 
        /// and you need to use one not listed there, always use number 10,000 or above</param>
        public static Exception LogEvent(Guid userID, Player player, int eventID, string message, string data)
        {
            try
            {
                DAL.User.LogEvent(userID, player == null ? -99 : player.ID, eventID, message, data);
            }
            catch (Exception e)
            {
                return e;
            }
            return null;
        }
        /// <summary>
        /// returns null on success, Exception otherwise
        /// </summary>
        public Exception LogEvent(Player player, string message, string data)
        {
            return LogEvent(player, -99, message, data);
        }
        #endregion

        #region User Flags
        /// <summary>
        /// returns null if no flag, UpdatedOn value if it does
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        /// <remarks>This use flag code suffers from the same flaw as the Players flag code fixed in Fbg.Bll.Player in Change Set 3307
        /// where by player with 2 sessions opened could set the same flag twice (set it in one browser, but in the other session, it 
        /// appears he does not have this flag yet) which could give him some rewards multiple times etc. But this is 
        /// unlikely so we don't fix it now </remarks>
        public object HasFlag(Flags flag)
        {
            return HasFlag(flag, true);
        }

        /// <summary>
        /// returns null if no flag, UpdatedOn value if it does
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="useCache">if false, code will check the database for latest flag info</param>
        /// <returns></returns>
        public object HasFlag(Flags flag, bool useCache)
        {
            DataRow dr = dtFlags.Rows.Find(flag);

            if (!useCache)
            {
                object updatedOn;
                string data;
                DAL.User.GetFlag(ID, (int)flag, out data, out updatedOn);
                Flag_Update((int)flag, updatedOn, data, ref dr);
            }
            if (dr != null)
            {
                return dr[CONSTS.FlagsColIndex.UpdatedOn];
            }
            return null;
        }
        public object HasFlag_GetData(Flags flag)
        {
            DataRow dr = dtFlags.Rows.Find(flag);
            if (dr != null)
            {
                return dr[CONSTS.FlagsColIndex.Data];
            }
            return null;
        }
        /// <summary>
        /// internal helper function
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="updateOn"></param>
        /// <param name="dr"></param>
        private void Flag_Update(int flag, object updatedOn, string data, ref DataRow dr)
        {
            if (updatedOn == null)
            {
                // player does not have this flag at all so remove it from cache if it was there 
                if (dr != null)
                {
                    dr.Delete();
                    dtFlags.AcceptChanges();
                    dr = null;
                }
            }
            else
            {
                //
                // update the flag 
                if (dr == null)
                {
                    dr = dtFlags.Rows.Add(new object[] { flag, updatedOn, data });
                    dtFlags.AcceptChanges();
                }
                else
                {
                    dr[CONSTS.FlagsColIndex.UpdatedOn] = updatedOn;
                    dr[CONSTS.FlagsColIndex.Data] = data;
                    dtFlags.AcceptChanges();
                }

            }
        }
        /// <summary>
        /// </summary>
        /// <param name="flag"></param>
        /// <remarks>This use flag code suffers from the same flaw as the Players flag code fixed in Fbg.Bll.Player in Change Set 3307
        /// where by player with 2 sessions opened could set the same flag twice (set it in one browser, but in the other session, it 
        /// appears he does not have this flag yet) which could give him some rewards multiple times etc. But this is 
        /// unlikely so we don't fix it now </remarks>
        public void SetFlag(Flags flag, String data)
        {
            DateTime now = DateTime.Now ;
            DataRow dr = dtFlags.Rows.Find(flag);
            if (dr == null)
            {
                dtFlags.Rows.Add(new object[] { flag, now, data });
                dtFlags.AcceptChanges();
            }
            else
            {
                dr[CONSTS.FlagsColIndex.UpdatedOn] = now;
                dr[CONSTS.FlagsColIndex.Data] = data;
                dtFlags.AcceptChanges();

            }

            DAL.User.SetFlag((int)flag, this.ID, data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag"></param>
        /// <remarks>This use flag code suffers from the same flaw as the Players flag code fixed in Fbg.Bll.Player in Change Set 3307
        /// where by player with 2 sessions opened could set the same flag twice (set it in one browser, but in the other session, it 
        /// appears he does not have this flag yet) which could give him some rewards multiple times etc. But this is 
        /// unlikely so we don't fix it now </remarks>
        public void SetFlag(Flags flag)
        {
            this.SetFlag(flag, null);
        }

        /// <summary>
        /// get avatarBorderID from user flags combination
        /// first checks if user wants to show off the avatr using Flags.VIPLevel_ShowInChat 
        /// if they do want to show, then it returns Flags.VIPLevel
        /// </summary>
        public int getAvatarBorderIDFlag() {

            if (this.HasFlag_GetData(Flags.VIPLevel_ShowInChat) != null && 
                Convert.ToInt32(this.HasFlag_GetData(Flags.VIPLevel_ShowInChat)) == 1) {
                return Convert.ToInt32(this.HasFlag_GetData(Flags.VIPLevel));
            }

            return 0;
        }

        #endregion




        public void Gifts_AcceptGift(int giftID)
        {
            DAL.User.Gift_AcceptGift(ID, giftID);
            _giftRecentlyAccepted = giftID;
        }

        public void Gifts_AcceptByRequest(string requestID)
        {
            Gifts_AcceptByRequests(requestID);

            if (_giftsRecentlyAccepted.Length > 0)
            {
                _giftRecentlyAccepted = _giftsRecentlyAccepted[0];
            }
        }

        public void Gifts_AcceptByRequests(string requestID)
        {
            _giftsRecentlyAccepted = DAL.User.Gift_AcceptByRequest(ID, requestID);
        }
        /// <summary>
        /// returns Int32.MinValue if no gift recently accepted. 
        /// returns gift id is there is. 
        /// </summary>
        public int Gift_GiftRecentlyAccepted
        {
            get
            {
                return _giftRecentlyAccepted;
            }
        }

        public int[] Gift_GiftsRecentlyAccepted
        {
            get
            {
                return _giftsRecentlyAccepted;
            }
        }

        public void Gift_ClearRecentlyAcceptedFlag()
        {
            _giftRecentlyAccepted = Int32.MinValue;
        }

        /// <summary>
        /// Depreciated. use Fbg.Bll.Api.Items.GetMyItems instead
        /// </summary>
        /// <param name="realmID"></param>
        /// <returns></returns>
        public DataTable Gifts_GetMyGifts(int realmID)
        {
            return DAL.User.Gift_GetMyGifts(ID, realmID);
        }



        public void Gifts_UseGift(int giftID, int realmID)
        {
            DAL.User.Gifts_UseGift(ID, giftID, realmID);
        }
        public void Gifts_DeleteGift(int recordID)
        {
            DAL.User.Gifts_DeleteGift(ID, recordID);
        }

        /// <summary>
        /// User.CONST.TodaysGiftsColIndex describes the table returned
        /// </summary>
        /// <returns></returns>
        public DataTable Gifts_GetTodaysGifts()
        {
            return DAL.User.Gifts_GetTodaysGifts(_userID);
        }

        public void RefreshFriends(string friendsList)
        {
            friends = DAL.User.RefreshFriends(friendsList, this.ID);
        }

        /// <summary>
        /// users friends. 
        /// </summary>
        public List<Fbg.Common.UsersFriend> Friends
        {
            get
            {
                if (friends == null)
                {
                    friends = DAL.User.GetFriends(this.ID);
                }
                return friends;
            }
        }


        public void PayRealmEntryFee(Realm realm)
        {
            DAL.User.PayRealmEntryFee(this.ID, realm.EntryCost, realm.ID);
        }

        /// <summary>
        /// 
        /// </summary>
        public enum RecoveryEmailState
        {
            NoEmail = 0,
            Unverified = 1,
            Verified = 2
        }
        /// <summary>
        /// returns state of recovery email -- Verified can only occur once user has clicked verify link sent by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public RecoveryEmailState GetRecoveryEmailState(string email)
        {
            RecoveryEmailState state = RecoveryEmailState.NoEmail;
            if (email.ToLower() != Fbg.Bll.CONSTS.DummyEmail)
            {
                if (HasFlag(Flags.Misc_VerifiedEmail) != null)
                {
                    object data = HasFlag_GetData(User.Flags.Misc_VerifiedEmail);
                    state = (RecoveryEmailState)int.Parse((string)data);
                }
            }
            return state;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public void SetRecoveryEmailState(RecoveryEmailState state)
        {
            SetFlag(Flags.Misc_VerifiedEmail, string.Format("{0}", (int)state));
        }

        public bool MigrateToBDAAccount(string email)
        {
            if (DAL.User.MigrateToBDAAccount(this.ID, email) != 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// may return null, which means the code was already used
        /// </summary>
        /// <returns></returns>
        public string FriendCodeGet()
        {
            return Fbg.DAL.User.FriendCodeGet(this._userID);
        }

        public bool FriendCodeUse(string friendCode, int rewardTheInviterGet, int rewardTheInviteeGets)
        {
            return Fbg.DAL.User.FriendCodeUse(this._userID, friendCode, rewardTheInviterGet, rewardTheInviteeGets);
        }

        public int ChangeGlobalPlayerName(string newName)
        {
            return Fbg.DAL.User.ChangeGlobalPlayerName(this._userID, newName);
        }

        public int SetAvatarID(int AvatarID) {
            int ret = Fbg.DAL.User.SetAvatarID(this._userID, AvatarID);
            if(ret == 0){
                this.UserAvatarID = AvatarID;
            }           
            return ret;
        }

        public int SetSex(int Sex)
        {
            int errorcode; //0 is all good, 1 means invalid sex input
            if (Sex == 0 || Sex == 1)
            {
                this.UserSex = Sex;
                Fbg.DAL.User.SetSex(this._userID, Sex);
                errorcode = 0;
            }
            else {
                errorcode = 1;
            }

            return errorcode;
        }

        public int toggleDisplayChatVIP(int status)
        {
            if (status == 1)
            {
                this.SetFlag(Flags.VIPLevel_ShowInChat, "1");
            }
            else {
                this.SetFlag(Flags.VIPLevel_ShowInChat, "0");
            }
            return Convert.ToInt32(this.HasFlag_GetData(Flags.VIPLevel_ShowInChat));
        }

        public void ThroneRoomInvalidate()
        {
            _throneRoom = null;
        }
        public ThroneRoom.MyThroneRoom ThroneRoom
        {
            get
            {
                if (_throneRoom == null)
                {
                    _throneRoom = new ThroneRoom.MyThroneRoom(this);
                }
                return _throneRoom;
            }
        }

        List<Fbg.Bll.Items2.Item2> _items2;
        public List<Fbg.Bll.Items2.Item2> Items2
        {
            get
            {
                if (_items2 == null)
                {                   
                    DataTable dt = Fbg.DAL.User.Items2_GetItems(this.ID);

                    _items2 = new List<Items2.Item2>(dt.Rows.Count);
                    foreach (DataRow dr in dt.Rows)
                    {                      
                        //
                        // Add item based on type
                        //
                        if (!(dr[CONSTS.Items2ColIndes.PFFeatureID] is DBNull))
                        {
                            Items2.Add(new Fbg.Bll.Items2.Item2_PFWithDuration(
                                (long)dr[CONSTS.Items2ColIndes.ItemID]
                                , this
                                , dr[CONSTS.Items2ColIndes.PlayerID] is DBNull ? 0 : (int)dr[CONSTS.Items2ColIndes.PlayerID]
                                , dr[CONSTS.Items2ColIndes.ExpiresOn] is DBNull ? null : (DateTime?)dr[CONSTS.Items2ColIndes.ExpiresOn]
                                , (int)dr[CONSTS.Items2ColIndes.PFFeatureID]
                                , new TimeSpan(0, (int)dr[CONSTS.Items2ColIndes.DurationInMin], 0)));
        }
                        else if (!(dr[CONSTS.Items2ColIndes.UnitTypeID] is DBNull))
                        {
                            Items2.Add(new Fbg.Bll.Items2.Item2_Troops(
                                (long)dr[CONSTS.Items2ColIndes.ItemID]
                                , this
                                , dr[CONSTS.Items2ColIndes.PlayerID] is DBNull ? 0 : (int)dr[CONSTS.Items2ColIndes.PlayerID]
                                , dr[CONSTS.Items2ColIndes.ExpiresOn] is DBNull ? null : (DateTime?)dr[CONSTS.Items2ColIndes.ExpiresOn]
                                , (int)dr[CONSTS.Items2ColIndes.UnitTypeID]
                                , (int)dr[CONSTS.Items2ColIndes.Amount]));
                        }
                        else if (!(dr[CONSTS.Items2ColIndes.SilverAmount] is DBNull))
                        {
                            Items2.Add(new Fbg.Bll.Items2.Item2_Silver(
                                (long)dr[CONSTS.Items2ColIndes.ItemID]
                                , this
                                , dr[CONSTS.Items2ColIndes.PlayerID] is DBNull ? 0 : (int)dr[CONSTS.Items2ColIndes.PlayerID]
                                , dr[CONSTS.Items2ColIndes.ExpiresOn] is DBNull ? null : (DateTime?)dr[CONSTS.Items2ColIndes.ExpiresOn]
                                , (int)dr[CONSTS.Items2ColIndes.SilverAmount]));
                        }
                        else if (!(dr[CONSTS.Items2ColIndes.MinutesAmount] is DBNull))
                        {
                            Items2.Add(new Fbg.Bll.Items2.Item2_BuildingSpeedup(
                                (long)dr[CONSTS.Items2ColIndes.ItemID]
                                , this
                                , dr[CONSTS.Items2ColIndes.PlayerID] is DBNull ? 0 : (int)dr[CONSTS.Items2ColIndes.PlayerID]
                                , dr[CONSTS.Items2ColIndes.ExpiresOn] is DBNull ? null : (DateTime?)dr[CONSTS.Items2ColIndes.ExpiresOn]
                                , (int)dr[CONSTS.Items2ColIndes.MinutesAmount]));
                        }
                        else if (!(dr[CONSTS.Items2ColIndes.ResearchSpeedUpMinutesAmount] is DBNull))
                        {
                            Items2.Add(new Fbg.Bll.Items2.Item2_ResearchSpeedup(
                                (long)dr[CONSTS.Items2ColIndes.ItemID]
                                , this
                                , dr[CONSTS.Items2ColIndes.PlayerID] is DBNull ? 0 : (int)dr[CONSTS.Items2ColIndes.PlayerID]
                                , dr[CONSTS.Items2ColIndes.ExpiresOn] is DBNull ? null : (DateTime?)dr[CONSTS.Items2ColIndes.ExpiresOn]
                                , (int)dr[CONSTS.Items2ColIndes.ResearchSpeedUpMinutesAmount]));
                        }

                    }
                }

                return _items2;
            }
        }

        private int _items2_cacheversion=1;
        public int Items2_cacheversion
        {
            get
            {
                return _items2_cacheversion;
            }        
        }
        public void Items2_Inalidate()
        {
            _items2 = null;
            _items2_cacheversion++;

        }

        public void UserStats_Invalidate()
        {
            //TODO - review

        }
    }
}
