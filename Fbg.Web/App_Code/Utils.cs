using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Linq;
//using Amazon.SimpleEmail.Model;
//using Amazon.SimpleEmail;

using Gmbc.Common.Diagnostics.ExceptionManagement;



/// <summary>
/// Summary description for Utils
/// </summary>
public class Utils
{
    public delegate void LoginToFacebookDelegate();

    public static string DisplayTimeAsHoursAgo(DateTime time)
    {
        time = time.ToUniversalTime();
        TimeSpan ago = DateTime.UtcNow - time;

        string s;

        if (ago.TotalMinutes < 1)
        {
            s = ago.TotalSeconds.ToString("#") + "s ago";
        }
        else if (ago.TotalHours < 1)
        {
            s = ago.TotalMinutes.ToString("#") + "m ago";
        }
        else if (ago.TotalHours < 24)
        {
            s = ago.TotalHours.ToString("#.#") + "h ago";
        }
        else
        {
            s = ago.TotalDays.ToString("#.#") + "d ago";
        }
        return s;
    }

    public static string FormatEventTime(DateTime time)
    {
        time = time.ToUniversalTime();

        string s;

        if (time.Date == DateTime.Today)
        {
            s = "today at " + time.ToString("HH:mm:ss");
        }
        else
        {
            s = time.ToString("MMM dd HH:mm:ss");
        }
        return s;
    }
    public static string FormatEventTime_Short(DateTime time)
    {
        time = time.ToUniversalTime();

        string s;

        if (time.Date == DateTime.Today)
        {
            s = "today at " + time.ToString("HH:mm");
        }
        else
        {
            s = time.ToString("MMM dd yy HH:mm");
        }
        return s;
    }


    /// <summary>
    /// same as FormatEventTime except it never display 'today at' but alwyas the exact date
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string FormatEventTime_NoToday(DateTime time)
    {
        time = time.ToUniversalTime();
        return time.ToString("MMM dd yy HH:mm:ss");
    }


    public static string FormatDuration(TimeSpan time)
    {
        return Fbg.Bll.utils.FormatDuration(time);
    }
    public static string FormatDuration_Long(TimeSpan time)
    {
        return Fbg.Bll.utils.FormatDuration_Long(time);
    }
    public static string FormatDuration_Long2(TimeSpan time)
    {
        return Fbg.Bll.utils.FormatDuration_Long2(time);
    }
    public static string FormatDuration_Short(TimeSpan time)
    {
        return Fbg.Bll.utils.FormatDuration_Short(time);
    }

    public static string FormatCost(int cost)
    {
        return Fbg.Bll.utils.FormatCost(cost);
    }

    public static string FormatCost(long cost)
    {
        return Fbg.Bll.utils.FormatCost(cost);
    }

    public static string FormatCost(double cost)
    {
        return Fbg.Bll.utils.FormatCost(cost);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="btn">pass in a Button or LinkButton control</param>
    public static void PreventDoubleSubmit(Page page, WebControl btn)
    {
        PreventDoubleSubmit(page, btn, "Please wait...");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="btn">pass in a Button or LinkButton control</param>
    /// <param name="pleaseWaitMsg"></param>
    public static void PreventDoubleSubmit(Page page, System.Web.UI.WebControls.WebControl btn, string pleaseWaitMsg)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("if (typeof(Page_ClientValidate) == 'function') { ");
        sb.Append("if (Page_ClientValidate() == false) { return false; }} ");
        sb.AppendFormat("this.value = '{0}';", pleaseWaitMsg);
        sb.Append("this.disabled = true;");
        sb.Append(page.ClientScript.GetPostBackEventReference(btn, null));
        sb.Append(";");
        btn.Attributes.Add("onclick", sb.ToString());
    }
    /// <summary>
    /// Method to make sure that user's inputs are not malicious
    /// </summary>
    /// <param name="text">User's Input</param>
    /// <param name="maxLength">Maximum length of input</param>
    /// <returns>The cleaned up version of the input</returns>
    public static string CleanupInputText(string text/*, int maxLength*/)
    {
        text = text.Trim();
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        //if (text.Length > maxLength)
        //    text = text.Substring(0, maxLength);
//        text = Regex.Replace(text, "[\\s]{2,}", " "); //two or more spaces
        text = Regex.Replace(text, "(<[b|B][r|R]/*>)|(<[p|P](.|\\n)*?>)", "\n"); //<br/>
        text = Regex.Replace(text, "(<[b|B][r|R] /*>)|(<[p|P](.|\\n)*?>)", "\n"); //<br />
        text = Regex.Replace(text, "(<[b|B][r|R]>)|(<[p|P](.|\\n)*?>)", "\n"); //<br>
        text = Regex.Replace(text, "(\\s*&[n|N][b|B][s|S][p|P];\\s*)", " "); //&nbsp;
        text = Regex.Replace(text, "<(.|\\n)*?>", string.Empty); //any other tags
        text = Regex.Replace(text, "<", "&lt"); // remove '<'
        text = Regex.Replace(text, ">", "&gt"); // remove '>'
        //text = text.Replace("'", "''");
        return text;
    }

    /// <summary>
    /// Method to change line breaks into BR
    /// </summary>
    /// <param name="InputText">Takes user input</param>
    /// <returns>return formatted text</returns>
    public static string ChangeLineBreaks(string InputText)
    {
        //string formattedText = "";
        InputText = InputText.Replace("\r\n", "<br />");
        InputText = InputText.Replace("\n", "<br />");
        InputText = InputText.Replace("\r", "<br />");

        return InputText;
    }
    public static string ChangeTabBreak(string InputText)
    {   //comment this part if u don't want to handle line breaks
        InputText = InputText.Replace("\r\n", "<br/>");
        InputText = InputText.Replace("\n", "<br/>");
        InputText = InputText.Replace("\r", "<br/>");
        InputText = InputText.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
       
      

        return InputText;
    }
    
    /// <summary>
    /// replace all HTML tags with blank value
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ClearHTMLCode(string text/*, int maxLength*/)
    {
        text = text.Trim();
        if (string.IsNullOrEmpty(text))
            return "";
        text = Regex.Replace(text, "<(.|\\n)*?>", ""); //all tags
        return text;
    }

    /// <summary>
    /// removes some common 'illegal' characters; characters or string we know cause us trouble.
    /// removes '\', '<'
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ClearInvalidChars(string text)
    {
        text = text.Trim();
        if (string.IsNullOrEmpty(text))
            return "";
        text = Regex.Replace(text, "<", ""); // remove '<'
        text = text.Replace("\\", "");
        return text;
    }

    /// <summary>
    /// strips anything non ascii, pretty aggressive.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string StripNonAscii(string inputString)
    {
        string asAscii = Encoding.ASCII.GetString(
            Encoding.Convert(
                Encoding.UTF8,
                Encoding.GetEncoding(
                    Encoding.ASCII.EncodingName,
                    new EncoderReplacementFallback(string.Empty),
                    new DecoderExceptionFallback()
                    ),
                Encoding.UTF8.GetBytes(inputString)
            )
        );
        return asAscii;
    }



    private static string GetFullAddressTemplateMe = "<a href='{0}' onclick=\"return popupPlayerOverviewByID(this, {7}, true);\">{1}</a> <b>-</B> <a href='{2}' x='{4}' y='{5}' vid='{6}' opid='{7}' class='jsV'>{3}({4},{5})</a>";
    private static string GetFullAddressTemplateMe2 = "<a href='{0}'>{1}</a> <b>-</B> <a href='{2}' x='{4}' y='{5}' vid='{6}' opid='{7}' class='jsV' target='_parent'>{3}({4},{5})</a>";
    private static string GetFullAddressTemplate = "<a href='{0}' onclick=\"return popupPlayerOverviewByID(this, {7}, true);\">{1}</a> <b>-</B> <a href='{2}' x='{4}' y='{5}' vid='{6}' opid='{7}' class='jsV' click=\"return popupVilageOverview2(this, {6}, true);\">{3}({4},{5})</a>";
    private static string GetFullAddressTemplate2 = "<a href='{0}'>{1}</a> <b>-</B> <a href='{2}' x='{4}' y='{5}' vid='{6}' opid='{7}' class='jsV'>{3}    ({4},{5})</a>";
    private static string GetFullAddressTemplate3 = "<a href='{0}'>{1}</a> <BR>     <a href='{2}' x='{4}' y='{5}' vid='{6}' opid='{7}' class='jsV'>{3}<BR>({4},{5})</a>";
    private static string GetVillageOnlyAddressTemplate = "<a href='{0}' x='{2}' y='{3}' vid='{4}' opid='{5}' class='jsV' click=\"return popupVilageOverview2(this, {4}, true)\">{1}({2},{3})</a>";
    private static string GetVillageOnlyAddressTemplate2 = "<a  href='{0}' x='{2}' y='{3}' vid='{4}' opid='{5}' class='jsV' >{1}    ({2},{3})</a>";
    private static string GetVillageOnlyAddressTemplate3 = "<a  href='{0}' x='{2}' y='{3}' vid='{4}' opid='{5}' class='jsV' >{1}<BR>({2},{3})</a>";
    private static string GetVillageOnlyAddressTemplateMe = "<a href='{0}' x='{2}' y='{3}' vid='{4}' opid='{5}' class='jsV'>{1}({2},{3})</a>";
    private static string GetVillageOnlyAddressTemplateMe2 = "<a target='_parent' href='{0}' x='{2}' y='{3}' vid='{4}' opid='{5}' class='jsV' target='_parent' >{1}({2},{3})</a>";


    public static string GetAddress_PlayerAndVillage(string fullPlayerOverviewAddress
        , string playerName
        , string fullVillageOtherOverviewLink
        , string villageName
        , string villageXCord
        , string villageYCord
        , string villageID
        , string ownerPlayerID
        , bool isInPopup)
    {
        return GetAddress_PlayerAndVillage(
          fullPlayerOverviewAddress
        , playerName
        , fullVillageOtherOverviewLink
        , villageName
        , villageXCord
        , villageYCord
        , villageID
        , ownerPlayerID
        , isInPopup
        , false);
    }

    /// <summary>
    /// Get the Player name and village name address like this: 
    /// PlayerName - VillageName(X,Y) with both the player name and village name being links 
    /// to overview screens
    /// </summary>
    /// <param name="targetParent">pass in true if you want target=_parent on the hyperlink</param>
    /// <example>
    ///    Utils.GetAddress_PlayerAndVillage( 
    ///             control.ResolveUrl(NavigationHelper.PlayerPublicOverview(34))
    ///            , "ThePlayerName"
    ///            , control.ResolveUrl(NavigationHelper.VillageOtherOverview(5))
    ///            , "TheVillageName"
    ///            , "-5"
    ///            , "-1");
    /// </example>
    public static string GetAddress_PlayerAndVillage(string fullPlayerOverviewAddress
        , string playerName
        , string fullVillageOtherOverviewLink
        , string villageName
        , string villageXCord
        , string villageYCord
        , string villageID
        , string ownerPlayerID
        , bool isInPopup
        , bool multipleLineDisplay)
    {
        return String.Format(isInPopup ? (multipleLineDisplay ?  GetFullAddressTemplate3 :GetFullAddressTemplate2): GetFullAddressTemplate
            , fullPlayerOverviewAddress
            , playerName
            , fullVillageOtherOverviewLink
            , villageName
            , villageXCord
            , villageYCord
            , villageID
            , ownerPlayerID);
    }

    /// <summary>
    /// Get the Player name (ME!) and my village name address like this:
    /// PlayerName - VillageName(X,Y) with both the player name and village name being links 
    /// to overview screens
    /// </summary>
    /// <param name="targetParent">pass in true if you want target=_parent on the hyperlink</param>
    /// <example>   
    /// </example>
    public static string GetAddress_MeAndVillage(string fullPlayerOverviewAddress
        , string playerName
        , string fullVillageOverviewLink
        , string villageName
        , string villageXCord
        , string villageYCord
        , string villageID
        , string ownerPlayerID
        , bool isInPopup)
    {
        return String.Format(isInPopup ? GetFullAddressTemplateMe2 : GetFullAddressTemplateMe
            , fullPlayerOverviewAddress
            , playerName
            , fullVillageOverviewLink
            , villageName
            , villageXCord
            , villageYCord
            , villageID
            , ownerPlayerID);
    }
    /// <summary>
    /// Get the Player name and village name address like this:
    /// PlayerName - VillageName(X,Y) with both the player name and village name being links 
    /// to overview screens
    /// </summary>
    /// <example>
    ///    Utils.GetAddress_PlayerAndVillage( 
    ///             control.ResolveUrl(NavigationHelper.PlayerPublicOverview(34))
    ///            , "ThePlayerName"
    ///            , control.ResolveUrl(NavigationHelper.VillageOtherOverview(5))
    ///            , "TheVillageName"
    ///            , "-5"
    ///            , "-1");
    /// </example>
    //public static string GetAddress_PlayerAndVillage(string fullPlayerOverviewAddress
    //    , string playerName
    //    , string fullVillageOtherOverviewLink
    //    , string villageName
    //    , string villageXCord
    //    , string villageYCord
    //    , string villageID
    //    , string ownerPlayerID)
    //{
    //    return GetAddress_PlayerAndVillage(fullPlayerOverviewAddress
    //        , playerName
    //        , fullVillageOtherOverviewLink
    //        , villageName
    //        , villageXCord
    //        , villageYCord
    //        , villageID
    //        , ownerPlayerID
    //        , false);
    //}

    public static string GetAddress_VillageOnly(
       string fullVillageOtherOverviewLink
       , string villageName
       , string villageXCord
       , string villageYCord
       , string villageID
       , string ownerPlayerID
       , bool isInPopup
       )
    {
        return GetAddress_VillageOnly(fullVillageOtherOverviewLink
       , villageName
       , villageXCord
       , villageYCord
       , villageID
       , ownerPlayerID
       , isInPopup
       , false);
    }
    /// <summary>
    /// Get the village name address like this:
    /// VillageName(X,Y) with both the player name and village name being links 
    /// to overview screens.
    /// 
    /// To be used for OTHER players village only - not your village!
    /// </summary>
    /// <param name="targetParent">pass in true if you want target=_parent on the hyperlink</param>
    /// <example>
    ///    Utils.GetAddress_VillageOnly( 
    ///            control.ResolveUrl(NavigationHelper.VillageOtherOverview(5))
    ///            , "TheVillageName"
    ///            , "-5"
    ///            , "-1");
    /// </example>
    public static string GetAddress_VillageOnly(
        string fullVillageOtherOverviewLink
        , string villageName
        , string villageXCord
        , string villageYCord
        , string villageID
        , string ownerPlayerID
        , bool isInPopup
        , bool multipleLineDisplay)
    {
        return String.Format(isInPopup ? (multipleLineDisplay ? GetVillageOnlyAddressTemplate3 : GetVillageOnlyAddressTemplate2) : GetVillageOnlyAddressTemplate
                , fullVillageOtherOverviewLink
                , villageName
                , villageXCord
                , villageYCord, villageID, ownerPlayerID);
    }

    /// <summary>
    /// Get the village name address, (FOR MY VILLAGE!) like this:
    /// VillageName(X,Y) with both the player name and village name being links 
    /// to overview screens.
    /// </summary>
    public static string GetAddress_MyVillageOnly(
        string fullVillageOtherOverviewLink
        , string villageName
        , string villageXCord
        , string villageYCord
        , string villageID
        , string ownerPlayerID
        , bool isInPopup)
    {
        return String.Format(isInPopup ? GetVillageOnlyAddressTemplateMe2 : GetVillageOnlyAddressTemplateMe
                , fullVillageOtherOverviewLink
                , villageName
                , villageXCord
                , villageYCord, villageID, ownerPlayerID);
    }

    /// <summary>
    /// Get the Player name and village name address like this:
    /// PlayerName - VillageName(X,Y) with both the player name and village name being links 
    /// to overview screens
    /// </summary>
    /// <example>
    ///    Utils.GetAddress_VillageOnly( 
    ///            control.ResolveUrl(NavigationHelper.VillageOtherOverview(5))
    ///            , "TheVillageName"
    ///            , "-5"
    ///            , "-1");
    /// </example>
    //public static string GetAddress_VillageOnly( 
    //    string fullVillageOtherOverviewLink
    //    , string villageName
    //    , string villageXCord
    //    , string villageYCord
    //      , string villageID
    //    , string ownerPlayerID)
    //{
    //    return GetAddress_VillageOnly(fullVillageOtherOverviewLink, villageName, villageXCord, villageYCord, villageID, ownerPlayerID, false);
    //}

    /// <summary>
    /// Checks given String value is valid Integer number or not
    /// </summary>
    /// <param name="strNumber"></param>
    /// <returns>true If Valid Integer number, otherwise false</returns>
    public static bool IsInteger(String strNumber)
    {
        Regex NotIntValue = new Regex("[^0-9-]");
        Regex IntValue = new Regex("^-[0-9]+$|^[0-9]+$");
        return !NotIntValue.IsMatch(strNumber) &&
        IntValue.IsMatch(strNumber);
    }


    /// <summary>
    /// returned/in support of GetLoginLink method 
    /// </summary>
    public enum LoginLinkType
    {
        Register,
        Register_MaxPlayerReached,
        PreRegister,
        Login,
        NoEntry_RealmClosedToNewPlayers,
        NoEntry_NeededTitleNotReached,
        NoEntry_NotANewPlayer,
        NoEntry_NoNP,
        NoEntry_NotOnMobileDeviceForMobOnlyRealm,
        NoEntry_MaxXPExceeded,
        NoEntry_MoreThan14DaysToEntry,
    }

    public static HyperLink GetLoginLink(Fbg.Bll.User user, Fbg.Bll.Realm realm, out LoginLinkType loginLinkType)
    {
        return GetLoginLink(user, realm, out loginLinkType, false, false);
    }

    public static HyperLink GetLoginLink(Fbg.Bll.User user, Fbg.Bll.Realm realm, out LoginLinkType loginLinkType, bool isAdmin, bool isDevice)
    {
        return GetLoginLink(user, realm, out loginLinkType, isAdmin, isDevice, DateTime.MaxValue);
    }
    /// <summary>
    /// used by pages allowing log or registration at realms.
    /// this MAY return NULL if this player cannot access this realm
    /// </summary>
    /// <param name="realm"></param>
    /// <returns></returns>
    public static HyperLink GetLoginLink(Fbg.Bll.User user, Fbg.Bll.Realm realm, out LoginLinkType loginLinkType, bool isAdmin, bool isDevice, DateTime dateUserRegisteredOn)
    {
        HyperLink hyperlink=null;
        loginLinkType = LoginLinkType.NoEntry_RealmClosedToNewPlayers;
        Fbg.Bll.Player player = user.PlayerByRealmID(realm.ID);

        if (player == null)
        {
            #region player not yet playing on this realm
            //
            // player not yet playing on this realm
            //


            #region Max players not reached
            if (realm.IsOpen == Fbg.Bll.Realm.RealmState.Running_OpenToAll
                || realm.IsOpen == Fbg.Bll.Realm.RealmState.PreRegistration)
            {
                #region realm is not closed
                //
                // realm is not closed
                //
                if ((realm.ID == 64 || realm.ID == BetaD2.D1OnlyRealm) && !BetaD2.canEnterD1(isAdmin, dateUserRegisteredOn))
                {
                    //
                    // TEMP - prevent D2 only player 
                    //
                    hyperlink = null;
                    loginLinkType = LoginLinkType.NoEntry_MaxXPExceeded;
                }
                else if (realm.RealmTitleEntryLimitations == 0
                        && !realm.RealmEntryLimitations_AllowEntryForNewPlayer(user))
                {
                    // 
                    // this realm if opened for new players only and this player is not a new player.
                    //
                    hyperlink = null;
                    loginLinkType = LoginLinkType.NoEntry_NotANewPlayer;
                }
                else if (realm.RealmEntryLimitations_MaxXP > 0 && user.XP > realm.RealmEntryLimitations_MaxXP)
                {
                    // 
                    // this realm is opened for players with some max xp and this user has a higher XP
                    //
                    hyperlink = null;
                    loginLinkType = LoginLinkType.NoEntry_MaxXPExceeded;
                }
                else if (!realm.RealmEntryLimitations_AllowEntryForNewPlayer(user))
                {
                    //
                    // this realm if opened for player who ahieved a certain rank and this user has not.
                    //
                    hyperlink = new HyperLink();
                    hyperlink.Text = Resources.global.Locked;
                    hyperlink.NavigateUrl = String.Format("~/ExclusiveRealmMessage.aspx?{0}={1}", CONSTS.QuerryString.RealmID, realm.ID);
                    loginLinkType = LoginLinkType.NoEntry_NeededTitleNotReached;
                }
                else if (realm.NeedNPInSomeOtherRealmToEnterThisOne && !user.HasNPInSomeRealm)
                {
                    //
                    // this realm if opened for player who ahieved a certain rank and this user has not.
                    //
                    hyperlink = new HyperLink();
                    hyperlink.Text = Resources.global.Locked;
                    hyperlink.NavigateUrl = "~/ExclusiveRealmMessage_NPOnly.aspx";
                    loginLinkType = LoginLinkType.NoEntry_NoNP;
                }
                else if (realm.OpenOn > DateTime.Now.AddDays(14))
                {
                    //
                    // this realm is more than 14 days to opening, so do not show it
                    //
                    hyperlink = null;
                    loginLinkType = LoginLinkType.NoEntry_MoreThan14DaysToEntry;
                }
                else
                {
                    //
                    // Realm entry limitations pass, now lets check more params
                    //
                    if (realm.AccessDeviceTypeLimitation == Fbg.Bll.Realm.AccessDeviceTypeLimitations.MobDevicesOnly && !isDevice && !isAdmin)
                    {
                        //
                        // realm for mobile devices only and this is not a device
                        //
                        hyperlink = new HyperLink();
                        hyperlink.Text = Resources.global.Locked;
                        hyperlink.NavigateUrl = "~/MobDevRealmMessage.aspx";
                        loginLinkType = LoginLinkType.NoEntry_NotOnMobileDeviceForMobOnlyRealm;
                    }
                    else if (realm.AccessDeviceTypeLimitation == Fbg.Bll.Realm.AccessDeviceTypeLimitations.OpenToAll_ButRegisterViaDesktopOnly && isDevice)
                    {
                        //
                        // realm for registrastion via desktop only! 
                        //
                        hyperlink = null;
                    }
                    else if (realm.IsOpen == Fbg.Bll.Realm.RealmState.Running_OpenToAll)
                    {
                        hyperlink = new HyperLink();
                        hyperlink.Text = Resources.global.Register;
                        hyperlink.NavigateUrl = "~/create.aspx?rid=" + realm.ID.ToString();
                        loginLinkType = LoginLinkType.Register;
                    }
                    else if (realm.IsOpen == Fbg.Bll.Realm.RealmState.PreRegistration)
                    {
                        hyperlink = new HyperLink();
                        hyperlink.Text = Resources.global.PreRegister;
                        hyperlink.NavigateUrl = "~/create.aspx?rid=" + realm.ID.ToString();
                        loginLinkType = LoginLinkType.PreRegister;
                    }
                }
                #endregion
            }
            else
            {
                // realm is close to new players so don't allow a new player to enter
                hyperlink = null;
                loginLinkType = LoginLinkType.NoEntry_RealmClosedToNewPlayers;
            }
            #endregion


            #endregion
        }
        else
        {
            #region player already plays on this realm
            if (realm.AccessDeviceTypeLimitation == Fbg.Bll.Realm.AccessDeviceTypeLimitations.MobDevicesOnly && !isDevice && !isAdmin)
            {
                //
                // realm for mobile devices only and this is not a device
                //
                hyperlink = new HyperLink();
                hyperlink.Text = Resources.global.Locked;
                hyperlink.NavigateUrl = "~/MobDevRealmMessage.aspx";
                loginLinkType = LoginLinkType.NoEntry_NotOnMobileDeviceForMobOnlyRealm;
            }
            else
            {
                //
                // we display a login link even if realm is not yet opened. 
                hyperlink = new HyperLink();
                hyperlink.Text = Resources.global.Login;
                hyperlink.NavigateUrl = "~/LoginToRealm.aspx?rid=" + realm.ID.ToString() + "&pid=" + player.ID;
                loginLinkType = LoginLinkType.Login;
            }
            #endregion 
        }

        return hyperlink;

    }

    /// <summary>
    /// Used by code that displays the list of realms
    /// </summary>
    /// <param name="realm"></param>
    /// <returns></returns>
    public static string GetRealmDetailsText(Fbg.Bll.Realm realm, LoginLinkType? loginLinkType)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
        sb.Append("<B>" + Utils.FormatCost(realm.Population) + " active players.</b>");
        if (realm.IsOpen != Fbg.Bll.Realm.RealmState.PreRegistration)
        {
            TimeSpan ts = DateTime.Now.Subtract(realm.OpenOn);
            sb.Append(" Realm running for <B>");
            sb.Append(ts.Days == 0 ? ts.TotalDays.ToString("#.##") : ts.Days.ToString());
            sb.Append (" days</B>.");
        }
        else
        {
            TimeSpan ts = realm.OpenOn.Subtract(DateTime.Now);
            sb.Append("<BR><B>Opens On</b>: ");
            sb.Append(realm.OpenOn.ToUniversalTime().ToString("MMM dd, HH:mm"));
            sb.Append(" <a style='font-size:8px' href='http://www.timeanddate.com/worldclock/converter.html' target='_blank'>UTC</a>");
            sb.Append("<BR><B>Opens In</b>:<span class='Countdown' redir='https://realmofempires.com/ChooseRealm2.aspx'>" + Utils.FormatDuration(realm.OpenOn.Subtract(DateTime.Now)) + "</span>");
        }

        if (loginLinkType != null && loginLinkType == LoginLinkType.NoEntry_NotOnMobileDeviceForMobOnlyRealm)
        {
            sb.Append("<BR>This Realm is accessible via ROE Mobile App only");            
        }
        sb.Append("<BR>");    


        return sb.ToString();
    }


    public static Fbg.Common.Sex ConvertGender(Facebook.Entity.Gender gender)
    {
        switch (gender)
        {
            case Facebook.Entity.Gender.Female:
                return Fbg.Common.Sex.Female;
            case Facebook.Entity.Gender.Male:
                return Fbg.Common.Sex.Male;
            default:
                return Fbg.Common.Sex.Unknown;
        }
    }
    /// <summary>
    /// this function add arrow images to header row depend on the direction 
    /// </summary>
    /// <param name="headerRow"></param>
    /// <param name="ColID">the postion of the coulumn that want to add arrow image to</param>
    /// <param name="ApplyDirection">if u want to apply the direction or leave it as default for this column</param>
    /// <param name="Direction">the direction for this column</param>
    public static void AddSortImage(ref GridViewRow headerRow, int colID, bool applyDirection,SortDirection direction)
    {
        Image sortImage = new Image();
        if (applyDirection)
        {
            if (SortDirection.Ascending == direction)
            {
                sortImage.ImageUrl = "https://static.realmofempires.com/images/UpArrow.png";
                sortImage.AlternateText = "Descending Order";
            }
            else
            {
                sortImage.ImageUrl = "https://static.realmofempires.com/images/DownArrow.png";
                sortImage.AlternateText = "Ascending Order";
                
            }
        }
        else
        {
            sortImage.ImageUrl = "https://static.realmofempires.com/images/DownArrow.png";
            sortImage.AlternateText = "Ascending Order";
        }

        // Add the image to the appropriate header cell.
        headerRow.Cells[colID].Controls.Add(sortImage);
    }


    const string templateDataPairTemplate = "\"{0}\":\"{1}\"";
    public static void RecordPublishedStoryOrAttempt(int playerID, Fbg.Bll.CONSTS.Stories story, Dictionary<String, String> data, bool isSuccess)
    {
        string value;
        StringBuilder sbData = new StringBuilder(data.Keys.Count * 20);
        sbData.Append("{");
        foreach (string key in data.Keys)
        {
            value = data[key];
            sbData.AppendFormat(templateDataPairTemplate, key, value);
            sbData.Append(",");
        }
        sbData.Append("}");
        sbData.Remove(sbData.Length - 2, 1);

        Fbg.Bll.utils.Stories_StoryPublished(playerID, (int)story, sbData.ToString(), isSuccess);
    }

    public static string CommandTypeImage(Fbg.Common.UnitCommand.CommandType commandType)
    {
        switch (commandType)
        {
            case Fbg.Common.UnitCommand.CommandType.Attack:
                return "https://static.realmofempires.com/images/attack.png";
            case Fbg.Common.UnitCommand.CommandType.Support:
                return "https://static.realmofempires.com/images/support.png";
            case Fbg.Common.UnitCommand.CommandType.Return:
            case Fbg.Common.UnitCommand.CommandType.Recall:
                return "https://static.realmofempires.com/images/returning.png";
            default:
                throw new ArgumentException("unrecognized value of commandType=" + commandType.ToString());
        }
    }
    private static string[] bannedWords = new string[] { "shit"};

    private static string[] bannedWords2 = new string[] {"anal"};

    private static string[] bannedNames = new string[] { "nameless", "roe_team"
        , "annonymous", "anonymous", "rebel", "rebels", "abandoned"
    };


    public static bool HaveBannedWord(string input)
    {
        foreach (string word in bannedWords)
        {
            if (Regex.IsMatch(input.ToLower(), word.ToLower()))
            {
                return true;
            }
        }
        if (bannedWords2.FirstOrDefault(s => s == input.ToLower()) != default(String))
        {
            return true;
        }
        return false;
    }

    public static bool IsBannedName(string name)
    {
        if (HaveBannedWord(name)){
            return true;
        }
        foreach (string word in bannedNames)
        {
            if (Regex.IsMatch(name.ToLower(), word.ToLower()))
            {
                return true;
            }
        }
        return false;
    }

    public static string ReplaceBannedWords(string input)
    {
        return Regex.Replace(input, "(" + string.Join("|", bannedWords) + ")|\\b(" + string.Join("|", bannedWords2) + ")\\b", delegate(Match m) { return new string('*', m.Length); }, RegexOptions.IgnoreCase);
    }


    public static void SendInvitedToClanNotification(Fbg.Bll.PlayerOther invitedPlayer
        , Fbg.Bll.Player invitingPlayer)
    {
        //
        // this is a crude way of ensureing this code only runs on production since we dont want 
        //  any emails going out from dev machines
        //
        if (!String.IsNullOrEmpty(FacebookConfig.DisconnectedFromFacebookUserID))
        {
            return;
        }

        //
        // this is a crude way of ensureing this code only runs on production since we dont want 
        //  any emails going out of staging or some dev server
        //
        if (!String.IsNullOrEmpty(BuildNumConfig.BuildNumber))
        {
            return;
        }

        //
        // send a notification to invited player under certain conditions
        //
        try
        {
            if (invitingPlayer.Clan != null)// sanity check. could would fail otherwise
            {
                MembershipUser user = Membership.GetUser(invitedPlayer.UserID);
                //
                // Send email 
                //
                if (invitedPlayer.Activity == Fbg.Bll.PlayerOther.ActivityLevel.VeryLow)
                {

                    if (user.Email != Fbg.Bll.CONSTS.DummyEmail)
                    {
                        Fbg.Bll.Mailer mailer = new Fbg.Bll.Mailer(Config.awsAccessKey, Config.awsSecretKey);
                        mailer.SendClanNotificationEmail(user.Email, invitingPlayer, invitedPlayer);
                        //Fbg.Bll.Notifier.SendClanNotification(invitingPlayer, invitedPlayer);
                    }
                }
                //
                // send to device notifcation Q
                //
                invitingPlayer.AddNotificationToQ(Fbg.Bll.Player.NotificationTypes.ClanInvite, String.Format(Resources.global.ClanInviteNotification
                    , invitingPlayer.Title.Level < 2 ? "" : invitingPlayer.TitleName
                    , invitingPlayer.Name
                    , invitingPlayer.Clan.Name));
            }
        }
        catch (Facebook.Exceptions.FacebookSessionExpiredException) { }
        catch (Exception ex)
        {
            System.Collections.Specialized.NameValueCollection col = new System.Collections.Specialized.NameValueCollection();
            BaseApplicationException.AddAdditionalInformation(col, "invitingPlayer.Clan.Name", invitingPlayer.Clan.Name);
            invitingPlayer.SerializeToNameValueCollection(col, "invitingPlayer");

            Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(new Exception("error while trying to send a 'you are invited to clan' email", ex), col);
            //
            // we eat the exception on purpose. do not want failure in case of problem here. 
        }
    }

    private readonly static string AddParamToUrl_WITHPARAMFORMAT = "{0}&{1}={2}";
    private readonly static string AddParamToUrl_NOPARAMFORMAT = "{0}?{1}={2}";
    /// <summary>
    /// add the specified paramater to any url - namly is distingusihsed between a url with params alreayd and one without
    /// </summary>
    /// <param name="url"></param>
    /// <param name="paramName"></param>
    /// <param name="paramValue"></param>
    /// <returns></returns>
    public static string AddParamToUrl(string url, string paramName, string paramValue)
    {
        if (url.Contains("?"))
        {
            // url already has params
            return String.Format(AddParamToUrl_WITHPARAMFORMAT, url, paramName, paramValue);
        }
        else
        {
            // url has no params
            return String.Format(AddParamToUrl_NOPARAMFORMAT, url, paramName, paramValue);
        }
        
    }

    /// <summary>
    /// String the url of the specified param
    /// </summary>
    /// <param name="gotoUrl"></param>
    /// <param name="p"></param>
    public static void RemoveParamFromUrl(ref string url, string paramName)
    {        
        int startLocOfParam=-1;
        startLocOfParam = url.IndexOf((paramName + "="));
        if (startLocOfParam != -1)
        {
            int endLoc;
            endLoc = url.IndexOf("&", startLocOfParam + 1);
            if (endLoc == -1)
            {
                endLoc = url.Length;
            }
            url = url.Remove(startLocOfParam-1, endLoc - startLocOfParam+1);

            RemoveParamFromUrl(ref url, paramName);
        }
        
    }


    public enum DisplayNoteType
    {
        Village,
        Player
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="isInPopup"></param>
    /// <param name="note"></param>
    /// <param name="villageID"></param>
    /// <param name="length"></param>
    /// <param name="noNoteMsg"></param>
    /// <returns></returns>
    public static string DisplayNote(DisplayNoteType type, bool isInPopup, string note, int villageOrPlayerID, int length, string noNoteMsg)
    {
        if (!String.IsNullOrEmpty(note))
        {
            return String.Format("<a class=pnote  href={0}>{1}</a>"
                , DislayNoteGetLink(type, isInPopup, villageOrPlayerID)
                , note.Substring(0, note.Length < length ? note.Length : length));
        }
        else
        {
            if (!String.IsNullOrEmpty(noNoteMsg))
            {
                return String.Format("<a class='pnonote'   href={0}>{1}</a>"
                    , DislayNoteGetLink(type, isInPopup, villageOrPlayerID)
                    , noNoteMsg);
            }
        }
        return "";
    }

    private static string DislayNoteGetLink(DisplayNoteType type, bool isInPopup, int villageOrPlayerID)  
    {
        if (type == DisplayNoteType.Village)
        {
            //village note
            return NavigationHelper.VillageOtherOverview_NoTilda(villageOrPlayerID, isInPopup);
        }
        else
        {
            //p[ayer note 
            return NavigationHelper.PlayerPublicOverview_NoTilda(villageOrPlayerID, isInPopup);
        }
    }

    public static string GetIframePopupHeaderForNotPopupBrowser(string title, bool writeIfTrue)
    {
        return GetIframePopupHeaderForNotPopupBrowser(title, writeIfTrue, false, false, "https://static.realmofempires.com/images/icons/m_ranking.png");
    }

    public static string GetIframePopupHeaderForNotPopupBrowser(string title, bool writeIfTrue, string iconUrl)
    {
        return GetIframePopupHeaderForNotPopupBrowser(title, writeIfTrue, false, false, iconUrl);
    }
    public static string GetIframePopupHeaderForNotPopupBrowser(string title, bool writeIfTrue, bool refreshHeader, bool refreshVOV)
    {
        return GetIframePopupHeaderForNotPopupBrowser(title, writeIfTrue, false, false, "https://static.realmofempires.com/images/icons/M_Stable.png");
    }
    
    public static string GetIframePopupHeaderForNotPopupBrowser(string title, bool writeIfTrue, bool refreshHeader, bool refreshVOV, string iconUrl)
    {
        /*
        return writeIfTrue ? @"<div class='IFrameDivTitle' style='width: 100%; height: 44px; text-align: left; background-color: rgba(41, 33, 22, 0.75);'>"
                    + "<span class='title' style='color: #C39037; float: left; font-size: 14px; font-weight: bold; padding: 15px 2px 0px 2px;'>" + title + "</span>"
                    + "<img style='float: right; width: 44px; height: 44px;' id='imgIframeClose' onclick='"
                    + (refreshHeader ? "window.opener.ROE.Frame.reloadFrame();" : "")
                    + (refreshVOV ? "window.opener.ROE.Frame.reloadView();" : "")
                    + "window.close();' src='https://static.realmofempires.com/images/LargeCancel.png' />"
                + "</div>"
                : " ";
        */

        return (writeIfTrue
            ? ""
             + "<img id='background' src='https://static.realmofempires.com/images/misc/SplashScreenMuted.jpg' class='stretch' style='position:fixed !important' >"    
               + "<div class=\"IFrameDivTitle\" style=\"position: relative; width: auto; height: 50px;\">"                
                    + "<section class=\"themeM-panel header clearfix\">"
                        + "<div class=\"bg\">"
                            + "<div class=\"corner-tl\"></div>"
                            + "<div class=\"corner-br\"></div>"
                            + "<div class=\"stripe\"></div>"
                        + "</div>"
                        + "<div class=\"fg\">"
                            + "<div class=\"themeM-icon scale-large\">"
                                + "<div class=\"bg\"></div>"
                                + "<div class=\"fg\">"
                                    + "<img src=\"" + iconUrl + "\" alt=\"\" /><br />"
                                + "</div>"
                            + "</div>"
                            + "<div class=\"label\">"
                                + "<span>" + title + "</span><br />"
                            + "</div>"
                            + "<div class=\"level\">"
                                + "<span></span><br />"
                            + "</div>"
                            + "<div class=\"action close\" onclick=\"ROE.UI.Sounds.click();"
                                + (refreshHeader ? "window.opener.ROE.Frame.reloadFrame();" : "")
                                + (refreshVOV ? "window.opener.ROE.Frame.reloadView();" : "")
                                + "window.close();\"></div>"
                        + "</div>"
                    + "</section>"
                + "</div>"
            : "");
    }

    /// <summary>
    /// any numbers 1000 or greater formatted as k, m, b, etc to maximum 999.9q(uadrillion) e.g. '12345' = '12.3k'
    /// </summary>
    /// <param name="num">the number</param>
    /// <returns>formatted string</returns>
    public static string FormatShortNum(long num)
    {
        string shortNum = num.ToString();
        try
        {
            if (num >= 1000)
            {
                int exp = Convert.ToInt32(Math.Floor(Math.Log(num) / Math.Log(1000)));
                string symbols = "kmbtq";
                shortNum = (Math.Round(num / Math.Pow(1000, exp), 1)).ToString() + symbols[exp - 1];
            }
        }
        catch
        {
        }
        return shortNum;
    }
    private string FormatCost(long num, bool isMobile)
    {
        if (isMobile) {
            return Utils.FormatShortNum(num);
        }
        else {
            return Utils.FormatCost(num);
        }
    }


    public static int UserLevel(System.Web.HttpContext context)
    {
        int userLevel = 0;
        if (context.User.IsInRole("tester"))
        {
            userLevel = 1;
        }
        if (context.User.IsInRole("Admin"))
        {
            userLevel = 2;
        }
        return userLevel;
    }
           

    public static string Analytics_CCTracking(System.Web.SessionState.HttpSessionState session)
    {
        string s = string.Empty;
        
        return s;
    }


    /// <summary>
    /// may return null if no events pending
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    private static List<AnalyticsEvent> Analytics_PendingEvent_get(System.Web.SessionState.HttpSessionState session, int realmID)
    {
        try
        {
            if (session[CONSTS.Session.AnalyticsEventObj] != null) 
            {
                List<AnalyticsEvent> allEvents;
                allEvents = (List<AnalyticsEvent>)session[CONSTS.Session.AnalyticsEventObj];
                //
                // get all events for this realm
                List<AnalyticsEvent> realmEvents = new List<AnalyticsEvent>(allEvents.Where(ae2 => ae2.RealmID == realmID || ae2.RealmID == -1));
                //
                // if we got at least one, remove them from the session object
                if (realmEvents.Count > 0)
                {
                    allEvents.RemoveAll(ae2 => ae2.RealmID == realmID || ae2.RealmID == -1);

                    //
                    // if some events left, store them in session, or remove the object all together
                    if (allEvents.Count > 0)
                    {
                        session[CONSTS.Session.AnalyticsEventObj] = allEvents;
                    }
                    else
                    {
                        session.Remove(CONSTS.Session.AnalyticsEventObj);
                    }

                    return realmEvents;
                }
                else
                {
                    // no events for this realm
                    return null;
                }
            }
        }
        catch (System.Threading.ThreadAbortException) { }
        catch (Exception ex)
        {
            //
            // this is not an essential functionlity so we eat the exception
            ExceptionManager.Publish(new Exception("UNREPORTED EXCEPTION: failure in Analytics_PendingEvent_get", ex));
        }

        return null;
    }
    /// <summary>
    /// may return string.empty
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public static string Analytics_PendingEvent_getScript(System.Web.SessionState.HttpSessionState session, int realmID)
    {
        try
        {
            object o = Analytics_PendingEvent_get(session, realmID);
            if (o != null)
            {
                List<AnalyticsEvent> events = (List<AnalyticsEvent>)o;
                StringBuilder s = new StringBuilder(events.Count * 50);
                StringBuilder attribs;
                foreach (AnalyticsEvent ae in events)
                {
                    //s.AppendFormat("mixpanel.track('{0}', {{'CCID': '{0}', 'CCM': '{1}', 'CCD': '{2}'}});", ae.EventName);
                    attribs = Analytics_PendingEvent_getScript_Attribs(ae.attribs);
                    s.AppendFormat("mixpanel.track('{0}'{1});", ae.EventName, attribs);
                }

                return s.ToString();
            }
        }
        catch (System.Threading.ThreadAbortException) { }
        catch (Exception ex)
        {
            //
            // this is not an essential functionlity so we eat the exception
            ExceptionManager.Publish(new Exception("UNREPORTED EXCEPTION: failure in Analytics_PendingEvent_getScript", ex));
        }
        return string.Empty;
    }

    private static StringBuilder Analytics_PendingEvent_getScript_Attribs(List<AnalyticsEvent.Attrib> attribs)
    {
        StringBuilder s = new StringBuilder();
        if (attribs.Count > 0)
        {
            
            StringBuilder stemp = new StringBuilder();      
            foreach (AnalyticsEvent.Attrib a in attribs)
            {
                if (stemp.Length > 0)
                {
                    stemp.Append(",");
                }
                stemp.AppendFormat("'{0}': '{1}'", a.name, a.value);
            }

            s.AppendFormat(", {{{0}}}", stemp);
        }
        return s;
    }



    public static void Analytics_SendEvent(Fbg.Bll.User user, string eventName, Dictionary<string, object> properties)
    {
        //MixpanelTracker mixpanelTracker = new MixpanelTracker(Config.CollectAnalytics_MixPanelID);

        ////var trackerProps = new Dictionary<string, object>();
        //// The value of distinct_id will be treated as a string, and used to 
        //// uniquely identify a user associated with your event. If you provide
        //// a distinct_id property with your events, you can track a given user
        //// through funnels and distinguish unique users for retention analyses. 
        //// You should always send the same distinct_id when an event is triggered by the same user.
        //properties.Add("distinct_id", user.ID.ToString());

        //mixpanelTracker.Track(eventName, properties);
    }

   


    /// <summary>
    /// tells if you the browser is such that we do not do iframe popups. like android
    /// </summary>
    public static bool IsiFramePopupsBrowser(HttpRequest request)
    {
        ////
        //// android cannot use iFrames. 
        //// Kindle web apps can - see https://developer.amazon.com/sdk/webapps/faq.html
        //if (request.UserAgent.Contains("Android") && !request.UserAgent.Contains("AmazonWebAppPlatform"))
        //{
        //    return false;
        //}
        return true;

    }


    ///// <summary>
    ///// Send realm email via amazon SES 
    ///// DEPRECIATED! DO NOT USE, use Fbg.Bll.Mailer.SendEmailViaAmazon instead
    ///// </summary>
    ///// <param name="toEmail"></param>
    ///// <param name="replyToAddress"> pass null or empty if you do not want to include it</param>
    ///// <param name="subject"></param>
    ///// <param name="body"></param>
    //public static void SendRealEmail(string toEmail, string replyToAddress, string subject, string body)
    //{
    //    //
    //    //
    //    // TODO 
    //    //
    //    // rewrite this to use Fbg.Bll.Mailer.SendEmailViaAmazon
    //    //
    //    //
    //    String source = "Realm of Empires < playeralerts@realmofempires.com >";      
    //    //String recipient = toEmail;
    //    //var oDestination = new Destination().(new List<string>() { recipient });


    //    //// Create the email subject.
    //    //var oSubject = new Amazon.SimpleEmail.Model.Content().WithData(subject.Trim());

    //    //string messageHtml = body;
    //    //// Create the email body.
    //    //var oTextBody = new Amazon.SimpleEmail.Model.Content().WithData(messageHtml);
    //    //var oBody = new Body().WithHtml(oTextBody);

    //    // Create and transmit the email to the recipients via Amazon SES.
        
    //    //var request = new SendEmailRequest().WithSource(source).WithDestination(oDestination).WithMessage(oMessage);
       
    //    try
    //    {
    //        using (var client = new AmazonSimpleEmailServiceClient(Config.awsAccessKey, Config.awsSecretKey))
    //        {

    //            var sendRequest = new SendEmailRequest
    //            {
    //                Source = source,
    //                Destination = new Destination
    //                {
    //                    ToAddresses =
    //                                       new List<string> { toEmail }
    //                },
    //                Message = new Message
    //                {
    //                    Subject = new Amazon.SimpleEmail.Model.Content(subject),
    //                    Body = new Body
    //                    {
    //                        Html = new Amazon.SimpleEmail.Model.Content
    //                        {
    //                            Charset = "UTF-8",
    //                            Data = body
    //                        }
    //                    }
    //                }

    //            };

    //            if (!string.IsNullOrEmpty(replyToAddress))
    //            {
    //                sendRequest.ReplyToAddresses = new List<string> { replyToAddress };
    //            }


    //            client.SendEmail(sendRequest);




    //        }
    //    }

    //    catch (Exception e)
    //    {
    //        if (Fbg.Bll.utils.isBadEmailException(e))
    //        {
    //            // this is old code, this exception is no longer fired
    //            Fbg.DAL.utils.SetInvalidAddress(toEmail, null, null);
    //        }
    //        else
    //        {
    //            Gmbc.Common.Diagnostics.ExceptionManagement.BaseApplicationException bex = new Gmbc.Common.Diagnostics.ExceptionManagement.BaseApplicationException("Error in SendRealEmail", e);
    //            bex.AddAdditionalInformation("toEmail", toEmail);
    //            bex.AddAdditionalInformation("replyToAddress", replyToAddress);
    //            bex.AddAdditionalInformation("subject", subject);
    //            bex.AddAdditionalInformation("body", body);
    //            throw bex;
    //        }
    //    }


    //}

    public static DateTime ConvertKnownJSDateStringFromAPI(string date)
    {
        return DateTime.ParseExact(date, "M/d/yyyy H:m:s:fff", System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// get the login type of the logged in player - it is just a helper function for Fbg.Common.Utils.GetLoginType(u.UserName, u.Email)
    /// </summary>
    public static Fbg.Common.Utils.LoginType GetLoginType(MembershipUser u) 
    {
        return Fbg.Common.Utils.GetLoginType(u.UserName, u.Email);
    }


    public static CONSTS.Device ToDevice(string userAgent)
    {
        CONSTS.Device device = CONSTS.Device.Other;
        try
        {
            if (Config.OverrideDeviceType != null)
            {
                //
                // for development purposes
                //
                device = (CONSTS.Device)Config.OverrideDeviceType;
            }
            else if (userAgent.Contains("ROEDroid1"))
            {
                device = CONSTS.Device.Android;
            }
            else if (userAgent.Contains("ROEIOS1"))//(userAgent.Contains("iPhone") || userAgent.Contains("iPad") || userAgent.Contains("iPod"))
            {
                device = CONSTS.Device.iOS;
            }
            //else if (userAgent.Contains("Kindle"))
            else if (userAgent.Contains("Android") && userAgent.Contains("AmazonWebAppPlatform"))
            {
                device = CONSTS.Device.Amazon;
            }
        }
        catch (Exception e)
        {
            BaseApplicationException bex = new BaseApplicationException("erorr in ToDevice", e);
            bex.AddAdditionalInformation("userAgent", userAgent);
            throw bex;
        }
        return device;
    }

    public static bool isMobile(HttpRequest Request)
    {

        //
        // the old way of knowing if we are on mobile, was to look for a cookie. 
        //  but since mobile is only when we are on a device, and since cookie method casues problems when perhaps it got lost like the case with mobile to tactica conversion, 
        //  we now reply on isDevice. 
        //  HOWEVER, we leave the cookie check because this allows us devs to set the cookie and get M on browser. 
        //
        HttpCookie isM = Request.Cookies[CONSTS.Cookies.isM];
        if (isM != null && isM.Value == "1")
        {
            return true;
        }
        return isDevice(Request);
    }

    public static bool isD2(HttpRequest Request)
    {

        if (!isMobile(Request))
        {
            HttpCookie isM = Request.Cookies[CONSTS.Cookies.isD2];
            if (isM != null && isM.Value == "1")
            {
                return true;
            }
        }
        return false;

    }

    /// <summary>
    /// tells you if this is one of our apps that is accessing the game, or something else (a browser)
    /// </summary>
    public static bool isDevice(HttpRequest Request)
    {
        return Utils.ToDevice(Request.UserAgent) != CONSTS.Device.Other;
    }



    public static Fbg.Common.UserLoginType getPlayerLoginType(HttpRequest request)
    {
        if (LoginModeHelper.isKongregate(request)) //LoginModeHelper.isKongregate(request)
        {
            return Fbg.Common.UserLoginType.Kong;
        }
        else if ((LoginModeHelper.LoginMode(request) == LoginModeHelper.LoginModeEnum.armoredgames))
        {
            return Fbg.Common.UserLoginType.ArmoredGames;
        }
        else if (LoginModeHelper.isFB(request))
        {
            return Fbg.Common.UserLoginType.FB;
        }
        else if (LoginModeHelper.isBDA(request))
        {
            return Fbg.Common.UserLoginType.Bda;
        }
        else if (LoginModeHelper.isMob(request))
        {
            if (Utils.ToDevice(request.UserAgent) == CONSTS.Device.Amazon)
            {
                return Fbg.Common.UserLoginType.Mobile_Amazon;
            }
            else if (Utils.ToDevice(request.UserAgent) == CONSTS.Device.Android)
            {
                return Fbg.Common.UserLoginType.Mobile_Android;
            }
            else if (Utils.ToDevice(request.UserAgent) == CONSTS.Device.iOS)
            {
                return Fbg.Common.UserLoginType.Mobile_iOS;
            }
            else
            {
                return Fbg.Common.UserLoginType.Mobile_Unknown;
            }
        }
        else
        {
            return Fbg.Common.UserLoginType.Unknown;
        }
    }
}