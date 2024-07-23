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

using Fbg.Bll;
using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class ChooseRealm2 : MyCanvasIFrameBasePage
{
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        //
        // if no 'no redirect' param is sent, and if person does not yet play at all, then send to nicer registration page
        //
        if (String.IsNullOrEmpty(Request.QueryString["nr"]) && FbgUser.Players.Count <= 0)
        {
            Response.Redirect("ChooseRealm_register.aspx");
        }

        if (!IsPostBack)
        {

            #region TEMPORARILY REMOVED
            /*
             * TEMPORARILY REMOVED AS I can't get it working properly, When fb session expires, we go on a redirect loop.
            //
            // test to make sure browser accepts cookies
            //
            if (Request.QueryString[CONSTS.QuerryString.AcceptsCookies] == null)
            {
                Response.Cookies[CONSTS.Cookies.AcceptsCookies].Value = "ok";
                Response.Cookies[CONSTS.Cookies.AcceptsCookies].Expires = DateTime.Now.AddMinutes(1);
                Response.Redirect("TestForCookies.aspx", false);
                return;
            }
             * */
            #endregion

            //
            // CRITICAL. This ensure that if a new person logges in facebook, 
            //  then we will force getting the new facebook user and hence login the correct one
            //
            this.InvalidateFbgUser();
            this.InvalidateFbgPlayer();

            //
            // remove the cookie (if it exist) telling me to login as someone else (as a steward).
            //
            HttpCookie stewardCookie = new HttpCookie(CONSTS.Cookies.StewardLoggedInAsRecordID);
            stewardCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(stewardCookie);

            lblWelcome.Text = "Welcome " + LoggedInFacebookUser.FirstName + "!";
            //lblWelcome.Text = "Welcome Greg!" ;

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

            HyperLink hyperlinkOfCurrentlyLoggedInPlayer = null;
            hyperlinkOfCurrentlyLoggedInPlayer = ListOfRealms1.Initialize(currentlyLoggedInPlayer, FbgUser, isDevice, LoggedInMembershipUser.CreationDate);

 
            if (hyperlinkOfCurrentlyLoggedInPlayer != null)
            {
                Response.Redirect(hyperlinkOfCurrentlyLoggedInPlayer.NavigateUrl, false);
            }


        }

    }


}