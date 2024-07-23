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

public partial class login_resetpassword2 : MyCanvasIFrameBasePage
{
    Guid userID;
    MembershipUser userMembershipUser;
    new protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Request.QueryString["uid"]))
            {
                throw new Exception("string.IsNullOrWhiteSpace(Request.QueryString[uid]) is true");
            }

            userID = Guid.Parse(Request.QueryString["uid"].Trim());

            userMembershipUser = Membership.GetUser(userID);
            if (userMembershipUser == null)
            {
                throw new Exception("could not find membership user with this userid");
            }
        }
        catch (Exception ex)
        {
            BaseApplicationException bex = new BaseApplicationException("Error in login_resetpassword2.Page_Load", ex);
            bex.AddAdditionalInformation("IsPostBack", IsPostBack);
            bex.AddAdditionalInformation("Request.QueryString[uid]", Request.QueryString["uid"]);
            ExceptionManager.Publish(bex);
            Response.Redirect("login_resetpassword.aspx");
        }


        string newpassword = userMembershipUser.ResetPassword();
        
        Mailer mailer = new Mailer(Config.awsAccessKey, Config.awsSecretKey);
        mailer.Send_ResetPasswordDone_email(newpassword, userMembershipUser.Email, Config.addressToBCCSomeEmailsTo);


    }
}