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

public partial class ClanEvents : MyCanvasIFrameBasePage
{
    private DataSet ds;
    new protected void Page_Load(object sender, EventArgs e)
    {

        base.Page_Load(sender, e);

        //
        // make sure player is in clan
        if (FbgPlayer.Clan == null)
        {
            Response.Redirect("ClanOverview.aspx");
        }


        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
        ClanMenu1.Player = FbgPlayer;
        ClanMenu1.IsMobile = isMobile;
        ClanMenu1.CurrentPage = Controls_ClanMenu.ManageClanPages.Events;
        BindGrid();


    }
    private void BindGrid()
    {
        if (FbgPlayer.Clan != null)
        {
            //Bind Part
            ds = Fbg.Bll.Clan.GetClanEvents(FbgPlayer);

            if (ds != null)
            {
                gvw_Events.DataSource = ds;
                gvw_Events.DataBind();

            }
            else
            {
                lbl_Error.Visible = true;
                lbl_Error.Text = " You are not member of clan";
            }

        }
    }



    protected void gvw_Events_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvw_Events.PageIndex = e.NewPageIndex;
        BindGrid();
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
