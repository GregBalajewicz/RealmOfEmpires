using System;
using System.IO;
using System.Text;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Diagnostics;

namespace Gmbc.Common.Diagnostics.Logging
{ 

	/// <summary>
	/// Allows you to write (log) an event to windows event log.
	/// Please note that you must have appropriate security permissions in order to be able 
	/// to work with windows event log. See Remarks for more information
	/// </summary>
	/// <remarks>
	/// <B>Configuration of logger</B>
	/// <P>
	/// In order to use this logger, it must be initialized via the application .config file.<BR></BR>
	/// This is the configuration section that must be added. 
	/// <XMP>
	/// 	<Gmbc.Common.Diagnostics>
	///			<WindowsEventLogger Log="SomeWindowsEvenLog" Source="SomeSource"/>
	///		</Gmbc.Common.Diagnostics>
	/// </XMP>
	/// You will also need to declare this configuration section in the .config file as such:
	/// <XMP>
	/// 	<configSections>
	///			<section name="Gmbc.Common.Diagnostics" type="Gmbc.Common.Diagnostics.Config, Gmbc.Common.Diagnostics"/>
	///		</configSections>
	/// </XMP>
	/// </P>
	/// <B>Security considerations</B>
	/// <P>
	///	When writing to Windows Event Log, you must have appropriate permissions to registry.
	///	This is a common problem for ASP.NET Web aplications - Here is an issue: ASP.NET does
	///	not have privilages by default to write to the event log. 
	///	The solutions are many. One is to, open regedt32 (not the standard, regedit) 
	///	and navigate to HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\EventLog 
	///	 and click on 'Security-->Permissions...' in the menu.  Add the ASPNET account 
	///	for the local machine and place a check next to 'Full Control' 
	///	However, sometimes we found that this did not work so we had to give 'Full Controll' to 'Everyone'.
	///	We think, you can avoid problem altogether if you use an already existing Event Log and Source. 
	///	For more information, see EventLog.CreateEventSource() method - this is the method which generated the security exception.
	///	</P>
	/// <P>For more information, see Microsoft article Q329291</P>
	/// </remarks>
	public class WindowsEventLogger 
	{
		/// <summary>
		/// The types of messages that can be logged using the WindowsEventLogger
		/// This is really just a mapping for System.Diagnostic.EventLogEntryTypes
		/// </summary>
		public enum MessageType 
		{
			Warning, 
			Error, 
			Info,
			FailureAudit, 
			SuccessAudit
		}

		static EventLog eventLog = new EventLog(); 
		static WindowsEventLogger()
		{
			//
			// Try to create a new event source and/or log if it does not exist
			//
			try 
			{
				if (!EventLog.SourceExists(Config.WindowsEventLogger.Source))   
				{ 
					EventLog.CreateEventSource(Config.WindowsEventLogger.Source, 
						Config.WindowsEventLogger.Log);
				}
			} 
			catch (System.Security.SecurityException se) 
			{
				SecurityException sex = new SecurityException ("Permission error while creating the Event Log and/or source."
					+ " When writing to Windows Event Log, you must have appropriate permissions to the registry. "
					+ " This is a common problem for all ASP.NET Web aplications - Here is an issue: By default ASP.NET does "
					+ " not have privileges to write to the event log. "
					+ "  The solutions are many. One is to, open regedt32 (not the standard, regedit) "
					+ @" and navigate to HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\EventLog "
					+ " and click on 'Security-->Permissions...' in the menu.  Add the ASPNET account "
					+ " for the local machine and place a check next to 'Full Control' "
					+ " However, sometimes we found that this did not work so we had to give 'Full Controll' to 'Everyone'."
					+ " We think, you can avoid problem altogether if you use an already existing Event Log and Source. "
					+ " For more information, see EventLog.CreateEventSource() method - this is the method which generated the security exception."
					, se);
				Config.SerializeToNameValueCollection(sex.AdditionalInformation);
				throw sex;
			} 
			catch (Exception ex) 
			{
				BaseApplicationException e = new BaseApplicationException("Error while creating the Event Log and/or source.", ex);
				Config.SerializeToNameValueCollection(e.AdditionalInformation);
				throw e;
			}
			//
			// Set the log and source to use to the values from the config file
			//
			eventLog.Log = Config.WindowsEventLogger.Log; 
			eventLog.Source = Config.WindowsEventLogger.Source;
		}

		private static EventLogEntryType GetType(MessageType messageType)
		{
			EventLogEntryType entryType = new EventLogEntryType();

			switch(messageType)
			{
				case MessageType.Warning:
					entryType = EventLogEntryType.Warning;
					break;
				case MessageType.Info:
					entryType = EventLogEntryType.Information;
					break;
				case MessageType.Error:
					entryType = EventLogEntryType.Error;
					break;
				case MessageType.FailureAudit:
					entryType = EventLogEntryType.FailureAudit;
					break;
				case MessageType.SuccessAudit:
					entryType = EventLogEntryType.SuccessAudit;
					break;
				default:
					break;
			}
			return entryType;
		}

		/// <summary>
		/// Writes an information type entry, with the given message text to Information the event log.
		/// The name of the Log file to write to and Source are set in the .config file.
		/// </summary>
		/// <param name="sLogEntry">Message text</param>
		public static void Write(string sLogEntry) 
		{
			EventLogEntryType entryType = GetType(MessageType.Info);
			eventLog.WriteEntry(sLogEntry, entryType); 
		}		
		
		/// <summary>
		/// Writes an information type entry, with the given message text to the event log.
		/// The name of the Log file to write to and Source are set in the .config file.
		/// </summary>
		/// <param name="sLogEntry">Message text</param>
		/// <param name="messageType">Message Type: error, warning, info, success audit, failure audit</param>
		public static void Write(string sLogEntry, MessageType messageType) 
		{
			EventLogEntryType entryType = GetType(messageType);
			eventLog.WriteEntry(sLogEntry, entryType);
		}		
		
		/// <summary>
		/// Writes an information type entry, with the given message text 
		/// and application-defined event identifier to the event log.
		/// The name of the Log file to write to and Source are set in the .config file.
		/// </summary>
		/// <param name="sLogEntry">Message text</param>
		/// <param name="messageType">Message Type: error, warning, info, success audit, failure audit</param>
		/// <param name="iEventID">uniquely identify an event</param>
		public static void Write(string sLogEntry, MessageType messageType, int iEventID) 
		{
			EventLogEntryType entryType = GetType(messageType);
			eventLog.WriteEntry(sLogEntry, entryType, iEventID);
		}		
		
		/// <summary>
		/// Writes an information type entry, with the given message text 
		/// and application-defined event identifier
		/// and application-defined category to the event log. 
		/// The name of the Log file to write to and Source are set in the .config file.
		/// </summary>
		/// <param name="sLogEntry">Message text</param>
		/// <param name="messageType">Message Type: error, warning, info, success audit, failure audit</param>
		/// <param name="iEventID">uniquely identify an event</param>
		/// <param name="sCategory">application-defined category </param>
		public static void Write(string sLogEntry, MessageType messageType, int iEventID, short sCategory) 
		{
			EventLogEntryType entryType = GetType(messageType);
			eventLog.WriteEntry(sLogEntry, entryType, iEventID, sCategory);
		}		
		
		/// <summary>
		/// Writes an information type entry, with the given message text 
		/// and application-defined event identifier
		/// and application-defined category 
		/// and appends binary data  to the event log. 
		/// The name of the Log file to write to and Source are set in the .config file.
		/// </summary>
		/// <param name="sLogEntry">Message text</param>
		/// <param name="messageType">Message Type: error, warning, info, success audit, failure audit</param>
		/// <param name="iEventID">uniquely identify an event</param>
		/// <param name="sCategory">application-defined category </param>
		/// <param name="bData">application-defined event-specific data</param>
		public static void Write(string sLogEntry, MessageType messageType, int iEventID, 
			short sCategory, byte[] bData) 
		{
			EventLogEntryType entryType = GetType(messageType);
			eventLog.WriteEntry(sLogEntry, entryType, iEventID, sCategory, bData);
		}

		/// <summary>
		/// Read a Entry from Windows Log file. The name of the Log file to write to and Source are set in the .config file.
		/// </summary>
		public static string Read() 
		{
			string sEntry = "";
			foreach (EventLogEntry entry in eventLog.Entries) 
			{
				if(entry.Source.Equals(Config.WindowsEventLogger.Source))
					sEntry = entry.Message;
			}
			return sEntry;
		}

	
	}
}
