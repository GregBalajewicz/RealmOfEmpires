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

public partial class ChooseRealmD2 : MyCanvasIFrameBasePage
{

    public bool IsTesterRoleOrHigher
    {
        get
        {
            return (Context.User.IsInRole("Admin") || Context.User.IsInRole("tester"));
        }
    }
    new protected void Page_Load(object sender, EventArgs e)
    {
        InitLoginType();
        HttpCookie isD2;
        HttpCookie isMobile = Request.Cookies["isM"];

        if (!String.IsNullOrEmpty(Request.QueryString["logout"]))
        {
            isD2 = new HttpCookie("isD2", "0");
            isD2.Value = "0";
            Response.Cookies.Add(isD2);

            Response.Redirect("villageoverview.aspx", false);
        }
        else
        {
            isD2 = new HttpCookie("isD2", "1");
            Response.Cookies.Add(isD2);


            if (isMobile == null)
            {
                isMobile = new HttpCookie("isM", "0");
            }
            isMobile.Value = "0";
            isMobile.Expires = DateTime.Now.AddDays(-10);
            Response.Cookies.Add(isMobile);


            Response.Redirect("chooserealm.aspx", false);
        }

      
    }


}