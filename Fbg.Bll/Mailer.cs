using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Gmbc.Common;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Threading;

using Fbg.DAL;
using System.Net.Mail;
using Amazon.SimpleEmail.Model;
using Amazon.SimpleEmail;

namespace Fbg.Bll
{

    public class Mailer : GmbcBaseClass
    {
        String _awsAccessKey;
        String _awsSecretKey;

        public Mailer(string awsAccessKey, string awsSecretKey)
        {
            _awsAccessKey = awsAccessKey;
            _awsSecretKey = awsSecretKey;
        }

        public void SendClanNotificationEmail(string toEmail, Player invitingPlayer, PlayerOther invitedPlayer)
        {
            try
            {
                if (!String.IsNullOrEmpty(toEmail) && toEmail != Fbg.Bll.CONSTS.DummyEmail)
                {
                    MailMessage msg = new MailMessage(
                        new MailAddress("playeralerts@realmofempires.com", "Realm of Empires Alerts")
                        , new MailAddress(toEmail));
                    msg.Subject = string.Format(Properties.Misc.email_ClanInvite_Subject, invitingPlayer.TitleName, invitingPlayer.Name);
                    msg.Body = Properties.Misc.email_header + String.Format(Properties.Misc.email_ClanInvite_Body
                        , invitingPlayer.Clan.Name, invitingPlayer.TitleName, invitingPlayer.Name, invitedPlayer.TitleName, invitedPlayer.PlayerName)
                        + String.Format(Properties.Misc.email_footer, 5);
                    msg.IsBodyHtml = true;
                    //msg.Bcc.Add(new MailAddress("roetest30@gmail.com"));
                    SendEmailViaAmazon(msg, toEmail);
                }
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error in SendClanNotificationEmail", e);
                ex.AddAdditionalInformation("toEmail", toEmail);
                ex.AddAdditionalInformation("invitingPlayer", invitingPlayer);
                ex.AddAdditionalInformation("Properties.Misc.email_ClanInvite_Body", Properties.Misc.email_ClanInvite_Body);

                throw ex;
            }
        }

        public void SendEmailViaAmazon(MailMessage msg, string toemail)
        {
            String source = "Realm of Empires < playeralerts@realmofempires.com >";      
        
            //var oDestination = new Destination().WithToAddresses(new List<string>() { toemail });
            //if (msg.Bcc.Count > 0 ) {
            //    oDestination.BccAddresses = new List<string>() { msg.Bcc[0].Address };
            //}


            //// Create the email subject.
            //var oSubject = new Amazon.SimpleEmail.Model.Content().WithData(msg.Subject);

            //// Create the email body.
            //var oTextBody = new Amazon.SimpleEmail.Model.Content().WithData(msg.Body);
            //var oBody = new Body().WithHtml(oTextBody);


            //// Create and transmit the email to the recipients via Amazon SES.
            //var oMessage = new Message().WithSubject(oSubject).WithBody(oBody);
            //var request = new SendEmailRequest().WithSource(source).WithDestination(oDestination).WithMessage(oMessage);

            try
            {
                using (var client = new AmazonSimpleEmailServiceClient(_awsAccessKey, _awsSecretKey, Amazon.RegionEndpoint.USEast1))
                {


                    var sendRequest = new SendEmailRequest
                    {
                        Source = source,
                        Destination = new Destination
                        {
                            ToAddresses =
                                            new List<string> { toemail }
                        },
                        Message = new Message
                        {
                            Subject = new Content(msg.Subject),
                            Body = new Body
                            {
                                Html = new Content
                                {
                                    Charset = "UTF-8",
                                    Data = msg.Body
                                },
                                Text = new Content
                                {
                                    Charset = "UTF-8",
                                    Data = msg.Body
                                }
                            }
                        }
                    };

                    if (msg.Bcc.Count > 0)
                    {
                        sendRequest.Destination.BccAddresses = new List<string>() { msg.Bcc[0].Address };
                    }

                    client.SendEmail(sendRequest);
                }
            }
            catch (Exception e)
            {
                if (utils.isBadEmailException(e))
                {
                    Fbg.DAL.utils.SetInvalidAddress(toemail, null, null);
                    TRACE.InfoLine(String.Format("Email Bounced - {0}", toemail));
                }
                else
                {
                    throw e;
                }
            }
        }
        /// <summary>
        /// Send an email to player to verify recovery email address
        /// </summary>
        /// <param name="host"></param>
        /// <param name="uid"></param>
        /// <param name="email"></param>
        /// <param name="playerName"></param>
        /// <param name="realmName"></param>
        /// <returns></returns>
        public bool SendRecoveryEmailVerification(string host, string uid, string email, string playerName, string realmName, string bcc)
        {
            return SendRecoveryEmailVerification(uid, email, bcc
                , String.Format(Properties.Misc.email_verifyrecoveryemailsubject)
                    , string.Format(Properties.Misc.email_verifyrecoveryemailbody, playerName, realmName, host, uid));            
        }
        /// <summary>
        /// sends an email asking player to verify email at registration
        /// </summary>
        /// <param name="host"></param>
        /// <param name="uid"></param>
        /// <param name="email"></param>
        /// <param name="bcc"></param>
        /// <returns></returns>
        public bool Send_VerifyEmail_email(string host, string uid, string email, string bcc)
        {
            return SendRecoveryEmailVerification(uid, email, bcc
                , String.Format(Properties.Misc.email_verifyemail_onregistration_subject)
                    , string.Format(Properties.Misc.email_verifyemail_onregistration_body, host, uid));            
        }
        private bool SendRecoveryEmailVerification(string uid, string email, string bcc, string subject, string body)
        {
            bool rc = false;
            try
            {
                MailMessage msg = new MailMessage(
                    new MailAddress("playeralerts@realmofempires.com", "Realm of Empires"),
                    new MailAddress(email));
                msg.Subject = subject;
                msg.Body = Properties.Misc.email_header2
                    + body
                    + Properties.Misc.email_verifyrecoveryemail_footer;
                msg.IsBodyHtml = true;
                if (!String.IsNullOrEmpty(bcc))
                {
                    msg.Bcc.Add(new MailAddress(bcc));
                }
                SendEmailViaAmazon(msg, email);
                rc = true;
            }
            catch (Exception x)
            {
                BaseApplicationException ex = new BaseApplicationException("Error in SendRecoveryEmailVerification", x);
                ex.AddAdditionalInformation("email", email);
                ex.AddAdditionalInformation("uid", uid);
                throw ex;
            }
            return rc;
        }

        public bool Send_ResetPassword_email(string userID, string email, string bcc, String host)
        {
            bool rc = false;
            string link = string.Format("{0}/login_resetpassword2.aspx?uid={1}", host, userID);
            try
            {
                MailMessage msg = new MailMessage(
                    new MailAddress("playeralerts@realmofempires.com", "Realm of Empires"),
                    new MailAddress(email));
                msg.Subject = Properties.Misc.email_resetpassword_subject;
                msg.Body = Properties.Misc.email_header2
                    + string.Format(Properties.Misc.email_resetpassword_body, link) 
                    + Properties.Misc.email_verifyrecoveryemail_footer;
                msg.IsBodyHtml = true;
                if (!String.IsNullOrEmpty(bcc))
                {
                    msg.Bcc.Add(new MailAddress(bcc));
                }
                SendEmailViaAmazon(msg, email);
                rc = true;
            }
            catch (Exception x)
            {
                BaseApplicationException ex = new BaseApplicationException("Error in Send_ResetPassword_email", x);
                ex.AddAdditionalInformation("email", email);
                ex.AddAdditionalInformation("uid", userID);
                throw ex;
            }
            return rc;
        }

        public bool Send_ResetPasswordDone_email(string newpassword, string email, string bcc)
        {
            bool rc = false;
            
            try
            {
                MailMessage msg = new MailMessage(
                    new MailAddress("playeralerts@realmofempires.com", "Realm of Empires"),
                    new MailAddress(email));
                msg.Subject = "Your password has been reset";
                msg.Body = Properties.Misc.email_header2
                    + string.Format(Properties.Misc.email_resetpasswordDone_body, newpassword)
                    + Properties.Misc.email_verifyrecoveryemail_footer;
                msg.IsBodyHtml = true;
                if (!String.IsNullOrEmpty(bcc))
                {
                    msg.Bcc.Add(new MailAddress(bcc));
                }
                SendEmailViaAmazon(msg, email);
                rc = true;
            }
            catch (Exception x)
            {
                BaseApplicationException ex = new BaseApplicationException("Error in Send_ResetPasswordDone_email", x);
                ex.AddAdditionalInformation("email", email);
                throw ex;
            }
            return rc;
        }



        /// <summary>
        /// send an email to user to seek final verification for the account change
        /// </summary>
        /// <param name="host"></param>
        /// <param name="ar"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool SendAccountRecoveryVerification(string host, utils.AccountRecovery ar, string email, string bcc)
        {
            bool rc = false;
            try
            {
                MailMessage msg = new MailMessage(
                    new MailAddress("playeralerts@realmofempires.com", "Realm of Empires"),
                    new MailAddress(email));
                msg.Subject = String.Format(Properties.Misc.email_verifyrecoveraccountsubject, ar.PlayerNames);
                if (ar.State == utils.AccountRecovery.eState.FacebookAccount)
                {
                    msg.Body = Properties.Misc.email_header2
                        + string.Format(Properties.Misc.email_verifyrecoveraccountbodyforfb, ar.PlayerNames)
                        + Properties.Misc.email_verifyrecoveryemail_footer;
                }
                else
                {
                    msg.Body = Properties.Misc.email_header2
                        + string.Format(Properties.Misc.email_verifyrecoveraccountbody, ar.PlayerNames, host, ar.ID, ar.UserId)
                        + Properties.Misc.email_verifyrecoveryemail_footer;
                }
                msg.IsBodyHtml = true;
                if (!String.IsNullOrEmpty(bcc))
                {
                    msg.Bcc.Add(new MailAddress(bcc));
                }

                SendEmailViaAmazon(msg, email);
                rc = true;
            }
            catch (Exception x)
            {
                BaseApplicationException ex = new BaseApplicationException("Error in SendAccountRecoveryVerification", x);
                ex.AddAdditionalInformation("email", email);
                ex.AddAdditionalInformation("ID", ar.ID);
                throw ex;
            }
            return rc;
        }
    }
}