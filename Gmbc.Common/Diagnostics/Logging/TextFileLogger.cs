using System;
using System.IO;
using System.Text;
using Gmbc.Common.Diagnostics.ExceptionManagement;



namespace Gmbc.Common.Diagnostics.Logging
{
	/// <summary>
	/// Allows you to write a string into a text file. Designed to Log error messages to a text file
	/// </summary>
	/// <remarks>
	/// In order to use this logger, it must be initialized via the application .config file.<BR></BR>
	/// This is the configuration section that must be added. 
	/// <XMP>
	/// 	<Gmbc.Common.Diagnostics>
	///			<TextFileLogger FileName="C:\fins\IntegrityChecks\IntegrityChecksErrorLog.txt"/>
	///		</Gmbc.Common.Diagnostics>
	/// </XMP>
	/// The FileName attribute specifies the file name to which
	/// this logger will write to.<p></p>
	/// You will also need to declare this configuration section in the .config file as such:
	/// <XMP>
	/// 	<configSections>
	///			<section name="Gmbc.Common.Diagnostics" type="Gmbc.Common.Diagnostics.Config, Gmbc.Common.Diagnostics"/>
	///		</configSections>
	/// </XMP>
	/// <P><B>Please Note</B> If this inframtion is not provided the config file, the logger with write
	/// the messages to ".\ApplicationError.log" file. If it is running in context of a web application, this will be resolved to
	/// "[virtual's directory base path]\ApplicationError.log", otherwise it will simply be a file in the same directory as the
	/// executable file</P>
	/// </remarks>
	public class TextFileLogger {
		private static string TEXT_SEPARATOR = "============================================================";

		private TextFileLogger() {
		}

		/// <summary>
		/// Write a log entry to a text file. The name of the file to write to is set in the .config file. If this is 
		/// not specifed, C:\ErrorLog.txt is used
		/// </summary>
		/// <param name="sLogEntry">Any string to write. It is mean that this should be the output of GMBC.ExceptionManager.ExceptionManager.SerializeToString()</param>
		public static void Write(string sLogEntry) {

			// Create StringBuilder to maintain publishing information.
			StringBuilder strInfo = new StringBuilder();

			strInfo.AppendFormat("{0}{1}{0}{0} LOG ENTRY at {2} {0}{0}{1}{0}", Environment.NewLine, TEXT_SEPARATOR, DateTime.Now);
			strInfo.Append(sLogEntry);

			// Write the entry to the log file.   
			using ( FileStream fs = File.Open(Config.TextFileLogger.FileName, FileMode.Append,FileAccess.Write)) {
				using (StreamWriter sw = new StreamWriter(fs)) {
					sw.Write(strInfo.ToString());
				}
			}
		}
	}
}
