


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
using Fbg.Bll;

/// <summary>
/// Summary description for UnitsDisplayTableHelper
/// </summary>
public static class BuildingDisplayTableHelper
{
    public static void AddBuildingTypesToHeaderRow(int realmID, TableRow rowHeader, List<Fbg.Bll.BuildingType> buildingTypes)
    {
        TableCell cell;
        HyperLink link;
        foreach (Fbg.Bll.BuildingType bt in buildingTypes)
        {
            cell = new TableCell();
            cell.HorizontalAlign = HorizontalAlign.Center;

            link = new HyperLink();
            link.ImageUrl = VillageOverviewImages.GetBuildingIconUrl(bt);
            link.NavigateUrl = NavigationHelper.BuildingsHelp(realmID, bt.ID);
            link.ToolTip = bt.Name;
            cell.Controls.Add(link);
            rowHeader.Cells.Add(cell);
        }
    }

    //public static void GetZeroedCells(ref TableCell[] cells, List<Fbg.Bll.UnitType> unitTypes)
    //{
    //    cells = new TableCell[unitTypes.Count];

    //    for (int i = 0; i < cells.Length; i++)
    //    {
    //        cells[i] = new TableCell();
    //        cells[i].Text = "0";
    //        cells[i].HorizontalAlign = HorizontalAlign.Right;
    //        cells[i].CssClass = "ZeroUnitCount";
    //    }
    //}

    //public static void SetUnitTypeCount(TableCell[] cells, int unitTypeID, int count, Realm realm)
    //{
    //    TableCell cell;
    //    cell = cells[realm.GetNormalizedUnitTypeLocation(unitTypeID)];
    //    cell.Text = count.ToString("#,###0");
    //    cell.HorizontalAlign = HorizontalAlign.Right;
    //    cell.CssClass = String.Empty;
    //}
}
