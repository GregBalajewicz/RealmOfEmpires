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
public static class UnitsDisplayTableHelper
{
    public static void AddUnitTypesToHeaderRow(int realmID, TableRow rowHeader, List<Fbg.Bll.UnitType> unitTypes)
    {
        TableHeaderCell cell;
        HyperLink link;
        foreach (Fbg.Bll.UnitType ut in unitTypes)
        {
            cell = new TableHeaderCell();
            cell.HorizontalAlign = HorizontalAlign.Center;

            link = new HyperLink();
            link.ImageUrl = ut.IconUrl;            
            link.CssClass = "i20x16";
            //link.NavigateUrl = NavigationHelper.UnitHelp(realmID, ut.ID);
            link.ToolTip = ut.Name;
            link.Text = ut.Name;
            cell.Controls.Add(link);
            rowHeader.Cells.Add(cell);
        }
    }
    //public static void AddUnitTypesToHeaderRow(int realmID, TableHeaderRow rowHeader, List<Fbg.Bll.UnitType> unitTypes)
    //{
    //    TableHeaderCell cell;
    //    HyperLink link;
    //    foreach (Fbg.Bll.UnitType ut in unitTypes)
    //    {
    //        cell = new TableHeaderCell();
    //        cell.HorizontalAlign = HorizontalAlign.Center;
    //        cell.Scope = TableHeaderScope.Column;

    //        link = new HyperLink();
    //        link.ImageUrl = ut.IconUrl;
    //        link.NavigateUrl = NavigationHelper.UnitHelp(realmID, ut.ID);
    //        link.ToolTip = ut.Name;
    //        link.Text = ut.Name;
    //        cell.Controls.Add(link);
    //        rowHeader.Cells.Add(cell);
    //    }
    //}



    public static void GetZeroedCells(ref TableCell[] cells, List<Fbg.Bll.UnitType> unitTypes)
    {
        cells = new TableCell[unitTypes.Count];

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = new TableCell();
            cells[i].Text = "0";
            cells[i].HorizontalAlign = HorizontalAlign.Right;
            cells[i].CssClass = "ZeroUnitCount";
        }
    }

    public static void SetUnitTypeCount(TableCell[] cells, int unitTypeID, int count, Realm realm, bool isMobile)
    {
        TableCell cell;        
        cell = cells[realm.GetNormalizedUnitTypeLocation(unitTypeID)];
        cell.Attributes.Add("data-cnt", count.ToString());
        cell.Controls.Add(new HtmlGenericControl("span") { InnerText= isMobile ?  Utils.FormatShortNum(count) : count.ToString("#,###0")});
        cell.HorizontalAlign = HorizontalAlign.Right;
        cell.CssClass = String.Empty;
        cell.Attributes.Add("uid",unitTypeID.ToString());
    }
}
