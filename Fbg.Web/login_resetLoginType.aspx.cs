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

public partial class login_resetLoginType : MyCanvasIFrameBasePage
{
    bool isForceRefreshFBFriends;
    new protected void Page_Load(object sender, EventArgs e)
    {
        HttpCookie myCookie = new HttpCookie("lt");
        myCookie.Expires = DateTime.Now.AddDays(-1d);
        Response.Cookies.Add(myCookie);

        myCookie = new HttpCookie(CONSTS.Cookies.LoginMethod);
        myCookie.Expires = DateTime.Now.AddDays(-1d);
        Response.Cookies.Add(myCookie);

        FormsAuthentication.SignOut();

        if (isMobile)
        {
            Response.Redirect("chooselogintype.aspx", false);
        }
        else
        {
            Response.Redirect("login_how.aspx", false);
        }
    }

  
}