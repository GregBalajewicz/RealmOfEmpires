using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for NavigationHelper
/// </summary>
public static class NavigationHelper
{
    #region variables
    private static string cancelEvent = "~/CancelEvent.aspx?" + CONSTS.QuerryString.EventType 
        + "={0}&" + CONSTS.QuerryString.EventID 
        + "={1}&" + CONSTS.QuerryString.ReturnUrl
        + "={2}&" + CONSTS.QuerryString.Data
        + "={3}";

    private static string villageOverview_Tilda = "~/VillageOverview.aspx?" + CONSTS.QuerryString.SelectedVillageID + "={0}";
    private static string villageOverview = "VillageOverview.aspx?" + CONSTS.QuerryString.SelectedVillageID + "={0}";
    private static string villageOverviewDet = "VillageOverview.aspx?" + CONSTS.QuerryString.SelectedVillageID + "={0}&" + CONSTS.QuerryString.ShowDetails+ "=1";
    private static string villageOtherOverview = "~/VillageOverviewOther.aspx?" + CONSTS.QuerryString.OtherVillageID + "={0}";
    private static string villageOtherOverviewPopup = "~/VillageOverviewOtherPopup.aspx?" + CONSTS.QuerryString.OtherVillageID + "={0}";
    private static string villageOtherOverview_NoTilda = "VillageOverviewOther.aspx?" + CONSTS.QuerryString.OtherVillageID + "={0}";
    private static string villageOtherOverview_NoTildaPopup = "VillageOverviewOtherPopup.aspx?" + CONSTS.QuerryString.OtherVillageID + "={0}";
    private static string clanPublicProfile = "ClanPublicProfile.aspx?" + CONSTS.QuerryString.ClanID + "={0}";
    private static string villageBuildings = "VillageBuildings.aspx?" + CONSTS.QuerryString.SelectedVillageID + "={0}";
    private static string sentMessageDetails = "~/messagedetail.aspx?" + CONSTS.QuerryString.MessageID + "={0}&" + CONSTS.QuerryString.MailType + "={1}";
    private static string inboxMessageDetails = "~/messageDetail.aspx?" + CONSTS.QuerryString.RecordID + "={0}";
    private static string messageList = "~/messages.aspx?" + CONSTS.QuerryString.FolderID + "={0}";
    private static string messageList_archived = "~/messages.aspx?" + CONSTS.QuerryString.ViewArchived + "={0}";
    private static string messageList_full = "~/messages.aspx?" + CONSTS.QuerryString.FolderID + "={0}&" + CONSTS.QuerryString.ViewArchived + "={1}";
    private static string reportsList_ByRT = "~/reports.aspx?" + CONSTS.QuerryString.ReportTypeID + "={0}";
    private static string reportsList_ByFID = "~/reports.aspx?" + CONSTS.QuerryString.FolderID + "={0}";
    private static string reportsList_ByArchived= "~/reports.aspx?" + CONSTS.QuerryString.ViewArchived + "={0}";
    private static string folders = "~/folders.aspx?" + CONSTS.QuerryString.FolderType+ "={0}";
    
    private static string messageMove = "~/MessageMove.aspx?" + CONSTS.QuerryString.RecordID + "={0}&" + CONSTS.QuerryString.MoveToFolderID+ "={1}";    
    private static string createMailBlank = "messages_create.aspx";
    private static string createMailBlank_ConfimMsgSent = "messages_create.aspx?ok=1";
    private static string commandTroops = "CommandTroops.aspx?" + CONSTS.QuerryString.XCord + "={0}&" + CONSTS.QuerryString.YCord + "={1}&" + CONSTS.QuerryString.CommandType + "={2}";
    private static string commandTroopsPopup = "CommandTroopsPopup.aspx?" + CONSTS.QuerryString.XCord + "={0}&" + CONSTS.QuerryString.YCord + "={1}&" + CONSTS.QuerryString.CommandType + "={2}";
    private static string createMailTo = "messages_create.aspx?" + CONSTS.QuerryString.RecipientName + "={0}";
    private static string createMailWithMsg = "messages_create.aspx?" + CONSTS.QuerryString.RecordID + "={0}&" + CONSTS.QuerryString.Action + "={1}";

    private static string BuildingPage_VHQ = "~/VillageBuildings.aspx";
    private static string BuildingPage_TP = "~/TransportSilver.aspx";
    private static string BuildingPage_Generic = "~/Building.aspx?bid={0}";
    private static string BuildingPage_Troops = "~/BuildingTroops.aspx?bid={0}";
    private static string BuildingPage_Defensive = "~/BuildingDef.aspx?bid={0}";
    private static string BuildingPage_Palace = "~/VillageGovernerRecruit.aspx";

    private static string unitHelp= "~/help.aspx?" + CONSTS.QuerryString.RealmID + "={0}&" + CONSTS.QuerryString.HelpPageType + "=unit&" + CONSTS.QuerryString.HelpObjectID + "={1}";
    private static string buildingHelp = "~/help.aspx?" + CONSTS.QuerryString.RealmID + "={0}&" + CONSTS.QuerryString.HelpPageType + "=Building&" + CONSTS.QuerryString.HelpObjectID + "={1}";
    private static string playerPublicOverviewByName = "~/Player.aspx?" + CONSTS.QuerryString.PlayerName + "={0}";
    private static string playerPublicOverviewByNamePopup = "~/PlayerPopup.aspx?" + CONSTS.QuerryString.PlayerName + "={0}";
    private static string map2byCords = "~/map_villreduction.aspx?" + CONSTS.QuerryString.XCord + "={0}&" + CONSTS.QuerryString.YCord + "={1}";
    private static string map2byCordsAndVillage = "~/map_villreduction.aspx?" + CONSTS.QuerryString.SelectedVillageID + "={0}&" + CONSTS.QuerryString.XCord + "={1}&" + CONSTS.QuerryString.YCord + "={2}";
    private static string mapbyCords = "~/map.aspx?" + CONSTS.QuerryString.XCord + "={0}&" + CONSTS.QuerryString.YCord + "={1}";
    private static string mapbyCordsAndVillage = "~/map.aspx?" + CONSTS.QuerryString.SelectedVillageID + "={0}&" + CONSTS.QuerryString.XCord + "={1}&" + CONSTS.QuerryString.YCord + "={2}";
    private static string TransportCoinsbyCords = "~/TransportSilver.aspx?" + CONSTS.QuerryString.XCord + "={0}&" + CONSTS.QuerryString.YCord + "={1}";
    private static string transportCoinsWithVillage = "~/TransportSilver.aspx?" + CONSTS.QuerryString.SelectedVillageID + "={0}";
    private static string quickSendCoins = "~/QuickSendSilver.aspx?" + CONSTS.QuerryString.VillageID + "={0}&"+ CONSTS.QuerryString.XCord + "={1}&" + CONSTS.QuerryString.YCord + "={2}";
   

    private static string playerPublicOverview= "~/Player.aspx?" + CONSTS.QuerryString.PlayerID + "={0}";
    private static string playerPublicOverviewPopup = "~/PlayerPopup.aspx?" + CONSTS.QuerryString.PlayerID + "={0}";
    private static string playerPublicOverview_NoTilda = "Player.aspx?" + CONSTS.QuerryString.PlayerID + "={0}";
    private static string playerPublicOverview_NoTildaPopup = "PlayerPopup.aspx?" + CONSTS.QuerryString.PlayerID + "={0}";

    private static readonly  string reports = "Reports.aspx?" + CONSTS.QuerryString.SelectedVillageID + "={0}";
    private static readonly string reports_attackDetails = "Report_AttackDetails.aspx?" + CONSTS.QuerryString.RecordID+ "={0}";

    private static readonly string PFOff = "~/PFOff.aspx?" + CONSTS.QuerryString.PFID+ "={0}";
    private static readonly string pfDesc = "~/PFDesc.aspx?" + CONSTS.QuerryString.PFID + "={0}";

    private static readonly string title= "~/TitleRanking.aspx?" + CONSTS.QuerryString.TitleID + "={0}";

    #endregion 

    #region methods
    public static string Folders(Fbg.Bll.Folders.FolderType folderType)
    {
        return String.Format(folders, (int)folderType);
    }
    public static string ReportsList_ShowArchived(bool showArchived)
    {
        return String.Format(reportsList_ByArchived, showArchived);
    }
    public static string ReportsList_ByReportType(int reportType)
    {
        return String.Format(reportsList_ByRT, reportType);
    }
    public static string ReportsList_ByFolderID(int folderID)
    {
        return String.Format(reportsList_ByFID, folderID);
    }

    public static string CancelEvent(EventType eventType, long eventID, string returnUrl)
    {
        return String.Format(cancelEvent, (int)eventType, eventID, HttpContext.Current.Server.UrlEncode(returnUrl), String.Empty);
    }
    public static string CancelEvent(EventType eventType, long eventID, string returnUrl, string optionalData)
    {
        return String.Format(cancelEvent, (int)eventType, eventID, HttpContext.Current.Server.UrlEncode(returnUrl), HttpContext.Current.Server.UrlEncode(optionalData));
    }

    public static string VillageOverview_Tilda(int villageID)
    {
        return String.Format(villageOverview_Tilda, villageID);
    }
    public static string VillageOverview(int villageID)
    {
        return String.Format(villageOverview, villageID);
    }
    public static string VillageOverview(int villageID, bool showDetail)
    {
        if (showDetail)
        {
            return String.Format(villageOverviewDet, villageID);
        }
        else
        {
            return String.Format(villageOverview, villageID);
        }
    }

    public static string VillageBuildings(int villageID)
    {
        return String.Format(villageBuildings, villageID);
    }

    public static string ClanPublicProfile(int clanID)
    {
        return String.Format(clanPublicProfile, clanID);
    }

    public static string VillageOtherOverview(int villageID, bool inPopup)
    {
        if (inPopup)
        {
            return VillageOtherOverviewPopup(villageID);
        }
        else
        {
            return VillageOtherOverview(villageID);
        }
    }
    public static string VillageOtherOverview(int villageID)
    {
        return String.Format(villageOtherOverview, villageID);
    }
    public static string VillageOtherOverviewPopup(int villageID)
    {
        return String.Format(villageOtherOverviewPopup, villageID);
    }
    public static string VillageOtherOverview_NoTilda(int villageID, bool inPopup)
    {
        if (inPopup)
        {
            return VillageOtherOverview_NoTildaPopup(villageID);
        }
        else
        {
            return VillageOtherOverview_NoTilda(villageID);
        }
    }
    public static string VillageOtherOverview_NoTilda(int villageID)
    {
        return String.Format(villageOtherOverview_NoTilda, villageID);
    }
    public static string VillageOtherOverview_NoTildaPopup(int villageID)
    {
        return String.Format(villageOtherOverview_NoTildaPopup, villageID);
    }
    public static string PlayerPublicOverview(int playerID, bool inPopup)
    {
        if (inPopup)
        {
            return PlayerPublicOverviewPopup(playerID);
        }
        else
        {
            return PlayerPublicOverview(playerID);
        }
    }
    public static string PlayerPublicOverview(int playerID)
    {
        return String.Format(playerPublicOverview, playerID);
    }
    public static string PlayerPublicOverviewPopup(int playerID)
    {
        return String.Format(playerPublicOverviewPopup, playerID);
    }
    public static string PlayerPublicOverview_NoTilda(int playerID, bool inPopup)
    {
        if (inPopup)
        {
            return PlayerPublicOverview_NoTildaPopup(playerID);
        }
        else
        {
            return PlayerPublicOverview_NoTilda(playerID);
        }
    }
    public static string PlayerPublicOverview_NoTilda(int playerID)
    {
        return String.Format(playerPublicOverview_NoTilda, playerID);
    }
    public static string PlayerPublicOverview_NoTildaPopup(int playerID)
    {
        return String.Format(playerPublicOverview_NoTildaPopup, playerID);
    }
    public static string PlayerPublicProfileByName(string PlayerName)
    {
        return String.Format(playerPublicOverviewByName, PlayerName);
    }
    public static string PlayerPublicProfileByNamePopup(string PlayerName)
    {
        return String.Format(playerPublicOverviewByNamePopup, PlayerName);
    }

    public static string MessageDetails(int recordID)
    {
        return String.Format(inboxMessageDetails, recordID);
    }


    public static string MessageList(bool showArchived)
    {
        return String.Format(messageList_archived, showArchived);
    }
    public static string MessageList(int folderID)
    {
        return String.Format(messageList, folderID);
    }
    public static string MessageList(int folderID, bool showArchived)
    {
        return String.Format(messageList_full, folderID, showArchived);
    }
    public static string MessageList()
    {
        return "~/messages.aspx";
    }
    public static string MessageMove(int recordID, int folderID)
    {
        return String.Format(messageMove, recordID, folderID);
    }
    public static string CreateMailTo(string PlayerName)
    {
        return String.Format(createMailTo, HttpUtility.UrlEncode(PlayerName));
    }


    public static string BuildingPageNoTilda(int buildingID, Fbg.Bll.Village curVillage)
    {
        return BuildingPage(buildingID, curVillage).Replace("~/", "");
    }
    public static string BuildingPageNoTilda(int buildingID)
    {
        return BuildingPage(buildingID).Replace("~/", "");
    }

    public static string BuildingPage(int buildingID)
    {
        return BuildingPage(buildingID, null);
    }
    public static string BuildingPage(int buildingID, Fbg.Bll.Village curVillage)
    {
        if (buildingID == Fbg.Bll.CONSTS.BuildingIDs.VillageHQ)
        {
            //
            // find out of perhaps we are to display the none-advanced mode
            // 
            if (curVillage != null) // this only applies for realm 6 and above
            {
                if (curVillage.owner.NumberOfVillages == 1)
                {
                    if (curVillage.GetBuildingLevel(Fbg.Bll.CONSTS.BuildingIDs.VillageHQ) < curVillage.owner.Realm.HQLevelNeededForUnlimitedQ)
                    {
                        if (curVillage.owner.HasFlag(Fbg.Bll.Player.Flags.Misc_AdvancedHQ) == null)
                        {
                            return String.Format(BuildingPage_Generic, buildingID);
                        }
                    }
                }
            }

            return BuildingPage_VHQ;
        }
        else if (buildingID == Fbg.Bll.CONSTS.BuildingIDs.TradePost)
        {
            return BuildingPage_TP;
        }
        else if (buildingID == Fbg.Bll.CONSTS.BuildingIDs.Barracks
            || buildingID == Fbg.Bll.CONSTS.BuildingIDs.Stable
            || buildingID == Fbg.Bll.CONSTS.BuildingIDs.SiegeWorkshop
            || buildingID == Fbg.Bll.CONSTS.BuildingIDs.Tavern)
        {
            return String.Format(BuildingPage_Troops, buildingID);
        }
        else if (buildingID == Fbg.Bll.CONSTS.BuildingIDs.Wall
           || buildingID == Fbg.Bll.CONSTS.BuildingIDs.DefenseTower)
        {
            return String.Format(BuildingPage_Defensive, buildingID);
        }
        else if (buildingID == Fbg.Bll.CONSTS.BuildingIDs.Palace)
        {
            return BuildingPage_Palace;
        }
        else
        {
            return String.Format(BuildingPage_Generic, buildingID);
        }

    }

    public static string CreateMailBlank()
    {
        return createMailBlank;
    }
    public static string CreateMailBlank(bool showMessageSentConfirmation)
    {
        return createMailBlank_ConfimMsgSent;
    }

    public static string CreateMailWithMsg(int recordID, Fbg.Bll.Mail.CONSTS.MailActionCreateType actionType)
    {
        return String.Format(createMailWithMsg, recordID, (int)actionType);
    }

    public static string UnitHelp(int realmID, int unitTypeID)
    {
        return String.Format(unitHelp, realmID, unitTypeID);
    }

    public static string BuildingsHelp(int realmID, int buildingTypeID)
    {
        return String.Format(buildingHelp, realmID, buildingTypeID);
    }

    public static string CommandTroops(string commandType, string XCord, string YCord, bool inPopup)
    {
        if (inPopup)
        {
            return CommandTroopsPopup(commandType, XCord, YCord);
        }
        else
        {
            return CommandTroops(commandType, XCord, YCord);
        }
    }
    public static string CommandTroops(string commandType, string XCord, string YCord)
    {
        return String.Format(commandTroops, XCord, YCord, commandType);
    }
    public static string CommandTroopsPopup(string commandType, string XCord, string YCord)
    {
        return String.Format(commandTroopsPopup, XCord, YCord, commandType);
    }

    public static string MapByCords(int XCord,int YCord)
    {
        return String.Format(mapbyCords, XCord, YCord);
    }
    public static string MapByCords(int SelectedVillageID,int XCord, int YCord)
    {
        return String.Format(mapbyCordsAndVillage,SelectedVillageID , XCord, YCord);
    }
    public static string Map2ByCords(int XCord, int YCord)
    {
        return String.Format(map2byCords, XCord, YCord);
    }
    public static string Map2ByCords(int SelectedVillageID, int XCord, int YCord)
    {
        return String.Format(map2byCordsAndVillage, SelectedVillageID, XCord, YCord);
    }
    public static string TransportCoinsByCords(int XCord, int YCord)
    {
        return String.Format(TransportCoinsbyCords , XCord, YCord);
    }
    public static string QuickSendCoins(int VillageID,int XCord, int YCord)
    {
        return String.Format(quickSendCoins, VillageID, XCord, YCord);
    }
    public static string Reports(int selectedVillageID)
    {
        return String.Format(reports, selectedVillageID);
    }
    public static string Reports_AttackDetails(int reportRecordID)
    {
        return String.Format(reports_attackDetails, reportRecordID);
    }
    public static string TransportCoinsWithVillage(int SelectedVillageID)
    {
        return String.Format(transportCoinsWithVillage, SelectedVillageID);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pfID">values from Fbg.Bll.CONSTS.PF</param>
    /// <returns></returns>
    public static string PFOffMessage(Fbg.Bll.CONSTS.PFs pfID)
    {
        return String.Format(PFOff, (int)pfID);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pfID">values from Fbg.Bll.CONSTS.PF</param>
    /// <returns></returns>
    public static string PFOffMessage_NoTilda(Fbg.Bll.CONSTS.PFs pfID)
    {
        return String.Format(PFOff, (int)pfID).Remove(0,2);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pfID">values from Fbg.Bll.CONSTS.PF</param>
    /// <returns></returns>
    public static string PFDesc(Fbg.Bll.CONSTS.PFs pfID)
    {
        return String.Format(pfDesc, (int)pfID);
    }

    /// <summary>
    /// Using this when ever displaying a title as a link
    /// </summary>
    /// <param name="TitleID">A valid Fbg.Bll.Title ID</param>
    /// <returns></returns>
    public static string Title(int titleID)
    {
        return String.Format(title, (int)titleID);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="villageID">pass -1 if you want to see all incoming</param>
    /// <returns></returns>
    public static string TroopsIncoming(int villageID)
    {
        if (villageID == -1)
        {
            return "~/TroopMovementsIn.aspx?vid=-1";
        }
        else
        {
            return String.Format("~/TroopMovementsIn.aspx?{0}={1}", CONSTS.QuerryString.SelectedVillageID, villageID);
        }
    }

#endregion

    public static string StewardAccesDenied()
    {
        return "AccountStewards_accessdenied.aspx";
    }
}
