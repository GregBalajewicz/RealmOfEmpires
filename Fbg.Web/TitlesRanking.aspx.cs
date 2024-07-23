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

public partial class TitlesRanking : MyCanvasIFrameBasePage
{
    DataTable dt;
    DataView dv;
    int lastPoints=Int32.MinValue;


    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);

        RMenuControl.CurrentPage = Controls_RankingMenu.RankingMenuPages.Titles;
        RMenuControl.IsMobile = isMobile;

        dt = Fbg.Bll.Stats.GetTitlesRanking(FbgPlayer.Realm);

        if (!IsPostBack)
        {
            //
            // First time this page loads. So we know we are showing ALL PLAYERS
            //
            InitliazeDataView_dv();
            GridView1.DataSource = dv;
            GridView1.DataBind();
        }
    }



    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Label lbl;
        HyperLink hLink ;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int rowID = (GridView1.PageIndex * GridView1.PageSize) + e.Row.RowIndex;

            hLink = (HyperLink)e.Row.FindControl("linkTitle");
            hLink.Text = (string)dv[rowID][Fbg.Bll.Stats.CONSTS.TitlesRanking.Title_Male] 
                + (isMobile ? "" : (" / " + (string)dv[rowID][Fbg.Bll.Stats.CONSTS.TitlesRanking.Title_Female]));

            hLink.NavigateUrl = NavigationHelper.Title((int)dv[rowID][Fbg.Bll.Stats.CONSTS.TitlesRanking.TitleID]);

            lbl = (Label)e.Row.FindControl("lblDesc");
            lbl.Text = (string)dv[rowID][Fbg.Bll.Stats.CONSTS.TitlesRanking.Description];

            lbl= (Label)e.Row.FindControl("lblPoints");
            if (lastPoints == Int32.MinValue)
            {
                //
                // top title 
                lbl.Text = Utils.FormatCost((int)dv[rowID + 1][Fbg.Bll.Stats.CONSTS.TitlesRanking.MaxPoints] + 1) + RS("lblText");
                lastPoints = (int)dv[rowID][Fbg.Bll.Stats.CONSTS.TitlesRanking.MaxPoints];
            }
            else 
            {
                if (rowID == dv.Count - 1)
                {
                    //
                    // this is the last title
                    lbl.Text = RS("upTo") + Utils.FormatCost((int)dv[rowID][Fbg.Bll.Stats.CONSTS.TitlesRanking.MaxPoints]) + RS("points");
                    lastPoints = (int)dv[rowID][Fbg.Bll.Stats.CONSTS.TitlesRanking.MaxPoints];
                }
                else
                {
                    //
                    // not the last title, for the first
                    lbl.Text = Utils.FormatCost((int)dv[rowID +1][Fbg.Bll.Stats.CONSTS.TitlesRanking.MaxPoints] + 1)
                        + " - " + Utils.FormatCost((int)dv[rowID][Fbg.Bll.Stats.CONSTS.TitlesRanking.MaxPoints]) + RS("points");
                    lastPoints = (int)dv[rowID][Fbg.Bll.Stats.CONSTS.TitlesRanking.MaxPoints];
                }
            }


            lbl = (Label)e.Row.FindControl("lblPlayerCount");
            lbl.Text = Utils.FormatCost((int)dv[rowID][Fbg.Bll.Stats.CONSTS.TitlesRanking.PlayerCount]);

            lbl = (Label)e.Row.FindControl("lblLevel");
            lbl.Text = Utils.FormatCost((long)dv[rowID][Fbg.Bll.Stats.CONSTS.TitlesRanking.Level]);

            GridView1.HeaderRow.Cells[1].Text = RS("headerTextLvl");
            GridView1.HeaderRow.Cells[2].Text = RS("headerTextnPly");

            if (FbgPlayer.Title.ID == (int)dv[rowID][Fbg.Bll.Stats.CONSTS.TitlesRanking.TitleID])
            {
                e.Row.CssClass = "selected";
            }
        }
    }




    private void InitliazeDataView_dv()
    {
        dv = new DataView(dt);
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