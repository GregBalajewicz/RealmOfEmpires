using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;


namespace Fbg.DAL
{
    /// <summary>
    /// Summary description for DB
    /// </summary>
   public class DB : Microsoft.Practices.EnterpriseLibrary.Data.Sql.SqlDatabase
    {
        
        DateTime beforeRun;
        TimeSpan duration;

        public DB(string connectionStr)
            : base(connectionStr)
        {
            //db = new DB(connectionStr);;        
        }

     
        public static DataSet ExecuteDataSet(Database db, string storedProcedureName, params object[] parameterValues)
        {
            DateTime beforeRun;
            TimeSpan duration;
            DataSet ds;
            beforeRun = DateTime.Now;

            ds = db.ExecuteDataSet(storedProcedureName, parameterValues);

            duration = DateTime.Now.Subtract(beforeRun);

            return ds;
        }
        public static DataSet ExecuteDataSet(Database db, System.Data.Common.DbCommand cmd)
        {
            DateTime beforeRun;
            TimeSpan duration;
            DataSet ds;
            beforeRun = DateTime.Now;

            ds = db.ExecuteDataSet(cmd);

            duration = DateTime.Now.Subtract(beforeRun);

            return ds;
        }


        public static int ExecuteNonQuery(Database db, string storedProcedureName, params object[] parameterValues)
        {
            DateTime beforeRun;
            TimeSpan duration;
            int ret;
            beforeRun = DateTime.Now;

            ret = db.ExecuteNonQuery(storedProcedureName, parameterValues);

            duration = DateTime.Now.Subtract(beforeRun);
          
            return ret;
        }

        public override int ExecuteNonQuery(System.Data.Common.DbCommand command)
        {

            int ret;
            beforeRun = DateTime.Now;

            ret = base.ExecuteNonQuery(command);

            duration = DateTime.Now.Subtract(beforeRun);
          
            return ret;
        }

        public static int ExecuteNonQuery(Database db, System.Data.Common.DbCommand command)
        {
            DateTime beforeRun;
            TimeSpan duration;

            int ret;
            beforeRun = DateTime.Now;

            ret = db.ExecuteNonQuery(command);

            duration = DateTime.Now.Subtract(beforeRun);
            return ret;
        }

        public override object ExecuteScalar(string storedProcedureName, params object[] parameterValues)
        {
            object ret;
            beforeRun = DateTime.Now;

            ret = base.ExecuteScalar(storedProcedureName, parameterValues);

            duration = DateTime.Now.Subtract(beforeRun);

            return ret;

        }

        public static object ExecuteScalar(Database db, string storedProcedureName, params object[] parameterValues)
        {
            DateTime beforeRun;
            TimeSpan duration;
            object ret;
            beforeRun = DateTime.Now;

            ret = db.ExecuteScalar(storedProcedureName, parameterValues);

            duration = DateTime.Now.Subtract(beforeRun);

            return ret;

        }




        public new IDataReader ExecuteReader(string storedProcedureName, params object[] parameterValues)
        {
            IDataReader ret;
            beforeRun = DateTime.Now;

            ret =  base.ExecuteReader(storedProcedureName, parameterValues);

            duration = DateTime.Now.Subtract(beforeRun);
            return ret;


        }



    }
}