using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fbg.Bll;
using System.Text;
using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class Footer : TemplatePage
{
    public Realm realm;
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public Footer()
    {
        R_OverridePageName = "Footer.aspx";
    }



}