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
using System.Resources;

using System.Security.Principal;

/// <summary>
/// Summary description for MyCanvasIFrameBasePage
/// </summary>
public class GmbcCanvasIFrameBasePage: CanvasIFrameBasePage
{
    protected Gmbc.Common.GmbcBaseClass.Trace TRACE;
    protected Gmbc.Common.GmbcBaseClass.Trace TRACE_FBPerf;
    DateTime beforeRun;
    TimeSpan duration;
    private Facebook.Entity.User loggedInFacebookUser = null;
    /// <summary>
    /// retrieves the Encrypted uid from the querystring
    /// </summary>
    public string EncUId
    {
        get
        {
            HttpCookie encUidCookie = Request.Cookies["uid"];
            string encUid = null;
            if (encUidCookie == null)
            {
                // if the header variable is not there then redirect to api page ... app will intercept and pass correct playerid param and reset header var
                Response.Redirect("roe://relogin");
            }
            else
            {
                encUid = encUidCookie.Value;
            }
            return encUid;
        }
    }
    /// <summary>
    /// sets the encrypted uid in the uid cookie from the query string
    /// </summary>
    /// <param name="uid"></param>
    protected void SetEncUid(string uid)
    {
        string encUid = ROEDeviceUtils.AESEncryption.Encrypt(uid);
        HttpCookie encUidCookie = new HttpCookie("uid", encUid);
        Response.Cookies.Add(encUidCookie);
    }
    /// <summary>
    /// Sets the login mode cookie from the query string
    /// </summary>
    /// <param name="lt"></param>
    protected void SetLoginMode(string lt)
    {
        LoginModeHelper.SetLoginModeInResponse(Response, lt);
    }
    /// <summary>
    ///  Initializes the LoginMode and optionally the EncUid
    /// </summary>
    public void InitLoginType()
    {
        SetLoginMode(LoginModeHelper.LoginModeFromRequestQS(Request));
        if (LoginMode == LoginModeHelper.LoginModeEnum.mobile)
        {
            SetEncUid(Request.QueryString["uid"]);
        }
        if (Request.QueryString["tz"] != null)
        {
            Session["tz"] = Request.QueryString["tz"];
        }
        TRACE.VerboseLine(string.Format("GmbcCanvasIFrameBasePage.InitLoginType: {0}", Request.QueryString.ToString()));
    }

    public LoginModeHelper.LoginModeEnum LoginMode
    {
        get 
        {
            return LoginModeHelper.LoginMode(Request);
        }
    }

    protected override void OnPreInit(EventArgs e)
    {
        beforeRun = DateTime.Now;
        base.OnPreInit(e);
    }
    protected override void OnSaveStateComplete(EventArgs e)
    {
        // get player id if possible
        string pid = String.Empty;
        string rid = String.Empty;

        Fbg.Bll.Player p = (Fbg.Bll.Player)Session[CONSTS.Session.fbgPlayer];
        if (p != null)
        {
            pid = p.ID.ToString();
            rid = ".R" + p.Realm.ID.ToString();
        }
        duration = DateTime.Now.Subtract(beforeRun);
        Fbg.DAL.Logger.LogEvent("web" + rid, Request.Url.AbsolutePath, "PID:" + pid + ",QS:" + Request.QueryString.ToString(), duration.Ticks);
        base.OnSaveStateComplete(e);

    }

    public GmbcCanvasIFrameBasePage()
    {
        TRACE = new Gmbc.Common.GmbcBaseClass.Trace("fbg.web", "fbg.web");
        TRACE_FBPerf = new Gmbc.Common.GmbcBaseClass.Trace("fbg.web_FBPerf", "fbg.web_FBPerf");

    }

    public bool IsThisAdminLoggedInAsThisPlayer
    {
        get
        {
            if (Session[CONSTS.Session.LoggedInAs] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public Facebook.Entity.User LoggedInFacebookUser
    {
        get
        {
            if (loggedInFacebookUser == null)
            {
                if (Session[CONSTS.Session.LoggedInAs] != null)
                {
                    //
                    // allows admin to login as anyone 
                    //
                    loggedInFacebookUser = new Facebook.Entity.User();
                    loggedInFacebookUser.UserId = Membership.GetUser((Guid)Session[CONSTS.Session.LoggedInAs]).UserName;
                }
              
                else
                {
                    #region do proper login, based on settings
                    if (LoginModeHelper.isMob(Request))
                    {
                        #region stand alone mobile device


                        // try to get ASPNET Membership UserID from session -- that is saved in session when droid login completes (in the DroidLogin.aspx as in the diagrams)
                        //  if found, create a dummy Facebook.Entity.User, and stick this username into Facebook.Entity.User.UserID property
                        //  if not found, send a REDIRECT directive to some page that the droid can recognize and initiate the login process
                        loggedInFacebookUser = new Facebook.Entity.User();
                        loggedInFacebookUser.UserId = EncUId;

                        #endregion
                    }
                    else if (LoginModeHelper.isBDA(Request))
                    {
                        #region Bda account login
                        if (Context.User.Identity.IsAuthenticated)
                        {
                            loggedInFacebookUser = new Facebook.Entity.User();
                            loggedInFacebookUser.UserId = Context.User.Identity.Name;
                        }
                        else
                        {
                            Response.Redirect("login_enter.aspx");
                        }

                        #endregion
                    }

                    else if (LoginModeHelper.isKongregate(Request))
                    {
                        #region Kongreagate account login
                        Response.Redirect("login_how.aspx");

                        #endregion
                    }
                    else if (LoginModeHelper.LoginMode(Request) == LoginModeHelper.LoginModeEnum.armoredgames)
                    {
                        #region armoredgames account login
                        Response.Redirect("login_how.aspx");

                        #endregion
                    }
                    else if (LoginModeHelper.isFB(Request))
                    {
                        #region under facebook
                        Response.Redirect("login_how.aspx");

                        #endregion
                    }
                    else
                    {
                        // Response.Redirect(Config.BaseUrl + "login_how.aspx");
                        Response.Redirect("login_how.aspx");
                    }

                    #endregion
                }
            }
            return loggedInFacebookUser;
        }
    }


    //
    // this peace of code has been taken from the facebook toolkit
    //
    private const string FACEBOOK_CANVAS_PARAM = "&canvas";
    private void RedirectToLogin()
    {
        TRACE.VerboseLine("in 'RedirectToLogin()'");

        //
        // clear sessino cookie
        //
        HttpCookie sessionKeyCookie = new HttpCookie(CONSTS.Cookies.FacebookSessionKey);
        HttpCookie userIDCookie = new HttpCookie(CONSTS.Cookies.FacebookUserID);
        sessionKeyCookie.Expires = DateTime.Now.AddDays(-1);
        userIDCookie.Expires = DateTime.Now.AddDays(-1);

        Response.Cookies.Add(sessionKeyCookie);
        Response.Cookies.Add(userIDCookie);


        base.RedirectToLogin();        
    }


    //
    // Necessary if you want to make ANY call to FB
    //
    public void LoginToFacebook()
    {
      

    }


    public void LoginUser()
    {
        // string s = Config.FacebookApiKey;
        

        if (!Context.User.Identity.IsAuthenticated)
        {

            //string name = LoggedInMembershipUser.UserName;
            HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new GenericIdentity(LoggedInFacebookUser.UserId), new string[0]);
            //Membership.ValidateUser(LoggedInMembershipUser.UserName, "p");
            //string name = LoggedInMembershipUser.UserName; // this step is absolytely essential here so that we trigger creation of "LoggedInMembershipUser"
            IssueTicket();
            TRACE.InfoLine("Logging in :" + LoggedInMembershipUser.UserName);
        }
    }


    public void IssueTicket()
    {
        HttpCookie cookie = FormsAuthentication.GetAuthCookie(LoggedInMembershipUser.UserName, false);
        FormsAuthenticationTicket ft = FormsAuthentication.Decrypt(cookie.Value);

        //Cutom user data
        //string userData = Session["facebook_session_key"].ToString();
        string userData = "";
        //if (Session["SessionKey"] != null)
        //{
        //    userData = Session["SessionKey"].ToString();
        //}

        FormsAuthenticationTicket newFt =
        new FormsAuthenticationTicket(
            ft.Version, //version
            ft.Name, //username
            ft.IssueDate, //Issue date
            ft.Expiration, //Expiration date
            ft.IsPersistent,
            userData,
            ft.CookiePath);
        //re-encrypt the new forms auth ticket that includes the user data
        string encryptedValue = FormsAuthentication.Encrypt(newFt);
        //reset the encrypted value of the cookie
        cookie.Value = encryptedValue;
        //set the authentication cookie and redirect
        Response.Cookies.Add(cookie);
        //Response.Redirect(FormsAuthentication.GetRedirectUrl(txtUsername.Text, false), false);
    }

    private MembershipUser loggedInMembershipUser = null;
    public MembershipUser LoggedInMembershipUser
    {
        get
        {
            //if (loggedInMembershipUser == null && Context.User.Identity.IsAuthenticated && LoginModeHelper.isBDA(Request))
            //{

            //    loggedInMembershipUser = Membership.GetUser(Context.User.Identity.Name);

            //}
            //else if (loggedInMembershipUser == null && !Context.User.Identity.IsAuthenticated)
            //{

            //    loggedInMembershipUser = Membership.GetUser(LoggedInFacebookUser.UserId);

            //    //
            //    // if this facebook user is not yet in our db then add him
            //    //
            //    if (loggedInMembershipUser == null)
            //    {
            //        loggedInMembershipUser = Membership.CreateUser(LoggedInFacebookUser.UserId, "p", Fbg.Bll.CONSTS.DummyEmail);
            //    }

            //}

            if (loggedInMembershipUser == null)
            {
                // not passing in true for second param, cause we update LastActivityDate in iLoginLog
                loggedInMembershipUser = Membership.GetUser(LoggedInFacebookUser.UserId);
             
                //
                // if this facebook user is not yet in our db then add him
                //
                if (loggedInMembershipUser == null)
                {
                   
                    string randomPwd = Membership.GeneratePassword(12, 5);
                    MembershipCreateStatus mc;
                    loggedInMembershipUser = Membership.CreateUser(LoggedInFacebookUser.UserId
                        , randomPwd
                        , Fbg.Bll.CONSTS.DummyEmail
                        , null // "Auto generated question and answer"
                        , null //Membership.GeneratePassword(12, 5)
                        , true
                        , out mc);
                    if (mc != MembershipCreateStatus.Success)
                    {
                        throw new MembershipCreateUserException(mc);
                    }                    
                }
            }

            return loggedInMembershipUser;
        }
    }


    new protected void Page_Load(object sender, EventArgs e)
    {
        //if (String.IsNullOrEmpty(Config.DisconnectedFromFacebookUserID))
        //{
        //    base.Api = Config.FacebookApiKey;
        //    base.Secret = Config.FacebookSecretKey;
        //    base.Page_Load(sender, e);
        //}

    }

    public bool isMobile
    {
        get
        {
            return Utils.isMobile(Request);
        }
    }

    
   

    /// <summary>
    /// tells if you the browser is such that we do not do iframe popups. like android
    /// </summary>
    public bool IsiFramePopupsBrowser
    {
        get
        {
            return Utils.IsiFramePopupsBrowser(Request);
        }
    }

    /// <summary>
    /// tells you if this is one of our apps that is accessing the game, or something else (a browser)
    /// </summary>
    public bool isDevice
    {
        get
        {
            return Utils.isDevice(Request);
        }
    }

    /// <summary>
    /// returns the type of device (browser) that is accessing the page
    /// </summary>
    public CONSTS.Device Device
    {
        get
        {
            return Utils.ToDevice(Request.UserAgent);
        }
    }
    #region resources

    /// <summary>
    /// allows a page, to set explicitly its name for the prupose of resource handling. for example, whe someone does server.tranfers to page A,  page A needs to set this 
    /// </summary>
    public string R_OverridePageName { get; set; }

    public string GetCurrentPageName()
    {
        if (String.IsNullOrEmpty(R_OverridePageName))
        {
            string sPath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            System.IO.FileInfo oInfo = new System.IO.FileInfo(sPath);
            return oInfo.Name;
        }
        else
        {
            return R_OverridePageName;
        }
    }

    public string Language()
    {
        return Request.UserLanguages[0].Split(';')[0];
    }

    private ResourceManager _R;
    public ResourceManager R
    {
        get
        {
            if (_R == null)
            {
                _R = new ResourceManager("Resources." + Config.Theme + "." + GetCurrentPageName(), typeof(Resources.global).Assembly);
            }
            return _R;
        }
    }
    private ResourceManager _RCommon;
    public ResourceManager RCommon
    {
        get
        {
            if (_RCommon == null) 
            {
                _RCommon = new ResourceManager("Resources." + Config.Theme + ".Common", typeof(Resources.global).Assembly);
            }
            return _RCommon;
        }
    }
 
    private ResourceManager _R_PFs;
    public ResourceManager R_PFs
    {
        get
        {
            if (_R_PFs == null)
            {
                _R_PFs = new ResourceManager("Resources." + Config.Theme + ".PFs", typeof(Resources.global).Assembly);
            }
            return _R_PFs;
        }
    }

    private ResourceManager _R_MiscMessages;
    public ResourceManager R_MiscMessages
    {
        get
        {
            if (_R_MiscMessages == null)
            {
                _R_MiscMessages = new ResourceManager("Resources." + Config.Theme + ".MiscMessages", typeof(Resources.global).Assembly);
            }
            return _R_MiscMessages;
        }
    }

    private ResourceManager _R_notifications;
    public ResourceManager R_notifications
    {
        get
        {
            if (_R_notifications == null)
            {
                _R_notifications = new ResourceManager("Resources." + Config.Theme + ".notifications", typeof(Resources.global).Assembly);
            }
            return _R_notifications;
        }
    }

    private ResourceManager _R_resStories;
    public ResourceManager R_resStories
    {
        get
        {
            if (_R_resStories == null)
            {
                _R_resStories = new ResourceManager("Resources." + Config.Theme + ".resStories", typeof(Resources.global).Assembly);
            }
            return _R_resStories;
        }
    }

    private ResourceManager _R_Tutorial;
    public ResourceManager R_Tutorial
    {
        get
        {
            if (_R_Tutorial == null)
            {
                _R_Tutorial = new ResourceManager("Resources." + Config.Theme + ".Tutorial", typeof(Resources.global).Assembly);
            }
            return _R_Tutorial;
        }
    }
    /// <summary>
    /// Acess to the COMMON resource file
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string RSc(string name)
    {
        return RCommon.GetString(name);
    }

    public string RS(string name)
    {
        return R.GetString(name);
    }

    /// <summary>
    /// For localized answers from api.aspx. We decided, that answers from will be a key 
    /// from resources, and messages you need to show will be from template. 
    /// On template there will be a class="phrases" that will store all dic items.
    /// ph attribute will be a key, inside tag will be current lang message
    /// Sample:
    /// <div class="phrases">
    ///     <div ph="KeyNotFound">Key not found!</div>
    /// </div>
    /// 
    /// You can see example in the end of /templates/ReportsPopup.aspx
    /// 
    /// For easier put this to file, I created this method Rsdiv, 
    /// you can use instead of RS. With RS it will look like
    /// <div class="phrases">
    ///     <div ph="NotPlayerName"><%= RS("NotPlayerName") %></div>
    ///     <div ph="KeyNotFound"><%= RS("KeyNotFound") %></div>
    /// </div>
    /// 
    /// instead this duplication of key with RSdiv you can write key once
    /// <div class="phrases">
    ///     <%= RSdiv("NotPlayerName") %>
    ///     <%= RSdiv("KeyNotFound") %>
    /// </div>
    /// 
    /// </summary>
    /// <param name="name">resource name</param>
    /// <returns></returns>
    public string RSdiv(string name)
    {
        return String.Format("<div ph='{0}'>{1}</div>", name, R.GetString(name));
    }

    private ResourceManager _RImgsCommon;
    public ResourceManager RImgsCommon
    {
        get
        {
            if (_RImgsCommon == null)
            {
                _RImgsCommon = new ResourceManager("Resources." + Config.Theme + ".ImgsCommon", typeof(Resources.global).Assembly);
            }
            return _RImgsCommon;
        }
    }

    private ResourceManager _RImgs;
    public ResourceManager RImgs
    {
        get
        {
            if (_RImgs == null)
            {
                _RImgs = new ResourceManager("Resources." + Config.Theme + ".Imgs", typeof(Resources.global).Assembly);
            }
            return _RImgs;
        }
    }

    /// <summary>
    /// access to images resource
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string RSi(string name)
    {
        return RImgs.GetString(name);
    }

    /// <summary>
    /// access to images resource
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string RSic(string name)
    {
        return RImgsCommon.GetString(name);
    }
    #endregion

}

