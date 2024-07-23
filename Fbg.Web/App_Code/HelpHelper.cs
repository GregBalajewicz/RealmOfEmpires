using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;
using Fbg.Bll;
using System.Collections.Generic;

/// <summary>
/// Summary description for HelpHelper
/// </summary>
public class HelpHelper
{

    public class Buildings
    {

        public static void ConstructTableWithLevels(StringBuilder tableBody, Realm r, BuildingType bt, bool isMobile)
        {
            ConstructTableWithLevels(tableBody, r, bt
                , isMobile ? 2: 1 );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="r"></param>
        /// <param name="bt"></param>
        /// <param name="displayType">1 - old desktop display, 2 - first version mobile, 3 new ThemeA mobile</param>
        public static void ConstructTableWithLevels(StringBuilder html, Realm r, BuildingType bt, int displayType)
        {
            bool isResearch = r.Research.IsResearchActive && r.Research.ResearchItemsForBuildingType(bt).Count>0;
            string buildingRequirementString;

            if (displayType == 2) {
                html.Append("<tr class='Sectionheader'><th ></th><TH></th></tr>");//6
            }
            else if (displayType == 1) {
                html.Append("<tr class='Sectionheader'><th width='220px'>&nbsp;Level</th><th width='45px'>&nbsp;Cost</th><th width='52px'>&nbsp;Time</th><th width='70px'>&nbsp;Food</th><th width='80px'>&nbsp;Points</th><th width='80px'>&nbsp;" + bt.Levels[0].EffectName + "</th></tr>");//6
            }
            else {
                html.Append("<table class=buildingLevels><tr class='Sectionheader'><th >" + Resources.global.Level + "</th><th >"+Resources.global.Cost+"</th><th >Time</th><th >"+Resources.global.Food+"</th><th >"+Resources.global.Pts+"</th><th >" + bt.Levels[0].EffectName + "</th></tr>");//6
            }

            foreach (BuildingTypeLevel level in bt.Levels)
            {
                if (displayType == 3) {
                    html.Append("<tr ><td nowrap>");
                }
                else {
                    html.Append("<tr class='DataRowAlternate highlight'><td nowrap>");

                }
                if (displayType == 1 && level.LevelName != String.Empty)
                {
                    html.Append("(Level ");
                    html.Append(level.Level.ToString());
                    html.Append(") - ");
                    html.Append(level.LevelName);
                }
                else if (displayType == 2) 
                {
                    html.Append("Level ");
                    html.Append(level.Level.ToString());
                }
                else {
                    html.Append(level.Level.ToString());
                }

                buildingRequirementString = BuildingRequirements(level, displayType);
                if (buildingRequirementString != string.Empty && displayType == 1)
                {
                    html.Append(displayType != 1 ? Resources.global.FirstNeed : " <br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Prerequisites:<br>");
                    html.Append(buildingRequirementString);
                }
                if (displayType == 2) {
                    #region mobile version
                    html.Append("</td><td align='left' class=levelParams >");

                    html.Append("<div>Cost : " + level.Cost.ToString("#,###"));
                    html.Append("</div>");


                    html.Append("<div>Upgrade Time : " + Utils.FormatDuration(level.BuildTime()));
                    html.Append("</div>");

                    html.Append("<div>");
                    if (level.PopulationCumulative == 0) {
                        html.Append("Food Needed : -");
                    }
                    else {
                            html.Append("Food Needed : ");
                            html.Append(level.Population.ToString("#,###0"));
                    }
                    html.Append("</div>");

                    html.Append("<div>Points : ");
                    html.Append(level.Points.ToString("#,###0"));
                    html.Append("</div>");

                    html.Append("<div>");
                    html.Append(level.EffectName + " : ");
                    html.Append(level.EffectFormatted );
                    html.Append("</div>");
                    html.Append("</td></tr>");
                    #endregion 
                }
                else {
                    html.Append("</td><td align='right' class='help' rel='s_BuildingsCost'>");

                    html.Append(level.Cost.ToString("#,###"));

                    html.Append("</td><td align='right' class='help' rel='s_BuildingsTime'>");

                    if (displayType == 3) {
                        html.Append(Utils.FormatDuration_Short(level.BuildTime()));
                    }
                    else {
                        html.Append(Utils.FormatDuration(level.BuildTime()));
                    }

                    if (level.PopulationCumulative == 0) {
                        html.Append("</td><td align='right' class='help UnavailableUnit' rel='s_BuildingsNoFood'>-");
                    }
                    else {
                        html.Append("</td><td align='right' class='help ' rel='s_BuildingsFood'>");
                        html.Append(level.Population.ToString("#,###0"));
                    }

                    html.Append("</td><td align='right' class='help' rel='s_BuildingsPoints'>");
                    html.Append(level.Points.ToString());

                    html.Append("</td><td align='right' class='help' rel='s_BuildingsEffect'>");
                    html.Append(level.EffectFormatted);
                    html.Append("</td></tr>");


                }
            }
            if (displayType == 1)
            {
                html.Append("<TR><TD colspan=6>"+Resources.global.Note1+"<BR></TD><tr>");
                if (isResearch) { html.Append("<TR><TD colspan=6><BR>" + Resources.global.Note1 + bt.Levels[0].EffectName + Resources.global.Note2 + "</TD><tr>"); }
            }

            html.Append("</td></tr></tbody>");

            if (displayType == 3) {
                html.Append("</table>");
            }

            if (displayType != 1)
            {
                html.Append("<TR><TD colspan=6>" + Resources.global.Note1 + "<BR></TD><tr>");
                if (isResearch) { html.Append("<BR>Note : " + bt.Levels[0].EffectName + " is based on 0% research bonus. Researching some items will improve this"); }
            }

        }

    }

    public class Units
    {

        public static void ConstructTableWithDefensiveStrengths(StringBuilder tableBody, Realm r, Fbg.Bll.UnitType ut)
        {
            tableBody.Append("<table width='50%' cellpadding='0' cellspacing='0'>");

            foreach (Fbg.Bll.UnitType uType in r.GetUnitTypes())
            {
                if (uType.AttackStrength > 0)
                {
                    tableBody.Append("<tr><td align='left'><img src='");
                    tableBody.Append(uType.IconUrl);
                    tableBody.Append("' alt='");
                    tableBody.Append(uType.Name);
                    tableBody.Append("' /></td><td>");
                    tableBody.Append(ut.DefenseStrength[uType].ToString());
                    tableBody.Append("</td></tr>");
                }

            }
            tableBody.Append("</table>");

        }


        public static void ConstructTableWithUnitsStatsD2(StringBuilder tableBody, Realm r, Fbg.Bll.UnitType ut, bool isMobile)
        {
            tableBody.Append("<div class='quickRecruitBtn smallRoundButtonDark' onclick='ROE.UI.Sounds.click(); ROE.QuickRecruit.showPopup(\"vov\",ROE.SVID);'><span></span></div>");
            #region construct the unit stats table structure
            tableBody.Append("<table cellpadding='0' cellspacing='0'>"); // TABLE #1
            tableBody.Append("<tr style='height:25px;'><td colspan='2' style='text-align:center; padding-right: 0;'>");
            tableBody.Append("<div class='memberDivider' style='margin-bottom: -6px; margin-top: 3px;'></div>");
            tableBody.AppendFormat("<img class='mainIcon' src='{0}' />&nbsp;<strong class='fontGoldNumbersXLrg'>{1}</strong>", ut.IconUrl.Replace("~/", "").Replace(".png", "_M.png"), ut.Name);
            tableBody.Append("<div class='memberDivider' style='margin-top: -10px;'></div>");
            tableBody.Append("</td></tr>");

            tableBody.Append("<tr>");
            tableBody.AppendFormat("<td style='padding-left:15px'><img border=0 src='{0}' alt='Unit Image' align='Left' /></td>", ut.Image);
            tableBody.Append("</tr>");

            tableBody.Append("<tr style='height:3px;'></tr>"); //divider

            tableBody.Append("<tr class='oneUnitDesc'><td>");
            tableBody.Append("<table cellpadding='0' cellspacing='0'>"); //TABLE #2
            tableBody.Append("<tr><td valign=top class=NoPad>");

            #region Unit Stats
            tableBody.Append("<table  cellpadding='0' cellspacing='1' class='TypicalTable stripeTable' ><tr class='highlight help' rel='s_UnitsCost' ><td class='tableHeader'>Costs:&nbsp;</td><td>");//
            tableBody.Append("<img class='imageIcon' src='https://static.realmofempires.com/images/icons/Silver44.png'/>");
            tableBody.Append(Utils.FormatCost(ut.Cost(null)));
            tableBody.Append("</td><td>");
            tableBody.Append("<img class='imageIcon' src='https://static.realmofempires.com/images/icons/Sheep44.png'/>");
            tableBody.Append(ut.Pop.ToString());
            tableBody.Append("</td><td colspan='2'>");
            tableBody.Append("<img class='imageIcon' src='https://static.realmofempires.com/images/HourGlass.png'/>");
            tableBody.Append(Utils.FormatDuration(ut.RecruitmentTime()));
            tableBody.Append("</td></tr>");

            tableBody.Append("<tr class='highlight help' rel='s_UnitsAttackStrength'><td  class='tableHeader'>Attack Strength:&nbsp;</td><td>");//
            if (ut.AttackStrength == 0)
            {
                tableBody.Append("n/a");
            }
            else
            {
                tableBody.Append(ut.AttackStrength.ToString());
            }
            tableBody.Append("</td></tr>");

            tableBody.Append("<tr class='highlight help' rel='s_UnitsDefenseStrength'><td class='tableHeader' valign='top'>Defense Strength:&nbsp;</td>");

            // Table with defensive strength against other units

            foreach (Fbg.Bll.UnitType uType in r.GetUnitTypes())
            {
                if (uType.AttackStrength > 0)
                {
                    tableBody.Append("<td><img class='imageIcon' src='");
                    tableBody.Append(uType.IconUrl);
                    tableBody.Append("' alt='");
                    tableBody.Append(uType.Name);
                    tableBody.Append("' />");
                    tableBody.Append(ut.DefenseStrength[uType].ToString());
                    tableBody.Append("</td>");
                }

            }

            tableBody.Append("</td></tr>");

            tableBody.Append("<tr class='highlight help' rel='s_UnitsMovementSpeed'><td class='tableHeader'>Carry/Movement:&nbsp;</td><td>");
            tableBody.Append("<img class='imageIcon' src='https://static.realmofempires.com/images/icons/Silver44.png'/>");
            tableBody.Append(ut.CarryCapacity.ToString());
            tableBody.Append("</td><td colspan='3'>");
            tableBody.Append(ut.Speed.ToString());
            tableBody.Append("&nbsp;squares per hour</td></tr>");

            tableBody.Append("<tr class='highlight'><td class='tableHeader' style='padding-top: 12px;'>Requirements:&nbsp;</td>");
            if (ut.Requirements_Research.Count > 0)
            {
                tableBody.AppendFormat("<td><div class='smallRoundButtonDark' onclick='ROE.UI.Sounds.click(); ROE.Research.showResearchPopup(\"u{0}\");'><img src='https://static.realmofempires.com/images/icons/M_ResearchList2.png'/>", ut.ID);
               // tableBody.Append("<img src='https://static.realmofempires.com/images/misc/M_SmGood.png'/></div></td>");
               //// tableBody.Append("<img src='https://static.realmofempires.com/images/misc/M_SmNotEnough.png'/></div></td>");
            }
            else
            {
                tableBody.Append("<td></td>");
            }
            
            
            foreach (BuildingTypeLevel requiredLevel in ut.Requirements)
            {
                tableBody.AppendFormat("<td colspan='3'><img src='{0}' style='width: 34px; height: 34px;'/> Level {1}</td>", r.BuildingTypesEntities[requiredLevel.Building.ID.ToString()].IconUrl_ThemeM, requiredLevel.Level.ToString());
            }
            tableBody.Append("</table>");
            #endregion

            tableBody.Append("</td>");
            tableBody.Append("</tr>");
            tableBody.Append("</table>"); //TABLE #2
            tableBody.Append("</td></tr>");

            tableBody.Append("<tr style='height:3px;'></tr>");  //divider

            //
            // description 
            tableBody.AppendFormat("<tr><td style='font-family: IM FELL French Canon; padding-left: 20px;' colspan='2'><em style='font-size: 14px; color: #e6cd90; font-style: normal;'>Strategy:</em> {0}</td></tr>", ut.Desc);

            tableBody.Append("</table>"); //TABLE #1

            #endregion
        }

        public static void ConstructTableWithUnitsStats(StringBuilder tableBody, Realm r, Fbg.Bll.UnitType ut, bool isMobile)
        {
            string RecruitReqString = "";


            #region construct the unit stats table structure

            tableBody.Append("<table cellpadding='0' cellspacing='0'>"); // TABLE #1
            tableBody.Append("<tr><td colspan='2' class='Sectionheader'>");
            tableBody.AppendFormat("<img src='{0}'' />&nbsp;<strong>{1}</strong>", ut.IconUrl.Replace("~/", ""), ut.Name);
            tableBody.Append("</td></tr>");

            tableBody.Append("<tr ><td >");
            tableBody.Append("<table cellpadding='0' cellspacing='0' width='50%' >"); //TABLE #2
            tableBody.Append("<tr><td valign=top class=NoPad>");

            #region Unit Stats
            tableBody.Append("<table  cellpadding='0' cellspacing='1' class='TypicalTable stripeTable' ><tr class='highlight help' rel='s_UnitsCost' ><td class='TableHeaderRow'>Cost:&nbsp;</td><td align='right'>");//
            tableBody.Append(Utils.FormatCost(ut.Cost(null)));
            tableBody.Append("&nbsp;pieces of silver</td></tr>");

            tableBody.Append("<tr class='highlight help' rel='s_UnitsRecruitTime'><td class='TableHeaderRow'>Base Recruit Time:&nbsp;</td><td align='right'>");//
            tableBody.Append(Utils.FormatDuration(ut.RecruitmentTime()));
            tableBody.Append("</td></tr>");

            tableBody.Append("<tr class='highlight help' rel='s_UnitsAttackStrength'><td  class='TableHeaderRow'>Attack Strength:&nbsp;</td><td align='right'>");//
            if (ut.AttackStrength == 0)
            {
                tableBody.Append("n/a");
            }
            else
            {
                tableBody.Append(ut.AttackStrength.ToString());
            }
            tableBody.Append("</td></tr>");

            tableBody.Append("<tr class='highlight help' rel='s_UnitsDefenseStrength'><td class='TableHeaderRow' valign='top'><strong>Defense Strength:</strong>&nbsp;</td><td align='right'>");

            // Table with defensive strength against other units
            HelpHelper.Units.ConstructTableWithDefensiveStrengths(tableBody, r, ut);

            tableBody.Append("</td></tr>");

            tableBody.Append("<tr class='highlight help' rel='s_UnitsFood'><td class='TableHeaderRow'><strong>Food:</strong>&nbsp;</td><td align='right'>");
            tableBody.Append(ut.Pop.ToString());
            tableBody.Append("</td></tr>");

            tableBody.Append("<tr class='highlight help' rel='s_UnitsMovementSpeed'><td class='TableHeaderRow'>Movement Speed:&nbsp;</td><td  align='right'>");
            tableBody.Append(ut.Speed.ToString());
            tableBody.Append("&nbsp;squares per hour</td></tr>");

            tableBody.Append("<tr class='highlight help' rel='s_UnitsCarry'><td class='TableHeaderRow'>Carry:&nbsp;</td><td align='right'>");
            tableBody.Append(ut.CarryCapacity.ToString());
            tableBody.Append("&nbsp;pieces of silver</td></tr>");

            RecruitReqString = HelpHelper.Requirements(ut.Requirements, ut.Requirements_Research, isMobile ? 2 : 1 );
            if (RecruitReqString != string.Empty)
            {
                tableBody.Append("<tr class='highlight'><td class='TableHeaderRow'>Requirements:&nbsp;</td><td align='right'>");
                tableBody.Append(RecruitReqString);
                tableBody.Append("</td></tr>");
            }
            tableBody.Append("</table>");
            #endregion

            if (!isMobile) {
                tableBody.Append("</td>");
                tableBody.AppendFormat("<td style='margin-left:10%'><img border=0 src='{0}' alt='Unit Image' align='Left' style='max-width: 280px;'/>", ut.Image);
                tableBody.Append("</td></tr>");
                tableBody.Append("</table>"); //TABLE #2
                tableBody.Append("</td></tr>");
            }
            else {
                tableBody.Append("</td></tr><tr>");
                tableBody.AppendFormat("<td style='margin-left:10%'><img border=0 src='{0}' alt='Unit Image' align='Left' style='max-width: 280px;'/>", ut.Image);
                tableBody.Append("</td></tr>");
                tableBody.Append("</table>"); //TABLE #2
                tableBody.Append("</td></tr>");
            }

            //
            // description 
            tableBody.AppendFormat("<tr><td style='font-size:10pt;' colspan='2'>{0}</td></tr>", ut.Desc);

            tableBody.Append("</table>"); //TABLE #1

            #endregion
        }
    }




    public static string Requirements(List<BuildingTypeLevel> requirements, List<ResearchItem> unsatisfiedResReqs, int displayType)
    {
        string req = string.Empty;
        foreach (BuildingTypeLevel requiredLevel in requirements)
        {
            if (displayType == 3) {
                req += requiredLevel.Building.Name;
                req += " (lvl" + requiredLevel.Level.ToString() + ")";
                req += "<BR>";
            }
            else {
                req += (displayType == 2 ? "" : "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
                    + requiredLevel.Building.Name;
                if (displayType == 1 && requiredLevel.LevelName != String.Empty) {
                    req += "(Level " + requiredLevel.Level.ToString() + " - " + requiredLevel.LevelName + ")";
                }
                else {
                    req += "(Level " + requiredLevel.Level.ToString() + ")";
                }

                req += "<BR>";
            }

        }

        if (unsatisfiedResReqs != null && unsatisfiedResReqs.Count > 0)
        {
            req += "<BR>Research:";
            foreach (ResearchItem ir in unsatisfiedResReqs)
            {
                req += "<div>" + ir.Name + "</div>";
            }
            req += "<BR>";
        }
        return req;
    }

    public static string BuildingRequirements(BuildingTypeLevel level, int displayType)
    {
        return HelpHelper.Requirements(level.Requirements, null, displayType);
    }
}
