using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace Fbg.Bll.Api.Reports
{
    /// <summary>
    /// Summary description for Report
    /// </summary>
   
        public class Report
        {
            Fbg.Bll.Player _player;
           

            public Report(Fbg.Bll.Player player)
            {
                _player = player;
            }

            public string GetBattleReport(int recordID) {
                 Report_BattleDet report = new Report_BattleDet(Fbg.Bll.Report.GetBattleReportDetails(_player, recordID));

                if (!report._rep.IsRetrievedReportValid)
                {
                    //TODO FINISH HERE 
                    return ApiHelper.RETURN_SUCCESS(null);
                }

                return ApiHelper.RETURN_SUCCESS(report);
                
            }

            public string GetSupportAttackedReport(int recordID)
            {
                Report_SupportAttacked report = new Report_SupportAttacked(new Fbg.Bll.Report_SupportAttacked(_player, recordID));

                if (!report._rep.IsRetrievedReportValid)
                {
                    //TODO FINISH HERE 
                    return ApiHelper.RETURN_SUCCESS(null);
                }

                return ApiHelper.RETURN_SUCCESS(report);

            }

            public string GetMiscReport(int recordID)
            {
                Report_Misc report = new Report_Misc( new Fbg.Bll.Report_Misc(_player, recordID));

                if (!report._rep.IsRetrievedReportValid)
                {
                    //TODO FINISH HERE 
                    return ApiHelper.RETURN_SUCCESS(null);
                }

                return ApiHelper.RETURN_SUCCESS(report);

            }

           


            public string GetReportList()
            {
                int vx = Int32.MinValue;
                int vy = Int32.MinValue;
                var ds = Fbg.Bll.Report.ReportList(_player, -1 /*all reports*/, vx, vy,
                                           "" /*no search*/, -1 /* inbox */, false, true /* full list*/);

                int fl1 = Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.FlagID1;
                int fl2 = Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.FlagID2;
                //string staticUrl = "https://static.realmofempires.com/images/";

                return ApiHelper.RETURN_SUCCESS(
                    ds.Tables[0].AsEnumerable().Select(
                        v => new
                        {
                            subject = v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.Subject].ToString(),
                            time = (DateTime)v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.ReportTime],
                            type = v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.ReportType].ToString(),
                            viewed = (bool)v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.IsViewed],
                            forwarded = v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.IsForwarded].ToString(),
                            url = GetReportDetailsUrl(v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.ReportTypeID],
                                                      v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.RecordID]),
                            flag1 = (v[fl1] is DBNull ? null :
                                ((short)v[fl1] == 0 ? "Green" :
                                ((short)v[fl1] == 1 ? "Yellow" : "Red"))),

                            flag2 = (v[fl2] is DBNull ? null :
                                ((short)v[fl2] == 0 ? "Check" : "Exclamation")),

                            flag3 = v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.FlagID3] is DBNull ? null :
                                    _player.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Gov).IconUrl,
                            id = v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.RecordID],
                            whatside = v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.WhatSide]
                        }
                    )
                );
            }


            public string GetReportListAll()
            {
                int vx = Int32.MinValue;
                int vy = Int32.MinValue;
                var ds = Fbg.Bll.Report.ReportListAll(_player, -1 /*all reports*/, vx, vy,
                                           "" /*no search*/, false, true /* full list*/);

                int fl1 = Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.FlagID1;
                int fl2 = Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.FlagID2;
                //string staticUrl = "https://static.realmofempires.com/images/";

                return ApiHelper.RETURN_SUCCESS(
                    ds.Tables[0].AsEnumerable().Select(
                        v => new
                        {
                            subject = v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.Subject].ToString(),
                            time = (DateTime)v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.ReportTime],
                            type = v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.ReportType].ToString(),
                            viewed = (bool)v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.IsViewed],
                            forwarded = v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.IsForwarded].ToString(),
                            url = GetReportDetailsUrl(v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.ReportTypeID],
                                                      v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.RecordID]),
                            flag1 = (v[fl1] is DBNull ? null :
                                ((short)v[fl1] == 0 ? "Green" :
                                ((short)v[fl1] == 1 ? "Yellow" : "Red"))),

                            flag2 = (v[fl2] is DBNull ? null :
                                ((short)v[fl2] == 0 ? "Check" : "Exclamation")),

                            flag3 = v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.FlagID3] is DBNull ? null :
                                    _player.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Gov).IconUrl,
                            id = v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.RecordID],
                            whatside = v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.WhatSide],
                            folderID = v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.FolderID],
                            folderName = v[Fbg.Bll.Report.CONSTS.ReportList.ReportListIndex.FolderName]
                        }
                    )
                );
            }



            // To get report detail page URL
            private string GetReportDetailsUrl(object ReportTypeID, object recordID)
            {
                string NavigateURL = string.Empty;
                Fbg.Bll.Report.reportType reportType = (Fbg.Bll.Report.reportType)Enum.Parse(typeof(Fbg.Bll.Report.reportType), ReportTypeID.ToString());

                switch (reportType)
                {
                    case Fbg.Bll.Report.reportType.Attack:
                        NavigateURL = "Report_AttackDetails.aspx?{0}={1}";//&{2}={3}";
                        break;
                    case Fbg.Bll.Report.reportType.Support:
                        NavigateURL = "Report_SupportAttacked.aspx?{0}={1}";//&{2}={3}";
                        break;
                    case Fbg.Bll.Report.reportType.Silver:
                        NavigateURL = "Report_MiscDetails.aspx?{0}={1}";//&{2}={3}";
                        break;
                    case Fbg.Bll.Report.reportType.SupportArrived:
                    case Fbg.Bll.Report.reportType.YourSupportArrived:
                    case Fbg.Bll.Report.reportType.SupportPulledOut:
                    case Fbg.Bll.Report.reportType.SupportSentBack:
                    case Fbg.Bll.Report.reportType.Misc:
                        NavigateURL = "Report_MiscDetails.aspx?{0}={1}";//&{2}={3}";
                        break;
                    default:
                        //Exception handling for report type not found
                        throw new ArgumentException("Report Type is not found", reportType.ToString());
                }

                return String.Format(NavigateURL, global::CONSTS.QuerryString.RecordID.ToString(), recordID);
            }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestIDs">maybe null; in which case, all unstarred reports are deleted</param>
        /// <returns></returns>
        public string Delete(string requestIDs)
        {
            try
            {
                if (requestIDs != null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer json_serializer = ApiHelper.GetJsonSerializer;
                    var recordIDs = json_serializer.Deserialize<List<long>>(requestIDs);
                    Fbg.Bll.Report.DeleteReport(_player, recordIDs);
                } else
                {
                    Fbg.Bll.Report.DeleteReport(_player, null);
                }
            }
            catch (Exception exc)
            {
                return ApiHelper.RETURN_FAILURE(exc.Message);
            }

            return ApiHelper.RETURN_SUCCESS("DeleteSuccessful");
        }

            public string Forward(string reportRecordIDs, string forwardToPlayerName)
            {
                try
                {
                    List<long> recs = new List<long>();
                    recs.AddRange(reportRecordIDs.Split().Select(i => Convert.ToInt64(i)));

                    Fbg.Bll.Report.checkPlayer result = Fbg.Bll.Report.ForwardReport(forwardToPlayerName, _player, recs);

                    if (result == Fbg.Bll.Report.checkPlayer.Success)
                    {
                        return ApiHelper.RETURN_SUCCESS("ForwardedSuccessful");
                    }
                    else
                    {
                        return ApiHelper.RETURN_SUCCESS("PlayerNameNotFound" );
                    }
                }
                catch (Exception exc)
                {
                    return ApiHelper.RETURN_FAILURE(exc.Message);
                }
            }



            #region Report Entities
            public class ReportBase
            {
                internal Fbg.Bll.Report_base _rep;


                public ReportBase(Fbg.Bll.Report_base report)
                {
                    _rep = report;
                }


                public string Subject
                {
                    get
                    {
                        return _rep.Subject;
                    }
                }

                
                /// <summary>
                /// may return null
                /// </summary>
                public Fbg.Bll.Report.ForwardedBy ForwardedBy
                {
                    get
                    {
                        return _rep.ForwardedBy;
                    }
                }



                public DateTime ReportTime
                {
                    get
                    {
                        return _rep.ReportTime;
                    }
                }

            }
            public class Report_Misc : ReportBase
            {
                internal Fbg.Bll.Report_Misc _rep;


                public Report_Misc(Fbg.Bll.Report_Misc report)
                    : base(report)
                {
                    _rep = report;
                }

                public string Description
                {
                    get
                    {
                        return _rep.Description;
                    }
                }
            }
            public class Report_SupportAttacked : ReportBase
            {
                internal Fbg.Bll.Report_SupportAttacked _rep;


                public Report_SupportAttacked(Fbg.Bll.Report_SupportAttacked report)
                    : base(report)
                {
                    _rep = report;
                }

                public List<Fbg.Bll.Report_SupportAttacked.VillageSupporting> VillagesSupporting
                {
                    get
                    {
                        return _rep.VillagesSupporting;
                    }
                }

                // Who and what village was the support sent to

                public int SupportedPlayerID
                {
                    get
                    {
                        return _rep.SupportedPlayerID;
                    }
                }

                public short SupportedAvatarID
                {
                    get
                    {
                        return _rep.SupportedAvatarID;
                    }
                }

                public string SupportedPlayerName
                {
                    get
                    {
                        return _rep.SupportedPlayerName;
                    }
                }

                public string SupportedVillageName
                {
                    get
                    {
                        return _rep.SupportedVillageName;
                    }
                }

                public short SupportedVillageTypeID
                {
                    get
                    {
                        return _rep.SupportedVillageTypeID;
                    }
                }

                public int SupportedVillagePoints
                {
                    get
                    {
                        return _rep.SupportedVillagePoints;
                    }
                }

                public int SupportedVillageX
                {
                    get
                    {
                        return _rep.SupportedVillageX;
                    }
                }


                public int SupportedVillageY
                {
                    get
                    {
                        return _rep.SupportedVillageY;
                    }
                }

                public int SupportedVillageID
                {
                    get
                    {
                        return _rep.SupportedVillageID;
                    }
                }

            }
            public class Report_BattleDet : ReportBase
            {
                internal Fbg.Bll.Report_BattleDet _rep;
                

                public Report_BattleDet(Fbg.Bll.Report_BattleDet report) : base(report)
                {
                    _rep = report;
                }


                public Dictionary<string, string> BuildingIntel
                {
                    get
                    {
                        return _rep.GetAgregatedBuildingIntel.ToDictionary(r => r.Key.ID.ToString(), r => r.Value.ToString()); 
                    }
                }

                public List<Fbg.Bll.Report_BattleDet.BuildingsInvolved> BuildingsAttacked
                {
                    get
                    {
                        return _rep.BuildingsAttacked;
                    }
                }


                /// <summary>
                /// returns -32768 if flag not found
                /// </summary>
                public short Flag1
                {
                    get
                    {
                        return _rep.Flag1;
                    }
                }
                /// <summary>
                /// returns -32768 if flag not found
                /// </summary>
                public short Flag2
                {
                    get
                    {
                        return _rep.Flag2;
                    }
                }
                /// <summary>
                /// returns -32768 if morale not specified not found
                /// </summary>
                public short Morale
                {
                    get
                    {
                        return _rep.Morale;
                    }
                }

            /// <summary>
            /// Point of view of the report - is the attacker looking at this or defender.
            ///     This is true if it is the attacker looking at the report; or attacker forwarded the report to someone and that someone
            ///     is looking at the report
            /// </summary>
            public bool IsAttacker
                {
                    get
                    {
                        return _rep.IsAttacker;
                    }
                }


                public bool CanAttackerSeeDefendingTroops
                {
                    get
                    {

                        return _rep.CanAttackerSeeDefendingTroops;
                    }
                }

                public int DefendersCoins
                {
                    get
                    {
                        // defendersCoins
                        return _rep.DefendersCoins;
                    }
                }


                public bool DoesDefenderKnownsAttackersIdentity
                {
                    get
                    {
                        return _rep.DoesDefenderKnownsAttackersIdentity;
                    }
                }


                public bool SpyOnlyAttack
                {
                    get
                    {
                        return _rep.SpyOnlyAttack;
                    }
                }

                /// <summary>
                /// See Report.CONSTS.BattleReport.SummaryTblColIndex.SpyOutcomeValues for valid values
                /// </summary>
                public short SpyOutcome
                {
                    get
                    {

                        //spiesSuccessful
                        return _rep.SpyOutcome;
                    }
                }



                public int Plunder
                {
                    get
                    {
                        return _rep.Plunder;
                    }
                }


                /// <summary>
                /// can be null if not available. int otherwise
                /// </summary>
                public object LoyaltyBeforeAttack
                {
                    get
                    {
                        return _rep.LoyaltyBeforeAttack;
                    }
                }
                /// <summary>
                /// can be null if not available. int otherwise
                /// </summary>
                public object LoyaltyChange
                {
                    get
                    {
                        return _rep.LoyaltyChange;
                    }
                }

                

                public List<Fbg.Bll.Report_BattleDet.UnitsInvolved> AttackerUnits
                {
                    get
                    {
                        return _rep.AttackerUnits2;
                    }
                }

                public List<Fbg.Bll.Report_BattleDet.UnitsInvolved> DefenderUnits
                {
                    get
                    {
                        return _rep.DefenderUnits2;
                    }
                }


                public int DefenderPlayerID
                {
                    get
                    {
                        return _rep.DefenderPlayerID;
                    }
                }
                public string DefenderPlayerName
                {
                    get
                    {
                        return _rep.DefenderPlayerName;
                    }
                }


                public int DefenderVillageID
                {
                    get
                    {
                        return _rep.DefenderVillageID;
                    }
                }


                public int DefenderVillageXCord
                {
                    get
                    {
                        return _rep.DefenderVillageXCord;
                    }
                }


                public int DefenderVillageYCord
                {
                    get
                    {
                        return _rep.DefenderVillageYCord;
                    }
                }

                public int attackerVillageXCord
                {
                    get
                    {
                        return _rep.AttackerVillageXCord;
                    }
                }


                public int attackerVillageYCord
                {
                    get
                    {
                        return _rep.AttackerVillageYCord;
                    }
                }

                public int attackerVillageID
                {
                    get
                    {
                        return _rep.AttackerVillageID;
                    }
                }

                public int attackerPlayerID
                {
                    get
                    {
                        return _rep.AttackerPlayerID;
                    }
                }


                
                public string DefenderVillageName
                {
                    get
                    {
                        return _rep.DefenderVillageName;
                    }
                }

                public string attackerVillageName
                {
                    get
                    {
                        return _rep.AttackerVillageName;
                    }
                }

                public string AttackerPlayerName
                {
                    get
                    {
                        return _rep.AttackerPlayerName;
                    }
                }

               
                public short AttackerAvatarID
                {
                    get
                    {

                        return _rep.AttackerAvatarID;
                    }
                }
                public int AttackerVillagePoints
                {
                    get
                    {
                        return _rep.AttackerVillagePoints;
                    }
                }
                public short AttackerVillageTypeID
                {
                    get
                    {
                        return _rep.AttackerVillageTypeID;
                    }
                }
                public short DefenderAvatarID
                {
                    get
                    {
                        return _rep.DefenderAvatarID;
                    }
                }
                public int DefenderVillagePoints
                {
                    get
                    {
                        return _rep.DefenderVillagePoints;
                    }
                }
                public short DefenderVillageTypeID
                {
                    get
                    {
                        return _rep.DefenderVillageTypeID;
                    }
                }                

                #region player and village notes
                public string AttackerPlayerNote
                {
                    get
                    {
                        return _rep.AttackerPlayerNote;
                    }
                }
                public string AttackerVillageNote
                {
                    get
                    {
                        return _rep.AttackerVillageNote;
                    }
                }
                public string DefenderPlayerNote
                {
                    get
                    {
                        return _rep.DefenderPlayerNote;
                    }
                }
                public string DefenderVillageNote
                {
                    get
                    {
                        return _rep.DefenderVillageNote;
                    }
                }

                #endregion 



            }

            #endregion 

        }
    

}