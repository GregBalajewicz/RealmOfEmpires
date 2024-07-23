using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

using Gmbc.Common.Diagnostics.ExceptionManagement;

using Facebook;
using Facebook.WebControls;

using Fbg.Bll;

public partial class AddVillages : MyCanvasIFrameBasePage
{
    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);


    }
    protected void Button1_Click3(object sender, EventArgs e)
    {
        Session.Abandon();
    }
}
