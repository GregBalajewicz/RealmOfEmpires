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

public partial class Controls_PlayerRanking : BaseControl
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

    public bool IsMobile { get; set; }
    public Controls_PlayerRanking()
    {
        BaseResName = "PlayerRanking.ascx"; 
    }

    public class CONSTS
    {
      
        public class GridColIndex
        {
            public static int Rank = 0;
            public static int PlayerName = 1;
            public static int Clan = 2;
            public static int NumVillages = 3;
            public static int Points = 4;
            public static int AvgPoints= 5;
            public static int AttackPoints = 7;
            public static int DefencePoints = 8;
            public static int GovKilled = 9;
        }
    }

    Player _player;


    public void Initalize(Player p, VillageBasicB v, Realm customRealm)
    {
        Initalize(p, v, customRealm, false);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="p">may be null if customRealm is specified</param>
    /// <param name="v">may be null if customRealm is specified</param>
    /// <param name="customRealm">specify realm to show stats for if other then the realm of the Player p paramater. If spcified, can pass null for p and v</param>
    public void Initalize(Player p, VillageBasicB v,  Realm customRealm, bool isMobile)
    {

        #region localzing some controls 
        //
        // this is needed for localiztion, so that <%# ... %> work
        //
        if (!IsPostBack)
        {
            this.btnChangeArea.DataBind();
            this.btnShowAllAreas.DataBind();
            this.BtnPlayerSearch.DataBind();
            this.BtnFBFriends.DataBind();
            this.HyperLink2.DataBind();
            this.HyperLink4.DataBind();
        }
        #endregion 

        IsMobile = isMobile;
        _player = p;
        _customRealm = customRealm;

        village = v;

        GridView1.Columns[CONSTS.GridColIndex.PlayerName].HeaderText = RS("PlayerName");
        GridView1.Columns[CONSTS.GridColIndex.Clan].HeaderText = RS("Clan");
        GridView1.Columns[CONSTS.GridColIndex.Rank].HeaderText = "";
        GridView1.Columns[CONSTS.GridColIndex.NumVillages].HeaderText = RS("NumberOfVillages");
        GridView1.Columns[CONSTS.GridColIndex.Points].HeaderText = RS("VillagePts");
        GridView1.Columns[CONSTS.GridColIndex.AvgPoints].HeaderText = RS("AvgVillagePts");
        GridView1.Columns[CONSTS.GridColIndex.AttackPoints].HeaderText = RS("AttackPts");
        GridView1.Columns[CONSTS.GridColIndex.DefencePoints].HeaderText = RS("DefensePts");
        GridView1.Columns[CONSTS.GridColIndex.GovKilled].HeaderText = RS("GovsKilled");

        if (IsShowingCustomRealm)
        {
            btnChangeArea.Visible = false;
            BtnFBFriends.Visible = false;
        }

        InitArea();

        //
        // get the ranking for later use
        //dt = Fbg.Bll.Stats.GetPlayerRanking(_player.Realm, _showAreaNumber, Expression);

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
    private bool IsShowingCustomRealm
    {
        get
        {
            return _customRealm != null;
        }
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
            if (IsShowingCustomRealm)
            {
                _showAreaNumber = 0;
            }
            else
            {
                _showAreaNumber = Fbg.Common.Area.GetAreaNumberFromCords(village.xcord, village.ycord);
            }
        }
        else
        {
            //if this is a post back, then lets grab the area from the text box if its there, default to cur village otherwise. 
            if (!Int32.TryParse(txtArea.Text.Trim(), out _showAreaNumber))
            {
                if (IsShowingCustomRealm)
                {
                    _showAreaNumber = 0;
                }
                else
                {
                    _showAreaNumber = Fbg.Common.Area.GetAreaNumberFromCords(village.xcord, village.ycord);
                }
            }
        }
        DisplayProperAreaMessage(_showAreaNumber);
    }

    /// <summary>
    /// this ONLY works when showing ALL PLAYERS. WILL NOT WORK when showing friends SO DO NOT CALL IT !!!
    /// </summary>
    private void SetPageIndex()
    {
        int playerIndex = -1;
        if (!String.IsNullOrEmpty(TxtPlayerSearch.Text.Trim()))
        {
            //
            // we got a name of player being searched therefore show the page withthis player if not found
            //
            string playerName = TxtPlayerSearch.Text.Replace("'", "''");
            DataRow[] dr = dv.Table.Select("Name='" + playerName + "'");

            if (dr.GetLength(0) > 0)
            {
                //
                // player found
                playerIndex = dv.Table.Rows.IndexOf(dr[0]);
            }
        }
        if (playerIndex < 0 )
        {
            //
            // if we dont have a player to show, try to find me in the list 
            //
            if (!IsShowingCustomRealm)
            {
                playerIndex = dv.Table.Rows.IndexOf(dv.Table.Rows.Find(_player.ID));
            }
        }

        playerIndex = playerIndex < 0 ? 0 : playerIndex;
        GridView1.PageIndex = Convert.ToInt32(playerIndex / GridView1.PageSize);
    }

    /// <summary>
    /// after calling this, the title member variable wil have the correct title
    /// </summary>
    /// <param name="dr"></param>
    private void GetTitle(DataRowView dr) 
    {
        //
        // we do this trick to not get new title for every row since most of the rows on 1 page will use the same title. 
        //
        if (title == null 
            || title.ID != (int)dr[Fbg.Bll.Stats.CONSTS.PlayerRanking.TitleID])
        {
            title = GetRealmToShow.TitleByID((int)dr[Fbg.Bll.Stats.CONSTS.PlayerRanking.TitleID]);
            //return 
        }        
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lbl=null;
            int rowID=0;
            HyperLink hLink=null;
            try
            {

                rowID = (GridView1.PageIndex * GridView1.PageSize) + e.Row.RowIndex;

                GetTitle(dv[rowID]);

                hLink = (HyperLink)e.Row.FindControl("linkTitle");
                hLink.Text = title.TitleName((Fbg.Common.Sex)(short)dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.Sex]);
                hLink.NavigateUrl = IsShowingCustomRealm ? "" : NavigationHelper.Title(title.ID);

                hLink = (HyperLink)e.Row.FindControl("LnkPlayerName");
                hLink.Text = dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName].ToString();
                
                hLink.NavigateUrl = IsShowingCustomRealm ? "" :
                    ( NavigationHelper.PlayerPublicProfileByName(dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName].ToString()));
                
               // hLink.Target = IsMobile ? "_blank" : "";

                hLink = (HyperLink)e.Row.FindControl("LnkClan");
                hLink.Text = dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.ClanName].ToString();
                if (!IsMobile) {
                    hLink.NavigateUrl = IsShowingCustomRealm ? "" : "~/" + NavigationHelper.ClanPublicProfile(Convert.ToInt32(dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.ClanID]));
                }

                lbl = (Label)e.Row.FindControl("LblNumbeOfVillages");
                lbl.Text = dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.VillageCount].ToString();

                lbl = (Label)e.Row.FindControl("LblTotalPoints");
                lbl.Text =  FormatNumber((int)dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.TotalPoints]);

                lbl = (Label)e.Row.FindControl("LblAvgPoints");
                lbl.Text =  FormatNumber((int)dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.AveragePoints]);

                lbl = (Label)e.Row.FindControl("LblRank");
                lbl.Text = Convert.ToString(rowID + 1);

                lbl = (Label)e.Row.FindControl("lblAttackPoints");
                lbl.Text =  FormatNumber((long)dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.AttackPoints]);

                lbl = (Label)e.Row.FindControl("lblDefencePoints");
                lbl.Text =  FormatNumber((long)dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.DefencePoints]);

                lbl = (Label)e.Row.FindControl("lblGivKilled");
                lbl.Text =  FormatNumber((long)dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.GovKilled]);

                string PlayerName = TxtPlayerSearch.Text.Trim() == "" ? (IsShowingCustomRealm ? "" : _player.Name) : TxtPlayerSearch.Text.Trim();

                if (String.Compare((string)dv[rowID][Fbg.Bll.Stats.CONSTS.PlayerRanking.PlayerName], PlayerName, true) == 0)
                {
                    e.Row.CssClass += " selected";
                }
            }
            catch (Exception ex)
            {
                BaseApplicationException be = new BaseApplicationException("Error in page_load", ex);
                be.AddAdditionalInformation("rowID", rowID);
                be.AddAdditionalInformation("lbl", lbl);
                be.AddAdditionalInformation("hLink", hLink);
                be.AddAdditionalInformation("dv", dv.ToTable());
                throw be;
            }
        }
    }


    protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
    {
        


            Expression = e.SortExpression;
            InitliazeDataView_dv();
            GridView1.DataSource = dt;
            GridView1.DataBind();
       

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
        SearchForPlayer();
    }


    protected void SearchForPlayer()
    {
        //
        // ASSUME that search for player is allowed ONLY when displaying all players. 
        //  ie, this event hanlder should never be called when displaying friends.
        //

        //
        // How does this work?
        //  this is a recursive function (just 1 recursion)
        //  we first search for player in current ranking (current area). 
        //      if not found, and if we were not searching in all areas, then change the ranking to all areas and search again
        //      if not found and we are already searching in all areas, display error message
        //
        InitliazeDataView_dv();

        string playerName = TxtPlayerSearch.Text.Replace("'", "''");

        DataRow[] dr = dv.Table.Select("Name='" + playerName + "'");

        if (dr.GetLength(0) == 0)
        {
            //
            // player not found
            //
            if (_showAreaNumber == 0)
            {
                //
                // player not found while searching the ranking of all areas so display the error mesage
                //
                if (dr.GetLength(0) == 0)
                {
                    LblPlayerError.Visible = true;
                    LblPlayerError.Text = RS("PlayerNotFound");
                }
            }
            else
            {
                //
                // was searching one area only so now try to search all areas
                //
                ShowAreaNumber = 0;
                DisplayProperAreaMessage(_showAreaNumber);
                dt = Fbg.Bll.Stats.GetPlayerRanking(_player.Realm, _showAreaNumber, Expression);
                SearchForPlayer();
            }

        }
        else
        {
            //
            // player found
            //
            LblPlayerError.Visible = false;
            int PlayerIndex = dv.Table.Rows.IndexOf(dr[0]);
            GridView1.PageIndex = Convert.ToInt32(PlayerIndex / GridView1.PageSize);

            GridView1.DataSource = dv;
            GridView1.DataBind();
        }

    }


    protected void BtnFBFriends_Click(object sender, EventArgs e)
    {
        if (BtnFBFriends.Text == RS("ShowAllPlayers"))
        {
            // 
            // Show all players button click, so show all players
            //
            PnlPlayerSearch.Visible = true;
            pnlArea.Visible = true;
            panelInviteMoreFriends.Visible = false;
            panelInviteFriends.Visible = false;
            BtnFBFriends.Text = RS("ShowFacebookOnly");

            InitliazeDataView_dv();
            SetPageIndex();
        }
        else if (BtnFBFriends.Text == RS("ShowFacebookOnly"))
        {
            //
            // Show FB Friends button clicked therefore show friends
            //
            PnlPlayerSearch.Visible = false;
            pnlArea.Visible = false;
            TxtPlayerSearch.Text = String.Empty;

            BtnFBFriends.Text = RS("ShowAllPlayers");
            //
            // get reanking for all areas for this purpose since when shoing FB friends, we want to show them all.
            ShowAreaNumber = 0;
            dt = Fbg.Bll.Stats.GetPlayerRanking(_player.Realm, ShowAreaNumber, Expression);
            DisplayProperAreaMessage(ShowAreaNumber);

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
                return _player.Realm;
            }
        }
    }

    private void InitliazeDataView_dv()
    {
        dt = Fbg.Bll.Stats.GetPlayerRanking(GetRealmToShow, _showAreaNumber, Expression);
        if (BtnFBFriends.Text == RS("ShowFacebookOnly"))
        {
            //
            // Since button now says 'Show FB Friends, therefore we know we are now showing all players
            //
            dv = new DataView(dt);
        }
        else if (BtnFBFriends.Text == RS("ShowAllPlayers"))
        {
            //
            // Since button now says 'Show ALL', therefore we know we are now showing only friends
            //

            DataTable dtFriends = _player.Friends;
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
                id += "," + _player.ID.ToString();
                dv = new DataView(dt);
                dv.RowFilter = "PlayerID in (" + id + ")";
                dv.Sort = "Rank asc";
            }
        }
    }

    protected void btnChangeArea_Command(object sender, CommandEventArgs e)
    {
        // DON't KNOW HOW TO TRANSLATE THIS TO it being a control now
        //if (!IsValid)
        //{
        //    return;
        //}

        if (e.CommandName == "all")
        {
            txtArea.Text = "0";
        }
        //
        // get the area in the text box and chang it if it different from current. 
        //  why do this? well its not even necessary since the page, on load, reads the text box and it if found a value, it inits to this area
        //  but we do this here JUST in case.
        //
        int tempArea = Convert.ToInt32(txtArea.Text);
        if (_showAreaNumber != tempArea)
        {
            _showAreaNumber = tempArea;
            DisplayProperAreaMessage(_showAreaNumber);

            dt = Fbg.Bll.Stats.GetPlayerRanking(GetRealmToShow, _showAreaNumber, Expression);
        }
        InitliazeDataView_dv();
        SetPageIndex();

        GridView1.DataSource = dv;
        GridView1.DataBind();

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

    private void DisplayProperAreaMessage(int showAreaNumber)
    {
        if (showAreaNumber == 0)
        {
            lblCurArea.Text = RS("ShowPlayerPointsAllAreas");
        }
        else
        {
            if (Fbg.Common.Area.GetAreaNumberFromCords(village.xcord, village.ycord) == showAreaNumber)
            {
                lblCurArea.Text = RS("ShowPlayerPointsCurrentArea") + showAreaNumber.ToString() + ")";
            }
            else
            {
                lblCurArea.Text = RS("ShowPlayerPointsArea") + showAreaNumber.ToString();
            }
        }


    }

    public string Expression
    {
        set
        {
            m_SortExpression = value;
            ViewState["_Expression_"] = m_SortExpression;
        }
        get
        {
            if (ViewState["_Expression_"] != null)
            {
                m_SortExpression = (string)ViewState["_Expression_"];
            }
            else
            {
                m_SortExpression = string.Empty;
            }
            return m_SortExpression;
        }
    }

    void HandleSortImages(GridViewRow headerRow)
    {
        switch (Expression)
        {
            case Fbg.Bll.Stats.CONSTS.PlayerRanking_SortExp.NumVillages:
                Utils.AddSortImage(ref headerRow, CONSTS.GridColIndex.NumVillages, true, SortDirection.Descending);
                break;
            case Fbg.Bll.Stats.CONSTS.PlayerRanking_SortExp.Points:
                Utils.AddSortImage(ref headerRow, CONSTS.GridColIndex.Points, true, SortDirection.Descending);
                break;
            case Fbg.Bll.Stats.CONSTS.PlayerRanking_SortExp.AveragePoints:
                Utils.AddSortImage(ref headerRow, CONSTS.GridColIndex.AvgPoints, true, SortDirection.Descending);
                break;
            case Fbg.Bll.Stats.CONSTS.PlayerRanking_SortExp.AttackPoints:
                Utils.AddSortImage(ref headerRow, CONSTS.GridColIndex.AttackPoints, true, SortDirection.Descending);
                break;
            case Fbg.Bll.Stats.CONSTS.PlayerRanking_SortExp.Defencepoints:
                Utils.AddSortImage(ref headerRow, CONSTS.GridColIndex.DefencePoints, true, SortDirection.Descending);
                break;
            case Fbg.Bll.Stats.CONSTS.PlayerRanking_SortExp.GovKilled:
                Utils.AddSortImage(ref headerRow, CONSTS.GridColIndex.GovKilled, true, SortDirection.Descending);
                break;
            default:
                Utils.AddSortImage(ref headerRow, CONSTS.GridColIndex.Points, true, SortDirection.Descending);
                break;
        }

    }

    protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {

            HandleSortImages(e.Row);

        }

    }
    private string FormatNumber(long num)
    {
        if (IsMobile) {
            return Utils.FormatShortNum(num);
        }
        else {
            return Utils.FormatCost(num);
        }
    }
}
