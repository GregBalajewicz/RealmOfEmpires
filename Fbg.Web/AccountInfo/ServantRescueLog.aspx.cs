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

using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class AccountInfo_ServantRescueLog : MyCanvasIFrameBasePage
{

    protected void Page_Load(object sender, EventArgs e)
    {


        SqlDataSource1.ConnectionString = FbgPlayer.Realm.ConnectionStr;
        txtPID.Text = FbgPlayer.ID.ToString();
    }
}
