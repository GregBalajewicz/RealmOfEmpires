using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

using Gmbc.Common.Diagnostics.ExceptionManagement;

using Facebook;
using Facebook.WebControls;

using Fbg.Bll;

public partial class Stats : MyCanvasIFrameBasePage
{
    DataTable dt;
    DataView dv;
    Title title; // cached for speed;
    VillageBasicB village;
    /// <summary>
    /// if 0, then show all areas
    /// </summary>
    int _showAreaNumber=0;
    private string m_SortExpression;
    private Realm _customRealm; 
    public class CONSTS
    {
      
        public class GridColIndex
        {
            public static int NumVillages = 3;
            public static int Points = 4;
            public static int AvgPoints= 5;
            public static int AttackPoints = 7;
            public static int DefencePoints = 8;
            public static int GovKilled = 9;
        }
    }
   

    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
        village = mainMasterPage.CurrentlySelectedVillageBasicB;

        if (!String.IsNullOrEmpty(Request.QueryString[global::CONSTS.QuerryString.RealmID]))
        {
            _customRealm = Realms.Realm(Convert.ToInt32(Request.QueryString[global::CONSTS.QuerryString.RealmID]));
           
        }

        RMenuControl.CurrentPage = Controls_RankingMenu.RankingMenuPages.PlayerRanking;
        RMenuControl.IsMobile = isMobile;

        PlayerRanking1.Initalize(FbgPlayer, village, _customRealm, isMobile);

    }
    private bool IsShowingCustomRealm
    {
        get
        {
            return _customRealm != null;
        }
    }
   

    private Realm GetRealmToShow
    {
        get
        {
            if (IsShowingCustomRealm)
            {
                return _customRealm;
            }
            else
            {
                return FbgPlayer.Realm;
            }
        }
    }

    protected override void OnPreInit(EventArgs e)
    {

        if (isMobile)
        {
            base.MasterPageFile = "masterMain_m.master";
        }
        else if (isD2)
        {
            base.MasterPageFile = "masterMain_d2.master";
        }
        base.OnPreInit(e);
    }
}