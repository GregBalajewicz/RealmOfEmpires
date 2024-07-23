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

public partial class currentAgeInfo_AOC : MyCanvasIFrameBasePage
{

    public Fbg.Bll.Realm.RealmAgeInfo ageInfo;
    MasterBase_Main mainMasterPage;

    public Realm.Consolidation c;
    public Fbg.Bll.Player _player;

    new protected void Page_Load(object sender, EventArgs e)
    {

        base.Page_Load(sender, e);
        _player = FbgPlayer;



        c = FbgPlayer.Realm.ConsolidationGet;
        if (c.isActiveOnThisRealm)
        {
            if (c.AttackFreezeStartOn > DateTime.Now)
            {
                aocInTheFuture.Visible = true;
                lblTimeTillNextAge.Text = string.Format("{0} days {1} hours {2} minutes"
                    , c.AttackFreezeStartIn.Days, c.AttackFreezeStartIn.Hours, c.AttackFreezeStartIn.Minutes);                
            }
            else if (c.IsAttackFreezeActiveNow)
            {
                aocNow.Visible = true;
            }
        }
        
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
