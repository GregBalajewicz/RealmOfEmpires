using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Linq;

namespace Fbg.Bll
{
    public class Report_SupportAttacked : Report_base, Gmbc.Common.Diagnostics.ExceptionManagement.ISerializableToNameValueCollection2
    {
        DataSet _dsReport;
        DataTable _dtUnits;
        List<VillageSupporting> villagesSupporting;


        public List<VillageSupporting> VillagesSupporting
        {
            get
            {
                return villagesSupporting;
            }
        }

        //public DataTable dtUnits 
        //{
        //    get
        //    {
        //        return _dtUnits;
        //    }
        //}

        //public new DataTable dtSummary
        //{
        //    get
        //    {
        //        return base.dtSummary;
        //    }
        //}

        public Report_SupportAttacked(Player player, int recordID)
            : base(player, recordID
            , Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.Subject
            , Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.Time
            , Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.ForwardedByPlayerID)
        {
            InitReport(player, recordID, DAL.Report.GetSupportAttackedReportDetails(player.Realm.ConnectionStr, player.ID, recordID));
        }

        public void InitReport(Player player, int recordID, DataSet ds)
        {
            
            try
            {
                _dsReport = ds;

                base.InitReport(_dsReport.Tables[Report.CONSTS.BattleReport.TableIndex.Summary]
                    , Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.ForwardedByPlayerName
                    , Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.ForwardedOn
                    , Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.ForwardedByPlayerID
                    , Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.ForwardedPlayerAvatarID);

                _dtUnits = _dsReport.Tables[Report.CONSTS.SupportAttackedReport.TableIndex.Units];

                villagesSupporting = new List<VillageSupporting>(dtSummary.Rows.Count);

                foreach (DataRow dr in dtSummary.Rows)
                {
                    villagesSupporting.Add(new VillageSupporting()
                    {
                        name = (string)dr[Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.SupportingVillageName]
                        ,
                        x = (int)dr[Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.SupportingVillageXCord]
                        ,
                        y = (int)dr[Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.SupportingVillageYCord]
                        ,
                        id = (int)dr[Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.SupportingVillageID]
                        ,
                        units = _dtUnits.AsEnumerable().Where(row => (int)row[Report.CONSTS.SupportAttackedReport.UnitsTblColIndex.SupportingVillageID] == (int)dr[Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.SupportingVillageID])
                        .Select(row => new UnitInvolved() { 
                            DeployedUnitCount = (int)row[Report.CONSTS.SupportAttackedReport.UnitsTblColIndex.DeployedUnitCount]
                            , KilledUnitCount = (int)row[Report.CONSTS.SupportAttackedReport.UnitsTblColIndex.KilledUnitCount]
                            , ReaminingUnitCount = (int)row[Report.CONSTS.SupportAttackedReport.UnitsTblColIndex.ReaminingUnitCount]
                            , UnitTypeID = (int)row[Report.CONSTS.SupportAttackedReport.UnitsTblColIndex.UnitTypeID]
                        }).ToList()
                       
                    });                 
                }



            } // TRY 

            catch (Exception ex)
            {
                BaseApplicationException bex = new BaseApplicationException("Error in InitReport", ex);
                throw bex;
            }
       

        }


        public class UnitInvolved
        {
            public int UnitTypeID { get; set; }
            public int DeployedUnitCount { get; set; }
            public int KilledUnitCount { get; set; }
            public int ReaminingUnitCount { get; set; }

        }


        public class VillageSupporting
        {
            public string name;
            public int x;
            public int y;
            public int id;
            public List<UnitInvolved> units;

        }



        // Who and what village was the support sent to

        public int SupportedPlayerID
        {
            get
            {
                return (int)dtSummary.Rows[0][Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.SupportedPlayerID];
            }
        }

        public short SupportedAvatarID
        {
            get
            {
                return (short)dtSummary.Rows[0][Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.SupportedAvatarID];
            }
        }

        public string SupportedPlayerName
        {
            get
            {
                return dtSummary.Rows[0][Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.SupportedPlayerName].ToString();
            }
        }

        public string SupportedVillageName
        {
            get
            {
                return dtSummary.Rows[0][Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.SupportedVillageName].ToString();
            }
        }

        public short SupportedVillageTypeID
        {
            get
            {
                return (short)dtSummary.Rows[0][Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.SupportedVillageTypeID];
            }
        }

        public int SupportedVillagePoints
        {
            get
            {
                return (int)dtSummary.Rows[0][Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.SupportedVillagePoints];
            }
        }

        public int SupportedVillageX
        {
            get
            {
                return (int)dtSummary.Rows[0][Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.SupportedVillageX];
            }
        }


        public int SupportedVillageY
        {
            get
            {
                return (int)dtSummary.Rows[0][Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.SupportedVillageY];
            }
        }

        public int SupportedVillageID
        {
            get
            {
                return (int)dtSummary.Rows[0][Report.CONSTS.SupportAttackedReport.SummaryTblColIndex.SupportedVillageID];
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
                    BaseApplicationException.AddAdditionalInformation(col, "_dsReport", _dsReport);
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
