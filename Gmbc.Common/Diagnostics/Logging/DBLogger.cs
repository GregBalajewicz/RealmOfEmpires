using System;
using System.IO;
using System.Text;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Data.SqlClient;
using System.Data;



namespace Gmbc.Common.Diagnostics.Logging
{
	/// <summary>
	/// Allows you to write a string into a DB file. Designed to Log error messages 
	/// </summary>
	/// <remarks>
    /// Database table required: (more nullable collumns can be added ) 
    /// 
    /// create table Errors (ErrorSource varchar(10), ErrorMsg  varchar(max))
    /// 
	/// In order to use this logger, it must be initialized via the application .config file.<BR></BR>
	/// This is the configuration section that must be added. 
	/// <XMP>
	/// 	<Gmbc.Common.Diagnostics>
    ///			      <DBLogger ConnectionString="Data Source=localhost;Initial Catalog=ErrorLog;Integrated Security=True;" ErrorSource="WEB" UseForPublish="false"/>
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
	/// </remarks>
	public class DBLogger {
		private static string TEXT_SEPARATOR = "============================================================";

		private DBLogger() {
		}

		/// <summary>
		/// Write a log entry to database
		/// </summary>
		/// <param name="sLogEntry">Any string to write. It is mean that this should be the output of GMBC.ExceptionManager.ExceptionManager.SerializeToString()</param>
		public static void Write(string sLogEntry) {

			
			// Write the entry to the log file.   
            using (Gmbc.Common.Sql.MS.Connection con = new Sql.MS.Connection(Config.DBLogger.ConnectionString))
            {
                string sql = "INSERT INTO Errors (ErrorSource,ErrorMsg) VALUES (@Val1,@Val2)";

                SqlCommand cmd = new SqlCommand(sql, con.SqlConnection);
                cmd.Parameters.AddWithValue("@Val1", Config.DBLogger.ErrorSource);
                cmd.Parameters.AddWithValue("@Val2", sLogEntry);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

            }
		}
	}
}
