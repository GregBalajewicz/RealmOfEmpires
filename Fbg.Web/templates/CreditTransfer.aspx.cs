using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fbg.Bll;
public partial class templates_CreditTransfer : TemplatePage
{
    public Realm realm;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(Request.QueryString["rid"]))
        {
            throw new ArgumentException("rid expected in Request.QueryString");
        }
        realm = Realms.Realm(Convert.ToInt32(Request.QueryString["rid"]));
    }


    public templates_CreditTransfer()
    {
        R_OverridePageName = "templates_CreditTransfer.aspx";
    }
}