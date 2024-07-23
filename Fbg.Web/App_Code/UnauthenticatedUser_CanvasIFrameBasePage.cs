
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Facebook;
using Facebook.WebControls;

using System.Security.Principal;

/// <summary>
/// Summary description for MyCanvasIFrameBasePage
/// </summary>
public class UnauthenticatedUser_CanvasIFrameBasePage : GmbcCanvasIFrameBasePage
{
    private Gmbc.Common.GmbcBaseClass.Trace TRACE;

    public UnauthenticatedUser_CanvasIFrameBasePage()
    {
        TRACE = new Gmbc.Common.GmbcBaseClass.Trace("fbg.web", "fbg.web.UnauthenticatedUser_CanvasIFrameBasePage");
    }



    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);
    }
}

