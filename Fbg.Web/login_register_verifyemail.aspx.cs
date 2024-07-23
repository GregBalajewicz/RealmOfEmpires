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

public partial class login_register_verifyemail : MyCanvasIFrameBasePage
{
    bool isForceRefreshFBFriends;
    new protected void Page_Load(object sender, EventArgs e)
    {

        if (!string.IsNullOrWhiteSpace(Config.awsAccessKey))
        {
            TRACE.ErrorLine("in login_register_verifyemail");
            try
            {
                string url = Request.Url.ToString();
                int slashslash = url.IndexOf("//") + 2;
                int slash = url.IndexOf("/", slashslash);
                string host = url.Substring(0, slash);
                MembershipUser mu = LoggedInMembershipUser;

                Mailer mailer = new Mailer(Config.awsAccessKey, Config.awsSecretKey);
                mailer.Send_VerifyEmail_email(host, mu.ProviderUserKey.ToString(), LoggedInMembershipUser.Email, Config.addressToBCCSomeEmailsTo);
                FbgUser.SetRecoveryEmailState(Fbg.Bll.User.RecoveryEmailState.Unverified);

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
                return;
            }
        }

        // this session key may be present if we are verifying email right after a device account was converted to tactica. 
        //  In this case, we are done with it, so remove it. it is safe to do if the key does not exist
        Session.Remove(CONSTS.Session.CreatedTacticaAccountEmail);

        Response.Redirect("ChooseRealm.aspx", false);     

    }

  
}