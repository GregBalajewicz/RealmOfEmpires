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
using System.Text;
using Fbg.Bll;
using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class Controls_GlobalStats : BaseControl
{
    Realm realm;
    DataSet ds;

    public Controls_GlobalStats()
    {
        BaseResName = "GlobalStats.ascx"; 
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ds = Fbg.Bll.Stats.GetGlobalStats(realm);
        loadData();

        loadNumberOfDetails();
    }

    public Realm Realm
    {
        get
        {
            return realm;
        }
        set
        {
            realm = value;
        }
    }
    public bool IsMobile { get; set; }

    /// <summary>
    /// Loads 'Total Troops' and 'Troops Per Player' Data
    /// </summary>
    private void loadData()
    {
        try
        {
            TableRow trTImg = new TableRow();
            TableRow trTData = new TableRow();
            TableRow trPImg = new TableRow();
            TableRow trPData = new TableRow();

            if (ds.Tables[Fbg.Bll.Stats.CONSTS.GlobalStatsTable.Troops].Rows.Count > 0)
            {
                TableRow trHeading = new TableRow();
                TableCell tcHeading = new TableCell();

                trHeading.CssClass = "Sectionheader";
                tcHeading.Text = RS("TotalTroops");
                tcHeading.Font.Bold = true;
                tcHeading.ColumnSpan = realm.GetUnitTypes().Count;
                trHeading.Cells.Add(tcHeading);
                TblTTroops.Rows.Add(trHeading);

                trHeading = new TableRow();
                tcHeading = new TableCell();
                trHeading.CssClass = "Sectionheader";
                tcHeading.Text = RS("TroopsPerPlayer");
                tcHeading.Font.Bold = true;
                tcHeading.ColumnSpan = realm.GetUnitTypes().Count;
                trHeading.Cells.Add(tcHeading);
                TblTPlayer.Rows.Add(trHeading);

                foreach (Fbg.Bll.UnitType ut in realm.GetUnitTypes())
                {
                    TableCell tcTImg = new TableCell();
                    TableCell tcPImg = new TableCell();
                    HyperLink lnkTImg = new HyperLink();
                    HyperLink lnkPImg = new HyperLink();

                    lnkTImg.ImageUrl = ut.IconUrl;
                    lnkTImg.NavigateUrl = NavigationHelper.UnitHelp(realm.ID, ut.ID);
                    lnkTImg.ToolTip = ut.Name;
                    lnkTImg.Text = ut.Name;

                    lnkPImg.ImageUrl = ut.IconUrl;
                    lnkPImg.NavigateUrl = NavigationHelper.UnitHelp(realm.ID, ut.ID);
                    lnkPImg.ToolTip = ut.Name;
                    lnkPImg.Text = ut.Name;

                    tcTImg.Controls.Add(lnkTImg);
                    tcPImg.Controls.Add(lnkPImg);

                    trTImg.Cells.Add(tcTImg);
                    trPImg.Cells.Add(tcPImg);
                }

                for (int i = 0; i < ds.Tables[Fbg.Bll.Stats.CONSTS.GlobalStatsTable.Troops].Rows.Count; i++)
                {
                    TableCell tcTData = new TableCell();
                    TableCell tcPData = new TableCell();
                    int Troops = Convert.ToInt32(ds.Tables[Fbg.Bll.Stats.CONSTS.GlobalStatsTable.Troops].Rows[i][Fbg.Bll.Stats.CONSTS.GlobalStatsTotalTroopsColumns.TotalCount]);
                    decimal TroopsPerPlayer = Convert.ToDecimal(Troops) / Convert.ToDecimal(ds.Tables[Fbg.Bll.Stats.CONSTS.GlobalStatsTable.Players].Rows[0][Fbg.Bll.Stats.CONSTS.GlobalStatsNumberOfColumns.NoOfPlayers].ToString());

                    tcTData.HorizontalAlign = HorizontalAlign.Right;
                    tcTData.Text =  FormatNumber(Troops);

                    tcPData.HorizontalAlign = HorizontalAlign.Right;
                    if (TroopsPerPlayer < 1)
                    {
                        tcPData.Text = decimal.Round(TroopsPerPlayer, 2).ToString();
                    }
                    else
                    {
                        tcPData.Text =  FormatNumber(Convert.ToInt32(TroopsPerPlayer));
                    }

                    trTData.Cells.Add(tcTData);
                    trPData.Cells.Add(tcPData);
                }

                trTImg.CssClass = "Sectionheader";
                trTData.CssClass = "DataRowAlternate";

                trPImg.CssClass = "Sectionheader";
                trPData.CssClass = "DataRowAlternate";

                TblTPlayer.Rows.Add(trPImg);
                TblTPlayer.Rows.Add(trPData);

                TblTTroops.Rows.Add(trTImg);
                TblTTroops.Rows.Add(trTData);
            }
        }
        catch (Exception e)
        {
            BaseApplicationException be = new BaseApplicationException("Error calling loadData", e);
            throw be;
        }
    }

    /// <summary>
    /// loads Player,Clan,Village and Silver Stats
    /// </summary>
    private void loadNumberOfDetails()
    {
        int NoOfPlayers = 0;
        int NoOfClans = 0;
        int NoOfVillages = 0;
        long NoOfSilver = 0;
        double SilverProduction = 0;

        try
        {
            NoOfPlayers = (int)ds.Tables[Fbg.Bll.Stats.CONSTS.GlobalStatsTable.Players].Rows[0][Fbg.Bll.Stats.CONSTS.GlobalStatsNumberOfColumns.NoOfPlayers];
            NoOfVillages = (int)ds.Tables[Fbg.Bll.Stats.CONSTS.GlobalStatsTable.Players].Rows[0][Fbg.Bll.Stats.CONSTS.GlobalStatsNumberOfColumns.NoOfVillages];
            NoOfSilver = (long)ds.Tables[Fbg.Bll.Stats.CONSTS.GlobalStatsTable.Players].Rows[0][Fbg.Bll.Stats.CONSTS.GlobalStatsNumberOfColumns.NoOfSilver];
            NoOfClans = (int)ds.Tables[Fbg.Bll.Stats.CONSTS.GlobalStatsTable.Players].Rows[0][Fbg.Bll.Stats.CONSTS.GlobalStatsNumberOfColumns.NoOfPClans];
            SilverProduction = (double)ds.Tables[Fbg.Bll.Stats.CONSTS.GlobalStatsTable.Players].Rows[0][Fbg.Bll.Stats.CONSTS.GlobalStatsNumberOfColumns.SilverProduction];

            LblPlayerNo.Text =   FormatNumber(NoOfPlayers);
            LblVillagesNo.Text =   FormatNumber(NoOfVillages);
            LblVillagesPlayer.Text =   FormatNumber(Convert.ToInt32(NoOfVillages / NoOfPlayers));
            LblClanNo.Text =   FormatNumber(NoOfClans);
            if (NoOfClans > 0)
            {
                LblPlayersClan.Text =   FormatNumber(Convert.ToInt32(NoOfPlayers / NoOfClans));
            }
            else
            {
                LblPlayersClan.Text = "0";
            }
            LblTSilver.Text =   FormatNumber(NoOfSilver);
            LblSilverPlayer.Text =   FormatNumber(Convert.ToInt32(NoOfSilver/ NoOfPlayers));
            LblSilverProdHour.Text =   FormatNumber(Convert.ToInt64(SilverProduction));
            LblSilverProdHourPlayer.Text =   FormatNumber(Convert.ToInt64(SilverProduction / NoOfPlayers));
        }
        catch (Exception e)
        {
            BaseApplicationException be = new BaseApplicationException("Error calling loadData", e);
            throw be;
        }
    }
    private string FormatNumber(long num)
    {
        if (IsMobile) 
        {
            return Utils.FormatShortNum(num);
        } else {
            return Utils.FormatCost(num);
        }
    }
}