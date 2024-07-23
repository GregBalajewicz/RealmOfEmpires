using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using Fbg.Bll;
using System.Diagnostics;

public partial class Controls_SummaryTable : System.Web.UI.UserControl
{
    public bool IsMobile { get; set; }
    public bool IsD2 { get; set; }
    public bool IsMobileOrD2 { get { return IsMobile || IsD2; } }

    HttpCookie pager_pageSizeCookie;
    int pager_pageSize = 20;
    int pager_itemCount;
    int pager_numOfPages;
    int pager_pageIndex ;
    int pager_pageLowerBoundItemIndex;
    int pager_pageUpperBoundItemIndex;
    const int pager_smallestPageSize = 20;
    DataTable dtNoTransportList;
    /// <summary>
    /// true if player has no PF and village count higher than 2 
    /// </summary>
    bool hasPF;
    Fbg.Bll.UnitType unit=null;
    int chestCost = 0;
    Fbg.Bll.Player _player;
    bool showVillageListIfNoPF;

    public enum SummaryType
    {
        buildings, 
        units,
        listVillagesOnly,
        tags
    }
    struct VillageControlGroup 
    {
        public VillageControlGroup(CheckBox checkbox, int villageID)
        {
            this.checkbox = checkbox;
            this.checkbox.CssClass = "vills";
            checkbox.ID = "ckVid" + villageID.ToString();
            this.villageID = villageID;
        }
        public CheckBox checkbox;
        public int villageID;
    }


    List<VillageControlGroup> villageControlGroups = new List<VillageControlGroup>(50);

    SummaryType summaryType;
    List<Fbg.Bll.VillageBase> villages = null;


    public bool AllowChestsBuying = true;

    public void Initialize(Fbg.Bll.Player player, SummaryType summaryType, List<Fbg.Bll.VillageBase> villages)
    {
        Initialize(player, summaryType, villages, false);
    }
    public void Initialize(Fbg.Bll.Player player, SummaryType summaryType, List<Fbg.Bll.VillageBase> villages, bool showVillageListIfNoPF)
    {
        _player = player;
        this.summaryType = summaryType;
        this.villages = villages;
        this.showVillageListIfNoPF = showVillageListIfNoPF;

        if (summaryType == SummaryType.buildings)
        {
            unit = _player.Realm.GetUnitTypesByID(Fbg.Bll.CONSTS.UnitIDs.Gov);
            chestCost = unit.Cost(null);
        }
        //
        // if player has no PF and village count > 2 then we lock out functionality
        //
        if (!_player.PF_HasPF(Fbg.Bll.CONSTS.PFs.SummaryPages) && villages.Count > 2)
        {
            hasPF = false;
        }
        else
        {
            hasPF = true;
        }

        if (!hasPF && showVillageListIfNoPF && summaryType != SummaryType.listVillagesOnly)
        {
            linkPF.Text = "Unlock the Summary Pages feature to get a full summary table displayed above rather than just village list";
        }


        pager_pageSizeCookie = Request.Cookies[CONSTS.Cookies.SummaryPagesPageSize];


        //
        // init page size to what is in the cookie
        if (pager_pageSizeCookie != null)
        {
            string pageSizeStr = pager_pageSizeCookie.Values["r" + _player.Realm.ID];
            if (!String.IsNullOrEmpty(pageSizeStr))
            {
                pager_pageSize = Convert.ToInt32(pageSizeStr);
            }
        }
        //
        // init all pager stuff
        //
        Pager_CalculatePaging();
        //
        // Does player have the PF for this? if not, display the message
        //
        if (!hasPF)
        {
            linkPF.NavigateUrl = NavigationHelper.PFOffMessage(Fbg.Bll.CONSTS.PFs.SummaryPages);
            panelPF.Visible = true;
        }
        else
        {
            panelPF.Visible = false;
        }
        //
        // populate villages on initial page load
        //
        if (!IsPostBack)
        {
            PopulateVillagesSummaryTable(_player);
        }




        if (_player.SelectedFilter != null)
        {
            imgFilter.ImageUrl = "https://static.realmofempires.com/images/funnel2.png";
            lblSelectedFilter.Text = string.Format("Showing {0} villages in <I>{1}</i> filter.", Utils.FormatCost(villages.Count), _player.SelectedFilter.Name );
        }
        else
        {
            lblSelectedFilter.Text = string.Format("Showing all {0} villages (not filtered)", Utils.FormatCost(villages.Count));
            imgFilter.ImageUrl = "https://static.realmofempires.com/images/funnel1.png";
        }



    }
    public void Pager_CalculatePaging()
    {
        pager_itemCount = villages.Count;
        if (hasPF || showVillageListIfNoPF)
        {
            //
            // figure out how many villages per page
            //
            link20.Font.Bold = link20.CommandArgument == pager_pageSize.ToString() ? true : false;
            link50.Font.Bold = link50.CommandArgument == pager_pageSize.ToString() ? true : false;
            link100.Font.Bold = link100.CommandArgument == pager_pageSize.ToString() ? true : false;

            if (pager_itemCount > pager_pageSize)
            {
                #region page size cannot accomodte all villages
                //
                // page size cannot accomodte all villages 
                //

                panelPager.Visible = true; // show the pager

                //
                // how many pages will we need for display all items?
                pager_numOfPages = pager_itemCount / pager_pageSize;
                if (pager_numOfPages * pager_pageSize < pager_itemCount)
                {
                    pager_numOfPages++;
                }
                //
                // get current page
                //
                if (ViewState["pager_pageIndex"] != null)
                {
                    pager_pageIndex = Convert.ToInt32(ViewState["pager_pageIndex"]);
                }
                else
                {
                    pager_pageIndex = 0;
                }
                //
                // figure out page's upper and loower bound indexes of items
                //
                pager_pageLowerBoundItemIndex = pager_pageIndex * pager_pageSize;
                pager_pageUpperBoundItemIndex = pager_pageLowerBoundItemIndex + pager_pageSize - 1;
                pager_pageUpperBoundItemIndex = pager_pageUpperBoundItemIndex > pager_itemCount - 1 ? pager_itemCount - 1 : pager_pageUpperBoundItemIndex;

                if (pager_pageLowerBoundItemIndex > pager_itemCount - 1)
                {
                    //
                    // then something went wrong, most likely pageindex is inconsistent. Default to first page
                    pager_pageIndex = 0;
                    pager_pageLowerBoundItemIndex = 0;
                    pager_pageUpperBoundItemIndex = pager_pageLowerBoundItemIndex + pager_itemCount - 1;
                }

                //
                // display prev|next nav?
                //
                linkPager_FirstPage.Visible = true;
                linkPager_LastPage.Visible = true;
                linkPager_Next.Visible = true;
                linkPager_Prev.Visible = true;
                lblPager_CurrentPage.Visible = true;
                if (pager_pageIndex > 0)
                {
                    linkPager_Prev.Enabled = true;
                    linkPager_FirstPage.Enabled = true;
                }
                else
                {
                    linkPager_Prev.Enabled = false;
                    linkPager_FirstPage.Enabled = false;
                }
                if (pager_pageIndex < pager_numOfPages - 1)
                {
                    linkPager_Next.Enabled = true;
                    linkPager_LastPage.Enabled = true;
                }
                else
                {
                    linkPager_Next.Enabled = false;
                    linkPager_LastPage.Enabled = false;
                }

                //
                // current page display
                //
                lblPager_CurrentPage.Text = String.Format("(page {0} of {1})", pager_pageIndex + 1, pager_numOfPages);
                #endregion
            }
            else //if (pager_itemCount > pager_pageSize)
            {
                #region page size larger than all items
                //
                // page size larger than all items
                //
                pager_pageLowerBoundItemIndex = 0;
                pager_pageUpperBoundItemIndex = pager_itemCount - 1;
             
                if (pager_itemCount < pager_smallestPageSize)
                {
                    // if we have fewer items than our smallest page, then do not show the pager. 
                    panelPager.Visible = false;
                }
                else
                {
                    // since we can fit all itmes on the same page, and we have more items than the smallest page
                    //  then show the page selection but do not show the page navigation part
                    linkPager_FirstPage.Visible = false;
                    linkPager_LastPage.Visible = false;
                    linkPager_Next.Visible = false;
                    linkPager_Prev.Visible = false;
                    lblPager_CurrentPage.Visible = false;
                }
                #endregion
            }
        }
        else // if (hasPF)
        {
            panelPager.Visible = false;
            pager_pageLowerBoundItemIndex = 0;
            pager_pageUpperBoundItemIndex = (pager_itemCount >= 2) ? 1 : 0;
            panelPager.Visible = false;
        }
    }
    private void PopulateVillagesSummaryTable(Fbg.Bll.Player player)
    {
        tblBuildings.Visible = false;
        tblUnits.Visible = false;
        tblTags.Visible = false;
        unitCountSelectors.Visible = tblUnits.Visible;
        tblVillageList.Visible = false;
        SummaryType tempSummaryType = summaryType;
        if (!hasPF && showVillageListIfNoPF)
        {
            tempSummaryType = SummaryType.listVillagesOnly;
        }

        switch (tempSummaryType)
        {
            case SummaryType.units:
                PopulateVillagesSummaryTable_Units(player);
                tblUnits.Visible = true;
                break;
            case SummaryType.buildings:
                PopulateVillagesSummaryTable_Buildings(player);
                tblBuildings.Visible = true;
                panelGetChests.Visible = hasPF && AllowChestsBuying;
                break;
            case SummaryType.listVillagesOnly:
                PopulateVillagesSummaryTable_ListVillagesOnly(player);
                tblVillageList.Visible = true;
                break;
            case SummaryType.tags:
                PopulateVillagesSummaryTable_Tags(player);
                tblTags.Visible = true;
                break;
        }
        unitCountSelectors.Visible = tblUnits.Visible;

    }

    #region Buildings Summary
    private void PopulateVillagesSummaryTable_Buildings(Fbg.Bll.Player player)
    {
        TableRow row;
        TableCell cell;
        HyperLink link;
        CheckBox box;
        bool alternateRow = false;
        int rowCounter = 0;

        if (player == null)
        {
            throw new ArgumentNullException("player");
        }


        BuildingDisplayTableHelper.AddBuildingTypesToHeaderRow(player.Realm.ID, tblBuildings.Rows[0], player.Realm.BuildingTypes);
  
        try
        {

            Village vil;
            Fbg.Bll.VillageBase vilBase;
            for(int i = pager_pageLowerBoundItemIndex; i <= pager_pageUpperBoundItemIndex; i++) 
            {
                vilBase = villages[i];
                //
                // are we over page size?
                //
                if (rowCounter >= pager_pageSize)
                {
                    break;
                }
                rowCounter++;
                vil = _player.Village(vilBase.id);
                row = new TableRow();
                row.Attributes.Add("vid", vilBase.id.ToString());
                if (alternateRow)
                {
                    row.CssClass = "DataRowAlternate highlight";
                    alternateRow = false;
                }
                else
                {
                    row.CssClass = "DataRowNormal highlight";
                    alternateRow = true;
                }

                cell = new TableCell();

                if (vil.GetBuildingLevel(Fbg.Bll.CONSTS.BuildingIDs.Palace) > 0 
                    && vil.coins >= chestCost
                    && AllowChestsBuying)
                {
                    box = new CheckBox();
                    villageControlGroups.Add(new VillageControlGroup(box, vilBase.id));
                    cell.Controls.Add(box);
                    btnGetChests.Visible = true;
                }

                row.Cells.Add(cell);


                cell = new TableCell();
                link = new HyperLink();
                link.CssClass = IsMobileOrD2 ? "" : "jsV";
                link.Attributes["rel"] = vil.id.ToString();
                link.Attributes["x"] = vil.Cordinates.X.ToString();
                link.Attributes["y"] = vil.Cordinates.Y.ToString();
                link.Attributes["vid"] = vil.id.ToString();
                link.Attributes["opid"] = vil.owner.ID.ToString();
                link.NavigateUrl = IsMobileOrD2 ? "" : NavigationHelper.VillageOverview_Tilda(vil.id);
                link.Text = String.Format("{0}({1},{2})", vil.name, vil.Cordinates.X, vil.Cordinates.Y);
                cell.Controls.Add(link);
                AddIncoming(cell, vil);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Right;
                cell.Text = Utils.FormatCost(vil.Points);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Right;
                cell.Text = Utils.FormatCost(vil.coins);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Right;
                cell.Text = Utils.FormatCost(vil.RemainingPopulation);
                row.Cells.Add(cell);


                //cell = new TableCell();
                //link = new HyperLink();
                //link.Text = "refresh";
                //row.Cells.Add(cell);

                PopulateBuildingsTable(vil, row);

                tblBuildings.Rows.Add(row);
            }
        }
        catch (Exception ex)
        {
            #region Collect error info
            BaseApplicationException be = new BaseApplicationException("Error populating list of villages", ex);
            be.AdditionalInformation.Add("_player.ID", player.ID.ToString());
            try
            {
                if (villages != null)
                {
                    foreach (Fbg.Bll.Village vil in villages)
                    {
                        vil.SerializeToNameValueCollection(be.AdditionalInformation);
                    }
                }
            }
            catch (Exception e2)
            {
                be.AdditionalInformation.Add("Error when serializing villages. msg:", e2.Message);
            }
            throw be;
            #endregion
        }
    }


    void PopulateBuildingsTable(Fbg.Bll.Village village, TableRow row)
    {

        TimeSpan timeRem = new TimeSpan(0);

        TableCell cell;
        Label level;
        Label countdown;
        BuildingTypeLevel curBuildingTypeLevel;
        int curBuildingUpgradingLevel;
        HyperLink btn;

        foreach (BuildingType bt in _player.Realm.BuildingTypes)
        {
            countdown = null;

            curBuildingTypeLevel = bt.Level(village.GetBuildingLevel(bt.ID));
            //curBuildingUpgradingLevel = village.GetUpgradingBuildingLevel(bt);


            cell = new TableCell();
            cell.HorizontalAlign = HorizontalAlign.Right;

            level = new Label();
             

            if (curBuildingTypeLevel == null)
            {
                level.Text = "<B>-</b>";
                level.CssClass = "help UnavailableBuilding";
                level.Attributes.Add("Rel", String.Format("K_{0}_Lvl_0", bt.ID));
            }
            else
            {
                level.Text = "<B>" + curBuildingTypeLevel.Level.ToString() + "</B>";
                //
                // figure out time remaining work in building
                //
                timeRem = village.GetBuildingWorkTimeRemaining(bt);
                if (timeRem != TimeSpan.MinValue)
                {
                    countdown = new Label();
                    if (timeRem > TimeSpan.Zero)
                    {
                        if (timeRem.TotalHours < 1)
                        {
                            countdown.Text = " (" + timeRem.TotalHours.ToString("#0.0") + "h)";
                        }
                        else
                        {
                            countdown.Text = " (" + Math.Round(timeRem.TotalHours).ToString("#0") + " h)";
                        }
                    }
                    else
                    {
                        countdown.Text = "";
                    }
                    countdown.CssClass = "UnavailableBuilding";
                    cell.Controls.Add(countdown);
                }
            }

            cell.Controls.Add(level);
            //
            // no transport indicator for trade post
            //
            if (curBuildingTypeLevel != null)
            {

                if (bt.ID == Fbg.Bll.CONSTS.BuildingIDs.TradePost)
                {
                    btn = new HyperLink();
                    btn.CssClass = "tinyIcon help nox";
                    btn.NavigateUrl = "~/NoQTransportVillages.aspx?svid=" + village.id;
                    if (IsVillageInNoTransportList(village.id))
                    {
                        btn.ImageUrl = "https://static.realmofempires.com/images/cancel_8x8.png";
                        btn.Attributes.Add("rel", "k_notran");
                    }
                    else
                    {
                        btn.ImageUrl = "https://static.realmofempires.com/images/checkmark2_8x8.gif";
                        btn.Attributes.Add("rel", "k_tran");
                    }
                    cell.Controls.Add(btn);
                }
            }

            row.Cells.Add(cell);

            row.Attributes.Add("vid", village.id.ToString());

        }

    }

    //void btn_Command_NoTransportVillages(object sender, CommandEventArgs e)
    //{
    //    int villageID;
    //    if (e.CommandName == "add")
    //    {
    //        villageID = Convert.ToInt32(e.CommandArgument);
    //        Fbg.Bll.CoinTransport.AddToNoTransportsCoinsList(FbgPlayer, villageID);
    //    }
    //    else if (e.CommandName == "rem")
    //    {
    //        villageID = Convert.ToInt32(e.CommandArgument);
    //        Fbg.Bll.CoinTransport.DeleteFromNoTransportsCoinsList(FbgPlayer, villageID);
    //    }
    //    PopulateVillagesSummaryTable(FbgPlayer);
    //}

    #endregion 

    #region Units Summary

    private void PopulateVillagesSummaryTable_Units(Fbg.Bll.Player player)
    {
        TableRow row;
        TableCell cell;
        HyperLink link;
        bool alternateRow = false;
        int rowCounter = 0;

        if (player == null)
        {
            throw new ArgumentNullException("player");
        }

        UnitsDisplayTableHelper.AddUnitTypesToHeaderRow(player.Realm.ID, tblUnits.Rows[0], player.Realm.GetUnitTypes());


        try
        {

            Village vil;
            Fbg.Bll.VillageBase vilBase;
            for (int i = pager_pageLowerBoundItemIndex; i <= pager_pageUpperBoundItemIndex; i++)
            {
                vilBase = villages[i];
                //
                // are we over page size?
                //
                if (rowCounter >= pager_pageSize)
                {
                    break;
                }
                rowCounter++;
                vil = _player.Village(vilBase.id);

                row = new TableRow();
                if (alternateRow)
                {
                    row.CssClass = "DataRowAlternate highlight";
                    alternateRow = false;
                }
                else
                {
                    row.CssClass = "DataRowNormal highlight";
                    alternateRow = true;
                }


                cell = new TableCell();

                link = new HyperLink();
                link.CssClass = IsMobileOrD2 ? "" : "jsV";
                link.Attributes["rel"] = vil.id.ToString();
                link.Attributes["x"] = vil.Cordinates.X.ToString();
                link.Attributes["y"] = vil.Cordinates.Y.ToString();
                link.Attributes["vid"] = vil.id.ToString();
                link.Attributes["opid"] = vil.owner.ID.ToString() ;
                link.NavigateUrl = IsMobileOrD2 ? "" : NavigationHelper.VillageOverview_Tilda(vil.id);
                link.Text = String.Format("{0}({1},{2})", vil.name, vil.Cordinates.X, vil.Cordinates.Y);
                cell.Controls.Add(link);
                AddIncoming(cell, vil);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Right;
                cell.Text = Utils.FormatCost(vil.Points);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Right;
                cell.Text = Utils.FormatCost(vil.coins);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Right;
                cell.Text = Utils.FormatCost(vil.RemainingPopulation);
                row.Cells.Add(cell);

                PopulateUnitsTable(vil, row);

                tblUnits.Rows.Add(row);
            }

        }
        catch (Exception ex)
        {
            #region Collect error info
            BaseApplicationException be = new BaseApplicationException("Error populating list of villages", ex);
            be.AdditionalInformation.Add("_player.ID", player.ID.ToString());
            try
            {
                if (villages != null)
                {
                    foreach (Fbg.Bll.Village vil in villages)
                    {
                        vil.SerializeToNameValueCollection(be.AdditionalInformation);
                    }
                }
            }
            catch (Exception e2)
            {
                be.AdditionalInformation.Add("Error when serializing villages. msg:", e2.Message);
            }
            throw be;
            #endregion
        }


    }

    void PopulateUnitsTable(Fbg.Bll.Village village, TableRow row)
    {

        TimeSpan timeRem = new TimeSpan(0);
        Label lbl;
        TableCell cell;
        Panel pnl;
        int count;
        UnitInVillage uiv;
        foreach (Fbg.Bll.UnitType ut in _player.Realm.GetUnitTypes())
        {
            uiv = village.GetVillageUnit(ut);
            cell = new TableCell();
            cell.CssClass = "uc"; // "UnitCount"

            if (uiv.CurrentlyRecruiting != 0)
            {
                lbl = new Label();
                lbl.Text = "(+" + Utils.FormatCost(uiv.CurrentlyRecruiting) + ") ";
                lbl.CssClass = "zuc";
                lbl.Style.Add("float", "left");
                cell.Controls.Add(lbl);
            }


            cell.HorizontalAlign = HorizontalAlign.Right;


            SetUnitCount(village.GetVillageUnit(ut).TotalNowInVillageCount, "uc ttl", cell);
            SetUnitCount(village.GetVillageUnit(ut).YourUnitsCurrentlyInVillageCount, "uc ycur", cell);
            SetUnitCount(village.GetVillageUnit(ut).YourUnitsTotalCount, "uc yttl", cell);
            SetUnitCount(village.GetVillageUnit(ut).SupportCount, "uc sup", cell);
            //lbl = new Label();
            //count = village.GetVillageUnit(ut).TotalNowInVillageCount;
            //lbl.CssClass = "uc_ttl";
            //lbl.Text = Utils.FormatCost(count);
            //if (count == 0)
            //{
            //    lbl.CssClass += " zuc";
            //}
            //cell.Controls.Add(lbl);

            row.Cells.Add(cell);
        }
    }
    #endregion 

    void SetUnitCount(int count, string countTypeID, TableCell cell)
    {
        Label lbl = new Label();
        lbl.CssClass = countTypeID;
        lbl.Text = Utils.FormatCost(count);
        if (count == 0)
        {
            lbl.CssClass += " zuc";
        }
        cell.Controls.Add(lbl);
    }

    private void PopulateVillagesSummaryTable_ListVillagesOnly(Fbg.Bll.Player player)
    {
        TableRow row;
        TableCell cell;
        HyperLink link;
        bool alternateRow = false;
        int rowCounter = 0;

        if (player == null)
        {
            throw new ArgumentNullException("player");
        }


        try
        {

            VillageBasicB vil;
            Fbg.Bll.VillageBase vilBase;
            for (int i = pager_pageLowerBoundItemIndex; i <= pager_pageUpperBoundItemIndex; i++)
            {
                vilBase = villages[i];
                //
                // are we over page size?
                //
                if (rowCounter >= pager_pageSize)
                {
                    break;
                }
                rowCounter++;
                vil = _player.VillageBasicB(vilBase.id);

                row = new TableRow();
                if (alternateRow)
                {
                    row.CssClass = "DataRowAlternate highlight";
                    alternateRow = false;
                }
                else
                {
                    row.CssClass = "DataRowNormal highlight";
                    alternateRow = true;
                }


                cell = new TableCell();
                link = new HyperLink();
                link.NavigateUrl = NavigationHelper.VillageOverview_Tilda(vil.id);
                link.Text = String.Format("{0}({1},{2})", vil.name, vil.Cordinates.X, vil.Cordinates.Y);
                cell.Controls.Add(link);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Right;
                cell.Text = Utils.FormatCost(vil.Points);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Right;
                cell.Text = Utils.FormatCost(vil.coins);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Right;
                cell.Text = Utils.FormatCost(vil.RemainingPopulation);
                row.Cells.Add(cell);


                tblVillageList.Rows.Add(row);
            }
        }
        catch (Exception ex)
        {
            #region Collect error info
            BaseApplicationException be = new BaseApplicationException("Error populating list of villages", ex);
            be.AdditionalInformation.Add("_player.ID", player.ID.ToString());
            try
            {
                if (villages != null)
                {
                    foreach (Fbg.Bll.Village vil in villages)
                    {
                        vil.SerializeToNameValueCollection(be.AdditionalInformation);
                    }
                }
            }
            catch (Exception e2)
            {
                be.AdditionalInformation.Add("Error when serializing villages. msg:", e2.Message);
            }
            throw be;
            #endregion
        }
    }

    private void PopulateVillagesSummaryTable_Tags(Fbg.Bll.Player player)
    {
        TableRow row;
        TableCell cell;
        HyperLink link;
        bool alternateRow = false;
        int rowCounter = 0;

        if (player == null)
        {
            throw new ArgumentNullException("player");
        }


        try
        {

            VillageBasicB vil;
            Fbg.Bll.VillageBase vilBase;
            for (int i = pager_pageLowerBoundItemIndex; i <= pager_pageUpperBoundItemIndex; i++)
            {
                vilBase = villages[i];
                //
                // are we over page size?
                //
                if (rowCounter >= pager_pageSize)
                {
                    break;
                }
                rowCounter++;
                vil = _player.VillageBasicB(vilBase.id);

                row = new TableRow();
                if (alternateRow)
                {
                    row.CssClass = "DataRowAlternate highlight";
                    alternateRow = false;
                }
                else
                {
                    row.CssClass = "DataRowNormal highlight";
                    alternateRow = true;
                }


                cell = new TableCell();
                link = new HyperLink();
                link.NavigateUrl = NavigationHelper.VillageOverview_Tilda(vil.id);
                link.Text = String.Format("{0}({1},{2})", vil.name, vil.Cordinates.X, vil.Cordinates.Y);
                cell.Controls.Add(link);
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.HorizontalAlign = HorizontalAlign.Right;
                
                var tags = player.Tags;

                cell.Text = String.Join("", tags.Select(
                    t => "<a class=\"" + (vil.HasTag(t.ID) ? "on" : "off") + " tag\"" +
                         "vilid=\"" + vil.id + "\" tagid=" + t.ID + ">" + t.Name + "</a> "
                    ).ToArray());
                row.Cells.Add(cell);

                tblTags.Rows.Add(row);
            }
        }
        catch (Exception ex)
        {
            #region Collect error info
            BaseApplicationException be = new BaseApplicationException("Error populating list of villages", ex);
            be.AdditionalInformation.Add("_player.ID", player.ID.ToString());
            try
            {
                if (villages != null)
                {
                    foreach (Fbg.Bll.Village vil in villages)
                    {
                        vil.SerializeToNameValueCollection(be.AdditionalInformation);
                    }
                }
            }
            catch (Exception e2)
            {
                be.AdditionalInformation.Add("Error when serializing villages. msg:", e2.Message);
            }
            throw be;
            #endregion
        }
    }


    private bool IsVillageInNoTransportList(int villageID)
    {
        if (dtNoTransportList == null)
        {
            dtNoTransportList =  Fbg.Bll.CoinTransport.GetNoTransportCoinsVillages(_player).Tables[Fbg.Bll.CoinTransport.CONSTS.NoQTransportCoinsTableIndex.NoVillages];
        }
        foreach (DataRow dr in  dtNoTransportList.Rows) 
        {
            if (villageID == (int) dr[Fbg.Bll.CoinTransport.CONSTS.YesVillagesCloumnIndex.VillageID])
            {
                return true;
            }
        }
        return false;
    }

    private void AddIncoming(TableCell cell, Village vil)
    {
        DataTable dt = vil.GetIncomingUnits();
        Image img;

        int attackCount=0;
        int supportCount = 0;
        int first = 0; // 1 means an attack is first, 2 means  support if first
        foreach (DataRow dr in dt.Rows) 
        {
            if ((Fbg.Common.UnitCommand.CommandType)(short)dr[UnitMovements.CONSTS.IncomingTroops.ColNames.CommandType] 
                == Fbg.Common.UnitCommand.CommandType.Attack)
            {
                if (first == 0) { first = 1; }
                attackCount++;
            }
            else if ((Fbg.Common.UnitCommand.CommandType)(short)dr[UnitMovements.CONSTS.IncomingTroops.ColNames.CommandType]
                == Fbg.Common.UnitCommand.CommandType.Support)
            {
                if (first == 0) { first = 2; }
                supportCount++;
            }

        }
        HyperLink linkAttacks=null;
        HyperLink linkSupport=null;

        if (attackCount != 0)
        {
            linkAttacks = new HyperLink();
            linkAttacks.ImageUrl = Utils.CommandTypeImage(Fbg.Common.UnitCommand.CommandType.Attack);
            linkAttacks.Text = String.Format("{0} attack{1} incoming", attackCount.ToString() ,attackCount > 1 ? "s":"");
            linkAttacks.NavigateUrl = NavigationHelper.TroopsIncoming(vil.id);
            linkAttacks.CssClass = "smlIcon";
        }
        if (supportCount != 0)
        {
            linkSupport = new HyperLink();
            linkSupport.ImageUrl = Utils.CommandTypeImage(Fbg.Common.UnitCommand.CommandType.Support);
            linkSupport.Text = String.Format("{0} support{1} incoming", supportCount.ToString(), supportCount > 1 ? "s" : "");
            linkSupport.NavigateUrl = NavigationHelper.TroopsIncoming(vil.id);
            linkSupport.CssClass = "smlIcon";
        }

        if (first == 1)
        {
            if (linkAttacks != null)
            {
                cell.Controls.Add(linkAttacks);
            }
            if (linkSupport != null)
            {
                cell.Controls.Add(linkSupport);
            }
        } 
        else if (first == 2 ) 
        {
            if (linkSupport != null)
            {
                cell.Controls.Add(linkSupport);
            }
            if (linkAttacks != null)
            {
                cell.Controls.Add(linkAttacks);
            }
        }
    }

    protected void linkPager_Prev_Click(object sender, EventArgs e)
    {
        pager_pageIndex--;
        ViewState["pager_pageIndex"] = pager_pageIndex.ToString();
        Pager_CalculatePaging();
        PopulateVillagesSummaryTable(_player);

    }
    protected void linkPage_Next_Click(object sender, EventArgs e)
    {
        pager_pageIndex++;
        ViewState["pager_pageIndex"] = pager_pageIndex.ToString();
        Pager_CalculatePaging();
        PopulateVillagesSummaryTable(_player);
    }
    protected void linkPager_FirstPage_Click(object sender, EventArgs e)
    {
        pager_pageIndex = 0;
        ViewState["pager_pageIndex"] = pager_pageIndex.ToString();
        Pager_CalculatePaging();
        PopulateVillagesSummaryTable(_player);
    }
    protected void linkPager_LastPage_Click(object sender, EventArgs e)
    {
        pager_pageIndex = pager_numOfPages -1;
        ViewState["pager_pageIndex"] = pager_pageIndex.ToString();
        Pager_CalculatePaging();
        PopulateVillagesSummaryTable(_player);
 
    }

    protected void Pager_SetPageSize(int pageSize)
    {
        if (pager_pageSizeCookie == null)
        {
            pager_pageSizeCookie = new HttpCookie(CONSTS.Cookies.SummaryPagesPageSize);
        }
        pager_pageSizeCookie.Values.Remove("r" + _player.Realm.ID);
        pager_pageSizeCookie.Values.Add("r" + _player.Realm.ID, pageSize.ToString());
        Response.Cookies.Add(pager_pageSizeCookie);

        pager_pageSize = pageSize;
        ViewState["pager_pageIndex"] = "0"; // reset to 1st page on page size change. 

        Pager_CalculatePaging();
        PopulateVillagesSummaryTable(_player);
    }


    protected void pager_PageSize_Command(object sender, CommandEventArgs e)
    {
        Pager_SetPageSize(Convert.ToInt32(e.CommandArgument));
    }
    protected void btnGetChests_Click(object sender, EventArgs e)
    {
        int indexOfChVil;
        int villageID;
        foreach (string key in Request.Form.AllKeys)
        {
            if (key != null)
            {
                indexOfChVil = key.IndexOf("ckVid");
                if (indexOfChVil != -1)
                {
                    villageID = Convert.ToInt32(key.Substring(indexOfChVil + 5, key.Length - indexOfChVil - 5));
                    Village village = _player.Village(villageID);
                    _player.BuyChests(village.coins / chestCost, village);
                }
            }
        }

        PopulateVillagesSummaryTable(_player);
    }

 }

