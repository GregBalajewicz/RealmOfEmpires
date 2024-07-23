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
/// Summary description for CONSTS
/// </summary>
public static class CONSTS
{
    public class Session
    {
        public static string fbgUser = "fbgUser";
        public static string fbgPlayer = "fbgPlayer";
        public static string fbgAvatarID = "fbgAvatarID";
        public static string fbgAvatar = "fbgAvatar";

        /// <summary>
        /// this holds the list of invited facebook IDs 
        /// </summary>
        public static string InvitedFBIds = "ids[]";
        /// <summary>
        /// this holds the ID of the gift that was sent via facebook to friends. Related to InvitedFBIds
        /// </summary>
        public static string GiftID = "GID";
        public static string LoggedInAs = "ADMIN_ONLY_LOGEDIN_AS_FBID";
        public static string FacebookUserID = "UserId";
        public static string FacebookSessionKey = "SessionKey";
        public static string KongregateGameAuthToken = "KongregateGameAuthToken";
        public static string KongregateUserID = "KongregateUserID";
        public static string ArmoredGamgesUserID = "agui";
        
        /// <summary>
        /// signals that playing coming in is accepting a gift
        /// </summary>
        public static string AcceptingGift = "accptGift";
        /// <summary>
        /// nickname the person entered in the intro
        /// </summary>
        public static string NickNameFromIntro = "nick";
        /// <summary>
        /// nickname the person entered in the intro
        /// </summary>
        public static string VillageNameFromIntro = "villname";

        /// <summary>
        /// where we store TrackingInfo object
        /// </summary>
        public static string CommunicationChannelTrackingObject = "CHObj";
        public static string AnalyticsEventObj = "aeObj";
        /// <summary>
        /// used buy the facebook service to cache call to FB that returns logged in player's info
        /// </summary>
        public static string FacebookEntityUser = "FBUSERCACHE";
        /// <summary>
        /// time stamp the helps us not do too many calls to refresh friends
        /// </summary>
        public static string SessionCachePrefix = "SessionCachePrefix_";
        /// <summary>
        /// used when creating a tactica account from device login cause in this situation, cannot send email via url param
        /// </summary>
        public static string CreatedTacticaAccountEmail = "CreatedTacticaAccountEmail";
      
    }

    public class QuerryString
    {
        public static string XCord = "x";
        public static string YCord = "y";
        public static string RealmID = "rid";
        public static string VillageID = "vid";
        public static string SelectedVillageID = "svid";
        /// <summary>
        /// use this to pass village id to be temporarily selected/used for some operation but not permanement selected as "SelectedVillageID" would do 
        /// </summary>
        public static string TempSelVillageID = "tsvid";
        public static string QEntryID = "qeid";
        public static string ReportID = "repID";
        public static string ReportTypeID = "reptid";
        public static string RecordID = "recID";
        public static string SupportedVillageID = "SdVID";
        public static string SupportingVillageID = "SiVID";
        public static string EventID = "EID";
        public static string HelpPageType = "HPN";
        public static string HelpObjectID = "HOID";
        public static string ClanID = "clanid";
        public static string OtherVillageID = "ovid";
        public static string SenderID = "sid";
        public static string RecipientID = "recid";
        public static string MessageType = "msgType";
        public static string MailType = "MT";
        public static string MessageID = "MID";
        public static string RecipientName = "To";
        /// <summary>
        /// enum EventType holds possible values
        /// </summary>
        public static string EventType = "et";
        public static string ReturnUrl = "ru";
        public static string FolderType = "ft";
        /// <summary>
        ///  enum Fbg.Common.UnitCommand.CommandType holds possible values
        /// </summary>
        public static string CommandType = "cmd";
        public static string PlayerID = "pid";
        public static string PlayerName = "pName";
        public static string RoleID = "roleid";//this for clan roles
        public static string Action = "Action";//this for clan roles add =1 remove =0
        public static string Data = "data";
        public static string ShowDetails = "sd";
        public static string RecallType = "RT";//this for units Recall 0 = recall all 1=recall some
        /// <summary>
        /// enum UnitAbroadPageStatus in the end of this page  holds possible values
        /// </summary>
        public static string PageState = "PS"; //this for UnitAbroad Page to handle menu and links states 0 means (See all my troops supporting other villages);1 means (See this village's troops supporting other villages);2 means (Show me troops from my current (X village) village instead!);
        /// <summary>
        /// used to test if browser accepts cookies. 
        /// </summary>
        public static string AcceptsCookies = "AcceptsCookies";
        /// <summary>
        /// Valida values are Fbg.Bll.CONSTS.PF
        /// </summary>
        public static string PFID = "pfid";
        /// <summary>
        /// ID of a package; ie FBGCommon.CreditPackages.CreditPackageID
        /// </summary>
        public static string CreditPackageID = "cpi";
        /// <summary>
        /// ID of a player's title
        /// </summary>
        public static string TitleID = "tid";
        /// <summary>
        /// ID of a player's title
        /// </summary>
        public static string Type = "type";
        /// <summary>
        /// Invitation ID - ID on an invitation to the game
        /// </summary>
        public static string InviteID = "inviteID";
        /// <summary>
        /// View archived (messages or reports). 'false' or 'true' are valid values
        /// </summary>
        public static string ViewArchived = "va";
        /// <summary>
        /// (message or report)'s folder ID
        /// </summary>
        public static string FolderID = "fid";
        /// <summary>
        /// tells the page to move the (message or report)to this folder  ID
        /// </summary>
        public static string MoveToFolderID = "mtfid";
        /// <summary>
        /// used by reports. 
        /// </summary>
        public static string NoSearch = "nosearch";
        /// <summary>
        /// used in a number of places to represent page index of a pagable grid. 
        /// </summary>
        public static string PageIndex = "pageindex";
        /// <summary>
        /// values from StoriesToPublish enum
        /// </summary>
        public static string StoryToPublish = "stp";
        /// <summary>
        /// tells us this person/GET request is coming from clicking a link on a facebook story
        /// </summary>
        public static string FromFacebookStory = "ffs";
        /// <summary>
        /// tells us this person/GET request is coming from some communication channel. 
        /// </summary>
        public static string CommunicationChannelID = "ccid";
        /// <summary>
        /// realted to CommunicationChannelID. Available usually when CommunicationChannelID is present
        /// </summary>
        public static string CommunicationChannelMessage = "ccm";
        /// <summary>
        /// realted to CommunicationChannelID. Available usually when CommunicationChannelID is present
        /// </summary>
        public static string CommunicationChannelData = "ccd";
        /// <summary>
        /// generic url param used to demote 'error' id or string. 
        /// </summary>
        public static string Error = "e";
        /// <summary>
        /// Gift ID - id of a sendable gift
        /// </summary>
        public static string GiftID = "gid";
        /// <summary>
        /// tells the choose realm page to go to this url without forcing the user to login to facebook/authorize the app
        /// </summary>
        public static string GoToNow = "gtn";
        /// <summary>
        /// tells the system that we should send the NEW user immediatelly to create a player page. do not ask him to click enter
        /// </summary>
        public static string GoToCreatePlayerImmediatelly = "gtc";
        public const string FacebookSessionKey = "fb_sig_session_key";

        public static string FbCreditOrderInfo = "order_info";
        public static string FbCreditOrderDetails = "order_details";
        public static string FbCreditUser = "buyer";
        public static string FbCreditOrderId = "order_id";
        public static string FbCreditSignedRequest = "signed_request";
    }

    public class Cookies
    {
        public static string MapSize = "MaxSize";
        public static string ClanMembersPageSize = "ClanMembersPageSize";
        /// <summary>
        /// used to test if browser accepts cookies. 
        /// </summary>
        public static string AcceptsCookies = "AcceptsCookies";
        public static string MailPageSizeIndex = "MailPageSizeIndex";
        public static string PlayerID = "pid";
        public static string Quests = "quests";
        public static string Last10Commands = "L10C";
        public static string OutgoingTroopsFilter = "otf";
        public static string IncomingTroopsFilter = "itf";
        public static string VillageSelectionPageSize = "VSps";
        public static string SummaryPagesPageSize = "SPps";
        public static string Messages = "messages";
        public static string Reports = "reports";
        public static string ReportPageSizeIndex = "ReportPageSizeIndex";
        public static string IncomingTroopsGrid = "incoming";
        public static string OutgoingTroopsGrid = "outgoing";
        public static string Poll = "poll";
        public static string FacebookUserID = "UserId";
        public static string FacebookSessionKey = "SessionKey";
        public static string StewardLoggedInAsRecordID = "SLLARID";
        public static string VOVType = "vovt";
        /// <summary>
        /// signals that playing coming in is accepting a gift
        /// </summary>
        public static string AcceptingGift = "accptGift";
        public static string isD2 = "isD2";
        public static string isM = "isM";
        public static string LoginMethod = "loginMethod";
    }
    /// <summary>
    /// maximum size of a chat message
    /// </summary>
    public static int ChatMaxLength = 160;
    public const int MaxJsonLength = 10000000;
    /// <summary>
    /// This is used by the tutorial and pages that interact with the tutorial. 
    /// </summary>
    public static class TutorialScreenElements
    {
        public static string VillageOverviewLink = "vol";
        public static string BuildingList = "bBuildingsTable";
        public static string HQLink = "HQLink";
        public static string SilverMine = "SM";
        public static string BuildingUpgradeLink = "upgrade{0}";
        public static string UpgradingBox = "tblUpgrades";
        public static string sample = "sample";
        public static string VillageHUD = "VillageHUD";
        public static string VillageHUD_VilName = "VillageHUD_VilName";
        public static string VillageRaname = "VillageRaname";
        public static string VillageHUD_CurSilver = "VillageHUD_CurSilver";
        public static string VillageHUD_TreasurySize = "VillageHUD_TreasurySize";
        public static string VillageHUD_SilverProd = "VillageHUD_SilverProd";
        public static string VillageHUD_Food = "VillageHUD_Food";
        public static string TroopsList = "TroopsList";
        public static string MapLink = "MapLink";
        public static string PlayerOverviewLink = "PlayerOverviewLink";
        //public static string ClanIcon = "jClanLink";
    }

    public enum Device
    {
        Other = 0,
        Android = 1,
        iOS = 2,
        BB = 3,
        WP = 4,
        Amazon = 5
    }

    public static string ROEScript =
        @"ROE.playerID={0};
        ROE.isInPopup={1};
        ROE.playersNumVillages={2};
        ROE.CONST.specialPlayer_Rebel={3};
        ROE.CONST.specialPlayer_Abandoned={4};
        ROE.realmID={5};
        ROE.isMobile={6};
        ROE.isDevice={7};
        ROE.deviceUseOnly='{8}';
        ROE.bonusVillChange={9};
        ROE.Player.restartCost={10};
        ROE.Player.credits={11};
        ROE.Player.isSteward={12};";

    public static string ROEScript3 =
    @"
        ROE.playerID={0};
        ROE.isInPopup={1};
        ROE.playersNumVillages={2};
        ROE.CONST.specialPlayer_Rebel={3};
        ROE.CONST.specialPlayer_Abandoned={4};
        ROE.realmID={5};
        ROE.SVID={6};
        ROE.Player.name='{7}';
        ROE.Player.avatar='{8}';
        ROE.InDev={9};
        ROE.isMobile={10};
        ROE.isDevice={11};
        ROE.isVPRealm={12};
        ROE.loginMode='{13}';
        ROE.CONST.IsBattleHandicapActive={14};
        ROE.CONST.UnitDesertionScalingFactor={15}
        ROE.CONST.UnitDesertionMinDistance={16};
        ROE.CONST.UnitDesertionMaxPopulation={17};
        ROE.CONST.Handicap_MaxHandicap={18};
        ROE.CONST.Handicap_StartRatio={19};
        ROE.CONST.Handicap_Steepness={20};
        ROE.deviceUseOnly='{21}';
        {22}
        ROE.bonusVillChange={23};
        ROE.Player.restartCost={24};
        ROE.Player.credits={25};
        ROE.Player.isSteward={26};
        {27}
        ROE.collectAnalyticsOnThisRealm={28}; 
        ROE.localDBVersion={29};
        ROE.rt='{30}';
        ROE.CONST.GovUnitTypeID={31};
        ROE.CONST.ramUnitTypeID={32};
        ROE.CONST.trebUnitTypeID={33};
        ROE.CONST.spyUnitTypeID={34};
        ROE.isD2={35};
        ROE.userID='{36}';" /*ROE.Player.Clan.id=X | empty*/;



   
}


public enum StoriesToPublish
{
    TitleOfKnight = 0
    , JoinedClan = 1
    , CreatedClan = 2
    , TileBelowMerchant = 3
    , TitleAboveMerchant = 4
    , VIllageTakeover=5
    , JoinedRealm3 = 6
    , SpyCaptured_released = 7
    , SpyCaptured_executed = 8
    , SpyCaptured_tortured = 9
    , SpyCaptured_tortured2 = 10
    , TutorialComplete = 11
    , NewRealmJoin = 12
    , ReserchInitated = 13
}


public enum EventType
{
    BuildingUpgrade =0
    , UnitMovement =1
    , UnitRecruitment =2

}
