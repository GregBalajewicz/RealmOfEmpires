using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Facebook;
using Facebook.WebControls;
using Fbg.Bll;
using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class TitleRanking : MyCanvasIFrameBasePage
{
    DataTable dt;
    DataView dv;
    Title title;

    new protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);

        RMenuControl.CurrentPage = Controls_RankingMenu.RankingMenuPages.Titles;
        RMenuControl.IsMobile = isMobile;

        #region Localize Controls
        //bind controls uisng <%# %>

        if (!IsPostBack)
        {
            this.BtnPlayerSearch.DataBind();
            this.BtnFBFriends.DataBind();
        }
        #endregion


        if (!String.IsNullOrEmpty(Request.QueryString[CONSTS.QuerryString.TitleID])) 
        {
            title = FbgPlayer.Realm.TitleByID(Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.TitleID]));
        }
        if (title == null)
        {
            throw new ArgumentException("pass in value for querry string CONSTS.QuerryString.TitleID invalid. Got:" + Request.QueryString[CONSTS.QuerryString.TitleID]);
        }

        linkTitle.Text =title.TitleName_Male + (isMobile ? "" : (" / " + title.TitleName_Female));

        lblDesc.Text = title.Desc;
        lblLevel.Text = String.Format("(+ {0} XP )", title.XP);

        Title nextTitle = title.NextTitle;
        Title prevTitle = title.PreviousTitle;
        if (nextTitle == null)
        {
            //
            // top title 
            lblPoints.Text = Utils.FormatCost(prevTitle.MaxPoints + 1) + RS("pntsAbove");
        }
        else
        {
            if (prevTitle == null)
            {
                //
                // this is the last title
                lblPoints.Text = RS("upTo") + Utils.FormatCost(title.MaxPoints) + RS("points");
            }
            else
            {
                //
                // not the last title, for the first
                lblPoints.Text = Utils.FormatCost(prevTitle.MaxPoints + 1)
                    + " - " + Utils.FormatCost(title.MaxPoints) + RS("points");
            }
        }

        if (nextTitle != null)
        {
            linkNext.NavigateUrl = NavigationHelper.Title(nextTitle.ID);
            linkNext.Text = RS("nextTitle");
            linkNext.Visible = true;
        }
        if (prevTitle != null)
        {
            linkPrevious.NavigateUrl = NavigationHelper.Title(prevTitle.ID);
            linkPrevious.Text = RS("prevTitle");
            linkPrevious.Visible = true;
        }




        dt = Fbg.Bll.Stats.GetTitleRanking(FbgPlayer.Realm, title);

        if (!IsPostBack)
        {
            //
            // First time this page loads. So we know we are showing ALL PLAYERS
            //
            InitliazeDataView_dv();
            SetPageIndex();
            GridView1.DataSource = dv;
            GridView1.DataBind();
        }
        Page.Form.DefaultButton = BtnPlayerSearch.UniqueID;
    }

    /// <summary>
    /// this ONLY works when showing ALL PLAYERS. WILL NOT WORK when showing friends SO DO NOT CALL IT !!!
    /// </summary>
    private void SetPageIndex()
    {
        int PlayerIndex = dv.Table.Rows.IndexOf(dv.Table.Rows.Find(FbgPlayer.ID));
        PlayerIndex = PlayerIndex < 0 ? 0 : PlayerIndex;
        GridView1.PageIndex = Convert.ToInt32(PlayerIndex / GridView1.PageSize);
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Label lbl;
        HyperLink hLink;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            int rowID = (GridView1.PageIndex * GridView1.PageSize) + e.Row.RowIndex;


            hLink = (HyperLink)e.Row.FindControl("LnkPlayerName");
            hLink.Text = dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName].ToString();
            hLink.NavigateUrl = !isMobile ? NavigationHelper.PlayerPublicProfileByName(dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName].ToString())
                : NavigationHelper.PlayerPublicProfileByNamePopup(dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName].ToString());

            HyperLink LnkClan = (HyperLink)e.Row.FindControl("LnkClan");
            LnkClan.Text = dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.ClanName].ToString();
            LnkClan.NavigateUrl = NavigationHelper.ClanPublicProfile(Convert.ToInt32(dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.ClanID]));

            lbl = (Label)e.Row.FindControl("LblNumbeOfVillages");
            lbl.Text = dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.VillageCount].ToString();

            Label LblTotalPoints = (Label)e.Row.FindControl("LblTotalPoints");
            LblTotalPoints.Text = Utils.FormatCost((int)dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.TotalPoints]);

            Label LblAvgPoints = (Label)e.Row.FindControl("LblAvgPoints");
            LblAvgPoints.Text = Utils.FormatCost((int)dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.AveragePoints]);

            Label LblRank = (Label)e.Row.FindControl("LblRank");
            LblRank.Text = Utils.FormatCost((long)dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerRank]);

            #region Localize Headers
            //localize the headers of table by setting the text of the cells here
            GridView1.HeaderRow.Cells[0].Text = RS("hplayerName");
            GridView1.HeaderRow.Cells[1].Text = RS("hClan");
            GridView1.HeaderRow.Cells[2].Text = RS("hOARank");
            GridView1.HeaderRow.Cells[3].Text = RS("hRankWinTitle");
            GridView1.HeaderRow.Cells[4].Text = RS("hnVillages");
            GridView1.HeaderRow.Cells[5].Text = RS("hTotPnts");
            GridView1.HeaderRow.Cells[6].Text = RS("hAvgPnts");
            #endregion

            string PlayerName = TxtPlayerSearch.Text.Trim() == "" ? FbgPlayer.Name : TxtPlayerSearch.Text.Trim();

            if (String.Compare((string)dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName], PlayerName, true) == 0 )
            {
                e.Row.CssClass = "selected";
            }
        }
    }

   
    
    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        InitliazeDataView_dv();

        GridView1.PageIndex = e.NewPageIndex;
        GridView1.DataSource = dv;
        GridView1.DataBind();
    }
    
    protected void BtnPlayerSearch_Click(object sender, EventArgs e)
    {
        //
        // ASSUME that search for player is allowed ONLY when displaying all players. 
        //  ie, this event hanlder should never be called when displaying friends.
        //

        InitliazeDataView_dv();

        string playerName = TxtPlayerSearch.Text.Replace("'", "''");

        DataRow[] dr = dv.Table.Select("Name='" + playerName + "'");
        if (dr.GetLength(0) == 0)
        {
            LblPlayerError.Visible = true;
            LblPlayerError.Text = RS("lblPlyrError");
        }
        else
        {
            LblPlayerError.Visible = false;
            int PlayerIndex = dv.Table.Rows.IndexOf(dr[0]);
            GridView1.PageIndex = Convert.ToInt32(PlayerIndex / GridView1.PageSize);

            GridView1.DataSource = dv;
            GridView1.DataBind();
        }
    }
    protected void BtnFBFriends_Click(object sender, EventArgs e)
    {
            if (BtnFBFriends.Text == RS("showAllPlys"))
            {
                // 
                // Show all players button click, so show all players
                //
                PnlPlayerSearch.Visible = true;
                panelInviteMoreFriends.Visible = false;
                panelInviteFriends.Visible = false;
                BtnFBFriends.Text = RS("showOnlyFrds_btn");

                InitliazeDataView_dv();
                SetPageIndex();
            }
            else if (BtnFBFriends.Text == RS("showOnlyFrds_btn"))
            {
                //
                // Show FB Friends button clicked therefore show friends
                //
                PnlPlayerSearch.Visible = false;
                TxtPlayerSearch.Text = String.Empty;

                BtnFBFriends.Text = RS("showAllPlys");

                InitliazeDataView_dv();
                GridView1.PageIndex = 0;

                if (dv.Table.Rows.Count > 0)
                {
                    panelInviteMoreFriends.Visible = true;
                }
                else
                {
                    panelInviteFriends.Visible = true;
                }
            }
            GridView1.DataSource = dv;
            GridView1.DataBind();
    }


    private void InitliazeDataView_dv()
    {
        if (BtnFBFriends.Text == RS("showOnlyFrds_btn"))
        {
            //
            // Since button now says 'Show FB Friends, therefore we know we are now showing all players
            //
            dv = new DataView(dt);
        }
        else if (BtnFBFriends.Text == RS("showAllPlys"))
        {
            //
            // Since button now says 'Show ALL', therefore we know we are now showing only friends
            //
            DataTable dtFriends = FbgPlayer.Friends;
            string id = "";

            for (int i = 0; i < dtFriends.Rows.Count; i++)
            {
                if (i == 0)
                {
                    id = dtFriends.Rows[i][0].ToString();
                }
                else
                {
                    id += "," + dtFriends.Rows[i][0].ToString();
                }
            }

            if (id.Trim() == "")
            {
                dv = new DataView(dtFriends);
            }
            else
            {
                id += "," + FbgPlayer.ID.ToString();
                dv = new DataView(dt, "PlayerID in (" + id + ")", "Rank asc", DataViewRowState.OriginalRows);
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