using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Data.SqlClient ;

namespace Fbg.DAL
{
    public class BattleSimulations
    {

        static Gmbc.Common.GmbcBaseClass.Trace TRACE = new Gmbc.Common.GmbcBaseClass.Trace("Fbg.DAL", "Fbg.DAL.BattleSimulations");

        public static DataSet  ApplyTestSimulationsScript(string connectionStr,string CommandText)
        {
            TRACE.InfoLine("in 'BattleSimulations'");
            

            try
            {
               SqlDataAdapter SDA =new SqlDataAdapter(CommandText ,new SqlConnection(connectionStr));
               DataSet ds = new DataSet();
               SDA.Fill(ds);
                //db = new DB(connectionStr);;

               return ds;
            }
            catch (Exception e)
            {
                BaseApplicationException ex = new BaseApplicationException("Error while calling qApplyTestSimulationsScript", e);
                ex.AdditionalInformation.Add("connectionStr", connectionStr);
                
                throw ex;
            }
            finally
            {
            }
        }
        
    }
}
