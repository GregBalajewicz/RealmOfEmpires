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

public partial class ChooseRealm : MyCanvasIFrameBasePage
{
    bool isForceRefreshFBFriends;

    enum RedirectingTo
    {
        logintoRealm,
        register,
        losingUrlParam
    }

    RedirectingTo? _redirectingTo = null;
    new protected void Page_Load(object sender, EventArgs e)
    {
        // this is done, because FB blocked us in april 2018, because we were redirecting from their frame. 
        //  this was all fixed, except for intro. new players jumped out of frame, because the check LoginType == FB did not work
        //  since the LoginType was not yet set. Doing this hack to handle this. 
        if (Request.QueryString["i"] == "1")
        {
            Session["facebook player"] = true;
        }
        //
        // if GOTO_NOW param was specified, then go there immadiatelly, 
        //  doing this before Page_load on purpose.
        //
        //  !!!!CODE HAS NOT BEEN TESTED!!!!!
        //
        //  so why isnt this code removed you ask? out of fear! out of hear that something perhaps uses it :)
        //
        if (!string.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.GoToNow]))
        {
            Server.Transfer(Request.QueryString[CONSTS.QuerryString.GoToNow], true);
        }

        if (Session["request_ids"] == null && Request["request_ids"] != null)
        {
            Session["request_ids"] = Request["request_ids"];
        }
        //
        // if communication channel is specified in URL 
        //  then send the player to the welcome page
        //
        //  example:
        //  user comes from an add, via a link such as this:
        //    http://apps.facebook.com/realmofempires_stg/?ccid=2&ccm=someMsg&ccd=someDesc
        //  
        //  system will then redirect to welcome.aspx, this will save a cookie and redirect to welcome2.aspx
        //  which will ensure the cookie is there, then redirect back to here, to "chooserealm.aspx" droping the "?ccid=2&ccm=someMsg&ccd=someDesc"
        //  welconme.aspx will save the chanllen info in sessions, 
        //      then this page will read the session and log all the info "?ccid=2&ccm=someMsg&ccd=someDesc" in to the DB
        //
        if (!String.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.CommunicationChannelID]))
        {
            //TRACE.ErrorLine("chooserealm:transfering to welcome");
            Session["In_FB_Frame"] = true;
            Server.Transfer("welcome.aspx");
        }

        //
        // if player has been taken to here, for the first time, from FB (ie, we'll have a "i" querry string param) then note
        //  the login type to be FB right away 
        //
        //  note, from FB, player is taken to https://www.realmofempires.com/chooserealm.aspx?i=1
        //
        //  we cannot tell if player has been taken to here "for the first time", but subsequent sends will not matter. 
        //
        if (LoginModeHelper.LoginMode(Request) == LoginModeHelper.LoginModeEnum.unknown)
        {
            if (!String.IsNullOrWhiteSpace(Request.QueryString["i"]))
            {
                Session["SetLoginModeTo"] = LoginModeHelper.LoginModeEnum.facebook;
            }
        }


        base.Page_Load(sender, e);

        isForceRefreshFBFriends = !String.IsNullOrEmpty(Request.QueryString["forceRefreshFriendList"]);

        //
        // CRITICAL. This ensure that if a new person logges in facebook, 
        //  then we will force getting the new facebook user and hence login the correct one
        //
        this.InvalidateFbgUser();
        this.InvalidateFbgPlayer();
        //
        // remove the cookie (if it exist) telling me to login as someone else (as a steward).
        //  DO NOT DO A REDIRECT (.., TRUE) OR THIS WILL FAIL!!
        //
        HttpCookie stewardCookie = new HttpCookie(CONSTS.Cookies.StewardLoggedInAsRecordID);
        stewardCookie.Expires = DateTime.Now.AddDays(-1);
        Response.Cookies.Add(stewardCookie);
        //
        // if user is coming by clicking a link on a story, or accessing the game from some other communication channel, then we want to note that
        //
        try
        {
            if (Session[CONSTS.Session.CommunicationChannelTrackingObject] != null)
            {
                TrackingInfo trck = (TrackingInfo)Session[CONSTS.Session.CommunicationChannelTrackingObject];
                FbgUser.LogEvent(trck.ChannelIDAsInt, trck.Message, trck.Data);
                Session.Remove(CONSTS.Session.CommunicationChannelTrackingObject);


                Dictionary<string, object> eventProps = new Dictionary<string, object>();
                eventProps["CCID"] = trck.ChannelIDAsInt.ToString();
                eventProps["CCM"] = trck.Message;
                eventProps["CCD"] = trck.Data;
                Utils.Analytics_SendEvent(FbgUser, "CC-Track", eventProps);
            }

        }
        catch (System.Threading.ThreadAbortException) { }
        catch (Exception ex)
        {
            //
            // this is not an essential functionlity so we eat the exception
            ExceptionManager.Publish(new Exception("UNREPORTED EXCEPTION: failure doing LogEvent", ex));
        }

        // if timezone was stored in session then update user's timezone
        if (Session["tz"] != null)
        {
            string tz = Session["tz"] as string;
            TRACE.VerboseLine(string.Format("ChooseRealm: Setting timezone: {0}", tz));
            Session["tz"] = null;
            try
            {
                float ftz = float.Parse(tz);
                // only update if changed
                if (FbgUser.TimeZone != ftz)
                {
                    FbgUser.Update(ftz);
                }
            }
            catch (Exception x)
            {
                TRACE.ErrorLine(string.Format("ChooseRealm: Error when setting timezone: {0}", x.Message));
            }
        }

        //
        // try to get the playerid from the cookie. 
        //  if we get it, then we will try to log this player in
        //
        HttpCookie cookie = Request.Cookies[CONSTS.Cookies.PlayerID];
        int currentlyLoggedInPlayer = 0;
        if (cookie != null)
        {
            Int32.TryParse(cookie.Value, out currentlyLoggedInPlayer);
        }

        //
        // if we dont have a cookie, ie, player is not logged in to any realm, 
        //  then load up some stuff from FB. 
        // Why do we do this ony if no cookie? just for performance reasons. Players are often taken to choose realm page,
        //  and we dont want to load this info every time
        // Unless forceRefreshFriendList is passed, in which case we do it no matter what. 
        //
        // HOW TO ENSURE THIS WILL NOT BE RUN for a steward?
        //   is steward ever accesses this page, the fbguser and player is invalidated and he is back to being logged in 
        //  as him self so this is a non-issue
        //
        if (LoginModeHelper.isFB(Request))
        {
            if (currentlyLoggedInPlayer == 0 || isForceRefreshFBFriends)
            {
                DoSomeFacebookStuff(FbgUser.ID);
            }
        }


        HyperLink hyperlinkOfCurrentlyLoggedInPlayer = null;
        //hyperlinkOfCurrentlyLoggedInPlayer = isMobile ? ListOfRealms1.Initialize(currentlyLoggedInPlayer, FbgUser, isDevice) : ListOfRealmsCompact1.Initialize(currentlyLoggedInPlayer, FbgUser, isDevice);
        hyperlinkOfCurrentlyLoggedInPlayer = ListOfRealms1.Initialize(currentlyLoggedInPlayer, FbgUser, isDevice, LoggedInMembershipUser.CreationDate);


        if (hyperlinkOfCurrentlyLoggedInPlayer != null)
        {
            Response.Redirect(hyperlinkOfCurrentlyLoggedInPlayer.NavigateUrl, false);
            _redirectingTo = RedirectingTo.logintoRealm;
        }
        else
        {
            //
            // if player is not playing on any realm yet, then send him to more friendly registration page
            //
            if (FbgUser.Players.Count > 0)
            {
                /*
                if (ListOfRealmsCompact1.onerealm_Realm != null)
                {
                    ListOfRealmsCompact1.Visible = false;
                    linkEnter.NavigateUrl = ListOfRealmsCompact1.onerealm_LoginLink;
                    linkEnter.Text = "Enter " + ListOfRealmsCompact1.onerealm_Realm.Name;
                    linkChange.Visible = true;
                }
                else
                {
                    linkEnter.NavigateUrl = "ChooseRealm2.aspx";
                    linkEnter.Text = RS("Enter");
                    
                }*/
            }
            else
            {
                if (!String.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.GoToCreatePlayerImmediatelly]))
                {
                    //
                    // if we got this flag in the url, then this means that we send the new user directly to the registration page. 
                    //  we don't display the main welcome page and force him click on enter link to register
                    //
                    Response.Redirect("ChooseRealm_register.aspx", false);
                    _redirectingTo = RedirectingTo.register;
                }
                else
                {
                    //
                    // new req - we alwyas send the person to registration immediatelly
                    //
                    Response.Redirect("ChooseRealm_register.aspx", false);
                    //linkEnter.NavigateUrl = "ChooseRealm_register.aspx";
                    //panelRealms.Visible = false;
                    _redirectingTo = RedirectingTo.register;
                }
            }
        }



        //
        // if player accepting a gift, put a special message
        //
        HttpCookie acceptingGiftCookie = Request.Cookies[CONSTS.Cookies.AcceptingGift];

        //if (acceptingGiftCookie == null && Session["request_ids"] != null)
        //{
        //    string rids = Session["request_ids"].ToString();
        //    Session.Remove("request_ids");
        //    FbgUser.Gifts_AcceptByRequest(rids);

        //    // remove on oauth
        //    var client = new FbSdk.FacebookClient(FacebookConfig.FacebookAccessToken);

        //    foreach (var rid in rids.Split(','))
        //    {
        //        var res = client.Delete(rid + "_" + Request.Cookies["UserId"].Value);
        //    }

        //    acceptingGiftCookie = new HttpCookie(CONSTS.Cookies.AcceptingGift, FbgUser.Gift_GiftRecentlyAccepted.ToString());
        //    Session[CONSTS.Session.AcceptingGift] = FbgUser.Gift_GiftRecentlyAccepted;
        //}


        //
        //when person first loggs in, the chooserealm.aspx page has all the params from facebook,
        // including session ID. it a player shared this url, anyone could log in as them. making 
        //  expire session call to FB fixed the problem until the account owner logged in again. 
        //  So this redirect makes is much harder to mistakenly share the url since the player does not see it. 
        // 
        // example of the url with all FB params:
        //
        //    http://www.realmofempires.com/chooserealm.aspx?i=1&ref=bookmarks&fb_sig_in_iframe=1&fb_sig_base_domain=realmofempires.com&fb_sig_locale=en_US&fb_sig_in_new_facebook=1&fb_sig_time=1287206318.1349&fb_sig_added=1&fb_sig_profile_update_time=1287131575&fb_sig_expires=1287212400&fb_sig_user=%201345231398&fb_sig_session_key=2.MzzL5Zh9sfIZ8opevySY_Q__.3600.1287212400-%201345231398&fb_sig_ss=_KSKTOuU0IZMcD6N_VSgUQ__&fb_sig_cookie_sig=4e48a0258751fe0e48c708f1ffbed866&fb_sig_ext_perms=email%2Cuser_birthday%2Cuser_religion_politics%2Cuser_relationships%2Cuser_relationship_details%2Cuser_hometown%2Cuser_location%2Cuser_likes%2Cuser_activities%2Cuser_interests%2Cuser_education_history%2Cuser_work_history%2Cuser_online_presence%2Cuser_website%2Cuser_groups%2Cuser_events%2Cuser_photos%2Cuser_videos%2Cuser_photo_video_tags%2Cuser_notes%2Cuser_about_me%2Cuser_status%2Cfriends_birthday%2Cfriends_religion_politics%2Cfriends_relationships%2Cfriends_relationship_details%2Cfriends_hometown%2Cfriends_location%2Cfriends_likes%2Cfriends_activities%2Cfriends_interests%2Cfriends_education_history%2Cfriends_work_history%2Cfriends_online_presence%2Cfriends_website%2Cfriends_groups%2Cfriends_events%2Cfriends_photos%2Cfriends_videos%2Cfriends_photo_video_tags%2Cfriends_notes%2Cfriends_about_me%2Cfriends_status&fb_sig_country=us&fb_sig_api_key=89dc1edc0036d868930ccbac657e8b7f&fb_sig_app_id=10471770557&fb_sig=e6c0817f8df6f4572702776ba7e20c10
        //
        if (!String.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.FacebookSessionKey]))
        {
            Response.Redirect("ChooseRealm.aspx", false);
            _redirectingTo = RedirectingTo.losingUrlParam;
        }


        //
        // redirect to the throne room, 
        //

        if (!Response.IsRequestBeingRedirected)
        {
            // we take a player to TR, unless already redirecting somewhere if the player choose to use it instead of choose realm. 
            //
            //  HOWEVER note that, we do so, even if the player is being directed to register; because if a non-logged in player, views the TR
            //  then choose to login, we set "throneroom" cookie, make it session cookie (see ThroneRoom.aspx.cs), 
            //  then send the player to here to login, so we dont want the person to go directly to register page, we want him to go to the TR first
            if (_redirectingTo == null || _redirectingTo == RedirectingTo.register)
            {
                //if (Request.Cookies["throneroom"] != null && Request.Cookies["throneroom"].Value == "1")
                //{
                //    string trLink = "ThroneRoom.aspx";

                //    // if we got this session param, that means the player just logged in (most likelY) but for sure accessed the TR in a non-logged in mode, 
                //    //  and with openChatRID param, that told the TR to open this realm chat 
                //    if (Session["TR_openChatRID"] != null)
                //    {
                //        trLink += "?openChatRID=" + Session["TR_openChatRID"].ToString();
                //        Session.Remove("TR_openChatRID");
                //    }
                //    Response.Redirect(trLink, false);
                //}


                string trLink = "ThroneRoom.aspx";
                // if we got this session param, that means the player just logged in (most likelY) but for sure accessed the TR in a non-logged in mode, 
                //  and with openChatRID param, that told the TR to open this realm chat 
                if (Session["TR_openChatRID"] != null)
                {
                    trLink += "?openChatRID=" + Session["TR_openChatRID"].ToString();
                    Session.Remove("TR_openChatRID");
                }
                Response.Redirect(trLink, false);

            }

        }

    }
    private static bool IsTesterRoleOrHigher
    {
        get
        {
            return (HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.User.IsInRole("tester"));
        }
    }



    public void DoSomeFacebookStuff(Guid loggedInUserID)
    {
        // bug fix: we foudn that if admin used login-as, and right before login would delete the PID cookie
        //  then currentlyLoggedInPlayer woudl be null, and control woudl get here and the person that the admin logged in as, 
        //  would get their info (friends and email) updated with that of the admin. this will prevent that. 
        //
        // we also tried the same when steward logs in as the account they are stewarding, but deleting the cookie did not effect this. We did not investigate exactly why
        //  but note that we cannot do here "IsLoggedInAsSteward" because this will kick player into creating FbgPlayer, and thus get him into a never ending redirect loop. 
        if (IsThisAdminLoggedInAsThisPlayer) { return; }

        try
        {
            //
            // make sure we are logged in to FB
            //
            LoginToFacebook();

            //GetFriends(loggedInUserID);
            GetAndUpdateEmail(loggedInUserID);
        }
        catch (System.Threading.ThreadAbortException) { }
        catch (Exception e)
        {
            //
            // we eat the exception on purpose since we don't want not being able to get friends to prevent someone from logging in. 
            //
            Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(new BaseApplicationException("CRITICAL - LoginToFacebook failed", e));
            return;
        }
    }

    private void GetFriends(Guid loggedInUserID)
    {
        string friendsList = null;

        if (SessionCache.stillInCache("FBService.GetFriendsAppUsersList") && !isForceRefreshFBFriends)
        {
            return;
        }
        SessionCache.put("FBService.GetFriendsAppUsersList", 100000/*10 minutes*/);

        //
        // get friends who use RoE
        //
        try
        {
            if (Session[CONSTS.Session.LoggedInAs] != null)
            {
                friendsList = null;
            }

            else
            {
                //Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(string.Format("GREG:GetFriends: LoggedInFacebookUser.UserId={0}", LoggedInFacebookUser.UserId));
                //
                // try to get the list of friends
                //
                try
                {
                    friendsList = FBService.GetFriendsAppUsersList();
                    //Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(string.Format("GREG:GetFriends: friendsList={0}", friendsList));
                }
                catch (Facebook.Exceptions.FacebookSessionExpiredException) { }
                catch (System.Threading.ThreadAbortException) { }
                catch (Exception e)
                {
                    BaseApplicationException bex = new BaseApplicationException("Error while calling FBService.GetFriendsAppUsersList()", e);
                    bex.AddAdditionalInformation("FbgUser", FbgUser);
                    bex.AddAdditionalInformation("LoggedInFacebookUser", LoggedInFacebookUser);
                    Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(bex);
                    // we eat the exception on putpos 
                }
            }


            //
            // now update our cached list of friend with the current one.
            //
            if (!String.IsNullOrEmpty(friendsList))
            {
                FbgUser.RefreshFriends(friendsList);
                Session["Researchers"] = null; // ensure tha researchers are refreshed
            }

            if (isForceRefreshFBFriends)
            {
                lblFriendsFreshedDebugMessage.Text = "Force refresh of your Facebook friends initiated. You have ";
                if (String.IsNullOrEmpty(friendsList))
                {
                    lblFriendsFreshedDebugMessage.Text += "no Facebook friends who also play Realm of Empires";
                }
                else
                {
                    lblFriendsFreshedDebugMessage.Text += (friendsList.Split(',').Length - 1).ToString() + " who also play Realm of Empires";
                }
            }
        }
        catch (System.Threading.ThreadAbortException) { }
        catch (Exception e)
        {
            //
            // we eat the exception on purpose since we don't want not being able to get friends to prevent someone from logging in. 
            //
            Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(
                new BaseApplicationException("CRITICAL - UNREPORTED ERROR: Error in GetFriends when loggin in", e));
        }

    }

    private void GetAndUpdateEmail(Guid loggedInUserID)
    {
        //
        // try to get the email
        //

        string email = "";
        try
        {
            if (String.IsNullOrEmpty(FacebookConfig.DisconnectedFromFacebookUserID))
            {
                // since we are passing in (true), that means we'll be using the cached version of the call; 
                // this mean we'll only refresh the email once, at login time
                email = FBService.GetUserInfo(true).Email;
            }
            // this line prevents the membership email from being reset to greg@ms.com for each subsequent login after initial disconnectedfromfacebookuserid
            // login
            else if (!String.IsNullOrEmpty(this.LoggedInMembershipUser.Email))
            {
                email = this.LoggedInMembershipUser.Email;
            }
            // set email for initial disconnectedfromfacebookuserid login
            else
            {
                email = "greg@ms.com";
            }
            if (!string.IsNullOrEmpty(email)
                && email.Trim().ToUpper() != this.LoggedInMembershipUser.Email.ToUpper())
            {
                this.LoggedInMembershipUser.Email = email.Substring(0, email.Length > 256 ? 256 : email.Length);
                Membership.UpdateUser(this.LoggedInMembershipUser);
                FbgUser.SetRecoveryEmailState(Fbg.Bll.User.RecoveryEmailState.Verified);
            }
        }
        catch (Facebook.Exceptions.FacebookSessionExpiredException) { }
        catch (System.Threading.ThreadAbortException) { }
        catch (Exception e)
        {
            System.Collections.Specialized.NameValueCollection col = new System.Collections.Specialized.NameValueCollection();
            BaseApplicationException.AddAdditionalInformation(col, "FbgUser", FbgUser);
            BaseApplicationException.AddAdditionalInformation(col, "email", email);
            BaseApplicationException.AddAdditionalInformation(col, "LoggedInFacebookUser", LoggedInFacebookUser);
            Gmbc.Common.Diagnostics.ExceptionManagement.ExceptionManager.Publish(
                new FacebookConnectionException("Error while calling FBService.DirectFQLQuery(SELECT email FROM user WHERE)", e)
                , col);
            //
            // we eat the exception on purpose since this is most likely a connection issue. 
            //  we'll monitor this for other problems.
            //
        }

    }


}