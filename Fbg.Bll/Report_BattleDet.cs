using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Linq;

namespace Fbg.Bll
{
    public class Report_BattleDet : Report_base, Gmbc.Common.Diagnostics.ExceptionManagement.ISerializableToNameValueCollection2
    {
        DataSet _dsReport;
        DataTable _dtUnits;
        DataTable _dtBuildingIntel;
        DataTable _dtBuildingsAttacked;
        /// <summary>
        /// used by GetAnyBuildingIntel, see this property for more info
        /// </summary>
        Dictionary<BuildingType, int> _agregatedBuildingIntel = null;



        DataRow[] _attackerUnits;
        DataRow[] _defenderUnits;


        /// <summary>
        /// returns short.MinValue if flag not found
        /// </summary>
        public short Flag1
        {
            get
            {
                if (dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.Flag1] is DBNull)
                {
                    return short.MinValue;
                }
                return (short)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.Flag1];
            }
        }
        /// <summary>
        /// returns short.MinValue if flag not found
        /// </summary>
        public bool Flag1_HasFlag
        {
            get
            {
                return Flag1 != short.MinValue;
            }
        }
        /// <summary>
        /// returns short.MinValue if flag not found
        /// </summary>
        public short Flag2
        {
            get
            {
                if (dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.Flag2] is DBNull)
                {
                    return short.MinValue;
                }
                return (short)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.Flag2];
            }
        }
        
        /// <summary>
        /// returns short.MinValue if flag not found
        /// </summary>
        public bool Flag2_HasFlag
        {
            get
            {
                return Flag2 != short.MinValue;
            }
        }
        /// <summary>
        /// returns short.MinValue if flag not found
        /// </summary>
        public short Morale
        {
            get
            {
                if (dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.Flag3_Morale] is DBNull)
                {
                    return short.MinValue;
                }
                return (short)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.Flag3_Morale];
            }
        }
        ///// <summary>
        ///// gives you attackers units. NOT A SAFE CALL!! Use AttackerUnitsSafe instead!!
        ///// </summary>
        //public DataRow[] AttackerUnits
        //{
        //    get
        //    {
        //        return _attackerUnits;
        //    }
        //}

        /// <summary>
        /// Prefered way of getting the attacker units. this will return the units only if they are visible to the viewer
        /// </summary>
        public DataRow[] AttackerUnits
        {
            get
            {
                return _attackerUnits;
            }
        }

        ///// <summary>
        ///// gives you attackers units. NOT A SAFE CALL!! Use AttackerUnitsSafe instead!!
        ///// </summary>
        //public DataRow[] DefenderUnits
        //{
        //    get
        //    {
        //        return _defenderUnits;
        //    }
        //}
        /// <summary>
        /// Prefered way of getting the defender units. this will return the units only if they are visible to the viewer, null otherwise
        /// </summary>
        public DataRow[] DefenderUnits
        {
            get
            {
                DataRow[] defenderUnits = _defenderUnits;
                if (IsAttacker)
                {
                    if (!CanAttackerSeeDefendingTroops)
                    {
                        defenderUnits = null;
                    }
                }
                return defenderUnits;
            }
        }

        /// <summary>
        /// DEPRECIATED!! Use GetAgregatedBuildingIntel instead when possible
        /// Prefered way of getting the building intel as this will only return a table when building intel is avail, null otherwise
        /// This returns info gathered  by spies it spies were attacking and were succesful. 
        /// This will not return you info about attacked buildings
        /// </summary>
        public DataTable GetBuildingIntel
        {
            get
            {
                if (SpyOutcome != Report.CONSTS.BattleReport.SummaryTblColIndex.SpyOutcomeValues.NoSpies)
                {
                    if (SpyOutcome == Report.CONSTS.BattleReport.SummaryTblColIndex.SpyOutcomeValues.SpiesSuccessful)
                    {
                        return _dtBuildingIntel;
                    }
                }
                return null;
            }
        }


        /// <summary>
        /// same as getting GetAgregatedBuildingIntel and searching for the building in it.
        /// 
        /// returns -1 if not found; 
        /// </summary>
        public int GetBuildingLevelFromAgregatedIntel(int buildingID) 
        {
            foreach (KeyValuePair<BuildingType, int> val in GetAgregatedBuildingIntel)
            {
                if (val.Key.ID == buildingID)
                {
                    return val.Value;
                }
            }
            return -1;
        }
        /// <summary>
        /// This gives you a list of building levels that you know about from the attack. If spies got info, then this is what this returns
        /// however even if spies were not present, there is still a possibility for some info. For example, when you attack a building 
        /// with rams or cats, then you know the level after the attack. this will return you this info; ie, the building at the village
        /// after the attack
        /// If no info, an empty list is returned. 
        /// </summary>
        public Dictionary<BuildingType, int> GetAgregatedBuildingIntel
        {
            get
            {
                if (_agregatedBuildingIntel == null)
                {
                    if (GetBuildingIntel != null)
                    {
                        //
                        // got spy info, so grab this
                        //
                        _agregatedBuildingIntel = new Dictionary<BuildingType, int>(GetBuildingIntel.Rows.Count);
                        DataTable dt = GetBuildingIntel;
                        foreach (DataRow dr in dt.Rows) 
                        {
                            _agregatedBuildingIntel.Add(Player.Realm.BuildingType((int)dr[Report.CONSTS.BattleReport.BuildingsIntelTblColIndex.BuildingID])
                                , (int)dr[Report.CONSTS.BattleReport.BuildingsIntelTblColIndex.Level]);
                        }
                    }
                    else if (_dtBuildingsAttacked.Rows.Count > 0)
                    {
                        //
                        // got some buildings attacked info so grab this. 
                        //
                        _agregatedBuildingIntel = new Dictionary<BuildingType, int>(_dtBuildingsAttacked.Rows.Count);
                        foreach (DataRow dr in _dtBuildingsAttacked.Rows)
                        {
                            _agregatedBuildingIntel.Add(Player.Realm.BuildingType((int)dr[Report.CONSTS.BattleReport.BuildingsTblColIndex.BuildingID])
                                , (int)dr[Report.CONSTS.BattleReport.BuildingsTblColIndex.AfterAttackLevel]);
                        }
                    }
                    else
                    {
                        //
                        // got no info on buildings at all 
                        //
                        _agregatedBuildingIntel = new Dictionary<BuildingType, int>(0);
                    }
                }
                return _agregatedBuildingIntel;
            }
        }


        public class BuildingsInvolved
        {
            public int BuildingID { get; set; }
            public int BeforeAttackLevel { get; set; }
            public int AfterAttackLevel { get; set; }           

        }

        public List<BuildingsInvolved> BuildingsAttacked
        {
            get
            {
                if (_dtBuildingsAttacked.Rows.Count > 0) {
                    return GetBuildingsAttacked();
                } else {
                    return null;
                }
            }
        }


        private List<BuildingsInvolved> GetBuildingsAttacked()
        {

            return _dtBuildingsAttacked.AsEnumerable().Select(
                   dr => new BuildingsInvolved
                   {
                       BuildingID = (int)dr[Report.CONSTS.BattleReport.BuildingsTblColIndex.BuildingID]
                       ,
                       BeforeAttackLevel = (int)dr[Report.CONSTS.BattleReport.BuildingsTblColIndex.BeforeAttackLevel]
                       ,
                       AfterAttackLevel = (int)dr[Report.CONSTS.BattleReport.BuildingsTblColIndex.AfterAttackLevel]
                       
                   }
                   ).ToList();
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
                if ((short)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.PointOfView] == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public bool CanAttackerSeeDefendingTroops
        {
            get
            {

                // canAttackerSeeDefendingTroops
                if ((short)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.CanAttackerSeeDefTroops] == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int DefendersCoins
        {
            get {
            // defendersCoins
            return (int)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.DefendersCoins];
            }
        }

         
        public bool DoesDefenderKnownsAttackersIdentity
        {
            get
            {
                // doesDefenderKnownsAttackersIdentity
                if ((short)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.DefenderKnowsAttackersIdentity] == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public bool SpyOnlyAttack
        {
            get
            {
                if (_attackerUnits.Length == 1)
                {
                    if ((int)_attackerUnits[0][Report.CONSTS.BattleReport.UnitsTblColIndex.UnitTypeID] == UnitTypeSpy.SpyUnitID)
                    {
                        return true;
                    }
                }
                return false;
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
                return (short)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.SpyOutcome];
            }
        }


        
        public int Plunder
        {
            get
            {
                return (Int32)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.Loot];
            }
        }


        /// <summary>
        /// can be null if not available. int otherwise
        /// </summary>
        public object LoyaltyBeforeAttack
        {
            get
            {
                if (dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.LoyaltyBeforeAttack] is DBNull)
                {
                    return null;
                }
                else
                {
                    return dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.LoyaltyBeforeAttack];
                }
            }
        }
        /// <summary>
        /// can be null if not available. int otherwise
        /// </summary>
        public object LoyaltyChange
        {
            get
            {
                if (LoyaltyBeforeAttack == null)
                {
                    return null;
                }
                else
                {
                    return (int)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.LoyaltyChange];
                }
            }
        }


        public DataTable dtUnits 
        {
            get
            {
                return _dtUnits;
            }
        }
        public DataTable dtBuildingIntel 
        {
            get
            {
                return _dtBuildingIntel;
            }
        }
        public DataTable dtBuildingsAttacked
        {
            get
            {
                return _dtBuildingsAttacked;
            }
        }

        public DateTime ReportTime_UniversalTime 
        {
            get {
                return ((DateTime)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.Time]).ToUniversalTime();
            }
        }

        public int DefenderVillagePoints
        {
            get
            {
                return (int)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.DefenderVillagePoints];
            }
        }

        public short DefenderVillageTypeID
        {
            get
            {
                return (short)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.DefenderVillageTypeID];
            }
        }
                
        public short DefenderAvatarID
        {
            get
            {
                return (short)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.DefenderAvatarID];
            }
        }


        public int DefenderPlayerID
        {
            get {
                return (int)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.DefenderPlayerID];
            }
        }

        public string DefenderPlayerName
        {
            get {
                return (string)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.DefenderPlayerName];
            }
        }
        public string DefenderVillageName
        {
            get
            {
                return dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.DefenderVillageName].ToString();
            }
        }


        public int DefenderVillageID
        {
            get {
                return (int)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.DefenderVillageID];
            }
        }


        public int DefenderVillageXCord
        {
            get
            {
                return (int)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.DefenderVillageX];
            }
        }


        public int DefenderVillageYCord
        {
            get
            {
                return (int)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.DefenderVillageY];
            }
        }



        // Determines if the attacker info should be included or not
        private bool ShouldHideAttackerIdentity()
        {
            if ((short)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.SpyOutcome] != Report.CONSTS.BattleReport.SummaryTblColIndex.SpyOutcomeValues.NoSpies)
            {
                if ((short)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.PointOfView] != 0)
                {
                    if ((short)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.DefenderKnowsAttackersIdentity] != 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }



        public int AttackerVillageXCord
        {
            get
            {
                if (ShouldHideAttackerIdentity())
                {
                    return 0;
                }
                else
                {
                    return (int)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.AttackerVillageX];
                }
            }
        }


        public int AttackerVillageYCord
        {
            get
            {
                if (ShouldHideAttackerIdentity())
                {
                    return 0;
                }
                else
                {
                    return (int)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.AttackerVillageY];
                }
            }
        }

        public int AttackerVillageID
        {
            get
            {
                if (ShouldHideAttackerIdentity())
                {
                    return 0;
                }
                else
                {
                    return (int)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.AttackerVillageID];
                }
            }
        }
        public string AttackerVillageName
        {
            get
            {
                if (ShouldHideAttackerIdentity())
                {
                    return "Unknown";
                }
                else
                {
                    return dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.AttackerVillageName].ToString();
                }
            }
        }
        
        public int AttackerVillagePoints
        {
            get
            {
                if (ShouldHideAttackerIdentity())
                {
                    return 0;
                }
                else
                {
                    return (int)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.AttackerVillagePoints];
                }
            }
        }

        public short AttackerVillageTypeID
        {
            get
            {
                if (ShouldHideAttackerIdentity())
                {
                    return 0;
                }
                else
                {
                    return (short)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.AttackerVillageTypeID];
                }
            }
        }

        public short AttackerAvatarID
        {
            get
            {
                if (ShouldHideAttackerIdentity())
                {
                    return 0;
                }
                else
                {
                    return (short)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.AttackerAvatarID];
                }
            }
        }

        public int AttackerPlayerID
        {
            get
            {
                if (ShouldHideAttackerIdentity())
                {
                    return 0;
                }
                else
                {
                    return (int)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.AttackerPlayerID];
                }
            }
        }
        public string AttackerPlayerName
        {
            get
            {
                if (ShouldHideAttackerIdentity())
                {
                    return "Unknown";
                }
                else
                {
                    return (string)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.AttackerPlayerName];
                }
            }
        }


        #region player and village notes
        public string AttackerPlayerNote 
        {
            get 
            {
                if (ShouldHideAttackerIdentity())
                {
                    return "";
                }
                else
                {
                    return (String)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.AttackerPlayerPN];
                }
            }
        }
        public string AttackerVillageNote 
        {
            get 
            {
                if (ShouldHideAttackerIdentity())
                {
                    return "";
                }
                else
                {
                    return (String)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.AttackerVillagePN];
                }
            }
        }
        public string DefenderPlayerNote
        {
            get
            {
                return (String)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.DefenderPlayerPN];
            }
        }
        public string DefenderVillageNote
        {
            get
            {
                return (String)dtSummary.Rows[0][Report.CONSTS.BattleReport.SummaryTblColIndex.DefenderVillagePN];
            }
        }

        #endregion 


        /// <summary>
        /// returns null if this info is not available
        /// </summary>
        /// <param name="buildingID"></param>
        /// <returns></returns>
        public object BuildingLevelNumberAfterAttack(int buildingID)
        {
            DataRow[] drs = dtBuildingIntel.Select(String.Format("{0} = {1}", Report.CONSTS.BattleReport.BuildingsIntelTblColNames.BuildingID, buildingID));

            if (drs.Length > 0 ) 
            {
                return drs[0][Report.CONSTS.BattleReport.BuildingsTblColIndex.AfterAttackLevel];
            }
            return null;
        }


        public class UnitsInvolved
        {
            public int UnitTypeID { get; set; }
            public int DeployedUnitCount { get; set; }
            public int KilledUnitCount { get; set; }
            public int ReaminingUnitCount { get; set; }

        }


        /// <summary>
        /// Prefered way of getting the attacker units. this will return the units only if they are visible to the viewer
        /// </summary>
        public List<UnitsInvolved> AttackerUnits2
        {
            get
            {
                return GetUnitsByParty(Fbg.Bll.Report.CONSTS.BattleReport.Party.Attacker);
            }
        }


        private List<UnitsInvolved> GetUnitsByParty(int party)
        {
            return _dtUnits.AsEnumerable().Where(s => (short)s[Fbg.Bll.Report.CONSTS.BattleReport.UnitsTblColNames.Party] == party).Select(
                   s => new UnitsInvolved
                   {
                       UnitTypeID = (int)s[Fbg.Bll.Report.CONSTS.BattleReport.UnitsTblColIndex.UnitTypeID]
                       ,
                       DeployedUnitCount = (int)s[Fbg.Bll.Report.CONSTS.BattleReport.UnitsTblColIndex.DeployedUnitCount]
                       ,
                       KilledUnitCount = (int)s[Fbg.Bll.Report.CONSTS.BattleReport.UnitsTblColIndex.KilledUnitCount]
                       ,
                       ReaminingUnitCount = (int)s[Fbg.Bll.Report.CONSTS.BattleReport.UnitsTblColIndex.ReaminingUnitCount]
                   }
                   ).ToList();
        }

        /// <summary>
        /// Prefered way of getting the defender units. this will return the units only if they are visible to the viewer, null otherwise
        /// </summary>
        public List<UnitsInvolved> DefenderUnits2
        {
            get
            {
                if (IsAttacker)
                {
                    if (!CanAttackerSeeDefendingTroops)
                    {
                        return null;
                    }
                }
                return  GetUnitsByParty(Fbg.Bll.Report.CONSTS.BattleReport.Party.Defender);;
            }
        }

        public Report_BattleDet(Player player, int recordID) : base(player, recordID
            , Report.CONSTS.BattleReport.SummaryTblColIndex.Subject
            , Report.CONSTS.BattleReport.SummaryTblColIndex.Time
            , Report.CONSTS.BattleReport.SummaryTblColIndex.ForwardPlayerID)
        {
            
            InitReport(player, recordID, DAL.Report.GetBattleReportDetails(player.Realm.ConnectionStr, player.ID, recordID));
        }

        public void InitReport(Player player, int recordID, DataSet ds)
        {
            
            try
            {               
                _dsReport = ds;               

                base.InitReport(_dsReport.Tables[Report.CONSTS.BattleReport.TableIndex.Summary]
                    , Report.CONSTS.BattleReport.SummaryTblColIndex.ForwardedPlayerName
                    , Report.CONSTS.BattleReport.SummaryTblColIndex.ForwardedOn
                    , Report.CONSTS.BattleReport.SummaryTblColIndex.ForwardPlayerID
                    , Report.CONSTS.BattleReport.SummaryTblColIndex.ForwardedPlayerAvatarID);

                _dtUnits = _dsReport.Tables[Report.CONSTS.BattleReport.TableIndex.Units];
                _dtBuildingIntel = _dsReport.Tables[Report.CONSTS.BattleReport.TableIndex.BuildingIntel];
                _dtBuildingsAttacked = _dsReport.Tables[Report.CONSTS.BattleReport.TableIndex.Buildings];
               
                //
                // did we get a report ?
                //
                if (dtSummary.Rows.Count > 0)
                {
                    _attackerUnits = _dtUnits.Select(String.Format("{0}={1}", Report.CONSTS.BattleReport.UnitsTblColNames.Party
                        , Report.CONSTS.BattleReport.Party.Attacker));

                    _defenderUnits = _dtUnits.Select(String.Format("{0}={1}", Report.CONSTS.BattleReport.UnitsTblColNames.Party
                        , Report.CONSTS.BattleReport.Party.Defender));

                }

                CheckIfQuestsSatisfied(player);
                
            } // TRY 

            catch (Exception ex)
            {
                BaseApplicationException bex = new BaseApplicationException("Error in InitReport", ex);
                throw bex;
            }
       

        }

        public void CheckIfQuestsSatisfied(Player player)
        {
            
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
                    BaseApplicationException.AddAdditionalInformation(col, "_dtUnits", _dtUnits);
                    BaseApplicationException.AddAdditionalInformation(col, "_dtBuildingIntel", _dtBuildingIntel);
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
