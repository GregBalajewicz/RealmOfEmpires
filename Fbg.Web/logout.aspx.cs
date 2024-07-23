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

public partial class logout : MyCanvasIFrameBasePage
{

   
    new protected void Page_Load(object sender, EventArgs e)
    {
        if (Utils.GetLoginType(LoggedInMembershipUser) == Fbg.Common.Utils.LoginType.Tactica)
        {
            FormsAuthentication.SignOut();
            Response.Redirect("ChooseRealm.aspx", false);
        }
        else if (Utils.GetLoginType(LoggedInMembershipUser) == Fbg.Common.Utils.LoginType.FB)
        {
            FormsAuthentication.SignOut();
            Response.Redirect("logoutFB.aspx", false);
        }
        else
        {
            // this shoudl never really kick in but leaving it just in case
            FormsAuthentication.SignOut();
            Response.Redirect("ChooseRealm.aspx", false);
        }
    }
}