using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Fbg.Bll
{
    public class Mail
    {
        public class CONSTS
        {
            /// <summary>
            /// tells you what type of a message this is. Is it my sent message or a inbound message
            /// </summary>
            public enum MessageType
            {
                /// <summary>
                /// sent message. Message I sent out. 
                /// </summary>
                Sent = 0,
                /// <summary>
                /// inbound item. message addressed to me. 
                /// </summary>
                Inbox = 1
            }
            public enum MailActionCreateType
            {
                Reply,
                Forward,
                ReplyAll,
                SendToAllMembers,
                CreateNewMail,
                CreateNewMail_PrepopulateTo
            }

            public class InboxMessageDetailColIndex
            {
                public static int SenderID = 1;
                public static int SenderName = 2;
                public static int Subject = 3;
                public static int Message = 4;
                public static int SentTime = 5;
                public static int RecipientNames = 6;
                public static int RecordID = 7;
                public static int MessageType = 8;
                public static int SenderPN = 9;
                public static int NextRec = 10;
                public static int PrevRec = 11;
            }

            /// <summary>
            /// describes the columns of table returned by getInbox
            /// </summary>
            public class InboxColumnIndex
            {
                //public static int MessageID = 0;
                public static int Subject = 1;
                public static int TimeSent = 2;
                public static int SenderID = 3;
                public static int SenderName = 4;
                public static int IsViewed = 5;
                public static int RecordID = 6;
                public static int FolderID = 7;
            }

            /// <summary>
            /// describes the columns of table returnd by getSentItems
            /// </summary>
            public class SentColumnIndex
            {
                //public static int MessageID = 0;
                public static int SenderID = 1;
                public static int SenderName = 2;
                public static int Subject = 3;
                public static int TimeSent = 4;
                public static int ReceiverNames = 5;
                public static int IsViewed = 6;
                public static int RecordID = 7;
            }

            /// <summary>
            /// describes the tables returned by userExists
            /// </summary>
            public class CheckRecipientsTableIndex
            {
                public static int PlayerIDDetails = 0;
                public static int PlayerNameDetails = 1;
            }

            public const int ArchiveMessagesInXDays = 14;

            /// <summary>
            /// describes the columns of table returnd by getSentItems
            /// </summary>
            public class CheckRecpDetColNames
            {
                public static string PlayerID = "PlayerID";
                public static string Name = "Name";
                public static string Points = "TotalPoints";
                public static string ClanName = "ClanName";
                public static string ClanID = "ClanID";
                public static string DiplomacyStatus = "StatusID";
                public static string PartialNote = "PartialNote";
                public static string Title = "TitleID";
                public static string Sex = "Sex";
            }

            static public string HiddenRecipients = "(names hidden)";
        }

        /// <summary>
        /// tables returned described in Fbg.Bll.CONSTS.CheckRecipientsTableIndex
        /// </summary>
        public static DataSet CheckRecipientsLight(Player player, string RecipientsName)
        {
            return Fbg.DAL.Mail.CheckRecipientsLight(RecipientsName, player.Realm.ConnectionStr, player.ID);
        }
        /// <summary>
        /// tables returned described in Fbg.Bll.CONSTS.CheckRecipientsTableIndex
        /// table Fbg.Bll.CONSTS.CheckRecipientsTableIndex.PlayerIDDetails columns described by 
        /// Fbg.Bll.CONSTS.CheckRecpDetColNames
        /// </summary>
        /// <param name="player"></param>
        /// <param name="RecipientsName"></param>
        /// <returns></returns>
        public static DataSet CheckRecipientsDetail(Player player, string RecipientsName, bool hasPF)
        {
            return Fbg.DAL.Mail.CheckRecipientsDetail(RecipientsName, player.Realm.ConnectionStr, player.ID, hasPF);
        }

        public static int sendEmail(int senderID,string RecipientIDs,string Subject,string Message,string RecipientNames,string ConnectionStr)
        {
            return Fbg.DAL.Mail.sendEmail(senderID, RecipientIDs, Subject, Message, RecipientNames, ConnectionStr);
        }

        /// <summary>
        /// returned table is described by Fbg.Bll.Mail.CONSTS.InboxColumnIndex
        /// </summary>
        public static DataSet getInbox(int RecipientID, string ConnectionStr, bool showArchived)
        {
            return getInbox(RecipientID, ConnectionStr, showArchived, -1);
        }

      
        /// <summary>
        /// returned table is described by Fbg.Bll.Mail.CONSTS.InboxColumnIndex
        /// </summary>
        public static DataSet getInbox(int RecipientID, string ConnectionStr, bool showArchived, int folderID)
        {
            return Fbg.DAL.Mail.getInbox(RecipientID, ConnectionStr, showArchived ? 9999 : CONSTS.ArchiveMessagesInXDays, folderID);
        }

        /// <summary>
        /// All version
        /// returned table is described by Fbg.Bll.Mail.CONSTS.InboxColumnIndex
        /// </summary>
        public static DataSet getInboxAll(int RecipientID, string ConnectionStr, bool showArchived)
        {
            //return getInbox(RecipientID, ConnectionStr, showArchived, -1);
            return Fbg.DAL.Mail.getInboxAll(RecipientID, ConnectionStr, showArchived ? 9999 : CONSTS.ArchiveMessagesInXDays, -1);
        }

        /// <summary>
        /// returned table is described by Fbg.Bll.Mail.CONSTS.SentColumnIndex
        /// </summary>
        public static DataSet getSentItems(int RecipientID, string ConnectionStr, bool showArchived)
        {
            return Fbg.DAL.Mail.getSentItems(RecipientID, ConnectionStr, showArchived ? 9999 : CONSTS.ArchiveMessagesInXDays);
        }
        //public static DataTable getMessageDetail(int MessageID,int MessageType, int RecipientID, int SenderID, string ConnectionStr)
        //{
        //    return Fbg.DAL.Mail.getMessageDetail(MessageID, MessageType, RecipientID, SenderID, ConnectionStr);
        //}
        /// <summary>
        /// returned table is described by Fbg.Bll.Mail.CONSTS.InboxMessageDetailColIndex
        /// If returned table has no rows, then no such message has been found. 
        /// </summary>
        public static DataTable getMessageDetail(Fbg.Bll.Player player, int recordID)
        {
            return Fbg.DAL.Mail.getMessageDetail(player.Realm.ConnectionStr, recordID, player.ID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordIDs">could be a comma delimited list of record ids</param>
        /// <param name="connectionStr"></param>
        /// <param name="PlayerID"></param>
        /// <returns></returns>
        public static int deleteMessage(string recordIDs, string connectionStr, int PlayerID)
        {
            return Fbg.DAL.Mail.deleteMessage(recordIDs, connectionStr, PlayerID);
        }

        public static bool CanBlockPlayer(Player player, string playerNameToBlock)
        {
            if (string.Compare( playerNameToBlock , player.Name) == 0 
                || Fbg.Bll.utils.IsSpecialPlayer(playerNameToBlock))
            {
                return false;
            }
            return true;
        }
        public static Fbg.Common.Mail.BlockPlayerResult BlockPlayer(Player Owner, string BlockedPlayerName)
        {
            if (Owner.Name == BlockedPlayerName)
            {
                return Fbg.Common.Mail.BlockPlayerResult.Cannot_Block_Yourself;
            }
            if (utils.IsSpecialPlayer(BlockedPlayerName))
            {
                return Fbg.Common.Mail.BlockPlayerResult.Blocked_Player_Not_Exist;
            }
            Fbg.Common.Mail.BlockPlayerResult res =  Fbg.DAL.Mail.BlockPlayer(Owner.Realm.ConnectionStr, Owner.ID, BlockedPlayerName);

            if (res == Common.Mail.BlockPlayerResult.Success)
            {
                PlayerOther po = PlayerOther.GetPlayer(Owner.Realm, BlockedPlayerName, Owner.ID);
                Fbg.Bll.Mail.sendEmail(Fbg.Bll.CONSTS.SpecialPlayers.roe_team_PlayerId(Owner.Realm), po.PlayerID.ToString(), Properties.Misc.email_youHaveBeenBlocked_subject
                    , Properties.Misc.email_youHaveBeenBlocked_body, BlockedPlayerName, Owner.Realm.ConnectionStr);
            }

            return res;
        }
        public static void  UnBlockPlayer(Player Owner, int BlockedPlayerID)
        {
           Fbg.DAL.Mail.UnBlockPlayer(Owner.Realm.ConnectionStr, Owner.ID, BlockedPlayerID);
        }
        public static DataTable  GetBlockedPlayers(Player Owner)
        {
            return Fbg.DAL.Mail.GetBlockedPlayers(Owner.Realm.ConnectionStr, Owner.ID);
        }
        public static string GetBlockPlayerMessage(Fbg.Common.Mail.BlockPlayerResult result)
        {
            string msg = string.Empty;
            switch (result)
            {
                case Fbg.Common.Mail.BlockPlayerResult.Blocked_Player_Not_Exist:
                    msg = "This player does not exists.";
                    break;
                case Fbg.Common.Mail.BlockPlayerResult.Cannot_Block_Yourself:
                    msg = "You can't block yourself";
                    break;
                case Fbg.Common.Mail.BlockPlayerResult.Player_Already_Blocked:
                    msg = "Player already blocked.";
                    break;
                case Fbg.Common.Mail.BlockPlayerResult.Success:
                    break;
                default:
                    throw new Exception("Unrecognized value of Mail.BlockPlayerResult:" + result.ToString());
            }
            return msg;
        }
        /// <summary>
        /// mark selected messages as read
        /// </summary>
        /// <param name="connectionStr"></param>
        /// <param name="PlayerID"></param>
        /// <param name="MessageIDs">ID's of messages marked as read separated by comma ',' ex:(1,2,3,)</param>
        public static void MarkMessageAsRead(string connectionStr, int PlayerID, string MessageIDs)
        {
            DAL.Mail.MarkMessageAsRead(connectionStr, MessageIDs, PlayerID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="messageType"></param>
        /// <param name="recordIDs">comma seperated list of record ids</param>
        /// <param name="folderID">pass in -1 if you want to move the message to the inbox</param>
        /// <param name="_hasPF"></param>
        public static void MoveMessageToFolder(Player player, CONSTS.MessageType messageType, List<int> recordIDs, int folderID, bool _hasPF)
        {
            if (folderID == 0)
            {
                // cannot move anything to sent folder
                throw new ArgumentException("folderid = 0 meaning move to sent items. not allowed");
            }
            if (messageType != CONSTS.MessageType.Inbox)
            {
                throw new ArgumentException("only can move inbound messages");
            }
            if (!_hasPF)
            {
                throw new ArgumentException("no pf");
            }
            DAL.Mail.MoveMessageToFolder(player.ID, player.Realm.ConnectionStr, recordIDs, folderID);
        }
    }
}