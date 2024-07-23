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


public partial class AccountStewards_AccessDenied : MyCanvasIFrameBasePage
{

    public class localCONST
    {
        
    }

    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);
        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
    }

    protected override void OnPreInit(EventArgs e)
    {

        if (isMobile)
        {
            base.MasterPageFile = "masterMain_m.master";
        }

        if (isD2)
        {
            base.MasterPageFile = "masterMain_d2.master";
        }


        base.OnPreInit(e);
    }
}
