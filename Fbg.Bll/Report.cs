using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Fbg.Bll
{
   
    public class Report
    {

        public class ForwardedBy
        {
            public string ForwardedPlayerName;
            public DateTime ForwardedOn;
            public short ForwardedPlayerAvatarID;
            public int ForwardedPlayerID;

            public ForwardedBy(string forwardedPlayerName, DateTime forwardedOn,  int forwardedPlayerID, short forwardedPlayerAvatarID)
            {
                ForwardedOn = forwardedOn;
                ForwardedPlayerName = forwardedPlayerName;
                ForwardedPlayerAvatarID = forwardedPlayerAvatarID;
                ForwardedPlayerID = forwardedPlayerID;
            }


        }
        public class CONSTS
        {
            /// <summary>
            /// Descriptions of the dataset returned from GetBattleReportDetails(..)
            /// </summary>
            public class BattleReport
            {
                public class TableIndex
                {
                    public static int Summary = 0;
                    public static int Units = 1;
                    public static int Buildings = 2;
                    public static int BuildingIntel = 3;
                }

                public class SummaryTblColIndex
                { 
                    public static int Time = 0;
                    public static int Subject = 1;
                    public static int ReportType = 2;
                    public static int AttackerPlayerName = 3;
                    public static int AttackerPlayerID = 4;
                    public static int AttackerVillageID = 5;
                    public static int AttackerVillageName = 6;
                    public static int DefenderPlayerName = 7;
                    public static int DefenderPlayerID = 8;
                    public static int DefenderVillageID = 9;
                    public static int DefenderVillageName = 10;
                    public static int Loot = 11;
                    public static int LoyaltyBeforeAttack = 12;
                    public static int LoyaltyChange = 13;
                    public static int PointOfView = 14;
                    public static int CanAttackerSeeDefTroops = 15;
                    public static int DefendersCoins = 16;
                    public static int DefenderKnowsAttackersIdentity = 17;                  
                    /// <summary>
                    /// -1 - no spies too part in the attack
                    /// 0 - spies took part in the attack but were unsucessful 
                    /// 1 - spies took part in the attack and were sucessful
                    /// </summary>
                    public static int SpyOutcome = 18;
                    public static int ForwardPlayerID = 19;
                    public static int ForwardedOn = 20;
                    public static int RecordID = 21;
                    public static int ForwardedPlayerName = 22;
                    public static int DefenderVillageX = 23;
                    public static int DefenderVillageY = 24;
                    public static int AttackerVillageX = 25;
                    public static int AttackerVillageY = 26;
                    public static int AttackerPlayerPN = 27;
                    public static int AttackerVillagePN = 28;
                    public static int DefenderPlayerPN = 29;
                    public static int DefenderVillagePN = 30;
                    public static int Flag1 = 31;
                    public static int Flag2 = 32;
                    // New columns addded here to prevent column index
                    // issues
                    public static int AttackerAvatarID = 33;
                    public static int AttackerVillagePoints = 34;
                    public static int AttackerVillageTypeID = 35;
                    public static int DefenderAvatarID = 36;
                    public static int DefenderVillagePoints = 37;
                    public static int DefenderVillageTypeID = 38;
                    public static int ForwardedPlayerAvatarID = 39;
                    public static int Flag3_Morale = 40;

                    public class SpyOutcomeValues
                    {
                        public static int NoSpies = -1;
                        public static int SpiesFailed = 0;
                        public static int SpiesSuccessful = 1;
                    }
                }

                public class UnitsTblColIndex
                {
                    public static int ReportID = 0;
                    public static int Party = 1;
                    public static int UnitTypeID = 2;
                    public static int DeployedUnitCount = 3;
                    public static int KilledUnitCount = 4;
                    public static int ReaminingUnitCount = 5;
                }

                public class BuildingsTblColIndex
                {
                    public static int BuildingName = 0;
                    public static int BeforeAttackLevel = 1;
                    public static int AfterAttackLevel = 2;
                    public static int BuildingID= 3;
                }

                public class BuildingsIntelTblColIndex
                {
                    public static int BuildingName = 0;
                    public static int BuildingID = 1;
                    public static int Level = 2;
                }
                public class BuildingsIntelTblColNames
                {
                    public static string BuildingID = "BuildingTypeID";
                    public static string Level = "Level";
                }


                public class UnitsTblColNames
                {
                    public static string Party = "Party";
                    public static string UnitTypeID= "UnitTypeID";

                }

                public class Party
                {
                    public static int Attacker = 0;
                    public static int Defender = 1;
                }


            }

            /// <summary>
            /// Descriptions of the dataset returned from FilterReportList
            /// </summary>
            public class ReportList
            {
                public class TableIndex
                {
                    public static int ReportList = 0;
                    public static int Status = 1;
                }

                public class ReportListIndex
                {
                    public static int ReportTime = 0;
                    public static int Subject = 1;
                    public static int ReportType = 2;
                    public static int IsViewed = 3;
                    public static int ReportTypeID = 4;
                    public static int IsForwarded = 5;
                    public static int RecordID = 6;
                    public static int FlagID1= 7;
                    public static int FlagID2 = 8;
                    public static int FlagID3 = 9;
                    public static int WhatSide = 10;
                    public static int FolderID = 11;
                    public static int FolderName = 12;
                }
                public class ReportListColNames
                {
                    public static string RecordID = "RecordID";
                }



                /// <summary>
                /// returns status of the VillageName, 0=Valid Valid VillageName, 1=Invalid Invalid VillageName
                /// </summary>
                public class StatusIndex
                {
                    public static int Status = 0;
                }
            }


            /// <summary>
            /// Descriptions of the dataset returned from GetSupportAttackedReportDetails(..)
            /// </summary>
            public class SupportAttackedReport
            {
                public class TableIndex
                {
                    public static int Summary = 0;
                    public static int Units = 1;
                }

                public class SummaryTblColIndex
                {
                    public static int Time = 0;
                    public static int Subject = 1;
                    public static int ReportType = 2;
                    public static int SupportingPlayerName = 3;
                    public static int SupportingPlayerID = 4;
                    public static int SupportingVillageID = 5;
                    public static int SupportingVillageName = 6;
                    public static int SupportingVillageXCord = 7;
                    public static int SupportingVillageYCord = 8;
                    public static int ForwardedByPlayerID = 9;
                    public static int ForwardedOn = 10;
                    public static int RecordID = 11;
                    public static int ForwardedByPlayerName = 12;

                    public static int SupportedPlayerID = 13;
                    public static int SupportedAvatarID = 14;
                    public static int SupportedPlayerName = 15;
                    public static int SupportedVillageName = 16;
                    public static int SupportedVillageTypeID = 17;
                    public static int SupportedVillagePoints = 18;
                    public static int SupportedVillageX = 19;
                    public static int SupportedVillageY = 20;
                    public static int SupportedVillageID = 21;

                    public static int ForwardedPlayerAvatarID = 22;
                }

                public class UnitsTblColIndex
                {
                    public static int SupportingVillageID = 0;
                    public static int UnitTypeID = 1;
                    public static int DeployedUnitCount = 2;
                    public static int KilledUnitCount = 3;
                    public static int ReaminingUnitCount = 4;
                }

                public class UnitsTblColNames
                {
                    public static string SupportingVillageID = "SupportingVillageID";
                }
            }


            /// <summary>
            /// The Datatable returned from GetReports
            /// </summary>

            public class ReportTypes
            {
                public static int ReportTypeID = 0;
                public static int Description = 1;
            }

            public class ReportTypeDetails
            {
                public class MiscReportIndex
                {
                    public static int Time = 0;
                    public static int Subject = 1;
                    public static int ReportTypeID = 2;
                    public static int Description = 3;
                    public static int ForwardedByPlayerID = 4;
                    public static int ForwardedByPlayerName = 5;
                    public static int ForwardedOn = 6;

                    public static int ForwardedPlayerAvatarID = 7;
                }
            }

            public const int ArchiveReportsInXDays = 14;

        }

        // Check Player Is Valid
        public enum checkPlayer : int
        {
            Success = 0,
            Cannot_find_player_name = 1
        }

        // Check VillageName is Valid
        public enum checkVillage : int
        {
            Success = 0,
            Cannot_find_village = 1
        }

        //Get ReportType
        public enum reportType : int
        {
            Attack = 1
            ,Support = 2
            ,Silver = 3
            ,Misc = 4
            ,YourSupportArrived = 5
            ,SupportArrived = 6
            , SupportSentBack =7
            ,SupportPulledOut=8

        }

        //public static DataTable GetReports(Player player)
        //{
        //    return DAL.Report.GetReports(player.Realm.ConnectionStr, player.ID);
        //}

        //public static DataTable GetReports(string connectionString, int playerID)
        //{
        //    return DAL.Report.GetReports(connectionString, playerID);
        //}

        public static Report_BattleDet GetBattleReportDetails(Player player, int recordID)
        {
            return new Report_BattleDet(player, recordID);
        }

        public static DataSet GetSupportAttackedReportDetails(Player player, int recordID)
        {
            return DAL.Report.GetSupportAttackedReportDetails(player.Realm.ConnectionStr, player.ID, recordID);
        }

        /// <summary>
        /// Delete reports
        /// </summary>
        /// <param name="recordIDs">Could be null!!!</param>
        public static void DeleteReport(Player player, List<Int64> recordIDs)
        {
            if (player == null)
            {
                throw new ArgumentNullException("Player is null");
            }

            DAL.Report.DeleteReport(player.Realm.ConnectionStr, player.ID, recordIDs);
        }

        /// <summary>
        /// Move reports to a folder
        /// </summary>
        /// <param name="folderID">sent -1 if moving to inbox</param>
        public static void MoveReporsToFolder(Player player, List<Int64> recordIDs, int folderID)
        {
            if (player == null)
            {
                throw new ArgumentNullException("Player is null");
            }

            DAL.Report.MoveReportsToFolder(player.Realm.ConnectionStr, player.ID, recordIDs, folderID);
        }

        /// <summary>
        /// Forward report to the PlayerName
        /// </summary>

        public static checkPlayer ForwardReport(string PlayerName, Player player, List<Int64> recordIDs)
        {
            if (player == null)
            {
                throw new ArgumentNullException("Player is null");
            }
            return (checkPlayer)Enum.Parse(typeof(checkPlayer), DAL.Report.ForwardReport(player.Realm.ConnectionStr, player.ID, PlayerName,recordIDs).ToString());
        }

        /// <summary>
        /// Get Report Type Details
        /// </summary>

        public static DataSet GetReportTypeDetails(Player player, int recordID)
        {
            return DAL.Report.GetReportTypeDetails(player.Realm.ConnectionStr, player.ID, recordID);
        }
         

        #region "GetReportType"

        private static DataTable dtReportTypes;
        /// <summary>
        /// DataTable to get All Report Types
        /// </summary>
        public static DataTable ReportTypes(Player player)
        {
            if (dtReportTypes == null)
            {
                dtReportTypes = DAL.Report.GetReportType(player.Realm.ConnectionStr);
                dtReportTypes.Constraints.Add("PKey", dtReportTypes.Columns[CONSTS.ReportTypes.ReportTypeID], true);
            }
            return dtReportTypes;
        }
        public static DataRow ReportType(Player player, int reportTypeID )
        {
            return ReportTypes(player).Rows.Find(reportTypeID);
        }


        #endregion

        /// <summary>
        /// Get Report List Dataset: ReportDetails and Status Tables
        /// </summary>
        /// <param name="ReportTypeID">can be set as null</param>
        /// <param name="SearchText">can be set as null</param>
        /// <param name="villageXCord">can be set toInt32.MinValue1 if not searching by village</param>
        /// <param name="villageYCord">can be set to Int32.MinValue if not searching by village</param>
        /// <param name="selectedFolderID">can be set to -1 if showing inbox</param>
        /// <returns>return Dataset having two Tables,ReportDetails Table and Status Tables, Reports.Consts.ReportList have each column details </returns>      
        public static DataSet ReportList(Player player, int ReportTypeID, int villageXCord, int villageYCord
            , string SearchTxt, int selectedFolderID, bool showArchived, bool retrieveAllData)
        {
            return DAL.Report.GetReportList(player.Realm.ConnectionStr, player.ID, ReportTypeID
                , villageXCord, villageYCord, SearchTxt, selectedFolderID, showArchived ? 9999 : CONSTS.ArchiveReportsInXDays, retrieveAllData);
        }

        /// <summary>
        /// Get Report List Dataset: ReportDetails and Status Tables, includes Inbox and all folders
        /// </summary>
        /// <param name="ReportTypeID">can be set as null</param>
        /// <param name="SearchText">can be set as null</param>
        /// <param name="villageXCord">can be set toInt32.MinValue1 if not searching by village</param>
        /// <param name="villageYCord">can be set to Int32.MinValue if not searching by village</param>
        /// <param name="selectedFolderID">can be set to -1 if showing inbox</param>
        /// <returns>return Dataset having two Tables,ReportDetails Table and Status Tables, Reports.Consts.ReportList have each column details </returns>      
        public static DataSet ReportListAll(Player player, int ReportTypeID, int villageXCord, int villageYCord
            , string SearchTxt, bool showArchived, bool retrieveAllData)
        {
            return DAL.Report.GetReportListAll(player.Realm.ConnectionStr, player.ID, ReportTypeID
                , villageXCord, villageYCord, SearchTxt, showArchived ? 9999 : CONSTS.ArchiveReportsInXDays, retrieveAllData);
        }

    }

}