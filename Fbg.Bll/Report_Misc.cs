using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Linq;

namespace Fbg.Bll
{
    public class Report_Misc : Report_base, Gmbc.Common.Diagnostics.ExceptionManagement.ISerializableToNameValueCollection2
    {
        public Report_Misc(Player player, int recordID)
            : base(player, recordID
            , Report.CONSTS.ReportTypeDetails.MiscReportIndex.Subject
            , Report.CONSTS.ReportTypeDetails.MiscReportIndex.Time
            , Report.CONSTS.ReportTypeDetails.MiscReportIndex.ForwardedByPlayerID)
        {
            InitReport(player, recordID, DAL.Report.GetReportTypeDetails(player.Realm.ConnectionStr, player.ID, recordID));
        }

        public void InitReport(Player player, int recordID, DataSet ds)
        {
            
            try
            {
                

                base.InitReport(ds.Tables[0]
                    , Report.CONSTS.ReportTypeDetails.MiscReportIndex.ForwardedByPlayerName
                    , Report.CONSTS.ReportTypeDetails.MiscReportIndex.ForwardedOn
                    , Report.CONSTS.ReportTypeDetails.MiscReportIndex.ForwardedByPlayerID
                    , Report.CONSTS.ReportTypeDetails.MiscReportIndex.ForwardedPlayerAvatarID);
            } // TRY 

            catch (Exception ex)
            {
                BaseApplicationException bex = new BaseApplicationException("Error in InitReport", ex);
                throw bex;
            }
       

        }

        public string Description
        {
            get
            {
                return dtSummary.Rows[0][Report.CONSTS.ReportTypeDetails.MiscReportIndex.Description].ToString();
            }
        }


        #region ISerializableToNameValueCollection2 Members

        override public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
        {
            SerializeToNameValueCollection(col, String.Empty);
        }
        override public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col, string prefix)
        {
            base.SerializeToNameValueCollection(col, prefix);
            try
            {
                string pre = prefix + "Report_BattleDet[" + this.RecordID.ToString() + "]";

                if (col == null)
                {
                    ExceptionManager.Publish("Error in Report_BattleDet.SerializeToNameValueCollection. argument 'col' is null");
                }
                else
                {
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish("Error in Report_BattleDet.SerializeToNameValueCollection. ExceptionManager.SerializeToString(e):" + ExceptionManager.SerializeToString(e));
            }
        }

        #endregion



    }
}
