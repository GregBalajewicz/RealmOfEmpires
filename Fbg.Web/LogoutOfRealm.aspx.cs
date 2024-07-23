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
using System.Collections.Generic;
using System.Xml;
using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class LogoutOfRealm : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        //
        // remove the player cookie so that the player can choose a different realm
        //
        if (Request.Cookies[CONSTS.Cookies.PlayerID] != null)
        {
            HttpCookie myCookie = new HttpCookie(CONSTS.Cookies.PlayerID);
            myCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(myCookie);
        }

        //
        // remove the cookie (if any) telling me to login as someone else (as a steward).
        //
        HttpCookie stewardCookie = new HttpCookie(CONSTS.Cookies.StewardLoggedInAsRecordID);
        stewardCookie.Expires = DateTime.Now.AddDays(-1);
        Response.Cookies.Add(stewardCookie);


        if (String.IsNullOrEmpty(Request.QueryString["isM"])) {
            Response.Redirect("ChooseRealm.aspx", false);
        }
        else {
            Response.Redirect("ChooseRealm.aspx", false);

        }

    }

}
