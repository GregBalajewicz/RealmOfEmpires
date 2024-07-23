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

public partial class login_resetpassword : MyCanvasIFrameBasePage
{
    new protected void Page_Load(object sender, EventArgs e)
    {


    }


    protected void btnReset_Click(object sender, EventArgs e)
    {
        lblDone.Visible = false;
        lblDone_NoEmail.Visible = false;

        MembershipUser mu = Membership.GetUser(txtEmail.Text.Trim());
        if (mu != null)
        {
            string url = Request.Url.ToString();
            int slashslash = url.IndexOf("//") + 2;
            int slash = url.IndexOf("/", slashslash);
            string host = url.Substring(0, slash);

            Mailer mailer = new Mailer(Config.awsAccessKey, Config.awsSecretKey);
            mailer.Send_ResetPassword_email(mu.ProviderUserKey.ToString(), mu.Email, Config.addressToBCCSomeEmailsTo, host);


            lblDone.Visible = true;
            login.NavigateUrl = String.Format("login_enter.aspx?email={0}", Server.UrlEncode(txtEmail.Text.Trim()));
        }
        else
        {
            lblDone_NoEmail.Visible = true;
        }
    }
}