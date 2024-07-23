using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class templates_VillageOverview : TemplatePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public templates_VillageOverview()
    {
        R_OverridePageName = "templates_VillageOverview.aspx";
    }

    public CONSTS.Device thisDevice
    {
        get
        {
            return Utils.ToDevice(Request.UserAgent);
        }
    }
}