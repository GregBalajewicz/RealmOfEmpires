using System;

namespace Gmbc.Common.Diagnostics.Logging
{
	/// <summary>
	/// CLASS UNDER DEVELOPMENT
	/// 
	/// </summary>
	public class LogManager
	{
		private LogManager(){} // Singleton

		/// <summary>
		/// The types of messages that can be logged using the LogManager
		/// </summary>
		public enum MessageType 
		{
			Warning, 
			Error, 
			Info,
		}


		public static void Log(string message, MessageType messageType) 
		{
			ExceptionManagement.ExceptionManager.Publish(message, messageType);
		}
	}
}
