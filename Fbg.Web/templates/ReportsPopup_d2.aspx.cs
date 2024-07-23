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
using System.Text;
using Fbg.Bll;



public partial class templates_ReportsPopup_d2 : TemplatePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);
      
    }

    public templates_ReportsPopup_d2()
    {
        // Use the same resource file as mobile
        R_OverridePageName = "templates_ReportsPopup.aspx";
    }
}



