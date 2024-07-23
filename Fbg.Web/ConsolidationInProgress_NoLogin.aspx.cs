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


public partial class ConsolidationInProgress_NoLogin : MyCanvasIFrameBasePage
{


    new protected void Page_Load(object sender, EventArgs e)
    {
        
       
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        if (FbgPlayer.Realm.ConsolidationGet.IsInFreezeTime(DateTime.Now)
            && FbgPlayer.Realm.ConsolidationGet.TimeOfConsolidation <= DateTime.Now
            && FbgPlayer.Realm.ConsolidationGet.TimeOfConsolidation != FbgPlayer.Realm.ConsolidationGet.AttackFreezeStartOn)
        {
            Label1.Visible = true;
            Label2.Visible = true;
        }
        else
        {
            Response.Redirect("ChooseRealm.aspx");
        }
    }
    

}
