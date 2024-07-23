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

public partial class Controls_RankingMenu : BaseControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        BaseResName = "RankingMenu.ascx";
        mnuRankingMenu.Items[0].Text = RS("PlayerRanking");
        mnuRankingMenu.Items[1].Text = RS("ClanRanking");
        mnuRankingMenu.Items[2].Text = RS("Titles");
        mnuRankingMenu.Items[3].Text = RS("GlobalStats");
    }

    public enum RankingMenuPages
    {
        PlayerRanking
        ,ClanRanking
       , Titles
        ,GlobalStats
       
    }

    RankingMenuPages _mainRankingPages;
    public bool IsMobile { get; set; }

    public RankingMenuPages CurrentPage
    {
        get
        {
            return _mainRankingPages;
        }
        set
        {
            _mainRankingPages= value;
            SetSelectedLink(_mainRankingPages);
        }
    }

    private void SetSelectedLink(RankingMenuPages mainRankingPages)
    {

        switch (mainRankingPages)
        {
            case RankingMenuPages.PlayerRanking:
                mnuRankingMenu.Items[0].Selected = true;
                break;
            case RankingMenuPages.ClanRanking:
                mnuRankingMenu.Items[1].Selected = true;
                break;
            case RankingMenuPages.Titles:
                mnuRankingMenu.Items[2].Selected = true;
                break;
            case RankingMenuPages.GlobalStats:
                mnuRankingMenu.Items[3].Selected = true;
                break;
            default:
                throw new ArgumentException(RS("NotRecognized") + " " + mainRankingPages.ToString());


        }
    }
}