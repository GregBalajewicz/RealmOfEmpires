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

public partial class ClanRanking : MyCanvasIFrameBasePage
{
    DataTable dtClanRanking;
    DataView dvClanRanking;
    VillageBasicB village;

    /// <summary>
    /// if 0, then show all areas
    /// </summary>
    int _showAreaNumber = 0;

    public class CONSTS
    {

        public class GridColIndex
        {
            public static int Rank = 0;
            public static int ClanName = 1;
            public static int NumPlayers = 2;
            public static int NumVillages = 3;
            public static int Points = 4;
            public static int AttackPts = 5;
            public static int DefencePts = 6;
            public static int AvgAttackPts = 7;
            public static int AvgDefencePts = 8;
            public static int RequestInvite = 9;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        base.Page_Load(sender, e);

        MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
        mainMasterPage.Initialize(FbgPlayer, MyVillages);
        village = mainMasterPage.CurrentlySelectedVillageBasicB;

        #region localzing some controls
        //
        // this is needed for localiztion, so that <%# ... %> work
        //
        this.btnChangeArea.DataBind();
        this.btnShowAllAreas.DataBind();
        this.BtnClanSearch.DataBind();
        #endregion

        RMenuControl.CurrentPage = Controls_RankingMenu.RankingMenuPages.ClanRanking;
        RMenuControl.IsMobile = isMobile;

        InitArea();

        dtClanRanking = Fbg.Bll.Stats.GetClanRanking(FbgPlayer.Realm, _showAreaNumber);
        dvClanRanking = new DataView(dtClanRanking);

        gvClanRanking.Columns[CONSTS.GridColIndex.ClanName].HeaderText = RS("ClanName");
        gvClanRanking.Columns[CONSTS.GridColIndex.Rank].HeaderText = "";
        gvClanRanking.Columns[CONSTS.GridColIndex.NumPlayers].HeaderText = RS("NumberOfPlayers");
        gvClanRanking.Columns[CONSTS.GridColIndex.NumVillages].HeaderText = RS("NumberOfVillages");
        gvClanRanking.Columns[CONSTS.GridColIndex.Points].HeaderText = RS("TotalPoints");
        gvClanRanking.Columns[CONSTS.GridColIndex.AttackPts].HeaderText = RS("AttackPts");
        gvClanRanking.Columns[CONSTS.GridColIndex.DefencePts].HeaderText = RS("DefensePts");
        gvClanRanking.Columns[CONSTS.GridColIndex.AvgAttackPts].HeaderText = RS("AvgAttackPts");
        gvClanRanking.Columns[CONSTS.GridColIndex.AvgDefencePts].HeaderText = RS("AvgDefensePts");
        if (isD2 || isMobile) { 
            gvClanRanking.Columns[CONSTS.GridColIndex.RequestInvite].HeaderText = RS("ClanInvites");
        }
        if (!IsPostBack)
        {
            SetPageIndex();

            gvClanRanking.DataSource = dvClanRanking;
            gvClanRanking.DataBind();
        }

        Page.Form.DefaultButton = BtnClanSearch.UniqueID;
    }
    private void InitArea()
    {
        //
        // we alwyas default to area of the currently selected village. Later on some event handler may change this.
        //  if area is specifed in search box, we take this instead
        //

        if (!IsPostBack)
        {
            // init
            _showAreaNumber = Fbg.Common.Area.GetAreaNumberFromCords(village.xcord, village.ycord);
        }
        else
        {
            //if this is a post back, then lets grab the area from the text box if its there, default to cur village otherwise. 
            if (!Int32.TryParse(txtArea.Text.Trim(), out _showAreaNumber))
            {
                _showAreaNumber = Fbg.Common.Area.GetAreaNumberFromCords(village.xcord, village.ycord);
            }
        }
        DisplayProperAreaMessage(_showAreaNumber);
    }
    private void DisplayProperAreaMessage(int showAreaNumber)
    {
        if (showAreaNumber == 0)
        {
            lblCurArea.Text = RS("ShowClanPointsAll");
        }
        else
        {
            if (Fbg.Common.Area.GetAreaNumberFromCords(village.xcord, village.ycord) == showAreaNumber)
            {
                lblCurArea.Text = RS("ShowClanPointsCurrent") + showAreaNumber.ToString() + ")";
            }
            else
            {
                lblCurArea.Text = RS("ShowClanPointsNumber") + showAreaNumber.ToString();
            }
        }
    }

    private void SetPageIndex()
    {
        //checking Clan, if not null, then check pageindex position of current player's Clan, and make that page current page, if no row found, first page would be shown
        if (FbgPlayer.Clan != null)
        {
            int ClanIndex = dvClanRanking.Table.Rows.IndexOf(dvClanRanking.Table.Rows.Find(FbgPlayer.Clan.ID));
            ClanIndex = ClanIndex < 0 ? 0 : ClanIndex;
            gvClanRanking.PageIndex = Convert.ToInt32(ClanIndex / gvClanRanking.PageSize);
        }
    }
    protected void gvClanRanking_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvClanRanking.PageIndex = e.NewPageIndex;
        gvClanRanking.DataSource = dvClanRanking;
        gvClanRanking.DataBind();
    }
    protected void gvClanRanking_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string ClanName = "";

            int rowID = (gvClanRanking.PageIndex * gvClanRanking.PageSize) + e.Row.RowIndex;
            HyperLink hLink = (HyperLink)e.Row.FindControl("linkClanName");
            hLink.Text = dvClanRanking.Table.Rows[rowID][Fbg.Bll.Stats.CONSTS.ClanRanking.ClanName].ToString();
                hLink.NavigateUrl = NavigationHelper.ClanPublicProfile((int)dtClanRanking.Rows[rowID][Fbg.Bll.Stats.CONSTS.ClanRanking.ClanID]);
            int players = Convert.ToInt32(dvClanRanking.Table.Rows[rowID][Fbg.Bll.Stats.CONSTS.ClanRanking.PlayerCount]);

            if (players == 0) { e.Row.Visible = false; return; }

            Label lblPlayers = (Label)e.Row.FindControl("LblNumbeOfPlayer");
            lblPlayers.Text = players.ToString();
            


            Label LblVillages = (Label)e.Row.FindControl("LblNumbeOfVillages");
            LblVillages.Text = dvClanRanking.Table.Rows[rowID][Fbg.Bll.Stats.CONSTS.ClanRanking.VillageCount].ToString();

            int attack = Convert.ToInt32(dvClanRanking.Table.Rows[rowID][Fbg.Bll.Stats.CONSTS.ClanRanking.AttackPts]);
            int defence = Convert.ToInt32(dvClanRanking.Table.Rows[rowID][Fbg.Bll.Stats.CONSTS.ClanRanking.DefencePts]);

            Label LblAttackPoints = (Label)e.Row.FindControl("LblAttackPoints");
            LblAttackPoints.Text = attack.ToString();

            Label LblDefencePoints = (Label)e.Row.FindControl("LblDefencePoints");
            LblDefencePoints.Text = defence.ToString();

            Label LblAttackPercent = (Label)e.Row.FindControl("LblAttackPercent");
            LblAttackPercent.Text = (attack / players).ToString();

            Label LblDefencePercent = (Label)e.Row.FindControl("LblDefencePercent");
            LblDefencePercent.Text = (defence / players).ToString();

            Label LblTotalPoints = (Label)e.Row.FindControl("LblTotalPoints");
            LblTotalPoints.Text =  FormatNumber((int)dvClanRanking.Table.Rows[rowID][Fbg.Bll.Stats.CONSTS.ClanRanking.TotalPoints]);

            Label LblRank = (Label)e.Row.FindControl("lblClanRank");
            LblRank.Text = Convert.ToString(rowID + 1);

            if (isD2 || isMobile)
            {
                Label LblInvite = (Label)e.Row.FindControl("LblInvite");
                // Since we're using this only on D2 and Mobile but in a iframe popup, we need to use window.parent
                // to get to the launch tool. If we move this into regular code, we'll need to remove window.parent.
                int cid = (int)dvClanRanking.Table.Rows[rowID][Fbg.Bll.Stats.CONSTS.ClanRanking.ClanID];
                if (FbgPlayer.Clan != null && FbgPlayer.Clan.ID == cid) {
                    LblInvite.Text = "Your Clan";
                } else {
                     LblInvite.Text = "<a class='requestClanInviteBtn' style='cursor:pointer' onclick=' window.parent.ROE.Frame.launchClanInviteRequestMessage(" + cid + ");'>Request Invite</a>";
                }
               
               
                
            }

            if (FbgPlayer.Clan!=null)
            {
                ClanName = TxtClanSearch.Text.Trim() == "" ? FbgPlayer.Clan.Name : TxtClanSearch.Text.Trim();
            }
            else
            {
                ClanName = TxtClanSearch.Text.Trim();
            }

            if (dvClanRanking.Table.Rows[rowID][Fbg.Bll.Stats.CONSTS.ClanRanking.ClanName].ToString().ToLower() == ClanName.ToLower())
            {
                e.Row.CssClass += "selected";
            }
        }
    }
    protected void BtnClanSearch_Click(object sender, EventArgs e)
    {
        string clanName = TxtClanSearch.Text.Replace("'", "''");

        DataRow[] dr = dvClanRanking.Table.Select("Name='" + clanName + "'");
        if (dr.GetLength(0) == 0)
        {
            LblClanError.Visible = true;
            LblClanError.Text = RS("ClanNotFound");
        }
        else
        {
            LblClanError.Visible = false;
            int ClanIndex = dvClanRanking.Table.Rows.IndexOf(dr[0]);
            gvClanRanking.PageIndex = Convert.ToInt32(ClanIndex / gvClanRanking.PageSize);

            gvClanRanking.DataSource = dvClanRanking;
            gvClanRanking.DataBind();
        }
    }

    protected void btnChangeArea_Command(object sender, CommandEventArgs e)
    {
        if (!IsValid)
        {
            return;
        }

        if (e.CommandName == "all")
        {
            txtArea.Text = "0";
        }
        //
        // get the area in the text box and chang it if it different from current. 
        //  why do this? well its not even necessary since the page, on load, reads the text box and it if found a value, it inits to this area
        //  but we do this here JUST in case.
        //
        if (!string.IsNullOrEmpty(txtArea.Text.Trim()))
        {
            int tempArea = 0;
            int.TryParse(txtArea.Text.Trim(), out tempArea);
            if (_showAreaNumber != tempArea)
            {
                _showAreaNumber = tempArea;
                DisplayProperAreaMessage(_showAreaNumber);

                dtClanRanking = Fbg.Bll.Stats.GetClanRanking(FbgPlayer.Realm, _showAreaNumber);
                dvClanRanking = new DataView(dtClanRanking);

            }
            SetPageIndex();

            gvClanRanking.DataSource = dvClanRanking;
            gvClanRanking.DataBind();
        }
    }

    /// <summary>
    /// set the _showAreaNumber using this property if you want the page to remember your selection on subsequent post backs
    /// </summary>
    private int ShowAreaNumber
    {
        get
        {
            return _showAreaNumber;
        }
        set
        {
            _showAreaNumber = value;
            txtArea.Text = _showAreaNumber.ToString();
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
    private string FormatNumber(long num)
    {
        if (isMobile) {
            return Utils.FormatShortNum(num);
        }
        else {
            return  Utils.FormatCost(num);
        }
    }
}
