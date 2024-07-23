using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;

using Gmbc.Common.Diagnostics.ExceptionManagement;
using Facebook.Entity;
using Fbg.Bll;
using System.Collections.Generic;

public partial class throneroom : MyCanvasIFrameBasePage
{

    public string GetStaticDir(string file) 
    {
        return string.Format("\"{0}/{1}\"", Fbg.Common.WebHelper.FileList.GetStaticDir(), file);
    }

    public string GetScriptDir(string file)
    {
        return string.Format("\"{0}/{1}\"", Fbg.Common.WebHelper.FileList.GetScriptDir(), file);
    }

    public static int userVipLevel = 0;
    public bool isSpectatorView = false;
    public bool isSpectatorLoggedIn = true;
    new protected void Page_Load(object sender, EventArgs e)
    {
        if (FbgUser.IsSuspended )
        {
            Response.Redirect("suspension.aspx", true);
        }
        //
        // if viewing throne room of some other user, then check it the viewer is logged in. 
        //
        if (!string.IsNullOrWhiteSpace(Request.QueryString["uid"]) || !string.IsNullOrWhiteSpace(Request.QueryString["pid"]))
        {
            //
            // in spectator view 
            // 
            isSpectatorView = true;

            if (!Context.User.Identity.IsAuthenticated)
            {
                isSpectatorLoggedIn = false;
                loginbutton.Visible = true;
            }
            else
            {
                // make sure we are logged in 
                base.Page_Load(sender, e);
                isSpectatorLoggedIn = false;
            }
        }
        else
        {
            // we are not in spectator view, make sure we are logged in 
            base.Page_Load(sender, e);
        }

        if (isSpectatorView)
        {
            ListOfRealms1.Visible = false;
            realmListCompact.Visible = false;
        }
        else // (!isSpectatorView)
        {
            if (Request.QueryString["try"] == "yes")
            {
                HttpCookie trcookie = new HttpCookie("throneroom", "1");
                trcookie.Expires = DateTime.Now.AddDays(355);
                Response.Cookies.Add(trcookie);
            }
            //
            // stewards have no access
            //
            if (Request.Cookies[CONSTS.Cookies.StewardLoggedInAsRecordID] != null)
            {
                Response.Redirect("AccountStewards_AccessDenied.aspx", true);
            }

            ////
            //// try to get the playerid from the cookie. 
            ////  if we get it, then we will try to log this player in
            ////
            HttpCookie cookie = Request.Cookies[CONSTS.Cookies.PlayerID];
            int currentlyLoggedInPlayer = 0;
            if (cookie != null)
            {
                Int32.TryParse(cookie.Value, out currentlyLoggedInPlayer);
            }


            HyperLink hyperlinkOfCurrentlyLoggedInPlayer = null;
            //hyperlinkOfCurrentlyLoggedInPlayer = isMobile ? ListOfRealms1.Initialize(currentlyLoggedInPlayer, FbgUser, isDevice) : ListOfRealmsCompact1.Initialize(currentlyLoggedInPlayer, FbgUser, isDevice);
            //hyperlinkOfCurrentlyLoggedInPlayer = Initialize(currentlyLoggedInPlayer, FbgUser, -1,  Context.User.IsInRole("Admin") || Context.User.IsInRole("tester"), isDevice, LoggedInMembershipUser.CreationDate);
            hyperlinkOfCurrentlyLoggedInPlayer = ListOfRealms1.Initialize(currentlyLoggedInPlayer, FbgUser, isDevice, LoggedInMembershipUser.CreationDate);
            realmListCompact.Initialize(currentlyLoggedInPlayer, FbgUser, isDevice);           

            //maybe let all users see their own VIP level, even if 0
            //then we get everyone to be aware of the VIP levels?
            if (FbgUser.VIP_isVIP)
            {
                userVipLevel = FbgUser.VIP_vipLevel;
                vipBadge.Visible = true;
                vipPopup.Visible = true;
                vipBadge.Attributes.Add("data-viplevel", userVipLevel.ToString());                
            }
        }

        //
        // log event if coming from newsletter
        try
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["uid"]) && !string.IsNullOrWhiteSpace(Request.QueryString["newsletter"]))
            {
                Fbg.DAL.User.LogEvent(new Guid(Request.QueryString["uid"]), 0, 1002/*SEE UserLogEvents*/, "tr newsletter:" + Request.QueryString["newsletter"], Request.QueryString["data"]);
            }
        }
        catch
        {
        }

     

    }

    public static bool IsTesterRoleOrHigher
    {
        get
        {
            return (HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.User.IsInRole("tester"));
        }
    }

    public static bool IsAdminRole
    {
        get
        {
            return (HttpContext.Current.User.IsInRole("Admin") );
        }
    }
    protected void lnkBacktoCR_Click(object sender, EventArgs e)
    {
        HttpCookie cookie = new HttpCookie("throneroom", "0");
        cookie.Expires = DateTime.Now.AddDays(-355);
        Response.Cookies.Add(cookie);
        Response.Redirect("chooserealm.aspx", false);   
    }
    protected void loginbutton_Click(object sender, EventArgs e)
    {
        // adding a session cookie, to take the player back to the TR after logging in. 
        HttpCookie cookie = new HttpCookie("throneroom", "1");
        Response.Cookies.Add(cookie);
        if (!string.IsNullOrWhiteSpace(Request.QueryString["openChatRID"]))
        {
            // if not logged in player had the url param to auto open the chat, remeber it when we come back
            Session["TR_openChatRID"] = Request.QueryString["openChatRID"];
        }
        Response.Redirect("chooserealm.aspx", false);
    }

    public string VipStatusName
    {
        get
        {
            switch (userVipLevel)
            {

                case 1:
                    return "bronze";
                case 2:
                    return "silver";
                case 3:
                    return "gold";
                case 4:
                    return "diamond";
                default:
                    return "";
            }
        }
    }
        
}