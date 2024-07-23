

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Facebook;
using Facebook.WebControls;
using System.Collections.Generic;

using System.Security.Principal;
using Fbg.Bll;

/// <summary>
/// Summary description for MyCanvasIFrameBasePage
/// </summary>
public class MyCanvasIFrameBasePage: GmbcCanvasIFrameBasePage
{
    //private Gmbc.Common.GmbcBaseClass.Trace TRACE;
    private Fbg.Bll.User _fbgUser;
    private Fbg.Bll.Player _fbgPlayer;
    private bool _noRedirectOnFBGPlayerGet = false;
    private Int16 _isAccountSteward = Int16.MinValue;

    /// <summary>
    /// Cached result from Bll.Player.Villages
    /// </summary>
    private List<VillageBase> _myVillages;

    /// <summary>
    /// tells you if user selected Desktop version 2 
    /// </summary>
    public bool isD2
    {
        get
        {
            return Utils.isD2(Request);
        }
    }
    public MyCanvasIFrameBasePage()
    {
       // TRACE = new Gmbc.Common.GmbcBaseClass.Trace("fbg.web", "fbg.web.MyCanvasIFrameBasePage");
    }

    /// <summary>
    /// true if the currently logged in player is actually his steward that is logged in as this player
    /// DEPRECIATED!! better just use FbgPlayer.IsLoggedInAsSteward
    /// 
    /// </summary>
    public bool IsLoggedInAsSteward
    {
        get {
            return FbgPlayer.Stewardship_IsLoggedInAsSteward;
        }
    }

    /// <summary>
    /// true if the player has a cookie that says that this player is loggin as a steward
    /// </summary>
    public bool HaveLoginAsStewardCookie
    {
        get
        {
            return Request.Cookies[CONSTS.Cookies.StewardLoggedInAsRecordID] != null 
                && !String.IsNullOrEmpty(Request.Cookies[CONSTS.Cookies.StewardLoggedInAsRecordID].Value.Trim());
        }
    }

    public Fbg.Bll.User FbgUser
    {
        get
        {
            if (_fbgUser == null)
            {
                TRACE.VerboseLine("in 'get FbgUser'. fbgUser is null. trying session");
                _fbgUser = (Fbg.Bll.User)Session[CONSTS.Session.fbgUser];
                if (_fbgUser == null
                    || !SessionCache.stillInCache(CONSTS.Session.fbgUser) // why do we do this? see case 25402
                    )
                {                   
                    TRACE.VerboseLine("in 'get FbgUser'. fbgUser is null, session[fbgUser] is also null");
                    FbgUser = new Fbg.Bll.User((Guid)LoggedInMembershipUser.ProviderUserKey);
                } 
            }
            return _fbgUser;
        }
        private set
        {
            _fbgUser = value;
            Session[CONSTS.Session.fbgUser] = _fbgUser;
            SessionCache.put(CONSTS.Session.fbgUser, 900000); // 900000 = 15 min. Why do we do this ? beacause of case 25402
        }
    }
    public Fbg.Bll.Player FbgPlayer
    {
        get
        {
            if (_fbgPlayer == null)
            {
                TRACE.VerboseLine("in 'get FbgPlayer'. fbgPlayer is null. trying session");
                _fbgPlayer = (Fbg.Bll.Player)Session[CONSTS.Session.fbgPlayer];
                if (_fbgPlayer == null)
                {
                    TRACE.VerboseLine("in 'get FbgPlayer'. fbgPlayer is null, session[fbgPlayer] is also null. trying Cookie");
                    HttpCookie cookie = Request.Cookies[CONSTS.QuerryString.PlayerID];
                    if (cookie != null)
                    {
                        TRACE.VerboseLine("in 'get FbgPlayer'. Got cookie pid:" + cookie.Value);
                        _fbgPlayer = FbgUser.PlayerByID(Convert.ToInt32(cookie.Value));
                        if (_fbgPlayer == null)
                        {
                            TRACE.VerboseLine("in 'get FbgPlayer'. fbgPlayer is null, session[fbgPlayer] is null, got cookie but FbgUser.PlayerByID(cookie) failed. sending to ChooseRealm");
                            if (!_noRedirectOnFBGPlayerGet)
                            {
                                Response.Redirect("~/ChooseRealm.aspx");
                            }
                        }
                        FbgPlayer = _fbgPlayer;


                        //
                        // Very important check
                        //
                        // check if player is suspended. 
                        //  Administrators allowed - this is realy only here to support the admin LoginAs tool. 
                        //  So it allows an admin to logon as a suspended player
                        //
                        if (_fbgPlayer.IsSuspended && !Context.User.IsInRole("Admin"))
                        {
                            Server.Transfer("suspension.aspx");
                        }
                        //
                        // check if player has an active steward. 
                        //  do not allow him to login unless the stewardship is cancelled. 
                        //  this check must also occur in LoginToRealm but it is here in case a player somehow tries to circumvent the login to realm page. 
                        //
                        if (_fbgPlayer.Stewardship_ActiveStewardPlayerID != null
                            && !_fbgPlayer.Stewardship_IsLoggedInAsSteward)
                        {
                            Server.Transfer("AccountStewards_DeactivateBeforeLogin.aspx");
                        }

                        if (!_fbgPlayer.Stewardship_IsLoggedInAsSteward && !IsThisAdminLoggedInAsThisPlayer)
                        {
                            _fbgPlayer.UpdateLastActivity = true;
                        }
                    }
                    else
                    {
                        TRACE.VerboseLine("in 'get FbgPlayer'. fbgPlayer is null, session[fbgPlayer] is null, cookie is null. sending to ChooseRealm");
                        if (!_noRedirectOnFBGPlayerGet)
                        {

                            Response.Redirect("~/ChooseRealm.aspx");
                        }
                    }
                }
                else
                {
                    //
                    // Very important check
                    //
                    // check if player is suspended. 
                    //  Administrators allowed - this is realy only here to support the admin LoginAs tool. 
                    //  So it allows an admin to logon as a suspended player
                    //
                    if (_fbgPlayer.IsSuspended && !Context.User.IsInRole("Admin"))
                    {
                        Server.Transfer("suspension.aspx");
                    }
                }
            }
            return _fbgPlayer;
        }

        set
        {
            _fbgPlayer = value;
            Session[CONSTS.Session.fbgPlayer] = _fbgPlayer;

            //if (!IsLoggedInAsSteward)
            //{
            //    _fbgPlayer.UpdateLastActivity = true;
            //}
        }
    }

    public void LoginAsSteward()
    {
        //
        // Now check to see if this player is logging in as a steward for someone. If so
        //  try to login this player as the player he is stewarding
        //
        if (HaveLoginAsStewardCookie)
        {
            if (_fbgPlayer == null) { throw new InvalidOperationException("_fbgPlayer is null"); }
            int loginAsStewardRecordID = Convert.ToInt32(Request.Cookies[CONSTS.Cookies.StewardLoggedInAsRecordID].Value);
            int loginAsPlayerID = Int32.MinValue;
            Guid loginAsUserID = Guid.Empty;
            Fbg.Bll.PlayerOther p;
            Fbg.Bll.User loginAsUser;
            Fbg.Bll.Player loginAsPlayer = null;

            DataTable myStewardship = _fbgPlayer.Stewardship_GetMyStewardship();
            foreach (DataRow dr in myStewardship.Rows)
            {
                if ((int)dr[Fbg.Bll.Player.CONSTS.MyStewardshipColIndex.RecordID] == loginAsStewardRecordID)
                {
                    loginAsPlayerID = (int)dr[Fbg.Bll.Player.CONSTS.MyStewardshipColIndex.AccountOwnerPlayerID];
                    break;
                }
            }
            if (loginAsPlayerID != Int32.MinValue)
            {
                p = Fbg.Bll.PlayerOther.GetPlayer(_fbgPlayer.Realm, loginAsPlayerID, _fbgPlayer.ID);                
                loginAsUser = new Fbg.Bll.User((Guid)p.UserID);
                loginAsPlayer = loginAsUser.PlayerByID(loginAsPlayerID);

                if (loginAsPlayer != null)
                {
                    //
                    // ok, all worked, we can not login as this user. 
                    //  so, overwrite the FbgUser and FbgPlayer with the player we are loggin in as. 
                    //
                    //  remove from user players collection all user except the one we are loggin in as in order to prevent 
                    //      any possibility of loggin in as another player of this user
                    //
                    loginAsPlayer.UpdateLastActivity = false; // althought this is the default, we do this just in case. 
                    loginAsUser.RemovePlayerExcept(loginAsPlayer);
                    FbgUser = loginAsUser;
                    FbgPlayer = loginAsPlayer;
                    FbgPlayer.Stewardship_SetIsLoggedInAsSteward(loginAsStewardRecordID);
                }
            }


            if (loginAsPlayer == null)
            {
                //
                // remove the cookie telling me to login as some else since the info is inconsistent somehow 
                //
                HttpCookie stewardCookie = new HttpCookie(CONSTS.Cookies.StewardLoggedInAsRecordID);
                stewardCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(stewardCookie);
            }
        }
    }

    public bool DoNotDoRedirectOnFbgPlayerGet
    {
        set
        {
            _noRedirectOnFBGPlayerGet = value;
        }
    }

    /// <summary>
    /// Same as calling FbgPlayer.Villages except that the result is cached for the lifetime of the page. 
    /// </summary>
    public List<VillageBase> MyVillages
    {
        get
        {
            if (_myVillages == null)
            {
                _myVillages = FbgPlayer.Villages;
            }
            return _myVillages;
        }
    }
    ///// <summary>
    ///// Same as calling FbgPlayer.Villages_Level2 except that the result is cached for the lifetime of the page. 
    ///// </summary>
    //public List<VillageBasicA> MyVillages_BasicA
    //{
    //    get
    //    {
    //        if (_myVillages_BasicA == null)
    //        {
    //            _myVillages_BasicA = FbgPlayer.Villages_BasicA;
    //        }
    //        return _myVillages_BasicA;
    //    }
    //}    /// <summary>
    /// forces MyVillages property to retieve new list from DB
    /// </summary>
    public void MyVillagesInvalidate()
    {
        _myVillages = null;
       // _myVillages_BasicA = null;
    }



    public void InvalidateFbgUser()
    {
        _fbgUser = null;
        Session[CONSTS.Session.fbgUser] = null;

        //  this caused double login to FB with click on login to realm link
        //  this is also found to be unnecessary. 
        //
        //Session.Remove(CONSTS.Session.FacebookSessionKey);
        //Session.Remove(CONSTS.Session.FacebookUserID);
        //Response.Cookies.Remove(CONSTS.Cookies.FacebookSessionKey);
        //Response.Cookies.Remove(CONSTS.Cookies.FacebookUserID);
        //Response.Cookies.Add(new HttpCookie(CONSTS.Cookies.FacebookSessionKey, String.Empty));
        //Response.Cookies.Add(new HttpCookie(CONSTS.Cookies.FacebookUserID, String.Empty));
        //Response.Cookies[CONSTS.Cookies.FacebookSessionKey].Expires = DateTime.Now.Subtract(new TimeSpan(1, 1, 1, 1));
        //Response.Cookies[CONSTS.Cookies.FacebookUserID].Expires = DateTime.Now.Subtract(new TimeSpan(1, 1, 1, 1));

    }
    public void InvalidateFbgPlayer()
    {
        _fbgPlayer = null;
        Session[CONSTS.Session.fbgPlayer] = null;
    }

    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);
        this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        LoginUser();
    }
    public void InvalidateFbgPlayerRoles()
    {
        if (FbgPlayer.Role != null)
        {
            FbgPlayer.Role = null;
        }
    }
    public void InvalidateFbgPlayerClan()
    {
        if (FbgPlayer.Clan != null)
        {
            FbgPlayer.Clan = null;
        }
    }

  

    /*
     * this will work when all pages are upgrade. 
    protected override void OnPreInit(EventArgs e)
    {

        if (Session[CONSTS.Session.IsFBMobile] != null)
        {
            if (!String.IsNullOrEmpty(base.MasterPageFile))
            {
                if (base.MasterPageFile.EndsWith("main.master"))
                {
                    base.MasterPageFile = "masterMain_m.master";
                }
            }
        }
        base.OnPreInit(e);
    }
    */

}












































