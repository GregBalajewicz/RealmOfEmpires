using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fbg.Bll;
using System.Text;
using Gmbc.Common.Diagnostics.ExceptionManagement;

public partial class templates_BuildingTempl2 : TemplatePage
{
    public Realm realm;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(Request.QueryString["rid"]))
        {
            throw new ArgumentException("rid expected in Request.QueryString");
        }

        realm = Realms.Realm(Convert.ToInt32(Request.QueryString["rid"]));

        loadBuildingObjects();
        loadUnitObjects();
    }

    public templates_BuildingTempl2() {
        R_OverridePageName = "BuildingTempl2.aspx";
    }



    protected void loadBuildingObjects()
    {
        StringBuilder html = new StringBuilder("");
        string buildingRequirementString = "";
        int IDCount = 1;
        Realm r = realm;

        foreach (BuildingType bt in realm.BuildingTypes) {
            try {

                html.Append(String.Format("<div class='oneBuildingHelp' data-bid='{0}'>", bt.ID));
                html.Append(Utils.ChangeLineBreaks(Utils.ClearHTMLCode(R_MiscMessages.GetString("Building" + bt.ID))));

                HelpHelper.Buildings.ConstructTableWithLevels(html, r, bt, 3);
                IDCount += 1;
                html.Append("</div>");
            }
            catch (Exception ex) {
                BaseApplicationException be = new BaseApplicationException("Error in loadBuildingObjects() method", ex);
                be.AddAdditionalInformation("html", html);
                be.AddAdditionalInformation("buildingRequirementString", buildingRequirementString);
                be.AddAdditionalInformation("IDCount", IDCount);
                throw be;
            }
        }

        Label1.Text = html.ToString();
    }


    private void loadUnitObjects()
    {
        StringBuilder html;
        int IDCount = 1;
        string RecruitReqString = "";
        TableRow row;
        TableCell cell;
        Realm r = FbgPlayer.Realm;
        Label lbl;



        try {
            Dictionary<BuildingType, List<Fbg.Bll.UnitType>> unitsByBT = FbgPlayer.Realm.GetUnitTypesByBuildingType();
            List<Fbg.Bll.UnitType> unitsInThisBuilding;

            foreach (BuildingType bt in realm.BuildingTypes) {

                if (unitsByBT.TryGetValue(bt, out unitsInThisBuilding)) {

                    foreach (Fbg.Bll.UnitType ut in unitsInThisBuilding) {
                        row = new TableRow();
                        row.Attributes.Add("data-bid", bt.ID.ToString());
                        row.CssClass = "oneUnitHelp";
                        #region Build the help for this unit
                        cell = new TableCell();
                        cell.VerticalAlign = VerticalAlign.Top;
                        lbl = new Label();
                        html = new StringBuilder(5000);
                        row.Cells.Add(cell);

                        HelpHelper.Units.ConstructTableWithUnitsStatsD2(html, r, ut, true);

                        lbl.Text = html.ToString();
                        cell.Controls.Add(lbl);
                        row.Cells.Add(cell);
                        #endregion
                        tblUnitHelp.Rows.Add(row);
                    }
                }
            }

        }
        catch (Exception ex) {
            BaseApplicationException be = new BaseApplicationException("Error in loadUnitObjects() method", ex);
            be.AddAdditionalInformation("RecruitReqString", RecruitReqString);
            be.AddAdditionalInformation("IDCount", IDCount);
            throw be;
        }
    }

}