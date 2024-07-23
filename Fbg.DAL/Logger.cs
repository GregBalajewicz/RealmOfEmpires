using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;

namespace Fbg.DAL
{
    public class Logger
    {


        public static void LogEvent(string connectionStr, string eventName, string data, long executionTime)
        {
            if (String.IsNullOrEmpty(Config.LogDBConnectionStr))
            {
                // if no log DB specified, then we do not log 
                return;
            }

            return;


        }


        public static void LogEvent(Database db, string eventName, string data, long executionTime)
        {
            LogEvent(db.ConnectionStringWithoutCredentials, eventName, data, executionTime);
        }

    }
}
