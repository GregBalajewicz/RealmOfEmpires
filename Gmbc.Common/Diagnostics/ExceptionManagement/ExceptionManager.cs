using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Reflection;
using Gmbc.Common.Diagnostics.Logging;

namespace Gmbc.Common.Diagnostics.ExceptionManagement {
	/// <summary>
	/// The main class used to write exceptions to different logs.
	/// <P>This class works with (uses) all the Loggers defined in Gmbc.Common.Diagnostics.Logging.
	/// The main function Publish(), simply uses those Loggers to write the exception information</P>
	/// <P>The SerializeToString() method takes an Exception object and serializes this exception and all its inner
	/// exceptions to a string. The method Publish uses the SerializeToString() method internally to serialize
	/// the exception into a string</P>
	/// </summary>
	/// <remarks>
	/// In order to use this class properly  it must be initialized via the application .config file.<BR></BR>
	/// This is the configuration section that must be added. 
	/// <XMP>
	/// 	<Gmbc.Common.Diagnostics>
	///			<TextFileLogger FileName="C:\fins\IntegrityChecks\IntegrityChecksErrorLog.txt" UseForPublish="True"/>
	///			<WindowsEventLogger Log="SomeWindowsEvenLog" Source="SomeSource"  UseForPublish="False"/>
	///		</Gmbc.Common.Diagnostics>
	/// </XMP>
	/// <p>This configures the loggers (what file to write to, what windows event log to use etc) so you can see
	/// documentation about the loggers for more details, however for the purpose of ExceptionManager, 
	/// we are interested in the UseForPublish attribute. This attribute controlls if ExceptionManager.Publish()
	/// will use this logger to log the exception or not.</p> 
	/// <list type="bullet">
	/// <item>
	/// <description>If this attribute is True, then ExceptionManager.Publish() will try to use this 
	/// logger to write the exception. </description>
	/// </item>
	/// <item>
	/// <description>If this attribute is False, or is not provided at all, then ExceptionManager.Publish() 
	/// will not use this logger to write the exception.</description>
	/// </item>
	/// </list>
	/// Please note that if you do not want to use a particular logger in ExceptionManager.Publish(), then its declaration
	/// can be left out completely. In this example, the EmailLogger was left out completely therefore it will
	/// not be used in ExceptionManager.Publish()
	/// 
	/// <P>You will also need to declare this configuration section in the .config file as such:
	/// <XMP>
	/// 	<configSections>
	///			<section name="Gmbc.Common.Diagnostics" type="Gmbc.Common.Diagnostics.Config, Gmbc.Common.Diagnostics"/>
	///		</configSections>
	/// </XMP>
	/// </P>
	/// </remarks>
	public class ExceptionManager
	{
		private static ArrayList m_SerializableObjects;	// holds ISerializableToNameValueCollection objects 
		private static string TEXT_SEPARATOR = "*********************************************";

		static ExceptionManager() {
			m_SerializableObjects = new ArrayList(5);
		}

		/// <summary>
		/// Method used serialize the exception information into a string. This string is readable and can
		/// be displayed or logged.
		/// </summary>
		/// <param name="exception">The exception object whose information should be serialized to string.</param>
		public static string SerializeToString(Exception exception) {
			return SerializeToString(exception, null);
		}

		/// <summary>
		/// Method used serialize the exception information and additional information in to a string. This string is readable and can
		/// be displayed or logged.
		/// This function also records some additional environment information such as Environment.MachineName etc
		/// </summary>
		/// <param name="exception">The exception object whose information should be serialized to string.</param>
		/// <param name="additionalInfo">A collection of additional data that should be written along with the exception information.</param>
		public static string SerializeToString(Exception exception, NameValueCollection additionalInfo) {
			//
			// add additional environment information
			//
			if (additionalInfo == null) {
				additionalInfo = new NameValueCollection();
			}
			AddEnvironmentInfo(additionalInfo);
			//
			// See if there are any ISerializableToNameValueCollection objects in our collection
			//	and if so, add this info to the additionlInfo. 
			//
			ISerializableToNameValueCollection SerializableObj;
			foreach (Object obj in m_SerializableObjects) {
				SerializableObj = (ISerializableToNameValueCollection) obj;
				try {
					SerializableObj.SerializeToNameValueCollection(additionalInfo);
				} catch (Exception e) {
					additionalInfo.Add("ERROR IN ExceptionManagement", "An error occured while calling SerializeToNameValueCollection() on object of type" + SerializableObj.GetType().ToString() + ". Exception.Message=" + e.Message);
				}
			}

			// Create StringBuilder to maintain publishing information.
			StringBuilder strInfo = new StringBuilder();


			if (exception == null) {
				strInfo.AppendFormat("{0}{0}No Exception object has been provided.{0}", Environment.NewLine);
			} else {
                ExceptionManager.SerializeExceptionChain(exception, strInfo);

				#region Loop through each exception class in the chain of exception objects
                /*
				// Loop through each exception class in the chain of exception objects.
				Exception currentException = exception;	// Temp variable to hold InnerException object during the loop.
				int intExceptionCount = 1;				// Count variable to track the number of exceptions in the chain.
				do {
					// Write title information for the exception object.
					strInfo.AppendFormat("{0}{0}Exception #{1} Information{0}{2}", Environment.NewLine, intExceptionCount.ToString(), TEXT_SEPARATOR);
					strInfo.AppendFormat("{0}[[ Exception Type ]] : {1}", Environment.NewLine, currentException.GetType().FullName);
				
					#region Loop through the public properties of the exception object and record their value
					//
					// Loop through the public properties of the exception object and record their value.
					//
					PropertyInfo[] aryPublicProperties = currentException.GetType().GetProperties();
					NameValueCollection currentAdditionalInfo;
					foreach (PropertyInfo p in aryPublicProperties) {
						// Do not log information for the InnerException or StackTrace or additionalInfo. This information is captured later in the process.
						if (p.Name != "InnerException" && p.Name != "StackTrace" && p.Name != "additionalInfo") {
							if (p.GetValue(currentException,null) == null) {
								strInfo.AppendFormat("{0}[[ {1} ]] : NULL", Environment.NewLine, p.Name);
							}
							else {
								strInfo.AppendFormat("{0}[[ {1} ]] : {2}", Environment.NewLine, p.Name, p.GetValue(currentException,null));
							}
						}
					}
					#endregion

					#region Record the content of additionalInfo if the exception 'is a' BaseApplicationException
					//
					// If the exception is of the BaseApplicationException, then records its additionInfo collection
					//
					if (currentException is BaseApplicationException) {
						currentAdditionalInfo = (NameValueCollection) ((BaseApplicationException)currentException).AdditionalInformation;

						if (currentAdditionalInfo != null && currentAdditionalInfo.Count > 0) {
							strInfo.AppendFormat("{0}AdditionalInformation:", Environment.NewLine);
							for (int i = 0; i < currentAdditionalInfo.Count; i++) {
								strInfo.AppendFormat("{0}[[ {1} ]] : {2}", Environment.NewLine, currentAdditionalInfo.GetKey(i), currentAdditionalInfo[i]);
							}
						}
					}
					#endregion
	
					#region Record the Exception StackTrace
					// Record the StackTrace with separate label.
					if (currentException.StackTrace != null) {
						strInfo.AppendFormat("{0}{0}StackTrace Information{0}{1}", Environment.NewLine, TEXT_SEPARATOR);
						strInfo.AppendFormat("{0}{1}", Environment.NewLine, currentException.StackTrace);
					}
					#endregion

					// Reset the temp exception object and iterate the counter.
					currentException = currentException.InnerException;
					intExceptionCount++;
				} while (currentException != null);
                 */
				#endregion
			}

            #region Record the contents of the AdditionalInfo collection
            //
            // Record the contents of the AdditionalInfo collection.
            //
            if (additionalInfo != null)
            {
                strInfo.AppendFormat("{0}General Information {0}{1}{0}Additional Info:", Environment.NewLine, TEXT_SEPARATOR);
                foreach (string i in additionalInfo)
                {
                    strInfo.AppendFormat("{0} [[ {1} ]] : {2}", Environment.NewLine, i, additionalInfo.Get(i));
                }
            }
            #endregion

			return strInfo.ToString();
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="strInfo"></param>
        /// <param name="intExceptionCount">Set </param>
        private static void SerializeExceptionChain(Exception ex, StringBuilder strInfo)
        {
            SerializeExceptionChain(ex,strInfo,1);
        }

        /// <summary>
        /// DO NOT CALL THIS DIRECTLY. SHOULD BE CALLED BY SerializeExceptionChain(Exception ex, StringBuilder strInfo) only
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="strInfo"></param>
        /// <param name="intExceptionCount">Set </param>
        private static void SerializeExceptionChain(Exception ex, StringBuilder strInfo, int intExceptionCount)
        {
            Exception currentException = ex;	// Temp variable to hold InnerException object during the loop.

            if (ex.InnerException != null)
            {
                SerializeExceptionChain(ex.InnerException, strInfo, intExceptionCount + 1);
            }

            // Write title information for the exception object.
            strInfo.AppendFormat("{0}{0}Exception #{1} Information{0}{2}", Environment.NewLine, intExceptionCount.ToString(), TEXT_SEPARATOR);
            strInfo.AppendFormat("{0}[[ Exception Type ]] : {1}", Environment.NewLine, currentException.GetType().FullName);

            #region Loop through the public properties of the exception object and record their value
            //
            // Loop through the public properties of the exception object and record their value.
            //
            PropertyInfo[] aryPublicProperties = currentException.GetType().GetProperties();
            NameValueCollection currentAdditionalInfo;
            foreach (PropertyInfo p in aryPublicProperties)
            {
                // Do not log information for the InnerException or StackTrace or additionalInfo. This information is captured later in the process.
                if (p.Name != "InnerException" && p.Name != "StackTrace" && p.Name != "additionalInfo")
                {
                    if (p.GetValue(currentException, null) == null)
                    {
                        strInfo.AppendFormat("{0}[[ {1} ]] : NULL", Environment.NewLine, p.Name);
                    }
                    else
                    {
                        strInfo.AppendFormat("{0}[[ {1} ]] : {2}", Environment.NewLine, p.Name, p.GetValue(currentException, null));
                    }
                }
            }
            #endregion

            #region Record the content of additionalInfo if the exception 'is a' BaseApplicationException
            //
            // If the exception is of the BaseApplicationException, then records its additionInfo collection
            //
            if (currentException is BaseApplicationException)
            {
                currentAdditionalInfo = (NameValueCollection)((BaseApplicationException)currentException).AdditionalInformation;

                if (currentAdditionalInfo != null && currentAdditionalInfo.Count > 0)
                {
                    strInfo.AppendFormat("{0}AdditionalInformation:", Environment.NewLine);
                    for (int i = 0; i < currentAdditionalInfo.Count; i++)
                    {
                        strInfo.AppendFormat("{0}[[ {1} ]] : {2}", Environment.NewLine, currentAdditionalInfo.GetKey(i), currentAdditionalInfo[i]);
                    }
                }
            }
            #endregion

            #region Record the Exception StackTrace
            // Record the StackTrace with separate label.
            if (currentException.StackTrace != null)
            {
                strInfo.AppendFormat("{0}{0}StackTrace Information{0}{1}", Environment.NewLine, TEXT_SEPARATOR);
                strInfo.AppendFormat("{0}{1}", Environment.NewLine, currentException.StackTrace);
            }
            #endregion

        }

		/// <summary>
		/// Write/log the information to what ever log that participates in publishing as specified in the .config file (use UseForPublish=true)
		/// </summary>
		/// <param name="sInfo">string to publish</param>
		public static void Publish (string sInfo) {
			if (Config.TextFileLogger.UseForPublish) {
				TextFileLogger.Write(sInfo);
			} 
			if (Config.EmailLogger.UseForPublish) {
				EmailLogger.SendEmail(sInfo);
			}
			if (Config.WindowsEventLogger.UseForPublish) {
				WindowsEventLogger.Write(sInfo, WindowsEventLogger.MessageType.Error);
			}
            if (Config.DBLogger.UseForPublish)
            {
                DBLogger.Write(sInfo);
            }
        }
		/// <summary>
		/// Write/log the information to what ever log that participates in publishing as specified in the .config file (use UseForPublish=true)
		/// </summary>
		/// <param name="sInfo">string to publish</param>
		internal static void Publish (string sInfo, Logging.LogManager.MessageType messageType) 
		{
			WindowsEventLogger.MessageType windowsMessageType = WindowsEventLogger.MessageType.Info;
			switch (messageType)  
			{
				case Logging.LogManager.MessageType.Error:
				windowsMessageType = WindowsEventLogger.MessageType.Error;
					break;
				case Logging.LogManager.MessageType.Warning:
					windowsMessageType = WindowsEventLogger.MessageType.Warning;
					break;
				case Logging.LogManager.MessageType.Info:
					windowsMessageType = WindowsEventLogger.MessageType.Info;
					break;
			}

			if (Config.TextFileLogger.UseForPublish) 
			{
				TextFileLogger.Write(sInfo);
			} 
			if (Config.EmailLogger.UseForPublish) 
			{
				EmailLogger.SendEmail(sInfo);
			}
			if (Config.WindowsEventLogger.UseForPublish) 
			{
				WindowsEventLogger.Write(sInfo, windowsMessageType);
			}
		}

		/// <summary>
		/// Write/log the information to what ever log that participates in publishing as specified in the .config file (use UseForPublish=true)
		/// This is the same as Publish(ExceptionManager.SerializeToString(exception)
		/// This function also records some additional environment information such as Environment.MachineName etc
		/// </summary>
		/// <param name="exception">Exception to log</param>
		public static void Publish (Exception exception) 
		{
			Publish(exception, new System.Collections.Specialized.NameValueCollection());
		}
		/// <summary>
		/// Write/log the information to what ever log that participates in publishing as specified in the .config file (use UseForPublish=true)
		/// This is the same as Publish(ExceptionManager.SerializeToString(exception, additionalInfo)
		/// This function also records some additional environment information such as Environment.MachineName etc
		/// </summary>
		/// <param name="exception">Exception to log</param>
		/// <param name="additionalInfo">additionalInfo to log</param>
		public static void Publish (Exception exception, NameValueCollection additionalInfo) {
//			AddEnvironmentInfo(additionalInfo);
			Publish(SerializeToString(exception, additionalInfo));
		}

		/// <summary>
		/// Add any object that is ISerializableToNameValueCollection and this object information will
		/// automatically be addded when Publish() or SerializeToString() is called
		/// </summary>
		/// <param name="obj">object that imeplements ISerializableToNameValueCollection. Cannot be null</param>
		/// <remarks>This method can be usefull if you have an object that holds, say, configuration
		/// information that would be very useful to have logged if an error occured. Instead or remembering
		/// to log all the important info of this object in every place an erorr is handled, you could
		/// simply have the class implement ISerializableToNameValueCollection, add it to ExceptionManger
		/// using this function and then every time you call Publish() or SerializeToString(), the information
		/// will always be written out.</remarks>
		public static void AddISerializeToNameValueCollectionObject(ISerializableToNameValueCollection obj){
			if (obj == null) {
				throw new ArgumentException("Argument cannot be null", "ISerializableToNameValueCollection obj");
			}
			m_SerializableObjects.Add(obj);
		}

		private static void AddEnvironmentInfo(NameValueCollection additionalInfo) {
			// Add environment information to the additionalInfo collection.
			try {
				additionalInfo.Add( "Environment.MachineName", Environment.MachineName);
			} catch (Exception e){
				additionalInfo.Add("Environment.MachineName", "Error getting this info; e.Message=" + e.Message);
			}
									
			try {
				additionalInfo.Add("Assembly.GetExecutingAssembly().FullName", Assembly.GetExecutingAssembly().FullName);
			} catch (Exception e){
				additionalInfo.Add("Assembly.GetExecutingAssembly().FullName", "Error getting this info; e.Message=" + e.Message);
			}
					
			try {
				additionalInfo.Add("AppDomain.CurrentDomain.FriendlyName", AppDomain.CurrentDomain.FriendlyName);
			} catch (Exception e){
				additionalInfo.Add("AppDomain.CurrentDomain.FriendlyName", "Error getting this info; e.Message=" + e.Message);
			}
						
			try {
				additionalInfo.Add("Thread.CurrentPrincipal.Identity.Name", System.Threading.Thread.CurrentPrincipal.Identity.Name);
			} catch (Exception e){
				additionalInfo.Add("Thread.CurrentPrincipal.Identity.Name", "Error getting this info; e.Message=" + e.Message);
			}
				
			try {
				additionalInfo.Add("WindowsIdentity.GetCurrent().Name", System.Security.Principal.WindowsIdentity.GetCurrent().Name);
			} catch (Exception e){
				additionalInfo.Add("WindowsIdentity.GetCurrent().Name", "Error getting this info; e.Message=" + e.Message);
			}

			try {
				additionalInfo.Add("Environment.StackTrace", Environment.StackTrace);
			} catch (Exception e){
				additionalInfo.Add("Environment.StackTrace", "Error getting this info; e.Message=" + e.Message);
			}

		}
	
	}
}
