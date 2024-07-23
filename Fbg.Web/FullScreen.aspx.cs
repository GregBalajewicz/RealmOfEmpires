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
using Gmbc.Common.Diagnostics.ExceptionManagement;


//
// purpose of this page : this page is mostly used to track where a person is coming from via the url param communication channel
//
public partial class FullScreen : System.Web.UI.Page
{
   

    protected void Page_Load(object sender, EventArgs e)
    {
       

        if (!IsPostBack)
        {
            //
            // test to make sure browser accepts cookies. this cookie we only use to make sure the browser accepts cookies
            //

            Response.Cookies["fullscreen"].Value = "1";
            Response.Redirect("chooserealm.aspx", false);
            


        }
    }
}
