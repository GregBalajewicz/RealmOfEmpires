using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Linq;

namespace Fbg.Bll
{
    abstract public class Report_base : Gmbc.Common.Diagnostics.ExceptionManagement.ISerializableToNameValueCollection2
    {
        int _recordID;
        DataTable _dtSummary;
         Player _player;
        Report.ForwardedBy _forwardedBy;
        readonly int _summaryColIndex_subject;
        readonly int _summaryColIndex_time;
        readonly int _summaryColIndex_ForwardPlayerID;
        readonly int _summaryColIndex_ForwardPlayerAvatarID;


        public string Subject
        {
            get
            {
                return (String)_dtSummary.Rows[0][_summaryColIndex_subject];
            }
        }

        //private DateTime ReportTime
        //{
        //    get
        //    {
        //        return ((DateTime)_dtSummary.Rows[0][_summaryColIndex_time]).ToUniversalTime();
        //    }
        //}


        public int RecordID
        {
            get
            {
                return _recordID;
            }
        }
        
        public Player Player
        {
            get
            {
            return  _player;
            }
        }



        /// <summary>
        /// may return null
        /// </summary>
        public Report.ForwardedBy ForwardedBy
        {
            get
            {
                return _forwardedBy;
            }
        }

        protected DataTable dtSummary
        {
            get
            {
                return _dtSummary;
            }
        }

        /// <summary>
        /// tells you if you report retrieved is valid 
        /// </summary>
        public bool IsRetrievedReportValid
        {
            get
            {
                return dtSummary.Rows.Count != 0;
            }
        }

        public DateTime ReportTime
        {
            get
            {
                return ((DateTime)_dtSummary.Rows[0][_summaryColIndex_time]).ToUniversalTime();
            }
        }

        public Report_base(Player player, int recordID, int summaryColIndex_subject, int summaryColIndex_time, int summaryColIndex_ForwardPlayerID)//, int summaryColIndex_ForwardPlayerAvatarID)
        {
            _recordID = recordID;
            _player = player;
            _summaryColIndex_subject = summaryColIndex_subject;
            _summaryColIndex_time = summaryColIndex_time;
            _summaryColIndex_ForwardPlayerID = summaryColIndex_ForwardPlayerID;
            //_summaryColIndex_ForwardPlayerAvatarID = summaryColIndex_ForwardPlayerAvatarID;
        }

        public void InitReport(DataTable dtSummary,
            int summaryColIndex_ForwardPlayerName, int summaryColIndex_ForwardedOn, int summaryColIndex_ForwardPlayerID, int summaryColIndex_ForwardPlayerAvatarID)
        {
            try
            {

                _dtSummary = dtSummary;

                //
                // did we get a report ?
                //
                if (_dtSummary.Rows.Count > 0)
                {

                    //If ForwardPlayerID is Zero, then its not forwarded Report
                    if (_dtSummary.Rows[0][_summaryColIndex_ForwardPlayerID].ToString() != "0")
                    {
                        _forwardedBy = new Report.ForwardedBy((string)_dtSummary.Rows[0][summaryColIndex_ForwardPlayerName]
                        , Convert.ToDateTime(_dtSummary.Rows[0][summaryColIndex_ForwardedOn].ToString())
                        , (int)_dtSummary.Rows[0][summaryColIndex_ForwardPlayerID]
                        , (short)_dtSummary.Rows[0][summaryColIndex_ForwardPlayerAvatarID]);
                    }
                    else
                    {
                        _forwardedBy = null;
                    }
                }
            } // TRY 

            catch (Exception ex)
            {
                BaseApplicationException bex = new BaseApplicationException("Error in constructor", ex);
                throw bex;
            }
       

        }


        #region ISerializableToNameValueCollection2 Members

        virtual public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
        {
            SerializeToNameValueCollection(col, String.Empty);
        }
        virtual public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col, string prefix)
        {
            try
            {
                string pre = prefix + "Report_BattleDet[" + this._recordID.ToString() + "]";

                if (col == null)
                {
                    ExceptionManager.Publish("Error in Report_BattleDet.SerializeToNameValueCollection. argument 'col' is null");
                }
                else
                {
                    BaseApplicationException.AddAdditionalInformation(col, "_dtSummary", _dtSummary);
                    BaseApplicationException.AddAdditionalInformation(col, "_forwardedBy", _forwardedBy);
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
