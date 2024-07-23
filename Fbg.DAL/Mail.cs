using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Data;

namespace Fbg.DAL
{
    public class Mail
    {
        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.Mail");

        internal class MessageDetailTableIndex
        {
            public static int MessageDetail = 0;
        }

        internal class TempUsersTableIndex
        {
            public static int TempUsers = 0;
        }

        public static DataSet CheckRecipientsLight(string playerNames,string connectionStr, int playerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("qCheckPlayers", new object[] { playerID, playerNames, 1, 1 });
            }
            catch(Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qCheckPlayers", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerNames", playerNames);
                ex.AddAdditionalInformation("playerID", playerID);
                throw ex;
            }
        }
        public static DataSet CheckRecipientsDetail(string playerNames, string connectionStr, int playerID, bool hasPF)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("qCheckPlayers", new object[] { playerID, playerNames, 2, hasPF });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qCheckPlayers", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerNames", playerNames);
                ex.AddAdditionalInformation("hasPF", hasPF);
                ex.AddAdditionalInformation("playerID", playerID);
                throw ex;
            }
        }

        public static DataSet CheckRecipients(string playerNames, string connectionStr)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("qCheckPlayers", new object[] { playerNames, 2 });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qCheckPlayers", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerNames", playerNames);
                throw ex;
            }
        }
        public static int sendEmail(int SenderID, string RecipientIDs, string subject, string message,string RecipientNames,string connectionStr)
        {
            TRACE.InfoLine("in 'iSendMail()'");
            Database db;

            try
            {
                db = new DB(connectionStr);;
                return db.ExecuteNonQuery("iSendMail", new object[] { SenderID, RecipientIDs, subject, message, RecipientNames });
            }
            catch(Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iSendMail", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("SenderID", SenderID.ToString());
                ex.AdditionalInformation.Add("RecipientIDs", RecipientIDs.ToString());
                ex.AdditionalInformation.Add("Subject", subject.ToString());
                ex.AdditionalInformation.Add("Message", message.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recipientID"></param>
        /// <param name="connectionStr"></param>
        /// <param name="maxDaysOld"></param>
        /// <param name="folderID">pass -1 if no folder is selected</param>
        /// <returns></returns>
        public static DataSet getInbox(int recipientID, string connectionStr, int maxDaysOld, int folderID)
        {
            TRACE.InfoLine("in 'qInbox()'");
            Database db;

            try
            {
                db = new DB(connectionStr);
                object folder = folderID == -1 ? null : (object)folderID;

                return db.ExecuteDataSet("qMessages", new object[] { recipientID, maxDaysOld, folder });
               
            }
            catch(Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qMessages", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("recipientID", recipientID.ToString());
                ex.AddAdditionalInformation("maxDaysOld", maxDaysOld);
                ex.AddAdditionalInformation("folderID", folderID);
                throw ex;
            }
        }


        /// <summary>
        /// Like get inbox, but also gets mail in folders (regardless of time max)
        /// </summary>
        /// <param name="recipientID"></param>
        /// <param name="connectionStr"></param>
        /// <param name="maxDaysOld"></param>
        /// <param name="folderID"></param>
        /// <returns></returns>
        public static DataSet getInboxAll(int recipientID, string connectionStr, int maxDaysOld, int folderID)
        {
            TRACE.InfoLine("in 'qInbox()'");
            Database db;

            try
            {
                db = new DB(connectionStr);
                object folder = folderID == -1 ? null : (object)folderID;
                               
                return db.ExecuteDataSet("qMessagesAll", new object[] { recipientID, maxDaysOld, folder });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qMessagesAll", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("recipientID", recipientID.ToString());
                ex.AddAdditionalInformation("maxDaysOld", maxDaysOld);
                ex.AddAdditionalInformation("folderID", folderID);
                throw ex;
            }
        }

        public static DataSet getSentItems(int recipientID, string connectionStr, int maxDaysOld)
        {
            TRACE.InfoLine("in 'qSent()'");
            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("qSent", new object[] { recipientID, maxDaysOld });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qSent", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("recipientID", recipientID.ToString());
                ex.AddAdditionalInformation("maxDaysOld", maxDaysOld);
                throw ex;
            }
        }

        public static DataTable getMessageDetail(string connectionStr, int recordID, int loggedInPlayerID)
        {
            Database db;

            try
            {
                db = new DB(connectionStr);
                return db.ExecuteDataSet("qMessageDetail", new object[] { loggedInPlayerID, recordID }).Tables[MessageDetailTableIndex.MessageDetail];
            }
            catch(Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qMessageDetail", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("recordID", recordID.ToString());
                ex.AdditionalInformation.Add("loggedInPlayerID", loggedInPlayerID.ToString());
                throw ex;
            }
        }

        public static int deleteMessage(string recordID, string connectionStr, int playerID)
        {
            TRACE.InfoLine("in 'dDeleteMessage()'");
            Database db;

            try
            {
                db = new DB(connectionStr);;
                return db.ExecuteNonQuery("dDeleteMessage", new object[] { playerID, recordID });
            }
            catch(Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qDeleteMessage", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("playerID", playerID.ToString());
                ex.AdditionalInformation.Add("recordID", recordID.ToString());
                throw ex;
            }
        }

        public static Fbg.Common.Mail.BlockPlayerResult  BlockPlayer( string connectionStr, int PlayerID, string BlockedPlayerName)
        {
            TRACE.InfoLine("in 'BlockPlayer()'");
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                return (Fbg.Common.Mail.BlockPlayerResult)Enum.Parse(typeof(Fbg.Common.Mail.BlockPlayerResult), db.ExecuteScalar("iBlockPlayer", new object[] { PlayerID, BlockedPlayerName }).ToString());
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling iBlockPlayer", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("BlockedPlayerName", BlockedPlayerName);
                throw ex;
            }
        }

        public static void UnBlockPlayer(string connectionStr, int PlayerID, int  BlockedPlayerID)
        {
            TRACE.InfoLine("in 'UnBlockPlayer()'");
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                 db.ExecuteNonQuery("dUnBlockPlayer", new object[] { PlayerID, BlockedPlayerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling dUnBlockPlayer", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("BlockedPlayerID", BlockedPlayerID.ToString());
                throw ex;
            }
        }

        public static DataTable GetBlockedPlayers(string connectionStr, int PlayerID)
        {
            TRACE.InfoLine("in 'GetBlockedPlayers()'");
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                DataSet ds= db.ExecuteDataSet("qBlockedPlayers", new object[] { PlayerID });
                return ds.Tables[0];
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qBlockedPlayers", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                throw ex;
            }
        }

        public static void MarkMessageAsRead(string connectionStr, string MessageIDs, int PlayerID)
        {
            TRACE.InfoLine("in 'MarkMessageAsRead()'");
            Database db;

            try
            {
                db = new DB(connectionStr); ;
                db.ExecuteNonQuery("uMessageAsRead", new object[] { MessageIDs , PlayerID });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uMessageAsRead", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", PlayerID.ToString());
                ex.AdditionalInformation.Add("MessageIDs", MessageIDs);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pplayerID"></param>
        /// <param name="connectionStr"></param>
        /// <param name="recordID"></param>
        /// <param name="folderID">pass in -1 if you want to move the message to the inbox</param>
        public static void MoveMessageToFolder(int playerID, string connectionStr, List<int> recordIDs, int folderID)
        {
            Database db;

            string record_IDs=null;
            try
            {
                record_IDs = Report.ListToString(recordIDs);


                db = new DB(connectionStr);
                object fid = folderID == -1 ? null : (object)folderID;
                db.ExecuteNonQuery("uMessageMove", new object[] { playerID, record_IDs, fid });
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling uMessageMove", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                ex.AdditionalInformation.Add("PlayerID", playerID.ToString());
                ex.AddAdditionalInformation("record_IDs", record_IDs);
                ex.AddAdditionalInformation("folderID", folderID);
                throw ex;
            }
        }
    }
}