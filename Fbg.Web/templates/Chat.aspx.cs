using Fbg.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Chat : TemplatePage
{
    public Realm realm;
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public Chat()
    {
        R_OverridePageName = "Chat.aspx";
    }
}