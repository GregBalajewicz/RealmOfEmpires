using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fbg.Bll;

public partial class InOutTroopsSummary_d2 : TemplatePage
{
    public Realm realm;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(Request.QueryString["rid"])) {
            throw new ArgumentException("rid expected in Request.QueryString");
        }

        realm = Realms.Realm(Convert.ToInt32(Request.QueryString["rid"]));       
    }
    public InOutTroopsSummary_d2()
    {
        R_OverridePageName = "InOutTroopsSummary_d2.aspx";
    }  
}