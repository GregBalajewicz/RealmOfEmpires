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

public partial class login_register : MyCanvasIFrameBasePage
{
    new protected void Page_Load(object sender, EventArgs e)
    {
        if (isMobile)
        {
            ((System.Web.UI.WebControls.CreateUserWizard)LoginView1.FindControl("CreateUserWizard1")).CancelDestinationPageUrl = "ChooseLoginType.aspx";
        }
    }

    protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
    {
        HttpCookie loginMethod = new HttpCookie(CONSTS.Cookies.LoginMethod, "bda");
        loginMethod.Expires = DateTime.Now.AddDays(356);
        Response.Cookies.Add(loginMethod);
        Response.Redirect("login_register_verifyemail.aspx", false);     



    }
    protected void CreateUserWizard1_CreatingUser(object sender, LoginCancelEventArgs e)
    {
        CreateUserWizard cw = ((CreateUserWizard)sender);

        bool emailsMatch = String.Equals(cw.UserName.Trim() ,cw.Email.Trim(), StringComparison.OrdinalIgnoreCase);

        if (!emailsMatch) //if emails dont match show error, and cancel
        {
            errorEmailMismatch.Visible = true;
            e.Cancel = true;
        }
        else {
            errorEmailMismatch.Visible = false;
        }

        if (Membership.GetUserNameByEmail(cw.Email.Trim()) != null) //if email already in use, show error, and cancel
        {
            errorEmailInUse.Visible = true;
            e.Cancel = true;
        }
        else {
            errorEmailInUse.Visible = false;
        }
               
    }
    protected void CreateUserWizard1_ContinueButtonClick(object sender, EventArgs e)
    {
        errorEmailInUse.Visible = false;
        errorEmailMismatch.Visible = false;
    }
}