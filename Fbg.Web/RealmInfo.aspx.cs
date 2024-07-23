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


public partial class RealmInfo : MyCanvasIFrameBasePage
{
    public Fbg.Bll.Player _player;
  
    MasterBase_Main mainMasterPage;

    new protected void Page_Load(object sender, EventArgs e)
    {
        _player = FbgPlayer;
        base.Page_Load(sender, e);

        mainMasterPage = (MasterBase_Main)this.Master;
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
