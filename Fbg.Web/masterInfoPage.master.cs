using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Fbg.Bll;
using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class masterInfoPage : BaseMasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }


     public bool isMobile
    {
        get
        {
            return Utils.isMobile(Request);
        }
    }
    public bool isD2
    {
        get
        {
            return Utils.isD2(Request);
        }
    }

}
