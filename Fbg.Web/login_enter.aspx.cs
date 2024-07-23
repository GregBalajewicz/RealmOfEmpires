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

public partial class login_enter : MyCanvasIFrameBasePage
{
    new protected void Page_Load(object sender, EventArgs e)
    {
        //
        // Kong player on M via Tactica hack
        {
            HttpCookie kviaT = Request.Cookies["isKongViaTactica"];
            if (kviaT != null && kviaT.Value == "1")
            {
                kviaT = new HttpCookie("isKongViaTactica");
                kviaT.Expires = DateTime.Now.AddDays(356);
                Response.Cookies.Add(kviaT);

                Response.Redirect("login_enter_kong.aspx", false);
            }
        }
        //
        // Armor Games player on M via Tactica hack
        {
            HttpCookie agviaT = Request.Cookies["isAGViaTactica"];
            if (agviaT != null && agviaT.Value == "1")
            {
                agviaT = new HttpCookie("isAGViaTactica");
                agviaT.Expires = DateTime.Now.AddDays(356);
                Response.Cookies.Add(agviaT);

                Response.Redirect("login_enter_ag.aspx", false);
            }
        }

        if (!IsPostBack)
        {
            if (!Context.User.Identity.IsAuthenticated)
            {
                string email = Request.QueryString["email"];
                if (string.IsNullOrEmpty(email))
                {
                    email = (string)Session[CONSTS.Session.CreatedTacticaAccountEmail];
                }
                ((TextBox)((System.Web.UI.WebControls.Login)LoginView1.FindControl("Login1")).FindControl("UserName")).Text = email;
            }


            if (Request.QueryString["validateemail"] == "yes"
                || Session[CONSTS.Session.CreatedTacticaAccountEmail] != null // this is done for tactica account created from device login account. we know that in this case, we need to verify the email 
                )
            {
                ((System.Web.UI.WebControls.Login)LoginView1.FindControl("Login1")).DestinationPageUrl = "login_register_verifyemail.aspx";
            }
        }

    }


    protected void Login1_LoggedIn(object sender, EventArgs e)
    {
        HttpCookie loginMethod = new HttpCookie(CONSTS.Cookies.LoginMethod, "bda");
        loginMethod.Expires = DateTime.Now.AddDays(356);
        Response.Cookies.Add(loginMethod);
    }
}