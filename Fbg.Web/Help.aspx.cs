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
using System.Text;
using Gmbc.Common.Diagnostics.ExceptionManagement;
using System.Collections.Generic;

public partial class Help : MyCanvasIFrameBasePage
{
    int realmID = 1;
    Realm r;
    int expandID = 0;

    new protected void Page_Load(object sender, EventArgs e)
         {
        try
        {
            base.Page_Load(sender, e);

            MasterBase_Main mainMasterPage = (MasterBase_Main)this.Master;
            mainMasterPage.Initialize(FbgPlayer, MyVillages);

            //get Current RealmID from QueryString
            if ((Request.QueryString[CONSTS.QuerryString.RealmID] != null && Request.QueryString[CONSTS.QuerryString.RealmID] != ""))
            {
                realmID = Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.RealmID]);
            }
            else
            {
                realmID = FbgPlayer.Realm.ID;
            }

            tblUnitOverview.Visible = false;

            //get the current Realm Name
            r = Realms.Realm(realmID);

            #region localzing some controls
            //
            // this is needed for localiztion, so that <%# ... %> work
            //
            this.HCellBlank.DataBind();
            this.HCellDefense.DataBind();

            #endregion

            //checking whether Query String has PageName[Building/Unit] and ObjectID or not
            if ((Request.QueryString[CONSTS.QuerryString.HelpObjectID] != null && Request.QueryString[CONSTS.QuerryString.HelpObjectID] != ""))
            {
                expandID = Convert.ToInt32(Request.QueryString[CONSTS.QuerryString.HelpObjectID].ToString());
            }
            else
            {
                expandID = 0;
            }
            if ((Request.QueryString[CONSTS.QuerryString.HelpPageType] != null && Request.QueryString[CONSTS.QuerryString.HelpPageType] != ""))
            {
                if (Request.QueryString[CONSTS.QuerryString.HelpPageType].ToString().Trim().ToLower() == "building")
                {
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "AddBuildings", "<script type='text/javascript' language='javascript'>var div=document.getElementById('DivMain');div.innerHTML=\"<table cellspacing=1 class='TypicalTable stripeTable ' GridLines='Both'>" + loadBuildingObjects() + "</table>\";</script>");
                }
                else if (Request.QueryString[CONSTS.QuerryString.HelpPageType].ToString().Trim().ToLower() == "unit")
                {
                    //Img0.Attributes.Add("onClick", "ExpandCollapse(); return false;");
                    tblUnitOverview.Visible = true;
                    loadUnitOverview();
                    lbl_MainTable.Text = "<table  cellspacing=1 class='TypicalTable stripeTable'>" + loadUnitObjects() + "</table>";
                    //Page.ClientScript.RegisterStartupScript(Page.GetType(), "AddUnits", "<script type='text/javascript' language='javascript'>var div=document.getElementById('DivMain');div.innerHTML=\"<table cellspacing=1 class='TypicalTable stripeTable'>" + loadUnitObjects() + "</table>\";</script>");
                }
                else
                {
                    throw new ArgumentException("Unrecognized PageType param:" + Request.QueryString[CONSTS.QuerryString.HelpPageType].ToString());
                }
            }
            else
            {
                lbl_MainTable.Text = "<table  cellspacing=1 class='TypicalTable stripeTable'>" + loadBuildingObjects() + "</table>";
            }
        }
        catch (Exception ex)
        {
            BaseApplicationException be = new BaseApplicationException("Error in Page_Load() method", ex);
            be.AddAdditionalInformation("RealmID", realmID);
            be.AddAdditionalInformation("ExpandID", expandID);
            throw be;
        }
    }

    protected void lnkBuilding_Click(object sender, EventArgs e)
    {
        Response.Redirect("help.aspx?rid=" + realmID + "&HPN=Building");
    }

    protected void lnkUnit_Click(object sender, EventArgs e)
    {
        Response.Redirect("help.aspx?rid=" + realmID + "&HPN=Unit");
    }

    protected String loadBuildingObjects()
    {
        StringBuilder tableBody = new StringBuilder("");
        string buildingRequirementString = "";
        int IDCount = 1;

        try
        {
            foreach (BuildingType bt in r.BuildingTypes)
            {
                if (bt.ID == expandID)
                {
                    tableBody.Append("<tr><td colspan='6' class='Sectionheader'><img src='https://static.realmofempires.com/images/collapse_button.gif' alt='Button Image' onClick='javascript:checkExpandCollapse(");
                    tableBody.Append(IDCount);
                    tableBody.Append(");' Id='Img");
                    tableBody.Append(IDCount);
                    tableBody.Append("'/>&nbsp;");
                }
                else
                {
                    tableBody.Append("<tr><td colspan='6' class='Sectionheader'><img src='https://static.realmofempires.com/images/expand_button.gif' alt='Button Image' onClick='javascript:checkExpandCollapse(");
                    tableBody.Append(IDCount);
                    tableBody.Append(");' Id='Img");
                    tableBody.Append(IDCount);
                    tableBody.Append("'/>&nbsp;<strong>");
                }
                //buildingIcon.ImageUrl = VillageOverviewImages.GetBuildingIconUrl(bt);
                tableBody.Append("<img src='" + VillageOverviewImages.GetBuildingIconUrl(bt).Replace("~/", "") + "' />&nbsp;");
                tableBody.Append(bt.Name);
                tableBody.Append("</strong></td></tr>");
                if (bt.ID == expandID)
                {
                    tableBody.Append("<tbody id='Tbody");
                    tableBody.Append(IDCount);
                    tableBody.Append("' style='display:table-row-group'>");
                }
                else
                {
                    tableBody.Append("<tbody id='Tbody");
                    tableBody.Append(IDCount);
                    tableBody.Append("' style='display:none'>");
                }
                tableBody.Append("<tr><td colspan='6'><table width='100%'><tr><td><img src='");
                tableBody.Append(VillageOverviewImages.GetBuildingImageForHelpScreen(bt.ID).Replace("~/",""));
                tableBody.Append("' /></td><td valign='top'>");
                tableBody.Append(Utils.ChangeLineBreaks(Utils.ClearHTMLCode(R_MiscMessages.GetString("Building" + bt.ID))));
                tableBody.Append("</td></table></td></tr>");

                HelpHelper.Buildings.ConstructTableWithLevels(tableBody, r, bt, isMobile);
                IDCount += 1;
            }

            tableBody.Append("<tr><td colspan='6'></td></tr>");
            return tableBody.ToString();
        }
        catch (Exception ex)
        {
            BaseApplicationException be = new BaseApplicationException("Error in loadBuildingObjects() method", ex);
            be.AddAdditionalInformation("TableBody", tableBody);
            be.AddAdditionalInformation("buildingRequirementString", buildingRequirementString);
            be.AddAdditionalInformation("IDCount", IDCount);
            be.AddAdditionalInformation("Realm", r);
            throw be;
        }
    }

    private void loadUnitOverview()
    {
        bool isFirstTime = true;
        bool alternateRow = false;

        try
        {
            TableCell tcell;
            tcell = new TableCell();
            tcell.Font.Bold = true;
            tcell.Text = "";
            HeaderRow1.Cells.Add(tcell);

            tcell = new TableCell();
            tcell.Font.Bold = true;
            tcell.Text = RS("Cost");
            HeaderRow1.Cells.Add(tcell);

            tcell = new TableCell();
            tcell.Font.Bold = true;
            tcell.Text = RS("Food");
            HeaderRow1.Cells.Add(tcell);

            tcell = new TableCell();
            tcell.Font.Bold = true;
            tcell.Text = RS("Speed") + "<br/>(squares/hour)";
            HeaderRow1.Cells.Add(tcell);

            tcell = new TableCell();
            tcell.Font.Bold = true;
            tcell.Text = RS("CarrySilver");
            HeaderRow1.Cells.Add(tcell);

            tcell = new TableCell();
            tcell.Font.Bold = true;
            tcell.Text = RS("AttackStrength");
            HeaderRow1.Cells.Add(tcell);

            foreach (Fbg.Bll.UnitType ut in r.GetUnitTypes())
            {
                TableRow tr = new TableRow();
                TableCell tc;

                if (alternateRow)
                {
                    tr.CssClass = "DataRowAlternate highlight";
                    alternateRow = false;
                }
                else
                {
                    tr.CssClass = "DataRowNormal highlight";
                    alternateRow = true;
                }

                Image img = new Image();
                Label lbl = new Label();

                tc = new TableCell();
                tc.HorizontalAlign = HorizontalAlign.Right;
                img.ImageUrl = ut.IconUrl;
                lbl.Text = ut.Name;
                tc.HorizontalAlign = HorizontalAlign.Left;
                tc.Controls.Add(img);
                tc.Controls.Add(lbl);
                tr.Cells.Add(tc);

                tc = new TableCell();
                tc.HorizontalAlign = HorizontalAlign.Right;
                tc.Text = Utils.FormatCost(ut.Cost(null));
                tc.CssClass = "help";
                tc.Attributes.Add("rel", "s_UnitsCost");
                tr.Cells.Add(tc);

                tc = new TableCell();
                tc.HorizontalAlign = HorizontalAlign.Right;
                tc.Text = ut.Pop.ToString();
                tc.CssClass = "help";
                tc.Attributes.Add("rel", "s_UnitsFood");
                tr.Cells.Add(tc);

                tc = new TableCell();
                tc.HorizontalAlign = HorizontalAlign.Right;
                tc.Text = ut.Speed.ToString();
                tc.CssClass = "help";
                tc.Attributes.Add("rel", "s_UnitsMovementSpeed");
                tr.Cells.Add(tc);

                tc = new TableCell();
                tc.HorizontalAlign = HorizontalAlign.Right;
                tc.Text = ut.CarryCapacity.ToString();
                tc.CssClass = "help";
                tc.Attributes.Add("rel", "s_UnitsCarry");
                tr.Cells.Add(tc);

                tc = new TableCell();
                tc.HorizontalAlign = HorizontalAlign.Right;
                if (ut.AttackStrength == 0)
                {
                    tc.CssClass = "UnavailableUnit";
                    tc.Text = "n/a";
                }
                else
                {
                    tc.Text = ut.AttackStrength.ToString();
                }
                tc.CssClass = "help";
                tc.Attributes.Add("rel", "s_UnitsAttackStrength");            
                tr.Cells.Add(tc);

                //Showing Defense Strength of the current UnitType
                foreach (Fbg.Bll.UnitType uType in r.GetUnitTypes())
                {
                    if (uType.AttackStrength > 0)
                    {
                        if (isFirstTime)
                        {
                            Image img1 = new Image();
                            tc = new TableCell();
                            tc.Font.Bold = true;
                            img1.AlternateText = uType.Name;
                            img1.ImageUrl = uType.IconUrl;
                            tc.Controls.Add(img1);
                            HeaderRow1.Cells.Add(tc);
                        }
                        tc = new TableCell();
                        tc.HorizontalAlign = HorizontalAlign.Right;
                        tc.Text = ut.DefenseStrength[uType].ToString();
                        tc.CssClass = "help";
                        tc.Attributes.Add("rel", "s_UnitsDefenseStrength");            

                        tr.Cells.Add(tc);
                    }
                }
                if (isFirstTime)
                {
                    HCellDefense.ColumnSpan = r.GetUnitTypes().Count;
                }
                isFirstTime = false;


                tblUnitOverview.Rows.Add(tr);
            }
        }
        catch(Exception ex)
        {
            BaseApplicationException be = new BaseApplicationException("Error in loadUnitOverview", ex);
            be.AddAdditionalInformation("IsFirstTime", isFirstTime);
            be.AddAdditionalInformation("AlternateRow", alternateRow);
            throw be;
        }

    }

  


    private String loadUnitObjects()
    {
        StringBuilder tableBody = new StringBuilder("");
        int IDCount = 1;
        string RecruitReqString = "";

        try
        {
            foreach (Fbg.Bll.UnitType ut in r.GetUnitTypes())
            {
                if (ut.ID == expandID)
                {
                    tableBody.Append("<tr><td colspan='2' class='Sectionheader'><img src='https://static.realmofempires.com/images/collapse_button.gif' alt='Button Image' onClick='javascript:checkExpandCollapse(" + IDCount + ");' Id='Img");
                    tableBody.Append(IDCount);
                    tableBody.Append("'/>&nbsp;");
                }
                else
                {
                    tableBody.Append("<tr><td colspan='2' class='Sectionheader'><img src='https://static.realmofempires.com/images/expand_button.gif' alt='Button Image' onClick='javascript:checkExpandCollapse(");
                    tableBody.Append(IDCount);
                    tableBody.Append(");' Id='Img");
                    tableBody.Append(IDCount);
                    tableBody.Append("'/>&nbsp;");
                }
                tableBody.Append("<img src='");
                tableBody.Append(ut.IconUrl.Replace("~/", ""));
                tableBody.Append("' />&nbsp;<strong>");
                tableBody.Append(ut.Name);

                tableBody.Append("</strong></td></tr>");

                if (ut.ID == expandID)
                {
                    tableBody.Append("<tbody id='Tbody");
                    tableBody.Append(IDCount);
                    tableBody.Append("' style='display:table-row-group'>");
                }
                else
                {
                    tableBody.Append("<tbody id='Tbody");
                    tableBody.Append(IDCount);
                    tableBody.Append("' style='display:none'>");
                }

                tableBody.Append("<tr><td colspan='2'>");
                tableBody.Append(ut.Desc);
                tableBody.Append("</td></tr>");


                tableBody.Append("<tr ><td ><table colspacing=0 class='TypicalTable' width='50%'><tr><td><table colspacing=0 class='TypicalTable' ><tr class='highlight help' rel='s_UnitsCost' ><td class='TableHeaderRow'>" + RS("CostColon") + "&nbsp;</td><td align='right'>");//
                tableBody.Append(Utils.FormatCost(ut.Cost(null)));
                tableBody.Append("&nbsp;" + RS("PeicesOfSilver") + "</td></tr>");

                tableBody.Append("<tr class='highlight help' rel='s_UnitsRecruitTime'><td class='TableHeaderRow'>" + RS("RecruitTime") + "&nbsp;</td><td align='right'>");//
                tableBody.Append(Utils.FormatDuration(ut.RecruitmentTime()));
                tableBody.Append("</td></tr>");

                tableBody.Append("<tr class='highlight help' rel='s_UnitsAttackStrength'><td  class='TableHeaderRow'>" + RS("AttackStrengthColon") + "&nbsp;</td><td align='right'>");//
                if (ut.AttackStrength == 0)
                {
                    tableBody.Append("n/a");
                }
                else
                {
                    tableBody.Append(ut.AttackStrength.ToString());
                }
                tableBody.Append("</td></tr>");

                tableBody.Append("<tr class='highlight help' rel='s_UnitsDefenseStrength'><td class='TableHeaderRow' valign='top'><strong>" + RS("DefenseStrengthColon") + "</strong>&nbsp;</td><td align='right'>");

                // Table with defensive strength against other units
                HelpHelper.Units.ConstructTableWithDefensiveStrengths(tableBody, r, ut);

                tableBody.Append("<tr class='highlight help' rel='s_UnitsFood'><td class='TableHeaderRow'><strong>" + RS("FoodColon") + "</strong>&nbsp;</td><td align='right'>");
                tableBody.Append(ut.Pop.ToString());
                tableBody.Append("</td></tr>");

                tableBody.Append("<tr class='highlight help' rel='s_UnitsMovementSpeed'><td class='TableHeaderRow' nowrap>" + RS("MovementSpeed") + "&nbsp;</td><td nowrap align='right'>");
                tableBody.Append(ut.Speed.ToString());
                tableBody.Append("&nbsp;" + RS("SquaresPerHour") + "</td></tr>");

                tableBody.Append("<tr class='highlight help' rel='s_UnitsCarry'><td class='TableHeaderRow'>" + RS("Carry") + "&nbsp;</td><td align='right'>");
                tableBody.Append(ut.CarryCapacity.ToString());
                tableBody.Append("&nbsp;" + RS("PeicesOfSilver") + "</td></tr>");

                RecruitReqString = HelpHelper.Requirements(ut.Requirements, ut.Requirements_Research, isMobile ? 2: 1 );
                if (RecruitReqString != string.Empty)
                {
                    tableBody.Append("<tr class='highlight'><td class='TableHeaderRow'>" + RS("Requirements") + "&nbsp;</td><td align='right'>");
                    tableBody.Append(RecruitReqString);
                    tableBody.Append("</td></tr>");
                }
                tableBody.Append("</table></td><td style='margin-left:10%'><img border=0 src='" + ut.Image + "' alt='Unit Image' align='Left' /></td></tr></table></td></tr></tbody>");

                IDCount += 1;
            }
            tableBody.Append("<tr><td colspan='2'></td></tr>");
            return tableBody.ToString();
        }
        catch (Exception ex)
        {
            BaseApplicationException be = new BaseApplicationException("Error in loadUnitObjects() method", ex);
            be.AddAdditionalInformation("TableBody", tableBody);
            be.AddAdditionalInformation("RecruitReqString", RecruitReqString);
            be.AddAdditionalInformation("IDCount", IDCount);
            be.AddAdditionalInformation("Realm", r);
            throw be;
        }
    }

        protected override void OnPreInit(EventArgs e)
    {

        if (isMobile)
        {
            base.MasterPageFile = "masterMain_m.master";
        }
        base.OnPreInit(e);
    }

}