using System;
using System.Net.Mail;
using Gmbc.Common.Diagnostics.ExceptionManagement;


namespace Gmbc.Common.Diagnostics.Logging
{
	/// <summary>
	/// Allow you to send a string message to an email address. This is another way of 'logging' an exception
	/// </summary>
	public class EmailLogger
	{
		private EmailLogger() {
		}

		/// <summary>
		/// Email a log entry. 'To' email adress, 'From' email address and 'Subject' must be specified in .config file. If 
		/// this information is not specified in the .config file, BaseApplicationException is raised.
		/// Please also ensure that the smtp service is properly configured.
		/// </summary>
		/// <exception cref="BaseApplicationException">Raised if ToEmailAddress was not set in .config file</exception>
		/// <param name="sLogEntry">Any string to email (body of the message). It is mean that this should be the output of GMBC.ExceptionManager.ExceptionManager.SerializeToString()</param>
		public static void SendEmail(string sLogEntry) {
			if (Config.EmailLogger.ToEmailAddress != "") {
				string subject = Config.EmailLogger.SubjectLine;
				string body = sLogEntry;
				try {
                    MailMessage msg = new MailMessage(Config.EmailLogger.FromEmailAddress, Config.EmailLogger.ToEmailAddress, subject, body);
                    msg.IsBodyHtml = false;

                    SmtpClient smtp = new SmtpClient();
                    smtp.Send(msg);

				} catch (Exception e) {
                    BaseApplicationException ex = new BaseApplicationException(@"Error calling smtp.Send(...)", e);
					Config.SerializeToNameValueCollection(ex.AdditionalInformation);
					ex.AdditionalInformation.Add("body", body);
					throw ex;
				}
			} else {
				BaseApplicationException ex = new BaseApplicationException(@"Cannot call SendEmail when Config.EmailLogger.ToEmailAddress == """);
				Config.SerializeToNameValueCollection(ex.AdditionalInformation);
				ex.AdditionalInformation.Add("sLogEntry", sLogEntry);
				throw ex;
			}
		}
	}
}
