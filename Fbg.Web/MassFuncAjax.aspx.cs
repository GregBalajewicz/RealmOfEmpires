using System;
using System.Collections;
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
using System.Collections.Generic;

using Fbg.Bll;

public partial class MassFunctAjax : JsonPage
{

    protected class rule
    {
        public int btid;
        public int level;
        public bool upReq;
    }
    /// <summary>
    /// true if player has no PF and village count higher than 2 
    /// </summary>
    bool hasPF;
    public override object Result()
    {
        try
        {
            //
            // if player has no PF and village count > 2 then we lock out functionality
            //
            hasPF = FbgPlayer.PF_HasPF(Fbg.Bll.CONSTS.PFs.MassRecruitAndUpgrade);

            System.Web.Script.Serialization.JavaScriptSerializer json_serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            string dataAsJSON = this.Request.Form["data"];

            List<rule> rules = json_serializer.Deserialize<List<rule>>(dataAsJSON);
           
            return MassUpgrade(rules);

        }
        catch (Exception exc)
        {
            throw exc;
        }
    }





    protected string MassUpgrade(List<rule> rules)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder(MyVillages.Count * 200);
        if (!hasPF) { return ""; }
        if (rules.Count > 0)
        {
            #region Try to do the upgrade
            Village vil;
            foreach (VillageBase vb in MyVillages)
            {
                vil = FbgPlayer.Village(vb.id);
                sb.AppendFormat("<span style='font-weight:bold;font-size:11pt;'>");
                sb.AppendFormat("<a x='{1}' y='{2}' vid='{3}' opid='{4}' href='{5}' class='jsV'>{0}({1},{2})</a></span>"
                    , vil.name, vil.xcord, vil.ycord, vil.id, vil.owner.ID, NavigationHelper.VillageOtherOverview_NoTilda(vil.id));
                sb.AppendFormat("<table CellPadding='2' CellSpacing='1' class='stripeTable TypicalTable'>");

                foreach (rule r in rules)
                {
                    BuildingType bt = FbgPlayer.Realm.BuildingType(r.btid);

                  
                    int desiredLevel = r.level;

                  
                    sb.AppendFormat("<TR><TD class='Sectionheader'>: {3} to level {4}</TD><TD nowrap class=DataRowAlternate>", vil.name, vil.xcord, vil.ycord, bt.Name, desiredLevel);

                    UpgradeInOneVillage(vil, bt, desiredLevel, sb, r.upReq);

                    sb.AppendFormat("</TD></TR>");
                }

                sb.AppendFormat("</table>");



            }
            #endregion
        }

        return sb.ToString();

    }

    private void UpgradeInOneVillage(Village vil, BuildingType bt, int desiredLevel, System.Text.StringBuilder sb, bool tryUpgradeReq)
    {

        BuildingTypeLevel nextBuildingTypeLevel;
        BuildingTypeLevel curBuildingTypeLevel;
        int curBuildingTypeLevelInt;
        BuildingTypeLevel maxNextBuildingTypeLevel;
        List<BuildingTypeLevel> unsatisfiedReqs;
        bool notEnoughSilver;
        bool notEnoughFood;
        Village.CanUpgradeResult canUpgrade;
        int curLevelToUpgradeTo;
        int upgradingBuildingLevel;
        int currentBuildingLevel;


        currentBuildingLevel = vil.GetBuildingLevel(bt);
        upgradingBuildingLevel = vil.GetUpgradingBuildingLevel(bt.ID);


        canUpgrade = vil.CanUpgrade(bt
            , out nextBuildingTypeLevel
            , out curBuildingTypeLevel
            , out maxNextBuildingTypeLevel
            , out unsatisfiedReqs
            , out notEnoughSilver
            , out notEnoughFood
            , false);


        if (canUpgrade == Village.CanUpgradeResult.Yes)
        {
            #region Can upgrade so do it
            curBuildingTypeLevelInt = curBuildingTypeLevel == null ? 0 : curBuildingTypeLevel.Level;
            curLevelToUpgradeTo = nextBuildingTypeLevel.Level;
            while (curLevelToUpgradeTo <= desiredLevel &&
               curLevelToUpgradeTo <= maxNextBuildingTypeLevel.Level)
            {
                FbgPlayer.DoUpgrade(vil.id, bt.ID, curLevelToUpgradeTo);
                curLevelToUpgradeTo++;
            }
            curLevelToUpgradeTo--; // since this is the last level successfully uppgraded to.

            if (curLevelToUpgradeTo == desiredLevel)
            {
                sb.AppendFormat("<span class=ConfirmMsg>" + RS("OK") + "</span>: " + RS("Level") + " {0} -> {1}", curBuildingTypeLevelInt, curLevelToUpgradeTo);
            }
            else if (curLevelToUpgradeTo < desiredLevel)
            {
                sb.AppendFormat("<span class=ConfirmMsg>" + RS("OK") + "</span> " + RS("PARTIALLevel") + " {0} -> {1}", curBuildingTypeLevelInt, curLevelToUpgradeTo);
            }
            #endregion
        }
        else if (canUpgrade == Village.CanUpgradeResult.No_UnsatisfiedReq && tryUpgradeReq)
        {
            bool someReqUpgrade = false;
            foreach (BuildingTypeLevel btl in unsatisfiedReqs)
            {
                someReqUpgrade = UpgradeRequiements(bt, desiredLevel, vil, btl, sb, tryUpgradeReq, 0) || someReqUpgrade;
            }
            if (someReqUpgrade)
            {
                sb.Append("<span class=ConfirmMsg>" + RS("OK") + "</span> " + RS("PARTIALSomeRequirements"));
            }
            else
            {
                sb.AppendFormat("<span class=Error2>" + RS("Failed") + "</span>: " + RS("RequirementsNotSatisfied"));
            }
        }
        else if (canUpgrade == Village.CanUpgradeResult.No_LackFood && tryUpgradeReq)
        {
            if (UpgradeFarm(vil))
            {
                sb.Append("<span class=ConfirmMsg>" + RS("OK") + "</span> " + RS("PARTIALFarmLandUpgraded"));
            }
            else
            {
                sb.Append("<span class=Error2>" + RS("Failed") + "</span> Food is lacking but not able to upgrade Farm") ;
            }
        }
        else if (canUpgrade == Village.CanUpgradeResult.No_BuildingFullyUpgraded)
        {
            sb.Append("<span class=ConfirmMsg>" + RS("OK") + "</span> " + RS("BuildingFullyUpgraded"));
        }
        else
        {
            sb.AppendFormat("<span class=Error2>" + RS("Failed") + "</span>: {0}", GetFailedUpgadeMessage(canUpgrade));
        }
    }
    /// <summary>
    /// returns true if the farm was upgraded
    /// </summary>
    /// <param name="vil"></param>
    /// <param name="btl"></param>
    /// <param name="sb"></param>
    /// <param name="tryUpgradeReq"></param>
    /// <returns></returns>
    private bool UpgradeFarm(Village vil)
    {
        BuildingType btFarm = FbgPlayer.Realm.BuildingType(Fbg.Bll.CONSTS.BuildingIDs.Farmland);



        BuildingTypeLevel nextBuildingTypeLevel;
        BuildingTypeLevel curBuildingTypeLevel;
        BuildingTypeLevel maxNextBuildingTypeLevel;
        List<BuildingTypeLevel> unsatisfiedReqs;
        bool notEnoughSilver;
        bool notEnoughFood;
        Village.CanUpgradeResult canUpgrade;
        int upgradingBuildingLevel;
        int currentBuildingLevel;

        currentBuildingLevel = vil.GetBuildingLevel(btFarm);
        upgradingBuildingLevel = vil.GetUpgradingBuildingLevel(btFarm.ID);

        if (currentBuildingLevel >= btFarm.MaxLevel || upgradingBuildingLevel >= btFarm.MaxLevel)
        {
            return false;
        }

        canUpgrade = vil.CanUpgrade(btFarm
            , out nextBuildingTypeLevel
            , out curBuildingTypeLevel
            , out maxNextBuildingTypeLevel
            , out unsatisfiedReqs
            , out notEnoughSilver
            , out notEnoughFood
            , false);

        if (canUpgrade == Village.CanUpgradeResult.Yes)
        {
            FbgPlayer.DoUpgrade(vil.id, btFarm.ID, nextBuildingTypeLevel.Level);
            return true;
        }
        return false;
    }
    /// <summary>
    /// returns true if the requirements successfully upgraded
    /// </summary>
    /// <param name="vil"></param>
    /// <param name="btl"></param>
    /// <param name="sb"></param>
    /// <param name="tryUpgradeReq"></param>
    /// <returns></returns>
    private bool UpgradeRequiements(BuildingType reqForBuilding, int reqForBuildingLevel, Village vil, BuildingTypeLevel btlRequirement, System.Text.StringBuilder sb, bool tryUpgradeReq, int iteration)
    {
        BuildingTypeLevel nextBuildingTypeLevel;
        BuildingTypeLevel curBuildingTypeLevel;
        int curBuildingTypeLevelInt;
        BuildingTypeLevel maxNextBuildingTypeLevel;
        List<BuildingTypeLevel> unsatisfiedReqs;
        bool notEnoughSilver;
        bool notEnoughFood;
        Village.CanUpgradeResult canUpgrade;
        int curLevelToUpgradeTo;
        int upgradingBuildingLevel;
        int currentBuildingLevel;

        currentBuildingLevel = vil.GetBuildingLevel(btlRequirement.Building);
        upgradingBuildingLevel = vil.GetUpgradingBuildingLevel(btlRequirement.Building.ID);

        if (currentBuildingLevel >= btlRequirement.Level || upgradingBuildingLevel >= btlRequirement.Level)
        {
            return false;
        }

        canUpgrade = vil.CanUpgrade(btlRequirement.Building
            , out nextBuildingTypeLevel
            , out curBuildingTypeLevel
            , out maxNextBuildingTypeLevel
            , out unsatisfiedReqs
            , out notEnoughSilver
            , out notEnoughFood
            , false);


        bool someReqUpgraded = false;

        if (canUpgrade == Village.CanUpgradeResult.Yes)
        {
            curBuildingTypeLevelInt = curBuildingTypeLevel == null ? 0 : curBuildingTypeLevel.Level;
            curLevelToUpgradeTo = nextBuildingTypeLevel.Level;
            while (curLevelToUpgradeTo <= btlRequirement.Level &&
               curLevelToUpgradeTo <= maxNextBuildingTypeLevel.Level)
            {
                FbgPlayer.DoUpgrade(vil.id, btlRequirement.Building.ID, curLevelToUpgradeTo);
                curLevelToUpgradeTo++;
            }
            curLevelToUpgradeTo--; // since this is the last level successfully uppgraded to.

            sb.AppendFormat(RS("UpgradingRequirementNeed") + "</span><BR>", btlRequirement.Building.Name, btlRequirement.Level, reqForBuilding.Name, reqForBuildingLevel, curLevelToUpgradeTo);
            someReqUpgraded = true;
        }
        else if (canUpgrade == Village.CanUpgradeResult.No_UnsatisfiedReq && tryUpgradeReq)
        {
            bool someReqUpgrade = false;
            foreach (BuildingTypeLevel btl in unsatisfiedReqs)
            {
                someReqUpgrade = someReqUpgrade || UpgradeRequiements(btlRequirement.Building, btlRequirement.Level, vil, btl, sb, tryUpgradeReq, iteration + 1);
            }
        }
        return someReqUpgraded;
    }

    private string GetFailedUpgadeMessage(Village.CanUpgradeResult canUpgrade)
    {
        switch (canUpgrade)
        {
            case Village.CanUpgradeResult.No_BuildingFullyUpgraded:
                return RS("BuildingFullyUpgraded");
            case Village.CanUpgradeResult.No_LackFood:
                return RS("NoFood");
            case Village.CanUpgradeResult.No_LackSilver:
                return RS("NoSilver");
            case Village.CanUpgradeResult.No_LockedFeature:
                return RS("MaxBuildingQueueReached");
            case Village.CanUpgradeResult.No_UnsatisfiedReq:
                return RS("RequirementsNotSatisfied");
            case Village.CanUpgradeResult.No_Busy:
                return RS("ContructionBusy");
            default:
                return "";
        }
    }
}
