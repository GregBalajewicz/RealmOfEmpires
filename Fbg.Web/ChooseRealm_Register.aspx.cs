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

public partial class ChooseRealm_Register : MyCanvasIFrameBasePage
{
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);
        bool skipIntro = Request.QueryString["skipintro"] != null;
        System.Diagnostics.Debug.WriteLine(string.Format("ChooseRealm_Register.PageLoad: Headers: {0} QueryString: {1}", 
            Request.Headers.ToString(), Request.QueryString.ToString()));
        if (!IsPostBack)
        {
            //
            // CRITICAL. This ensure that if a new person logges in facebook, 
            //  then we will force getting the new facebook user and hence login the correct one
            //
            this.InvalidateFbgUser();
            this.InvalidateFbgPlayer();

         


            lblWelcome.Text = "Welcome " + LoggedInFacebookUser.Name + "!";

            //
            // try to get the playerid from the cookie. 
            //  if we get it, then we will try to log this player in. Greg March 29 09 - dont think this code actually logs in current player  ..... 
            //
            HttpCookie cookie = Request.Cookies[CONSTS.Cookies.PlayerID];
            int currentlyLoggedInPlayer = 0;
            if (cookie != null)
            {
                Int32.TryParse(cookie.Value, out currentlyLoggedInPlayer);
            }

            lor.Initialize(currentlyLoggedInPlayer, FbgUser, 2, isDevice, LoggedInMembershipUser.CreationDate);




            if (isDevice)
            {
                Random rnd = new Random();
                int randomNum = rnd.Next(0, Config.NewMobilePlayerRealmID.Length);
                Response.Redirect("~/create.aspx?rid=" + Convert.ToInt32(Config.NewMobilePlayerRealmID[randomNum]));
            }
            else
            {
                Random rnd = new Random();
                int randomNum = rnd.Next(0, Config.NewPlayerRealmIDs.Length);
                Response.Redirect("~/create.aspx?rid=" + Convert.ToInt32(Config.NewPlayerRealmIDs[randomNum]));
                
            }

        }

    }


}