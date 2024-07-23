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

using Fbg.Bll;
using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class MobDevRealmMessage : MyCanvasIFrameBasePage
{
    public Realm realm ;
    int realmID;

    public CONSTS.Device isDevice
    {
        get
        {
            return Utils.ToDevice(Request.UserAgent);
        }
    }

    new protected void Page_Load(object sender, EventArgs e)
    {   
        
        base.Page_Load(sender, e);
        
    }

}